using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
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

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Index Method called from home controller");

            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri($"http://{_settings.WebApiHost}/test");
                var response = await client.SendAsync(request);
                ViewData["ApiMessage"] += await response.Content.ReadAsStringAsync();
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(Notification payload)
        {
            var client = new PushMessageClient(_logger);

            await client.PublishMessageWithCustomAttributesAsync(_settings.GcpProjectId, _settings.GcpTopicId, payload);
            ViewData["Message"] += "Message with Isin:  " + payload.Isin + "...published to GCP Pub Sub.";
     
            return View();
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
