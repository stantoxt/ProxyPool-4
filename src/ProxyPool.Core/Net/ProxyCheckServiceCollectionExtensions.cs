using ProxyPool.Core.Net;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ProxyCheckServiceCollectionExtensions
    {
        public static IServiceCollection AddProxyCheck(this IServiceCollection services)
        {
            services.AddScoped<IProxyCheck, DefaultProxyCheck>();
            return services;
        }
    }
}
