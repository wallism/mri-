using Common.Utilities.Models;
using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace internal_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly DaprClient _dapr;

        public HealthController(DaprClient dapr)
        {
            _dapr = dapr;
        }


        [HttpGet("ping")]
        public string Get()
        {
            return "pong";
        }

        [HttpGet("check")]
        public IActionResult GetCheck()
        {
            return Ok("all good");
        }


        [HttpPost("", Name = "CountChanged")]
        [Topic("sample-pubsub", "CountChanged")]
        public IActionResult CountChanged(Message message)
        {
            var text = $"{message.Text} -> Internal";

            foreach (var h in Request.Headers)
            {
                if (h.Key == "traceparent")
                    Log.Information("H: {key} {value}", h.Key, h.Value);
            }
            Log.Warning(text);
            return Ok();
        }
    }
}
