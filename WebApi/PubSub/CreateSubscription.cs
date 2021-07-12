using Google.Apis.Logging;
using Google.Cloud.PubSub.V1;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.PubSub
{
    public class CreatePubSubSubscription
    {
        private readonly ILogger _logger;

        public CreatePubSubSubscription(ILogger logger)
        {
            _logger = logger;
        }

        public Subscription CreateSubscription(string projectId, string topicId, string subscriptionName)
        {
            SubscriberServiceApiClient subscriber = SubscriberServiceApiClient.Create();
            //TopicName topicName = TopicName.FromProjectTopic(projectId, topicId);

            Subscription subscription = null;

            try
            {
                subscription = subscriber.CreateSubscription(subscriptionName, topicId, pushConfig: null, ackDeadlineSeconds: 60);
                //https://cloud.google.com/pubsub/docs/filtering
                subscription.Filter = "";
                _logger.Info($"Subscription {subscriptionName} for topic {topicId} created");
            }
            catch (RpcException e) when (e.Status.StatusCode == StatusCode.AlreadyExists)
            {
                // Already exists.  That's fine.
                _logger.Info($"Subscription {subscriptionName} for topic {topicId} already exists");
            }
            catch (Exception e) {
                _logger.Error(e, $"Error creating subscription {subscriptionName} for topic {topicId}");
            }
            return subscription;
        }
    }
}
