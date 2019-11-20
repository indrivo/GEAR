using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.Documents.Migrations
{
    public partial class DocumentsDbContext_addDocumentVersionAddOwner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "DocumentVersions",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "DocumentVersions");
        }
    }
}
