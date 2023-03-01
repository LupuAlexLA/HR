using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niva.Erp.Models.AutoOperation
{
    public class AutoOperationOper : AuditedEntity<int>, IMustHaveTenant
    {
        public AutoOperationType AutoOperType { get; set; }

        public int OperationType { get; set; }

        public DateTime OperationDate { get; set; }

        [ForeignKey("DocumentType")]
        public int DocumentTypeId { get; set; }
        public virtual DocumentType DocumentType { get; set; }

        public string DocumentNumber { get; set; }

        public DateTime DocumentDate { get; set; }

        [ForeignKey("Currency")]
        public int CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }

        public bool Validated { get; set; }

        public State State { get; set; }

        public List<AutoOperationDetail> OperationDetails { get; set; }
 
        public int TenantId { get; set; }
    }

    public class AutoOperationDetail : AuditedEntity<int>, IMustHaveTenant
    {
        [ForeignKey("AutoOper")]
        public int AutoOperId { get; set; }
        public AutoOperationOper AutoOper { get; set; }

        [ForeignKey("DebitAccount")]
        public int DebitAccountId { get; set; }
        public Account DebitAccount { get; set; }

        [ForeignKey("CreditAccount")]
        public int CreditAccountId { get; set; }
        public Account CreditAccount { get; set; }

        public virtual decimal Value { get; set; }

        public virtual decimal ValueCurr { get; set; }

        public virtual string Details { get; set; }

        public virtual int OperationalId { get; set; }

        [ForeignKey("OperationDetail")]
        public int? OperationDetailId { get; set; }
        public OperationDetails OperationDetail { get; set; }

        
        public int TenantId { get; set; }
    }
}
