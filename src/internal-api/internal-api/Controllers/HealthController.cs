using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> CountChanged(Message message)
        {
            var text = $"{message.Text} -> Internal";
            Console.WriteLine(message.Text);
            return Ok();
        }
    }

    public class Message
    {
        public string Text { get; set; }
    }

}
