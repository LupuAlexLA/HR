using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class paapFrecventaData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContractsPaymentInstalmentFreq",
                table: "BVC_PAAP",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "FirstInstalmentDate",
                table: "BVC_PAAP",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContractsPaymentInstalmentFreq",
                table: "BVC_PAAP");

            migrationBuilder.DropColumn(
                name: "FirstInstalmentDate",
                table: "BVC_PAAP");
        }
    }
}
