using System;
using System.Threading.Tasks;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;

namespace WebApplication1.Services
{
    public class PushMessageClient
    {
        public async Task PublishMessageWithCustomAttributesAsync(string projectId, string topicId, string messageText)
        {
            try
            {
                TopicName topicName = TopicName.FromProjectTopic(projectId, topicId);
                PublisherClient publisher = await PublisherClient.CreateAsync(topicName);

                var pubsubMessage = new PubsubMessage
                {
                    // The data is any arbitrary ByteString. Here, we're using text.
                    Data = ByteString.CopyFromUtf8(messageText),
                    // The attributes provide metadata in a string-to-string dictionary.
                    Attributes =
                    {
                        { "id", "test" }
                    }
                };
                string message = await publisher.PublishAsync(pubsubMessage);
                Console.WriteLine($"Published message {message}");
            }
            catch (Exception e)
            {
                throw new Exception("Error publishing to Google Pub Sub", e);
            }
        }
    }
}
