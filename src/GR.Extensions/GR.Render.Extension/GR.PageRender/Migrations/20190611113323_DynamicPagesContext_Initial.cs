using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.PageRender.Migrations
{
    public partial class DynamicPagesContext_Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Pages");

            migrationBuilder.CreateTable(
                name: "BlockCategories",
                schema: "Pages",
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
                name: "PageSettings",
                schema: "Pages",
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
                schema: "Pages",
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
                name: "Templates",
                schema: "Pages",
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
                schema: "Pages",
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
                name: "ViewModels",
                schema: "Pages",
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
                });

            migrationBuilder.CreateTable(
                name: "Blocks",
                schema: "Pages",
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
                        principalSchema: "Pages",
                        principalTable: "BlockCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pages",
                schema: "Pages",
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
                    LayoutId = table.Column<Guid>(nullable: true),
                    IsEnabledAcl = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pages_Pages_LayoutId",
                        column: x => x.LayoutId,
                        principalSchema: "Pages",
                        principalTable: "Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pages_PageTypes_PageTypeId",
                        column: x => x.PageTypeId,
                        principalSchema: "Pages",
                        principalTable: "PageTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pages_PageSettings_SettingsId",
                        column: x => x.SettingsId,
                        principalSchema: "Pages",
                        principalTable: "PageSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TrackAuditDetails",
                schema: "Pages",
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
                        principalSchema: "Pages",
                        principalTable: "TrackAudits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ViewModelFields",
                schema: "Pages",
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
                        name: "FK_ViewModelFields_ViewModels_ViewModelId",
                        column: x => x.ViewModelId,
                        principalSchema: "Pages",
                        principalTable: "ViewModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlockAttributes",
                schema: "Pages",
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
                        principalSchema: "Pages",
                        principalTable: "Blocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PageScripts",
                schema: "Pages",
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
                        principalSchema: "Pages",
                        principalTable: "Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PageStyles",
                schema: "Pages",
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
                        principalSchema: "Pages",
                        principalTable: "Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePagesAcls",
                schema: "Pages",
                columns: table => new
                {
                    PageId = table.Column<Guid>(nullable: false),
                    RoleId = table.Column<Guid>(nullable: false),
                    AllowAccess = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePagesAcls", x => new { x.RoleId, x.PageId });
                    table.ForeignKey(
                        name: "FK_RolePagesAcls_Pages_PageId",
                        column: x => x.PageId,
                        principalSchema: "Pages",
                        principalTable: "Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlockAttributes_BlockId",
                schema: "Pages",
                table: "BlockAttributes",
                column: "BlockId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockCategories_TenantId",
                schema: "Pages",
                table: "BlockCategories",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Blocks_BlockCategoryId",
                schema: "Pages",
                table: "Blocks",
                column: "BlockCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Blocks_TenantId",
                schema: "Pages",
                table: "Blocks",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_LayoutId",
                schema: "Pages",
                table: "Pages",
                column: "LayoutId");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_PageTypeId",
                schema: "Pages",
                table: "Pages",
                column: "PageTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_SettingsId",
                schema: "Pages",
                table: "Pages",
                column: "SettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_TenantId",
                schema: "Pages",
                table: "Pages",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PageScripts_PageId",
                schema: "Pages",
                table: "PageScripts",
                column: "PageId");

            migrationBuilder.CreateIndex(
                name: "IX_PageStyles_PageId",
                schema: "Pages",
                table: "PageStyles",
                column: "PageId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePagesAcls_PageId",
                schema: "Pages",
                table: "RolePagesAcls",
                column: "PageId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackAuditDetails_TrackAuditId",
                schema: "Pages",
                table: "TrackAuditDetails",
                column: "TrackAuditId");

            migrationBuilder.CreateIndex(
                name: "IX_ViewModelFields_TenantId",
                schema: "Pages",
                table: "ViewModelFields",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ViewModelFields_ViewModelId",
                schema: "Pages",
                table: "ViewModelFields",
                column: "ViewModelId");

            migrationBuilder.CreateIndex(
                name: "IX_ViewModels_TenantId",
                schema: "Pages",
                table: "ViewModels",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlockAttributes",
                schema: "Pages");

            migrationBuilder.DropTable(
                name: "PageScripts",
                schema: "Pages");

            migrationBuilder.DropTable(
                name: "PageStyles",
                schema: "Pages");

            migrationBuilder.DropTable(
                name: "RolePagesAcls",
                schema: "Pages");

            migrationBuilder.DropTable(
                name: "Templates",
                schema: "Pages");

            migrationBuilder.DropTable(
                name: "TrackAuditDetails",
                schema: "Pages");

            migrationBuilder.DropTable(
                name: "ViewModelFields",
                schema: "Pages");

            migrationBuilder.DropTable(
                name: "Blocks",
                schema: "Pages");

            migrationBuilder.DropTable(
                name: "Pages",
                schema: "Pages");

            migrationBuilder.DropTable(
                name: "TrackAudits",
                schema: "Pages");

            migrationBuilder.DropTable(
                name: "ViewModels",
                schema: "Pages");

            migrationBuilder.DropTable(
                name: "BlockCategories",
                schema: "Pages");

            migrationBuilder.DropTable(
                name: "PageTypes",
                schema: "Pages");

            migrationBuilder.DropTable(
                name: "PageSettings",
                schema: "Pages");
        }
    }
}
