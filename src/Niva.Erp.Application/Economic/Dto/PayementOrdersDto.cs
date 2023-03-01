using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Niva.Erp.Models.Conta;
using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Economic
{
    public class PayementOrdersForm
    {
        public DateTime SearchStartDate { get; set; }

        public DateTime SearchEndDate { get; set; }

        public int PayerId { get; set; }

        public int? PayerBankId { get; set; }

        public int? ThirdPartyId { get; set; }

        public List<PayementOrdersList> OPList { get; set; }

        public PayementOrderDetail OPDetail { get; set; }

        public bool ShowList { get; set; }

        public bool ShowEditForm { get; set; }

        public int AppClientId { get; set; }

    }

    public class PayementOrdersList
    {
        public int Id { get; set; }

        public int OrderNr { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal Value { get; set; }

        public string PayerBank { get; set; }

        public string PayerBankAccount { get; set; }

        public string BenefId1 { get; set; }

        public string Beneficiary { get; set; }

        public string BenefBank { get; set; }

        public string BenefBankAccount { get; set; }

        public string Currency { get; set; }

        public string Invoice { get; set; }

        public string PaymentDetails { get; set; }

        public bool Finalised { get; set; }

        public bool FinalisedDb { get; set; }
    }

    public class PayementOrderDetail
    {
        public int Id { get; set; }

        public int OrderNr { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal Value { get; set; }

        public string WrittenValue { get; set; }

        public int? PayerBankId { get; set; }

        public int? PayerBankAccountId { get; set; }

        public int? BeneficiaryId { get; set; }

        public string BeneficiaryName { get; set; }

        public int? BenefBankId { get; set; }

        public int? BenefBankAccountId { get; set; }

        public int? CurrencyId { get; set; }

        public List<InvoiceListSelectableDto> InvoicesList { get; set; }    

        public string PaymentDetails { get; set; }

        public OperationStatus Finalised { get; set; }
    }

    public class PaymentOrderForForeignOperationDto
    {
        public int Id { get; set; }
        public string PayerBank { get; set; }
        public string PaymentDetails { get; set; }
    }

    public class PaymentOrderExportFormDto
    {
        public int BenefBankId { get; set; }
        public List<PayementOrdersList> OPList { get; set; }
    }

    public class PaymentOrderExportList
    {
        public int Id { get; set; }

        public int OrderNr { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal Value { get; set; }

        public string PayerBank { get; set; }

        public string PayerBankAccount { get; set; }

        public string BenefId1 { get; set; }

        public string Beneficiary { get; set; }

        public string BenefBank { get; set; }

        public string BenefBankAccount { get; set; }
        public string BenefBankBic { get; set; }

        public string Currency { get; set; }

        public string Invoice { get; set; }

        public string PaymentDetails { get; set; }

        public bool Finalised { get; set; }

        public bool FinalisedDb { get; set; }
        public bool Selected { get; set; }
    }
}
