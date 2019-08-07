using Microsoft.EntityFrameworkCore;
using ST.Audit.Contexts;
using ST.Core.Abstractions;
using ST.ECommerce.Abstractions;
using ST.ECommerce.Abstractions.Models;
using ST.ECommerce.Abstractions.Models.Address;
using ST.ECommerce.BaseImplementations.Extensions;

namespace ST.ECommerce.BaseImplementations.Data
{
    public class CommerceDbContext : TrackerDbContext, ICommerceContext
    {
        /// <summary>
        /// Schema
        /// Do not remove this, is used on audit 
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public const string Schema = "Commerce";
        public CommerceDbContext(DbContextOptions<CommerceDbContext> options) : base(options)
        {

        }

        #region Entities
        /// <summary>
        /// Products
        /// </summary>
        public virtual DbSet<Product> Products { get; set; }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }
        public virtual DbSet<Discount> Discounts { get; set; }
        public virtual DbSet<ProductDiscount> ProductDiscounts { get; set; }
        public virtual DbSet<ProductImage> ProductImages { get; set; }
        public virtual DbSet<ProductOptions> ProductOptions { get; set; }
        public virtual DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<ProductPrice> ProductPrices { get; set; }
        public virtual DbSet<ProductOrder> ProductOrders { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<ShipmentAddress> ShipmentAddresses { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<StatesOrProvinces> StatesOrProvinces { get; set; }
        public virtual DbSet<ProductAttribute> ProductAttribute { get; set; }
        public virtual DbSet<AttributeGroup> AttributeGroups { get; set; }
        public virtual DbSet<ProductAttributes> ProductAttributes { get; set; }


        #endregion

        /// <summary>
        /// On model creating
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema(Schema);
            builder.RegisterIndexes();
            builder.Entity<ProductCategory>().HasKey(x => new { x.CategoryId, x.ProductId });
            builder.Entity<ProductDiscount>().HasKey(x => new { x.DiscountId, x.ProductId });
            builder.Entity<ProductOrder>().HasKey(x => new { x.OrderId, x.ProductId });
            builder.Entity<ProductAttributes>().HasKey(x => new { x.ProductAttributeId, x.ProductId });
        }

        /// <inheritdoc />
        /// <summary>
        /// Set entity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public virtual DbSet<TEntity> SetEntity<TEntity>() where TEntity : class, IBaseModel
        {
            return Set<TEntity>();
        }
    }
}
