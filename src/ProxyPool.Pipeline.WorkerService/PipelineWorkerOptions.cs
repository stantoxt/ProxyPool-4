namespace ProxyPool.Pipeline.WorkerService
{
    public class PipelineWorkerOptions
    {
        public const string Position = "Worker:Pipeline";

        public int ThreadCount { get; set; }
    }
}
