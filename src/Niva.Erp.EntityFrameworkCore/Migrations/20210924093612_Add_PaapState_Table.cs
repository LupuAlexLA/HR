using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class Add_PaapState_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaapState",
                table: "BVC_PAAP");

            migrationBuilder.AlterColumn<int>(
                name: "ObiectTranzactie",
                table: "BVC_PAAP",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssetClassCodesId",
                table: "BVC_PAAP",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DepartamentId",
                table: "BVC_PAAP",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Descriere",
                table: "BVC_PAAP",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InvoiceElementsDetailsCategoryId",
                table: "BVC_PAAP",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BVC_PAAP_AvailableSum",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    InvoiceElementsDetailsId = table.Column<int>(nullable: false),
                    SumApproved = table.Column<decimal>(nullable: false),
                    SumAllocated = table.Column<decimal>(nullable: false),
                    Rest = table.Column<decimal>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_PAAP_AvailableSum", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BVC_PAAP_AvailableSum_InvoiceElementsDetails_InvoiceElementsDetailsId",
                        column: x => x.InvoiceElementsDetailsId,
                        principalTable: "InvoiceElementsDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BVC_PAAP_State",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    BVC_PAAP_Id = table.Column<int>(nullable: false),
                    OperationDate = table.Column<DateTime>(nullable: false),
                    Paap_State = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_PAAP_State", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BVC_PAAP_State_BVC_PAAP_BVC_PAAP_Id",
                        column: x => x.BVC_PAAP_Id,
                        principalTable: "BVC_PAAP",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Departament",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    State = table.Column<int>(nullable: false),
                    IdExtern = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departament", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BVC_PAAP_AssetClassCodesId",
                table: "BVC_PAAP",
                column: "AssetClassCodesId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_PAAP_DepartamentId",
                table: "BVC_PAAP",
                column: "DepartamentId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_PAAP_InvoiceElementsDetailsCategoryId",
                table: "BVC_PAAP",
                column: "InvoiceElementsDetailsCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_PAAP_AvailableSum_InvoiceElementsDetailsId",
                table: "BVC_PAAP_AvailableSum",
                column: "InvoiceElementsDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_PAAP_State_BVC_PAAP_Id",
                table: "BVC_PAAP_State",
                column: "BVC_PAAP_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_PAAP_ImoAssetClassCode_AssetClassCodesId",
                table: "BVC_PAAP",
                column: "AssetClassCodesId",
                principalTable: "ImoAssetClassCode",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_PAAP_Departament_DepartamentId",
                table: "BVC_PAAP",
                column: "DepartamentId",
                principalTable: "Departament",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_PAAP_InvoiceElementsDetailsCategory_InvoiceElementsDetailsCategoryId",
                table: "BVC_PAAP",
                column: "InvoiceElementsDetailsCategoryId",
                principalTable: "InvoiceElementsDetailsCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_PAAP_ImoAssetClassCode_AssetClassCodesId",
                table: "BVC_PAAP");

            migrationBuilder.DropForeignKey(
                name: "FK_BVC_PAAP_Departament_DepartamentId",
                table: "BVC_PAAP");

            migrationBuilder.DropForeignKey(
                name: "FK_BVC_PAAP_InvoiceElementsDetailsCategory_InvoiceElementsDetailsCategoryId",
                table: "BVC_PAAP");

            migrationBuilder.DropTable(
                name: "BVC_PAAP_AvailableSum");

            migrationBuilder.DropTable(
                name: "BVC_PAAP_State");

            migrationBuilder.DropTable(
                name: "Departament");

            migrationBuilder.DropIndex(
                name: "IX_BVC_PAAP_AssetClassCodesId",
                table: "BVC_PAAP");

            migrationBuilder.DropIndex(
                name: "IX_BVC_PAAP_DepartamentId",
                table: "BVC_PAAP");

            migrationBuilder.DropIndex(
                name: "IX_BVC_PAAP_InvoiceElementsDetailsCategoryId",
                table: "BVC_PAAP");

            migrationBuilder.DropColumn(
                name: "AssetClassCodesId",
                table: "BVC_PAAP");

            migrationBuilder.DropColumn(
                name: "DepartamentId",
                table: "BVC_PAAP");

            migrationBuilder.DropColumn(
                name: "Descriere",
                table: "BVC_PAAP");

            migrationBuilder.DropColumn(
                name: "InvoiceElementsDetailsCategoryId",
                table: "BVC_PAAP");

            migrationBuilder.AlterColumn<string>(
                name: "ObiectTranzactie",
                table: "BVC_PAAP",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "PaapState",
                table: "BVC_PAAP",
                type: "int",
                nullable: true);
        }
    }
}
