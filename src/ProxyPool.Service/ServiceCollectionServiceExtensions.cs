using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProxyPool.Data.EntityFramework;
using ProxyPool.Service;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionServiceExtensions
    {
        public static IServiceCollection AddProxyPoolService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddProxyCheck();
            services.AddScoped<IProxyService, ProxyService>();
            services.AddScoped<IProxyCheckService, ProxyCheckService>();
            services.AddDbContext<ProxyPoolContext>(options =>
            {
                options.EnableSensitiveDataLogging(true);
                options.UseMySql(configuration.GetConnectionString("ProxyPoolContext"));
            });
            return services;
        }
    }
}
