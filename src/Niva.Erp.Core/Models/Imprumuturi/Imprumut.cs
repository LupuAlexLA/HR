using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.AutoOperation;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Emitenti;
using Niva.Erp.Repositories.Conta;

namespace Niva.Erp.Models.Imprumuturi
{
    public class Imprumut : Entity<int>, IMustHaveTenant
    {
        [ForeignKey("DocumentType")]
        public int? DocumentTypeId { get; set; }
        public DocumentType DocumentType { get; set; }
        public int DocumentNr { get; set; }
        public DateTime DocumentDate { get; set; }

        [ForeignKey("ImprumuturiTipuri")]
        public virtual int? ImprumuturiTipuriId { get; set; }
        public virtual ImprumutTip ImprumuturiTipuri { get; set; }

        [ForeignKey("Currency")]
        public virtual int CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }

        [ForeignKey("Bank")]
        public virtual int? BankId { get; set; }
        public virtual Issuer Bank { get; set; }

        [ForeignKey("LoanAccount")]
        public virtual int? LoanAccountId { get; set; }
        public virtual BankAccount LoanAccount { get; set; }

        [ForeignKey("PaymentAccount")]
        public virtual int? PaymentAccountId { get; set; }
        public virtual BankAccount PaymentAccount { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime StartDate { get; set; }
        public bool isFinalDeLuna { get; set; }
        public int Durata { get; set; }
        public ImprumuturiTipDurata ImprumuturiTipDurata { get; set; }
        public TipCreditare TipCreditare { get; set; }
        public DateTime EndDate { get; set; }

        [ForeignKey("ImprumuturiTermen")]
        public virtual int? ImprumuturiTermenId { get; set; }
        public virtual ImprumutTermen ImprumuturiTermen { get; set; }

        [ForeignKey("DobanziReferinta")]
        public virtual int? DobanziReferintaId { get; set; }
        public virtual DobanziReferinta DobanziReferinta { get; set; }

        [ForeignKey("ActivityType")]
        public virtual int? ActivityTypeId { get; set; }
        public virtual ActivityType ActivityType { get; set; }
        public decimal Suma { get; set; }
        public decimal MarjaFixa { get; set; }
        public decimal ProcentDobanda { get; set; }
        public PerioadaTipDurata PerioadaTipDurata { get; set; }
        public TipRata TipRata { get; set; }
        public TipDobanda TipDobanda { get; set; }
        public int Periodicitate { get; set; }
        public State State { get; set; }
        public int TenantId { get; set; }
        [ForeignKey("Account")]
        public virtual int? ContContabilId { get; set; }
        public virtual Account Account { get; set; }

        [ForeignKey("ThirdParty")]
        public virtual int? ThirdPartyId { get; set; }
        public virtual Person ThirdParty { get; set; }

        public virtual IList<ImprumutState> ImprumutStateList { get; set; }

        public ImprumuturiStare? GetImprumuturiState
        {
            get { return ImprumutStateList?.Where(f => f.ImprumutId == Id).OrderByDescending(f => f.Id).Select(f => f.ImprumuturiStare).First(); }
        }
        public virtual List<Garantie> Garantii { get; set; }

        public virtual List<Rata> Rate { get; set; }
        public virtual List<Tragere> Tragere { get; set; }
        public virtual List<Comision> Comisioane { get; set; }

    }


    public class Comision : Entity<int>, IMustHaveTenant
    {
        [ForeignKey("Imprumut")]
        public virtual int? ImprumutId { get; set; }
        public virtual Imprumut Imprumut { get; set; }
        public TipComision TipComision { get; set; }
        public string Description { get; set; }
        public TipValoareComision TipValoareComision { get; set; }

        public decimal ValoareComision { get; set; }
        public ModCalculComision ModCalculComision { get; set; }
        public TipSumaComision TipSumaComision { get; set; }
        public int BazaDeCalcul { get; set; }

        public State State { get; set; }
        public int TenantId { get; set; }

