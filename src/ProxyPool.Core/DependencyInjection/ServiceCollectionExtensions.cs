using Microsoft.Extensions.Configuration;
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
            return services;
        }

        public static IServiceCollection AddProxyCheck(this IServiceCollection services)
        {
            services.AddScoped<IProxyCheck, DefaultProxyCheck>();
            return services;
        }

        public static IServiceCollection AddProxyChannel(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IProxyPipeline, RedisProxyPipieline>();
            services.Configure<RedisProxyPipelineOptions>(configuration.GetSection(RedisProxyPipelineOptions.Position));
            return services;
        }
    }
}
