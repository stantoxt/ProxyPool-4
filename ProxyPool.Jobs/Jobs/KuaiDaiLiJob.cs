using Microsoft.Extensions.Logging;
using ProxyPool.Spiders;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProxyPool.Jobs.Jobs
{
    [DisallowConcurrentExecution]
    public class KuaiDaiLiJob : IJob
    {
        private readonly ILogger<KuaiDaiLiJob> _logger;
        public KuaiDaiLiJob(ILogger<KuaiDaiLiJob> logger)
        {
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            KuaiDaiLiSpider spider = new KuaiDaiLiSpider();
            throw new NotImplementedException();
        }
    }
}
