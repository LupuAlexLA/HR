using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class MigSumeReinvestite : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BVC_VenitTitluCFReinv",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    BVC_VenitTitluCFId = table.Column<int>(nullable: false),
                    DobandaTotala = table.Column<decimal>(nullable: false),
                    DataReinvestire = table.Column<DateTime>(nullable: false),
                    SumaReinvestita = table.Column<decimal>(nullable: false),
                    ProcDobanda = table.Column<decimal>(nullable: false),
                    MainValue = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_VenitTitluCFReinv", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BVC_VenitTitluCFReinv_BVC_VenitTitluCF_BVC_VenitTitluCFId",
                        column: x => x.BVC_VenitTitluCFId,
                        principalTable: "BVC_VenitTitluCF",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BVC_VenitTitluCFReinv_BVC_VenitTitluCFId",
                table: "BVC_VenitTitluCFReinv",
                column: "BVC_VenitTitluCFId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BVC_VenitTitluCFReinv");
        }
    }
}
