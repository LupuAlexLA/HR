
namespace Niva.Erp.Models.Contracts
{
    using Abp.Domain.Entities;
    using Abp.Domain.Entities.Auditing;
    using Niva.Erp.Models.Conta;
    using Niva.Erp.Models.Conta.Enums;
    using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
	using System.Text;

	public partial class InvoiceElementAccounts : AuditedEntity<int>, IMustHaveTenant
    {
		public virtual InvoiceElementAccountType InvoiceElementAccountType { get; set; }

        [ForeignKey("Account")]
        public int AccountId { get; set; }
        public virtual Account Account { get; set; }

       

        public virtual State State { get; set; }
      
        public int TenantId { get; set; }
    }
}

