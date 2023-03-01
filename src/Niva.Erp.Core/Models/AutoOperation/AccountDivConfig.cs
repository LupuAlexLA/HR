using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niva.Erp.Models.AutoOperation
{
    public class AccountDivConfig : AuditedEntity<int> ,IMustHaveTenant
    {
        public AccountDivConfigType? AccountType { get; set; }

        public AccountDivConfigPersType? PersType { get; set; }

        public AccountDivConfigResidenceType? ResidenceType { get; set; }

        public int DivYear { get; set; }

        [ForeignKey("Account")]
        public int AccountId { get; set; }
        public Account Account { get; set; }

       

      
        public int TenantId { get; set; }
    }

    public enum AccountDivConfigPersType
    {
        Fizic,
        Juridic
    }

    public enum AccountDivConfigResidenceType
    {
        Rezident,
        Nerezident
    }

    public enum AccountDivConfigType
    {
        [Description("Cont dividend")]
        ContDividend,
        [Description("Cont creanta")]
        ContCreanta
    }
}
