using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class UpdateAutoOperationSearchConfig1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AutoOperationConfig_AutoOperationSearchConfig_AutoOperationSearchConfigId",
                table: "AutoOperationConfig");

            migrationBuilder.DropIndex(
                name: "IX_AutoOperationConfig_AutoOperationSearchConfigId",
                table: "AutoOperationConfig");

            migrationBuilder.DropColumn(
                name: "AutoOperationSearchConfigId",
                table: "AutoOperationConfig");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AutoOperationSearchConfigId",
                table: "AutoOperationConfig",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AutoOperationConfig_AutoOperationSearchConfigId",
                table: "AutoOperationConfig",
                column: "AutoOperationSearchConfigId");

            migrationBuilder.AddForeignKey(
                name: "FK_AutoOperationConfig_AutoOperationSearchConfig_AutoOperationSearchConfigId",
                table: "AutoOperationConfig",
                column: "AutoOperationSearchConfigId",
                principalTable: "AutoOperationSearchConfig",
                principalColumn: "Id");
        }
    }
}
