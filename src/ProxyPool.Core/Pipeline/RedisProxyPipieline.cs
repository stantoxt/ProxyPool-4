using Newtonsoft.Json;
using ProxyPool.Core.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProxyPool.Core.Pipeline
{
    internal class RedisProxyPipieline : IProxyPipeline
    {
        private readonly IRedisClientFactory _redisClientFactory;
        private const string PIPE_LINE_NAME = "ProxyPool:PipeLine";

        public RedisProxyPipieline(IRedisClientFactory redisClientFactory)
        {
            _redisClientFactory = redisClientFactory;
        }

        public async Task AddAsync(ProxyInfo proxy)
        {
            if (proxy == null)
                throw new ArgumentNullException(nameof(proxy));

            var redisClient = _redisClientFactory.CreateClient();
            await redisClient.RPushAsync(PIPE_LINE_NAME, JsonConvert.SerializeObject(proxy));
        }

        public async Task AddAsync(IEnumerable<ProxyInfo> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));


            var redisClient = _redisClientFactory.CreateClient();
            await redisClient.RPushAsync(PIPE_LINE_NAME, list.Select(c => JsonConvert.SerializeObject(c)));
        }

        public async Task<ProxyInfo> TakeAsync()
        {
            var redisClient = _redisClientFactory.CreateClient();
            var data = await redisClient.BLPopAsync(PIPE_LINE_NAME, 0);
            return JsonConvert.DeserializeObject<ProxyInfo>(data);
        }
    }
}
