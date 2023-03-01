using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class TranzactiiFonduri : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TranzactiiFonduri",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Numar = table.Column<int>(nullable: false),
                    Data = table.Column<DateTime>(nullable: false),
                    Debit = table.Column<decimal>(nullable: false),
                    Credit = table.Column<decimal>(nullable: false),
                    Suma = table.Column<decimal>(nullable: false),
                    Fel_doc = table.Column<string>(nullable: true),
                    Nr_doc = table.Column<string>(nullable: true),
                    Data_doc = table.Column<DateTime>(nullable: false),
                    Explicatie = table.Column<string>(nullable: true),
                    Tip = table.Column<string>(nullable: true),
                    Nota = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TranzactiiFonduri", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TranzactiiFonduri");
        }
    }
}
