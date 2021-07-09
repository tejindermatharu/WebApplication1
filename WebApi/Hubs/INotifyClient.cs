using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Hubs
{
    public interface INotifyClient
    {
        Task ReceiveMessage(List<Notification> message);
    }
}
