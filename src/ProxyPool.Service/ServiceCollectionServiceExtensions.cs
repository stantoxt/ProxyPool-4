using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProxyPool.Data.EntityFramework;
using ProxyPool.Service.Abstracts;
using ProxyPool.Service.Implementations;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionServiceExtensions
    {
        public static IServiceCollection AddProxyPoolService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddProxyPoolCore(configuration);
            services.AddScoped<IProxyService, ProxyService>();
            services.AddScoped<IProxyCheckService, ProxyCheckService>();
            services.AddScoped<IProxyRandomService, ProxyRandomService>();
            services.AddDbContext<ProxyPoolContext>(options =>
            {
                options.EnableSensitiveDataLogging(true);
                options.UseMySql(configuration.GetConnectionString("ProxyPoolContext"), builder=>
                {
                    builder.EnableRetryOnFailure(
                         maxRetryCount: 3
                        );
                });
            });
            return services;
        }
    }
}
