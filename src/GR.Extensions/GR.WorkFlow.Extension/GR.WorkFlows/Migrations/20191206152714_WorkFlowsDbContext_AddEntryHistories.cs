using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.WorkFlows.Migrations
{
    public partial class WorkFlowsDbContext_AddEntryHistories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Message",
                schema: "WorkFlows",
                table: "EntryStates",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EntryStateHistories",
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
                    EntryStateId = table.Column<Guid>(nullable: false),
                    FromStateId = table.Column<Guid>(nullable: false),
                    ToStateId = table.Column<Guid>(nullable: false),
                    Message = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntryStateHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntryStateHistories_EntryStates_EntryStateId",
                        column: x => x.EntryStateId,
                        principalSchema: "WorkFlows",
                        principalTable: "EntryStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntryStateHistories_States_FromStateId",
                        column: x => x.FromStateId,
                        principalSchema: "WorkFlows",
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntryStateHistories_States_ToStateId",
                        column: x => x.ToStateId,
                        principalSchema: "WorkFlows",
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntryStateHistories_EntryStateId",
                schema: "WorkFlows",
                table: "EntryStateHistories",
                column: "EntryStateId");

            migrationBuilder.CreateIndex(
                name: "IX_EntryStateHistories_FromStateId",
                schema: "WorkFlows",
                table: "EntryStateHistories",
                column: "FromStateId");

            migrationBuilder.CreateIndex(
                name: "IX_EntryStateHistories_ToStateId",
                schema: "WorkFlows",
                table: "EntryStateHistories",
                column: "ToStateId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntryStateHistories",
                schema: "WorkFlows");

            migrationBuilder.DropColumn(
                name: "Message",
                schema: "WorkFlows",
                table: "EntryStates");
        }
    }
}
