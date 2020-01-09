using System;
using System.Threading.Tasks;
using GR.Audit.Contexts;
using Microsoft.EntityFrameworkCore;

namespace GR.Core.Tests.Helpers
{
    public class MockDbContext : TrackerDbContext
    {
        public MockDbContext(DbContextOptions<MockDbContext> options) : base(options)
        {

        }

        /// <summary>
        /// Seed data
        /// </summary>
        /// <returns></returns>
        public override Task InvokeSeedAsync(IServiceProvider services)
        {
            return Task.CompletedTask;
        }
    }
}
