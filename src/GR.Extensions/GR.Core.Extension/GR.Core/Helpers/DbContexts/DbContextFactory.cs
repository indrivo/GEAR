using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using GR.Core.Extensions;

namespace GR.Core.Helpers.DbContexts
{
    public static class DbContextFactoryBuilder
    {
        /// <summary>
        /// Build Options
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <returns></returns>
        public static DbContextOptions<TContext> BuildOptions<TContext>() where TContext : DbContext
        {
            var builder = new DbContextOptionsBuilder<TContext>();
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json");
            IConfiguration configurator = configBuilder.Build();

            builder.GetDefaultOptions(configurator);

            return builder.Options;
        }
    }

    public static class DbContextFactory<TDbContext, TBaseDbContext> where TBaseDbContext : DbContext where TDbContext : TBaseDbContext
    {
        public static TDbContext CreateFactoryDbContext()
        {
            var context = (TDbContext)Activator.CreateInstance(typeof(TDbContext), DbContextFactoryBuilder.BuildOptions<TBaseDbContext>());
            return context;
        }
    }
}
