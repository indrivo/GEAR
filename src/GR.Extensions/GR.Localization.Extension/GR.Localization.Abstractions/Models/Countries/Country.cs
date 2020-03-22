using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GR.Audit.Abstractions.Attributes;
using GR.Audit.Abstractions.Enums;

namespace GR.Localization.Abstractions.Models.Countries
{
    [TrackEntity(Option = TrackEntityOption.AllFields)]
    public class Country
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        public Country()
        {
        }

        public Country(string id)
        {
            Id = id;
        }

        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(450)]
        public string Name { get; set; }

        [StringLength(450)]
        public string Code3 { get; set; }

        public bool IsBillingEnabled { get; set; }

        public bool IsShippingEnabled { get; set; }

        public bool IsCityEnabled { get; set; } = true;

        public bool IsZipCodeEnabled { get; set; } = true;

        public bool IsDistrictEnabled { get; set; } = true;

        public ICollection<StateOrProvince> StatesOrProvinces { get; set; } = new List<StateOrProvince>();
    }
}