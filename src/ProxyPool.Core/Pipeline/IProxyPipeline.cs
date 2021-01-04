using System.Threading.Tasks;
using System.Collections.Generic;

namespace ProxyPool.Core.Pipeline
{
    public interface IProxyPipeline
    {
        Task AddAsync(ProxyInfo proxy);
        Task AddAsync(IEnumerable<ProxyInfo> list);
        Task<ProxyInfo> TakeAsync();
    }
}
