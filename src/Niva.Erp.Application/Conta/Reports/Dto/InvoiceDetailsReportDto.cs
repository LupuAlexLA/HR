using System;
using System.Collections.Generic;

namespace Niva.Erp.Conta.Reports.Dto
{
    public class InvoiceDetailsReportDto    
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? InvoiceElementsDetailsId { get; set; }
        public string InvoiceElementsDetails { get; set; }
        public virtual int? InvoiceElementsDetailsCategoryId { get; set; }
        public virtual string InvoiceElementsDetailsCategory { get; set; }
        public int? ContCheltuialaId { get; set; }
        public string ContCheltuiala { get; set; }
      
        public List<InvoiceDetailsList> Details { get; set; }
    }

    public class InvoiceDetailsList
    {
        public DateTime InvoiceDate { get; set; }
        public string ContCheltuiala { get; set; }
        public virtual string InvoiceElementsDetailsCategory { get; set; }
        public string InvoiceElementsDetails { get; set; }
        public string ThirdPartyName { get; set; }
        public string DocumentTypeName { get; set; }
        public string DocumentNumber { get; set; }
        public decimal Value { get; set; }
        public string CurrencyName { get; set; }
        public decimal ValoareValuta { get; set; }
        public string Explicatii { get; set; }
        public int TenantId { get; set; }
    }
}
