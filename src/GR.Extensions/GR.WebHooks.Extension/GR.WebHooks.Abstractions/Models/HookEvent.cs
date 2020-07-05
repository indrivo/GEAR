using System;
using System.ComponentModel.DataAnnotations;
using GR.Core;

namespace GR.WebHooks.Abstractions.Models
{
    public class HookEvent : BaseModel
    {
        public virtual WebHook WebHook { get; set; }
        public virtual Guid WebHookId { get; set; }

        [Required]
        public virtual string EventName { get; set; }
    }
}