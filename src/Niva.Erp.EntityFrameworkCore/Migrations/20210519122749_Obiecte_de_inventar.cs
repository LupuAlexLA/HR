using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class Obiecte_de_inventar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InvCateg",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Name = table.Column<string>(maxLength: 1000, nullable: true),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvCateg", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvStorage",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Name = table.Column<string>(maxLength: 1000, nullable: true),
                    CentralStorage = table.Column<bool>(nullable: false),
                    State = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvStorage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvObjectItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Name = table.Column<string>(maxLength: 1000, nullable: true),
                    OperationDate = table.Column<DateTime>(nullable: false),
                    DocumentNr = table.Column<int>(nullable: false),
                    DocumentDate = table.Column<DateTime>(nullable: false),
                    DocumentTypeId = table.Column<int>(nullable: false),
                    PrimDocumentTypeId = table.Column<int>(nullable: true),
                    PrimDocumentNr = table.Column<string>(maxLength: 1000, nullable: true),
                    PrimDocumentDate = table.Column<DateTime>(nullable: true),
                    ThirdPartyId = table.Column<int>(nullable: true),
                    PriceUnit = table.Column<decimal>(nullable: false),
                    InventoryNr = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    InventoryValue = table.Column<decimal>(nullable: false),
                    InvObjectStorageId = table.Column<int>(nullable: true),
                    InvCategId = table.Column<int>(nullable: false),
                    AssetAccountId = table.Column<int>(nullable: true),
                    ExpenseAccountId = table.Column<int>(nullable: true),
                    State = table.Column<int>(nullable: false),
                    Processed = table.Column<bool>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvObjectItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvObjectItem_Account_AssetAccountId",
                        column: x => x.AssetAccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvObjectItem_DocumentType_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DocumentType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvObjectItem_Account_ExpenseAccountId",
                        column: x => x.ExpenseAccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvObjectItem_InvCateg_InvCategId",
                        column: x => x.InvCategId,
                        principalTable: "InvCateg",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvObjectItem_InvStorage_InvObjectStorageId",
                        column: x => x.InvObjectStorageId,
                        principalTable: "InvStorage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvObjectItem_DocumentType_PrimDocumentTypeId",
                        column: x => x.PrimDocumentTypeId,
                        principalTable: "DocumentType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvObjectItem_Persons_ThirdPartyId",
                        column: x => x.ThirdPartyId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvObjectItem_AssetAccountId",
                table: "InvObjectItem",
                column: "AssetAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_InvObjectItem_DocumentTypeId",
                table: "InvObjectItem",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_InvObjectItem_ExpenseAccountId",
                table: "InvObjectItem",
                column: "ExpenseAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_InvObjectItem_InvCategId",
                table: "InvObjectItem",
                column: "InvCategId");

            migrationBuilder.CreateIndex(
                name: "IX_InvObjectItem_InvObjectStorageId",
                table: "InvObjectItem",
                column: "InvObjectStorageId");

            migrationBuilder.CreateIndex(
                name: "IX_InvObjectItem_PrimDocumentTypeId",
                table: "InvObjectItem",
                column: "PrimDocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_InvObjectItem_ThirdPartyId",
                table: "InvObjectItem",
                column: "ThirdPartyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvObjectItem");

            migrationBuilder.DropTable(
                name: "InvCateg");

            migrationBuilder.DropTable(
                name: "InvStorage");
        }
    }
}
