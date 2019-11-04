﻿// <auto-generated />
using System;
using GR.ECommerce.BaseImplementations.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace GR.ECommerce.BaseImplementations.Migrations
{
    [DbContext(typeof(CommerceDbContext))]
    [Migration("20191031135240_CommerceDbContext_Prices_and_Orders")]
    partial class CommerceDbContext_Prices_and_Orders
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("Commerce")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("GR.Audit.Abstractions.Models.TrackAudit", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Author");

                    b.Property<DateTime>("Changed");

                    b.Property<DateTime>("Created");

                    b.Property<string>("DatabaseContextName");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid>("RecordId");

                    b.Property<Guid?>("TenantId");

                    b.Property<int>("TrackEventType");

                    b.Property<string>("TypeFullName");

                    b.Property<string>("UserName");

                    b.Property<int>("Version");

                    b.HasKey("Id");

                    b.ToTable("TrackAudits");
                });

            modelBuilder.Entity("GR.Audit.Abstractions.Models.TrackAuditDetails", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Author");

                    b.Property<DateTime>("Changed");

                    b.Property<DateTime>("Created");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("PropertyName");

                    b.Property<string>("PropertyType");

                    b.Property<Guid?>("TenantId");

                    b.Property<Guid>("TrackAuditId");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.HasIndex("TrackAuditId");

                    b.ToTable("TrackAuditDetails");
                });

            modelBuilder.Entity("GR.ECommerce.Abstractions.Models.AttributeGroup", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Author");

                    b.Property<DateTime>("Changed");

                    b.Property<DateTime>("Created");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<Guid?>("TenantId");

                    b.Property<int>("Version");

                    b.HasKey("Id");

                    b.ToTable("AttributeGroups");
                });

            modelBuilder.Entity("GR.ECommerce.Abstractions.Models.Brand", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Author");

                    b.Property<DateTime>("Changed");

                    b.Property<DateTime>("Created");

                    b.Property<string>("DisplayName");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<Guid?>("TenantId");

                    b.Property<int>("Version");

                    b.HasKey("Id");

                    b.ToTable("Brands");
                });

            modelBuilder.Entity("GR.ECommerce.Abstractions.Models.Cart", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Author");

                    b.Property<DateTime>("Changed");

                    b.Property<DateTime>("Created");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid?>("TenantId");

                    b.Property<double>("TotalPrice");

                    b.Property<Guid>("UserId");

                    b.Property<int>("Version");

                    b.HasKey("Id");

                    b.ToTable("Carts");
                });

            modelBuilder.Entity("GR.ECommerce.Abstractions.Models.CartItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Amount");

                    b.Property<string>("Author");

                    b.Property<Guid>("CartId");

                    b.Property<DateTime>("Changed");

                    b.Property<DateTime>("Created");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid>("ProductId");

                    b.Property<Guid?>("ProductVariationId");

                    b.Property<Guid?>("TenantId");

                    b.Property<int>("Version");

                    b.HasKey("Id");

                    b.HasIndex("CartId");

                    b.HasIndex("ProductId");

                    b.HasIndex("ProductVariationId");

                    b.ToTable("CartItems");
                });

            modelBuilder.Entity("GR.ECommerce.Abstractions.Models.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Author");

                    b.Property<DateTime>("Changed");

                    b.Property<DateTime>("Created");

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<string>("DisplayName")
                        .IsRequired();

                    b.Property<int>("DisplayOrder");

                    b.Property<bool>("IsDeleted");

                    b.Property<bool>("IsPublished");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<Guid?>("ParentCategoryId");

                    b.Property<Guid?>("TenantId");

                    b.Property<int>("Version");

                    b.HasKey("Id");

                    b.HasIndex("ParentCategoryId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("GR.ECommerce.Abstractions.Models.Discount", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Author");

                    b.Property<DateTime>("Changed");

                    b.Property<DateTime>("Created");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("ModifiedBy");

                    b.Property<double>("Percentage");

                    b.Property<Guid?>("TenantId");

                    b.Property<int>("Version");

                    b.HasKey("Id");

                    b.ToTable("Discounts");
                });

            modelBuilder.Entity("GR.ECommerce.Abstractions.Models.Order", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Author");

                    b.Property<DateTime>("Changed");

                    b.Property<DateTime>("Created");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Notes")
                        .HasMaxLength(255);

                    b.Property<int>("OrderState");

                    b.Property<Guid?>("TenantId");

                    b.Property<Guid>("UserId");

                    b.Property<int>("Version");

                    b.HasKey("Id");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("GR.ECommerce.Abstractions.Models.OrderHistory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Author");

                    b.Property<DateTime>("Changed");

                    b.Property<DateTime>("Created");

                    b.Property<string>("Description");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid>("OrderId");

                    b.Property<int>("OrderState");

                    b.Property<Guid?>("TenantId");

                    b.Property<int>("Version");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderHistory");
                });

            modelBuilder.Entity("GR.ECommerce.Abstractions.Models.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Author");

                    b.Property<Guid>("BrandId");

                    b.Property<DateTime>("Changed");

                    b.Property<DateTime>("Created");

                    b.Property<string>("Description");

                    b.Property<string>("DisplayName")
                        .IsRequired();

                    b.Property<string>("Gtin");

                    b.Property<bool>("IsDeleted");

                    b.Property<bool>("IsPublished");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<Guid>("ProductTypeId");

                    b.Property<string>("ShortDescription");

                    b.Property<string>("Sku");

                    b.Property<string>("Specification");

                    b.Property<Guid?>("TenantId");

                    b.Property<byte[]>("Thumbnail");

                    b.Property<int>("Version");

                    b.HasKey("Id");

                    b.HasIndex("BrandId");

                    b.HasIndex("Name");

                    b.HasIndex("ProductTypeId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("GR.ECommerce.Abstractions.Models.ProductAttribute", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("AttributeGroupId");

                    b.Property<string>("Author");

                    b.Property<DateTime>("Changed");

                    b.Property<DateTime>("Created");

                    b.Property<string>("DisplayName")
                        .IsRequired();

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<Guid?>("TenantId");

                    b.Property<int>("Version");

                    b.HasKey("Id");

                    b.HasIndex("AttributeGroupId");

                    b.ToTable("ProductAttribute");
                });

            modelBuilder.Entity("GR.ECommerce.Abstractions.Models.ProductAttributes", b =>
                {
                    b.Property<Guid>("ProductAttributeId");

                    b.Property<Guid>("ProductId");

                    b.Property<bool>("IsAvailable");

                    b.Property<bool>("IsPublished");

                    b.Property<string>("Value");

                    b.HasKey("ProductAttributeId", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductAttributes");
                });

            modelBuilder.Entity("GR.ECommerce.Abstractions.Models.ProductCategory", b =>
                {
                    b.Property<Guid>("CategoryId");

                    b.Property<Guid>("ProductId");

                    b.HasKey("CategoryId", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductCategories");
                });

            modelBuilder.Entity("GR.ECommerce.Abstractions.Models.ProductDiscount", b =>
                {
                    b.Property<Guid>("DiscountId");

                    b.Property<Guid>("ProductId");

                    b.HasKey("DiscountId", "ProductId");

                    b.HasIndex("ProductId")
                        .IsUnique();

                    b.ToTable("ProductDiscounts");
                });

            modelBuilder.Entity("GR.ECommerce.Abstractions.Models.ProductImage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Author");

                    b.Property<DateTime>("Changed");

                    b.Property<DateTime>("Created");

                    b.Property<byte[]>("Image");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid>("ProductId");

                    b.Property<Guid?>("TenantId");

                    b.Property<int>("Version");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductImages");
                });

            modelBuilder.Entity("GR.ECommerce.Abstractions.Models.ProductOption", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Author");

                    b.Property<DateTime>("Changed");

                    b.Property<DateTime>("Created");

                    b.Property<bool>("IsDeleted");

                    b.Property<bool>("IsPublish");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<Guid?>("TenantId");

                    b.Property<int>("Version");

                    b.HasKey("Id");

                    b.ToTable("ProductOption");
                });

            modelBuilder.Entity("GR.ECommerce.Abstractions.Models.ProductOptions", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Author");

                    b.Property<DateTime>("Changed");

                    b.Property<DateTime>("Created");

                    b.Property<string>("DisplayName")
                        .IsRequired();

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<Guid>("ProductId");

                    b.Property<Guid?>("TenantId");

                    b.Property<int>("Version");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductOptions");
                });

            modelBuilder.Entity("GR.ECommerce.Abstractions.Models.ProductOrder", b =>
                {
                    b.Property<Guid>("OrderId");

                    b.Property<Guid>("ProductId");

                    b.Property<double>("Price");

                    b.Property<Guid?>("ProductVariationId");

                    b.HasKey("OrderId", "ProductId");

                    b.HasIndex("ProductId");

                    b.HasIndex("ProductVariationId");

                    b.ToTable("ProductOrders");
                });

            modelBuilder.Entity("GR.ECommerce.Abstractions.Models.ProductPrice", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Author");

                    b.Property<DateTime>("Changed");

                    b.Property<DateTime>("Created");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("ModifiedBy");

                    b.Property<double>("Price");

                    b.Property<Guid>("ProductId");

                    b.Property<Guid?>("TenantId");

                    b.Property<int>("Version");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductPrices");
                });

            modelBuilder.Entity("GR.ECommerce.Abstractions.Models.ProductType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Author");

                    b.Property<DateTime>("Changed");

                    b.Property<DateTime>("Created");

                    b.Property<string>("DisplayName")
                        .IsRequired();

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<Guid?>("TenantId");

                    b.Property<int>("Version");

                    b.HasKey("Id");

                    b.ToTable("ProductTypes");
                });

            modelBuilder.Entity("GR.ECommerce.Abstractions.Models.ProductVariation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Author");

                    b.Property<DateTime>("Changed");

                    b.Property<DateTime>("Created");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("ModifiedBy");

                    b.Property<double>("Price");

                    b.Property<Guid>("ProductId");

                    b.Property<Guid?>("TenantId");

                    b.Property<int>("Version");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductVariations");
                });

            modelBuilder.Entity("GR.ECommerce.Abstractions.Models.ProductVariationDetail", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Author");

                    b.Property<DateTime>("Changed");

                    b.Property<DateTime>("Created");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid>("ProductOptionId");

                    b.Property<Guid>("ProductVariationId");

                    b.Property<Guid?>("TenantId");

                    b.Property<string>("Value");

                    b.Property<int>("Version");

                    b.HasKey("Id");

                    b.HasIndex("ProductOptionId");

                    b.HasIndex("ProductVariationId");

                    b.ToTable("ProductVariationDetails");
                });

            modelBuilder.Entity("GR.ECommerce.Abstractions.Models.ShipmentAddress", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Author");

                    b.Property<DateTime>("Changed");

                    b.Property<string>("CountryId")
                        .IsRequired();

                    b.Property<DateTime>("Created");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Street");

                    b.Property<Guid?>("TenantId");

                    b.Property<int>("Version");

                    b.HasKey("Id");

                    b.ToTable("ShipmentAddresses");
                });

            modelBuilder.Entity("GR.Audit.Abstractions.Models.TrackAuditDetails", b =>
                {
                    b.HasOne("GR.Audit.Abstractions.Models.TrackAudit")
                        .WithMany("AuditDetailses")
                        .HasForeignKey("TrackAuditId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GR.ECommerce.Abstractions.Models.CartItem", b =>
                {
                    b.HasOne("GR.ECommerce.Abstractions.Models.Cart", "Cart")
                        .WithMany("CartItems")
                        .HasForeignKey("CartId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GR.ECommerce.Abstractions.Models.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GR.ECommerce.Abstractions.Models.ProductVariation", "ProductVariation")
                        .WithMany()
                        .HasForeignKey("ProductVariationId");
                });

            modelBuilder.Entity("GR.ECommerce.Abstractions.Models.Category", b =>
                {
                    b.HasOne("GR.ECommerce.Abstractions.Models.Category", "ParentCategory")
                        .WithMany()
                        .HasForeignKey("ParentCategoryId");
                });

            modelBuilder.Entity("GR.ECommerce.Abstractions.Models.OrderHistory", b =>
                {
                    b.HasOne("GR.ECommerce.Abstractions.Models.Order", "Order")
                        .WithMany("OrderHistories")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GR.ECommerce.Abstractions.Models.Product", b =>
                {
                    b.HasOne("GR.ECommerce.Abstractions.Models.Brand", "Brand")
                        .WithMany()
                        .HasForeignKey("BrandId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GR.ECommerce.Abstractions.Models.ProductType", "ProductType")
                        .WithMany()
                        .HasForeignKey("ProductTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GR.ECommerce.Abstractions.Models.ProductAttribute", b =>
                {
                    b.HasOne("GR.ECommerce.Abstractions.Models.AttributeGroup", "AttributeGroup")
                        .WithMany()
                        .HasForeignKey("AttributeGroupId");
                });

            modelBuilder.Entity("GR.ECommerce.Abstractions.Models.ProductAttributes", b =>
                {
                    b.HasOne("GR.ECommerce.Abstractions.Models.ProductAttribute", "ProductAttribute")
                        .WithMany()
                        .HasForeignKey("ProductAttributeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GR.ECommerce.Abstractions.Models.Product", "Product")
                        .WithMany("ProductAttributes")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GR.ECommerce.Abstractions.Models.ProductCategory", b =>
                {
                    b.HasOne("GR.ECommerce.Abstractions.Models.Category", "Category")
                        .WithMany("ProductCategories")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GR.ECommerce.Abstractions.Models.Product", "Product")
                        .WithMany("ProductCategories")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GR.ECommerce.Abstractions.Models.ProductDiscount", b =>
                {
                    b.HasOne("GR.ECommerce.Abstractions.Models.Discount", "Discount")
                        .WithMany()
                        .HasForeignKey("DiscountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GR.ECommerce.Abstractions.Models.Product", "Product")
                        .WithOne("ProductDiscount")
                        .HasForeignKey("GR.ECommerce.Abstractions.Models.ProductDiscount", "ProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GR.ECommerce.Abstractions.Models.ProductImage", b =>
                {
                    b.HasOne("GR.ECommerce.Abstractions.Models.Product", "Product")
                        .WithMany("ProductImages")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GR.ECommerce.Abstractions.Models.ProductOptions", b =>
                {
                    b.HasOne("GR.ECommerce.Abstractions.Models.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GR.ECommerce.Abstractions.Models.ProductOrder", b =>
                {
                    b.HasOne("GR.ECommerce.Abstractions.Models.Order", "Order")
                        .WithMany("ProductOrders")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GR.ECommerce.Abstractions.Models.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GR.ECommerce.Abstractions.Models.ProductVariation", "ProductVariation")
                        .WithMany()
                        .HasForeignKey("ProductVariationId");
                });

            modelBuilder.Entity("GR.ECommerce.Abstractions.Models.ProductPrice", b =>
                {
                    b.HasOne("GR.ECommerce.Abstractions.Models.Product", "Product")
                        .WithMany("ProductPrices")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GR.ECommerce.Abstractions.Models.ProductVariation", b =>
                {
                    b.HasOne("GR.ECommerce.Abstractions.Models.Product", "Product")
                        .WithMany("ProductVariations")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GR.ECommerce.Abstractions.Models.ProductVariationDetail", b =>
                {
                    b.HasOne("GR.ECommerce.Abstractions.Models.ProductOption", "ProductOption")
                        .WithMany()
                        .HasForeignKey("ProductOptionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GR.ECommerce.Abstractions.Models.ProductVariation", "ProductVariation")
                        .WithMany("ProductVariationDetails")
                        .HasForeignKey("ProductVariationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
