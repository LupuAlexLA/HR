using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class FileDocErr : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileDocErrors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(nullable: false),
                    DocumentId = table.Column<int>(nullable: false),
                    DocumentNr = table.Column<string>(nullable: true),
                    MesajEroare = table.Column<string>(nullable: true),
                    Rezolvat = table.Column<bool>(nullable: false),
                    LastErrorDate = table.Column<DateTime>(nullable: false),
                    RezolvatDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileDocErrors", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileDocErrors");
        }
    }
}
