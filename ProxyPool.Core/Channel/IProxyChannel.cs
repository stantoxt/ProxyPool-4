using System.Threading.Tasks;
using System.Collections.Generic;

namespace ProxyPool.Core.Channel
{
    public interface IProxyChannel
    {
        Task AddAsync(ProxyInfo proxy);
        Task AddAsync(IEnumerable<ProxyInfo> list);
        Task<ProxyInfo> TakeAsync();
    }
}
