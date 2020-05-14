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
        /// Hook direction
        /// </summary>
        public virtual HookDirection Direction { get; set; }
    }
}