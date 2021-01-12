using System.Threading.Tasks;

namespace ProxyPool.Service.Abstracts
{
    public interface IProxyCheckService
    {
        Task PublishAsync();

        Task ConsumeAsync();
    }
}
