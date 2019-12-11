using System;
using GR.Core;
using GR.ECommerce.Abstractions.Enums;

namespace GR.Orders.Abstractions.Models
{
    public class OrderHistory : BaseModel
    {
        /// <summary>
        /// Order reference
        /// </summary>
        public virtual Order Order { get; set; }
        public virtual Guid OrderId { get; set; }

        /// <summary>
        /// On state changed description  
        /// </summary>
        public virtual string Notes { get; set; }

        /// <summary>
        /// Order state
        /// </summary>
        public virtual OrderState OrderState { get; set; } = OrderState.New;
    }
}
