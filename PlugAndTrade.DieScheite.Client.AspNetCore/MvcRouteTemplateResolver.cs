using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Internal;

namespace PlugAndTrade.DieScheite.Client.AspNetCore
{
    public class MvcRouteTemplateResolver : IRouteNameResolver
    {
        private const string ApiVersionToken = "{version:apiversion}";
        private const string MsVersionpolicyIsAppliedToken = "MS_VersionPolicyIsApplied";
        private const string VersionRouteDataToken = "version";

        public Task<string> ResolveMatchingTemplateRouteAsync(RouteData routeData)
        {
            return Task.FromResult(GetRouteTemplate(routeData) ?? GetMvcAttributeRouteHandlerTemplate(routeData) ?? string.Empty);
        }

        private string GetMvcAttributeRouteHandlerTemplate(RouteData routeData)
        {
            var attributeRouteHandler = routeData.Routers.FirstOrDefault(r => r.GetType().Name == nameof(MvcAttributeRouteHandler))
                as MvcAttributeRouteHandler;

            if (attributeRouteHandler == null || !attributeRouteHandler.Actions.Any())
            {
                return null;
            }

            if (attributeRouteHandler.Actions.Length == 1)
            {
                var singleDescriptor = attributeRouteHandler.Actions.Single();

                return ExtractRouteTemplate(routeData, singleDescriptor);
            }

            foreach (var actionDescriptor in attributeRouteHandler.Actions)
            {
                if (actionDescriptor.Properties != null && actionDescriptor.Properties.ContainsKey(MsVersionpolicyIsAppliedToken))
                {
                    return ExtractRouteTemplate(routeData, actionDescriptor);
                }
            }

            var firstDescriptor = attributeRouteHandler.Actions.First();

            return ExtractRouteTemplate(routeData, firstDescriptor);
        }

        private static string ExtractRouteTemplate(RouteData routeData, ActionDescriptor actionDescriptor)
        {
            var routeTemplate = actionDescriptor.AttributeRouteInfo?.Template.ToLower() ?? string.Empty;

            if (actionDescriptor.Properties != null && actionDescriptor.Properties.ContainsKey(MsVersionpolicyIsAppliedToken)
                && routeData.Values.ContainsKey(VersionRouteDataToken))
            {
                routeTemplate = routeTemplate.Replace(ApiVersionToken, routeData.Values[VersionRouteDataToken].ToString());
            }

            return routeTemplate;
        }

        /**
         * From: https://github.com/AppMetrics/AspNetCore/blob/87dcdc89ae25fdec868b9abf25b8ea46b762eaa4/src/App.Metrics.AspNetCore.Routing/DefaultMetricsRouteNameResolver.cs
         */
        private string GetRouteTemplate(RouteData routeData)
        {
            var templateRoute = routeData.Routers.FirstOrDefault(r => r.GetType().Name == "Route") as Route;

            if (templateRoute == null)
            {
                return null;
            }

            var controller = routeData.Values.FirstOrDefault(v => v.Key == "controller");
            var action = routeData.Values.FirstOrDefault(v => v.Key == "action");

            var result = templateRoute.ToTemplateString(controller.Value as string, action.Value as string);

            return result;
        }
    }

    /**
     * From: https://github.com/AppMetrics/AspNetCore/blob/87dcdc89ae25fdec868b9abf25b8ea46b762eaa4/src/App.Metrics.AspNetCore.Routing/Extensions/TemplatePartExtensions.cs
     */
    public static class TemplatePartExtensions
    {
        public static string ToTemplatePartString(this TemplatePart templatePart)
        {
            if (templatePart.IsParameter)
            {
                return string.Join("", new []
                {
                    "{",
                    templatePart.IsCatchAll ? "*" : string.Empty,
                    templatePart.Name,
                    templatePart.IsOptional ? "?" : string.Empty,
                    "}"
                }).ToLower();
            }

            return templatePart.Text.ToLower();
        }

        public static string ToTemplateSegmentString(this TemplateSegment templateSegment) =>
            string.Join(string.Empty, templateSegment.Parts.Select(ToTemplatePartString));

        public static string ToTemplateString(this Route templateRoute, string controller, string action) =>
            string.Join("/", templateRoute.ParsedTemplate.Segments.Select(ToTemplateSegmentString))
                  .Replace("{controller}", controller)
                  .Replace("{action}", action).ToLower();
    }
}
