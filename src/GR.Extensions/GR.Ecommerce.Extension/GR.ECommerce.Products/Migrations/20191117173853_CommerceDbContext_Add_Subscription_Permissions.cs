using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.ECommerce.BaseImplementations.Migrations
{
    public partial class CommerceDbContext_Add_Subscription_Permissions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Valability",
                schema: "Commerce",
                table: "Subscription",
                newName: "Availability");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "Commerce",
                table: "Subscription",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                schema: "Commerce",
                table: "ProductVariations",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.CreateTable(
                name: "SubscriptionPermission",
                schema: "Commerce",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Author = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Changed = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Version = table.Column<int>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: true),
                    SubscriptionId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPermission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscriptionPermission_Subscription_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalSchema: "Commerce",
                        principalTable: "Subscription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPermission_SubscriptionId",
                schema: "Commerce",
                table: "SubscriptionPermission",
                column: "SubscriptionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubscriptionPermission",
                schema: "Commerce");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "Commerce",
                table: "Subscription");

            migrationBuilder.RenameColumn(
                name: "Availability",
                schema: "Commerce",
                table: "Subscription",
                newName: "Valability");

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                schema: "Commerce",
                table: "ProductVariations",
                nullable: false,
                oldClrType: typeof(decimal));
        }
    }
}
