using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Forms.Abstractions;
using GR.Forms.Abstractions.Models.FormModels;

namespace GR.Forms.Data
{
    public static class FormDbContextSeeder<TContext> where TContext : DbContext, IFormContext
    {
        /// <summary>
        /// Seed async
        /// </summary>
        /// <param name="context"></param>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public static async Task SeedAsync(TContext context, Guid tenantId)
        {
            var strategy = context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () => { await SeedFormTypesAsync(context, tenantId); });
        }

        /// <summary>
        /// Seed form types
        /// </summary>
        /// <returns></returns>
        private static async Task SeedFormTypesAsync(TContext context, Guid tenantId)
        {
            context.Validate();
            var formTypes = JsonParser.ReadObjectDataFromJsonFile<SeedFormData>(Path.Combine(AppContext.BaseDirectory, "Configuration/FormTypes.json"));
            if (formTypes == null)
                return;

            // Check and seed form types
            if (formTypes.FormTypes.Any())
            {
                foreach (var item in formTypes.FormTypes)
                {
                    if (context.FormTypes.Any(x => x.Name == item.Name)) continue;
                    item.TenantId = tenantId;
                    context.FormTypes.Add(item);
                    await context.SaveChangesAsync();
                }
            }
        }

        private sealed class SeedFormData
        {
            public List<FormType> FormTypes { get; set; }
        }
    }
}
