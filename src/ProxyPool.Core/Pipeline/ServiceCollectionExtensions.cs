﻿using Microsoft.Extensions.Configuration;
using ProxyPool.Core.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProxyChannel(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IProxyPipeline, RedisProxyPipieline>();
            services.Configure<RedisProxyPipelineOptions>(configuration.GetSection(RedisProxyPipelineOptions.Position));
            return services;
        }
    }
}
