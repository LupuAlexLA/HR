using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niva.Erp.Models.ImoAsset
{
    public class ImoAssetItem : AuditedEntity<int>, IMustHaveTenant
    {
        [StringLength(1000)]
        public string Name { get; set; }

        public ImoAssetType ImoAssetType { get; set; }

        public decimal PriceUnit { get; set; }

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

        public bool InConservare { get; set; }

        public bool InStock { get; set; }

        public DateTime OperationDate { get; set; }

        public int DocumentNr { get; set; }

        public DateTime DocumentDate { get; set; }

        [ForeignKey("DocumentType")]
        public int DocumentTypeId { get; set; }
        public DocumentType DocumentType { get; set; }

        [ForeignKey("PrimDocumentType")]
        public int? PrimDocumentTypeId { get; set; }
        public DocumentType PrimDocumentType { get; set; }

        [StringLength(1000)]
        public string PrimDocumentNr { get; set; }

        public DateTime? PrimDocumentDate { get; set; }

        [ForeignKey("ThirdParty")]
        public int? ThirdPartyId { get; set; }
        public Person ThirdParty { get; set; }

        public ImoAssetOperType OperationType { get; set; }

       

        [ForeignKey("InvoiceDetails")]
        public int? InvoiceDetailsId { get; set; }
        public InvoiceDetails InvoiceDetails { get; set; }

        [ForeignKey("AssetClassCodes")]
        public int? AssetClassCodesId { get; set; }
        public ImoAssetClassCode AssetClassCodes { get; set; }

        [ForeignKey("AssetAccount")]
        public int? AssetAccountId { get; set; }
        public Account AssetAccount { get; set; }      
        
        [ForeignKey("AssetAccountInUse")]
        public int? AssetAccountInUseId { get; set; }
        public Account AssetAccountInUse { get; set; }

        [ForeignKey("DepreciationAccount")]
        public int? DepreciationAccountId { get; set; }
        public Account DepreciationAccount { get; set; }

        [ForeignKey("ExpenseAccount")]
        public int? ExpenseAccountId { get; set; }
        public Account ExpenseAccount { get; set; }

        [ForeignKey("ImoAssetStorage")]
        public int? ImoAssetStorageId { get; set; }
        public ImoAssetStorage ImoAssetStorage { get; set; }

        public State State { get; set; }

        public bool Processed { get; set; }
        public bool ProcessedIn { get; set; } // ProcesatIntrare
        public bool ProcessedInUse { get; set; }

        public int TenantId { get; set; }

    }

    public enum ImoAssetType
    {
        [Description("Mijloc fix")]
        MijlocFix,
        [Description("Drept de utilizare")]
        DreptDeUtilizare
    }
}
