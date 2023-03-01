using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class DobRobor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MarjaFixa",
                table: "Imprumuturi",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TipDobanda",
                table: "Imprumuturi",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ExceptiiEliminareRegInventarConfig",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    AccountId = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExceptiiEliminareRegInventarConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExceptiiEliminareRegInventarConfig_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExceptiiRegInventarConfig",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    AccountId = table.Column<int>(nullable: false),
                    Formula = table.Column<string>(maxLength: 1000, nullable: true),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExceptiiRegInventarConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExceptiiRegInventarConfig_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExceptiiEliminareRegInventarConfig_AccountId",
                table: "ExceptiiEliminareRegInventarConfig",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ExceptiiRegInventarConfig_AccountId",
                table: "ExceptiiRegInventarConfig",
                column: "AccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExceptiiEliminareRegInventarConfig");

            migrationBuilder.DropTable(
                name: "ExceptiiRegInventarConfig");

            migrationBuilder.DropColumn(
                name: "MarjaFixa",
                table: "Imprumuturi");

            migrationBuilder.DropColumn(
                name: "TipDobanda",
                table: "Imprumuturi");
        }
    }
}
