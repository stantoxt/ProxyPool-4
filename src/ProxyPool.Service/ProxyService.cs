using Microsoft.EntityFrameworkCore;
using ProxyPool.Data.EntityFramework;
using ProxyPool.Data.EntityFramework.Models;
using ProxyPool.Service.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ProxyPool.Service
{
    internal class ProxyService : IProxyService
    {
        protected readonly ProxyPoolContext _context;

        public ProxyService(ProxyPoolContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ProxyDto dto)
        {
            await _context.Proxys.AddAsync(new Proxy
            {
                AnonymousDegree = dto.AnonymousDegree,
                CreatedTime = dto.CreatedTime,
                IP = dto.IP,
                Port = dto.Port,
                Score = dto.Score,
                UpdatedTime = dto.UpdatedTime
            });
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string ip, int port)
        {
            return await _context.Proxys
                .AsNoTracking()
                .Where(c => c.IP == ip && c.Port == port)
                .AnyAsync();
        }

        public async Task RemoveAsync(int id)
        {
             _context.Remove(new Proxy
            {
                Id = id
            });
            await _context.SaveChangesAsync();
        }

        public async Task UpdateScoreAsync(int id, int score)
        {
            var proxy = new Proxy
            {
                Id = id,
                Score = score
            };
            _context.Proxys.Attach(proxy);
            _context.Entry(proxy).Property(c => c.Score).IsModified = true;
            await _context.SaveChangesAsync();
        }
    }
}
