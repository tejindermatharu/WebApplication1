using System;
using System.Threading.Tasks;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace WebApplication1.Services
{
    public class PushMessageClient
    {
        private readonly ILogger _logger;

        public PushMessageClient(ILogger logger)
        {
            _logger = logger;
        }

        public async Task PublishMessageWithCustomAttributesAsync(string projectId, string topicId, object messageObj)
        {
            try
            {
                TopicName topicName = TopicName.FromProjectTopic(projectId, topicId);
                PublisherClient publisher = await PublisherClient.CreateAsync(topicName);

                var pubsubMessage = new PubsubMessage
                {
                    // The data is any arbitrary ByteString. Here, we're using text.
                    Data = ByteString.CopyFromUtf8(JsonConvert.SerializeObject(messageObj)),
                    // The attributes provide metadata in a string-to-string dictionary.
                    Attributes =
                    {
                        { "id", "test" }
                    }
                };
                string message = await publisher.PublishAsync(pubsubMessage);
                _logger.LogInformation($"Published message {message}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error publishing to Google Pub Sub: {e.Message}");
                throw new Exception("Error publishing to Google Pub Sub", e);
            }
        }
    }
}
