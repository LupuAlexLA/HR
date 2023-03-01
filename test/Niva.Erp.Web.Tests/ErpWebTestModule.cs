using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Niva.Erp.EntityFrameworkCore;
using Niva.Erp.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Niva.Erp.Web.Tests
{
    [DependsOn(
        typeof(ErpWebMvcModule),
        typeof(AbpAspNetCoreTestBaseModule)
    )]
    public class ErpWebTestModule : AbpModule
    {
        public ErpWebTestModule(ErpEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
        } 
        
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ErpWebTestModule).GetAssembly());
        }
        
        public override void PostInitialize()
        {
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(ErpWebMvcModule).Assembly);
        }
    }
}