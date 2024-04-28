using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class AddNomenclatoare : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Masuratori");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Masuratori");

            migrationBuilder.AddColumn<string>(
                name: "Adresa",
                table: "Masuratori",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "JudetId",
                table: "Masuratori",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Latitudine",
                table: "Masuratori",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Localitate",
                table: "Masuratori",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitudine",
                table: "Masuratori",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "OwnerId",
                table: "Masuratori",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Stare",
                table: "Masuratori",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TaraId",
                table: "Masuratori",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Judete",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    Denumire = table.Column<string>(nullable: true),
                    Abreviere = table.Column<string>(nullable: true),
                    Stare = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Judete", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MasuratoriInterpretari",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    MasuratoareId = table.Column<int>(nullable: false),
                    Expresie1 = table.Column<string>(nullable: true),
                    DescriereRezultat1 = table.Column<string>(nullable: true),
                    Culoare1 = table.Column<int>(nullable: false),
                    Expresie2 = table.Column<string>(nullable: true),
                    DescriereRezultat2 = table.Column<string>(nullable: true),
                    Culoare2 = table.Column<int>(nullable: false),
                    Stare = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasuratoriInterpretari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MasuratoriInterpretari_Masuratori_MasuratoareId",
                        column: x => x.MasuratoareId,
                        principalTable: "Masuratori",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Studii",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    Poza = table.Column<byte[]>(nullable: true),
                    Titlu = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: true),
                    Link = table.Column<string>(nullable: true),
                    Stare = table.Column<int>(nullable: false),
                    OwnerId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Studii", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Studii_AbpUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tari",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    Denumire = table.Column<string>(nullable: true),
                    Abreviere = table.Column<string>(nullable: true),
                    Stare = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tari", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Masuratori_JudetId",
                table: "Masuratori",
                column: "JudetId");

            migrationBuilder.CreateIndex(
                name: "IX_Masuratori_OwnerId",
                table: "Masuratori",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Masuratori_TaraId",
                table: "Masuratori",
                column: "TaraId");

            migrationBuilder.CreateIndex(
                name: "IX_MasuratoriInterpretari_MasuratoareId",
                table: "MasuratoriInterpretari",
                column: "MasuratoareId");

            migrationBuilder.CreateIndex(
                name: "IX_Studii_OwnerId",
                table: "Studii",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Masuratori_Judete_JudetId",
                table: "Masuratori",
                column: "JudetId",
                principalTable: "Judete",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Masuratori_AbpUsers_OwnerId",
                table: "Masuratori",
                column: "OwnerId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Masuratori_Tari_TaraId",
                table: "Masuratori",
                column: "TaraId",
                principalTable: "Tari",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Masuratori_Judete_JudetId",
                table: "Masuratori");

            migrationBuilder.DropForeignKey(
                name: "FK_Masuratori_AbpUsers_OwnerId",
                table: "Masuratori");

            migrationBuilder.DropForeignKey(
                name: "FK_Masuratori_Tari_TaraId",
                table: "Masuratori");

            migrationBuilder.DropTable(
                name: "Judete");

            migrationBuilder.DropTable(
                name: "MasuratoriInterpretari");

            migrationBuilder.DropTable(
                name: "Studii");

            migrationBuilder.DropTable(
                name: "Tari");

            migrationBuilder.DropIndex(
                name: "IX_Masuratori_JudetId",
                table: "Masuratori");

            migrationBuilder.DropIndex(
                name: "IX_Masuratori_OwnerId",
                table: "Masuratori");

            migrationBuilder.DropIndex(
                name: "IX_Masuratori_TaraId",
                table: "Masuratori");

            migrationBuilder.DropColumn(
                name: "Adresa",
                table: "Masuratori");

            migrationBuilder.DropColumn(
                name: "JudetId",
                table: "Masuratori");

            migrationBuilder.DropColumn(
                name: "Latitudine",
                table: "Masuratori");

            migrationBuilder.DropColumn(
                name: "Localitate",
                table: "Masuratori");

            migrationBuilder.DropColumn(
                name: "Longitudine",
                table: "Masuratori");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Masuratori");

            migrationBuilder.DropColumn(
                name: "Stare",
                table: "Masuratori");

            migrationBuilder.DropColumn(
                name: "TaraId",
                table: "Masuratori");

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "Masuratori",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "Masuratori",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
