using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.ECommerce.BaseImplementations.Migrations
{
    public partial class ComerceDbContext_addCart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductAttribute_Products_ProductId",
                schema: "Commerce",
                table: "ProductAttribute");

            migrationBuilder.DropIndex(
                name: "IX_ProductAttribute_ProductId",
                schema: "Commerce",
                table: "ProductAttribute");

            migrationBuilder.DropColumn(
                name: "ProductId",
                schema: "Commerce",
                table: "ProductAttribute");

            migrationBuilder.CreateTable(
                name: "Carts",
                schema: "Commerce",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Author = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Changed = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Version = table.Column<int>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: true),
                    TotalPrice = table.Column<double>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                schema: "Commerce",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Author = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Changed = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Version = table.Column<int>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: true),
                    CartId = table.Column<Guid>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false),
                    VariationId = table.Column<Guid>(nullable: true),
                    ProductVariationId = table.Column<Guid>(nullable: true),
                    Amount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItems_Carts_CartId",
                        column: x => x.CartId,
                        principalSchema: "Commerce",
                        principalTable: "Carts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "Commerce",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItems_ProductVariations_ProductVariationId",
                        column: x => x.ProductVariationId,
                        principalSchema: "Commerce",
                        principalTable: "ProductVariations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId",
                schema: "Commerce",
                table: "CartItems",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ProductId",
                schema: "Commerce",
                table: "CartItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ProductVariationId",
                schema: "Commerce",
                table: "CartItems",
                column: "ProductVariationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItems",
                schema: "Commerce");

            migrationBuilder.DropTable(
                name: "Carts",
                schema: "Commerce");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                schema: "Commerce",
                table: "ProductAttribute",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttribute_ProductId",
                schema: "Commerce",
                table: "ProductAttribute",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductAttribute_Products_ProductId",
                schema: "Commerce",
                table: "ProductAttribute",
                column: "ProductId",
                principalSchema: "Commerce",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
