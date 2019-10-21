using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.Identity.Migrations
{
    public partial class ApplicationDbContext_FixStateOrProvince_Ref_DataType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tenants_StateOrProvinces_CityId1",
                schema: "Identity",
                table: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_CityId1",
                schema: "Identity",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "CityId1",
                schema: "Identity",
                table: "Tenants");

            migrationBuilder.AlterColumn<long>(
                name: "CityId",
                schema: "Identity",
                table: "Tenants",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_CityId",
                schema: "Identity",
                table: "Tenants",
                column: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tenants_StateOrProvinces_CityId",
                schema: "Identity",
                table: "Tenants",
                column: "CityId",
                principalSchema: "Identity",
                principalTable: "StateOrProvinces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tenants_StateOrProvinces_CityId",
                schema: "Identity",
                table: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_CityId",
                schema: "Identity",
                table: "Tenants");

            migrationBuilder.AlterColumn<int>(
                name: "CityId",
                schema: "Identity",
                table: "Tenants",
                nullable: true,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CityId1",
                schema: "Identity",
                table: "Tenants",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_CityId1",
                schema: "Identity",
                table: "Tenants",
                column: "CityId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Tenants_StateOrProvinces_CityId1",
                schema: "Identity",
                table: "Tenants",
                column: "CityId1",
                principalSchema: "Identity",
                principalTable: "StateOrProvinces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
