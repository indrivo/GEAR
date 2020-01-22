using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Entities.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GR.Core;
using GR.Entities.Abstractions.Models.Tables;

namespace GR.Entities.Data
{
    public static class EntitiesDbContextSeeder<TContext> where TContext : DbContext, IEntityContext
    {
        /// <summary>
        /// Seed with default data
        /// </summary>
        public static async Task SeedAsync(TContext context, Guid tenantId)
        {
            context.Validate();
            var strategy = context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                var entity = JsonParser.ReadObjectDataFromJsonFile<SeedEntity>(Path.Combine(AppContext.BaseDirectory, "Configuration/FieldTypes.json"));
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

                var configurationModel =
                    JsonParser.ReadObjectDataFromJsonFile<SeedEntity>(Path.Combine(AppContext.BaseDirectory,
                        "Configuration/EntitiesConfiguration.json"))
                    ?? new SeedEntity();

                //Seed system entity type
                configurationModel.EntityTypes.Add(new EntityType
                {
                    Changed = DateTime.Now,
                    Created = DateTime.Now,
                    IsSystem = true,
                    Author = GlobalResources.Roles.ANONIMOUS_USER,
                    MachineName = GearSettings.DEFAULT_ENTITY_SCHEMA,
                    Name = GearSettings.DEFAULT_ENTITY_SCHEMA,
                    TenantId = GearSettings.TenantId
                });

                // Check and seed entities types
                if (configurationModel.EntityTypes.Any())
                {
                    foreach (var item in configurationModel.EntityTypes)
                    {
                        if (context.EntityTypes.Any(x => x.Name == item.Name)) continue;
                        item.TenantId = tenantId;
                        context.EntityTypes.Add(item);
                    }
                }

                var dbResult = await context.PushAsync();
                if (dbResult.IsSuccess)
                {
                    GearApplication.BackgroundTaskQueue.PushBackgroundWorkItemInQueue(async x =>
                    {
                        var entityRepository = x.InjectService<IEntityService>();

                        //Create dynamic tables for configured tenant
                        await entityRepository.CreateDynamicTablesFromInitialConfigurationsFile(GearSettings.TenantId, GearSettings.DEFAULT_ENTITY_SCHEMA);
                    });
                }
            });
        }
    }
}