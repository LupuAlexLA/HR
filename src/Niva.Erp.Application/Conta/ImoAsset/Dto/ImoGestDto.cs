using Niva.Erp.Models.ImoAsset;
using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Conta.ImoAsset
{
    public class ImoGestListDto
    {
        public DateTime? DataStart { get; set; }

        public DateTime? DataEnd { get; set; }

        public int? AssetId { get; set; }

        public int? StorageId { get; set; }

        public List<ImoGestDetailListDto> GestDetail { get; set; }
    }

    public class ImoGestDetailListDto
    {
        public int Id { get; set; }

        public string ImoAssetItem { get; set; }

        public DateTime StockDate { get; set; }

        public string StockDateStr { get; set; }

        public string OperType { get; set; }

        public int TranzQuantity { get; set; }

        public int Quantity { get; set; }

        public int TranzDuration { get; set; }

        public int Duration { get; set; }

        public string Storage { get; set; }

        public decimal TranzInventoryValue { get; set; }

        public decimal InventoryValue { get; set; }

        public decimal TranzFiscalInventoryValue { get; set; }

        public decimal FiscalInventoryValue { get; set; }

        public decimal TranzDeprec { get; set; }

        public decimal Deprec { get; set; }

        public decimal TranzFiscalDeprec { get; set; }

        public decimal FiscalDeprec { get; set; }

        public List<ImoGestReserveDto> ReserveDetail { get; set; }

        public List<ImoGestModernizDto> ModernizDetail { get; set; }

        public bool ShowReserve { get; set; }

        public bool ShowModerniz { get; set; }
    }

    public class ImoGestReserveDto
    {
        public int ImoAssetStockId { get; set; }

        public string OperationDate { get; set; }

        public decimal TranzDeprecReserve { get; set; }

        public decimal DeprecReserve { get; set; }

        public decimal TranzReserve { get; set; }

        public decimal Reserve { get; set; }

        public decimal ExpenseReserve { get; set; }
    }


    public class ImoGestModernizDto
    {
        public int ImoAssetStockId { get; set; }

        public string OperationDate { get; set; }

        public decimal TranzDeprecModerniz { get; set; }

        public decimal DeprecModerniz { get; set; }

        public decimal TranzModerniz { get; set; }

        public decimal Moderniz { get; set; }

        public decimal ExpenseModerniz { get; set; }
    }

    public class ImoGestDelListDto
    {
        public DateTime? DataStart { get; set; }

        public DateTime? DataEnd { get; set; }

        public List<ImoGestDelDetailDto> GestDelDetail { get; set; }
    }

    public class ImoGestDelDetailDto
    {
        public DateTime DateGest { get; set; }
    }

    public class ImoGestComputeListDto
    {
        public DateTime UnprocessedDate { get; set; }

        public DateTime? ComputeDate { get; set; }

        public bool ShowCompute { get; set; }

        public List<ImoOperationListDto> OperationList { get; set; }
    }

    public class ImoOperationListDto
    {
        public int Id { get; set; }

        public DateTime OperationDate { get; set; }

        public string OperationDateStr { get; set; }

        public int DocumentNr { get; set; }

        public DateTime DocumentDate { get; set; }

        public string DocumentDateStr { get; set; }

        public string DocumentType { get; set; }

        public string OperationType { get; set; }

        public string StorageOut { get; set; }

        public string StorageIn { get; set; }

        public int OrdProcess { get; set; }

        public DateTime OperationDateSort { get; set; }
    }

    public class ImoGestRowDto
    {
        public int ImoAssetItemId { get; set; }

        public ImoAssetOperType OperType { get; set; }

        public DateTime StockDate { get; set; }

        public int TranzQuantity { get; set; }

        public int Quantity { get; set; }

        public int TranzDuration { get; set; }

        public int Duration { get; set; }

        public int StorageId { get; set; }

        public decimal TranzInventoryValue { get; set; }

        public decimal InventoryValue { get; set; }

        public decimal TranzFiscalInventoryValue { get; set; }

        public decimal FiscalInventoryValue { get; set; }

        public decimal TranzDeprec { get; set; }

        public decimal Deprec { get; set; }

        public decimal TranzFiscalDeprec { get; set; }

        public decimal FiscalDeprec { get; set; }

        public bool InConservare { get; set; }
    }

}
