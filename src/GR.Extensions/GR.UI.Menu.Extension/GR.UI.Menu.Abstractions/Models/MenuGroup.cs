using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GR.Core;

namespace GR.UI.Menu.Abstractions.Models
{
    public class MenuGroup : BaseModel
    {
        [Required]
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public ICollection<MenuItem> MenuItems { get; set; }
    }
}
