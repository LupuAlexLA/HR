using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Contracts;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.Buget
{
    public class BVC_Formular : AuditedEntity<int>, IMustHaveTenant
    {
        public int AnBVC { get; set; }
        public State State { get; set; }
        public int TenantId { get; set; }
        public List<BVC_FormRand> FormRanduri { get; set; }
    }

    public class BVC_FormRand : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("Formular")]
        public int FormularId { get; set; }
        [Required]
        public virtual BVC_Formular Formular { get; set; }
        public int CodRand { get; set; }
        public string Descriere { get; set; }
        public BVC_RowType? TipRand { get; set; }
        public int OrderView { get; set; }

        [ForeignKey("RandParent")]
        public int? RandParentId { get; set; }
        public virtual BVC_FormRand RandParent { get; set; }
        public int? RandParentIdFromUI { get; set; }
        public int? NivelRand { get; set; }
        public List<BVC_FormRand> RanduriChild { get; set; }
        public bool IsTotal { get; set; }
        public bool Bold { get; set; }
        public string FormulaBVC { get; set; }
        public string FormulaCashFlow { get; set; }
        public virtual BVC_RowTypeIncome? TipRandVenit { get; set; }
        public virtual BVC_RowTypeSalarizare? TipRandSalarizare { get; set; }

        [ForeignKey("TipRandCheltuiala")]
        public int? TipRandCheltuialaId { get; set; }
        public virtual InvoiceElementsDetails TipRandCheltuiala { get; set; }
        public bool AvailableBVC { get; set; }
        public bool AvailableCashFlow { get; set; }
        public bool BalIsTotal { get; set; }
        public string BalFormulaBVC { get; set; }
        public string BalFormulaCashFlow { get; set; }
        public virtual IList<BVC_FormRandDetails> DetaliiRand { get; set; }

        public int TenantId { get; set; }

        public virtual bool LastRowLevel
        {
            get
            {
                return (RanduriChild.Count == 0);
            }
        }
    }

    public class BVC_FormRandDetails : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("FormRand")]
        public int FormRandId { get; set; }
        [Required]
        public virtual BVC_FormRand FormRand { get; set; }
        [ForeignKey("TipRandCheltuiala")]
        public int? TipRandCheltuialaId { get; set; }
        public virtual InvoiceElementsDetails TipRandCheltuiala { get; set; }
        public int TenantId { get; set; }
    }

    public enum BVC_RowType
    {
        Venituri,
        Cheltuieli,
        Investitii,
        Rezultat,
        Amortizari,
        Salarizare
    }

    public enum BVC_RowTypeIncome
    {
        Dobanzi = 0,
        [Description("Diferenta curs valutar")]
        DiferentaCursValutar = 1,
        Contributii,
        Creante,
        Altele,
        SumeReinvestite,
        IncasariDobanzi
    }

    public enum BVC_RowTypeSalarizare
    {
        [Description("Indemnizatii CS")]
        IndemnizatiiCS,
        [Description("Fond salarii")]
        FondSalarii,
        Contributii,
        [Description("Fond social")]
        FondSocial,
        Diverse,
        Salarii,
        [Description("Prima CO")]
        PrimaCO,
        [Description("Prima Ziua Fondului")]
        PrimaZiuaFondului,
        [Description("Cadou 1 Iunie")]
        Cadou1Iunie,
        [Description("Cadou 8 Martie")]
        Cadou8Martie,
        [Description("Cadou Craciun")]
        CadouCraciun,
        [Description("Tinute vestimentare")]
        TinuteVestimentare,
        [Description("Fond profit")]
        FondProfit,
        [Description("Contibutii si CS")]
        ContibutiiSiCS
    }
}
