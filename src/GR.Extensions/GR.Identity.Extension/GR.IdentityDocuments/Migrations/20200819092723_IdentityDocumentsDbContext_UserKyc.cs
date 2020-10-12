using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.IdentityDocuments.Migrations
{
    public partial class IdentityDocumentsDbContext_UserKyc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "Identity",
                table: "IdentityDocuments");

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                schema: "Identity",
                table: "IdentityDocuments",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserKycId",
                schema: "Identity",
                table: "IdentityDocuments",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "ValidationState",
                schema: "Identity",
                table: "IdentityDocuments",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "UserKyc",
                schema: "Identity",
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
                    UserId = table.Column<Guid>(nullable: false),
                    ValidationState = table.Column<int>(nullable: false),
                    Reason = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserKyc", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IdentityDocuments_UserKycId",
                schema: "Identity",
                table: "IdentityDocuments",
                column: "UserKycId");

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityDocuments_UserKyc_UserKycId",
                schema: "Identity",
                table: "IdentityDocuments",
                column: "UserKycId",
                principalSchema: "Identity",
                principalTable: "UserKyc",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdentityDocuments_UserKyc_UserKycId",
                schema: "Identity",
                table: "IdentityDocuments");

            migrationBuilder.DropTable(
                name: "UserKyc",
                schema: "Identity");

            migrationBuilder.DropIndex(
                name: "IX_IdentityDocuments_UserKycId",
                schema: "Identity",
                table: "IdentityDocuments");

            migrationBuilder.DropColumn(
                name: "Reason",
                schema: "Identity",
                table: "IdentityDocuments");

            migrationBuilder.DropColumn(
                name: "UserKycId",
                schema: "Identity",
                table: "IdentityDocuments");

            migrationBuilder.DropColumn(
                name: "ValidationState",
                schema: "Identity",
                table: "IdentityDocuments");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                schema: "Identity",
                table: "IdentityDocuments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
