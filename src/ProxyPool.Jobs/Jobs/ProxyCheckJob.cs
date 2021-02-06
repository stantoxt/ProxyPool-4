using Microsoft.Extensions.Logging;
using ProxyPool.Service.Abstracts;
using ProxyPool.Service.Check;
using Quartz;
using System.Threading.Tasks;

namespace ProxyPool.Jobs
{
    [DisallowConcurrentExecution]
    public class ProxyCheckJob : IJob
    {
        private readonly IProxyService _proxyService;
        private readonly IProxyCheckTaskQueue _checkTaskQueue;
        private readonly ILogger<ProxyCheckJob> _logger;

        public ProxyCheckJob(IProxyService proxyService, IProxyCheckTaskQueue checkTaskQueue, ILogger<ProxyCheckJob> logger)
        {
            _proxyService = proxyService;
            _checkTaskQueue = checkTaskQueue;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            int count = 500;
            int lastId = 0;

            while (true)
            {
                try
                {
                    var (records, nextLastId) = await _proxyService.GetPagedProxys(count, lastId);
                    if (records == null || records.Count <= 0)
                        break;

                    await _checkTaskQueue.EnqueueTaskItemAsync(records);
                    lastId = nextLastId;
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
        }
    }
}
