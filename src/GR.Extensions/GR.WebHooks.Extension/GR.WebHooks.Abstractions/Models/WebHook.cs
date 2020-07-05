using System;
using System.Collections.Generic;
using GR.Core;
using GR.WebHooks.Abstractions.Enums;

namespace GR.WebHooks.Abstractions.Models
{
    public class WebHook : BaseModel
    {
        /// <summary>
        /// Name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        public virtual string Token { get; set; }

        /// <summary>
        /// Check if allow requests without token
        /// </summary>
        public virtual bool AllowAnonymous { get; set; }

        /// <summary>
        /// Hook direction
        /// </summary>
        public virtual HookDirection Direction { get; set; }

        /// <summary>
        /// Hook events
        /// </summary>
        public IEnumerable<HookEvent> Events { get; set; } = new List<HookEvent>();

        public virtual HookProvider Provider { get; set; }
        public virtual Guid? ProviderId { get; set; }
    }
}