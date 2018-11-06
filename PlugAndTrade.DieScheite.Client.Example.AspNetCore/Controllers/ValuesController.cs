using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using PlugAndTrade.DieScheite.Client.Common;

namespace PlugAndTrade.DieScheite.Client.Example.AspNetCore.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly LogEntry _log;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ValuesController(LogEntry log, IHttpContextAccessor httpContextAccessor)
        {
            _log = log;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            _log.Info("Getting all dem values");
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost("server_error")]
        public IActionResult ServerError()
        {
            return new ContentResult()
            {
                Content = "foobar",
                StatusCode = 500
            };
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody] string value)
        {
            _log.Info("Set dat value");
            return new ContentResult()
            {
                Content = "foobar",
                StatusCode = 201
            };
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
