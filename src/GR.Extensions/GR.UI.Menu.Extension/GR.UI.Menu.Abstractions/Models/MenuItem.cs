using System;
using System.ComponentModel.DataAnnotations;
using GR.Core;
using GR.Core.Attributes;

namespace GR.UI.Menu.Abstractions.Models
{
    public class MenuItem : BaseModel
    {
        /// <summary>
        /// Name
        /// </summary>
        [Required]
        [DisplayTranslate(Key = "name")]
        public virtual string Name { get; set; }

        /// <summary>
        /// Href
        /// </summary>
        public virtual string Href { get; set; } = "#";

        /// <summary>
        /// Translate key
        /// </summary>
        public virtual string Translate { get; set; } = "none";

        /// <summary>
        /// Menu item icon
        /// </summary>
        public virtual string Icon { get; set; }

        /// <summary>
        /// Menu group reference
        /// </summary>
        public virtual MenuGroup Menu { get; set; }
        public virtual Guid MenuId { get; set; }

        /// <summary>
        /// Parent item 
        /// </summary>
        public virtual MenuItem ParentMenuItem { get; set; }
        public virtual Guid? ParentMenuItemId { get; set; }

        /// <summary>
        /// Allowed roles
        /// </summary>
        public virtual string AllowedRoles { get; set; }

        /// <summary>
        /// Order
        /// </summary>
        [DisplayTranslate(Key = "system_order")]
        public int Order { get; set; } = 1;
    }
}
