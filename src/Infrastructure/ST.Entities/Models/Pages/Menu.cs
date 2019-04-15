﻿using ST.BaseRepository;
using System;
using System.ComponentModel.DataAnnotations;
using ST.Audit.Models;

namespace ST.Entities.Models.Pages
{
    public class Menu : ExtendedModel
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class MenuItem : ExtendedModel
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
    }

    public class MenuViewModel : MenuItem
    {
        public MenuViewModel[] SubItems { get; set; }
    }
}