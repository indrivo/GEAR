using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.Procesess.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Processes");

            migrationBuilder.CreateTable(
                name: "ProcessSchemas",
                schema: "Processes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Author = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Changed = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: true),
                    Diagram = table.Column<string>(nullable: false),
                    Description = table.Column<string>(maxLength: 50, nullable: true),
                    Synchronized = table.Column<bool>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    Version = table.Column<int>(nullable: false),
                    InitialSchemaId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessSchemas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessSchemas_ProcessSchemas_InitialSchemaId",
                        column: x => x.InitialSchemaId,
                        principalSchema: "Processes",
                        principalTable: "ProcessSchemas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TrackAudits",
                schema: "Processes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Author = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Changed = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DatabaseContextName = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    TrackEventType = table.Column<int>(nullable: false),
                    RecordId = table.Column<Guid>(nullable: false),
                    TypeFullName = table.Column<string>(nullable: true),
                    Version = table.Column<int>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackAudits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Processes",
                schema: "Processes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Author = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Changed = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    ProcessSchemaId = table.Column<Guid>(nullable: false),
                    Version = table.Column<int>(nullable: false),
                    IntitialProcessId = table.Column<Guid>(nullable: true),
                    ProcessSettings = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Processes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Processes_Processes_IntitialProcessId",
                        column: x => x.IntitialProcessId,
                        principalSchema: "Processes",
                        principalTable: "Processes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Processes_ProcessSchemas_ProcessSchemaId",
                        column: x => x.ProcessSchemaId,
                        principalSchema: "Processes",
                        principalTable: "ProcessSchemas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrackAuditDetails",
                schema: "Processes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Author = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Changed = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    TrackAuditId = table.Column<Guid>(nullable: false),
                    PropertyName = table.Column<string>(nullable: true),
                    PropertyType = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    TenantId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackAuditDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackAuditDetails_TrackAudits_TrackAuditId",
                        column: x => x.TrackAuditId,
                        principalSchema: "Processes",
                        principalTable: "TrackAudits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcessInstances",
                schema: "Processes",
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
                    StartedBy = table.Column<Guid>(nullable: false),
                    ProcessId = table.Column<Guid>(nullable: false),
                    InstanceState = table.Column<int>(nullable: false),
                    Progress = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessInstances_Processes_ProcessId",
                        column: x => x.ProcessId,
                        principalSchema: "Processes",
                        principalTable: "Processes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcessTransitions",
                schema: "Processes",
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
                    TransitionType = table.Column<int>(nullable: false),
                    ProcessId = table.Column<Guid>(nullable: false),
                    TransitionSettings = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessTransitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessTransitions_Processes_ProcessId",
                        column: x => x.ProcessId,
                        principalSchema: "Processes",
                        principalTable: "Processes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IncomingTransitions",
                schema: "Processes",
                columns: table => new
                {
                    ProcessTransitionId = table.Column<Guid>(nullable: false),
                    IncomingTransitionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomingTransitions", x => new { x.ProcessTransitionId, x.IncomingTransitionId });
                    table.ForeignKey(
                        name: "FK_IncomingTransitions_ProcessTransitions_IncomingTransitionId",
                        column: x => x.IncomingTransitionId,
                        principalSchema: "Processes",
                        principalTable: "ProcessTransitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OutGoingTransitions",
                schema: "Processes",
                columns: table => new
                {
                    ProcessTransitionId = table.Column<Guid>(nullable: false),
                    OutgoingTransitionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutGoingTransitions", x => new { x.ProcessTransitionId, x.OutgoingTransitionId });
                    table.ForeignKey(
                        name: "FK_OutGoingTransitions_ProcessTransitions_OutgoingTransitionId",
                        column: x => x.OutgoingTransitionId,
                        principalSchema: "Processes",
                        principalTable: "ProcessTransitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcessInstanceHistories",
                schema: "Processes",
                columns: table => new
                {
                    ProcessInstanceId = table.Column<Guid>(nullable: false),
                    ProcessTransitionId = table.Column<Guid>(nullable: false),
                    TransitionState = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessInstanceHistories", x => new { x.ProcessTransitionId, x.ProcessInstanceId });
                    table.ForeignKey(
                        name: "FK_ProcessInstanceHistories_ProcessInstances_ProcessInstanceId",
                        column: x => x.ProcessInstanceId,
                        principalSchema: "Processes",
                        principalTable: "ProcessInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProcessInstanceHistories_ProcessTransitions_ProcessTransiti~",
                        column: x => x.ProcessTransitionId,
                        principalSchema: "Processes",
                        principalTable: "ProcessTransitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProcessTasks",
                schema: "Processes",
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
                    ProcessTaskState = table.Column<int>(nullable: false),
                    Life = table.Column<int>(nullable: false),
                    Assigned = table.Column<Guid>(nullable: true),
                    ProcessTransitionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessTasks_ProcessTransitions_ProcessTransitionId",
                        column: x => x.ProcessTransitionId,
                        principalSchema: "Processes",
                        principalTable: "ProcessTransitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransitionActors",
                schema: "Processes",
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
                    RoleId = table.Column<Guid>(nullable: false),
                    ProcessTransitionId = table.Column<Guid>(nullable: false),
                    ActorSettings = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransitionActors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransitionActors_ProcessTransitions_ProcessTransitionId",
                        column: x => x.ProcessTransitionId,
                        principalSchema: "Processes",
                        principalTable: "ProcessTransitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserProcessTasks",
                schema: "Processes",
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
                    ProcessTaskId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProcessTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProcessTasks_ProcessTasks_ProcessTaskId",
                        column: x => x.ProcessTaskId,
                        principalSchema: "Processes",
                        principalTable: "ProcessTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncomingTransitions_IncomingTransitionId",
                schema: "Processes",
                table: "IncomingTransitions",
                column: "IncomingTransitionId");

            migrationBuilder.CreateIndex(
                name: "IX_OutGoingTransitions_OutgoingTransitionId",
                schema: "Processes",
                table: "OutGoingTransitions",
                column: "OutgoingTransitionId");

            migrationBuilder.CreateIndex(
                name: "IX_Processes_IntitialProcessId",
                schema: "Processes",
                table: "Processes",
                column: "IntitialProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_Processes_ProcessSchemaId",
                schema: "Processes",
                table: "Processes",
                column: "ProcessSchemaId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessInstanceHistories_ProcessInstanceId",
                schema: "Processes",
                table: "ProcessInstanceHistories",
                column: "ProcessInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessInstances_ProcessId",
                schema: "Processes",
                table: "ProcessInstances",
                column: "ProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessSchemas_InitialSchemaId",
                schema: "Processes",
                table: "ProcessSchemas",
                column: "InitialSchemaId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessTasks_ProcessTransitionId",
                schema: "Processes",
                table: "ProcessTasks",
                column: "ProcessTransitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessTransitions_ProcessId",
                schema: "Processes",
                table: "ProcessTransitions",
                column: "ProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackAuditDetails_TrackAuditId",
                schema: "Processes",
                table: "TrackAuditDetails",
                column: "TrackAuditId");

            migrationBuilder.CreateIndex(
                name: "IX_TransitionActors_ProcessTransitionId",
                schema: "Processes",
                table: "TransitionActors",
                column: "ProcessTransitionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProcessTasks_ProcessTaskId",
                schema: "Processes",
                table: "UserProcessTasks",
                column: "ProcessTaskId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncomingTransitions",
                schema: "Processes");

            migrationBuilder.DropTable(
                name: "OutGoingTransitions",
                schema: "Processes");

            migrationBuilder.DropTable(
                name: "ProcessInstanceHistories",
                schema: "Processes");

            migrationBuilder.DropTable(
                name: "TrackAuditDetails",
                schema: "Processes");

            migrationBuilder.DropTable(
                name: "TransitionActors",
                schema: "Processes");

            migrationBuilder.DropTable(
                name: "UserProcessTasks",
                schema: "Processes");

            migrationBuilder.DropTable(
                name: "ProcessInstances",
                schema: "Processes");

            migrationBuilder.DropTable(
                name: "TrackAudits",
                schema: "Processes");

            migrationBuilder.DropTable(
                name: "ProcessTasks",
                schema: "Processes");

            migrationBuilder.DropTable(
                name: "ProcessTransitions",
                schema: "Processes");

            migrationBuilder.DropTable(
                name: "Processes",
                schema: "Processes");

            migrationBuilder.DropTable(
                name: "ProcessSchemas",
                schema: "Processes");
        }
    }
}
