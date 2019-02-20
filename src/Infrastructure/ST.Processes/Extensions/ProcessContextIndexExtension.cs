using Microsoft.EntityFrameworkCore;
using ST.Procesess.Models;

namespace ST.Procesess.Extensions
{
    public static class ProcessContextIndexExtension
    {
        /// <summary>
        /// Register process context indexes
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ModelBuilder RegisterIndexes(this ModelBuilder builder)
        {
            builder.Entity<STProcessSchema>()
                .HasIndex(x => x.TenantId);

            builder.Entity<STProcess>()
                .HasIndex(x => x.TenantId);

            builder.Entity<STProcessInstance>()
               .HasIndex(x => x.TenantId);

            builder.Entity<STTransitionActor>()
                .HasIndex(x => x.TenantId);

            builder.Entity<STProcessTransition>()
               .HasIndex(x => x.TenantId);

            builder.Entity<STProcessTask>()
               .HasIndex(x => x.TenantId);

            builder.Entity<UserProcessTasks>()
                .HasIndex(x => x.TenantId);

            return builder;
        }
    }
}
