using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Hubs
{
    public class NotifyHub : Hub<INotifyClient>
    {
        private readonly ILogger<NotifyHub> _logger;
        public static int _userCount = 0;

        public NotifyHub(ILogger<NotifyHub> logger)
        {
            _logger = logger;
        }

        public override Task OnConnectedAsync()
        {
            _userCount++;
            _logger.LogInformation($"Client connected: {Context.ConnectionId}. Total number connected={_userCount}" );
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _userCount = 0;

            _logger.LogInformation($"Client disconnected: {Context.ConnectionId}. Total number connected={_userCount}");
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(List<Notification> messages)
        {
            await Clients.All.ReceiveMessage(messages);
        }
    }
}
