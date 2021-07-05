using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SampleWebSettings _settings;

        public HomeController(ILogger<HomeController> logger, IOptions<SampleWebSettings> settingsOptions)
        {
            _logger = logger;
            _settings = settingsOptions.Value;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Index Method called from home controller");

            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri($"http://{_settings.WebApiHost}/weatherforecast");
                var response = await client.SendAsync(request);
                ViewData["Message"] += "and " + await response.Content.ReadAsStringAsync();
            }

            return View();
        }

        [Route("/Home/Test", Name = "Custom")]
        public async Task<string> Test(MessagePayload payload)
        {
            var client = new PushMessageClient(_logger);

            await client.PublishMessageWithCustomAttributesAsync(_settings.GcpProjectId, _settings.GcpTopicId, payload);

            return $"Message {payload.Message} published to gcp pub sub";
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
