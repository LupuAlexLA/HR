using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.MultiTenancy;
using Niva.Erp.Authorization.Users;


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



        //public virtual List<User> Users { get; set; }

        //public virtual List<UserRegistrationRequest> UserRegistrationRequest { get; set; }
    }
}
