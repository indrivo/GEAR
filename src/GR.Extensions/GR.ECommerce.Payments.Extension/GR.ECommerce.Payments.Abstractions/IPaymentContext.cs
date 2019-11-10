using GR.Core.Abstractions;
using GR.ECommerce.Payments.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace GR.ECommerce.Payments.Abstractions
{
    public interface IPaymentContext : IDbContext
    {
        /// <summary>
        /// Payment methods
        /// </summary>
        DbSet<PaymentMethod> PaymentMethods { get; set; }

        /// <summary>
        /// Payments
        /// </summary>
        DbSet<Payment> Payments { get; set; }
    }
}