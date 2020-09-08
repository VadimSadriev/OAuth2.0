using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OAuth.Common.Configuration;
using OAuth.Common.Database;
using OAuth.Server.Configuration;
using OAuth.Server.Data;
using OAuth.Server.Extensions;
using OAuth.Server.Middlewares;
using System.Text.Json.Serialization;

namespace OAuth.Server
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
            services.Configure<AuthConfiguration>(authSection);

            services.AddDb<DataContext>(Configuration.GetSection("Database"));
            services.Addidentity();
            services.AddApplication();
            services.AddDistributedMemoryCache();
            services.AddAuthentication(Configuration.GetSection("Auth:Jwt"));
            services.AddControllersWithViews()
                .AddJsonOptions(opts => opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
                .AddRazorRuntimeCompilation();
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

            app.UseRouting();

            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseAuthorization();

            app.UseErrorMiddleware();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
