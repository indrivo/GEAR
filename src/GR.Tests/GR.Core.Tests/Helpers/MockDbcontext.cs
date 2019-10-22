using GR.Core.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace GR.Core.Tests.Helpers
{
    public class MockDbContext : DbContext, IDbContext
    {
        public MockDbContext(DbContextOptions options)
        {

        }

        public DbSet<T> SetEntity<T>() where T : class, IBaseModel
        {
            return Set<T>();
        }
    }
}
