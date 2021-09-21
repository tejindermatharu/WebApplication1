using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Hubs;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class NotifyController : ControllerBase
    {
        private readonly ILogger<NotifyController> _logger;
        private readonly IHubContext<NotifyHub, INotifyClient> _notifyHub;

        public NotifyController(ILogger<NotifyController> logger, IHubContext<NotifyHub, INotifyClient> notifyHub)
        {
            _logger = logger;
            _notifyHub = notifyHub;
        }


        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<ActionResult> Get()
        {
            _logger.LogInformation("Notifiy web: Notify me request received");

            //var pullMessages = new PullMessages(_logger);
            var pullMessages = new PullMessagesSync(_logger);

            var messages = pullMessages.PullMessages("pivotal-leaf-326613", "test-messaging-sub2", true);

            return Ok(messages.ToList());
        }

        [HttpGet]
        [Route("/Notify/Send", Name = "Custom")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<ActionResult> SendNotification()
        {
            _logger.LogInformation("Notifiy web: Notify me request received");

            //var pullMessages = new PullMessages(_logger);
            var pullMessages = new PullMessagesSync(_logger);

            var messages = pullMessages.PullMessages("pivotal-leaf-326613", "test-messaging-sub2", true);

            if (NotifyHub._userCount == 0)
            {
                _logger.LogWarning("NO CONNECTED CLIENTS ON THIS HUB");
            }

            await _notifyHub.Clients.All.ReceiveMessage(messages);
            return Ok();
        }

    }
}
