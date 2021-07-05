using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Google.Cloud.Diagnostics.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Diagnostics.Common;
using Microsoft.Extensions.Logging;

namespace PubSubSubscriptionService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    //string credential_path = hostContext.Configuration["gcp_application_credentials_file_path"];
                    //System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credential_path);
                    services.AddSingleton<ILoggerProvider>(sp => GoogleLoggerProvider.Create(sp, "green-hall-318914"));
                    services.AddHostedService<Worker>();
                    // Add trace service.
                    //services.AddGoogleTrace(options =>
                    //{
                    //    //options.ProjectId = Configuration["Stackdriver:ProjectId"];
                    //    options.ProjectId = "neat-fin-309913";
                    //    options.Options = TraceOptions.Create(
                    //        bufferOptions: BufferOptions.NoBuffer());
                    //});
                });
    }
}
