using Abp.Application.Services;
using Abp.Domain.Repositories;
using Niva.Erp.Models.ImoAsset;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Conta.ImoAsset
{
    public interface IImoAssetOperDocTypeAppService : IApplicationService
    {
        GetImoAssetOperDocTypeOutput OperDocTypeList();

        ImoAssetOperDocTypeEditDto GetOperDocTypeById(int id);

        void SaveOperDocType(ImoAssetOperDocTypeEditDto operDocType);

        void DeleteOperDocType(int id);
    }

    public class GetImoAssetOperDocTypeOutput
    {
        public List<ImoAssetOperDocTypeListDto> GetImoAssetOperDocType { get; set; }
    }

    public class ImoAssetOperDocTypeAppService : ErpAppServiceBase, IImoAssetOperDocTypeAppService
    {
        IRepository<ImoAssetOperDocType> _imoAssetOperDocTypeRepository;

        public ImoAssetOperDocTypeAppService(IRepository<ImoAssetOperDocType> imoAssetOperDocTypeRepository)
        {
            _imoAssetOperDocTypeRepository = imoAssetOperDocTypeRepository;
        }

        public GetImoAssetOperDocTypeOutput OperDocTypeList()
        {
            var _operDocType = _imoAssetOperDocTypeRepository.GetAllIncluding(f => f.DocumentType)
                                    .OrderBy(f => f.OperType);

            var ret = new GetImoAssetOperDocTypeOutput { GetImoAssetOperDocType = ObjectMapper.Map<List<ImoAssetOperDocTypeListDto>>(_operDocType) };
            return ret;
        }


        public ImoAssetOperDocTypeEditDto GetOperDocTypeById(int id)
        {
            var _operDocType = _imoAssetOperDocTypeRepository.FirstOrDefault(f => f.Id == id);
            var ret = ObjectMapper.Map<ImoAssetOperDocTypeEditDto>(_operDocType);
            return ret;
        }

        public void SaveOperDocType(ImoAssetOperDocTypeEditDto operDocType)
        {
            var _operDocType = ObjectMapper.Map<ImoAssetOperDocType>(operDocType);
            if (_operDocType.Id == 0)
            {
                _imoAssetOperDocTypeRepository.Insert(_operDocType);
            }
            else
            {
                var appClient = GetCurrentTenant();
                _operDocType.TenantId = appClient.Id;
                _imoAssetOperDocTypeRepository.Update(_operDocType);
            }
        }


        public void DeleteOperDocType(int id)
        {
            var _operDocType = _imoAssetOperDocTypeRepository.FirstOrDefault(f => f.Id == id);
            _imoAssetOperDocTypeRepository.Delete(_operDocType);
        }

    }
}
