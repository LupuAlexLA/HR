using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class prepaymentMigr : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImoAssetOperDetail_ImoAssetOper_ImoAssetOperId1",
                table: "ImoAssetOperDetail");

            migrationBuilder.DropIndex(
                name: "IX_ImoAssetOperDetail_ImoAssetOperId1",
                table: "ImoAssetOperDetail");

            migrationBuilder.DropColumn(
                name: "ImoAssetOperId1",
                table: "ImoAssetOperDetail");

            migrationBuilder.CreateTable(
                name: "PrepaymentDocType",
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
                    table.PrimaryKey("PK_PrepaymentDocType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrepaymentDocType_DocumentType_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DocumentType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrepaymentsDecDeprecSetup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    PrepaymentType = table.Column<int>(nullable: false),
                    DecimalAmort = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrepaymentsDecDeprecSetup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PrepaymentsDurationSetup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    PrepaymentType = table.Column<int>(nullable: false),
                    PrepaymentDurationCalc = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrepaymentsDurationSetup", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrepaymentDocType_DocumentTypeId",
                table: "PrepaymentDocType",
                column: "DocumentTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrepaymentDocType");

            migrationBuilder.DropTable(
                name: "PrepaymentsDecDeprecSetup");

            migrationBuilder.DropTable(
                name: "PrepaymentsDurationSetup");

            migrationBuilder.AddColumn<int>(
                name: "ImoAssetOperId1",
                table: "ImoAssetOperDetail",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetOperDetail_ImoAssetOperId1",
                table: "ImoAssetOperDetail",
                column: "ImoAssetOperId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ImoAssetOperDetail_ImoAssetOper_ImoAssetOperId1",
                table: "ImoAssetOperDetail",
                column: "ImoAssetOperId1",
                principalTable: "ImoAssetOper",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
