using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Client = IdentityServer4.EntityFramework.Entities.Client;

namespace GR.Identity.Razor.ViewModels.PermissionViewModels
{
    public class CreatePermissionViewModel
    {
        [StringLength(50)]
        [Required]
        [Display(Name = "Perrmisiion Name")]
        public string PermissionName { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "Permission Description")]
        public string Description { get; set; }

        [Required]
        [MaxLength(25)]
        [Display(Name = "Permission Key")]
        public string PermissionKey { get; set; }

        [Required]
        [Display(Name = "Application Id")]
        public int ClientId { get; set; }

        public IEnumerable<Client> Clients { get; set; }

        [Display(Name = "Is deleted")]
        public bool IsDeleted { get; set; }
    }

    public class EditPermissionViewModel
    {
        public Guid PermissionId { get; set; }

        [StringLength(50)]
        [Required]
        [Display(Name = "Perrmisiion Name")]
        public string PermissionName { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "Permission Description")]
        public string Description { get; set; }
    }
}