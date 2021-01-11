using FreeRedis;
using Microsoft.Extensions.Options;
using System;

namespace ProxyPool.Core.Redis
{
    internal class RedisClientFactory : IRedisClientFactory, IDisposable
    {
        private readonly IOptions<RedisOptions> _options;
        private readonly Lazy<RedisClient> _redisClient;
        protected RedisClient Client => _redisClient.Value;

        public RedisClientFactory(IOptions<RedisOptions> options)
        {
            _options = options;
            _redisClient = new Lazy<RedisClient>(() =>
            {
                return new RedisClient(_options.Value.ConnectionString);
            });
        }

        public RedisClient CreateClient()
        {
            return _redisClient.Value;
        }

        public void Dispose()
        {
            if (_redisClient.IsValueCreated)
                _redisClient.Value.Dispose();
        }
    }
}
