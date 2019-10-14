using Microsoft.EntityFrameworkCore.Migrations;

namespace ST.Calendar.Migrations
{
    public partial class CalendarDbContext_AddReminderMinutes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MinutesToRemind",
                schema: "Calendar",
                table: "CalendarEvents",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinutesToRemind",
                schema: "Calendar",
                table: "CalendarEvents");
        }
    }
}
