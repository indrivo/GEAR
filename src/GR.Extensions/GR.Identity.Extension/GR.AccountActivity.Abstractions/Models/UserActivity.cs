using System;
using GR.Core;

namespace GR.AccountActivity.Abstractions.Models
{
    public class UserActivity : BaseModel
    {
        /// <summary>
        /// User activity
        /// </summary>
        public virtual string Activity { get; set; }

        /// <summary>
        /// Reference to device
        /// </summary>
        public virtual UserDevice Device { get; set; }
        public virtual Guid DeviceId { get; set; }
    }
}