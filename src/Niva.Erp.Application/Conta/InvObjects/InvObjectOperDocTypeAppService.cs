using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.Conta.InvObjects.Dto;
using Niva.Erp.Models.InvObjects;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Conta.InvObjects
{
    public interface IInvObjectOperDocTypeAppService : IApplicationService
    {
        GetInvObjOperDocTypeOutput InvObjOperDocTypeList();
        InvObjectOperDocTypeEditDto GetInvObjOperDocTypeById(int operDocTypeId);
        void SaveInvObjOperDocType(InvObjectOperDocTypeEditDto operDocType);
        void DeleteInvObjOperDocType(int operDocTypeId);
    }

    public class GetInvObjOperDocTypeOutput
    {
        public List<InvObjectOperDocTypeDto> GetInvObjOperDocType { get; set; }
    }

    public class InvObjectOperDocTypeAppService: ErpAppServiceBase, IInvObjectOperDocTypeAppService
    {
        IRepository<InvObjectOperDocType> _invObjOperDocTypeRepository;

        public InvObjectOperDocTypeAppService(IRepository<InvObjectOperDocType> invObjOperDocTypeRepository)
        {
            _invObjOperDocTypeRepository = invObjOperDocTypeRepository;
        }

        public void DeleteInvObjOperDocType(int operDocTypeId)
        {
            try
            {
                var _operDocType = _invObjOperDocTypeRepository.GetAll().FirstOrDefault(f => f.Id == operDocTypeId);
                _invObjOperDocTypeRepository.Delete(_operDocType);

            }
            catch (System.Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex);
            }
        }

        public InvObjectOperDocTypeEditDto GetInvObjOperDocTypeById(int operDocTypeId)
        {
            var _operDoctType = _invObjOperDocTypeRepository.GetAll().FirstOrDefault(f => f.Id == operDocTypeId);
            var ret = ObjectMapper.Map<InvObjectOperDocTypeEditDto>(_operDoctType);
            return ret;
        }

        public GetInvObjOperDocTypeOutput InvObjOperDocTypeList()
        {
            var operDocTypeList = _invObjOperDocTypeRepository.GetAllIncluding(f => f.DocumentType).OrderBy(f => f.OperType);
            var ret = new GetInvObjOperDocTypeOutput { GetInvObjOperDocType = ObjectMapper.Map<List<InvObjectOperDocTypeDto>>(operDocTypeList) };
            return ret;
        }

        public void SaveInvObjOperDocType(InvObjectOperDocTypeEditDto operDocType)
        {
            var _operDocType = ObjectMapper.Map<InvObjectOperDocType>(operDocType);
            if (_operDocType.Id == 0)
            {
                _invObjOperDocTypeRepository.Insert(_operDocType);
            }
            else
            {
                var appClient = GetCurrentTenant();
                _operDocType.TenantId = appClient.Id;
                _invObjOperDocTypeRepository.Update(_operDocType);
            }
        }
    }
}
