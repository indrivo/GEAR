using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.Documents.Migrations
{
    public partial class DocumentsDbContext_addDocumentCategoryList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DocumentCategoryId",
                schema: "Documents",
                table: "Documents",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_DocumentCategoryId",
                schema: "Documents",
                table: "Documents",
                column: "DocumentCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_DocumentCategories_DocumentCategoryId",
                schema: "Documents",
                table: "Documents",
                column: "DocumentCategoryId",
                principalSchema: "Documents",
                principalTable: "DocumentCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_DocumentCategories_DocumentCategoryId",
                schema: "Documents",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_DocumentCategoryId",
                schema: "Documents",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "DocumentCategoryId",
                schema: "Documents",
                table: "Documents");
        }
    }
}
