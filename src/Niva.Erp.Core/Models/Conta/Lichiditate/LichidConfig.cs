using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Niva.Erp.Models.Conta.Lichiditate
{
    public class LichidConfig : AuditedEntity<int>, IMustHaveTenant
    {
        public string DenumireRand { get; set; }
        public string CodRand { get; set; }
        public bool EDinConta { get; set; }
        public string FormulaConta { get; set; }
        public string FormulaTotal { get; set; }
        public string TipInstrument { get; set; }
        public int OrderView { get; set; }

        [ForeignKey("LichidBenzi")]
        public int? LichidBenziId { get; set; }
        public virtual LichidBenzi LichidBenzi { get; set; }
        public virtual State State { get; set; }
        public int TenantId { get; set; }
    }
}
