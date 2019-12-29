using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.Calendar.Migrations
{
    public partial class CalendarDbContext_AddEventRemindState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RemindSent",
                schema: "Calendar",
                table: "CalendarEvents",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemindSent",
                schema: "Calendar",
                table: "CalendarEvents");
        }
    }
}
