using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.Dashboard.Migrations
{
    public partial class DashBoardDbContext_Widget_Independent : Migration
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

            migrationBuilder.AddColumn<bool>(
                name: "IsSystem",
                schema: "DashBoard",
                table: "WidgetGroups",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "RowId",
                schema: "DashBoard",
                table: "TabbedWidgets",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<Guid>(
                name: "RowId",
                schema: "DashBoard",
                table: "ReportWidgets",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<Guid>(
                name: "RowId",
                schema: "DashBoard",
                table: "ListWidgets",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<Guid>(
                name: "RowId",
                schema: "DashBoard",
                table: "CustomWidgets",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<Guid>(
                name: "RowId",
                schema: "DashBoard",
                table: "ChartWidgets",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.UpdateData(
                schema: "DashBoard",
                table: "WidgetGroups",
                keyColumn: "Id",
                keyValue: new Guid("05490637-ba30-4bbb-9165-2cbeba51995a"),
                columns: new[] { "Changed", "Created", "IsSystem" },
                values: new object[] { new DateTime(2019, 9, 29, 16, 52, 17, 656, DateTimeKind.Utc).AddTicks(9580), new DateTime(2019, 9, 29, 16, 52, 17, 656, DateTimeKind.Utc).AddTicks(9576), true });

            migrationBuilder.UpdateData(
                schema: "DashBoard",
                table: "WidgetGroups",
                keyColumn: "Id",
                keyValue: new Guid("aced3174-744a-48e4-82b9-de80d8d76114"),
                columns: new[] { "Changed", "Created", "IsSystem" },
                values: new object[] { new DateTime(2019, 9, 29, 16, 52, 17, 630, DateTimeKind.Utc).AddTicks(6134), new DateTime(2019, 9, 29, 16, 52, 17, 630, DateTimeKind.Utc).AddTicks(5584), true });

            migrationBuilder.UpdateData(
                schema: "DashBoard",
                table: "WidgetGroups",
                keyColumn: "Id",
                keyValue: new Guid("c67f5e2f-507d-4ed2-aab3-c107384b1937"),
                columns: new[] { "Changed", "Created", "IsSystem" },
                values: new object[] { new DateTime(2019, 9, 29, 16, 52, 17, 656, DateTimeKind.Utc).AddTicks(9865), new DateTime(2019, 9, 29, 16, 52, 17, 656, DateTimeKind.Utc).AddTicks(9864), true });

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "IsSystem",
                schema: "DashBoard",
                table: "WidgetGroups");

            migrationBuilder.AlterColumn<Guid>(
                name: "RowId",
                schema: "DashBoard",
                table: "TabbedWidgets",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "RowId",
                schema: "DashBoard",
                table: "ReportWidgets",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "RowId",
                schema: "DashBoard",
                table: "ListWidgets",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "RowId",
                schema: "DashBoard",
                table: "CustomWidgets",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "RowId",
                schema: "DashBoard",
                table: "ChartWidgets",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.UpdateData(
                schema: "DashBoard",
                table: "WidgetGroups",
                keyColumn: "Id",
                keyValue: new Guid("05490637-ba30-4bbb-9165-2cbeba51995a"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2019, 9, 29, 16, 34, 54, 46, DateTimeKind.Utc).AddTicks(190), new DateTime(2019, 9, 29, 16, 34, 54, 46, DateTimeKind.Utc).AddTicks(186) });

            migrationBuilder.UpdateData(
                schema: "DashBoard",
                table: "WidgetGroups",
                keyColumn: "Id",
                keyValue: new Guid("aced3174-744a-48e4-82b9-de80d8d76114"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2019, 9, 29, 16, 34, 54, 18, DateTimeKind.Utc).AddTicks(1365), new DateTime(2019, 9, 29, 16, 34, 54, 18, DateTimeKind.Utc).AddTicks(765) });

            migrationBuilder.UpdateData(
                schema: "DashBoard",
                table: "WidgetGroups",
                keyColumn: "Id",
                keyValue: new Guid("c67f5e2f-507d-4ed2-aab3-c107384b1937"),
                columns: new[] { "Changed", "Created" },
                values: new object[] { new DateTime(2019, 9, 29, 16, 34, 54, 46, DateTimeKind.Utc).AddTicks(504), new DateTime(2019, 9, 29, 16, 34, 54, 46, DateTimeKind.Utc).AddTicks(504) });

            migrationBuilder.AddForeignKey(
                name: "FK_ChartWidgets_Rows_RowId",
                schema: "DashBoard",
                table: "ChartWidgets",
                column: "RowId",
                principalSchema: "DashBoard",
                principalTable: "Rows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomWidgets_Rows_RowId",
                schema: "DashBoard",
                table: "CustomWidgets",
                column: "RowId",
                principalSchema: "DashBoard",
                principalTable: "Rows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ListWidgets_Rows_RowId",
                schema: "DashBoard",
                table: "ListWidgets",
                column: "RowId",
                principalSchema: "DashBoard",
                principalTable: "Rows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportWidgets_Rows_RowId",
                schema: "DashBoard",
                table: "ReportWidgets",
                column: "RowId",
                principalSchema: "DashBoard",
                principalTable: "Rows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TabbedWidgets_Rows_RowId",
                schema: "DashBoard",
                table: "TabbedWidgets",
                column: "RowId",
                principalSchema: "DashBoard",
                principalTable: "Rows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
