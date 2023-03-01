using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace Niva.Erp.Controllers
{
    public abstract class ErpControllerBase: AbpController
    {
        protected ErpControllerBase()  
        {
            LocalizationSourceName = ErpConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
