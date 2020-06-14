using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.ECommerce.Infrastructure.Migrations
{
    public partial class CommerceDbContext_AddOrderHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderHistory_Orders_OrderId",
                schema: "Commerce",
                table: "OrderHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderHistory",
                schema: "Commerce",
                table: "OrderHistory");

            migrationBuilder.RenameTable(
                name: "OrderHistory",
                schema: "Commerce",
                newName: "OrderHistories",
                newSchema: "Commerce");

            migrationBuilder.RenameColumn(
                name: "Description",
                schema: "Commerce",
                table: "OrderHistories",
                newName: "Notes");

            migrationBuilder.RenameIndex(
                name: "IX_OrderHistory_OrderId",
                schema: "Commerce",
                table: "OrderHistories",
                newName: "IX_OrderHistories_OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderHistories",
                schema: "Commerce",
                table: "OrderHistories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderHistories_Orders_OrderId",
                schema: "Commerce",
                table: "OrderHistories",
                column: "OrderId",
                principalSchema: "Commerce",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderHistories_Orders_OrderId",
                schema: "Commerce",
                table: "OrderHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderHistories",
                schema: "Commerce",
                table: "OrderHistories");

            migrationBuilder.RenameTable(
                name: "OrderHistories",
                schema: "Commerce",
                newName: "OrderHistory",
                newSchema: "Commerce");

            migrationBuilder.RenameColumn(
                name: "Notes",
                schema: "Commerce",
                table: "OrderHistory",
                newName: "Description");

            migrationBuilder.RenameIndex(
                name: "IX_OrderHistories_OrderId",
                schema: "Commerce",
                table: "OrderHistory",
                newName: "IX_OrderHistory_OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderHistory",
                schema: "Commerce",
                table: "OrderHistory",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderHistory_Orders_OrderId",
                schema: "Commerce",
                table: "OrderHistory",
                column: "OrderId",
                principalSchema: "Commerce",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
