using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.Entities.Migrations
{
    public partial class EntitiesDbContext_Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Entities");

            migrationBuilder.CreateTable(
                name: "EntityTypes",
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
                    Name = table.Column<string>(nullable: true),
                    MachineName = table.Column<string>(nullable: true),
                    IsSystem = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Table",
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
                    Name = table.Column<string>(nullable: false),
                    EntityType = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    IsSystem = table.Column<bool>(nullable: false),
                    IsCommon = table.Column<bool>(nullable: false),
                    IsPartOfDbContext = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Table", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TableFieldGroups",
                schema: "Entities",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    GroupName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableFieldGroups", x => x.Id);
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
                name: "TableFieldTypes",
                schema: "Entities",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    DataType = table.Column<string>(nullable: true),
                    Code = table.Column<string>(type: "char(2)", nullable: true),
                    TableFieldGroupsId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableFieldTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TableFieldTypes_TableFieldGroups_TableFieldGroupsId",
                        column: x => x.TableFieldGroupsId,
                        principalSchema: "Entities",
                        principalTable: "TableFieldGroups",
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

            migrationBuilder.CreateTable(
                name: "TableFieldConfigs",
                schema: "Entities",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Code = table.Column<string>(type: "char(4)", nullable: true),
                    TableFieldTypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableFieldConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TableFieldConfigs_TableFieldTypes_TableFieldTypeId",
                        column: x => x.TableFieldTypeId,
                        principalSchema: "Entities",
                        principalTable: "TableFieldTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TableFields",
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
                    AllowNull = table.Column<bool>(nullable: false),
                    DataType = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    DisplayName = table.Column<string>(nullable: true),
                    Synchronized = table.Column<bool>(nullable: false),
                    IsSystem = table.Column<bool>(nullable: false),
                    IsCommon = table.Column<bool>(nullable: false),
                    TableId = table.Column<Guid>(nullable: false),
                    TableFieldTypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TableFields_TableFieldTypes_TableFieldTypeId",
                        column: x => x.TableFieldTypeId,
                        principalSchema: "Entities",
                        principalTable: "TableFieldTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TableFields_Table_TableId",
                        column: x => x.TableId,
                        principalSchema: "Entities",
                        principalTable: "Table",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TableFieldConfigValues",
                schema: "Entities",
                columns: table => new
                {
                    TableModelFieldId = table.Column<Guid>(nullable: false),
                    TableFieldConfigId = table.Column<Guid>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableFieldConfigValues", x => new { x.TableModelFieldId, x.TableFieldConfigId });
                    table.ForeignKey(
                        name: "FK_TableFieldConfigValues_TableFieldConfigs_TableFieldConfigId",
                        column: x => x.TableFieldConfigId,
                        principalSchema: "Entities",
                        principalTable: "TableFieldConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TableFieldConfigValues_TableFields_TableModelFieldId",
                        column: x => x.TableModelFieldId,
                        principalSchema: "Entities",
                        principalTable: "TableFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntityTypes_TenantId",
                schema: "Entities",
                table: "EntityTypes",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Table_TenantId",
                schema: "Entities",
                table: "Table",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TableFieldConfigs_TableFieldTypeId",
                schema: "Entities",
                table: "TableFieldConfigs",
                column: "TableFieldTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TableFieldConfigValues_TableFieldConfigId",
                schema: "Entities",
                table: "TableFieldConfigValues",
                column: "TableFieldConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_TableFields_TableFieldTypeId",
                schema: "Entities",
                table: "TableFields",
                column: "TableFieldTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TableFields_TableId",
                schema: "Entities",
                table: "TableFields",
                column: "TableId");

            migrationBuilder.CreateIndex(
                name: "IX_TableFields_TenantId",
                schema: "Entities",
                table: "TableFields",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TableFieldTypes_TableFieldGroupsId",
                schema: "Entities",
                table: "TableFieldTypes",
                column: "TableFieldGroupsId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackAuditDetails_TrackAuditId",
                schema: "Entities",
                table: "TrackAuditDetails",
                column: "TrackAuditId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntityTypes",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "TableFieldConfigValues",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "TrackAuditDetails",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "TableFieldConfigs",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "TableFields",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "TrackAudits",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "TableFieldTypes",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "Table",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "TableFieldGroups",
                schema: "Entities");
        }
    }
}
