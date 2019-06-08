using Microsoft.EntityFrameworkCore.Migrations;

namespace ST.Entities.Migrations
{
    public partial class EntityDbContext_Add_IsCommonField_for_users_common_data : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCommon",
                schema: "Entities",
                table: "Table",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCommon",
                schema: "Entities",
                table: "Table");
        }
    }
}
