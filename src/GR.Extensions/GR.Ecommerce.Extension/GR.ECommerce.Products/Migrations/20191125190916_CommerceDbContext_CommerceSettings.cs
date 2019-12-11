using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace GR.ECommerce.BaseImplementations.Migrations
{
    public partial class CommerceDbContext_CommerceSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommerceSettings_Currencies_CurrencyId",
                schema: "Commerce",
                table: "CommerceSettings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CommerceSettings",
                schema: "Commerce",
                table: "CommerceSettings");

            migrationBuilder.DropIndex(
                name: "IX_CommerceSettings_CurrencyId",
                schema: "Commerce",
                table: "CommerceSettings");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "Commerce",
                table: "CommerceSettings");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                schema: "Commerce",
                table: "CommerceSettings");

            migrationBuilder.AddColumn<string>(
                name: "Key",
                schema: "Commerce",
                table: "CommerceSettings",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                schema: "Commerce",
                table: "CommerceSettings",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Value",
                schema: "Commerce",
                table: "CommerceSettings",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommerceSettings",
                schema: "Commerce",
                table: "CommerceSettings",
                column: "Key");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CommerceSettings",
                schema: "Commerce",
                table: "CommerceSettings");

            migrationBuilder.DropColumn(
                name: "Key",
                schema: "Commerce",
                table: "CommerceSettings");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "Commerce",
                table: "CommerceSettings");

            migrationBuilder.DropColumn(
                name: "Value",
                schema: "Commerce",
                table: "CommerceSettings");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                schema: "Commerce",
                table: "CommerceSettings",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AddColumn<string>(
                name: "CurrencyId",
                schema: "Commerce",
                table: "CommerceSettings",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommerceSettings",
                schema: "Commerce",
                table: "CommerceSettings",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CommerceSettings_CurrencyId",
                schema: "Commerce",
                table: "CommerceSettings",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommerceSettings_Currencies_CurrencyId",
                schema: "Commerce",
                table: "CommerceSettings",
                column: "CurrencyId",
                principalSchema: "Commerce",
                principalTable: "Currencies",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
