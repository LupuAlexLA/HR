using Abp.Domain.Repositories;
using Abp.Domain.Services;
using CsvHelper.Configuration;
using Niva.Erp.Models.Conta;
using Niva.Erp.Repositories;
using Niva.Erp.Repositories.Conta.Nomenclatures;
using Niva.Erp.Repositories.Conta.Operation;
using Niva.Erp.Repositories.Economic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using Niva.Erp.Models.Conta.Enums;
using System.Linq;
using CsvHelper;
using Niva.Erp.ModelObjects;

namespace Niva.Erp.Managers
{
    public class OperationsManager : DomainService
    {
        IDocumentTypeRepository _documentTypeRepository;
        IOperationRepository _operationRepository;
        ICurrencyRepository _currencyRepository;
        IAccountRepository _accountRepository;
        IForeignOperationRepository _foreignOperationRepository;
        IPersonRepository _personRepository;
        IRepository<BankAccount> _bankAccountRepository;
        IPaymentOrdersRepository _paymentOrdersRepository;
        IRepository<AccountTaxProperty> _accountTaxPropertyRepository;
        IRepository<ForeignOperationDictionary> _foDictionaryRepository;

        public OperationsManager(IDocumentTypeRepository documentTypeRepository, IOperationRepository operationRepository, ICurrencyRepository currencyRepository,
                                 IAccountRepository accountRepository, IForeignOperationRepository foreignOperationRepository, IPersonRepository personRepository,
                                 IRepository<BankAccount> bankAccountRepository, IPaymentOrdersRepository paymentOrdersRepository, IRepository<AccountTaxProperty> accountTaxPropertyRepository,
                                 IRepository<ForeignOperationDictionary> foDictionaryRepository)
        {
            _documentTypeRepository = documentTypeRepository;
            _operationRepository = operationRepository;
            _currencyRepository = currencyRepository;
            _accountRepository = accountRepository;
            _foreignOperationRepository = foreignOperationRepository;
            _personRepository = personRepository;
            _bankAccountRepository = bankAccountRepository;
            _paymentOrdersRepository = paymentOrdersRepository;
            _accountTaxPropertyRepository = accountTaxPropertyRepository;
            _foDictionaryRepository = foDictionaryRepository;
        }

