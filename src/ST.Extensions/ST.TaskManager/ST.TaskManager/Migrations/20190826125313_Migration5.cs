using Microsoft.EntityFrameworkCore.Migrations;

namespace ST.TaskManager.Migrations
{
    public partial class Migration5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tasks_UserId",
                schema: "Task",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_Id_IsDeleted",
                schema: "Task",
                table: "Tasks");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_UserId",
                schema: "Task",
                table: "Tasks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_Id_IsDeleted",
                schema: "Task",
                table: "Tasks",
                columns: new[] { "Id", "IsDeleted" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tasks_UserId",
                schema: "Task",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_Id_IsDeleted",
                schema: "Task",
                table: "Tasks");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_UserId",
                schema: "Task",
                table: "Tasks",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_Id_IsDeleted",
                schema: "Task",
                table: "Tasks",
                columns: new[] { "Id", "IsDeleted" },
                unique: true);
        }
    }
}
