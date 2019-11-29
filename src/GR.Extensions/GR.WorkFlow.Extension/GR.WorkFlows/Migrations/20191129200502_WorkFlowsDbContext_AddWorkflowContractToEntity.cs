using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.WorkFlows.Migrations
{
    public partial class WorkFlowsDbContext_AddWorkflowContractToEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contracts",
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
                    EntityName = table.Column<string>(nullable: true),
                    WorkFlowId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contracts_WorkFlows_WorkFlowId",
                        column: x => x.WorkFlowId,
                        principalSchema: "WorkFlows",
                        principalTable: "WorkFlows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntryStates",
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
                    ContractId = table.Column<Guid>(nullable: false),
                    EntryId = table.Column<string>(nullable: true),
                    StateId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntryStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntryStates_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalSchema: "WorkFlows",
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntryStates_States_StateId",
                        column: x => x.StateId,
                        principalSchema: "WorkFlows",
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_EntityName",
                schema: "WorkFlows",
                table: "Contracts",
                column: "EntityName");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_WorkFlowId",
                schema: "WorkFlows",
                table: "Contracts",
                column: "WorkFlowId");

            migrationBuilder.CreateIndex(
                name: "IX_EntryStates_ContractId",
                schema: "WorkFlows",
                table: "EntryStates",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_EntryStates_EntryId",
                schema: "WorkFlows",
                table: "EntryStates",
                column: "EntryId");

            migrationBuilder.CreateIndex(
                name: "IX_EntryStates_StateId",
                schema: "WorkFlows",
                table: "EntryStates",
                column: "StateId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntryStates",
                schema: "WorkFlows");

            migrationBuilder.DropTable(
                name: "Contracts",
                schema: "WorkFlows");
        }
    }
}
