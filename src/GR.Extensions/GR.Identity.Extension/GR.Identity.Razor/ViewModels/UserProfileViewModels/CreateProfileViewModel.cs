using GR.Entities.Abstractions.Models.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GR.Identity.Razor.ViewModels.UserProfileViewModels
{
    public class CreateProfileViewModel
    {
        [Required(AllowEmptyStrings = false)]
        public string ProfileName { get; set; }

        public IEnumerable<TableModel> Entities { get; set; }

        [Required(ErrorMessage = "Select a entity for this profile"), Display(Name = "Entities profile")]
        public Guid EntityId { get; set; }
    }
}