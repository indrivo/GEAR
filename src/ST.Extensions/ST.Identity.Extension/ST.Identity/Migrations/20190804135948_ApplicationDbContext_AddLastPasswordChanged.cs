using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ST.Identity.Migrations
{
    public partial class ApplicationDbContext_AddLastPasswordChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastLogin",
                schema: "Identity",
                table: "Users",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastPasswordChanged",
                schema: "Identity",
                table: "Users",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastLogin",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastPasswordChanged",
                schema: "Identity",
                table: "Users");
        }
    }
}
