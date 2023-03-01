using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.Buget
{

    public enum BVC_PlasamentType
    {
        [Description("Depozite bancare")]
        DepoziteBancare,
        [Description("Titluri de plasament")]
        TitluriDePlasament,
        Obligatiuni
    }

    public class BVC_DobandaReferinta : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("Formular")]
        public int FormularId { get; set; }
        public virtual BVC_Formular Formular { get; set; }
        public BVC_PlasamentType? PlasamentType { get; set; }    
        public decimal Procent { get; set; }
        public DateTime DataStart { get; set; }
        public DateTime DataEnd { get; set; }
        public State State { get; set; }
        public int TenantId { get; set; }

        [ForeignKey("Currency")]
        public int? CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }

        [ForeignKey("ActivityType")]
        public int? ActivityTypeId { get; set; }
        public virtual ActivityType ActivityType { get; set; }
    }
}
