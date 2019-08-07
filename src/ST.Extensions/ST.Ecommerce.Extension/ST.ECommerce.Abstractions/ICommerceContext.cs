using Microsoft.EntityFrameworkCore;
using ST.Core.Abstractions;
using ST.ECommerce.Abstractions.Models;
using ST.ECommerce.Abstractions.Models.Address;

namespace ST.ECommerce.Abstractions
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
        /// Countries
        /// </summary>
        DbSet<Country> Countries { get; set; }

        /// <summary>
        /// States Or Provinces
        /// </summary>
        DbSet<StatesOrProvinces> StatesOrProvinces { get; set; }

        DbSet<ProductAttribute> ProductAttribute { get; set; }

        DbSet<AttributeGroup> AttributeGroups { get; set; }

        DbSet<ProductAttributes> ProductAttributes { get; set; }
    }
}
