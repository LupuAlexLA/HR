using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Contracts;
using Niva.Erp.Models.SectoareBnr;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Niva.Erp.Models.Conta
{
    public partial class Account : AuditedEntity<int>, IMustHaveTenant
    {
        public virtual string Symbol { get; set; }

        [ForeignKey("SyntheticAccount")]
        public int? SyntheticAccountId { get; set; }
        public virtual Account SyntheticAccount { get; set; }
        public virtual List<Account> AnalyticAccounts { get; set; }

        [Required]
        public virtual string AccountName { get; set; }
        public virtual string ExternalCode { get; set; }
        [ForeignKey("Currency")]
        public int CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }

        [ForeignKey("ActivityType")]
        public int? ActivityTypeId { get; set; }
        public virtual ActivityType ActivityType { get; set; }
        [ForeignKey("ThirdParty")]
        public virtual int? ThirdPartyId { get; set; }
        public virtual Person ThirdParty { get; set; }
        public virtual State Status { get; set; }

        public virtual AccountTypes AccountTypes { get; set; }

        public virtual AccountFuncType AccountFuncType { get; set; }

        public bool ComputingAccount { get; set; } // e disponibil pentru folosirea in operatii contabile, etc. (e analitic sau sintetic fara analitic)

        public bool AccountStatus { get; set; } // cont activ sau inactiv

        public TaxStatus TaxStatus { get; set; }

        [ForeignKey("BankAccount")]
        public virtual int? BankAccountId { get; set; }
        public virtual BankAccount BankAccount { get; set; } 

        public virtual bool IsSynthethic
        {
            get
            {
                if ((SyntheticAccountId == null && AnalyticAccounts.Count == 0) || (AnalyticAccounts.Count != 0))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public int TenantId { get; set; }

        [ForeignKey("BNR_Sector")]
        public int? SectorBnrId { get; set; }
        public virtual BNR_Sector BNR_Sector { get; set; }

        public int? NivelRand { get; set; }
        public DateTime DataValabilitate { get; set; }
    }

    public partial class AccountHistory : AuditedEntity<int>, IMustHaveTenant
    {
        public int AccountId { get; set; }
        public virtual string Symbol { get; set; }
        public int? SyntheticAccountId { get; set; }
        public virtual Account SyntheticAccount { get; set; }
        
        [Required]
        public virtual string AccountName { get; set; }
        public virtual string ExternalCode { get; set; }
        [ForeignKey("Currency")]
        public int CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }

        [ForeignKey("ActivityType")]
        public int? ActivityTypeId { get; set; }
        public virtual ActivityType ActivityType { get; set; }
        [ForeignKey("ThirdParty")]
        public virtual int? ThirdPartyId { get; set; }
        public virtual Person ThirdParty { get; set; }
        public virtual State Status { get; set; }
        public virtual AccountTypes AccountTypes { get; set; }
        public virtual AccountFuncType AccountFuncType { get; set; }
        public bool ComputingAccount { get; set; } // e disponibil pentru folosirea in operatii contabile, etc. (e analitic sau sintetic fara analitic)
        public bool AccountStatus { get; set; } // cont activ sau inactiv
        public TaxStatus TaxStatus { get; set; }

        [ForeignKey("BankAccount")]
        public virtual int? BankAccountId { get; set; }
        public virtual BankAccount BankAccount { get; set; }
        public int TenantId { get; set; }

        [ForeignKey("BNR_Sector")]
        public int? SectorBnrId { get; set; }
        public virtual BNR_Sector BNR_Sector { get; set; }
        public int? NivelRand { get; set; }
        public DateTime DataValabilitate { get; set; }
    }


}
