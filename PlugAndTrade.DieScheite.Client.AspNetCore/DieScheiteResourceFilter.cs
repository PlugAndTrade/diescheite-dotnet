using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PlugAndTrade.DieScheite.Client.AspNetCore
{
    public static class DieScheiteHttpContextExtensions
    {
        public const string ROUTE_TEMPLATE_KEY = "DieScheiteRouteTemplate";
        public const string CONTROLLER_NAME_KEY = "DieScheiteControllerName";
        public const string ACTION_NAME_KEY = "DieScheiteActionName";

        public static string GetRouteTemplate(this HttpContext context) => context.Items.ContainsKey(ROUTE_TEMPLATE_KEY)
            ? context.Items[ROUTE_TEMPLATE_KEY] as string
            : "";

        public static string GetControllerName(this HttpContext context) => context.Items.ContainsKey(CONTROLLER_NAME_KEY)
            ? context.Items[CONTROLLER_NAME_KEY] as string
            : "";

        public static string GetActionName(this HttpContext context) => context.Items.ContainsKey(ACTION_NAME_KEY)
            ? context.Items[ACTION_NAME_KEY] as string
            : "";
    }

    public class DieScheiteResourceFilter : IAsyncResourceFilter
    {
        private readonly IRouteNameResolver _routeNameResolver;

        public DieScheiteResourceFilter(IRouteNameResolver routeNameResolver)
        {
            _routeNameResolver = routeNameResolver;
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var templateRoute = await _routeNameResolver?.ResolveMatchingTemplateRouteAsync(context.RouteData);

            if (context.RouteData.Values.TryGetValue("controller", out var controller))
            {
                context.HttpContext.Items.Add(DieScheiteHttpContextExtensions.CONTROLLER_NAME_KEY, controller);
            }

            if (context.RouteData.Values.TryGetValue("action", out var action))
            {
                context.HttpContext.Items.Add(DieScheiteHttpContextExtensions.ACTION_NAME_KEY, action);
            }

            context.HttpContext.Items.Add(DieScheiteHttpContextExtensions.ROUTE_TEMPLATE_KEY, templateRoute ?? "");

            await next.Invoke();
        }

    }
}
