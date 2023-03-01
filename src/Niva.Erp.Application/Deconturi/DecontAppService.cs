using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.Deconturi.Dto;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.EntityFrameworkCore.Repositories.Conta;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Contracts;
using Niva.Erp.Models.Deconturi;
using Niva.Erp.Models.PrePayments;
using Niva.Erp.Repositories.Conta;
using Niva.Erp.Repositories.Conta.AutoOperation;
using Niva.Erp.Repositories.Conta.Nomenclatures;
using Niva.Erp.Repositories.Conta.Prepayments;
using Niva.Erp.Repositories.Doconturi;
using Niva.Erp.Repositories.Economic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Deconturi
{
    public interface IDecontAppService : IApplicationService
    {
        DecontInitForm InitForm();
        List<DecontListDto> SearchDecont(DecontInitForm decontForm);
        DecontEditDto GetDecont(int decontId);
        DecontEditDto SaveDecont(DecontEditDto decontDto);
        void DeleteDecont(int decontId);
        void SaveDecontToConta(DecontEditDto decont);
        int GetNextDecontNumber(string documentTypeNameShort);
        DecontEditDto ComputeDiurnaAcordataForDecont(int diurnaLegalaId, DateTime dataStart, DateTime dataEnd);
    }

    public class DecontAppService : ErpAppServiceBase, IDecontAppService
    {
        private const int MIN_PER_DAY = 1440;
        private const int MIN_PER_HOUR = 60;
        private const int H_PER_HALF_DAY = 12;
        private const int H_PER_DAY = 24;


        IDecontRepository _decontRepository;
        IInvoiceRepository _invoiceRepository;
        IAutoOperationRepository _autoOperationRepository;
        IDiurnaLegalaRepository _diurnaLegalaRepository;
        IDiurnaZiRepository _diurnaZiRepository;
        IRepository<DocumentType> _documentTypeRepository;
        IExchangeRatesRepository _exchangeRatesRepository;
        IPrepaymentRepository _prepaymentRepository;
        IRepository<PrepaymentDocType> _prepaymentOperDocTypeRepository;
        IBalanceRepository _balanceRepository;
        IRepository<PrepaymentBalance> _prepaymentBalanceRepository;
        OperationRepository _operationRepository;

        public DecontAppService(IDecontRepository decontRepository, IInvoiceRepository invoiceRepository, IAutoOperationRepository autoOperationRepository,
                                IDiurnaLegalaRepository diurnaLegalaRepository, IDiurnaZiRepository diurnaZiRepository, IRepository<DocumentType> documentTypeRepository,
                                IExchangeRatesRepository exchangeRatesRepository, IPrepaymentRepository prepaymentRepository, IRepository<PrepaymentDocType> prepaymentOperDocTypeRepository,
                                IBalanceRepository balanceRepository, IRepository<PrepaymentBalance> prepaymentBalanceRepository, OperationRepository operationRepository)
        {
            _decontRepository = decontRepository;
            _invoiceRepository = invoiceRepository;
            _autoOperationRepository = autoOperationRepository;
            _diurnaLegalaRepository = diurnaLegalaRepository;
            _diurnaZiRepository = diurnaZiRepository;
            _documentTypeRepository = documentTypeRepository;
            _exchangeRatesRepository = exchangeRatesRepository;
            _prepaymentRepository = prepaymentRepository;
            _prepaymentOperDocTypeRepository = prepaymentOperDocTypeRepository;
            _balanceRepository = balanceRepository;
            _prepaymentBalanceRepository = prepaymentBalanceRepository;
            _operationRepository = operationRepository;
        }
        //[AbpAuthorize("Conta.Deconturi.Acces")]
        public DecontInitForm InitForm()
        {
            var decont = new DecontInitForm
            {
                DecontStartDate = _balanceRepository.BalanceDateNextDay(),
                DecontEndDate = LazyMethods.Now(),
                DocumentType = "-"
            };
            return decont;
        }

        //[AbpAuthorize("Conta.Deconturi.Acces")]
        public List<DecontListDto> SearchDecont(DecontInitForm decontForm)
        {
            var list = _decontRepository.GetAllIncluding(f => f.ThirdParty, f => f.DiurnaLegala, f => f.DocumentType).Where(f => f.State == State.Active && f.DecontDate >= decontForm.DecontStartDate && f.DecontDate <= decontForm.DecontEndDate).ToList();

            if (decontForm.ThirdPartyId != null)
            {
                list = list.Where(f => f.ThirdPartyId == decontForm.ThirdPartyId).ToList();
            }

            if (decontForm.DecontStartDate != null && decontForm.DelegatieEndDate != null)
            {
                list = list.Where(f => f.DecontDate >= decontForm.DecontStartDate && f.DecontDate <= decontForm.DecontEndDate).ToList();
            }

            if (decontForm.DelegatieStartDate != null && decontForm.DelegatieEndDate != null)
            {
                list = list.Where(f => !(f.DateEnd < decontForm.DelegatieStartDate) && !(f.DateStart > decontForm.DelegatieEndDate)).ToList();
            }

            if (decontForm.DiurnaLegalaId != null)
            {
                list = list.Where(f => f.DiurnaLegalaId == decontForm.DiurnaLegalaId).ToList();
            }

            if (decontForm.DecontTypeId != null)
            {
                list = list.Where(f => f.DecontType == (DecontType)decontForm.DecontTypeId).ToList();
            }
            if (decontForm.DocumentType != "-")
            {
                list = list.Where(f => f.DocumentType.TypeNameShort == decontForm.DocumentType).ToList();
            }

            if (decontForm.ScopDeplasareTypeId != null)
            {
                list = list.Where(f => f.ScopDeplasareType == (ScopDeplasareType)decontForm.ScopDeplasareTypeId).ToList();
            }

            list = list.OrderByDescending(f => f.DecontDate).ThenByDescending(f => f.DecontNumber).ToList();

            var ret = ObjectMapper.Map<List<DecontListDto>>(list);
            // decontForm.DecontList = ret;

            return ret;
        }

        //[AbpAuthorize("Conta.Deconturi.Acces")]
        public DecontEditDto GetDecont(int decontId)
        {

            if (decontId == 0)
            {
                var appClient = GetCurrentTenant();
                var diurna = _diurnaLegalaRepository.GetAllIncluding(f => f.Country).FirstOrDefault(f => f.Country.CountryAbrv == "ROM");
                var documentType = _documentTypeRepository.GetAll().Where(f => f.TypeNameShort == "DEC").FirstOrDefault();

                var ret = new DecontEditDto
                {
                    DecontDate = LazyMethods.Now(),
                    DocumentType = documentType.TypeNameShort,
                    DecontNumber = GetNextDecontNumber(documentType.TypeNameShort),
                    DecontTypeId = null,
                    CurrencyId = appClient.LocalCurrencyId.Value,
                    DiurnaLegalaId = diurna.Id,
                    DiurnaLegala = (int)(diurna.Value * 2.5),
                    DiurnaZi = _diurnaZiRepository.GetDiurnaZiValue(diurna.CountryId),
                    ScopDeplasareTypeId = (int)ScopDeplasareType.Deplasare,
                };

                return ret;
            }
            else
            {
                var decont = _decontRepository.GetAllIncluding(f => f.ThirdParty, f => f.DiurnaLegala, f => f.DocumentType).FirstOrDefault(f => f.Id == decontId);
                var ret = ObjectMapper.Map<DecontEditDto>(decont);
                return ret;
            }

        }

        //[AbpAuthorize("Conta.Deconturi.Acces")]
        public int GetNextDecontNumber(string documentTypeNameShort)
        {
            var nextNumber = 0;
            var appClient = GetCurrentTenant();
            var documentType = _documentTypeRepository.GetAll().Where(f => f.TypeNameShort == documentTypeNameShort && f.TenantId == appClient.Id).FirstOrDefault();
            var decont = _decontRepository.GetAllIncluding(f => f.DocumentType).Where(f => f.State == State.Active && f.TenantId == appClient.Id && f.DocumentTypeId == documentType.Id).OrderByDescending(f => f.DecontNumber).FirstOrDefault();

            if (decont != null)
            {
                nextNumber = decont.DecontNumber + 1;
            }
            else
            {
                nextNumber++;
            }

            return nextNumber;
        }

        [AbpAuthorize("Conta.Deconturi.Modificare")]
        public DecontEditDto SaveDecont(DecontEditDto decontDto)
        {
            if (!_operationRepository.VerifyClosedMonth(decontDto.DecontDate))
                throw new UserFriendlyException("Eroare", "Nu se poate inregistra operatia deoarece luna contabila este inchisa");

            if ((decontDto.DecontDate == null) || (decontDto.ThirdPartyId == 0) || (decontDto.DecontTypeId == null))
            {
                throw new UserFriendlyException("Eroare", "Trebuie sa completati data, tertul si tipul decontului");
            }

            var documentTypeId = _documentTypeRepository.GetAll().Where(f => f.TypeNameShort == decontDto.DocumentType).FirstOrDefault().Id;

            var _decont = new Decont
            {
                Id = decontDto.Id,
                DecontDate = decontDto.DecontDate,
                DecontNumber = decontDto.DecontNumber,
                DocumentTypeId = documentTypeId,
                ThirdPartyId = decontDto.ThirdPartyId,
                DateStart = decontDto.DateStart,
                DateEnd = decontDto.DateEnd,
                DiurnaZi = decontDto.DiurnaZi,
                DiurnaLegalaId = decontDto.DiurnaLegalaId,
                TotalDiurnaAcordata = decontDto.TotalDiurnaAcordata,
                TotalDiurnaImpozabila = decontDto.TotalDiurnaImpozabila,
                DiurnaImpozabila = decontDto.DiurnaImpozabila,
                DiurnaLegalaValue = decontDto.DiurnaLegala,
                NrZile = decontDto.NrZile,
                DecontType = (DecontType)decontDto.DecontTypeId,
                CurrencyId = decontDto.CurrencyId,
                State = State.Active,
                ScopDeplasareType = (ScopDeplasareType)decontDto.ScopDeplasareTypeId,
                OperationId = decontDto.OperationId
            };

            var appClient = GetCurrentTenant();

            try
            {
                if (_decont.Id == 0)
                {
                    _decontRepository.Insert(_decont);
                }
                else
                {
                    _decont.TenantId = appClient.Id;
                    _decontRepository.Update(_decont);
                }
            }
            catch
            {

                throw new UserFriendlyException("Eroare la adaugarea decontului");
            }

            CurrentUnitOfWork.SaveChanges();
            var ret = ObjectMapper.Map<DecontEditDto>(_decont);
            return ret;
        }

        [AbpAuthorize("Conta.Deconturi.Modificare")]
        public void DeleteDecont(int decontId)
        {
            //var operCount = _invoiceRepository.GetAllIncluding(f => f.InvoiceDetails).Where(f => f.State == State.Active && f.DecontId == decontId && f.InvoiceDetails.Any(g => g.ContaOperationDetailId != null)).Count();
            //if (operCount != 0)
            //{
            //    throw new UserFriendlyException("Eroare", "Nu puteti sa stergeti aceasta inregistrare, deoarece exista operatii definite pentru acest decont");
            //}

            var appClientId = GetCurrentTenant();

            try
            {
                var decont = _decontRepository.GetAllIncluding(f => f.InvoiceDetails).Where(f => f.Id == decontId && f.State == State.Active).FirstOrDefault();

                if (!_operationRepository.VerifyClosedMonth(decont.DecontDate))
                    throw new Exception("Nu se poate inregistra operatia deoarece luna contabila este inchisa");


                RemovePrepaymentsFromDecont(decont.Id);

                _autoOperationRepository.DeleteInvoicesFromDecont(decont.Id, appClientId.Id);

                if (decont.OperationId != null)
                {
                    var oper = _operationRepository.GetAll().FirstOrDefault(f => f.Id == decont.OperationId);
                    oper.State = State.Inactive;
                    decont.OperationId = null;
                }

                decont.State = State.Inactive;

                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.Message);
            }

        }

        [AbpAuthorize("Conta.Deconturi.Modificare")]
        public void SaveDecontToConta(DecontEditDto decont)
        {
            int count = 0;
            var localCurrencyId = GetCurrentTenant().LocalCurrencyId.Value;
            var appClientId = GetCurrentTenant();

            if (!_operationRepository.VerifyClosedMonth(decont.DecontDate))
                throw new UserFriendlyException("Eroare", "Nu se poate inregistra operatia deoarece luna contabila este inchisa");


            decont = SaveDecont(decont);

            try
            {
                RemovePrepaymentsFromDecont(decont.Id);

                _autoOperationRepository.RemoveContaInvoicesFromDecont(decont.Id, appClientId.Id);
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare", "Documentele nu au fost sterse");
            }

            count = _invoiceRepository.GetAll().Where(f => f.State == State.Active && f.DecontId == decont.Id).Count();

            if (count == 0 && decont.ScopDeplasareTypeId == (int)ScopDeplasareType.AlteDeconturi)
                throw new UserFriendlyException("Eroare", "Trebuie sa introduceti documente pentru a putea fi inregistrate in contabilitate");

            //if (count >0)
            //{
                try
                {
                    //var invoicesList = _invoiceRepository.GetAllIncludeElemDet().Where(f => f.DecontId == decont.Id && f.State == State.Active).ToList();

                    //foreach (var invoice in invoicesList)
                    //{
                    //    foreach (var item in invoice.InvoiceDetails)
                    //    {
                    // generez nota contabila direct din decont
                    _autoOperationRepository.SaveDecontDirectToConta(decont.Id, decont.DecontDate, localCurrencyId, decont.DecontType, decont.ThirdPartyId, decont.DocumentType);
                    SavePrepaymentsFromDecont(decont.Id);
                    //    }
                    //}
                }
                catch (Exception ex)
                {

                    throw new UserFriendlyException("Eroare", ex.Message);
                }
            //}
        }

        [AbpAuthorize("Conta.Deconturi.Modificare")]
        private void SavePrepaymentsFromDecont(int decontId)
        {
            try
            {
                int localCurrecyId = GetCurrentTenant().LocalCurrencyId.Value;
                var invoicesList = _invoiceRepository.GetAllIncludeElemDet().Where(f => f.DecontId == decontId && f.State == State.Active).ToList();
                foreach (var invoice in invoicesList)
                {
                    foreach (var item in invoice.InvoiceDetails.Where(f => f.DurationInMonths != 0))
                    {
                        var lastBalanceDate = _balanceRepository.GetAll().Where(f => f.Status == State.Active)
                                     .OrderByDescending(f => f.BalanceDate)
                                     .FirstOrDefault().BalanceDate;

                        var documentType = _prepaymentOperDocTypeRepository.GetAllIncluding(f => f.DocumentType)
                                                 .Where(f => f.OperType == PrepaymentOperType.Constituire)
                                                 .FirstOrDefault();

                        var elementDetail = item.InvoiceElementsDetails;
                        int prepaymentAccountId = 0;



                        if (elementDetail.CorrespondentAccount.StartsWith('6') && invoice.ActivityTypeId != null)
                        {
                            prepaymentAccountId = _autoOperationRepository.GetAutoAccountActivityType(elementDetail.CorrespondentAccount, invoice.ThirdPartyId, invoice.ActivityTypeId.Value, invoice.OperationDate, localCurrecyId, null);
                        }
                        else
                        {
                            prepaymentAccountId = _autoOperationRepository.GetAutoAccount(elementDetail.CorrespondentAccount, invoice.ThirdPartyId, invoice.OperationDate, localCurrecyId, null);

                        }
                        var deprecAccountId = _autoOperationRepository.GetAutoAccount(elementDetail.ExpenseAmortizAccount, invoice.ThirdPartyId, invoice.OperationDate, localCurrecyId, null);

                        var exchangeRate = _exchangeRatesRepository.GetExchangeRateForOper(invoice.InvoiceDate, invoice.CurrencyId, localCurrecyId);

                        var prepayment = new Prepayment
                        {
                            Description = item.Element,
                            DurationInMonths = item.DurationInMonths ?? 0,
                            InvoiceDetailsId = item.Id,
                            InvoiceDetails = item,
                            PaymentDate = invoice.OperationDate, //LazyMethods.Now(),
                            PrepaymentAccountId = prepaymentAccountId,
                            DepreciationAccountId = deprecAccountId,
                            PrepaymentType = PrepaymentType.CheltuieliInAvans,
                            PrepaymentValue = (item.Value + item.VAT) * exchangeRate,
                            ThirdPartyId = invoice.ThirdPartyId,
                            PrimDocumentNr = invoice.InvoiceNumber,
                            PrimDocumentDate = invoice.InvoiceDate,
                            DepreciationStartDate = item.DataStartAmortizare.Value, //LazyMethods.FirstDayNextMonth(invoice.OperationDate),
                            PrimDocumentTypeId = documentType.DocumentType.Id,
                            TenantId = GetCurrentTenant().Id
                        };

                        _prepaymentRepository.InsertOrUpdateV(prepayment);
                        CurrentUnitOfWork.SaveChanges();

                        //calculez gestiunea
                        _prepaymentRepository.GestComputingForPrepayment(invoice.OperationDate, prepayment.PrepaymentType, lastBalanceDate, prepayment.Id);
                        _autoOperationRepository.AutoPrepaymentsOperationAdd(invoice.OperationDate, localCurrecyId, prepayment.PrepaymentType, lastBalanceDate, null);
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [AbpAuthorize("Conta.Deconturi.Modificare")]
        private void RemovePrepaymentsFromDecont(int decontId)
        {
            var invoiceDecontOldList = _invoiceRepository.GetAllIncludeElemDet().Where(f => f.State == State.Active && f.DecontId == decontId).ToList();
            foreach (var invoice in invoiceDecontOldList)
            {
                foreach (var item in invoice.InvoiceDetails.Where(f => f.InvoiceElementsDetails.InvoiceElementsType == InvoiceElementsType.CheltuieliInAvans))
                {
                    var prepayment = _prepaymentRepository.GetAll().FirstOrDefault(f => f.InvoiceDetailsId == item.Id && f.State == State.Active);

                    if (prepayment != null)
                    {
                        var prepaymentBalance = _prepaymentBalanceRepository.GetAll().Where(f => f.PrepaymentPFId == prepayment.Id).FirstOrDefault();
                        if (prepaymentBalance != null)
                        {
                            _autoOperationRepository.DeleteUncheckedAutoOperation(prepayment.PaymentDate, prepayment.PrepaymentType);
                        }

                        var lastProcessedDateForPrepayment = _prepaymentRepository.LastProcessedDateForPrepayment(prepayment.PrepaymentType, prepayment.Id);

                        if (invoice.OperationDate <= lastProcessedDateForPrepayment)
                        {
                            _prepaymentRepository.GestDelComputingForPrepayment(invoice.OperationDate, prepayment.PrepaymentType, prepayment.Id);
                        }

                        _prepaymentRepository.Delete(prepayment.Id);
                    }
                }
            }
        }

        public DecontEditDto ComputeDiurnaAcordataForDecont(int diurnaLegalaId, DateTime dataStart, DateTime dataEnd)
        {
            try
            {
                var decontEdit = new DecontEditDto();
                var diurnaLegala = _diurnaLegalaRepository.Get(diurnaLegalaId);
                var diurnaLegalaValue = _diurnaLegalaRepository.GetDiurnalLegalaValue(diurnaLegalaId);
                var diurnaZiValue = _diurnaZiRepository.GetDiurnaZiValue(diurnaLegala.CountryId);
                var isDiurnaExterna = diurnaLegala.DiurnaType == DiurnaType.Externa ? true : false;


                decontEdit.NrZile = ReturnDaysNumber(dataStart, dataEnd, isDiurnaExterna);
                decontEdit.DateStart = dataStart;
                decontEdit.DateEnd = dataEnd;
                decontEdit.DiurnaLegala = diurnaLegalaValue;
                decontEdit.DiurnaZi = diurnaZiValue;

                ComputeDiurna(diurnaLegalaValue, diurnaZiValue, decontEdit);

                return decontEdit;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        private void ComputeDiurna(int diurnaLegalaValue, int diurnaZiValue, DecontEditDto decontEdit)
        {
            try
            {
                int diurnaImpozabila = diurnaZiValue - diurnaLegalaValue;
                if (diurnaImpozabila < 0)
                {
                    decontEdit.DiurnaImpozabila = 0;
                }
                else
                {
                    decontEdit.DiurnaImpozabila = diurnaImpozabila;
                }

                decontEdit.TotalDiurnaAcordata = (int)decontEdit.NrZile.Value * decontEdit.DiurnaZi.Value;
                decontEdit.TotalDiurnaImpozabila = (int)decontEdit.NrZile.Value * decontEdit.DiurnaImpozabila.Value;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private decimal ReturnDaysNumber(DateTime dataStart, DateTime dataEnd, bool isDiurnaExterna)
        {
            var durationInMinutes = (dataEnd - dataStart).TotalMinutes;
            var days = Math.Truncate(durationInMinutes / MIN_PER_DAY);
            durationInMinutes -= days * MIN_PER_DAY;

            var hours = Math.Truncate(durationInMinutes / MIN_PER_HOUR);

            if (hours != 0)
            {
                if (hours > 12 && isDiurnaExterna == false)
                {
                    days++;
                }
                else if (isDiurnaExterna == true && hours < 12)
                {
                    days += H_PER_HALF_DAY / H_PER_DAY;
                }
                else if (isDiurnaExterna == true && hours > 12)
                {
                    days++;
                }
            }

            return (decimal)days;
        }
    }
}
