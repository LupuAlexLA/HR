using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class GarantieCeGaranteazaTypo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Garantie_GarantieCeGaranteza_GarantieCeGarantezaId",
                table: "Garantie");

            migrationBuilder.DropTable(
                name: "GarantieCeGaranteza");

            migrationBuilder.DropIndex(
                name: "IX_Garantie_GarantieCeGarantezaId",
                table: "Garantie");

            migrationBuilder.DropColumn(
                name: "GarantieCeGarantezaId",
                table: "Garantie");

            migrationBuilder.AddColumn<int>(
                name: "GarantieCeGaranteazaId",
                table: "Garantie",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "GarantieCeGaranteaza",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(nullable: true),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarantieCeGaranteaza", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Garantie_GarantieCeGaranteazaId",
                table: "Garantie",
                column: "GarantieCeGaranteazaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Garantie_GarantieCeGaranteaza_GarantieCeGaranteazaId",
                table: "Garantie",
                column: "GarantieCeGaranteazaId",
                principalTable: "GarantieCeGaranteaza",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Garantie_GarantieCeGaranteaza_GarantieCeGaranteazaId",
                table: "Garantie");

            migrationBuilder.DropTable(
                name: "GarantieCeGaranteaza");

            migrationBuilder.DropIndex(
                name: "IX_Garantie_GarantieCeGaranteazaId",
                table: "Garantie");

            migrationBuilder.DropColumn(
                name: "GarantieCeGaranteazaId",
                table: "Garantie");

            migrationBuilder.AddColumn<int>(
                name: "GarantieCeGarantezaId",
                table: "Garantie",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "GarantieCeGaranteza",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarantieCeGaranteza", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Garantie_GarantieCeGarantezaId",
                table: "Garantie",
                column: "GarantieCeGarantezaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Garantie_GarantieCeGaranteza_GarantieCeGarantezaId",
                table: "Garantie",
                column: "GarantieCeGarantezaId",
                principalTable: "GarantieCeGaranteza",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
