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
        public async Task SendMessage(List<Notification> messages)
        {
            await Clients.All.ReceiveMessage(messages);
        }
    }
}
