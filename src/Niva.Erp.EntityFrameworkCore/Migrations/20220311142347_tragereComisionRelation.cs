using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class tragereComisionRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TragereId",
                table: "DateComision",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DateComision_TragereId",
                table: "DateComision",
                column: "TragereId");

            migrationBuilder.AddForeignKey(
                name: "FK_DateComision_Tragere_TragereId",
                table: "DateComision",
                column: "TragereId",
                principalTable: "Tragere",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DateComision_Tragere_TragereId",
                table: "DateComision");

            migrationBuilder.DropIndex(
                name: "IX_DateComision_TragereId",
                table: "DateComision");

            migrationBuilder.DropColumn(
                name: "TragereId",
                table: "DateComision");
        }
    }
}
