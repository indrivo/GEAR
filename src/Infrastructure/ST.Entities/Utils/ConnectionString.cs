using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace ST.Entities.Utils
{
    public static class ConnectionString
    {
        /// <summary>
        /// Get connection string
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public static (DbProviderType, string) Get(IConfiguration configuration, IHostingEnvironment env)
        {
            var dbType = DbProviderType.MsSqlServer;
            var postgreSettings = configuration.GetSection("ConnectionStrings")
                    .GetSection("PostgreSQL");
            var isPostgreSQL = postgreSettings.GetValue<bool>("UsePostgreSQL");
            string connectionString = "";
            if (isPostgreSQL)
            {
                connectionString = postgreSettings
                    .GetValue<string>("ConnectionString");
                dbType = DbProviderType.PostgreSql;
            }
            else
            {
                connectionString = configuration.GetConnectionString("MSSQLConnection");
                dbType = DbProviderType.MsSqlServer;
            }

            return (dbType, connectionString);
        }
    }
    public enum DbProviderType
    {
        MsSqlServer, PostgreSql
    }
}
