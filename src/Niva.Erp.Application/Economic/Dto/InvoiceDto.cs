using Niva.Erp.Models.Buget;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Contracts;
using System;
using System.Collections.Generic;

namespace Niva.Erp.Economic
{
    public class NirDetailDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ThirdPartyDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class InvoiceListDto
    {
        public int Id { get; set; }
        public string Details { get; set; }
        public decimal TotalValue { get; set; }
        public decimal RemainingValue { get; set; }
    }

    public class InvoiceListSelectableDto
    {
        public int Id { get; set; }
        public string Details { get; set; }
        public decimal TotalValue { get; set; }
        public decimal RemainingValue { get; set; }
        public decimal Rest { get; set; }
        public decimal PayedValue { get; set; }
        public int? CurrencyId { get; set; }
        public int? ThirdPartyId { get; set; }
        public bool Selected { get; set; }
    }

    public class InvoiceListForImoAssetDto
    {
        public int Id { get; set; }
        public string Details { get; set; }
        public decimal InvValue { get; set; }
        public decimal FiscalValue { get; set; }
        public int? Duration { get; set; }
    }

    public class InvoiceListForInvObjectDto
    {
        public int Id { get; set; }
        public string Details { get; set; }
        public decimal InvValue { get; set; }
        public int? Duration { get; set; }
    }

    public class InvoiceDTO
    {
        public int Id { get; set; }
        public DateTime OperationDate { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceSeries { get; set; }
        public Decimal Value { get; set; }
        public Decimal ValueLocalCurr { get; set; }
        //public Decimal VAT { get; set; }
        public Decimal VATLocalCurr { get; set; }
        public DateTime? DueDate { get; set; }
        public string Description { get; set; }
        public string RegNumber { get; set; }
        public DateTime? RegDate { get; set; }
        public string ContractNumber { get; set; }
        public int? ThirdPartyId { get; set; }
        public string ThirdPartyName { get; set; }
        public IList<InvoiceDetailsDTO> InvoiceDetails { get; set; }
        public virtual State State { get; set; }
        public virtual ThirdPartyQuality ThirdPartyQuality { get; set; }
        public virtual string ThirdPartyQualityStr { get; set; }
        public virtual int? ContaOperationId { get; set; }
        //public virtual Operation ContaOperation { get; set; }
        public int ContaOperationStatus { get; set; } // starea notei contabile checked/unchecked
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public int? ContractsId { get; set; }
        //public virtual Contracts Contracts { get; set; } 
        public bool EnableEdit { get; set; }
        public int DocumentTypeId { get; set; }
        public string DocumentType { get; set; }
        public string DocumentTypeShortName { get; set; }

        public DateTime? StartDatePeriod { get; set; }
        public DateTime? EndDatePeriod { get; set; }

        public virtual int? ActivityTypeId { get; set; } // tip fond

        public int? DecontId { get; set; }
        public bool HasDecont { get; set; }
        public decimal RestPlata { get; set; }
        public int FileDocId { get; set; }
        public decimal FileDocValue { get; set; }
        public int? DecontNumber { get; set; }
        public bool DecontareInLei { get; set; }
        public decimal? CursValutar { get; set; }
        public decimal? ValoareTotalaFactura { get; set; }
        public virtual int? MonedaFacturaId { get; set; }
        public virtual bool ForcePaid { get; set; }
        public DateTime? DataStartAmortizare { get; set; }
    }

