using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GR.Core.Helpers.ConnectionStrings
{
    public static class DbUtil
    {
        /// <summary>
        /// GetConnectionString connection string
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static (DbProviderType, string) GetConnectionString(IConfiguration configuration)
        {
            var settings = configuration.GetSection("ConnectionStrings");
            var provider = settings.GetValue<string>("Provider");
            var connectionString = settings.GetValue<string>("ConnectionString");
            var providerType = GetProviderType(provider);
            return (providerType, connectionString);
        }

        /// <summary>
        /// Get connection string
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetConnectionString<TContext>(this TContext context) where TContext : DbContext
        {
            return context.Database.GetDbConnection().ConnectionString;
        }

        /// <summary>
        /// Return provider type
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static DbProviderType GetProviderType(string provider)
        {
            var result = DbProviderType.PostgreSql;
            switch (provider)
            {
                case DbProvider.PostgreSQL:
                    {
                        result = DbProviderType.PostgreSql;
                    }
                    break;

                case DbProvider.SqlServer:
                    {
                        result = DbProviderType.MsSqlServer;
                    }
                    break;

            }

            return result;
        }

        /// <summary>
        /// Get provider type
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static DbProviderType GetProviderType<TContext>(this TContext context) where TContext : DbContext
        {
            var result = DbProviderType.PostgreSql;
            var provider = context.Database.ProviderName;
            switch (provider)
            {
                case DbProvider.PostgreSQL:
                    result = DbProviderType.PostgreSql;
                    break;
                case DbProvider.SqlServer:
                    result = DbProviderType.MsSqlServer;
                    break;
            }

            return result;
        }
    }
}
