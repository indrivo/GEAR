using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.Documents.Migrations
{
    public partial class DocumentsDbContext_SetDoumentTypeId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_DocumentCategories_DocumentCategoryId",
                schema: "Documents",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_DocumentTypes_DocumentTypeId",
                schema: "Documents",
                table: "Documents");

            migrationBuilder.AlterColumn<Guid>(
                name: "DocumentTypeId",
                schema: "Documents",
                table: "Documents",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<Guid>(
                name: "DocumentCategoryId",
                schema: "Documents",
                table: "Documents",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_DocumentCategories_DocumentCategoryId",
                schema: "Documents",
                table: "Documents",
                column: "DocumentCategoryId",
                principalSchema: "Documents",
                principalTable: "DocumentCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_DocumentTypes_DocumentTypeId",
                schema: "Documents",
                table: "Documents",
                column: "DocumentTypeId",
                principalSchema: "Documents",
                principalTable: "DocumentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_DocumentCategories_DocumentCategoryId",
                schema: "Documents",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_DocumentTypes_DocumentTypeId",
                schema: "Documents",
                table: "Documents");

            migrationBuilder.AlterColumn<Guid>(
                name: "DocumentTypeId",
                schema: "Documents",
                table: "Documents",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DocumentCategoryId",
                schema: "Documents",
                table: "Documents",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_DocumentCategories_DocumentCategoryId",
                schema: "Documents",
                table: "Documents",
                column: "DocumentCategoryId",
                principalSchema: "Documents",
                principalTable: "DocumentCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_DocumentTypes_DocumentTypeId",
                schema: "Documents",
                table: "Documents",
                column: "DocumentTypeId",
                principalSchema: "Documents",
                principalTable: "DocumentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
