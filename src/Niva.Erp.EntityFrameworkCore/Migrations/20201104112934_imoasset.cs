using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class imoasset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_ContractsCategories_ContractsCategoryId",
                table: "Contracts");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_ContractsCategoryId",
                table: "Contracts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContractsCategories",
                table: "ContractsCategories");

            migrationBuilder.RenameTable(
                name: "ContractsCategories",
                newName: "ContractsCategory");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContractsCategory",
                table: "ContractsCategory",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ImoAssetClassCode",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Name = table.Column<string>(maxLength: 1000, nullable: true),
                    Code = table.Column<string>(maxLength: 1000, nullable: true),
                    AssetAccountId = table.Column<int>(nullable: true),
                    DepreciationAccountId = table.Column<int>(nullable: true),
                    ExpenseAccountId = table.Column<int>(nullable: true),
                    ClassCodeParrentId = table.Column<int>(nullable: true),
                    DurationMin = table.Column<int>(nullable: false),
                    DurationMax = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImoAssetClassCode", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImoAssetClassCode_Account_AssetAccountId",
                        column: x => x.AssetAccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImoAssetClassCode_ImoAssetClassCode_ClassCodeParrentId",
                        column: x => x.ClassCodeParrentId,
                        principalTable: "ImoAssetClassCode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImoAssetClassCode_Account_DepreciationAccountId",
                        column: x => x.DepreciationAccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImoAssetClassCode_Account_ExpenseAccountId",
                        column: x => x.ExpenseAccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ImoAssetOperDocType",
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
                    table.PrimaryKey("PK_ImoAssetOperDocType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImoAssetOperDocType_DocumentType_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DocumentType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImoAssetOperForType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    ImoAssetType = table.Column<int>(nullable: false),
                    ImoAssetOperType = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImoAssetOperForType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImoAssetSetup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    ReserveDepreciation = table.Column<bool>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImoAssetSetup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImoAssetStorage",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    StorageName = table.Column<string>(maxLength: 1000, nullable: true),
                    CentralStorage = table.Column<bool>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImoAssetStorage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImoAssetItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Name = table.Column<string>(maxLength: 1000, nullable: true),
                    ImoAssetType = table.Column<int>(nullable: false),
                    PriceUnit = table.Column<decimal>(nullable: false),
                    InventoryNr = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    InventoryValue = table.Column<decimal>(nullable: false),
                    FiscalInventoryValue = table.Column<decimal>(nullable: false),
                    UseStartDate = table.Column<DateTime>(nullable: false),
                    DepreciationStartDate = table.Column<DateTime>(nullable: false),
                    DurationInMonths = table.Column<int>(nullable: false),
                    RemainingDuration = table.Column<int>(nullable: true),
                    Depreciation = table.Column<decimal>(nullable: true),
                    FiscalDepreciation = table.Column<decimal>(nullable: true),
                    MonthlyDepreciation = table.Column<decimal>(nullable: true),
                    MonthlyFiscalDepreciation = table.Column<decimal>(nullable: true),
                    InConservare = table.Column<bool>(nullable: false),
                    InStock = table.Column<bool>(nullable: false),
                    OperationDate = table.Column<DateTime>(nullable: false),
                    DocumentNr = table.Column<int>(nullable: false),
                    DocumentDate = table.Column<DateTime>(nullable: false),
                    DocumentTypeId = table.Column<int>(nullable: false),
                    PrimDocumentTypeId = table.Column<int>(nullable: true),
                    PrimDocumentNr = table.Column<string>(maxLength: 1000, nullable: true),
                    PrimDocumentDate = table.Column<DateTime>(nullable: true),
                    ThirdPartyId = table.Column<int>(nullable: true),
                    OperationType = table.Column<int>(nullable: false),
                    InvoiceDetailsId = table.Column<int>(nullable: true),
                    AssetClassCodesId = table.Column<int>(nullable: true),
                    AssetAccountId = table.Column<int>(nullable: true),
                    DepreciationAccountId = table.Column<int>(nullable: true),
                    ExpenseAccountId = table.Column<int>(nullable: true),
                    ImoAssetStorageId = table.Column<int>(nullable: true),
                    State = table.Column<int>(nullable: false),
                    Processed = table.Column<bool>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImoAssetItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImoAssetItem_Account_AssetAccountId",
                        column: x => x.AssetAccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImoAssetItem_ImoAssetClassCode_AssetClassCodesId",
                        column: x => x.AssetClassCodesId,
                        principalTable: "ImoAssetClassCode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImoAssetItem_Account_DepreciationAccountId",
                        column: x => x.DepreciationAccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImoAssetItem_DocumentType_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DocumentType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ImoAssetItem_Account_ExpenseAccountId",
                        column: x => x.ExpenseAccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImoAssetItem_ImoAssetStorage_ImoAssetStorageId",
                        column: x => x.ImoAssetStorageId,
                        principalTable: "ImoAssetStorage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImoAssetItem_InvoiceDetails_InvoiceDetailsId",
                        column: x => x.InvoiceDetailsId,
                        principalTable: "InvoiceDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImoAssetItem_DocumentType_PrimDocumentTypeId",
                        column: x => x.PrimDocumentTypeId,
                        principalTable: "DocumentType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImoAssetItem_Persons_ThirdPartyId",
                        column: x => x.ThirdPartyId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ImoAssetOper",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    OperationDate = table.Column<DateTime>(nullable: false),
                    OperUseStartDate = table.Column<DateTime>(nullable: false),
                    DocumentNr = table.Column<int>(nullable: false),
                    DocumentDate = table.Column<DateTime>(nullable: false),
                    DocumentTypeId = table.Column<int>(nullable: false),
                    AssetsOperType = table.Column<int>(nullable: false),
                    AssetsStoreInId = table.Column<int>(nullable: true),
                    AssetsStoreOutId = table.Column<int>(nullable: true),
                    Processed = table.Column<bool>(nullable: false),
                    InvoiceId = table.Column<int>(nullable: true),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImoAssetOper", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImoAssetOper_ImoAssetStorage_AssetsStoreInId",
                        column: x => x.AssetsStoreInId,
                        principalTable: "ImoAssetStorage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImoAssetOper_ImoAssetStorage_AssetsStoreOutId",
                        column: x => x.AssetsStoreOutId,
                        principalTable: "ImoAssetStorage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImoAssetOper_DocumentType_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DocumentType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ImoAssetOper_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ImoAssetOperDetail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    ImoAssetItemId = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    InvValueModif = table.Column<decimal>(nullable: false),
                    FiscalValueModif = table.Column<decimal>(nullable: false),
                    DurationModif = table.Column<int>(nullable: false),
                    DeprecModif = table.Column<decimal>(nullable: false),
                    FiscalDeprecModif = table.Column<decimal>(nullable: false),
                    ImoAssetOperId = table.Column<int>(nullable: false),
                    InvoiceDetailId = table.Column<int>(nullable: true),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false),
                    ImoAssetOperId1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImoAssetOperDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImoAssetOperDetail_ImoAssetItem_ImoAssetItemId",
                        column: x => x.ImoAssetItemId,
                        principalTable: "ImoAssetItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ImoAssetOperDetail_ImoAssetOper_ImoAssetOperId",
                        column: x => x.ImoAssetOperId,
                        principalTable: "ImoAssetOper",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ImoAssetOperDetail_ImoAssetOper_ImoAssetOperId1",
                        column: x => x.ImoAssetOperId1,
                        principalTable: "ImoAssetOper",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImoAssetOperDetail_InvoiceDetails_InvoiceDetailId",
                        column: x => x.InvoiceDetailId,
                        principalTable: "InvoiceDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ImoAssetStock",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    ImoAssetItemId = table.Column<int>(nullable: false),
                    OperType = table.Column<int>(nullable: false),
                    StockDate = table.Column<DateTime>(nullable: false),
                    TranzQuantity = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    TranzDuration = table.Column<int>(nullable: false),
                    Duration = table.Column<int>(nullable: false),
                    StorageId = table.Column<int>(nullable: false),
                    TranzInventoryValue = table.Column<decimal>(nullable: false),
                    InventoryValue = table.Column<decimal>(nullable: false),
                    TranzFiscalInventoryValue = table.Column<decimal>(nullable: false),
                    FiscalInventoryValue = table.Column<decimal>(nullable: false),
                    TranzDeprec = table.Column<decimal>(nullable: false),
                    Deprec = table.Column<decimal>(nullable: false),
                    TranzFiscalDeprec = table.Column<decimal>(nullable: false),
                    FiscalDeprec = table.Column<decimal>(nullable: false),
                    InConservare = table.Column<bool>(nullable: false),
                    MonthlyDepreciation = table.Column<decimal>(nullable: false),
                    MonthlyFiscalDepreciation = table.Column<decimal>(nullable: false),
                    ImoAssetItemPFId = table.Column<int>(nullable: true),
                    ImoAssetOperDetId = table.Column<int>(nullable: true),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImoAssetStock", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImoAssetStock_ImoAssetItem_ImoAssetItemId",
                        column: x => x.ImoAssetItemId,
                        principalTable: "ImoAssetItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ImoAssetStock_ImoAssetItem_ImoAssetItemPFId",
                        column: x => x.ImoAssetItemPFId,
                        principalTable: "ImoAssetItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImoAssetStock_ImoAssetOperDetail_ImoAssetOperDetId",
                        column: x => x.ImoAssetOperDetId,
                        principalTable: "ImoAssetOperDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImoAssetStock_ImoAssetStorage_StorageId",
                        column: x => x.StorageId,
                        principalTable: "ImoAssetStorage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImoAssetStockReserve",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TranzDeprecReserve = table.Column<decimal>(nullable: false),
                    DeprecReserve = table.Column<decimal>(nullable: false),
                    TranzReserve = table.Column<decimal>(nullable: false),
                    Reserve = table.Column<decimal>(nullable: false),
                    ExpenseReserve = table.Column<decimal>(nullable: false),
                    ImoAssetStockId = table.Column<int>(nullable: true),
                    ImoAssetOperDetailId = table.Column<int>(nullable: true),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImoAssetStockReserve", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImoAssetStockReserve_ImoAssetOperDetail_ImoAssetOperDetailId",
                        column: x => x.ImoAssetOperDetailId,
                        principalTable: "ImoAssetOperDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImoAssetStockReserve_ImoAssetStock_ImoAssetStockId",
                        column: x => x.ImoAssetStockId,
                        principalTable: "ImoAssetStock",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ContractsCategoryId",
                table: "Contracts",
                column: "ContractsCategoryId",
                unique: true,
                filter: "[ContractsCategoryId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetClassCode_AssetAccountId",
                table: "ImoAssetClassCode",
                column: "AssetAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetClassCode_ClassCodeParrentId",
                table: "ImoAssetClassCode",
                column: "ClassCodeParrentId");

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetClassCode_DepreciationAccountId",
                table: "ImoAssetClassCode",
                column: "DepreciationAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetClassCode_ExpenseAccountId",
                table: "ImoAssetClassCode",
                column: "ExpenseAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetItem_AssetAccountId",
                table: "ImoAssetItem",
                column: "AssetAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetItem_AssetClassCodesId",
                table: "ImoAssetItem",
                column: "AssetClassCodesId");

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetItem_DepreciationAccountId",
                table: "ImoAssetItem",
                column: "DepreciationAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetItem_DocumentTypeId",
                table: "ImoAssetItem",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetItem_ExpenseAccountId",
                table: "ImoAssetItem",
                column: "ExpenseAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetItem_ImoAssetStorageId",
                table: "ImoAssetItem",
                column: "ImoAssetStorageId");

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetItem_InvoiceDetailsId",
                table: "ImoAssetItem",
                column: "InvoiceDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetItem_PrimDocumentTypeId",
                table: "ImoAssetItem",
                column: "PrimDocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetItem_ThirdPartyId",
                table: "ImoAssetItem",
                column: "ThirdPartyId");

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetOper_AssetsStoreInId",
                table: "ImoAssetOper",
                column: "AssetsStoreInId");

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetOper_AssetsStoreOutId",
                table: "ImoAssetOper",
                column: "AssetsStoreOutId");

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetOper_DocumentTypeId",
                table: "ImoAssetOper",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetOper_InvoiceId",
                table: "ImoAssetOper",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetOperDetail_ImoAssetItemId",
                table: "ImoAssetOperDetail",
                column: "ImoAssetItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetOperDetail_ImoAssetOperId",
                table: "ImoAssetOperDetail",
                column: "ImoAssetOperId");

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetOperDetail_ImoAssetOperId1",
                table: "ImoAssetOperDetail",
                column: "ImoAssetOperId1");

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetOperDetail_InvoiceDetailId",
                table: "ImoAssetOperDetail",
                column: "InvoiceDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetOperDocType_DocumentTypeId",
                table: "ImoAssetOperDocType",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetStock_ImoAssetItemId",
                table: "ImoAssetStock",
                column: "ImoAssetItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetStock_ImoAssetItemPFId",
                table: "ImoAssetStock",
                column: "ImoAssetItemPFId");

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetStock_ImoAssetOperDetId",
                table: "ImoAssetStock",
                column: "ImoAssetOperDetId");

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetStock_StorageId",
                table: "ImoAssetStock",
                column: "StorageId");

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetStockReserve_ImoAssetOperDetailId",
                table: "ImoAssetStockReserve",
                column: "ImoAssetOperDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetStockReserve_ImoAssetStockId",
                table: "ImoAssetStockReserve",
                column: "ImoAssetStockId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_ContractsCategory_ContractsCategoryId",
                table: "Contracts",
                column: "ContractsCategoryId",
                principalTable: "ContractsCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_ContractsCategory_ContractsCategoryId",
                table: "Contracts");

            migrationBuilder.DropTable(
                name: "ImoAssetOperDocType");

            migrationBuilder.DropTable(
                name: "ImoAssetOperForType");

            migrationBuilder.DropTable(
                name: "ImoAssetSetup");

            migrationBuilder.DropTable(
                name: "ImoAssetStockReserve");

            migrationBuilder.DropTable(
                name: "ImoAssetStock");

            migrationBuilder.DropTable(
                name: "ImoAssetOperDetail");

            migrationBuilder.DropTable(
                name: "ImoAssetItem");

            migrationBuilder.DropTable(
                name: "ImoAssetOper");

            migrationBuilder.DropTable(
                name: "ImoAssetClassCode");

            migrationBuilder.DropTable(
                name: "ImoAssetStorage");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_ContractsCategoryId",
                table: "Contracts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContractsCategory",
                table: "ContractsCategory");

            migrationBuilder.RenameTable(
                name: "ContractsCategory",
                newName: "ContractsCategories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContractsCategories",
                table: "ContractsCategories",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ContractsCategoryId",
                table: "Contracts",
                column: "ContractsCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_ContractsCategories_ContractsCategoryId",
                table: "Contracts",
                column: "ContractsCategoryId",
                principalTable: "ContractsCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
