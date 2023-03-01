using Niva.Erp.Models.ImoAsset;
using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Conta.ImoAsset
{
    public class ImoAssetOperListDto
    {
        public DateTime DataStart { get; set; }

        public DateTime DataEnd { get; set; }

        public List<ImoAssetOperListDetailDto> ListDetail { get; set; }
    }

    public class ImoAssetOperListDetailDto
    {
        public int Id { get; set; }

        public string OperationType { get; set; }
        public int OperationTypeId { get; set; }

        public DateTime OperationDate { get; set; }

        public string DocumentType { get; set; }

        public int DocumentNr { get; set; }

        public DateTime DocumentDate { get; set; }

        public bool Processed { get; set; }
        public bool ShowReportBtn { 
            get 
            { 
                if(OperationTypeId == (int)ImoAssetOperType.BonMiscare || OperationTypeId == (int)ImoAssetOperType.Transfer)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }

    public class OperTypeListDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

    }

    public class DocumentTypeListDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

    }

    public class ImoAssetOperEditDto
    {
        public int Id { get; set; }

        public int? OperationTypeId { get; set; }

        public string OperationType { get; set; }

        public DateTime OperationDate { get; set; }

        public string OperationDateStr { get; set; }

        public int DocumentNr { get; set; }

        public DateTime DocumentDate { get; set; }

        public string DocumentDateStr { get; set; }

        public int? DocumentTypeId { get; set; }

        public string DocumentType { get; set; }

        public ImoAssetOperType AssetsOperType { get; set; }

        public int? AssetsStoreInId { get; set; }

        public string AssetsStoreIn { get; set; }

        public int? AssetsStoreOutId { get; set; }

        public string AssetsStoreOut { get; set; }
        public int? PersonStoreInId { get; set; }
        public string PersonStoreInName { get; set; }

        public int? PersonStoreOutId { get; set; }
        public string PersonStoreOutName { get; set; }
        public int? InvoiceId { get; set; }

        public bool ShowForm1 { get; set; }

        public bool ShowForm2 { get; set; }

        public bool ShowForm3 { get; set; }

        public bool FinishAdd { get; set; }

        public bool ShowStorage { get; set; }

        public bool ShowValues { get; set; }

        public bool ShowModifValues { get; set; } // la true afisez doar campurile cu diferenta de valoare, la false afisez valorile intregi si valorile modif sunt disabled

        public List<ImoAssetOperDetailEditDto> Details { get; set; }
        public List<ImoAssetOperDetailEditModifAccountDto> AssetAccountDetails { get; set; }
    }

    public class ImoAssetOperDetailEditDto
    {
        public int Id { get; set; }

        public int? ImoAssetItemId { get; set; }

        public string ImoAssetItem { get; set; }

        public int Quantity { get; set; }

        public Decimal InvValueOld { get; set; }

        public Decimal FiscalValueOld { get; set; }

        public Decimal InvValueNew { get; set; }

        public Decimal FiscalValueNew { get; set; }

        public Decimal InvValueModif { get; set; }

        public Decimal FiscalValueModif { get; set; }

        public int DurationModif { get; set; }

        public Decimal DeprecModif { get; set; }

        public Decimal FiscalDeprecModif { get; set; }

        public int IdOrd { get; set; }

        public int? InvoiceDetailId { get; set; }

    }

    public class ImoAssetOperDetailEditModifAccountDto
    {
        public int Id { get; set; }

        public int? ImoAssetItemId { get; set; }

        public string ImoAssetItem { get; set; }
        public int IdOrd { get; set; }
        public int? OldAssetAccountId { get; set; }
        public string OldAssetAccount { get; set; }
        public int? OldAssetAccountInUseId { get; set; }
        public string OldAssetAccountInUse { get; set; }

        public int? OldDepreciationAccountId { get; set; }
        public string OldDepreciationAccount { get; set; }

        public int? OldExpenseAccountId { get; set; }
        public string OldExpenseAccount { get; set; }

        public int? NewAssetAccountId { get; set; }
        public string NewAssetAccount { get; set; }

        public int? NewAssetAccountInUseId { get; set; }
        public string NewAssetAccountInUse { get; set; }

        public int? NewDepreciationAccountId { get; set; }
        public string NewDepreciationAccount { get; set; }

        public int? NewExpenseAccountId { get; set; }
        public string NewExpenseAccount { get; set; }

    }
}
