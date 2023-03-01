using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class tokens : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dispositions_Operations_OperationId",
                table: "Dispositions");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationsDetails_Operations_OperationId",
                table: "OperationsDetails");

            migrationBuilder.CreateTable(
                name: "Tokens",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Token = table.Column<string>(nullable: true),
                    UserId = table.Column<long>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tokens_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_UserId",
                table: "Tokens",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dispositions_Operations_OperationId",
                table: "Dispositions",
                column: "OperationId",
                principalTable: "Operations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationsDetails_Operations_OperationId",
                table: "OperationsDetails",
                column: "OperationId",
                principalTable: "Operations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dispositions_Operations_OperationId",
                table: "Dispositions");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationsDetails_Operations_OperationId",
                table: "OperationsDetails");

            migrationBuilder.DropTable(
                name: "Tokens");

            migrationBuilder.AddForeignKey(
                name: "FK_Dispositions_Operations_OperationId",
                table: "Dispositions",
                column: "OperationId",
                principalTable: "Operations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OperationsDetails_Operations_OperationId",
                table: "OperationsDetails",
                column: "OperationId",
                principalTable: "Operations",
                principalColumn: "Id");
        }
    }
}
