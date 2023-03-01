using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Niva.EntityFramework.Repositories.Nomenclatures;
using Niva.Erp.Authorization.Users;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Models.Administration;
using Niva.Erp.Models.AutoOperation;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Contracts;
using Niva.Erp.Models.Setup;
using Niva.Erp.MultiTenancy;
using Niva.Erp.Repositories.Conta.AutoOperation;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Niva.Erp.EntityFrameworkCore.Repositories.Conta.AutoOperation
{
    public class AutomatRepository : ErpRepositoryBase<AutoOperationOper, int>, IAutomatRepository
    {
        AutoOperationRepository _autoOperationRepository;
        AccountRepository _accountRepository;
        PersonRepository _personRepository;
        int _tenantId;

        public AutomatRepository(IDbContextProvider<ErpDbContext> context) : base(context)
        {
            _autoOperationRepository = new AutoOperationRepository(context);
            _accountRepository = new AccountRepository(context);
            _personRepository = new PersonRepository(context);
            _tenantId = 1;
        }

        private void getCredentials(ErpDbContext context, Tokens token, out Tenant tenant, out User user)
        {
            var vAppClient = context.Tenants.FirstOrDefault(f => f.Id == token.TenantId);
            user = context.Users.FirstOrDefault(f => f.Id == token.User.Id);
            var currency = Context.Currency.FirstOrDefault(f => f.CurrencyCode == vAppClient.LocalCurrency.CurrencyCode);
            tenant = vAppClient;
        }

        public void getCredentials(Tokens token, out Tenant tenant, out User user)
        {
            getCredentials(Context, token, out tenant, out user);
        }

        public string AddAccountAutomat(Tokens token, string symbol, string synthetic, string accountName, string externalCode, string thirdPartyCif, string currency, int accountFuncType)
        {
            var account = new Account();
            User user; Tenant tenant;
            getCredentials(Context, token, out tenant, out user);
            int selectedAppClient = tenant.Id;

            if (thirdPartyCif != null)
            {
                var thirdParty = Context.Persons
                                        .FirstOrDefault(f => f.Id1 == thirdPartyCif && f.TenantId == selectedAppClient);
                if (thirdParty != null) account.ThirdParty = thirdParty;
            }
            var currencyItem = Context.Currency.FirstOrDefault(f => f.CurrencyCode == currency);
            if (currencyItem != null) account.Currency = currencyItem;

            account.Symbol = symbol;

            var accountSynthetic = Context.Account.Where(f => f.TenantId == selectedAppClient && f.Symbol == synthetic).FirstOrDefault();

            account.SyntheticAccountId = accountSynthetic.Id;
            account.ExternalCode = externalCode;
            account.AccountTypes = (accountSynthetic == null) ? ((AccountTypes)0) : accountSynthetic.AccountTypes;
            account.AccountFuncType = (AccountFuncType)accountFuncType;
            account.AccountName = accountName;
            account.ComputingAccount = true;

            account.TenantId = selectedAppClient;
            account.Status = State.Active;
            //account.Now(user);
            var check = AccountCheck(account, OperationType.Add, selectedAppClient);
            if (check == "OK")
            {
                Context.Account.Add(account);
                Context.SaveChanges();
                check = "OK";
            }
            return check;
        }

        private string AccountCheck(Account account, OperationType operationType, int selectedAppClient)
        {
            if ((operationType == OperationType.Add) || (operationType == OperationType.Modify))
            {
                if (account.AccountName == null) return "Nu ati completat numele contului";
                if (account.SyntheticAccount == null) return "Nu ati completat contul sintetic";
            }

            switch (operationType)
            {
                case (OperationType.Add):
                    {
                        var count = Context.Account
                            .Count(f => (f.Symbol ?? "-") == (account.Symbol ?? "-")
                                && f.Status == State.Active
                                && f.TenantId == selectedAppClient);
                        if (count != 0)
                        { return "Contul este deja definit"; }

                        break;
                    }
                case (OperationType.Modify):
                    {
                        var count = Context.Account
                            .Count(f => (f.Symbol ?? "-") == (account.Symbol ?? "-")
                                && f.Status == State.Active
                                && f.TenantId == selectedAppClient
                                && f.Id != account.Id);
                        if (count != 0)
                        { return "Contul este deja definit"; }

                        break;
                    }
                case (OperationType.Delete):
                    {
                        var count = Context.OperationsDetails
                            .Count(f => f.Debit.Id == account.Id);
                        if (count != 0) { return "Acest cont este folosit la inregistrarea unei operatii"; }
                        count = Context.OperationsDetails
                            .Count(f => f.Credit.Id == account.Id);
                        if (count != 0) { return "Acest cont este folosit la inregistrarea unei operatii"; }

                        break;
                    }
            }
            return "OK";
        }

        private static bool IsAlphaNumeric(string strToCheck)
        {
            Regex rg = new Regex(@"^[a-zA-Z0-9]*$");
            return rg.IsMatch(strToCheck);
        }

        public string AddOperation(Tokens token, DateTime operationDate, string documentNr, DateTime documentDate, bool closingMonth, int operationStatus, string currency, string documentType, out int idOperation)
        {
            idOperation = 0;
            var operation = new Operation();
            User user; Tenant tenant;
            getCredentials(Context, token, out tenant, out user);
            int selectedAppClient = tenant.Id;

            operation.OperationDate = operationDate;
            var documentTypeItem = Context.DocumentType.FirstOrDefault(f => f.TypeNameShort == documentType && f.TenantId == tenant.Id);
            if (documentTypeItem != null) operation.DocumentType = documentTypeItem;

            //string _documentNr = _autoOperationRepository.GetDocumentNumber(selectedAppClient, documentTypeItem, documentNr, operationDate);
            operation.DocumentNumber = documentNr;// _documentNr;
            operation.DocumentDate = documentDate;
            operation.ClosingMonth = closingMonth;
            operation.OperationStatus = (OperationStatus)operationStatus;
            operation.ExternalOperation = true;
            var currencyItem = Context.Currency.FirstOrDefault(f => f.CurrencyCode == currency);
            if (currencyItem != null) operation.Currency = currencyItem;

            operation.TenantId = tenant.Id;

            operation.State = State.Active;
            //operation.Now(user);
            Context.Operations.Add(operation);
            Context.SaveChanges();
            idOperation = operation.Id;
            return "OK";
        }

        public string AddOperationDetails(Tokens token, int operationId, string debitSymbol, string creditSymbol, decimal value, decimal valueCurrency, string details)
        {
            var detail = new OperationDetails();
            User user; Tenant tenant;
            getCredentials(Context, token, out tenant, out user);
            int selectedAppClient = tenant.Id;
            int localCurrency = tenant.LocalCurrencyId.Value;
            var operation = Context.Operations.FirstOrDefault(f => f.Id == operationId);

            var document = Context.DocumentType.FirstOrDefault(f => f.Id == operation.DocumentTypeId);

            var debitAccount = _accountRepository.FirstOrDefault(f => f.Symbol == debitSymbol && f.TenantId == selectedAppClient);
            if (debitAccount == null) return "Contul " + debitSymbol + " nu este definit";
            var creditAccount = _accountRepository.FirstOrDefault(f => f.Symbol == creditSymbol && f.TenantId == selectedAppClient);
            if (creditAccount == null) return "Contul " + creditSymbol + " nu este definit";
            detail.Debit = debitAccount;
            detail.Credit = creditAccount;
            detail.Details = details;
            detail.Value = value;
            detail.ValueCurr = (localCurrency == operation.CurrencyId) ? 0 : valueCurrency;

            // deductibilitate
            //var debitAccountDeduc = Context.AccountTaxProperty
            //                                                .Where(f => f.AccountId == detail.DebitId && f.PropertyDate <= operation.OperationDate)
            //                                                .OrderByDescending(f => f.PropertyDate)
            //                                                .FirstOrDefault();
            //decimal debitDeducProc = (debitAccountDeduc == null) ? 0 : debitAccountDeduc.PropertyValue;
            //decimal debitDeducValue = Math.Round(detail.Value * debitDeducProc, 2);

            //var creditAccountDeduc = Context.AccountTaxProperty
            //                                                .Where(f => f.AccountId == detail.CreditId && f.PropertyDate <= operation.OperationDate)
            //                                                .OrderByDescending(f => f.PropertyDate)
            //                                                .FirstOrDefault();
            //decimal creditDeducProc = (creditAccountDeduc == null) ? 0 : creditAccountDeduc.PropertyValue;
            //decimal creditDeducValue = Math.Round(detail.Value * creditDeducProc, 2);

            //detail.DebitValueDeduct = debitDeducValue;
            //detail.CreditValueDeduct = creditDeducValue;

            if (document.AutoNumber == true)
            {
                var detailNr = Context.OperationsDetails.Count(f => f.OperationId == operation.Id) + 1;
                detail.DetailNr = detailNr;
            }

            operation.OperationsDetails.Add(detail);
            Context.SaveChanges();

            return "OK";
        }

        public string DeleteOperation(Tokens token, int idOperation)
        {
            User user; Tenant tenant;
            getCredentials(Context, token, out tenant, out user);
            var operation = Context.Operations.FirstOrDefault(f => f.Id == idOperation);

            operation.State = State.Inactive;
            Context.SaveChanges();
            return "OK";
        }

        public IQueryable<Account> GetAccounts(Tokens token, string synthetic, string includeSynthetic)
        {
            User user; Tenant tenant;
            getCredentials(Context, token, out tenant, out user);
            int selectedAppClient = tenant.Id;
            var x = Context.Account.Include(f => f.SyntheticAccount).Where(f => f.TenantId == selectedAppClient && f.SyntheticAccount.Symbol == synthetic)
                    .OrderByDescending(f => f.Symbol);
            return x;
        }

        public string ExchangeRatesAddModify(string currencyCode, DateTime exchangeDate, decimal value)
        {
            try
            {
                OperationType operationType = OperationType.Add;

                var x = Context.ExchangeRates.FirstOrDefault(f => f.Currency.CurrencyCode == currencyCode && f.ExchangeDate == exchangeDate);
                ExchangeRates item;
                if (x == null) // add
                {
                    item = new ExchangeRates();
                }
                else
                {
                    operationType = OperationType.Modify;
                    item = x;
                }
                item.Currency = Context.Currency.FirstOrDefault(f => f.CurrencyCode == currencyCode);
                item.ExchangeDate = exchangeDate;
                item.Value = value;
                if (operationType == OperationType.Add)
                {
                    Context.ExchangeRates.Add(item);
                }
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return "OK";
        }

        public void UpdateNaturalPerson(NaturalPerson person)
        {
            var personDB = Context.NaturalPersons.FirstOrDefault(f => f.Id == person.Id);

            Context.Entry(personDB).CurrentValues.SetValues(person);
        }

        public void UpdateLegalPerson(LegalPerson person)
        {
            var personDB = Context.LegalPersons.FirstOrDefault(f => f.Id == person.Id);

            Context.Entry(personDB).CurrentValues.SetValues(person);
        }

        public string AddModifyBankAccount(string tokenId, string thirdParty, string bank, string currency, string iban, int appClientId)
        {
            string message = "OK";
            int thirdPartyId = 0, bankId = 0, currencyId = 0;

            var thirdPartyObj = Context.Persons.FirstOrDefault(f => f.Id1 == thirdParty && f.TenantId == appClientId);
            if (thirdPartyObj != null) thirdPartyId = thirdPartyObj.Id;

            var bankObj = Context.Issuer.Include(f => f.LegalPerson).FirstOrDefault(f => f.LegalPerson.Id1 == bank && f.TenantId == appClientId);
            if (bankObj != null) bankId = bankObj.LegalPersonId;

            if (currency == null || currency == "")
            {
                currencyId = Context.Tenants.FirstOrDefault(f => f.Id == appClientId).LocalCurrencyId.Value;
            }
            else
            {
                var currencyObj = Context.Currency.FirstOrDefault(f => f.CurrencyCode == currency);
                if (currencyObj != null) currencyId = currencyObj.Id;
            }

            if (thirdPartyId != 0 && bankId != 0 && currencyId != 0 && iban != null)
            {
                BankAccount bankAccount;
                bankAccount = Context.BankAccount.FirstOrDefault(f => f.PersonId == thirdPartyId && f.BankId == bankId && f.CurrencyId == currencyId);
                if (bankAccount == null)
                    bankAccount = new BankAccount();

                bankAccount.PersonId = thirdPartyId;
                bankAccount.BankId = bankId;
                bankAccount.CurrencyId = currencyId;
                bankAccount.IBAN = iban;

                if (bankAccount.Id == 0)
                {
                    Context.BankAccount.Add(bankAccount);
                }
                Context.SaveChanges();

            }

            return message;
        }

        public string ContaOperationSave(DateTime operationDate, string documentNr, DateTime documentDate, string currency, string activityType, int operType, int operationType, out int idOperation)
        {
            idOperation = 0;
            int documentTypeId = 0;
            int currencyId = 0;

            // verificare luna inchisa
            var count = Context.Balance.Count(f => f.TenantId == _tenantId && f.Status == State.Active && f.BalanceDate >= operationDate);
            if (count != 0)
            {
                return "Operatia nu se poate inregistra. Luna contabila este inchisa";
            }

            var autoOperationType = (AutoOperationType)operType;
            var operationConfig = Context.AutoOperationSearchConfig.FirstOrDefault(f => f.TenantId == _tenantId && f.AutoOperType == autoOperationType
                                                                                   && f.OperationType == operationType);
            if (operationConfig == null)
            {
                return "Nu am identificat monografia pentru aceasta operatie. Verificati monografiile definite in aplicatia Conta";
            }

            if (operationConfig.DocumentTypeId == null)
            {
                return "Nu ati specificat tipul documentului la definirea monografiei. Verificati monografiile definite in aplicatia Conta";
            }
            documentTypeId = operationConfig.DocumentTypeId.Value;

            if (currency == null || currency == "")
            {
                return "Nu ati specificat moneda operatiei";
            }
            var currencyObj = Context.Currency.FirstOrDefault(f => f.CurrencyCode == currency);
            if (currencyObj == null)
            {
                return "Nu am identificat moneda " + currency;
            }
            currencyId = currencyObj.Id;

            var operation = new Operation
            {
                TenantId = _tenantId,
                OperationDate = operationDate,
                DocumentNumber = documentNr,
                DocumentDate = documentDate,
                ClosingMonth = false,
                OperationStatus = OperationStatus.Unchecked,
                ExternalOperation = true,
                CurrencyId = currencyId,
                DocumentTypeId = documentTypeId,
                State = State.Active
            };
            Context.Operations.Add(operation);
            Context.SaveChanges();

            var localCurrencyId = Context.Tenants.FirstOrDefault(f => f.Id == _tenantId).LocalCurrencyId;
            var operationChild = new Operation(); // e folosit in cazul in care avem documente in valuta
            if (localCurrencyId != operation.CurrencyId)
            {
                operationChild = new Operation
                {
                    TenantId = _tenantId,
                    CurrencyId = localCurrencyId.Value,
                    OperationDate = operation.OperationDate,
                    DocumentTypeId = operation.DocumentTypeId,
                    DocumentNumber = operation.DocumentNumber,
                    DocumentDate = operation.DocumentDate,
                    OperationStatus = operation.OperationStatus,
                    State = State.Active,
                    ExternalOperation = true,
                    ClosingMonth = false,
                    OperationParentId = operation.Id
                };
                Context.Operations.Add(operationChild);
            }

            Context.SaveChanges();
            idOperation = operation.Id;
            return "OK";
        }

        public string ContaOperationDelete(int idOperation)
        {
            try
            {
                var operation = Context.Operations.FirstOrDefault(f => f.Id == idOperation && f.State == State.Active);

                if (operation == null)
                {
                    return "Nu am identificat operatia contabila cu id-ul " + idOperation.ToString();
                }

                // verificare luna inchisa
                var count = Context.Balance.Count(f => f.TenantId == _tenantId && f.Status == State.Active && f.BalanceDate >= operation.OperationDate);
                if (count != 0)
                {
                    return "Operatia nu se poate sterge. Luna contabila este inchisa";
                }

                Context.OperationsDetails.RemoveRange(Context.OperationsDetails.Where(f => f.OperationId == idOperation));

                operation.State = State.Inactive;

                var childOperation = Context.Operations.Include(f => f.OperationsDetails).FirstOrDefault(f => f.OperationParentId == idOperation);
                if (childOperation != null)
                {
                    if (childOperation.OperationsDetails.Count != 0)
                    {
                        Context.OperationsDetails.RemoveRange(Context.OperationsDetails.Where(f => f.OperationId == childOperation.Id));
                    }
                    childOperation.State = State.Inactive;
                }

                Context.SaveChanges();

                return "OK";
            }
            catch (Exception ex)
            {
                return LazyMethods.GetErrMessage(ex);
            }
        }

        public string ContaOperationDetailSave(int idOperation, string activityType, int operType, int operationType, int valueType, decimal value, string bank, string explicatii, bool storno)
        {
            try
            {
                var operation = Context.Operations.FirstOrDefault(f => f.Id == idOperation && f.State == State.Active);

                if (operation == null)
                {
                    return "Nu am identificat operatia contabila cu id-ul " + idOperation.ToString();
                }

                // verificare luna inchisa
                var count = Context.Balance.Count(f => f.TenantId == _tenantId && f.Status == State.Active && f.BalanceDate >= operation.OperationDate);
                if (count != 0)
                {
                    return "Operatia nu se poate modifica. Luna contabila este inchisa";
                }

                var localCurrencyId = Context.Tenants.FirstOrDefault(f => f.Id == _tenantId).LocalCurrencyId;
                var operationChild = new Operation();
                operationChild = Context.Operations.FirstOrDefault(f => f.OperationParentId == idOperation);

                var autoOperationType = (AutoOperationType)operType;
                var operationConfig = Context.AutoOperationConfig.Where(f => f.TenantId == _tenantId && f.AutoOperType == autoOperationType && f.OperationType == operationType
                                                                        && f.StartDate <= operation.OperationDate && operation.OperationDate <= (f.EndDate ?? operation.OperationDate)
                                                                        && f.ElementId == valueType && f.State == State.Active)
                                                                  .ToList();
                int valueSign = value < 0 ? -1 : 1;
                if (storno) // daca am stornare caut monografia pentru valoare cu sens opus
                {
                    valueSign = -1 * valueSign;
                }

                operationConfig = operationConfig.Where(f => f.ValueSign == valueSign).ToList();
                if (operationConfig.Count == 0)
                {
                    return "Nu am identificat monografia pentru aceasta operatie si pentru tipul valorii introduse. Verificati monografiile definite in aplicatia Conta";
                }

                int? activityTypeId = null;
                var activityTypeObj = Context.ActivityTypes.FirstOrDefault(f => f.Status == State.Active && f.ActivityName == activityType);
                if (activityTypeObj != null)
                {
                    activityTypeId = activityTypeObj.Id;
                }

                decimal exchangeRate = 1;

                if (localCurrencyId != operation.CurrencyId)
                {
                    var exchangeRateObj = Context.ExchangeRates.Where(f => f.CurrencyId == operation.CurrencyId && f.ExchangeDate <= operation.OperationDate) // se ia cursul din data - 1
                                                               .OrderByDescending(f => f.ExchangeDate)
                                                               .FirstOrDefault();
                    if (exchangeRateObj == null)
                    {
                        var currencyObj = Context.Currency.FirstOrDefault(f => f.Id == operation.CurrencyId);
                        return "Nu am identificat cursul valutar pentru " + currencyObj.CurrencyCode + " in data " + LazyMethods.DateToString(operation.OperationDate);
                    }
                    exchangeRate = exchangeRateObj.Value;
                }
                foreach (var oper in operationConfig)
                {
                    if (explicatii == null || explicatii == "")
                    {
                        explicatii = oper.Details;
                    }
                    else
                    {
                        explicatii = explicatii.Replace("_", " ");
                    }
                    var debitAccountId = ContaAutomatGetAccount(_tenantId, oper.DebitAccount, activityTypeId, (oper.DebitAccount.IndexOf("6") == 0 ? localCurrencyId.Value : operation.CurrencyId), bank);
                    var creditAccountId = ContaAutomatGetAccount(_tenantId, oper.CreditAccount, activityTypeId, (oper.CreditAccount.IndexOf("7") == 0 ? localCurrencyId.Value : operation.CurrencyId), bank);

                    var detailNrObj = Context.OperationsDetails.Where(f => f.OperationId == idOperation).OrderByDescending(f => f.DetailNr).FirstOrDefault();
                    int detailNr = 1;
                    if (detailNrObj != null)
                    {
                        detailNr = detailNrObj.DetailNr.Value;
                        detailNr++;
                    }
                    if (!storno) // daca nu am stornare valoarea e intotdeauna pozitiva
                    {
                        value = Math.Abs(value);
                    }

                    if ((localCurrencyId == operation.CurrencyId) || ((oper.DebitAccount.IndexOf("6") != 0 && oper.CreditAccount.IndexOf("7") != 0))) // operatie in lei sau fara conturi de venit sau cheltuiala
                    {
                        var operationDetail = new OperationDetails
                        {
                            OperationId = operation.Id,
                            DebitId = debitAccountId,
                            CreditId = creditAccountId,
                            DetailNr = detailNr,
                            Value = value,
                            Details = explicatii
                        };
                        Context.OperationsDetails.Add(operationDetail);
                        Context.SaveChanges();
                    }
                    else // operatie in valuta cu conturi de venit sau cheltuiala
                    {
                        int pozitieSchimbAccountId = 0;
                        int contravaloarePozitieSchimbAccountId = 0;
                        var pozitieSchimbAccount = Context.Account.FirstOrDefault(f => f.AccountFuncType == AccountFuncType.PozitieSchimbOperatiuni && f.ActivityTypeId == activityTypeId
                                                                                  && f.CurrencyId == operation.CurrencyId);
                        if (pozitieSchimbAccount == null)
                        {
                            return "Nu am identificat contul contabil pentru pozitia de schimb";
                        }
                        pozitieSchimbAccountId = pozitieSchimbAccount.Id;

                        var currencyName = Context.Currency.FirstOrDefault(f => f.Id == operation.CurrencyId).CurrencyCode;
                        var contravaloarePozitieSchimbAccount = Context.Account.FirstOrDefault(f => f.AccountFuncType == AccountFuncType.ContravaloarePozitieSchimbOperatiuni && f.ActivityTypeId == activityTypeId
                                                                                               && f.AccountName.IndexOf(currencyName) >= 0);
                        if (contravaloarePozitieSchimbAccount == null)
                        {
                            return "Nu am identificat contul contabil pentru contravaloarea pozitiei de schimb";
                        }
                        contravaloarePozitieSchimbAccountId = contravaloarePozitieSchimbAccount.Id;

                        var valueLocalCurrency = Math.Round(value * exchangeRate, 2);
                        int operDebit = 0, operCredit = 0, childOperDebit = 0, childOperCredit = 0;

                        if (oper.CreditAccount.IndexOf("7") == 0)
                        {
                            operDebit = debitAccountId;
                            operCredit = pozitieSchimbAccountId;
                            childOperDebit = contravaloarePozitieSchimbAccountId;
                            childOperCredit = creditAccountId;
                        }
                        else
                        {
                            operDebit = pozitieSchimbAccountId;
                            operCredit = creditAccountId;
                            childOperDebit = debitAccountId;
                            childOperCredit = contravaloarePozitieSchimbAccountId;
                        }

                        var operationDetail = new OperationDetails
                        {
                            OperationId = operation.Id,
                            DebitId = operDebit,
                            CreditId = operCredit,
                            DetailNr = detailNr,
                            Value = value,
                            Details = explicatii
                        };
                        Context.OperationsDetails.Add(operationDetail);


                        var childOperationDetail = new OperationDetails
                        {
                            OperationId = operationChild.Id,
                            DebitId = childOperDebit,
                            CreditId = childOperCredit,
                            DetailNr = detailNr,
                            Value = valueLocalCurrency,
                            Details = explicatii + ", curs valutar: " + exchangeRate.ToString("N4")
                        };
                        Context.OperationsDetails.Add(childOperationDetail);
                        Context.SaveChanges();
                    }
                }

                return "OK";
            }
            catch (Exception ex)
            {
                return LazyMethods.GetErrMessage(ex);
            }
        }

        private int ContaAutomatGetAccount(int tenantId, string symbol, int? activityTypeId, int currencyId, string bank)
        {
            int accountId = 0;

            if (symbol == "CB") // am cont bancar
            {
                var tenant = Context.Tenants.FirstOrDefault(f => f.Id == 1);
                var personId = tenant.LegalPersonId;
                var bankItem = Context.Issuer.FirstOrDefault(f => f.IbanAbrv == bank);
                if (bankItem == null)
                {
                    throw new Exception("Nu am identificat banca " + bank);
                }
                var currency = Context.Currency.FirstOrDefault(f => f.Id == currencyId);
                var bankAccountObj = Context.BankAccount.FirstOrDefault(f => f.BankId == bankItem.Id && f.CurrencyId == currencyId && f.TenantId == tenantId && f.PersonId == personId);
                if (bankAccountObj == null)
                {
                    throw new Exception("Nu am identificat contul bancar pentru banca cu codul " + bank + " si moneda " + currency.CurrencyCode);
                }
                var bankAccountId = bankAccountObj.Id;
                var account = Context.Account.FirstOrDefault(f => f.TenantId == tenantId && f.BankAccountId == bankAccountId && f.Status == State.Active && f.ComputingAccount);

                if (account == null)
                {
                    throw new Exception("Nu am identificat contul contabil pentru contul bancar deschis la banca cu codul " + bank + " si moneda " + currency.CurrencyCode + ". Verificati planul de conturi");
                }
                accountId = account.Id;
            }
            else if ("#DCA#FCA#DCS#FCS#".IndexOf("#" + symbol + "#") >= 0)
            {
                // conturi de contributii anuale
                var currency = Context.Currency.FirstOrDefault(f => f.Id == currencyId);
                var bankItem = Context.Issuer.FirstOrDefault(f => f.IbanAbrv == bank);
                if (bankItem == null)
                {
                    throw new Exception("Nu am identificat banca cu codul " + bank);
                }

                var bankLegalPers = bankItem.LegalPersonId;

                var accountConfig = Context.AccountConfig.FirstOrDefault(f => f.Symbol == symbol && f.ActivityTypeId == activityTypeId && f.Status == State.Active);
                if (accountConfig == null)
                {
                    throw new Exception("Nu am identificat configurarea pentru conturile contabile cu codul " + symbol + ". Verificati Configurarea conturilor contari automate.");
                }

                var symbolAccount = accountConfig.AccountRad;
                var symbolSearch = symbolAccount.Replace(("." + currency.CurrencyCode), "");
                var account = Context.Account.FirstOrDefault(f => f.Symbol.IndexOf(symbolSearch) == 0 && f.CurrencyId == currencyId && f.ComputingAccount
                                                             && f.ActivityTypeId == activityTypeId && f.ThirdPartyId == bankLegalPers && f.Status == State.Active);
                if (account != null)
                {
                    accountId = account.Id;
                }
                else // construiesc contul
                {
                    var accountList = Context.Account.Where(f => f.Symbol.IndexOf(symbolSearch) == 0 && f.ActivityTypeId == activityTypeId && f.Status == State.Active && f.ComputingAccount).ToList();
                    string newSymbol = symbolSearch + ".";

                    if (accountConfig.NrCaractere != null)
                    {
                        var count = Context.Account.Count(f => f.Symbol.IndexOf(newSymbol) == 0 && f.ActivityTypeId == activityTypeId && f.Status == State.Active && f.ComputingAccount);
                        if (count == 0)
                        {
                            newSymbol = symbolSearch + "." + "1".PadLeft(accountConfig.NrCaractere.Value, '0') + "." + currency.CurrencyCode;
                        }
                        else
                        {
                            try
                            {
                                var maxAccount = Context.Account.Where(f => f.Symbol.IndexOf(newSymbol) >= 0 && f.ComputingAccount && f.Symbol != newSymbol).OrderByDescending(f => f.Symbol).FirstOrDefault();
                                var maxSymbol = maxAccount.Symbol;
                                maxSymbol = maxSymbol.Substring(0, maxSymbol.Length - 4);
                                var substrSymbol = maxSymbol.Substring(newSymbol.Length);
                                var nextItem = int.Parse(substrSymbol);
                                nextItem++;
                                newSymbol += nextItem.ToString().PadLeft(accountConfig.NrCaractere.Value, '0') + "." + currency.CurrencyCode;
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("Nu am reusit sa generez contul " + symbol + ". Verificati daca contul sintetic al contului dorit nu este marcat ca fiind cont de calcul.");
                            }
                        }

                        // caut sinteticul
                        var syntheticAccount = Context.Account.FirstOrDefault(f => f.Status == State.Active && f.Symbol == accountConfig.AccountRad);
                        if (syntheticAccount == null)
                        {
                            throw new Exception("Contul sintetic " + accountConfig.AccountRad + " nu este definit in planul de conturi.");
                        }
                        string bankName = Context.Persons.FirstOrDefault(f => f.Id == bankLegalPers).FullName;

                        var dataAdd = Context.Balance.Where(f => f.Status == State.Active).OrderByDescending(f => f.BalanceDate).FirstOrDefault().BalanceDate;
                        dataAdd = dataAdd.AddDays(1);

                        var newAccount = new Account
                        {
                            AccountFuncType = syntheticAccount.AccountFuncType,
                            AccountName = bankName,
                            AccountTypes = syntheticAccount.AccountTypes,
                            ComputingAccount = true,
                            CurrencyId = currencyId,
                            ThirdPartyId = bankLegalPers,
                            Status = State.Active,
                            SyntheticAccountId = syntheticAccount.Id,
                            Symbol = newSymbol,
                            AccountStatus = true,
                            ActivityTypeId = activityTypeId,
                            TenantId = tenantId,
                            DataValabilitate = dataAdd
                        };
                        Context.Account.Add(newAccount);
                        Context.SaveChanges();

                        accountId = newAccount.Id;
                    }
                }
            }
            else
            {
                var accountDirect = Context.Account.FirstOrDefault(f => f.TenantId == tenantId && f.Status == State.Active && f.Symbol == symbol && f.ComputingAccount
                                                                   && f.CurrencyId == currencyId);

                if (accountDirect != null) // am identificat contul
                {
                    if (symbol.IndexOf("6") == 0 && activityTypeId != null)
                    {
                        accountDirect = Context.Account.FirstOrDefault(f => f.TenantId == tenantId && f.Status == State.Active && f.SyntheticAccountId == accountDirect.Id && f.ComputingAccount
                                                                       && f.CurrencyId == currencyId && f.ActivityTypeId == activityTypeId);
                        if (accountDirect == null)
                        {
                            throw new Exception("Nu am identificat contul de cheltuiala specific tipului de fond selectat. Verificati planul de conturi");
                        }
                        accountId = accountDirect.Id;
                    }
                    else
                    {
                        accountId = accountDirect.Id;
                    }
                }
                else
                {
                    // caut sinteticul cu simbolul Symbol
                    var sintetic = Context.Account.FirstOrDefault(f => f.TenantId == tenantId && f.Status == State.Active && f.Symbol == symbol);
                    if (sintetic == null)
                    {
                        throw new Exception("Nu am identificat contul sintetic cu simbolul " + symbol + ". Verificati monografiile");
                    }
                    var accountList = Context.Account.Where(f => f.TenantId == tenantId && f.Status == State.Active && f.ComputingAccount && f.SyntheticAccountId == sintetic.Id
                                                            && f.CurrencyId == currencyId);
                    if (activityTypeId != null)
                    {
                        accountList = accountList.Where(f => f.ActivityTypeId == activityTypeId);
                    }

                    var account = accountList.FirstOrDefault();
                    if (account == null)
                    {
                        throw new Exception("Nu am identificat contul contabil specificat in monografie cu simbolul " + symbol + ". Verificati planul de conturi si monografia");
                    }
                    accountId = account.Id;
                }
            }

            return accountId;
        }

        public string ContaOperationEndSave(int idOperation)
        {
            try
            {
                var operation = Context.Operations.FirstOrDefault(f => f.Id == idOperation && f.State == State.Active);

                if (operation == null)
                {
                    return "Nu am identificat operatia contabila cu id-ul " + idOperation.ToString();
                }

                // verificare luna inchisa
                var count = Context.Balance.Count(f => f.TenantId == _tenantId && f.Status == State.Active && f.BalanceDate >= operation.OperationDate);
                if (count != 0)
                {
                    return "Operatia nu se poate sterge. Luna contabila este inchisa";
                }

                var childOperation = Context.Operations.Include(f => f.OperationsDetails).FirstOrDefault(f => f.OperationParentId == idOperation);
                if (childOperation != null)
                {
                    if (childOperation.OperationsDetails.Count == 0)
                    {
                        childOperation.State = State.Inactive;
                        Context.SaveChanges();
                    }
                }

                return "OK";
            }
            catch (Exception ex)
            {
                return LazyMethods.GetErrMessage(ex);
            }
        }

        public string ContaOperationSaveDirect(DateTime operationDate, string documentNr, DateTime documentDate, string currency, string documentType, out int idOperation)
        {
            idOperation = 0;
            int documentTypeId = 0;
            int currencyId = 0;

            // verificare luna inchisa
            var count = Context.Balance.Count(f => f.TenantId == _tenantId && f.Status == State.Active && f.BalanceDate >= operationDate);
            if (count != 0)
            {
                return "Operatia nu se poate inregistra. Luna contabila este inchisa";
            }

            var documentTypeObj = Context.DocumentType.FirstOrDefault(f => f.TenantId == _tenantId && f.TypeNameShort == documentType);
            if (documentTypeObj == null)
            {
                return "Nu am identificat documentul cu tipul " + documentType;
            }
            documentTypeId = documentTypeObj.Id;

            if (currency == null || currency == "")
            {
                return "Nu ati specificat moneda operatiei";
            }
            var currencyObj = Context.Currency.FirstOrDefault(f => f.CurrencyCode == currency);
            if (currencyObj == null)
            {
                return "Nu am identificat moneda " + currency;
            }
            currencyId = currencyObj.Id;

            var operation = new Operation
            {
                TenantId = _tenantId,
                OperationDate = operationDate,
                DocumentNumber = documentNr,
                DocumentDate = documentDate,
                ClosingMonth = false,
                OperationStatus = OperationStatus.Unchecked,
                ExternalOperation = true,
                CurrencyId = currencyId,
                DocumentTypeId = documentTypeId,
                State = State.Active
            };
            Context.Operations.Add(operation);
            Context.SaveChanges();

            Context.SaveChanges();
            idOperation = operation.Id;
            return "OK";
        }

        public string ContaOperationDetailSaveDirect(int idOperation, string debit, string credit, decimal value, string explicatii)
        {
            try
            {
                var operation = Context.Operations.FirstOrDefault(f => f.Id == idOperation && f.State == State.Active);

                if (operation == null)
                {
                    return "Nu am identificat operatia contabila cu id-ul " + idOperation.ToString();
                }

                // verificare luna inchisa
                var count = Context.Balance.Count(f => f.TenantId == _tenantId && f.Status == State.Active && f.BalanceDate >= operation.OperationDate);
                if (count != 0)
                {
                    return "Operatia nu se poate modifica. Luna contabila este inchisa";
                }

                var localCurrencyId = Context.Tenants.FirstOrDefault(f => f.Id == _tenantId).LocalCurrencyId;

                var debitAccount = Context.Account.FirstOrDefault(f => f.Status == State.Active && f.ComputingAccount && f.Symbol == debit);
                if (debitAccount == null)
                {
                    return "Nu am identificat contul debitor cu simbolul " + debit;
                }
                int debitAccountId = 0;
                debitAccountId = debitAccount.Id;

                var creditAccount = Context.Account.FirstOrDefault(f => f.Status == State.Active && f.ComputingAccount && f.Symbol == credit);
                if (creditAccount == null)
                {
                    return "Nu am identificat contul creditor cu simbolul " + credit;
                }
                int creditAccountId = 0;
                creditAccountId = creditAccount.Id;

                var detailNrObj = Context.OperationsDetails.Where(f => f.OperationId == idOperation).OrderByDescending(f => f.DetailNr).FirstOrDefault();
                int detailNr = 1;
                if (detailNrObj != null)
                {
                    detailNr = detailNrObj.DetailNr.Value;
                    detailNr++;
                }
                var operationDetail = new OperationDetails
                {
                    OperationId = operation.Id,
                    DebitId = debitAccountId,
                    CreditId = creditAccountId,
                    DetailNr = detailNr,
                    Value = value,
                    Details = explicatii
                };
                Context.OperationsDetails.Add(operationDetail);
                Context.SaveChanges();

                return "OK";
            }
            catch (Exception ex)
            {
                return LazyMethods.GetErrMessage(ex);
            }
        }

    }
}