using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class add_table_BVC_PrevResurse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BVC_PrevResurse",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Descriere = table.Column<string>(nullable: true),
                    OrderView = table.Column<int>(nullable: false),
                    Suma = table.Column<decimal>(nullable: false),
                    ActivityTypeId = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_PrevResurse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BVC_PrevResurse_ActivityTypes_ActivityTypeId",
                        column: x => x.ActivityTypeId,
                        principalTable: "ActivityTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BVC_PrevResurseDetalii",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IdPlasament = table.Column<int>(nullable: false),
                    DataStart = table.Column<DateTime>(nullable: false),
                    DataEnd = table.Column<DateTime>(nullable: false),
                    ProcentDobanda = table.Column<decimal>(nullable: false),
                    NrZilePlasament = table.Column<int>(nullable: false),
                    SumaInvestita = table.Column<decimal>(nullable: false),
                    BVC_PrevResurseId = table.Column<int>(nullable: false),
                    CurrencyId = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_PrevResurseDetalii", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BVC_PrevResurseDetalii_BVC_PrevResurse_BVC_PrevResurseId",
                        column: x => x.BVC_PrevResurseId,
                        principalTable: "BVC_PrevResurse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BVC_PrevResurseDetalii_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BVC_PrevResurse_ActivityTypeId",
                table: "BVC_PrevResurse",
                column: "ActivityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_PrevResurseDetalii_BVC_PrevResurseId",
                table: "BVC_PrevResurseDetalii",
                column: "BVC_PrevResurseId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_PrevResurseDetalii_CurrencyId",
                table: "BVC_PrevResurseDetalii",
                column: "CurrencyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BVC_PrevResurseDetalii");

            migrationBuilder.DropTable(
                name: "BVC_PrevResurse");
        }
    }
}
