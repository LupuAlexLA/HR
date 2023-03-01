using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class contractStateDepartament : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepartamentId",
                table: "Contracts",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Contracts_State",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    ContractsId = table.Column<int>(nullable: false),
                    Contract_State = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts_State", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contracts_State_Contracts_ContractsId",
                        column: x => x.ContractsId,
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_DepartamentId",
                table: "Contracts",
                column: "DepartamentId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_State_ContractsId",
                table: "Contracts_State",
                column: "ContractsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Departament_DepartamentId",
                table: "Contracts",
                column: "DepartamentId",
                principalTable: "Departament",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Departament_DepartamentId",
                table: "Contracts");

            migrationBuilder.DropTable(
                name: "Contracts_State");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_DepartamentId",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "DepartamentId",
                table: "Contracts");
        }
    }
}
