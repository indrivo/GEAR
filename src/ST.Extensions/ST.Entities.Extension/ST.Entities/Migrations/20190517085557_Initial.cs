using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ST.Entities.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Entities");

            migrationBuilder.CreateTable(
                name: "BlockCategories",
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
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockCategories", x => x.Id);
                });

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
                name: "PageSettings",
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
                    Identifier = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Icon = table.Column<string>(nullable: true),
                    HtmlCode = table.Column<string>(nullable: true),
                    CssCode = table.Column<string>(nullable: true),
                    JsCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PageTypes",
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
                    Description = table.Column<string>(nullable: true),
                    IsSystem = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageTypes", x => x.Id);
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
                name: "Templates",
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
                    IdentifierName = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Templates", x => x.Id);
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
                name: "Pages",
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
                    PageTypeId = table.Column<Guid>(nullable: false),
                    SettingsId = table.Column<Guid>(nullable: true),
                    Path = table.Column<string>(nullable: true),
                    IsSystem = table.Column<bool>(nullable: false),
                    IsLayout = table.Column<bool>(nullable: false),
                    LayoutId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pages_Pages_LayoutId",
                        column: x => x.LayoutId,
                        principalSchema: "Entities",
                        principalTable: "Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pages_PageTypes_PageTypeId",
                        column: x => x.PageTypeId,
                        principalSchema: "Entities",
                        principalTable: "PageTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pages_PageSettings_SettingsId",
                        column: x => x.SettingsId,
                        principalSchema: "Entities",
                        principalTable: "PageSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Blocks",
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
                    Description = table.Column<string>(nullable: true),
                    BlockCategoryId = table.Column<Guid>(nullable: false),
                    FaIcon = table.Column<string>(nullable: true),
                    HtmlCode = table.Column<string>(nullable: true),
                    CssCode = table.Column<string>(nullable: true),
                    TableModelId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Blocks_BlockCategories_BlockCategoryId",
                        column: x => x.BlockCategoryId,
                        principalSchema: "Entities",
                        principalTable: "BlockCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Blocks_Table_TableModelId",
                        column: x => x.TableModelId,
                        principalSchema: "Entities",
                        principalTable: "Table",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    table.ForeignKey(
                        name: "FK_EntityPermissions_Table_TableModelId",
                        column: x => x.TableModelId,
                        principalSchema: "Entities",
                        principalTable: "Table",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ViewModels",
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
                    TableModelId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViewModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ViewModels_Table_TableModelId",
                        column: x => x.TableModelId,
                        principalSchema: "Entities",
                        principalTable: "Table",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "PageScripts",
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
                    PageId = table.Column<Guid>(nullable: false),
                    Script = table.Column<string>(nullable: true),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageScripts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageScripts_Pages_PageId",
                        column: x => x.PageId,
                        principalSchema: "Entities",
                        principalTable: "Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PageStyles",
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
                    PageId = table.Column<Guid>(nullable: false),
                    Script = table.Column<string>(nullable: true),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageStyles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageStyles_Pages_PageId",
                        column: x => x.PageId,
                        principalSchema: "Entities",
                        principalTable: "Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlockAttributes",
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
                    Value = table.Column<string>(nullable: true),
                    DefaultValue = table.Column<string>(nullable: true),
                    BlockId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockAttributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlockAttributes_Blocks_BlockId",
                        column: x => x.BlockId,
                        principalSchema: "Entities",
                        principalTable: "Blocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    table.ForeignKey(
                        name: "FK_EntityFieldPermissions_TableFields_TableModelFieldId",
                        column: x => x.TableModelFieldId,
                        principalSchema: "Entities",
                        principalTable: "TableFields",
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

            migrationBuilder.CreateTable(
                name: "ViewModelFields",
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
                    ViewModelId = table.Column<Guid>(nullable: false),
                    TableModelFieldsId = table.Column<Guid>(nullable: true),
                    Translate = table.Column<string>(nullable: true),
                    Template = table.Column<string>(nullable: true),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViewModelFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ViewModelFields_TableFields_TableModelFieldsId",
                        column: x => x.TableModelFieldsId,
                        principalSchema: "Entities",
                        principalTable: "TableFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ViewModelFields_ViewModels_ViewModelId",
                        column: x => x.ViewModelId,
                        principalSchema: "Entities",
                        principalTable: "ViewModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlockAttributes_BlockId",
                schema: "Entities",
                table: "BlockAttributes",
                column: "BlockId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockCategories_TenantId",
                schema: "Entities",
                table: "BlockCategories",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Blocks_BlockCategoryId",
                schema: "Entities",
                table: "Blocks",
                column: "BlockCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Blocks_TableModelId",
                schema: "Entities",
                table: "Blocks",
                column: "TableModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Blocks_TenantId",
                schema: "Entities",
                table: "Blocks",
                column: "TenantId");

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
                name: "IX_EntityTypes_TenantId",
                schema: "Entities",
                table: "EntityTypes",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_LayoutId",
                schema: "Entities",
                table: "Pages",
                column: "LayoutId");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_PageTypeId",
                schema: "Entities",
                table: "Pages",
                column: "PageTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_SettingsId",
                schema: "Entities",
                table: "Pages",
                column: "SettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_TenantId",
                schema: "Entities",
                table: "Pages",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PageScripts_PageId",
                schema: "Entities",
                table: "PageScripts",
                column: "PageId");

            migrationBuilder.CreateIndex(
                name: "IX_PageStyles_PageId",
                schema: "Entities",
                table: "PageStyles",
                column: "PageId");

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

            migrationBuilder.CreateIndex(
                name: "IX_ViewModelFields_TableModelFieldsId",
                schema: "Entities",
                table: "ViewModelFields",
                column: "TableModelFieldsId");

            migrationBuilder.CreateIndex(
                name: "IX_ViewModelFields_TenantId",
                schema: "Entities",
                table: "ViewModelFields",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ViewModelFields_ViewModelId",
                schema: "Entities",
                table: "ViewModelFields",
                column: "ViewModelId");

            migrationBuilder.CreateIndex(
                name: "IX_ViewModels_TableModelId",
                schema: "Entities",
                table: "ViewModels",
                column: "TableModelId");

            migrationBuilder.CreateIndex(
                name: "IX_ViewModels_TenantId",
                schema: "Entities",
                table: "ViewModels",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlockAttributes",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "EntityFieldPermissions",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "EntityPermissionAccesses",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "EntityTypes",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "PageScripts",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "PageStyles",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "TableFieldConfigValues",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "Templates",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "TrackAuditDetails",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "ViewModelFields",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "Blocks",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "EntityPermissions",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "Pages",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "TableFieldConfigs",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "TrackAudits",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "TableFields",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "ViewModels",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "BlockCategories",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "PageTypes",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "PageSettings",
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
