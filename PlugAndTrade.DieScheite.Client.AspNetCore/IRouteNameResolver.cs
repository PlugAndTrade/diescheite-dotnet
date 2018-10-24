using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;

namespace PlugAndTrade.DieScheite.Client.AspNetCore
{
    public interface IRouteNameResolver
    {
        Task<string> ResolveMatchingTemplateRouteAsync(RouteData routeData);
    }
}
