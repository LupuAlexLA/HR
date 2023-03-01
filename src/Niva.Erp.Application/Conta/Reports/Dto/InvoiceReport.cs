using System;
using System.Collections.Generic;

namespace Niva.Erp.Conta.Reports.Dto
{
    public class InvoiceModel
    {
        public int TenantId { get; set; }
        public string AppClientId1 { get; set; }

        public string AppClientId2 { get; set; }

        public string AppClientName { get; set; }
        public string AppClientAddress { get; set; }
        public string AppClientBank { get; set; }
        public string AppClientBankAccount { get; set; }
        public string ClientName { get; set; }

        public string ClientId1 { get; set; }
        public string ClientId2 { get; set; }
        public string ClientAddress { get; set; }
        public string ClientBank { get; set; }
        public string ClientBankAccount { get; set; }
        public decimal Value { get; set; }
        public string CurrencyName { get; set; }
        public DateTime InvoiceDate { get; set; }

        public  string InvoiceNumber { get; set; }

        public virtual string InvoiceSeries { get; set; }
        public decimal Total { get; set; }

        public List<InvoiceDetailsReport> InvoiceDetails { get; set; }

    }

    public class InvoiceDetailsReport
    {
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal Value { get; set; }
    }
}
