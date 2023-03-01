using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class ObiecteDeInventar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "InvStorage");

            migrationBuilder.AddColumn<string>(
                name: "StorageName",
                table: "InvStorage",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "InvStorage",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "InvCateg",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "InvObjectOperDocType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    OperType = table.Column<int>(nullable: false),
                    DocumentTypeId = table.Column<int>(nullable: false),
                    AppOperation = table.Column<bool>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvObjectOperDocType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvObjectOperDocType_DocumentType_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DocumentType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvObjectOperDocType_DocumentTypeId",
                table: "InvObjectOperDocType",
                column: "DocumentTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvObjectOperDocType");

            migrationBuilder.DropColumn(
                name: "StorageName",
                table: "InvStorage");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "InvStorage");

            migrationBuilder.DropColumn(
                name: "State",
                table: "InvCateg");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "InvStorage",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }
    }
}
