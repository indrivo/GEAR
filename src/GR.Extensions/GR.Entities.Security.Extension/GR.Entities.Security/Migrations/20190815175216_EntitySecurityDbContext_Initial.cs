using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.Entities.Security.Migrations
{
    public partial class EntitySecurityDbContext_Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Entities");

            migrationBuilder.CreateTable(
                name: "EntityFieldPermissions",
                schema: "Entities",
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
                    TableModelFieldId = table.Column<Guid>(nullable: false),
                    FieldAccessType = table.Column<int>(nullable: false),
                    ApplicationRoleId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityFieldPermissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EntityPermissions",
                schema: "Entities",
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
                    ApplicationRoleId = table.Column<Guid>(nullable: false),
                    TableModelId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityPermissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrackAudits",
                schema: "Entities",
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
                name: "EntityPermissionAccesses",
                schema: "Entities",
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
                    AccessType = table.Column<int>(nullable: false),
                    EntityPermissionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityPermissionAccesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityPermissionAccesses_EntityPermissions_EntityPermission~",
                        column: x => x.EntityPermissionId,
                        principalSchema: "Entities",
                        principalTable: "EntityPermissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrackAuditDetails",
                schema: "Entities",
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
                        principalSchema: "Entities",
                        principalTable: "TrackAudits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntityFieldPermissions_ApplicationRoleId",
                schema: "Entities",
                table: "EntityFieldPermissions",
                column: "ApplicationRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityFieldPermissions_TableModelFieldId",
                schema: "Entities",
                table: "EntityFieldPermissions",
                column: "TableModelFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityPermissionAccesses_EntityPermissionId",
                schema: "Entities",
                table: "EntityPermissionAccesses",
                column: "EntityPermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityPermissions_ApplicationRoleId",
                schema: "Entities",
                table: "EntityPermissions",
                column: "ApplicationRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityPermissions_TableModelId",
                schema: "Entities",
                table: "EntityPermissions",
                column: "TableModelId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackAuditDetails_TrackAuditId",
                schema: "Entities",
                table: "TrackAuditDetails",
                column: "TrackAuditId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntityFieldPermissions",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "EntityPermissionAccesses",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "TrackAuditDetails",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "EntityPermissions",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "TrackAudits",
                schema: "Entities");
        }
    }
}
