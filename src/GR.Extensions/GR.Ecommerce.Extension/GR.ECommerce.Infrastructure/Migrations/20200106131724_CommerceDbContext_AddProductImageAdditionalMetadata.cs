using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.ECommerce.Infrastructure.Migrations
{
    public partial class CommerceDbContext_AddProductImageAdditionalMetadata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                schema: "Commerce",
                table: "ProductImages",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                schema: "Commerce",
                table: "ProductImages",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Height",
                schema: "Commerce",
                table: "ProductImages",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "Commerce",
                table: "ProductImages",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Width",
                schema: "Commerce",
                table: "ProductImages",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                schema: "Commerce",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "FileName",
                schema: "Commerce",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "Height",
                schema: "Commerce",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "Commerce",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "Width",
                schema: "Commerce",
                table: "ProductImages");
        }
    }
}
