using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ST.Entities.Utils;

namespace ST.Entities.Extensions
{
    public static class DbOptionsBuilderExtension
    {
        /// <summary>
        /// Get default options
        /// </summary>
        /// <param name="options"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static DbContextOptionsBuilder GetDefaultOptions(this DbContextOptionsBuilder options, IConfiguration Configuration, IHostingEnvironment HostingEnvironment)
        {
            var connectionString = ConnectionString.Get(Configuration, HostingEnvironment);
            var (dbType, connection) = connectionString;
            if (dbType == DbProviderType.PostgreSql)
            {
                options.UseNpgsql(connection);
            }
            else
            {
                options.UseSqlServer(connection);
            }
            return options;
        }
    }
}
