using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.ECommerce.BaseImplementations.Migrations
{
    public partial class CommerceDbContext_Order_Prices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                schema: "Commerce",
                table: "ProductOrders",
                newName: "PriceWithOutDiscount");

            migrationBuilder.AddColumn<double>(
                name: "DiscountValue",
                schema: "Commerce",
                table: "ProductOrders",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<Guid>(
                name: "BillingAddress",
                schema: "Commerce",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ShipmentAddress",
                schema: "Commerce",
                table: "Orders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountValue",
                schema: "Commerce",
                table: "ProductOrders");

            migrationBuilder.DropColumn(
                name: "BillingAddress",
                schema: "Commerce",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShipmentAddress",
                schema: "Commerce",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "PriceWithOutDiscount",
                schema: "Commerce",
                table: "ProductOrders",
                newName: "Price");
        }
    }
}
