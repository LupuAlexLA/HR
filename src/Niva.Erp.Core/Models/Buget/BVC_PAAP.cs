using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Contracts;
using Niva.Erp.Models.HR;
using Niva.Erp.Models.ImoAsset;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Niva.Erp.Models.Buget
{
    public class BVC_PAAP : AuditedEntity<int>, IMustHaveTenant
    {
        public ObiectTranzactie ObiectTranzactie { get; set; }
        public string Descriere { get; set; }
        public string CodCPV { get; set; }
        public decimal ValoareEstimataFaraTvaLei { get; set; }
        public decimal ValoareTotalaValuta { get; set; }
        public decimal ValoareTotalaLei { get; set; }
        public virtual SursaFinantare SursaFinatare { get; set; }
        public DateTime DataStart { get; set; }
        public DateTime DataEnd { get; set; }
        public DateTime FirstInstalmentDate { get; set; }
        public ModalitateDerulare ModalitateDerulare { get; set; }
        public ContractsPaymentInstalmentFreq ContractsPaymentInstalmentFreq { get; set; }

        public virtual bool IsAddedAfterApproval { get; set; } // marchez True, daca achizitia a fost adaugata dupa ce planul a fost aprobat pe un an 

        public virtual IList<BVC_PAAP_State> PaapStateList { get; set; }

        public PAAP_State? GetPaapState
        {
            get { return PaapStateList?.Where(f => f.State == State.Active && f.BVC_PAAP_Id == Id).OrderByDescending(f => f.Id).Select(f => f.Paap_State).First(); }

        }

        public State State { get; set; }

        public int? DurationInMonths { get; set; }
        public int? LocalCurrencyId { get; set; }

        [ForeignKey("InvoiceElementsDetails")]
        public virtual int InvoiceElementsDetailsId { get; set; }
        public virtual InvoiceElementsDetails InvoiceElementsDetails { get; set; }

        [ForeignKey("InvoiceElementsDetailsCategory")]
        public virtual int InvoiceElementsDetailsCategoryId { get; set; }
        public virtual InvoiceElementsDetailsCategory InvoiceElementsDetailsCategory { get; set; }

        [ForeignKey("AssetClassCodes")]
        public int? AssetClassCodesId { get; set; }
        public ImoAssetClassCode AssetClassCodes { get; set; } // cod de clasificare in cazul in care Obiectul tranzactiei este MijlocFix

        [ForeignKey("Departament")]
        public int? DepartamentId { get; set; }
        public virtual Departament Departament { get; set; }

        [ForeignKey("Currency")]
        public virtual int? CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }

        [ForeignKey("CotaTVA")]
        public int? CotaTVA_Id { get; set; }
        public virtual CotaTVA CotaTVA { get; set; }
        public int NrTranse { get; set; }
        public List<BVC_PAAPTranse> Transe { get; set; }

        public int TenantId { get; set; }
    }

    public enum ModalitateDerulare : int
    {
        Offline,
        Online
    }

    public enum SursaFinantare : int
    {
        Proprie
    }

    public enum PAAP_State : int
    {
        Inregistrat,
        Aprobat,
        Finalizat,
        Anulat,
        Amanat,
        Referat
    }

    public enum ObiectTranzactie : int
    {
        Produse,
        Servicii,
        Investitii,
        [Description("Mijloace fixe")]
        MijloaceFixe
    }

    public class BVC_PAAPTranse : AuditedEntity<int>
    {
        [ForeignKey("BVC_PAAP")]
        public int BVC_PAAPId { get; set; }
        public virtual BVC_PAAP BVC_PAAP { get; set; }
        public DateTime DataTransa { get; set; }
        public decimal ValoareLei { get; set; }
        public decimal ValoareLeiFaraTVA { get; set; }

    }
}
