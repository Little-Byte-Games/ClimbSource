using Microsoft.AspNetCore.Mvc.Rendering;

namespace Climb.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static bool IsDebug(this IHtmlHelper htmlHelper)
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }
}