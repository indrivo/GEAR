using System.ComponentModel.DataAnnotations;
using GR.Core;

namespace GR.ECommerce.Abstractions.Models
{
    public class ShipmentAddress : BaseModel
    {
        [Required]
        public string CountryId { get; set; }

        public string Street { get; set; }

    }
}
