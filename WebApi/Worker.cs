using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Hubs;
using WebApi.Services;

namespace WebApi
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHubContext<NotifyHub, INotifyClient> _notifyHub;

        public Worker(ILogger<Worker> logger, IHubContext<NotifyHub, INotifyClient> notifyHub)
        {
            _logger = logger;
            _notifyHub = notifyHub;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker started running at: {time}", DateTimeOffset.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                var pullMessages = new PullMessages(_logger);

                var messages = await pullMessages.PullMessagesAsync("green-hall-318914", "test-messaging-sub2", true);

                if (messages.Any())
                {
                    _logger.LogInformation("Worker broadcast to clients", DateTimeOffset.Now);
                    await _notifyHub.Clients.All.ReceiveMessage(messages.ToList());
                }

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
