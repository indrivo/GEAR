using System;
using System.Collections.Generic;
using GR.Core;

namespace GR.ECommerce.Abstractions.Models
{
    public class Cart : BaseModel
    {
        /// <summary>
        /// Total price
        /// </summary>
        public virtual double TotalPrice { get; set; }

        /// <summary>
        /// User id
        /// </summary>
        public virtual Guid UserId { get; set; }

        /// <summary>
        /// Cart items
        /// </summary>
        public virtual IEnumerable<CartItem> CartItems { get; set; }
    }
}
