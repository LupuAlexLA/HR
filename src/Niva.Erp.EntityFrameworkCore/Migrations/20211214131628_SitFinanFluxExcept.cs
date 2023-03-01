using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class SitFinanFluxExcept : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SitFinanRowModCalc",
                table: "SitFinanRapConfigCorel");

            migrationBuilder.AddColumn<int>(
                name: "SitFinanRowModCalc",
                table: "SitFinanRapConfig",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SitFinanRapFluxConfig",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    SitFinanRapId = table.Column<int>(nullable: false),
                    Debit = table.Column<string>(nullable: true),
                    Credit = table.Column<string>(nullable: true),
                    SitFinanFluxRowType = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SitFinanRapFluxConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SitFinanRapFluxConfig_SitFinanRap_SitFinanRapId",
                        column: x => x.SitFinanRapId,
                        principalTable: "SitFinanRap",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SitFinanRapFluxConfig_SitFinanRapId",
                table: "SitFinanRapFluxConfig",
                column: "SitFinanRapId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SitFinanRapFluxConfig");

            migrationBuilder.DropColumn(
                name: "SitFinanRowModCalc",
                table: "SitFinanRapConfig");

            migrationBuilder.AddColumn<int>(
                name: "SitFinanRowModCalc",
                table: "SitFinanRapConfigCorel",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
