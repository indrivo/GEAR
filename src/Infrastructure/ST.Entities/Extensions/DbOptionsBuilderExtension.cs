using Microsoft.EntityFrameworkCore;
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
        public static DbContextOptionsBuilder GetDefaultOptions(this DbContextOptionsBuilder options, (DbProviderType, string) connectionString)
        {
            var (dbType, connection) = connectionString;
            if (dbType == DbProviderType.PostgreSQL)
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
