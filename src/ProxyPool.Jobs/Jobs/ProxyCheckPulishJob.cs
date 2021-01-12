using ProxyPool.Service.Abstracts;
using Quartz;
using System.Threading.Tasks;

namespace ProxyPool.Jobs
{
    [DisallowConcurrentExecution]
    public class ProxyCheckPulishJob : IJob
    {
        private readonly IProxyCheckService _checkService;

        public ProxyCheckPulishJob(IProxyCheckService checkService)
        {
            _checkService = checkService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _checkService.PublishAsync();
        }
    }
}
