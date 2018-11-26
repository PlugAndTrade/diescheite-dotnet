using Microsoft.AspNetCore.Mvc;

namespace PlugAndTrade.DieScheite.Client.Example.AspNetCore.Controllers
{
    [Route("healthcheck")]
    public class HealthcheckController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return new ContentResult()
            {
                Content = "OK",
                ContentType = "text/plain"
            };
        }
    }
}
