using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using GR.Core;

namespace GR.ECommerce.Abstractions.Models
{
    public class ProductOption : BaseModel
    {
        
        [Required(AllowEmptyStrings = false)]
        public virtual string Name { get; set; }
        [Required]
        public virtual bool IsPublish { get; set; }
    }
}
