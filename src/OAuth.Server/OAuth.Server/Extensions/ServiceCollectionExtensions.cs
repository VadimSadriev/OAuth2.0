using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using OAuth.Server.Data;
using OAuth.Server.Data.Entities;

namespace OAuth.Server.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection Addidentity(this IServiceCollection services)
        {
            services.AddIdentity<Account, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
               .AddEntityFrameworkStores<DataContext>()
               .AddDefaultTokenProviders();

            return services;
        }
    }
}
