using Abp.Application.Services;
using Abp.Domain.Repositories;
using Niva.Erp.Models.PrePayments;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Conta.Prepayments
{
    public interface IPrepaymentsOperDocTypeAppService : IApplicationService
    {
        GetPrepaymentsOperDocTypeOutput OperDocTypeList();

        PrepaymentsOperDocTypeEditDto GetOperDocTypeById(int id);

        void SaveOperDocType(PrepaymentsOperDocTypeEditDto operDocType);

        void DeleteOperDocType(int id);
    }

    public class GetPrepaymentsOperDocTypeOutput
    {
        public List<PrepaymentsOperDocTypeListDto> GetPrepaymentsOperDocType { get; set; }
    }

    public class PrepaymentsOperDocTypeAppService : ErpAppServiceBase, IPrepaymentsOperDocTypeAppService
    {
        IRepository<PrepaymentDocType> _prepaymentOperDocTypeRepository;

        public PrepaymentsOperDocTypeAppService(IRepository<PrepaymentDocType> prepaymentOperDocTypeRepository)
        {
            _prepaymentOperDocTypeRepository = prepaymentOperDocTypeRepository;
        }
        //[AbpAuthorize("Administrare.CheltAvans.TipuriDoc.Acces")]
        //[AbpAuthorize("Administrare.VenitAvans.TipuriDoc.Acces")]
        public GetPrepaymentsOperDocTypeOutput OperDocTypeList()
        {
            var _operDocType = _prepaymentOperDocTypeRepository.GetAllIncluding(f => f.DocumentType)
                                    .OrderBy(f => f.OperType);

            var ret = new GetPrepaymentsOperDocTypeOutput { GetPrepaymentsOperDocType = ObjectMapper.Map<List<PrepaymentsOperDocTypeListDto>>(_operDocType) };
            return ret;
        }

        public PrepaymentsOperDocTypeEditDto GetOperDocTypeById(int id)
        {
            var _operDocType = _prepaymentOperDocTypeRepository.FirstOrDefault(f => f.Id == id);
            var ret = ObjectMapper.Map<PrepaymentsOperDocTypeEditDto>(_operDocType);
            return ret;
        }

        //[AbpAuthorize("Administrare.CheltAvans.TipuriDoc.Acces")]
        //[AbpAuthorize("Administrare.VenitAvans.TipuriDoc.Acces")]
        public void SaveOperDocType(PrepaymentsOperDocTypeEditDto operDocType)
        {
            var _operDocType = ObjectMapper.Map<PrepaymentDocType>(operDocType);
            var appClient = GetCurrentTenant();
            if (_operDocType.Id == 0)
            {
                _prepaymentOperDocTypeRepository.Insert(_operDocType);
            }
            else
            {
                _operDocType.TenantId = appClient.Id;
                _prepaymentOperDocTypeRepository.Update(_operDocType);
            }
        }

        //[AbpAuthorize("Administrare.CheltAvans.TipuriDoc.Acces")]
        //[AbpAuthorize("Administrare.VenitAvans.TipuriDoc.Acces")]
        public void DeleteOperDocType(int id)
        {
            var _operDocType = _prepaymentOperDocTypeRepository.FirstOrDefault(f => f.Id == id);
            _prepaymentOperDocTypeRepository.Delete(_operDocType);
        }

    }
}
