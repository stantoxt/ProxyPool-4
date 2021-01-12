using FreeRedis;
using ProxyPool.Core.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProxyPool.Core
{
    internal class RedisRandomPool : IRandomPool
    {
        private readonly IRedisClientFactory _redisClientFactory;
        private const string PROXY_RANDOM_POOL_KEY = "ProxyPool:Random";
        private const string DELIMITER = ":";

        public RedisRandomPool(IRedisClientFactory redisClientFactory)
        {
            _redisClientFactory = redisClientFactory;
        }

        RedisClient RedisClient => _redisClientFactory.CreateClient();

        public async Task<(string host, int port)> NextAsync()
        {
            var value = await RedisClient.SRandMemberAsync(PROXY_RANDOM_POOL_KEY);
            return GetUnformatValue(value);
        }

        public async Task AddAsync(string host, int port)
        {
            var value = GetFormatValue(host, port);
            await RedisClient.SAddAsync(PROXY_RANDOM_POOL_KEY, value);
        }

        public async Task AddAsync(IEnumerable<(string host, int port)> ts)
        {
            var values = ts.Select(c => GetFormatValue(c.host, c.port));
            await RedisClient.SAddAsync(PROXY_RANDOM_POOL_KEY, values.ToArray());
        }

        public async Task RemoveAsync(string host, int port)
        {
            var value = GetFormatValue(host, port);
            await RedisClient.SRemAsync(PROXY_RANDOM_POOL_KEY, value);
        }

        public async Task RemoveAsync(IEnumerable<(string host, int port)> ts)
        {
            var values = ts.Select(c => GetFormatValue(c.host, c.port));
            await RedisClient.SRemAsync(PROXY_RANDOM_POOL_KEY, values.ToArray());
        }

        private string GetFormatValue(string host, int port)
        {
            return $"{host}{DELIMITER}{port}";
        }

        private (string host, int port) GetUnformatValue(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return (default, default);

            var array = str.Split(DELIMITER);
            int port = 0;
            if (array.Length > 1)
                int.TryParse(array[1], out port);

            return (array[0], port);
        }
    }
}
