using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.AccountActivity.Impl.Migrations
{
    public partial class UserActivityDbContext_AddPlatform : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Platform",
                schema: "AccountActivity",
                table: "Devices",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Browser",
                schema: "AccountActivity",
                table: "Activities",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Platform",
                schema: "AccountActivity",
                table: "Activities",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Platform",
                schema: "AccountActivity",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Browser",
                schema: "AccountActivity",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "Platform",
                schema: "AccountActivity",
                table: "Activities");
        }
    }
}
