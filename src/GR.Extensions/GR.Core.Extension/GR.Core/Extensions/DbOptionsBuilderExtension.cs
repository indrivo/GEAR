using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using GR.Core.Helpers.ConnectionStrings;

namespace GR.Core.Extensions
{
    public static class DbOptionsBuilderExtension
    {
        /// <summary>
        /// Get default options
        /// </summary>
        /// <param name="options"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static DbContextOptionsBuilder GetDefaultOptions(this DbContextOptionsBuilder options, IConfiguration configuration)
        {
            var (dbType, connection) = DbUtil.GetConnectionString(configuration);

            switch (dbType)
            {
                case DbProviderType.MsSqlServer:
                    {
                        options.UseSqlServer(connection, sqlOptions =>
                        {
                            sqlOptions.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
                        });
                    }
                    break;
                case DbProviderType.PostgreSql:
                    {
                        options.UseNpgsql(connection, sqlOptions =>
                        {
                            sqlOptions.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
                        });
                    }
                    break;
            }

            return options;
        }
    }
}
