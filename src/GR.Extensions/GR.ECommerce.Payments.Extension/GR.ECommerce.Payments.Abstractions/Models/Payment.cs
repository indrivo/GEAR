using System;
using System.ComponentModel.DataAnnotations;
using GR.Core;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Payments.Abstractions.Enums;
using GR.Orders.Abstractions.Models;

namespace GR.ECommerce.Payments.Abstractions.Models
{
    public class Payment : BaseModel
    {
        /// <summary>
        /// Reference to order
        /// </summary>
        public virtual Order Order { get; set; }

        /// <summary>
        /// Order id
        /// </summary>
        [Required]
        public virtual Guid OrderId { get; set; }

        /// <summary>
        /// Total
        /// </summary>
        [Range(0, double.MaxValue)]
        public virtual decimal Total { get; set; }

        /// <summary>
        /// Payment status
        /// </summary>
        public virtual PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Failed;

        /// <summary>
        /// Transaction 
        /// </summary>
        public virtual string GatewayTransactionId { get; set; }

        /// <summary>
        /// Failure message
        /// </summary>
        public virtual string FailureMessage { get; set; }

        /// <summary>
        /// Reference to payment provider
        /// </summary>
        public virtual PaymentMethod PaymentMethod { get; set; }
        public virtual string PaymentMethodId { get; set; }

        /// <summary>
        /// User id
        /// </summary>
        public virtual Guid UserId { get; set; }
    }
}
