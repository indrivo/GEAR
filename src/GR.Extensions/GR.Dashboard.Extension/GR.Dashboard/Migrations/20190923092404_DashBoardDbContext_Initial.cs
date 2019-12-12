using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.Dashboard.Migrations
{
    public partial class DashBoardDbContext_Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "DashBoard");

            migrationBuilder.CreateTable(
                name: "Rows",
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
                    Name = table.Column<string>(nullable: true),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rows", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrackAudits",
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
                    DatabaseContextName = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    TrackEventType = table.Column<int>(nullable: false),
                    RecordId = table.Column<Guid>(nullable: false),
                    TypeFullName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackAudits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WidgetGroups",
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
                    Name = table.Column<string>(nullable: false),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WidgetGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrackAuditDetails",
                schema: "DashBoard",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Author = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Changed = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: true),
                    TrackAuditId = table.Column<Guid>(nullable: false),
                    PropertyName = table.Column<string>(nullable: true),
                    PropertyType = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackAuditDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackAuditDetails_TrackAudits_TrackAuditId",
                        column: x => x.TrackAuditId,
                        principalSchema: "DashBoard",
                        principalTable: "TrackAudits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChartWidgets",
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
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    AllowCache = table.Column<bool>(nullable: false),
                    CacheRefreshSpan = table.Column<TimeSpan>(nullable: false),
                    LastRefreshTime = table.Column<DateTime>(nullable: true),
                    Order = table.Column<int>(nullable: false),
                    Template = table.Column<string>(nullable: false),
                    WidgetTemplateType = table.Column<int>(nullable: false),
                    RowId = table.Column<Guid>(nullable: false),
                    WidgetGroupId = table.Column<Guid>(nullable: false),
                    Width = table.Column<string>(nullable: true),
                    Height = table.Column<string>(nullable: true),
                    BackGroundColor = table.Column<string>(nullable: true),
                    BorderRadius = table.Column<int>(nullable: false),
                    BorderStyle = table.Column<string>(nullable: true),
                    ClassAttribute = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChartWidgets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChartWidgets_Rows_RowId",
                        column: x => x.RowId,
                        principalSchema: "DashBoard",
                        principalTable: "Rows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChartWidgets_WidgetGroups_WidgetGroupId",
                        column: x => x.WidgetGroupId,
                        principalSchema: "DashBoard",
                        principalTable: "WidgetGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomWidgets",
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
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    AllowCache = table.Column<bool>(nullable: false),
                    CacheRefreshSpan = table.Column<TimeSpan>(nullable: false),
                    LastRefreshTime = table.Column<DateTime>(nullable: true),
                    Order = table.Column<int>(nullable: false),
                    Template = table.Column<string>(nullable: false),
                    WidgetTemplateType = table.Column<int>(nullable: false),
                    RowId = table.Column<Guid>(nullable: false),
                    WidgetGroupId = table.Column<Guid>(nullable: false),
                    Width = table.Column<string>(nullable: true),
                    Height = table.Column<string>(nullable: true),
                    BackGroundColor = table.Column<string>(nullable: true),
                    BorderRadius = table.Column<int>(nullable: false),
                    BorderStyle = table.Column<string>(nullable: true),
                    ClassAttribute = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomWidgets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomWidgets_Rows_RowId",
                        column: x => x.RowId,
                        principalSchema: "DashBoard",
                        principalTable: "Rows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomWidgets_WidgetGroups_WidgetGroupId",
                        column: x => x.WidgetGroupId,
                        principalSchema: "DashBoard",
                        principalTable: "WidgetGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ListWidgets",
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
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    AllowCache = table.Column<bool>(nullable: false),
                    CacheRefreshSpan = table.Column<TimeSpan>(nullable: false),
                    LastRefreshTime = table.Column<DateTime>(nullable: true),
                    Order = table.Column<int>(nullable: false),
                    Template = table.Column<string>(nullable: false),
                    WidgetTemplateType = table.Column<int>(nullable: false),
                    RowId = table.Column<Guid>(nullable: false),
                    WidgetGroupId = table.Column<Guid>(nullable: false),
                    Width = table.Column<string>(nullable: true),
                    Height = table.Column<string>(nullable: true),
                    BackGroundColor = table.Column<string>(nullable: true),
                    BorderRadius = table.Column<int>(nullable: false),
                    BorderStyle = table.Column<string>(nullable: true),
                    ClassAttribute = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListWidgets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListWidgets_Rows_RowId",
                        column: x => x.RowId,
                        principalSchema: "DashBoard",
                        principalTable: "Rows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ListWidgets_WidgetGroups_WidgetGroupId",
                        column: x => x.WidgetGroupId,
                        principalSchema: "DashBoard",
                        principalTable: "WidgetGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportWidgets",
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
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    AllowCache = table.Column<bool>(nullable: false),
                    CacheRefreshSpan = table.Column<TimeSpan>(nullable: false),
                    LastRefreshTime = table.Column<DateTime>(nullable: true),
                    Order = table.Column<int>(nullable: false),
                    Template = table.Column<string>(nullable: false),
                    WidgetTemplateType = table.Column<int>(nullable: false),
                    RowId = table.Column<Guid>(nullable: false),
                    WidgetGroupId = table.Column<Guid>(nullable: false),
                    Width = table.Column<string>(nullable: true),
                    Height = table.Column<string>(nullable: true),
                    BackGroundColor = table.Column<string>(nullable: true),
                    BorderRadius = table.Column<int>(nullable: false),
                    BorderStyle = table.Column<string>(nullable: true),
                    ClassAttribute = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportWidgets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportWidgets_Rows_RowId",
                        column: x => x.RowId,
                        principalSchema: "DashBoard",
                        principalTable: "Rows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportWidgets_WidgetGroups_WidgetGroupId",
                        column: x => x.WidgetGroupId,
                        principalSchema: "DashBoard",
                        principalTable: "WidgetGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TabbedWidgets",
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
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    AllowCache = table.Column<bool>(nullable: false),
                    CacheRefreshSpan = table.Column<TimeSpan>(nullable: false),
                    LastRefreshTime = table.Column<DateTime>(nullable: true),
                    Order = table.Column<int>(nullable: false),
                    Template = table.Column<string>(nullable: false),
                    WidgetTemplateType = table.Column<int>(nullable: false),
                    RowId = table.Column<Guid>(nullable: false),
                    WidgetGroupId = table.Column<Guid>(nullable: false),
                    Width = table.Column<string>(nullable: true),
                    Height = table.Column<string>(nullable: true),
                    BackGroundColor = table.Column<string>(nullable: true),
                    BorderRadius = table.Column<int>(nullable: false),
                    BorderStyle = table.Column<string>(nullable: true),
                    ClassAttribute = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TabbedWidgets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TabbedWidgets_Rows_RowId",
                        column: x => x.RowId,
                        principalSchema: "DashBoard",
                        principalTable: "Rows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TabbedWidgets_WidgetGroups_WidgetGroupId",
                        column: x => x.WidgetGroupId,
                        principalSchema: "DashBoard",
                        principalTable: "WidgetGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_ChartWidgets_RowId",
                schema: "DashBoard",
                table: "ChartWidgets",
                column: "RowId");

            migrationBuilder.CreateIndex(
                name: "IX_ChartWidgets_WidgetGroupId",
                schema: "DashBoard",
                table: "ChartWidgets",
                column: "WidgetGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomWidgets_RowId",
                schema: "DashBoard",
                table: "CustomWidgets",
                column: "RowId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomWidgets_WidgetGroupId",
                schema: "DashBoard",
                table: "CustomWidgets",
                column: "WidgetGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ListWidgets_RowId",
                schema: "DashBoard",
                table: "ListWidgets",
                column: "RowId");

            migrationBuilder.CreateIndex(
                name: "IX_ListWidgets_WidgetGroupId",
                schema: "DashBoard",
                table: "ListWidgets",
                column: "WidgetGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportWidgets_RowId",
                schema: "DashBoard",
                table: "ReportWidgets",
                column: "RowId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportWidgets_WidgetGroupId",
                schema: "DashBoard",
                table: "ReportWidgets",
                column: "WidgetGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_TabbedWidgets_RowId",
                schema: "DashBoard",
                table: "TabbedWidgets",
                column: "RowId");

            migrationBuilder.CreateIndex(
                name: "IX_TabbedWidgets_WidgetGroupId",
                schema: "DashBoard",
                table: "TabbedWidgets",
                column: "WidgetGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackAuditDetails_TrackAuditId",
                schema: "DashBoard",
                table: "TrackAuditDetails",
                column: "TrackAuditId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChartWidgets",
                schema: "DashBoard");

            migrationBuilder.DropTable(
                name: "CustomWidgets",
                schema: "DashBoard");

            migrationBuilder.DropTable(
                name: "ListWidgets",
                schema: "DashBoard");

            migrationBuilder.DropTable(
                name: "ReportWidgets",
                schema: "DashBoard");

            migrationBuilder.DropTable(
                name: "TabbedWidgets",
                schema: "DashBoard");

            migrationBuilder.DropTable(
                name: "TrackAuditDetails",
                schema: "DashBoard");

            migrationBuilder.DropTable(
                name: "Rows",
                schema: "DashBoard");

            migrationBuilder.DropTable(
                name: "WidgetGroups",
                schema: "DashBoard");

            migrationBuilder.DropTable(
                name: "TrackAudits",
                schema: "DashBoard");
        }
    }
}
