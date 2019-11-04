using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.ECommerce.BaseImplementations.Migrations
{
    public partial class CommerceDbContext_AddPayments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                schema: "Commerce",
                table: "ProductPrices",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "PriceWithOutDiscount",
                schema: "Commerce",
                table: "ProductOrders",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountValue",
                schema: "Commerce",
                table: "ProductOrders",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "Percentage",
                schema: "Commerce",
                table: "Discounts",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                schema: "Commerce",
                columns: table => new
                {
                    Name = table.Column<string>(maxLength: 450, nullable: false),
                    IsEnabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
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
                    OrderId = table.Column<Guid>(nullable: false),
                    Total = table.Column<decimal>(nullable: false),
                    PaymentStatus = table.Column<int>(nullable: false),
                    GatewayTransactionId = table.Column<string>(nullable: true),
                    FailureMessage = table.Column<string>(nullable: true),
                    PaymentMethodId = table.Column<string>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "Commerce",
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payments_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalSchema: "Commerce",
                        principalTable: "PaymentMethods",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId",
                schema: "Commerce",
                table: "Payments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentMethodId",
                schema: "Commerce",
                table: "Payments",
                column: "PaymentMethodId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payments",
                schema: "Commerce");

            migrationBuilder.DropTable(
                name: "PaymentMethods",
                schema: "Commerce");

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                schema: "Commerce",
                table: "ProductPrices",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "PriceWithOutDiscount",
                schema: "Commerce",
                table: "ProductOrders",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "DiscountValue",
                schema: "Commerce",
                table: "ProductOrders",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "Percentage",
                schema: "Commerce",
                table: "Discounts",
                nullable: false,
                oldClrType: typeof(decimal));
        }
    }
}
