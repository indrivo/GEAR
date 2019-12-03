using System.Threading.Tasks;
using GR.Core.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace GR.Core.Tests.Helpers
{
    public class MockDbContext : DbContext, IDbContext
    {
        public MockDbContext(DbContextOptions options)
        {

        }

        public DbSet<T> SetEntity<T>() where T : class, IBaseModel => Set<T>();

        /// <summary>
        /// Seed data
        /// </summary>
        /// <returns></returns>
        public virtual Task InvokeSeedAsync()
        {
            return Task.CompletedTask;
        }
    }
}
