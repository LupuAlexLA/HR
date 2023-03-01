﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class addBVCStatCalcul : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BVC_BugetPrevStatCalcul",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    BugetPrevId = table.Column<int>(nullable: false),
                    ElemCalc = table.Column<int>(nullable: false),
                    StatusCalc = table.Column<bool>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_BugetPrevStatCalcul", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BVC_BugetPrevStatCalcul_BVC_BugetPrev_BugetPrevId",
                        column: x => x.BugetPrevId,
                        principalTable: "BVC_BugetPrev",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BVC_BugetPrevStatCalcul_BugetPrevId",
                table: "BVC_BugetPrevStatCalcul",
                column: "BugetPrevId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BVC_BugetPrevStatCalcul");
        }
    }
}