using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Niva.Erp.Models.AutoOperation
{
    public class OperGenerate : AuditedEntity<int>, IMustHaveTenant
    {
        public DateTime DataOperatie { get; set; }
        
        [ForeignKey("TipOperatie")]
        public int TipOperatieId { get; set; }
        public OperGenerateTipuri TipOperatie { get; set; }
        public State State { get; set; }
        public int TenantId { get; set; }
    }
}
