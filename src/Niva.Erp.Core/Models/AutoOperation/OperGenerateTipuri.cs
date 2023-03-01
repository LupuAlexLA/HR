using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Niva.Erp.Models.AutoOperation
{
    public class OperGenerateTipuri : AuditedEntity<int>, IMustHaveTenant
    {
        public string Tip { get; set; }
        public string Descriere { get; set; }
        public bool SfarsitLuna { get; set; } 

        [ForeignKey("Categ")]
        public int CategId { get; set; }
        public OperGenerateCateg Categ { get; set; }
        public State State { get; set; }
        public int ExecOrder { get; set; }
        public int TenantId { get; set; }
    }

    public class OperGenerateCateg : AuditedEntity<int>, IMustHaveTenant
    {
        public string CategTypeShort { get; set; }
        public string CategType { get; set; }
        public State State { get; set; }
        public int TenantId { get; set; }
    }
}
