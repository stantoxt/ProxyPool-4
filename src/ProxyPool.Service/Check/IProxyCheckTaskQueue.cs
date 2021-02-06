using ProxyPool.Service.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProxyPool.Service.Check
{
    public interface IProxyCheckTaskQueue
    {
        Task EnqueueTaskItemAsync(ProxyDto dto);
        Task EnqueueTaskItemAsync(ICollection<ProxyDto> dtos);
        Task<ProxyDto> DequeueAsync();
    }
}
