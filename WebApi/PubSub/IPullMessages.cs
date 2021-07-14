using System.Collections.Concurrent;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Services
{
    public interface IPullMessages
    {
        Task<ConcurrentBag<Notification>> PullMessagesAsync(string projectId, string subscriptionId, bool acknowledge);
    }
}