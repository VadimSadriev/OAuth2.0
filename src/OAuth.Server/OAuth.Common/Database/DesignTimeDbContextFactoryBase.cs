using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;

namespace OAuth.Common.Database
{
    public abstract class DesignTimeDbContextFactoryBase<TContext> : IDesignTimeDbContextFactory<TContext> where TContext : DbContext
    {
        protected abstract string BasePath { get; }

        private string AspNetCoreEnvironment = "Development";

        protected abstract TContext CreateContext(DbContextOptions<TContext> options);

        /// <summary> Creates new <see cref="TContext"/> </summary>
        public TContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(BasePath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{AspNetCoreEnvironment}.json", optional: true)
                .Build();

            var connectionString = configuration["Database:ConnectionString"];

            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("Connection string is empty");

            Console.WriteLine($"Environment: {AspNetCoreEnvironment}");
            Console.WriteLine($"Creating {typeof(TContext).Name}. Connection string: {connectionString}");

            var optionsBuilder = new DbContextOptionsBuilder<TContext>();

            optionsBuilder.UseSqlite(connectionString);

            return CreateContext(optionsBuilder.Options);
        }
    }
}
