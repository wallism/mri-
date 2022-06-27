﻿using Microsoft.AspNetCore.Mvc;

namespace internal_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {

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

        
    }
}
