using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Hubs
{
    public class NotifyHub : Hub<INotifyClient>
    {
        public static int _userCount = 0;

        public override Task OnConnectedAsync()
        {
            _userCount++;
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _userCount--;
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(List<Notification> messages)
        {
            await Clients.All.ReceiveMessage(messages);
        }
    }
}
