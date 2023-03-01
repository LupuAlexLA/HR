using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.Conta.Nomenclatures.Dto;
using Niva.Erp.EntityFrameworkCore.Repositories.Conta;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Conta.Nomenclatures
{
    public interface IOperationTypesAppService : IApplicationService
    {
        List<OperationTypesListDto> OperTypesList();
        void SaveOperType(OperationTypesEditDto operTypes);
        OperationTypesEditDto GetOperTypeById(int id);
        void Delete(int id);

    }
    public class OperationTypesAppService : ErpAppServiceBase, IOperationTypesAppService
    {
        IRepository<OperationTypes> _operTypesRepository;
        OperationRepository _operationRepository;
        public OperationTypesAppService(IRepository<OperationTypes> operTypesRepository, OperationRepository operationRepository)
        {
            _operTypesRepository = operTypesRepository;
            _operationRepository = operationRepository;
        }
        
        //AbpAuthorize("Admin.Conta.TipOperContab.Acces")]
        public void Delete(int id)
        {
            try
            {
                var operType = _operTypesRepository.FirstOrDefault(f => f.Id == id);
                var operation = _operationRepository.GetAll().Where(f => f.OperationTypeId == operType.Id).Count();

                if (operation != 0)
                {
                    throw new Exception("Tipul operatiei nu poate fi sters, deoarece este utilizat in definirea unei note contabile");
                }
                else
                {
                    operType.State = State.Inactive;
                    CurrentUnitOfWork.SaveChanges();
                }

            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.Message);
            }

        }
      
        //[AbpAuthorize("Admin.Conta.TipOperContab.Acces")]
        public OperationTypesEditDto GetOperTypeById(int id)
        {
            var operType = _operTypesRepository.Get(id);
            var ret = ObjectMapper.Map<OperationTypesEditDto>(operType);
            return ret;
        }
       
        //[AbpAuthorize("Admin.Conta.TipOperContab.Acces")]
        public List<OperationTypesListDto> OperTypesList()
        {
            var operTypes = _operTypesRepository.GetAll().Where(f => f.State == State.Active).ToList();

            var ret = ObjectMapper.Map<List<OperationTypesListDto>>(operTypes);
            return ret;
        }
       
        //[AbpAuthorize("Admin.Conta.TipOperContab.Acces")]
        public void SaveOperType(OperationTypesEditDto operTypes)
        {
            try
            {
                // get current tenant
                var appClient = GetCurrentTenant();
                var _operTypes = ObjectMapper.Map<OperationTypes>(operTypes);


                if (_operTypes.Id == 0)
                {
                    _operTypesRepository.Insert(_operTypes);
                    CurrentUnitOfWork.SaveChanges();
                }
                else
                {
                    _operTypes.TenantId = appClient.Id;
                    _operTypesRepository.Update(_operTypes);
                    CurrentUnitOfWork.SaveChanges();

                }

            }
            catch (Exception ex)
            {
                throw new Abp.UI.UserFriendlyException("Eroare", ex.Message);
            }
        }
    }
}