        public virtual List<DataComision> DateComision { get; set; }
    }
    public class DataComision : Entity<int>, IMustHaveTenant
    {
        [ForeignKey("Comision")]
        public virtual int? ComisionId { get; set; }
        public virtual Comision Comision { get; set; }
        [ForeignKey("Imprumut")]
        public virtual int? ImprumutId { get; set; }
        public virtual Imprumut Imprumut { get; set; }
        [ForeignKey("Tragere")]
        public virtual int? TragereId { get; set; }
        public virtual Tragere Tragere { get; set; }

        [ForeignKey("ContaOperation")]
        public virtual int? ContaOperationId { get; set; }
        public virtual Operation ContaOperation { get; set; }

        [ForeignKey("ContaOperationDetail")]
        public virtual int? ContaOperationDetailId { get; set; }
        public virtual OperationDetails ContaOperationDetail { get; set; }

        public int Index { get; set; }
        public DateTime DataPlataComision { get; set; }
        public decimal SumaComision { get; set; }
        public TipValoareComision TipValoareComision { get; set; }
        public TipSumaComision TipSumaComision { get; set; }

        public decimal ValoareComision { get; set; }
        public State State { get; set; }
        public int TenantId { get; set; }
        public bool IsValid { get; set; }
    }



    public class Garantie : Entity<int>, IMustHaveTenant
    {

        //  public string PartenerGarantie { get; set; }

        public int DocumentNr { get; set; }
        public DateTime DocumentDate { get; set; }
        [ForeignKey("GarantieAccount")]
        public virtual int? GarantieAccountId { get; set; }
        public virtual BankAccount GarantieAccount { get; set; }
        public decimal SumaGarantiei { get; set; }
        [ForeignKey("Currency")]
        public virtual int CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }
        [ForeignKey("LegalPerson")]
        public virtual int LegalPersonId { get; set; }
        public virtual LegalPerson LegalPerson { get; set; } // Partener Garantie
        public string Mentiuni { get; set; }
        [ForeignKey("GarantieCeGaranteaza")]
        public virtual int GarantieCeGaranteazaId { get; set; }
        public virtual GarantieCeGaranteaza GarantieCeGaranteaza { get; set; }
        [ForeignKey("GarantieTip")]
        public virtual int GarantieTipId { get; set; }
        public virtual GarantieTip GarantieTip { get; set; }
        public DateTime StartDateGarantie { get; set; }

        public TipGarantiePrimitaDataEnum TipGarantiePrimitaDataEnum { get; set; }

