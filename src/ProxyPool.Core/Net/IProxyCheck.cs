using System.Threading.Tasks;

namespace ProxyPool.Core.Net
{
    public interface IProxyCheck
    {
        Task<ProxyStatus> ConnectAsync(string host, int port, int timeout);
    }
}
