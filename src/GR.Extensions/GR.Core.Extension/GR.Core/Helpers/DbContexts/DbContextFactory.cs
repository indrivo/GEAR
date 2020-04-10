using System;
using System.Collections.Generic;
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
        public static TDbContext CreateFactoryDbContext(params object[] parameters)
        {
            var buildParameters = new List<object>
            {
                DbContextFactoryBuilder.BuildOptions<TBaseDbContext>()
            };

            buildParameters.AddRange(parameters);

            TDbContext context = null;

            try
            {
                context = (TDbContext)Activator.CreateInstance(typeof(TDbContext), buildParameters.ToArray());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return context;
        }
    }
}
