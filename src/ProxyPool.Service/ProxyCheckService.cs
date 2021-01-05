using Microsoft.EntityFrameworkCore;
using ProxyPool.Core.Net;
using ProxyPool.Data.EntityFramework;
using ProxyPool.Data.EntityFramework.Models;
using System.Threading.Tasks;

namespace ProxyPool.Service
{
    internal class ProxyCheckService : IProxyCheckService
    {
        private readonly ProxyPoolContext _dbContext;
        private readonly IProxyCheck _proxyCheck;

        public ProxyCheckService(ProxyPoolContext dbContext, IProxyCheck proxyCheck)
        {
            _dbContext = dbContext;
            _proxyCheck = proxyCheck;
        }

        public async Task CheckAsync()
        {
            var records =await _dbContext.Proxys.AsNoTracking().ToListAsync();
            if (records.Count <= 0)
                return;

            foreach(var record in records)
            {
                var proxyStatus = await _proxyCheck.ConnectAsync(record.IP, record.Port, 5000);
                if (proxyStatus.Connected)
                {
                    await IncreaseScoreAsync(record, 1);
                }
                else
                {
                    if (record.Score <= 1)
                        await DeleteProxyAsync(record);
                    else
                        await IncreaseScoreAsync(record, -1);
                }
            }
        }

        private async Task DeleteProxyAsync(Proxy proxy)
        {
            _dbContext.Proxys.Remove(proxy);
            await _dbContext.SaveChangesAsync();
            _dbContext.Entry(proxy).State = EntityState.Detached;
        }

        private async Task IncreaseScoreAsync(Proxy proxy, int value)
        {
            var model = new Proxy() { Id = proxy.Id, Score = proxy.Score + value };
            var entry = _dbContext.Entry(model);
            entry.State = EntityState.Modified;
            entry.Property(c => c.Score).IsModified = true;
            await _dbContext.SaveChangesAsync();
            entry.State = EntityState.Detached;
        }
    }
}