        // needs refactoring with manager
        public void LoadOperationsFromFile(string content)
        {
            try
            {
                int idx = 0;
                using (StringReader sr = new StringReader(content))
                {
                    bool firstLine = true;

                    Operation operation = null;
                    var nrCrt = 0;
                    var opDate = 1;
                    var docType = 2;
                    var docNr = 3;
                    var docDate = 4;
                    var dbSymbol = 5;
                    var crSymbol = 7;
                    var sum = 9;
                    var sumCur = 10;
                    var curr = 11;
                    var explications = 12;
                    var explications2 = 13;

                    var nrDocPrec = "";

                    while (sr.Peek() >= 0)
                    {
                        var column = sr.ReadLine().Split(';');

                        if (firstLine || nrDocPrec != column[nrCrt])
                        {
                            if (operation != null)
                            {
                                _operationRepository.Insert(operation);
                                // Context.Operations.Add(operation);
                            }

                            operation = new Operation();
                            firstLine = false;
                            operation.OperationDate = DateTime.ParseExact(column[opDate], "dd.MM.yyyy", CultureInfo.InvariantCulture);
                            operation.DocumentDate = column[docDate] == "" ? DateTime.ParseExact(column[opDate], "dd.MM.yyyy", CultureInfo.InvariantCulture) : DateTime.ParseExact(column[docDate], "dd.MM.yyyy", CultureInfo.InvariantCulture);
                            operation.DocumentNumber = column[docNr];

                            if (_documentTypeRepository.GetDocumentTypeByShortName(column[docType]) == null)
                                operation.DocumentTypeId = _documentTypeRepository.GetDocumentTypeById(92).Id;
                            else
                                operation.DocumentTypeId = _documentTypeRepository.GetDocumentTypeByShortName(column[docType]).Id;

                            //operation.DateModify = System.DateTime.Now;
                            operation.Currency = column[curr] == "" ? _currencyRepository.GetByCode("RON") : _currencyRepository.GetByCode(column[curr]);
                            operation.State = State.Active;
                            operation.OperationStatus = OperationStatus.Checked;
                            //operation.UserModify = Util.CurrentUser;
                            operation.OperationsDetails = new List<OperationDetails>();
                            firstLine = false;
                            nrDocPrec = column[nrCrt];
                        }

                        var accountDBSymbol = column[dbSymbol];
                        var accountCRSymbol = column[crSymbol];

                        Account accountDB = null;
                        Account accountCR = null;

                        accountDB = _accountRepository.GetAccountBySymbol(accountDBSymbol);
                        if (accountDB == null)
                        {
                            throw new Exception("Contul cu simbolul " + accountDBSymbol + " nu este inregistrat in baza de date.");
                        }

                        accountCR = _accountRepository.GetAccountBySymbol(accountCRSymbol);
                        if (accountCR == null)
                        {
                            throw new Exception("Contul cu simbolul " + accountCRSymbol + " nu este inregistrat in baza de date.");
                        }

                        if (accountDB != null && accountCR != null)
                        {

                            var opDetails = new OperationDetails();
                            opDetails.Credit = accountCR;
                            opDetails.Debit = accountDB;
                            opDetails.Details = column[explications] + " " + column[explications2];
                            opDetails.Value = decimal.Parse(column[sum], new CultureInfo("en-GB"));
                            opDetails.ValueCurr = operation.Currency.CurrencyCode == "RON" ? 0 : decimal.Parse(column[sumCur], new CultureInfo("en-GB"));
                            operation.OperationsDetails.Add(opDetails);

                            //deductibilitate
                            opDetails = GetOperationDetDeductibilitate(opDetails, operation.OperationDate);
                        }
                        idx++;
                    }

                    //Daca am terminat de citit si exista operatie 
                    if (sr.Peek() < 0 && idx > 0 && operation != null)
                    {
                        _operationRepository.Insert(operation);
                    }

                    //try
                    //{
                    //    CurrentUnitOfWork.SaveChanges();
                    //}
                    //catch (Exception ex)
                    //{
                    //    throw new Exception(operation.DocumentNumber + "/" + operation.DocumentDate.ToShortDateString() + " - " + GetErrMessage(ex));
                    //}

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void LoadForeignOperationsFromFile(int documentType, DateTime operationDate, string documentNumber, DateTime documentDate, int bankAccount, int account, string content)
        {
            try
            {
                if (content != null)
                {
                    var operation = new ForeignOperation();
                    operation.OperationDate = operationDate;
                    operation.DocumentDate = documentDate;
                    operation.DocumentNumber = documentNumber;
                    operation.BankAccountId = bankAccount;
                    var currencyId = _personRepository.GetBankAccById(bankAccount).CurrencyId;
                    operation.CurrencyId = currencyId;
                    operation.DocumentTypeId = documentType;

                    operation.State = State.Active;

                    var bankIbanAbrv = _bankAccountRepository.GetAllIncluding(f => f.Bank, f => f.Bank.LegalPerson).FirstOrDefault(f => f.Id == bankAccount).Bank.IbanAbrv;
                    var bank = _bankAccountRepository.GetAllIncluding(f => f.Bank, f => f.Bank.LegalPerson).FirstOrDefault(f => f.Id == bankAccount);
                    // if ("#8119644#".Contains("#" + bankIBANAbrv + "#"))


                    var operDetails = new List<ForeignOperationsDetails>();
                    using (StringReader sr = new StringReader(content))
                    using (var csv = new CsvReader(sr, CultureInfo.InvariantCulture))
                    {
                        IEnumerable<LibraExtras> records;

                        if (bank.IBAN.Contains("BREL"))
                        {
                            csv.Configuration.RegisterClassMap<LibraExtrasMap>();
                            records = csv.GetRecords<LibraExtras>();
                        }
                        else if (bank.IBAN.Contains("BTRL"))
                        {
                            csv.Configuration.RegisterClassMap<BTExtrasMap>();
                            records = csv.GetRecords<LibraExtras>();

                            csv.Configuration.BadDataFound = x =>
                            {
                                Console.WriteLine($"Bad data: <{x.RawRecord}>");
                            };
                        }
                        else
                        {
                            throw new Exception("Operatiunea nu este implementata pentru aceasta banca! Incarcarea extrasului este disponibila doar pentru Banca Transilvania si LibraBank!");
                        }

                        foreach (var item in records)
                        {
                            DateTime operDateFromExtras = new DateTime();
                            try
                            {
                                if (bank.IBAN.Contains("BREL"))
                                {
                                    operDateFromExtras = DateTime.ParseExact(item.Data, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                }else if (bank.IBAN.Contains("BTRL"))
                                {
                                    try
                                    {
                                        operDateFromExtras = DateTime.ParseExact(item.Data, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                                    }
                                    catch
                                    {
                                        operDateFromExtras = DateTime.ParseExact(item.Data, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                    }
                                }
                            }
                            catch
                            {
                                throw new Exception("Data operatiei din extras nu este in formatul corespunzator " + item.DataOperare);
                            }
                            //if (operDateFromExtras != operationDate)
                            //{
                            //    throw new Exception("Data operatiei este diferita de data operarii din extras " + operationDate.Day + "." + operationDate.Month + "." + operationDate.Year + " <> " + operDateFromExtras);
                            //}


                            decimal debitValue = 0, creditValue = 0;
                            int? dictionaryAccountId = null;
                            if (item.Debit != "")
                            {
                                debitValue = decimal.Parse(item.Debit, new CultureInfo("en-GB"));
                            }
                            if (item.Credit != "")
                            {
                                creditValue = decimal.Parse(item.Credit, new CultureInfo("en-GB"));
                            }
                            //var debitValue = decimal.Parse(item.Debit ?? "0", new CultureInfo("en-GB"));
                            //var creditValue = decimal.Parse(item.Credit ?? "0", new CultureInfo("en-GB"));

                            var dictionaries = _foDictionaryRepository.GetAllIncluding(f => f.Account).Where(f => f.State == State.Active).ToList();
                            var value = debitValue != 0 ? debitValue : creditValue;

                            if(value > 0) // INCASARE
                            {
                                dictionaries = dictionaries.Where(f => f.FODictionaryType == FODictionaryType.Incasare || f.FODictionaryType == FODictionaryType.Toate).ToList();
                            }
                            else //PLATA
                            {
                                dictionaries = dictionaries.Where(f => f.FODictionaryType == FODictionaryType.Plata || f.FODictionaryType == FODictionaryType.Toate).ToList();
                            }

                            bool found = false;
                            foreach (var dict in dictionaries)
                            {
                                if (dictionaryAccountId == null)
                                {
                                    var wordList = dict.Expression.Split("%");
                                    if (wordList.Length == 1)
                                    {
                                        found = item.Descriere.ToLower().Contains(dict.Expression.ToLower());
                                    }
                                    else
                                    {
                                        found = true;
                                        int index = 0;
                                        for (int i = 0; i < wordList.Length; i++)
                                        {
                                            string p = wordList[i];
                                            if (p.Length > 0)
                                            {
                                                if (item.Descriere.IndexOf(p, index) >= 0)
                                                {
                                                    index = item.Descriere.IndexOf(p, index) + p.Length;
                                                }
                                                else
                                                {
                                                    found = false;
                                                }
                                            }
                                        }                                        
                                    }
                                    if (found)
                                    {
                                        dictionaryAccountId = dict.AccountId;
                                    }
                                }
                            }
                            

                            var debitAccount = value > 0 ? account : dictionaryAccountId;
                            var creditAccount = value < 0 ? account : dictionaryAccountId;
                            var detail = item.Descriere;
                            
                            
                            var opDetails = new ForeignOperationsDetails
                            {
                                Value = value,
                                ValueCurr = 0,
                                OriginalDetails = detail,
                                DocumentTypeId = documentType,
                                DocumentDate = documentDate,
                                DocumentNumber = documentNumber
                            };

                            value = Math.Abs(value);
                            var opAccounting = new ForeignOperationsAccounting
                            {
                                DebitId = debitAccount,
                                CreditId = creditAccount,
                                Value = value,
                                ValueCurr = 0
                            };

                            var _detail = detail;
                            if (_detail.IndexOf("(") > 0)
                            {
                                _detail = _detail.Substring(0, _detail.IndexOf("(") - 1);
                            }
                            opAccounting.Details = _detail;
                            var opAccountingList = new List<ForeignOperationsAccounting>();
                            opAccountingList.Add(opAccounting);
                            opDetails.OperationsAccounting = opAccountingList;

                            operDetails.Add(opDetails);
                        }

                    }

                    operation.ForeignOperationsDetails = operDetails;

                    operation = ProcessOperationDetails(operation);

                    _foreignOperationRepository.Insert(operation);


                }
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ForeignOperation ProcessOperationDetails(ForeignOperation operation)
        {
            foreach (var item in operation.ForeignOperationsDetails)
            {
                var textReferinta = "Tranzactie Referinta: ";
                if (item.OriginalDetails.IndexOf(textReferinta) == 0) // pot sa identific OP-ul
                {
                    var details = item.OriginalDetails.Substring(textReferinta.Length);
                    var nrOP = details.Substring(0, details.IndexOf(" "));
                    int nrOPint = 0;
                    try
                    {
                        nrOPint = int.Parse(nrOP);
                    }
                    catch
                    {

                    }

                    var op = _paymentOrdersRepository.FirstOrDefault(f => f.State == State.Active && f.OrderDate.Year == operation.OperationDate.Year
                                                                     && f.OrderNr == nrOPint);
                    if (op != null)
                    {
                        item.PaymentOrderId = op.Id;
                    }


                }
            }


            return operation;
        }

        //public void LoadForeignOperationsFromFile(int appClientId, int documentType, DateTime operationDate, string documentNumber, DateTime documentDate, int bankAccount, int account, string content)
        //{
        //    try
        //    {
        //        if (content != null)
        //        {
        //            var operation = new ForeignOperation();
        //            operation.OperationDate = operationDate;
        //            operation.DocumentDate = documentDate;
        //            operation.DocumentNumber = documentNumber;
        //            operation.BankAccountId = bankAccount;
        //            var currencyId = _personRepository.GetBankAccById(bankAccount).CurrencyId;
        //            operation.CurrencyId = currencyId;
        //            operation.DocumentTypeId = documentType;

        //            operation.State = State.Active;
        //            operation.AppClientId = appClientId;

        //            var operDetails = new List<ForeignOperationsDetails>();
        //            using (StringReader sr = new StringReader(content))
        //            using (var csv = new CsvReader(sr))
        //            {
        //                var records = csv.GetRecords<LibraExtras>();

        //                foreach (var item in records)
        //                {
        //                    var debitValue = decimal.Parse(item.Debit, new CultureInfo("en-GB"));
        //                    var creditValue = decimal.Parse(item.Credit, new CultureInfo("en-GB"));
        //                    var debitAccount = creditValue != 0 ? account : (int?)null;
        //                    var creditAccount = debitValue != 0 ? account : (int?)null;
        //                    var detail = item.Descriere;
        //                    var value = debitValue != 0 ? debitValue : creditValue;

        //                    var opDetails = new ForeignOperationsDetails
        //                    {
        //                        AppClientId = appClientId,
        //                        Value = value,
        //                        ValueCurr = 0,
        //                        OriginalDetails = detail
        //                    };

        //                    var opAccounting = new ForeignOperationsAccounting
        //                    {
        //                        AppClientId = appClientId,
        //                        DebitId = debitAccount,
        //                        CreditId = creditAccount,
        //                        Value = value,
        //                        ValueCurr = 0
        //                    };

        //                    var _detail = detail;
        //                    if (_detail.IndexOf("(") > 0)
        //                    {
        //                        _detail = _detail.Substring(0, _detail.IndexOf("(") - 1);
        //                    }
        //                    opAccounting.Details = _detail;
        //                    var opAccountingList = new List<ForeignOperationsAccounting>();
        //                    opAccountingList.Add(opAccounting);
        //                    opDetails.OperationsAccounting = opAccountingList;

        //                    operDetails.Add(opDetails);
        //                }

        //            }

        //            operation.ForeignOperationsDetails = operDetails;
        //            _foreignOperationRepository.Insert(operation);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public static string GetErrMessage(Exception ex)
        {
            string err = "";

            while (ex != null)
            {
                err += ex.Message;
                ex = ex.InnerException;
            }
            return err;
        }

        public OperationDetails GetOperationDetDeductibilitate(OperationDetails detail, DateTime operationDate)
        {
            // deductibilitate
            var debitAccountDeduc = _accountTaxPropertyRepository.GetAll()
                                                            .Where(f => f.AccountId == detail.DebitId && f.PropertyDate <= operationDate)
                                                            .OrderByDescending(f => f.PropertyDate)
                                                            .FirstOrDefault();
            decimal debitDeducProc = (debitAccountDeduc == null) ? 0 : debitAccountDeduc.PropertyValue;
            decimal debitDeducValue = Math.Round(detail.Value * debitDeducProc, 2);

            var creditAccountDeduc = _accountTaxPropertyRepository.GetAll()
                                                            .Where(f => f.AccountId == detail.CreditId && f.PropertyDate <= operationDate)
                                                            .OrderByDescending(f => f.PropertyDate)
                                                            .FirstOrDefault();
            decimal creditDeducProc = (creditAccountDeduc == null) ? 0 : creditAccountDeduc.PropertyValue;
            decimal creditDeducValue = Math.Round(detail.Value * creditDeducProc, 2);

            detail.DebitValueDeduct = debitDeducValue;
            detail.CreditValueDeduct = creditDeducValue;
            return detail;
        }
    }

    public class LibraExtras
    {
        public string Data { get; set; }

        //public string EmptyString1 { get; set; }

        public string Descriere { get; set; }

        //public string EmptyString2 { get; set; }

        //public string EmptyString3 { get; set; }

        //public string EmptyString4 { get; set; }

        //public string EmptyString5 { get; set; }

        public string DataOperare { get; set; }

        //public string EmptyString6 { get; set; }

        //public string EmptyString7 { get; set; }

        //public string EmptyString8 { get; set; }

        //public string EmptyString9 { get; set; }

        public string Debit { get; set; }

        //public string EmptyString10 { get; set; }

        public string Credit { get; set; }

        //public string EmptyString11 { get; set; }

        //public string EmptyString12 { get; set; }

        //public string EmptyString13 { get; set; }

        public string Sold { get; set; }

        //public string EmptyString14 { get; set; }
    }

    public class LibraExtrasMap : ClassMap<LibraExtras>
    {
        public LibraExtrasMap()
        {
            Map(m => m.Data).Index(0);
            Map(m => m.DataOperare).Index(7);
            Map(m => m.Debit).Index(12);
            Map(m => m.Credit).Index(14);
            Map(m => m.Descriere).Index(2);
            Map(m => m.Sold).Index(17);
        }
    }
    public class BTExtrasMap : ClassMap<LibraExtras>
    {
        public BTExtrasMap()
        {
            Map(m => m.Data).Index(0);
            Map(m => m.DataOperare).Index(1);
            Map(m => m.Debit).Index(4);
            Map(m => m.Credit).Index(5);
            Map(m => m.Descriere).Index(2);
            Map(m => m.Sold).Index(6);
        }
    }
}
