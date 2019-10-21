using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.Identity.Migrations
{
    public partial class ApplicationDbContext_FixAddressFieldDataType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Users_ApplicationUserId1",
                schema: "Identity",
                table: "Addresses");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_ApplicationUserId1",
                schema: "Identity",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId1",
                schema: "Identity",
                table: "Addresses");

            migrationBuilder.AlterColumn<Guid>(
                name: "ApplicationUserId",
                schema: "Identity",
                table: "Addresses",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_ApplicationUserId",
                schema: "Identity",
                table: "Addresses",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Users_ApplicationUserId",
                schema: "Identity",
                table: "Addresses",
                column: "ApplicationUserId",
                principalSchema: "Identity",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Users_ApplicationUserId",
                schema: "Identity",
                table: "Addresses");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_ApplicationUserId",
                schema: "Identity",
                table: "Addresses");

            migrationBuilder.AlterColumn<Guid>(
                name: "ApplicationUserId",
                schema: "Identity",
                table: "Addresses",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationUserId1",
                schema: "Identity",
                table: "Addresses",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_ApplicationUserId1",
                schema: "Identity",
                table: "Addresses",
                column: "ApplicationUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Users_ApplicationUserId1",
                schema: "Identity",
                table: "Addresses",
                column: "ApplicationUserId1",
                principalSchema: "Identity",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
