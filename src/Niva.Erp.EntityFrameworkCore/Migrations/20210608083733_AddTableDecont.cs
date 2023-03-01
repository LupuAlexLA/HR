using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class AddTableDecont : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DecontId",
                table: "Invoices",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Decont",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    DecontDate = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    DecontNumber = table.Column<int>(nullable: false),
                    DateStart = table.Column<DateTime>(nullable: true),
                    DateEnd = table.Column<DateTime>(nullable: true),
                    Diurna = table.Column<int>(nullable: true),
                    DecontType = table.Column<int>(nullable: false),
                    ThirdPartyId = table.Column<int>(nullable: true),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Decont", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Decont_Persons_ThirdPartyId",
                        column: x => x.ThirdPartyId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_DecontId",
                table: "Invoices",
                column: "DecontId");

            migrationBuilder.CreateIndex(
                name: "IX_Decont_ThirdPartyId",
                table: "Decont",
                column: "ThirdPartyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Decont_DecontId",
                table: "Invoices",
                column: "DecontId",
                principalTable: "Decont",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Decont_DecontId",
                table: "Invoices");

            migrationBuilder.DropTable(
                name: "Decont");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_DecontId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "DecontId",
                table: "Invoices");
        }
    }
}
