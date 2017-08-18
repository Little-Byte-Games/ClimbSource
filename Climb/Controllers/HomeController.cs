using System;
using Climb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Climb.Controllers
{
    public class HomeController : Controller
    {
        public readonly IConfiguration configuration;
        private readonly ClimbContext context;

        public HomeController(IConfiguration configuration, ClimbContext context)
        {
            this.configuration = configuration;
            this.context = context;
        }

        public IActionResult Index()
        {
            ViewData["UserID"] = new SelectList(context.LeagueUser, "ID", "ID");
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> UploadProfilePic(int id, IFormFile file)
        {
            var accessKey = configuration.GetSection("AWS")["AccessKey"];
            var secretKey = configuration.GetSection("AWS")["SecretKey"];
            var bucketName = configuration.GetSection("AWS")["Bucket"];

            var fileKey = $"{Guid.NewGuid()}_{DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm", CultureInfo.InvariantCulture)}{Path.GetExtension(file.FileName)}";
            fileKey = fileKey.ToLowerInvariant();

            var transfer = new TransferUtility(accessKey, secretKey, RegionEndpoint.USEast1);
            await transfer.UploadAsync(file.OpenReadStream(), bucketName, fileKey);

            var user = await context.LeagueUser.SingleOrDefaultAsync(u => u.UserID == id);
            if (user != null)
            {
                user.ProfilePicKey = fileKey;
                context.Update(user);
                await context.SaveChangesAsync();
            }

            return View("Index");
        }
    }
}
