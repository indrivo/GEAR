using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.ECommerce.BaseImplementations.Migrations
{
    public partial class CommerceDbContext_addProductVariation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductOption",
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
                    Name = table.Column<string>(nullable: true),
                    IsPublish = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductOption", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductVariations",
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
                    ProductId = table.Column<Guid>(nullable: false),
                    Price = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVariations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductVariations_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "Commerce",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductVariationDetails",
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
                    Value = table.Column<string>(nullable: true),
                    ProductVariationId = table.Column<Guid>(nullable: false),
                    ProductOptionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVariationDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductVariationDetails_ProductOption_ProductOptionId",
                        column: x => x.ProductOptionId,
                        principalSchema: "Commerce",
                        principalTable: "ProductOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductVariationDetails_ProductVariations_ProductVariationId",
                        column: x => x.ProductVariationId,
                        principalSchema: "Commerce",
                        principalTable: "ProductVariations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariationDetails_ProductOptionId",
                schema: "Commerce",
                table: "ProductVariationDetails",
                column: "ProductOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariationDetails_ProductVariationId",
                schema: "Commerce",
                table: "ProductVariationDetails",
                column: "ProductVariationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariations_ProductId",
                schema: "Commerce",
                table: "ProductVariations",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductVariationDetails",
                schema: "Commerce");

            migrationBuilder.DropTable(
                name: "ProductOption",
                schema: "Commerce");

            migrationBuilder.DropTable(
                name: "ProductVariations",
                schema: "Commerce");
        }
    }
}
