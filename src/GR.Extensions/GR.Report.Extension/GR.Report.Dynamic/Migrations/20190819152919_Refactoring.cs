using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.Report.Dynamic.Migrations
{
    public partial class Refactoring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChartType",
                schema: "Report",
                table: "DynamicReports");

            migrationBuilder.DropColumn(
                name: "ColumnNames",
                schema: "Report",
                table: "DynamicReports");

            migrationBuilder.DropColumn(
                name: "EndDateTime",
                schema: "Report",
                table: "DynamicReports");

            migrationBuilder.DropColumn(
                name: "FiltersList",
                schema: "Report",
                table: "DynamicReports");

            migrationBuilder.DropColumn(
                name: "GraphType",
                schema: "Report",
                table: "DynamicReports");

            migrationBuilder.DropColumn(
                name: "StartDateTime",
                schema: "Report",
                table: "DynamicReports");

            migrationBuilder.DropColumn(
                name: "TimeFrameEnum",
                schema: "Report",
                table: "DynamicReports");

            migrationBuilder.RenameColumn(
                name: "TableName",
                schema: "Report",
                table: "DynamicReports",
                newName: "ReportData");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReportData",
                schema: "Report",
                table: "DynamicReports",
                newName: "TableName");

            migrationBuilder.AddColumn<int>(
                name: "ChartType",
                schema: "Report",
                table: "DynamicReports",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ColumnNames",
                schema: "Report",
                table: "DynamicReports",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDateTime",
                schema: "Report",
                table: "DynamicReports",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "FiltersList",
                schema: "Report",
                table: "DynamicReports",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GraphType",
                schema: "Report",
                table: "DynamicReports",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDateTime",
                schema: "Report",
                table: "DynamicReports",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "TimeFrameEnum",
                schema: "Report",
                table: "DynamicReports",
                nullable: false,
                defaultValue: 0);
        }
    }
}
