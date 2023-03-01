using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using AutoMapper;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.ImoAsset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Niva.Erp.Conta.ImoAsset
{
    public interface IImoAssetClassCodeAppService : IApplicationService
    {
        GetImoAssetClassCodeOutput ImoAssetClassCodeList();

        List<ImoAssetClassCodeListDDDto> ImoAssetClassCodeListDD();

        ImoAssetClassCodeEditDto GetClassCodeById(int id);

        void SaveClassCode(ImoAssetClassCodeEditDto ClassCode);

        void DeleteClassCode(int id);
    }

    public class GetImoAssetClassCodeOutput
    {
        public List<ImoAssetClassCodeListDto> GetImoAssetClassCode { get; set; }
    }

    public class ImoAssetClassCodeAppService : ErpAppServiceBase, IImoAssetClassCodeAppService
    {
        IRepository<ImoAssetClassCode> _imoAssetClassCodeRepository;

        public ImoAssetClassCodeAppService(IRepository<ImoAssetClassCode> imoAssetClassCodeRepository)
        {
            _imoAssetClassCodeRepository = imoAssetClassCodeRepository;
        }

        public GetImoAssetClassCodeOutput ImoAssetClassCodeList()
        {
            var _ClassCode = _imoAssetClassCodeRepository.GetAllIncluding(f => f.AssetAccount, f => f.DepreciationAccount, f => f.ExpenseAccount, f => f.ClassCodeParrent)
                                                    .Where(f => f.State == State.Active)
                                                    .OrderBy(f => f.Code);

            var ret = new GetImoAssetClassCodeOutput { GetImoAssetClassCode = ObjectMapper.Map<List<ImoAssetClassCodeListDto>>(_ClassCode) };
            return ret;
        }

        public List<ImoAssetClassCodeListDDDto> ImoAssetClassCodeListDD()
        {
            var _ClassCode = _imoAssetClassCodeRepository.GetAll()
                                                    .Where(f => f.State == State.Active)
                                                    .OrderBy(f => f.Code);

            var ret = ObjectMapper.Map<List<ImoAssetClassCodeListDDDto>>(_ClassCode);
            return ret;
        }

        public ImoAssetClassCodeEditDto GetClassCodeById(int id)
        {
            var _classCode = _imoAssetClassCodeRepository.FirstOrDefault(f => f.Id == id);
            var ret = ObjectMapper.Map<ImoAssetClassCodeEditDto>(_classCode);
            return ret;
        }

        public void SaveClassCode(ImoAssetClassCodeEditDto ClassCode)
        {
            var _classCode = ObjectMapper.Map<ImoAssetClassCode>(ClassCode);
            var appClient = GetCurrentTenant();
            if (_classCode.Id == 0)
            {

                int _chk = _imoAssetClassCodeRepository.GetAll().Where(f => f.Name == _classCode.Name && f.State == State.Active).Count();

                if (_chk > 0) throw new UserFriendlyException("Eroare", "ClassCode existent!");

                _imoAssetClassCodeRepository.Insert(_classCode);
            }
            else
            {
                _classCode.TenantId = appClient.Id;
                _imoAssetClassCodeRepository.Update(_classCode);
            }
        }

        public void DeleteClassCode(int id)
        {
            var _classCode = _imoAssetClassCodeRepository.FirstOrDefault(f => f.Id == id);

            var count = _imoAssetClassCodeRepository.GetAll().Where(f => f.ClassCodeParrentId == id).Count();

            if (count > 0)
            {
                throw new UserFriendlyException("Eroare", "Codul de verificare nu poate fi sters, deoarece este folosit la definirea altul cod.");
            }
            else
            {
                _classCode.State = State.Inactive;
                _imoAssetClassCodeRepository.Update(_classCode);
            }
        }

    }
}
