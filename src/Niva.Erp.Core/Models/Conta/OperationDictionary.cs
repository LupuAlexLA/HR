﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
namespace Niva.Erp.Models.Conta
{
	 
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.Linq;
	using System.Text;
    using Abp.Domain.Entities;
    using Abp.Domain.Entities.Auditing;

    public class OperationDictionary : AuditedEntity,IMustHaveTenant
	{
		 

		public virtual string Expression
		{
			get;
			set;
		}

		public virtual string Name
		{
			get;
			set;
		}
 
        public int TenantId { get; set; }
    }
}
