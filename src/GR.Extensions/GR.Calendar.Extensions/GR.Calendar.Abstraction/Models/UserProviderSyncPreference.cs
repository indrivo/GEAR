using System;

namespace GR.Calendar.Abstractions.Models
{
    public class UserProviderSyncPreference
    {
        /// <summary>
        /// User id
        /// </summary>
        public virtual Guid UserId { get; set; }

        /// <summary>
        /// Provider
        /// </summary>
        public virtual string Provider { get; set; }

        /// <summary>
        /// Sync state
        /// </summary>
        public virtual bool Sync { get; set; }
    }
}