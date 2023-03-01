using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Abp.Application.Services;
using Abp.IdentityFramework;
using Abp.Runtime.Session;
using Niva.Erp.Authorization.Users;
using Niva.Erp.MultiTenancy;
using Abp.Domain.Entities;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;

namespace Niva.Erp
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class ErpAppServiceBase : ApplicationService
    {
        public TenantManager TenantManager { get; set; }

        public UserManager UserManager { get; set; }

        protected ErpAppServiceBase()
        {
            LocalizationSourceName = ErpConsts.LocalizationSourceName;
        }

        protected virtual async Task<User> GetCurrentUserAsync()
        {
            var user = await UserManager.FindByIdAsync(AbpSession.GetUserId().ToString());
            if (user == null)
            {
                throw new Exception("There is no current user!");
            }

            return user;
        }

        protected virtual Task<Tenant> GetCurrentTenantAsync()
        {
            return TenantManager.GetByIdAsync(AbpSession.GetTenantId());
        }

        protected virtual Tenant GetCurrentTenant()
        {
            return TenantManager.GetById(AbpSession.GetTenantId());
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }

    //public class ErpCrudAppService<T, U> : CrudAppService<T, U> where T : Entity where U : IEntityDto
    //{
    //    public TenantManager _tenantManager { get; set; }
    //    public ErpCrudAppService(IRepository<T, int> repository) : base(repository)
    //    {
            
    //    }

    //    protected virtual Tenant GetCurrentTenant()
    //    {
    //        return  _tenantManager.GetById(AbpSession.GetTenantId());
    //    }
    //}
}
