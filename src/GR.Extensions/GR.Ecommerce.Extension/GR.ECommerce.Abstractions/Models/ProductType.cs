using System.ComponentModel.DataAnnotations;
using GR.Core;

namespace GR.ECommerce.Abstractions.Models
{
    public class ProductType : BaseModel
    {
        [Required]
        public virtual string Name { get; set; }
        [Required]
        public virtual string DisplayName { get; set; }
    }
}
