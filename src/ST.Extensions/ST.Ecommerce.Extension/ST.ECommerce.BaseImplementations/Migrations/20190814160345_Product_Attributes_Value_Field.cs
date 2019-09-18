using Microsoft.EntityFrameworkCore.Migrations;

namespace ST.ECommerce.BaseImplementations.Migrations
{
    public partial class Product_Attributes_Value_Field : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Value",
                schema: "Commerce",
                table: "ProductAttributes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Value",
                schema: "Commerce",
                table: "ProductAttributes");
        }
    }
}
