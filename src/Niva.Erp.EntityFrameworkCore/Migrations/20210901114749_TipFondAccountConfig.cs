using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class TipFondAccountConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActivityTypeId",
                table: "AccountConfig",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountConfig_ActivityTypeId",
                table: "AccountConfig",
                column: "ActivityTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountConfig_ActivityTypes_ActivityTypeId",
                table: "AccountConfig",
                column: "ActivityTypeId",
                principalTable: "ActivityTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountConfig_ActivityTypes_ActivityTypeId",
                table: "AccountConfig");

            migrationBuilder.DropIndex(
                name: "IX_AccountConfig_ActivityTypeId",
                table: "AccountConfig");

            migrationBuilder.DropColumn(
                name: "ActivityTypeId",
                table: "AccountConfig");
        }
    }
}
