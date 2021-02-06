using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Serilog;
using Serilog.Formatting.Compact;
using System;
using System.IO;

namespace ProxyPool.Jobs
{
    class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables()
               .Build();

        static int Main(string[] args)
        {
            Log.Logger = CreateSeriLogger();

            try
            {
                Log.Information("Starting host");
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
              .ConfigureServices((hostContext, services) =>
              {
                  services.AddQuartz(q =>
                  {
                      // Use a Scoped container to create jobs. I'll touch on this later
                      q.UseMicrosoftDependencyInjectionScopedJobFactory();

                      q.AddJobAndTrigger<KuaiDaiLiJob>(hostContext.Configuration);
                      q.AddJobAndTrigger<XiLaDaiLiJob>(hostContext.Configuration);
                      q.AddJobAndTrigger<ProxyCheckJob>(hostContext.Configuration);
                  });

                  services.AddQuartzHostedService(
                    q => q.WaitForJobsToComplete = true);

                  services.AddProxyPoolService(hostContext.Configuration);
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
                       .WriteTo.File(
                            formatter: new RenderedCompactJsonFormatter(),
                            "logs\\log-.txt", 
                            rollingInterval: RollingInterval.Day)
                       .CreateLogger();
        }
    }
}
