using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.TaskManager.Migrations
{
    public partial class TaskManagerDbContext_AddFileFiled : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Files",
                schema: "Task",
                table: "Tasks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Files",
                schema: "Task",
                table: "Tasks");
        }
    }
}
