using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class NomenclatorGaraantie : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CeGaranteaza",
                table: "Garantie");

            migrationBuilder.DropColumn(
                name: "TipGarantie",
                table: "Garantie");

            migrationBuilder.AddColumn<int>(
                name: "GarantieCeGarantezaId",
                table: "Garantie",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GarantieTipId",
                table: "Garantie",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "GarantieCeGaranteza",
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
                    table.PrimaryKey("PK_GarantieCeGaranteza", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GarantieTip",
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
                    table.PrimaryKey("PK_GarantieTip", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Garantie_GarantieCeGarantezaId",
                table: "Garantie",
                column: "GarantieCeGarantezaId");

            migrationBuilder.CreateIndex(
                name: "IX_Garantie_GarantieTipId",
                table: "Garantie",
                column: "GarantieTipId");

            migrationBuilder.AddForeignKey(
                name: "FK_Garantie_GarantieCeGaranteza_GarantieCeGarantezaId",
                table: "Garantie",
                column: "GarantieCeGarantezaId",
                principalTable: "GarantieCeGaranteza",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Garantie_GarantieTip_GarantieTipId",
                table: "Garantie",
                column: "GarantieTipId",
                principalTable: "GarantieTip",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Garantie_GarantieCeGaranteza_GarantieCeGarantezaId",
                table: "Garantie");

            migrationBuilder.DropForeignKey(
                name: "FK_Garantie_GarantieTip_GarantieTipId",
                table: "Garantie");

            migrationBuilder.DropTable(
                name: "GarantieCeGaranteza");

            migrationBuilder.DropTable(
                name: "GarantieTip");

            migrationBuilder.DropIndex(
                name: "IX_Garantie_GarantieCeGarantezaId",
                table: "Garantie");

            migrationBuilder.DropIndex(
                name: "IX_Garantie_GarantieTipId",
                table: "Garantie");

            migrationBuilder.DropColumn(
                name: "GarantieCeGarantezaId",
                table: "Garantie");

            migrationBuilder.DropColumn(
                name: "GarantieTipId",
                table: "Garantie");

            migrationBuilder.AddColumn<string>(
                name: "CeGaranteaza",
                table: "Garantie",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipGarantie",
                table: "Garantie",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
