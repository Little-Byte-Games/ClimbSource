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
            services.AddDbContext<ClimbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("ClimbContext")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ClimbContext>()
                .AddDefaultTokenProviders();

            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<ICDNService, CDNService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ILeagueUserService, LeagueUserService>();
            services.AddTransient<ILeagueService, LeagueService>();
            services.AddTransient<ISeasonService, SeasonService>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddMvc();

            Console.WriteLine("[YEAGER] Configuring Services");
            services.AddAuthentication().AddGoogle(googleOptions =>
            {
                //googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
                googleOptions.ClientId = "479023458299-gskdaocjluc4tvopltqkkdq4qc3g1dic.apps.googleusercontent.com";
                Console.WriteLine($"[YEAGER] ClientId={googleOptions.ClientId}");
                if(string.IsNullOrWhiteSpace(googleOptions.ClientId))
                {
                    throw new ArgumentNullException(nameof(googleOptions.ClientId), "Google Client ID is missing!");
                }

                //googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                googleOptions.ClientSecret = "DlsgWNDkVKCxqZLDnLXJpNrs";
                Console.WriteLine($"[YEAGER] ClientSecret={googleOptions.ClientSecret}");
                if (string.IsNullOrWhiteSpace(googleOptions.ClientSecret))
                {
                    throw new ArgumentNullException(nameof(googleOptions.ClientSecret), "Google Client secret is missing!");
                }
            });

            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new RequireHttpsAttribute());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
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
                if (context.Request.IsHttps)
                {
                    await next();
                }
                else
                {
                    var withHttps = "https://" + context.Request.Host + context.Request.Path;
                    context.Response.Redirect(withHttps);
                }
            });

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