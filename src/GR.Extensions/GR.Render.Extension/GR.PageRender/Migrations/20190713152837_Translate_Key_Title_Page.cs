using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.PageRender.Migrations
{
    public partial class Translate_Key_Title_Page : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TitleTranslateKey",
                schema: "Pages",
                table: "PageSettings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TitleTranslateKey",
                schema: "Pages",
                table: "PageSettings");
        }
    }
}
