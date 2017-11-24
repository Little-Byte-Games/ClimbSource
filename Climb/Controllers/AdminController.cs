using Climb.Data;
using Climb.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Climb.Controllers
{
    public class AdminController : Controller
    {
        private readonly ClimbContext context;
        private readonly IHostingEnvironment environment;

        public AdminController(ClimbContext context, IHostingEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }

        #region Pages
        public IActionResult Index()
        {
            if (!environment.IsDevelopment())
            {
                return NotFound("You're not a site admin!");
            }
            return View();
        }
        #endregion

        #region API
        [HttpPost]
        public IActionResult ResetDB()
        {
            DbInitializer.Initialize(context, true);
            return Ok("Db reset");
        }
        #endregion
    }
}