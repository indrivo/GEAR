using Microsoft.EntityFrameworkCore.Migrations;

namespace ST.Calendar.Migrations
{
    public partial class CalendarDbContext_AddEventPriority : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UIClassName",
                schema: "Calendar",
                table: "CalendarEvents");

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                schema: "Calendar",
                table: "CalendarEvents",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                schema: "Calendar",
                table: "CalendarEvents");

            migrationBuilder.AddColumn<string>(
                name: "UIClassName",
                schema: "Calendar",
                table: "CalendarEvents",
                nullable: true);
        }
    }
}
