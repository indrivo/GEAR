using System;
using System.ComponentModel.DataAnnotations;
using GR.Audit.Abstractions.Attributes;
using GR.Audit.Abstractions.Enums;
using GR.Core;
using GR.Identity.Abstractions;
using GR.Localization.Abstractions.Models.Countries;

namespace GR.Identity.Profile.Abstractions.Models.AddressModels
{
    [TrackEntity(Option = TrackEntityOption.AllFields)]
    public class Address : BaseModel
    {
        /// <summary>
        /// Contact name
        /// </summary>
        [StringLength(450)]
        public virtual string ContactName { get; set; }

        /// <summary>
        /// PHONE number
        /// </summary>
        [StringLength(450)]
        public virtual string Phone { get; set; }

        /// <summary>
        /// Primary address
        /// </summary>
        [StringLength(450)]
        public virtual string AddressLine1 { get; set; }

        /// <summary>
        /// Second address
        /// </summary>
        [StringLength(450)]
        public virtual string AddressLine2 { get; set; }

        /// <summary>
        /// Zip code
        /// </summary>
        [StringLength(450)]
        public virtual string ZipCode { get; set; }

        /// <summary>
        /// Reference to cities
        /// </summary>
        [Required(ErrorMessage = "The {0} field is required.")]
        public virtual Guid StateOrProvinceId { get; set; }

        public virtual StateOrProvince StateOrProvince { get; set; }

        /// <summary>
        /// Reference to countries
        /// </summary>
        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(450)]
        public virtual string CountryId { get; set; }

        public virtual Country Country { get; set; }

        /// <summary>
        /// Reference to Users
        /// </summary>
        public virtual GearUser User { get; set; }

        public virtual Guid UserId { get; set; }

        /// <summary>
        /// Represent default user address
        /// </summary>
        public virtual bool IsDefault { get; set; }

        /// <summary>
        /// Region
        /// </summary>
        public virtual string Region { get; set; }
    }
}