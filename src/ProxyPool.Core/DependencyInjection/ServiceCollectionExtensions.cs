using Microsoft.Extensions.Configuration;
using ProxyPool.Core;
using ProxyPool.Core.Net;
using ProxyPool.Core.Pipeline;
using ProxyPool.Core.Redis;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProxyPoolCore(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RedisOptions>(configuration.GetSection(RedisOptions.Position));
            services.AddSingleton<IRedisClientFactory, RedisClientFactory>();
            services.AddScoped<IProxySocket, DefaultProxySocket>();
            services.AddSingleton<IProxyPipeline, RedisProxyPipieline>();
            services.AddSingleton<IRandomPool, RedisRandomPool>();
            return services;
        }
    }
}
