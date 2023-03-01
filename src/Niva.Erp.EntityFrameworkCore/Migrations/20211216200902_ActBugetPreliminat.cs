using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class ActBugetPreliminat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_VenitProcRepartiz_BVC_Formular_FormularId",
                table: "BVC_VenitProcRepartiz");

            migrationBuilder.DropIndex(
                name: "IX_BVC_VenitProcRepartiz_FormularId",
                table: "BVC_VenitProcRepartiz");

            migrationBuilder.DropColumn(
                name: "FormularId",
                table: "BVC_VenitProcRepartiz");

            migrationBuilder.AddColumn<int>(
                name: "BVC_VenitBugetPrelimId",
                table: "BVC_VenitProcRepartiz",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BVC_VenitProcRepartiz_BVC_VenitBugetPrelimId",
                table: "BVC_VenitProcRepartiz",
                column: "BVC_VenitBugetPrelimId");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_VenitProcRepartiz_BVC_VenitBugetPrelim_BVC_VenitBugetPrelimId",
                table: "BVC_VenitProcRepartiz",
                column: "BVC_VenitBugetPrelimId",
                principalTable: "BVC_VenitBugetPrelim",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_VenitProcRepartiz_BVC_VenitBugetPrelim_BVC_VenitBugetPrelimId",
                table: "BVC_VenitProcRepartiz");

            migrationBuilder.DropIndex(
                name: "IX_BVC_VenitProcRepartiz_BVC_VenitBugetPrelimId",
                table: "BVC_VenitProcRepartiz");

            migrationBuilder.DropColumn(
                name: "BVC_VenitBugetPrelimId",
                table: "BVC_VenitProcRepartiz");

            migrationBuilder.AddColumn<int>(
                name: "FormularId",
                table: "BVC_VenitProcRepartiz",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BVC_VenitProcRepartiz_FormularId",
                table: "BVC_VenitProcRepartiz",
                column: "FormularId");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_VenitProcRepartiz_BVC_Formular_FormularId",
                table: "BVC_VenitProcRepartiz",
                column: "FormularId",
                principalTable: "BVC_Formular",
                principalColumn: "Id");
        }
    }
}
