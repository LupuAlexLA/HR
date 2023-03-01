using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class add_table_BVC_VenitTitluParams : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BVC_VenitTitluParams",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    FormularId = table.Column<int>(nullable: false),
                    MonthStart = table.Column<int>(nullable: false),
                    MonthEnd = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_VenitTitluParams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BVC_VenitTitluParams_BVC_Formular_FormularId",
                        column: x => x.FormularId,
                        principalTable: "BVC_Formular",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BVC_VenitTitluParams_FormularId",
                table: "BVC_VenitTitluParams",
                column: "FormularId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BVC_VenitTitluParams");
        }
    }
}
