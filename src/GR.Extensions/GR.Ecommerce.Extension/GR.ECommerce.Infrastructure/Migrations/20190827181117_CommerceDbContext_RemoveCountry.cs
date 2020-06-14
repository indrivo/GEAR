using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.ECommerce.Infrastructure.Migrations
{
    public partial class CommerceDbContext_RemoveCountry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentAddresses_Countries_CountryId",
                schema: "Commerce",
                table: "ShipmentAddresses");

            migrationBuilder.DropTable(
                name: "StatesOrProvinces",
                schema: "Commerce");

            migrationBuilder.DropTable(
                name: "Countries",
                schema: "Commerce");

            migrationBuilder.DropIndex(
                name: "IX_ShipmentAddresses_CountryId",
                schema: "Commerce",
                table: "ShipmentAddresses");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                schema: "Commerce",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Code3 = table.Column<string>(nullable: false),
                    IsBillingEnabled = table.Column<bool>(nullable: false),
                    IsCityEnabled = table.Column<bool>(nullable: false),
                    IsDistrictEnabled = table.Column<bool>(nullable: false),
                    IsShippingEnabled = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StatesOrProvinces",
                schema: "Commerce",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Code = table.Column<string>(nullable: false),
                    CountryId = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatesOrProvinces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatesOrProvinces_Countries_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "Commerce",
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentAddresses_CountryId",
                schema: "Commerce",
                table: "ShipmentAddresses",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_StatesOrProvinces_CountryId",
                schema: "Commerce",
                table: "StatesOrProvinces",
                column: "CountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentAddresses_Countries_CountryId",
                schema: "Commerce",
                table: "ShipmentAddresses",
                column: "CountryId",
                principalSchema: "Commerce",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
