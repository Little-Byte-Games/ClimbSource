using System;
using Climb.Models;
using Climb.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Climb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("defaultConnection");
            if(string.IsNullOrWhiteSpace(connectionString))
            {
                services.AddDbContext<ClimbContext>(options => options.UseInMemoryDatabase("Test"));
            }
            else
            {
                services.AddDbContext<ClimbContext>(options => options.UseSqlServer(connectionString));
            }

            var cdnType = Configuration["CDN"];
            switch(cdnType)
            {
                case "S3":
                    services.AddSingleton<CdnService, S3Cdn>();
                    break;
                case "Local":
                    services.AddSingleton<CdnService, FileStorageCdn>();
                    break;
                default:
                    throw new NotSupportedException("Need to set a CDN type.");
            }

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ClimbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = ApplicationUser.RequiredChars;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 0;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            });

            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ILeagueUserService, LeagueUserService>();
            services.AddTransient<ILeagueService, LeagueService>();
            services.AddTransient<ISeasonService, SeasonService>();
            services.AddTransient<ISetService, SetService>();
            services.AddTransient<IAccountService, AccountService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddCors();
            services.AddMvc();

            services.Configure<MvcOptions>(options => { options.Filters.Add(new RequireHttpsAttribute()); });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if(env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.Use(async (context, next) =>
            {
                if(context.Request.IsHttps)
                {
                    await next();
                }
                else
                {
                    var withHttps = "https://" + context.Request.Host + context.Request.Path;
                    context.Response.Redirect(withHttps);
                }
            });

            app.UseCors(options => options.AllowAnyOrigin());
            app.UseStatusCodePagesWithReExecute("/Error/{0}");
            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}