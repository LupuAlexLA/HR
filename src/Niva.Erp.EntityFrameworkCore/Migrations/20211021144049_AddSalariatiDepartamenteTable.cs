using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class AddSalariatiDepartamenteTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SalariatiDepartamente",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    PersonId = table.Column<int>(nullable: false),
                    DepartamentId = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalariatiDepartamente", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalariatiDepartamente_Departament_DepartamentId",
                        column: x => x.DepartamentId,
                        principalTable: "Departament",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SalariatiDepartamente_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalariatiDepartamente_DepartamentId",
                table: "SalariatiDepartamente",
                column: "DepartamentId");

            migrationBuilder.CreateIndex(
                name: "IX_SalariatiDepartamente_PersonId",
                table: "SalariatiDepartamente",
                column: "PersonId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalariatiDepartamente");
        }
    }
}
