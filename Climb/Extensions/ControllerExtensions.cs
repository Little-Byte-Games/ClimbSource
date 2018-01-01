using Climb.Models;
using Microsoft.AspNetCore.Mvc;

namespace Climb.Extensions
{
    public static class ControllerExtensions
    {
        public static IActionResult NotFoundPage(this Controller controller, User user, string message = null)
        {
            return controller.ErrorPage(user, 404, message);
        }

        public static IActionResult ErrorPage(this Controller controller, User user, int statusCode, string message = null)
        {
            return controller.RedirectToAction("Index", "Error", new {statusCode, message});
        }
    }
}
