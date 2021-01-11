using FreeRedis;

namespace ProxyPool.Core.Redis
{
    public interface IRedisClientFactory
    {
        RedisClient CreateClient();
    }
}
