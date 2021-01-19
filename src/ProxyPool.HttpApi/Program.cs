using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Formatting.Compact;
using System;
using System.IO;

namespace ProxyPool.HttpApi
{
    public class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables()
               .Build();

        public static int Main(string[] args)
        {
            Log.Logger = CreateSeriLogger();

            try
            {
                Log.Information("Starting web host");
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog();

        static ILogger CreateSeriLogger()
        {
            return new LoggerConfiguration().ReadFrom.Configuration(Configuration)
#if DEBUG
                       .MinimumLevel.Debug()
#endif
                       .Enrich.FromLogContext()
                       .WriteTo.Console(new RenderedCompactJsonFormatter())
                       .WriteTo.File(formatter: new RenderedCompactJsonFormatter(), "logs\\log-.txt", rollingInterval: RollingInterval.Day)
                       .CreateLogger();
        }
    }
}
