using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.ECommerce.BaseImplementations.Migrations
{
    public partial class CommerceDbContext_AddAmount_On_ProductOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShipmentAddresses",
                schema: "Commerce");

            migrationBuilder.AddColumn<int>(
                name: "Amount",
                schema: "Commerce",
                table: "ProductOrders",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                schema: "Commerce",
                table: "ProductOrders");

            migrationBuilder.CreateTable(
                name: "ShipmentAddresses",
                schema: "Commerce",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Author = table.Column<string>(nullable: true),
                    Changed = table.Column<DateTime>(nullable: false),
                    CountryId = table.Column<string>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Street = table.Column<string>(nullable: true),
                    TenantId = table.Column<Guid>(nullable: true),
                    Version = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentAddresses", x => x.Id);
                });
        }
    }
}
