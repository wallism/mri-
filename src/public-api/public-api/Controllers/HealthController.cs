using Common.Utilities.Models;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace public_api.Controllers
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


        [HttpPost("count")]
        public async Task<IActionResult> PostCount(Message message)
        {
            var text = $"{message.Text} -> Public";
            // ForContext is a more succinct way to add log properties https://benfoster.io/blog/serilog-best-practices/
            Log.ForContext("Service", "public-api")
                .Warning(text);
            
            // Publish
            await _dapr.PublishEventAsync("sample-pubsub", "CountChanged", new Message { Text = text });
            
            return Ok();
        }

    }
    
}
