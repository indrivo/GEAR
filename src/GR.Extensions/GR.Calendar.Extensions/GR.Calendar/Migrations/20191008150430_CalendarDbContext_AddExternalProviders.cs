using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.Calendar.Migrations
{
    public partial class CalendarDbContext_AddExternalProviders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExternalProviderTokens",
                schema: "Calendar",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    ProviderName = table.Column<string>(nullable: false),
                    Attribute = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalProviderTokens", x => new { x.UserId, x.Attribute, x.ProviderName });
                });

            migrationBuilder.CreateTable(
                name: "UserProviderSyncPreferences",
                schema: "Calendar",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    Provider = table.Column<string>(nullable: false),
                    Sync = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProviderSyncPreferences", x => new { x.UserId, x.Provider });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExternalProviderTokens",
                schema: "Calendar");

            migrationBuilder.DropTable(
                name: "UserProviderSyncPreferences",
                schema: "Calendar");
        }
    }
}
