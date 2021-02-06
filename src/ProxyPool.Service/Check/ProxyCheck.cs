using Microsoft.EntityFrameworkCore;
using ProxyPool.Core;
using ProxyPool.Core.Net;
using ProxyPool.Data.EntityFramework;
using ProxyPool.Data.EntityFramework.Models;
using ProxyPool.Service.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProxyPool.Service.Check
{
    class ProxyCheck : IProxyCheck
    {
        private const int MAX_SCORE = 5;
        private readonly ProxyPoolContext _dbContext;
        private readonly IProxySocket _socket;
        private readonly IRandomPool _randomPool;

        public ProxyCheck(ProxyPoolContext dbContext, IProxySocket socket, IRandomPool randomPool)
        {
            _dbContext = dbContext;
            _socket = socket;
            _randomPool = randomPool;
        }

        public async Task Check(ProxyDto dto)
        {
            var proxy = await GetProxy(dto.Id);
            if (proxy == null)
                return;

            var proxyStatus = await _socket.ConnectAsync(proxy.IP, proxy.Port, 5000);
            if (proxyStatus.Connected)
            {
                await IncreaseScoreAsync(proxy, 1);
                await _randomPool.AddAsync(proxy.IP, proxy.Port);
            }
            else
            {
                if (proxy.Score <= 1)
                {
                    await DeleteProxyAsync(proxy);
                    await _randomPool.RemoveAsync(proxy.IP, proxy.Port);
                }
                else
                {
                    await IncreaseScoreAsync(proxy, -1);
                    await _randomPool.AddAsync(proxy.IP, proxy.Port);
                }
            }
        }

        async Task<Proxy> GetProxy(int id)
        {
            return await _dbContext.Proxys.AsNoTracking().Where(c => c.Id == id).FirstOrDefaultAsync();
        }

        async Task DeleteProxyAsync(Proxy proxy)
        {
            _dbContext.Proxys.Remove(proxy);
            await _dbContext.SaveChangesAsync();
        }

        async Task IncreaseScoreAsync(Proxy proxy, int value)
        {
            proxy.Score = proxy.Score + value;
            if (proxy.Score > MAX_SCORE)
                return;

            proxy.UpdatedTime = DateTime.Now;
            _dbContext.Update(proxy);
            await _dbContext.SaveChangesAsync();
        }
    }
}
