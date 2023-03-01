using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class comisioane : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comisioane",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImprumutId = table.Column<int>(nullable: true),
                    TipComision = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    TipValoareComision = table.Column<int>(nullable: false),
                    ValoareComision = table.Column<decimal>(nullable: false),
                    ModCalculComision = table.Column<int>(nullable: false),
                    TipSumaComision = table.Column<int>(nullable: false),
                    BazaDeCalcul = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comisioane", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comisioane_Imprumuturi_ImprumutId",
                        column: x => x.ImprumutId,
                        principalTable: "Imprumuturi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comisioane_ImprumutId",
                table: "Comisioane",
                column: "ImprumutId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comisioane");
        }
    }
}
