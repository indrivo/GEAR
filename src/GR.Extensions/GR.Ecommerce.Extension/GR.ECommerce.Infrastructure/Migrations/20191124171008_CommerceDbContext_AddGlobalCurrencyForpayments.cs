using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace GR.ECommerce.Infrastructure.Migrations
{
    public partial class CommerceDbContext_AddGlobalCurrencyForpayments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrencyId",
                schema: "Commerce",
                table: "Orders",
                nullable: false,
                defaultValue: "USD");

            migrationBuilder.CreateTable(
                name: "CommerceSettings",
                schema: "Commerce",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CurrencyId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommerceSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommerceSettings_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalSchema: "Commerce",
                        principalTable: "Currencies",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CurrencyId",
                schema: "Commerce",
                table: "Orders",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CommerceSettings_CurrencyId",
                schema: "Commerce",
                table: "CommerceSettings",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Currencies_CurrencyId",
                schema: "Commerce",
                table: "Orders",
                column: "CurrencyId",
                principalSchema: "Commerce",
                principalTable: "Currencies",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Currencies_CurrencyId",
                schema: "Commerce",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "CommerceSettings",
                schema: "Commerce");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CurrencyId",
                schema: "Commerce",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                schema: "Commerce",
                table: "Orders");
        }
    }
}
