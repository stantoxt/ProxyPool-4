using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;

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

                      q.AddJobAndTrigger<KuaiDaiLiJob>(hostContext.Configuration);
                      q.AddJobAndTrigger<XiLaDaiLiJob>(hostContext.Configuration);
                      q.AddJobAndTrigger<ProxyCheckPulishJob>(hostContext.Configuration);
                  });

                  services.AddQuartzHostedService(
                    q => q.WaitForJobsToComplete = true);

                  services.AddProxyPoolService(hostContext.Configuration);
              });
    }
}
