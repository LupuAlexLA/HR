using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Conta.AutoOperation
{
    public class AutoOperationDto
    {
        public int? AutoOperType { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool ShowSearchForm { get; set; }

        public bool ShowSummary { get; set; }

        public bool ShowResults { get; set; }

        public bool ShowCompute { get; set; }

        public bool ShowComputeDetails { get; set; }

        public DateTime AddStartDate { get; set; }

        public DateTime AddEndDate { get; set; }

        public List<AutoOperationSummaryDto> Summary { get; set; }

        public List<AutoOperationComputeDto> PrepareCompute { get; set; }
    }

    public class AutoOperationSummaryDto
    {
        public int Id { get; set; }

        public int AutoOperType { get; set; }

        public int OperationTypeId { get; set; }

        public string OperationType { get; set; }

        public DateTime OperationDate { get; set; }

        public int CurrencyId { get; set; }

        public string Currency { get; set; }

        public virtual string DocumentType { get; set; }

        public string DocumentNumber { get; set; }

        public DateTime DocumentDate { get; set; }

        public bool ShowDetail { get; set; }

        public List<AutoOperationSummaryDetailDto> SummaryDetail { get; set; }
    }

    public class AutoOperationSummaryDetailDto
    {
        public string AssetName { get; set; }

        public string DebitAccount { get; set; }

        public string CreditAccount { get; set; }

        public virtual decimal Value { get; set; }

        public virtual decimal ValueCurr { get; set; }

        public virtual string Details { get; set; }

        public virtual int OperationalId { get; set; }

        public int? OperationDetailId { get; set; }
    }

    public class AutoOperationComputeDto
    {
        public int? OperationId { get; set; }

        public int? OperationDetailId { get; set; }

        public int? GestId { get; set; }

        public DateTime OperationDate { get; set; }

        public int? DocumentTypeId { get; set; }

        public string DocumentType { get; set; }

        public string DocumentNr { get; set; }

        public DateTime? DocumentDate { get; set; }

        public int CurrencyId { get; set; }

        public string Currency { get; set; }

        public int OperationTypeId { get; set; }

        public string OperationType { get; set; }

        public int ItemId { get; set; }

        public string ItemName { get; set; }

        public int? StorageOutId { get; set; }

        public string StorageOut { get; set; }

        public int? StorageInId { get; set; }

        public string StorageIn { get; set; }

        public decimal InventoryValue { get; set; }

        public decimal DepreciationValue { get; set; }

        public decimal VATValue { get; set; }

        public decimal DepreciationVAT { get; set; }

        public string AccountSort { get; set; }
    }
}
