using GR.Core;
using GR.Core.Attributes;
using GR.Identity.Abstractions.Models.AddressModels;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GR.Identity.Abstractions.Models.MultiTenants
{
    public class Tenant : BaseModel
    {
        /// <summary>
        /// Name of tenant
        /// </summary>
        [Required]
        [DisplayTranslate(Key = "iso_company_name")]
        public virtual string Name { get; set; }

        /// <summary>
        /// Name for system
        /// </summary>
        [Required]
        public virtual string MachineName { get; set; }

        /// <summary>
        /// Description for this tenant
        /// </summary>
        [DisplayTranslate(Key = "description")]
        public string Description { get; set; }

        /// <summary>
        /// The url of site web
        /// </summary>
        [Display(Name = "Site Web")]
        [DisplayTranslate(Key = "iso_company_website")]
        public string SiteWeb { get; set; }

        /// <summary>
        /// Address
        /// </summary>
        [DisplayTranslate(Key = "system_adress")]
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
        [DisplayTranslate(Key = "system_select_country")]
        public string CountryId { get; set; }

        /// <summary>
        /// City
        /// </summary>
        public StateOrProvince City { get; set; }

        [Display(Name = "Select city")]
        [DisplayTranslate(Key = "system_select_city")]
        public long? CityId { get; set; }

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