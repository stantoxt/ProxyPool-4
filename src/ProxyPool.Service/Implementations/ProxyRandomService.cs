using Newtonsoft.Json;
using ProxyPool.Core.Redis;
using ProxyPool.Service.Abstracts;
using ProxyPool.Service.Models;
using System.Threading.Tasks;

namespace ProxyPool.Service.Implementations
{
    internal class ProxyRandomService : IProxyRandomService
    {
        private readonly IRedisClientFactory _redisClientFactory;
        private const string PROXY_SET_KEY = "ProxyPool:List";

        public ProxyRandomService(IRedisClientFactory redisClientFactory)
        {
            _redisClientFactory = redisClientFactory;
        }

        public async Task<ProxyOutputDto> GetAsync()
        {
            var redisClient = _redisClientFactory.CreateClient();
            var json = await redisClient.SRandMemberAsync(PROXY_SET_KEY);
            return JsonConvert.DeserializeObject<ProxyOutputDto>(json);
        }
    }
}
