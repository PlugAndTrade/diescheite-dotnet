using Microsoft.AspNetCore.Mvc;
using PlugAndTrade.DieScheite.Client.Common;
namespace PlugAndTrade.DieScheite.Client.Example.AspNetCore.Controllers
{
    public class RoutedController
    {
        private readonly LogEntry _log;

        public RoutedController(LogEntry log)
        {
            _log = log;
        }

        public IActionResult GetSomeRandomMethod(string id)
        {
            return new NoContentResult();
        }
    }
}
