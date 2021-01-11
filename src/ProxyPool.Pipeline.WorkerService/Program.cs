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
                    services
                        .AddProxyPoolService(hostContext.Configuration);
                    services.AddHostedService<Worker>(provider =>
                    {
                        var logger = provider.GetService<ILogger<Worker>>();
                        var pipeline = provider.GetService<IProxyPipeline>();
                        return new Worker(logger, pipeline, provider);
                    });
                });
    }
}
