using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.Economic.Dto;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.EntityFrameworkCore.Repositories.Conta;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Contracts;
using Niva.Erp.Models.Economic.Casierii;
using Niva.Erp.Repositories.Conta.AutoOperation;
using Niva.Erp.Repositories.Economic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Economic
{
    public interface IDispositionAppService : IApplicationService
    {
        // dispositii catre casierie
        List<DispositionListDto> DispositionList(DateTime dataStart, DateTime dataEnd, int? dispositionState);
        DispositionEditDto GetDisposition(int dispositionId);
        void SaveDisposition(DispositionEditDto disposition);
        void DeleteDisposition(int dispositionId);
        int GetNextNumber(DateTime date);
        int? GetNextNumberForChitanta(DateTime currentDate);
    }

    public class DispositionAppService : ErpAppServiceBase, IDispositionAppService
    {
        IDispositionRepository _dispositionRepository;
        IAutoOperationRepository _autoOperationRepository;
        OperationRepository _operationRepository;
        IInvoiceRepository _invoiceRepository;
        IRepository<Currency> _currencyRepository;
        IRepository<DispositionInvoice> _dispositionInvoiceRepository;
        IRepository<DocumentType> _documentTypeRepository;

        public DispositionAppService(IDispositionRepository dispositionRepository, IAutoOperationRepository autoOperationRepository, OperationRepository operationRepository, IInvoiceRepository invoiceRepository,
            IRepository<Currency> currencyRepository, IRepository<DispositionInvoice> dispositionInvoiceRepository, IRepository<DocumentType> documentTypeRepository)
        {
            _dispositionRepository = dispositionRepository;
            _autoOperationRepository = autoOperationRepository;
            _operationRepository = operationRepository;
            _invoiceRepository = invoiceRepository;
            _currencyRepository = currencyRepository;
            _dispositionInvoiceRepository = dispositionInvoiceRepository;
            _documentTypeRepository = documentTypeRepository;
        }

        [AbpAuthorize("Casierie.Numerar.Dispozitii.Modificare")]
        public void DeleteDisposition(int dispositionId)
        {
            var disposition = _dispositionRepository.GetAllIncluding(f => f.Operation, f => f.DispositionInvoices).FirstOrDefault(f => f.Id == dispositionId);

            if (!_operationRepository.VerifyClosedMonth(disposition.DispositionDate))
                throw new UserFriendlyException("Eroare", "Nu se poate inregistra operatia deoarece luna contabila este inchisa");

            try
            {
                if (disposition.Operation != null)
                {
                    _operationRepository.Delete(disposition.OperationId.Value);

                }

                if (disposition.DispositionInvoices.Count > 0)
                {
                    foreach (var dispInvoice in disposition.DispositionInvoices)
                    {
                        _dispositionInvoiceRepository.Delete(dispInvoice.Id);
                    }
                    CurrentUnitOfWork.SaveChanges();
                }

                _dispositionRepository.Delete(disposition);
                CurrentUnitOfWork.SaveChanges();

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare!", ex.Message);
            }
        }

        //[AbpAuthorize("Casierie.Numerar.Dispozitii.Acces")]
        public List<DispositionListDto> DispositionList(DateTime dataStart, DateTime dataEnd, int? dispositionState)
        {
            var dispositions = _dispositionRepository.GetAllIncluding(f => f.Currency, f => f.DocumentType, f => f.ThirdParty, f => f.Operation)
                                                     .Where(f => f.State == State.Active && f.DispositionDate >= dataStart && f.DispositionDate <= dataEnd &&
                                                    (dispositionState == null ?
                                                    (f.OperationType == OperationType.Plata || f.OperationType == OperationType.Incasare) :
                                                    (f.OperationType == (OperationType)dispositionState.Value))
                                                     )
                                                     .OrderByDescending(f => f.DispositionDate)
                                                     .ToList();

            var ret = ObjectMapper.Map<List<DispositionListDto>>(dispositions);
            return ret;
        }

        //[AbpAuthorize("Casierie.Numerar.Dispozitii.Acces")]
        public DispositionEditDto GetDisposition(int dispositionId)
        {
            DispositionEditDto disposition;
            var _currentDate = LazyMethods.Now();
            var localCurrencyId = GetCurrentTenant().LocalCurrencyId.Value;

            if (dispositionId == 0)
            {
                var categoryElementId = _invoiceRepository.GetInvoiceElementsDetailsCategories().FirstOrDefault(f => f.CategoryType == CategoryType.PlatiIncasari).Id;
                var documentTypeId = _documentTypeRepository.GetAll().FirstOrDefault(f => f.TypeNameShort == "DEC").Id;
                var elementId = _invoiceRepository.GetInvoiceElementsDetails().FirstOrDefault(f => f.InvoiceElementsDetailsCategoryId.Value == categoryElementId &&
                                                                                            f.Description == "Avans decontare").Id;
                disposition = new DispositionEditDto
                {
                    DispositionNumber = GetNextNumber(_currentDate),
                    DispositionDate = _currentDate,
                    CategoryElementId = categoryElementId,
                    CurrencyId = localCurrencyId,
                    OperationTypeId = null,
                    DocumentTypeId = documentTypeId,
                    ElementId = elementId,
                    InvoiceList = new List<InvoiceListSelectableDto>()
                };
            }
            else
            {
                var disp = _dispositionRepository.GetAllIncluding(f => f.ThirdParty, f => f.Currency, f => f.DocumentType, f => f.InvoiceElementsDetails, f => f.Operation/*, f => f.DispositionInvoice*/)
                                                 .FirstOrDefault(f => f.Id == dispositionId);

                try
                {
                    disposition = new DispositionEditDto
                    {
                        DispositionNumber = disp.DispositionNumber,
                        ThirdPartyId = disp.ThirdPartyId.Value,
                        ThirdPartyName = disp.ThirdParty.FullName,
                        DispositionDate = disp.DispositionDate,
                        OperationType = disp.OperationType,
                        Description = disp.Description,
                        DocumentNumber = disp.DocumentNumber,
                        DocumentDate = disp.DocumentDate,
                        DocumentTypeId = disp.DocumentTypeId,
                        Value = disp.Value,
                        CurrencyId = disp.CurrencyId,
                        Id = disp.Id,
                        ElementId = disp.InvoiceElementsDetailsId,
                        CategoryElementId = (disp.InvoiceElementsDetails != null) ? disp.InvoiceElementsDetails.InvoiceElementsDetailsCategoryId : 0,
                        OperationId = disp.OperationId,
                        TenantId = disp.TenantId,
                        State = disp.State,
                        NrChitanta = disp.NrChitanta ?? 0,
                        NumePrenume = disp.NumePrenume,
                        TipDoc = disp.TipDoc,
                        ActIdentitate = disp.ActIdentitate,
                        InvoiceList = new List<InvoiceListSelectableDto>()
                    };
                }
                catch (Exception ex)
                {

                    throw new UserFriendlyException("Eroare", ex);
                }

            }
            return disposition;
        }

        [AbpAuthorize("Casierie.Numerar.Dispozitii.Modificare")]
        public void SaveDisposition(DispositionEditDto disposition)
        {

            var appClient = GetCurrentTenant();
            decimal payedValue = 0;
            if (!_operationRepository.VerifyClosedMonth(disposition.DispositionDate))
                throw new UserFriendlyException("Eroare", "Nu se poate inregistra operatia deoarece luna contabila este inchisa");

            try
            {
                //sterg facturile paltite pentru dispozitia cu id-ul dispositionId
                var dispInvoices = _dispositionInvoiceRepository.GetAllIncluding(f => f.Disposition, f => f.Invoice).Where(f => f.DispositionId == disposition.Id && f.State == State.Active);
                foreach (var item in dispInvoices)
                {
                    _dispositionInvoiceRepository.Delete(item.Id);
                }
                CurrentUnitOfWork.SaveChanges();

                disposition.SumOper = disposition.OperationType == OperationType.Plata ? -disposition.Value : disposition.Value;
                //var invoice = _invoiceRepository.GetAllIncluding(f => f.PaymentOrders, f => f.InvoiceDetails, f => f.Dispositions, f=>f.ThirdParty)
                //                                .FirstOrDefault(f => f.State == State.Active && f.Id == disposition.InvoiceId);
                // var invoice = _invoiceRepository.InvoiceForDisposition(disposition.InvoiceId??0);

                if (disposition.ThirdPartyId == 0)
                {
                    throw new Exception("Nu ati selectat tertul");
                }
                if (disposition.CurrencyId == 0)
                {
                    throw new Exception("Nu ati selectat moneda");
                }

                var currency = _currencyRepository.Get(disposition.CurrencyId.Value);

                if (disposition.CategoryElementId == null || disposition.ElementId == null)
                {
                    throw new Exception("Nu ati selectat tipul operatiunii si operatiunea dispozitiei");
                }

                var sold = _dispositionRepository.SoldPrec(disposition.DispositionDate.AddDays(-1), disposition.CurrencyId.Value);
                var intraDay = _dispositionRepository.GetAll()
                                                     .Where(f => f.DispositionDate == disposition.DispositionDate && f.State == State.Active && f.Id != disposition.Id && f.OperationType != OperationType.SoldInitial)
                                                     .Sum(f => f.SumOper);
                sold += intraDay;

                if (sold < Math.Abs(disposition.SumOper) && disposition.OperationType == OperationType.Plata)
                {
                    throw new Exception($"Soldul { sold } este insuficient pentru a efectua o plata in valoare de {disposition.Value}");
                }

                //if (invoice != null)
                //{
                foreach (var item in disposition.InvoiceList.Where(f => f.Selected))
                {
                    var invoice = _invoiceRepository.InvoiceForDisposition(item.Id);
                    decimal restPlata = invoice.RestPlata;

                    if (item.PayedValue > invoice.RestPlata)
                    {
                        throw new Exception($"Suma platita nu poate fi mai mare decat restul de plata in valoare de {invoice.RestPlata} {invoice.Currency.CurrencyName}");
                    }

                    if (item.PayedValue < invoice.RestPlata && disposition.Value < item.PayedValue)
                    {
                        throw new Exception($"Valoarea platita pentru factura depaseste suma dispozitiei in valoare de {disposition.Value} {invoice.Currency.CurrencyName}");
                    }
                    if (disposition.Id != 0)
                    {
                        // iau total platit mai putin valoarea dispozitiei inainte de modificare
                        //  restPlata = invoice.PaymentOrder.Where(f => f.State == State.Active).Sum(f => f.Value);
                        //   restPlata += invoice.Dispositions.Where(f => f.Id != disposition.Id && f.State == State.Active).Sum(f => f.Value);

                        restPlata = invoice.PaymentOrderInvoices.Where(f => f.State == State.Active).Sum(f => f.PayedValue);
                        restPlata += invoice.DispositionInvoices.Where(f => f.Id != disposition.Id && f.State == State.Active).Sum(f => f.PayedValue);
                        restPlata = invoice.Value - restPlata;
                    }

                    //if (disposition.OperationType == OperationType.Plata && disposition.Value > restPlata)
                    //{
                    //    throw new Exception("Suma platita trebuie sa fie cel mult egala cu restul de plata in valoare de " + restPlata + ' ' + currency.CurrencyCode);
                    //}
                    if (disposition.Id == 0)
                    {
                        disposition.Description = invoice.ThirdParty.FullName + ", " + invoice.InvoiceSeries + " " + invoice.InvoiceNumber + " / " + invoice.InvoiceDate.ToShortDateString() + ", " +
                                  (invoice.StartDatePeriod.HasValue && invoice.EndDatePeriod.HasValue ? invoice.StartDatePeriod.Value.ToShortDateString() + " - " + invoice.EndDatePeriod.Value.ToShortDateString()
                                                                                                : "") + " " + disposition.Description;
                    }
                }
                // }

                // var _disp = ObjectMapper.Map<Disposition>(disposition);

                var _disp = new Disposition
                {
                    Id = disposition.Id,
                    ActIdentitate = disposition.ActIdentitate,
                    CurrencyId = disposition.CurrencyId.Value,
                    Description = disposition.Description,
                    DispositionDate = disposition.DispositionDate,
                    Value = disposition.Value,
                    DocumentDate = disposition.DocumentDate,
                    DispositionNumber = disposition.DispositionNumber,
                    DocumentNumber = disposition.DocumentNumber,
                    DocumentTypeId = disposition.DocumentTypeId,
                    InvoiceElementsDetailsId = disposition.ElementId,
                    NrChitanta = disposition.NrChitanta,
                    NumePrenume = disposition.NumePrenume,
                    OperationId = disposition.OperationId,
                    State = State.Active,
                    SumOper = disposition.SumOper,
                    TenantId = appClient.Id,
                    ThirdPartyId = disposition.ThirdPartyId,
                    TipDoc = disposition.TipDoc,
                    OperationType = disposition.OperationType
                };

                _dispositionRepository.InsertOrUpdateV(_disp);


                foreach (var item in disposition.InvoiceList.Where(f => f.Selected))
                {
                    var dispInvoice = new DispositionInvoice
                    {
                        InvoiceId = item.Id,
                        DispositionId = _disp.Id,
                        OperationDate = _disp.DispositionDate,
                        PayedValue = item.PayedValue,
                    };

                    _dispositionInvoiceRepository.Insert(dispInvoice);
                    CurrentUnitOfWork.SaveChanges();
                }

                _autoOperationRepository.DispositionToConta(_disp.Id, _disp.DispositionDate, appClient.LocalCurrencyId.Value);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }


        }

        public int GetNextNumber(DateTime date)
        {
            var nextNumber = 0;
            var item = _dispositionRepository.GetAll().Where(f => f.State == State.Active && f.DispositionDate.Year == date.Year)
                                                        .OrderByDescending(f => f.DispositionNumber)
                                                        .FirstOrDefault();
            if (item != null)
            {
                nextNumber = item.DispositionNumber + 1;
            }
            else
            {
                nextNumber = 1;
            }

            return nextNumber;
        }

        public int? GetNextNumberForChitanta(DateTime currentDate)
        {
            var nextNumber = 0;
            var item = _dispositionRepository.GetAll().Where(f => f.State == State.Active && f.DispositionDate.Year == currentDate.Year && f.OperationType == OperationType.Incasare)
                                                        .OrderByDescending(f => f.NrChitanta)
                                                        .FirstOrDefault();
            if (item == null)
            {
                return 1;
            }
            if (item.NrChitanta != null)
            {
                nextNumber = item.NrChitanta.Value + 1;
            }
            else
            {
                nextNumber = 1;
            }

            return nextNumber;
        }

    }
}
