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
            var proxyStatus = await _socket.ConnectAsync(dto.IP, dto.Port, 5000);
            if (proxyStatus.Connected)
            {
                await IncreaseScoreAsync(dto, 1);
                await _randomPool.AddAsync(dto.IP, dto.Port);
            }
            else
            {
                if (dto.Score <= 1)
                {
                    await DeleteProxyAsync(dto);
                    await _randomPool.RemoveAsync(dto.IP, dto.Port);
                }
                else
                {
                    await IncreaseScoreAsync(dto, -1);
                    await _randomPool.AddAsync(dto.IP, dto.Port);
                }
            }
        }

        async Task DeleteProxyAsync(ProxyDto proxy)
        {
            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM t_proxys WHERE id={proxy.Id}");
        }

        async Task IncreaseScoreAsync(ProxyDto proxy, int value)
        {
            var newScore = proxy.Score + value;
            if (newScore > MAX_SCORE)
                return;

            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"UPDATE t_proxys SET score={newScore}, updated_time={DateTime.Now} WHERE  id={proxy.Id}");
        }
    }
}
