using Microsoft.Extensions.Hosting;
using Quartz;
using System;

namespace ProxyPool.Jobs
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
              .ConfigureServices((hostContext, services) =>
              {
                  services.AddQuartz(q =>
                  {
                    // Use a Scoped container to create jobs. I'll touch on this later
                    q.UseMicrosoftDependencyInjectionScopedJobFactory();
                  });

                  // Add the Quartz.NET hosted service
                  services.AddQuartzHostedService(
                    q => q.WaitForJobsToComplete = true);
                  // other config
              });
    }
}
