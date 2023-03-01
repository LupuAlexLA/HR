using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.AutoOperation
{
    public class AutoOperationConfig : AuditedEntity<int> , IMustHaveTenant
    {
        public AutoOperationType AutoOperType { get; set; }

        public int OperationType { get; set; }

        public int ValueSign { get; set; }

        public int ElementId { get; set; }

        [StringLength(1000)]
        public string DebitAccount { get; set; }

        [StringLength(1000)]
        public string CreditAccount { get; set; }

        [StringLength(1000)]
        public string Details { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int EntryOrder { get; set; }

        public State State { get; set; }

        public bool IndividualOperation { get; set; }

        public bool UnreceiveInvoice { get; set; }

        public int TenantId { get; set; } 
    }

    public enum AutoOperationType
    {
        [Description("Mijloace fixe")]
        MijloaceFixe,
        [Description("Obiecte de inventar")]
        ObiecteDeInventar,
        [Description("Cheltuieli in avans")]
        CheltuieliInAvans,
        [Description("Venituri in avans")]
        VenituriInAvans,
        [Description("Impozit pe profit")]
        ImpozitPeProfit,
        [Description("Titluri de plasament AFS")]
        TitluriDePlasamentAFS,
        [Description("Obligatiuni AFS")]
        ObligatiuniAFS,
        [Description("Titluri de plasament HTML")]
        TitluriDePlasamentHTML,
        [Description("Obligatiuni HTML")]
        ObligatiuniHTML,
        [Description("Depozite bancare")]
        DepoziteBancare,
        [Description("Repo Depo date in pensiune livrata")]
        RepoDepoDateInPensiuneLivrata,
        [Description("Repo Depo primite in pensiune livrata")]
        RepoDepoPrimiteInPensiuneLivrata,
        [Description("Certificate depozit")]
        CertificateDepozit,
        Contributii,
        [Description("Ajustari depreciere plasamente")]
        AjustariDeprecierePlasamente,
        Reclasificari,
        Imprumuturi,
        Dobanda
    }

    public enum ImoAssetStockElement
    {
        [Description("Valoare inventar")]
        ValoareInventar,
        Amortizare,
        [Description("Valoare vanzare")]
        ValoareVanzare
    }

    public enum InvObjectStockElement
    {
        Valoare,
        [Description("Valoare descarcata")]
        ValoareDescarcata
    }

    public enum PrepaymentStockElement
    {
        Valoare,
        Amortizare,
        [Description("Valoare TVA")]
        ValoareTVA,
        [Description("Amortizare TVA")]
        AmortizareTVA
    }

    public enum TaxProfitElement
    {
        [Description("Impozit pe profit declarat")]
        ImpozitPeProfitDeclarat,
        [Description("Impozit pe profit datorat")]
        ImpozitPeProfitDatorat
    }

    public enum ValueSign
    {
        Pozitiv = 1,
        Negativ = -1
    }

    public enum TitluriDePlasamentOperType
    {
        Achizitie,
        [Description("Inreg venit lunar")]
        InregVenitLunar,
        [Description("Ajustare nominal")]
        AjustareNominal,
        Scadenta,
        [Description("Vanzare plasament")]
        VanzarePlasament,
        [Description("Profit pierdere tranzactie")]
        ProfitPierdereTranzactie
    }

    public enum TitluriDePlasamentElement
    {
        [Description("Valoare achizitie")]
        ValoareAchizitie,
        [Description("Principal cupon scurs")]
        PrincipalCuponScurs,
        [Description("Venit luna curenta")]
        VenitLunaCurenta,
        [Description("Diferenta venit Calculat si randament contractat")]
        DifVenitCalcSiRandamentContractat,
        Principal,
        Cupon,
        [Description("Creanta acumulata")]
        CreantaAcumulata,
        [Description("Diferenta valoare incasata si principal creanta")]
        DifValIncasataSiPrincipalCreanta,
        [Description("Valoare tranzactie")]
        ValoareTranzactie,
        [Description("Valoare achizitie decontare")]
        ValoareAchizitieDecont
    }

    public enum DepoziteBancareOperType
    {
        Constituire,
        [Description("Inregistrare venit lunar")]
        InregVenitLunar,
        Scadenta,
        [Description("Lichidare inainte scadenta recalcul dobanda")]
        LichidareInainteScadentaRecalcDobanda,
        [Description("Lichidare inainte scadenta mentinere dobanda")]
        LichidareInainteScadentaMentinereDobanda
    }

    public enum DepoziteBancareElement
    {
        [Description("Valoare depozit")]
        ValoareDepozit,
        [Description("Dobanda lunara")]
        DobandaLunara,
        Principal,
        Dobanda,
        [Description("Venit cumulat inregistrat")]
        VenitCumulatInregistrat
    }

    public enum RepoDepoDateInPensiuneLivrataOperType
    {
        [Description("Primire imprumut")]
        PrimireImprumut,
        [Description("Inregistrare dobanda")]
        InregistrareDobanda,
        Rambursare
    }

    public enum RepoDepoDateInPensiuneLivrataElement
    {
        [Description("Suma primita")]
        SumaPrimita,
        [Description("Dobanda datorata FGDB")]
        DobandaDatorataFGDB,
        [Description("Dobanda datorata FRB")]
        DobandaDatorataFRB,
        Principal,
        Dobanda
    }

    public enum RepoDepoPrimiteInPensiuneLivrataOperType
    {
        [Description("Acordare credit")]
        AcordareCredit,
        [Description("Inregistrare dobanda")]
        InregistrareDobanda,
        Rambursare,
        Restanta
    }

    public enum RepoDepoPrimiteInPensiuneLivrataElement
    {
        [Description("Suma tranzactie")]
        SumaTranzactie,
        Principal,
        Dobanda
    }

    public enum CertificateDepozitOperType
    {
        Constituire,
        [Description("Inregistrare dobanda")]
        InregistrareDobanda,
        Scadenta
    }

    public enum CertificateDepozitElement
    {
        [Description("Suma tranzactie")]
        SumaTranzactie,
        Principal,
        Dobanda
    }

    public enum ContributiiOperType
    {
        [Description("Contributii anuale")]
        ContributiiAnuale,
        [Description("Contributii speciale")]
        ContributiiSpeciale
    }
    
    public enum ContributiiElement
    {
        Contributie
    }

    public enum AjustariDeprecierePlasamenteOperType
    {
        [Description("Ajustari depreciere plasamente")]
        AjustariDeprecierePlasamente
    }

    public enum AjustariDeprecierePlasamenteElement
    {
        [Description("Diferente pozitive reevaluare")]
        DiferentePozitiveReevaluare,
        [Description("Diferente negative reevaluare")]
        DiferenteNegativeReevaluare
    }

    public enum ReclasificariOperType
    {
        [Description("Titluri plasament AFS in HTM")]
        TitluriPlasamentAFSinHTM,
        [Description("Obligatiuni AFS in HTM")]
        ObligatiuniAFSinHTM,
        [Description("Titluri plasament HTM in AFS")]
        TitluriPlasamentHTMinAFS,
        [Description("Obligatiuni HTM in AFS")]
        ObligatiuniHTMinAFS
    }

    public enum ReclasificariElement
    {
        Principal,
        Dobanda,
        Provizion
    }

    public enum ImprumuturiOperType
    {
        [Description("Inregistrarea angajamentului de finantare")]
        InregistrareaAngajamentuluiDeFinantare,
        
        [Description("Primirea imprumuturilor")]
        PrimireaImprumuturilor,
        [Description("Diminuarea/Inchiderea angajamentului")]
        DiminuareaAngajamentului,
        [Description("Inregistrare dobanda datorata")]
        InregistrareDobandaDatorata,
        [Description("Plata dobanda datorata")]
        PlataDobandaDatorata,
        [Description("Plata comision periodic")]
        PlataComisionPeriodic,
        [Description("Plata comision lunar")]
        PlataComisionLunar,
        [Description("Majorare gaj fara deposedare primit")]
        MajorareGajFaraDeposedarePrimit,
        [Description("Diminuare gaj fara deposedare primit")]
        DiminuareGajFaraDeposedarePrimit,
        [Description("Majorare gaj cu deposedare primit")]
        MajorareGajCuDeposedarePrimit,
        [Description("Diminuare gaj cu deposedare primit")]
        DiminuareGajCuDeposedarePrimit,
        [Description("Majorare scrisoare de garantie primita")]
        MajorareScrisoareDeGarantiePrimita,
        [Description("Diminuare scrisoare de garantie  primit")]
        DiminuareScrisoareDeGarantiePrimita,
        [Description("Majorare gaj fara deposedare primit")]
        MajorareGajFaraDeposedareDat,
        [Description("Diminuare gaj fara deposedare primit")]
        DiminuareGajFaraDeposedareDat,
        [Description("Majorare gaj cu deposedare primit")]
        MajorareGajCuDeposedareDat,
        [Description("Diminuare gaj cu deposedare primit")]
        DiminuareGajCuDeposedareDat,
        [Description("Majorare scrisoare de garantie primita")]
        MajorareScrisoareDeGarantieData,
        [Description("Diminuare scrisoare de garantie  primit")]
        DiminuareScrisoareDeGarantieData,
    }

    public enum ImprumuturiElement
    {
        Principal,
        Dobanda,
        Comision
    }
}
