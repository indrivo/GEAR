using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ST.DynamicEntityStorage.Abstractions;
using ST.Entities.Extensions;
using ST.Notifications.Abstractions.Models.Notifications;

namespace ST.Configuration.Seed
{
    public static class NotificationManager
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
        /// Seed notification types
        /// </summary>
        public static async Task SeedNotificationTypesAsync()
        {
            var dataService = IoC.Resolve<IDynamicService>();
            if (dataService == null) throw new Exception("IDynamicService is not registered");
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
            public List<NotificationTypes> NotificationTypes { get; set; }
        }
    }
}
