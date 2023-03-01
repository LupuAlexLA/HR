using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class TvaLaIncasare : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVATCollector",
                table: "Persons",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VATCollectedStartDate",
                table: "Persons",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVATCollector",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "VATCollectedStartDate",
                table: "Persons");
        }
    }
}
