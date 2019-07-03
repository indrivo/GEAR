using System;
using System.ComponentModel.DataAnnotations;
using ST.Core;

namespace ST.PageRender.Abstractions.Models.Pages
{
    public class Menu : BaseModel
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class MenuItem : BaseModel
    {
        [Required]
        public string Name { get; set; }
        public string Href { get; set; }
        public string Translate { get; set; }
        [Required]
        public string Icon { get; set; }
        public Guid MenuId { get; set; }
        public Guid? ParentMenuItemId { get; set; }
        public string AllowedRoles { get; set; }
        public int Order { get; set; } = 1;
    }

    public class MenuViewModel : MenuItem
    {
        public MenuViewModel[] Children { get; set; }
    }
}
