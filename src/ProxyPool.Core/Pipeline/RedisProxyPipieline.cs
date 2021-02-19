using Newtonsoft.Json;
using ProxyPool.Core.Redis;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProxyPool.Core.Pipeline
{
    internal class RedisProxyPipieline : IProxyPipeline
    {
        private readonly IConnectionMultiplexerFactory _connectionMultiplexerFactory;
        private const string PIPE_LINE_NAME = "ProxyPool:PipeLine";

        public RedisProxyPipieline(IConnectionMultiplexerFactory connectionMultiplexerFactory)
        {
            _connectionMultiplexerFactory = connectionMultiplexerFactory;
        }

        IConnectionMultiplexer ConnectionMultiplexer => _connectionMultiplexerFactory.CreateConnectionMultiplexer();

        public async Task AddAsync(ProxyInfo proxy)
        {
            if (proxy == null)
                throw new ArgumentNullException(nameof(proxy));

            var db = ConnectionMultiplexer.GetDatabase();
            await db.ListRightPushAsync(PIPE_LINE_NAME, JsonConvert.SerializeObject(proxy));
        }

        public async Task AddAsync(IEnumerable<ProxyInfo> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));


            var db = ConnectionMultiplexer.GetDatabase();
            var redisValues = list.Select(c => new RedisValue(JsonConvert.SerializeObject(c)));
            await db.ListRightPushAsync(PIPE_LINE_NAME, redisValues.ToArray());
        }

        public async Task<ProxyInfo> TakeAsync()
        {
            var db = ConnectionMultiplexer.GetDatabase();
            var value = await db.ListLeftPopAsync(PIPE_LINE_NAME);
            if (value.IsNullOrEmpty)
                return null;

            return JsonConvert.DeserializeObject<ProxyInfo>(value);
        }
    }
}
