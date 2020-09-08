using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OAuth.Common.Configuration;
using OAuth.Server.Configuration;
using OAuth.Server.Data;
using OAuth.Server.Data.Entities;
using System;
using System.Text;

namespace OAuth.Server.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection Addidentity(this IServiceCollection services)
        {
            services.AddIdentity<Account, Role>(options =>
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

        public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfigurationSection authSection)
        {
            var jwtSection = authSection.CheckExistence();

            var jwtOptions = new JwtOptions();
            jwtSection.Bind(jwtOptions);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
            };

            services.Configure<JwtOptions>(jwtSection);
            services.AddSingleton(tokenValidationParameters);
            services.AddAuthentication("OAuth")
                .AddJwtBearer("OAuth", options =>
                {
                    options.TokenValidationParameters = tokenValidationParameters;
                });

            return services;
        }
    }
}
