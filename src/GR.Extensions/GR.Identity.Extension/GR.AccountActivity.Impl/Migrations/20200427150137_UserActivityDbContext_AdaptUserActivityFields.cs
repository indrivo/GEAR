using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.AccountActivity.Impl.Migrations
{
    public partial class UserActivityDbContext_AdaptUserActivityFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Browser",
                schema: "AccountActivity",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                schema: "AccountActivity",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "Location",
                schema: "AccountActivity",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "Platform",
                schema: "AccountActivity",
                table: "Activities");

            migrationBuilder.RenameColumn(
                name: "UserId",
                schema: "AccountActivity",
                table: "Activities",
                newName: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_DeviceId",
                schema: "AccountActivity",
                table: "Activities",
                column: "DeviceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Devices_DeviceId",
                schema: "AccountActivity",
                table: "Activities",
                column: "DeviceId",
                principalSchema: "AccountActivity",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Devices_DeviceId",
                schema: "AccountActivity",
                table: "Activities");

            migrationBuilder.DropIndex(
                name: "IX_Activities_DeviceId",
                schema: "AccountActivity",
                table: "Activities");

            migrationBuilder.RenameColumn(
                name: "DeviceId",
                schema: "AccountActivity",
                table: "Activities",
                newName: "UserId");

            migrationBuilder.AddColumn<string>(
                name: "Browser",
                schema: "AccountActivity",
                table: "Activities",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                schema: "AccountActivity",
                table: "Activities",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                schema: "AccountActivity",
                table: "Activities",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Platform",
                schema: "AccountActivity",
                table: "Activities",
                nullable: true);
        }
    }
}
