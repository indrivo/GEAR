using Microsoft.EntityFrameworkCore.Migrations;

namespace ST.Files.Migrations
{
    public partial class InitialUp2FileMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                schema: "File",
                table: "Files");

            migrationBuilder.AlterColumn<long>(
                name: "Size",
                schema: "File",
                table: "Files",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Size",
                schema: "File",
                table: "Files",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "File",
                table: "Files",
                nullable: true);
        }
    }
}
