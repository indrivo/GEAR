using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.Dashboard.Migrations
{
    public partial class DashBoardDbContext_Fix_Props : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<Guid>(
                name: "ReportId",
                schema: "DashBoard",
                table: "ReportWidgets",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.InsertData(
                schema: "DashBoard",
                table: "WidgetGroups",
                columns: new[] { "Id", "Author", "Changed", "Created", "IsDeleted", "ModifiedBy", "Name", "Order", "TenantId", "Version" },
                values: new object[,]
                {
                    { new Guid("aced3174-744a-48e4-82b9-de80d8d76114"), null, new DateTime(2019, 9, 29, 16, 34, 54, 18, DateTimeKind.Utc).AddTicks(1365), new DateTime(2019, 9, 29, 16, 34, 54, 18, DateTimeKind.Utc).AddTicks(765), false, null, "Charts", 1, null, 0 },
                    { new Guid("05490637-ba30-4bbb-9165-2cbeba51995a"), null, new DateTime(2019, 9, 29, 16, 34, 54, 46, DateTimeKind.Utc).AddTicks(190), new DateTime(2019, 9, 29, 16, 34, 54, 46, DateTimeKind.Utc).AddTicks(186), false, null, "Reports", 2, null, 0 },
                    { new Guid("c67f5e2f-507d-4ed2-aab3-c107384b1937"), null, new DateTime(2019, 9, 29, 16, 34, 54, 46, DateTimeKind.Utc).AddTicks(504), new DateTime(2019, 9, 29, 16, 34, 54, 46, DateTimeKind.Utc).AddTicks(504), false, null, "Custom", 3, null, 0 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "DashBoard",
                table: "WidgetGroups",
                keyColumn: "Id",
                keyValue: new Guid("05490637-ba30-4bbb-9165-2cbeba51995a"));

            migrationBuilder.DeleteData(
                schema: "DashBoard",
                table: "WidgetGroups",
                keyColumn: "Id",
                keyValue: new Guid("aced3174-744a-48e4-82b9-de80d8d76114"));

            migrationBuilder.DeleteData(
                schema: "DashBoard",
                table: "WidgetGroups",
                keyColumn: "Id",
                keyValue: new Guid("c67f5e2f-507d-4ed2-aab3-c107384b1937"));

            migrationBuilder.DropColumn(
                name: "ReportId",
                schema: "DashBoard",
                table: "ReportWidgets");

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
    }
}
