using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.Identity.Profile.Migrations
{
    public partial class ProfileDbContext_AddRegion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserAddresses_DistrictId",
                schema: "Identity",
                table: "UserAddresses");

            migrationBuilder.DropColumn(
                name: "DistrictId",
                schema: "Identity",
                table: "UserAddresses");

            migrationBuilder.AddColumn<string>(
                name: "Region",
                schema: "Identity",
                table: "UserAddresses",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Region",
                schema: "Identity",
                table: "UserAddresses");

            migrationBuilder.AddColumn<Guid>(
                name: "DistrictId",
                schema: "Identity",
                table: "UserAddresses",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAddresses_DistrictId",
                schema: "Identity",
                table: "UserAddresses",
                column: "DistrictId");
        }
    }
}
