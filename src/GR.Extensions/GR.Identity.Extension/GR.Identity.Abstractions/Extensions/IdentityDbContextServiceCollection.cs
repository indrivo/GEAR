using GR.Core.Helpers.ConnectionStrings;
using GR.Identity.Abstractions.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace GR.Identity.Abstractions.Extensions
{
    public static class IdentityDbContextServiceCollection
    {
        public static DbContextOptionsBuilder RegisterIdentityStorage(this DbContextOptionsBuilder builder, IConfiguration configuration, string migrationsAssembly)
        {
            var (providerType, connectionString) = DbUtil.GetConnectionString(configuration);
            switch (providerType)
            {
                case DbProviderType.MsSqlServer:
                    builder.UseSqlServer(connectionString, opts =>
                    {
                        opts.MigrationsAssembly(migrationsAssembly);
                        opts.MigrationsHistoryTable("IdentityServerConfigurationMigrationHistory",
                            IdentityConfig.DEFAULT_SCHEMA);
                    });
                    break;

                case DbProviderType.PostgreSql:
                    builder.UseNpgsql(connectionString, opts =>
                    {
                        opts.MigrationsAssembly(migrationsAssembly);
                        opts.MigrationsHistoryTable("IdentityServerConfigurationMigrationHistory",
                            IdentityConfig.DEFAULT_SCHEMA);
                    });
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            return builder;
        }
    }
}