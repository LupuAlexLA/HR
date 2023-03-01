using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.Deconturi
{
    public enum DiurnaType : int
    {
        Interna,
        Externa
    };

    public enum ScopDeplasareType : int
    {
        Deplasare,
        Lichidare,
        [Description("Alte deconturi")]
        AlteDeconturi
    }

    public class Diurna : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("Country")]
        public int CountryId { get; set; }
        public Country Country { get; set; }

        [ForeignKey("Currency")]
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }

        public int Value { get; set; }

        public DateTime DataValabilitate { get; set; }

        public virtual DiurnaType DiurnaType { get; set; }
        public virtual ScopDeplasareType ScopDeplasareType { get; set; }

        public State State { get; set; }
        public int TenantId { get; set; }
    }

    public partial class DiurnaLegala : Diurna { }

    public partial class DiurnaZi : Diurna { }
}
