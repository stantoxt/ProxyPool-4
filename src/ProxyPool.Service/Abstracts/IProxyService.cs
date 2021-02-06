using ProxyPool.Service.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProxyPool.Service.Abstracts
{
    public interface IProxyService
    {
        Task AddAsync(ProxyDto dto);

        Task<bool> ExistsAsync(string ip, int port);

        Task<(ICollection<ProxyDto>, int nextLastId)> GetPagedProxys(int count, int lastId = 0);
    }
}
