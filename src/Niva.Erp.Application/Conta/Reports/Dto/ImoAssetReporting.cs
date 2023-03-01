using System;
using System.Collections.Generic;

namespace Niva.Erp.Conta.Reports.Dto
{
    public class ImoAssetRegistruReport
    {
        public string AppClientId1 { get; set; }
        public string AppClientId2 { get; set; }
        public string AppClientName { get; set; }
        public DateTime RegDate { get; set; }
        public List<ImoAssetRegistruDetails> RegDetails { get; set; }
    }

    public class ImoAssetRegistruDetails
    {
        public string ImoAssetName { get; set; }
        public decimal InventoryValue { get; set; }
        public decimal MonthlyDepreciation { get; set; }
        public decimal Depreciation { get; set; } // amortizare cumulata
        public decimal RemainingInventoryValue { get; set; } // val ramasa
        public int InventoryNr { get; set; }
        public int Duration { get; set; }
        public int RemainingDuration { get; set; }
        public string SyntheticAccount { get; set; }
        public string AnalyticAccount { get; set; }
        public string AccountName { get; set; }
        public string AccountForGroup { get; set; }
        public string DocumentType { get; set; }
        public string DocumentNr { get; set; }
        public DateTime? DocumentDate { get; set; }
        public DateTime? DepreciationStartDate { get; set; }
        public string ThirdParty { get; set; }
        public int? StorageId { get; set; }
        public string Storage { get; set; }
    }

    public class ImoAssetRegV2Report
    {
        public DateTime OperationDate { get; set; }
        public string StorageName { get; set; }
        public List<ImoAssetV2Details> ImoAssetDetails { get; set; }
    }

    public class ImoAssetV2Details
    {
        public string ImoAssetName { get; set; }
        public int InventoryNumber { get; set; }
        public string CategoryName { get; set; }
        public string AssetClassCodes { get; set; }
        public DateTime? DatePIF { get; set; } // data punere in functiune
        public DateTime? DateStartDeprec { get; set; }
        public int DurataUtila { get; set; }
        public int DurataScursaUtila { get; set; }
        public decimal InitialValue { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal MonthlyDeprec { get; set; }
        public decimal CumulativDeprec { get; set; }
        public decimal YearDeprec { get; set; }
        public string StorageName { get; set; } // localizare
        public string OutOfUse { get; set; }
        public string IsSold { get; set; }
        public string IsInUse { get; set; }
        public string ImoAssetAccount { get; set; }
        public string DeprecAccount { get; set; }
        public string ExpenseAccount { get; set; }
        public string HasModerniz { get; set; }

        public List<ImoAssetModerniz> ImoAssetModernizDetails { get; set; }

        public bool ShowModerniz { get; set; }

    }

    public class ImoAssetModerniz
    {
        public DateTime OperationDate { get; set; }
        public DateTime DatePIF { get; set; } // data punere in functiune
        public DateTime DateStartDeprec { get; set; } // sfarsitul lunii in care a fost operatia
        public int DurataUtila { get; set; }
        public int DurataScursaUtila { get; set; }
        public decimal InitialValue { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal MonthlyDeprec { get; set; }
        public decimal CumulativDeprec { get; set; }
        public decimal YearDeprec { get; set; }
        public string StorageName { get; set; }

    }

    public class ImoAssetFisaReport
    {
        public string AppClientId1 { get; set; }
        public string AppClientId2 { get; set; }
        public string AppClientName { get; set; }

        public int InventoryNr { get; set; }
        public string ImoAssetName { get; set; }
        public string DocumentType { get; set; }
        public string DocumentNr { get; set; }
        public DateTime? DocumentDate { get; set; }
        public decimal InventoryValue { get; set; }
        public decimal MonthlyDepreciation { get; set; }
        public string ClassCode { get; set; }
        public DateTime UseStartDate { get; set; }
        public DateTime DepreciationEnd { get; set; }
        public int? ClassCodeNormDuration { get; set; }
        public decimal ProcDeprec { get; set; }
        public List<ImoAssetFisaDetail> FisaDetail { get; set; }
    }

    public class ImoAssetFisaDetail
    {
        public DateTime OperationDate { get; set; }
        public string DocumentType { get; set; }
        public string DocumentNr { get; set; }
        public DateTime? DocumentDate { get; set; }
        public string ThirdParty { get; set; }
        public string OperType { get; set; }
        public string Storage { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Sold { get; set; }
    }

    public class InvObjectImoAssetReport
    {
        public DateTime OperationDate { get; set; }
        public string Storage { get; set; }
        public int TenantId { get; set; }
        public string Parameters { get; set; }
        public List<InventoryDetails> InventoryDetails { get; set; }

    }

    public class InventoryDetails
    {
         public string Storage { get; set; }
        public string ImoAssetName { get; set; }

        public int InventoryNr { get; set; }

        public string UM { get; set; }

        public int StockScriptic { get; set; }

        public int StockFaptic { get; set; }

        public decimal UnitPrice { get; set; }
        public decimal InventoryValue { get; set; }
        public decimal AccountingValue { get; set; }
        public DateTime? OperationDate { get; set; }
    }

    public enum InventoryTypes
    {
        MijloaceFixe,
        ObiecteDeInventar
    }
}
