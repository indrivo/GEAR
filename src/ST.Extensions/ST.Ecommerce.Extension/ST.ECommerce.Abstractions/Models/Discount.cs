using System.ComponentModel.DataAnnotations;
using ST.Core;

namespace ST.ECommerce.Abstractions.Models
{
    public class Discount : BaseModel
    {
        /// <summary>
        /// Discount of Product
        /// </summary>
        [Range(0, 100), Required]
        public virtual decimal Percentage { get; set; }
    }
}
