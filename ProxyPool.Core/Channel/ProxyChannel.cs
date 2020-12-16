using FreeRedis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProxyPool.Core.Channel
{
    public class ProxyChannel : IProxyChannel
    {
        private static RedisClient _redisClient = new RedisClient("localhost:6379");
        private const string REDIS_CHANNEL_KEY= "ProxyPool:Channel";
        public async Task AddAsync(ProxyInfo proxy)
        {
            if (proxy == null)
                throw new ArgumentNullException(nameof(proxy));

            await _redisClient.RPushAsync(REDIS_CHANNEL_KEY, JsonConvert.SerializeObject(proxy));
        }

        public async Task AddAsync(IEnumerable<ProxyInfo> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            await _redisClient.RPushXAsync(REDIS_CHANNEL_KEY, list.Select(c => JsonConvert.SerializeObject(c)));
        }

        public async Task<ProxyInfo> TakeAsync()
        {
            var data = await _redisClient.BLPopAsync(REDIS_CHANNEL_KEY, 0);
            return JsonConvert.DeserializeObject<ProxyInfo>(data);
        }
    }
}
