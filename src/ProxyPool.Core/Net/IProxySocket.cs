using System.Threading.Tasks;

namespace ProxyPool.Core.Net
{
    public interface IProxySocket
    {
        Task<ProxyStatus> ConnectAsync(string host, int port, int timeout);
    }
}
