using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.Identity.Migrations
{
    public partial class GearIdentityDbContext_AuthenticationType_To_String : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AuthenticationType",
                schema: "Identity",
                table: "Users",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AuthenticationType",
                schema: "Identity",
                table: "Users",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
