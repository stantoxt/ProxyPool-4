using ProxyPool.Service.Models;
using System.Threading.Tasks;

namespace ProxyPool.Service.Abstracts
{
    public interface IProxyRandomService
    {
        Task<ProxyOutputDto> GetAsync();
    }
}
