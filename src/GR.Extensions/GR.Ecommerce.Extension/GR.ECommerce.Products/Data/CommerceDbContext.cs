using GR.Audit.Contexts;
using GR.Core.Abstractions;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.BaseImplementations.Data.Extensions;
using GR.ECommerce.Payments.Abstractions;
using GR.ECommerce.Payments.Abstractions.Models;
using GR.Orders.Abstractions;
using GR.Orders.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace GR.ECommerce.BaseImplementations.Data
{
    public class CommerceDbContext : TrackerDbContext, ICommerceContext, IOrderDbContext, IPaymentContext
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
        public virtual DbSet<ProductPrice> ProductPrices { get; set; }
        public virtual DbSet<ProductOrder> ProductOrders { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<ProductAttribute> ProductAttribute { get; set; }
        public virtual DbSet<AttributeGroup> AttributeGroups { get; set; }
        public virtual DbSet<ProductAttributes> ProductAttributes { get; set; }
        public virtual DbSet<ProductVariation> ProductVariations { get; set; }
        public virtual DbSet<ProductVariationDetail> ProductVariationDetails { get; set; }
        public virtual DbSet<ProductOption> ProductOption { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<CartItem> CartItems { get; set; }
        public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<OrderHistory> OrderHistories { get; set; }

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

            builder.Entity<PaymentMethod>().HasKey(x => x.Name);
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
