using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Niva.Erp.Models.ImoAsset;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Niva.Erp.Conta.ImoAsset
{
    public interface IImoAssetSetupAppService : IApplicationService
    {
        ImoAssetSetupDto InitForm();

        ImoAssetSetupDto SaveSetup(ImoAssetSetupDto setup);
    }

    public class ImoAssetSetupAppService : ErpAppServiceBase, IImoAssetSetupAppService
    {
        IRepository<ImoAssetSetup> _imoAssetSetupRepository;

        public ImoAssetSetupAppService(IRepository<ImoAssetSetup> imoAssetSetupRepository)
        {
            _imoAssetSetupRepository = imoAssetSetupRepository;
        }
        //[AbpAuthorize("Administrare.MF.Setup.Acces")]
        public ImoAssetSetupDto InitForm()
        {
            ImoAssetSetup setup;
            setup = _imoAssetSetupRepository.GetAll().FirstOrDefault();
            if (setup == null)
            {
                setup = new ImoAssetSetup();
            }

            var ret = ObjectMapper.Map<ImoAssetSetupDto>(setup);
            return ret;
        }
        //[AbpAuthorize("Administrare.MF.Setup.Acces")]
        public ImoAssetSetupDto SaveSetup(ImoAssetSetupDto setup)
        {
            ImoAssetSetup _setup = ObjectMapper.Map<ImoAssetSetup>(setup);

            var appClient = GetCurrentTenant();
            _setup.TenantId = appClient.Id;
            _imoAssetSetupRepository.InsertOrUpdate(_setup);
            CurrentUnitOfWork.SaveChanges();

            var ret = ObjectMapper.Map<ImoAssetSetupDto>(_setup);
            return ret;
        }
    }
}
