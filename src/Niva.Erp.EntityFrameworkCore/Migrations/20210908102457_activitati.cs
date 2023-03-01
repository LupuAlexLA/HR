using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class activitati : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActivityTypeId",
                table: "Imprumuturi",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Imprumuturi_ActivityTypeId",
                table: "Imprumuturi",
                column: "ActivityTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Imprumuturi_ActivityTypes_ActivityTypeId",
                table: "Imprumuturi",
                column: "ActivityTypeId",
                principalTable: "ActivityTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Imprumuturi_ActivityTypes_ActivityTypeId",
                table: "Imprumuturi");

            migrationBuilder.DropIndex(
                name: "IX_Imprumuturi_ActivityTypeId",
                table: "Imprumuturi");

            migrationBuilder.DropColumn(
                name: "ActivityTypeId",
                table: "Imprumuturi");
        }
    }
}
