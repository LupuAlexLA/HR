using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class updateBVC_PAAP_State : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CotaTVA_Id",
                table: "BVC_PAAP_State",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValoareEstimataFaraTvaLei",
                table: "BVC_PAAP_State",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValoareTotalaLei",
                table: "BVC_PAAP_State",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValoareTotalaValuta",
                table: "BVC_PAAP_State",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_BVC_PAAP_State_CotaTVA_Id",
                table: "BVC_PAAP_State",
                column: "CotaTVA_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_PAAP_State_CotaTVA_CotaTVA_Id",
                table: "BVC_PAAP_State",
                column: "CotaTVA_Id",
                principalTable: "CotaTVA",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_PAAP_State_CotaTVA_CotaTVA_Id",
                table: "BVC_PAAP_State");

            migrationBuilder.DropIndex(
                name: "IX_BVC_PAAP_State_CotaTVA_Id",
                table: "BVC_PAAP_State");

            migrationBuilder.DropColumn(
                name: "CotaTVA_Id",
                table: "BVC_PAAP_State");

            migrationBuilder.DropColumn(
                name: "ValoareEstimataFaraTvaLei",
                table: "BVC_PAAP_State");

            migrationBuilder.DropColumn(
                name: "ValoareTotalaLei",
                table: "BVC_PAAP_State");

            migrationBuilder.DropColumn(
                name: "ValoareTotalaValuta",
                table: "BVC_PAAP_State");
        }
    }
}
