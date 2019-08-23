using Microsoft.EntityFrameworkCore.Migrations;

namespace ST.TaskManager.Migrations
{
    public partial class MigrationTask4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskItems_Tasks_TaskId",
                schema: "Task",
                table: "TaskItems");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItems_Tasks_TaskId",
                schema: "Task",
                table: "TaskItems",
                column: "TaskId",
                principalSchema: "Task",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskItems_Tasks_TaskId",
                schema: "Task",
                table: "TaskItems");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItems_Tasks_TaskId",
                schema: "Task",
                table: "TaskItems",
                column: "TaskId",
                principalSchema: "Task",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
