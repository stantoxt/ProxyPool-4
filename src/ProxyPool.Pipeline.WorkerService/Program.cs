using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProxyPool.Core.Pipeline;
using ProxyPool.Service.Abstracts;

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
                    services
                        .AddProxyPoolService(hostContext.Configuration);

                    services
                        .AddHostedService(provider =>
                        {
                            var logger = provider.GetService<ILogger<PipelineWorker>>();
                            var pipeline = provider.GetService<IProxyPipeline>();
                            return new PipelineWorker(logger, pipeline, provider);
                        })
                        .AddHostedService<ProxyCheckConsumeWork>();
                });
    }
}
