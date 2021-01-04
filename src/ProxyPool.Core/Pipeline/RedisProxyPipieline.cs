using FreeRedis;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProxyPool.Core.Pipeline
{
    internal class RedisProxyPipieline : IProxyPipeline
    {
        private readonly IOptions<RedisProxyPipelineOptions> _option;
        private readonly Lazy<RedisClient> _redisClient;

        public RedisProxyPipieline(IOptions<RedisProxyPipelineOptions> option)
        {
            _option = option;
            _redisClient = new Lazy<RedisClient>(() =>
            {
                return new RedisClient(_option.Value.ConnectionString);
            });
        }

        protected RedisClient RedisClient => _redisClient.Value;

        public async Task AddAsync(ProxyInfo proxy)
        {
            if (proxy == null)
                throw new ArgumentNullException(nameof(proxy));

            await RedisClient.RPushAsync(_option.Value.PipelineName, JsonConvert.SerializeObject(proxy));
        }

        public async Task AddAsync(IEnumerable<ProxyInfo> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            await RedisClient.RPushXAsync(_option.Value.PipelineName, list.Select(c => JsonConvert.SerializeObject(c)));
        }

        public async Task<ProxyInfo> TakeAsync()
        {
            var data = await RedisClient.BLPopAsync(_option.Value.PipelineName, 0);
            return JsonConvert.DeserializeObject<ProxyInfo>(data);
        }
    }
}
