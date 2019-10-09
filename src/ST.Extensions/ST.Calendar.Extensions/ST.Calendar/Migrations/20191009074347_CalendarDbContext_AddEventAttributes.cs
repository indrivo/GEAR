using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ST.Calendar.Migrations
{
    public partial class CalendarDbContext_AddEventAttributes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Attributes",
                schema: "Calendar",
                columns: table => new
                {
                    EventId = table.Column<Guid>(nullable: false),
                    AttributeName = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attributes", x => new { x.EventId, x.AttributeName });
                    table.ForeignKey(
                        name: "FK_Attributes_CalendarEvents_EventId",
                        column: x => x.EventId,
                        principalSchema: "Calendar",
                        principalTable: "CalendarEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attributes",
                schema: "Calendar");
        }
    }
}
