using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.ECommerce.Infrastructure.Migrations
{
    public partial class CommerceDbContext_Prices_and_Orders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductDiscounts_ProductId",
                schema: "Commerce",
                table: "ProductDiscounts");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductVariationId",
                schema: "Commerce",
                table: "ProductOrders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                schema: "Commerce",
                table: "Orders",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Percentage",
                schema: "Commerce",
                table: "Discounts",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.CreateIndex(
                name: "IX_ProductOrders_ProductVariationId",
                schema: "Commerce",
                table: "ProductOrders",
                column: "ProductVariationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDiscounts_ProductId",
                schema: "Commerce",
                table: "ProductDiscounts",
                column: "ProductId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductOrders_ProductVariations_ProductVariationId",
                schema: "Commerce",
                table: "ProductOrders",
                column: "ProductVariationId",
                principalSchema: "Commerce",
                principalTable: "ProductVariations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductOrders_ProductVariations_ProductVariationId",
                schema: "Commerce",
                table: "ProductOrders");

            migrationBuilder.DropIndex(
                name: "IX_ProductOrders_ProductVariationId",
                schema: "Commerce",
                table: "ProductOrders");

            migrationBuilder.DropIndex(
                name: "IX_ProductDiscounts_ProductId",
                schema: "Commerce",
                table: "ProductDiscounts");

            migrationBuilder.DropColumn(
                name: "ProductVariationId",
                schema: "Commerce",
                table: "ProductOrders");

            migrationBuilder.DropColumn(
                name: "Notes",
                schema: "Commerce",
                table: "Orders");

            migrationBuilder.AlterColumn<decimal>(
                name: "Percentage",
                schema: "Commerce",
                table: "Discounts",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.CreateIndex(
                name: "IX_ProductDiscounts_ProductId",
                schema: "Commerce",
                table: "ProductDiscounts",
                column: "ProductId");
        }
    }
}
