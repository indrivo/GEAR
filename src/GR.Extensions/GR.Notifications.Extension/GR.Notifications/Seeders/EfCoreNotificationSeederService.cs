using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Notifications.Abstractions;
using GR.Notifications.Abstractions.Models.Notifications;
using Microsoft.EntityFrameworkCore;

namespace GR.Notifications.EFCore.Seeders
{
    public class EfCoreNotificationSeederService : INotificationSeederService
    {
        #region Injectable

        /// <summary>
        /// Inject notifications context
        /// </summary>
        private readonly INotificationsContext _context;

        #endregion


        public EfCoreNotificationSeederService(INotificationsContext context)
        {
            _context = context;
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
                    var exist = await _context.NotificationTypes.AnyAsync(x => x.Name.Equals(item.Name));
                    if (exist) continue;
                    item.Author = "admin";
                    item.ModifiedBy = "admin";
                    await _context.NotificationTypes.AddAsync(item);
                    var response = await _context.PushAsync();
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
