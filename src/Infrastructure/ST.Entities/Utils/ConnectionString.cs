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
                dbType = DbProviderType.PostgreSQL;
            }
            else
            {
                connectionString = configuration.GetConnectionString("DevelopmentConnection");
                if (env.IsProduction())
                {
                    connectionString = configuration.GetConnectionString("ProductionConnection");
                }

                if (env.IsEnvironment("Stage"))
                {
                    connectionString = configuration.GetConnectionString("StageConnection");
                }
                dbType = DbProviderType.MsSqlServer;
            }

            return (dbType, connectionString);
        }
    }
    public enum DbProviderType
    {
        MsSqlServer, PostgreSQL
    }
}
