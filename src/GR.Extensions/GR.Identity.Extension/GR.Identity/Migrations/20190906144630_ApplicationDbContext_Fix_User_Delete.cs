using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.Identity.Migrations
{
    public partial class ApplicationDbContext_Fix_User_Delete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Users_ApplicationUserId",
                schema: "Identity",
                table: "Addresses");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Users_ApplicationUserId",
                schema: "Identity",
                table: "Addresses",
                column: "ApplicationUserId",
                principalSchema: "Identity",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Users_ApplicationUserId",
                schema: "Identity",
                table: "Addresses");

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
    }
}
