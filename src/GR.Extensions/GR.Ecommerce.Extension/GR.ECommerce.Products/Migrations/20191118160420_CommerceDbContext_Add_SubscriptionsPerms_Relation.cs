using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.ECommerce.BaseImplementations.Migrations
{
    public partial class CommerceDbContext_Add_SubscriptionsPerms_Relation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubscriptionPermission_Subscription_SubscriptionId",
                schema: "Commerce",
                table: "SubscriptionPermission");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubscriptionPermission",
                schema: "Commerce",
                table: "SubscriptionPermission");

            migrationBuilder.RenameTable(
                name: "SubscriptionPermission",
                schema: "Commerce",
                newName: "SubscriptionPermissions",
                newSchema: "Commerce");

            migrationBuilder.RenameIndex(
                name: "IX_SubscriptionPermission_SubscriptionId",
                schema: "Commerce",
                table: "SubscriptionPermissions",
                newName: "IX_SubscriptionPermissions_SubscriptionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubscriptionPermissions",
                schema: "Commerce",
                table: "SubscriptionPermissions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubscriptionPermissions_Subscription_SubscriptionId",
                schema: "Commerce",
                table: "SubscriptionPermissions",
                column: "SubscriptionId",
                principalSchema: "Commerce",
                principalTable: "Subscription",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubscriptionPermissions_Subscription_SubscriptionId",
                schema: "Commerce",
                table: "SubscriptionPermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubscriptionPermissions",
                schema: "Commerce",
                table: "SubscriptionPermissions");

            migrationBuilder.RenameTable(
                name: "SubscriptionPermissions",
                schema: "Commerce",
                newName: "SubscriptionPermission",
                newSchema: "Commerce");

            migrationBuilder.RenameIndex(
                name: "IX_SubscriptionPermissions_SubscriptionId",
                schema: "Commerce",
                table: "SubscriptionPermission",
                newName: "IX_SubscriptionPermission_SubscriptionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubscriptionPermission",
                schema: "Commerce",
                table: "SubscriptionPermission",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubscriptionPermission_Subscription_SubscriptionId",
                schema: "Commerce",
                table: "SubscriptionPermission",
                column: "SubscriptionId",
                principalSchema: "Commerce",
                principalTable: "Subscription",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
