using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class AddOperGenerate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OperGenerateId",
                table: "Operations",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OperGenerateCateg",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    CategTypeShort = table.Column<string>(nullable: true),
                    CategType = table.Column<string>(nullable: true),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperGenerateCateg", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OperGenerateTipuri",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Tip = table.Column<string>(nullable: true),
                    Descriere = table.Column<string>(nullable: true),
                    SfarsitLuna = table.Column<bool>(nullable: false),
                    CategId = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperGenerateTipuri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperGenerateTipuri_OperGenerateCateg_CategId",
                        column: x => x.CategId,
                        principalTable: "OperGenerateCateg",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OperGenerate",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    DataOperatie = table.Column<DateTime>(nullable: false),
                    TipOperatieId = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperGenerate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperGenerate_OperGenerateTipuri_TipOperatieId",
                        column: x => x.TipOperatieId,
                        principalTable: "OperGenerateTipuri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Operations_OperGenerateId",
                table: "Operations",
                column: "OperGenerateId");

            migrationBuilder.CreateIndex(
                name: "IX_OperGenerate_TipOperatieId",
                table: "OperGenerate",
                column: "TipOperatieId");

            migrationBuilder.CreateIndex(
                name: "IX_OperGenerateTipuri_CategId",
                table: "OperGenerateTipuri",
                column: "CategId");

            migrationBuilder.AddForeignKey(
                name: "FK_Operations_OperGenerate_OperGenerateId",
                table: "Operations",
                column: "OperGenerateId",
                principalTable: "OperGenerate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Operations_OperGenerate_OperGenerateId",
                table: "Operations");

            migrationBuilder.DropTable(
                name: "OperGenerate");

            migrationBuilder.DropTable(
                name: "OperGenerateTipuri");

            migrationBuilder.DropTable(
                name: "OperGenerateCateg");

            migrationBuilder.DropIndex(
                name: "IX_Operations_OperGenerateId",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "OperGenerateId",
                table: "Operations");
        }
    }
}
