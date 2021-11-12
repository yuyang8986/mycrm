using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyCRM.Persistence;
using MyCRM.Shared.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

//using Microsoft.Extensions.Logging.AzureAppServices;

namespace MyCRM.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            //if (Environment.IsDevelopment())
            //    DatabaseInitializer.EnsureSeedData(Configuration.GetConnectionString("LocalConnection"));
            //else
            //    DatabaseInitializer.EnsureSeedData(Configuration.GetConnectionString("DefaultConnection"));

            //Seed database
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                try
                {
                    var databaseInitializer = services.GetRequiredService<IDatabaseInitializer>();
                    databaseInitializer.SeedAsync().Wait();
                    logger.LogInformation("Seeded the database.");
                }
                catch (Exception ex)
                {
                     
                     logger.LogWarning(LoggingEvents.GenerateItems, ex, "Seed the database failed");
                     throw new Exception(LoggingEvents.GenerateItems.ToString(), ex);
                }
            }

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddDebug();
                    logging.AddEventSourceLogger();
                }).ConfigureServices(services =>
                {
                    services.AddHostedService<TimedHostedService>();
                })
            .UseSerilog((context, configuration) =>
            {
                configuration
                    .MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .MinimumLevel.Override("System", LogEventLevel.Information)
                    .Enrich.FromLogContext()
                    .WriteTo.RollingFile("Systemlog/Systemlog-{Date}.txt", outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
                    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Literate);
            });
        
    }
}