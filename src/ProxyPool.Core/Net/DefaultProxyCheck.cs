using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ProxyPool.Core.Net
{
    internal class DefaultProxyCheck : IProxyCheck
    {
        public async Task<ProxyStatus> ConnectAsync(string host, int port, int timeout)
        {
            using TcpClient client = new TcpClient();
            var watch = Stopwatch.StartNew();
            try
            {
                await client.ConnectAsync(host, port);
            }
            catch
            {
            }
            finally
            {
                watch.Stop();
            }

            var elapsedMilliseconds = client.Connected ? watch.ElapsedMilliseconds : 0;
            return new ProxyStatus(client.Connected, elapsedMilliseconds);
        }
    }
}
