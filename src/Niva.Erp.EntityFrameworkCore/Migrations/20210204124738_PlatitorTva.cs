using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class PlatitorTva : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVATPayer",
                table: "Persons",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDateVATPayment",
                table: "Persons",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVATPayer",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "StartDateVATPayment",
                table: "Persons");
        }
    }
}
