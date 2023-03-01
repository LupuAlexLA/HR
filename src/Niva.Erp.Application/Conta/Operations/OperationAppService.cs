using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Niva.Erp.Common;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.EntityFrameworkCore.Repositories.Conta;
using Niva.Erp.Managers;
using Niva.Erp.Models.AutoOperation;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Contracts;
using Niva.Erp.Repositories;
using Niva.Erp.Repositories.Conta;
using Niva.Erp.Repositories.Conta.Nomenclatures;

namespace Niva.Erp.Conta.Operations
{
    public interface IOperationAppService : IApplicationService
    {
        OperationSearchDto InitSearch();

        OperationSearchDto SearchOperations(OperationSearchDto search);
        List<OperationDTO> SearchOperationsByOperGenerateId(int operGenerateId);

        OperationSearchDto SaveValidation(OperationSearchDto search);

        void DeleteOperation(int _operationId);

        OperationEditDto InitOperation(int operationId);

        OperationEditDto AddOperationDetail(OperationEditDto operation);

        OperationEditDto DeleteOperationDetail(OperationEditDto operation, int idOrd);

        OperationEditDto ActExchangeRate(OperationEditDto operation);

        OperationEditDto SaveOperation(OperationEditDto operation);

        void UploadOperationFile(FileUploadDto file);

        SoldOperationDto SoldOperation(int accountId, int currencyId, DateTime operationDate);
        void AllowDeletionOperExterna(bool allowDeletion);

        bool GetSetupStergOperExterna();
    }

    public class OperationAppService : ErpAppServiceBase, IOperationAppService
    {
        OperationRepository _operationRepository;
        IRepository<ExchangeRates> _exchangeRatesRepository;
        IRepository<OperationDetails> _operationDetailsRepository;
        OperationsManager _operationsManager;
        IBalanceRepository _balanceRepository;
        ICurrencyRepository _currencyRepository;
        IRepository<ActivityType> _activityTypeRepository;
        IAccountRepository _accountRepository;
        IRepository<SetupStergOperExterna> _setupStergOperExternaRepository;
        IRepository<AutoOperationDetail> _autoOperationDetailRepository;

        public OperationAppService(OperationRepository operationRepository, IRepository<ExchangeRates> exchangeRatesRepository,
                                   IRepository<OperationDetails> operationDetailsRepository, OperationsManager operationsManager,
                                   IBalanceRepository balanceRepository, ICurrencyRepository currencyRepository, IRepository<ActivityType> activityTypeRepository,
                                   IAccountRepository accountRepository, IRepository<SetupStergOperExterna> setupStergOperExternaRepository,
                                   IRepository<AutoOperationDetail> autoOperationDetailRepository)
        {
            _operationRepository = operationRepository;
            _exchangeRatesRepository = exchangeRatesRepository;
            _operationDetailsRepository = operationDetailsRepository;
            _operationsManager = operationsManager;
            _balanceRepository = balanceRepository;
            _currencyRepository = currencyRepository;
            _activityTypeRepository = activityTypeRepository;
            _accountRepository = accountRepository;
            _setupStergOperExternaRepository = setupStergOperExternaRepository;
            _autoOperationDetailRepository = autoOperationDetailRepository;
        }

        //[AbpAuthorize("Conta.OperContab.OperContab.Acces")]
        public OperationSearchDto InitSearch()
        {
            var ret = new OperationSearchDto
            {
                DataStart = _balanceRepository.BalanceDateNextDay(),
                DataEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
                //OperationStatus = (int)OperationStatus.Checked
            };
            //ret = SearchOperations(ret);
            return ret;
        }

