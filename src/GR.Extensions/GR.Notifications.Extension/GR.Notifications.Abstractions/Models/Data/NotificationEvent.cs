using System.ComponentModel.DataAnnotations;
using GR.Audit.Abstractions.Attributes;
using GR.Audit.Abstractions.Enums;

namespace GR.Notifications.Abstractions.Models.Data
{
    [TrackEntity(Option = TrackEntityOption.Ignore)]
    public class NotificationEvent
    {
        /// <summary>
        /// Event name
        /// </summary>
        [Key]
        public string EventId { get; set; }

        /// <summary>
        /// Event group
        /// </summary>
        public string EventGroupName { get; set; }
    }
}
