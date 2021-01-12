using Microsoft.EntityFrameworkCore;
using ProxyPool.Core;
using ProxyPool.Data.EntityFramework;
using ProxyPool.Data.EntityFramework.Models;
using ProxyPool.Service.Abstracts;
using ProxyPool.Service.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ProxyPool.Service.Implementations
{
    internal class ProxyRandomService : IProxyRandomService
    {
        private readonly IRandomPool _randomPool;
        protected readonly ProxyPoolContext _dbContext;

        public ProxyRandomService(IRandomPool randomPool, ProxyPoolContext dbContext)
        {
            _randomPool = randomPool;
            _dbContext = dbContext;
        }

        public async Task<ProxyOutputDto> GetAsync()
        {
            var (host, port) = await _randomPool.NextAsync();
            if (string.IsNullOrWhiteSpace(host))
                return default;

            var proxy = await GetProxyAsync(host, port);
            if (proxy == null)
            {
                return new ProxyOutputDto { IP = host, Port = port };
            }
            else
            {
                return new ProxyOutputDto
                {
                    Port = proxy.Port,
                    IP = proxy.IP,
                    AnonymousDegree = proxy.AnonymousDegree,
                    CreatedTime = proxy.CreatedTime,
                    Score = proxy.Score,
                    UpdatedTime = proxy.UpdatedTime
                };
            }
        }

        private async Task<Proxy> GetProxyAsync(string host, int port)
        {
            return await _dbContext.Proxys
                    .AsNoTracking()
                    .Where(c => c.IP == host && c.Port == port)
                    .FirstOrDefaultAsync();
        }
    }
}
