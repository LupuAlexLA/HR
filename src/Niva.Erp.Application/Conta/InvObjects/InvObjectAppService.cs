using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.Conta.InvObjects.Dto;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Contracts;
using Niva.Erp.Models.InvObjects;
using Niva.Erp.Repositories;
using Niva.Erp.Repositories.Conta;
using Niva.Erp.Repositories.Conta.AutoOperation;
using Niva.Erp.Repositories.Conta.Nomenclatures;
using Niva.Erp.Repositories.Economic;
using Niva.Erp.Repositories.InvObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Conta.InvObjects
{
    public interface IInvObjectAppService : IApplicationService
    {
        GetInvObjectOutput InvObjectsList(DateTime dataStart, DateTime dataEnd);
        InvObjectAddDto AddFromInvoiceInit();
        InvObjectAddDirectDto AddDirectInit(int invObjectId);
        List<InvObjectAddInvoiceDetailDto> GetInvoiceDetails(int? invoiceId, int documentTypeId, DateTime operationDate);
        InvObjectAddDto PrepareInvObjects(InvObjectAddDto oper);
        InvObjectAddDirectDto SaveObjectDirect(InvObjectAddDirectDto invObject);
        InvObjectAddDto SaveObject(InvObjectAddDto oper);
        void DeleteInvObject(int invObjectId);
    }

    public class GetInvObjectOutput
    {
        public List<InvObjectListDto> GetInvObjects { get; set; }
    }

    public class InvObjectAppService : ErpAppServiceBase, IInvObjectAppService
    {
        IInvObjectRepository _invObjectRepository;
        IOperationRepository _operationRepository;
        IInvoiceRepository _invoiceRepository;
        IInvOperationRepository _invOperationRepository;
        IAutoOperationRepository _autoOperationRepository;
        IBalanceRepository _balanceRepository;
        IRepository<InvObjectOperDocType> _invObjectOperDocTypeRepository;
        IRepository<DocumentType> _documentTypeRepository;
        IRepository<InvStorage> _invStorageRepository;
        IRepository<InvoiceDetails> _invoiceDetailsRepository;
        IRepository<InvObjectStock> _invObjectStockRepository;
        IExchangeRatesRepository _exchangeRatesRepository;

        public InvObjectAppService(IInvObjectRepository invObjectRepository, IOperationRepository operationRepository, IRepository<InvObjectOperDocType> invObjectOperDocTypeRepository,
            IRepository<DocumentType> documentTypeRepository, IInvoiceRepository invoiceRepository, IInvOperationRepository invOperationRepository, IRepository<InvStorage> invStorageRepository,
            IRepository<InvoiceDetails> invoiceDetailsRepository, IAutoOperationRepository autoOperationRepository, IBalanceRepository balanceRepository,
            IRepository<InvObjectStock> invObjectStockRepository, IExchangeRatesRepository exchangeRatesRepository)
        {
            _invObjectRepository = invObjectRepository;
            _operationRepository = operationRepository;
            _invObjectOperDocTypeRepository = invObjectOperDocTypeRepository;
            _documentTypeRepository = documentTypeRepository;
            _invoiceRepository = invoiceRepository;
            _invOperationRepository = invOperationRepository;
            _invStorageRepository = invStorageRepository;
            _invoiceDetailsRepository = invoiceDetailsRepository;
            _autoOperationRepository = autoOperationRepository;
            _balanceRepository = balanceRepository;
            _invObjectStockRepository = invObjectStockRepository;
            _exchangeRatesRepository = exchangeRatesRepository;
        }

        //[AbpAuthorize("Conta.ObInventar.Intrari.Acces")]
        public GetInvObjectOutput InvObjectsList(DateTime dataStart, DateTime dataEnd)
        {
            DateTime _dataStart = new DateTime(dataStart.Year, dataStart.Month, dataStart.Day);
            DateTime _dataEnd = new DateTime(dataEnd.Year, dataEnd.Month, dataEnd.Day);

            var _invObjects = _invObjectRepository.GetAllIncluding(f => f.ThirdParty, f => f.PrimDocumentType, f => f.InvCateg).Where(f => f.State == State.Active
                                                           && f.OperationDate <= _dataEnd && f.OperationDate >= _dataStart).OrderBy(f => f.OperationDate).ThenBy(f => f.InventoryNr);

            var ret = new GetInvObjectOutput { GetInvObjects = ObjectMapper.Map<List<InvObjectListDto>>(_invObjects) };
            return ret;
        }

        // adaugare obiect de inventar pornind de la factura
        [AbpAuthorize("Conta.ObInventar.Intrari.Modificare")]
        public InvObjectAddDto AddFromInvoiceInit()
        {
            var ret = new InvObjectAddDto
            {
                OperationDate = LazyMethods.Now(),
                OperationType = InvObjectOperType.Intrare,
                ShowForm1 = true
            };
            var documentType = _documentTypeRepository.GetAllIncluding()
                                                             .Where(f => f.TypeNameShort == "FF")
                                                             .FirstOrDefault();
            ret.DocumentTypeId = documentType.Id;
            ret.DocumentType = documentType.TypeName;
            ret.FinishAdd = false;

            return ret;
        }

        // adaugare obiect de inventar fara factura
        //   [AbpAuthorize("Conta.ObInventar.Intrari.Acces")]
        public InvObjectAddDirectDto AddDirectInit(int invObjectId)
        {
            var ret = new InvObjectAddDirectDto();
            if (invObjectId == 0)
            {
                ret = new InvObjectAddDirectDto
                {
                    OperationDate = DateTime.Now,
                    DocumentDate = DateTime.Now,
                    OperationType = InvObjectOperType.Intrare,
                    Quantity = 1,
                    Processed = false,
                    ThirdPartyName = ""
                };

                var documentType = _invObjectOperDocTypeRepository.GetAllIncluding(f => f.DocumentType)
                                                                 .Where(f => f.OperType == InvObjectOperType.Intrare)
                                                                 .FirstOrDefault();
                ret.DocumentTypeId = documentType.DocumentType.Id;
                ret.DocumentType = documentType.DocumentType.TypeName;
                ret.FinishAdd = false;
            }
            else
            {
                var invObject = _invObjectRepository.GetAllIncluding(f => f.InvStorage, f => f.ThirdParty, f => f.InvCateg, f => f.InvoiceDetails).FirstOrDefault(f => f.Id == invObjectId);

                ret = new InvObjectAddDirectDto
                {
                    Id = invObject.Id,
                    OperationDate = invObject.OperationDate,
                    DocumentDate = invObject.DocumentDate,
                    DocumentNr = invObject.DocumentNr,
                    DocumentTypeId = invObject.DocumentTypeId,
                    InventoryNr = invObject.InventoryNr,
                    InventoryValue = invObject.InventoryValue,
                    InvCategoryId = invObject.InvCategId,
                    InvoiceDetailsId = invObject.InvoiceDetailsId,
                    Name = invObject.Name,
                    Processed = invObject.Processed,
                    OperationType = invObject.OperationType,
                    InvAccountId = invObject.InvObjectAccountId,
                    ExpenseAccountId = invObject.ExpenseAccountId,
                    Quantity = invObject.Quantity,
                    StorageInId = invObject.InvObjectStorageId,
                    FinishAdd = false,
                    PrimDocumentTypeId = invObject.PrimDocumentTypeId,
                    PrimDocumentNr = invObject.PrimDocumentNr,
                    PrimDocumentDate = invObject.PrimDocumentDate,
                    ThirdPartyId = invObject.ThirdPartyId,
                    ThirdPartyName = (invObject.ThirdParty != null) ? invObject.ThirdParty.FullName : "",
                };
            }
            return ret;
        }

        // detalii factura obiect de inventar
        public List<InvObjectAddInvoiceDetailDto> GetInvoiceDetails(int? invoiceId, int documentTypeId, DateTime operationDate)
        {
            var localCurrencyId = GetCurrentTenant().LocalCurrencyId.Value;
            if (invoiceId == null)
            {
                throw new UserFriendlyException("Eroare adaugare obiect de inventar", "Nu ati selectat factura!");
            }

            if (!_operationRepository.VerifyClosedMonth(operationDate))
                throw new UserFriendlyException("Eroare", "Nu se poate inregistra operatia deoarece luna contabila este inchisa");

            var documentNr = _invOperationRepository.NextDocumentNumber(documentTypeId);
            var _invoice = _invoiceRepository.GetAllIncludeElemDet().FirstOrDefault(f => f.Id == invoiceId);
            var _details = new List<InvObjectAddInvoiceDetailDto>();

            var invoiceDocType = _documentTypeRepository.FirstOrDefault(f => f.TypeNameShort == "FF");

            var exchangeRate = _exchangeRatesRepository.GetExchangeRateForOper(_invoice.InvoiceDate, _invoice.CurrencyId, localCurrencyId);

            foreach (var item in _invoice.InvoiceDetails.Where(f => f.InvoiceElementsDetails.InvoiceElementsType == InvoiceElementsType.ObiecteDeInventar && f.UsedInGest == false))
            {
                var _detail = new InvObjectAddInvoiceDetailDto
                {
                    InvoiceId = invoiceId ?? 0,
                    InvObjectName = item.Element,
                    ActivityTypeId = (item.Invoices.ActivityTypeId != null) ? item.Invoices.ActivityTypeId : null,
                    DocumentDate = DateTime.Now,
                    DocumentNr = documentNr,
                    Quantity = item.Quantity,
                    InvValue = Math.Round((item.Value + item.VAT) * exchangeRate, 2),
                    InvoiceDetailsId = item.Id,
                    PrimDocumentTypeId = invoiceDocType.Id,
                    PrimDocumentNr = item.Invoices.InvoiceNumber,
                    PrimDocumentDate = item.Invoices.InvoiceDate,
                    ThirdPartyId = item.Invoices.ThirdPartyId

                };
                _details.Add(_detail);
                documentNr++;
            }

            return _details;
        }

        public InvObjectAddDto PrepareInvObjects(InvObjectAddDto oper)
        {
            try
            {
                var appClient = GetCurrentTenant();
                var localCurrencyId = appClient.LocalCurrencyId.Value;
                var _details = new List<InvObjectAddDetailDto>();
                var _id = 0;
                foreach (var item in oper.InvoiceDetail)
                {
                    var storage = _invStorageRepository.FirstOrDefault(f => f.Id == item.StorageInId).StorageName;
                    var _invoiceElemDetails = _invoiceDetailsRepository.GetAllIncluding(f => f.InvoiceElementsDetails).Where(f => f.InvoicesId == item.InvoiceId && f.Id == item.InvoiceDetailsId)
                                                                       .Select(f => f.InvoiceElementsDetails).ToList();

                    var _detail = new InvObjectAddDetailDto
                    {
                        Id = _id,
                        Name = item.InvObjectName,
                        InventoryValue = Math.Round(item.InvValue / item.Quantity, 2),
                        Quantity = item.Quantity,
                        InvoiceDetailsId = item.InvoiceDetailsId,
                        StorageInId = item.StorageInId,
                        StorageIn = storage,
                        DocumentNr = item.DocumentNr,
                        DocumentDate = item.DocumentDate.Date,
                        PrimDocumentTypeId = item.PrimDocumentTypeId,
                        PrimDocumentNr = item.PrimDocumentNr,
                        PrimDocumentDate = item.PrimDocumentDate,
                        ThirdPartyId = item.ThirdPartyId
                    };

                    foreach (var elemDetail in _invoiceElemDetails)
                    {
                        try
                        {
                            var invObjectAccountId = _autoOperationRepository.GetAutoAccount(elemDetail.CorrespondentAccount, item.ThirdPartyId, oper.OperationDate, localCurrencyId, null);

                            _detail.InvObjectAccountId = invObjectAccountId;

                        }
                        catch
                        {
                            throw new Exception("Nu am identificat contul pentru obiectul de inventar: " + elemDetail.CorrespondentAccount);
                        }

                        try
                        {
                            var amortExpenseAccountId = 0;
                            if (item.ActivityTypeId != null && elemDetail.ExpenseAmortizAccount.StartsWith('6'))
                            {
                                amortExpenseAccountId = _autoOperationRepository.GetAutoAccountActivityType(elemDetail.ExpenseAmortizAccount, item.ThirdPartyId, item.ActivityTypeId.Value, oper.OperationDate, localCurrencyId, null);
                            }
                            else
                            {
                                amortExpenseAccountId = _autoOperationRepository.GetAutoAccount(elemDetail.ExpenseAmortizAccount, item.ThirdPartyId, oper.OperationDate, localCurrencyId, null);

                            }
                            _detail.ExpenseAccountId = amortExpenseAccountId;
                        }
                        catch
                        {
                            throw new Exception("Nu am identificat contul de cheltuiala: " + elemDetail.ExpenseAmortizAccount);
                        }
                    }

                    for (int i = 1; i <= item.Quantity; i++)
                    {
                        _detail.Quantity = 1;
                        _details.Add(_detail);
                        _id++;
                    }
                }
                oper.InvObjects = _details;
                return oper;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        //salvare obiect de inventar fara factura
        [AbpAuthorize("Conta.ObInventar.Intrari.Modificare")]
        public InvObjectAddDirectDto SaveObjectDirect(InvObjectAddDirectDto invObject)
        {
            //verificare completare date
            if (!_operationRepository.VerifyClosedMonth(invObject.OperationDate))
                throw new UserFriendlyException("Eroare", "Nu se poate inregistra operatia deoarece luna contabila este inchisa");

            if ((invObject.InvAccountId == null) || (invObject.ExpenseAccountId == null) || (invObject.StorageInId == null))
            {
                throw new UserFriendlyException("Eroare adaugare obiect inventar", "Trebuie sa selectati gestiunea si conturile corespunzatoare");
            }

            //sterg notele contabile si gestiunea pentru obiectul de inventar
            if (invObject.Processed == true)
            {
                var invObjectItem = _invObjectStockRepository.GetAll().Where(f => f.InvObjectItemPFId == invObject.Id && f.OperType == InvObjectOperType.Intrare).FirstOrDefault();
                if (invObjectItem != null)
                {
                    _autoOperationRepository.DeleteInvObjectDetailOperation(invObject.OperationDate, invObject.Id);
                }

                var lastProcessedDateForAsset = _invOperationRepository.LastProcessedDateForInvObject(invObject.Id);
                try
                {
                    if (invObject.OperationDate <= lastProcessedDateForAsset)
                    {
                        _invOperationRepository.GestDelComputingForInvObject(invObject.OperationDate, invObject.Id);
                    }
                }
                catch (Exception ex)
                {

                    throw new UserFriendlyException("Eroare", ex.Message);
                }

            }


            // iau urmatorul numar de inventar
            var invObjectList = _invObjectRepository.GetAll().Where(f => f.State == State.Active);
            var nextInvNumber = 0;
            if (invObject.Id == 0)
            {
                try
                {
                    var countInvObject = invObjectList.Count();
                    if (countInvObject == 0)
                    {
                        nextInvNumber = 1;
                    }
                    else
                    {
                        nextInvNumber = invObjectList.Max(f => f.InventoryNr) + 1;
                    }
                }
                catch
                {
                    nextInvNumber = 1;
                }
            }
            else
            {
                nextInvNumber = invObject.InventoryNr;
            }

            var inventoryObj = new InvObjectItem
            {
                Id = invObject.Id,
                Name = invObject.Name,
                InventoryValue = invObject.InventoryValue,
                InvObjectAccountId = invObject.InvAccountId,
                ExpenseAccountId = invObject.ExpenseAccountId,
                PriceUnit = invObject.InventoryValue,
                Processed = false,
                Quantity = invObject.Quantity,
                DocumentTypeId = invObject.DocumentTypeId,
                DocumentNr = invObject.DocumentNr,
                DocumentDate = invObject.DocumentDate.Date,
                InventoryNr = nextInvNumber,
                InvCategId = invObject.InvCategoryId.Value,
                OperationDate = invObject.OperationDate.Date,
                InvObjectStorageId = invObject.StorageInId,
                InvoiceDetailsId = invObject.InvoiceDetailsId,
                State = State.Active,
                PrimDocumentTypeId = invObject.PrimDocumentTypeId,
                PrimDocumentNr = invObject.PrimDocumentNr,
                PrimDocumentDate = invObject.PrimDocumentDate,
                ThirdPartyId = invObject.ThirdPartyId
            };

            var appClient = GetCurrentTenant();
            try
            {
                if (inventoryObj.Id == 0)
                {
                    _invObjectRepository.Insert(inventoryObj);
                }
                else
                {
                    inventoryObj.TenantId = appClient.Id;
                    _invObjectRepository.UpdateInvObject(inventoryObj);
                }
                CurrentUnitOfWork.SaveChanges();

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare adaugare Obiect de Inventar", ex.Message);
            }

            //calcul gestiune pentru obiectul de inventar
            try
            {
                _invOperationRepository.GestComputingForInvObject(inventoryObj.Id, inventoryObj.OperationDate);
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare calcul gestiune", ex.Message);
            }

            // generare nota contabila
            try
            {
                var lastBalanceDate = _balanceRepository.GetAll().Where(f => f.Status == State.Active).OrderByDescending(f => f.BalanceDate).FirstOrDefault().BalanceDate;
                _autoOperationRepository.AutoInvObjectOperationAdd(inventoryObj.OperationDate, appClient.Id, appClient.LocalCurrencyId.Value, lastBalanceDate, null);
            }
            catch (Exception ex)
            {
                throw new Exception($"Nota contabila nu poate fi generata pentru obiectul de inventar {inventoryObj.Name}");
            }

            invObject.FinishAdd = true;

            return invObject;
        }

        // adaug obiect de inventar pornind de la factura
        public InvObjectAddDto SaveObject(InvObjectAddDto oper)
        {
            //verificare completare date
            foreach (var item in oper.InvObjects)
            {
                if ((item.InvObjectAccountId == null) || (item.ExpenseAccountId == null))
                {
                    throw new UserFriendlyException("Eroare adaugare obiecte de inventar", "Trebuie sa selectati conturile corespunzatoare");
                }
            }

            foreach (var item in oper.InvObjects)
            {
                if ((item.InvCategoryId == null))
                {
                    throw new UserFriendlyException("Eroare adaugare obiecte de inventar", "Trebuie sa selectati categoria obiectului");
                }
            }

            // iau urmatorul numar de inventar
            var invObjectList = _invObjectRepository.GetAll().Where(f => f.State == State.Active);
            var nextInvNumber = 0;
            try
            {
                var countInvObject = invObjectList.Count();
                if (countInvObject == 0)
                {
                    nextInvNumber = 1;
                }
                else
                {
                    nextInvNumber = invObjectList.Max(f => f.InventoryNr) + 1;
                }
            }
            catch
            {
                nextInvNumber = 1;
            }

            //salvare obiecte de inventar
            foreach (var item in oper.InvObjects)
            {

                var invObject = new InvObjectItem
                {
                    Name = item.Name,
                    InventoryValue = item.InventoryValue,
                    InvObjectAccountId = item.InvObjectAccountId,
                    ExpenseAccountId = item.ExpenseAccountId,
                    PriceUnit = item.InventoryValue,
                    Processed = false,
                    Quantity = item.Quantity,
                    OperationType = oper.OperationType,
                    DocumentTypeId = oper.DocumentTypeId,
                    DocumentNr = item.DocumentNr,
                    DocumentDate = item.DocumentDate.Date,
                    InvCategId = item.InvCategoryId.Value,
                    InventoryNr = nextInvNumber,
                    InvoiceDetailsId = item.InvoiceDetailsId,
                    OperationDate = oper.OperationDate.Date,
                    InvObjectStorageId = item.StorageInId,
                    State = State.Active,
                    PrimDocumentTypeId = item.PrimDocumentTypeId,
                    PrimDocumentNr = item.PrimDocumentNr,
                    PrimDocumentDate = item.PrimDocumentDate,
                    ThirdPartyId = item.ThirdPartyId
                };

                try
                {
                    _invObjectRepository.Insert(invObject);
                    CurrentUnitOfWork.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new UserFriendlyException("Eroare adaugare Obiect de inventar", ex.Message);
                }

                try
                {
                    _invOperationRepository.GestComputingForInvObject(invObject.Id, invObject.OperationDate);
                }
                catch (Exception ex)
                {

                    throw new UserFriendlyException("Eroare", ex.Message);
                }

                nextInvNumber++;
            }

            // generez note contabile 

            try
            {
                var appClient = GetCurrentTenant();
                var lastBalanceDate = _balanceRepository.GetAll().Where(f => f.Status == State.Active).OrderByDescending(f => f.BalanceDate).FirstOrDefault().BalanceDate;
                _autoOperationRepository.AutoInvObjectOperationAdd(oper.OperationDate, appClient.Id, appClient.LocalCurrencyId.Value, lastBalanceDate, null);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", "Notele contabile nu au fost generate");
            }

            oper.FinishAdd = true;

            return oper;
        }

        [AbpAuthorize("Conta.ObInventar.Intrari.Modificare")]
        public void DeleteInvObject(int invObjectId)
        {
            var invObject = _invObjectRepository.FirstOrDefault(f => f.Id == invObjectId);

            if (!_operationRepository.VerifyClosedMonth(invObject.OperationDate))
                throw new UserFriendlyException("Eroare", "Nu se poate inregistra operatia deoarece luna contabila este inchisa");

            invObject.State = State.Inactive;
        }

    }
}
