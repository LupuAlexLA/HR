using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Emitenti;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.Conta
{
    public partial class BankAccount : AuditedEntity<int>, IMustHaveTenant
    {
        [StringLength(1000)]
        public virtual string IBAN { get; set; }

        [ForeignKey("Currency")]
        public virtual int CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }

        [ForeignKey("Bank")]
        public virtual int BankId { get; set; }
        public virtual Issuer Bank { get; set; }

        [ForeignKey("Person")]
        public virtual int PersonId { get; set; }
        public virtual Person Person { get; set; }

        public int TenantId { get; set; }
        
    }
}