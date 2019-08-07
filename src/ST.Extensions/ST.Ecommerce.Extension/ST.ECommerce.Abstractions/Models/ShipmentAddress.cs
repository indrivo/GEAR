using System.ComponentModel.DataAnnotations;
using ST.Core;
using ST.ECommerce.Abstractions.Models.Address;

namespace ST.ECommerce.Abstractions.Models
{
    public class ShipmentAddress : BaseModel
    {
        [Required]
        public Country Country { get; set; }
        [Required]
        public string CountryId { get; set; }

        public string Street { get; set; }

    }
}
