using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.Forms.Migrations
{
    public partial class FormsInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Forms");

            migrationBuilder.CreateTable(
                name: "ColumnFields",
                schema: "Forms",
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
                    ColumnId = table.Column<Guid>(nullable: false),
                    FieldId = table.Column<Guid>(nullable: false),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColumnFields", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Configs",
                schema: "Forms",
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
                    Fieldset = table.Column<bool>(nullable: false),
                    Legend = table.Column<string>(nullable: true),
                    InputGroup = table.Column<bool>(nullable: false),
                    Width = table.Column<string>(nullable: true),
                    Label = table.Column<string>(nullable: true),
                    HideLabel = table.Column<bool>(nullable: false),
                    Editable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FormTypes",
                schema: "Forms",
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
                    Description = table.Column<string>(nullable: true),
                    Code = table.Column<string>(type: "char(4)", nullable: true),
                    IsSystem = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Meta",
                schema: "Forms",
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
                    Group = table.Column<string>(nullable: true),
                    Icon = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meta", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RowColumns",
                schema: "Forms",
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
                    RowId = table.Column<Guid>(nullable: false),
                    ColumnId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RowColumns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                schema: "Forms",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Author = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Changed = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Version = table.Column<int>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StageRows",
                schema: "Forms",
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
                    StageId = table.Column<Guid>(nullable: false),
                    RowId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StageRows", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrackAudits",
                schema: "Forms",
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
                name: "DisabledAttrs",
                schema: "Forms",
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
                    ConfigId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisabledAttrs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DisabledAttrs_Configs_ConfigId",
                        column: x => x.ConfigId,
                        principalSchema: "Forms",
                        principalTable: "Configs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Forms",
                schema: "Forms",
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
                    Description = table.Column<string>(nullable: true),
                    TableId = table.Column<Guid>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: false),
                    SettingsId = table.Column<Guid>(nullable: false),
                    RedirectUrl = table.Column<string>(nullable: true),
                    PostUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Forms_Settings_SettingsId",
                        column: x => x.SettingsId,
                        principalSchema: "Forms",
                        principalTable: "Settings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Forms_FormTypes_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "Forms",
                        principalTable: "FormTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TrackAuditDetails",
                schema: "Forms",
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
                        principalSchema: "Forms",
                        principalTable: "TrackAudits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Stages",
                schema: "Forms",
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
                    SettingsId = table.Column<Guid>(nullable: true),
                    FormId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stages_Forms_FormId",
                        column: x => x.FormId,
                        principalSchema: "Forms",
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Stages_Settings_SettingsId",
                        column: x => x.SettingsId,
                        principalSchema: "Forms",
                        principalTable: "Settings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Rows",
                schema: "Forms",
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
                    ConfigId = table.Column<Guid>(nullable: false),
                    FormId = table.Column<Guid>(nullable: false),
                    StageId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rows_Configs_ConfigId",
                        column: x => x.ConfigId,
                        principalSchema: "Forms",
                        principalTable: "Configs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rows_Forms_FormId",
                        column: x => x.FormId,
                        principalSchema: "Forms",
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rows_Stages_StageId",
                        column: x => x.StageId,
                        principalSchema: "Forms",
                        principalTable: "Stages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Columns",
                schema: "Forms",
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
                    ConfigId = table.Column<Guid>(nullable: false),
                    ClassName = table.Column<string>(nullable: true),
                    FormId = table.Column<Guid>(nullable: false),
                    RowId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Columns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Columns_Configs_ConfigId",
                        column: x => x.ConfigId,
                        principalSchema: "Forms",
                        principalTable: "Configs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Columns_Forms_FormId",
                        column: x => x.FormId,
                        principalSchema: "Forms",
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Columns_Rows_RowId",
                        column: x => x.RowId,
                        principalSchema: "Forms",
                        principalTable: "Rows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Fields",
                schema: "Forms",
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
                    Tag = table.Column<string>(nullable: true),
                    ConfigId = table.Column<Guid>(nullable: false),
                    FMap = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    MetaId = table.Column<Guid>(nullable: false),
                    FormId = table.Column<Guid>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    TableFieldId = table.Column<Guid>(nullable: true),
                    ColumnId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fields_Columns_ColumnId",
                        column: x => x.ColumnId,
                        principalSchema: "Forms",
                        principalTable: "Columns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fields_Configs_ConfigId",
                        column: x => x.ConfigId,
                        principalSchema: "Forms",
                        principalTable: "Configs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Fields_Forms_FormId",
                        column: x => x.FormId,
                        principalSchema: "Forms",
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Fields_Meta_MetaId",
                        column: x => x.MetaId,
                        principalSchema: "Forms",
                        principalTable: "Meta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Attrs",
                schema: "Forms",
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
                    Key = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    RowId = table.Column<Guid>(nullable: true),
                    FieldId = table.Column<Guid>(nullable: true),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attrs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attrs_Fields_FieldId",
                        column: x => x.FieldId,
                        principalSchema: "Forms",
                        principalTable: "Fields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Attrs_Rows_RowId",
                        column: x => x.RowId,
                        principalSchema: "Forms",
                        principalTable: "Rows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormFieldEvents",
                schema: "Forms",
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
                    Handler = table.Column<string>(nullable: false),
                    Event = table.Column<int>(nullable: false),
                    FieldId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormFieldEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormFieldEvents_Fields_FieldId",
                        column: x => x.FieldId,
                        principalSchema: "Forms",
                        principalTable: "Fields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Options",
                schema: "Forms",
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
                    Label = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    Selected = table.Column<bool>(nullable: false),
                    FieldId = table.Column<Guid>(nullable: false),
                    TypeValue = table.Column<string>(nullable: true),
                    TypeLabel = table.Column<string>(nullable: true),
                    ClassName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Options", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Options_Fields_FieldId",
                        column: x => x.FieldId,
                        principalSchema: "Forms",
                        principalTable: "Fields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attrs_FieldId",
                schema: "Forms",
                table: "Attrs",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_Attrs_RowId",
                schema: "Forms",
                table: "Attrs",
                column: "RowId");

            migrationBuilder.CreateIndex(
                name: "IX_Columns_ConfigId",
                schema: "Forms",
                table: "Columns",
                column: "ConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_Columns_FormId",
                schema: "Forms",
                table: "Columns",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_Columns_RowId",
                schema: "Forms",
                table: "Columns",
                column: "RowId");

            migrationBuilder.CreateIndex(
                name: "IX_DisabledAttrs_ConfigId",
                schema: "Forms",
                table: "DisabledAttrs",
                column: "ConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_Fields_ColumnId",
                schema: "Forms",
                table: "Fields",
                column: "ColumnId");

            migrationBuilder.CreateIndex(
                name: "IX_Fields_ConfigId",
                schema: "Forms",
                table: "Fields",
                column: "ConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_Fields_FormId",
                schema: "Forms",
                table: "Fields",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_Fields_MetaId",
                schema: "Forms",
                table: "Fields",
                column: "MetaId");

            migrationBuilder.CreateIndex(
                name: "IX_FormFieldEvents_FieldId",
                schema: "Forms",
                table: "FormFieldEvents",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_SettingsId",
                schema: "Forms",
                table: "Forms",
                column: "SettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_TenantId",
                schema: "Forms",
                table: "Forms",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_TypeId",
                schema: "Forms",
                table: "Forms",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Options_FieldId",
                schema: "Forms",
                table: "Options",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_Rows_ConfigId",
                schema: "Forms",
                table: "Rows",
                column: "ConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_Rows_FormId",
                schema: "Forms",
                table: "Rows",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_Rows_StageId",
                schema: "Forms",
                table: "Rows",
                column: "StageId");

            migrationBuilder.CreateIndex(
                name: "IX_Stages_FormId",
                schema: "Forms",
                table: "Stages",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_Stages_SettingsId",
                schema: "Forms",
                table: "Stages",
                column: "SettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackAuditDetails_TrackAuditId",
                schema: "Forms",
                table: "TrackAuditDetails",
                column: "TrackAuditId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attrs",
                schema: "Forms");

            migrationBuilder.DropTable(
                name: "ColumnFields",
                schema: "Forms");

            migrationBuilder.DropTable(
                name: "DisabledAttrs",
                schema: "Forms");

            migrationBuilder.DropTable(
                name: "FormFieldEvents",
                schema: "Forms");

            migrationBuilder.DropTable(
                name: "Options",
                schema: "Forms");

            migrationBuilder.DropTable(
                name: "RowColumns",
                schema: "Forms");

            migrationBuilder.DropTable(
                name: "StageRows",
                schema: "Forms");

            migrationBuilder.DropTable(
                name: "TrackAuditDetails",
                schema: "Forms");

            migrationBuilder.DropTable(
                name: "Fields",
                schema: "Forms");

            migrationBuilder.DropTable(
                name: "TrackAudits",
                schema: "Forms");

            migrationBuilder.DropTable(
                name: "Columns",
                schema: "Forms");

            migrationBuilder.DropTable(
                name: "Meta",
                schema: "Forms");

            migrationBuilder.DropTable(
                name: "Rows",
                schema: "Forms");

            migrationBuilder.DropTable(
                name: "Configs",
                schema: "Forms");

            migrationBuilder.DropTable(
                name: "Stages",
                schema: "Forms");

            migrationBuilder.DropTable(
                name: "Forms",
                schema: "Forms");

            migrationBuilder.DropTable(
                name: "Settings",
                schema: "Forms");

            migrationBuilder.DropTable(
                name: "FormTypes",
                schema: "Forms");
        }
    }
}
