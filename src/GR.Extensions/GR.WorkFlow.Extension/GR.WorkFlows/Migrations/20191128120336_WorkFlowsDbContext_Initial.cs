using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.WorkFlows.Migrations
{
    public partial class WorkFlowsDbContext_Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "WorkFlows");

            migrationBuilder.CreateTable(
                name: "TrackAudits",
                schema: "WorkFlows",
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
                name: "WorkflowActions",
                schema: "WorkFlows",
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
                    Name = table.Column<string>(nullable: false),
                    ClassName = table.Column<string>(nullable: false),
                    ClassNameWithNameSpace = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    IsSystem = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowActions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkFlows",
                schema: "WorkFlows",
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
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Enabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkFlows", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrackAuditDetails",
                schema: "WorkFlows",
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
                        principalSchema: "WorkFlows",
                        principalTable: "TrackAudits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "States",
                schema: "WorkFlows",
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
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    IsStartState = table.Column<bool>(nullable: false),
                    IsEndState = table.Column<bool>(nullable: false),
                    WorkFlowId = table.Column<Guid>(nullable: false),
                    AdditionalSettings = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.Id);
                    table.ForeignKey(
                        name: "FK_States_WorkFlows_WorkFlowId",
                        column: x => x.WorkFlowId,
                        principalSchema: "WorkFlows",
                        principalTable: "WorkFlows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transitions",
                schema: "WorkFlows",
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
                    Name = table.Column<string>(nullable: false),
                    FromStateId = table.Column<Guid>(nullable: false),
                    ToStateId = table.Column<Guid>(nullable: false),
                    WorkflowId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transitions_States_FromStateId",
                        column: x => x.FromStateId,
                        principalSchema: "WorkFlows",
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transitions_States_ToStateId",
                        column: x => x.ToStateId,
                        principalSchema: "WorkFlows",
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transitions_WorkFlows_WorkflowId",
                        column: x => x.WorkflowId,
                        principalSchema: "WorkFlows",
                        principalTable: "WorkFlows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransitionActions",
                schema: "WorkFlows",
                columns: table => new
                {
                    TransitionId = table.Column<Guid>(nullable: false),
                    ActionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransitionActions", x => new { x.TransitionId, x.ActionId });
                    table.ForeignKey(
                        name: "FK_TransitionActions_WorkflowActions_ActionId",
                        column: x => x.ActionId,
                        principalSchema: "WorkFlows",
                        principalTable: "WorkflowActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransitionActions_Transitions_TransitionId",
                        column: x => x.TransitionId,
                        principalSchema: "WorkFlows",
                        principalTable: "Transitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransitionRoles",
                schema: "WorkFlows",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(nullable: false),
                    TransitionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransitionRoles", x => new { x.RoleId, x.TransitionId });
                    table.ForeignKey(
                        name: "FK_TransitionRoles_Transitions_TransitionId",
                        column: x => x.TransitionId,
                        principalSchema: "WorkFlows",
                        principalTable: "Transitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_States_WorkFlowId",
                schema: "WorkFlows",
                table: "States",
                column: "WorkFlowId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackAuditDetails_TrackAuditId",
                schema: "WorkFlows",
                table: "TrackAuditDetails",
                column: "TrackAuditId");

            migrationBuilder.CreateIndex(
                name: "IX_TransitionActions_ActionId",
                schema: "WorkFlows",
                table: "TransitionActions",
                column: "ActionId");

            migrationBuilder.CreateIndex(
                name: "IX_TransitionRoles_TransitionId",
                schema: "WorkFlows",
                table: "TransitionRoles",
                column: "TransitionId");

            migrationBuilder.CreateIndex(
                name: "IX_Transitions_FromStateId",
                schema: "WorkFlows",
                table: "Transitions",
                column: "FromStateId");

            migrationBuilder.CreateIndex(
                name: "IX_Transitions_ToStateId",
                schema: "WorkFlows",
                table: "Transitions",
                column: "ToStateId");

            migrationBuilder.CreateIndex(
                name: "IX_Transitions_WorkflowId",
                schema: "WorkFlows",
                table: "Transitions",
                column: "WorkflowId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrackAuditDetails",
                schema: "WorkFlows");

            migrationBuilder.DropTable(
                name: "TransitionActions",
                schema: "WorkFlows");

            migrationBuilder.DropTable(
                name: "TransitionRoles",
                schema: "WorkFlows");

            migrationBuilder.DropTable(
                name: "TrackAudits",
                schema: "WorkFlows");

            migrationBuilder.DropTable(
                name: "WorkflowActions",
                schema: "WorkFlows");

            migrationBuilder.DropTable(
                name: "Transitions",
                schema: "WorkFlows");

            migrationBuilder.DropTable(
                name: "States",
                schema: "WorkFlows");

            migrationBuilder.DropTable(
                name: "WorkFlows",
                schema: "WorkFlows");
        }
    }
}
