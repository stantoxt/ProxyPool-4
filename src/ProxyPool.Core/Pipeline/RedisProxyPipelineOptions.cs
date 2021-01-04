namespace ProxyPool.Core.Pipeline
{
    internal class RedisProxyPipelineOptions
    {
        public const string Position = "Pipeline:Redis";

        public string ConnectionString { get; set; }
        public string PipelineName { get; set; }
    }
}
