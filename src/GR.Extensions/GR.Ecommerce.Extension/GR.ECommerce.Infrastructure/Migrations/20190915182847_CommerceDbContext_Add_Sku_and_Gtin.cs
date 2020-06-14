using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.ECommerce.Infrastructure.Migrations
{
    public partial class CommerceDbContext_Add_Sku_and_Gtin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Gtin",
                schema: "Commerce",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sku",
                schema: "Commerce",
                table: "Products",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gtin",
                schema: "Commerce",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Sku",
                schema: "Commerce",
                table: "Products");
        }
    }
}
