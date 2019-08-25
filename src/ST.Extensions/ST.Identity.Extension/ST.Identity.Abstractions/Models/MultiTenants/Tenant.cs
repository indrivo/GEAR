using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ST.Core;
using ST.Identity.Abstractions.Models.AddressModels;

namespace ST.Identity.Abstractions.Models.MultiTenants
{
    public class Tenant : BaseModel
    {
        /// <summary>
        /// Name of tenant
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Name for system
        /// </summary>
        [Required]
        public string MachineName { get; set; }
        /// <summary>
        /// Description for this tenant
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The url of site web
        /// </summary>
        public string SiteWeb { get; set; }

        /// <summary>
        /// Address
        /// </summary>
        public string Address { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        [NotMapped]
        public override Guid? TenantId { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        [NotMapped]
        public override int Version { get; set; }

        /// <summary>
        /// Photo
        /// </summary>
        public byte[] OrganizationLogo { get; set; }

        /// <summary>
        /// Country
        /// </summary>
        public Country Country { get; set; }
        [Display(Name = "Select country")]
        public string CountryId { get; set; }

        /// <summary>
        /// City
        /// </summary>
        public StateOrProvince City { get; set; }
        [Display(Name = "Select country")]
        public int? CityId { get; set; }

        /// <summary>
        /// TIme zone
        /// </summary>
        public string TimeZone { get; set; }

        /// <summary>
        /// Date format
        /// </summary>
        public string DateFormat { get; set; }
    }
}
