using System;
using System.Threading.Tasks;
using JOS.HttpDelegatingHandler.Infra.Http;
using JOS.HttpDelegatingHandler.Infra.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace JOS.HttpDelegatingHandler
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder().AddCommandLine(args).Build();
            var environment = GetEnvironment(configuration);

            var builder = new HostBuilder()
                .UseEnvironment(environment)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddConfiguration(configuration);
                    config.AddJsonFile("appsettings.json");
                    config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment}.json", optional: true);
                    config.AddJsonFile("appsettings.Local.json", optional: true);
                    config.AddEnvironmentVariables();
                    config.AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddTransient<DefaultHttpDelegatingHandler>();
                    services.AddHttpClient<DummyApiHttpClient>((client) =>
                    {
                        client.BaseAddress = new Uri("http://localhost:5000");
                    }).AddHttpMessageHandler<DefaultHttpDelegatingHandler>();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    new LoggerConfigurator(hostingContext.Configuration, hostingContext.HostingEnvironment).Configure();
                    logging.AddSerilog(dispose: true);
                })
                .UseDefaultServiceProvider((context, options) =>
                {
                    options.ValidateOnBuild = true;
                    options.ValidateScopes = true;
                });

            var host = builder.Build();

            var hehe = host.Services.GetRequiredService<DummyApiHttpClient>();
            var test = await hehe.GetWeatherForecast("Axvall");

            try
            {
                await host.RunAsync();
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static string GetEnvironment(IConfiguration configuration)
        {
            var environment = configuration.GetValue<string>("environment");

            if (string.IsNullOrWhiteSpace(environment))
            {
                environment = Environments.Development;
            }

            return environment;
        }
    }
}
