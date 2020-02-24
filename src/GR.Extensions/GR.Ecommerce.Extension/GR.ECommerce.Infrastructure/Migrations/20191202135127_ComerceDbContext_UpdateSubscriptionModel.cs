using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.ECommerce.BaseImplementations.Migrations
{
    public partial class ComerceDbContext_UpdateSubscriptionModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscription_Orders_OrderId",
                schema: "Commerce",
                table: "Subscription");

            migrationBuilder.AlterColumn<Guid>(
                name: "OrderId",
                schema: "Commerce",
                table: "Subscription",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<bool>(
                name: "IsFree",
                schema: "Commerce",
                table: "Subscription",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscription_Orders_OrderId",
                schema: "Commerce",
                table: "Subscription",
                column: "OrderId",
                principalSchema: "Commerce",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscription_Orders_OrderId",
                schema: "Commerce",
                table: "Subscription");

            migrationBuilder.DropColumn(
                name: "IsFree",
                schema: "Commerce",
                table: "Subscription");

            migrationBuilder.AlterColumn<Guid>(
                name: "OrderId",
                schema: "Commerce",
                table: "Subscription",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscription_Orders_OrderId",
                schema: "Commerce",
                table: "Subscription",
                column: "OrderId",
                principalSchema: "Commerce",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
