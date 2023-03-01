using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using AutoMapper;
using Niva.Conta.Nomenclatures;
using Niva.Erp.Models.Conta;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Conta.Nomenclatures
{
    public interface IAccountRelationAppService : IApplicationService
    {
        List<AccountRelationDto> AccountRelationList();

        AccountRelationDto AccountRelationEditInit(int relationId);

        AccountRelationDto SaveAccountRelation(AccountRelationDto accountRelation);

        void DeleteAccountRelation(int accountRelationId);
    }

    public class AccountRelationAppService : ErpAppServiceBase, IAccountRelationAppService
    {
        IRepository<AccountRelation> _accountRelationRepository;

        public AccountRelationAppService(IRepository<AccountRelation> accountRelationRepository)
        {
            _accountRelationRepository = accountRelationRepository;
        }

        public List<AccountRelationDto> AccountRelationList()
        {
         
            var _accountRelation = _accountRelationRepository.GetAll()
                                        
                                        .OrderBy(f=>f.DebitRoot).ThenBy(f=>f.CreditRoot);
            var ret = ObjectMapper.Map<List<AccountRelationDto>>(_accountRelation);
            return ret;
        }

        public AccountRelationDto AccountRelationEditInit(int relationId)
        {
            var ret = new AccountRelationDto();
            AccountRelation _accountRelation;
            if (relationId == 0)
            {
                _accountRelation = new AccountRelation();
                
            }
            else
            {
                _accountRelation = _accountRelationRepository.FirstOrDefault(f => f.Id == relationId);
            }
            ret = ObjectMapper.Map<AccountRelationDto>(_accountRelation);

            return ret;
        }

        public AccountRelationDto SaveAccountRelation(AccountRelationDto accountRelation)
        {
           
            // verify existing account
            var count = _accountRelationRepository.GetAll()
                                          .Count(f => f.DebitRoot == accountRelation.DebitRoot && f.CreditRoot == accountRelation.CreditRoot &&   f.Id != accountRelation.Id);
            if (count != 0)
                throw new UserFriendlyException("Eroare", "Aceasta relatie a mai fost definita.");

            try
            {
                var _accountRelation = ObjectMapper.Map<AccountRelation>(accountRelation);

                var appClient = GetCurrentTenant();
                _accountRelation.TenantId = appClient.Id;

                if (_accountRelation.Id == 0)
                {
                    _accountRelationRepository.Insert(_accountRelation);
                }
                else
                {
                    _accountRelationRepository.Update(_accountRelation);
                }

                CurrentUnitOfWork.SaveChanges();

                var ret = ObjectMapper.Map<AccountRelationDto>(_accountRelation);
                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public void DeleteAccountRelation(int accountRelationId)
        {
            var accountRelation = _accountRelationRepository.FirstOrDefault(f => f.Id == accountRelationId);
            _accountRelationRepository.Delete(accountRelation);
            CurrentUnitOfWork.SaveChanges();
        }
    }
}
