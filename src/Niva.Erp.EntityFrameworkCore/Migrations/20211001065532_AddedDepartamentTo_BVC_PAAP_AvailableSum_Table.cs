using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class AddedDepartamentTo_BVC_PAAP_AvailableSum_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepartamentId",
                table: "BVC_PAAP_AvailableSum",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BVC_PAAP_AvailableSum_DepartamentId",
                table: "BVC_PAAP_AvailableSum",
                column: "DepartamentId");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_PAAP_AvailableSum_Departament_DepartamentId",
                table: "BVC_PAAP_AvailableSum",
                column: "DepartamentId",
                principalTable: "Departament",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_PAAP_AvailableSum_Departament_DepartamentId",
                table: "BVC_PAAP_AvailableSum");

            migrationBuilder.DropIndex(
                name: "IX_BVC_PAAP_AvailableSum_DepartamentId",
                table: "BVC_PAAP_AvailableSum");

            migrationBuilder.DropColumn(
                name: "DepartamentId",
                table: "BVC_PAAP_AvailableSum");
        }
    }
}
