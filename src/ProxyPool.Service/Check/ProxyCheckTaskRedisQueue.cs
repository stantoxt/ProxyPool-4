using Newtonsoft.Json;
using ProxyPool.Core.Redis;
using ProxyPool.Service.Models;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProxyPool.Service.Check
{
    class ProxyCheckTaskRedisQueue : IProxyCheckTaskQueue
    {
        private readonly IConnectionMultiplexerFactory _connectionMultiplexerFactory;
        private readonly string REDIS_CHECK_KEY = "ProxyPool:Check";

        public ProxyCheckTaskRedisQueue(IConnectionMultiplexerFactory connectionMultiplexerFactory)
        {
            _connectionMultiplexerFactory = connectionMultiplexerFactory;
        }

        public async Task EnqueueTaskItemAsync(ProxyDto dto)
        {
            var conn = _connectionMultiplexerFactory.CreateConnectionMultiplexer();
            var db = conn.GetDatabase();
            await db.ListRightPushAsync(REDIS_CHECK_KEY, JsonConvert.SerializeObject(dto));
        }
        public async Task EnqueueTaskItemAsync(ICollection<ProxyDto> dtos)
        {
            var conn = _connectionMultiplexerFactory.CreateConnectionMultiplexer();
            var db = conn.GetDatabase();
            var redisValues = dtos.Select(c => new RedisValue(JsonConvert.SerializeObject(c))).ToArray();
            await db.ListRightPushAsync(REDIS_CHECK_KEY, redisValues);
        }

        public async Task<ProxyDto> DequeueAsync()
        {
            var conn = _connectionMultiplexerFactory.CreateConnectionMultiplexer();
            var db = conn.GetDatabase();
            var redisValue = await db.ListLeftPopAsync(REDIS_CHECK_KEY);
            if (!redisValue.HasValue || redisValue.IsNullOrEmpty)
                return null;

            return JsonConvert.DeserializeObject<ProxyDto>(redisValue);
        }
    }
}
