using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProxyPool.Core;
using ProxyPool.Core.Pipeline;
using ProxyPool.Spiders;
using Quartz;
using System;
using System.Threading.Tasks;

namespace ProxyPool.Jobs
{
    [DisallowConcurrentExecution]
    public class KuaiDaiLiJob : IJob
    {
        private readonly ILogger<KuaiDaiLiJob> _logger;
        private readonly IProxyPipeline _channel;

        public KuaiDaiLiJob(ILogger<KuaiDaiLiJob> logger, IProxyPipeline channel)
        {
            _logger = logger;
            _channel = channel;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            KuaiDaiLiSpider spider = new KuaiDaiLiSpider();
            await foreach (var proxy in spider.GetProxyInfos())
            {
                _logger.LogDebug(JsonConvert.SerializeObject(proxy));
                await _channel.AddAsync(proxy);
            }
        }
    }
}
