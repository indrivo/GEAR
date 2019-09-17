using System;
using System.ComponentModel.DataAnnotations;
using ST.Core;

namespace ST.ECommerce.Abstractions.Models
{
    public class ProductOptions : BaseModel
    {
        [Required] public virtual string Name { get; set; }
        [Required] public virtual string DisplayName { get; set; }
        public virtual Product Product { get; set; }
        [Required] public virtual Guid ProductId { get; set; }
    }
}