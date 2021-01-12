using System.Threading.Tasks;
using System.Collections.Generic;

namespace ProxyPool.Core
{
    public interface IRandomPool
    {
        Task<(string host, int port)> NextAsync();

        Task AddAsync(string host, int port);

        Task AddAsync(IEnumerable<(string host, int port)> ts);

        Task RemoveAsync(string host, int port);

        Task RemoveAsync(IEnumerable<(string host, int port)> ts);
    }
}
