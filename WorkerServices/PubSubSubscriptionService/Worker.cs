using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PubSubSubscriptionService.Models;
using PubSubSubscriptionService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PubSubSubscriptionService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly SampleWebSettings _settings;

        public Worker(ILogger<Worker> logger, IOptions<SampleWebSettings> settings)
        {
            _logger = logger;
            _settings = settings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker started running at: {time}", DateTimeOffset.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                var pullMessages = new PullMessages(_logger);

                var message = await pullMessages.PullMessagesAsync("green-hall-318914", "test-messaging-sub", true);

                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage();
                    request.RequestUri = new Uri($"http://{_settings.WebApiHost}/weatherforecast");

                    string strPayload = JsonConvert.SerializeObject(message);
                    HttpContent httpContent = new StringContent(strPayload, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync($"http://{_settings.WebApiHost}/notify", httpContent);
                }

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
