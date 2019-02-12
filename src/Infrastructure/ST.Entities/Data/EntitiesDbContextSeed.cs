using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ST.Entities.Models.Forms;
using ST.Entities.Models.Notifications;
using ST.Entities.Models.Tables;
using ST.Entities.Services.Abstraction;
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
        public static async Task SeedAsync(EntitiesDbContext context, IConfiguration configuration, int retry = 0)
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
                        await context.SaveChangesAsync();
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
                        context.EntityTypes.Add(item);
                        await context.SaveChangesAsync();
                    }
                }
                await SeedFormTypesAsync(context);
            });
        }
        /// <summary>
        /// Seed form types
        /// </summary>
        /// <returns></returns>
        private static async Task SeedFormTypesAsync(EntitiesDbContext context)
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
                    context.FormTypes.Add(item);
                    await context.SaveChangesAsync();
                }
            }
        }
        /// <summary>
        /// Seed notification types
        /// </summary>
        public static async Task SeedNotificationTypesAsync(IDynamicEntityDataService dataService)
        {
            var types = ReadData(Path.Combine(AppContext.BaseDirectory, "NotificationTypes.json"));
            if (types == null)
                return;

            if (types.NotificationTypes.Any())
            {
                foreach (var item in types.NotificationTypes)
                {
                    var exist = await dataService.GetAll<NotificationTypes>(x => x["Name"].Equals(item.Name));
                    if (exist.Result.Any()) continue;
                    item.Author = "admin";
                    item.ModifiedBy = "admin";
                    var response = await dataService.AddSystem(item);
                    if (!response.IsSuccess)
                    {
                        Console.WriteLine("Fail to add");
                    }
                }
            }
        }

        public class SeedEntity
        {
            public List<EntityType> EntityTypes { get; set; }
            public List<SynchronizeTableViewModel> SynchronizeTableViewModels { get; set; }
            public List<TableFieldGroups> TableFieldGroups { get; set; }
            public List<FormType> FormTypes { get; set; }
            public List<NotificationTypes> NotificationTypes { get; set; }
        }
    }
}