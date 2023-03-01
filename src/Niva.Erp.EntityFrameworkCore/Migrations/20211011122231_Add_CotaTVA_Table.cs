using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class Add_CotaTVA_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CotaTVA_Id",
                table: "BVC_PAAP",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CotaTVA",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    VAT = table.Column<decimal>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CotaTVA", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BVC_PAAP_CotaTVA_Id",
                table: "BVC_PAAP",
                column: "CotaTVA_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_PAAP_CotaTVA_CotaTVA_Id",
                table: "BVC_PAAP",
                column: "CotaTVA_Id",
                principalTable: "CotaTVA",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_PAAP_CotaTVA_CotaTVA_Id",
                table: "BVC_PAAP");

            migrationBuilder.DropTable(
                name: "CotaTVA");

            migrationBuilder.DropIndex(
                name: "IX_BVC_PAAP_CotaTVA_Id",
                table: "BVC_PAAP");

            migrationBuilder.DropColumn(
                name: "CotaTVA_Id",
                table: "BVC_PAAP");
        }
    }
}
