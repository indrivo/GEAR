using Microsoft.EntityFrameworkCore.Migrations;

namespace ST.Identity.Migrations
{
    public partial class ApplicationDbContext_AddDefaultUserAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                schema: "Identity",
                table: "Addresses");

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                schema: "Identity",
                table: "Addresses",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefault",
                schema: "Identity",
                table: "Addresses");

            migrationBuilder.AddColumn<string>(
                name: "City",
                schema: "Identity",
                table: "Addresses",
                maxLength: 450,
                nullable: true);
        }
    }
}
