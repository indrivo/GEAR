using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.Documents.Migrations
{
    public partial class DocumentsDbContext_addDocumentCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                schema: "Documents",
                table: "DocumentTypes");

            migrationBuilder.DropColumn(
                name: "IsSystem",
                schema: "Documents",
                table: "DocumentTypes");

            migrationBuilder.CreateTable(
                name: "DocumentCategories",
                schema: "Documents",
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
                    Code = table.Column<int>(nullable: false),
                    IsSystem = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentCategories", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentCategories",
                schema: "Documents");

            migrationBuilder.AddColumn<int>(
                name: "Code",
                schema: "Documents",
                table: "DocumentTypes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsSystem",
                schema: "Documents",
                table: "DocumentTypes",
                nullable: false,
                defaultValue: false);
        }
    }
}
