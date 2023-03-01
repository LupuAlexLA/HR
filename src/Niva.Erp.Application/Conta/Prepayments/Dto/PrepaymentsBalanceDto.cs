using Niva.Erp.Models.PrePayments;
using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Conta.Prepayments
{
    public class PrepaymentsBalanceListDto
    {
        public DateTime? DataStart { get; set; }

        public DateTime? DataEnd { get; set; }

        public int? PrepaymentId { get; set; }

        public PrepaymentType PrepaymentType { get; set; }

        public List<PrepaymentsBalanceDetailListDto> GestDetail { get; set; }
    }

    public class PrepaymentsBalanceDetailListDto
    {
        public int Id { get; set; }

        public string Prepayment { get; set; }

        public DateTime GestDate { get; set; }

        public string OperType { get; set; }

        public int TranzQuantity { get; set; }

        public int Quantity { get; set; }

        public int TranzDuration { get; set; }

        public int Duration { get; set; }

        public decimal TranzPaymentValue { get; set; }

        public decimal PaymentValue { get; set; }

        public decimal TranzDeprec { get; set; }

        public decimal Deprec { get; set; }

        public decimal TranzPaymentVAT { get; set; }

        public decimal PaymentVAT { get; set; }

        public decimal TranzDeprecVAT { get; set; }

        public decimal DeprecVAT { get; set; }
    }

    public class PrepaymentsBalanceComputeListDto
    {
        public DateTime UnprocessedDate { get; set; }

        public DateTime? ComputeDate { get; set; }

        public bool ShowCompute { get; set; }

        public PrepaymentType PrepaymentType { get; set; }

        public List<PrepaymentsOperationListDto> OperationList { get; set; }
    }

    public class PrepaymentsOperationListDto
    {
        public int Id { get; set; }

        public string Prepayment { get; set; }

        public DateTime OperationDate { get; set; }

        public string DocumentNr { get; set; }

        public DateTime DocumentDate { get; set; }

        public string DocumentType { get; set; }

        public string OperationType { get; set; }

        public int OrdProcess { get; set; }

        public DateTime OperationDateSort { get; set; }
    }

    public class PrepaymentsBalanceDelListDto
    {
        public DateTime? DataStart { get; set; }

        public DateTime? DataEnd { get; set; }

        public PrepaymentType PrepaymentType { get; set; }

        public List<PrepaymentsBalanceDelDetailDto> GestDelDetail { get; set; }
    }

    public class PrepaymentsBalanceDelDetailDto
    {
        public DateTime DateGest { get; set; }
    }
}
