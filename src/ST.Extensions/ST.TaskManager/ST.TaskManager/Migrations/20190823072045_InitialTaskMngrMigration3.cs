using Microsoft.EntityFrameworkCore.Migrations;

namespace ST.TaskManager.Migrations
{
    public partial class InitialTaskMngrMigration3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Tasks_Id_IsDeleted",
                schema: "Task",
                table: "Tasks",
                columns: new[] { "Id", "IsDeleted" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tasks_Id_IsDeleted",
                schema: "Task",
                table: "Tasks");
        }
    }
}
