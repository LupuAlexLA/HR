using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Niva.Erp.Configuration;
using Abp.Timing;
using DevExpress.AspNetCore.Reporting.WebDocumentViewer;
using Abp.Dependency;
using DevExpress.AspNetCore.Reporting.QueryBuilder;
using DevExpress.AspNetCore.Reporting.ReportDesigner;
using DevExpress.XtraReports.Web.Extensions;
using Abp.Configuration.Startup;
using Abp.AspNetCore.Configuration;
using Abp.Threading.BackgroundWorkers;
using Niva.Erp.BackgroudWorkers.Bnr;
using Niva.Erp.BackgroudWorkers;

namespace Niva.Erp.Web.Host.Startup
{
    [DependsOn(
       typeof(ErpWebCoreModule))]
    public class ErpWebHostModule: AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public ErpWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }
        
        public override void Initialize()
        {
            Clock.Provider = ClockProviders.Utc;
            IocManager.RegisterAssemblyByConvention(typeof(ErpWebHostModule).GetAssembly());
            IocManager.Register(typeof(WebDocumentViewerController), DependencyLifeStyle.Transient);
            IocManager.Register(typeof(QueryBuilderController), DependencyLifeStyle.Transient);
            IocManager.Register(typeof(ReportDesignerController), DependencyLifeStyle.Transient);
            IocManager.Register(typeof(ReportStorageWebExtension), DependencyLifeStyle.Transient);
        }

        public override void PreInitialize()
        {
            //Configuration.Modules.AbpAspNetCore().DefaultWrapResultAttribute.WrapOnError = false;
            //Configuration.Modules.AbpAspNetCore().DefaultWrapResultAttribute.WrapOnSuccess = false;
        }
        public override void PostInitialize()
        {
            base.PostInitialize();
            var workManager = IocManager.Resolve<IBackgroundWorkerManager>();
            workManager.Add(IocManager.Resolve<CursBnrBackgroundWorker>());
            workManager.Add(IocManager.Resolve<PreluareFileDocBackgroundWorker>());
        }
    }
}
