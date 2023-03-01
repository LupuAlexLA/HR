using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Contracts;
using Niva.Erp.Models.PrePayments;
using Niva.Erp.Repositories;
using Niva.Erp.Repositories.Conta;
using Niva.Erp.Repositories.Conta.AutoOperation;
using Niva.Erp.Repositories.Conta.Nomenclatures;
using Niva.Erp.Repositories.Conta.Prepayments;
using Niva.Erp.Repositories.Economic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Conta.Prepayments
{
    public interface IPrepaymentsAppService : IApplicationService
    {
        List<PrepaymentsListDto> PrepaymentsEntryList(int prepaymentType, DateTime dataStart, DateTime dataEnd);
        PrepaymentsAddDto AddFromInvoiceInit(int prepaymentType);
        List<PrepaymentsListDto> GetPrepaymentsForInvoiceDetails(int invoiceId);
        PrepaymentsAddDto InvoiceDetails(PrepaymentsAddDto oper);
        PrepaymentsAddDto SavePrepayments(PrepaymentsAddDto oper);
        void DeletePrepayment(int prepaymentId);
        PrepaymentsAddDirectDto SavePrepaymentDirect(PrepaymentsAddDirectDto oper);
        PrepaymentsAddDirectDto AddDirectInit(int prepaymentId, int prepaymentType);

        PrepaymentsAddDirectDto AddDirectChangeDate(PrepaymentsAddDirectDto oper);
        int CalcDurata(DateTime startDate, DateTime endDate, int modCalcul);
        PrepaymentsExitDto PrepaymentsExitInit(int prepaymentId, int prepaymentType);
        PrepaymentsExitDto SavePrepaymentExit(PrepaymentsExitDto oper);
        PrepaymentsDurationSetupDto DurationSetupDetails(int prepaymentType);
        PrepaymentsDurationSetupDto DurationSetupSave(PrepaymentsDurationSetupDto setup);
        PrepaymentsDecDeprecSetupDto DecDeprecSetupDetails(int prepaymentType);
        PrepaymentsDecDeprecSetupDto DecDeprecSetupSave(PrepaymentsDecDeprecSetupDto setup);

    }

    public class PrepaymentsAppService : ErpAppServiceBase, IPrepaymentsAppService
    {
        IPrepaymentRepository _prepaymentRepository;
        IRepository<PrepaymentDocType> _prepaymentOperDocTypeRepository;
        IRepository<PrepaymentBalance> _prepaymentBalanceRepository;
        IInvoiceRepository _invoiceRepository;
        IOperationRepository _operationRepository;
        IRepository<Currency> _currencyRepository;
        IExchangeRatesRepository _exchangeRatesRepository;
        IRepository<PrepaymentsDurationSetup> _prepaymentsDurationSetupRepository;
        IRepository<PrepaymentsDecDeprecSetup> _prepaymentsDecDeprecSetupRepository;
        IAutoOperationRepository _autoOperationRepository;
        IBalanceRepository _balanceRepository;
        IAccountRepository _accountRepository;
        public PrepaymentsAppService(IPrepaymentRepository prepaymentRepository, IRepository<PrepaymentDocType> prepaymentOperDocTypeRepository, IInvoiceRepository invoiceRepository,
                                     IOperationRepository operationRepository, IExchangeRatesRepository exchangeRatesRepository, IRepository<Currency> currencyRepository,
                                     IRepository<PrepaymentsDurationSetup> prepaymentsDurationSetupRepository, IRepository<PrepaymentsDecDeprecSetup> prepaymentsDecDeprecSetupRepository,
                                     IRepository<PrepaymentBalance> prepaymentBalanceRepository, IAutoOperationRepository autoOperationRepository, IBalanceRepository balanceRepository, IAccountRepository accountRepository)
        {
            _prepaymentRepository = prepaymentRepository;
            _prepaymentOperDocTypeRepository = prepaymentOperDocTypeRepository;
            _invoiceRepository = invoiceRepository;
            _operationRepository = operationRepository;
            _exchangeRatesRepository = exchangeRatesRepository;
            _currencyRepository = currencyRepository;
            _prepaymentsDurationSetupRepository = prepaymentsDurationSetupRepository;
            _prepaymentsDecDeprecSetupRepository = prepaymentsDecDeprecSetupRepository;
            _prepaymentBalanceRepository = prepaymentBalanceRepository;
            _autoOperationRepository = autoOperationRepository;
            _balanceRepository = balanceRepository;
            _accountRepository = accountRepository;
        }
        //[AbpAuthorize("Administrare.CheltAvans.Setup.Acces")]
        public List<PrepaymentsListDto> PrepaymentsEntryList(int prepaymentType, DateTime dataStart, DateTime dataEnd)
        {
            PrepaymentType _prepaymentType = (PrepaymentType)prepaymentType;
            DateTime _dataStart = new DateTime(dataStart.Year, dataStart.Month, dataStart.Day);
            DateTime _dataEnd = new DateTime(dataEnd.Year, dataEnd.Month, dataEnd.Day);

            var _prepaymentList = _prepaymentRepository.GetAllIncluding(f => f.InvoiceDetails, f => f.InvoiceDetails.Invoices,
                                                                   f => f.InvoiceDetails.Invoices.ThirdParty, f => f.InvoiceDetails.Invoices.ThirdParty,
                                                                   f => f.ThirdParty, f => f.PrimDocumentType)
                                                    .Where(f => f.State == State.Active && f.PrepaymentType == _prepaymentType
                                                           && f.PaymentDate <= _dataEnd && f.PaymentDate >= _dataStart)
                                                    .OrderBy(f => f.PaymentDate);

            var ret = ObjectMapper.Map<List<PrepaymentsListDto>>(_prepaymentList);
            return ret;
        }

        //[AbpAuthorize("Administrare.CheltAvans.Setup.Acces")]
        public PrepaymentsAddDto AddFromInvoiceInit(int prepaymentType)
        {
            var ret = new PrepaymentsAddDto
            {
                OperationDate = LazyMethods.Now(),
                PrepaymentType = (PrepaymentType)prepaymentType,
                ShowForm1 = true
            };

            var documentType = _prepaymentOperDocTypeRepository.GetAllIncluding(f => f.DocumentType)
                                                             .Where(f => f.OperType == PrepaymentOperType.Constituire)
                                                             .FirstOrDefault();
            ret.DocumentTypeId = documentType.DocumentType.Id;
            ret.DocumentType = documentType.DocumentType.TypeName;

            ret.FinishAdd = false;

            ret = ShowForm(ret, 1);
            return ret;
        }

        public PrepaymentsAddDto ShowForm(PrepaymentsAddDto oper, int formNr)
        {
            oper.ShowForm1 = (formNr == 1);
            oper.ShowForm2 = (formNr == 2);
            return oper;
        }

        public PrepaymentsAddDto InvoiceDetails(PrepaymentsAddDto oper)
        {
            if (oper.InvoiceId == null)
            {
                throw new UserFriendlyException("Eroare adaugare venit/cheltuiala in avans", "Nu ati selectat factura!");
            }

            var appClient = GetCurrentTenant();
            var localCurrencyId = appClient.LocalCurrencyId;
            //var localCurrency = 
            if (!_operationRepository.VerifyClosedMonth(oper.OperationDate))
                throw new UserFriendlyException("Eroare", "Nu se poate inregistra operatia deoarece luna contabila este inchisa");

            //DateTime lastProcessedDate = _imoOperationRepository.LastProcessedDateAdd(selectedAppClientId);
            //if (lastProcessedDate >= oper.OperationDate)
            //    throw new UserFriendlyException("Eroare data operatie", "Data operatiei nu poate fi mai mica decat data ultimei operatii procesate in gestiune " + lastProcessedDate.ToShortDateString());

            var _invoice = _invoiceRepository.GetAllIncludeElemDet().FirstOrDefault(f => f.Id == oper.InvoiceId);
            var _details = new List<PrepaymentsAddInvoiceDetailDto>();


            var elementType = (oper.PrepaymentType == PrepaymentType.CheltuieliInAvans ? InvoiceElementsType.CheltuieliInAvans : InvoiceElementsType.VenituriInAvans);
            foreach (var item in _invoice.InvoiceDetails.Where(f => f.InvoiceElementsDetails.InvoiceElementsType == elementType))
            {
                var _detail = new PrepaymentsAddInvoiceDetailDto
                {
                    InvoiceId = oper.InvoiceId ?? 0,
                    PrepaymentName = item.Element,
                    DocumentDate = _invoice.InvoiceDate,
                    DocumentNr = _invoice.InvoiceNumber,
                    Quantity = item.Quantity,
                    InvoiceDetailsId = item.Id,
                    PrimDocumentTypeId = oper.DocumentTypeId,
                    PrimDocumentNr = item.Invoices.InvoiceNumber,
                    PrimDocumentDate = item.Invoices.InvoiceDate,
                    ThirdPartyId = item.Invoices.ThirdPartyId,
                    DepreciationStartDate = LazyMethods.FirstDayNextMonth(oper.OperationDate),
                };
                decimal curs = _exchangeRatesRepository.GetLocalExchangeRate(_invoice.InvoiceDate, _invoice.CurrencyId, localCurrencyId.Value);
                if (oper.InregVAT)
                {
                    _detail.Value = Math.Round(curs * item.Value, 2);
                    _detail.VAT = Math.Round(curs * item.VAT, 2);
                }
                else
                {
                    _detail.Value = Math.Round(curs * (item.Value + item.VAT), 2);
                }
                _details.Add(_detail);
            }
            oper.InvoiceDetail = _details;
            oper = ShowForm(oper, 2);

            return oper;
        }

        public PrepaymentsAddDto SavePrepayments(PrepaymentsAddDto oper)
        {
            try
            {
                //verificare completare date
                foreach (var item in oper.InvoiceDetail)
                {
                    if ((item.PrepaymentAccountId == null) || (item.DepreciationAccountId == null) || (item.DurationInMonths == null))
                    {
                        throw new UserFriendlyException("Eroare adaugare", "Trebuie sa selectati conturile corespunzatoare si sa completati durata de amortizare");
                    }
                }

                foreach (var item in oper.InvoiceDetail)
                {
                    var invoice = _invoiceRepository.GetAll().FirstOrDefault(f => f.Id == item.InvoiceId);

                    var account = _accountRepository.GetAll().FirstOrDefault(f => f.Id == item.PrepaymentAccountId && f.Status == State.Active);
                    int prepaymentAccountId = 0;
                    if (invoice.ActivityTypeId != null && account.Symbol.StartsWith('6'))
                    {
                        prepaymentAccountId = _autoOperationRepository.GetAutoAccountActivityType(account.Symbol, item.ThirdPartyId, invoice.ActivityTypeId.Value, oper.OperationDate, invoice.CurrencyId, null);
                    }
                    else
                    {
                        prepaymentAccountId = item.PrepaymentAccountId.Value;
                    }

                    var prepayment = new Prepayment
                    {
                        Description = item.PrepaymentName,
                        DurationInMonths = item.DurationInMonths ?? 0,
                        InvoiceDetailsId = item.InvoiceDetailsId ?? 0,
                        PaymentDate = new DateTime(oper.OperationDate.Year, oper.OperationDate.Month, oper.OperationDate.Day),
                        PrepaymentAccountId = prepaymentAccountId,
                        DepreciationAccountId = item.DepreciationAccountId,
                        PrepaymentType = oper.PrepaymentType,
                        PrepaymentValue = item.Value,
                        PrimDocumentTypeId = item.PrimDocumentTypeId,
                        PrimDocumentNr = item.PrimDocumentNr,
                        PrimDocumentDate = item.PrimDocumentDate,
                        ThirdPartyId = item.ThirdPartyId,
                        DepreciationStartDate = item.DepreciationStartDate,
                        UnreceiveInvoice = oper.UnreceiveInvoice,
                        EndDateChelt = item.EndDateChelt,
                        PrepaymentAccountVATId = item.PrepaymentAccountVATId,
                        DepreciationAccountVATId = item.DepreciationAccountVATId,
                        PrepaymentVAT = item.VAT
                    };

                    _prepaymentRepository.Insert(prepayment);
                    CurrentUnitOfWork.SaveChanges();

                    var lastBalanceDate = _balanceRepository.GetAll().Where(f => f.Status == State.Active).OrderByDescending(f => f.BalanceDate).FirstOrDefault().BalanceDate;

                    //calculez gestiunea
                    _prepaymentRepository.GestComputingForPrepayment(prepayment.PaymentDate, prepayment.PrepaymentType, lastBalanceDate, prepayment.Id);
                    _autoOperationRepository.AutoPrepaymentsOperationAdd(prepayment.PaymentDate, invoice.CurrencyId, prepayment.PrepaymentType, lastBalanceDate, null);
                }

                oper.FinishAdd = true;

                return oper;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public void DeletePrepayment(int prepaymentId)
        {
            var asset = _prepaymentRepository.FirstOrDefault(f => f.Id == prepaymentId);

            if (!_operationRepository.VerifyClosedMonth(asset.PaymentDate))
                throw new UserFriendlyException("Eroare", "Nu se poate inregistra operatia deoarece luna contabila este inchisa");

            asset.State = State.Inactive;
        }

        public PrepaymentsAddDirectDto AddDirectInit(int prepaymentId, int prepaymentType)
        {
            PrepaymentsAddDirectDto ret;
            if (prepaymentId == 0)
            {
                ret = new PrepaymentsAddDirectDto
                {
                    PaymentDate = LazyMethods.Now(),
                    DepreciationStartDate = LazyMethods.FirstDayNextMonth(LazyMethods.Now()),
                    PrimDocumentDate = LazyMethods.Now(),
                    Processed = false,
                    ThirdPartyName = "",
                    PrepaymentType = (PrepaymentType)prepaymentType
                };
                var documentType = _prepaymentOperDocTypeRepository.GetAllIncluding(f => f.DocumentType)
                                                                 .Where(f => f.OperType == PrepaymentOperType.Constituire)
                                                                 .FirstOrDefault();
                ret.PrimDocumentTypeId = documentType.DocumentType.Id;
                //var documentNr = _appClientRepository.NextDocumentNumber(selectedAppClientId, ret.DocumentTypeId);
                //ret.DocumentNr = documentNr;                
                ret.PrimDocumentType = documentType.DocumentType.TypeName;
                ret.FinishAdd = false;
            }
            else
            {
                var item = _prepaymentRepository.GetAllIncluding(f => f.ThirdParty, f => f.PrimDocumentType)
                                               .FirstOrDefault(f => f.Id == prepaymentId);
                ret = new PrepaymentsAddDirectDto
                {
                    Id = item.Id,
                    Name = item.Description,
                    Processed = item.Processed,
                    Depreciation = item.Depreciation,
                    PrepaymentAccountId = item.PrepaymentAccountId,
                    DepreciationAccountId = item.DepreciationAccountId,
                    DepreciationStartDate = item.DepreciationStartDate,
                    EndDate = item.EndDate,
                    EndDateChelt = item.EndDateChelt,
                    DurationInMonths = item.DurationInMonths,
                    InvoiceDetailsId = item.InvoiceDetailsId,
                    FinishAdd = false,
                    ThirdPartyId = item.ThirdPartyId,
                    ThirdPartyName = (item.ThirdParty != null) ? item.ThirdParty.FullName : "",
                    PrimDocumentNr = item.PrimDocumentNr,
                    PrimDocumentDate = item.PrimDocumentDate,
                    PrepaymentType = (PrepaymentType)prepaymentType,
                    PrepaymentValue = item.PrepaymentValue,
                    PaymentDate = item.PaymentDate,
                    RemainingDuration = item.RemainingDuration,
                    MontlyDepreciation = item.MontlyDepreciation,
                    PrimDocumentType = (item.PrimDocumentType != null) ? item.PrimDocumentType.TypeName : "",
                    PrimDocumentTypeId = item.PrimDocumentTypeId,
                    UnreceiveInvoice = item.UnreceiveInvoice,
                    PrepaymentVAT = item.PrepaymentVAT,
                    PrepaymentAccountVATId = item.PrepaymentAccountVATId,
                    DepreciationAccountVATId = item.DepreciationAccountVATId,
                    DepreciationVAT = item.DepreciationVAT,
                    MontlyDepreciationVAT = item.MontlyDepreciationVAT
                };
            }

            return ret;
        }

        public PrepaymentsAddDirectDto SavePrepaymentDirect(PrepaymentsAddDirectDto oper)
        {
            if (!_operationRepository.VerifyClosedMonth(oper.PaymentDate))
                throw new UserFriendlyException("Eroare", "Nu se poate inregistra operatia deoarece luna contabila este inchisa");

            try
            {
                var appClient = GetCurrentTenant();
                //verificare completare date

                if ((oper.DepreciationAccountId == null) || (oper.DurationInMonths == 0))
                {
                    throw new Exception("Trebuie sa selectati conturile corespunzatoare si sa completati durata de amortizare");
                }

                /*DateTime lastProcessedDate = _prepaymentRepository.LastProcessedDate(selectedAppClientId);
                if (lastProcessedDate >= oper.UseStartDate)
                    throw new UserFriendlyException("Eroare data operatie", "Data operatiei nu poate fi mai mica decat data ultimei operatii procesate in gestiune " + lastProcessedDate.ToShortDateString());*/

                var item = new Prepayment
                {

                    Id = oper.Id,
                    Description = oper.Name,
                    PrepaymentAccountId = oper.PrepaymentAccountId,
                    DepreciationAccountId = oper.DepreciationAccountId,
                    Depreciation = oper.Depreciation,
                    Processed = oper.Processed,
                    DepreciationStartDate = oper.DepreciationStartDate,
                    EndDate = oper.EndDate,
                    EndDateChelt = oper.EndDateChelt,
                    DurationInMonths = oper.DurationInMonths,
                    PaymentDate = new DateTime(oper.PaymentDate.Year, oper.PaymentDate.Month, oper.PaymentDate.Day),
                    State = State.Active,
                    ThirdPartyId = oper.ThirdPartyId,
                    PrimDocumentTypeId = oper.PrimDocumentTypeId,
                    PrimDocumentNr = oper.PrimDocumentNr,
                    PrimDocumentDate = oper.PrimDocumentDate,
                    PrepaymentValue = oper.PrepaymentValue,
                    RemainingDuration = oper.RemainingDuration,
                    MontlyDepreciation = oper.MontlyDepreciation,
                    UnreceiveInvoice = oper.UnreceiveInvoice,
                    PrepaymentVAT = oper.PrepaymentVAT,
                    PrepaymentAccountVATId = oper.PrepaymentAccountVATId,
                    DepreciationAccountVATId = oper.DepreciationAccountVATId,
                    DepreciationVAT = oper.DepreciationVAT,
                    MontlyDepreciationVAT = oper.MontlyDepreciationVAT,
                    PrepaymentType = oper.PrepaymentType,
                    InvoiceDetailsId = oper.InvoiceDetailsId
                };

                try
                {
                    if (item.Id == 0)
                    {
                        _prepaymentRepository.Insert(item);
                    }
                    else
                    {
                        item.TenantId = appClient.Id;
                        _prepaymentRepository.Update(item);
                    }
                    CurrentUnitOfWork.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new UserFriendlyException("Eroare adaugare", ex.Message);
                }

                // sterg notele contabile pentru cheltuila in avans
                try
                {
                    var prepaymentBalance = _prepaymentBalanceRepository.GetAll().Where(f => f.PrepaymentPFId == item.Id).FirstOrDefault();
                    if (prepaymentBalance != null)
                    {
                        _autoOperationRepository.DeleteUncheckedAutoOperationForPrepayment(item.PaymentDate, item.PrepaymentType, item.Id);
                    }
                }
                catch (Exception ex)
                {

                    throw new UserFriendlyException("Eroare", ex.Message);
                }

                // sterg gestiunea pentru cheltuiala in avans
                try
                {
                    var lastProcessedDateForPrepayment = _prepaymentRepository.LastProcessedDateForPrepayment(item.PrepaymentType, item.Id);

                    if (item.PaymentDate <= lastProcessedDateForPrepayment)
                    {
                        _prepaymentRepository.GestDelComputingForPrepayment(item.PaymentDate, item.PrepaymentType, item.Id);
                    }
                }
                catch (Exception ex)
                {
                    throw new UserFriendlyException("Eroare", ex.Message);
                }

                var lastBalanceDate = _balanceRepository.GetAll().Where(f => f.Status == State.Active).OrderByDescending(f => f.BalanceDate).FirstOrDefault().BalanceDate;
                //calculez gestiunea
                _prepaymentRepository.GestComputingForPrepayment(item.PaymentDate, item.PrepaymentType, lastBalanceDate, item.Id);

                // generez notele contabile
                _autoOperationRepository.AutoPrepaymentsOperationAdd(item.PaymentDate, appClient.LocalCurrencyId.Value, item.PrepaymentType, lastBalanceDate, null);


                oper.FinishAdd = true;

                return oper;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare adaugare", ex.Message);
            }
        }

        public int CalcDurata(DateTime startDate, DateTime endDate, int modCalcul)
        {
            int ret = 0;
            var _modCalcul = (PrepaymentDurationCalc)modCalcul;
            if (_modCalcul == PrepaymentDurationCalc.Zilnic)
            {
                ret = (endDate - startDate).Days;
            }
            else
            {
                ret = (endDate.Year - startDate.Year) * 12 + endDate.Month - startDate.Month;
            }

            return ret;
        }

        public PrepaymentsExitDto PrepaymentsExitInit(int prepaymentId, int prepaymentType)
        {
            PrepaymentsExitDto ret;

            var item = _prepaymentRepository.GetAllIncluding(f => f.ThirdParty, f => f.PrimDocumentType)
                                           .FirstOrDefault(f => f.Id == prepaymentId);
            ret = new PrepaymentsExitDto
            {
                Id = item.Id,
                Name = item.Description,
                ProcessedOut = item.Processed,
                FinishAdd = false,
                ThirdPartyName = (item.ThirdParty != null) ? item.ThirdParty.FullName : "",
                PrimDocumentNr = item.PrimDocumentNr,
                PrimDocumentDate = item.PrimDocumentDate,
                PrepaymentType = (PrepaymentType)prepaymentType,
                PrepaymentValue = item.PrepaymentValue,
                PaymentDate = item.PaymentDate,
                PrimDocumentType = (item.PrimDocumentType != null) ? item.PrimDocumentType.TypeNameShort : "",
                EndDate = item.EndDate
            };

            return ret;
        }

        public PrepaymentsExitDto SavePrepaymentExit(PrepaymentsExitDto oper)
        {
            var lastProcessedDate = _prepaymentRepository.LastProcessedDate(oper.PrepaymentType);
            if (oper.EndDate != null && oper.EndDate <= lastProcessedDate)
            {
                throw new UserFriendlyException("Eroare", "Data de iesire din gestiune trebuie sa fie mai mare decat ultima data procesata in gestiune: " + LazyMethods.DateToString(lastProcessedDate));
            }
            if (oper.EndDate != null && oper.EndDate <= oper.PaymentDate)
            {
                throw new UserFriendlyException("Eroare", "Data de iesire din gestiune trebuie sa fie mai mare decat data constituirii: " + LazyMethods.DateToString(oper.PaymentDate));
            }
            try
            {
                var item = _prepaymentRepository.GetAllIncluding(f => f.ThirdParty, f => f.PrimDocumentType)
                                                .FirstOrDefault(f => f.Id == oper.Id);
                item.EndDate = oper.EndDate;
                item.ProcessedOut = oper.ProcessedOut;
                _prepaymentRepository.Update(item);
                CurrentUnitOfWork.SaveChanges();

                return oper;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        //[AbpAuthorize("Administrare.CheltAvans.Setup.Acces")]
        //[AbpAuthorize("Administrare.VenitAvans.Setup.Acces")]
        public PrepaymentsDurationSetupDto DurationSetupDetails(int prepaymentType)
        {
            try
            {
                PrepaymentsDurationSetupDto ret;
                var setup = _prepaymentsDurationSetupRepository.GetAll().Where(f => f.PrepaymentType == (PrepaymentType)prepaymentType).FirstOrDefault();
                if (setup == null)
                {
                    ret = new PrepaymentsDurationSetupDto
                    {
                        PrepaymentDurationCalcId = 0,
                        PrepaymentTypeId = prepaymentType
                    };
                }
                else
                {
                    ret = ObjectMapper.Map<PrepaymentsDurationSetupDto>(setup);
                }
                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", LazyMethods.GetErrMessage(ex));
            }
        }

        //[AbpAuthorize("Administrare.CheltAvans.Setup.Acces")]
        //[AbpAuthorize("Administrare.VenitAvans.Setup.Acces")]
        public PrepaymentsDurationSetupDto DurationSetupSave(PrepaymentsDurationSetupDto setup)
        {
            try
            {
                var _setup = ObjectMapper.Map<PrepaymentsDurationSetup>(setup);
                if (_setup.Id == 0)
                {
                    _prepaymentsDurationSetupRepository.Insert(_setup);
                }
                else
                {
                    _prepaymentsDurationSetupRepository.Update(_setup);
                }
                CurrentUnitOfWork.SaveChanges();

                setup = DurationSetupDetails(setup.PrepaymentTypeId);
                return setup;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", LazyMethods.GetErrMessage(ex));
            }
        }

        //[AbpAuthorize("Administrare.CheltAvans.Setup.Acces")]
        //[AbpAuthorize("Administrare.VenitAvans.Setup.Acces")]
        public PrepaymentsDecDeprecSetupDto DecDeprecSetupDetails(int prepaymentType)
        {
            try
            {
                PrepaymentsDecDeprecSetupDto ret;
                var setup = _prepaymentsDecDeprecSetupRepository.GetAll().Where(f => f.PrepaymentType == (PrepaymentType)prepaymentType).FirstOrDefault();
                if (setup == null)
                {
                    ret = new PrepaymentsDecDeprecSetupDto
                    {
                        DecimalAmort = 2,
                        PrepaymentTypeId = prepaymentType
                    };
                }
                else
                {
                    ret = ObjectMapper.Map<PrepaymentsDecDeprecSetupDto>(setup);
                }
                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", LazyMethods.GetErrMessage(ex));
            }
        }

        //[AbpAuthorize("Administrare.CheltAvans.Setup.Acces")]
        //[AbpAuthorize("Administrare.VenitAvans.Setup.Acces")]
        public PrepaymentsDecDeprecSetupDto DecDeprecSetupSave(PrepaymentsDecDeprecSetupDto setup)
        {
            try
            {
                var _setup = ObjectMapper.Map<PrepaymentsDecDeprecSetup>(setup);
                if (_setup.Id == 0)
                {
                    _prepaymentsDecDeprecSetupRepository.Insert(_setup);
                }
                else
                {
                    _prepaymentsDecDeprecSetupRepository.Update(_setup);
                }
                CurrentUnitOfWork.SaveChanges();

                setup = DecDeprecSetupDetails(setup.PrepaymentTypeId);
                return setup;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", LazyMethods.GetErrMessage(ex));
            }
        }

        public PrepaymentsAddDirectDto AddDirectChangeDate(PrepaymentsAddDirectDto oper)
        {
            oper.DepreciationStartDate = LazyMethods.FirstDayNextMonth(oper.PaymentDate);
            oper.EndDateChelt = oper.EndDateChelt != null ? LazyMethods.FirstDayNextMonth(oper.PaymentDate) : oper.EndDateChelt;
            return oper;
        }

        public List<PrepaymentsListDto> GetPrepaymentsForInvoiceDetails(int invoiceId)
        {
            var prepayments = new List<Prepayment>();
            var invoice = _invoiceRepository.GetById(invoiceId);

            foreach (var item in invoice.InvoiceDetails)
            {
                var prepayment = _prepaymentRepository.GetAllIncluding(f => f.InvoiceDetails).Where(f => f.InvoiceDetailsId == item.Id && f.State == State.Active).FirstOrDefault();
                prepayments.Add(prepayment);
            }
            var ret = ObjectMapper.Map<List<PrepaymentsListDto>>(prepayments);
            return ret;
        }
    }
}
