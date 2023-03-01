 
namespace Niva.Erp.Models.Contracts
{
    using Abp.Domain.Entities;
    using Abp.Domain.Entities.Auditing;
    using Abp.EntityHistory;
    using Niva.Erp.Models.Conta.Enums;
    using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.Linq;
	using System.Text;

	public class ContractsCategory : AuditedEntity<int>, IMustHaveTenant
	{
		 

		public virtual string CategoryName
		{
			get;
			set;
		}

		public virtual State State
		{
			get;
			set;
		}

	 //use IAudited Fields !!
        //public virtual DateTime DateModify
        //{
        //    get;
        //    set;
        //}

        //public virtual User UserModify
        //{
        //    get;
        //    set;
        //}
        public int TenantId { get; set; }

        public virtual Contracts Contract { get; set; }

    }
}

