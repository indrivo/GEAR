using System.Collections.Generic;
using System.Linq;
using ST.Identity.Abstractions;
using ST.Notifications.Abstractions.Models.Data;

namespace ST.Notifications.Razor.ViewModels.Subscriptions
{
    public class NotificationSubscribeGetViewModel : NotificationEvent
    {
        /// <summary>
        /// Template
        /// </summary>
        public string Template { get; set; }

        /// <summary>
        /// Subscribed roles
        /// </summary>
        public IEnumerable<ApplicationRole> SubscribedRoles { get; set; }

        /// <summary>
        /// Is role subscribed
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public bool IsRoleSubscribed(ApplicationRole role)
        {
            return SubscribedRoles?.Select(x => x.Name).Contains(role?.Name) ?? false;
        }
    }
}
