using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProxyPool.Core.Pipeline;
using ProxyPool.Spiders;
using Quartz;
using System.Threading.Tasks;

namespace ProxyPool.Jobs
{
    [DisallowConcurrentExecution]
    public class XiLaDaiLiJob : IJob
    {
        private readonly ILogger<XiLaDaiLiJob> _logger;
        private readonly IProxyPipeline _channel;

        public XiLaDaiLiJob(ILogger<XiLaDaiLiJob> logger, IProxyPipeline channel)
        {
            _logger = logger;
            _channel = channel;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            XiLaDaiLiSpider spider = new XiLaDaiLiSpider();
            await foreach (var proxy in spider.GetProxyInfos())
            {
                _logger.LogDebug(JsonConvert.SerializeObject(proxy));
                await _channel.AddAsync(proxy);
            }
        }
    }
}
