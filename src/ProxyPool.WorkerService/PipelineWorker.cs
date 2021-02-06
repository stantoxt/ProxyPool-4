using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProxyPool.Core;
using ProxyPool.Core.Pipeline;
using ProxyPool.Service.Abstracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProxyPool.WorkerService
{
    public class PipelineWorker : BackgroundService
    {
        private readonly ILogger<PipelineWorker> _logger;
        private readonly IProxyPipeline _pipeline;
        private readonly IServiceProvider _serviceProvider;

        public PipelineWorker(
            ILogger<PipelineWorker> logger,
            IProxyPipeline pipeline,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _pipeline = pipeline;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var info = await _pipeline.TakeAsync();
                    if (info == null)
                    {
                        await Task.Delay(100);
                        continue;
                    }

                    Exec(info);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
        }

        async Task Exec(ProxyInfo info)
        {
            using var scope = _serviceProvider.CreateScope();
            var proxyService = scope.ServiceProvider.GetService<IProxyService>();
            if (string.IsNullOrWhiteSpace(info.IP))
                return;

            var exists = await proxyService.ExistsAsync(info.IP, info.Port);
            if (exists)
                return;

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
    }
}
