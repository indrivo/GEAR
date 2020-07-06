using System;
using System.Collections.Generic;
using System.Linq;
using GR.Notifications.Abstractions.Models.Notifications;

namespace GR.Notifications.Dynamic.Helpers
{
    public class NotificationsDictionaryConvertor
    {
        public static IEnumerable<Notification> Convert(IEnumerable<Dictionary<string, object>> collection)
        {
            foreach (var item in collection)
            {
                yield return new Notification
                {
                    Id = (Guid)item.FirstOrDefault(x => x.Key == nameof(Notification.Id)).Value,
                    Author = (string)item.FirstOrDefault(x => x.Key == nameof(Notification.Author)).Value,
                    IsDeleted = (bool)item.FirstOrDefault(x => x.Key == nameof(Notification.IsDeleted)).Value,
                    UserId = (Guid)item.FirstOrDefault(x => x.Key == nameof(Notification.UserId)).Value,
                    Created = (DateTime)item.FirstOrDefault(x => x.Key == nameof(Notification.Created)).Value,
                    Changed = (DateTime)item.FirstOrDefault(x => x.Key == nameof(Notification.Changed)).Value,
                    Content = (string)item.FirstOrDefault(x => x.Key == nameof(Notification.Content)).Value,
                    ModifiedBy = (string)item.FirstOrDefault(x => x.Key == nameof(Notification.ModifiedBy)).Value,
                    NotificationTypeId = (Guid)item.FirstOrDefault(x => x.Key == nameof(Notification.NotificationTypeId)).Value,
                    Subject = (string)item.FirstOrDefault(x => x.Key == nameof(Notification.Subject)).Value
                };
            }
        }
    }
}
