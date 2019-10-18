using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.Identity.Migrations
{
    public partial class ApplicationDbContext_AddMetadataForTenant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CityId",
                schema: "Identity",
                table: "Tenants",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CityId1",
                schema: "Identity",
                table: "Tenants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CountryId",
                schema: "Identity",
                table: "Tenants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DateFormat",
                schema: "Identity",
                table: "Tenants",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "OrganizationLogo",
                schema: "Identity",
                table: "Tenants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TimeZone",
                schema: "Identity",
                table: "Tenants",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_CityId1",
                schema: "Identity",
                table: "Tenants",
                column: "CityId1");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_CountryId",
                schema: "Identity",
                table: "Tenants",
                column: "CountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tenants_StateOrProvinces_CityId1",
                schema: "Identity",
                table: "Tenants",
                column: "CityId1",
                principalSchema: "Identity",
                principalTable: "StateOrProvinces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tenants_Countries_CountryId",
                schema: "Identity",
                table: "Tenants",
                column: "CountryId",
                principalSchema: "Identity",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tenants_StateOrProvinces_CityId1",
                schema: "Identity",
                table: "Tenants");

            migrationBuilder.DropForeignKey(
                name: "FK_Tenants_Countries_CountryId",
                schema: "Identity",
                table: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_CityId1",
                schema: "Identity",
                table: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_CountryId",
                schema: "Identity",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "CityId",
                schema: "Identity",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "CityId1",
                schema: "Identity",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "CountryId",
                schema: "Identity",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "DateFormat",
                schema: "Identity",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "OrganizationLogo",
                schema: "Identity",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "TimeZone",
                schema: "Identity",
                table: "Tenants");
        }
    }
}
