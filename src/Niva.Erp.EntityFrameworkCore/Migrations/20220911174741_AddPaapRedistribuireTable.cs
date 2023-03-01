using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class AddPaapRedistribuireTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BVC_PaapRedistribuire",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    PaapCarePrimesteId = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false),
                    SumaPlatita = table.Column<double>(nullable: false),
                    DataRedistribuire = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_PaapRedistribuire", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BVC_PaapRedistribuireDetalii",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    PaapCarePierdeId = table.Column<int>(nullable: false),
                    SumaPierduta = table.Column<double>(nullable: false),
                    BVC_PaapRedistribuireId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_PaapRedistribuireDetalii", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BVC_PaapRedistribuireDetalii_BVC_PaapRedistribuire_BVC_PaapRedistribuireId",
                        column: x => x.BVC_PaapRedistribuireId,
                        principalTable: "BVC_PaapRedistribuire",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BVC_PaapRedistribuireDetalii_BVC_PaapRedistribuireId",
                table: "BVC_PaapRedistribuireDetalii",
                column: "BVC_PaapRedistribuireId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BVC_PaapRedistribuireDetalii");

            migrationBuilder.DropTable(
                name: "BVC_PaapRedistribuire");
        }
    }
}
