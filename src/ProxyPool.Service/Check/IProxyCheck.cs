using ProxyPool.Service.Models;
using System.Threading.Tasks;

namespace ProxyPool.Service.Check
{
    public interface IProxyCheck
    {
        Task Check(ProxyDto dto);
    }
}
