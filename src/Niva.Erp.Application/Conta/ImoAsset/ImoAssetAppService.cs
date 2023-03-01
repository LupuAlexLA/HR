using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Contracts;
using Niva.Erp.Models.ImoAsset;
using Niva.Erp.Repositories;
using Niva.Erp.Repositories.Conta;
using Niva.Erp.Repositories.Conta.AutoOperation;
using Niva.Erp.Repositories.Conta.Nomenclatures;
using Niva.Erp.Repositories.Economic;
using Niva.Erp.Repositories.ImoAsset;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Conta.ImoAsset
{
    public interface IImoAssetAppService : IApplicationService
    {
        GetImoAssetOutput ImoAssetsEntryList(DateTime dataStart, DateTime dataEnd);

        ImoAssetAddDto AddFromInvoiceInit();

        ImoAssetAddDto ShowForm(ImoAssetAddDto oper, int formNr);

        List<ImoAssetAddInvoiceDetailDto> GetInvoiceDetails(int? invoiceId, int documentTypeId, DateTime operationDate);

        ImoAssetAddDto PrepareAssets(ImoAssetAddDto oper);

        ImoAssetAddDto SaveAssets(ImoAssetAddDto oper);


        ImoAssetAddDirectDto AddDirectInit(int assetId);

        ImoAssetAddDirectDto AddDirectChangeType(ImoAssetAddDirectDto asset);

        ImoAssetAddDirectDto AddDirectChangeDate(ImoAssetAddDirectDto oper);

        ImoAssetAddDirectDto AddInUseChangeDate(ImoAssetAddDirectDto oper); // setez data pentru amortizare

        ImoAssetAddDirectDto SaveAssetDirect(ImoAssetAddDirectDto oper);

        void SaveAssetInUse(ImoAssetAddDirectDto oper);

        void DeleteAsset(int assetId);
    }

    public class GetImoAssetOutput
    {
        public List<ImoAssetListDto> GetImoAssets { get; set; }
    }

    public class ImoAssetAppService : ErpAppServiceBase, IImoAssetAppService
    {
        IImoAssetRepository _imoAssetRepository;
        IRepository<ImoAssetOperDocType> _imoAssetOperDocTypeRepository;
        IInvoiceRepository _invoiceRepository;
        IRepository<ImoAssetStorage> _imoAssetStorageRepository;
        IImoOperationRepository _imoOperationRepository;
        IOperationRepository _operationRepository;
        IRepository<DocumentType> _documentTypeRepository;
        IAutoOperationRepository _autoOperationRepository;
        IBalanceRepository _balanceRepository;
        IRepository<InvoiceDetails> _invoiceDetailsRepository;
        IAccountRepository _accountRepository;
        IRepository<ImoAssetStock> _imoAssetStockRepository;
        IExchangeRatesRepository _exchangeRatesRepository;

        public ImoAssetAppService(IImoAssetRepository imoAssetRepository, IRepository<ImoAssetOperDocType> imoAssetOperDocTypeRepository,
                                  IInvoiceRepository invoiceRepository, IRepository<ImoAssetStorage> imoAssetStorageRepository, IImoOperationRepository imoOperationRepository,
                                  IOperationRepository operationRepository, IRepository<DocumentType> documentTypeRepository,
                                  IAutoOperationRepository autoOperationRepository, IBalanceRepository balanceRepository, IRepository<InvoiceDetails> invoiceDetailsRepository,
                                  IAccountRepository accountRepository, IRepository<ImoAssetStock> imoAssetStockRepository, IExchangeRatesRepository exchangeRatesRepository)
        {
            _imoAssetRepository = imoAssetRepository;
            _imoAssetOperDocTypeRepository = imoAssetOperDocTypeRepository;
            _invoiceRepository = invoiceRepository;
            _imoAssetStorageRepository = imoAssetStorageRepository;
            _imoOperationRepository = imoOperationRepository;
            _operationRepository = operationRepository;
            _documentTypeRepository = documentTypeRepository;
            _autoOperationRepository = autoOperationRepository;
            _balanceRepository = balanceRepository;
            _invoiceDetailsRepository = invoiceDetailsRepository;
            _accountRepository = accountRepository;
            _imoAssetStockRepository = imoAssetStockRepository;
            _exchangeRatesRepository = exchangeRatesRepository;
        }

        //[AbpAuthorize("Conta.MF.Intrari.Acces")]
        public GetImoAssetOutput ImoAssetsEntryList(DateTime dataStart, DateTime dataEnd)
        {
            DateTime _dataStart = new DateTime(dataStart.Year, dataStart.Month, dataStart.Day);
            DateTime _dataEnd = new DateTime(dataEnd.Year, dataEnd.Month, dataEnd.Day);

            var _myImoAssets = _imoAssetRepository.GetAllIncluding(f => f.InvoiceDetails, f => f.DocumentType, f => f.AssetClassCodes, f => f.InvoiceDetails.Invoices,
                                                                  f => f.InvoiceDetails.Invoices.ThirdParty, f => f.InvoiceDetails.Invoices.ThirdParty,
                                                                  f => f.ThirdParty, f => f.PrimDocumentType)
                                                   .Where(f => f.State == State.Active
                                                           && f.OperationDate <= _dataEnd && f.OperationDate >= _dataStart)
                                                   .OrderBy(f => f.OperationDate).ThenBy(f => f.InventoryNr);

            var ret = new GetImoAssetOutput { GetImoAssets = ObjectMapper.Map<List<ImoAssetListDto>>(_myImoAssets) };
            return ret;
        }

        //[AbpAuthorize("Conta.MF.Intrari.Acces")]
        public ImoAssetAddDto AddFromInvoiceInit()
        {
            var ret = new ImoAssetAddDto
            {
                OperationDate = DateTime.Now,
                OperationType = ImoAssetOperType.Intrare,
                ShowForm1 = true
            };
            var documentType = _documentTypeRepository.GetAllIncluding()
                                                             .Where(f => f.TypeNameShort == "FF")
                                                             .FirstOrDefault();
            ret.DocumentTypeId = documentType.Id;
            ret.DocumentType = documentType.TypeName;
            ret.FinishAdd = false;

            //ret = ShowForm(ret, 1);
            return ret;
        }

        //[AbpAuthorize("Conta.MF.Intrari.Acces")]
        public ImoAssetAddDto ShowForm(ImoAssetAddDto oper, int formNr)
        {
            oper.ShowForm1 = (formNr == 1);
            oper.ShowForm2 = (formNr == 2);
            oper.ShowForm3 = (formNr == 3);
            return oper;
        }

        //[AbpAuthorize("Conta.MF.Intrari.Acces")]
        public List<ImoAssetAddInvoiceDetailDto> GetInvoiceDetails(int? invoiceId, int documentTypeId, DateTime operationDate)
        {
            var localCurrencyId = GetCurrentTenant().LocalCurrencyId.Value;
            if (invoiceId == null)
            {
                throw new UserFriendlyException("Eroare adaugare mijloace fixe", "Nu ati selectat factura!");
            }

            if (!_operationRepository.VerifyClosedMonth(operationDate))
                throw new UserFriendlyException("Eroare", "Nu se poate inregistra operatia deoarece luna contabila este inchisa");

            //DateTime lastProcessedDate = _imoOperationRepository.LastProcessedDateAdd();
            //if (lastProcessedDate >= operationDate)
            //    throw new UserFriendlyException("Eroare data operatie", "Data operatiei nu poate fi mai mica decat data ultimei operatii procesate in gestiune " + lastProcessedDate.ToShortDateString());


            var documentNr = _imoOperationRepository.NextDocumentNumber(documentTypeId);
            var _invoice = _invoiceRepository.GetAllIncludeElemDet().FirstOrDefault(f => f.Id == invoiceId);
            var _details = new List<ImoAssetAddInvoiceDetailDto>();

            var invoiceDocType = _documentTypeRepository.FirstOrDefault(f => f.TypeNameShort == "FF");

            var exchangeRate = _exchangeRatesRepository.GetExchangeRateForOper(_invoice.InvoiceDate, _invoice.CurrencyId, localCurrencyId);

            foreach (var item in _invoice.InvoiceDetails.Where(f => f.InvoiceElementsDetails.InvoiceElementsType == InvoiceElementsType.MijloaceFixe && f.UsedInGest == false))
            {
                var _detail = new ImoAssetAddInvoiceDetailDto
                {
                    InvoiceId = invoiceId ?? 0,
                    AssetName = item.Element,
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

            //oper = ShowForm(oper, 2);
            return _details;
        }
        
        //[AbpAuthorize("Conta.MF.Intrari.Acces")]
        public ImoAssetAddDto PrepareAssets(ImoAssetAddDto oper)
        {
            try
            {
                var appClient = GetCurrentTenant();
                var localCurrencyId = appClient.LocalCurrencyId.Value;
                var _details = new List<ImoAssetAddDetailDto>();
                var _id = 0;
                foreach (var item in oper.InvoiceDetail)
                {
                    var storage = _imoAssetStorageRepository.FirstOrDefault(f => f.Id == item.StorageInId).StorageName;
                    var _invoiceElemDetails = _invoiceDetailsRepository.GetAllIncluding(f => f.InvoiceElementsDetails).Where(f => f.InvoicesId == item.InvoiceId && f.Id == item.InvoiceDetailsId).Select(f => f.InvoiceElementsDetails).ToList();
                    var invoice = _invoiceRepository.GetAll().FirstOrDefault(f => f.Id == item.InvoiceId);

                    var _detail = new ImoAssetAddDetailDto
                    {
                        Id = _id,
                        Name = item.AssetName,
                        InventoryValue = Math.Round(item.InvValue / item.Quantity, 2),
                        FiscalInventoryValue = Math.Round(item.InvValue / item.Quantity, 2),
                        Quantity = item.Quantity,
                        InvoiceDetailsId = item.InvoiceDetailsId,
                        Depreciation = 0,
                        FiscalDepreciation = 0,
                        DurationInMonths = 0,
                        DepreciationStartDate = null,
                        UseStartDate = null,
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
                            var assetAccountId = _autoOperationRepository.GetAutoAccount(elemDetail.CorrespondentAccount, item.ThirdPartyId, oper.OperationDate, localCurrencyId, null);
                            _detail.AssetAccountId = assetAccountId;
                        }
                        catch
                        {
                            throw new Exception("Nu am identificat contul pentru mijlocul fix: " + elemDetail.CorrespondentAccount);
                        }

                        try
                        {
                            var amortAccountId = _autoOperationRepository.GetAutoAccount(elemDetail.AmortizationAccount, item.ThirdPartyId, oper.OperationDate, localCurrencyId, null);
                            _detail.DepreciationAccountId = amortAccountId;
                        }
                        catch
                        {
                            throw new Exception("Nu am identificat contul de amortizare: " + elemDetail.AmortizationAccount);
                        }

                        try
                        {
                            var amortExpenseAccountId = _autoOperationRepository.GetAutoAccount(elemDetail.ExpenseAmortizAccount, item.ThirdPartyId, oper.OperationDate, localCurrencyId, null);
                            _detail.ExpenseAccountId = amortExpenseAccountId;
                        }
                        catch
                        {
                            throw new Exception("Nu am identificat contul de cheltuiala cu amortizarea: " + elemDetail.ExpenseAmortizAccount);
                        }
                    }

                    for (int i = 1; i <= item.Quantity; i++)
                    {
                        _detail.Quantity = 1;
                        _details.Add(_detail);
                        _id++;
                    }


                }

                oper.Assets = _details;
                //oper = ShowForm(oper, 3);
                return oper;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        // adaug MF pornind de la factura
        [AbpAuthorize("Conta.MF.Intrari.Modificare")]
        public ImoAssetAddDto SaveAssets(ImoAssetAddDto oper)
        {
            try
            {
                var localCurrencyId = GetCurrentTenant().LocalCurrencyId.Value;
                //verificare completare date
                foreach (var item in oper.Assets)
                {
                    if ((item.AssetAccountId == null) || (item.ExpenseAccountId == null) || (item.DepreciationAccountId == null) || (item.DurationInMonths == 0))
                    {
                        throw new UserFriendlyException("Eroare adaugare mijloace fixe", "Trebuie sa selectati conturile corespunzatoare si sa completati durata de amortizare");
                    }
                }

                // iau urmatorul numar de inventar
                var assetList = _imoAssetRepository.GetAll().Where(f => f.State == State.Active);
                var nextInvNumber = 0;
                try
                {
                    var countAsset = assetList.Count();
                    if (countAsset == 0)
                    {
                        nextInvNumber = 1;
                    }
                    else
                    {
                        nextInvNumber = assetList.Max(f => f.InventoryNr) + 1;
                    }
                }
                catch
                {
                    nextInvNumber = 1;
                }

                int assetAccountId = 0;
                var invoice = _invoiceRepository.GetAll().FirstOrDefault(f => f.Id == oper.InvoiceId);

                //salvare mijloace fixe
                foreach (var item in oper.Assets)
                {
                    var account = _accountRepository.GetAll().FirstOrDefault(f => f.Id == item.AssetAccountId && f.Status == State.Active);

                    if (invoice.ActivityTypeId != null && account.Symbol.StartsWith('6'))
                    {
                        assetAccountId = _autoOperationRepository.GetAutoAccountActivityType(account.Symbol, item.ThirdPartyId, invoice.ActivityTypeId.Value, oper.OperationDate, invoice.CurrencyId, null);
                    }
                    else
                    {
                        assetAccountId = item.AssetAccountId.Value;
                    }

                    var asset = new ImoAssetItem
                    {
                        Name = item.Name,
                        InventoryValue = item.InventoryValue,
                        FiscalInventoryValue = item.FiscalInventoryValue,
                        AssetAccountId = assetAccountId,
                        DepreciationAccountId = item.DepreciationAccountId,
                        ExpenseAccountId = item.ExpenseAccountId,
                        Depreciation = 0,
                        FiscalDepreciation = 0,
                        InConservare = false,
                        InStock = true,
                        PriceUnit = item.InventoryValue,
                        Processed = false,
                        ProcessedIn = false,
                        ProcessedInUse = false,
                        Quantity = item.Quantity,
                        OperationType = oper.OperationType,
                        DocumentTypeId = oper.DocumentTypeId,
                        DocumentNr = item.DocumentNr,
                        DocumentDate = item.DocumentDate.Date,
                        UseStartDate = item.UseStartDate,
                        DepreciationStartDate = item.DepreciationStartDate,
                        InventoryNr = nextInvNumber,
                        AssetClassCodesId = item.AssetClassCodesId,
                        DurationInMonths = item.DurationInMonths,
                        InvoiceDetailsId = item.InvoiceDetailsId,
                        OperationDate = oper.OperationDate.Date,
                        ImoAssetStorageId = item.StorageInId,
                        State = State.Active,
                        PrimDocumentTypeId = item.PrimDocumentTypeId,
                        PrimDocumentNr = item.PrimDocumentNr,
                        PrimDocumentDate = item.PrimDocumentDate,
                        ThirdPartyId = item.ThirdPartyId
                    };

                    try
                    {
                        _imoAssetRepository.Insert(asset);
                        CurrentUnitOfWork.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        throw new UserFriendlyException("Eroare adaugare Mijloc fix", ex.Message);
                    }

                    if (asset.UseStartDate != null)
                    {
                        // generez gestiune pentru operatia de Intrare
                        GestComputeForAssetIn(asset.OperationDate, asset.Id, asset.ImoAssetType);

                        // generez gestiune pentru operatia de PunereInFunctiune
                        GestComputeForAssetInUse(asset.UseStartDate.Value, asset.Id, asset.ImoAssetType);
                    }
                    else
                    {
                        // generez gestiune pentru operatia de Intrare
                        GestComputeForAssetIn(asset.OperationDate, asset.Id, asset.ImoAssetType);
                    }

                    nextInvNumber++;
                }

                oper.FinishAdd = true;


                return oper;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        // gestiune pentru operatia de Intrare
        [AbpAuthorize("Conta.MF.Intrari.Modificare")]
        private void GestComputeForAssetIn(DateTime operationDate, int assetId, ImoAssetType imoAssetType)
        {
            //calculez gestiune mijloc fix
            try
            {
                _imoOperationRepository.GestComputingForAsset(operationDate, assetId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            try
            {
                var lastBalanceDate = _balanceRepository.GetAll().Where(f => f.Status == State.Active)
                             .OrderByDescending(f => f.BalanceDate)
                             .FirstOrDefault().BalanceDate;
                var appClient = GetCurrentTenant();
                _autoOperationRepository.AutoImoAssetOperationAdd(operationDate, appClient.Id, appClient.LocalCurrencyId.Value, imoAssetType, lastBalanceDate, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // gestiune pentru operatia de PunereInFunctiune
        [AbpAuthorize("Conta.MF.Intrari.Modificare")]
        private void GestComputeForAssetInUse(DateTime useStartDate, int assetId, ImoAssetType imoAssetType)
        {
            //calculez gestiune mijloc fix
            try
            {
                _imoOperationRepository.GestComputingForAsset(useStartDate, assetId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            try
            {
                var lastBalanceDate = _balanceRepository.GetAll().Where(f => f.Status == State.Active)
                             .OrderByDescending(f => f.BalanceDate)
                             .FirstOrDefault().BalanceDate;
                var appClient = GetCurrentTenant();

                _autoOperationRepository.AutoImoAssetOperationAdd(useStartDate, appClient.Id, appClient.LocalCurrencyId.Value, imoAssetType, lastBalanceDate, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [AbpAuthorize("Conta.MF.Intrari.Modificare")]
        public ImoAssetAddDirectDto AddDirectInit(int assetId)
        {
            ImoAssetAddDirectDto ret;
            if (assetId == 0)
            {
                ret = new ImoAssetAddDirectDto
                {
                    OperationDate = DateTime.Now,
                    OperationType = ImoAssetOperType.Intrare,
                    DepreciationStartDate = LazyMethods.FirstDayNextMonth(DateTime.Now),
                    UseStartDate = DateTime.Now/*LazyMethods.FirstDayNextMonth(DateTime.Now)*/,
                    DocumentDate = DateTime.Now,
                    Quantity = 1,
                    Processed = false,
                    ThirdPartyName = "",
                    AssetType = 0
                };

                var operType = (ImoAssetType)ret.AssetType == ImoAssetType.MijlocFix ? ImoAssetOperType.PunereInFunctiune : ImoAssetOperType.Intrare;
                var documentType = _imoAssetOperDocTypeRepository.GetAllIncluding(f => f.DocumentType)
                                                                 .Where(f => f.OperType == operType)
                                                                 .FirstOrDefault();
                ret.DocumentTypeId = documentType.DocumentType.Id;
                var documentNr = _imoOperationRepository.NextDocumentNumber(ret.DocumentTypeId);
                ret.DocumentNr = documentNr;
                ret.DocumentType = documentType.DocumentType.TypeName;
                ret.FinishAdd = false;
            }
            else
            {
                var asset = _imoAssetRepository.GetAllIncluding(f => f.ImoAssetStorage, f => f.ThirdParty).FirstOrDefault(f => f.Id == assetId);

                ret = new ImoAssetAddDirectDto
                {
                    Id = asset.Id,
                    OperationDate = asset.OperationDate,
                    DocumentNr = asset.DocumentNr,
                    DocumentDate = asset.DocumentDate,
                    DocumentTypeId = asset.DocumentTypeId,
                    InventoryNr = asset.InventoryNr,
                    Name = asset.Name,
                    Processed = asset.Processed,
                    ProcessedIn = asset.ProcessedIn,
                    ProcessedInUse = asset.ProcessedInUse,
                    OperationType = asset.OperationType,
                    AssetAccountId = asset.AssetAccountId,
                    AssetAccountInUseId = asset.AssetAccountInUseId,
                    AssetClassCodesId = asset.AssetClassCodesId,
                    Depreciation = asset.Depreciation,
                    DepreciationAccountId = asset.DepreciationAccountId,
                    DepreciationStartDate = asset.DepreciationStartDate,
                    DurationInMonths = asset.DurationInMonths,
                    RemainingDuration = asset.RemainingDuration,
                    ExpenseAccountId = asset.ExpenseAccountId,
                    FiscalDepreciation = asset.FiscalDepreciation,
                    FiscalInventoryValue = asset.FiscalInventoryValue,
                    MonthlyDepreciation = asset.MonthlyDepreciation,
                    MonthlyFiscalDepreciation = asset.MonthlyFiscalDepreciation,
                    InventoryValue = asset.InventoryValue,
                    InvoiceDetailsId = asset.InvoiceDetailsId,
                    Quantity = asset.Quantity,
                    StorageInId = asset.ImoAssetStorageId,
                    UseStartDate = asset.UseStartDate,
                    FinishAdd = false,
                    PrimDocumentTypeId = asset.PrimDocumentTypeId,
                    PrimDocumentNr = asset.PrimDocumentNr,
                    PrimDocumentDate = asset.PrimDocumentDate,
                    ThirdPartyId = asset.ThirdPartyId,
                    ThirdPartyName = (asset.ThirdParty != null) ? asset.ThirdParty.FullName : "",
                    AssetType = (int)asset.ImoAssetType
                };
            }

            return ret;
        }
      
        [AbpAuthorize("Conta.MF.Intrari.Modificare")]
        public ImoAssetAddDirectDto AddDirectChangeType(ImoAssetAddDirectDto asset)
        {
            var operType = (ImoAssetType)asset.AssetType == ImoAssetType.MijlocFix ? ImoAssetOperType.PunereInFunctiune : ImoAssetOperType.Intrare;
            var documentType = _imoAssetOperDocTypeRepository.GetAllIncluding(f => f.DocumentType)
                                                             .Where(f => f.OperType == operType)
                                                             .FirstOrDefault();
            asset.DocumentTypeId = documentType.DocumentType.Id;
            var documentNr = _imoOperationRepository.NextDocumentNumber(asset.DocumentTypeId);
            asset.DocumentNr = documentNr;
            asset.DocumentType = documentType.DocumentType.TypeName;
            asset.FinishAdd = false;
            asset.OperationType = operType;

            return asset;
        }
      
        [AbpAuthorize("Conta.MF.Intrari.Modificare")]
        public ImoAssetAddDirectDto AddDirectChangeDate(ImoAssetAddDirectDto oper)
        {
            oper.UseStartDate = LazyMethods.FirstDayNextMonth(oper.OperationDate);
            oper.DepreciationStartDate = LazyMethods.FirstDayNextMonth(oper.OperationDate);
            return oper;
        }
       
        [AbpAuthorize("Conta.MF.Intrari.Modificare")]
        public ImoAssetAddDirectDto SaveAssetDirect(ImoAssetAddDirectDto oper)
        {
            //verificare completare date
            if (!_operationRepository.VerifyClosedMonth(oper.OperationDate))
                throw new UserFriendlyException("Eroare", "Nu se poate inregistra operatia deoarece luna contabila este inchisa");

            if ((oper.AssetAccountId == null) || (oper.ExpenseAccountId == null) || (oper.DepreciationAccountId == null) || (oper.DurationInMonths == 0) || (oper.StorageInId == null))
            {
                throw new UserFriendlyException("Eroare adaugare mijloace fixe", "Trebuie sa selectati gestiunea, conturile corespunzatoare si sa completati durata de amortizare");
            }

            // stergere operatii contabile si gestiune pana la data specificata
            if (oper.ProcessedIn == true)
            {
                var imoStock = _imoAssetStockRepository.GetAll().Where(f => f.ImoAssetItemPFId == oper.Id && f.OperType == ImoAssetOperType.Intrare).FirstOrDefault();
                if (imoStock != null)
                {
                    _autoOperationRepository.DeleteUncheckedAssetOperation(oper.OperationDate, oper.Id);
                }

                var lastProcessedDateForAsset = _imoOperationRepository.LastProcessedDateForAsset(oper.Id);
                if (oper.OperationDate <= lastProcessedDateForAsset)
                {
                    try
                    {
                        _imoOperationRepository.GestDelComputingForAsset(oper.OperationDate, oper.Id);
                    }
                    catch (Exception ex)
                    {

                        throw new UserFriendlyException("Eroare", ex.Message);
                    }
                }
            }

            if (oper.ProcessedInUse == true)
            {
                var imoStock = _imoAssetStockRepository.GetAll().Where(f => f.ImoAssetItemPFId == oper.Id && f.OperType == ImoAssetOperType.PunereInFunctiune).FirstOrDefault();
                if (imoStock != null)
                {
                    _autoOperationRepository.DeleteUncheckedAssetOperation(oper.UseStartDate.Value, oper.Id);
                }

                var lastProcessedDateForAsset = _imoOperationRepository.LastProcessedDateForAsset(oper.Id);
                if (oper.OperationDate <= lastProcessedDateForAsset)
                {
                    try
                    {
                        _imoOperationRepository.GestDelComputingForAsset(oper.UseStartDate.Value, oper.Id);
                    }
                    catch (Exception ex)
                    {

                        throw new UserFriendlyException("Eroare", ex.Message);
                    }
                }
            }

            DateTime lastProcessedDate = _imoOperationRepository.LastProcessedDateForAsset(oper.Id);
            if (lastProcessedDate >= oper.OperationDate)
                throw new UserFriendlyException("Eroare data operatie", "Data operatiei nu poate fi mai mica decat data ultimei operatii procesate in gestiune " + lastProcessedDate.ToShortDateString());

            // iau urmatorul numar de inventar
            var assetList = _imoAssetRepository.GetAll().Where(f => f.State == State.Active);
            var nextInvNumber = 0;
            if (oper.Id == 0)
            {
                try
                {
                    var countAsset = assetList.Count();
                    if (countAsset == 0)
                    {
                        nextInvNumber = 1;
                    }
                    else
                    {
                        nextInvNumber = assetList.Max(f => f.InventoryNr) + 1;
                    }
                }
                catch
                {
                    nextInvNumber = 1;
                }
            }
            else
            {
                nextInvNumber = oper.InventoryNr;
            }

            var asset = new ImoAssetItem
            {
                Id = oper.Id,
                Name = oper.Name,
                InventoryValue = oper.InventoryValue,
                FiscalInventoryValue = oper.InventoryValue,
                AssetAccountId = oper.AssetAccountId,
                AssetAccountInUseId = oper.AssetAccountInUseId,
                DepreciationAccountId = oper.DepreciationAccountId,
                ExpenseAccountId = oper.ExpenseAccountId,
                Depreciation = oper.Depreciation,
                FiscalDepreciation = oper.FiscalDepreciation,
                InConservare = false,
                InStock = true,
                PriceUnit = oper.InventoryValue,
                //Processed = oper.Processed,
                ProcessedIn = false,
                ProcessedInUse = false,
                Quantity = oper.Quantity,
                OperationType = oper.OperationType,
                DocumentTypeId = oper.DocumentTypeId,
                DocumentNr = oper.DocumentNr,
                DocumentDate = oper.DocumentDate.Date,
                UseStartDate = oper.UseStartDate,
                DepreciationStartDate = oper.DepreciationStartDate,
                InventoryNr = nextInvNumber,
                AssetClassCodesId = oper.AssetClassCodesId,
                DurationInMonths = oper.DurationInMonths,
                RemainingDuration = oper.RemainingDuration,
                MonthlyDepreciation = oper.MonthlyDepreciation,
                MonthlyFiscalDepreciation = oper.MonthlyFiscalDepreciation,
                InvoiceDetailsId = oper.InvoiceDetailsId,
                OperationDate = oper.OperationDate.Date,
                ImoAssetStorageId = oper.StorageInId,
                State = State.Active,
                PrimDocumentTypeId = oper.PrimDocumentTypeId,
                PrimDocumentNr = oper.PrimDocumentNr,
                PrimDocumentDate = oper.PrimDocumentDate,
                ThirdPartyId = oper.ThirdPartyId,
                ImoAssetType = (ImoAssetType)oper.AssetType
            };

            var appClient = GetCurrentTenant();
            try
            {
                if (asset.Id == 0)
                {
                    _imoAssetRepository.Insert(asset);
                }
                else
                {
                    asset.TenantId = appClient.Id;
                    _imoAssetRepository.UpdateAsset(asset);
                }
                CurrentUnitOfWork.SaveChanges();

                var lastBalanceDate = _balanceRepository.GetAll().Where(f => f.Status == State.Active)
                .OrderByDescending(f => f.BalanceDate)
                .FirstOrDefault().BalanceDate;

                // daca data dare in folosinta este completata => generez gestiunea
                if (asset.UseStartDate != null)
                {
                    GestComputeForAssetIn(asset.OperationDate, asset.Id, asset.ImoAssetType);

                    GestComputeForAssetInUse(asset.UseStartDate.Value, asset.Id, asset.ImoAssetType);
                }
                else
                {
                    GestComputeForAssetIn(asset.OperationDate, asset.Id, asset.ImoAssetType);
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare adaugare Mijloc fix", ex.Message);
            }

            oper.FinishAdd = true;

            return oper;
        }
     
        [AbpAuthorize("Conta.MF.Intrari.Modificare")]
        public void DeleteAsset(int assetId)
        {
            var operCount = _imoOperationRepository.GetAllIncluding(f => f.OperDetails).Where(f => f.State == State.Active && f.OperDetails.Any(g => g.ImoAssetItemId == assetId)).Count();
            if (operCount != 0)
            {
                throw new UserFriendlyException("Eroare", "Nu puteti sa stergeti aceasta inregistrare, deoarece exista operatii definite pentru acest mijloc fix");
            }

            var asset = _imoAssetRepository.FirstOrDefault(f => f.Id == assetId);
            asset.State = State.Inactive;
        }
      
        [AbpAuthorize("Conta.MF.Intrari.Modificare")]
        public void SaveAssetInUse(ImoAssetAddDirectDto oper)
        {
            if (!_operationRepository.VerifyClosedMonth(oper.UseStartDate.Value))
                throw new UserFriendlyException("Eroare", "Nu se poate inregistra operatia deoarece luna contabila este inchisa");

            var asset = _imoAssetRepository.Get(oper.Id);

            asset.UseStartDate = oper.UseStartDate;
            asset.DepreciationStartDate = oper.DepreciationStartDate;
            asset.AssetAccountInUseId = oper.AssetAccountInUseId;

            try
            {
                _imoAssetRepository.UpdateAsset(asset);
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare adaugare Mijloc fix", ex.Message);
            }

            //calculez gestiune mijloc fix
            try
            {
                _imoOperationRepository.GestComputingForAsset(asset.UseStartDate.Value, asset.Id);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare adaugare data dare in folosinta pentru Mijloc fix"); ;
            }

            try
            {
                var lastBalanceDate = _balanceRepository.GetAll().Where(f => f.Status == State.Active)
                             .OrderByDescending(f => f.BalanceDate)
                             .FirstOrDefault().BalanceDate;
                var appClient = GetCurrentTenant();
                _autoOperationRepository.AutoImoAssetOperationAdd(asset.UseStartDate.Value, appClient.Id, appClient.LocalCurrencyId.Value, asset.ImoAssetType, lastBalanceDate, null);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }
      
        [AbpAuthorize("Conta.MF.Intrari.Modificare")]
        public ImoAssetAddDirectDto AddInUseChangeDate(ImoAssetAddDirectDto oper)
        {
            try
            {
                oper.DepreciationStartDate = LazyMethods.FirstDayNextMonth(oper.UseStartDate.Value);
                return oper;
            }
            catch (Exception)
            {
                throw new UserFriendlyException("Data darii in folosinta trebuie completata");
            }

        }
    }
}
