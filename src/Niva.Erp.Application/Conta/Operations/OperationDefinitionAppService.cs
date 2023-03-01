using Abp.Application.Services;
using Abp.Domain.Repositories;
using Niva.Erp.Models.Conta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Abp.UI;

namespace Niva.Erp.Conta.Operations
{
    public interface IOperationDefinitionAppService : IApplicationService
    {

    }
        public class OperationDefinitionAppService : ErpAppServiceBase, IOperationDefinitionAppService
    {
        IRepository<OperationDefinition> _operationDefinitionRepository;
        IRepository<OperationDefinitionDetails> _operationDefinitionDetailsRepository;
        public OperationDefinitionAppService(IRepository<OperationDefinition> operationDefinitionRepository, IRepository<OperationDefinitionDetails> operationDefinitionDetailsRepository)
        {

            _operationDefinitionRepository = operationDefinitionRepository;
            _operationDefinitionDetailsRepository = operationDefinitionDetailsRepository;   
                
        }


        public OperationDefinitionDto InitOperation(int operationId)
        {
            var selectedTenant = GetCurrentTenant();
            var localCurrencyId = selectedTenant.LocalCurrencyId.Value;

        
            if (operationId == 0)
            {
                var ret = new OperationDefinitionDto
                {

                    CurrencyId = localCurrencyId,

                };

                var details = new List<OperationDefinitionDetailsDto>() { new OperationDefinitionDetailsDto()};
                ret.OperationDetails = details;

                return ret;
            }
            else
            {
                var ret = ObjectMapper.Map<OperationDefinitionDto>(_operationDefinitionRepository.GetAll().Include(f => f.OperationDefinitionDetails).ThenInclude(f => f.Credit).Include(f => f.OperationDefinitionDetails).ThenInclude(f => f.Debit).Where(f => f.Id == operationId).FirstOrDefault());
                CurrentUnitOfWork.SaveChanges();
               
                
                return ret;
            }
        }


        public List<OperationDefinitionDetailsDto> OperationDefinitionDetailsList()
        {
            var _operations = _operationDefinitionDetailsRepository.GetAllIncluding(f => f.OperationDefinition).Where(f => f.OperationDefinition.Status == Models.Conta.Enums.State.Active);

            var ret = ObjectMapper.Map<List<OperationDefinitionDetailsDto>>(_operations).ToList();
            return ret;
        }

        public List<OperationDefinitionDto> OperationDefinitionList()
        {
            var _operations = _operationDefinitionRepository.GetAll().Include(f => f.OperationDefinitionDetails).ThenInclude(f => f.Credit).Include(f => f.OperationDefinitionDetails).ThenInclude(f => f.Debit).Where(f => f.Status == Models.Conta.Enums.State.Active);

            var ret = ObjectMapper.Map<List<OperationDefinitionDto>>(_operations).ToList();
            return ret;
        }

        public OperationDefinitionDetailsDto GetOperationDefinitionDetailsId(int id)
        {
            var ret = _operationDefinitionDetailsRepository.Get(id);
            return ObjectMapper.Map<OperationDefinitionDetailsDto>(ret);
        }

        public OperationDefinitionDto GetOperationDefinitionId(int id)
        {
            var ret = _operationDefinitionRepository.GetAll().Include(f => f.OperationDefinitionDetails).Where(f => f.Id == id).FirstOrDefault();
            return ObjectMapper.Map<OperationDefinitionDto>(ret);
        }

        public void SaveOperationDefinitionDetails(OperationDefinitionDetailsDto Operation)
        {
            var _Operation = ObjectMapper.Map<OperationDefinitionDetails>(Operation);

            if (_Operation.Id == 0)
            {

                int _chk = _operationDefinitionDetailsRepository.GetAll().Include(f => f.OperationDefinition).Where(f => f.OperationDefinition.Name == _Operation.OperationDefinition.Name && f.OperationDefinition.Status == Models.Conta.Enums.State.Active).Count();

                if (_chk > 0) throw new UserFriendlyException("Eroare", "Termen existent!");

                _operationDefinitionDetailsRepository.Insert(_Operation);
            }
            else
            {
                var appClient = GetCurrentTenant();
                _Operation.TenantId = appClient.Id;
                _operationDefinitionDetailsRepository.Update(_Operation);
            }
        }

        public void SaveOperationDefinition(OperationDefinitionDto Operation)
        {
            var _Operation = ObjectMapper.Map<OperationDefinition>(Operation);

            if (_Operation.Id == 0)
            {

                int _chk = _operationDefinitionRepository.GetAll().Where(f => f.Name == _Operation.Name && f.Status == Models.Conta.Enums.State.Active).Count();

                if (_chk > 0) throw new UserFriendlyException("Eroare", "Termen existent!");

                var _details = _Operation.OperationDefinitionDetails;
                _Operation.OperationDefinitionDetails = null;
                var id = _operationDefinitionRepository.InsertAndGetId(_Operation);

                foreach (var value in _details)
                {
                    value.OperationDefinitionId = id;
                    _operationDefinitionDetailsRepository.Insert(value);
                }
            }
            else
            {


                var _oldDetails = _operationDefinitionDetailsRepository.GetAll().Where(f => f.OperationDefinitionId == _Operation.Id);
                foreach (var item in _oldDetails)
                {
                    _operationDefinitionDetailsRepository.Delete(item);
                }
                CurrentUnitOfWork.SaveChanges();

                foreach (var item in _Operation.OperationDefinitionDetails)
                {
                    item.OperationDefinitionId = _Operation.Id;
                    item.Id = 0;
                    _operationDefinitionDetailsRepository.Insert(item);
                }
                CurrentUnitOfWork.SaveChanges();

                var appClient = GetCurrentTenant();
                _Operation.TenantId = appClient.Id;
                _operationDefinitionRepository.Update(_Operation);
                CurrentUnitOfWork.SaveChanges();
            }
        }

        public void DeleteOperationDefinitionDetails(int id)
        {
            var _Operation = _operationDefinitionDetailsRepository.GetAll().Include(f => f.OperationDefinition).Where(f => f.OperationDefinition.Id == id).FirstOrDefault();
            _Operation.OperationDefinition.Status = Models.Conta.Enums.State.Inactive;
            _operationDefinitionDetailsRepository.Update(_Operation);
        }

        public void DeleteOperationDefinition(int id)
        {
            var _Operation = _operationDefinitionRepository.GetAll().Where(f => f.Id == id).FirstOrDefault();
            _Operation.Status = Models.Conta.Enums.State.Inactive;
            _operationDefinitionRepository.Update(_Operation);
        }



    }
}
