using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class AddTAble_BVC_PAAP_Referat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BVC_PAAP_Referat",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    BVC_PAAP_Id = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    OperationDate = table.Column<DateTime>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_PAAP_Referat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BVC_PAAP_Referat_BVC_PAAP_BVC_PAAP_Id",
                        column: x => x.BVC_PAAP_Id,
                        principalTable: "BVC_PAAP",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BVC_PAAP_Referat_BVC_PAAP_Id",
                table: "BVC_PAAP_Referat",
                column: "BVC_PAAP_Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BVC_PAAP_Referat");
        }
    }
}
