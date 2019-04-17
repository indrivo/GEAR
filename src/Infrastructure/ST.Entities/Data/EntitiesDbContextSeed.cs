using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ST.Entities.Models.Forms;
using ST.Entities.Models.Tables;
using ST.Entities.ViewModels.Table;

namespace ST.Entities.Data
{
    public class EntitiesDbContextSeed
    {
        public static SeedEntity ReadData(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            try
            {
                SeedEntity entity;

                using (var str = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var sReader = new StreamReader(str))
                using (var reader = new JsonTextReader(sReader))
                {
                    var fileObj = JObject.Load(reader);
                    entity = fileObj.ToObject<SeedEntity>();
                }

                return entity;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            return null;
        }
        /// <summary>
        /// Seed with default data
        /// </summary>
        public static async Task SeedAsync(EntitiesDbContext context, IConfiguration configuration, Guid tenantId, int retry = 0)
        {
            var strategy = context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                var entity = ReadData(Path.Combine(AppContext.BaseDirectory, "FieldTypes.json"));
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

                var configurationModel = ReadData(Path.Combine(AppContext.BaseDirectory, "EntitiesConfiguration.json"));
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
                await SeedFormTypesAsync(context, tenantId);
            });
        }
        /// <summary>
        /// Seed form types
        /// </summary>
        /// <returns></returns>
        private static async Task SeedFormTypesAsync(EntitiesDbContext context, Guid tenantId)
        {
            var formTypes = ReadData(Path.Combine(AppContext.BaseDirectory, "FormTypes.json"));
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

        public class SeedEntity
        {
            public List<EntityType> EntityTypes { get; set; }
            public List<SynchronizeTableViewModel> SynchronizeTableViewModels { get; set; }
            public List<TableFieldGroups> TableFieldGroups { get; set; }
            public List<FormType> FormTypes { get; set; }
        }
    }
}