    public class InvoiceForDecontDTO
    {
        public virtual int Id { get; set; }
        public string ThirdPartyAccount { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceSeries { get; set; }
        public Decimal Value { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public int MonedaPlataId { get; set; }
        public string MonedaPlataName { get; set; }
        public bool Selected { get; set; }
        public int FileDocId { get; set; }
        public decimal FileDocValue { get; set; }
        public virtual State State { get; set; }
    }

    public enum ItemState
    {
        Unchanged, New, Modified, Deleted
    }

    public class InvoiceDetailsDTO
    {
        public int Id { get; set; }
        public string Element { get; set; }
        public int Quantity { get; set; }
        public decimal UnitValue { get; set; }
        public Decimal Value { get; set; }
        //public Decimal VAT { get; set; }
        //public Decimal ProcVAT { get; set; }
        public int? CotaTVA_Id { get; set; }
        public virtual int? DurationInMonths { get; set; }
        public State State { get; set; }
        public int InvoiceElementsDetailsId { get; set; }
        public InvoiceElementsDetailsDTO InvoiceElementsDetails { get; set; }
        public ItemState ItemState { get; set; }
        public bool UsedInGest { get; set; }
        public virtual int? ContaOperationDetailId { get; set; }

        //public   int? DebitorAccountId { get; set; }
        //public   Account DebitorAccount { get; set; }


        //public   int? CreditorAccountId { get; set; }
        //public   Account CreditorAccount { get; set; }


        public int InvoicesId { get; set; }
        //public   Invoices Invoices { get; set; }
        public decimal? ValoareTotalaDetaliu { get; set; }
        public DateTime? DataStartAmortizare { get; set; }  
    }

    public class InvoiceElementsDetailsDTO
    {
        public int Id { get; set; }
        public virtual string Description { get; set; }
        public virtual int InvoiceElementsType { get; set; }
        public virtual string InvoiceElementsTypeStr { get; set; }
        public string ThirdPartyAccount { get; set; }
        public string CorrespondentAccount { get; set; }
        public string AmortizationAccount { get; set; }
        public virtual string ExpenseAmortizAccount { get; set; }
        public virtual State State { get; set; }
        public int? InvoiceElementsDetailsCategoryId { get; set; }
        public string InvoiceElementsDetailsCategory { get; set; }
        // Afisare campuri Cod clasificare si Durata => PAAP selectare categorie MF
        public bool ShowMF { get { return (InvoiceElementsType == (int)Niva.Erp.Models.Contracts.InvoiceElementsType.MijloaceFixe); } }
        // Afisare campuri  Durata => PAAP selectare categorie CheltAvans
        public bool ShowCA { get { return (InvoiceElementsType == (int)Niva.Erp.Models.Contracts.InvoiceElementsType.CheltuieliInAvans); } }

    }

    public class InvoiceElementsDetailsCategoryEditDTO
    {
        public int Id { get; set; }
        public string CategoryElementDetName { get; set; }

        public virtual int? CategoryTypeId { get; set; }
        public State State { get; set; }
    }

    public class InvoiceElementsDetailsCategoryListDTO
    {
        public int Id { get; set; }
        public string CategoryElementDetName { get; set; }

        public virtual string CategoryType { get; set; }
        public State State { get; set; }
    }

    public partial class InvoiceElementAccountsDTO
    {
        public int Id { get; set; }
        public InvoiceElementAccountType InvoiceElementAccountType { get; set; }
        public string InvoiceElementAccountTypeStr { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public virtual State State { get; set; }
    }

    public class AutoInvForm
    {
        public DateTime DataStart { get; set; }

        public DateTime DataEnd { get; set; }

        public List<AutoInvPInvoices> Invoices { get; set; }
        public List<AutoInvNInvoices> NotProcessedInvoices { get; set; }
        public List<AutoInvDInvoices> DeletedInvoices { get; set; }

        public bool ShowProcessedForm { get; set; }

        public bool ShowNotProcessedForm { get; set; }

        public bool ShowDeleteForm { get; set; }
    }

    public class AutoInvNStart
    {
        public DateTime DataStart { get; set; }

        public DateTime DataEnd { get; set; }

        public List<AutoInvNInvoices> Invoices { get; set; }
    }

    public class AutoInvNInvoices
    {
        public virtual int Id { get; set; }

        public virtual bool Selected { get; set; }

        public virtual string ThirdPartyQualityStr { get; set; }

        public virtual string ThirdParty { get; set; }

        public virtual DateTime OperationDate { get; set; }

        public virtual DateTime InvoiceDate { get; set; }

        public virtual string InvoiceNumber { get; set; }

        public virtual string InvoiceSeries { get; set; }

        public virtual Decimal Value { get; set; }

        public Decimal VAT { get; set; }

        public string CurrencyName { get; set; }

        public string Description { get; set; }
    }

    public class AutoInvPStart
    {
        public DateTime DataStart { get; set; }

        public DateTime DataEnd { get; set; }

        public List<AutoInvPInvoices> Invoices { get; set; }
    }

    public class AutoInvPInvoices
    {
        public virtual int Id { get; set; }

        public virtual string ThirdPartyQualityStr { get; set; }

        public virtual string ThirdParty { get; set; }

        public virtual DateTime InvoiceDate { get; set; }

        public virtual string InvoiceNumber { get; set; }

        public virtual string InvoiceSeries { get; set; }

        public virtual Decimal Value { get; set; }

        public Decimal VAT { get; set; }

        public string CurrencyName { get; set; }

        public string Description { get; set; }
    }

    public class AutoInvDStart
    {
        public DateTime DataStart { get; set; }

        public DateTime DataEnd { get; set; }

        public List<AutoInvDInvoices> Invoices { get; set; }
    }

    public class AutoInvDInvoices
    {
        public virtual int Id { get; set; }

        public virtual string ThirdPartyQualityStr { get; set; }

        public virtual string ThirdParty { get; set; }

        public virtual DateTime InvoiceDate { get; set; }

        public virtual string InvoiceNumber { get; set; }

        public virtual string InvoiceSeries { get; set; }

        public virtual Decimal Value { get; set; }

        public Decimal VAT { get; set; }

        public string CurrencyName { get; set; }

        public string Description { get; set; }
    }

    public class InvoiceDetailsForPAAPDto
    {
        public List<InvoiceDetailPAAPDto> InvoiceDetailsAllocatedForPAAP { get; set; }
        public List<InvoiceDetailPAAPDto> InvoiceDetailsAvailableForPAAP { get; set; }
    }

    public class InvoiceDetailPAAPDto
    {
        public virtual int Id { get; set; }
        public string ThirdPartyAccount { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceSeries { get; set; }
        public Decimal Value { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public Decimal DetailValue { get; set; }
        public Decimal? DetailValueLocalCurr { get; set; }
        public bool Selected { get; set; }
    }

    public class InvoiceDetailPAAPWithInvoiceElementsDto
    {
        public virtual int Id { get; set; }
        public string ThirdPartyAccount { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceSeries { get; set; }
        public Decimal Value { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public Decimal DetailValue { get; set; }
        public Decimal? DetailValueLocalCurr { get; set; }
        public string DetailDescription { get; set; }
        public bool Selected { get; set; }
        public int InvoiceElementsDetailsId { get; set; }
        public List<BVC_PAAP_Description_Date> PossiblePaap { get; set; }
    }

    public class BVC_PAAP_Description_Date
    {

        public int BVC_PAAP_Id { get; set; }
        public string Descriere { get; set; }
        public DateTime DataEnd { get; set; }
        public decimal Rest { get; set; }
    }
}
