using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.Report.Dynamic.Migrations
{
    public partial class Report_Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Report");

            migrationBuilder.CreateTable(
                name: "DynamicReportsFolders",
                schema: "Report",
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
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicReportsFolders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DynamicReports",
                schema: "Report",
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
                    Name = table.Column<string>(nullable: true),
                    TableName = table.Column<string>(nullable: true),
                    ColumnNames = table.Column<string>(nullable: true),
                    StartDateTime = table.Column<DateTime>(nullable: false),
                    EndDateTime = table.Column<DateTime>(nullable: false),
                    FiltersList = table.Column<string>(nullable: true),
                    GraphType = table.Column<int>(nullable: false),
                    ChartType = table.Column<int>(nullable: false),
                    TimeFrameEnum = table.Column<int>(nullable: false),
                    DynamicReportFolderId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DynamicReports_DynamicReportsFolders_DynamicReportFolderId",
                        column: x => x.DynamicReportFolderId,
                        principalSchema: "Report",
                        principalTable: "DynamicReportsFolders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DynamicReports_DynamicReportFolderId",
                schema: "Report",
                table: "DynamicReports",
                column: "DynamicReportFolderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DynamicReports",
                schema: "Report");

            migrationBuilder.DropTable(
                name: "DynamicReportsFolders",
                schema: "Report");
        }
    }
}
