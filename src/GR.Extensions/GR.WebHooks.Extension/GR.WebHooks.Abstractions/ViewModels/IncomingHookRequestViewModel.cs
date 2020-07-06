using System;

namespace GR.WebHooks.Abstractions.ViewModels
{
    public class IncomingHookRequestViewModel
    {
        public virtual Guid? ProviderId { get; set; }
        public virtual Guid HookId { get; set; }
    }
}