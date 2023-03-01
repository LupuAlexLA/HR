using Niva.Erp.Models.ImoAsset;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Niva.Erp.Conta.ImoAsset
{
    public class ImoAssetListDto
    {
        public virtual int Id { get; set; }

        [StringLength(1000)]
        public virtual string Name { get; set; }

        public virtual int InventoryNr { get; set; }

        public virtual decimal PriceUnit { get; set; }

        public virtual decimal InventoryValue { get; set; }

        public virtual decimal FiscalInventoryValue { get; set; }

        public DateTime OperationDate { get; set; }

        public virtual string UseStartDate { get; set; }

        public virtual string DepreciationStartDate { get; set; }

        public virtual int DurationInMonths { get; set; }

        public virtual bool InConservare { get; set; }

        public virtual bool InStock { get; set; }

        public int DocumentNr { get; set; }

        public DateTime DocumentDate { get; set; }

        public string DocumentType { get; set; }

        public string ClassCode { get; set; }

        public string ThirdParty { get; set; }

        public string Invoice { get; set; }

        public bool Processed { get; set; }
        public bool ProcessedIn { get; set; }
        public bool ProcessedInUse { get; set; }

    }

    public class ImoAssetAddDto
    {
        public DateTime OperationDate { get; set; }

        public int DocumentTypeId { get; set; }

        public string DocumentType { get; set; }

        public ImoAssetOperType OperationType { get; set; }

        public int? InvoiceId { get; set; }

        public IList<ImoAssetAddInvoiceDetailDto> InvoiceDetail { get; set; }

        public List<ImoAssetAddDetailDto> Assets { get; set; }

        public bool ShowForm1 { get; set; }

        public bool ShowForm2 { get; set; }

        public bool ShowForm3 { get; set; }

        public bool FinishAdd { get; set; }
    }

    public class ImoAssetAddInvoiceDetailDto
    {
        public int InvoiceId { get; set; }

        public int? InvoiceDetailsId { get; set; }

        public int DocumentNr { get; set; }

        public DateTime DocumentDate { get; set; }

        public string AssetName { get; set; }

        public int Quantity { get; set; }

        public decimal InvValue { get; set; }

        public int? StorageInId { get; set; }

        public int? PrimDocumentTypeId { get; set; }

        public string PrimDocumentNr { get; set; }

        public DateTime? PrimDocumentDate { get; set; }

        public int? ThirdPartyId { get; set; }
    }

    public class ImoAssetAddDetailDto
    {
        public int Id { get; set; }

        [StringLength(1000)]
        public string Name { get; set; }

        public int InventoryNr { get; set; }

        public int Quantity { get; set; }

        public decimal InventoryValue { get; set; }

        public decimal FiscalInventoryValue { get; set; }

        public DateTime? UseStartDate { get; set; }

        public DateTime? DepreciationStartDate { get; set; }

        public int DurationInMonths { get; set; }

        public decimal Depreciation { get; set; }

        public decimal FiscalDepreciation { get; set; }

        public int? InvoiceDetailsId { get; set; }

        public int? AssetClassCodesId { get; set; }

        public int? AssetAccountId { get; set; }
        public int? AssetAccountInUseId { get; set; }

        public int? DepreciationAccountId { get; set; }

        public int? ExpenseAccountId { get; set; }

        public int? StorageInId { get; set; }

        public string StorageIn { get; set; }

        public int DocumentNr { get; set; }

        public DateTime DocumentDate { get; set; }

        public int? PrimDocumentTypeId { get; set; }

        public string PrimDocumentNr { get; set; }

        public DateTime? PrimDocumentDate { get; set; }

        public int? ThirdPartyId { get; set; }
    }

    public class ImoAssetsDDDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int InventoryNr { get; set; }
    }

    public class ImoAssetAddDirectDto
    {
        public int Id { get; set; }

        public int AssetType { get; set; }

        public DateTime OperationDate { get; set; }

        public int DocumentTypeId { get; set; }

        public string DocumentType { get; set; }

        public ImoAssetOperType OperationType { get; set; }

        public int? InvoiceId { get; set; }

        public bool FinishAdd { get; set; }

        [StringLength(1000)]
        public string Name { get; set; }

        public int InventoryNr { get; set; }

        public int Quantity { get; set; }

        public decimal InventoryValue { get; set; }

        public decimal FiscalInventoryValue { get; set; }

        public DateTime? UseStartDate { get; set; }

        public DateTime? DepreciationStartDate { get; set; }

        public int DurationInMonths { get; set; }

        public int? RemainingDuration { get; set; }

        public decimal? Depreciation { get; set; }

        public decimal? FiscalDepreciation { get; set; }

        public decimal? MonthlyDepreciation { get; set; }

        public decimal? MonthlyFiscalDepreciation { get; set; }

        public int? InvoiceDetailsId { get; set; }

        public int? AssetClassCodesId { get; set; }

        public int? AssetAccountId { get; set; }

        public int? DepreciationAccountId { get; set; }

        public int? ExpenseAccountId { get; set; }
        public int? AssetAccountInUseId { get; set; }

        public int? StorageInId { get; set; }

        public string StorageIn { get; set; }

        public int DocumentNr { get; set; }

        public DateTime DocumentDate { get; set; }

        public bool Processed { get; set; }
        public bool ProcessedIn { get; set; }
        public bool ProcessedInUse { get; set; }

        public int? PrimDocumentTypeId { get; set; }

        public string PrimDocumentNr { get; set; }

        public DateTime? PrimDocumentDate { get; set; }

        public int? ThirdPartyId { get; set; }

        public string ThirdPartyName { get; set; }
    }
}
