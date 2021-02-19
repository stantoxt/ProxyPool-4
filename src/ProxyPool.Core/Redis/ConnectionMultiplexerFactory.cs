using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;

namespace ProxyPool.Core.Redis
{
    internal class ConnectionMultiplexerFactory : IConnectionMultiplexerFactory, IDisposable
    {
        private readonly IOptions<RedisOptions> _options;
        private readonly Lazy<IConnectionMultiplexer> _connection;
        protected IConnectionMultiplexer Client => _connection.Value;

        public ConnectionMultiplexerFactory(IOptions<RedisOptions> options)
        {
            _options = options;

            _connection = new Lazy<IConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect(_options.Value.ConnectionString);
            });
        }

        public IConnectionMultiplexer CreateConnectionMultiplexer()
        {
            return _connection.Value;
        }

        public void Dispose()
        {
            if (_connection.IsValueCreated)
                _connection.Value.Dispose();
        }
    }
}
