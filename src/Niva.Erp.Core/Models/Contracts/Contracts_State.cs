using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Niva.Erp.Models.Contracts
{
    public class Contracts_State : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("Contracts")]
        public int ContractsId { get; set; }
        public virtual Contracts Contracts { get; set; }
        public DateTime OperationDate { get; set; }
        public string Comentarii { get; set; }
        public Contract_State Contract_State { get; set; }
        public State State { get; set; }
        public int TenantId { get; set; }

    }
}
