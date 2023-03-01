using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class update_BVC_DobandaReferintaTable_add_ActivityType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActivityTypeId",
                table: "BVC_DobandaReferinta",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BVC_DobandaReferinta_ActivityTypeId",
                table: "BVC_DobandaReferinta",
                column: "ActivityTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_DobandaReferinta_ActivityTypes_ActivityTypeId",
                table: "BVC_DobandaReferinta",
                column: "ActivityTypeId",
                principalTable: "ActivityTypes",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_DobandaReferinta_ActivityTypes_ActivityTypeId",
                table: "BVC_DobandaReferinta");

            migrationBuilder.DropIndex(
                name: "IX_BVC_DobandaReferinta_ActivityTypeId",
                table: "BVC_DobandaReferinta");

            migrationBuilder.DropColumn(
                name: "ActivityTypeId",
                table: "BVC_DobandaReferinta");
        }
    }
}
