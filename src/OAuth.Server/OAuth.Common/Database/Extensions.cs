using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OAuth.Common.Configuration;

namespace OAuth.Common.Database
{
    public static class Extensions
    {
        public static IServiceCollection AddDb<TDatabase>(this IServiceCollection services, IConfigurationSection dbSection)
        where TDatabase : DbContext
        {
            dbSection.CheckExistence();

            services.AddDbContext<TDatabase>(config =>
            {
                config.UseSqlite(dbSection["ConnectionString"]);
            });

            return services;
        }
    }
}
