using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class corectiedobanda : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DobandaEveniment");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DobandaEveniment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DobandaId = table.Column<int>(type: "int", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    ValoareDobandaDatorata = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValoarePrincipalDatorat = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DobandaEveniment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DobandaEveniment_Dobanda_DobandaId",
                        column: x => x.DobandaId,
                        principalTable: "Dobanda",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DobandaEveniment_DobandaId",
                table: "DobandaEveniment",
                column: "DobandaId",
                unique: true,
                filter: "[DobandaId] IS NOT NULL");
        }
    }
}
