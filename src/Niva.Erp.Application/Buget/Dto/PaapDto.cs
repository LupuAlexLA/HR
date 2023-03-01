using Niva.Erp.Models.Buget;
using Niva.Erp.Models.Conta.Enums;
using System;
using System.Collections.Generic;

namespace Niva.Erp.Buget.Dto
{
    public class PaapDto
    {
        public int Id { get; set; }
        public string ObiectTranzactie { get; set; }
        public string Descriere { get; set; }
        public string CodCPV { get; set; }
        public decimal Value { get; set; }
        public string SursaFinantare { get; set; }
        public string InvoiceElementsDetailsName { get; set; }
        public int? InvoiceElementsDetailsId { get; set; }
        public string CurrencyName { get; set; }
        public int? CotaTVA_Id { get; set; }
        public int? LocalCurrencyId { get; set; }
        public string InvoiceElementsDetailsCategoryName { get; set; }
        public int? InvoiceElementsDetailsCategoryId { get; set; }
        public DateTime DataStart { get; set; }
        public DateTime DataEnd { get; set; }
        public DateTime FirstInstalmentDate { get; set; }
        public string ModDerulare { get; set; }
        public string ContractsPaymentInstalmentFreq { get; set; }
        public string PersoanaResponsabila { get; set; }
        public string StatePAAP { get; set; }
        public int StatePAAPId { get; set; }
        public string DepartamentName { get; set; }
        public int? DepartamentId { get; set; }
        public bool ShowCategory { get; set; }

        public decimal ValoareTotalaLei { get; set; }
        public decimal ValoareTotalaValuta { get; set; }
        public decimal ValoareRealizata { get; set; }
        public int NrTranse { get; set; }
        public bool IsValueEqualToSumTranse { get; set; }
        public bool HasTranse { get; set; }
        public State State { get; set; }
        public int TenantId { get; set; }
        public List<BVC_PAAPTranseDto> Transe { get; set; }
    }

    public class PaapEditDto
    {
        public int Id { get; set; }
        public string Descriere { get; set; }
        public string Comentarii { get; set; }
        public int? InvoiceElementsDetailsCategoryId { get; set; }
        public int? InvoiceElementsDetailsId { get; set; }
        public int? ObiectTranzactieId { get; set; }
        public int? DepartamentId { get; set; }
        public int? CurrencyId { get; set; }
        public int? ImoAssetClassCodeId { get; set; }
        public int? CotaTVA_Id { get; set; }
        public int? LocalCurrencyId { get; set; }
        public string CodCPV { get; set; }
        public decimal ValoareEstimataFaraTvaLei { get; set; }
        public decimal ValoareTotalaValuta { get; set; }
        public decimal ValoareTotalaLei { get; set; }
        public decimal? AvailableValue { get; set; }
        public int? SursaFinantareId { get; set; }
        public DateTime DataStart { get; set; }
        public DateTime DataEnd { get; set; }
        public DateTime FirstInstalmentDate { get; set; }
        public int? ModalitateDerulareId { get; set; }
        public int? ContractsPaymentInstalmentFreqId { get; set; }
        public int? DurationInMonths { get; set; }
        public int NrTranse { get; set; }
        public List<BVC_PAAPTranseDto> Transe { get; set; }

        public State State { get; set; }
        public int TenantId { get; set; }
    }

    public class BVC_PAAPTranseDto
    {
        public int Id { get; set; }
        public int BVC_PAAPId { get; set; }
        public DateTime DataTransa { get; set; }
        public decimal ValoareLei { get; set; }
        public decimal ValoareLeiFaraTVA { get; set; }
    }

    public class PaapTranseListDto
    {
        public int Id { get; set; }
        public int BVC_PAAPId { get; set; }
        public DateTime DataTransa { get; set; }
        public decimal ValoareLei { get; set; }
        public decimal ValoareLeiFaraTVA { get; set; }
        public string InvoiceElementsDetailsName { get; set; }
        public string InvoiceElementsDetailsCategoryName { get; set; }
        public int? InvoiceElementsDetailsCategoryId { get; set; }
        public int? InvoiceElementsDetailsId { get; set; }
        public string StatePAAP { get; set; }
    }

    public class PaapDepartamentListDto
    {
        public string DepartamentName { get; set; }
        public List<PaapDto> PaapListDetails { get; set; }
    }

    public class PaapStateListDto
    {
        public string Paap_State { get; set; }

        public decimal ValoareEstimataFaraTvaLei { get; set; }
        public decimal ValoareTotalaLei { get; set; }

        public string CurrencyName { get; set; }

        public decimal ValoareTotalaValuta { get; set; }
        public decimal TVA { get; set; }
        public DateTime DataEnd { get; set; }
        public string Comentarii { get; set; }
    }

    public class PaapStateEditDto
    {
        public int Id { get; set; }
        public int PaapId { get; set; }
        public DateTime? OperationDate { get; set; }
        public int TenantId { get; set; }
        public PAAP_State Paap_State { get; set; }
        public State State { get; set; }
        public DateTime DataEnd { get; set; }
        public string Comentarii { get; set; }
    }

    public class PaapApprovedYearDto
    {
        public int Year { get; set; }
        public State State { get; set; }
        public int TenantId { get; set; }
    }

    public class BVC_PAAP_InvoiceDetailsDto
    {
        public int BVC_PAAPId { get; set; }
        public int InvoiceDetailsId { get; set; }
        public int TenantId { get; set; }
    }

    public class BaseOperationPaap
    {
        public int PaapId { get; set; }
        public DateTime DataEnd { get; set; }
        public string Comentarii { get; set; }
    }

    public class AmanarePaapEditDto : BaseOperationPaap
    { }

    public class RealocarePaapDto : BaseOperationPaap
    {
        public decimal ValoareTotala { get; set; }
        public List<BVC_PAAPTranseDto> Transe { get; set; }
    }
}