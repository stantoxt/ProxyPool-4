using ProxyPool.Core.Channel;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProxyChannel(this IServiceCollection services)
        {
            services.AddScoped<IProxyChannel, RedisProxyChannel>();
            return services;
        }
    }
}
