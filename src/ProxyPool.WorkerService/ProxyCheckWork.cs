using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProxyPool.Service.Check;
using ProxyPool.Service.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProxyPool.WorkerService
{
    public class ProxyCheckWork : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IProxyCheckTaskQueue _taskQueue;
        private readonly ILogger<ProxyCheckWork> _logger;

        public ProxyCheckWork(IServiceProvider serviceProvider, IProxyCheckTaskQueue taskQueue, ILogger<ProxyCheckWork> logger)
        {
            _serviceProvider = serviceProvider;
            _taskQueue = taskQueue;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            while (!stoppingToken.IsCancellationRequested)
            {
                var item = await _taskQueue.DequeueAsync();
                if (item == null)
                {
                    await Task.Delay(100);
                    continue;
                }

                DoWork(item);
            }
        }

        async Task DoWork(ProxyDto dto)
        {
            using var scope = _serviceProvider.CreateScope();
            try
            {
                var proxyCheck = scope.ServiceProvider.GetService<IProxyCheck>();
                await proxyCheck.Check(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}
