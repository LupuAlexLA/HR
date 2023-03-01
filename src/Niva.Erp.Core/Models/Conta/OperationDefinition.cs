﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
namespace Niva.Erp.Models.Conta
{
	using Abp.Domain.Entities;
	using Niva.Erp.Models.Conta.Enums;
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations.Schema;

	public class OperationDefinition : Entity<int>, IMustHaveTenant
	{
		public virtual string Name { get; set; }

		public virtual State Status { get; set; }

		[ForeignKey("DocumentType")]
		public int DocumentTypeId { get; set; }
		public virtual DocumentType DocumentType { get; set; }

		[ForeignKey("Currency")]
		public int CurrencyId { get; set; }
		public virtual Currency Currency { get; set; }

		public virtual IList<OperationDefinitionDetails> OperationDefinitionDetails { get; set; }
		public int TenantId { get; set; }
	}
}

