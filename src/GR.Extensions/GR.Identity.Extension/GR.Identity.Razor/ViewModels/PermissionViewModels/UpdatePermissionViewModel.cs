using GR.Core;
using IdentityServer4.EntityFramework.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GR.Identity.Razor.ViewModels.PermissionViewModels
{
    public class UpdatePermissionViewModel : BaseModel
    {
        [StringLength(50)]
        [Required]
        [Display(Name = "Perrmisiion Name")]
        public string PermissionName { get; set; }

        [Required]
        [Display(Name = "Application Id")]
        public int ClientId { get; set; }

        public IEnumerable<Client> Clients { get; set; }
    }
}