using System.Collections.Generic;
using Abp.Domain.Entities;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Contracts;

namespace Niva.Erp.Models.Conta
{
	public class Currency : Entity<int>
	{
		public virtual string CurrencyCode
		{
			get;
			set;
		}

		public virtual string CurrencyName
		{
			get;
			set;
		}

		public virtual State Status
		{
			get;
			set;
		}
		public virtual List<BankAccount> BankAccounts { get; set; }
		public virtual List<Invoices> Invoice { get; set; }

	}
}