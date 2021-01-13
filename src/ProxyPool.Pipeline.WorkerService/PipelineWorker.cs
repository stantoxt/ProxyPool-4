using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProxyPool.Core.Pipeline;
using ProxyPool.Service.Abstracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProxyPool.Pipeline.WorkerService
{
    public class PipelineWorker : BackgroundService
    {
        private readonly ILogger<PipelineWorker> _logger;
        private readonly IProxyPipeline _pipeline;
        private readonly IServiceProvider _serviceProvider;
        private readonly IOptions<PipelineWorkerOptions> _options;

        public PipelineWorker(
            ILogger<PipelineWorker> logger,
            IProxyPipeline pipeline,
            IServiceProvider serviceProvider,
            IOptions<PipelineWorkerOptions> options)
        {
            _logger = logger;
            _pipeline = pipeline;
            _serviceProvider = serviceProvider;
            _options = options;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task[] tasks = new Task[_options.Value.ThreadCount];
            for (var i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(async () =>
               {
                   while (!stoppingToken.IsCancellationRequested)
                   {
                       var info = await _pipeline.TakeAsync();
                       using var scope = _serviceProvider.CreateScope();
                       var proxyService = scope.ServiceProvider.GetService<IProxyService>();
                       if (string.IsNullOrWhiteSpace(info.IP))
                           continue;

                       var exists = await proxyService.ExistsAsync(info.IP, info.Port);
                       if (exists)
                           continue;

                       await proxyService.AddAsync(new Service.Models.ProxyDto
                       {
                           AnonymousDegree = (int)info.AnonymousDegree,
                           CreatedTime = DateTime.Now,
                           IP = info.IP,
                           Port = info.Port,
                           Score = 1,
                           UpdatedTime = DateTime.Now
                       });
                       _logger.LogInformation($"successfully added {info.IP}:{info.Port}");
                   }
               }, stoppingToken);
            }

            Task.WaitAll(tasks, stoppingToken);
        }
    }
}
