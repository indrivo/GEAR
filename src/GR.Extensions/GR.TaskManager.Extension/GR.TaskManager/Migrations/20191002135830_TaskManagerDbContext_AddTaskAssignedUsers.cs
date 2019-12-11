using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.TaskManager.Migrations
{
    public partial class TaskManagerDbContext_AddTaskAssignedUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaskAssignedUsers",
                schema: "Task",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    TaskId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskAssignedUsers", x => new { x.TaskId, x.UserId });
                    table.ForeignKey(
                        name: "FK_TaskAssignedUsers_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalSchema: "Task",
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskAssignedUsers",
                schema: "Task");
        }
    }
}
