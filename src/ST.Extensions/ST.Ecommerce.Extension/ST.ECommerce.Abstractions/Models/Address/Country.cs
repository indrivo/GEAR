using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ST.ECommerce.Abstractions.Models.Address
{
    public class Country
    {
        [Required]
        public virtual string Id { get; set; }
        [Required]
        public virtual string Name { get; set; }
        [Required]
        public virtual string Code3 { get; set; }
        [Required]
        public virtual bool IsBillingEnabled { get; set; }
        [Required]
        public virtual bool IsShippingEnabled { get; set; }
        [Required]
        public virtual bool IsCityEnabled { get; set; }
        [Required]
        public virtual bool IsDistrictEnabled { get; set; }
        public virtual IEnumerable<StatesOrProvinces> StatesOrProvinces { get; set; }
    }
}
