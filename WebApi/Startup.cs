using Google.Cloud.Diagnostics.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Hubs;
using WebApi.Services;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (System.Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "false")
            {
                string credential_path = Configuration["gcp_application_credentials_file_path"];
                System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credential_path);
            }

            ServiceProvider serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetService<ILogger<PullMessages>>();
            services.AddSingleton(typeof(ILogger), logger);

            services.AddSingleton<IPullMessages, PullMessages>();

            services.AddHostedService<Worker>();  // the worker will run as a background service
            services.AddSignalR(r => r.EnableDetailedErrors = true);
            services.AddControllers();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", corsPolicyBuilder =>
                {
                    corsPolicyBuilder.WithOrigins("http://localhost:3375", "http://localhost:8080")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.AddGoogle(app.ApplicationServices, "green-hall-318914");

            var logger = loggerFactory.CreateLogger("testnotifyapp");

            // Write the log entry.
            logger.LogInformation("Notify web site started. This is a log message.");
            
            app.UseCors("CorsPolicy");
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<NotifyHub>("/hubs/notify");
            });
        }
    }
}
