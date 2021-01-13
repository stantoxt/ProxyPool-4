using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProxyPool.Core.Pipeline;

namespace ProxyPool.Pipeline.WorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSystemd()
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<PipelineWorkerOptions>(hostContext.Configuration.GetSection(PipelineWorkerOptions.Position));

                    services
                        .AddProxyPoolService(hostContext.Configuration);

                    services
                        .AddHostedService<PipelineWorker>()
                        .AddHostedService<ProxyCheckConsumeWork>();
                });
    }
}
