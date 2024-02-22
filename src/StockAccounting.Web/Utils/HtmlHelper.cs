using Microsoft.AspNetCore.Mvc.Rendering;

namespace StockAccounting.Web.Utils
{
    public static class HtmlHelper
    {
        public static string IsSelected(this IHtmlHelper htmlHelper, string controllers, string actions, string cssClass = "active")
        {
            var currentAction = htmlHelper.ViewContext.RouteData.Values["action"] as string;
            var currentController = htmlHelper.ViewContext.RouteData.Values["controller"] as string;

            IEnumerable<string> acceptedActions = (actions ?? currentAction)!.Split(',');
            IEnumerable<string> acceptedControllers = (controllers ?? currentController)!.Split(',');

            return acceptedActions.Contains(currentAction) && acceptedControllers.Contains(currentController) ?
                cssClass : string.Empty;
        }
    }
}
