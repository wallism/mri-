using Dapr.Client;
using Microsoft.AspNetCore.Mvc;

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
            try
            {
                var text = $"{message.Text} -> Public";
                Console.WriteLine(text);

                // Publish
                await _dapr.PublishEventAsync("sample-pubsub", "CountChanged", new Message { Text = text });


                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Problem(ex.Message);
            }
        }

    }

    public class Message
    {
        public string Text { get; set; }
    }
}
