using ProxyPool.Core.Redis;
using ProxyPool.Service.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProxyPool.Service.Check
{
    class ProxyCheckTaskRedisQueue : IProxyCheckTaskQueue
    {
        private readonly IRedisClientFactory _redisClientFactory;
        private readonly string REDIS_CHECK_KEY = "ProxyPool:Check";

        public ProxyCheckTaskRedisQueue(IRedisClientFactory redisClientFactory)
        {
            _redisClientFactory = redisClientFactory;
        }

        public async Task EnqueueTaskItemAsync(ProxyDto dto)
        {
            var redisClient = _redisClientFactory.CreateClient();
            await redisClient.RPushAsync(REDIS_CHECK_KEY, dto);
        }
        public async Task EnqueueTaskItemAsync(ICollection<ProxyDto> dtos)
        {
            var redisClient = _redisClientFactory.CreateClient();
            await redisClient.RPushAsync(REDIS_CHECK_KEY, dtos.ToArray());
        }

        public async Task<ProxyDto> DequeueAsync()
        {
            var redisClient = _redisClientFactory.CreateClient();
            return await redisClient.LPopAsync<ProxyDto>(REDIS_CHECK_KEY);
        }
    }
}
