using GR.ECommerce.Abstractions;
using GR.Orders.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace GR.Orders.Abstractions
{
    public interface IOrderDbContext : ICommerceContext
    {
        /// <summary>
        /// Orders
        /// </summary>
        DbSet<Order> Orders { get; set; }

        /// <summary>
        /// Product orders
        /// </summary>
        DbSet<ProductOrder> ProductOrders { get; set; }
    }
}