        //[AbpAuthorize("Conta.OperContab.OperContab.Acces")]
        public OperationSearchDto SearchOperations(OperationSearchDto search)
        {
            var _operations = _operationRepository.GetAllIncluding(f => f.DocumentType, f => f.Currency, f => f.OperationType).Include(f => f.OperationsDetails).ThenInclude(g => g.Debit).Include(f => f.OperationsDetails).ThenInclude(g => g.Credit)
                                                  //GetAllIncluding(f => f.DocumentType, f => f.Currency, f => f.OperationsDetails, f => f.OperationsDetails.Select(g => g.Debit),
                                                  //                                                    f => f.OperationsDetails.Select(g => g.Credit))
                                                  .Where(f => f.State == State.Active && f.OperationDate >= search.DataStart && f.OperationDate <= search.DataEnd);

            if (search.OperationId != null)
                _operations = _operations.Where(f => f.Id == (search.OperationId ?? 0));
            if (search.DocumentTypeId != null)
                _operations = _operations.Where(f => f.DocumentTypeId == (search.DocumentTypeId ?? 0));
            //if (search.OperationStatus != null)
            //    _operations = _operations.Where(f => f.OperationStatus == (OperationStatus)(search.OperationStatus ?? 0));
            if (search.DocumentNumber != null && search.DocumentNumber != "")
                _operations = _operations.Where(f => f.DocumentNumber == search.DocumentNumber);
            if (search.Value != null)
                _operations = _operations.Where(f => f.OperationsDetails.Any(g => Math.Round(g.Value, 0) == Math.Round(search.Value ?? 0, 0)) || f.OperationsDetails.Sum(g => g.Value) == (search.Value ?? 0));
            if (search.Explication != null && search.Explication != "")
                _operations = _operations.Where(f => f.OperationsDetails.Any(g => g.Details.ToLower().IndexOf(search.Explication.ToLower()) >= 0));
            var _operationsList = _operations.ToList();

            if (search.Account1 != null && search.Account1 != "")
                _operationsList = _operationsList.Where(f => f.OperationsDetails.Any(g => g.Debit.Symbol.ToLower().IndexOf(search.Account1.ToLower()) >= 0) || f.OperationsDetails.Any(g => g.Credit.Symbol.ToLower().IndexOf(search.Account1.ToLower()) >= 0)).ToList();
            if (search.Account2 != null && search.Account2 != "")
                _operationsList = _operationsList.Where(f => f.OperationsDetails.Any(g => g.Debit.Symbol.ToLower().IndexOf(search.Account2.ToLower()) >= 0) || f.OperationsDetails.Any(g => g.Credit.Symbol.ToLower().IndexOf(search.Account2.ToLower()) >= 0)).ToList();

            if (search.OperationTypeId != null)
                _operationsList = _operationsList.Where(f => f.OperationTypeId == search.OperationTypeId).ToList();

            _operationsList = _operationsList.OrderByDescending(f => f.OperationDate).ThenByDescending(f => f.Id).ThenByDescending(f => f.OperationParentId).ToList();

            var operations = new List<OperationDTO>();
            foreach (var operation in _operationsList)
            {
                var operationDto = new OperationDTO
                {
                    Id = operation.Id,
                    OperationDate = operation.OperationDate,
                    DocumentDate = operation.DocumentDate,
                    DocumentNumber = operation.DocumentNumber,
                    DocumentTypeId = operation.DocumentTypeId,
                    DocumentType = operation.DocumentType.TypeName,
                    OperationTypeId = operation.OperationTypeId,
                    OperationType = operation.OperationType?.Name,
                    CurrencyId = operation.CurrencyId,
                    CurrencyName = operation.Currency.CurrencyName,
                    //OperationStatus = (operation.OperationStatus == OperationStatus.Checked),
                    // OperationStatusDB = (operation.OperationStatus == OperationStatus.Checked),
                    Value = 0,
                    ValueCurr = 0,
                    ExternalOperation = operation.ExternalOperation
                };
                var operationDetails = new List<OperationDetailsDTO>();
                decimal value = 0, valueCurr = 0;
                foreach (var detail in operation.OperationsDetails.OrderBy(f => f.Id))
                {
                    var operationDetail = new OperationDetailsDTO
                    {
                        Id = detail.Id,
                        DebitId = detail.DebitId,
                        Debit = detail.Debit.Symbol,
                        DebitName = detail.Debit.AccountName,
                        CreditId = detail.CreditId,
                        Credit = detail.Credit.Symbol,
                        CreditName = detail.Credit.AccountName,
                        Details = detail.Details,
                        Value = detail.Value,
                        ValueCurr = detail.ValueCurr,
                        VAT = detail.VAT,
                        DetailNr = detail.DetailNr
                    };
                    value += detail.Value;
                    valueCurr += detail.ValueCurr;

                    operationDetails.Add(operationDetail);
                }
                operationDto.Value = value;
                operationDto.ValueCurr = valueCurr;
                operationDto.OperationsDetails = operationDetails;
                operations.Add(operationDto);
            }

            if (search.Value != null)
                operations = operations.Where(f => f.Value == (search.Value ?? 0)).ToList();


            //search.Operations = operations.Where(f => f.Value != 0).ToList();
            search.Operations = operations.Where(f => f.OperationsDetails.Count != 0).ToList();
            return search;
        }

