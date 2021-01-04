using ProxyPool.Core.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProxyChannel(this IServiceCollection services)
        {
            services.AddSingleton<IProxyPipeline, RedisProxyPipieline>();
            return services;
        }
    }
}
