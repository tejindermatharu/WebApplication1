using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PubSubSubscriptionService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PubSubSubscriptionService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var pullMessages = new PullMessages();

                var message = await pullMessages.PullMessagesAsync("neat-fin-309913", "test-messaging-sub", true);

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
