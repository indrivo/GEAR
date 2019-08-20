using Microsoft.EntityFrameworkCore.Migrations;

namespace ST.Notifications.Migrations
{
    public partial class NotificationDbContext_AddSubject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Subject",
                schema: "Notifications",
                table: "NotificationTemplates",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subject",
                schema: "Notifications",
                table: "NotificationTemplates");
        }
    }
}
