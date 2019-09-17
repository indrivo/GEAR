using System.ComponentModel.DataAnnotations;
using ST.Core;

namespace ST.ECommerce.Abstractions.Models
{
    public class ShipmentAddress : BaseModel
    {
        [Required] public string CountryId { get; set; }

        public string Street { get; set; }
    }
}