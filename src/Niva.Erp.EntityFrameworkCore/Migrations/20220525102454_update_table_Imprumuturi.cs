using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class update_table_Imprumuturi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImprumuturiStare",
                table: "Imprumuturi");

            migrationBuilder.AddColumn<int>(
                name: "ThirdPartyId",
                table: "Imprumuturi",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContaOperationDetailId",
                table: "ImprumutState",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Imprumuturi_ThirdPartyId",
                table: "Imprumuturi",
                column: "ThirdPartyId");

            migrationBuilder.CreateIndex(
                name: "IX_ImprumutState_ContaOperationDetailId",
                table: "ImprumutState",
                column: "ContaOperationDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImprumutState_OperationsDetails_ContaOperationDetailId",
                table: "ImprumutState",
                column: "ContaOperationDetailId",
                principalTable: "OperationsDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Imprumuturi_Persons_ThirdPartyId",
                table: "Imprumuturi",
                column: "ThirdPartyId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImprumutState_OperationsDetails_ContaOperationDetailId",
                table: "ImprumutState");

            migrationBuilder.DropForeignKey(
                name: "FK_Imprumuturi_Persons_ThirdPartyId",
                table: "Imprumuturi");

            migrationBuilder.DropIndex(
                name: "IX_Imprumuturi_ThirdPartyId",
                table: "Imprumuturi");

            migrationBuilder.DropIndex(
                name: "IX_ImprumutState_ContaOperationDetailId",
                table: "ImprumutState");

            migrationBuilder.DropColumn(
                name: "ThirdPartyId",
                table: "Imprumuturi");

            migrationBuilder.DropColumn(
                name: "ContaOperationDetailId",
                table: "ImprumutState");

            migrationBuilder.AddColumn<int>(
                name: "ImprumuturiStare",
                table: "Imprumuturi",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
