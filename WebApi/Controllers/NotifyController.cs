using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class NotifyController : ControllerBase
    {
        private readonly ILogger<NotifyController> _logger;

        public NotifyController(ILogger<NotifyController> logger)
        {
            _logger = logger;
        }


        [HttpGet]
        public async Task<ActionResult> Get()
        {
            _logger.LogInformation("Notifiy web: Notify me request received");

            var pullMessages = new PullMessages(_logger);

            var messages = await pullMessages.PullMessagesAsync("neat-fin-309913", "test-messaging-sub2", true);

            return Ok(messages.ToList());
        }
    }
}
