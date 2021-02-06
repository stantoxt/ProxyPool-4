using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProxyPool.Data.EntityFramework;
using ProxyPool.Service.Abstracts;
using ProxyPool.Service.Check;
using ProxyPool.Service.Implementations;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionServiceExtensions
    {
        public static IServiceCollection AddProxyPoolService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddProxyPoolCore(configuration);
            services.AddScoped<IProxyService, ProxyService>();
            services.AddScoped<IProxyRandomService, ProxyRandomService>();
            services.AddScoped<IProxyCheck, ProxyCheck>();
            services.AddSingleton<IProxyCheckTaskQueue, ProxyCheckTaskRedisQueue>();
            services.AddDbContext<ProxyPoolContext>(options =>
            {
#if DEBUG
                options.EnableSensitiveDataLogging(true);
#endif
                options.UseMySql(configuration.GetConnectionString("ProxyPoolContext"), builder =>
                {
                    builder.EnableRetryOnFailure(maxRetryCount: 3);
                });
            });
            return services;
        }
    }
}
