using Microsoft.AspNetCore.Mvc;
using ProxyPool.Service.Abstracts;
using ProxyPool.Service.Models;
using System.Threading.Tasks;

namespace ProxyPool.HttpApi.Controllers
{
    [ApiController]
    [Route("api/proxy/")]
    public class ProxyRandomController : ControllerBase
    {
        private readonly IProxyRandomService _randomService;

        public ProxyRandomController(IProxyRandomService randomService)
        {
            _randomService = randomService;
        }

        [HttpGet]
        [Route("random")]
        public async Task<ProxyOutputDto> RandomAsync()
        {
            return await _randomService.GetAsync();
        }
    }
}
