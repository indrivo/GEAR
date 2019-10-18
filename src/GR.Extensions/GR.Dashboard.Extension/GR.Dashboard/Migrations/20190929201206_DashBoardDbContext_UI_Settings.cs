using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.Dashboard.Migrations
{
    public partial class DashBoardDbContext_UI_Settings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChartWidgets_Rows_RowId",
                schema: "DashBoard",
                table: "ChartWidgets");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomWidgets_Rows_RowId",
                schema: "DashBoard",
                table: "CustomWidgets");

            migrationBuilder.DropForeignKey(
                name: "FK_ListWidgets_Rows_RowId",
                schema: "DashBoard",
                table: "ListWidgets");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportWidgets_Rows_RowId",
                schema: "DashBoard",
                table: "ReportWidgets");

            migrationBuilder.DropForeignKey(
                name: "FK_TabbedWidgets_Rows_RowId",
                schema: "DashBoard",
                table: "TabbedWidgets");

            migrationBuilder.DropIndex(
                name: "IX_TabbedWidgets_RowId",
                schema: "DashBoard",
                table: "TabbedWidgets");

            migrationBuilder.DropIndex(
                name: "IX_ReportWidgets_RowId",
                schema: "DashBoard",
                table: "ReportWidgets");

            migrationBuilder.DropIndex(
                name: "IX_ListWidgets_RowId",
                schema: "DashBoard",
                table: "ListWidgets");

            migrationBuilder.DropIndex(
                name: "IX_CustomWidgets_RowId",
                schema: "DashBoard",
                table: "CustomWidgets");

            migrationBuilder.DropIndex(
                name: "IX_ChartWidgets_RowId",
                schema: "DashBoard",
                table: "ChartWidgets");

            migrationBuilder.DropColumn(
                name: "BackGroundColor",
                schema: "DashBoard",
                table: "TabbedWidgets");

            migrationBuilder.DropColumn(
                name: "BorderRadius",
                schema: "DashBoard",
                table: "TabbedWidgets");

            migrationBuilder.DropColumn(
                name: "BorderStyle",
                schema: "DashBoard",
                table: "TabbedWidgets");

            migrationBuilder.DropColumn(
                name: "ClassAttribute",
                schema: "DashBoard",
                table: "TabbedWidgets");

            migrationBuilder.DropColumn(
                name: "Height",
                schema: "DashBoard",
                table: "TabbedWidgets");

            migrationBuilder.DropColumn(
                name: "Order",
                schema: "DashBoard",
                table: "TabbedWidgets");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "DashBoard",
                table: "TabbedWidgets");

            migrationBuilder.DropColumn(
                name: "Width",
                schema: "DashBoard",
                table: "TabbedWidgets");

            migrationBuilder.DropColumn(
                name: "BackGroundColor",
                schema: "DashBoard",
                table: "ReportWidgets");

            migrationBuilder.DropColumn(
                name: "BorderRadius",
                schema: "DashBoard",
                table: "ReportWidgets");

            migrationBuilder.DropColumn(
                name: "BorderStyle",
                schema: "DashBoard",
                table: "ReportWidgets");

            migrationBuilder.DropColumn(
                name: "ClassAttribute",
                schema: "DashBoard",
                table: "ReportWidgets");

            migrationBuilder.DropColumn(
                name: "Height",
                schema: "DashBoard",
                table: "ReportWidgets");

            migrationBuilder.DropColumn(
                name: "Order",
                schema: "DashBoard",
                table: "ReportWidgets");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "DashBoard",
                table: "ReportWidgets");

            migrationBuilder.DropColumn(
                name: "Width",
                schema: "DashBoard",
                table: "ReportWidgets");

            migrationBuilder.DropColumn(
                name: "BackGroundColor",
                schema: "DashBoard",
                table: "ListWidgets");

            migrationBuilder.DropColumn(
                name: "BorderRadius",
                schema: "DashBoard",
                table: "ListWidgets");

            migrationBuilder.DropColumn(
                name: "BorderStyle",
                schema: "DashBoard",
                table: "ListWidgets");

            migrationBuilder.DropColumn(
                name: "ClassAttribute",
                schema: "DashBoard",
                table: "ListWidgets");

            migrationBuilder.DropColumn(
                name: "Height",
                schema: "DashBoard",
                table: "ListWidgets");

            migrationBuilder.DropColumn(
                name: "Order",
                schema: "DashBoard",
                table: "ListWidgets");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "DashBoard",
                table: "ListWidgets");

            migrationBuilder.DropColumn(
                name: "Width",
                schema: "DashBoard",
                table: "ListWidgets");

            migrationBuilder.DropColumn(
                name: "BackGroundColor",
                schema: "DashBoard",
                table: "CustomWidgets");

            migrationBuilder.DropColumn(
                name: "BorderRadius",
                schema: "DashBoard",
                table: "CustomWidgets");

            migrationBuilder.DropColumn(
                name: "BorderStyle",
                schema: "DashBoard",
                table: "CustomWidgets");

            migrationBuilder.DropColumn(
                name: "ClassAttribute",
                schema: "DashBoard",
                table: "CustomWidgets");

            migrationBuilder.DropColumn(
                name: "Height",
                schema: "DashBoard",
                table: "CustomWidgets");

            migrationBuilder.DropColumn(
                name: "Order",
                schema: "DashBoard",
                table: "CustomWidgets");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "DashBoard",
                table: "CustomWidgets");

            migrationBuilder.DropColumn(
                name: "Width",
                schema: "DashBoard",
                table: "CustomWidgets");

            migrationBuilder.DropColumn(
                name: "BackGroundColor",
                schema: "DashBoard",
                table: "ChartWidgets");

            migrationBuilder.DropColumn(
                name: "BorderRadius",
                schema: "DashBoard",
                table: "ChartWidgets");

            migrationBuilder.DropColumn(
                name: "BorderStyle",
                schema: "DashBoard",
                table: "ChartWidgets");

            migrationBuilder.DropColumn(
                name: "ClassAttribute",
                schema: "DashBoard",
                table: "ChartWidgets");

            migrationBuilder.DropColumn(
                name: "Height",
                schema: "DashBoard",
                table: "ChartWidgets");

            migrationBuilder.DropColumn(
                name: "Order",
                schema: "DashBoard",
                table: "ChartWidgets");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "DashBoard",
                table: "ChartWidgets");

            migrationBuilder.DropColumn(
                name: "Width",
                schema: "DashBoard",
                table: "ChartWidgets");

            migrationBuilder.AddColumn<bool>(
                name: "IsSystem",
                schema: "DashBoard",
                table: "TabbedWidgets",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSystem",
                schema: "DashBoard",
                table: "ReportWidgets",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSystem",
                schema: "DashBoard",
                table: "ListWidgets",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSystem",
                schema: "DashBoard",
                table: "CustomWidgets",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSystem",
                schema: "DashBoard",
                table: "ChartWidgets",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "RowChartWidgets",
                schema: "DashBoard",
                columns: table => new
                {
                    RowId = table.Column<Guid>(nullable: false),
                    ChartWidgetId = table.Column<Guid>(nullable: false),
                    Width = table.Column<string>(nullable: true),
                    Height = table.Column<string>(nullable: true),
                    BackGroundColor = table.Column<string>(nullable: true),
                    BorderRadius = table.Column<int>(nullable: false),
                    BorderStyle = table.Column<string>(nullable: true),
                    ClassAttribute = table.Column<string>(nullable: true),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RowChartWidgets", x => new { x.ChartWidgetId, x.RowId });
                    table.ForeignKey(
                        name: "FK_RowChartWidgets_ChartWidgets_ChartWidgetId",
                        column: x => x.ChartWidgetId,
                        principalSchema: "DashBoard",
                        principalTable: "ChartWidgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RowChartWidgets_Rows_RowId",
                        column: x => x.RowId,
                        principalSchema: "DashBoard",
                        principalTable: "Rows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RowCustomWidgets",
                schema: "DashBoard",
                columns: table => new
                {
                    RowId = table.Column<Guid>(nullable: false),
                    CustomWidgetId = table.Column<Guid>(nullable: false),
                    Width = table.Column<string>(nullable: true),
                    Height = table.Column<string>(nullable: true),
                    BackGroundColor = table.Column<string>(nullable: true),
                    BorderRadius = table.Column<int>(nullable: false),
                    BorderStyle = table.Column<string>(nullable: true),
                    ClassAttribute = table.Column<string>(nullable: true),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RowCustomWidgets", x => new { x.CustomWidgetId, x.RowId });
                    table.ForeignKey(
                        name: "FK_RowCustomWidgets_CustomWidgets_CustomWidgetId",
                        column: x => x.CustomWidgetId,
                        principalSchema: "DashBoard",
                        principalTable: "CustomWidgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RowCustomWidgets_Rows_RowId",
                        column: x => x.RowId,
                        principalSchema: "DashBoard",
                        principalTable: "Rows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RowReportWidgets",
                schema: "DashBoard",
                columns: table => new
                {
                    RowId = table.Column<Guid>(nullable: false),
                    ReportWidgetId = table.Column<Guid>(nullable: false),
                    Width = table.Column<string>(nullable: true),
                    Height = table.Column<string>(nullable: true),
                    BackGroundColor = table.Column<string>(nullable: true),
                    BorderRadius = table.Column<int>(nullable: false),
                    BorderStyle = table.Column<string>(nullable: true),
                    ClassAttribute = table.Column<string>(nullable: true),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RowReportWidgets", x => new { x.ReportWidgetId, x.RowId });
                    table.ForeignKey(
                        name: "FK_RowReportWidgets_ReportWidgets_ReportWidgetId",
                        column: x => x.ReportWidgetId,
                        principalSchema: "DashBoard",
                        principalTable: "ReportWidgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RowReportWidgets_Rows_RowId",
                        column: x => x.RowId,
                        principalSchema: "DashBoard",
                        principalTable: "Rows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_RowChartWidgets_RowId",
                schema: "DashBoard",
                table: "RowChartWidgets",
                column: "RowId");

            migrationBuilder.CreateIndex(
                name: "IX_RowCustomWidgets_RowId",
                schema: "DashBoard",
                table: "RowCustomWidgets",
                column: "RowId");

            migrationBuilder.CreateIndex(
                name: "IX_RowReportWidgets_RowId",
                schema: "DashBoard",
                table: "RowReportWidgets",
                column: "RowId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RowChartWidgets",
                schema: "DashBoard");

            migrationBuilder.DropTable(
                name: "RowCustomWidgets",
                schema: "DashBoard");

            migrationBuilder.DropTable(
                name: "RowReportWidgets",
                schema: "DashBoard");

            migrationBuilder.DropColumn(
                name: "IsSystem",
                schema: "DashBoard",
                table: "TabbedWidgets");

            migrationBuilder.DropColumn(
                name: "IsSystem",
                schema: "DashBoard",
                table: "ReportWidgets");

            migrationBuilder.DropColumn(
                name: "IsSystem",
                schema: "DashBoard",
                table: "ListWidgets");

            migrationBuilder.DropColumn(
                name: "IsSystem",
                schema: "DashBoard",
                table: "CustomWidgets");

            migrationBuilder.DropColumn(
                name: "IsSystem",
                schema: "DashBoard",
                table: "ChartWidgets");

            migrationBuilder.AddColumn<string>(
                name: "BackGroundColor",
                schema: "DashBoard",
                table: "TabbedWidgets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BorderRadius",
                schema: "DashBoard",
                table: "TabbedWidgets",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BorderStyle",
                schema: "DashBoard",
                table: "TabbedWidgets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClassAttribute",
                schema: "DashBoard",
                table: "TabbedWidgets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Height",
                schema: "DashBoard",
                table: "TabbedWidgets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                schema: "DashBoard",
                table: "TabbedWidgets",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "DashBoard",
                table: "TabbedWidgets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Width",
                schema: "DashBoard",
                table: "TabbedWidgets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BackGroundColor",
                schema: "DashBoard",
                table: "ReportWidgets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BorderRadius",
                schema: "DashBoard",
                table: "ReportWidgets",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BorderStyle",
                schema: "DashBoard",
                table: "ReportWidgets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClassAttribute",
                schema: "DashBoard",
                table: "ReportWidgets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Height",
                schema: "DashBoard",
                table: "ReportWidgets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                schema: "DashBoard",
                table: "ReportWidgets",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "DashBoard",
                table: "ReportWidgets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Width",
                schema: "DashBoard",
                table: "ReportWidgets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BackGroundColor",
                schema: "DashBoard",
                table: "ListWidgets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BorderRadius",
                schema: "DashBoard",
                table: "ListWidgets",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BorderStyle",
                schema: "DashBoard",
                table: "ListWidgets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClassAttribute",
                schema: "DashBoard",
                table: "ListWidgets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Height",
                schema: "DashBoard",
                table: "ListWidgets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                schema: "DashBoard",
                table: "ListWidgets",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "DashBoard",
                table: "ListWidgets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Width",
                schema: "DashBoard",
                table: "ListWidgets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BackGroundColor",
                schema: "DashBoard",
                table: "CustomWidgets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BorderRadius",
                schema: "DashBoard",
                table: "CustomWidgets",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BorderStyle",
                schema: "DashBoard",
                table: "CustomWidgets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClassAttribute",
                schema: "DashBoard",
                table: "CustomWidgets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Height",
                schema: "DashBoard",
                table: "CustomWidgets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                schema: "DashBoard",
                table: "CustomWidgets",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "DashBoard",
                table: "CustomWidgets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Width",
                schema: "DashBoard",
                table: "CustomWidgets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BackGroundColor",
                schema: "DashBoard",
                table: "ChartWidgets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BorderRadius",
                schema: "DashBoard",
                table: "ChartWidgets",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BorderStyle",
                schema: "DashBoard",
                table: "ChartWidgets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClassAttribute",
                schema: "DashBoard",
                table: "ChartWidgets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Height",
                schema: "DashBoard",
                table: "ChartWidgets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                schema: "DashBoard",
                table: "ChartWidgets",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "DashBoard",
                table: "ChartWidgets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Width",
                schema: "DashBoard",
                table: "ChartWidgets",
                nullable: true);

            migrationBuilder.UpdateData(
                schema: "DashBoard",
                table: "WidgetGroups",
                keyColumn: "Id",
                keyValue: new Guid("05490637-ba30-4bbb-9165-2cbeba51995a"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2019, 9, 29, 16, 52, 17, 656, DateTimeKind.Utc).AddTicks(9580), new DateTime(2019, 9, 29, 16, 52, 17, 656, DateTimeKind.Utc).AddTicks(9576) });

            migrationBuilder.UpdateData(
                schema: "DashBoard",
                table: "WidgetGroups",
                keyColumn: "Id",
                keyValue: new Guid("aced3174-744a-48e4-82b9-de80d8d76114"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2019, 9, 29, 16, 52, 17, 630, DateTimeKind.Utc).AddTicks(6134), new DateTime(2019, 9, 29, 16, 52, 17, 630, DateTimeKind.Utc).AddTicks(5584) });

            migrationBuilder.UpdateData(
                schema: "DashBoard",
                table: "WidgetGroups",
                keyColumn: "Id",
                keyValue: new Guid("c67f5e2f-507d-4ed2-aab3-c107384b1937"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2019, 9, 29, 16, 52, 17, 656, DateTimeKind.Utc).AddTicks(9865), new DateTime(2019, 9, 29, 16, 52, 17, 656, DateTimeKind.Utc).AddTicks(9864) });

            migrationBuilder.CreateIndex(
                name: "IX_TabbedWidgets_RowId",
                schema: "DashBoard",
                table: "TabbedWidgets",
                column: "RowId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportWidgets_RowId",
                schema: "DashBoard",
                table: "ReportWidgets",
                column: "RowId");

            migrationBuilder.CreateIndex(
                name: "IX_ListWidgets_RowId",
                schema: "DashBoard",
                table: "ListWidgets",
                column: "RowId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomWidgets_RowId",
                schema: "DashBoard",
                table: "CustomWidgets",
                column: "RowId");

            migrationBuilder.CreateIndex(
                name: "IX_ChartWidgets_RowId",
                schema: "DashBoard",
                table: "ChartWidgets",
                column: "RowId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChartWidgets_Rows_RowId",
                schema: "DashBoard",
                table: "ChartWidgets",
                column: "RowId",
                principalSchema: "DashBoard",
                principalTable: "Rows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomWidgets_Rows_RowId",
                schema: "DashBoard",
                table: "CustomWidgets",
                column: "RowId",
                principalSchema: "DashBoard",
                principalTable: "Rows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ListWidgets_Rows_RowId",
                schema: "DashBoard",
                table: "ListWidgets",
                column: "RowId",
                principalSchema: "DashBoard",
                principalTable: "Rows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportWidgets_Rows_RowId",
                schema: "DashBoard",
                table: "ReportWidgets",
                column: "RowId",
                principalSchema: "DashBoard",
                principalTable: "Rows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TabbedWidgets_Rows_RowId",
                schema: "DashBoard",
                table: "TabbedWidgets",
                column: "RowId",
                principalSchema: "DashBoard",
                principalTable: "Rows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
