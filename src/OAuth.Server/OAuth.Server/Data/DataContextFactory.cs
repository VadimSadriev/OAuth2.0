using Microsoft.EntityFrameworkCore;
using OAuth.Common.Database;
using System.IO;

namespace OAuth.Server.Data
{
    public class DataContextFactory : DesignTimeDbContextFactoryBase<DataContext>
    { 
        protected override string BasePath => Directory.GetCurrentDirectory();

        protected override DataContext CreateContext(DbContextOptions<DataContext> options)
        {
            return new DataContext(options);
        }
    }
}
