using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.Dashboard.Migrations
{
    public partial class DashBoardDbContext_Add_Multiple_Dashboards : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "DashBoard",
                table: "WidgetGroups",
                keyColumn: "Id",
                keyValue: new Guid("1b5ff978-6ab0-4ba7-9084-577c525c9adf"));

            migrationBuilder.DeleteData(
                schema: "DashBoard",
                table: "WidgetGroups",
                keyColumn: "Id",
                keyValue: new Guid("4dfecf64-7dfb-43ed-9bce-2de01544e5f7"));

            migrationBuilder.DeleteData(
                schema: "DashBoard",
                table: "WidgetGroups",
                keyColumn: "Id",
                keyValue: new Guid("7fd7f268-db82-4b9c-be2a-1e8c271739c2"));

            migrationBuilder.DeleteData(
                schema: "DashBoard",
                table: "WidgetGroups",
                keyColumn: "Id",
                keyValue: new Guid("e177db49-c0af-4355-bd82-02ab0224d822"));

            migrationBuilder.DeleteData(
                schema: "DashBoard",
                table: "WidgetGroups",
                keyColumn: "Id",
                keyValue: new Guid("fd05e6c8-ec04-4b26-aec7-32057055477a"));

            migrationBuilder.AddColumn<Guid>(
                name: "DashboardId",
                schema: "DashBoard",
                table: "Rows",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Dashboards",
                schema: "DashBoard",
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
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dashboards", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "DashBoard",
                table: "WidgetGroups",
                columns: new[] { "Id", "Author", "Changed", "Created", "IsDeleted", "ModifiedBy", "Name", "Order", "TenantId", "Version" },
                values: new object[,]
                {
                    { new Guid("43d52663-e6d9-4d39-aeff-fd8743cbcce1"), null, new DateTime(2019, 9, 23, 9, 33, 3, 694, DateTimeKind.Utc).AddTicks(6443), new DateTime(2019, 9, 23, 9, 33, 3, 694, DateTimeKind.Utc).AddTicks(5876), false, null, "Charts", 1, null, 0 },
                    { new Guid("3e4d679b-c3a9-4c3d-9b2a-f054583cf8d0"), null, new DateTime(2019, 9, 23, 9, 33, 3, 717, DateTimeKind.Utc).AddTicks(8227), new DateTime(2019, 9, 23, 9, 33, 3, 717, DateTimeKind.Utc).AddTicks(8220), false, null, "Reports", 2, null, 0 },
                    { new Guid("c0a787e3-5cb6-4e6e-b652-601167b251de"), null, new DateTime(2019, 9, 23, 9, 33, 3, 717, DateTimeKind.Utc).AddTicks(8565), new DateTime(2019, 9, 23, 9, 33, 3, 717, DateTimeKind.Utc).AddTicks(8565), false, null, "Custom", 3, null, 0 },
                    { new Guid("79422e99-5fd4-402f-9a38-6c80c436dbad"), null, new DateTime(2019, 9, 23, 9, 33, 3, 717, DateTimeKind.Utc).AddTicks(8599), new DateTime(2019, 9, 23, 9, 33, 3, 717, DateTimeKind.Utc).AddTicks(8598), false, null, "Samples", 4, null, 0 },
                    { new Guid("28bbb537-2303-4cbb-a5d6-f2df6f2e214f"), null, new DateTime(2019, 9, 23, 9, 33, 3, 717, DateTimeKind.Utc).AddTicks(8617), new DateTime(2019, 9, 23, 9, 33, 3, 717, DateTimeKind.Utc).AddTicks(8617), false, null, "Custom", 5, null, 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rows_DashboardId",
                schema: "DashBoard",
                table: "Rows",
                column: "DashboardId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rows_Dashboards_DashboardId",
                schema: "DashBoard",
                table: "Rows",
                column: "DashboardId",
                principalSchema: "DashBoard",
                principalTable: "Dashboards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rows_Dashboards_DashboardId",
                schema: "DashBoard",
                table: "Rows");

            migrationBuilder.DropTable(
                name: "Dashboards",
                schema: "DashBoard");

            migrationBuilder.DropIndex(
                name: "IX_Rows_DashboardId",
                schema: "DashBoard",
                table: "Rows");

            migrationBuilder.DeleteData(
                schema: "DashBoard",
                table: "WidgetGroups",
                keyColumn: "Id",
                keyValue: new Guid("28bbb537-2303-4cbb-a5d6-f2df6f2e214f"));

            migrationBuilder.DeleteData(
                schema: "DashBoard",
                table: "WidgetGroups",
                keyColumn: "Id",
                keyValue: new Guid("3e4d679b-c3a9-4c3d-9b2a-f054583cf8d0"));

            migrationBuilder.DeleteData(
                schema: "DashBoard",
                table: "WidgetGroups",
                keyColumn: "Id",
                keyValue: new Guid("43d52663-e6d9-4d39-aeff-fd8743cbcce1"));

            migrationBuilder.DeleteData(
                schema: "DashBoard",
                table: "WidgetGroups",
                keyColumn: "Id",
                keyValue: new Guid("79422e99-5fd4-402f-9a38-6c80c436dbad"));

            migrationBuilder.DeleteData(
                schema: "DashBoard",
                table: "WidgetGroups",
                keyColumn: "Id",
                keyValue: new Guid("c0a787e3-5cb6-4e6e-b652-601167b251de"));

            migrationBuilder.DropColumn(
                name: "DashboardId",
                schema: "DashBoard",
                table: "Rows");

            migrationBuilder.InsertData(
                schema: "DashBoard",
                table: "WidgetGroups",
                columns: new[] { "Id", "Author", "Changed", "Created", "IsDeleted", "ModifiedBy", "Name", "Order", "TenantId", "Version" },
                values: new object[,]
                {
                    { new Guid("7fd7f268-db82-4b9c-be2a-1e8c271739c2"), null, new DateTime(2019, 9, 23, 9, 24, 4, 383, DateTimeKind.Utc).AddTicks(3401), new DateTime(2019, 9, 23, 9, 24, 4, 383, DateTimeKind.Utc).AddTicks(2439), false, null, "Charts", 1, null, 0 },
                    { new Guid("4dfecf64-7dfb-43ed-9bce-2de01544e5f7"), null, new DateTime(2019, 9, 23, 9, 24, 4, 411, DateTimeKind.Utc).AddTicks(2585), new DateTime(2019, 9, 23, 9, 24, 4, 411, DateTimeKind.Utc).AddTicks(2576), false, null, "Reports", 2, null, 0 },
                    { new Guid("fd05e6c8-ec04-4b26-aec7-32057055477a"), null, new DateTime(2019, 9, 23, 9, 24, 4, 411, DateTimeKind.Utc).AddTicks(2919), new DateTime(2019, 9, 23, 9, 24, 4, 411, DateTimeKind.Utc).AddTicks(2918), false, null, "Custom", 3, null, 0 },
                    { new Guid("e177db49-c0af-4355-bd82-02ab0224d822"), null, new DateTime(2019, 9, 23, 9, 24, 4, 411, DateTimeKind.Utc).AddTicks(2956), new DateTime(2019, 9, 23, 9, 24, 4, 411, DateTimeKind.Utc).AddTicks(2955), false, null, "Samples", 4, null, 0 },
                    { new Guid("1b5ff978-6ab0-4ba7-9084-577c525c9adf"), null, new DateTime(2019, 9, 23, 9, 24, 4, 411, DateTimeKind.Utc).AddTicks(3027), new DateTime(2019, 9, 23, 9, 24, 4, 411, DateTimeKind.Utc).AddTicks(3025), false, null, "Custom", 5, null, 0 }
                });
        }
    }
}
