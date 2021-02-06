using Microsoft.EntityFrameworkCore;
using ProxyPool.Data.EntityFramework;
using ProxyPool.Data.EntityFramework.Models;
using ProxyPool.Service.Abstracts;
using ProxyPool.Service.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProxyPool.Service.Implementations
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

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public async Task<bool> ExistsAsync(string ip, int port)
        {
            return await _context.Proxys
                .AsNoTracking()
                .Where(c => c.IP == ip && c.Port == port)
                .AnyAsync();
        }

        public async Task<(ICollection<ProxyDto>, int nextLastId)> GetPagedProxys(int count, int lastId = 0)
        {
            var query = _context.Proxys.AsNoTracking();
            var records = await query
                                 .Where(c => c.Id > lastId)
                                 .OrderBy(c => c.Id)
                                 .Take(count)
                                 .ToListAsync();

            return (records.Select(c=> Map(c)).ToList(), records?.LastOrDefault()?.Id ?? 0);
        }

        ProxyDto Map(Proxy proxy)
        {
            return new ProxyDto
            {
                Id = proxy.Id,
                AnonymousDegree = proxy.AnonymousDegree,
                CreatedTime = proxy.CreatedTime,
                IP = proxy.IP,
                Port = proxy.Port,
                Score = proxy.Score,
                UpdatedTime = proxy.UpdatedTime
            };
        }
    }
}
