﻿using Microsoft.Extensions.Configuration;
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
            services.AddScoped<IProxyCheck, DefaultProxyCheck>();
            services.AddSingleton<IProxyPipeline, RedisProxyPipieline>();
            return services;
        }
    }
}
