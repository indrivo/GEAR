using Microsoft.EntityFrameworkCore;
using GR.Core.Abstractions;
using GR.ECommerce.Abstractions.Models;

namespace GR.ECommerce.Abstractions
{
    public interface ICommerceContext : IDbContext
    {
        /// <summary>
        /// Products
        /// </summary>
        DbSet<Product> Products { get; set; }

        /// <summary>
        /// Product categories
        /// </summary>
        DbSet<Category> Categories { get; set; }

        /// <summary>
        /// Product brands 
        /// </summary>
        DbSet<Brand> Brands { get; set; }

        /// <summary>
        /// Product categories
        /// </summary>
        DbSet<ProductCategory> ProductCategories { get; set; }

        /// <summary>
        /// Discounts
        /// </summary>
        DbSet<Discount> Discounts { get; set; }

        /// <summary>
        /// Product discounts
        /// </summary>
        DbSet<ProductDiscount> ProductDiscounts { get; set; }

        /// <summary>
        /// Product images
        /// </summary>
        DbSet<ProductImage> ProductImages { get; set; }

        /// <summary>
        /// Product options
        /// </summary>
        DbSet<ProductOptions> ProductOptions { get; set; }

        /// <summary>
        /// Product types
        /// </summary>
        DbSet<ProductType> ProductTypes { get; set; }

        /// <summary>
        /// Product prices
        /// </summary>
        DbSet<ProductPrice> ProductPrices { get; set; }

        /// <summary>
        /// Product orders
        /// </summary>
        DbSet<ProductOrder> ProductOrders { get; set; }

        /// <summary>
        /// Product orders
        /// </summary>
        DbSet<Order> Orders { get; set; }

        /// <summary>
        /// Shipment Addresses
        /// </summary>
        DbSet<ShipmentAddress> ShipmentAddresses { get; set; }

        /// <summary>
        /// Product attributes
        /// </summary>
        DbSet<ProductAttribute> ProductAttribute { get; set; }

        /// <summary>
        /// Attribute groups
        /// </summary>
        DbSet<AttributeGroup> AttributeGroups { get; set; }

        /// <summary>
        /// Product attributes
        /// </summary>
        DbSet<ProductAttributes> ProductAttributes { get; set; }
    }
}
