using Microsoft.EntityFrameworkCore;
using ProxyPool.Data.EntityFramework.Models;

namespace ProxyPool.Data.EntityFramework
{
    public class ProxyPoolContext : DbContext
    {
        public DbSet<Proxy> Proxys { get; set; }

        public ProxyPoolContext(DbContextOptions<ProxyPoolContext> options) : base(options)
        {
        }

    }
}
