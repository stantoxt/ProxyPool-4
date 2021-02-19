using StackExchange.Redis;

namespace ProxyPool.Core.Redis
{
    public interface IConnectionMultiplexerFactory
    {
        IConnectionMultiplexer CreateConnectionMultiplexer();
    }
}
