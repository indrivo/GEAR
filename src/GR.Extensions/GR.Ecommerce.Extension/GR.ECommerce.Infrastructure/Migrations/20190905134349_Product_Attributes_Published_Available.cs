using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.ECommerce.Infrastructure.Migrations
{
    public partial class Product_Attributes_Published_Available : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                schema: "Commerce",
                table: "ProductAttributes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                schema: "Commerce",
                table: "ProductAttributes",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvailable",
                schema: "Commerce",
                table: "ProductAttributes");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                schema: "Commerce",
                table: "ProductAttributes");
        }
    }
}
