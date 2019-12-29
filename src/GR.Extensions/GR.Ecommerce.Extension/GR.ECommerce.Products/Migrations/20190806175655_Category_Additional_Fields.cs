using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.ECommerce.BaseImplementations.Migrations
{
    public partial class Category_Additional_Fields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Specification",
                schema: "Commerce",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "Commerce",
                table: "Categories",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                schema: "Commerce",
                table: "Categories",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                schema: "Commerce",
                table: "Categories",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentCategoryId",
                schema: "Commerce",
                table: "Categories",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentCategoryId",
                schema: "Commerce",
                table: "Categories",
                column: "ParentCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Categories_ParentCategoryId",
                schema: "Commerce",
                table: "Categories",
                column: "ParentCategoryId",
                principalSchema: "Commerce",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Categories_ParentCategoryId",
                schema: "Commerce",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_ParentCategoryId",
                schema: "Commerce",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Specification",
                schema: "Commerce",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "Commerce",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                schema: "Commerce",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                schema: "Commerce",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "ParentCategoryId",
                schema: "Commerce",
                table: "Categories");
        }
    }
}
