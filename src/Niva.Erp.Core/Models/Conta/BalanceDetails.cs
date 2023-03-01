﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
namespace Niva.Erp.Models.Conta
{
    using Abp.Domain.Entities;
  
    using Niva.Erp.Models.Conta;
    using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
	using System.Text;

	public class BalanceDetails : Entity<int>, IMustHaveTenant
    {
        [ForeignKey("Account")]
        public int AccountId { get; set; }
        public virtual Account Account { get; set; }

        public virtual decimal CrValueI { get; set; }

        public virtual decimal DbValueI { get; set; }

        public virtual decimal CrValueM { get; set; }

        public virtual decimal DbValueM { get; set; }

        public virtual decimal CrValueY { get; set; }

        public virtual decimal DbValueY { get; set; }

        public virtual decimal CrValueF { get; set; }

        public virtual decimal DbValueF { get; set; }

        [ForeignKey("Currency")]
        public int CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }

      

        [ForeignKey("Balance")]
        public int BalanceId { get; set; }
        public virtual Balance Balance { get; set; }
        public int TenantId { get; set; }
    }
}

