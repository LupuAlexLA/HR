using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.SectoareBnr
{
    public class BNR_Anexa : AuditedEntity<int>, IMustHaveTenant
    {
        public string Denumire { get; set; }
        public int TenantId { get; set; }

    }

    public class BNR_AnexaDetail : AuditedEntity<int>, IMustHaveTenant
    {
        public string NrCrt { get; set; }
        public string DenumireRand { get; set; }
        public string CodRand { get; set; } 
        public bool EDinConta { get; set; }
        public string FormulaConta { get; set; }
        public string FormulaTotal { get; set; }
        public string TipInstrument { get; set; }
        public int? DurataMinima { get; set; }
        public int? DurataMaxima { get; set; }
        public bool Sectorizare { get; set; }
        public string TipTitlu { get; set; }
        public string FormulaCresteri { get; set; }
        public string FormulaReduceri { get; set; } 
        public int OrderView { get; set; }
        public virtual State State
        {
            get;
            set;
        }
        public int TenantId { get; set; }

        [ForeignKey("BNR_Anexa")]
        public int? AnexaId { get; set; }
        public virtual BNR_Anexa BNR_Anexa { get; set; }

    }
}
