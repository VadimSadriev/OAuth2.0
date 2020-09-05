using Microsoft.Extensions.DependencyInjection;
using OAuth.Server.Services;

namespace OAuth.Server.Extensions
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
