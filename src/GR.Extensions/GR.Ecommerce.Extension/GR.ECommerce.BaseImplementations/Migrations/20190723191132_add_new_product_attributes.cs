using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.ECommerce.BaseImplementations.Migrations
{
    public partial class add_new_product_attributes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductAttribute_AttributeGroup_AttributeGroupId",
                schema: "Commerce",
                table: "ProductAttribute");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AttributeGroup",
                schema: "Commerce",
                table: "AttributeGroup");

            migrationBuilder.RenameTable(
                name: "AttributeGroup",
                schema: "Commerce",
                newName: "AttributeGroups",
                newSchema: "Commerce");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AttributeGroups",
                schema: "Commerce",
                table: "AttributeGroups",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ProductAttributes",
                schema: "Commerce",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(nullable: false),
                    ProductAttributeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttributes", x => new { x.ProductAttributeId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_ProductAttributes_ProductAttribute_ProductAttributeId",
                        column: x => x.ProductAttributeId,
                        principalSchema: "Commerce",
                        principalTable: "ProductAttribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductAttributes_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "Commerce",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributes_ProductId",
                schema: "Commerce",
                table: "ProductAttributes",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductAttribute_AttributeGroups_AttributeGroupId",
                schema: "Commerce",
                table: "ProductAttribute",
                column: "AttributeGroupId",
                principalSchema: "Commerce",
                principalTable: "AttributeGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductAttribute_AttributeGroups_AttributeGroupId",
                schema: "Commerce",
                table: "ProductAttribute");

            migrationBuilder.DropTable(
                name: "ProductAttributes",
                schema: "Commerce");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AttributeGroups",
                schema: "Commerce",
                table: "AttributeGroups");

            migrationBuilder.RenameTable(
                name: "AttributeGroups",
                schema: "Commerce",
                newName: "AttributeGroup",
                newSchema: "Commerce");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AttributeGroup",
                schema: "Commerce",
                table: "AttributeGroup",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductAttribute_AttributeGroup_AttributeGroupId",
                schema: "Commerce",
                table: "ProductAttribute",
                column: "AttributeGroupId",
                principalSchema: "Commerce",
                principalTable: "AttributeGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
