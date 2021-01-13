using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ProxyPool.Service.Abstracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProxyPool.WorkerService
{
    public class ProxyCheckConsumeWork : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IOptions<ProxyCheckConsumeWorkOptions> _options;

        public ProxyCheckConsumeWork(
            IServiceProvider serviceProvider,
            IOptions<ProxyCheckConsumeWorkOptions> options)
        {
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
                    using var scope = _serviceProvider.CreateScope();
                    IProxyCheckService _checkService = scope.ServiceProvider.GetService<IProxyCheckService>();
                    await _checkService.ConsumeAsync();
                }, stoppingToken);
            }

            Task.WaitAll(tasks, stoppingToken);
        }
    }
}
