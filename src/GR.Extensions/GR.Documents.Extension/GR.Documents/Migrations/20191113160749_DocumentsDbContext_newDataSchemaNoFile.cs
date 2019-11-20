using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.Documents.Migrations
{
    public partial class DocumentsDbContext_newDataSchemaNoFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentVersions_FileStorage_FileStorageId",
                schema: "Documents",
                table: "DocumentVersions");

            migrationBuilder.DropTable(
                name: "FileStorage",
                schema: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_DocumentVersions_FileStorageId",
                schema: "Documents",
                table: "DocumentVersions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileStorage",
                schema: "Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Author = table.Column<string>(nullable: true),
                    Changed = table.Column<DateTime>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    FileExtension = table.Column<string>(nullable: true),
                    Hash = table.Column<byte[]>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Size = table.Column<long>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: true),
                    Version = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileStorage", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentVersions_FileStorageId",
                schema: "Documents",
                table: "DocumentVersions",
                column: "FileStorageId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentVersions_FileStorage_FileStorageId",
                schema: "Documents",
                table: "DocumentVersions",
                column: "FileStorageId",
                principalSchema: "Documents",
                principalTable: "FileStorage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
