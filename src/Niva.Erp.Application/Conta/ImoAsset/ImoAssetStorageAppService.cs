using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.ImoAsset;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Conta.ImoAsset
{
    public interface IImoAssetStorageAppService : IApplicationService
    {
        GetImoAssetStorageOutput ImoAssetStorageList();

        ImoAssetStorageDto GetStorageById(int id);

        void SaveStorage(ImoAssetStorageDto storage);

        void DeleteStorage(int id);
    }

    public class GetImoAssetStorageOutput
    {
        public List<ImoAssetStorageDto> GetImoAssetStorage { get; set; }
    }

    public class ImoAssetStorageAppService : ErpAppServiceBase, IImoAssetStorageAppService
    {
        IRepository<ImoAssetStorage> _imoAssetStorageRepository;

        public ImoAssetStorageAppService(IRepository<ImoAssetStorage> imoAssetStorageRepository)
        {
            _imoAssetStorageRepository = imoAssetStorageRepository;
        }

        public GetImoAssetStorageOutput ImoAssetStorageList()
        {
            var _storage = _imoAssetStorageRepository.GetAll()
                                                    .Where(f => f.State == State.Active)
                                                    .OrderBy(f => f.StorageName);

            var ret = new GetImoAssetStorageOutput { GetImoAssetStorage = ObjectMapper.Map<List<ImoAssetStorageDto>>(_storage) };
            return ret;
        }

        public ImoAssetStorageDto GetStorageById(int id)
        {
            var _storage = _imoAssetStorageRepository.FirstOrDefault(f => f.Id == id);
            var ret = ObjectMapper.Map<ImoAssetStorageDto>(_storage);
            return ret;
        }

        public void SaveStorage(ImoAssetStorageDto storage)
        {
            var _storage = ObjectMapper.Map<ImoAssetStorage>(storage);
            
            if (_storage.Id == 0)
            {

                int _chk = _imoAssetStorageRepository.GetAll().Where(f => f.StorageName == _storage.StorageName && f.State == State.Active).Count();

                if (_chk > 0) throw new UserFriendlyException("Eroare", "Storage existent!");

                _imoAssetStorageRepository.Insert(_storage);
            }
            else
            {
                var appClient = GetCurrentTenant();
                _storage.TenantId = appClient.Id;
                _imoAssetStorageRepository.Update(_storage);
            }
        }

        public void DeleteStorage(int id)
        {
            var _storage = _imoAssetStorageRepository.FirstOrDefault(f => f.Id == id);
            _storage.State = State.Inactive;
            _imoAssetStorageRepository.Update(_storage);
        }

    }
}
