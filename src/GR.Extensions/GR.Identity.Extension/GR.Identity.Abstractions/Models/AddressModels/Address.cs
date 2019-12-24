using GR.Audit.Abstractions.Attributes;
using GR.Audit.Abstractions.Enums;
using GR.Core;
using System;
using System.ComponentModel.DataAnnotations;

namespace GR.Identity.Abstractions.Models.AddressModels
{
    [TrackEntity(Option = TrackEntityOption.AllFields)]
    public class Address : BaseModel
    {
        /// <summary>
        /// Contact name
        /// </summary>
        [StringLength(450)]
        public string ContactName { get; set; }

        /// <summary>
        /// PHONE number
        /// </summary>
        [StringLength(450)]
        public string Phone { get; set; }

        /// <summary>
        /// Primary address
        /// </summary>
        [StringLength(450)]
        public string AddressLine1 { get; set; }

        /// <summary>
        /// Second address
        /// </summary>
        [StringLength(450)]
        public string AddressLine2 { get; set; }

        /// <summary>
        /// Zip code
        /// </summary>
        [StringLength(450)]
        public string ZipCode { get; set; }

        /// <summary>
        /// District
        /// </summary>
        public Guid? DistrictId { get; set; }

        public District District { get; set; }

        /// <summary>
        /// Reference to cities
        /// </summary>
        [Required(ErrorMessage = "The {0} field is required.")]
        public long StateOrProvinceId { get; set; }

        public StateOrProvince StateOrProvince { get; set; }

        /// <summary>
        /// Reference to countries
        /// </summary>
        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(450)]
        public string CountryId { get; set; }

        public Country Country { get; set; }

        /// <summary>
        /// Reference to Users
        /// </summary>
        public GearUser ApplicationUser { get; set; }

        public string ApplicationUserId { get; set; }

        /// <summary>
        /// Represent default user address
        /// </summary>
        public bool IsDefault { get; set; }
    }
}