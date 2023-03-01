 
namespace Niva.Erp.Models.Conta
{
    using Abp.Domain.Entities;
    using Abp.Domain.Entities.Auditing;
    using Niva.Erp.Models.Conta.Enums;
    using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
	using System.Text;

	public class Balance :  AuditedEntity<int> , IMustHaveTenant
    {
		public virtual DateTime BalanceDate { get; set; }

        public virtual DateTime StartDate { get; set; }

        public bool OkValid { get; set; }

        public virtual State Status { get; set; }

        public virtual IList<SavedBalance> SavedBalance { get; set; }

        public virtual IList<BalanceDetails> BalanceDetails { get; set; }
        public int TenantId { get; set; }
    }
}

