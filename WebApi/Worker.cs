using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Hubs;
using WebApi.Models;
using WebApi.Services;

namespace WebApi
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHubContext<NotifyHub, INotifyClient> _notifyHub;
        private readonly IPullMessages _pullMessages;
        private List<Notification> _notificationsCache;


        public Worker(ILogger<Worker> logger, IHubContext<NotifyHub, INotifyClient> notifyHub, IPullMessages pullMessages)
        {
            _logger = logger;
            _notifyHub = notifyHub;
            _pullMessages = pullMessages;
            _notificationsCache = new List<Notification>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker started running at: {time}", DateTimeOffset.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                var messages = await _pullMessages.PullMessagesAsync("green-hall-318914", "test-messaging-sub2", true);
                _notificationsCache.AddRange(messages);

                if (_notificationsCache.Any() && NotifyHub._userCount > 0)
                {
                    _logger.LogInformation("Worker broadcast to clients", DateTimeOffset.Now);
                    await _notifyHub.Clients.All.ReceiveMessage(_notificationsCache);
                    _notificationsCache.Clear();
                }

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
