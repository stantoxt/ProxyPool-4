namespace ProxyPool.Core.Net
{
    public class ProxyStatus
    {
        public bool Connected { get; }

        public long ElapsedMilliseconds { get; }

        public ProxyStatus(bool connected, long elapsedMilliseconds)
        {
            Connected = connected;
            ElapsedMilliseconds = elapsedMilliseconds;
        }
    }
}
