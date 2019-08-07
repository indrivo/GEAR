using System.ComponentModel.DataAnnotations;

namespace ST.ECommerce.Abstractions.Models.Address
{
    public class StatesOrProvinces
    {
        [Required]
        public virtual string Id { get; set; }
        public virtual Country Country { get; set; }
        [Required]
        public virtual string CountryId { get; set; }
        [Required]
        public virtual string Code { get; set; }
        [Required]
        public virtual string Name { get; set; }

    }
}
