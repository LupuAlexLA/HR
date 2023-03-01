using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Conta.Nomenclatures;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Niva.Erp.Conta.Nomenclatures
{
    public interface IAccountConfigAppService : IApplicationService
    {
        List<AccountConfigDto> AccountConfigList();

        List<AccountConfigDto> AccountConfigListByDate(DateTime searchDate);

        AccountConfigDto GetAccountConfigById(int id);

        void SaveAccountConfig(AccountConfigDto account);

        void DeleteAccountConfig(int id);
    }

    public class AccountConfigAppService : ErpAppServiceBase, IAccountConfigAppService
    {
        IRepository<AccountConfig> _accountConfigRepository;

        public AccountConfigAppService(IRepository<AccountConfig> accountConfigRepository)
        {
            _accountConfigRepository = accountConfigRepository;
        }
       
        //[AbpAuthorize("Admin.Conta.ConfigConturi.Acces")]
        public List<AccountConfigDto> AccountConfigList()
        {
            var list = _accountConfigRepository.GetAllIncluding(f => f.ImoAssetStorage, f => f.ActivityType)
                            .Where(f => f.Status == State.Active)
                            .OrderByDescending(f => f.ValabilityDate).ThenBy(f => f.Symbol);
            var ret = ObjectMapper.Map<List<AccountConfigDto>>(list);
            return ret;
        }
       
        //[AbpAuthorize("Admin.Conta.ConfigConturi.Acces")]
        public List<AccountConfigDto> AccountConfigListByDate(DateTime searchDate)
        {
            var list = _accountConfigRepository.GetAllIncluding(f => f.ImoAssetStorage, f => f.ActivityType)
                            .Where(f => f.Status == State.Active && f.ValabilityDate <= searchDate)
                            .OrderByDescending(f => f.ValabilityDate).ThenBy(f => f.Symbol);
            var ret = ObjectMapper.Map<List<AccountConfigDto>>(list);
            return ret;
        }
      
        //[AbpAuthorize("Admin.Conta.ConfigConturi.Acces")]
        public AccountConfigDto GetAccountConfigById(int id)
        {
            var _account = _accountConfigRepository.GetAllIncluding(f => f.ImoAssetStorage, f => f.ActivityType).FirstOrDefault(f => f.Id == id);
            var ret = ObjectMapper.Map<AccountConfigDto>(_account);
            return ret;
        }
     
        //[AbpAuthorize("Admin.Conta.ConfigConturi.Acces")]
        public void SaveAccountConfig(AccountConfigDto account)
        {
            try
            {
                if (account.Description == null)
                {
                    throw new Exception("Nu ati completat descrierea");
                }
                var _account = ObjectMapper.Map<AccountConfig>(account);
                var appClient = GetCurrentTenant();

                if (_account.Id == 0)
                {

                    _accountConfigRepository.Insert(_account);
                }
                else
                {
                    _account.TenantId = appClient.Id;
                    _accountConfigRepository.Update(_account);
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }
     
        //[AbpAuthorize("Admin.Conta.ConfigConturi.Acces")]
        public void DeleteAccountConfig(int id)
        {
            try
            {
                var _account = _accountConfigRepository.FirstOrDefault(f => f.Id == id);
                _accountConfigRepository.Delete(_account);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }
    }
}
