using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.Dashboard.Migrations
{
    public partial class DashBoardDbContext_AddWidgetAcl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WidgetAcls",
                schema: "DashBoard",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(nullable: false),
                    RowId = table.Column<Guid>(nullable: false),
                    WidgetId = table.Column<Guid>(nullable: false),
                    Allow = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WidgetAcls", x => new { x.RowId, x.WidgetId, x.RoleId });
                });

            migrationBuilder.UpdateData(
                schema: "DashBoard",
                table: "WidgetGroups",
                keyColumn: "Id",
                keyValue: new Guid("05490637-ba30-4bbb-9165-2cbeba51995a"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2019, 10, 23, 16, 15, 17, 841, DateTimeKind.Utc).AddTicks(5896), new DateTime(2019, 10, 23, 16, 15, 17, 841, DateTimeKind.Utc).AddTicks(5885) });

            migrationBuilder.UpdateData(
                schema: "DashBoard",
                table: "WidgetGroups",
                keyColumn: "Id",
                keyValue: new Guid("aced3174-744a-48e4-82b9-de80d8d76114"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2019, 10, 23, 16, 15, 17, 800, DateTimeKind.Utc).AddTicks(1954), new DateTime(2019, 10, 23, 16, 15, 17, 800, DateTimeKind.Utc).AddTicks(1002) });

            migrationBuilder.UpdateData(
                schema: "DashBoard",
                table: "WidgetGroups",
                keyColumn: "Id",
                keyValue: new Guid("c67f5e2f-507d-4ed2-aab3-c107384b1937"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2019, 10, 23, 16, 15, 17, 841, DateTimeKind.Utc).AddTicks(6354), new DateTime(2019, 10, 23, 16, 15, 17, 841, DateTimeKind.Utc).AddTicks(6353) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WidgetAcls",
                schema: "DashBoard");

            migrationBuilder.UpdateData(
                schema: "DashBoard",
                table: "WidgetGroups",
                keyColumn: "Id",
                keyValue: new Guid("05490637-ba30-4bbb-9165-2cbeba51995a"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2019, 9, 29, 20, 12, 6, 456, DateTimeKind.Utc).AddTicks(518), new DateTime(2019, 9, 29, 20, 12, 6, 456, DateTimeKind.Utc).AddTicks(510) });

            migrationBuilder.UpdateData(
                schema: "DashBoard",
                table: "WidgetGroups",
                keyColumn: "Id",
                keyValue: new Guid("aced3174-744a-48e4-82b9-de80d8d76114"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2019, 9, 29, 20, 12, 6, 425, DateTimeKind.Utc).AddTicks(8413), new DateTime(2019, 9, 29, 20, 12, 6, 425, DateTimeKind.Utc).AddTicks(7821) });

            migrationBuilder.UpdateData(
                schema: "DashBoard",
                table: "WidgetGroups",
                keyColumn: "Id",
                keyValue: new Guid("c67f5e2f-507d-4ed2-aab3-c107384b1937"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2019, 9, 29, 20, 12, 6, 456, DateTimeKind.Utc).AddTicks(935), new DateTime(2019, 9, 29, 20, 12, 6, 456, DateTimeKind.Utc).AddTicks(935) });
        }
    }
}
