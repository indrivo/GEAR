using System.ComponentModel.DataAnnotations;
using GR.Core;

namespace GR.ECommerce.Abstractions.Models
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
