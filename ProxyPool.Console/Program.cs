using ProxyPool.Spiders;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProxyPool.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            XiLaDaiLiSpider spider = new XiLaDaiLiSpider();
            await foreach(var proxyInfo in spider.GetProxyInfos())
            {
                Console.WriteLine($"IP:{proxyInfo.IP}\tPort:{proxyInfo.Port}\tAnonymousDegree:{proxyInfo.AnonymousDegree}");
            }

            Console.ReadLine();
        }
    }
}
