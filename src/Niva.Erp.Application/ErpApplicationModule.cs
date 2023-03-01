using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Niva.Erp.Authorization;

namespace Niva.Erp
{
    [DependsOn(
        typeof(ErpCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class ErpApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<ErpAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(ErpApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}
