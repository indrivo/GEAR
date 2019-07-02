using System;
using Microsoft.EntityFrameworkCore;

namespace ST.Core.Helpers.DbContexts
{
    public static class DbContextFactory<TDbContext, TBaseDbContext> where TBaseDbContext : DbContext where TDbContext : TBaseDbContext
    {
        /// <summary>
        /// Options
        /// </summary>
        public static DbContextOptionsBuilder<TBaseDbContext> Options { get; set; } = new DbContextOptionsBuilder<TBaseDbContext>();

        public static TDbContext CreateFactoryDbContext()
        {
            var context = (TDbContext)Activator.CreateInstance(typeof(TDbContext), Options);
            return context;
        }
    }
}
