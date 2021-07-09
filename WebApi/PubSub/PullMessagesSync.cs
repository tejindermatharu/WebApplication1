using Google.Cloud.PubSub.V1;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Services
{
    public class PullMessagesSync
    {
        private readonly ILogger _logger;

        public PullMessagesSync(ILogger logger)
        {
            _logger = logger;
        }

        public List<Notification> PullMessages(string projectId, string subscriptionId, bool acknowledge)
        {
            SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(projectId, subscriptionId);
            SubscriberServiceApiClient subscriberClient = SubscriberServiceApiClient.Create();
            int messageCount = 0;
            List<Notification> notificationsList = new List<Notification>();

            try
            {
                // Pull messages from server,
                // allowing an immediate response if there are no messages.
                PullResponse response = subscriberClient.Pull(subscriptionName, returnImmediately: true, maxMessages: 20);
                // Print out each received message.
                foreach (ReceivedMessage msg in response.ReceivedMessages)
                {
                    string text = System.Text.Encoding.UTF8.GetString(msg.Message.Data.ToArray());
                    Notification notification = JsonConvert.DeserializeObject<Notification>(text);
                    _logger.LogInformation($"Notify App Message received: {msg.Message.MessageId}: {text}");
                    Interlocked.Increment(ref messageCount);
                    notificationsList.Add(notification);
                }

                // If acknowledgement required, send to server.
                if (acknowledge && messageCount > 0)
                {
                    subscriberClient.Acknowledge(subscriptionName, response.ReceivedMessages.Select(msg => msg.AckId));
                }
            }
            catch (RpcException ex) when (ex.Status.StatusCode == StatusCode.Unavailable)
            {
                _logger.LogError(ex, $"UNAVAILABLE due to too many concurrent pull requests pending for subscription id: {subscriptionId}: {ex.Message}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Problem parsing notification message: {e.Message} for subscription id: {subscriptionId}");
            }

            return notificationsList;
        }
    }
}