        public DateTime EndDateGarantie { get; set; }
        public State State { get; set; }
        [ForeignKey("Imprumut")]
        public virtual int? ImprumutId { get; set; }
        public virtual Imprumut Imprumut { get; set; }
        public int TenantId { get; set; }
        public virtual List<OperatieGarantie> OperatiiGarantie { get; set; }
    }

    public class GarantieTip : Entity<int>, IMustHaveTenant
    {
        public string Description { get; set; }

        public State State { get; set; }
        public int TenantId { get; set; }
    }
    public class GarantieCeGaranteaza : Entity<int>, IMustHaveTenant
    {
        public string Description { get; set; }

        public State State { get; set; }
        public int TenantId { get; set; }
    }
    public class Rata : Entity<int>, IMustHaveTenant
    {
        public int Index { get; set; }
        public TipRata TipRata { get; set; }
        public TipDobanda TipDobanda { get; set; }
        public int NumarOrdinDePlata { get; set; }
        public DateTime DataPlataRata { get; set; }
        public decimal SumaPrincipal { get; set; }
        public decimal SumaDobanda { get; set; }
        public decimal SumaPlatita { get; set; }
        public decimal Sold { get; set; }
        public decimal ProcentDobanda { get; set; }
        [ForeignKey("Currency")]
        public virtual int CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }
        public State State { get; set; }
        [ForeignKey("Imprumut")]
        public virtual int? ImprumutId { get; set; }
        public virtual Imprumut Imprumut { get; set; }

        [ForeignKey("ContaOperationDetail")]
        public virtual int? ContaOperationDetailId { get; set; }
        public virtual OperationDetails ContaOperationDetail { get; set; }
        public int TenantId { get; set; }
        public bool IsValid { get; set; }
    }


    public class ImprumutTermen : Entity<int>, IMustHaveTenant
    {
        public string Description { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public State State { get; set; }
        public int TenantId { get; set; }
    }


    public class ImprumutTip : Entity<int>, IMustHaveTenant
    {
        public string Description { get; set; }
        //public string  Account { get; set; }
        public State State { get; set; }
        public int TenantId { get; set; }
    }

    public class ImprumutTipDetaliu : Entity<int>, IMustHaveTenant
    {
        public ImprumuturiTipDetaliuEnum Description { get; set; }

        public virtual string ContImprumut { get; set; }

        [ForeignKey("ActivityType")]
        public int ActivityTypeId { get; set; }
        public virtual ActivityType ActivityType { get; set; }

        [ForeignKey("ImprumutTip")]
        public int ImprumutTipId { get; set; }
        public virtual ImprumutTip ImprumutTip { get; set; }
        public int TenantId { get; set; }
    }

    public class Tragere : Entity<int>, IMustHaveTenant
    {
        public TipTragere TipTragere { get; set; }
        public DateTime DataTragere { get; set; }
        public decimal SumaDisponibila { get; set; } // suma neutilizata
        public decimal SumaTrasa { get; set; } // Suma
        public decimal SumaImprumutata { get; set; }
        public decimal Dobanda { get; set; }

        public decimal Comision { get; set; }

        [ForeignKey("Currency")]
        public virtual int CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }
        public State State { get; set; }
        [ForeignKey("Imprumut")]
        public virtual int? ImprumutId { get; set; }
        public virtual Imprumut Imprumut { get; set; }
        public int TenantId { get; set; }

        public virtual List<DataComision> Comisions { get; set; }
    }

    public class ImprumutState : Entity<int>, IMustHaveTenant
    {
        [ForeignKey("ContaOperationDetail")]
        public virtual int? ContaOperationDetailId { get; set; }
        public virtual OperationDetails ContaOperationDetail { get; set; }

        public ImprumuturiStare ImprumuturiStare { get; set; }
        public String Comentariu { get; set; }
        public DateTime OperationDate { get; set; }

        [ForeignKey("Imprumut")]
        public virtual int? ImprumutId { get; set; }
        public virtual Imprumut Imprumut { get; set; }
        public int TenantId { get; set; }
    }

    public class OperatieDobandaComision : Entity<int>, IMustHaveTenant
    {
        [ForeignKey("Imprumut")]
        public virtual int? ImprumutId { get; set; }
        public virtual Imprumut Imprumut { get; set; }

        [ForeignKey("OperGenerate")]
        public virtual int? OperGenerateId { get; set; }
        public virtual OperGenerate OperGenerate { get; set; }

        [ForeignKey("Operation")]
        public virtual int? OperationId { get; set; }
        public virtual Operation Operation { get; set; }

        public TipOperatieDobandaComision TipOperatieDobandaComision { get; set; }

        public DateTime DataStart { get; set; }
        public DateTime DataEnd { get; set; }

        public State State { get; set; }
        [ForeignKey("Rata")]
        public virtual int? RataId { get; set; }
        public virtual Rata Rata { get; set; }
        public decimal? Suma{ get; set; }
        

        public int TenantId { get; set; }
    }

    public class ComisionV2 : Entity<int>, IMustHaveTenant
    {
        [ForeignKey("Imprumut")]
        public virtual int? ImprumutId { get; set; }
        public virtual Imprumut Imprumut { get; set; }
        public TipComisionV2 TipComision { get; set; }
        public string Descriere { get; set; }
        public TipValoareComision TipValoareComision { get; set; }

        public decimal ValoareComision { get; set; }
        public decimal ValoareCalculata { get; set; }
        public ModCalculComision ModCalculComision { get; set; }
        public TipSumaComision TipSumaComision { get; set; }
        public int BazaDeCalcul { get; set; }
        public DateTime DataStart { get; set; }
        public DateTime DataEnd { get; set; }
        public int NrLuni { get; set; }
        public State State { get; set; }
        public int TenantId { get; set; }


    }

   
    public class OperatieGarantie : Entity<int>, IMustHaveTenant
    {
        [ForeignKey("Garantie")]
        public virtual int GarantieId { get; set; }
        public virtual Garantie Garantie { get; set; }

        [ForeignKey("ContaOperation")]
        public virtual int? ContaOperationId { get; set; }
        public virtual Operation ContaOperation { get; set; }

        [ForeignKey("ContaOperationDetail")]
        public virtual int? ContaOperationDetailId { get; set; }
        public virtual OperationDetails ContaOperationDetail { get; set; }
        public decimal Suma { get; set; }
        public decimal Sold { get; set; }
        public TipOperatieGarantieEnum TipOperatieGarantieEnum { get; set; }
        public DateTime DataOperatiei { get; set; }
        public State State { get; set; }
        public int TenantId { get; set; }

    }


    //public enum ImprumuturiTip
    //{
    //    ImprumuturiPrimiteDeLaInstitutiiDeCredit,
    //    ImprumuturiPrimiteDeLaSocietatiFinanciare,
    //    ImprumuturiPrimiteDeLaAlteInstitutii
    //}
    public enum ImprumuturiStare
    {
        Inregistrat,
        Acordat,
        Anulat,
        Lichidat,

    }
    
    public enum ImprumuturiTipDurata
    {
        Luni,
        Zile
    }
    public enum PerioadaTipDurata
    {
        Luni,
        Zile
    }

    public enum TipRata
    {
        RataTotalaEgala,
        RateDescrescatoare
    }

    public enum TipDobanda
    {
        Fixa,
        Variabila
    }
    public enum TipComision
    {
        Acordare,
        Administrare,
        Neutilizare,
        Altele
    }

    public enum TipComisionV2
    {
        cheltuialaIntegrala,
        cheltuialaInAvans,
        Periodic,
     
    }
    public enum TipValoareComision
    {
        Procent,
        ValoareFixa,
    }
    public enum ModCalculComision
    {
        
        LaAcordare,
        Lunar,
        Trimestrial,
        Semestrial,
        Anual,
        Tragere

    }
    public enum TipSumaComision
    {
        ValoareImprumut,
        Sold,
        SumaTrasa
    }

    public enum TipCreditare
    {
        Credit,
        LinieDeCredit,
        
    }

    public enum TipTragere
    {
        Acordare,
        Tragere,
        Dobanda,
        Rambursare
    }

    public enum TipOperatieDobandaComision
    {
        comision,
        dobanda
    }

    public enum ImprumuturiTipDetaliuEnum: int
    {
        [Description("Cont imprumut")]
        CI,
        [Description("Angajamente primite")]
        AP,
        [Description("Datorii atasate")]
        DA,
        [Description("Fondul de despagubire")]
        FDC,
        [Description("Comisioane Inregistrate")]
        CMI,
        [Description("Comisioane Platite Periodic")]
        CPP,
        [Description("Comisioane Platite Lunar")]
        CPL,
        [Description("Gajuri fără deposedare primite")]
        GFDP,
        [Description("Gajuri cu deposedare primite")]
        GCDP,
        [Description("Garantii financiare primite ")]
        GFP,
        [Description("Gajuri fără deposedare date")]
        GFDD,
        [Description("Gajuri cu deposedare date")]
        GCDD,
        [Description("Garantii financiare date ")]
        GFD,
    }

    public enum TipGarantiePrimitaDataEnum: int
    {
        Primita,
        Data
    }

    public enum TipOperatieGarantieEnum : int
    {
        Majorare,
        Diminuare
    }
}
