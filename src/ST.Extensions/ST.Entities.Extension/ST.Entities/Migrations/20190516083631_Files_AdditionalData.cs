using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ST.Entities.Migrations
{
    public partial class Files_AdditionalData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalLink",
                schema: "Entities",
                table: "Documents",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ResponsibleId",
                schema: "Entities",
                table: "Documents",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ResponsibleId1",
                schema: "Entities",
                table: "Documents",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ApplicationUser",
                schema: "Entities",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    NormalizedUserName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    NormalizedEmail = table.Column<string>(nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    Author = table.Column<string>(nullable: true),
                    Changed = table.Column<DateTime>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsEditable = table.Column<bool>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    UserPhoto = table.Column<byte[]>(nullable: true),
                    AuthenticationType = table.Column<int>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: true),
                    Version = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuthGroup",
                schema: "Entities",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Author = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Changed = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Version = table.Column<int>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserGroup",
                schema: "Entities",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Author = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Changed = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Version = table.Column<int>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: true),
                    AuthGroupId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserGroup_AuthGroup_AuthGroupId",
                        column: x => x.AuthGroupId,
                        principalSchema: "Entities",
                        principalTable: "AuthGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserGroup_ApplicationUser_UserId",
                        column: x => x.UserId,
                        principalSchema: "Entities",
                        principalTable: "ApplicationUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ResponsibleId1",
                schema: "Entities",
                table: "Documents",
                column: "ResponsibleId1");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroup_AuthGroupId",
                schema: "Entities",
                table: "UserGroup",
                column: "AuthGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroup_UserId",
                schema: "Entities",
                table: "UserGroup",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_ApplicationUser_ResponsibleId1",
                schema: "Entities",
                table: "Documents",
                column: "ResponsibleId1",
                principalSchema: "Entities",
                principalTable: "ApplicationUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_ApplicationUser_ResponsibleId1",
                schema: "Entities",
                table: "Documents");

            migrationBuilder.DropTable(
                name: "UserGroup",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "AuthGroup",
                schema: "Entities");

            migrationBuilder.DropTable(
                name: "ApplicationUser",
                schema: "Entities");

            migrationBuilder.DropIndex(
                name: "IX_Documents_ResponsibleId1",
                schema: "Entities",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ExternalLink",
                schema: "Entities",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ResponsibleId",
                schema: "Entities",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ResponsibleId1",
                schema: "Entities",
                table: "Documents");
        }
    }
}
