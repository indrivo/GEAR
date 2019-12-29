using System.Collections.Generic;
using System.Linq;
using GR.Identity.Abstractions;
using GR.Notifications.Abstractions.Models.Data;

namespace GR.Notifications.Razor.ViewModels.Subscriptions
{
    public class NotificationSubscribeGetViewModel : NotificationEvent
    {
        /// <summary>
        /// Subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Template
        /// </summary>
        public string Template { get; set; }

        /// <summary>
        /// Subscribed roles
        /// </summary>
        public IEnumerable<GearRole> SubscribedRoles { get; set; }

        /// <summary>
        /// Is role subscribed
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public bool IsRoleSubscribed(GearRole role)
        {
            return SubscribedRoles?.Select(x => x.Name).Contains(role?.Name) ?? false;
        }
    }
}
