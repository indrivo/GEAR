﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ST.Core.Helpers;
using ST.DynamicEntityStorage.Abstractions;
using ST.Notifications.Abstractions.Models.Notifications;

namespace ST.Notifications.Abstractions.Seeders
{
    public static class NotificationManager
    {
        /// <summary>
        /// Seed notification types
        /// </summary>
        public static async Task SeedNotificationTypesAsync()
        {
            var dataService = IoC.Resolve<IDynamicService>();
            if (dataService == null) throw new Exception("IDynamicService is not registered");
            var types = JsonParser.ReadObjectDataFromJsonFile<SeedEntity>(Path.Combine(AppContext.BaseDirectory, "NotificationTypes.json"));
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
                    var response = await dataService.AddWithReflection(item);
                    if (!response.IsSuccess)
                    {
                        Console.WriteLine("Fail to add");
                    }
                }
            }
        }

        private class SeedEntity
        {
            public List<NotificationTypes> NotificationTypes { get; set; }
        }
    }
}
