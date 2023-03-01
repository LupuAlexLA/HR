using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class PartenerGarantiePersons : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PartenerGarantie",
                table: "Garantie");

            migrationBuilder.AddColumn<int>(
                name: "PersonId",
                table: "Garantie",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Garantie_PersonId",
                table: "Garantie",
                column: "PersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Garantie_Persons_PersonId",
                table: "Garantie",
                column: "PersonId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Garantie_Persons_PersonId",
                table: "Garantie");

            migrationBuilder.DropIndex(
                name: "IX_Garantie_PersonId",
                table: "Garantie");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "Garantie");

            migrationBuilder.AddColumn<string>(
                name: "PartenerGarantie",
                table: "Garantie",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
