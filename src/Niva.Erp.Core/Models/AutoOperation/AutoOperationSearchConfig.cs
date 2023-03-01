using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.AutoOperation
{
    public class AutoOperationSearchConfig : AuditedEntity<int>, IMustHaveTenant
    {
        public AutoOperationType AutoOperType { get; set; }

        public int OperationType { get; set; }

        [ForeignKey("DocumentType")]
        public int? DocumentTypeId { get; set; }
        public virtual DocumentType DocumentType { get; set; }
        public State State { get; set; }
        public int TenantId { get; set; }
    }
}
