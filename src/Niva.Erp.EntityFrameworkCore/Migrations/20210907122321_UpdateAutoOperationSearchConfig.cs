using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class UpdateAutoOperationSearchConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AutoOperationConfig_AutoOperationSearchConfig_AutoOperationSearchConfigId",
                table: "AutoOperationConfig");

            migrationBuilder.DropColumn(
                name: "AutoOperSearchConfigId",
                table: "AutoOperationConfig");

            migrationBuilder.AddForeignKey(
                name: "FK_AutoOperationConfig_AutoOperationSearchConfig_AutoOperationSearchConfigId",
                table: "AutoOperationConfig",
                column: "AutoOperationSearchConfigId",
                principalTable: "AutoOperationSearchConfig",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AutoOperationConfig_AutoOperationSearchConfig_AutoOperationSearchConfigId",
                table: "AutoOperationConfig");

            migrationBuilder.AddColumn<int>(
                name: "AutoOperSearchConfigId",
                table: "AutoOperationConfig",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_AutoOperationConfig_AutoOperationSearchConfig_AutoOperationSearchConfigId",
                table: "AutoOperationConfig",
                column: "AutoOperationSearchConfigId",
                principalTable: "AutoOperationSearchConfig",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
