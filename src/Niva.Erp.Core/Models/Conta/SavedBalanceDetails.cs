 
namespace Niva.Erp.Models.Conta
{
    using Abp.Domain.Entities;
 
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
	using System.Text;

	public class SavedBalanceDetails : Entity<int>
    {
        [ForeignKey("Account")]
        public int AccountId { get; set; }
        public virtual Account Account { get; set; }

        public virtual decimal CrValueF { get; set; }

        public virtual decimal CrValueI { get; set; }

        public virtual decimal CrValueM { get; set; }

        public virtual decimal CrValueY { get; set; }

        public virtual decimal DbValueF { get; set; }

        public virtual decimal DbValueI { get; set; }

        public virtual decimal DbValueM { get; set; }

        public virtual decimal DbValueY { get; set; }

        [ForeignKey("SavedBalance")]
        public int SavedBalanceId { get; set; }
        public virtual SavedBalance SavedBalance { get; set; }

        [ForeignKey("Currency")]
        public int CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual Decimal ExhangeRate { get; set; }

    }
}

