using Niva.Erp.Models.InvObjects;
using System;
using System.Collections.Generic;

namespace Niva.Erp.Conta.InvObjects.Dto
{
    public class InvObjectGestListDto
    {
        public DateTime? DataStart { get; set; }

        public DateTime? DataEnd { get; set; }

        public int? InvObjectId { get; set; }

        public int? StorageId { get; set; }

        public List<InvObjectGestDetailListDto> GestDetail { get; set; }
    }

    public class InvObjectGestDetailListDto
    {
        public int Id { get; set; }

        public string InvObjectItem { get; set; }

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

        public List<InvObjectGestReserveDto> ReserveDetail { get; set; }

        public bool ShowReserve { get; set; }
    }

    public class InvObjectGestReserveDto
    {
        public int InvObjectStockId { get; set; }

        public string OperationDate { get; set; }

        public decimal TranzDeprecReserve { get; set; }

        public decimal DeprecReserve { get; set; }

        public decimal TranzReserve { get; set; }

        public decimal Reserve { get; set; }

        public decimal ExpenseReserve { get; set; }
    }

    public class InvObjectDelListDto
    {
        public DateTime? DataStart { get; set; }

        public DateTime? DataEnd { get; set; }

        public List<InvObjectGestDelDetailDto> GestDelDetail { get; set; }
    }

    public class InvObjectGestDelDetailDto
    {
        public DateTime DateGest { get; set; }
    }

    public class InvObjectGestComputeListDto
    {
        public DateTime UnprocessedDate { get; set; }

        public DateTime? ComputeDate { get; set; }

        public bool ShowCompute { get; set; }

        public List<InvObjectOperationListDto> OperationList { get; set; }
    }

    public class InvObjectOperationListDto
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

    public class InvObjectGestRowDto
    {
        public int InvObjectItemId { get; set; }

        public InvObjectOperType OperType { get; set; }

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
