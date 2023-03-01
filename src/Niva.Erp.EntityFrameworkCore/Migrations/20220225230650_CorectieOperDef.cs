using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class CorectieOperDef : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OperationDefinitionDetails_OperationDefinition_OperationDefinitionId",
                table: "OperationDefinitionDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationDefinitionDetails_OperationDefinition_OperationDefinitionId1",
                table: "OperationDefinitionDetails");

            migrationBuilder.DropIndex(
                name: "IX_OperationDefinitionDetails_OperationDefinitionId1",
                table: "OperationDefinitionDetails");

            migrationBuilder.DropColumn(
                name: "OperationDefinitionId1",
                table: "OperationDefinitionDetails");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationDefinitionDetails_OperationDefinition_OperationDefinitionId",
                table: "OperationDefinitionDetails",
                column: "OperationDefinitionId",
                principalTable: "OperationDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OperationDefinitionDetails_OperationDefinition_OperationDefinitionId",
                table: "OperationDefinitionDetails");

            migrationBuilder.AddColumn<int>(
                name: "OperationDefinitionId1",
                table: "OperationDefinitionDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OperationDefinitionDetails_OperationDefinitionId1",
                table: "OperationDefinitionDetails",
                column: "OperationDefinitionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationDefinitionDetails_OperationDefinition_OperationDefinitionId",
                table: "OperationDefinitionDetails",
                column: "OperationDefinitionId",
                principalTable: "OperationDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationDefinitionDetails_OperationDefinition_OperationDefinitionId1",
                table: "OperationDefinitionDetails",
                column: "OperationDefinitionId1",
                principalTable: "OperationDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
