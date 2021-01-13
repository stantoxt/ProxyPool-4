using Microsoft.EntityFrameworkCore;
using ProxyPool.Core;
using ProxyPool.Core.Net;
using ProxyPool.Core.Redis;
using ProxyPool.Data.EntityFramework;
using ProxyPool.Data.EntityFramework.Models;
using ProxyPool.Service.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProxyPool.Service.Implementations
{
    internal class ProxyCheckService : IProxyCheckService
    {
        private readonly ProxyPoolContext _dbContext;
        private readonly IProxyCheck _proxyCheck;
        private readonly IRedisClientFactory _redisClientFactory;
        private readonly string REDIS_CHECK_KEY = "ProxyPool:Check";
        private readonly IRandomPool _randomPool;
        private const int MAX_SCORE = 5;

        public ProxyCheckService(
            ProxyPoolContext dbContext,
            IProxyCheck proxyCheck,
            IRedisClientFactory redisClientFactory,
            IRandomPool randomPool)
        {
            _dbContext = dbContext;
            _proxyCheck = proxyCheck;
            _redisClientFactory = redisClientFactory;
            _randomPool = randomPool;
        }

        public async Task ConsumeAsync()
        {
            var redisClient = _redisClientFactory.CreateClient();
            while (true)
            {
                var proxy = await redisClient.BLPopAsync<Proxy>(REDIS_CHECK_KEY, 0);
                var proxyStatus = await _proxyCheck.ConnectAsync(proxy.IP, proxy.Port, 5000);
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
        }

        public async Task PublishAsync()
        {
            int lastId = 0;
            int count = 500;
            var redisClient = _redisClientFactory.CreateClient();

            while (true)
            {
                var (records, nextLastId) = await GetPagedProxys(count, lastId);
                if (records?.Count <= 0)
                    break;

                lastId = nextLastId;
                await redisClient.RPushAsync(REDIS_CHECK_KEY, records.ToArray());
            }
        }

        private async Task<(List<Proxy>, int nextLastId)> GetPagedProxys(int count, int lastId = 0)
        {
            var query = _dbContext.Proxys.AsNoTracking();
            var records = await query
                                 .Where(c => c.Id > lastId)
                                 .OrderBy(c => c.Id)
                                 .Take(count)
                                 .ToListAsync();

            return (records, records?.LastOrDefault()?.Id ?? 0);
        }

        private async Task DeleteProxyAsync(Proxy proxy)
        {
            _dbContext.Proxys.Remove(proxy);
            await _dbContext.SaveChangesAsync();
            _dbContext.Entry(proxy).State = EntityState.Detached;
        }

        private async Task IncreaseScoreAsync(Proxy proxy, int value)
        {
            proxy.Score = proxy.Score + value;
            if (proxy.Score > MAX_SCORE)
                return;

            proxy.UpdatedTime = DateTime.Now;
            _dbContext.Update(proxy);
            await _dbContext.SaveChangesAsync();
            _dbContext.Entry(proxy).State = EntityState.Detached;
        }
    }
}
