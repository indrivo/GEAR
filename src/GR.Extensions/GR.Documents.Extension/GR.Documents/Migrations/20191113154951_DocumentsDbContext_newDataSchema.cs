using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.Documents.Migrations
{
    public partial class DocumentsDbContext_newDataSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentVersions_FileStorage_FileStorageId",
                table: "DocumentVersions");

            migrationBuilder.EnsureSchema(
                name: "Documents");

            migrationBuilder.RenameTable(
                name: "TrackAudits",
                newName: "TrackAudits",
                newSchema: "Documents");

            migrationBuilder.RenameTable(
                name: "TrackAuditDetails",
                newName: "TrackAuditDetails",
                newSchema: "Documents");

            migrationBuilder.RenameTable(
                name: "FileStorage",
                newName: "FileStorage",
                newSchema: "Documents");

            migrationBuilder.RenameTable(
                name: "DocumentVersions",
                newName: "DocumentVersions",
                newSchema: "Documents");

            migrationBuilder.RenameTable(
                name: "DocumentTypes",
                newName: "DocumentTypes",
                newSchema: "Documents");

            migrationBuilder.RenameTable(
                name: "Documents",
                newName: "Documents",
                newSchema: "Documents");

            migrationBuilder.AlterColumn<Guid>(
                name: "FileStorageId",
                schema: "Documents",
                table: "DocumentVersions",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "Documents",
                table: "DocumentTypes",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentVersions_FileStorage_FileStorageId",
                schema: "Documents",
                table: "DocumentVersions");

            migrationBuilder.RenameTable(
                name: "TrackAudits",
                schema: "Documents",
                newName: "TrackAudits");

            migrationBuilder.RenameTable(
                name: "TrackAuditDetails",
                schema: "Documents",
                newName: "TrackAuditDetails");

            migrationBuilder.RenameTable(
                name: "FileStorage",
                schema: "Documents",
                newName: "FileStorage");

            migrationBuilder.RenameTable(
                name: "DocumentVersions",
                schema: "Documents",
                newName: "DocumentVersions");

            migrationBuilder.RenameTable(
                name: "DocumentTypes",
                schema: "Documents",
                newName: "DocumentTypes");

            migrationBuilder.RenameTable(
                name: "Documents",
                schema: "Documents",
                newName: "Documents");

            migrationBuilder.AlterColumn<Guid>(
                name: "FileStorageId",
                table: "DocumentVersions",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "DocumentTypes",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentVersions_FileStorage_FileStorageId",
                table: "DocumentVersions",
                column: "FileStorageId",
                principalTable: "FileStorage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
