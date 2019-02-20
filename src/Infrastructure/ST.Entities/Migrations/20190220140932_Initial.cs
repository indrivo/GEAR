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
                name: "Attrs",
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
                    ClassName = table.Column<string>(nullable: true),
                    Required = table.Column<bool>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attrs", x => x.Id);
                });

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
                name: "ColumnFields",
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
                name: "FormTypes",
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
                    Code = table.Column<string>(type: "char(4)", nullable: true),
                    IsSystem = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Meta",
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
                    Group = table.Column<string>(nullable: true),
                    Icon = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meta", x => x.Id);
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
                    Name = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Icon = table.Column<string>(nullable: true),
                    PhysicPath = table.Column<string>(nullable: true)
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
                name: "RowColumns",
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
                    RowId = table.Column<Guid>(nullable: false),
                    ColumnId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RowColumns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
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
                    TenantId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StageRows",
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
                    StageId = table.Column<Guid>(nullable: false),
                    RowId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StageRows", x => x.Id);
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
                    Name = table.Column<string>(nullable: true),
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
                    Content = table.Column<string>(nullable: true)
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
                    UserName = table.Column<string>(nullable: true),
                    TrackEventType = table.Column<int>(nullable: false),
                    RecordId = table.Column<Guid>(nullable: false),
                    TypeFullName = table.Column<string>(nullable: true),
                    Version = table.Column<int>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackAudits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DisabledAttrs",
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
                    ConfigId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisabledAttrs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DisabledAttrs_Configs_ConfigId",
                        column: x => x.ConfigId,
                        principalSchema: "Entities",
                        principalTable: "Configs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "Forms",
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
                        principalSchema: "Entities",
                        principalTable: "Settings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Forms_Table_TableId",
                        column: x => x.TableId,
                        principalSchema: "Entities",
                        principalTable: "Table",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Forms_FormTypes_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "Entities",
                        principalTable: "FormTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    TrackAuditId = table.Column<Guid>(nullable: false),
                    PropertyName = table.Column<string>(nullable: true),
                    PropertyType = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    TenantId = table.Column<Guid>(nullable: true)
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
                name: "Stages",
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
                    SettingsId = table.Column<Guid>(nullable: true),
                    FormId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stages_Forms_FormId",
                        column: x => x.FormId,
                        principalSchema: "Entities",
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Stages_Settings_SettingsId",
                        column: x => x.SettingsId,
                        principalSchema: "Entities",
                        principalTable: "Settings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    Name = table.Column<string>(nullable: true),
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
                name: "Rows",
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
                    ConfigId = table.Column<Guid>(nullable: false),
                    AttrsId = table.Column<Guid>(nullable: false),
                    FormId = table.Column<Guid>(nullable: false),
                    StageId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rows_Attrs_AttrsId",
                        column: x => x.AttrsId,
                        principalSchema: "Entities",
                        principalTable: "Attrs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rows_Configs_ConfigId",
                        column: x => x.ConfigId,
                        principalSchema: "Entities",
                        principalTable: "Configs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rows_Forms_FormId",
                        column: x => x.FormId,
                        principalSchema: "Entities",
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rows_Stages_StageId",
                        column: x => x.StageId,
                        principalSchema: "Entities",
                        principalTable: "Stages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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

            migrationBuilder.CreateTable(
                name: "Columns",
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
                        principalSchema: "Entities",
                        principalTable: "Configs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Columns_Forms_FormId",
                        column: x => x.FormId,
                        principalSchema: "Entities",
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Columns_Rows_RowId",
                        column: x => x.RowId,
                        principalSchema: "Entities",
                        principalTable: "Rows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Fields",
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
                    Tag = table.Column<string>(nullable: true),
                    AttrsId = table.Column<Guid>(nullable: false),
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
                        name: "FK_Fields_Attrs_AttrsId",
                        column: x => x.AttrsId,
                        principalSchema: "Entities",
                        principalTable: "Attrs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Fields_Columns_ColumnId",
                        column: x => x.ColumnId,
                        principalSchema: "Entities",
                        principalTable: "Columns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fields_Configs_ConfigId",
                        column: x => x.ConfigId,
                        principalSchema: "Entities",
                        principalTable: "Configs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Fields_Forms_FormId",
                        column: x => x.FormId,
                        principalSchema: "Entities",
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Fields_Meta_MetaId",
                        column: x => x.MetaId,
                        principalSchema: "Entities",
                        principalTable: "Meta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Fields_TableFields_TableFieldId",
                        column: x => x.TableFieldId,
                        principalSchema: "Entities",
                        principalTable: "TableFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Options",
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
                        principalSchema: "Entities",
                        principalTable: "Fields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlockAttributes_BlockId",
                schema: "Entities",
                table: "BlockAttributes",
                column: "BlockId");

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
                name: "IX_Columns_ConfigId",
                schema: "Entities",
                table: "Columns",
                column: "ConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_Columns_FormId",
                schema: "Entities",
                table: "Columns",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_Columns_RowId",
                schema: "Entities",
                table: "Columns",
                column: "RowId");

            migrationBuilder.CreateIndex(
                name: "IX_DisabledAttrs_ConfigId",
                schema: "Entities",
                table: "DisabledAttrs",
                column: "ConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_Fields_AttrsId",
                schema: "Entities",
                table: "Fields",
                column: "AttrsId");

            migrationBuilder.CreateIndex(
                name: "IX_Fields_ColumnId",
                schema: "Entities",
                table: "Fields",
                column: "ColumnId");

            migrationBuilder.CreateIndex(
                name: "IX_Fields_ConfigId",
                schema: "Entities",
                table: "Fields",
                column: "ConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_Fields_FormId",
                schema: "Entities",
                table: "Fields",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_Fields_MetaId",
                schema: "Entities",
                table: "Fields",
                column: "MetaId");

            migrationBuilder.CreateIndex(
                name: "IX_Fields_TableFieldId",
                schema: "Entities",
                table: "Fields",
                column: "TableFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_SettingsId",
                schema: "Entities",
                table: "Forms",
                column: "SettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_TableId",
                schema: "Entities",
                table: "Forms",
                column: "TableId");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_TypeId",
                schema: "Entities",
                table: "Forms",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Options_FieldId",
                schema: "Entities",
                table: "Options",
                column: "FieldId");

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
                name: "IX_Rows_AttrsId",
                schema: "Entities",
                table: "Rows",
                column: "AttrsId");

            migrationBuilder.CreateIndex(
                name: "IX_Rows_ConfigId",
                schema: "Entities",
                table: "Rows",
                column: "ConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_Rows_FormId",
                schema: "Entities",
                table: "Rows",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_Rows_StageId",
                schema: "Entities",
                table: "Rows",
                column: "StageId");

            migrationBuilder.CreateIndex(
                name: "IX_Stages_FormId",
                schema: "Entities",
                table: "Stages",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_Stages_SettingsId",
                schema: "Entities",
                table: "Stages",
                column: "SettingsId");

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
                name: "IX_ViewModelFields_ViewModelId",
                schema: "Entities",
                table: "ViewModelFields",
                column: "ViewModelId");

            migrationBuilder.CreateIndex(
                name: "IX_ViewModels_TableModelId",
                schema: "Entities",
                table: "ViewModels",
                column: "TableModelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlockAttributes",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "ColumnFields",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "DisabledAttrs",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "EntityTypes",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "Options",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "PageScripts",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "PageStyles",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "RowColumns",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "StageRows",
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
                name: "Fields",
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
                name: "ViewModels",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "BlockCategories",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "Columns",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "Meta",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "TableFields",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "PageTypes",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "PageSettings",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "Rows",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "TableFieldTypes",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "Attrs",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "Configs",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "Stages",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "TableFieldGroups",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "Forms",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "Settings",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "Table",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "FormTypes",
                schema: "Entities");
        }
    }
}
