using Microsoft.EntityFrameworkCore.Migrations;

namespace ST.Entities.Migrations
{
    public partial class docExtension : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Extension",
                schema: "Entities",
                table: "Documents",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Extension",
                schema: "Entities",
                table: "Documents");
        }
    }
}
