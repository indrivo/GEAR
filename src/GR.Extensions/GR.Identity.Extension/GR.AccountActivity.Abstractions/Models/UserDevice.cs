using System;
using GR.AccountActivity.Abstractions.Helpers;
using GR.Core;

namespace GR.AccountActivity.Abstractions.Models
{
    public class UserDevice : BaseModel
    {
        /// <summary>
        /// Is confirmed
        /// </summary>
        public virtual bool IsConfirmed { get; set; }

        /// <summary>
        /// Confirm date
        /// </summary>
        public virtual DateTime ConfirmDate { get; set; }

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
        /// Get device cache key
        /// </summary>
        /// <returns></returns>
        public string GetDeviceCacheKey()
         => AccountActivityResources.GetDeviceCacheKey(UserId, IpAddress, Platform, Location, Browser);
    }
}