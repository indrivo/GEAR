using System;
using GR.Core;

namespace GR.AccountActivity.Abstractions.ViewModels
{
    public class UserActivityViewModel : BaseModel
    {
        /// <summary>
        /// When
        /// </summary>
        public virtual string When { get; set; }

        /// <summary>
        /// Source of activity
        /// </summary>
        public virtual string Source { get; set; }

        /// <summary>
        /// Ip address
        /// </summary>
        public virtual string IpAddress { get; set; }

        /// <summary>
        /// Location
        /// </summary>
        public virtual string Location { get; set; }

        /// <summary>
        /// User id
        /// </summary>
        public virtual Guid UserId { get; set; }

        /// <summary>
        /// Browser
        /// </summary>
        public virtual string Browser { get; set; }

        /// <summary>
        /// Platform
        /// </summary>
        public virtual string Platform { get; set; }

        /// <summary>
        /// Device id
        /// </summary>
        public virtual Guid DeviceId { get; set; }

        /// <summary>
        /// User activity
        /// </summary>
        public virtual string Activity { get; set; }
    }
}
