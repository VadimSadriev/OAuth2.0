using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OAuth.Client.Configuration;
using OAuth.Common.Configuration;

namespace OAuth.Client
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var authSection = Configuration.GetSection("Auth").CheckExistence();
            var authConfig = new AuthConfiguration();
            authSection.Bind(authConfig);

            services.AddAuthentication(opts =>
            {
                opts.DefaultAuthenticateScheme = "Cookie";
                opts.DefaultSignInScheme = "Cookie";
                opts.DefaultChallengeScheme = "AuthServer";
                
            })
                .AddCookie("Cookie", opts =>
                {
                })
                .AddOAuth("AuthServer", opts =>
                {
                    opts.ClientId = authConfig.ClientId;
                    opts.ClientSecret = authConfig.ClientSecret;
                    opts.CallbackPath = authConfig.CallbackPath;
                    opts.AuthorizationEndpoint = authConfig.AuthorizationEndpoint;
                    opts.TokenEndpoint = authConfig.TokenEndpoint;
                });

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
