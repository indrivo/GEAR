using System.ComponentModel.DataAnnotations;
using GR.Core;

namespace GR.WebHooks.Abstractions.Models
{
    public class HookProvider : BaseModel
    {
        [Required]
        public virtual string Name { get; set; }

        public virtual string Url { get; set; }

        public virtual string LogoUrl { get; set; }
    }
}