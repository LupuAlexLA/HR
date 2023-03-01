 
namespace Niva.Erp.Models.Contracts
{
 
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.Linq;
	using System.Text;
    using Abp.Domain.Entities;
    using Abp.Domain.Entities.Auditing;
    using Niva.Erp.Models.Conta;

    public class ContractsInstalments:AuditedEntity<int>, IMustHaveTenant
	{
		 
		public virtual Decimal Value
		{
			get;
			set;
		}

		public virtual Decimal VAT
		{
			get;
			set;
		}

		public virtual DateTime InstalmentDate
		{
			get;
			set;
		}

		public virtual Currency Currency
		{
			get;
			set;
		}

		public virtual Contracts Contracts
		{
			get;
			set;
		}
        public int TenantId { get; set; }
    }
}

