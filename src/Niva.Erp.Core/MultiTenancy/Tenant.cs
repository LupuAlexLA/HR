using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.MultiTenancy;
using Niva.Erp.Authorization.Users;
using Niva.Erp.Models.Conta;

namespace Niva.Erp.MultiTenancy
{
    public class Tenant : AbpTenant<User>
    {
        public Tenant()
        {            
        }

        public Tenant(string tenancyName, string name)
            : base(tenancyName, name)
        {
        }

        [ForeignKey("LegalPerson")]
        public virtual int? LegalPersonId { get; set; }
        public virtual LegalPerson LegalPerson { get; set; }

        //public virtual List<UserRole> UserRoles { get; set; }

        [ForeignKey("LocalCurrency")]
        public virtual int? LocalCurrencyId { get; set; }
        public virtual Currency LocalCurrency { get; set; }

        public virtual List<Person> Person { get; set; }

        //public virtual List<User> Users { get; set; }

        //public virtual List<UserRegistrationRequest> UserRegistrationRequest { get; set; }
    }
}
