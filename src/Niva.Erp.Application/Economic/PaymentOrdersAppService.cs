using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Niva.Erp.EntityFrameworkCore.Repositories.Conta;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Economic;
using Niva.Erp.Repositories.Conta.Nomenclatures;
using Niva.Erp.Repositories.Economic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Niva.Erp.Economic
{
    public interface IPaymentOrdersAppService : IApplicationService
    {
        PayementOrdersForm InitForm();

        PayementOrdersForm PaymentOrdersList(PayementOrdersForm form);

        PayementOrdersForm PaymentOrderDetail(PayementOrdersForm form, int id);

        PayementOrdersForm PaymentOrderSave(PayementOrdersForm form);

        PayementOrdersForm PaymentOrderDelete(PayementOrdersForm form, int id);

        PayementOrdersForm SaveValidation(PayementOrdersForm form);
        

        List<PaymentOrderExportList> SearchPaymentOrderByBenefBankId(int benefBankId);

        List<BTExport> ExportOPToCSV(List<int> selectedOpIds);
        byte[] ExportOPToCSVBin(List<int> selectedOpIds);
        void FinalizeExportedOp(List<int> selectedOPIds);
    }

    public class PaymentOrdersAppService : ErpAppServiceBase, IPaymentOrdersAppService
    {
        IPaymentOrdersRepository _paymentOrdersRepository;
        OperationRepository _operationRepository;
        IRepository<Currency> _currencyRepository;
        IRepository<PaymentOrderInvoice> _paymentOrderInvoiceRepository;
        IInvoiceRepository _invoiceRepository;
        IPersonRepository _personRepository;
        IRepository<LegalPerson> _legalPersonRepository { get; set; }

        public PaymentOrdersAppService(IPaymentOrdersRepository paymentOrdersRepository,
                                       OperationRepository operationRepository, IRepository<Currency> currencyRepository, IRepository<PaymentOrderInvoice> paymentOrderInvoiceRepository,
                                       IInvoiceRepository invoiceRepository, IPersonRepository personRepository, IRepository<LegalPerson> legalPersonRepository)
        {
            _paymentOrdersRepository = paymentOrdersRepository;
            _operationRepository = operationRepository;
            _currencyRepository = currencyRepository;
            _paymentOrderInvoiceRepository = paymentOrderInvoiceRepository;
            _invoiceRepository = invoiceRepository;
            _personRepository = personRepository;
            _legalPersonRepository = legalPersonRepository;
        }

        //[AbpAuthorize("Casierie.OP.Acces")]
        public PayementOrdersForm InitForm()
        {
            try
            {
                var selectedAppClient = GetCurrentTenant();
                int payerId = selectedAppClient.LegalPersonId.Value;
                var _currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                var ret = new PayementOrdersForm
                {
                    SearchStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                    SearchEndDate = _currentDate,
                    ShowList = true,
                    ShowEditForm = false,
                    PayerId = payerId
                };
                ret = PaymentOrdersList(ret);

                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        //[AbpAuthorize("Casierie.OP.Acces")]
        public PayementOrdersForm PaymentOrdersList(PayementOrdersForm form)
        {
            try
            {
                var list = _paymentOrdersRepository.GetAllIncluding(f => f.Currency,
                                                                    f => f.PayerBankAccount.Bank.LegalPerson,
                                                                    f => f.PayerBankAccount,
                                                                    f => f.BenefAccount.Person,
                                                                    f => f.BenefAccount.Bank,
                                                                    f => f.BenefAccount.Bank.LegalPerson,
                                                                    f => f.BenefAccount
                                                                    )
                                                   .Where(f => f.State == State.Active && f.OrderDate >= form.SearchStartDate
                                                          && f.OrderDate <= form.SearchEndDate)
                                                   .OrderByDescending(f => f.OrderDate).ThenByDescending(f => f.OrderNr)
                                                   .ToList()
                                                   .Select(f => new PayementOrdersList
                                                   {
                                                       Id = f.Id,
                                                       OrderNr = f.OrderNr,
                                                       OrderDate = f.OrderDate,
                                                       Value = f.Value,
                                                       PayerBank = f.PayerBankAccount.Bank.LegalPerson.Name,
                                                       PayerBankAccount = f.PayerBankAccount.IBAN,
                                                       BenefId1 = f.BenefAccount.Person.Id1,
                                                       Beneficiary = f.BenefAccount.Person.FullName,
                                                       BenefBank = f.BenefAccount.Bank.LegalPerson.Name,
                                                       BenefBankAccount = f.BenefAccount.IBAN,
                                                       Currency = f.Currency.CurrencyCode,
                                                       PaymentDetails = f.PaymentDetails,
                                                       Finalised = (f.Status == OperationStatus.Checked),
                                                       FinalisedDb = (f.Status == OperationStatus.Checked),
                                                   })
                                                   .ToList()
                                                   ;
                form.OPList = list;
                form.ShowList = true;
                form.ShowEditForm = false;

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }

            return form;
        }

        [AbpAuthorize("Casierie.OP.Modificare")]
        public PayementOrdersForm PaymentOrderDetail(PayementOrdersForm form, int id)
        {
            try
            {
                var appClient = GetCurrentTenant();
                int localCurrencyId = appClient.LocalCurrencyId.Value;

                PayementOrderDetail detail;

                if (id == 0)
                {
                    detail = new PayementOrderDetail
                    {
                        OrderDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                        CurrencyId = localCurrencyId,
                        Finalised = OperationStatus.Unchecked,
                        InvoicesList = new List<InvoiceListSelectableDto>()
                    };
                    form.OPDetail = detail;
                    form = GetNextOPNumber(form);
                }
                else
                {
                    var op = _paymentOrdersRepository.GetAllIncluding(f => f.PayerBankAccount, f => f.BenefAccount, f => f.PaymentOrderInvoices, f => f.Currency, f => f.Beneficiary)
                                                     .FirstOrDefault(f => f.Id == id);

                    detail = new PayementOrderDetail
                    {
                        Id = op.Id,
                        OrderNr = op.OrderNr,
                        OrderDate = op.OrderDate,
                        Value = op.Value,
                        WrittenValue = op.WrittenValue,
                        PayerBankId = op.PayerBankAccount.BankId,
                        PayerBankAccountId = op.PayerBankAccountId,
                        BeneficiaryId = op.BeneficiaryId,
                        BenefBankId = op.BenefAccount.BankId,
                        BenefBankAccountId = op.BenefAccountId,
                        BeneficiaryName = op.Beneficiary.FullName,
                        CurrencyId = op.CurrencyId,
                        //InvoiceId = op.InvoiceId,
                        PaymentDetails = op.PaymentDetails,
                        Finalised = op.Status,
                        InvoicesList = new List<InvoiceListSelectableDto>()
                    };
                }

                form.OPDetail = detail;
                form.ShowList = false;
                form.ShowEditForm = true;

                return form;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public PayementOrdersForm GetNextOPNumber(PayementOrdersForm form)
        {
            var item = _paymentOrdersRepository.GetAll().Where(f => f.State == State.Active && f.OrderDate.Year == form.OPDetail.OrderDate.Year)
                                                        .OrderByDescending(f => f.OrderNr)
                                                        .FirstOrDefault();
            int nextNumber = 1;
            if (item != null)
            {
                nextNumber = item.OrderNr + 1;
            }
            form.OPDetail.OrderNr = nextNumber;

            return form;
        }

        public PayementOrdersForm PaymentOrderSave(PayementOrdersForm form)
        {
            try
            {
                OPSaveValidation(form);
                var writtenValue = ""; // LazyMethods.CifreToLitere(form.OPDetail.Value);
                // var invoice = _invoiceRepository.GetAllIncluding(/*f => f.PaymentOrders, f => f.Dispositions*/ f => f.InvoiceDetails  ).FirstOrDefault(f => f.Id == form.OPDetail.InvoiceId && f.State == State.Active);
                var currency = _currencyRepository.Get(form.OPDetail.CurrencyId.Value);

                //if(form.OPDetail.Value > invoice.RestPlata && invoice.RestPlata != 0)
                //{
                //    throw new Exception("Suma platita trebuie sa fie cel mult egala cu restul de plata in valoare de " + invoice.RestPlata + ' ' + currency.CurrencyCode);
                //}

                // sterg facturile platite pentru OP-ul selectat
                if (form.OPDetail.Id != 0)
                {
                    var paymentOrderInvoices = _paymentOrderInvoiceRepository.GetAll().Include(f => f.PaymentOrder).AsNoTracking().ToList().Where(f => f.PaymentOrderId == form.OPDetail.Id).ToList();
                    foreach (var item in paymentOrderInvoices)
                    {
                        _paymentOrderInvoiceRepository.Delete(item.Id);
                    }
                    CurrentUnitOfWork.SaveChanges();
                }

                foreach (var item in form.OPDetail.InvoicesList.Where(f => f.Selected))
                {
                    var invoice = _invoiceRepository.InvoiceForPaymentOrder(item.Id);

                    if (form.OPDetail.Id == 0)
                    {
                        form.OPDetail.PaymentDetails = invoice.ThirdParty.FullName + ", " + invoice.InvoiceSeries + " " + invoice.InvoiceNumber + " / " + invoice.InvoiceDate.ToShortDateString() + ", " +
                                  (invoice.StartDatePeriod.HasValue && invoice.EndDatePeriod.HasValue ? invoice.StartDatePeriod.Value.ToShortDateString() + " - " + invoice.EndDatePeriod.Value.ToShortDateString()
                                                                                                : "") + " " + form.OPDetail.PaymentDetails;
                    }
                }

                var op = new PaymentOrders
                {
                    Id = form.OPDetail.Id,
                    OrderNr = form.OPDetail.OrderNr,
                    OrderDate = form.OPDetail.OrderDate,
                    Value = form.OPDetail.Value,
                    WrittenValue = writtenValue,
                    PayerBankAccountId = form.OPDetail.PayerBankAccountId ?? 0,
                    BeneficiaryId = form.OPDetail.BeneficiaryId ?? 0,
                    BenefAccountId = form.OPDetail.BenefBankAccountId ?? 0,
                    PaymentDetails = form.OPDetail.PaymentDetails,
                    //InvoiceId = form.OPDetail.InvoiceId,
                    CurrencyId = form.OPDetail.CurrencyId ?? 0,
                    State = State.Active,
                    Status = OperationStatus.Unchecked,
                    StatusDate = DateTime.Now
                };

                var appClient = GetCurrentTenant();
                op.TenantId = appClient.Id;

                _paymentOrdersRepository.InsertOrUpdate(op);
                CurrentUnitOfWork.SaveChanges();

                if (form.OPDetail.InvoicesList.Count > 0)
                {

                    foreach (var invoice in form.OPDetail.InvoicesList.Where(f => f.PayedValue != 0))
                    {
                        var paymentOrderInvoice = new PaymentOrderInvoice
                        {
                            InvoiceId = invoice.Id,
                            OperationDate = op.OrderDate,
                            PayedValue = invoice.PayedValue,
                            PaymentOrderId = op.Id,
                            State = State.Active,
                        };
                        _paymentOrderInvoiceRepository.Insert(paymentOrderInvoice);
                        CurrentUnitOfWork.SaveChanges();
                    }
                }

                form = PaymentOrdersList(form);

                return form;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public void OPSaveValidation(PayementOrdersForm form)
        {
            if (form.OPDetail.PayerBankAccountId == null || form.OPDetail.BenefBankAccountId == null)
                throw new Exception("Trebuie sa selectati contul platitorului si contul beneficiarului");
            if (!_operationRepository.VerifyClosedMonth(form.OPDetail.OrderDate))
                throw new Exception("Nu se poate inregistra operatia deoarece luna contabila este inchisa");
            if (form.OPDetail.PayerBankAccountId == form.OPDetail.BenefBankAccountId)
                throw new Exception("Contul platitorului trebuie sa fie diferit de contul beneficiarului");

            if (!_operationRepository.VerifyClosedMonth(form.OPDetail.OrderDate))
                throw new Exception("Nu se poate inregistra operatia deoarece luna contabila este inchisa");


            var count = _paymentOrdersRepository.GetAll().AsNoTracking().Count(f => f.Id != form.OPDetail.Id && f.OrderNr == form.OPDetail.OrderNr
                                                                && f.OrderDate.Year == form.OPDetail.OrderDate.Year && f.State == State.Active);
            if (count != 0)
                throw new Exception("Exista un alt OP cu acelasi numar pentru anul " + form.OPDetail.OrderDate.Year.ToString());
        }

        [AbpAuthorize("Casierie.OP.Modificare")]
        public PayementOrdersForm PaymentOrderDelete(PayementOrdersForm form, int id)
        {
            try
            {
                var op = _paymentOrdersRepository.FirstOrDefault(f => f.Id == id);
                op.State = State.Inactive;
                if (!_operationRepository.VerifyClosedMonth(op.OrderDate))
                    throw new Exception("Nu se poate inregistra operatia deoarece luna contabila este inchisa");

                CurrentUnitOfWork.SaveChanges();

                // sterg facturile platite din PaymentOrderInvoice
                var opInvoices = _paymentOrderInvoiceRepository.GetAllIncluding(f => f.PaymentOrder, f => f.Invoice).Where(f => f.PaymentOrderId == id && f.State == State.Active);
                foreach (var item in opInvoices)
                {
                    _paymentOrderInvoiceRepository.Delete(item.Id);
                }
                CurrentUnitOfWork.SaveChanges();

                form = PaymentOrdersList(form);

                return form;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        [AbpAuthorize("Casierie.OP.Modificare")]
        public PayementOrdersForm SaveValidation(PayementOrdersForm form)
        {
            try
            {
                foreach (var item in form.OPList)
                {
                    var _op = _paymentOrdersRepository.FirstOrDefault(f => f.Id == item.Id);
                    if (!_operationRepository.VerifyClosedMonth(item.OrderDate))
                        throw new Exception("Nu se poate inregistra operatia deoarece luna contabila este inchisa");

                    _op.Status = (item.Finalised == true ? OperationStatus.Checked : OperationStatus.Unchecked);
                    _op.StatusDate = DateTime.Now;
                    _paymentOrdersRepository.Update(_op);
                    item.FinalisedDb = item.Finalised;
                }
                CurrentUnitOfWork.SaveChanges();
                return form;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public List<PaymentOrderExportList> SearchPaymentOrderByBenefBankId(int benefBankId)
        {
            try
            {
                var appClient = GetCurrentTenant();
                int localCurrencyId = appClient.LocalCurrencyId.Value;

                var list = _paymentOrdersRepository.GetAllIncluding(f => f.Currency,
                                                                   f => f.PayerBankAccount.Bank.LegalPerson,
                                                                   f => f.PayerBankAccount,
                                                                   f => f.BenefAccount.Person,
                                                                   f => f.BenefAccount.Bank,
                                                                   f => f.BenefAccount.Bank.LegalPerson,
                                                                   f => f.BenefAccount
                                                                   )
                                                  .Where(f => f.State == State.Active && f.PayerBankAccountId == benefBankId && f.Status == OperationStatus.Unchecked &&
                                                              f.CurrencyId == localCurrencyId)
                                                  .OrderByDescending(f => f.OrderDate).ThenByDescending(f => f.OrderNr)
                                                  .ToList()
                                                  .Select(f => new PaymentOrderExportList
                                                  {
                                                      Id = f.Id,
                                                      OrderNr = f.OrderNr,
                                                      OrderDate = f.OrderDate,
                                                      Value = f.Value,
                                                      PayerBank = f.PayerBankAccount.Bank.LegalPerson.Name,
                                                      PayerBankAccount = f.PayerBankAccount.IBAN,
                                                      BenefId1 = f.BenefAccount.Person.Id1,
                                                      Beneficiary = f.BenefAccount.Person.FullName,
                                                      BenefBank = f.BenefAccount.Bank.LegalPerson.Name,
                                                      BenefBankAccount = f.BenefAccount.IBAN,
                                                      BenefBankBic = f.PayerBankAccount.Bank.Bic,
                                                      Currency = f.Currency.CurrencyCode,
                                                      PaymentDetails = f.PaymentDetails,
                                                      Finalised = (f.Status == OperationStatus.Checked),
                                                      FinalisedDb = (f.Status == OperationStatus.Checked),
                                                      Selected = false
                                                  })
                                                  .ToList();
                return list;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public List<BTExport> ExportOPToCSV(List<int> selectedOpIds)
        {
            var appClientId = 1;
            var personId = _personRepository.GetPersonTenantId(appClientId);
            var person = _legalPersonRepository.FirstOrDefault(f => f.Id == personId);

            try
            {
                //exportul se realizeaza doar pentru op-urile selectate
                var opList = _paymentOrdersRepository.GetAll().Where(f => selectedOpIds.Contains(f.Id));
                var exportList = opList.Select(f => new BTExport
                {
                    Amount = f.Value,
                    BeneficiaryBankBIC = f.BenefAccount.IBAN.Contains("TREZ") ? "" : f.BenefAccount.Bank.Bic,
                    BeneficiaryFiscalCode = f.BenefAccount.IBAN.Contains("TREZ") ? person.Id1 : "",
                    BeneficiaryName = f.Beneficiary.FullName,
                    OrderNumber = f.OrderNr,
                    PaymentRef1 = f.PaymentDetails,
                    PaymentRef2 = "",
                    SourceAccountNumber = f.PayerBankAccount.IBAN,
                    TargetAccountNumber = f.BenefAccount.IBAN,
                    Urgent = "F",
                    ValueDate = f.OrderDate
                })
                                       .ToList();


                //var csvPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"BT-export-{DateTime.Now.ToFileTime()}.csv");

                //using (var writer = new StreamWriter(csvPath))
                //using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
                //{
                //    csvWriter.Configuration.Delimiter = ",";
                //    csvWriter.Configuration.RegisterClassMap<BTExportMap>();

                //    csvWriter.WriteHeader<BTExport>();

                //    csvWriter.NextRecord();
                //    csvWriter.WriteRecords(exportList);

                //    writer.Flush();
                //}

                // update payment order status
                //foreach (var item in opList)
                //{
                //   item.Status = OperationStatus.Checked;
                //}

                //CurrentUnitOfWork.SaveChanges();
                return exportList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] ExportOPToCSVBin(List<int> selectedOpIds)
        {
            var exportList = ExportOPToCSV(selectedOpIds);
            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream))
                {
                    using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
                    {
                        csvWriter.Configuration.Delimiter = ",";
                        csvWriter.Configuration.RegisterClassMap<BTExportMap>();

                        csvWriter.WriteHeader<BTExport>();

                        csvWriter.NextRecord();
                        csvWriter.WriteRecords(exportList);

                        streamWriter.Flush();

                        return memoryStream.ToArray();
                    }

                }

            }
        }

        public void FinalizeExportedOp(List<int> selectedOpIds)
        {
            var opList = _paymentOrdersRepository.GetAll().Where(f => selectedOpIds.Contains(f.Id));
            foreach (var op in opList)
            {
                op.Status = OperationStatus.Checked;
            }

            CurrentUnitOfWork.SaveChanges();
        }
    }
}
