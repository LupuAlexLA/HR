using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class DobRoborV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DobanziReferinta",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Dobanda = table.Column<string>(nullable: true),
                    Descriere = table.Column<string>(nullable: true),
                    PerioadaCalcul = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DobanziReferinta", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DateDobanziReferinta",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<DateTime>(nullable: false),
                    Dobanda = table.Column<string>(nullable: true),
                    Valoare = table.Column<decimal>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false),
                    DobanziReferintaId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DateDobanziReferinta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DateDobanziReferinta_DobanziReferinta_DobanziReferintaId",
                        column: x => x.DobanziReferintaId,
                        principalTable: "DobanziReferinta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DateDobanziReferinta_DobanziReferintaId",
                table: "DateDobanziReferinta",
                column: "DobanziReferintaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DateDobanziReferinta");

            migrationBuilder.DropTable(
                name: "DobanziReferinta");
        }
    }
}
