namespace ProxyPool.Pipeline.WorkerService
{
    public class ProxyCheckConsumeWorkOptions
    {
        public const string Position = "Work:ProxyCheckConsume";

        public int ThreadCount { get; set; }
    }
}
