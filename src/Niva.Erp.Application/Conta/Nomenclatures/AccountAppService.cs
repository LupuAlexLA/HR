using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Conta.Nomenclatures;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Contracts;
using Niva.Erp.Models.PrePayments;
using Niva.Erp.Repositories.Conta;
using Niva.Erp.Repositories.Conta.Nomenclatures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Conta.Nomenclatures
{
    public interface IAccountAppService : IApplicationService
    {
        GetAccountOutput InvObjAccountList();

        GetAccountOutput ImoAssetAccountList();

        GetAccountOutput PrepaymentsAccountList(int prepaymentType);

        GetAccountOutput InvoiceElementAccountList(string Prefix, string accountType);

        GetAccountOutput GetAllSyntheticAccounts(string Prefix);

        List<AccountListDDDto> AccountListComputingAll();

        List<AccountListDDDto> AccountListComputing(string search, int currencyId);

        List<AccountListDDDto> AccountListAll(string search);
        List<AccountListDDDto> AccountListNeded(string search); //conturi nedeductibile

        AccountListFormDto AccountListInit();

        AccountListFormDto AccountList(AccountListFormDto accountList);

        AccountEditDto AccountEditInit(int accountId);

        AccountEditDto SaveAccount(AccountEditDto account);

        void DeleteAccount(int accountId);

        AccountDeductibilityForm AccountDeductibilityInit(string accountSearch, int accountId);

        AccountDeductibilityForm AccountDeductibilityList(AccountDeductibilityForm form);

        AccountDeductibilityForm AccountDeductibilityEditInit(AccountDeductibilityForm form, int propertyId);

        AccountDeductibilityForm AccountDeductibilityEditSave(AccountDeductibilityForm form);

        AccountDeductibilityForm AccountDeductibilityDelete(AccountDeductibilityForm form, int propertyId);
        AccountListDDDto GetAccountbyId(int AccountId);

        List<AccountListDDDto> GetAnalythicsForRegistruInventar();
        List<AccountListDDDto> GetAccountsList(string search);
        List<AccountListDDDto> GetAccountsRegInvExceptEliminare();
        List<AccountListDDDto> GetAccountsExceptEliminareList(string search);

        int? GetAccountNivelRandMax();

        List<AccountHistoryListDto> AccountHistoryList(int accountId);

    }

    public class GetAccountOutput
    {
        public List<AccountListDDDto> GetAccounts { get; set; }
    }

    public class AccountAppService : ErpAppServiceBase, IAccountAppService
    {
        IAccountRepository _accountRepository;
        IRepository<AccountTaxProperty> _accountTaxPropertyRepository;
        IRepository<AccountHistory> _accountHistoryRepository;
        IBalanceRepository _balanceRepository;

        public AccountAppService(IAccountRepository accountRepository, IRepository<AccountTaxProperty> accountTaxPropertyRepository, IRepository<AccountHistory> accountHistoryRepository,
                                 IBalanceRepository balanceRepository)
        {
            _accountRepository = accountRepository;
            _accountTaxPropertyRepository = accountTaxPropertyRepository;
            _accountHistoryRepository = accountHistoryRepository;
            _balanceRepository = balanceRepository;
        }

        public GetAccountOutput InvObjAccountList()
        {

            var _accounts = _accountRepository.GetAll()
                                 .Where(f => f.AccountFuncType == AccountFuncType.ObiecteDeInventar)
                                 .ToList()
                                 .OrderBy(f => f.Symbol);

            var ret = new GetAccountOutput { GetAccounts = ObjectMapper.Map<List<AccountListDDDto>>(_accounts) };
            return ret;
        }

        public GetAccountOutput ImoAssetAccountList()
        {

            var _accounts = _accountRepository.GetAll()
                                 .Where(f => f.AccountFuncType == AccountFuncType.MijloaceFixe && f.AccountStatus == true)
                                 .ToList()
                                 .OrderBy(f => f.Symbol);
            var ret = new GetAccountOutput { GetAccounts = ObjectMapper.Map<List<AccountListDDDto>>(_accounts) };
            return ret;
        }

        public GetAccountOutput PrepaymentsAccountList(int prepaymentType)
        {
            var appClient = GetCurrentTenant();
            var _prepaymentType = ((PrepaymentType)prepaymentType == PrepaymentType.CheltuieliInAvans ? AccountFuncType.CheltuieliInAvans : AccountFuncType.VenituriInAvans);
            var _accounts = _accountRepository.GetAll()
                                 .Where(f => f.AccountFuncType == _prepaymentType && f.CurrencyId == appClient.LocalCurrencyId.Value)
                                 .ToList()
                                 .OrderBy(f => f.Symbol);
            var ret = new GetAccountOutput { GetAccounts = ObjectMapper.Map<List<AccountListDDDto>>(_accounts) };
            return ret;
        }

        public GetAccountOutput InvoiceElementAccountList(string Prefix, string accountType)
        {

            var res = _accountRepository.InvoiceElementAccountList(Prefix, accountType);
            var ret = new GetAccountOutput { GetAccounts = ObjectMapper.Map<List<AccountListDDDto>>(res) };
            return ret;
        }

        public GetAccountOutput GetAllSyntheticAccounts(string Prefix)
        {

            var res = _accountRepository.GetAllSyntheticAccounts(Prefix);
            var ret = new GetAccountOutput { GetAccounts = ObjectMapper.Map<List<AccountListDDDto>>(res) };
            return ret;
        }

        public List<AccountListDDDto> AccountListComputingAll()
        {

            var _accounts = _accountRepository.GetAllIncluding(f => f.AccountFuncType)
                                 .Where(f => f.ComputingAccount && f.Status == State.Active && f.AccountStatus)
                                 .ToList()
                                 .OrderBy(f => f.Symbol);
            var ret = ObjectMapper.Map<List<AccountListDDDto>>(_accounts);
            return ret;
        }

        public AccountListDDDto GetAccountbyId(int AccountId)
        {
            var ret = _accountRepository.GetAccountById(AccountId);

            var item = new AccountListDDDto()
            {
                Id = ret.Id,
                Name = ret.Symbol + "-" + ret.AccountName
            };

            return item;
        }

        public List<AccountListDDDto> AccountListComputing(string search, int currencyId)
        {
            var appClientId = GetCurrentTenant();
            var accounts = _accountRepository.GetAll();

            List<Account> list;

            if (search == null)
            {
                list = new List<Account>();
            }
            else
            {
                list = accounts
                     .Where(f => f.TenantId == appClientId.Id && f.ComputingAccount && f.Status == State.Active && f.AccountStatus
                            && (f.CurrencyId == currencyId || f.Symbol.IndexOf("6") == 0 || f.Symbol.IndexOf("7") == 0))
                     .AsEnumerable()
                     .Where(g => g.Symbol.ToUpper().StartsWith(search.ToUpper()) || g.AccountName.ToUpper().Contains(search.ToUpper()))
                     .ToList()
                     .OrderBy(f => f.Symbol).ToList()
                     .Take(50).ToList();
            }

            var ret = ObjectMapper.Map<List<AccountListDDDto>>(list);
            return ret;
        }
        


        public List<AccountListDDDto> AccountListAll(string search)
        {

            if (search == null)
            {
                return null;
            }

            var appClient = GetCurrentTenant();

            var _accounts = _accountRepository.GetAll()
                                                 .Where(f => f.TenantId == appClient.Id && f.Status == State.Active && f.AccountStatus)
                                                 .AsEnumerable()
                                                 .Where(g => g.Symbol.ToUpper().StartsWith(search.ToUpper()) || g.AccountName.ToUpper().Contains(search.ToUpper()))
                                                 .ToList()
                                                 .OrderBy(f => f.Symbol).ToList()
                                                 .Take(50);
            var ret = ObjectMapper.Map<List<AccountListDDDto>>(_accounts);
            return ret;
        }

        public AccountListFormDto AccountListInit()
        {
            var ret = new AccountListFormDto();
            var accounts = new List<AccountListDto>();
            ret.Accounts = accounts;

            return ret;
        }
        //[AbpAuthorize("Admin.Conta.PlanConturi.Acces")]
        public AccountListFormDto AccountList(AccountListFormDto accountList)
        {
            try
            {
                var selectedAppClient = GetCurrentTenant();
                var _accounts = _accountRepository.AccountList((int)selectedAppClient.Id, accountList.SearchAccount, accountList.AccountStatus);
                var retAccounts = ObjectMapper.Map<List<AccountListDto>>(_accounts);
                accountList.Accounts = retAccounts;

                return accountList;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }
        [AbpAuthorize("Admin.Conta.PlanConturi.Modificare")]
        public AccountEditDto AccountEditInit(int accountId)
        {
            var tenant = GetCurrentTenant();
            var ret = new AccountEditDto();
            Account _account;
            if (accountId == 0)
            {
                _account = new Account();
                _account.CurrencyId = GetCurrentTenant().LocalCurrencyId.Value; //TODO uncomment and fix
                _account.Status = State.Active;
                _account.Symbol = "";
                _account.DataValabilitate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            }
            else
            {
                _account = _accountRepository.GetAllIncluding(f => f.SyntheticAccount, f => f.AnalyticAccounts).FirstOrDefault(f => f.Id == accountId);

            }
            ret = ObjectMapper.Map<AccountEditDto>(_account);

            return ret;
        }
        [AbpAuthorize("Admin.Conta.PlanConturi.Modificare")]
        public AccountEditDto SaveAccount(AccountEditDto account)
        {
            var appClient = GetCurrentTenant();
            // verify existing account
            var count = _accountRepository.GetAll()
                                          .Count(f => f.Symbol == account.Symbol
                                                  && f.Status == State.Active && f.Id != account.Id);
            if (count != 0)
                throw new UserFriendlyException("Eroare", "Este definit un alt cont cu acest simbol");
            if (account.AccountName == "")
                throw new UserFriendlyException("Eroare", "Trebuie sa completati denumirea contului");

            var lastBalance = _balanceRepository.GetAll().Where(f => f.Status == State.Active).OrderByDescending(f => f.BalanceDate).FirstOrDefault();
            if (lastBalance.BalanceDate >= account.DataValabilitate)
            {
                throw new UserFriendlyException("Eroare", "Data valabilitatii modificarii nu poate fi mai veche de data ultimei balante");
            }

            try
            {
                var _account = new Account();

                if (account.Id != 0)
                {
                    _account = _accountRepository.GetAllIncluding(f => f.SyntheticAccount, f => f.AnalyticAccounts).FirstOrDefault(f => f.Id == account.Id);
                    var accountHistory = new AccountHistory
                    {
                        AccountId = _account.Id,
                        AccountFuncType = _account.AccountFuncType,
                        AccountName = _account.AccountName,
                        AccountStatus = _account.AccountStatus,
                        AccountTypes = _account.AccountTypes,
                        ActivityTypeId = _account.ActivityTypeId,
                        BankAccountId = _account.BankAccountId,
                        ComputingAccount = _account.ComputingAccount,
                        CurrencyId = _account.CurrencyId,
                        DataValabilitate = _account.DataValabilitate,
                        NivelRand = _account.NivelRand,
                        Status = _account.Status,
                        Symbol = _account.Symbol,
                        SyntheticAccountId = _account.SyntheticAccountId,
                        TaxStatus = _account.TaxStatus,
                        SectorBnrId = _account.SectorBnrId,
                        TenantId = _account.TenantId,
                        ThirdPartyId = _account.ThirdPartyId
                    };
                    _accountHistoryRepository.Insert(accountHistory);
                }

                _account.Id = _account.Id;
                _account.AccountFuncType = (AccountFuncType)account.AccountFuncType;
                _account.AccountName = account.AccountName;
                _account.AccountStatus = account.AccountStatus;
                _account.AccountTypes = (AccountTypes)account.AccountType;
                _account.ActivityTypeId = account.ActivityTypeId;
                _account.BankAccountId = account.BankAccountId;
                _account.ComputingAccount = account.ComputingAccount;
                _account.CurrencyId = account.CurrencyId;
                _account.DataValabilitate = account.DataValabilitate;
                _account.NivelRand = account.NivelRand;
                _account.Status = State.Active;
                _account.Symbol = account.Symbol;
                _account.SyntheticAccountId = account.SyntheticAccountId;
                _account.TaxStatus = (TaxStatus)account.TaxStatus;
                _account.SectorBnrId = account.SectorBnrId;
                _account.TenantId = appClient.Id;
                _account.ThirdPartyId = account.ThirdPartyId;

                if (_account.AccountFuncType != AccountFuncType.ContBancar)
                    _account.BankAccountId = null;


                // preiau contul sintetic
                if (account.SyntheticAccount != null && account.SyntheticAccount != "")
                {
                    var synthetic = _accountRepository.FirstOrDefault(f => f.Symbol == account.SyntheticAccount && f.Status == State.Active);
                    if (synthetic == null)
                    {
                        throw new UserFriendlyException("Eroare", "Sinteticul specificat nu exista in planul de conturi");
                    }
                    else
                    {
                        _account.SyntheticAccountId = synthetic.Id;
                    }
                }

                if (_account.Id == 0)
                {
                    _accountRepository.Insert(_account); // INSERT
                }
                else
                {
                    _accountRepository.Update(_account); // UPDATE

                }
                //  _accountRepository.InsertOrUpdate(_account);
                CurrentUnitOfWork.SaveChanges();

                var ret = ObjectMapper.Map<AccountEditDto>(_account);
                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }
        [AbpAuthorize("Admin.Conta.PlanConturi.Modificare")]
        public AccountEditDto SaveAccountOld(AccountEditDto account)
        {

            // verify existing account
            var count = _accountRepository.GetAll()
                                          .Count(f => f.Symbol == account.Symbol
                                                  && f.Status == State.Active && f.Id != account.Id);
            if (count != 0)
                throw new UserFriendlyException("Eroare", "Este definit un alt cont cu acest simbol");
            if (account.AccountName == "")
                throw new UserFriendlyException("Eroare", "Trebuie sa completati denumirea contului");

            var lastBalance = _balanceRepository.GetAll().Where(f => f.Status == State.Active).OrderByDescending(f => f.BalanceDate).FirstOrDefault();
            if (lastBalance.BalanceDate >= account.DataValabilitate)
            {
                throw new UserFriendlyException("Eroare", "Data valabilitatii modificarii nu poate fi mai veche de data ultimei balante");
            }

            try
            {
                var _account = ObjectMapper.Map<Account>(account);
                if (_account.AccountFuncType != AccountFuncType.ContBancar)
                    _account.BankAccountId = null;

                var appClient = GetCurrentTenant();
                _account.TenantId = appClient.Id;

                // preiau contul sintetic
                if (account.SyntheticAccount != null)
                {
                    var synthetic = _accountRepository.FirstOrDefault(f => f.Symbol == account.SyntheticAccount && f.Status == State.Active);
                    if (synthetic == null)
                    {
                        throw new UserFriendlyException("Eroare", "Sinteticul specificat nu exista in planul de conturi");
                    }
                    else
                    {
                        _account.SyntheticAccountId = synthetic.Id;
                    }
                }

                if (_account.Id == 0)
                {
                    _accountRepository.Insert(_account); // INSERT
                }
                else
                {
                    var _accountForHistory = _accountRepository.GetAllIncluding(f => f.SyntheticAccount, f => f.AnalyticAccounts).FirstOrDefault(f => f.Id == _account.Id);
                    var accountHistory = new AccountHistory
                    {
                        AccountId = _accountForHistory.Id,
                        AccountFuncType = _accountForHistory.AccountFuncType,
                        AccountName = _accountForHistory.AccountName,
                        AccountStatus = _accountForHistory.AccountStatus,
                        AccountTypes = _accountForHistory.AccountTypes,
                        ActivityTypeId = _accountForHistory.ActivityTypeId,
                        BankAccountId = _accountForHistory.BankAccountId,
                        ComputingAccount = _accountForHistory.ComputingAccount,
                        CurrencyId = _accountForHistory.CurrencyId,
                        DataValabilitate = _accountForHistory.DataValabilitate,
                        NivelRand = _accountForHistory.NivelRand,
                        Status = _accountForHistory.Status,
                        Symbol = _accountForHistory.Symbol,
                        SyntheticAccountId = _accountForHistory.SyntheticAccountId,
                        TaxStatus = _accountForHistory.TaxStatus,
                        SectorBnrId = _accountForHistory.SectorBnrId,
                        TenantId = _accountForHistory.TenantId,
                        ThirdPartyId = _accountForHistory.ThirdPartyId
                    };
                    _accountHistoryRepository.Insert(accountHistory);

                    _accountRepository.Update(_account); // UPDATE

                }
                //  _accountRepository.InsertOrUpdate(_account);
                CurrentUnitOfWork.SaveChanges();

                var ret = ObjectMapper.Map<AccountEditDto>(_account);
                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }
        [AbpAuthorize("Admin.Conta.PlanConturi.Modificare")]
        public void DeleteAccount(int accountId)
        {
            var verify = _accountRepository.VerifyAccountDelete(accountId);
            if (verify != "OK")
            {
                throw new UserFriendlyException("Eroare", verify);
            }

            var account = _accountRepository.FirstOrDefault(f => f.Id == accountId);
            account.Status = State.Inactive;
            CurrentUnitOfWork.SaveChanges();
        }

        public AccountDeductibilityForm AccountDeductibilityInit(string accountSearch, int accountId)
        {
            var account = _accountRepository.GetAll().FirstOrDefault(f => f.Id == accountId);
            string accountName = account.Symbol + " - " + account.AccountName;
            var ret = new AccountDeductibilityForm
            {
                AccountId = accountId,
                AccountSearch = accountSearch,

                AccountName = accountName,
                ShowList = true,
                ShowEdit = false
            };
            ret = AccountDeductibilityList(ret);
            return ret;
        }

        public AccountDeductibilityForm AccountDeductibilityList(AccountDeductibilityForm form)
        {
            // throw new NotImplementedException("Decomendeaza codul din metoda"); //TODO uncomment
            var appClient = GetCurrentTenant();
            var list = _accountTaxPropertyRepository.GetAll().Where(f => f.AccountId == form.AccountId && f.TenantId == appClient.Id)
                                                             .OrderByDescending(f => f.PropertyDate)
                                                             .Select(f => new AccountDeductibilityDto
                                                             {
                                                                 Id = f.Id,
                                                                 PropertyDate = f.PropertyDate,
                                                                 PropertyValue = f.PropertyValue * 100,
                                                                 PropertyType = f.PropertyType,
                                                                 PropertyValueStr = f.PropertyType.ToString(),
                                                                 AccountNeded = (f.AccountNededId != null ? f.AccountNeded.Symbol : "")
                                                             })
                                                             .ToList();
            form.DeductibilityList = list;
            form.ShowList = true;
            form.ShowEdit = false;

            return form;
        }

        public AccountDeductibilityForm AccountDeductibilityEditInit(AccountDeductibilityForm form, int propertyId)
        {
            //  throw new NotImplementedException("Decomendeaza codul din metoda"); //TODO uncomment
            var property = new AccountDeductibilityDto();
            if (propertyId == 0)
            {
                property.Id = 0;
                property.PropertyDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                property.PropertyValue = 0;
                property.PropertyType = AccountTaxPropertyType.Nedeductibil;
                property.PropertyTypeId = (int)AccountTaxPropertyType.Nedeductibil;

            }
            else
            {
                var _propertyDb = _accountTaxPropertyRepository.GetAllIncluding(f => f.AccountNeded).FirstOrDefault(f => f.Id == propertyId);
                property.Id = _propertyDb.Id;
                property.PropertyDate = _propertyDb.PropertyDate;
                property.PropertyValue = _propertyDb.PropertyValue * 100;
                property.PropertyType = _propertyDb.PropertyType;
                property.PropertyTypeId = (int)_propertyDb.PropertyType;
                property.AccountNededId = _propertyDb.AccountNededId;
                property.AccountNeded = (_propertyDb.AccountNededId != null) ? (_propertyDb.AccountNeded.Symbol + " - " + _propertyDb.AccountNeded.AccountName) : "";
            }
            form.DeductibilityEdit = property;
            form.ShowEdit = true;
            form.ShowList = false;
            return form;
        }

        public AccountDeductibilityForm AccountDeductibilityEditSave(AccountDeductibilityForm form)
        {
            //        throw new NotImplementedException("Decomendeaza codul din metoda"); //TODO uncomment
            var appClient = GetCurrentTenant();
            AccountTaxProperty _property;
            if (form.DeductibilityEdit.Id == 0)
            {
                _property = new AccountTaxProperty();
                _property.AccountId = form.AccountId;
                _property.TenantId = appClient.Id;
            }
            else
            {
                _property = _accountTaxPropertyRepository.GetAll().FirstOrDefault(f => f.Id == form.DeductibilityEdit.Id);
            }
            _property.PropertyDate = form.DeductibilityEdit.PropertyDate;
            _property.PropertyValue = form.DeductibilityEdit.PropertyValue / 100;
            _property.PropertyType = (AccountTaxPropertyType)form.DeductibilityEdit.PropertyTypeId;
            _property.AccountNededId = form.DeductibilityEdit.AccountNededId;

            if (_property.PropertyType == AccountTaxPropertyType.Nedeductibil)
            {
                _property.PropertyValue = 0;
            }

            //validation
            var count = _accountTaxPropertyRepository.GetAll().Where(f => f.AccountId == _property.AccountId && f.Id != _property.Id && f.PropertyDate == _property.PropertyDate).Count();
            if (count != 0)
            {
                throw new UserFriendlyException("Eroare", "Exista o alta inregistrare pentru aceasta data!");
            }

            if (form.DeductibilityEdit.PropertyValue != 0 && form.DeductibilityEdit.PropertyValue != 100)
            {
                if (form.DeductibilityEdit.AccountNededId == null)
                {
                    throw new UserFriendlyException("Eroare", "Nu ati selectat contul nedeductibil corespunzator!");
                }
            }
            else
            {
                _property.AccountNededId = null;
            }

            if (_property.Id == 0)
            {
                _accountTaxPropertyRepository.Insert(_property);
            }
            else
            {
                _accountTaxPropertyRepository.Update(_property);
            }
            try
            {
                CurrentUnitOfWork.SaveChanges();
                AccountDeductibilityList(form);

                return form;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public AccountDeductibilityForm AccountDeductibilityDelete(AccountDeductibilityForm form, int propertyId)
        {
            //   throw new NotImplementedException("Decomendeaza codul din metoda"); //TODO uncomment
            var _propertyDb = _accountTaxPropertyRepository.GetAll().FirstOrDefault(f => f.Id == propertyId);

            try
            {
                _accountTaxPropertyRepository.Delete(_propertyDb);
                CurrentUnitOfWork.SaveChanges();
                AccountDeductibilityList(form);

                return form;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }

        }

        public List<AccountListDDDto> AccountListNeded(string search)
        {
            int appClientId = GetCurrentTenant().Id;
            var _accounts = new List<Account>();
            if (search != null)
            {
                _accounts = _accountRepository.GetAllNededAccount(appClientId)
                                   .Where(g => String.Concat(g.Symbol, ".", g.AccountName).ToUpper().Contains(search.ToUpper())
                                             && g.Status == State.Active).ToList()
                                   .Take(50)
                                   .ToList();
            }

            var ret = ObjectMapper.Map<List<AccountListDDDto>>(_accounts);
            return ret;
        }

        public List<AccountListDDDto> GetAnalythicsForRegistruInventar()
        {
            try
            {
                var list = _accountRepository.GetAnalythicsWithoutActivityType();
                var ret = ObjectMapper.Map<List<AccountListDDDto>>(list);
                return ret;
            }
            catch (Exception)
            {
                throw new UserFriendlyException("Eroare", "Operatiunea nu poate fi inregistrata");
            }
        }

        public List<AccountListDDDto> GetAccountsList(string search)
        {
            List<Account> list;
            if (search == null || search == "")
            {
                list = new List<Account>();
            }
            else
            {
                list = _accountRepository.GetAnalythicsWithoutActivityType().Where(f => f.Status == State.Active)
                       .AsEnumerable()
                       .Where(g => g.Symbol.ToUpper().StartsWith(search.ToUpper()) || g.AccountName.ToUpper().Contains(search.ToUpper()))
                       .ToList()
                       .OrderBy(f => f.Symbol).ToList()
                       .Take(50).ToList();
            }

            var ret = ObjectMapper.Map<List<AccountListDDDto>>(list);
            return ret;
        }

        public List<AccountListDDDto> GetAccountsRegInvExceptEliminare()
        {
            try
            {
                var list = _accountRepository.GetAccountRegInventarExceptEliminare();
                var ret = ObjectMapper.Map<List<AccountListDDDto>>(list);
                return ret;
            }
            catch (Exception)
            {
                throw new UserFriendlyException("Eroare", "Operatiunea nu poate fi inregistrata");
            }
        }

        public List<AccountListDDDto> GetAccountsExceptEliminareList(string search)
        {
            List<Account> list;
            if (search == null || search == "")
            {
                list = new List<Account>();
            }
            else
            {
                list = _accountRepository.GetAccountRegInventarExceptEliminare().ToList().Where(f => f.Status == State.Active)
                       .AsEnumerable()
                       .Where(g => g.Symbol.ToUpper().StartsWith(search.ToUpper()) || g.AccountName.ToUpper().Contains(search.ToUpper()))
                       .ToList()
                       .OrderBy(f => f.Symbol).ToList()
                       .Take(50).ToList();
            }

            var ret = ObjectMapper.Map<List<AccountListDDDto>>(list);
            return ret.OrderBy(f => f.Name).ToList();
        }

        public int? GetAccountNivelRandMax()
        {
            try
            {
                var appClient = GetCurrentTenant();
                var nivelMax = _accountRepository.GetAll().Where(f => f.Status == State.Active && f.TenantId == appClient.Id).Max(f => f.NivelRand);
                return nivelMax;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public List<AccountHistoryListDto> AccountHistoryList(int accountId)
        {
            try
            {
                var ret = new List<AccountHistoryListDto>();
                var historyList = _accountHistoryRepository.GetAllIncluding(f => f.Currency, f => f.ActivityType, f => f.ThirdParty)
                                                           .Where(f => f.AccountId == accountId)
                                                           .OrderByDescending(f => f.DataValabilitate)
                                                           .ToList();
                ret = ObjectMapper.Map<List<AccountHistoryListDto>>(historyList);

                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }
    }
}
