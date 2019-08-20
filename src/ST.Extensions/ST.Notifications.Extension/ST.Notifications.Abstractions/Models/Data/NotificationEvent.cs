using System.ComponentModel.DataAnnotations;
using ST.Audit.Attributes;
using ST.Audit.Enums;

namespace ST.Notifications.Abstractions.Models.Data
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
