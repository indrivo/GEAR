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
        /// <param name="configuration"></param>
        /// <param name="hostingEnvironment"></param>
        /// <returns></returns>
        public static DbContextOptionsBuilder GetDefaultOptions(this DbContextOptionsBuilder options, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            var connectionString = ConnectionString.Get(configuration, hostingEnvironment);
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
