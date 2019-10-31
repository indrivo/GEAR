using System;
using System.Collections.Generic;
using System.Text;
using GR.Core;

namespace GR.ECommerce.Abstractions.Models
{
    public class Cart: BaseModel
    {
        public double TotalPrice { get; set; }

        public Guid UserId { get; set; }

        public IEnumerable<CartItem> CartItems { get; set; } 
    }
}
