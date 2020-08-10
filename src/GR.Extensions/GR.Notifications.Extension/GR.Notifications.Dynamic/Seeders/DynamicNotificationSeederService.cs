using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.DynamicEntityStorage.Abstractions;
using GR.Notifications.Abstractions;
using GR.Notifications.Abstractions.Models.Notifications;

namespace GR.Notifications.Dynamic.Seeders
{
    public class DynamicNotificationSeederService : INotificationSeederService
    {
        #region Injectable

        /// <summary>
        /// Inject dynamic service
        /// </summary>
        private readonly IDynamicService _dynamicService;

        #endregion

        public DynamicNotificationSeederService(IDynamicService dynamicService)
        {
            _dynamicService = dynamicService;
        }

        /// <summary>
        /// Seed notification types
        /// </summary>
        public async Task SeedNotificationTypesAsync()
        {
            var types = JsonParser.ReadObjectDataFromJsonFile<SeedEntity>(Path.Combine(AppContext.BaseDirectory, "Configuration/NotificationTypes.json"));
            if (types == null)
                return;

            if (types.NotificationTypes.Any())
            {
                foreach (var item in types.NotificationTypes)
                {
                    var exist = await _dynamicService.GetAllAsync(nameof(NotificationTypes), x => x["Name"].Equals(item.Name));
                    if (exist.Result?.Any() ?? false) continue;
                    item.Author = "admin";
                    item.ModifiedBy = "admin";
                    var dict = item.ToDictionary();
                    var response = await _dynamicService.AddAsync(nameof(NotificationTypes), new Dictionary<string, object>(dict));
                    if (!response.IsSuccess)
                    {
                        Console.WriteLine("Fail to add");
                    }
                }
            }
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private sealed class SeedEntity
        {
            // ReSharper disable once CollectionNeverUpdated.Local
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public List<NotificationTypes> NotificationTypes { get; set; }
        }
    }
}
