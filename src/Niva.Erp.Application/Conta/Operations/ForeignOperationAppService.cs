using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Niva.Erp.Conta.Operations.Dto;
using Niva.Erp.Economic;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.EntityFrameworkCore.Repositories.Conta;
using Niva.Erp.Managers;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Repositories;
using Niva.Erp.Repositories.Conta;
using Niva.Erp.Repositories.Conta.Operation;
using Niva.Erp.Repositories.Economic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace Niva.Erp.Conta.Operations
{
    public interface IForeignOperationAppService : IApplicationService
    {
        ForeignOperationDto InitForm();
        ForeignOperationDto OperSearch(ForeignOperationDto form);
        ForeignOperationDto UploadFileStart(ForeignOperationDto form);
        ForeignOperationDto UploadOperationFile(ForeignOperationDto form);
        ForeignOperationDto SaveSelection(ForeignOperationDto form);
        ForeignOperationDto AddAccountingRow(ForeignOperationDto form, int detailId);
        ForeignOperationDto AccountingRowDelete(ForeignOperationDto form, int detailId, int index);
        ForeignOperationDto GenerateConta(ForeignOperationDto form);
        ForeignOperationDto InitDeleteForm(ForeignOperationDto form);
        ForeignOperationDto OperDeleteSearch(ForeignOperationDto form);
        ForeignOperationDto OperDeleteNC(int foreignOperationId, ForeignOperationDto form);
        ForeignOperationDto OperDeleteExtras(ForeignOperationDto form, int foreignOperationId);
        void CheckPaymentOrderForFgnOperDetail(int pymentOrderId, ForeignOperationDto form);
    }

    public class ForeignOperationAppService : ErpAppServiceBase, IForeignOperationAppService
    {
        IForeignOperationRepository _foreignOperationRepository;
        IBalanceRepository _balanceRepository;
        ICurrencyRepository _currencyRepository;
        OperationsManager _operationsManager;
        OperationRepository _operationRepository;
        IRepository<Account> _accountRepository;
        IRepository<ForeignOperationsDetails> _foreignOperationsDetailsRepository;
        IRepository<ForeignOperationsAccounting> _foreignOperationsAccountingRepository;
        IRepository<OperationDetails> _operationDetailsRepository;
        IPaymentOrdersRepository _paymentOrdersRepository;


        public ForeignOperationAppService(IForeignOperationRepository foreignOperationRepository, IBalanceRepository balanceRepository, ICurrencyRepository currencyRepository, IRepository<Account> accountRepository,
                    OperationsManager operationsManager, OperationRepository operationRepository, IRepository<ForeignOperationsDetails> foreignOperationsDetailsRepository,
                    IRepository<ForeignOperationsAccounting> foreignOperationsAccountingRepository, IRepository<OperationDetails> operationDetailsRepository, IPaymentOrdersRepository paymentOrdersRepository)
        {
            _foreignOperationRepository = foreignOperationRepository;
            _balanceRepository = balanceRepository;
            _currencyRepository = currencyRepository;
            _accountRepository = accountRepository;
            _operationsManager = operationsManager;
            _operationRepository = operationRepository;
            _foreignOperationsDetailsRepository = foreignOperationsDetailsRepository;
            _foreignOperationsAccountingRepository = foreignOperationsAccountingRepository;
            _operationDetailsRepository = operationDetailsRepository;
            _paymentOrdersRepository = paymentOrdersRepository;
        }

        public ForeignOperationDto InitForm()
        
        {
            var appClient = GetCurrentTenant();

            var ret = new ForeignOperationDto
            {
                DataStart = _balanceRepository.BalanceDateNextDay(),
                DataEnd = LazyMethods.Now(),
                ShowList = true,
                ShowUploadForm = false,
                OkGenerate = false,
                IncludeGenerate = false,
                TenantId = appClient.Id,
                ThirdPartyId = appClient.LegalPersonId.Value
            };

            return ret;
        }

        public ForeignOperationDto OperSearch(ForeignOperationDto form)
        {
            var list = _foreignOperationsDetailsRepository.GetAll()
                                                          .Include(f => f.ForeignOperation).Include(f => f.ForeignOperation.BankAccount)
                                                          .Include(f => f.OperationsAccounting).Include(f => f.OperationsAccounting).ThenInclude(f => f.Debit)
                                                          .Include(f => f.OperationsAccounting).ThenInclude(f => f.Credit).Include(f => f.PaymentOrder).Include(f => f.DocumentType)
                                                          .Where(f => f.ForeignOperation.TenantId == form.TenantId && f.ForeignOperation.OperationDate >= form.DataStart
                                                                           && f.ForeignOperation.OperationDate <= form.DataEnd
                                                                           && f.ForeignOperation.BankAccountId == (form.BankAccountId ?? f.ForeignOperation.BankAccountId))
                                                          .ToList();

            if (form.QuickSearch != null)
            {
                list = list.Where(f => f.OriginalDetails.ToUpper().IndexOf(form.QuickSearch.ToUpper()) >= 0).ToList();
            }

            var listPrep = list.Select(f => new ForeignOperationList
            {
                Id = f.Id,
                ForeignOperId = f.ForeignOperation.Id,
                BankAccountId = f.ForeignOperation.BankAccountId,
                BankAccount = f.ForeignOperation.BankAccount.IBAN,
                CurrencyId = f.ForeignOperation.CurrencyId,
                DocumentTypeId = f.DocumentTypeId,
                DocumentTypeStr = f.DocumentType.TypeName,
                DocumentNumber = f.DocumentNumber,
                DocumentDate = f.DocumentDate,
                OperationDate = f.ForeignOperation.OperationDate,
                Value = f.Value,
                OriginalDetails = f.OriginalDetails,
                PaymentOrderId = f.PaymentOrderId,
                SelectedPaymentOrderId = f.PaymentOrderId,
                SelectedOP = f.PaymentOrderId != null ? true : false,
                SelectedPaymentOrderDetails = f.PaymentOrderId != null ? f.PaymentOrder.PaymentDetails : null,
                PaymentOrder = (f.PaymentOrderId != null) ? (f.PaymentOrder.OrderNr + " / " + LazyMethods.DateToString(f.PaymentOrder.OrderDate)) : ""
            })
            .OrderByDescending(f => f.OperationDate).ThenBy(f => f.Id)
            .ToList();

            foreach (var item in listPrep)
            {
                var accounting = _foreignOperationsAccountingRepository.GetAllIncluding(f => f.Debit, f => f.Credit)
                                       .Where(f => f.FgnOperationsDetailId == item.Id)
                                       .ToList()
                                       .Select(f => new ForeignOperationAccountingList
                                       {
                                           Id = f.Id,
                                           Details = f.Details,
                                           DebitAccountId = f.DebitId,
                                           DebitAccount = (f.DebitId == null) ? "" : f.Debit.Symbol + " " + f.Debit.AccountName,
                                           CreditAccountId = f.CreditId,
                                           CreditAccount = (f.CreditId == null) ? "" : f.Credit.Symbol + " " + f.Credit.AccountName,
                                           OperationsDetailId = f.OperationsDetailId,
                                           Value = f.Value,
                                           ValueCurr = f.ValueCurr,
                                           DetailOperId = f.FgnOperationsDetailId
                                       })
                                       .OrderBy(f => f.Id).ToList();
                item.AccountingList = accounting;

                var assignedOPIdsList = _foreignOperationsDetailsRepository.GetAllIncluding(f => f.ForeignOperation)
                                                                           .Where(f => f.ForeignOperation.State == State.Active && f.PaymentOrderId != null)
                                                                           .Select(f => f.PaymentOrderId)
                                                                           .ToList();
                var paymentOrders = _paymentOrdersRepository.GetAll().Where(f => f.PayerBankAccountId == item.BankAccountId && !assignedOPIdsList.Contains(f.Id) &&
                                                                    f.OrderDate >= item.OperationDate.AddDays(-3) && f.OrderDate <= item.OperationDate.AddDays(3) && f.State == State.Active).ToList().Select(f => new PaymentOrderForForeignOperationDto
                                                                    {
                                                                        Id = f.Id,
                                                                        PaymentDetails = f.OrderNr + " / " + LazyMethods.DateToString(f.OrderDate)
                                                                    }).ToList();

                item.PaymentOrdersList = paymentOrders;
            }


            form.OperList = form.IncludeGenerate == false ? listPrep.Where(f => f.AccountingList.Any(g => g.OperationsDetailId == null)).ToList() : listPrep;
            form.ShowList = true;
            form.ShowUploadForm = false;
            form = VerifyOkGenerate(form);

            return form;
        }

        public ForeignOperationDto SaveSelection(ForeignOperationDto form)
        {
            try
            {
                var appClient = GetCurrentTenant();
                foreach (var item in form.OperList.Where(f => !f.NCChildGenerated))
                {
                    var operationDetailDb = new ForeignOperationsDetails
                    {
                        Id = item.Id,
                        ForeignOperationId = item.ForeignOperId,
                        OriginalDetails = item.OriginalDetails,
                        TenantId = appClient.Id,
                        PaymentOrderId = item.PaymentOrderId,
                        Value = item.Value,
                        ValueCurr = item.ValueCurr,
                        DocumentTypeId = item.DocumentTypeId,
                        DocumentNumber = item.DocumentNumber,
                        DocumentDate = item.DocumentDate
                    };

                    var paymentOrder = _paymentOrdersRepository.GetAllIncluding(f => f.FgnOperationsDetail).FirstOrDefault(f => f.FgnOperDetailId == item.Id);
                    if (paymentOrder != null)
                    {
                        paymentOrder.Status = OperationStatus.Unchecked;
                        paymentOrder.FgnOperDetailId = null;
                        _paymentOrdersRepository.Update(paymentOrder);
                    }

                    var accountingListDb = new List<ForeignOperationsAccounting>();
                    if (item.AccountingList != null)
                    {
                        foreach (var itemDetail in item.AccountingList)
                        {
                            var accountDb = new ForeignOperationsAccounting
                            {
                                Id = itemDetail.Id,
                                TenantId = appClient.Id,
                                CreditId = itemDetail.CreditAccountId,
                                DebitId = itemDetail.DebitAccountId,
                                Details = itemDetail.Details,
                                FgnOperationsDetailId = item.Id,
                                Value = Math.Round(itemDetail.Value, 2),
                                ValueCurr = itemDetail.ValueCurr,
                                OperationsDetailId = itemDetail.OperationsDetailId
                            };
                            accountingListDb.Add(accountDb);
                        }
                    }

                    if (item.PaymentOrderId != null)
                    {
                        var paymentOrderDb = _paymentOrdersRepository.FirstOrDefault(f => f.Id == item.PaymentOrderId.Value);

                        paymentOrderDb.Status = OperationStatus.Checked;
                        paymentOrderDb.FgnOperDetailId = item.Id;
                        _paymentOrdersRepository.Update(paymentOrderDb);
                        CurrentUnitOfWork.SaveChanges();
                    }

                    operationDetailDb.OperationsAccounting = accountingListDb;

                    _foreignOperationRepository.UpdateFgnOperationDetail(operationDetailDb);
                }

                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
            form = OperSearch(form);
            return form;
        }

        public ForeignOperationDto AddAccountingRow(ForeignOperationDto form, int detailId)
        {
            var newItem = new ForeignOperationAccountingList
            {
                Id = 0,
                Value = 0,
                DetailOperId = detailId
            };
            var operationDetail = form.OperList.FirstOrDefault(f => f.Id == detailId);
            operationDetail.AccountingList.Add(newItem);

            return form;
        }

        public ForeignOperationDto AccountingRowDelete(ForeignOperationDto form, int detailId, int index)
        {
            var operationDetail = form.OperList.FirstOrDefault(f => f.Id == detailId);
            //operationDetail.AccountingList.RemoveAt(index);

            var accountingList = _foreignOperationsAccountingRepository.GetAllIncluding(f => f.FgnOperationsDetail)
                                                           .FirstOrDefault(f => f.FgnOperationsDetailId == detailId);


            _foreignOperationsAccountingRepository.Delete(accountingList);
            CurrentUnitOfWork.SaveChanges();

            form.OperList.RemoveAt(index);

            return form;
        }

        public ForeignOperationDto GenerateConta(ForeignOperationDto form)
        {
            var appClient = GetCurrentTenant();
            var localCurrencyId = appClient.LocalCurrencyId;
            var currencyId = _currencyRepository.GetAll().FirstOrDefault(f => f.Id == localCurrencyId && f.Status == State.Active);
            decimal value = 0, valueCurr = 0;

            var list = form.OperList.Where(f => f.AccountingList.Any(g => !g.NcGenerated)).ToList();
            if (list.Count == 0)
            {
                throw new UserFriendlyException("Eroare", "Nu sunt operatii care nu au fost generate in contabilitate");
            }

            var operHeader = list
                  .GroupBy(f => new { f.ForeignOperId, f.BankAccountId, f.DocumentNumber, f.DocumentDate, f.OperationDate, f.DocumentTypeId, f.CurrencyId })
                  .Select(f => new { f.Key.ForeignOperId, f.Key.BankAccountId, f.Key.DocumentNumber, f.Key.DocumentDate, f.Key.OperationDate, f.Key.DocumentTypeId, f.Key.CurrencyId });

            foreach (var itemHeader in operHeader)
            {
                if (!_operationRepository.VerifyClosedMonth(itemHeader.OperationDate))
                    throw new UserFriendlyException("Eroare", "Nu se poate inregistra operatia deoarece luna contabila este inchisa");

                var operation = new Operation
                {
                    TenantId = form.TenantId,
                    CurrencyId = itemHeader.CurrencyId,
                    OperationDate = itemHeader.OperationDate,
                    DocumentTypeId = itemHeader.DocumentTypeId,
                    DocumentNumber = itemHeader.DocumentNumber,
                    DocumentDate = itemHeader.DocumentDate,
                    OperationStatus = OperationStatus.Unchecked,
                    ExternalOperation = true,
                    State = State.Active
                };

                _operationRepository.Insert(operation);
                CurrentUnitOfWork.SaveChanges();
                int operationId = operation.Id;

                var detailsAccounting = list.Where(f => f.CurrencyId == itemHeader.CurrencyId && f.OperationDate == itemHeader.OperationDate && f.DocumentNumber == itemHeader.DocumentNumber
                                         && f.DocumentDate == itemHeader.DocumentDate && f.BankAccountId == itemHeader.BankAccountId && f.DocumentTypeId == itemHeader.DocumentTypeId)
                                  .Select(g => g.AccountingList);

                var detailList = new List<FOPrepareAccounting>();

                foreach (var listItem in detailsAccounting)
                {
                    foreach (var item in listItem)
                    {
                        var prepare = new FOPrepareAccounting
                        {
                            Id = item.Id,
                            DebitAccountId = item.DebitAccountId ?? 0,
                            CreditAccountId = item.CreditAccountId ?? 0,
                            Value = item.Value,
                            ValueCurr = item.ValueCurr,
                            Details = item.Details
                        };
                        detailList.Add(prepare);
                    }
                }

                var details = detailList.GroupBy(f => new { f.DebitAccountId, f.CreditAccountId, f.Details })
                                  .Select(f => new
                                  {
                                      f.Key.DebitAccountId,
                                      f.Key.CreditAccountId,
                                      f.Key.Details,
                                      Value = f.Sum(x => x.Value),
                                      Detail = f.Select(x => x.Id)
                                  });


                foreach (var itemDetail in details)
                {
                    if (localCurrencyId == itemHeader.CurrencyId)
                    {
                        value = itemDetail.Value;
                        valueCurr = 0;
                    }
                    else
                    {
                        value = itemDetail.Value;
                        valueCurr = 0;
                    }
                    var detail = new OperationDetails()
                    {
                        DebitId = itemDetail.DebitAccountId,
                        CreditId = itemDetail.CreditAccountId,
                        Value = value,
                        ValueCurr = valueCurr,
                        Details = itemDetail.Details.ToUpper(),
                        VAT = 0,
                        OperationId = operationId
                    };
                    //deductibilitate
                    detail = _operationsManager.GetOperationDetDeductibilitate(detail, operation.OperationDate);

                    _operationDetailsRepository.Insert(detail);
                    CurrentUnitOfWork.SaveChanges();
                    var operationDetailId = detail.Id;

                    var foreignDetails = _foreignOperationsAccountingRepository.GetAll()
                                              .Where(f => itemDetail.Detail.Contains(f.Id));
                    foreach (var item in foreignDetails)
                    {
                        item.OperationsDetailId = operationDetailId;
                    }
                    CurrentUnitOfWork.SaveChanges();
                }
            }

            form = OperSearch(form);

            return form;
        }

        public ForeignOperationDto InitDeleteForm(ForeignOperationDto form)
        {
            var appClient = GetCurrentTenant();

            var deleteForm = new FOInitDeleteDto
            {
                DataStart = _balanceRepository.BalanceDateNextDay(),
                DataEnd = LazyMethods.Now()
            };

            form.ShowDeleteForm = true;
            form.ShowList = false;
            form.DeleteForm = deleteForm;
            form = OperDeleteSearch(form);
            return form;
        }

        public ForeignOperationDto OperDeleteSearch(ForeignOperationDto form)
        {
            var list = _foreignOperationRepository.GetAll().Include(f => f.ForeignOperationsDetails).ThenInclude(f => f.OperationsAccounting).Include(f => f.BankAccount)
                           .Where(f => f.TenantId == form.TenantId && f.OperationDate >= form.DeleteForm.DataStart
                                  && f.OperationDate <= form.DeleteForm.DataEnd
                                  && f.BankAccountId == (form.DeleteForm.BankAccountId ?? f.BankAccountId))
                           .ToList();
            var listPrep = list.Select(f => new FODeleteList
            {
                ForeignOperId = f.Id,
                BankAccountId = f.BankAccountId,
                BankAccount = f.BankAccount.IBAN,
                DocumentNumber = f.DocumentNumber,
                DocumentDate = f.DocumentDate,
                OperationDate = f.OperationDate,
                NcGenerated = (f.ForeignOperationsDetails.Count(x => x.OperationsAccounting.Any(g => g.OperationsDetailId != null)) != 0)
            })
                            .OrderByDescending(f => f.OperationDate).ThenBy(f => f.ForeignOperId)
                            .ToList();
            form.DeleteForm.List = listPrep;

            return form;
        }

        public ForeignOperationDto UploadFileStart(ForeignOperationDto form)
        {
            var upload = new ForeignOperationUpload
            {
                DocumentDate = LazyMethods.Now(),
                DocumentNumber = "",
                OperationDate = LazyMethods.Now()
            };

            form.UploadFile = upload;
            form.ShowList = false;
            form.ShowUploadForm = true;

            return form;
        }

        public ForeignOperationDto UploadOperationFile(ForeignOperationDto form)
        {
            var lastBalanceDate = _balanceRepository.GetAll().Where(f => f.Status == State.Active && f.TenantId == form.TenantId).OrderByDescending(f => f.BalanceDate).FirstOrDefault().BalanceDate;

            if (form.UploadFile.OperationDate <= lastBalanceDate)
            {
                throw new Exception("Nu puteti sa incarcati extrase bancare pentru data " + LazyMethods.DateToString(form.UploadFile.OperationDate) + " deoarece luna contabila este inchisa");
            }

            var fileUpld = form.UploadFile.FileUpld;
            var separator1 = fileUpld.Content.IndexOf(";");
            var mimeType = fileUpld.Content.Substring(0, separator1);
            var separator2 = fileUpld.Content.IndexOf(",");
            var contentBase64 = fileUpld.Content.Substring(separator2 + 1);
            var contentBytes = Convert.FromBase64String(contentBase64);
            var str = Encoding.Default.GetString(contentBytes);
            var reader = new StringReader(str);
            reader.ReadLine();

            int accountId = 0;
            var account = _accountRepository.GetAll().FirstOrDefault(f => f.TenantId == form.TenantId && f.BankAccountId == (form.UploadFile.BankAccountId ?? 0));
            if (account != null)
            {
                accountId = account.Id;
            }

            if (accountId == 0)
            {
                throw new UserFriendlyException("Eroare", "Nu exista nici un cont contabil asociat contului bancar selectat. Realizati asocierea in Conta -> Nomenclatoare -> Plan de conturi!");
            }

            var countExtras = _foreignOperationRepository.GetAll().Count(f => f.OperationDate == form.UploadFile.OperationDate && f.BankAccountId == form.UploadFile.BankAccountId);
            if (countExtras != 0)
                throw new UserFriendlyException("Eroare", "A mai fost incarcat un extras pentru acest cont bancar!");

            try
            {
                _operationsManager.LoadForeignOperationsFromFile(form.UploadFile.DocumentTypeId ?? 0, form.UploadFile.OperationDate, form.UploadFile.DocumentNumber,
                                                                 form.UploadFile.DocumentDate, form.UploadFile.BankAccountId ?? 0, accountId, str);
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }

            form.ShowList = true;
            form.ShowUploadForm = false;
            CurrentUnitOfWork.SaveChanges();

            form = OperSearch(form);

            return form;

        }

        public ForeignOperationDto VerifyOkGenerate(ForeignOperationDto form)
        {
            var rowsWithoutAccount = form.OperList.Count(f => f.AccountingList.Any(g => g.CreditAccountId == null || g.DebitAccountId == null));
            var rowsNotGenerated = form.OperList.Count(f => f.AccountingList.Any(g => g.OperationsDetailId == null));

            if (rowsWithoutAccount == 0 && rowsNotGenerated != 0)
            {
                form.OkGenerate = true;
            }
            else
            {
                form.OkGenerate = false;
            }

            return form;
        }

        public ForeignOperationDto OperDeleteNC(int foreignOperationId, ForeignOperationDto form)
        {
            try
            {
                _foreignOperationRepository.DeleteNC(foreignOperationId);


                form = OperDeleteSearch(form);
                return form;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public ForeignOperationDto OperDeleteExtras(ForeignOperationDto form, int foreignOperationId)
        {
            try
            {
                var appClient = GetCurrentTenant();
                var foreignOperation = _foreignOperationRepository.FirstOrDefault(f => f.Id == foreignOperationId);
                var lastBalanceDate = _balanceRepository.GetAll().Where(f => f.TenantId == appClient.Id && f.Status == State.Active)
                                                            .OrderByDescending(f => f.BalanceDate)
                                                            .FirstOrDefault().BalanceDate;
                if (foreignOperation.OperationDate <= lastBalanceDate)
                {
                    throw new Exception("Nu puteti sa stergeti extrasul bancar pentru data " + LazyMethods.DateToString(foreignOperation.OperationDate) + " deoarece luna contabila este inchisa");
                }

                _foreignOperationRepository.DeleteFO(foreignOperationId);
                CurrentUnitOfWork.SaveChanges();

                form = OperDeleteSearch(form);
                return form;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public void CheckPaymentOrderForFgnOperDetail(int paymentOrderId, ForeignOperationDto form)
        {
            try
            {
                var paymentOrder = _paymentOrdersRepository.FirstOrDefault(f => f.Id == paymentOrderId);
                var checkPaymentOrder = form.OperList.FindAll(f => f.PaymentOrderId == paymentOrder.Id).Count();

                if (checkPaymentOrder > 1)
                {
                    throw new Exception("Ordinul de plata " + paymentOrder.OrderNr + " / " + LazyMethods.DateToString(paymentOrder.OrderDate) + " a fost selectat pentru un alt cont bancar");
                }
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }
    }
}
