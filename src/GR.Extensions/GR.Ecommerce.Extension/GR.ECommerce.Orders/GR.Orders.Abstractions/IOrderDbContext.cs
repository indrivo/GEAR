using GR.Core.Abstractions;
using GR.Orders.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace GR.Orders.Abstractions
{
    public interface IOrderDbContext : IDbContext
    {
        /// <summary>
        /// Orders
        /// </summary>
        DbSet<Order> Orders { get; set; }

        /// <summary>
        /// Product orders
        /// </summary>
        DbSet<ProductOrder> ProductOrders { get; set; }

        /// <summary>
        /// Order histories
        /// </summary>
        DbSet<OrderHistory> OrderHistories { get; set; }
    }
}