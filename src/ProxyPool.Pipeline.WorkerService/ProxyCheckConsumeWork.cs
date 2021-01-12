using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProxyPool.Service.Abstracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProxyPool.Pipeline.WorkerService
{
    public class ProxyCheckConsumeWork : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public ProxyCheckConsumeWork(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            IProxyCheckService _checkService = scope.ServiceProvider.GetService<IProxyCheckService>();
            await _checkService.ConsumeAsync();
        }
    }
}
