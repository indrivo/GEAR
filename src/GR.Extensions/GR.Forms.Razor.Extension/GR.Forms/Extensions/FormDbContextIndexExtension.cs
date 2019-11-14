using Microsoft.EntityFrameworkCore;
using GR.Forms.Abstractions.Models.FormModels;

namespace GR.Forms.Extensions
{
    internal static class FormDbContextIndexExtension
    {
        /// <summary>
        /// Register indexes of EntitiesDbContext
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ModelBuilder RegisterIndexes(this ModelBuilder builder)
        {
            builder.Entity<Form>()
                .HasIndex(x => x.TenantId);

            return builder;
        }
    }
}
