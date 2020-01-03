using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.UI.Menu.Migrations
{
    public partial class MenuDbContext_AdjustConstraints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Icon",
                schema: "Menu",
                table: "MenuItems",
                nullable: true,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Icon",
                schema: "Menu",
                table: "MenuItems",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
