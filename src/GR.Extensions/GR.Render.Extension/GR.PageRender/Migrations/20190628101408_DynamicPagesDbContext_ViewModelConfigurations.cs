using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace GR.PageRender.Migrations
{
    public partial class DynamicPagesDbContext_ViewModelConfigurations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "Pages",
                table: "ViewModels",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "Pages",
                table: "ViewModelFields",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VirtualDataType",
                schema: "Pages",
                table: "ViewModelFields",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ViewModelFieldCodesCodes",
                schema: "Pages",
                columns: table => new
                {
                    Code = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViewModelFieldCodesCodes", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "ViewModelFieldConfigurations",
                schema: "Pages",
                columns: table => new
                {
                    ViewModelFieldId = table.Column<Guid>(nullable: false),
                    ViewModelFieldCodeId = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViewModelFieldConfigurations", x => new { x.ViewModelFieldCodeId, x.ViewModelFieldId });
                    table.ForeignKey(
                        name: "FK_ViewModelFieldConfigurations_ViewModelFieldCodesCodes_ViewM~",
                        column: x => x.ViewModelFieldCodeId,
                        principalSchema: "Pages",
                        principalTable: "ViewModelFieldCodesCodes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ViewModelFieldConfigurations_ViewModelFields_ViewModelField~",
                        column: x => x.ViewModelFieldId,
                        principalSchema: "Pages",
                        principalTable: "ViewModelFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ViewModelFieldConfigurations_ViewModelFieldId",
                schema: "Pages",
                table: "ViewModelFieldConfigurations",
                column: "ViewModelFieldId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ViewModelFieldConfigurations",
                schema: "Pages");

            migrationBuilder.DropTable(
                name: "ViewModelFieldCodesCodes",
                schema: "Pages");

            migrationBuilder.DropColumn(
                name: "VirtualDataType",
                schema: "Pages",
                table: "ViewModelFields");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "Pages",
                table: "ViewModels",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "Pages",
                table: "ViewModelFields",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
