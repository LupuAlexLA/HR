﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class Buget_CheltuieliDescriere : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Descriere",
                table: "BVC_Cheltuieli",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Descriere",
                table: "BVC_Cheltuieli");
        }
    }
}