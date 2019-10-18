using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.Dashboard.Migrations
{
    public partial class DashBoardDbContext_Add_Active_Dash : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "DashBoard",
                table: "Dashboards",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                schema: "DashBoard",
                table: "WidgetGroups",
                columns: new[] { "Id", "Author", "Changed", "Created", "IsDeleted", "ModifiedBy", "Name", "Order", "TenantId", "Version" },
                values: new object[,]
                {
                    { new Guid("12e6ffae-7b23-474c-a190-6c374c6785fc"), null, new DateTime(2019, 9, 23, 9, 48, 50, 141, DateTimeKind.Utc).AddTicks(3942), new DateTime(2019, 9, 23, 9, 48, 50, 141, DateTimeKind.Utc).AddTicks(3129), false, null, "Charts", 1, null, 0 },
                    { new Guid("72a4e0d4-9e1e-40c5-8c41-fccc0c16b653"), null, new DateTime(2019, 9, 23, 9, 48, 50, 171, DateTimeKind.Utc).AddTicks(3797), new DateTime(2019, 9, 23, 9, 48, 50, 171, DateTimeKind.Utc).AddTicks(3792), false, null, "Reports", 2, null, 0 },
                    { new Guid("ef761fae-ff74-4f35-9e4e-5a1e74dea628"), null, new DateTime(2019, 9, 23, 9, 48, 50, 171, DateTimeKind.Utc).AddTicks(4109), new DateTime(2019, 9, 23, 9, 48, 50, 171, DateTimeKind.Utc).AddTicks(4108), false, null, "Custom", 3, null, 0 },
                    { new Guid("d72ff788-0ecd-41a0-962b-b350ef40b6fc"), null, new DateTime(2019, 9, 23, 9, 48, 50, 171, DateTimeKind.Utc).AddTicks(4194), new DateTime(2019, 9, 23, 9, 48, 50, 171, DateTimeKind.Utc).AddTicks(4193), false, null, "Samples", 4, null, 0 },
                    { new Guid("354baeff-f03b-448b-9f52-824d1fab4c7e"), null, new DateTime(2019, 9, 23, 9, 48, 50, 171, DateTimeKind.Utc).AddTicks(4218), new DateTime(2019, 9, 23, 9, 48, 50, 171, DateTimeKind.Utc).AddTicks(4218), false, null, "Custom", 5, null, 0 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "DashBoard",
                table: "WidgetGroups",
                keyColumn: "Id",
                keyValue: new Guid("12e6ffae-7b23-474c-a190-6c374c6785fc"));

            migrationBuilder.DeleteData(
                schema: "DashBoard",
                table: "WidgetGroups",
                keyColumn: "Id",
                keyValue: new Guid("354baeff-f03b-448b-9f52-824d1fab4c7e"));

            migrationBuilder.DeleteData(
                schema: "DashBoard",
                table: "WidgetGroups",
                keyColumn: "Id",
                keyValue: new Guid("72a4e0d4-9e1e-40c5-8c41-fccc0c16b653"));

            migrationBuilder.DeleteData(
                schema: "DashBoard",
                table: "WidgetGroups",
                keyColumn: "Id",
                keyValue: new Guid("d72ff788-0ecd-41a0-962b-b350ef40b6fc"));

            migrationBuilder.DeleteData(
                schema: "DashBoard",
                table: "WidgetGroups",
                keyColumn: "Id",
                keyValue: new Guid("ef761fae-ff74-4f35-9e4e-5a1e74dea628"));

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "DashBoard",
                table: "Dashboards");

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
        }
    }
}
