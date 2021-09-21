using System;
using Google.Cloud.PubSub.V1;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using WebApi.Models;
using Newtonsoft.Json;
using Grpc.Core;

namespace WebApi.Services
{
    public class PullMessages : IPullMessages
    {
        private readonly ILogger<PullMessages> _logger;

        public PullMessages(ILogger<PullMessages> logger)
        {
            _logger = logger;
        }

        public async Task<ConcurrentBag<Notification>> PullMessagesAsync(string projectId, string subscriptionId, bool acknowledge)
        {
            SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(projectId, subscriptionId);
            SubscriberClient subscriber = await SubscriberClient.CreateAsync(subscriptionName);
            // SubscriberClient runs your message handle function on multiple
            // threads to maximize throughput.
            var notificationsBag = new ConcurrentBag<Notification>();

            try
            {
                Task startTask = subscriber.StartAsync((PubsubMessage message, System.Threading.CancellationToken cancel) =>
            {
                try
                {
                    string text = System.Text.Encoding.UTF8.GetString(message.Data.ToArray());
                    Notification notification = JsonConvert.DeserializeObject<Notification>(text);
                    _logger.LogInformation($"Notify App Message received: {message.MessageId}: {text}");
                    //Interlocked.Increment(ref messageCount);
                    notificationsBag.Add(notification);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Problem parsing notification message: {e.Message}");
                }

                return Task.FromResult(acknowledge ? SubscriberClient.Reply.Ack : SubscriberClient.Reply.Nack);

            });

            // Run for 5 seconds./.
            await Task.Delay(5000);
            await subscriber.StopAsync(CancellationToken.None);
            // Lets make sure that the start task finished successfully after the call to stop.
            await startTask;

            }
            catch (Grpc.Core.RpcException ex) when (ex.Status.StatusCode == StatusCode.Unavailable)
            {
                _logger.LogError(ex, $"UNAVAILABLE due to too many concurrent pull requests pending for subscription id: {subscriptionId}: {ex.Message}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Problem parsing notification message: {e.Message} for subscription id: {subscriptionId}");
            }
            return notificationsBag;
        }
    }
}
