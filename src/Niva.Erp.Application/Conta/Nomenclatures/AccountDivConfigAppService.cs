using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using AutoMapper;
using Niva.Conta.Nomenclatures;
using Niva.Erp;
using Niva.Erp.Models.AutoOperation;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niva.Erp.Conta.Nomenclatures
{

    public interface IAccountDivConfigAppService : IApplicationService
    {
        List<AccountDivConfigDto> AccountDivConfigList();

        AccountDivConfigDto GetDivAccountById(int id);

        void SaveDivAccount(AccountDivConfigDto account);

        void DeleteDivAccount(int id);
    }


    public class AccountDivConfigAppService :ErpAppServiceBase, IAccountDivConfigAppService
    {
        IRepository<AccountDivConfig> _accountDivConfigRepository;

        public AccountDivConfigAppService(IRepository<AccountDivConfig> accountDivConfigRepository)
        {
            _accountDivConfigRepository = accountDivConfigRepository;
        }

        public List<AccountDivConfigDto> AccountDivConfigList()
        {
            var list = _accountDivConfigRepository.GetAllIncluding(f => f.Account)
                          
                            .OrderByDescending(f=>f.DivYear).ThenBy(f=>f.PersType).ThenBy(f=>f.ResidenceType);
            var ret = ObjectMapper.Map<List<AccountDivConfigDto>>(list);
            return ret;
        }

        public AccountDivConfigDto GetDivAccountById(int id)
        {
            var _account = _accountDivConfigRepository.GetAllIncluding(f=>f.Account).FirstOrDefault(f => f.Id == id);
            var ret = ObjectMapper.Map<AccountDivConfigDto>(_account);
            return ret;
        }

        public void SaveDivAccount(AccountDivConfigDto account)
        {
            try
            {
               
                var _account = ObjectMapper.Map<AccountDivConfig>(account);

                if (_account.AccountType == AccountDivConfigType.ContCreanta)
                {
                    var count = _accountDivConfigRepository.GetAll().Where(f =>  f.AccountType == _account.AccountType && f.Id != _account.Id).Count();
                    if (count != 0)
                        throw new UserFriendlyException("Eroare", "Puteti sa definiti doar un singur cont de creanta");
                }
                int _chk = _accountDivConfigRepository.GetAll().Where(f => f.DivYear == _account.DivYear && f.ResidenceType == _account.ResidenceType
                                                                      && f.PersType == _account.PersType && f.Id != _account.Id).Count();
                if (_chk > 0) throw new UserFriendlyException("Eroare", "Este deja un cont definit pentru acest an si acest tip de actionar");

                if (_account.Id == 0)
                {
                  
                    _accountDivConfigRepository.Insert(_account);
                }
                else
                {
                    _accountDivConfigRepository.Update(_account);
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public void DeleteDivAccount(int id)
        {
            try
            { 
                var _account = _accountDivConfigRepository.FirstOrDefault(f => f.Id == id);
                _accountDivConfigRepository.Delete(_account);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }
    }
}