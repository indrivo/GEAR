using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ST.Entities.Migrations
{
    public partial class AddCommomFieldForTenantUsage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ViewModelFields_TableFields_TableModelFieldsId",
                schema: "Entities",
                table: "ViewModelFields");

            migrationBuilder.DropIndex(
                name: "IX_ViewModelFields_TableModelFieldsId",
                schema: "Entities",
                table: "ViewModelFields");

            migrationBuilder.AddColumn<Guid>(
                name: "TableModelFieldId",
                schema: "Entities",
                table: "ViewModelFields",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCommon",
                schema: "Entities",
                table: "TableFields",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_ViewModelFields_TableModelFieldId",
                schema: "Entities",
                table: "ViewModelFields",
                column: "TableModelFieldId");

            migrationBuilder.AddForeignKey(
                name: "FK_ViewModelFields_TableFields_TableModelFieldId",
                schema: "Entities",
                table: "ViewModelFields",
                column: "TableModelFieldId",
                principalSchema: "Entities",
                principalTable: "TableFields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ViewModelFields_TableFields_TableModelFieldId",
                schema: "Entities",
                table: "ViewModelFields");

            migrationBuilder.DropIndex(
                name: "IX_ViewModelFields_TableModelFieldId",
                schema: "Entities",
                table: "ViewModelFields");

            migrationBuilder.DropColumn(
                name: "TableModelFieldId",
                schema: "Entities",
                table: "ViewModelFields");

            migrationBuilder.DropColumn(
                name: "IsCommon",
                schema: "Entities",
                table: "TableFields");

            migrationBuilder.CreateIndex(
                name: "IX_ViewModelFields_TableModelFieldsId",
                schema: "Entities",
                table: "ViewModelFields",
                column: "TableModelFieldsId");

            migrationBuilder.AddForeignKey(
                name: "FK_ViewModelFields_TableFields_TableModelFieldsId",
                schema: "Entities",
                table: "ViewModelFields",
                column: "TableModelFieldsId",
                principalSchema: "Entities",
                principalTable: "TableFields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
