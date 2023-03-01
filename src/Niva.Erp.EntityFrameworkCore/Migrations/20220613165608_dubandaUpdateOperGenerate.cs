using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class dubandaUpdateOperGenerate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlataRataDate",
                table: "Dobanda");

            migrationBuilder.DropColumn(
                name: "ValoareDobandaPlata",
                table: "Dobanda");

            migrationBuilder.AddColumn<int>(
                name: "OperGenerateId",
                table: "Dobanda",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Dobanda",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Dobanda_OperGenerateId",
                table: "Dobanda",
                column: "OperGenerateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dobanda_OperGenerate_OperGenerateId",
                table: "Dobanda",
                column: "OperGenerateId",
                principalTable: "OperGenerate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dobanda_OperGenerate_OperGenerateId",
                table: "Dobanda");

            migrationBuilder.DropIndex(
                name: "IX_Dobanda_OperGenerateId",
                table: "Dobanda");

            migrationBuilder.DropColumn(
                name: "OperGenerateId",
                table: "Dobanda");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Dobanda");

            migrationBuilder.AddColumn<DateTime>(
                name: "PlataRataDate",
                table: "Dobanda",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValoareDobandaPlata",
                table: "Dobanda",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
