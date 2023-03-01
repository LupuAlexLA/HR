using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class add_table_Notificare : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notificare",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    DepartamentId = table.Column<int>(nullable: true),
                    SalariatId = table.Column<int>(nullable: true),
                    StareNotificare = table.Column<bool>(nullable: false),
                    Mesaj = table.Column<string>(nullable: true),
                    UserVizualizare = table.Column<int>(nullable: false),
                    DataVizualizare = table.Column<DateTime>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificare", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notificare_Departament_DepartamentId",
                        column: x => x.DepartamentId,
                        principalTable: "Departament",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notificare_DepartamentId",
                table: "Notificare",
                column: "DepartamentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notificare");
        }
    }
}
