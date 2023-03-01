﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class UpdateTableInvObjectInventariere : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "InvObjectInventariere",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "InvObjectInventariere");
        }
    }
}
