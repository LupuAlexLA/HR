using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Niva.Erp.Authorization.Users;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Managers.Reporting;
using Niva.Erp.ModelObjects;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.ConfigurareRapoarte;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Conta.SituatiiFinanciare;
using Niva.Erp.Models.Contracts;
using Niva.Erp.Models.Setup;
using Niva.Erp.MultiTenancy;
using Niva.Erp.Repositories;
using Niva.Erp.Repositories.Conta;
using Niva.Erp.Repositories.Conta.AutoOperation;
using Niva.Erp.Repositories.Conta.ConfigurareRapoarte;
using Niva.Erp.Repositories.Conta.Nomenclatures;
using Niva.Erp.Repositories.Conta.SituatiiFinanciare;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Niva.Erp.Conta.AutoOperation
{
    [AbpAllowAnonymous]
    public interface IAutomatAppService : IApplicationService
    {
        Tokens GetToken(string tokenId);

        int ValidateToken(string tokenId);

        int FindAccount(string tokenId, string partialSymbol, string externalCode, string accountFuncType, int exactAccount);

        IEnumerable<GetAccountDto> GetAccount(string tokenId, string partialSymbol, string externalCode, string accountFuncType, int exactAccount);

        string GetNewAccount(string tokenId, string partialSymbol, string accountName, string externalCode, int accountFuncType, int nrOfFigures);

        string SaveNewAccount(string tokenId, string symbol, string synthetic, string accountName, string externalCode, string thirdPartyCif, string currency, int accountFuncType);

        string VerifyClosedMonth(string tokenId, string operationDate);

        // Add operation header
        ResultMessageIdDto AddOperation(string tokenId, string operationDate, string documentNr, string documentDate, int closingMonth, int operationStatus, string currency, string documentType);

        string AddOperationDetail(string tokenId, int operationId, string debitSymbol, string creditSymbol, decimal value, decimal valueCurrency, string details);

        string DeleteOperation(string tokenId, int idOperation);

        IEnumerable<GetAccountListDto> GetAccounts(string tokenId, string synthetic, string includeSynthetic);

        string GetSoldAccount(string tokenId, string account, DateTime date, out decimal sold);

        string ExchangeRatesAddModify(string currencyCode, DateTime exchangeDate, decimal value);

        string AddModifyThirdParty(string tokenId, string personType, string id1, string id2, string name, string lastName, string address, string countryCode, string regionCode, string locality);

        string AddModifyBank(string tokenId, string personType, string id1, string id2, string name, string lastName, string address, string countryCode, string regionCode, string locality, string bic, string radIban);

        string AddModifyBankAccount(string tokenId, string thirdParty, string bank, string currency, string iban);

        ResultMessageValueDto GetExchangeRate(string currency, string exchangeDate, string tipRet); // tipRet = OPER sau REEV

        string GetExchangeRateCtl(string currency, string exchangeDate, string tipRet, out decimal exchangeRate); // tipRet = OPER sau REEV

        ResultMessageIdDto ContaOperationSaveCtl(string operationDate, string documentNr, string documentDate, string currency, string activityType, int operType, int operationType);

        string ContaOperationDetailSaveCtl(int idOperation, string activityType, int operType, int operationType, int valueType, decimal value, string bankAccount, string explicatii);

        string ContaOperationDelete(int idOperation);

        string ContaOperationEndSave(int idOperation);

        ResultMessageIdDto ContaOperationSaveDirect(OperationAutoDirectDto operation);

        ResultMessageIdDto ContaOperationSave(OperationAutoDto operation);

        string LastBalanceDay();

        ResultMessageStringDto MyBankAccounts();

        void TestContaOperationSave();

        string GetSavedBalance(string savedBalanceDate);

        string IndicatoriReport(string dataRap);
    }
    [AbpAllowAnonymous]
    public class AutomatAppService : ErpAppServiceBase, IAutomatAppService
    {
        IRepository<Tokens> _tokensRepository;
        IAccountRepository _accountRepository;
        IAutomatRepository _automatRepository;
        IBalanceRepository _balanceRepository;
        IOperationRepository _operationRepository;
        IPersonRepository _personRepository;
        IExchangeRatesRepository _exchangeRatesRepository;
        ICurrencyRepository _currencyRepository;
        IRepository<Tenant> _tenantRepository;
        IRepository<BankAccount> _bankAccountRepository;
        IConfigReportRepository _configReportRepository;
        IRepository<Report> _reportRepository;
        ISavedBalanceRepository _savedBalanceRepository;
        IRepository<LegalPerson> _legalPersonRepository;
        IRepository<SavedBalanceViewDet> _savedBalanceViewDetRepository;
        ReportManager _reportManager;
        IRepository<SitFinanRap> _sitFinanRapRepository;
        ISitFinanCalcRepository _sitFinanCalcRepository;

        public AutomatAppService(IRepository<Tokens> tokensRepository, IAccountRepository accountRepository, IAutomatRepository automatRepository,
                                  IBalanceRepository balanceRepository, IOperationRepository operationRepository,
                                  IPersonRepository personRepository, IExchangeRatesRepository exchangeRatesRepository, ICurrencyRepository currencyRepository,
                                  IRepository<Tenant> tenantRepository, IRepository<BankAccount> bankAccountRepository, IConfigReportRepository configReportRepository,
                                  IRepository<Report> reportRepository, ISavedBalanceRepository savedBalanceRepository, IRepository<LegalPerson> legalPersonRepository, IRepository<SavedBalanceViewDet> savedBalanceViewDetRepository,
                                  ReportManager reportManager, IRepository<SitFinanRap> sitFinanRapRepository, ISitFinanCalcRepository sitFinanCalcRepository)
        {
            _tokensRepository = tokensRepository;
            _accountRepository = accountRepository;
            _automatRepository = automatRepository;
            _balanceRepository = balanceRepository;
            _operationRepository = operationRepository;
            _personRepository = personRepository;
            _exchangeRatesRepository = exchangeRatesRepository;
            _currencyRepository = currencyRepository;
            _tenantRepository = tenantRepository;
            _bankAccountRepository = bankAccountRepository;
            _configReportRepository = configReportRepository;
            _reportRepository = reportRepository;
            _savedBalanceRepository = savedBalanceRepository;
            _legalPersonRepository = legalPersonRepository;
            _savedBalanceViewDetRepository = savedBalanceViewDetRepository;
            _reportManager = reportManager;
            _sitFinanRapRepository = sitFinanRapRepository;
            _sitFinanCalcRepository = sitFinanCalcRepository;

        }

        public Tokens GetToken(string tokenId)
        {
            var token = _tokensRepository.FirstOrDefault(f => f.Token == tokenId);
            return token;
        }

        public int ValidateToken(string tokenId)
        {
            int rez = 0;
            var token = GetToken(tokenId);
            rez = (token == null) ? 1 : 0;
            return rez;
        }

        public int FindAccount(string tokenId, string partialSymbol, string externalCode, string accountFuncType, int exactAccount)
        {
            int _accountFunctType = int.Parse(accountFuncType);
            int rez = 1; // presupun ca nu exista
            var token = GetToken(tokenId);
            var x = _accountRepository.GetAll().Where(f => f.TenantId == token.TenantId
                                                && (f.ExternalCode == externalCode)
                                                && f.AccountFuncType == (AccountFuncType)_accountFunctType).ToList();
            var account = x.Where(f => ("#" + f.Symbol + ((exactAccount == 0) ? "#" : "")).IndexOf("#" + partialSymbol + ((exactAccount == 0) ? "#" : "")) >= 0).ToList();
            // iau doar conturile de ultim nivel
            var accountLastLevel = account.Where(f => f.ComputingAccount).ToList();

            var count = accountLastLevel.Count(f => ("#" + f.Symbol).IndexOf("#" + partialSymbol) >= 0);
            rez = (count > 0) ? 0 : 1;
            return rez;
        }

        public IEnumerable<GetAccountDto> GetAccount(string tokenId, string partialSymbol, string externalCode, string accountFuncType, int exactAccount)
        {
            int _accountFunctType = int.Parse(accountFuncType);
            var token = GetToken(tokenId);
            var x = _accountRepository.GetAll().Where(f => f.TenantId == token.TenantId
                                                && (f.ExternalCode == externalCode)
                                                && f.AccountFuncType == (AccountFuncType)_accountFunctType).ToList();
            var account = x.Where(f => ("#" + f.Symbol + ((exactAccount == 0) ? "#" : "")).IndexOf("#" + partialSymbol + ((exactAccount == 0) ? "#" : "")) >= 0).ToList();
            // iau doar conturile de ultim nivel
            var accountLastLevel = account.Where(f => f.ComputingAccount).ToList();

            var ret = accountLastLevel.Select(f => new GetAccountDto { Symbol = f.Symbol, AccountName = f.AccountName });
            return ret;
        }

        public string GetNewAccount(string tokenId, string partialSymbol, string accountName, string externalCode, int accountFuncType, int nrOfFigures)
        {
            string newAccount = "";
            var token = GetToken(tokenId);

            if (nrOfFigures != 0)
            {
                var x = _accountRepository.GetAll().Where(f => f.TenantId == token.TenantId
                                                   && f.AccountFuncType == (AccountFuncType)accountFuncType).ToList();
                var account = x.Where(f => ("#" + f.Symbol).IndexOf("#" + partialSymbol) >= 0).ToList()
                               .Select(f => new { symbol = f.Symbol }).ToList();

                var numericAccounts = new List<int>();
                foreach (var item in account)
                {
                    try
                    {
                        var num = int.Parse(item.symbol.Substring(item.symbol.IndexOf(partialSymbol) + partialSymbol.Length));
                        numericAccounts.Add(num);
                    }
                    catch
                    {

                    }
                }

                var newAccountFigure = 0;
                try
                {
                    newAccountFigure = numericAccounts.Max();
                }
                catch
                { }

                newAccountFigure = newAccountFigure + ((nrOfFigures == 0) ? 0 : 1);
                newAccount = partialSymbol + newAccountFigure.ToString().PadLeft(nrOfFigures, '0');
            }
            else
            {
                newAccount = partialSymbol;
            }

            return newAccount;
        }

        public string SaveNewAccount(string tokenId, string symbol, string synthetic, string accountName, string externalCode, string thirdPartyCif, string currency, int accountFuncType)
        {
            string message = "OK";
            var token = GetToken(tokenId);
            string result = _automatRepository.AddAccountAutomat(token, symbol, synthetic, accountName.Replace("$", " "), externalCode, thirdPartyCif, currency, accountFuncType);
            if (result != "OK")
            {
                message = result;
            }
            return message;
        }

        public string VerifyClosedMonth(string tokenId, string operationDate)
        {
            string message = "OK";
            var format = new System.Globalization.CultureInfo("fr-FR", true);
            DateTime _operationDate = DateTime.Parse(operationDate, format);
            var token = GetToken(tokenId);
            var x = _balanceRepository.GetAll().Count(f => f.BalanceDate >= _operationDate && f.Status == State.Active && f.TenantId == token.TenantId);
            message = (x == 0) ? "OK" : "Luna contabila este inchisa";

            return message;
        }

        // Add operation header
        public ResultMessageIdDto AddOperation(string tokenId, string operationDate, string documentNr, string documentDate, int closingMonth, int operationStatus, string currency, string documentType)
        {
            int idOperation = 0;
            var token = GetToken(tokenId);
            string message = "OK";
            try
            {
                var format = new System.Globalization.CultureInfo("fr-FR", true);
                DateTime _operationDate = DateTime.Parse(operationDate, format);
                DateTime _documentDate = DateTime.Parse(documentDate, format);
                bool _closingMonth = (closingMonth == 1);
                message = _automatRepository.AddOperation(token, _operationDate, documentNr, _documentDate, _closingMonth, operationStatus, currency, documentType, out idOperation);

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            var rez = new ResultMessageIdDto { Message = message, Id = idOperation };
            return rez;
        }

        // Add operation detail
        public string AddOperationDetail(string tokenId, int operationId, string debitSymbol, string creditSymbol, decimal value, decimal valueCurrency, string details)
        {
            var token = GetToken(tokenId);
            string message = "OK";
            try
            {
                message = _automatRepository.AddOperationDetails(token, operationId, debitSymbol, creditSymbol, value, valueCurrency, details.Replace("$", " "));
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return message;
        }

        // delete operation
        public string DeleteOperation(string tokenId, int idOperation)
        {
            var token = GetToken(tokenId);
            string message = "OK";
            try
            {
                message = _automatRepository.DeleteOperation(token, idOperation);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return message;
        }

        public IEnumerable<GetAccountListDto> GetAccounts(string tokenId, string synthetic, string includeSynthetic)
        {
            var token = GetToken(tokenId);
            try
            {
                var x = _accountRepository.GetAccounts(synthetic, includeSynthetic).ToList()
                                          .Select(f => new GetAccountListDto { SyntheticAccount = (f.SyntheticAccount == null ? f.Symbol : f.SyntheticAccount.Symbol), Symbol = f.Symbol });
                return x;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetSoldAccount(string tokenId, string account, DateTime date, out decimal sold)
        {
            var message = "OK";
            int localCurrencyId = 1;
            sold = 0;
            User user; Tenant tenant;
            var token = GetToken(tokenId);

            _automatRepository.getCredentials(token, out tenant, out user);
            int selectedAppClientId = tenant.Id;
            var vAccount = _accountRepository.GetAll().FirstOrDefault(f => f.Symbol == account && f.TenantId == tenant.Id);
            var currencyId = tenant.LocalCurrencyId.Value;
            sold = _balanceRepository.GetSoldAccount(date, vAccount, selectedAppClientId, localCurrencyId, localCurrencyId, true);
            return message;
        }

        public string ExchangeRatesAddModify(string currencyCode, DateTime exchangeDate, decimal value)
        {
            var message = "OK";

            message = _automatRepository.ExchangeRatesAddModify(currencyCode, exchangeDate, value);
            return message;
        }

        public string AddModifyThirdParty(string tokenId, string personType, string id1, string id2, string name, string lastName, string address, string countryCode, string regionCode, string locality)
        {
            var message = "OK";
            //try
            //{
            //    User user; Tenant tenant;
            //    var token = GetToken(tokenId);

            //    _automatRepository.getCredentials(token, out tenant, out user);
            //    int selectedAppClientId = tenant.Id;
            //    int personId = 0;
            //    // Add/modify person 
            //    message = AddModifyPerson(personType, id1, id2, name, lastName, address, countryCode, regionCode, locality, selectedAppClientId, out personId);

            //    // Add / modify thirdParty
            //    P thirdParty = new ThirdParty { PersonId = personId, IsClient = false, IsProvider = false, IsOther = false };
            //    _personRepository.ThirdPartyInsertOrUpdate(thirdParty);


            //}
            //catch (Exception ex)
            //{
            //    return ex.Message;
            //}

            return message;
        }

        public string AddModifyBank(string tokenId, string personType, string id1, string id2, string name, string lastName, string address, string countryCode, string regionCode, string locality, string bic, string radIban)
        {
            var message = "OK";
            //try
            //{
            //    User user; AppClient appClient;
            //    var token = GetToken(tokenId);

            //    _automatRepository.getCredentials(token, out appClient, out user);
            //    int selectedAppClientId = appClient.Id;
            //    int personId = 0;
            //    // Add/modify person 
            //    message = AddModifyPerson(personType, id1, id2, name, lastName, address, countryCode, regionCode, locality, selectedAppClientId, out personId);

            //    // Add / modify thirdParty
            //    Bank bank = new Bank { LegalPersonId = personId, Bic = bic, IbanAbrv = radIban };
            //    _personRepository.BankInsertOrUpdate(bank);


            //}
            //catch (Exception ex)
            //{
            //    return ex.Message;
            //}

            return message;
        }

        private string AddModifyPerson(string personType, string id1, string id2, string name, string lastName, string address, string countryCode, string regionCode, string locality, int appClientId, out int personId)
        {
            personId = 0;
            string message = "OK";

            //var _personDto = new PersonEditDto();
            //var _naturalPerson = new NaturalPerson();
            //var _legalPerson = new LegalPerson();

            //try
            //{
            //    // verific existenta persoanei
            //    var personDb = _personRepository.FirstOrDefault(f => f.Id1 == id1 && f.DefinedById == appClientId);
            //    if (personDb != null) // exista => fac update
            //    {
            //        _naturalPerson = _personRepository.GetNaturalPersonById(personDb.Id);
            //        if (_naturalPerson != null) // e natural Person
            //        {
            //            _personDto = Mapper.Map<PersonEditDto>(_naturalPerson);
            //            _personDto.PersonType = "NP";
            //        }
            //        else
            //        {
            //            _legalPerson = _personRepository.GetLegalPersonById(personDb.Id);
            //            _personDto = Mapper.Map<PersonEditDto>(_legalPerson);
            //            _personDto.PersonType = "LP";
            //        }
            //    }

            //    // actualizez valorile
            //    _personDto.PersonType = (personType == "F") ? "NP" : "LP";
            //    _personDto.Name = ((personType == "F") ? "" : name).Replace('$', ' ');
            //    _personDto.FirstName = ((personType == "F") ? name : "").Replace('$', ' ');
            //    _personDto.LastName = ((personType == "F") ? lastName : "").Replace('$', ' ');
            //    _personDto.Id1 = id1;
            //    _personDto.Id2 = id2;
            //    _personDto.AddressStreet = address.Replace('$', ' '); ;
            //    var country = _personRepository.CountryList().FirstOrDefault(f => f.CountryAbrv == countryCode);
            //    _personDto.AddressCountryId = (country != null) ? country.Id : (int?)null;
            //    var region = _personRepository.RegionList(_personDto.AddressCountryId ?? 0).FirstOrDefault(f => f.RegionAbrv == regionCode);
            //    _personDto.AddressRegionId = (region != null) ? region.Id : (int?)null;
            //    _personDto.AddressLocality = locality.Replace('$', ' ');
            //    _personDto.DefinedById = appClientId;

            //    // insert sau update
            //    if (_personDto.IsNaturalPerson == true) // Natural Person
            //    {
            //        var _person = Mapper.Map<NaturalPerson>(_personDto);
            //        if (_person.Id == 0)
            //        {
            //            _person.DefinedById = appClientId;
            //            _personRepository.Insert(_person);
            //        }
            //        else
            //        {
            //            _automatRepository.UpdateNaturalPerson(_person);
            //        }
            //        CurrentUnitOfWork.SaveChanges();
            //        personId = _person.Id;
            //    }
            //    else //Legal Person
            //    {
            //        var _person = Mapper.Map<LegalPerson>(_personDto);
            //        if (_person.Id == 0)
            //        {
            //            _person.DefinedById = appClientId;
            //            _personRepository.Insert(_person);
            //        }
            //        else
            //        {
            //            _automatRepository.UpdateLegalPerson(_person);
            //        }
            //        CurrentUnitOfWork.SaveChanges();
            //        personId = _person.Id;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    return ex.Message;
            //}
            return message;
        }

        public string AddModifyBankAccount(string tokenId, string thirdParty, string bank, string currency, string iban)
        {
            string message = "OK";
            User user; Tenant tenant;
            var token = GetToken(tokenId);

            _automatRepository.getCredentials(token, out tenant, out user);
            int selectedAppClientId = tenant.Id;

            message = _automatRepository.AddModifyBankAccount(tokenId, thirdParty, bank, currency, iban, selectedAppClientId);
            return message;
        }

        public ResultMessageValueDto GetExchangeRate(string currency, string exchangeDate, string tipRet) // tipRet = OPER sau REEV
        {
            var ret = new ResultMessageValueDto();
            decimal exchangeRate = 0;
            string message = "OK";
            try
            {
                var format = new System.Globalization.CultureInfo("ro-RO", true);
                DateTime _exchangeDate = DateTime.Parse(exchangeDate, format);
                var currencyObj = _currencyRepository.GetAll().FirstOrDefault(f => f.CurrencyCode == currency);
                var localCurrnecyId = _personRepository.GetLocalCurrency(1);

                if (tipRet == "OPER")
                {
                    exchangeRate = _exchangeRatesRepository.GetExchangeRateForOper(_exchangeDate, currencyObj.Id, localCurrnecyId);
                }
                else
                {
                    exchangeRate = _exchangeRatesRepository.GetExchangeRate(_exchangeDate, currencyObj.Id, localCurrnecyId);
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            ret.Message = message;
            ret.Value = exchangeRate;
            return ret;
        }

        public string GetExchangeRateCtl(string currency, string exchangeDate, string tipRet, out decimal exchangeRate) // tipRet = OPER sau REEV
        {
            exchangeRate = 0;
            string message = "OK";
            try
            {
                var format = new System.Globalization.CultureInfo("fr-FR", true);
                DateTime _exchangeDate = DateTime.Parse(exchangeDate, format);
                var currencyObj = _currencyRepository.GetAll().FirstOrDefault(f => f.CurrencyCode == currency);
                var localCurrnecyId = _personRepository.GetLocalCurrency(1);

                if (tipRet == "OPER")
                {
                    exchangeRate = _exchangeRatesRepository.GetExchangeRateForOper(_exchangeDate, currencyObj.Id, localCurrnecyId);
                }
                else
                {
                    exchangeRate = _exchangeRatesRepository.GetExchangeRate(_exchangeDate, currencyObj.Id, localCurrnecyId);
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            var rez = message;
            return rez;
        }

        public ResultMessageIdDto ContaOperationSaveCtl(string operationDate, string documentNr, string documentDate, string currency, string activityType, int operType, int operationType)
        {
            int idOperation = 0;
            string message = "OK";
            try
            {
                var format = new System.Globalization.CultureInfo("fr-FR", true);
                DateTime _operationDate = DateTime.Parse(operationDate, format);
                DateTime _documentDate = DateTime.Parse(documentDate, format);
                message = _automatRepository.ContaOperationSave(_operationDate, documentNr, _documentDate, currency, activityType, operType, operationType, out idOperation);

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            var rez = new ResultMessageIdDto { Message = message, Id = idOperation };
            return rez;
        }

        public string ContaOperationDetailSaveCtl(int idOperation, string activityType, int operType, int operationType, int valueType, decimal value, string bankAccount, string explicatii)
        {
            string message = "OK";
            try
            {
                explicatii = explicatii.Replace("_", " ");
                message = _automatRepository.ContaOperationDetailSave(idOperation, activityType, operType, operationType, valueType, value, bankAccount, explicatii, false);

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return message;
        }

        public string ContaOperationDelete(int idOperation)
        {
            string message = "OK";
            try
            {
                message = _automatRepository.ContaOperationDelete(idOperation);

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return message;
        }

        public string ContaOperationEndSave(int idOperation)
        {
            string message = "OK";
            try
            {
                message = _automatRepository.ContaOperationEndSave(idOperation);

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return message;
        }

        public ResultMessageIdDto ContaOperationSaveDirect(OperationAutoDirectDto operation)
        {
            int idOperation = 0;
            string message = "OK";
            var rez = new ResultMessageIdDto { Message = message, Id = idOperation };


            try
            {
                var format = new System.Globalization.CultureInfo("fr-FR", true);
                DateTime _operationDate = DateTime.Parse(operation.OperationDate, format);
                DateTime _documentDate = DateTime.Parse(operation.DocumentDate, format);
                message = _automatRepository.ContaOperationSaveDirect(_operationDate, operation.DocumentNr, _documentDate, operation.Currency, operation.DocumentType, out idOperation);
                if (message != "OK")
                {
                    rez.Message = message;
                    return rez;
                }
                rez.Id = idOperation;

                foreach (var item in operation.Details)
                {
                    var explicatii = item.Explicatii.Replace("_", " ");
                    message = _automatRepository.ContaOperationDetailSaveDirect(idOperation, item.Debit, item.Credit, item.Value, explicatii);
                    if (message != "OK")
                    {
                        rez.Message = message;
                        message = _automatRepository.ContaOperationDelete(idOperation);
                        return rez;
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            rez.Message = message;
            return rez;
        }

        public ResultMessageIdDto ContaOperationSave(OperationAutoDto operation)
        {
            int idOperation = 0;
            string message = "OK";
            var rez = new ResultMessageIdDto();
            rez.Id = idOperation;

            try
            {
                var format = new System.Globalization.CultureInfo("fr-FR", true);
                DateTime _operationDate = DateTime.Parse(operation.OperationDate, format);
                DateTime _documentDate = DateTime.Parse(operation.DocumentDate, format);
                string activityType = operation.ActivityType == "SGD" ? "FGDB" : operation.ActivityType;
                message = _automatRepository.ContaOperationSave(_operationDate, operation.DocumentNr, _documentDate, operation.Currency, activityType, operation.OperType, operation.OperationType, out idOperation);

                if (message != "OK")
                {
                    rez.Message = message;
                    return rez;
                }

                foreach (var item in operation.Details)
                {
                    var explicatii = item.Explicatii.Replace("_", " ");
                    message = _automatRepository.ContaOperationDetailSave(idOperation, activityType, operation.OperType, operation.OperationType, item.ValueType, item.Value, item.Bank, explicatii, item.Storno);
                    if (message != "OK")
                    {
                        rez.Message = message;
                        message = _automatRepository.ContaOperationDelete(idOperation);
                        return rez;
                    }
                }

                message = _automatRepository.ContaOperationEndSave(idOperation);
                if (message != "OK")
                {
                    rez.Message = message;
                    return rez;
                }
            }
            catch (Exception ex)
            {
                message = LazyMethods.GetErrMessage(ex);
            }

            rez.Message = message;
            rez.Id = idOperation;
            return rez;
        }

        [HttpGet]
        public string LastBalanceDay()
        {
            string rez = "";

            var lastBalanceDate = _balanceRepository.GetAll().Where(f => f.Status == State.Active).Select(f => f.BalanceDate).OrderByDescending(f => f).FirstOrDefault();
            rez = LazyMethods.DateToString(lastBalanceDate);
            return rez;
        }

        [HttpGet]
        public ResultMessageStringDto MyBankAccounts()
        {
            var result = new ResultMessageStringDto { Message = "OK" };
            var personId = _tenantRepository.FirstOrDefault(f => f.Id == 1).LegalPersonId;
            var bankAccountList = _bankAccountRepository.GetAllIncluding(f => f.Bank, f => f.Currency).Where(f => f.PersonId == personId)
                                                        .Select(f => new { IBAN = f.IBAN, Bank = f.Bank.IbanAbrv, Currency = f.Currency.CurrencyName })
                                                        .OrderBy(f => f.Bank).ThenBy(f => f.Currency).ToList();
            string bankAccountSer = JsonSerializer.Serialize(bankAccountList);

            result.Value = bankAccountSer;

            return result;
        }

        [HttpGet]
        public ResultSitResurseDto SitResurse(string reportDate)
        {
            string message = "OK";
            var calculReport = new ReportCalc();
            var format = new System.Globalization.CultureInfo("fr-FR", true);
            DateTime _reportDate = DateTime.Parse(reportDate, format);

            var balance = _balanceRepository.GetAll().FirstOrDefault(f => f.BalanceDate == _reportDate && f.Status == State.Active);
            if (balance == null)
            {
                message = "Pentru data " + reportDate + " nu exista o balanta calculata";
                var ret = new ResultSitResurseDto { FGcuProfitCurent = 0, FRcuProfitCurent = 0, Message = message };
                return ret;
            }

            var reportId = _configReportRepository.GetAllIncluding(f => f.Report).FirstOrDefault(f => f.Report.ReportSymbol == "A").ReportId;
            try
            {
                calculReport = CalculRaport(reportId, _reportDate);
            }
            catch (Exception ex)
            {
                message = LazyMethods.GetErrMessage(ex);
            }


            var fgProfitCurrentValue = Math.Round(calculReport.ReportCalcItems.FirstOrDefault(f => f.RowCode == 4).RowValue, 2);
            var frProfitCurentValue = Math.Round(calculReport.ReportCalcItems.FirstOrDefault(f => f.RowCode == 9).RowValue, 2);

            var result = new ResultSitResurseDto { FGcuProfitCurent = fgProfitCurrentValue, FRcuProfitCurent = frProfitCurentValue, Message = message };
            return result;
        }

        private ReportCalc CalculRaport(int reportId, DateTime reportDate)
        {
            try
            {
                var appClient = 1;
                var personId = _tenantRepository.FirstOrDefault(f => f.Id == appClient).LegalPersonId;

                var report = new ReportCalc();
                report.ReportCalcItems = new List<ReportCalcItem>();

                var calcRap = new List<ReportCalcItem>();

                calcRap = CalcRaportNoTotal(reportId, reportDate);

                var ret = calcRap.OrderBy(f => f.OrderView).ToList();
                report.ReportCalcItems.AddRange(ret);
                return report;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<ReportCalcItem> CalcRaportNoTotal(int reportId, DateTime reportDate)
        {
            try
            {
                var report = _reportRepository.FirstOrDefault(f => f.Id == reportId);
                var rowList = _configReportRepository.GetAll().Where(f => f.ReportId == reportId).OrderBy(f => f.OrderView).ToList();
                var rapoarteList = _reportRepository.GetAll().Where(f => f.State == State.Active && f.ReportInitId == report.ReportInitId).ToList();
                var calcRap = new List<ReportCalcItem>();
                var appClientId = 1;
                var localCurrencyId = 1;

                var startDate = LazyMethods.FirstDayNextMonth(reportDate.AddMonths(-1));
                var contaOperList = _balanceRepository.ContaOperationList(startDate, reportDate, appClientId, 0, localCurrencyId, true);
                var balance = _balanceRepository.GetBalanceAnyDate(reportDate, false, appClientId, localCurrencyId, true);

                // initializez clasa de calcul
                try
                {
                    calcRap = rowList.Select(f => new ReportCalcItem
                    {
                        ReportDate = reportDate,
                        RowName = f.RowName,
                        RowValue = 0,
                        RowCode = f.RowCode,
                        OrderView = f.OrderView,
                        ReportConfigRowId = f.Id,
                        Bold = f.Bold
                    }).ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                //calculez valorile
                foreach (var item in rowList.Where(f => f.TotalRow == false).OrderBy(f => f.OrderView))
                {
                    calcRap = _configReportRepository.CalcReportValue(item, 1, reportId, calcRap, reportDate, appClientId, 0, localCurrencyId, rapoarteList, false, contaOperList, balance);
                }

                // calculez totaluri
                foreach (var item in rowList.Where(f => f.TotalRow == true).OrderBy(f => f.OrderView))
                {
                    calcRap = _configReportRepository.CalcReportValue(item, 1, reportId, calcRap, reportDate, appClientId, 0, localCurrencyId, rapoarteList, false, contaOperList, balance);
                }
                var ret = calcRap.OrderBy(f => f.OrderView).ToList();
                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void TestContaOperationSave()
        {
            var operation = new OperationAutoDto
            {
                OperationDate = "28.03.2022",
                DocumentNr = "15424",
                DocumentDate = "28.03.2022",
                Currency = "RON",
                ActivityType = "SGD",
                OperType = 13,
                OperationType = 0
            };

            var list = new List<OperationAutoDetailDto>();
            var detail = new OperationAutoDetailDto
            {
                Bank = "FNNB",
                Explicatii = "Contributii Anuale",
                Value = 1834403,
                ValueType = 0,
                Storno = false
            };

            list.Add(detail);

            operation.Details = list;

            ContaOperationSave(operation);

        }

        public string GetSavedBalance(string savedBalanceDate)
        {
            var format = new System.Globalization.CultureInfo("fr-FR", true);
            DateTime _savedBalanceDate = DateTime.Parse(savedBalanceDate, format);

            var balView = new SavedBalanceViewDto();
            var maxBalanceDate = _savedBalanceRepository.GetAllIncluding(f => f.SavedBalanceDetails).Where(f => f.SaveDate <= _savedBalanceDate).ToList().Max(f => f.SaveDate);
            var savedBalanceId = _savedBalanceRepository.GetAllIncluding(f => f.SavedBalanceDetails).Where(f => f.SaveDate == maxBalanceDate).ToList().Max(f => f.Id);
            balView = _reportManager.GetSavedBalanceViewList(savedBalanceId, null, null, 0);

            string balViewStr = JsonSerializer.Serialize(balView);

            return balViewStr;
        }

        [HttpGet]
        public string IndicatoriReport(string dataRap)
        {
            var format = new System.Globalization.CultureInfo("fr-FR", true);
            DateTime _dataRap = DateTime.Parse(dataRap, format);
            
            var sitFinanRap = new SitFinanReportModel();
            var maxBalanceDate = _savedBalanceRepository.GetAllIncluding(f => f.SavedBalanceDetails).Where(f => f.SaveDate <= _dataRap).ToList().Max(f => f.SaveDate);
            var savedBalanceId = _savedBalanceRepository.GetAllIncluding(f => f.SavedBalanceDetails).Where(f => f.SaveDate == maxBalanceDate).ToList().Max(f => f.Id);
            var reportId = _sitFinanRapRepository.GetAll().FirstOrDefault(f => f.ReportSymbol == "G").Id;
            var colNumber = _sitFinanRapRepository.GetAll().FirstOrDefault(f => f.ReportSymbol == "G").NrCol;
            sitFinanRap = _reportManager.SitFinanRapIndicatori(savedBalanceId, reportId);

            string sitFinanRapStr = JsonSerializer.Serialize(sitFinanRap);

            return sitFinanRapStr;
        }
    }
}
