using Microsoft.EntityFrameworkCore.Migrations;

namespace ST.TaskManager.Migrations
{
    public partial class InitialTaskMngrMigration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Tasks_UserId",
                schema: "Task",
                table: "Tasks",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_UserId_IsDeleted",
                schema: "Task",
                table: "Tasks",
                columns: new[] { "UserId", "IsDeleted" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tasks_UserId",
                schema: "Task",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_UserId_IsDeleted",
                schema: "Task",
                table: "Tasks");
        }
    }
}
