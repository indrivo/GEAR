using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.TaskManager.Migrations
{
    public partial class TaskManagerDbContext_TaskNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TaskNumber",
                schema: "Task",
                table: "Tasks",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_EndDate",
                schema: "Task",
                table: "Tasks",
                column: "EndDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tasks_EndDate",
                schema: "Task",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "TaskNumber",
                schema: "Task",
                table: "Tasks");
        }
    }
}
