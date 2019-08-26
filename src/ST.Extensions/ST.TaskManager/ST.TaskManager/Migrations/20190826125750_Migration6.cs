using Microsoft.EntityFrameworkCore.Migrations;

namespace ST.TaskManager.Migrations
{
    public partial class Migration6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tasks_UserId_IsDeleted",
                schema: "Task",
                table: "Tasks");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_UserId_IsDeleted",
                schema: "Task",
                table: "Tasks",
                columns: new[] { "UserId", "IsDeleted" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tasks_UserId_IsDeleted",
                schema: "Task",
                table: "Tasks");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_UserId_IsDeleted",
                schema: "Task",
                table: "Tasks",
                columns: new[] { "UserId", "IsDeleted" },
                unique: true);
        }
    }
}
