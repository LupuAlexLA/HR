using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class VenitRepartiz : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BVC_VenitProcRepartiz",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    FormularId = table.Column<int>(nullable: false),
                    ActivityTypeId = table.Column<int>(nullable: false),
                    ProcRepartiz = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_VenitProcRepartiz", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BVC_VenitProcRepartiz_ActivityTypes_ActivityTypeId",
                        column: x => x.ActivityTypeId,
                        principalTable: "ActivityTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BVC_VenitProcRepartiz_BVC_Formular_FormularId",
                        column: x => x.FormularId,
                        principalTable: "BVC_Formular",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BVC_VenitProcRepartiz_ActivityTypeId",
                table: "BVC_VenitProcRepartiz",
                column: "ActivityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_VenitProcRepartiz_FormularId",
                table: "BVC_VenitProcRepartiz",
                column: "FormularId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BVC_VenitProcRepartiz");
        }
    }
}
