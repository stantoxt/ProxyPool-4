using ProxyPool.Service.Models;
using System.Threading.Tasks;

namespace ProxyPool.Service.Abstracts
{
    public interface IProxyService
    {
        Task AddAsync(ProxyDto dto);

        Task RemoveAsync(int id);

        Task UpdateScoreAsync(int id, int score);

        Task<bool> ExistsAsync(string ip, int port);
    }
}
