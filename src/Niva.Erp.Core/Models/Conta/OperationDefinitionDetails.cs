
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.Conta
{
	 

	public class OperationDefinitionDetails : AuditedEntity<int>, IMustHaveTenant
	{ 

		public virtual string Observations { get; set; }

		[ForeignKey("Debit")]
		public int DebitId { get; set; }
		public virtual Account Debit { get; set; }

		[ForeignKey("Credit")]
		public int CreditId { get; set; }
		public virtual Account Credit { get; set; }
		public int TenantId { get; set; }

		[ForeignKey("OperationDefinition")]
		public int OperationDefinitionId { get; set; }
		public virtual OperationDefinition OperationDefinition { get; set; }
	}
}