        [AbpAuthorize("Conta.OperContab.OperContab.Modificare")]
        public OperationSearchDto SaveValidation(OperationSearchDto search)
        {
            string message = "";
            try
            {
                foreach (var item in search.Operations)
                {
                    var _operation = _operationRepository.FirstOrDefault(f => f.Id == item.Id);
                    if (!_operationRepository.VerifyClosedMonth(item.OperationDate))
                    {
                        message = "Nu se poate inregistra operatia deoarece luna contabila este inchisa";
                        throw new UserFriendlyException("Eroare", message);
                    }
                    // _operation.OperationStatus = (OperationStatus)(item.OperationStatus == true ? 1 : 0);
                    _operationRepository.Update(_operation);
                    // item.OperationStatusDB = item.OperationStatus;
                }
                CurrentUnitOfWork.SaveChanges();
                return search;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message + " " + message);
            }
        }

        [AbpAuthorize("Conta.OperContab.OperContab.Modificare")]
        public void DeleteOperation(int _operationId)
        {
            var _operation = _operationRepository.FirstOrDefault(f => f.Id == _operationId);

            if (!_operationRepository.VerifyClosedMonth(_operation.OperationDate))
                throw new UserFriendlyException("Eroare", "Nu se poate inregistra operatia deoarece luna contabila este inchisa");

            try
            {
                var _childOperation = _operationRepository.FirstOrDefault(f => f.OperationParentId == _operationId);
                if (_childOperation != null)
                {
                    _childOperation.State = State.Inactive;
                    _operationRepository.Update(_childOperation);
                }
                _operation.State = State.Inactive;
                _operationRepository.Update(_operation);
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        //[AbpAuthorize("Conta.OperContab.OperContab.Acces")]
        public OperationEditDto InitOperation(int operationId)
        {
            var selectedTenant = GetCurrentTenant();
            var localCurrencyId = selectedTenant.LocalCurrencyId.Value;

            var last10 = SearchLast10Operations();

            if (operationId == 0)
            {
                var ret = new OperationEditDto
                {
                    OperationDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                    DocumentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                    //OperationStatus = OperationStatus.Unchecked,
                    ClosingMonth = false,
                    CurrencyId = localCurrencyId,
                    LocalCurrencyId = localCurrencyId,
                    ExchangeRate = 0
                };
                var details = new List<OperationEditDetailsDto>();
                ret.OperationDetails = details;

                ret = AddOperationDetail(ret);
                ret.Last10 = last10;

                return ret;
            }
            else
            {
                var _operation = _operationRepository.GetAll().Include(f => f.OperationsDetails).ThenInclude(f => f.Credit)
                                                              .Include(f => f.OperationsDetails).ThenInclude(f => f.Debit).FirstOrDefault(f => f.Id == operationId);
                var ret = ObjectMapper.Map<OperationEditDto>(_operation);
                ret.LocalCurrencyId = localCurrencyId;
                ret.ExchangeRate = GetExchangeRate(ret.DocumentDate, ret.CurrencyId, ret.LocalCurrencyId);

                int index = 1;
                var _details = new List<OperationEditDetailsDto>();
                foreach (var item in _operation.OperationsDetails)
                {
                    var _detail = ObjectMapper.Map<OperationEditDetailsDto>(item);
                    _detail.IdOrd = index;
                    _details.Add(_detail);
                }
                ret.OperationDetails = _details;

                ret.Last10 = last10;
                return ret;
            }
        }

        [AbpAuthorize("Conta.OperContab.OperContab.Modificare")]
        public OperationEditDto AddOperationDetail(OperationEditDto operation)
        {
            int maxIdOrd = 0;
            if (operation.OperationDetails.Count != 0)
            {
                maxIdOrd = operation.OperationDetails.Max(f => f.IdOrd);
            }
            maxIdOrd++;
            var detail = new OperationEditDetailsDto
            {
                Value = 0,
                ValueCurr = 0,
                VAT = 0,
                IdOrd = maxIdOrd
            };
            operation.OperationDetails.Add(detail);

            return operation;
        }

        [AbpAuthorize("Conta.OperContab.OperContab.Modificare")]
        public OperationEditDto DeleteOperationDetail(OperationEditDto operation, int idOrd)
        {
            var detail = operation.OperationDetails.FirstOrDefault(f => f.IdOrd == idOrd);
            operation.OperationDetails.Remove(detail);

            return operation;
        }

        [AbpAuthorize("Conta.OperContab.OperContab.Modificare")]
        public OperationEditDto ActExchangeRate(OperationEditDto operation)
        {
            //operation.ExchangeRate = GetExchangeRate(operation.DocumentDate, operation.CurrencyId, operation.LocalCurrencyId);

            foreach (var item in operation.OperationDetails)
            {
                item.ValueCurr = (operation.ExchangeRate == 0) ? 0 : Math.Round(item.Value / operation.ExchangeRate, 2);
            }

            return operation;
        }

        //[AbpAuthorize("Conta.OperContab.OperContab.Acces")]
        protected decimal GetExchangeRate(DateTime exchangeDate, int currencyId, int localCurrencyId)
        {
            decimal ret = 0;
            if (currencyId == localCurrencyId)
            {
                ret = 0;
            }
            else
            {
                var exchangeRate = _exchangeRatesRepository.GetAll()
                                                           .Where(f => f.CurrencyId == currencyId && f.ExchangeDate < exchangeDate)
                                                           .OrderByDescending(f => f.ExchangeDate)
                                                           .FirstOrDefault();
                if (exchangeRate == null)
                {
                    ret = 0;
                }
                else
                {
                    ret = exchangeRate.Value;
                }
            }

            return ret;
        }

        [AbpAuthorize("Conta.OperContab.OperContab.Modificare")]
        public OperationEditDto SaveOperation(OperationEditDto operation)
        {
            try
            {
                var appClient = GetCurrentTenant();
                var localCurrencyId = appClient.LocalCurrencyId.Value;
                if (operation.OperationDetails.Count == 0)
                {
                    throw new Exception("Nu ati completat nici un detaliu pentru aceasta operatie!");
                }
                if (!_operationRepository.VerifyClosedMonth(operation.OperationDate))
                    throw new Exception("Nu se poate inregistra operatia deoarece luna contabila este inchisa");

                if (operation.DocumentTypeId == 0)
                {
                    throw new Exception("Nu ati selectat tipul documentului!");
                }

                var _operation = ObjectMapper.Map<Operation>(operation);
                _operation.TenantId = appClient.Id;
                var _operationDetails = new List<OperationDetails>();
                foreach (var item in operation.OperationDetails)
                {
                    var _operationDetail = ObjectMapper.Map<OperationDetails>(item);
                    //deductibilitate
                    _operationDetail = _operationsManager.GetOperationDetDeductibilitate(_operationDetail, _operation.OperationDate);
                    _operationDetails.Add(_operationDetail);
                }

                if (operation.Id == 0) // Adaugare
                {
                    _operation.OperationsDetails = _operationDetails;
                    _operation.ExternalOperation = false;
                    _operationRepository.Insert(_operation);

                    CurrentUnitOfWork.SaveChanges();
                }
                else // modificare
                {
                    _operationRepository.Update(_operation);
                    CurrentUnitOfWork.SaveChanges();
                    // sterg detaliile anterioare
                    var _oldDetails = _operationDetailsRepository.GetAll().Where(f => f.OperationId == _operation.Id);
                    foreach (var item in _oldDetails)
                    {
                        _operationDetailsRepository.Delete(item);
                    }


                    //CurrentUnitOfWork.SaveChanges();
                    // adaug detaliile noi
                    foreach (var item in _operationDetails)
                    {
                        var autoOperationDetail = _autoOperationDetailRepository.GetAllIncluding(f => f.OperationDetail).FirstOrDefault(f => f.OperationDetailId.Value == item.Id);
                        item.OperationId = _operation.Id;
                        item.Id = 0;
                        _operationDetailsRepository.Insert(item);

                        CurrentUnitOfWork.SaveChanges();

                        if (autoOperationDetail != null)
                        {
                            autoOperationDetail.CreditAccountId = item.CreditId;
                            autoOperationDetail.DebitAccountId = item.DebitId;
                            autoOperationDetail.OperationDetailId = item.Id;
                            _autoOperationDetailRepository.Update(autoOperationDetail);
                            CurrentUnitOfWork.SaveChanges();
                        }
                    }
                }

                // salvez operatiile in valuta care au conturi de cheltuiala sau venit
                if (operation.CurrencyId != localCurrencyId)
                {
                    // sterg operatia copil anterioara daca e cazul
                    var operationChildOld = _operationRepository.GetAll().FirstOrDefault(f => f.OperationParentId == _operation.Id);
                    if (operationChildOld != null)
                    {
                        foreach (var detail in _operationDetailsRepository.GetAll().Where(f => f.OperationId == operationChildOld.Id))
                        {
                            _operationDetailsRepository.Delete(detail);
                        }
                        CurrentUnitOfWork.SaveChanges();
                        _operationRepository.Delete(operationChildOld);

                        CurrentUnitOfWork.SaveChanges();
                    }

                    var _operationChild = ObjectMapper.Map<Operation>(operation);
                    _operationChild.Id = 0;
                    _operationChild.TenantId = appClient.Id;
                    _operationChild.CurrencyId = localCurrencyId;
                    _operationChild.ExternalOperation = true;
                    _operationRepository.Insert(_operationChild);
                    CurrentUnitOfWork.SaveChanges();

                    var _operationChildDetails = new List<OperationDetails>();

                    var activityTypeId = _activityTypeRepository.GetAll().FirstOrDefault(f => f.Status == State.Active && f.MainActivity).Id;
                    int pozitieSchimbAccountId = 0;
                    int contravaloarePozitieSchimbAccountId = 0;
                    var pozitieSchimbAccount = _accountRepository.GetAll()
                                                                 .FirstOrDefault(f => f.AccountFuncType == AccountFuncType.PozitieSchimbCheltuieli
                                                                                 && f.ActivityTypeId == activityTypeId && f.CurrencyId == operation.CurrencyId);
                    if (pozitieSchimbAccount == null)
                    {
                        throw new Exception("Nu am identificat contul contabil pentru pozitia de schimb");
                    }
                    pozitieSchimbAccountId = pozitieSchimbAccount.Id;

                    var currencyName = _currencyRepository.GetAll().FirstOrDefault(f => f.Id == operation.CurrencyId).CurrencyCode;
                    var contravaloarePozitieSchimbAccount = _accountRepository.GetAll()
                                                                              .FirstOrDefault(f => f.AccountFuncType == AccountFuncType.ContravaloarePozitieSchimbCheltuieli
                                                                                              && f.ActivityTypeId == activityTypeId && f.AccountName.IndexOf(currencyName) >= 0);
                    if (contravaloarePozitieSchimbAccount == null)
                    {
                        throw new Exception("Nu am identificat contul contabil pentru contravaloarea pozitiei de schimb");
                    }
                    contravaloarePozitieSchimbAccountId = contravaloarePozitieSchimbAccount.Id;

                    var exchangeRate = GetExchangeRate(operation.OperationDate, operation.CurrencyId, localCurrencyId);

                    foreach (var detail in _operationDetails)
                    {
                        var debitId = detail.DebitId;
                        var creditId = detail.CreditId;
                        var symbolDebit = _accountRepository.GetAll().FirstOrDefault(f => f.Id == detail.DebitId).Symbol;
                        var symbolCredit = _accountRepository.GetAll().FirstOrDefault(f => f.Id == detail.CreditId).Symbol;
                        if (symbolCredit.IndexOf("7") == 0 || symbolDebit.IndexOf("6") == 0)
                        {
                            _operationChild.OperationParentId = _operation.Id;

                            var operChildDetail = new OperationDetails
                            {
                                DebitId = detail.DebitId,
                                CreditId = detail.CreditId,
                                DetailNr = detail.DetailNr,
                                OperationId = _operationChild.Id,
                                Value = Math.Round(detail.Value * exchangeRate, 2),
                                Details = detail.Details + ", curs valutar: " + exchangeRate.ToString("N4")
                            };

                            if (symbolCredit.IndexOf("7") == 0)
                            {
                                operChildDetail.DebitId = contravaloarePozitieSchimbAccountId;
                                operChildDetail.CreditId = creditId;

                                detail.CreditId = pozitieSchimbAccountId;
                            }

                            if (symbolDebit.IndexOf("6") == 0)
                            {
                                operChildDetail.DebitId = debitId;
                                operChildDetail.CreditId = contravaloarePozitieSchimbAccountId;

                                detail.DebitId = pozitieSchimbAccountId;
                            }
                            _operationChildDetails.Add(operChildDetail);
                        }

                    }

                    if (_operationChildDetails.Count != 0)
                    {
                        foreach (var item in _operationChildDetails)
                        {
                            _operationDetailsRepository.Insert(item);
                        }
                    }
                    else
                    {
                        _operationRepository.Delete(_operationChild);
                    }
                }

                CurrentUnitOfWork.SaveChanges();

                operation = InitOperation(0);
                operation.OperationDate = _operation.OperationDate;
                operation.DocumentDate = _operation.DocumentDate;

                return operation;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        //[AbpAuthorize("Conta.OperContab.OperContab.Acces")]
        public void UploadOperationFile(FileUploadDto file)
        {
            var separator1 = file.Content.IndexOf(";");
            var mimeType = file.Content.Substring(0, separator1);
            var separator2 = file.Content.IndexOf(",");
            var contentBase64 = file.Content.Substring(separator2 + 1);
            var contentBytes = Convert.FromBase64String(contentBase64);
            var str = Encoding.Default.GetString(contentBytes);
            var reader = new StringReader(str);
            reader.ReadLine();
            try
            {
                _operationsManager.LoadOperationsFromFile(str);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", LazyMethods.GetErrMessage(ex));
            }

        }

        //[AbpAuthorize("Conta.OperContab.OperContab.Acces")]
        public List<OperationLast10DTO> SearchLast10Operations()
        {
            var _operations = _operationRepository.GetAllIncluding(f => f.DocumentType, f => f.Currency, f => f.OperationsDetails, f => f.OperationType)
                                                  .Where(f => f.State == State.Active);

            var _operationsList = _operations.OrderByDescending(f => f.Id).Take(10).ToList();

            var operations = new List<OperationLast10DTO>();
            foreach (var operation in _operationsList)
            {
                var operationDto = new OperationLast10DTO
                {
                    Id = operation.Id,
                    OperationDate = operation.OperationDate,
                    DocumentDate = operation.DocumentDate,
                    DocumentNumber = operation.DocumentNumber,
                    DocumentTypeId = operation.DocumentTypeId,
                    DocumentType = operation.DocumentType.TypeName,
                    OperationTypeId = operation.OperationTypeId,
                    OperationType = operation.OperationType?.Name,
                    CurrencyId = operation.CurrencyId,
                    CurrencyName = operation.Currency.CurrencyName,
                    //OperationStatus = (operation.OperationStatus == OperationStatus.Checked),
                    //OperationStatusDB = (operation.OperationStatus == OperationStatus.Checked),
                    Value = 0,
                    ValueCurr = 0
                };
                decimal value = 0, valueCurr = 0;
                foreach (var detail in operation.OperationsDetails.OrderBy(f => f.Id))
                {
                    value += detail.Value;
                    valueCurr += detail.ValueCurr;
                }
                operationDto.Value = value;
                operationDto.ValueCurr = valueCurr;
                operations.Add(operationDto);
            }

            return operations;
        }

        //[AbpAuthorize("Conta.OperContab.OperContab.Acces")]
        public List<OperationDTO> SearchOperationsByOperGenerateId(int operGenerateId)
        {
            var _operationsList = _operationRepository.GetAllIncluding(f => f.DocumentType, f => f.Currency, f => f.OperationType, f => f.OperGenerate).Include(f => f.OperationsDetails)
                                                      .ThenInclude(g => g.Debit)
                                                      .Include(f => f.OperationsDetails)
                                                      .ThenInclude(g => g.Credit)
                                                      .Where(f => f.OperGenerateId == operGenerateId).ToList();

            var operations = new List<OperationDTO>();
            foreach (var operation in _operationsList)
            {
                var operationDto = new OperationDTO
                {
                    Id = operation.Id,
                    OperationDate = operation.OperationDate,
                    DocumentDate = operation.DocumentDate,
                    DocumentNumber = operation.DocumentNumber,
                    DocumentTypeId = operation.DocumentTypeId,
                    DocumentType = operation.DocumentType.TypeName,
                    OperationTypeId = operation.OperationTypeId,
                    OperationType = operation.OperationType?.Name,
                    CurrencyId = operation.CurrencyId,
                    CurrencyName = operation.Currency.CurrencyName,
                    //OperationStatus = (operation.OperationStatus == OperationStatus.Checked),
                    //OperationStatusDB = (operation.OperationStatus == OperationStatus.Checked),
                    Value = 0,
                    ValueCurr = 0,
                    ExternalOperation = operation.ExternalOperation
                };
                var operationDetails = new List<OperationDetailsDTO>();
                decimal value = 0, valueCurr = 0;
                foreach (var detail in operation.OperationsDetails.OrderBy(f => f.Id))
                {
                    var operationDetail = new OperationDetailsDTO
                    {
                        Id = detail.Id,
                        DebitId = detail.DebitId,
                        Debit = detail.Debit.Symbol,
                        DebitName = detail.Debit.AccountName,
                        CreditId = detail.CreditId,
                        Credit = detail.Credit.Symbol,
                        CreditName = detail.Credit.AccountName,
                        Details = detail.Details,
                        Value = detail.Value,
                        ValueCurr = detail.ValueCurr,
                        VAT = detail.VAT,
                        DetailNr = detail.DetailNr
                    };
                    value += detail.Value;
                    valueCurr += detail.ValueCurr;

                    operationDetails.Add(operationDetail);
                }
                operationDto.Value = value;
                operationDto.ValueCurr = valueCurr;
                operationDto.OperationsDetails = operationDetails;
                operations.Add(operationDto);
            }

            return operations;
        }

        //[AbpAuthorize("Conta.OperContab.OperContab.Acces")]
        public SoldOperationDto SoldOperation(int accountId, int currencyId, DateTime operationDate)
        {
            try
            {
                var appClient = GetCurrentTenant();
                var account = _accountRepository.FirstOrDefault(f => f.Id == accountId);
                //var currentDate = LazyMethods.Now();

                var soldInitObj = _balanceRepository.GetSoldTypeAccount(operationDate, account.Id, appClient.Id, currencyId, appClient.LocalCurrencyId.Value, false, "");
                var soldInit = soldInitObj.Sold;
                var soldInitType = soldInitObj.TipSold;

                var soldOper = new SoldOperationDto
                {
                    CurrentDate = operationDate,
                    SoldInitial = Math.Abs(soldInit),
                    SoldInitialType = soldInitType
                };

                return soldOper;

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public void AllowDeletionOperExterna(bool allowDeletion)
        {
            try
            {
                var appClientId = GetCurrentTenant().Id;
                var existingSetup = _setupStergOperExternaRepository.FirstOrDefault(f => f.TenantId == appClientId);


                if (existingSetup == null) // INSERT
                {
                    _setupStergOperExternaRepository.Insert(new SetupStergOperExterna
                    {
                        TenantId = appClientId,
                        AllowDeletion = allowDeletion
                    });
                }
                else
                {
                    existingSetup.AllowDeletion = allowDeletion;
                    _setupStergOperExternaRepository.Update(existingSetup);
                }
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public bool GetSetupStergOperExterna()
        {
            var appClientId = GetCurrentTenant().Id;
            var ret = _setupStergOperExternaRepository.FirstOrDefault(f => f.TenantId == appClientId);

            if (ret != null)
            {
                return ret.AllowDeletion;
            }

            return false;
        }
    }
}
