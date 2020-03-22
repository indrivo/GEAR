using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.Identity.Profile.Migrations
{
    public partial class ProfileDbContext_Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Identity");

            migrationBuilder.CreateTable(
                name: "Profiles",
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
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ProfileLevel = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrackAudits",
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
                    DatabaseContextName = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    TrackEventType = table.Column<int>(nullable: false),
                    RecordId = table.Column<Guid>(nullable: false),
                    TypeFullName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackAudits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAddresses",
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
                    ContactName = table.Column<string>(maxLength: 450, nullable: true),
                    Phone = table.Column<string>(maxLength: 450, nullable: true),
                    AddressLine1 = table.Column<string>(maxLength: 450, nullable: true),
                    AddressLine2 = table.Column<string>(maxLength: 450, nullable: true),
                    ZipCode = table.Column<string>(maxLength: 450, nullable: true),
                    DistrictId = table.Column<Guid>(nullable: true),
                    StateOrProvinceId = table.Column<Guid>(nullable: false),
                    CountryId = table.Column<string>(maxLength: 450, nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    IsDefault = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAddresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleProfiles",
                schema: "Identity",
                columns: table => new
                {
                    ProfileId = table.Column<Guid>(nullable: false),
                    RoleId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleProfiles", x => new { x.RoleId, x.ProfileId });
                    table.ForeignKey(
                        name: "FK_RoleProfiles_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalSchema: "Identity",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrackAuditDetails",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Author = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Changed = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: true),
                    TrackAuditId = table.Column<Guid>(nullable: false),
                    PropertyName = table.Column<string>(nullable: true),
                    PropertyType = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackAuditDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackAuditDetails_TrackAudits_TrackAuditId",
                        column: x => x.TrackAuditId,
                        principalSchema: "Identity",
                        principalTable: "TrackAudits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserProfiles",
                schema: "Identity",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    RoleProfileId = table.Column<Guid>(nullable: false),
                    RoleProfileRoleId = table.Column<Guid>(nullable: true),
                    RoleProfileProfileId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => new { x.UserId, x.RoleProfileId });
                    table.ForeignKey(
                        name: "FK_UserProfiles_RoleProfiles_RoleProfileRoleId_RoleProfileProf~",
                        columns: x => new { x.RoleProfileRoleId, x.RoleProfileProfileId },
                        principalSchema: "Identity",
                        principalTable: "RoleProfiles",
                        principalColumns: new[] { "RoleId", "ProfileId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_TenantId",
                schema: "Identity",
                table: "Profiles",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleProfiles_ProfileId",
                schema: "Identity",
                table: "RoleProfiles",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackAuditDetails_TrackAuditId",
                schema: "Identity",
                table: "TrackAuditDetails",
                column: "TrackAuditId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAddresses_CountryId",
                schema: "Identity",
                table: "UserAddresses",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAddresses_DistrictId",
                schema: "Identity",
                table: "UserAddresses",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAddresses_StateOrProvinceId",
                schema: "Identity",
                table: "UserAddresses",
                column: "StateOrProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAddresses_UserId",
                schema: "Identity",
                table: "UserAddresses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_UserId",
                schema: "Identity",
                table: "UserProfiles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_RoleProfileRoleId_RoleProfileProfileId",
                schema: "Identity",
                table: "UserProfiles",
                columns: new[] { "RoleProfileRoleId", "RoleProfileProfileId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrackAuditDetails",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "UserAddresses",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "UserProfiles",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "TrackAudits",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "RoleProfiles",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Profiles",
                schema: "Identity");
        }
    }
}
