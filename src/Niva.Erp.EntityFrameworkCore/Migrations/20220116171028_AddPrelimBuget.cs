using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class AddPrelimBuget : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "VenitRepartizBVC",
                table: "BVC_VenitProcRepartiz",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "BVC_BugetPrevId",
                table: "BVC_VenitBugetPrelim",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BVC_VenitBugetPrelim_BVC_BugetPrevId",
                table: "BVC_VenitBugetPrelim",
                column: "BVC_BugetPrevId");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_VenitBugetPrelim_BVC_BugetPrev_BVC_BugetPrevId",
                table: "BVC_VenitBugetPrelim",
                column: "BVC_BugetPrevId",
                principalTable: "BVC_BugetPrev",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_VenitBugetPrelim_BVC_BugetPrev_BVC_BugetPrevId",
                table: "BVC_VenitBugetPrelim");

            migrationBuilder.DropIndex(
                name: "IX_BVC_VenitBugetPrelim_BVC_BugetPrevId",
                table: "BVC_VenitBugetPrelim");

            migrationBuilder.DropColumn(
                name: "VenitRepartizBVC",
                table: "BVC_VenitProcRepartiz");

            migrationBuilder.DropColumn(
                name: "BVC_BugetPrevId",
                table: "BVC_VenitBugetPrelim");
        }
    }
}
