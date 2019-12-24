using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System;

namespace GR.Identity.Migrations
{
    public partial class ApplicationDbContext_Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Identity");

            migrationBuilder.CreateTable(
                name: "AuthGroups",
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
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 450, nullable: false),
                    Code3 = table.Column<string>(maxLength: 450, nullable: true),
                    IsBillingEnabled = table.Column<bool>(nullable: false),
                    IsShippingEnabled = table.Column<bool>(nullable: false),
                    IsCityEnabled = table.Column<bool>(nullable: false),
                    IsZipCodeEnabled = table.Column<bool>(nullable: false),
                    IsDistrictEnabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
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
                    PermissionName = table.Column<string>(maxLength: 100, nullable: true),
                    ClientId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 300, nullable: true),
                    PermissionKey = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

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
                    ProfileName = table.Column<string>(nullable: false),
                    IsSystem = table.Column<bool>(nullable: false),
                    EntityId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Author = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Changed = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ClientId = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    IsNoEditable = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    TenantId = table.Column<Guid>(nullable: true),
                    Version = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
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
                name: "Users",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
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
                    UserFirstName = table.Column<string>(maxLength: 50, nullable: true),
                    UserLastName = table.Column<string>(maxLength: 50, nullable: true),
                    IsDisabled = table.Column<bool>(nullable: false),
                    Birthday = table.Column<DateTime>(nullable: false),
                    AboutMe = table.Column<string>(maxLength: 500, nullable: true),
                    Author = table.Column<string>(nullable: true),
                    Changed = table.Column<DateTime>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsEditable = table.Column<bool>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    UserPhoto = table.Column<byte[]>(nullable: true),
                    AuthenticationType = table.Column<int>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: true),
                    LastPasswordChanged = table.Column<DateTime>(nullable: false),
                    LastLogin = table.Column<DateTime>(nullable: false),
                    Version = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupPermissions",
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
                    GroupId = table.Column<Guid>(nullable: false),
                    AuthGroupId = table.Column<Guid>(nullable: true),
                    PermissionCode = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupPermissions_AuthGroups_AuthGroupId",
                        column: x => x.AuthGroupId,
                        principalSchema: "Identity",
                        principalTable: "AuthGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StateOrProvinces",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CountryId = table.Column<string>(maxLength: 450, nullable: true),
                    Code = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 450, nullable: false),
                    Type = table.Column<string>(maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StateOrProvinces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StateOrProvinces_Countries_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "Identity",
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    RoleId = table.Column<Guid>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Identity",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
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
                    RoleId = table.Column<Guid>(nullable: true),
                    PermissionCode = table.Column<string>(nullable: false),
                    PermissionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalSchema: "Identity",
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Identity",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoleProfiles",
                schema: "Identity",
                columns: table => new
                {
                    ProfileId = table.Column<Guid>(nullable: false),
                    ApplicationRoleId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleProfiles", x => new { x.ApplicationRoleId, x.ProfileId });
                    table.ForeignKey(
                        name: "FK_RoleProfiles_Roles_ApplicationRoleId",
                        column: x => x.ApplicationRoleId,
                        principalSchema: "Identity",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "UserClaims",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    UserId = table.Column<Guid>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserGroups",
                schema: "Identity",
                columns: table => new
                {
                    AuthGroupId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    Id = table.Column<Guid>(nullable: false),
                    Author = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Changed = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Version = table.Column<int>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroups", x => new { x.AuthGroupId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserGroups_AuthGroups_AuthGroupId",
                        column: x => x.AuthGroupId,
                        principalSchema: "Identity",
                        principalTable: "AuthGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserGroups_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                schema: "Identity",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                schema: "Identity",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    RoleId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Identity",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                schema: "Identity",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Districts",
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
                    StateOrProvinceId = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 450, nullable: false),
                    Type = table.Column<string>(maxLength: 450, nullable: true),
                    Location = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Districts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Districts_StateOrProvinces_StateOrProvinceId",
                        column: x => x.StateOrProvinceId,
                        principalSchema: "Identity",
                        principalTable: "StateOrProvinces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Author = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Changed = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    MachineName = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    SiteWeb = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    OrganizationLogo = table.Column<byte[]>(nullable: true),
                    CountryId = table.Column<string>(nullable: true),
                    CityId = table.Column<long>(nullable: true),
                    TimeZone = table.Column<string>(nullable: true),
                    DateFormat = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tenants_StateOrProvinces_CityId",
                        column: x => x.CityId,
                        principalSchema: "Identity",
                        principalTable: "StateOrProvinces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tenants_Countries_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "Identity",
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
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
                    StateOrProvinceId = table.Column<long>(nullable: false),
                    CountryId = table.Column<string>(maxLength: 450, nullable: false),
                    ApplicationUserId = table.Column<Guid>(nullable: true),
                    IsDefault = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Addresses_Users_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Addresses_Countries_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "Identity",
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Addresses_Districts_DistrictId",
                        column: x => x.DistrictId,
                        principalSchema: "Identity",
                        principalTable: "Districts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Addresses_StateOrProvinces_StateOrProvinceId",
                        column: x => x.StateOrProvinceId,
                        principalSchema: "Identity",
                        principalTable: "StateOrProvinces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "Identity",
                table: "Countries",
                columns: new[] { "Id", "Code3", "IsBillingEnabled", "IsCityEnabled", "IsDistrictEnabled", "IsShippingEnabled", "IsZipCodeEnabled", "Name" },
                values: new object[,]
                {
                    { "BD", "BGD", false, true, true, false, true, "Bangladesh" },
                    { "CY", "CYP", false, true, true, false, true, "Cyprus" },
                    { "CX", "CXR", false, true, true, false, true, "Christmas Island" },
                    { "CR", "CRI", false, true, true, false, true, "Costa Rica" },
                    { "CW", "CUW", false, true, true, false, true, "Curacao" },
                    { "CV", "CPV", false, true, true, false, true, "Cape Verde" },
                    { "CU", "CUB", false, true, true, false, true, "Cuba" },
                    { "SZ", "SWZ", false, true, true, false, true, "Swaziland" },
                    { "SY", "SYR", false, true, true, false, true, "Syria" },
                    { "SX", "SXM", false, true, true, false, true, "Sint Maarten" },
                    { "KG", "KGZ", false, true, true, false, true, "Kyrgyzstan" },
                    { "KE", "KEN", false, true, true, false, true, "Kenya" },
                    { "SS", "SSD", false, true, true, false, true, "South Sudan" },
                    { "SR", "SUR", false, true, true, false, true, "Suriname" },
                    { "KI", "KIR", false, true, true, false, true, "Kiribati" },
                    { "KH", "KHM", false, true, true, false, true, "Cambodia" },
                    { "KN", "KNA", false, true, true, false, true, "Saint Kitts and Nevis" },
                    { "KM", "COM", false, true, true, false, true, "Comoros" },
                    { "ST", "STP", false, true, true, false, true, "Sao Tome and Principe" },
                    { "SK", "SVK", false, true, true, false, true, "Slovakia" },
                    { "KR", "KOR", false, true, true, false, true, "South Korea" },
                    { "SI", "SVN", false, true, true, false, true, "Slovenia" },
                    { "KP", "PRK", false, true, true, false, true, "North Korea" },
                    { "KW", "KWT", false, true, true, false, true, "Kuwait" },
                    { "SN", "SEN", false, true, true, false, true, "Senegal" },
                    { "SM", "SMR", false, true, true, false, true, "San Marino" },
                    { "SL", "SLE", false, true, true, false, true, "Sierra Leone" },
                    { "SC", "SYC", false, true, true, false, true, "Seychelles" },
                    { "CZ", "CZE", false, true, true, false, true, "Czech Republic" },
                    { "KZ", "KAZ", false, true, true, false, true, "Kazakhstan" },
                    { "CD", "COD", false, true, true, false, true, "Democratic Republic of the Congo" },
                    { "CG", "COG", false, true, true, false, true, "Republic of the Congo" },
                    { "FJ", "FJI", false, true, true, false, true, "Fiji" },
                    { "FK", "FLK", false, true, true, false, true, "Falkland Islands" },
                    { "FM", "FSM", false, true, true, false, true, "Micronesia" },
                    { "FO", "FRO", false, true, true, false, true, "Faroe Islands" },
                    { "NI", "NIC", false, true, true, false, true, "Nicaragua" },
                    { "NL", "NLD", false, true, true, false, true, "Netherlands" },
                    { "NO", "NOR", false, true, true, false, true, "Norway" },
                    { "NA", "NAM", false, true, true, false, true, "Namibia" },
                    { "VU", "VUT", false, true, true, false, true, "Vanuatu" },
                    { "NC", "NCL", false, true, true, false, true, "New Caledonia" },
                    { "NE", "NER", false, true, true, false, true, "Niger" },
                    { "NF", "NFK", false, true, true, false, true, "Norfolk Island" },
                    { "NG", "NGA", false, true, true, false, true, "Nigeria" },
                    { "NZ", "NZL", false, true, true, false, true, "New Zealand" },
                    { "NP", "NPL", false, true, true, false, true, "Nepal" },
                    { "NR", "NRU", false, true, true, false, true, "Nauru" },
                    { "NU", "NIU", false, true, true, false, true, "Niue" },
                    { "CK", "COK", false, true, true, false, true, "Cook Islands" },
                    { "XK", "XKX", false, true, true, false, true, "Kosovo" },
                    { "CI", "CIV", false, true, true, false, true, "Ivory Coast" },
                    { "CH", "CHE", false, true, true, false, true, "Switzerland" },
                    { "CO", "COL", false, true, true, false, true, "Colombia" },
                    { "CN", "CHN", false, true, true, false, true, "China" },
                    { "CM", "CMR", false, true, true, false, true, "Cameroon" },
                    { "CL", "CHL", false, true, true, false, true, "Chile" },
                    { "CC", "CCK", false, true, true, false, true, "Cocos Islands" },
                    { "CA", "CAN", false, true, true, false, true, "Canada" },
                    { "CF", "CAF", false, true, true, false, true, "Central African Republic" },
                    { "KY", "CYM", false, true, true, false, true, "Cayman Islands" },
                    { "SG", "SGP", false, true, true, false, true, "Singapore" },
                    { "SE", "SWE", false, true, true, false, true, "Sweden" },
                    { "TC", "TCA", false, true, true, false, true, "Turks and Caicos Islands" },
                    { "LY", "LBY", false, true, true, false, true, "Libya" },
                    { "VA", "VAT", false, true, true, false, true, "Vatican" },
                    { "VC", "VCT", false, true, true, false, true, "Saint Vincent and the Grenadines" },
                    { "AE", "ARE", false, true, true, false, true, "United Arab Emirates" },
                    { "AD", "AND", false, true, true, false, true, "Andorra" },
                    { "AG", "ATG", false, true, true, false, true, "Antigua and Barbuda" },
                    { "AF", "AFG", false, true, true, false, true, "Afghanistan" },
                    { "AI", "AIA", false, true, true, false, true, "Anguilla" },
                    { "VI", "VIR", false, true, true, false, true, "U.S. Virgin Islands" },
                    { "IS", "ISL", false, true, true, false, true, "Iceland" },
                    { "IR", "IRN", false, true, true, false, true, "Iran" },
                    { "AM", "ARM", false, true, true, false, true, "Armenia" },
                    { "AL", "ALB", false, true, true, false, true, "Albania" },
                    { "AO", "AGO", false, true, true, false, true, "Angola" },
                    { "AQ", "ATA", false, true, true, false, true, "Antarctica" },
                    { "AS", "ASM", false, true, true, false, true, "American Samoa" },
                    { "AR", "ARG", false, true, true, false, true, "Argentina" },
                    { "AU", "AUS", false, true, true, false, true, "Australia" },
                    { "AT", "AUT", false, true, true, false, true, "Austria" },
                    { "AW", "ABW", false, true, true, false, true, "Aruba" },
                    { "IN", "IND", false, true, true, false, true, "India" },
                    { "AX", "ALA", false, true, true, false, true, "Aland Islands" },
                    { "AZ", "AZE", false, true, true, false, true, "Azerbaijan" },
                    { "IE", "IRL", false, true, true, false, true, "Ireland" },
                    { "ID", "IDN", false, true, true, false, true, "Indonesia" },
                    { "UA", "UKR", false, true, true, false, true, "Ukraine" },
                    { "TD", "TCD", false, true, true, false, true, "Chad" },
                    { "TG", "TGO", false, true, true, false, true, "Togo" },
                    { "TF", "ATF", false, true, true, false, true, "French Southern Territories" },
                    { "TH", "THA", false, true, true, false, true, "Thailand" },
                    { "SD", "SDN", false, true, true, false, true, "Sudan" },
                    { "DO", "DOM", false, true, true, false, true, "Dominican Republic" },
                    { "DM", "DMA", false, true, true, false, true, "Dominica" },
                    { "DJ", "DJI", false, true, true, false, true, "Djibouti" },
                    { "DK", "DNK", false, true, true, false, true, "Denmark" },
                    { "VG", "VGB", false, true, true, false, true, "British Virgin Islands" },
                    { "DE", "DEU", false, true, true, false, true, "Germany" },
                    { "YE", "YEM", false, true, true, false, true, "Yemen" },
                    { "DZ", "DZA", false, true, true, false, true, "Algeria" },
                    { "US", "USA", false, true, true, false, true, "United States" },
                    { "UY", "URY", false, true, true, false, true, "Uruguay" },
                    { "YT", "MYT", false, true, true, false, true, "Mayotte" },
                    { "UM", "UMI", false, true, true, false, true, "United States Minor Outlying Islands" },
                    { "FI", "FIN", false, true, true, false, true, "Finland" },
                    { "LB", "LBN", false, true, true, false, true, "Lebanon" },
                    { "LA", "LAO", false, true, true, false, true, "Laos" },
                    { "TV", "TUV", false, true, true, false, true, "Tuvalu" },
                    { "TW", "TWN", false, true, true, false, true, "Taiwan" },
                    { "TT", "TTO", false, true, true, false, true, "Trinidad and Tobago" },
                    { "TR", "TUR", false, true, true, false, true, "Turkey" },
                    { "LK", "LKA", false, true, true, false, true, "Sri Lanka" },
                    { "LI", "LIE", false, true, true, false, true, "Liechtenstein" },
                    { "LV", "LVA", false, true, true, false, true, "Latvia" },
                    { "TO", "TON", false, true, true, false, true, "Tonga" },
                    { "LT", "LTU", false, true, true, false, true, "Lithuania" },
                    { "LU", "LUX", false, true, true, false, true, "Luxembourg" },
                    { "LR", "LBR", false, true, true, false, true, "Liberia" },
                    { "LS", "LSO", false, true, true, false, true, "Lesotho" },
                    { "LC", "LCA", false, true, true, false, true, "Saint Lucia" },
                    { "SH", "SHN", false, true, true, false, true, "Saint Helena" },
                    { "IO", "IOT", false, true, true, false, true, "British Indian Ocean Territory" },
                    { "FR", "FRA", false, true, true, false, true, "France" },
                    { "TK", "TKL", false, true, true, false, true, "Tokelau" },
                    { "GW", "GNB", false, true, true, false, true, "Guinea-Bissau" },
                    { "GU", "GUM", false, true, true, false, true, "Guam" },
                    { "GT", "GTM", false, true, true, false, true, "Guatemala" },
                    { "GS", "SGS", false, true, true, false, true, "South Georgia and the South Sandwich Islands" },
                    { "GR", "GRC", false, true, true, false, true, "Greece" },
                    { "GQ", "GNQ", false, true, true, false, true, "Equatorial Guinea" },
                    { "GP", "GLP", false, true, true, false, true, "Guadeloupe" },
                    { "JP", "JPN", false, true, true, false, true, "Japan" },
                    { "GY", "GUY", false, true, true, false, true, "Guyana" },
                    { "GG", "GGY", false, true, true, false, true, "Guernsey" },
                    { "GF", "GUF", false, true, true, false, true, "French Guiana" },
                    { "GE", "GEO", false, true, true, false, true, "Georgia" },
                    { "GD", "GRD", false, true, true, false, true, "Grenada" },
                    { "GB", "GBR", false, true, true, false, true, "United Kingdom" },
                    { "GA", "GAB", false, true, true, false, true, "Gabon" },
                    { "SV", "SLV", false, true, true, false, true, "El Salvador" },
                    { "GN", "GIN", false, true, true, false, true, "Guinea" },
                    { "GM", "GMB", false, true, true, false, true, "Gambia" },
                    { "GL", "GRL", false, true, true, false, true, "Greenland" },
                    { "GI", "GIB", false, true, true, false, true, "Gibraltar" },
                    { "GH", "GHA", false, true, true, false, true, "Ghana" },
                    { "OM", "OMN", false, true, true, false, true, "Oman" },
                    { "TN", "TUN", false, true, true, false, true, "Tunisia" },
                    { "JO", "JOR", false, true, true, false, true, "Jordan" },
                    { "HR", "HRV", false, true, true, false, true, "Croatia" },
                    { "HT", "HTI", false, true, true, false, true, "Haiti" },
                    { "RO", "ROU", false, true, true, false, true, "Romania" },
                    { "TJ", "TJK", false, true, true, false, true, "Tajikistan" },
                    { "TM", "TKM", false, true, true, false, true, "Turkmenistan" },
                    { "RE", "REU", false, true, true, false, true, "Reunion" },
                    { "BE", "BEL", false, true, true, false, true, "Belgium" },
                    { "BF", "BFA", false, true, true, false, true, "Burkina Faso" },
                    { "BG", "BGR", false, true, true, false, true, "Bulgaria" },
                    { "BA", "BIH", false, true, true, false, true, "Bosnia and Herzegovina" },
                    { "BB", "BRB", false, true, true, false, true, "Barbados" },
                    { "WF", "WLF", false, true, true, false, true, "Wallis and Futuna" },
                    { "BL", "BLM", false, true, true, false, true, "Saint Barthelemy" },
                    { "BM", "BMU", false, true, true, false, true, "Bermuda" },
                    { "BN", "BRN", false, true, true, false, true, "Brunei" },
                    { "BO", "BOL", false, true, true, false, true, "Bolivia" },
                    { "BH", "BHR", false, true, true, false, true, "Bahrain" },
                    { "BI", "BDI", false, true, true, false, true, "Burundi" },
                    { "BJ", "BEN", false, true, true, false, true, "Benin" },
                    { "HU", "HUN", false, true, true, false, true, "Hungary" },
                    { "BT", "BTN", false, true, true, false, true, "Bhutan" },
                    { "BV", "BVT", false, true, true, false, true, "Bouvet Island" },
                    { "BW", "BWA", false, true, true, false, true, "Botswana" },
                    { "WS", "WSM", false, true, true, false, true, "Samoa" },
                    { "BQ", "BES", false, true, true, false, true, "Bonaire, Saint Eustatius and Saba " },
                    { "BR", "BRA", false, true, true, false, true, "Brazil" },
                    { "BS", "BHS", false, true, true, false, true, "Bahamas" },
                    { "JE", "JEY", false, true, true, false, true, "Jersey" },
                    { "BY", "BLR", false, true, true, false, true, "Belarus" },
                    { "BZ", "BLZ", false, true, true, false, true, "Belize" },
                    { "RU", "RUS", false, true, true, false, true, "Russia" },
                    { "RW", "RWA", false, true, true, false, true, "Rwanda" },
                    { "RS", "SRB", false, true, true, false, true, "Serbia" },
                    { "TL", "TLS", false, true, true, false, true, "East Timor" },
                    { "JM", "JAM", false, true, true, false, true, "Jamaica" },
                    { "QA", "QAT", false, true, true, false, true, "Qatar" },
                    { "HK", "HKG", false, true, true, false, true, "Hong Kong" },
                    { "HM", "HMD", false, true, true, false, true, "Heard Island and McDonald Islands" },
                    { "ME", "MNE", false, true, true, false, true, "Montenegro" },
                    { "MD", "MDA", false, true, true, false, true, "Moldova" },
                    { "MG", "MDG", false, true, true, false, true, "Madagascar" },
                    { "MF", "MAF", false, true, true, false, true, "Saint Martin" },
                    { "MA", "MAR", false, true, true, false, true, "Morocco" },
                    { "MC", "MCO", false, true, true, false, true, "Monaco" },
                    { "UZ", "UZB", false, true, true, false, true, "Uzbekistan" },
                    { "MM", "MMR", false, true, true, false, true, "Myanmar" },
                    { "ML", "MLI", false, true, true, false, true, "Mali" },
                    { "MO", "MAC", false, true, true, false, true, "Macao" },
                    { "MN", "MNG", false, true, true, false, true, "Mongolia" },
                    { "MH", "MHL", false, true, true, false, true, "Marshall Islands" },
                    { "MK", "MKD", false, true, true, false, true, "Macedonia" },
                    { "MU", "MUS", false, true, true, false, true, "Mauritius" },
                    { "MT", "MLT", false, true, true, false, true, "Malta" },
                    { "MW", "MWI", false, true, true, false, true, "Malawi" },
                    { "MV", "MDV", false, true, true, false, true, "Maldives" },
                    { "MQ", "MTQ", false, true, true, false, true, "Martinique" },
                    { "MP", "MNP", false, true, true, false, true, "Northern Mariana Islands" },
                    { "MS", "MSR", false, true, true, false, true, "Montserrat" },
                    { "MR", "MRT", false, true, true, false, true, "Mauritania" },
                    { "IM", "IMN", false, true, true, false, true, "Isle of Man" },
                    { "UG", "UGA", false, true, true, false, true, "Uganda" },
                    { "TZ", "TZA", false, true, true, false, true, "Tanzania" },
                    { "MY", "MYS", false, true, true, false, true, "Malaysia" },
                    { "MX", "MEX", false, true, true, false, true, "Mexico" },
                    { "IL", "ISR", false, true, true, false, true, "Israel" },
                    { "ER", "ERI", false, true, true, false, true, "Eritrea" },
                    { "ES", "ESP", false, true, true, false, true, "Spain" },
                    { "SA", "SAU", false, true, true, false, true, "Saudi Arabia" },
                    { "ZW", "ZWE", false, true, true, false, true, "Zimbabwe" },
                    { "VE", "VEN", false, true, true, false, true, "Venezuela" },
                    { "PR", "PRI", false, true, true, false, true, "Puerto Rico" },
                    { "PS", "PSE", false, true, true, false, true, "Palestinian Territory" },
                    { "PW", "PLW", false, true, true, false, true, "Palau" },
                    { "PT", "PRT", false, true, true, false, true, "Portugal" },
                    { "SJ", "SJM", false, true, true, false, true, "Svalbard and Jan Mayen" },
                    { "PY", "PRY", false, true, true, false, true, "Paraguay" },
                    { "IQ", "IRQ", false, true, true, false, true, "Iraq" },
                    { "PA", "PAN", false, true, true, false, true, "Panama" },
                    { "PF", "PYF", false, true, true, false, true, "French Polynesia" },
                    { "PG", "PNG", false, true, true, false, true, "Papua New Guinea" },
                    { "PE", "PER", false, true, true, false, true, "Peru" },
                    { "PK", "PAK", false, true, true, false, true, "Pakistan" },
                    { "HN", "HND", false, true, true, false, true, "Honduras" },
                    { "PH", "PHL", false, true, true, false, true, "Philippines" },
                    { "PL", "POL", false, true, true, false, true, "Poland" },
                    { "PM", "SPM", false, true, true, false, true, "Saint Pierre and Miquelon" },
                    { "ZM", "ZMB", false, true, true, false, true, "Zambia" },
                    { "EH", "ESH", false, true, true, false, true, "Western Sahara" },
                    { "EE", "EST", false, true, true, false, true, "Estonia" },
                    { "EG", "EGY", false, true, true, false, true, "Egypt" },
                    { "ZA", "ZAF", false, true, true, false, true, "South Africa" },
                    { "EC", "ECU", false, true, true, false, true, "Ecuador" },
                    { "IT", "ITA", false, true, true, false, true, "Italy" },
                    { "VN", "VNM", false, true, true, false, true, "Vietnam" },
                    { "SB", "SLB", false, true, true, false, true, "Solomon Islands" },
                    { "ET", "ETH", false, true, true, false, true, "Ethiopia" },
                    { "SO", "SOM", false, true, true, false, true, "Somalia" },
                    { "PN", "PCN", false, true, true, false, true, "Pitcairn" },
                    { "MZ", "MOZ", false, true, true, false, true, "Mozambique" }
                });

            migrationBuilder.InsertData(
                schema: "Identity",
                table: "StateOrProvinces",
                columns: new[] { "Id", "Code", "CountryId", "Name", "Type" },
                values: new object[,]
                {
                    { 1L, null, "BD", "Dhaka", null },
                    { 159L, null, "CY", "Nicosia", null },
                    { 160L, null, "CX", "Flying Fish Cove", null },
                    { 161L, null, "CR", "San Jose", null },
                    { 162L, null, "CW", " Willemstad", null },
                    { 163L, null, "CV", "Praia", null },
                    { 164L, null, "CU", "Havana", null },
                    { 165L, null, "SZ", "Mbabane", null },
                    { 166L, null, "SY", "Damascus", null },
                    { 167L, null, "SX", "Philipsburg", null },
                    { 168L, null, "KG", "Bishkek", null },
                    { 169L, null, "KE", "Nairobi", null },
                    { 170L, null, "SS", "Juba", null },
                    { 171L, null, "SR", "Paramaribo", null },
                    { 172L, null, "KI", "Tarawa", null },
                    { 173L, null, "KH", "Phnom Penh", null },
                    { 174L, null, "KN", "Basseterre", null },
                    { 175L, null, "KM", "Moroni", null },
                    { 176L, null, "ST", "Sao Tome", null },
                    { 177L, null, "SK", "Bratislava", null },
                    { 178L, null, "KR", "Seoul", null },
                    { 179L, null, "SI", "Ljubljana", null },
                    { 180L, null, "KP", "Pyongyang", null },
                    { 181L, null, "KW", "Kuwait City", null },
                    { 182L, null, "SN", "Dakar", null },
                    { 183L, null, "SM", "San Marino", null },
                    { 184L, null, "SL", "Freetown", null },
                    { 185L, null, "SC", "Victoria", null },
                    { 158L, null, "CZ", "Prague", null },
                    { 186L, null, "KZ", "Astana", null },
                    { 157L, null, "CD", "Kinshasa", null },
                    { 155L, null, "CG", "Brazzaville", null },
                    { 128L, null, "FJ", "Suva", null },
                    { 129L, null, "FK", "Stanley", null },
                    { 130L, null, "FM", "Palikir", null },
                    { 131L, null, "FO", "Torshavn", null },
                    { 132L, null, "NI", "Managua", null },
                    { 133L, null, "NL", "Amsterdam", null },
                    { 134L, null, "NO", "Oslo", null },
                    { 135L, null, "NA", "Windhoek", null },
                    { 136L, null, "VU", "Port Vila", null },
                    { 137L, null, "NC", "Noumea", null },
                    { 138L, null, "NE", "Niamey", null },
                    { 139L, null, "NF", "Kingston", null },
                    { 140L, null, "NG", "Abuja", null },
                    { 141L, null, "NZ", "Wellington", null },
                    { 142L, null, "NP", "Kathmandu", null },
                    { 143L, null, "NR", "Yaren", null },
                    { 144L, null, "NU", "Alofi", null },
                    { 145L, null, "CK", "Avarua", null },
                    { 146L, null, "XK", "Pristina", null },
                    { 147L, null, "CI", "Yamoussoukro", null },
                    { 148L, null, "CH", "Berne", null },
                    { 149L, null, "CO", "Bogota", null },
                    { 150L, null, "CN", "Beijing", null },
                    { 151L, null, "CM", "Yaounde", null },
                    { 152L, null, "CL", "Santiago", null },
                    { 153L, null, "CC", "West Island", null },
                    { 154L, null, "CA", "Ottawa", null },
                    { 156L, null, "CF", "Bangui", null },
                    { 187L, null, "KY", "George Town", null },
                    { 188L, null, "SG", "Singapur", null },
                    { 189L, null, "SE", "Stockholm", null },
                    { 222L, null, "TC", "Cockburn Town", null },
                    { 223L, null, "LY", "Tripolis", null },
                    { 224L, null, "VA", "Vatican City", null },
                    { 225L, null, "VC", "Kingstown", null },
                    { 226L, null, "AE", "Abu Dhabi", null },
                    { 227L, null, "AD", "Andorra la Vella", null },
                    { 228L, null, "AG", "GR. John's", null },
                    { 229L, null, "AF", "Kabul", null },
                    { 230L, null, "AI", "The Valley", null },
                    { 231L, null, "VI", "Charlotte Amalie", null },
                    { 232L, null, "IS", "Reykjavik", null },
                    { 233L, null, "IR", "Tehran", null },
                    { 234L, null, "AM", "Yerevan", null },
                    { 235L, null, "AL", "Tirana", null },
                    { 236L, null, "AO", "Luanda", null },
                    { 237L, null, "AQ", "", null },
                    { 238L, null, "AS", "Pago Pago", null },
                    { 239L, null, "AR", "Buenos Aires", null },
                    { 240L, null, "AU", "Canberra", null },
                    { 241L, null, "AT", "Vienna", null },
                    { 242L, null, "AW", "Oranjestad", null },
                    { 243L, null, "IN", "New Delhi", null },
                    { 244L, null, "AX", "Mariehamn", null },
                    { 245L, null, "AZ", "Baku", null },
                    { 246L, null, "IE", "Dublin", null },
                    { 247L, null, "ID", "Jakarta", null },
                    { 248L, null, "UA", "Kiev", null },
                    { 221L, null, "TD", "N'Djamena", null },
                    { 220L, null, "TG", "Lome", null },
                    { 219L, null, "TF", "Port-aux-Francais", null },
                    { 218L, null, "TH", "Bangkok", null },
                    { 190L, null, "SD", "Khartoum", null },
                    { 191L, null, "DO", "Santo Domingo", null },
                    { 192L, null, "DM", "Roseau", null },
                    { 193L, null, "DJ", "Djibouti", null },
                    { 194L, null, "DK", "Copenhagen", null },
                    { 195L, null, "VG", "Road Town", null },
                    { 196L, null, "DE", "Berlin", null },
                    { 197L, null, "YE", "Sanaa", null },
                    { 198L, null, "DZ", "Algiers", null },
                    { 199L, null, "US", "Washington", null },
                    { 200L, null, "UY", "Montevideo", null },
                    { 201L, null, "YT", "Mamoudzou", null },
                    { 202L, null, "UM", "", null },
                    { 127L, null, "FI", "Helsinki", null },
                    { 203L, null, "LB", "Beirut", null },
                    { 205L, null, "LA", "Vientiane", null },
                    { 206L, null, "TV", "Funafuti", null },
                    { 207L, null, "TW", "Taipei", null },
                    { 208L, null, "TT", "Port of Spain", null },
                    { 209L, null, "TR", "Ankara", null },
                    { 210L, null, "LK", "Colombo", null },
                    { 211L, null, "LI", "Vaduz", null },
                    { 212L, null, "LV", "Riga", null },
                    { 213L, null, "TO", "Nuku'alofa", null },
                    { 214L, null, "LT", "Vilnius", null },
                    { 215L, null, "LU", "Luxembourg", null },
                    { 216L, null, "LR", "Monrovia", null },
                    { 217L, null, "LS", "Maseru", null },
                    { 204L, null, "LC", "Castries", null },
                    { 126L, null, "SH", "Jamestown", null },
                    { 125L, null, "IO", "Diego Garcia", null },
                    { 124L, null, "FR", "Paris", null },
                    { 34L, null, "TK", "", null },
                    { 35L, null, "GW", "Bissau", null },
                    { 36L, null, "GU", "Hagatna", null },
                    { 37L, null, "GT", "Guatemala City", null },
                    { 38L, null, "GS", "Grytviken", null },
                    { 39L, null, "GR", "Athens", null },
                    { 40L, null, "GQ", "Malabo", null },
                    { 41L, null, "GP", "Basse-Terre", null },
                    { 42L, null, "JP", "Tokyo", null },
                    { 43L, null, "GY", "Georgetown", null },
                    { 44L, null, "GG", "St Peter Port", null },
                    { 45L, null, "GF", "Cayenne", null },
                    { 46L, null, "GE", "Tbilisi", null },
                    { 47L, null, "GD", "GR. George's", null },
                    { 48L, null, "GB", "London", null },
                    { 49L, null, "GA", "Libreville", null },
                    { 50L, null, "SV", "San Salvador", null },
                    { 51L, null, "GN", "Conakry", null },
                    { 52L, null, "GM", "Banjul", null },
                    { 53L, null, "GL", "Nuuk", null },
                    { 54L, null, "GI", "Gibraltar", null },
                    { 55L, null, "GH", "Accra", null },
                    { 56L, null, "OM", "Muscat", null },
                    { 57L, null, "TN", "Tunis", null },
                    { 58L, null, "JO", "Amman", null },
                    { 59L, null, "HR", "Zagreb", null },
                    { 60L, null, "HT", "Port-au-Prince", null },
                    { 33L, null, "RO", "Bucharest", null },
                    { 32L, null, "TJ", "Dushanbe", null },
                    { 31L, null, "TM", "Ashgabat", null },
                    { 30L, null, "RE", "Saint-Denis", null },
                    { 2L, null, "BE", "Brussels", null },
                    { 3L, null, "BF", "Ouagadougou", null },
                    { 4L, null, "BG", "Sofia", null },
                    { 5L, null, "BA", "Sarajevo", null },
                    { 6L, null, "BB", "Bridgetown", null },
                    { 7L, null, "WF", "Mata Utu", null },
                    { 8L, null, "BL", "Gustavia", null },
                    { 9L, null, "BM", "Hamilton", null },
                    { 10L, null, "BN", "Bandar Seri Begawan", null },
                    { 11L, null, "BO", "Sucre", null },
                    { 12L, null, "BH", "Manama", null },
                    { 13L, null, "BI", "Bujumbura", null },
                    { 14L, null, "BJ", "Porto-Novo", null },
                    { 61L, null, "HU", "Budapest", null },
                    { 15L, null, "BT", "Thimphu", null },
                    { 17L, null, "BV", "", null },
                    { 18L, null, "BW", "Gaborone", null },
                    { 19L, null, "WS", "Apia", null },
                    { 20L, null, "BQ", "", null },
                    { 21L, null, "BR", "Brasilia", null },
                    { 22L, null, "BS", "Nassau", null },
                    { 23L, null, "JE", "Saint Helier", null },
                    { 24L, null, "BY", "Minsk", null },
                    { 25L, null, "BZ", "Belmopan", null },
                    { 26L, null, "RU", "Moscow", null },
                    { 27L, null, "RW", "Kigali", null },
                    { 28L, null, "RS", "Belgrade", null },
                    { 29L, null, "TL", "Dili", null },
                    { 16L, null, "JM", "Kingston", null },
                    { 249L, null, "QA", "Doha", null },
                    { 62L, null, "HK", "Hong Kong", null },
                    { 64L, null, "HM", "", null },
                    { 97L, null, "ME", "Podgorica", null },
                    { 98L, null, "MD", "Chisinau", null },
                    { 99L, null, "MG", "Antananarivo", null },
                    { 100L, null, "MF", "Marigot", null },
                    { 101L, null, "MA", "Rabat", null },
                    { 102L, null, "MC", "Monaco", null },
                    { 103L, null, "UZ", "Tashkent", null },
                    { 104L, null, "MM", "Nay Pyi Taw", null },
                    { 105L, null, "ML", "Bamako", null },
                    { 106L, null, "MO", "Macao", null },
                    { 107L, null, "MN", "Ulan Bator", null },
                    { 108L, null, "MH", "Majuro", null },
                    { 109L, null, "MK", "Skopje", null },
                    { 110L, null, "MU", "Port Louis", null },
                    { 111L, null, "MT", "Valletta", null },
                    { 112L, null, "MW", "Lilongwe", null },
                    { 113L, null, "MV", "Male", null },
                    { 114L, null, "MQ", "Fort-de-France", null },
                    { 115L, null, "MP", "Saipan", null },
                    { 116L, null, "MS", "Plymouth", null },
                    { 117L, null, "MR", "Nouakchott", null },
                    { 118L, null, "IM", "Douglas, Isle of Man", null },
                    { 119L, null, "UG", "Kampala", null },
                    { 120L, null, "TZ", "Dodoma", null },
                    { 121L, null, "MY", "Kuala Lumpur", null },
                    { 122L, null, "MX", "Mexico City", null },
                    { 123L, null, "IL", "Jerusalem", null },
                    { 96L, null, "ER", "Asmara", null },
                    { 95L, null, "ES", "Madrid", null },
                    { 94L, null, "SA", "Riyadh", null },
                    { 93L, null, "ZW", "Harare", null },
                    { 65L, null, "VE", "Caracas", null },
                    { 66L, null, "PR", "San Juan", null },
                    { 67L, null, "PS", "East Jerusalem", null },
                    { 68L, null, "PW", "Melekeok", null },
                    { 69L, null, "PT", "Lisbon", null },
                    { 70L, null, "SJ", "Longyearbyen", null },
                    { 71L, null, "PY", "Asuncion", null },
                    { 72L, null, "IQ", "Baghdad", null },
                    { 73L, null, "PA", "Panama City", null },
                    { 74L, null, "PF", "Papeete", null },
                    { 75L, null, "PG", "Port Moresby", null },
                    { 76L, null, "PE", "Lima", null },
                    { 77L, null, "PK", "Islamabad", null },
                    { 63L, null, "HN", "Tegucigalpa", null },
                    { 78L, null, "PH", "Manila", null },
                    { 80L, null, "PL", "Warsaw", null },
                    { 81L, null, "PM", "Saint-Pierre", null },
                    { 82L, null, "ZM", "Lusaka", null },
                    { 83L, null, "EH", "El-Aaiun", null },
                    { 84L, null, "EE", "Tallinn", null },
                    { 85L, null, "EG", "Cairo", null },
                    { 86L, null, "ZA", "Pretoria", null },
                    { 87L, null, "EC", "Quito", null },
                    { 88L, null, "IT", "Rome", null },
                    { 89L, null, "VN", "Hanoi", null },
                    { 90L, null, "SB", "Honiara", null },
                    { 91L, null, "ET", "Addis Ababa", null },
                    { 92L, null, "SO", "Mogadishu", null },
                    { 79L, null, "PN", "Adamstown", null },
                    { 250L, null, "MZ", "Maputo", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_ApplicationUserId",
                schema: "Identity",
                table: "Addresses",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CountryId",
                schema: "Identity",
                table: "Addresses",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_DistrictId",
                schema: "Identity",
                table: "Addresses",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_StateOrProvinceId",
                schema: "Identity",
                table: "Addresses",
                column: "StateOrProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthGroups_TenantId",
                schema: "Identity",
                table: "AuthGroups",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Districts_StateOrProvinceId",
                schema: "Identity",
                table: "Districts",
                column: "StateOrProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupPermissions_AuthGroupId",
                schema: "Identity",
                table: "GroupPermissions",
                column: "AuthGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_TenantId",
                schema: "Identity",
                table: "Permissions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_TenantId",
                schema: "Identity",
                table: "Profiles",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                schema: "Identity",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                schema: "Identity",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId",
                schema: "Identity",
                table: "RolePermissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleProfiles_ProfileId",
                schema: "Identity",
                table: "RoleProfiles",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "Identity",
                table: "Roles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_TenantId",
                schema: "Identity",
                table: "Roles",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_StateOrProvinces_CountryId",
                schema: "Identity",
                table: "StateOrProvinces",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_CityId",
                schema: "Identity",
                table: "Tenants",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_CountryId",
                schema: "Identity",
                table: "Tenants",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackAuditDetails_TrackAuditId",
                schema: "Identity",
                table: "TrackAuditDetails",
                column: "TrackAuditId");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                schema: "Identity",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroups_UserId",
                schema: "Identity",
                table: "UserGroups",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                schema: "Identity",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                schema: "Identity",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "Identity",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "Identity",
                table: "Users",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId",
                schema: "Identity",
                table: "Users",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "GroupPermissions",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "RoleClaims",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "RolePermissions",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "RoleProfiles",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Tenants",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "TrackAuditDetails",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "UserClaims",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "UserGroups",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "UserLogins",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "UserRoles",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "UserTokens",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Districts",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Permissions",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Profiles",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "TrackAudits",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "AuthGroups",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "StateOrProvinces",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Countries",
                schema: "Identity");
        }
    }
}