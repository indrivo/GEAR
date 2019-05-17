using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ST.Core.Extensions;
using ST.Core.Helpers;
using ST.Entities.Abstractions;

namespace ST.Entities.Data
{
    public static class EntitiesDbContextSeed<TContext> where  TContext : DbContext, IEntityContext
    {
        /// <summary>
        /// Seed with default data
        /// </summary>
        public static async Task SeedAsync(TContext context, Guid tenantId, int retry = 0)
        {
            context.Validate();
            var strategy = context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                var entity = JsonParser.ReadObjectDataFromJsonFile<SeedEntity>(Path.Combine(AppContext.BaseDirectory, "FieldTypes.json"));
                if (entity == null)
                    return;

                // Check and seed field types
                if (entity.TableFieldGroups.Any())
                {
                    if (!context.TableFieldGroups.Any())
                    {
                        context.TableFieldGroups.AddRange(entity.TableFieldGroups);
                        try
                        {
                            await context.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                    }
                }

                var configurationModel = JsonParser.ReadObjectDataFromJsonFile<SeedEntity>(Path.Combine(AppContext.BaseDirectory, "EntitiesConfiguration.json"));
                if (configurationModel == null)
                    return;

                // Check and seed entities types
                if (configurationModel.EntityTypes.Any())
                {
                    foreach (var item in configurationModel.EntityTypes)
                    {
                        if (context.EntityTypes.Any(x => x.Name == item.Name)) continue;
                        item.TenantId = tenantId;
                        context.EntityTypes.Add(item);
                        await context.SaveChangesAsync();
                    }
                }
            });
        }
    }
}