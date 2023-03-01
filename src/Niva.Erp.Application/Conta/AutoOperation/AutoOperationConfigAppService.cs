using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.Models.AutoOperation;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Contracts;
using Niva.Erp.Repositories.Conta.AutoOperation;
using Niva.Erp.Repositories.Conta.Nomenclatures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Conta.AutoOperation
{
    public interface IAutoOperationConfigAppService : IApplicationService
    {
        AutoOperationConfigDto InitForm();

        AutoOperationConfigDto SearchConfig(AutoOperationConfigDto config);

        AutoOperationConfigDto AddDetailRow(AutoOperationConfigDto config);

        List<AccountConfigDDDto> GetAccounts(int autoOperTypeId);

        AutoOperationConfigDto SaveConfig(AutoOperationConfigDto config);
        AutoOperationConfigDto SaveAutoOperSearch(AutoOperationConfigDto form);
    }

    public class AutoOperationConfigAppService : ErpAppServiceBase, IAutoOperationConfigAppService
    {
        IAutoOperationConfigRepository _autoOperationConfigRepository;
        IRepository<AutoOperationSearchConfig> _autoOperSearchConfigRepository;
        IAccountRepository _accountRepository;

        public AutoOperationConfigAppService(IAutoOperationConfigRepository autoOperationConfigRepository, IAccountRepository accountRepository, IRepository<AutoOperationSearchConfig> autoOperSearchConfigRepository)
        {
            _autoOperationConfigRepository = autoOperationConfigRepository;
            _accountRepository = accountRepository;
            _autoOperSearchConfigRepository = autoOperSearchConfigRepository;
        }
        //[AbpAuthorize("Admin.Conta.Monografii.Acces")]
        public AutoOperationConfigDto InitForm()
        {
            var ret = new AutoOperationConfigDto
            {
                AutoOperType = 0,
                OperationType = 0,
                SearchDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                NoDateSearch = false,
                Details = new List<AutoOperationConfigDetailsDto>(),
                ShowUnreceiveInvoice = false,
                DocumentTypeId = null
            };

            ret = SearchConfig(ret);

            return ret;
        }
        //[AbpAuthorize("Admin.Conta.Monografii.Acces")]
        public AutoOperationConfigDto SearchConfig(AutoOperationConfigDto config)
        {
            AutoOperationType _autoOperationType = (AutoOperationType)config.AutoOperType;

            var autoSearchConfig = _autoOperSearchConfigRepository.GetAllIncluding(f => f.DocumentType).FirstOrDefault(f => f.State == State.Active && f.AutoOperType == _autoOperationType
                                                                  && f.OperationType == config.OperationType);

            var configListDto = new List<AutoOperationConfigDetailsDto>();
            if (autoSearchConfig != null)
            {
                config.DocumentTypeId = autoSearchConfig.DocumentTypeId;
            }
            else
            {
                config.DocumentTypeId = null;
            }
            var configList = _autoOperationConfigRepository.GetAll()
                                                .Where(f => f.State == State.Active && f.AutoOperType == _autoOperationType
                                                 && f.OperationType == config.OperationType)
                                                .OrderBy(f => f.OperationType).ThenBy(f => f.EntryOrder).ToList();

            var x = (!config.NoDateSearch) ?
            configList.Where(f => (f.StartDate <= config.SearchDate) && ((f.EndDate ?? config.SearchDate) >= config.SearchDate)) : configList;
            configListDto = ObjectMapper.Map<List<AutoOperationConfigDetailsDto>>(x.ToList());

            foreach (var item in configListDto)
            {
                item.Deleted = false;
            }

            config.Details = configListDto;

            return config;
        }
        //[AbpAuthorize("Admin.Conta.Monografii.Acces")]
        public AutoOperationConfigDto AddDetailRow(AutoOperationConfigDto config)
        {
            int entryOrder = 0;
            if (config.Details.Count == 0)
            {
                entryOrder = 1;
            }
            else
            {
                entryOrder = (config.Details.Max(f => f.EntryOrder)) + 1;
            }

            var detail = new AutoOperationConfigDetailsDto
            {
                AutoOperType = config.AutoOperType,
                OperationType = config.OperationType,
                StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                EndDate = null,
                EntryOrder = entryOrder,
                UnreceiveInvoice = false,
            };

            var configList = config.Details;
            configList.Add(detail);
            configList = configList.OrderBy(f => f.OperationType).ThenBy(f => f.EntryOrder).ToList();
            config.Details = configList;

            return config;
        }
        //[AbpAuthorize("Admin.Conta.Monografii.Acces")]
        public List<AccountConfigDDDto> GetAccounts(int autoOperTypeId)
        {
            var autoOperType = (AutoOperationType)autoOperTypeId;
            var accountFunctType = (autoOperType == AutoOperationType.MijloaceFixe) ? AccountFuncType.MijloaceFixe : AccountFuncType.ObiecteDeInventar;
            var ret = new List<AccountConfigDDDto>();
            var accountList = _accountRepository.GetAll().Where(f => f.AccountFuncType == accountFunctType && f.Status == State.Active)
                                 .ToList()
                                 .OrderBy(f => f.Symbol);
            var accountListFurn = _accountRepository.GetAllIncluding(f => f.SyntheticAccount)
                                                    .Where(f => f.SyntheticAccount.Symbol == "404" && f.Status == State.Active)
                                     .ToList()
                                     .OrderBy(f => f.Symbol);

            if (autoOperType == AutoOperationType.MijloaceFixe)
            {
                ret.Add(new AccountConfigDDDto { Id = "AA", Name = "Cont mijloc fix" });
                ret.Add(new AccountConfigDDDto { Id = "DA", Name = "Cont amortizare" });
                ret.Add(new AccountConfigDDDto { Id = "EA", Name = "Cont cheltuiala" });
            }
            else
            {
                ret.Add(new AccountConfigDDDto { Id = "IC", Name = "Cont obiect de inventar" });
                ret.Add(new AccountConfigDDDto { Id = "EC", Name = "Cont cheltuiala" });
                ret.Add(new AccountConfigDDDto { Id = "XC", Name = "Cont extrabilantier" });
            }
            foreach (var item in accountList)
            {
                ret.Add(new AccountConfigDDDto { Id = item.Id.ToString(), Name = item.Symbol + " - " + item.AccountName });
            }

            foreach (var item in accountListFurn)
            {
                ret.Add(new AccountConfigDDDto { Id = item.Id.ToString(), Name = item.Symbol + " - " + item.AccountName });
            }

            return ret;
        }
        //[AbpAuthorize("Admin.Conta.Monografii.Acces")]
        public AutoOperationConfigDto SaveConfig(AutoOperationConfigDto config)
        {
            try
            {
                foreach (var item in config.Details.Where(f => f.Deleted == true))
                {
                    if (item.Id != 0)
                    {
                        var dbItem = _autoOperationConfigRepository.GetAll().FirstOrDefault(f => f.Id == item.Id);
                        dbItem.State = State.Inactive;
                    }
                }

                CurrentUnitOfWork.SaveChanges();

                var configList = ObjectMapper.Map<List<AutoOperationConfig>>(config.Details.Where(f => f.Deleted == false).ToList());

                _autoOperationConfigRepository.SaveConfigToDb(configList);

                var ret = SearchConfig(config);

                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }
        //[AbpAuthorize("Admin.Conta.Monografii.Acces")]
        public AutoOperationConfigDto SaveAutoOperSearch(AutoOperationConfigDto form)
        {
            try
            {
                var autoOperSearchDb = ObjectMapper.Map<AutoOperationSearchConfig>(form);
                if (autoOperSearchDb.Id == 0)
                {
                    _autoOperSearchConfigRepository.Insert(autoOperSearchDb);
                }
                else
                {
                    autoOperSearchDb.TenantId = GetCurrentTenant().Id;
                    _autoOperSearchConfigRepository.Update(autoOperSearchDb);
                }
                CurrentUnitOfWork.SaveChanges();
                if (form.Details != null)
                {
                    foreach (var item in form.Details.Where(f => f.Deleted == false))
                    {
                        item.AutoOperSearchConfigId = autoOperSearchDb.Id;
                    }
                }
                form = SearchConfig(form);

                return form;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }

        }
    }
}
