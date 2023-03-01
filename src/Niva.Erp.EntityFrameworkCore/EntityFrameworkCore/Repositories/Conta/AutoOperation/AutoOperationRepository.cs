using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Niva.EntityFramework.Repositories.Nomenclatures;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Managers;
using Niva.Erp.Models.AutoOperation;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Contracts;
using Niva.Erp.Models.Deconturi;
using Niva.Erp.Models.Economic.Casierii;
using Niva.Erp.Models.ImoAsset;
using Niva.Erp.Models.Imprumuturi;
using Niva.Erp.Models.InvObjects;
using Niva.Erp.Models.PrePayments;
using Niva.Erp.Repositories.Conta.AutoOperation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.EntityFrameworkCore.Repositories.Conta.AutoOperation
{
    public class AutoOperationRepository : ErpRepositoryBase<AutoOperationOper, int>, IAutoOperationRepository
    {

        AccountRepository _accountRepository;
        ExchangeRatesRepository _exchangeRatesRepository;
        CurrencyRepository _currencyRepository;
        BalanceRepository _balanceRepository;

        public AutoOperationRepository(IDbContextProvider<ErpDbContext> context) : base(context)
        {
            _accountRepository = new AccountRepository(context);
            _exchangeRatesRepository = new ExchangeRatesRepository(context);
            _currencyRepository = new CurrencyRepository(context);
            _balanceRepository = new BalanceRepository(context);
        }

        #region ImoAsset
        private int? StorageImoAssetDate(int imoAssetId, DateTime operationDate)
        {
            int? ret = null;

            var stock = Context.ImoAssetStock.Where(f => f.ImoAssetItemId == imoAssetId && f.StockDate <= operationDate)
                                             .OrderByDescending(f => f.StockDate)
                                             .FirstOrDefault();

            if (stock != null)
            {
                ret = stock.StorageId;
            }

            return ret;
        }

        public IQueryable<AutoOperationCompute> ImoAssetPrepareAdd(DateTime startDate, DateTime endDate, int appClientId)
        {
            var currency = Context.Account.Include(f => f.Currency).Where(f => f.TenantId == appClientId).Select(f => f.Currency).FirstOrDefault();
            var processedOper = Context.AutoOperationDetail.Include(f => f.AutoOper)
                                       .Where(f => f.AutoOper.AutoOperType == AutoOperationType.MijloaceFixe && f.AutoOper.OperationDate >= startDate && f.AutoOper.OperationDate <= endDate)
                                       .Select(f => f.OperationalId)
                                       .ToList();

            var exceptOperContare = new List<ImoAssetOperType>
            {
                ImoAssetOperType.IntrareInConservare,
                ImoAssetOperType.IesireDinConservare,
                ImoAssetOperType.Transfer,
                ImoAssetOperType.BonMiscare
            };

            var list = Context.ImoAssetStock.Include(f => f.ImoAssetItem)
                                            .Include(f => f.ImoAssetItem.AssetAccount)
                                            .Include(f => f.ImoAssetOperDet)
                                            .Include(f => f.ImoAssetOperDet.ImoAssetOper)
                                            .Include(f => f.ImoAssetOperDet.ImoAssetOper.DocumentType)
                                            .Include(f => f.Storage)
                                            .Include(f => f.ImoAssetOperDet.ImoAssetOper.AssetsStoreOut)
                                            .Where(f => f.StockDate >= startDate && f.StockDate <= endDate
                                                    //&& f.OperType == ImoAssetOperType.PunereInFunctiune
                                                    && (f.TranzInventoryValue != 0 || f.TranzDeprec != 0)
                                                    && !processedOper.Contains(f.Id))
                                            .Where(f => !exceptOperContare.Contains(f.OperType))
                                            // .ToList()
                                            .Select(f => new AutoOperationCompute
                                            {
                                                OperationId = (f.OperType != ImoAssetOperType.PunereInFunctiune && f.OperType != ImoAssetOperType.Intrare && f.OperType != ImoAssetOperType.AmortizareLunara) ? f.ImoAssetOperDet.ImoAssetOperId : (int?)null,
                                                OperationDetailId = f.ImoAssetOperDetId,
                                                AssetAccountId = f.ImoAssetItem.AssetAccountId,
                                                AssetAccountInUseId = f.ImoAssetItem.AssetAccountInUseId,
                                                GestId = f.Id,
                                                OperationDate = f.StockDate,
                                                DocumentTypeId = (f.OperType == ImoAssetOperType.AmortizareLunara)
                                                                  ? (int?)null
                                                                  : ((f.OperType == ImoAssetOperType.PunereInFunctiune || f.OperType == ImoAssetOperType.Intrare) ? f.ImoAssetItem.DocumentTypeId : f.ImoAssetOperDet.ImoAssetOper.DocumentTypeId),
                                                DocumentType = (f.OperType == ImoAssetOperType.AmortizareLunara)
                                                                  ? ""
                                                                  : ((f.OperType == ImoAssetOperType.PunereInFunctiune || f.OperType == ImoAssetOperType.Intrare) ? f.ImoAssetItem.DocumentType.TypeNameShort : f.ImoAssetOperDet.ImoAssetOper.DocumentType.TypeNameShort),
                                                DocumentNr = (f.OperType == ImoAssetOperType.AmortizareLunara)
                                                                  ? null
                                                                  : ((f.OperType == ImoAssetOperType.PunereInFunctiune || f.OperType == ImoAssetOperType.Intrare) ? f.ImoAssetItem.DocumentNr.ToString() : f.ImoAssetOperDet.ImoAssetOper.DocumentNr.ToString()),
                                                DocumentDate = (f.OperType == ImoAssetOperType.AmortizareLunara)
                                                                  ? (DateTime?)null
                                                                  : ((f.OperType == ImoAssetOperType.PunereInFunctiune || f.OperType == ImoAssetOperType.Intrare) ? f.ImoAssetItem.DocumentDate : f.ImoAssetOperDet.ImoAssetOper.DocumentDate),
                                                CurrencyId = currency.Id,
                                                Currency = currency.CurrencyCode,
                                                OperationTypeId = (int)f.OperType,
                                                OperationType = f.OperType.ToString(),
                                                ItemId = f.ImoAssetItemId,
                                                ItemName = f.ImoAssetItem.Name,
                                                StorageOutId = (f.OperType != ImoAssetOperType.PunereInFunctiune && f.OperType != ImoAssetOperType.Intrare && f.OperType != ImoAssetOperType.AmortizareLunara) ? f.ImoAssetOperDet.ImoAssetOper.AssetsStoreOutId : (int?)null,
                                                StorageOut = (f.OperType != ImoAssetOperType.PunereInFunctiune && f.OperType != ImoAssetOperType.Intrare && f.OperType != ImoAssetOperType.AmortizareLunara) ? f.ImoAssetOperDet.ImoAssetOper.AssetsStoreOut.StorageName : null,
                                                StorageInId = f.StorageId,
                                                StorageIn = f.Storage.StorageName,
                                                InventoryValue = f.TranzInventoryValue,
                                                DepreciationValue = f.TranzDeprec,
                                                AccountSort = (f.ImoAssetItem.AssetAccount == null) ? "" : f.ImoAssetItem.AssetAccount.Symbol
                                            }).ToList()
            .OrderBy(f => f.OperationDate).ThenBy(f => f.AccountSort).AsQueryable();
            return /*(IQueryable<AutoOperationCompute>)*/list;
        }

        public void ImoAssetOperationAdd(List<AutoOperationCompute> list, DateTime lastBalanceDate, AutoOperationType autoOperationType, int localCurrencyId, int? operGenId)
        {
            try
            {
                var dateList = list.Select(f => new { OperDate = f.OperationDate }).Distinct().OrderBy(f => f.OperDate);
                // pentru toate datele
                foreach (var dateItem in dateList)
                {
                    if (dateItem.OperDate <= lastBalanceDate)
                    {
                        throw new Exception("Nu puteti sa generati note contabile pentru data " + LazyMethods.DateToString(dateItem.OperDate) + " deoarece luna contabila este inchisa");
                    }

                    var operTypeList = list.Where(f => f.OperationDate == dateItem.OperDate).Select(f => new { OperationTypeId = f.OperationTypeId, OperationType = (ImoAssetOperType)f.OperationTypeId }).Distinct();
                    //pentru toate tipurile de operatii
                    foreach (var operTypeItem in operTypeList)
                    {
                        var monog = Context.AutoOperationConfig
                                           .Where(f => f.AutoOperType == AutoOperationType.MijloaceFixe)
                                           .Where(f => f.State == State.Active && dateItem.OperDate >= f.StartDate && dateItem.OperDate <= (f.EndDate ?? dateItem.OperDate)
                                                  && f.OperationType == operTypeItem.OperationTypeId)
                                           .OrderBy(f => f.EntryOrder)
                                           .ToList();
                        if (monog.Count == 0)
                        {
                            throw new Exception("Nu exista o monografie definita pentru aceasta operatie");
                        }
                        var individualOper = monog.FirstOrDefault().IndividualOperation;

                        var listToDB = new List<AutoOperationCompute>();

                        var operList = list.Where(f => f.OperationDate == dateItem.OperDate && f.OperationTypeId == operTypeItem.OperationTypeId).ToList();

                        if (individualOper) // daca e operatie individuala => inregistrez operatie cu operatie, altfel e o singura operatie cu multiple detalii
                        {
                            foreach (var operItem in operList)
                            {
                                if ((ImoAssetOperType)operItem.OperationTypeId == ImoAssetOperType.PunereInFunctiune && operItem.AssetAccountId == operItem.AssetAccountInUseId)
                                {

                                }
                                else
                                {
                                    listToDB.Add(operItem);
                                    OperationAddByType(listToDB, (int)operTypeItem.OperationType, monog, autoOperationType, localCurrencyId, operGenId);
                                    listToDB.Clear();
                                }
                            }
                        }
                        else
                        {
                            if (operTypeItem.OperationType == ImoAssetOperType.AmortizareLunara)
                            {
                                foreach (var operItem in operList)
                                {
                                    listToDB.Add(operItem);
                                }
                                OperationAddByType(listToDB, (int)operTypeItem.OperationType, monog, autoOperationType, localCurrencyId, operGenId);
                                listToDB.Clear();
                            }
                            else
                            {
                                foreach (var operItem in operList)
                                {
                                    if ((ImoAssetOperType)operItem.OperationTypeId == ImoAssetOperType.PunereInFunctiune && operItem.AssetAccountId == operItem.AssetAccountInUseId)
                                    {

                                    }
                                    else
                                    {
                                        listToDB.Add(operItem);
                                        OperationAddByType(listToDB, (int)operTypeItem.OperationType, monog, autoOperationType, localCurrencyId, operGenId);
                                        listToDB.Clear();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void AutoImoAssetOperationAdd(DateTime dataEnd, int appClientId, int localCurrencyId, ImoAssetType imoAssetType, DateTime lastBalanceDate, int? operGenId)
        {
            var imoAssetQr = AutoImoAssetPrepareAdd(dataEnd, localCurrencyId, appClientId);
           
            var imoAssetList = imoAssetQr.ToList();

            AutoOperationType autoOperType;
            if (imoAssetType == ImoAssetType.MijlocFix)
            {
                autoOperType = AutoOperationType.MijloaceFixe;
                ImoAssetOperationAdd(imoAssetList, lastBalanceDate, autoOperType, localCurrencyId, operGenId);
            }
        }

        public IQueryable<AutoOperationCompute> AutoImoAssetPrepareAdd(DateTime endDate, int localCurrencyId, int appClientId)
        {
            var nextDayLastBalance = Context.Balance.Where(f => f.TenantId == appClientId && f.Status == State.Active).Max(f => f.BalanceDate).AddDays(1);

            var currency = Context.Account.Include(f => f.Currency).Where(f => f.TenantId == appClientId).Select(f => f.Currency).FirstOrDefault();
            var processedOper = Context.AutoOperationDetail.Include(f => f.AutoOper)
                                       .Where(f => f.AutoOper.AutoOperType == AutoOperationType.MijloaceFixe && f.AutoOper.OperationDate >= nextDayLastBalance && f.AutoOper.OperationDate <= endDate)
                                       .Select(f => f.OperationalId)
                                       .ToList();
            var exceptOperContare = new List<ImoAssetOperType>();
            exceptOperContare.Add(ImoAssetOperType.IntrareInConservare);
            exceptOperContare.Add(ImoAssetOperType.IesireDinConservare);
            exceptOperContare.Add(ImoAssetOperType.Transfer);
            exceptOperContare.Add(ImoAssetOperType.BonMiscare);

            var list = Context.ImoAssetStock.AsNoTracking()
                                            .Include(f => f.ImoAssetItem)
                                            .Include(f => f.ImoAssetItem.AssetAccount)
                                            .Include(f => f.ImoAssetItem.DocumentType)
                                            .Include(f => f.ImoAssetItem.PrimDocumentType)
                                            .Include(f => f.ImoAssetOperDet)
                                            .Include(f => f.ImoAssetOperDet.ImoAssetOper)
                                            .Include(f => f.ImoAssetOperDet.ImoAssetOper.DocumentType)
                                            .Include(f => f.Storage)
                                            .Include(f => f.ImoAssetOperDet.ImoAssetOper.AssetsStoreOut)
                                            .Include(f => f.ImoAssetItem.ThirdParty)
                                            .Include(f => f.ImoAssetItem.InvoiceDetails)
                                            .ThenInclude(f => f.Invoices)
                                            .Include(f => f.ImoAssetItem.InvoiceDetails)
                                            .ThenInclude(f => f.InvoiceElementsDetails)
                                            .Where(f => f.StockDate >= nextDayLastBalance && f.StockDate <= endDate
                                                    && (f.OperType != ImoAssetOperType.BonMiscare && f.OperType != ImoAssetOperType.Transfer)
                                                    && (f.OperType == ImoAssetOperType.PunereInFunctiune ? (f.ImoAssetItem.AssetAccountId != f.ImoAssetItem.AssetAccountInUseId) : true)
                                                    && !processedOper.Contains(f.Id))
                                            .Where(f => !exceptOperContare.Contains(f.OperType))
                                            .Select(f => new AutoOperationCompute
                                            {
                                                OperationId = (f.OperType != ImoAssetOperType.PunereInFunctiune && f.OperType != ImoAssetOperType.Intrare && f.OperType != ImoAssetOperType.AmortizareLunara) ? f.ImoAssetOperDet.ImoAssetOperId : (int?)null,
                                                OperationDetailId = f.ImoAssetOperDetId,
                                                AssetAccountId = f.ImoAssetItem.AssetAccountId,
                                                AssetAccountInUseId = f.ImoAssetItem.AssetAccountInUseId,
                                                GestId = f.Id,
                                                OperationDate = f.StockDate,
                                                DocumentTypeId = (f.OperType == ImoAssetOperType.AmortizareLunara)
                                                                  ? (int?)null
                                                                  : ((f.OperType == ImoAssetOperType.PunereInFunctiune || f.OperType == ImoAssetOperType.Intrare) ? f.ImoAssetItem.PrimDocumentTypeId : f.ImoAssetOperDet.ImoAssetOper.DocumentTypeId),
                                                DocumentType = (f.OperType == ImoAssetOperType.AmortizareLunara)
                                                                  ? ""
                                                                  : ((f.OperType == ImoAssetOperType.PunereInFunctiune || f.OperType == ImoAssetOperType.Intrare) ? f.ImoAssetItem.PrimDocumentType.TypeNameShort : f.ImoAssetOperDet.ImoAssetOper.DocumentType.TypeNameShort),
                                                DocumentNr = (f.OperType == ImoAssetOperType.AmortizareLunara)
                                                                  ? null
                                                                  : ((f.OperType == ImoAssetOperType.PunereInFunctiune || f.OperType == ImoAssetOperType.Intrare) ? f.ImoAssetItem.PrimDocumentNr.ToString() : f.ImoAssetOperDet.ImoAssetOper.DocumentNr.ToString()),
                                                DocumentDate = (f.OperType == ImoAssetOperType.AmortizareLunara)
                                                                  ? (DateTime?)null
                                                                  : ((f.OperType == ImoAssetOperType.PunereInFunctiune || f.OperType == ImoAssetOperType.Intrare) ? f.ImoAssetItem.PrimDocumentDate : f.ImoAssetOperDet.ImoAssetOper.DocumentDate),
                                                CurrencyId = currency.Id,
                                                Currency = currency.CurrencyCode,
                                                OperationTypeId = (int)f.OperType,
                                                OperationType = f.OperType.ToString(),
                                                ItemId = f.ImoAssetItemId,
                                                ItemName = f.ImoAssetItem.Name,
                                                ItemInventoryNumber = f.ImoAssetItem.InventoryNr,
                                                StorageOutId = (f.OperType != ImoAssetOperType.PunereInFunctiune && f.OperType != ImoAssetOperType.Intrare && f.OperType != ImoAssetOperType.AmortizareLunara) ? f.ImoAssetOperDet.ImoAssetOper.AssetsStoreOutId : (int?)null,
                                                StorageOut = (f.OperType != ImoAssetOperType.PunereInFunctiune && f.OperType != ImoAssetOperType.Intrare && f.OperType != ImoAssetOperType.AmortizareLunara) ? f.ImoAssetOperDet.ImoAssetOper.AssetsStoreOut.StorageName : null,
                                                StorageInId = f.StorageId,
                                                StorageIn = f.Storage.StorageName,
                                                RemainingInventoryValue = f.InventoryValue,
                                                TotalDepreciationValue = f.Deprec,
                                                InventoryValue = (f.OperType == ImoAssetOperType.PunereInFunctiune ? f.InventoryValue : f.TranzInventoryValue),
                                                DepreciationValue = (f.OperType == ImoAssetOperType.PunereInFunctiune == true ? f.Deprec : f.TranzDeprec),
                                                AccountSort = (f.ImoAssetItem.AssetAccount == null) ? "" : f.ImoAssetItem.AssetAccount.Symbol,
                                                Details = (f.ImoAssetItem.DocumentType.TypeNameShort == "FF" ? (f.ImoAssetItem.ThirdParty.FullName + ", " + f.ImoAssetItem.InvoiceDetails.Invoices.InvoiceSeries + "  " + f.ImoAssetItem.InvoiceDetails.Invoices.InvoiceNumber +
                                                                                                          " / " + f.ImoAssetItem.InvoiceDetails.Invoices.InvoiceDate.ToShortDateString() +
                                                                                                          ", " + f.ImoAssetItem.InvoiceDetails.InvoiceElementsDetails.Description +
                                                                                                          ((f.ImoAssetItem.InvoiceDetails.Invoices.StartDatePeriod != null && f.ImoAssetItem.InvoiceDetails.Invoices.EndDatePeriod != null) ? ", " + f.ImoAssetItem.InvoiceDetails.Invoices.StartDatePeriod.Value.ToShortDateString() + " - " +
                                                                                                                 f.ImoAssetItem.InvoiceDetails.Invoices.EndDatePeriod.Value.ToShortDateString() : null))
                                                                                                            : null)
                                            });
            return list.OrderBy(f => f.OperationDate);
        }

        // generare operatii contabile
        protected void OperationAddByType(List<AutoOperationCompute> list, int operType, List<AutoOperationConfig> monog, AutoOperationType autoOperationType, int localCurrencyId, int? operGenId)
        {
            try
            {
                int currencyId = list[0].CurrencyId;
                var autoOperation = new AutoOperationOper
                {
                    AutoOperType = autoOperationType,
                    OperationType = list[0].OperationTypeId,
                    OperationDate = list[0].OperationDate,
                    CurrencyId = currencyId,
                    Validated = false,
                    State = State.Active
                };

                if (list[0].DocumentTypeId == null)
                {
                    int documentTypeId = 0;

                    if (autoOperationType == AutoOperationType.MijloaceFixe)
                    {
                        var imoAssetOperType = (ImoAssetOperType)operType;
                        var documentType = Context.ImoAssetOperDocType.Include(f => f.DocumentType)
                                                                      .Where(f => f.OperType == imoAssetOperType)
                                                                      .FirstOrDefault();
                        if (documentType == null)
                        {
                            throw new Exception("Documentul nu a fost identificat. Verificati modulul Mijloace fixe -> Nomenclatoare");
                        }
                        documentTypeId = documentType.DocumentTypeId;
                        autoOperation.DocumentNumber = GetDocumentNextNumber(documentType.DocumentType, list[0].OperationDate);
                    }
                    else if (autoOperationType == AutoOperationType.CheltuieliInAvans || autoOperationType == AutoOperationType.VenituriInAvans)
                    {
                        var prepaymentOperType = (PrepaymentOperType)operType;
                        var documentType = Context.PrepaymentDocType.Include(f => f.DocumentType)
                                                                      .Where(f => f.OperType == prepaymentOperType)
                                                                      .FirstOrDefault();
                        documentTypeId = documentType.DocumentTypeId;
                        autoOperation.DocumentNumber = GetDocumentNextNumber(documentType.DocumentType, list[0].OperationDate);
                    }
                    else if (autoOperationType == AutoOperationType.ObiecteDeInventar)
                    {
                        var invObjectOperType = (InvObjectOperType)operType;
                        var documentType = Context.InvObjectOperDocType.Include(f => f.DocumentType)
                                                                      .Where(f => f.OperType == invObjectOperType)
                                                                      .FirstOrDefault();
                        documentTypeId = documentType.DocumentTypeId;
                        autoOperation.DocumentNumber = GetDocumentNextNumber(documentType.DocumentType, list[0].OperationDate);
                    }
                    autoOperation.DocumentTypeId = documentTypeId;
                    /* autoOperation.DocumentNumber = "";*/// GetDocumentNextNumber(appClientId, documentType.DocumentType, list[0].OperationDate);
                    autoOperation.DocumentDate = list[0].OperationDate;
                }
                else
                {
                    autoOperation.DocumentTypeId = list[0].DocumentTypeId ?? 0;
                    autoOperation.DocumentNumber = list[0].DocumentNr.ToString();
                    autoOperation.DocumentDate = list[0].DocumentDate ?? list[0].OperationDate;
                }

                var autoNumber = Context.DocumentType.FirstOrDefault(f => f.Id == autoOperation.DocumentTypeId).AutoNumber;
                int operDetailIndex = 1;

                var operation = new Operation
                {
                    CurrencyId = localCurrencyId,
                    OperationDate = list[0].OperationDate,
                    DocumentTypeId = autoOperation.DocumentTypeId,
                    DocumentNumber = autoOperation.DocumentNumber,
                    DocumentDate = autoOperation.DocumentDate,
                    OperationStatus = OperationStatus.Unchecked,
                    State = State.Active,
                    ExternalOperation = true,
                    OperGenerateId = operGenId
                };

                decimal exchangeRate = 1;
                if (localCurrencyId != currencyId)
                {
                    exchangeRate = _exchangeRatesRepository.GetExchangeRateForOper(list[0].OperationDate, currencyId, localCurrencyId);
                }

                var details = new List<AutoOperationDetail>();
                var detailList = new List<OperationDetails>();
                Context.SaveChanges();
                int prepThirdPartyDecontId = 0;
                int imoAssetThirdPartyDecontId = 0;

                foreach (var item in list)
                {
                    prepThirdPartyDecontId = 0;
                    if (autoOperationType == AutoOperationType.CheltuieliInAvans || autoOperationType == AutoOperationType.VenituriInAvans)
                    {
                        if ((PrepaymentOperType)item.OperationTypeId == PrepaymentOperType.Constituire)
                        {
                            var prepayment = Context.Prepayment.FirstOrDefault(f => f.Id == item.ItemId);

                            if (prepayment.InvoiceDetailsId != null)
                            {
                                var invoiceDetail = Context.InvoiceDetails.Include(f => f.Invoices).FirstOrDefault(f => f.Id == prepayment.InvoiceDetailsId);
                                decimal exchangeRateInvoice = 1;
                                if (invoiceDetail.Invoices.CurrencyId != localCurrencyId)
                                {
                                    exchangeRateInvoice = _exchangeRatesRepository.GetExchangeRateForOper(invoiceDetail.Invoices.InvoiceDate, invoiceDetail.Invoices.CurrencyId, localCurrencyId);
                                    currencyId = invoiceDetail.Invoices.CurrencyId;
                                }

                            }

                            monog = monog.Where(f => f.UnreceiveInvoice == prepayment.UnreceiveInvoice).ToList();

                            // daca am decont => trebuie sa folosesc la thirdParty contul persoanei care face decontul si nu tertul din facturi. Il determin si il prind mai departe
                            if (prepayment.InvoiceDetailsId != null)
                            {
                                var invoice = Context.InvoiceDetails.Include(f => f.Invoices).Where(f => f.Id == prepayment.InvoiceDetailsId).Select(f => f.Invoices).FirstOrDefault();
                                if (invoice.DecontId != null)
                                {
                                    var decont = Context.Decont.FirstOrDefault(f => f.Id == invoice.DecontId);
                                    var tipContThirdParty = (decont.DecontType == DecontType.Card ? AccountFuncType.DecontCard : AccountFuncType.DecontCasa);
                                    var decontAccount = Context.Account.Where(f => f.Status == State.Active && f.ThirdPartyId == decont.ThirdPartyId && f.AccountFuncType == tipContThirdParty
                                                                              && f.CurrencyId == decont.CurrencyId)
                                                                       .FirstOrDefault();
                                    if (decontAccount == null) // nu are analitic => iau sinteticul
                                    {
                                        decontAccount = Context.Account.Include(f => f.AnalyticAccounts)
                                                                       .Where(f => f.Status == State.Active && f.AccountFuncType == tipContThirdParty && f.AnalyticAccounts.Count != 0)
                                                                       .FirstOrDefault();
                                    }
                                    if (decontAccount == null) // daca nu a fost idetificat nici sinteticul -> afisez eroare
                                    {
                                        throw new Exception("Nu a fost identificat in planul de conturi un cont cu moneda si tipul decontului specific tertului");
                                    }
                                    prepThirdPartyDecontId = GetAutoAccountForDecont(decontAccount.Symbol, decont.ThirdPartyId, decont.DecontDate, decont.CurrencyId, decont.DecontType, null);
                                }

                            }

                        }
                    }

                    foreach (var monogItem in monog)
                    {
                        decimal value = 0;
                        int debitAccount = 0, creditAccount = 0;

                        if (autoOperationType == AutoOperationType.MijloaceFixe)
                        {
                            var assetItem = Context.ImoAssetItem.Include(f => f.InvoiceDetails).ThenInclude(f => f.InvoiceElementsDetails).Include(f => f.AssetAccountInUse).Include(f => f.InvoiceDetails.Invoices)
                                                                .ThenInclude(f => f.Decont).ThenInclude(f => f.ThirdParty).FirstOrDefault(f => f.Id == item.ItemId);

                            if ((ImoAssetOperType)autoOperation.OperationType == ImoAssetOperType.Intrare || (ImoAssetOperType)autoOperation.OperationType == ImoAssetOperType.PunereInFunctiune)
                            {
                                if (assetItem.InvoiceDetailsId != null) // MF adaugat porning de la factura
                                {
                                    // iau moneda din factura
                                    var invoice = assetItem.InvoiceDetails.Invoices;
                                    currencyId = invoice.CurrencyId;

                                    debitAccount = GetImoAssetAccountId(monogItem.OperationType == (int)ImoAssetOperType.PunereInFunctiune ? assetItem.AssetAccountInUse.Symbol :
                                                                 assetItem.InvoiceDetails.InvoiceElementsDetails.CorrespondentAccount, monogItem.OperationType, assetItem,
                                                                 autoOperation.OperationDate, localCurrencyId);
                                    if (invoice.DecontId != null)
                                    {
                                        var tipContThirdParty = (invoice.Decont.DecontType == DecontType.Card ? AccountFuncType.DecontCard : AccountFuncType.DecontCasa);
                                        var account = Context.Account.Where(f => f.Status == State.Active && f.ThirdPartyId == invoice.Decont.ThirdPartyId && f.AccountFuncType == tipContThirdParty && f.CurrencyId == invoice.Decont.CurrencyId)
                                                                             .FirstOrDefault();

                                        imoAssetThirdPartyDecontId = GetAutoAccountForDecont(account.Symbol, invoice.Decont.ThirdPartyId, invoice.Decont.DecontDate, invoice.Decont.CurrencyId, invoice.Decont.DecontType, null);

                                    }
                                }
                                else
                                {
                                    if (monogItem.OperationType == (int)ImoAssetOperType.PunereInFunctiune && assetItem.AssetAccountInUse == null)
                                        throw new Exception("Trebuie sa specificati cont mijlocului fix pentru puenrea in functiune");
                                    debitAccount = GetImoAssetAccountId(monogItem.OperationType == (int)ImoAssetOperType.PunereInFunctiune ? assetItem.AssetAccountInUse.Symbol
                                                                                                                         : monogItem.DebitAccount, monogItem.OperationType, assetItem, autoOperation.OperationDate, localCurrencyId);
                                }
                            }
                            else if ((ImoAssetOperType)autoOperation.OperationType == ImoAssetOperType.ModificareConturiCuInregistrareNotaContabila)
                            {
                                var operDetail = Context.ImoAssetOperDetail.Where(f => f.Id == item.OperationDetailId && f.ImoAssetOper.AssetsOperType == (ImoAssetOperType)operType && f.ImoAssetOper.OperationDate == autoOperation.OperationDate).FirstOrDefault();
                                if (monogItem.DebitAccount == "CN")
                                {
                                    debitAccount = ((ImoAssetStockElement)monogItem.ElementId == ImoAssetStockElement.ValoareInventar) ? operDetail.NewAssetAccountInUseId.Value : operDetail.NewDepreciationAccountId.Value;
                                }
                                else
                                {
                                    debitAccount = ((ImoAssetStockElement)monogItem.ElementId == ImoAssetStockElement.ValoareInventar) ? operDetail.OldAssetAccountInUseId.Value : operDetail.OldDepreciationAccountId.Value;
                                }

                                if (monogItem.CreditAccount == "CV")
                                {
                                    creditAccount = ((ImoAssetStockElement)monogItem.ElementId == ImoAssetStockElement.ValoareInventar) ? operDetail.OldAssetAccountInUseId.Value : operDetail.OldDepreciationAccountId.Value;
                                }
                                else
                                {
                                    creditAccount = ((ImoAssetStockElement)monogItem.ElementId == ImoAssetStockElement.ValoareInventar) ? operDetail.NewAssetAccountInUseId.Value : operDetail.NewDepreciationAccountId.Value;
                                }
                            }
                            else
                            {
                                debitAccount = GetImoAssetAccountId(monogItem.DebitAccount, monogItem.OperationType, assetItem, autoOperation.OperationDate, localCurrencyId);
                            }

                            if ((ImoAssetOperType)autoOperation.OperationType == ImoAssetOperType.Modernizare)
                            {
                                // preiau operatia de modernizare din ImoAssetOperDetail pentru a avea acces la furnizorul din operatie
                                var imoAssetOperDetail = Context.ImoAssetOperDetail
                                                            .Include(f => f.ImoAssetItem).Include(f => f.InvoiceDetail).ThenInclude(f => f.Invoices).ThenInclude(f => f.ThirdParty)
                                                            .FirstOrDefault(f => f.ImoAssetItemId == assetItem.Id && f.State == State.Active && f.TenantId == assetItem.TenantId);
                                //creditAccount = GetAutoAccountActivityType(monogItem.CreditAccount, assetItem.ThirdPartyId, null, autoOperation.OperationDate, currencyId, null);
                                creditAccount = GetAutoAccount(monogItem.CreditAccount, imoAssetOperDetail.InvoiceDetail.Invoices.ThirdPartyId/*assetItem.ThirdPartyId*/, autoOperation.OperationDate, currencyId, null);
                            }
                            else if ((ImoAssetOperType)autoOperation.OperationType == ImoAssetOperType.PunereInFunctiune)
                            {
                                creditAccount = assetItem.AssetAccountId.Value;
                            }
                            else if ((ImoAssetOperType)autoOperation.OperationType == ImoAssetOperType.ModificareConturiCuInregistrareNotaContabila)
                            {
                                var operDetail = Context.ImoAssetOperDetail.Where(f => f.Id == item.OperationDetailId && f.ImoAssetOper.AssetsOperType == (ImoAssetOperType)operType && f.ImoAssetOper.OperationDate == autoOperation.OperationDate).FirstOrDefault();
                                if (monogItem.DebitAccount == "CN")
                                {
                                    debitAccount = ((ImoAssetStockElement)monogItem.ElementId == ImoAssetStockElement.ValoareInventar) ? operDetail.NewAssetAccountInUseId.Value : operDetail.NewDepreciationAccountId.Value;
                                }
                                else
                                {
                                    debitAccount = ((ImoAssetStockElement)monogItem.ElementId == ImoAssetStockElement.ValoareInventar) ? operDetail.OldAssetAccountInUseId.Value : operDetail.OldDepreciationAccountId.Value;
                                }

                                if (monogItem.CreditAccount == "CV")
                                {
                                    creditAccount = ((ImoAssetStockElement)monogItem.ElementId == ImoAssetStockElement.ValoareInventar) ? operDetail.OldAssetAccountInUseId.Value : operDetail.OldDepreciationAccountId.Value;
                                }
                                else
                                {
                                    creditAccount = ((ImoAssetStockElement)monogItem.ElementId == ImoAssetStockElement.ValoareInventar) ? operDetail.NewAssetAccountInUseId.Value : operDetail.NewDepreciationAccountId.Value;
                                }
                            }
                            else if (monogItem.CreditAccount.IndexOf("3566") >= 0)
                            {
                                creditAccount = GetImoAssetAccountId(monogItem.CreditAccount, monogItem.OperationType, assetItem, autoOperation.OperationDate, currencyId);
                            }
                            else
                            {
                                creditAccount = GetImoAssetAccountId(monogItem.CreditAccount, monogItem.OperationType, assetItem, autoOperation.OperationDate, localCurrencyId);
                            }


                            if ((ImoAssetStockElement)monogItem.ElementId == ImoAssetStockElement.ValoareInventar)
                            {
                                if ((ImoAssetOperType)autoOperation.OperationType != ImoAssetOperType.ModificareConturiCuInregistrareNotaContabila)
                                    value = Math.Abs(item.InventoryValue);
                                else
                                    value = Math.Abs(item.RemainingInventoryValue);
                            }
                            else if ((ImoAssetStockElement)monogItem.ElementId == ImoAssetStockElement.Amortizare)
                            {
                                if ((ImoAssetOperType)autoOperation.OperationType != ImoAssetOperType.ModificareConturiCuInregistrareNotaContabila)
                                    value = Math.Abs(item.DepreciationValue);
                                else
                                    value = Math.Abs(item.TotalDepreciationValue);
                            }
                            else if ((ImoAssetStockElement)monogItem.ElementId == ImoAssetStockElement.ValoareVanzare)
                            {
                                if (item.OperationDetailId != null)
                                {
                                    var operDetail = Context.ImoAssetOperDetail.FirstOrDefault(f => f.Id == item.OperationDetailId);
                                    if (operDetail.InvoiceDetailId != null)
                                    {
                                        var invoiceDetail = Context.InvoiceDetails.FirstOrDefault(f => f.Id == operDetail.InvoiceDetailId);
                                        value = (invoiceDetail != null) ? (invoiceDetail.Value + invoiceDetail.VAT) : 0;
                                    }
                                }

                            }

                            if (imoAssetThirdPartyDecontId != 0)
                            {
                                creditAccount = imoAssetThirdPartyDecontId;
                            }
                        }
                        else if (autoOperationType == AutoOperationType.CheltuieliInAvans || autoOperationType == AutoOperationType.VenituriInAvans)
                        {
                            var prepayment = Context.Prepayment.Include(f => f.InvoiceDetails).Include(f => f.InvoiceDetails.Invoices).ThenInclude(f => f.ActivityType).FirstOrDefault(f => f.Id == item.ItemId);

                            if ((PrepaymentStockElement)monogItem.ElementId == PrepaymentStockElement.Valoare)
                            {
                                value = Math.Abs(item.InventoryValue);
                            }
                            else if ((PrepaymentStockElement)monogItem.ElementId == PrepaymentStockElement.Amortizare)
                            {
                                value = item.DepreciationValue;
                            }
                            else if ((PrepaymentStockElement)monogItem.ElementId == PrepaymentStockElement.ValoareTVA)
                            {
                                value = Math.Abs(item.VATValue);
                            }
                            else if ((PrepaymentStockElement)monogItem.ElementId == PrepaymentStockElement.AmortizareTVA)
                            {
                                value = item.DepreciationVAT;
                            }

                            debitAccount = GetPrepaymentAccountId(monogItem.DebitAccount, prepayment, autoOperation.OperationDate, localCurrencyId);
                            if (monogItem.CreditAccount.IndexOf("3566") >= 0)
                            {
                                creditAccount = GetPrepaymentAccountId(monogItem.CreditAccount, prepayment, autoOperation.OperationDate, currencyId);
                            }
                            else
                            {
                                creditAccount = GetPrepaymentAccountId(monogItem.CreditAccount, prepayment, autoOperation.OperationDate, localCurrencyId);
                            }
                            if (prepThirdPartyDecontId != 0)
                            {
                                creditAccount = prepThirdPartyDecontId;
                            }

                        }
                        else if (autoOperationType == AutoOperationType.ObiecteDeInventar)
                        {
                            var invObject = Context.InvObjectItem.Include(f => f.InvoiceDetails).ThenInclude(f => f.Invoices).Include(f => f.InvoiceDetails.Invoices).ThenInclude(f => f.Decont).FirstOrDefault(f => f.Id == item.ItemId);
                            Decont decont = null;
                            try
                            {
                                decont = Context.Decont.FirstOrDefault(f => f.Id == invObject.InvoiceDetails.Invoices.DecontId);
                            }
                            catch
                            {

                            }
                            if ((InvObjectOperType)autoOperation.OperationType == InvObjectOperType.Intrare)
                            {
                                if (invObject.InvoiceDetailsId != null)
                                {
                                    currencyId = invObject.InvoiceDetails.Invoices.CurrencyId;
                                }
                            }

                            if ((InvObjectStockElement)monogItem.ElementId == InvObjectStockElement.Valoare)
                            {
                                value = Math.Abs(item.InventoryValue);
                            }
                            else if ((InvObjectStockElement)monogItem.ElementId == InvObjectStockElement.ValoareDescarcata)
                            {

                            }

                            debitAccount = GetInvObjectAccountId(monogItem.DebitAccount, invObject, autoOperation.OperationDate, localCurrencyId);


                            if (monogItem.CreditAccount.IndexOf("3566") >= 0)
                            {
                                if ((InvObjectOperType)autoOperation.OperationType == InvObjectOperType.Modernizare)
                                {
                                    int thirdPartyId = 0;
                                    try
                                    {
                                        var gest = Context.InvObjectStock.FirstOrDefault(f => f.Id == item.GestId);
                                        var invOperDetId = gest.InvObjectOperDetId;
                                        var operDetail = Context.InvObjectOperDetail.Include(f => f.InvObjectOper).Include(f => f.InvoiceDetail).Include(f => f.InvoiceDetail.Invoices)
                                                                                    .FirstOrDefault(f => f.Id == invOperDetId);
                                        thirdPartyId = operDetail.InvoiceDetail.Invoices.ThirdPartyId.Value;
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new Exception("Nu am indentificat factura pentru operatiunea de modernizare");
                                    }
                                    creditAccount = GetAutoAccount(monogItem.CreditAccount, thirdPartyId, autoOperation.OperationDate, currencyId, null);
                                }
                                else
                                {
                                    if (decont != null)
                                    {
                                        // iau contul tertului
                                        var tipContThirdParty = (decont.DecontType == DecontType.Card ? AccountFuncType.DecontCard : AccountFuncType.DecontCasa);
                                        var account = Context.Account.Where(f => f.Status == State.Active && f.ThirdPartyId == decont.ThirdPartyId && f.AccountFuncType == tipContThirdParty && f.CurrencyId == decont.CurrencyId)
                                                                             .FirstOrDefault();
                                        if (account == null) // nu are analitic => iau sinteticul
                                        {
                                            account = Context.Account.Include(f => f.AnalyticAccounts)
                                                                     .Where(f => f.Status == State.Active && f.AccountFuncType == tipContThirdParty && f.AnalyticAccounts.Count != 0)
                                                                     .FirstOrDefault();
                                        }

                                        if (account == null) // daca nu a fost idetificat nici sinteticul -> afisez eroare
                                        {
                                            throw new Exception("Nu a fost identificat in planul de conturi un cont cu moneda si tipul decontului specific tertului");
                                        }
                                        creditAccount = GetAutoAccountForDecont(account.Symbol, decont.ThirdPartyId, autoOperation.OperationDate, decont.CurrencyId, decont.DecontType, null);
                                    }
                                    else
                                    {
                                        creditAccount = GetInvObjectAccountId(monogItem.CreditAccount, invObject, autoOperation.OperationDate, currencyId);
                                    }
                                }
                            }
                            else
                            {
                                creditAccount = GetInvObjectAccountId(monogItem.CreditAccount, invObject, autoOperation.OperationDate, localCurrencyId);
                            }
                        }

                        decimal valueCurr = 0;

                        if (value != 0)
                        {
                            var detail = new AutoOperationDetail
                            {
                                DebitAccountId = debitAccount,
                                CreditAccountId = creditAccount,
                                Value = value,
                                ValueCurr = valueCurr,
                                Details = monogItem.Details,
                                OperationalId = item.GestId ?? 0
                            };

                            var operationDetail = new OperationDetails();

                            operationDetail.ValueCurr = valueCurr;
                            operationDetail.Value = value;
                            operationDetail.VAT = 0;

                            if (autoOperationType == AutoOperationType.CheltuieliInAvans || autoOperationType == AutoOperationType.VenituriInAvans)
                            {
                                operationDetail.Details = item.Details ?? monogItem.Details;
                            }
                            else if (autoOperationType == AutoOperationType.MijloaceFixe)
                            {
                                operationDetail.Details = (operType == (int)ImoAssetOperType.Intrare || operType == (int)ImoAssetOperType.PunereInFunctiune) ?
                                                          (item.Details != null ? item.Details : monogItem.Details) :
                                                          ((monogItem.OperationType == (int)ImoAssetOperType.Casare) ? monogItem.Details + ", Nr. inv. " + item.ItemInventoryNumber + " / " + item.ItemName :
                                                          monogItem.Details + ", Nr. inv. " + item.ItemInventoryNumber + " / " + operation.OperationDate.ToShortDateString());
                            }
                            else if (autoOperationType == AutoOperationType.ObiecteDeInventar)
                            {
                                if (item.Details == null)
                                {
                                    operationDetail.Details = monogItem.Details + " - Numar inventar " + item.ItemInventoryNumber;
                                }
                                else if (item.Details.Replace(", ", "") == "")
                                {
                                    operationDetail.Details = monogItem.Details + " - Numar inventar " + item.ItemInventoryNumber;
                                }
                                else
                                {
                                    operationDetail.Details = item.Details;
                                }
                            }

                            operationDetail.DebitId = debitAccount;
                            operationDetail.CreditId = creditAccount;
                            if (autoNumber)
                            {
                                operationDetail.DetailNr = operDetailIndex;
                                operDetailIndex++;
                            }

                            // caut conturi de cheltuiala partial deductibile
                            int expenseAccount = 0;
                            var accountDebit = Context.Account.FirstOrDefault(f => f.Id == debitAccount);
                            if (accountDebit.Symbol.StartsWith("6"))
                            {
                                expenseAccount = accountDebit.Id;
                            }
                            var accountCredit = Context.Account.FirstOrDefault(f => f.Id == creditAccount);
                            if (accountCredit.Symbol.StartsWith("6"))
                            {
                                expenseAccount = accountCredit.Id;
                            }

                            if (expenseAccount != 0)
                            {
                                bool okAddNededOper = true;
                                var nededOperationDetail = GenerateNededOperDetail(ref operationDetail, list[0].OperationDate, expenseAccount, out okAddNededOper);
                                if (okAddNededOper)
                                {
                                    detailList.Add(nededOperationDetail);
                                    if (autoNumber)
                                    {
                                        nededOperationDetail.DetailNr = operDetailIndex;
                                        operDetailIndex++;
                                    }
                                    var nededDetail = new AutoOperationDetail
                                    {
                                        DebitAccountId = nededOperationDetail.DebitId,
                                        CreditAccountId = nededOperationDetail.CreditId,
                                        Value = nededOperationDetail.Value,
                                        ValueCurr = nededOperationDetail.ValueCurr,
                                        Details = monogItem.Details,
                                        OperationalId = item.GestId ?? 0
                                    };
                                    details.Add(nededDetail);
                                    nededDetail.OperationDetail = nededOperationDetail;

                                    detail.Value = operationDetail.Value;
                                    detail.ValueCurr = operationDetail.ValueCurr;
                                }

                            }

                            if (detail.DebitAccountId != detail.CreditAccountId)
                            {
                                details.Add(detail);

                                detailList.Add(operationDetail);
                                detail.OperationDetail = operationDetail;
                            }
                        }
                    }
                }
                autoOperation.OperationDetails = details;
                Context.AutoOperationOper.Add(autoOperation);
                operation.OperationsDetails = detailList;

                if (detailList.Count != 0)
                {
                    Context.Operations.Add(operation);
                    Context.SaveChanges();

                    OperationAddValuta(operation.Id, autoOperation.Id, localCurrencyId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void OperationAddValuta(int operationId, int autoOperationId, int localCurrencyId)
        {
            bool operValuta = false;
            int currencyId = localCurrencyId;

            var operation = Context.Operations.Include(f => f.OperationsDetails).ThenInclude(f => f.Debit).Include(f => f.OperationsDetails).ThenInclude(f => f.Credit)
                                              .FirstOrDefault(f => f.Id == operationId);
            var autoOperation = Context.AutoOperationOper.Include(f => f.OperationDetails).FirstOrDefault(f => f.Id == autoOperationId);

            // cheltuieli in avans
            if (autoOperation.AutoOperType == AutoOperationType.CheltuieliInAvans && (PrepaymentOperType)autoOperation.OperationType == PrepaymentOperType.Constituire)
            {
                foreach (var item in autoOperation.OperationDetails)
                {
                    var prepBalance = Context.PrepaymentBalance.FirstOrDefault(f => f.Id == item.OperationalId);
                    var prepayment = Context.Prepayment.Include(f => f.InvoiceDetails).FirstOrDefault(f => f.Id == prepBalance.PrepaymentId);
                    if (prepayment.InvoiceDetailsId != null)
                    {
                        var invoice = Context.InvoiceDetails.Include(f => f.Invoices).FirstOrDefault(f => f.Id == prepayment.InvoiceDetailsId).Invoices;
                        if (invoice.CurrencyId != localCurrencyId)
                        {
                            currencyId = invoice.CurrencyId;
                            operValuta = true;
                        }
                    }
                }

                if (operValuta) // daca am operatie in valuta
                {
                    var operationChild = new Operation
                    {
                        CurrencyId = currencyId,
                        OperationDate = operation.OperationDate,
                        DocumentTypeId = operation.DocumentTypeId,
                        DocumentNumber = operation.DocumentNumber,
                        DocumentDate = operation.DocumentDate,
                        OperationStatus = OperationStatus.Unchecked,
                        State = State.Active,
                        ExternalOperation = true,
                        OperationParentId = operation.Id
                    };
                    Context.Operations.Add(operationChild);
                    Context.SaveChanges();

                    foreach (var item in operation.OperationsDetails)
                    {
                        if (item.Credit.Symbol.IndexOf("3566") >= 0)
                        {
                            int thirdPartyAccountId = item.CreditId;
                            int corespAccountId = item.DebitId;

                            var activityTypeId = GetMainActivityType();
                            int pozitieSchimbAccountId = 0;
                            int contravaloarePozitieSchimbAccountId = 0;
                            // iau pozitia de schimb valutar
                            try
                            {

                                pozitieSchimbAccountId = _accountRepository.GetAll().FirstOrDefault(f => f.AccountFuncType == AccountFuncType.PozitieSchimbCheltuieli && f.CurrencyId == currencyId
                                                                                                    && f.ActivityTypeId == activityTypeId).Id;
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("Nu am identificat contul pentru pozitia de schimb valutar");
                            }
                            // iau contravaloarea pozitiei de schimb valutar
                            try
                            {
                                var denumireMoneda = _currencyRepository.GetCurrencyById(currencyId).CurrencyCode;
                                contravaloarePozitieSchimbAccountId = _accountRepository.GetAll().FirstOrDefault(f => f.AccountFuncType == AccountFuncType.ContravaloarePozitieSchimbCheltuieli
                                                                                                                 && f.ActivityTypeId == activityTypeId
                                                                                                                 && f.AccountName.IndexOf(denumireMoneda) >= 0).Id;
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("Nu am identificat contul pentru contravaloarea pozitiei de schimb valutar");
                            }

                            var autoOperDetail = Context.AutoOperationDetail.FirstOrDefault(f => f.OperationDetailId == item.Id);
                            var prepBalance = Context.PrepaymentBalance.FirstOrDefault(f => f.Id == autoOperDetail.OperationalId);
                            var prepayment = Context.Prepayment.FirstOrDefault(f => f.Id == prepBalance.PrepaymentId);
                            var invoiceDetail = Context.InvoiceDetails.Include(f => f.Invoices).FirstOrDefault(f => f.Id == prepayment.InvoiceDetailsId);
                            var exchangeRate = _exchangeRatesRepository.GetExchangeRateForOper(invoiceDetail.Invoices.InvoiceDate, currencyId, localCurrencyId);

                            var operationDetailChild = new OperationDetails
                            {
                                ValueCurr = 0,
                                Value = invoiceDetail.Value + invoiceDetail.VAT,
                                VAT = invoiceDetail.ProcVAT,
                                Details = item.Details,
                                OperationId = operationChild.Id,
                                DebitId = pozitieSchimbAccountId,
                                CreditId = thirdPartyAccountId
                            };

                            item.CreditId = contravaloarePozitieSchimbAccountId;
                            item.Details += ", Curs valutar: " + exchangeRate.ToString();

                            Context.OperationsDetails.Add(operationDetailChild);
                            Context.SaveChanges();
                        }
                    }
                }
            }
            // mijloace fixe
            else if (autoOperation.AutoOperType == AutoOperationType.MijloaceFixe)
            {
                if ((ImoAssetOperType)autoOperation.OperationType == ImoAssetOperType.Intrare)
                {
                    foreach (var item in autoOperation.OperationDetails)
                    {
                        var stock = Context.ImoAssetStock.FirstOrDefault(f => f.Id == item.OperationalId);
                        var asset = Context.ImoAssetItem.FirstOrDefault(f => f.Id == stock.ImoAssetItemId);
                        if (asset.InvoiceDetailsId != null)
                        {
                            var invoice = Context.InvoiceDetails.Include(f => f.Invoices).FirstOrDefault(f => f.Id == asset.InvoiceDetailsId).Invoices;
                            if (invoice.CurrencyId != localCurrencyId)
                            {
                                currencyId = invoice.CurrencyId;
                                operValuta = true;
                            }
                        }
                    }

                    if (operValuta) // daca am operatie in valuta
                    {
                        var operationChild = new Operation
                        {
                            CurrencyId = currencyId,
                            OperationDate = operation.OperationDate,
                            DocumentTypeId = operation.DocumentTypeId,
                            DocumentNumber = operation.DocumentNumber,
                            DocumentDate = operation.DocumentDate,
                            OperationStatus = OperationStatus.Unchecked,
                            State = State.Active,
                            ExternalOperation = true,
                            OperationParentId = operation.Id
                        };

                        Context.Operations.Add(operationChild);
                        Context.SaveChanges();

                        foreach (var item in operation.OperationsDetails)
                        {
                            if (item.Credit.Symbol.IndexOf("3566") >= 0)
                            {
                                int thirdPartyAccountId = item.CreditId;
                                int corespAccountId = item.DebitId;

                                var activityTypeId = GetMainActivityType();
                                int pozitieSchimbAccountId = 0;
                                int contravaloarePozitieSchimbAccountId = 0;
                                // iau pozitia de schimb valutar
                                try
                                {

                                    pozitieSchimbAccountId = _accountRepository.GetAll().FirstOrDefault(f => f.AccountFuncType == AccountFuncType.PozitieSchimbCheltuieli && f.CurrencyId == currencyId
                                                                                                        && f.ActivityTypeId == activityTypeId).Id;
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception("Nu am identificat contul pentru pozitia de schimb valutar");
                                }
                                // iau contravaloarea pozitiei de schimb valutar
                                try
                                {
                                    var denumireMoneda = _currencyRepository.GetCurrencyById(currencyId).CurrencyCode;
                                    contravaloarePozitieSchimbAccountId = _accountRepository.GetAll().FirstOrDefault(f => f.AccountFuncType == AccountFuncType.ContravaloarePozitieSchimbCheltuieli
                                                                                                                     && f.ActivityTypeId == activityTypeId
                                                                                                                     && f.AccountName.IndexOf(denumireMoneda) >= 0).Id;
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception("Nu am identificat contul pentru contravaloarea pozitiei de schimb valutar");
                                }

                                var autoOperDetail = Context.AutoOperationDetail.FirstOrDefault(f => f.OperationDetailId == item.Id);
                                var stock = Context.ImoAssetStock.FirstOrDefault(f => f.Id == autoOperDetail.OperationalId);
                                var asset = Context.ImoAssetItem.FirstOrDefault(f => f.Id == stock.ImoAssetItemId);
                                var invoiceDetail = Context.InvoiceDetails.Include(f => f.Invoices).FirstOrDefault(f => f.Id == asset.InvoiceDetailsId);
                                var exchangeRate = _exchangeRatesRepository.GetExchangeRateForOper(invoiceDetail.Invoices.InvoiceDate, currencyId, localCurrencyId);

                                var operationDetailChild = new OperationDetails
                                {
                                    ValueCurr = 0,
                                    Value = invoiceDetail.Value + invoiceDetail.VAT,
                                    VAT = invoiceDetail.ProcVAT,
                                    Details = item.Details,
                                    OperationId = operationChild.Id,
                                    DebitId = pozitieSchimbAccountId,
                                    CreditId = thirdPartyAccountId
                                };

                                item.CreditId = contravaloarePozitieSchimbAccountId;
                                item.Details += ", Curs valutar: " + exchangeRate.ToString();

                                Context.OperationsDetails.Add(operationDetailChild);
                                Context.SaveChanges();
                            }
                        }
                    }
                }
            }
            // obiecte de inventar
            else if (autoOperation.AutoOperType == AutoOperationType.ObiecteDeInventar)
            {
                if ((InvObjectOperType)autoOperation.OperationType == InvObjectOperType.Intrare)
                {
                    foreach (var item in autoOperation.OperationDetails)
                    {
                        var stock = Context.InvObjectStock.FirstOrDefault(f => f.Id == item.OperationalId);
                        var obj = Context.InvObjectItem.FirstOrDefault(f => f.Id == stock.InvObjectItemId);
                        if (obj.InvoiceDetailsId != null)
                        {
                            var invoice = Context.InvoiceDetails.Include(f => f.Invoices).FirstOrDefault(f => f.Id == obj.InvoiceDetailsId).Invoices;
                            if (invoice.CurrencyId != localCurrencyId)
                            {
                                currencyId = invoice.CurrencyId;
                                operValuta = true;
                            }
                        }
                    }

                    if (operValuta) // daca am operatie in valuta
                    {
                        var operationChild = new Operation
                        {
                            CurrencyId = currencyId,
                            OperationDate = operation.OperationDate,
                            DocumentTypeId = operation.DocumentTypeId,
                            DocumentNumber = operation.DocumentNumber,
                            DocumentDate = operation.DocumentDate,
                            OperationStatus = OperationStatus.Unchecked,
                            State = State.Active,
                            ExternalOperation = true,
                            OperationParentId = operation.Id
                        };

                        Context.Operations.Add(operationChild);
                        Context.SaveChanges();

                        foreach (var item in operation.OperationsDetails)
                        {
                            if (item.Credit.Symbol.IndexOf("3566") >= 0)
                            {
                                int thirdPartyAccountId = item.CreditId;
                                int corespAccountId = item.DebitId;

                                var activityTypeId = GetMainActivityType();
                                int pozitieSchimbAccountId = 0;
                                int contravaloarePozitieSchimbAccountId = 0;
                                // iau pozitia de schimb valutar
                                try
                                {

                                    pozitieSchimbAccountId = _accountRepository.GetAll().FirstOrDefault(f => f.AccountFuncType == AccountFuncType.PozitieSchimbCheltuieli && f.CurrencyId == currencyId
                                                                                                        && f.ActivityTypeId == activityTypeId).Id;
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception("Nu am identificat contul pentru pozitia de schimb valutar");
                                }
                                // iau contravaloarea pozitiei de schimb valutar
                                try
                                {
                                    var denumireMoneda = _currencyRepository.GetCurrencyById(currencyId).CurrencyCode;
                                    contravaloarePozitieSchimbAccountId = _accountRepository.GetAll().FirstOrDefault(f => f.AccountFuncType == AccountFuncType.ContravaloarePozitieSchimbCheltuieli
                                                                                                                     && f.ActivityTypeId == activityTypeId
                                                                                                                     && f.AccountName.IndexOf(denumireMoneda) >= 0).Id;
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception("Nu am identificat contul pentru contravaloarea pozitiei de schimb valutar");
                                }

                                var autoOperDetail = Context.AutoOperationDetail.FirstOrDefault(f => f.OperationDetailId == item.Id);
                                var stock = Context.InvObjectStock.FirstOrDefault(f => f.Id == autoOperDetail.OperationalId);
                                var obj = Context.InvObjectItem.FirstOrDefault(f => f.Id == stock.InvObjectItemId);
                                var invoiceDetail = Context.InvoiceDetails.Include(f => f.Invoices).FirstOrDefault(f => f.Id == obj.InvoiceDetailsId);
                                var exchangeRate = _exchangeRatesRepository.GetExchangeRateForOper(invoiceDetail.Invoices.InvoiceDate, currencyId, localCurrencyId);

                                var operationDetailChild = new OperationDetails
                                {
                                    ValueCurr = 0,
                                    Value = invoiceDetail.Value + invoiceDetail.VAT,
                                    VAT = invoiceDetail.ProcVAT,
                                    Details = item.Details,
                                    OperationId = operationChild.Id,
                                    DebitId = pozitieSchimbAccountId,
                                    CreditId = thirdPartyAccountId
                                };

                                item.CreditId = contravaloarePozitieSchimbAccountId;
                                item.Details += ", Curs valutar: " + exchangeRate.ToString();

                                Context.OperationsDetails.Add(operationDetailChild);
                                Context.SaveChanges();
                            }
                        }
                    }
                }
            }
        }

        // Generez nota contabila pentru fiecare chletuiala in parte
        //protected void OperationsAddByType(List<AutoOperationCompute> list, int operType, List<AutoOperationConfig> monog, AutoOperationType autoOperationType, int localCurrencyId)
        //{
        //    try
        //    {
        //        foreach (var item in list)
        //        {
        //            var autoOperation = new AutoOperationOper
        //            {
        //                AutoOperType = autoOperationType,
        //                OperationType = item.OperationTypeId,
        //                OperationDate = item.OperationDate,
        //                CurrencyId = item.CurrencyId,
        //                Validated = false,
        //                State = State.Active
        //            };

        //            if (item.DocumentTypeId == null)
        //            {
        //                int documentTypeId = 0;

        //                if (autoOperationType == AutoOperationType.MijloaceFixe)
        //                {
        //                    var imoAssetOperType = (ImoAssetOperType)operType;
        //                    var documentType = Context.ImoAssetOperDocType.Include(f => f.DocumentType)
        //                                                                  .Where(f => f.OperType == imoAssetOperType)
        //                                                                  .FirstOrDefault();
        //                    documentTypeId = documentType.DocumentTypeId;
        //                }
        //                else if (autoOperationType == AutoOperationType.CheltuieliInAvans || autoOperationType == AutoOperationType.VenituriInAvans)
        //                {
        //                    var prepaymentOperType = (PrepaymentOperType)operType;
        //                    var documentType = Context.PrepaymentDocType.Include(f => f.DocumentType)
        //                                                                  .Where(f => f.OperType == prepaymentOperType)
        //                                                                  .FirstOrDefault();
        //                    documentTypeId = documentType.DocumentTypeId;
        //                }
        //                autoOperation.DocumentTypeId = documentTypeId;
        //                autoOperation.DocumentNumber = "";
        //                autoOperation.DocumentDate = list[0].OperationDate;
        //            }
        //            else
        //            {
        //                autoOperation.DocumentTypeId = item.DocumentTypeId ?? 0;
        //                autoOperation.DocumentNumber = item.DocumentNr.ToString();
        //                autoOperation.DocumentDate = item.DocumentDate ?? item.OperationDate;
        //            }

        //            var autoNumber = Context.DocumentType.FirstOrDefault(f => f.Id == autoOperation.DocumentTypeId).AutoNumber;
        //            int operDetailIndex = 1;

        //            var operation = new Operation
        //            {
        //                CurrencyId = item.CurrencyId,
        //                OperationDate = item.OperationDate,
        //                DocumentTypeId = autoOperation.DocumentTypeId,
        //                DocumentNumber = autoOperation.DocumentNumber,
        //                DocumentDate = autoOperation.DocumentDate,
        //                OperationStatus = OperationStatus.Unchecked,
        //                State = State.Active,
        //                ExternalOperation = true
        //            };

        //            decimal exchangeRate = 1;
        //            if (localCurrencyId != item.CurrencyId)
        //            {
        //                exchangeRate = Context.ExchangeRates.Where(f => f.CurrencyId == item.CurrencyId && f.ExchangeDate < item.OperationDate) // se ia cursul din data - 1
        //                                                           .OrderByDescending(f => f.ExchangeDate)
        //                                                           .FirstOrDefault().Value;
        //            }

        //            var details = new List<AutoOperationDetail>();
        //            var detailList = new List<OperationDetails>();
        //            Context.SaveChanges();

        //            if (autoOperationType == AutoOperationType.CheltuieliInAvans || autoOperationType == AutoOperationType.VenituriInAvans)
        //            {
        //                if ((PrepaymentOperType)item.OperationTypeId == PrepaymentOperType.Constituire)
        //                {
        //                    var prepayment = Context.Prepayment.FirstOrDefault(f => f.Id == item.ItemId);

        //                    if (prepayment.InvoiceDetailsId != null)
        //                    {
        //                        var invoiceDetail = Context.InvoiceDetails.Include(f => f.Invoices).FirstOrDefault(f => f.Id == prepayment.InvoiceDetailsId);
        //                        decimal exchangeRateInvoice = 1;
        //                        if (invoiceDetail.Invoices.CurrencyId != localCurrencyId)
        //                        {
        //                            exchangeRateInvoice = Context.ExchangeRates.Where(f => f.CurrencyId == invoiceDetail.Invoices.CurrencyId && f.ExchangeDate < invoiceDetail.Invoices.InvoiceDate) // se ia cursul din data - 1
        //                                                       .OrderByDescending(f => f.ExchangeDate)
        //                                                       .FirstOrDefault().Value;
        //                        }

        //                    }

        //                    monog = monog.Where(f => f.UnreceiveInvoice == prepayment.UnreceiveInvoice).ToList();

        //                }
        //            }

        //            foreach (var monogItem in monog)
        //            {
        //                decimal value = 0;
        //                int debitAccount = 0, creditAccount = 0;

        //                if (autoOperationType == AutoOperationType.MijloaceFixe)
        //                {
        //                    var assetItem = Context.ImoAssetItem.FirstOrDefault(f => f.Id == item.ItemId);
        //                    debitAccount = GetImoAssetAccountId(monogItem.DebitAccount, assetItem, autoOperation.OperationDate, localCurrencyId);
        //                    creditAccount = GetImoAssetAccountId(monogItem.CreditAccount, assetItem, autoOperation.OperationDate, localCurrencyId);


        //                    if ((ImoAssetStockElement)monogItem.ElementId == ImoAssetStockElement.ValoareInventar)
        //                    {
        //                        value = Math.Abs(item.InventoryValue);
        //                    }
        //                    else if ((ImoAssetStockElement)monogItem.ElementId == ImoAssetStockElement.Amortizare)
        //                    {
        //                        value = Math.Abs(item.DepreciationValue);
        //                    }
        //                    else if ((ImoAssetStockElement)monogItem.ElementId == ImoAssetStockElement.ValoareVanzare)
        //                    {
        //                        if (item.OperationDetailId != null)
        //                        {
        //                            var operDetail = Context.ImoAssetOperDetail.FirstOrDefault(f => f.Id == item.OperationDetailId);
        //                            if (operDetail.InvoiceDetailId != null)
        //                            {
        //                                var invoiceDetail = Context.InvoiceDetails.FirstOrDefault(f => f.Id == operDetail.InvoiceDetailId);
        //                                value = (invoiceDetail != null) ? (invoiceDetail.Value + invoiceDetail.VAT) : 0;
        //                            }
        //                        }

        //                    }
        //                }
        //                else if (autoOperationType == AutoOperationType.CheltuieliInAvans || autoOperationType == AutoOperationType.VenituriInAvans)
        //                {
        //                    var prepayment = Context.Prepayment.FirstOrDefault(f => f.Id == item.ItemId);


        //                    if ((PrepaymentStockElement)monogItem.ElementId == PrepaymentStockElement.Valoare)
        //                    {
        //                        value = Math.Abs(item.InventoryValue);
        //                    }
        //                    else if ((PrepaymentStockElement)monogItem.ElementId == PrepaymentStockElement.Amortizare)
        //                    {
        //                        value = item.DepreciationValue;
        //                    }
        //                    else if ((PrepaymentStockElement)monogItem.ElementId == PrepaymentStockElement.ValoareTVA)
        //                    {
        //                        value = Math.Abs(item.VATValue);
        //                    }
        //                    else if ((PrepaymentStockElement)monogItem.ElementId == PrepaymentStockElement.AmortizareTVA)
        //                    {
        //                        value = item.DepreciationVAT;
        //                    }

        //                    debitAccount = GetPrepaymentAccountId(monogItem.DebitAccount, prepayment, autoOperation.OperationDate, localCurrencyId);
        //                    creditAccount = GetPrepaymentAccountId(monogItem.CreditAccount, prepayment, autoOperation.OperationDate, localCurrencyId);

        //                }

        //                decimal valueCurr = 0;

        //                if (value != 0)
        //                {
        //                    var detail = new AutoOperationDetail
        //                    {
        //                        DebitAccountId = debitAccount,
        //                        CreditAccountId = creditAccount,
        //                        Value = value,
        //                        ValueCurr = valueCurr,
        //                        Details = monogItem.Details,
        //                        OperationalId = item.GestId ?? 0
        //                    };

        //                    var operationDetail = new OperationDetails();

        //                    operationDetail.ValueCurr = valueCurr;
        //                    operationDetail.Value = value;
        //                    operationDetail.VAT = 0;
        //                    operationDetail.Details = monogItem.Details;
        //                    operationDetail.DebitId = debitAccount;
        //                    operationDetail.CreditId = creditAccount;
        //                    if (autoNumber)
        //                    {
        //                        operationDetail.DetailNr = operDetailIndex;
        //                        operDetailIndex++;
        //                    }

        //                    // caut conturi de cheltuiala partial deductibile
        //                    int expenseAccount = 0;
        //                    var accountDebit = Context.Account.FirstOrDefault(f => f.Id == debitAccount);
        //                    if (accountDebit.Symbol.StartsWith("6"))
        //                    {
        //                        expenseAccount = accountDebit.Id;
        //                    }
        //                    var accountCredit = Context.Account.FirstOrDefault(f => f.Id == creditAccount);
        //                    if (accountCredit.Symbol.StartsWith("6"))
        //                    {
        //                        expenseAccount = accountCredit.Id;
        //                    }

        //                    if (expenseAccount != 0)
        //                    {
        //                        bool okAddNededOper = true;
        //                        var nededOperationDetail = GenerateNededOperDetail(ref operationDetail, list[0].OperationDate, expenseAccount, out okAddNededOper);
        //                        if (okAddNededOper)
        //                        {
        //                            detailList.Add(nededOperationDetail);
        //                            if (autoNumber)
        //                            {
        //                                nededOperationDetail.DetailNr = operDetailIndex;
        //                                operDetailIndex++;
        //                            }
        //                            var nededDetail = new AutoOperationDetail
        //                            {
        //                                DebitAccountId = nededOperationDetail.DebitId,
        //                                CreditAccountId = nededOperationDetail.CreditId,
        //                                Value = nededOperationDetail.Value,
        //                                ValueCurr = nededOperationDetail.ValueCurr,
        //                                Details = monogItem.Details,
        //                                OperationalId = item.GestId ?? 0
        //                            };
        //                            details.Add(nededDetail);
        //                            nededDetail.OperationDetail = nededOperationDetail;

        //                            detail.Value = operationDetail.Value;
        //                            detail.ValueCurr = operationDetail.ValueCurr;
        //                        }

        //                    }

        //                    details.Add(detail);

        //                    detailList.Add(operationDetail);
        //                    detail.OperationDetail = operationDetail;
        //                }
        //            }
        //            autoOperation.OperationDetails = details;
        //            Context.AutoOperationOper.Add(autoOperation);
        //            operation.OperationsDetails = detailList;
        //            Context.Operations.Add(operation);
        //            Context.SaveChanges();
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}

        public string GetDocumentNumber(DocumentType docType, string documentNr, DateTime operationDate)
        {
            string retDocNr = documentNr;
            if (docType.AutoNumber)
            {
                retDocNr = "";
                //var x = Context.Operations.Where(f => f.AppClientId == appClientId && f.DocumentType.Id == docType.Id
                //                                 && f.DocumentDate.Year == operationDate.Year).ToList();
                //if (x.Count == 0)
                //{
                //    retDocNr = "1";
                //}
                //else
                //{
                //    List<int> nr = new List<int>();
                //    foreach (var item in x)
                //    {
                //        try
                //        {
                //            int _nr = int.Parse(item.DocumentNumber);
                //            nr.Add(_nr);
                //        }
                //        catch
                //        {

                //        }
                //    }
                //    if (nr.Count == 0)
                //    {
                //        retDocNr = "1";
                //    }
                //    else
                //    {
                //        retDocNr = (nr.Max() + 1).ToString();
                //    }
                //}
            }

            return retDocNr;
        }

        public string GetDocumentNextNumber(DocumentType docType, DateTime operationDate)
        {
            string retDocNr = "";

            var x = Context.Operations.Where(f => f.DocumentType.Id == docType.Id && f.DocumentDate.Year == operationDate.Year).ToList();
            if (x.Count == 0)
            {
                retDocNr = "1";
            }
            else
            {
                List<int> nr = new List<int>();
                foreach (var item in x)
                {
                    try
                    {
                        int _nr = int.Parse(item.DocumentNumber);
                        nr.Add(_nr);
                    }
                    catch
                    {

                    }
                }
                if (nr.Count == 0)
                {
                    retDocNr = "1";
                }
                else
                {
                    retDocNr = (nr.Max() + 1).ToString();
                }
            }

            return retDocNr;
        }

        public int GetImoAssetAccountId(string account, int operationType, ImoAssetItem assetItem, DateTime operationDate, int localCurrencyId)
        {
            var accountId = 0;

            if (account == "MF")
            {
                accountId = ((int)((ImoAssetOperType)operationType == ImoAssetOperType.Intrare ? assetItem.AssetAccountId : (assetItem.AssetAccountInUseId ?? 0)));
            }
            else if (account == "CA")
            {
                accountId = assetItem.DepreciationAccountId ?? 0;
            }
            else if (account == "CC")
            {
                accountId = assetItem.ExpenseAccountId ?? 0;
            }
            else if (account == "CN")
            {
                accountId = assetItem.AssetAccountId.Value;
            }
            else
            {
                if (account.StartsWith('6') && assetItem.InvoiceDetails != null && assetItem.InvoiceDetails.Invoices.ActivityTypeId != null)
                {
                    accountId = GetAutoAccountActivityType(account, assetItem.ThirdPartyId, assetItem.InvoiceDetails.Invoices.ActivityTypeId.Value, operationDate, localCurrencyId, null);
                }
                else
                {
                    //var storageId = StorageImoAssetDate(assetItem.Id, operationDate);
                    accountId = GetAutoAccount(account, assetItem.ThirdPartyId, operationDate, localCurrencyId, null);
                    //accountId = int.Parse(account);
                }
            }

            var accountSynthetic = Context.Account.FirstOrDefault(f => f.Id == accountId);
            if (accountSynthetic.Symbol.StartsWith('6') && assetItem.InvoiceDetails != null && assetItem.InvoiceDetails.Invoices.ActivityTypeId != null)
            {
                accountId = GetAutoAccountActivityType(accountSynthetic.Symbol, assetItem.ThirdPartyId, assetItem.InvoiceDetails.Invoices.ActivityTypeId.Value, operationDate, localCurrencyId, null);
            }

            return accountId;
        }

        public int GetInvObjectAccountId(string account, InvObjectItem invObjectItem, DateTime operationDate, int localCurrencyId)
        {
            var accountId = 0;

            if (account == "OI")
            {
                accountId = invObjectItem.InvObjectAccountId ?? 0;
            }
            else
            {
                if (account.StartsWith('6') && invObjectItem.InvoiceDetails != null && invObjectItem.InvoiceDetails.Invoices.ActivityTypeId != null)
                {
                    accountId = GetAutoAccountActivityType(account, invObjectItem.ThirdPartyId, invObjectItem.InvoiceDetails.Invoices.ActivityTypeId.Value, operationDate, localCurrencyId, null);
                }
                else
                {
                    //var storageId = StorageImoAssetDate(assetItem.Id, operationDate);
                    accountId = GetAutoAccount(account, invObjectItem.ThirdPartyId, operationDate, localCurrencyId, null);
                    //accountId = int.Parse(account);
                }
            }

            var accountSynthetic = Context.Account.FirstOrDefault(f => f.Id == accountId);
            if (accountSynthetic.Symbol.StartsWith('6') && invObjectItem.InvoiceDetails != null && invObjectItem.InvoiceDetails.Invoices.ActivityTypeId != null)
            {
                accountId = GetAutoAccountActivityType(accountSynthetic.Symbol, invObjectItem.ThirdPartyId, invObjectItem.InvoiceDetails.Invoices.ActivityTypeId.Value, operationDate, localCurrencyId, null);
            }

            return accountId;
        }

        public void DeleteAutoOperation(int operationId, DateTime lastBalanceDate)
        {
            try
            {
                // anulez operatiile contabile
                var contaOperationDetails = Context.AutoOperationDetail.Where(f => f.AutoOperId == operationId && f.OperationDetailId != null).Select(f => f.OperationDetailId).ToList();
                if (contaOperationDetails.Count != 0)
                {
                    var contaOperationId = Context.OperationsDetails.Where(f => contaOperationDetails.Contains(f.Id)).Select(f => f.OperationId).ToList().Distinct();
                    var contaOperation = Context.Operations.Where(f => contaOperationId.Contains(f.Id));
                    foreach (var item in contaOperation)
                    {
                        if (item.OperationStatus == OperationStatus.Checked)
                        {
                            throw new Exception("Nu puteti sa stergeti nota contabila deoarece este validata.");
                        }
                        if (item.OperationDate <= lastBalanceDate)
                        {
                            throw new Exception("Nu puteti sterge notele deoarece luna contabila este inchisa.");
                        }
                        item.State = State.Inactive;
                    }
                }
                // sterg operatiile automate
                var autoOperDetails = Context.AutoOperationDetail.Where(f => f.AutoOperId == operationId).ToList();
                Context.AutoOperationDetail.RemoveRange(autoOperDetails);
                var autoOper = Context.AutoOperationOper.FirstOrDefault(f => f.Id == operationId);
                Context.AutoOperationOper.Remove(autoOper);
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteAutoOperationV1(int operationId, DateTime lastBalanceDate, AutoOperationType autoOperationType)
        {
            try
            {
                // anulez operatiile contabile
                var contaOperationDetails = Context.AutoOperationDetail.Include(f=>f.AutoOper)
                                                   .Where(f => f.OperationalId == operationId && f.OperationDetailId != null && f.AutoOper.AutoOperType == autoOperationType)
                                                   .Select(f => f.OperationDetailId).ToList();
                if (contaOperationDetails.Count != 0)
                {
                    var contaOperationId = Context.OperationsDetails.Where(f => contaOperationDetails.Contains(f.Id)).Select(f => f.OperationId).ToList().Distinct();
                    var contaOperation = Context.Operations.Where(f => contaOperationId.Contains(f.Id)).ToList();
                    foreach (var item in contaOperation)
                    {

                        if (item.OperationDate <= lastBalanceDate)
                        {
                            throw new Exception("Operatiile contabile nu pot fi sterse deoarece luna contabila este inchisa.");
                        }
                        item.State = State.Inactive;

                        var operationChild = Context.Operations.FirstOrDefault(f => f.OperationParentId == item.Id);
                        if (operationChild != null)
                        {
                            operationChild.State = State.Inactive;
                        }
                    }
                }
                // sterg operatiile automate
                var autoOperDetails = Context.AutoOperationDetail.Where(f => f.OperationalId == operationId).ToList();

                foreach (var oper in autoOperDetails)
                {
                    var autoOper = Context.AutoOperationOper.FirstOrDefault(f => f.Id == oper.AutoOperId);
                    Context.AutoOperationOper.Remove(autoOper);
                }
                Context.AutoOperationDetail.RemoveRange(autoOperDetails);
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Sterg notele contabile nevalidate pentru mijloace fixe
        public void DeleteUncheckedAssetOperation(DateTime operationDate, int? assetId)
        {
            var _gestList = Context.ImoAssetStock.Include(f => f.ImoAssetItem)
                                                 .Where(f => f.StockDate >= operationDate && f.ImoAssetItemPFId == assetId /* && f.OperType == ImoAssetOperType.PunereInFunctiune*/)
                                                 .OrderByDescending(f => f.StockDate).ThenByDescending(f => f.Id).ToList();

            var lastBalanceDate = Context.Balance.Where(f => f.Status == State.Active).OrderByDescending(f => f.BalanceDate).FirstOrDefault().BalanceDate;
            foreach (var gest in _gestList)
            {
                DeleteAutoOperationV1(gest.Id, lastBalanceDate, AutoOperationType.MijloaceFixe);
            }
        }
        public void DeleteUncheckedAssetDetailOperation(DateTime operationDate, int? assetId)
        {
            var _gestList = Context.ImoAssetStock.Include(f => f.ImoAssetItem)
                                                 .Where(f => f.StockDate >= operationDate && f.ImoAssetItemId == assetId /* && f.OperType == ImoAssetOperType.PunereInFunctiune*/)
                                                 .OrderByDescending(f => f.StockDate).ThenByDescending(f => f.Id).ToList();

            var lastBalanceDate = Context.Balance.Where(f => f.Status == State.Active).OrderByDescending(f => f.BalanceDate).FirstOrDefault().BalanceDate;
            foreach (var gest in _gestList)
            {
                DeleteAutoOperationV1(gest.Id, lastBalanceDate, AutoOperationType.MijloaceFixe);
            }
        }

        public void DeleteAssetOperations(DateTime operationDate)
        {
            var _gestList = Context.ImoAssetStock.Include(f => f.ImoAssetItem)
                                                .Where(f => f.StockDate >= operationDate /* && f.OperType == ImoAssetOperType.PunereInFunctiune*/)
                                                .OrderByDescending(f => f.StockDate).ThenByDescending(f => f.Id).ToList();

            var lastBalanceDate = Context.Balance.Where(f => f.Status == State.Active).OrderByDescending(f => f.BalanceDate).FirstOrDefault().BalanceDate;
            foreach (var gest in _gestList)
            {
                DeleteAutoOperationV1(gest.Id, lastBalanceDate, AutoOperationType.MijloaceFixe);
            }
        }

        #endregion

        #region Prepayments
        public List<AutoOperationCompute> PrepaymentsPrepareAdd(DateTime startDate, DateTime endDate, int localCurrencyId, PrepaymentType prepaymentType)
        {
            var nextDayLastBalance = Context.Balance.Where(f => f.TenantId == 1 && f.Status == State.Active).Max(f => f.BalanceDate).AddDays(1);
            var currency = Context.Currency.FirstOrDefault(f => f.Id == localCurrencyId);
            var processedOper = Context.AutoOperationDetail.Include(f => f.AutoOper)
                                       .Where(f => f.AutoOper.AutoOperType == AutoOperationType.CheltuieliInAvans || f.AutoOper.AutoOperType == AutoOperationType.VenituriInAvans)
                                       .Select(f => f.OperationalId)
                                       .ToList();
            try
            {
                var list = Context.PrepaymentBalance.Include(f => f.Prepayment)
                                                    .Include(f => f.Prepayment.PrepaymentAccount)
                                                    .Include(f => f.Prepayment)
                                                    .Include(f => f.Prepayment.InvoiceDetails).ThenInclude(f => f.Invoices)
                                                    .Include(f => f.Prepayment).ThenInclude(f => f.PrimDocumentType)
                                                    .Include(f => f.Prepayment).ThenInclude(f => f.ThirdParty)
                                                .Where(f => f.ComputeDate >= nextDayLastBalance && f.ComputeDate >= startDate && f.ComputeDate <= endDate //&& f.OperType == ImoAssetOperType.PunereInFunctiune
                                                        && (f.TranzPrepaymentValue != 0 || f.TranzDeprec != 0)
                                                        && f.OperType != PrepaymentOperType.Iesire
                                                        && !processedOper.Contains(f.Id))
                                                .ToList()
                                                .Select(f => new AutoOperationCompute
                                                {
                                                    OperationId = (int?)null,
                                                    OperationDetailId = (int?)null,
                                                    GestId = f.Id,
                                                    OperationDate = f.ComputeDate,
                                                    DocumentTypeId = (f.OperType == PrepaymentOperType.AmortizareLunara)
                                                                      ? (int?)null
                                                                      : f.Prepayment.PrimDocumentTypeId,
                                                    DocumentType = (f.OperType == PrepaymentOperType.AmortizareLunara)
                                                                      ? ""
                                                                      : (f.Prepayment.PrimDocumentType != null ? f.Prepayment.PrimDocumentType.TypeNameShort : ""),
                                                    DocumentNr = (f.OperType == PrepaymentOperType.AmortizareLunara)
                                                                      ? ""
                                                                      : f.Prepayment.PrimDocumentNr,
                                                    DocumentDate = (f.OperType == PrepaymentOperType.AmortizareLunara)
                                                                      ? (DateTime?)null
                                                                      : f.Prepayment.PrimDocumentDate,
                                                    CurrencyId = currency.Id,
                                                    Currency = currency.CurrencyCode,
                                                    OperationTypeId = (int)f.OperType,
                                                    OperationType = f.OperType.ToString(),
                                                    ItemId = f.PrepaymentId,
                                                    ItemName = f.Prepayment.Description + " - " + f.Prepayment.PrimDocumentNr + " / " + f.Prepayment.PrimDocumentDate.Value.ToShortDateString(),
                                                    StorageOutId = (int?)null,
                                                    StorageOut = null,
                                                    StorageInId = (int?)null,
                                                    StorageIn = null,
                                                    InventoryValue = f.TranzPrepaymentValue,
                                                    DepreciationValue = f.TranzDeprec,
                                                    VATValue = f.TranzPrepaymentVAT,
                                                    DepreciationVAT = f.TranzDeprecVAT,
                                                    AccountSort = f.Prepayment.PrepaymentAccount.Symbol,
                                                    Details = (f.Prepayment.PrimDocumentType.TypeNameShort == "FF" ? (f.Prepayment.ThirdParty?.FullName + ", " + f.Prepayment.InvoiceDetails?.Invoices.InvoiceSeries + "  " + f.Prepayment.InvoiceDetails?.Invoices.InvoiceNumber +
                                                                                                              " / " + f.Prepayment.InvoiceDetails?.Invoices.InvoiceDate.ToShortDateString() +
                                                                                                              ", " + f.Prepayment.InvoiceDetails?.InvoiceElementsDetails.Description +
                                                                                                              ", " + ((f.Prepayment.InvoiceDetails?.Invoices.StartDatePeriod != null &&
                                                                                                                      f.Prepayment.InvoiceDetails?.Invoices.EndDatePeriod != null) ? (f.Prepayment.InvoiceDetails?.Invoices.StartDatePeriod.Value.ToShortDateString() + " - " +
                                                                                                                     f.Prepayment.InvoiceDetails?.Invoices.EndDatePeriod.Value.ToShortDateString()) :
                                                                                                                     "")) : null)
                                                })
                                                .OrderBy(f => f.OperationDate).ThenBy(f => f.AccountSort)
                                                .ToList();
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void PrepaymentsOperationAdd(List<AutoOperationCompute> list, DateTime lastBalanceDate, AutoOperationType autoOperationType, int localCurrencyId, int? operGenId)
        {
            var dateList = list.Select(f => new { OperDate = f.OperationDate }).Distinct().OrderBy(f => f.OperDate);
            // pentru toate datele
            foreach (var dateItem in dateList)
            {
                if (dateItem.OperDate <= lastBalanceDate)
                {
                    var unId = list.Where(f => f.OperationDate == dateItem.OperDate).FirstOrDefault();
                    throw new Exception("Nu puteti sa generati note contabile pentru data " + LazyMethods.DateToString(dateItem.OperDate) + " deoarece luna contabila este inchisa"
                                        + " " + unId.GestId + " " + unId.ItemName);
                }

                var operTypeList = list.Where(f => f.OperationDate == dateItem.OperDate)
                                       .Select(f => new { OperationTypeId = f.OperationTypeId, OperationType = (PrepaymentOperType)f.OperationTypeId })
                                       .Distinct();
                //pentru toate tipurile de operatii
                foreach (var operTypeItem in operTypeList)
                {
                    var monog = Context.AutoOperationConfig
                                       .Where(f => f.AutoOperType == autoOperationType)
                                       .Where(f => f.State == State.Active && dateItem.OperDate >= f.StartDate && dateItem.OperDate <= (f.EndDate ?? dateItem.OperDate)
                                              && f.OperationType == operTypeItem.OperationTypeId)
                                       .OrderBy(f => f.EntryOrder)
                                       .ToList();
                    if (monog.Count == 0)
                    {
                        throw new Exception("Nu exista o monografie definita pentru aceasta operatie");
                    }
                    var individualOper = monog.FirstOrDefault().IndividualOperation;

                    var listToDB = new List<AutoOperationCompute>();

                    var operList = list.Where(f => f.OperationDate == dateItem.OperDate && f.OperationTypeId == operTypeItem.OperationTypeId).ToList();
                    if (individualOper) // daca e operatie individuala => inregistrez operatie cu operatie, altfel e o singura operatie cu multiple detalii
                    {
                        foreach (var operItem in operList)
                        {
                            listToDB.Add(operItem);
                            OperationAddByType(listToDB, (int)operTypeItem.OperationType, monog, autoOperationType, localCurrencyId, operGenId);
                            listToDB.Clear();
                        }
                    }
                    else
                    {
                        listToDB = operList;
                        OperationAddByType(listToDB, (int)operTypeItem.OperationType, monog, autoOperationType, localCurrencyId, operGenId);
                    }
                }
            }
        }

        public int GetPrepaymentAccountId(string account, Prepayment prepayment, DateTime operationDate, int localCurrencyId)
        {
            var accountId = 0;
            if (account == "C")
            {
                accountId = prepayment.PrepaymentAccountId ?? 0;
            }
            else if (account == "CA")
            {
                accountId = prepayment.DepreciationAccountId ?? 0;
            }
            else if (account == "CT")
            {
                accountId = prepayment.PrepaymentAccountVATId ?? 0;
            }
            else if (account == "CAT")
            {
                accountId = prepayment.DepreciationAccountVATId ?? 0;
            }
            else
            {
                if (account.StartsWith('6') && prepayment.InvoiceDetails.Invoices.ActivityTypeId != null)
                {
                    accountId = GetAutoAccountActivityType(account, prepayment.ThirdPartyId, prepayment.InvoiceDetails.Invoices.ActivityTypeId.Value, operationDate, localCurrencyId, null);
                }
                else
                {
                    accountId = GetAutoAccount(account, prepayment.ThirdPartyId, operationDate, localCurrencyId, null);
                }
                //accountId = int.Parse(account);
            }

            if (prepayment.InvoiceDetails != null)
            {
                var accountSynthetic = Context.Account.FirstOrDefault(f => f.Id == accountId);
                if (accountSynthetic != null && accountSynthetic.Symbol.StartsWith('6') && prepayment.InvoiceDetails.Invoices.ActivityTypeId != null)
                {
                    accountId = GetAutoAccountActivityType(accountSynthetic.Symbol, prepayment.ThirdPartyId, prepayment.InvoiceDetails.Invoices.ActivityTypeId.Value, operationDate, localCurrencyId, null);
                }
            }

            return accountId;
        }

        //Contari automate Prepayments din gestiune
        public void AutoPrepaymentsOperationAdd(DateTime dataEnd, int localCurrencyId, PrepaymentType prepaymentType, DateTime lastBalanceDate, int? operGenId)
        {
            var prepaymentsToAdd = AutoPrepaymentsPrepareAdd(dataEnd, localCurrencyId);

            var autoOperationType = (prepaymentType == PrepaymentType.CheltuieliInAvans ? AutoOperationType.CheltuieliInAvans : AutoOperationType.VenituriInAvans);
            PrepaymentsOperationAdd(prepaymentsToAdd, lastBalanceDate, autoOperationType, localCurrencyId, operGenId);
        }

        public List<AutoOperationCompute> AutoPrepaymentsPrepareAdd(DateTime endDate, int localCurrencyId)
        {
            var lastBalanceDay = Context.Balance.Where(f => f.Status == State.Active).Select(f => f.BalanceDate).OrderByDescending(f => f).FirstOrDefault();

            var currency = Context.Currency.FirstOrDefault(f => f.Id == localCurrencyId);
            var processedOper = Context.AutoOperationDetail.Include(f => f.AutoOper)
                                       .Where(f => f.AutoOper.AutoOperType == AutoOperationType.CheltuieliInAvans || f.AutoOper.AutoOperType == AutoOperationType.VenituriInAvans)
                                       .Select(f => f.OperationalId)
                                       .ToList();

            var list = Context.PrepaymentBalance.Include(f => f.Prepayment).ThenInclude(f => f.ThirdParty)
                                                .Include(f => f.Prepayment.InvoiceDetails).ThenInclude(f => f.Invoices)
                                                .Include(f => f.Prepayment.InvoiceDetails).ThenInclude(f => f.InvoiceElementsDetails)
                                                .Include(f => f.Prepayment.PrepaymentAccount)
                                            .Where(f => f.ComputeDate > lastBalanceDay && f.ComputeDate <= endDate //&& f.OperType == ImoAssetOperType.PunereInFunctiune
                                                    && (f.TranzPrepaymentValue != 0 || f.TranzDeprec != 0)
                                                    && f.OperType != PrepaymentOperType.Iesire
                                                    && !processedOper.Contains(f.Id))
                                            .ToList()
                                            .Select(f => new AutoOperationCompute
                                            {
                                                OperationId = (int?)null,
                                                OperationDetailId = (int?)null,
                                                GestId = f.Id,
                                                OperationDate = f.ComputeDate,
                                                DocumentTypeId = (f.OperType == PrepaymentOperType.AmortizareLunara)
                                                                   ? (int?)null
                                                                   : f.Prepayment.PrimDocumentTypeId,
                                                DocumentType = (f.OperType == PrepaymentOperType.AmortizareLunara)
                                                                   ? ""
                                                                   : (f.Prepayment.PrimDocumentType != null ? f.Prepayment.PrimDocumentType.TypeNameShort : ""),
                                                DocumentNr = (f.OperType == PrepaymentOperType.AmortizareLunara)
                                                                   ? ""
                                                                   : f.Prepayment.PrimDocumentNr,
                                                DocumentDate = (f.OperType == PrepaymentOperType.AmortizareLunara)
                                                                   ? (DateTime?)null
                                                                   : f.Prepayment.PrimDocumentDate,
                                                CurrencyId = currency.Id,
                                                Currency = currency.CurrencyCode,
                                                OperationTypeId = (int)f.OperType,
                                                OperationType = f.OperType.ToString(),
                                                ItemId = f.PrepaymentId,
                                                ItemName = f.Prepayment.Description + " - " + f.Prepayment.PrimDocumentNr + " / " + f.Prepayment.PrimDocumentDate.Value.ToShortDateString(),
                                                StorageOutId = null,
                                                StorageOut = null,
                                                StorageInId = null,
                                                StorageIn = null,
                                                InventoryValue = f.TranzPrepaymentValue,
                                                DepreciationValue = f.TranzDeprec,
                                                VATValue = f.TranzPrepaymentVAT,
                                                DepreciationVAT = f.TranzDeprecVAT,
                                                AccountSort = f.Prepayment.PrepaymentAccount.Symbol,
                                                Details = (f.Prepayment.ThirdParty == null ? "" : f.Prepayment.ThirdParty.FullName)
                                                           + (f.Prepayment.InvoiceDetails != null ? ", " + f.Prepayment.InvoiceDetails.Invoices.InvoiceSeries + "  " + f.Prepayment.InvoiceDetails.Invoices.InvoiceNumber + " / "
                                                               + f.Prepayment.InvoiceDetails.Invoices.InvoiceDate.ToShortDateString() +
                                                          ", " + f.Prepayment.InvoiceDetails.Element + ", " + f.Prepayment.InvoiceDetails.InvoiceElementsDetails.Description +
                                                         (f.Prepayment.InvoiceDetails.Invoices.StartDatePeriod != null && f.Prepayment.InvoiceDetails.Invoices.EndDatePeriod != null ? (", " + f.Prepayment.InvoiceDetails.Invoices.StartDatePeriod.Value.ToShortDateString() + " - "
                                                               + f.Prepayment.InvoiceDetails.Invoices.EndDatePeriod.Value.ToShortDateString()) : "") : ", " + f.Prepayment.PrimDocumentNr + " / " + f.Prepayment.PrimDocumentDate.Value.ToShortDateString() + ", " + f.Prepayment.Description)
                                            })
                                            .OrderBy(f => f.OperationDate).ThenBy(f => f.AccountSort)
                                            .ToList();
            return list;
        }

        /// <summary>
        /// Sterg notele contabile si contarile automate aferente datei din gestiune, daca nota nu a fost validata
        /// </summary>
        /// <param name="operationDate"> Data de calcul din gestiune </param>
        /// <param name="prepaymentType">Tipul operatiei din gestiune</param>
        public void DeleteUncheckedAutoOperation(DateTime operationDate, PrepaymentType prepaymentType)
        {
            var _gestList = Context.PrepaymentBalance.Include(f => f.Prepayment)
                    .Where(f => f.ComputeDate >= operationDate && f.Prepayment.PrepaymentType == prepaymentType)
                    .OrderByDescending(f => f.ComputeDate).ThenByDescending(f => f.Id).ToList();

            var lastBalanceDate = Context.Balance.Where(f => f.Status == State.Active).OrderByDescending(f => f.BalanceDate).FirstOrDefault().BalanceDate;
            foreach (var gest in _gestList)
            {
                DeleteAutoOperationV1(gest.Id, lastBalanceDate, prepaymentType == PrepaymentType.CheltuieliInAvans ? AutoOperationType.CheltuieliInAvans : AutoOperationType.VenituriInAvans);
            }
        }

        // Sterg nota contabila si contarea automata pentru cheltuiala selectata
        public void DeleteUncheckedAutoOperationForPrepayment(DateTime operationDate, PrepaymentType prepaymentType, int prepaymentId)
        {
            var _gestList = Context.PrepaymentBalance.Include(f => f.Prepayment)
                    .Where(f => f.ComputeDate >= operationDate && f.Prepayment.PrepaymentType == prepaymentType && f.PrepaymentPFId == prepaymentId)
                    .OrderByDescending(f => f.ComputeDate).ThenByDescending(f => f.Id).ToList();

            var lastBalanceDate = Context.Balance.Where(f => f.Status == State.Active).OrderByDescending(f => f.BalanceDate).FirstOrDefault().BalanceDate;
            foreach (var gest in _gestList)
            {
                DeleteAutoOperationV1(gest.Id, lastBalanceDate, (prepaymentType == PrepaymentType.CheltuieliInAvans ? AutoOperationType.CheltuieliInAvans : AutoOperationType.VenituriInAvans));
            }
        }
        #endregion

        #region InvObjects
        public void AutoInvObjectOperationAdd(DateTime dataEnd, int appClientId, int localCurrencyId, DateTime lastBalanceDate, int? operGenId)
        {
            var invObjectList = AutoInvObjectPrepareAdd(dataEnd, localCurrencyId, appClientId).ToList();

            var autoOperType = AutoOperationType.ObiecteDeInventar;
            InvObjectOperationAdd(invObjectList, lastBalanceDate, autoOperType, localCurrencyId, operGenId);
        }

        public void DeleteInvObjectOperations(DateTime gestDate)
        {
            var _gestList = Context.InvObjectStock.Include(f => f.InvObjectItem)
                                                .Where(f => f.StockDate >= gestDate)
                                                .OrderByDescending(f => f.StockDate).ThenByDescending(f => f.Id).ToList();

            var lastBalanceDate = Context.Balance.Where(f => f.Status == State.Active).OrderByDescending(f => f.BalanceDate).FirstOrDefault().BalanceDate;
            foreach (var gest in _gestList)
            {
                DeleteAutoOperationV1(gest.Id, lastBalanceDate, AutoOperationType.ObiecteDeInventar);
            }
        }

        public void DeleteInvObjectDetailOperation(DateTime operationDate, int? invObjectId)
        {
            var _gestList = Context.InvObjectStock.Include(f => f.InvObjectItem)
                                                 .Where(f => f.StockDate >= operationDate && f.InvObjectItemId == invObjectId)
                                                 .OrderByDescending(f => f.StockDate).ThenByDescending(f => f.Id).ToList();

            var lastBalanceDate = Context.Balance.Where(f => f.Status == State.Active).OrderByDescending(f => f.BalanceDate).FirstOrDefault().BalanceDate;
            foreach (var gest in _gestList)
            {
                DeleteAutoOperationV1(gest.Id, lastBalanceDate, AutoOperationType.ObiecteDeInventar);
            }
        }

        private List<AutoOperationCompute> AutoInvObjectPrepareAdd(DateTime dataEnd, int localCurrencyId, int appClientId)
        {
            var nextDayLastBalance = Context.Balance.Where(f => f.TenantId == appClientId && f.Status == State.Active).Max(f => f.BalanceDate).AddDays(1);

            var currency = Context.Account.Include(f => f.Currency).Where(f => f.TenantId == appClientId).Select(f => f.Currency).FirstOrDefault();
            var processedOper = Context.AutoOperationDetail.Include(f => f.AutoOper)
                                       .Where(f => f.AutoOper.AutoOperType == AutoOperationType.ObiecteDeInventar)
                                       .Select(f => f.OperationalId)
                                       .ToList();

            var exceptOperContare = new List<InvObjectOperType>
            {
                InvObjectOperType.Transfer,
                InvObjectOperType.BonMiscare
            };

            var list = Context.InvObjectStock.Include(f => f.InvObjectItem)
                                            .Include(f => f.InvObjectItem.PrimDocumentType)
                                            .Include(f => f.InvObjectItem.DocumentType)
                                            .Include(f => f.InvObjectItem.InvObjectAccount)
                                            .Include(f => f.InvObjectItem.ThirdParty)
                                            .Include(f => f.InvObjectItem.InvoiceDetails)
                                            .Include(f => f.InvObjectItem.InvoiceDetails).ThenInclude(f => f.Invoices)
                                            .Include(f => f.InvObjectItem.InvoiceDetails).ThenInclude(f => f.InvoiceElementsDetails)
                                            .Include(f => f.InvObjectOperDet)
                                            .Include(f => f.InvObjectOperDet.InvObjectOper)
                                            .Include(f => f.InvObjectOperDet.InvObjectOper.DocumentType)
                                            .Include(f => f.Storage)

                                            .Where(f => f.StockDate >= nextDayLastBalance && f.StockDate <= dataEnd
                                                   && !processedOper.Contains(f.Id))
                                            .Where(f => !exceptOperContare.Contains(f.OperType))
                                            .ToList()
                                            .Select(f => new AutoOperationCompute
                                            {
                                                OperationId = (int?)null,
                                                OperationDetailId = (int?)null,
                                                GestId = f.Id,
                                                OperationDate = f.StockDate,
                                                DocumentTypeId = (f.OperType == InvObjectOperType.Intrare) ? f.InvObjectItem.PrimDocumentTypeId : f.InvObjectOperDet.InvObjectOper.DocumentTypeId,
                                                DocumentType = (f.OperType == InvObjectOperType.Intrare)
                                                                ? (f.InvObjectItem.PrimDocumentType != null ? f.InvObjectItem.PrimDocumentType.TypeNameShort : "")
                                                                : (f.InvObjectOperDet.InvObjectOper != null ? f.InvObjectOperDet.InvObjectOper.DocumentType.TypeNameShort : ""),
                                                DocumentNr = (f.OperType == InvObjectOperType.Intrare) ? f.InvObjectItem.PrimDocumentNr : f.InvObjectOperDet.InvObjectOper.DocumentNr.ToString(),
                                                DocumentDate = (f.OperType == InvObjectOperType.Intrare) ? f.InvObjectItem.PrimDocumentDate : f.InvObjectOperDet.InvObjectOper.DocumentDate,
                                                CurrencyId = currency.Id,
                                                Currency = currency.CurrencyCode,
                                                OperationTypeId = (int)f.OperType,
                                                OperationType = f.OperType.ToString(),
                                                ItemId = f.InvObjectItemId,
                                                ItemName = f.InvObjectItem.Name,
                                                ItemInventoryNumber = f.InvObjectItem.InventoryNr,
                                                StorageOutId = null,
                                                StorageOut = null,
                                                StorageInId = f.StorageId,
                                                StorageIn = f.Storage.StorageName,
                                                InventoryValue = f.TranzInventoryValue,
                                                DepreciationValue = f.Deprec,
                                                AccountSort = (f.InvObjectItem.InvObjectAccount == null) ? "" : f.InvObjectItem.InvObjectAccount.Symbol,
                                                Details = (f.InvObjectItem.PrimDocumentType.TypeNameShort == "FF" ? ((f.InvObjectItem.ThirdParty == null ? "" : f.InvObjectItem.ThirdParty.FullName) +
                                                                                                          ", " + (f.InvObjectItem.InvoiceDetails != null ? f.InvObjectItem.InvoiceDetails.Invoices.InvoiceSeries + "  " + f.InvObjectItem.InvoiceDetails.Invoices.InvoiceNumber +
                                                                                                          " / " + f.InvObjectItem.InvoiceDetails.Invoices.InvoiceDate.ToShortDateString() +
                                                                                                          ", " + f.InvObjectItem.InvoiceDetails.InvoiceElementsDetails.Description + ": " + f.InvObjectItem.InvoiceDetails.Element.ToString() +
                                                                                                          ((f.InvObjectItem.InvoiceDetails.Invoices.StartDatePeriod != null && f.InvObjectItem.InvoiceDetails.Invoices.EndDatePeriod != null) ? ", " + f.InvObjectItem.InvoiceDetails.Invoices.StartDatePeriod.Value.ToShortDateString() + " - " +
                                                                                                                 f.InvObjectItem.InvoiceDetails.Invoices.EndDatePeriod.Value.ToShortDateString() : null) : null))
                                                                                                            : null)
                                            })
                                            .OrderBy(f => f.OperationDate).ThenBy(f => f.AccountSort)
                                            .ToList();
            var ret = new List<AutoOperationCompute>();
            foreach (var item in list)
            {
                if ((InvObjectOperType)item.OperationTypeId == InvObjectOperType.DareInConsum)
                {
                    var inventoryNr = Context.InvObjectItem.FirstOrDefault(f => f.Id == item.ItemId).InventoryNr;
                    item.Details = inventoryNr + " " + item.ItemName + " - Dare in consum";
                }
                if ((InvObjectOperType)item.OperationTypeId == InvObjectOperType.DareInConsum && item.InventoryValue < 0)
                {

                }
                else
                {
                    ret.Add(item);
                }

            }

            return ret;
        }

        private void InvObjectOperationAdd(List<AutoOperationCompute> invObjectList, DateTime lastBalanceDate, AutoOperationType autoOperType, int localCurrencyId, int? operGenId)
        {
            var dateList = invObjectList.Select(f => new { OperDate = f.OperationDate }).Distinct().OrderBy(f => f.OperDate);
            // pentru toate datele
            foreach (var dateItem in dateList)
            {
                if (dateItem.OperDate <= lastBalanceDate)
                {
                    throw new Exception("Nu puteti sa generati note contabile pentru data " + LazyMethods.DateToString(dateItem.OperDate) + " deoarece luna contabila este inchisa");
                }

                var operTypeList = invObjectList.Where(f => f.OperationDate == dateItem.OperDate).Select(f => new { OperationTypeId = f.OperationTypeId, OperationType = (InvObjectOperType)f.OperationTypeId }).Distinct();
                //pentru toate tipurile de operatii
                foreach (var operTypeItem in operTypeList)
                {
                    var monog = Context.AutoOperationConfig
                                       .Where(f => f.AutoOperType == AutoOperationType.ObiecteDeInventar)
                                       .Where(f => f.State == State.Active && dateItem.OperDate >= f.StartDate && dateItem.OperDate <= (f.EndDate ?? dateItem.OperDate)
                                              && f.OperationType == operTypeItem.OperationTypeId)
                                       .OrderBy(f => f.EntryOrder)
                                       .ToList();
                    if (monog.Count == 0)
                    {
                        throw new Exception("Nu exista o monografie definita pentru aceasta operatie");
                    }
                    var individualOper = monog.FirstOrDefault().IndividualOperation;

                    var listToDB = new List<AutoOperationCompute>();

                    var operList = invObjectList.Where(f => f.OperationDate == dateItem.OperDate && f.OperationTypeId == operTypeItem.OperationTypeId).ToList();
                    if (individualOper) // daca e operatie individuala => inregistrez operatie cu operatie, altfel e o singura operatie cu multiple detalii
                    {
                        foreach (var operItem in operList)
                        {
                            listToDB.Add(operItem);
                            OperationAddByType(listToDB, (int)operTypeItem.OperationType, monog, autoOperType, localCurrencyId, operGenId);
                            listToDB.Clear();
                        }
                    }
                    else
                    {
                        listToDB = operList;
                        OperationAddByType(listToDB, (int)operTypeItem.OperationType, monog, autoOperType, localCurrencyId, operGenId);
                    }
                }
            }
        }

        #endregion

        // Invoices
        #region Invoices
        public void InvoiceToConta(int invoiceId, DateTime operationDate, int localCurrencyId)
        {
            var invoice = Context.Invoices.Include(f => f.InvoiceDetails).ThenInclude(f => f.InvoiceElementsDetails).Include(f => f.ThirdParty).FirstOrDefault(f => f.Id == invoiceId);
            var invoiceDetailsForConta = invoice.InvoiceDetails.Where(f => f.InvoiceElementsDetails.InvoiceElementsType == InvoiceElementsType.Altele);

            if (invoiceDetailsForConta.Count() == 0)
            {
                return;
            }

            var documentType = Context.DocumentType.FirstOrDefault(f => f.Id == invoice.DocumentTypeId);
            if (documentType == null)
                throw new Exception("Nu a fost identificat tipul de document pentru inregistrarea facturilor sau bonurilor. Acesta trebuie sa aiba codul FF sau BF!");

            var operationChild = new Operation(); // e folosit in cazul in care avem documente in valuta

            var operation = new Operation
            {
                CurrencyId = localCurrencyId,
                OperationDate = operationDate,
                DocumentTypeId = documentType.Id,
                DocumentNumber = invoice.InvoiceNumber,
                DocumentDate = invoice.InvoiceDate,
                OperationStatus = OperationStatus.Unchecked,
                State = State.Active,
                ExternalOperation = true
            };

            Context.Operations.Add(operation);
            Context.SaveChanges();

            decimal exchangeRate = 1;
            if (localCurrencyId != invoice.CurrencyId)
            {
                exchangeRate = _exchangeRatesRepository.GetExchangeRateForOper(invoice.InvoiceDate, invoice.CurrencyId, localCurrencyId);

                operationChild = new Operation // pentr operatiile in valuta... in operation am valorile in lei, iar in operationChild am valorile in valuta
                {
                    CurrencyId = invoice.CurrencyId,
                    OperationDate = operationDate,
                    DocumentTypeId = documentType.Id,
                    DocumentNumber = invoice.InvoiceNumber,
                    DocumentDate = invoice.InvoiceDate,
                    OperationStatus = OperationStatus.Unchecked,
                    State = State.Active,
                    ExternalOperation = true,
                    OperationParentId = operation.Id
                };
                Context.Operations.Add(operationChild);
                Context.SaveChanges();
            }

            var detailList = new List<OperationDetails>();
            foreach (var detail in invoiceDetailsForConta)
            {
                var operationDetailChild = new OperationDetails(); // pentru operatiile in valuta
                var operationDetail = new OperationDetails();
                int pozitieSchimbAccountId = 0;
                int contravaloarePozitieSchimbAccountId = 0;

                if (invoice.CurrencyId != invoice.MonedaFacturaId)
                {
                    operationDetail = new OperationDetails
                    {
                        ValueCurr = (localCurrencyId != invoice.CurrencyId) ? (detail.ValoareTotalaDetaliu.Value + detail.VAT) : 0,
                        Value = (detail.ValoareTotalaDetaliu.Value + detail.VAT),
                        VAT = detail.ProcVAT,
                        Details = invoice.ThirdParty.FullName + ", " + invoice.InvoiceSeries + " " + invoice.InvoiceNumber + " / " + invoice.InvoiceDate.ToShortDateString() + ", "
                           + detail.Element + ", " + detail.InvoiceElementsDetails.Description + ", " +
                           (invoice.StartDatePeriod.HasValue && invoice.EndDatePeriod.HasValue ? invoice.StartDatePeriod.Value.ToShortDateString() + " - " + invoice.EndDatePeriod.Value.ToShortDateString()
                                                                                         : "")
                           + (invoice.CurrencyId != invoice.MonedaFacturaId ? (", Curs valutar: " + invoice.CursValutar.ToString()) : "")
                    };
                }
                else
                {
                    operationDetail = new OperationDetails
                    {
                        ValueCurr = (localCurrencyId != invoice.CurrencyId) ? (detail.Value + detail.VAT) : 0,
                        Value = (detail.Value + detail.VAT) * exchangeRate,
                        VAT = detail.ProcVAT,
                        Details = invoice.ThirdParty.FullName + ", " + invoice.InvoiceSeries + " " + invoice.InvoiceNumber + " / " + invoice.InvoiceDate.ToShortDateString() + ", "
                                 + detail.Element + ", " + detail.InvoiceElementsDetails.Description + ", " +
                                 (invoice.StartDatePeriod.HasValue && invoice.EndDatePeriod.HasValue ? invoice.StartDatePeriod.Value.ToShortDateString() + " - " + invoice.EndDatePeriod.Value.ToShortDateString()
                                                                                               : "")
                                 + (invoice.CurrencyId != localCurrencyId ? (", Curs valutar: " + exchangeRate.ToString()) : "")
                    };
                }

                var element = Context.InvoiceElementsDetails.FirstOrDefault(f => f.Id == detail.InvoiceElementsDetailsId);
                if (element.ThirdPartyAccount == null || element.CorrespondentAccount == null)
                    throw new Exception("Conturile pentru terti si conturile corespondente trebuie completate. Verificati daca elementele facturii sunt completate in Economic -> Facturi -> Elemente");

                var thirdPartyAccountId = GetAutoAccount(element.ThirdPartyAccount, invoice.ThirdPartyId, operationDate, invoice.CurrencyId, null);
                var correspAccountId = GetAutoAccount(element.CorrespondentAccount, invoice.ThirdPartyId, operationDate, invoice.CurrencyId, null);

                if (invoice.CurrencyId != localCurrencyId)
                {
                    operationDetailChild = new OperationDetails
                    {
                        ValueCurr = (localCurrencyId != invoice.CurrencyId) ? (detail.Value + detail.VAT) : 0,
                        Value = (detail.Value + detail.VAT),
                        VAT = detail.ProcVAT,
                        Details = invoice.ThirdParty.FullName + ", " + invoice.InvoiceSeries + " " + invoice.InvoiceNumber + " / " + invoice.InvoiceDate.ToShortDateString() + ", " +
                              detail.Element + ", " + detail.InvoiceElementsDetails.Description + ", " +
                              (invoice.StartDatePeriod.HasValue && invoice.EndDatePeriod.HasValue ? invoice.StartDatePeriod.Value.ToShortDateString() + " - " + invoice.EndDatePeriod.Value.ToShortDateString()
                                                                                            : "")
                    };

                    var activityTypeId = invoice.ActivityTypeId ?? GetMainActivityType();
                    // iau pozitia de schimb valutar
                    try
                    {

                        pozitieSchimbAccountId = _accountRepository.GetAll().FirstOrDefault(f => f.AccountFuncType == AccountFuncType.PozitieSchimbCheltuieli && f.CurrencyId == invoice.CurrencyId
                                                                                            && f.ActivityTypeId == activityTypeId).Id;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Nu am identificat contul pentru pozitia de schimb valutar");
                    }
                    // iau contravaloarea pozitiei de schimb valutar
                    try
                    {
                        var denumireMoneda = _currencyRepository.GetCurrencyById(invoice.CurrencyId).CurrencyCode;
                        contravaloarePozitieSchimbAccountId = _accountRepository.GetAll().FirstOrDefault(f => f.AccountFuncType == AccountFuncType.ContravaloarePozitieSchimbCheltuieli
                                                                                                         && f.ActivityTypeId == activityTypeId
                                                                                                         && f.AccountName.IndexOf(denumireMoneda) >= 0).Id;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Nu am identificat contul pentru contravaloarea pozitiei de schimb valutar");
                    }

                    if (invoice.ThirdPartyQuality == ThirdPartyQuality.Client) // Facturi de la clienti
                    {
                        operationDetailChild.DebitId = thirdPartyAccountId;
                        operationDetailChild.CreditId = pozitieSchimbAccountId;
                        operationDetail.DebitId = contravaloarePozitieSchimbAccountId;
                        operationDetail.CreditId = correspAccountId;
                    }
                    else
                    {
                        operationDetailChild.DebitId = pozitieSchimbAccountId;
                        operationDetailChild.CreditId = thirdPartyAccountId;
                        operationDetail.DebitId = correspAccountId;
                        operationDetail.CreditId = contravaloarePozitieSchimbAccountId;
                    }
                    operationDetailChild.OperationId = operationChild.Id;
                    Context.OperationsDetails.Add(operationDetailChild);
                }
                else
                {
                    if (invoice.ThirdPartyQuality == ThirdPartyQuality.Client) // Facturi de la clienti
                    {
                        operationDetail.DebitId = thirdPartyAccountId;
                        operationDetail.CreditId = correspAccountId;
                    }
                    else
                    {
                        operationDetail.DebitId = correspAccountId;
                        operationDetail.CreditId = thirdPartyAccountId;
                    }
                }
                operationDetail.OperationId = operation.Id;

                bool okAddNededOper = true;
                var nededOperDetail = GenerateNededOperDetail(ref operationDetail, operationDate, correspAccountId, out okAddNededOper);


                detailList.Add(operationDetail);
                Context.OperationsDetails.Add(operationDetail);

                if (okAddNededOper)
                {
                    detailList.Add(nededOperDetail);
                    Context.OperationsDetails.Add(nededOperDetail);
                }

                Context.SaveChanges();
                detail.ContaOperationDetailId = operationDetail.Id;
            }

            //operation.OperationsDetails = detailList;
            //invoice.ContaOperationId = operation.Id;
            Context.SaveChanges();
        }

        public void SaveDirectToConta(int invoiceId, DateTime operationDate, int localCurrencyId)
        {
            var invoice = Context.Invoices.Include(f => f.InvoiceDetails).ThenInclude(f => f.InvoiceElementsDetails).Include(f => f.ThirdParty)
                                           .Include(f => f.Decont).ThenInclude(f => f.ThirdParty).FirstOrDefault(f => f.Id == invoiceId);
            var invoiceDetailsForConta = invoice.InvoiceDetails.Where(f => f.InvoiceElementsDetails.InvoiceElementsType == InvoiceElementsType.CheltuieliInAvans && f.DurationInMonths == 0);

            if (invoiceDetailsForConta.Count() == 0)
            {
                return;
            }

            var documentType = Context.DocumentType.FirstOrDefault(f => f.Id == invoice.DocumentTypeId);
            if (documentType == null)
                throw new Exception("Nu a fost identificat tipul de document pentru inregistrarea facturilor sau bonurilor. Acesta trebuie sa aiba codul FF sau BF!");

            var operationChild = new Operation(); // e folosit in cazul in care avem documente in valuta

            var operation = new Operation
            {
                CurrencyId = localCurrencyId,
                OperationDate = operationDate,
                DocumentTypeId = documentType.Id,
                DocumentNumber = invoice.InvoiceNumber,
                DocumentDate = invoice.InvoiceDate,
                OperationStatus = OperationStatus.Unchecked,
                State = State.Active,
                ExternalOperation = true
            };

            Context.Operations.Add(operation);
            Context.SaveChanges();

            decimal exchangeRate = 1;
            if (invoice.CurrencyId != localCurrencyId)
            {
                exchangeRate = _exchangeRatesRepository.GetExchangeRateForOper(invoice.InvoiceDate, invoice.CurrencyId, localCurrencyId);

                operationChild = new Operation // pentr operatiile in valuta... in operation am valorile in lei, iar in operationChild am valorile in valuta
                {
                    CurrencyId = invoice.CurrencyId,
                    OperationDate = operationDate,
                    DocumentTypeId = documentType.Id,
                    DocumentNumber = invoice.InvoiceNumber,
                    DocumentDate = invoice.InvoiceDate,
                    OperationStatus = OperationStatus.Unchecked,
                    State = State.Active,
                    ExternalOperation = true,
                    OperationParentId = operation.Id
                };
                Context.Operations.Add(operationChild);
                Context.SaveChanges();
            }

            var detailList = new List<OperationDetails>();
            foreach (var detail in invoiceDetailsForConta)
            {
                var operationDetailChild = new OperationDetails(); // pentru operatiile in valuta
                var operationDetail = new OperationDetails();
                int pozitieSchimbAccountId = 0;
                int contravaloarePozitieSchimbAccountId = 0;

                if (invoice.CurrencyId != invoice.MonedaFacturaId)
                {
                    operationDetail = new OperationDetails
                    {
                        ValueCurr = (localCurrencyId != invoice.CurrencyId) ? (detail.ValoareTotalaDetaliu.Value + detail.VAT) : 0,
                        Value = (detail.ValoareTotalaDetaliu.Value + detail.VAT),
                        VAT = detail.ProcVAT,
                        Details = detail.Element + ", " + invoice.ThirdParty.FullName + ", " + invoice.InvoiceSeries + " " + invoice.InvoiceNumber + " / " + invoice.InvoiceDate.ToShortDateString() +
                          ", " + detail.InvoiceElementsDetails.Description + ", " +
                            (invoice.StartDatePeriod.HasValue && invoice.EndDatePeriod.HasValue ? invoice.StartDatePeriod.Value.ToShortDateString() + " - " + invoice.EndDatePeriod.Value.ToShortDateString()
                                                                                          : "")
                            + (invoice.CurrencyId != invoice.MonedaFacturaId ? (", Curs valutar: " + invoice.CursValutar.Value.ToString()) : "")
                            +(invoice.DecontId != null ? ", Decont: " + invoice.Decont.ThirdParty.FullName : "")
                    };
                }
                else
                {
                    operationDetail = new OperationDetails
                    {
                        ValueCurr = (localCurrencyId != invoice.CurrencyId) ? (detail.Value + detail.VAT) : 0,
                        Value = (detail.Value + detail.VAT) * exchangeRate,
                        VAT = detail.ProcVAT,
                        Details = detail.Element + ", " + invoice.ThirdParty.FullName + ", " + invoice.InvoiceSeries + " " + invoice.InvoiceNumber + " / " + invoice.InvoiceDate.ToShortDateString() +
                                                  ", " + detail.InvoiceElementsDetails.Description + ", " + (invoice.StartDatePeriod.HasValue && invoice.EndDatePeriod.HasValue ?
                                                         invoice.StartDatePeriod.Value.ToShortDateString() + " - " + invoice.EndDatePeriod.Value.ToShortDateString() : "") + (invoice.CurrencyId != localCurrencyId ?
                                                         (", Curs valutar: " + exchangeRate.ToString()) : "")
                                                          +(invoice.DecontId != null ? ", Decont: " + invoice.Decont.ThirdParty.FullName : "")
                    };
                }

                var element = Context.InvoiceElementsDetails.FirstOrDefault(f => f.Id == detail.InvoiceElementsDetailsId);
                if (element.ThirdPartyAccount == null || element.CorrespondentAccount == null)
                    throw new Exception("Conturile pentru terti si conturile corespondente trebuie completate. Verificati daca elementele facturii sunt completate in Economic -> Facturi -> Elemente");

                var thirdPartyAccountId = GetAutoAccount(element.ThirdPartyAccount, invoice.ThirdPartyId, operationDate, invoice.CurrencyId, null);

                int directAccountId = 0;
                if (element.ExpenseAmortizAccount == null)
                {
                    throw new Exception("Nu a fost specificat contul de cheltuiala directa la definirea tipurilor de cheltuiala");
                }

                if (element.ExpenseAmortizAccount.StartsWith('6') && invoice.ActivityTypeId != null)
                {
                    directAccountId = GetAutoAccountActivityType(element.ExpenseAmortizAccount, invoice.ThirdPartyId, invoice.ActivityTypeId.Value, operationDate, localCurrencyId, null);
                }
                else
                {
                    directAccountId = GetAutoAccount(element.ExpenseAmortizAccount, invoice.ThirdPartyId, operationDate, invoice.CurrencyId, null);
                }

                if (invoice.ThirdPartyQuality == ThirdPartyQuality.Client) // Facturi de la clienti
                {
                    operationDetail.DebitId = thirdPartyAccountId;
                    operationDetail.CreditId = directAccountId;
                }
                else
                {
                    operationDetail.DebitId = directAccountId;
                    operationDetail.CreditId = thirdPartyAccountId;
                }

                if (invoice.CurrencyId != localCurrencyId)
                {
                    operationDetailChild = new OperationDetails
                    {
                        ValueCurr = (localCurrencyId != invoice.CurrencyId) ? (detail.Value + detail.VAT) : 0,
                        Value = (detail.Value + detail.VAT),
                        VAT = detail.ProcVAT,
                        Details = detail.Element + ", " + invoice.ThirdParty.FullName + ", " + invoice.InvoiceSeries + " " + invoice.InvoiceNumber + " / " + invoice.InvoiceDate.ToShortDateString() + ", " +
                              (invoice.StartDatePeriod.HasValue && invoice.EndDatePeriod.HasValue ? invoice.StartDatePeriod.Value.ToShortDateString() + " - " + invoice.EndDatePeriod.Value.ToShortDateString()
                                                                                            : "")
                    };

                    var activityTypeId = invoice.ActivityTypeId ?? GetMainActivityType();
                    // iau pozitia de schimb valutar
                    try
                    {

                        pozitieSchimbAccountId = _accountRepository.GetAll().FirstOrDefault(f => f.AccountFuncType == AccountFuncType.PozitieSchimbCheltuieli && f.CurrencyId == invoice.CurrencyId
                                                                                            && f.ActivityTypeId == activityTypeId).Id;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Nu am identificat contul pentru pozitia de schimb valutar");
                    }
                    // iau contravaloarea pozitiei de schimb valutar
                    try
                    {
                        var denumireMoneda = _currencyRepository.GetCurrencyById(invoice.CurrencyId).CurrencyCode;
                        contravaloarePozitieSchimbAccountId = _accountRepository.GetAll().FirstOrDefault(f => f.AccountFuncType == AccountFuncType.ContravaloarePozitieSchimbCheltuieli
                                                                                                         && f.ActivityTypeId == activityTypeId
                                                                                                         && f.AccountName.IndexOf(denumireMoneda) >= 0).Id;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Nu am identificat contul pentru contravaloarea pozitiei de schimb valutar");
                    }

                    if (invoice.ThirdPartyQuality == ThirdPartyQuality.Client) // Facturi de la clienti
                    {
                        operationDetailChild.DebitId = thirdPartyAccountId;
                        operationDetailChild.CreditId = pozitieSchimbAccountId;
                        operationDetail.DebitId = contravaloarePozitieSchimbAccountId;
                        operationDetail.CreditId = directAccountId;
                    }
                    else
                    {
                        operationDetailChild.DebitId = pozitieSchimbAccountId;
                        operationDetailChild.CreditId = thirdPartyAccountId;
                        operationDetail.DebitId = directAccountId;
                        operationDetail.CreditId = contravaloarePozitieSchimbAccountId;
                    }
                    operationDetailChild.OperationId = operationChild.Id;
                    Context.OperationsDetails.Add(operationDetailChild);
                }
                else
                {
                    if (invoice.ThirdPartyQuality == ThirdPartyQuality.Client) // Facturi de la clienti
                    {
                        operationDetail.DebitId = thirdPartyAccountId;
                        operationDetail.CreditId = directAccountId;
                    }
                    else
                    {
                        operationDetail.DebitId = directAccountId;
                        operationDetail.CreditId = thirdPartyAccountId;
                    }
                }

                operationDetail.OperationId = operation.Id;

                bool okAddNededOper = true;
                var nededOperDetail = GenerateNededOperDetail(ref operationDetail, operationDate, directAccountId, out okAddNededOper);


                detailList.Add(operationDetail);
                Context.OperationsDetails.Add(operationDetail);

                if (okAddNededOper)
                {
                    detailList.Add(nededOperDetail);
                    Context.OperationsDetails.Add(nededOperDetail);
                }

                Context.SaveChanges();
                detail.ContaOperationDetailId = operationDetail.Id;
            }
            Context.SaveChanges();
        }

        public int GetAutoAccount(string symbol, int? thirdPartyId, DateTime operationDate, int currencyId, int? imoAssetStorage)
        {
            var ret = new Account();

            if (symbol == "")
            {
                throw new Exception("Pentru elementul selectat nu a fost definit un cont de cheltuiala.");
            }

            var count = Context.AccountConfig.Count(f => f.Symbol == symbol && f.ValabilityDate <= operationDate);

            if (count == 0) // daca nu e definit in configurarea conturilor automate => caut contul dupa simbol direct in planul de conturi
            {
                ret = Context.Account.FirstOrDefault(f => f.Status == State.Active && f.Symbol == symbol);
                if (ret == null)
                {
                    throw new Exception("Contul cu simbolul " + symbol + " nu a fost gasit in planul de conturi.");
                }
                if (!ret.ComputingAccount)
                {
                    throw new Exception("Contul cu simbolul " + symbol + " nu este marcat in planul de conturi ca un cont in care se pot inregistra operatii contabile. Verificati configurarea conturilor pentru contarile automate si planul de conturi.");
                }
            }
            else // este definit in configurare => reconstitui simbolul contului
            {
                var autoAccountList = Context.AccountConfig
                                             .Where(f => f.Symbol == symbol && f.ValabilityDate <= operationDate)
                                             .OrderByDescending(f => f.ValabilityDate)
                                             .ToList();
                if (imoAssetStorage != null)
                {
                    autoAccountList = autoAccountList.Where(f => f.ImoAssetStorageId == imoAssetStorage).ToList();
                }

                var autoAccount = autoAccountList.OrderByDescending(f => f.ValabilityDate)
                                                  .FirstOrDefault();
                if (autoAccount == null)
                {
                    throw new Exception("Nu a fost identificata nici o configurare pentru contul " + symbol + " care sa indeplineasca conditiile");
                }

                string newSymbol = "";
                newSymbol = autoAccount.AccountRad;

                // e cont de tert => caut contul cu sinteticul accountRad si tertul specificat
                if (autoAccount.ThirdPartyAccount)
                {
                    var accountThirdParty = Context.Account.Include(f => f.SyntheticAccount)
                                                           .FirstOrDefault(f => f.SyntheticAccount.Symbol == autoAccount.AccountRad && f.AccountStatus && f.ComputingAccount
                                                                                && f.ThirdPartyId == thirdPartyId && f.CurrencyId == currencyId && f.Status == State.Active);
                    if (accountThirdParty != null)
                    {
                        return accountThirdParty.Id;
                    }
                }

                ret = GenerateAccount(autoAccount, currencyId, thirdPartyId);

            }
            return ret.Id;
        }

        public Account GenerateAccount(AccountConfig autoAccount, int currencyId, int? thirdPartyId)
        {
            string newSymbol = autoAccount.AccountRad;
            string accountName = "";
            Account newAccount = new Account();

            if (autoAccount.Sufix != null)
            {
                newSymbol += "." + autoAccount.Sufix;
            }
            accountName = autoAccount.Description;
            if (autoAccount.IncludId1 || autoAccount.ThirdPartyAccount)
            {
                if (thirdPartyId == null)
                    throw new Exception("Pentru contul cu simbolul " + autoAccount.Symbol + " trebuie specificat tertul");

                var thirdParty = Context.Persons.FirstOrDefault(f => f.Id == thirdPartyId);
                var id1 = thirdParty.Id1;
                if (autoAccount.IncludId1)
                {
                    newSymbol += "." + id1;
                }
                accountName = thirdParty.FullName;
            }

            if (autoAccount.NrCaractere != null)
            {
                var account = Context.Account.Where(f => f.Symbol.IndexOf(newSymbol) >= 0 && f.ComputingAccount && f.Symbol != newSymbol && f.Status == State.Active).OrderByDescending(f => f.Symbol).FirstOrDefault();

                if (account == null)
                {
                    newSymbol += "." + "1".PadLeft(autoAccount.NrCaractere.Value, '0');
                }
                else
                {
                    var maxSymbol = account.Symbol;
                    var substrSymbol = maxSymbol.Substring(newSymbol.Length + 1);
                    if (substrSymbol.IndexOf(".") >= 0)
                    {
                        substrSymbol = substrSymbol.Substring(0, substrSymbol.IndexOf("."));
                        var nextItem = int.Parse(substrSymbol);
                        nextItem++;
                        newSymbol += "." + nextItem.ToString().PadLeft(autoAccount.NrCaractere.Value, '0');
                    }
                }
            }

            if (autoAccount.IncludMoneda)
            {
                var moneda = Context.Currency.FirstOrDefault(f => f.Id == currencyId);
                newSymbol += "." + moneda.CurrencyCode;
            }

            // caut contul format in planul de conturi
            newAccount = Context.Account.FirstOrDefault(f => f.Status == State.Active && f.Symbol == newSymbol);
            if (newAccount == null) // nu e in planul de conturi => il adaug
            {
                // caut sinteticul
                var syntheticAccount = Context.Account.FirstOrDefault(f => f.Status == State.Active && f.Symbol == autoAccount.AccountRad);
                if (syntheticAccount == null)
                {
                    throw new Exception("Contul sintetic " + autoAccount.AccountRad + " nu este definit in planul de conturi.");
                }

                if (accountName == null)
                {
                    throw new Exception("Pentru contul " + autoAccount.AccountRad + " nu a fost definita descrierea. Verificari in Administrare -> Contabilitate -> Config conturi");
                }

                newAccount = new Account
                {
                    AccountFuncType = syntheticAccount.AccountFuncType,
                    AccountName = accountName,
                    AccountTypes = syntheticAccount.AccountTypes,
                    ComputingAccount = true,
                    CurrencyId = currencyId,
                    ThirdPartyId = (autoAccount.IncludId1 || autoAccount.ThirdPartyAccount) ? thirdPartyId : null,
                    Status = State.Active,
                    SyntheticAccountId = syntheticAccount.Id,
                    Symbol = newSymbol,
                    AccountStatus = true,
                    NivelRand = (syntheticAccount.NivelRand ?? 0) + 1
                };
                Context.Account.Add(newAccount);
                Context.SaveChanges();
            }

            return newAccount;
        }

        public int GetAutoAccountActivityType(string symbol, int? thirdPartyId, int? activityTypeId, DateTime operationDate, int currencyId, int? imoAssetStorage)
        {
            try
            {


                var ret = new Account();

                var count = Context.AccountConfig.Count(f => f.Symbol == symbol && f.ValabilityDate <= operationDate);

                var account = Context.Account.FirstOrDefault(f => f.Status == State.Active && f.Symbol == symbol);

                // caut analiticul 
                ret = Context.Account.FirstOrDefault(f => f.Status == State.Active && f.SyntheticAccountId == account.Id && f.ActivityTypeId == activityTypeId && f.CurrencyId == currencyId);
                if (ret == null)
                {
                    ret = account;// throw new Exception("Contul cu simbolul " + symbol + " nu a fost gasit in planul de conturi.");
                }
                if (!ret.ComputingAccount)
                {
                    throw new Exception("Contul cu simbolul " + symbol + " nu este marcat in planul de conturi ca un cont in care se pot inregistra operatii contabile. Verificati configurarea conturilor pentru contarile automate si planul de conturi.");
                }


                //if (count == 0) // daca nu e definit in configurarea conturilor automate => caut contul dupa simbol direct in planul de conturi
                //{
                //    //caut sinteticul

                //}
                //else // este definit in configurare => reconstitui simbolul contului
                //{
                //    var autoAccountList = Context.AccountConfig
                //                                 .Where(f => f.Symbol == symbol && f.ValabilityDate <= operationDate)
                //                                 .OrderByDescending(f => f.ValabilityDate)
                //                                 .ToList();
                //    if (imoAssetStorage != null)
                //    {
                //        autoAccountList = autoAccountList.Where(f => f.ImoAssetStorageId == imoAssetStorage).ToList();
                //    }

                //    var autoAccount = autoAccountList.OrderByDescending(f => f.ValabilityDate)
                //                                      .FirstOrDefault();
                //    if (autoAccount == null)
                //    {
                //        throw new Exception("Nu a fost identificata nici o configurare pentru contul " + symbol + " care sa indeplineasca conditiile");
                //    }

                //    string newSymbol = "";
                //    newSymbol = autoAccount.AccountRad;

                //    // e cont de tert => caut contul cu sinteticul accountRad si tertul specificat
                //    if (autoAccount.ThirdPartyAccount)
                //    {
                //        var accountThirdParty = Context.Account.Include(f => f.SyntheticAccount)
                //                                               .FirstOrDefault(f => f.SyntheticAccount.Symbol == autoAccount.AccountRad && f.AccountStatus && f.ComputingAccount
                //                                                                    && f.ThirdPartyId == thirdPartyId && f.CurrencyId == currencyId && f.ActivityTypeId == activityTypeId);
                //        if (accountThirdParty != null)
                //        {
                //            return accountThirdParty.Id;
                //        }
                //    }

                //    ret = GenerateAccount(autoAccount, currencyId, thirdPartyId);

                //}
                return ret.Id;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }


        // sterg notele contabile pentru factura selectata
        public void InvoiceDeleteConta(int invoiceId, DateTime lastBalanceDate, int? invoiceElementsType)
        {
            try
            {
                var operDetailsIdList = Context.InvoiceDetails.Include(f => f.InvoiceElementsDetails).Where(f => f.InvoicesId == invoiceId && f.ContaOperationDetailId != null
                                                                         && f.InvoiceElementsDetails.InvoiceElementsType == (InvoiceElementsType)invoiceElementsType)
                                                              .Select(f => f.ContaOperationDetailId)
                                                              .ToList();
                var operList = Context.OperationsDetails.Include(f => f.Operation).Where(f => operDetailsIdList.Contains(f.Id)).ToList();
                foreach (var item in operList)
                {
                    var operation = Context.Operations.FirstOrDefault(f => f.Id == item.OperationId);
                    if (operation.OperationDate <= lastBalanceDate)
                    {
                        throw new Exception("Nu puteti sterge notele deoarece luna contabila este inchisa.");
                    }
                    if (operation.OperationStatus == OperationStatus.Checked)
                    {
                        throw new Exception("Nu puteti sterge nota contabila nr. " + operation.Id + " deoarece este validata");
                    }
                    operation.State = State.Inactive;
                    var invoiceDetails = Context.InvoiceDetails.Where(f => f.ContaOperationDetailId == item.Id);
                    foreach (var invoiceDetailItem in invoiceDetails)
                    {
                        invoiceDetailItem.ContaOperationDetailId = null;
                    }

                    // sterg si eventuala operatie copil provenita din valuta
                    var operationChildList = Context.Operations.Where(f => f.OperationParentId == operation.Id);
                    foreach (var operationChild in operationChildList)
                    {
                        operationChild.State = State.Inactive;
                    }
                }
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString()/*.Message*/);
            }
        }

        public int CheckExistingConta(int invoiceId, DateTime operationDate, int currencyId)
        {
            var ret = 0; // exista
            var operDetailsIdList = Context.InvoiceDetails.Include(f => f.InvoiceElementsDetails).Where(f => f.InvoicesId == invoiceId && f.ContaOperationDetailId != null
                                                                        && f.InvoiceElementsDetails.InvoiceElementsType == InvoiceElementsType.CheltuieliInAvans)
                                                             .Select(f => f.ContaOperationDetailId)
                                                             .ToList();
            var operList = Context.OperationsDetails.Include(f => f.Operation).Where(f => operDetailsIdList.Contains(f.Id) && f.Operation.State == State.Active).Count();
            if (operList == 0)
            {
                ret = 1;
            }

            return ret;
        }

        #endregion

        #region Decont
        public void SaveDecontDirectToConta(int decontId, DateTime operationDate, int localCurrencyId, string decontType, int thirdPartyId, string documentTypeNameShort)
        {
            var decont = Context.Decont.Include(f => f.Currency).Include(f => f.DiurnaLegala).Include(f=>f.ThirdParty).FirstOrDefault(f => f.Id == decontId && f.State == State.Active);

            var invoices = Context.Invoices.Include(f => f.InvoiceDetails).ThenInclude(f => f.InvoiceElementsDetails).Include(f => f.ThirdParty).Where(f => f.DecontId == decont.Id && f.State == State.Active);

            decimal sum = 0;
            var operationChild = new Operation(); // se foloseste in cazul in care avem documente in valuta


            var documentType = Context.DocumentType.FirstOrDefault(f => f.TypeNameShort == documentTypeNameShort);
            if (documentType == null)
                throw new Exception("Nu a fost identificat tipul de document pentru inregistrarea facturilor sau bonurilor. Acesta trebuie sa aiba codul FF sau BF!");

            var operation = new Operation
            {
                CurrencyId = localCurrencyId,
                OperationDate = operationDate,
                DocumentTypeId = documentType.Id,
                DocumentNumber = decont.DecontNumber.ToString(),
                DocumentDate = decont.DecontDate,
                OperationStatus = OperationStatus.Unchecked,
                State = State.Active,
                ExternalOperation = true
            };

            Context.Operations.Add(operation);
            Context.SaveChanges();

            decimal exchangeRate = 1;
            if (decont.CurrencyId != localCurrencyId)
            {
                operationChild = new Operation // pentru operatiile in valuta... in operation am valorile in lei, iar in operationChild am valorile in valuta
                {
                    CurrencyId = decont.CurrencyId,
                    OperationDate = operationDate,
                    DocumentTypeId = documentType.Id,
                    DocumentNumber = decont.DecontNumber.ToString(),
                    DocumentDate = decont.DecontDate,
                    OperationStatus = OperationStatus.Unchecked,
                    State = State.Active,
                    ExternalOperation = true,
                    OperationParentId = operation.Id
                };
                Context.Operations.Add(operationChild);
                Context.SaveChanges();
            }

            // iau contul tertului
            var tipContThirdParty = (decont.DecontType == DecontType.Card ? AccountFuncType.DecontCard : AccountFuncType.DecontCasa);
            var account = Context.Account.Where(f => f.Status == State.Active && f.ThirdPartyId == thirdPartyId && f.AccountFuncType == tipContThirdParty && f.CurrencyId == decont.CurrencyId)
                                                 .FirstOrDefault();
            if (account == null) // nu are analitic => iau sinteticul
            {
                account = Context.Account.Include(f => f.AnalyticAccounts)
                                         .Where(f => f.Status == State.Active && f.AccountFuncType == tipContThirdParty && f.AnalyticAccounts.Count != 0)
                                         .FirstOrDefault();
            }

            if (account == null) // daca nu a fost idetificat nici sinteticul -> afisez eroare
            {
                throw new Exception("Nu a fost identificat in planul de conturi un cont cu moneda si tipul decontului specific tertului");
            }
            var thirdPartyAccountId = GetAutoAccountForDecont(account.Symbol, thirdPartyId, operationDate, decont.CurrencyId, decont.DecontType, null);

            // contez facturile
            foreach (var item in invoices.ToList())
            {
                if (item.InvoiceDetails.Count() == 0)
                {
                    return;
                }

                exchangeRate = _exchangeRatesRepository.GetExchangeRateForOper(item.InvoiceDate, decont.CurrencyId, localCurrencyId);

                var detailList = new List<OperationDetails>();
                foreach (var detail in item.InvoiceDetails.Where(f => f.DurationInMonths == 0 && f.InvoiceElementsDetails.InvoiceElementsType != InvoiceElementsType.MijloaceFixe &&
                                                                      f.InvoiceElementsDetails.InvoiceElementsType != InvoiceElementsType.ObiecteDeInventar))
                {

                    var operationDetailChild = new OperationDetails(); // pentru operatiile in valuta
                    var operationDetail = new OperationDetails();
                    int pozitieSchimbAccountId = 0;
                    int contravaloarePozitieSchimbAccountId = 0;

                    if (item.CurrencyId != localCurrencyId)
                    {
                        operationDetail = new OperationDetails
                        {
                            ValueCurr = (localCurrencyId != item.CurrencyId) ? (detail.ValoareTotalaDetaliu.Value + detail.VAT) : 0,
                            Value = (detail.ValoareTotalaDetaliu.Value + detail.VAT) * exchangeRate,
                            VAT = detail.ProcVAT,
                            Details = detail.Element + ", " + item.ThirdParty.FullName + ", " + item.InvoiceSeries + " " + item.InvoiceNumber + " / " + item.InvoiceDate.ToShortDateString() +
                          ", " + detail.InvoiceElementsDetails.Description + ", " +
                            (item.StartDatePeriod.HasValue && item.EndDatePeriod.HasValue ? item.StartDatePeriod.Value.ToShortDateString() + " - " + item.EndDatePeriod.Value.ToShortDateString()
                                                                                          : "")
                            + (item.CurrencyId != localCurrencyId ? (", Curs valutar: " + exchangeRate.ToString()) : "")
                        };
                    }
                    else
                    {
                        operationDetail = new OperationDetails
                        {
                            ValueCurr = (localCurrencyId != item.CurrencyId) ? (detail.Value + detail.VAT) : 0,
                            Value = (localCurrencyId != item.CurrencyId) ? ((detail.ValoareTotalaDetaliu ?? 0) * exchangeRate) : (detail.ValoareTotalaDetaliu ?? 0),
                            VAT = detail.ProcVAT,
                            Details = detail.Element + ", " + item.ThirdParty.FullName + ", " + item.InvoiceSeries + " " + item.InvoiceNumber + " / " + item.InvoiceDate.ToShortDateString() + ", " + detail.InvoiceElementsDetails.Description + ", " +
                     (item.StartDatePeriod.HasValue && item.EndDatePeriod.HasValue ? item.StartDatePeriod.Value.ToShortDateString() + " - " + item.EndDatePeriod.Value.ToShortDateString()
                                                                                   : "")
                     + (item.CurrencyId != localCurrencyId ? (", Curs valutar: " + exchangeRate.ToString()) : "")
                        };

                    }

                    var element = Context.InvoiceElementsDetails.FirstOrDefault(f => f.Id == detail.InvoiceElementsDetailsId);
                    if (element.ThirdPartyAccount == null || element.CorrespondentAccount == null)
                        throw new Exception("Conturile pentru terti si conturile corespondente trebuie completate. Verificati daca elementele facturii sunt completate in Economic -> Facturi -> Elemente");



                    int directAccountId = 0;
                    string correspAccount = (detail.DurationInMonths != 0 ? element.CorrespondentAccount : element.ExpenseAmortizAccount);
                    if (correspAccount == null)
                    {
                        throw new Exception("Nu a fost specificat contul de cheltuiala directa la definirea tipurilor de cheltuiala");
                    }

                    if (correspAccount.StartsWith('6') && item.ActivityTypeId != null)
                    {
                        directAccountId = GetAutoAccountActivityType(correspAccount, item.ThirdPartyId, item.ActivityTypeId.Value, operationDate, item.CurrencyId, null);
                    }
                    else
                    {
                        directAccountId = GetAutoAccount(correspAccount, item.ThirdPartyId, operationDate, item.CurrencyId, null);
                    }

                    if (item.ThirdPartyQuality == ThirdPartyQuality.Client) // Facturi de la clienti
                    {
                        operationDetail.DebitId = contravaloarePozitieSchimbAccountId;
                        operationDetail.CreditId = directAccountId;
                    }
                    else
                    {
                        operationDetail.DebitId = directAccountId;
                        operationDetail.CreditId = contravaloarePozitieSchimbAccountId;
                    }
                    operationDetail.OperationId = operationChild.Id;
                    Context.OperationsDetails.Add(operationDetail);

                    // operatie in valuta => iau pozitia de schimb si contravaloarea si inregistrez operatia copil
                    if (decont.CurrencyId != localCurrencyId)
                    {
                        operationDetailChild = new OperationDetails
                        {
                            ValueCurr = (localCurrencyId != decont.CurrencyId) ? (detail.Value + detail.VAT) : 0,
                            Value = (detail.Value + detail.VAT),
                            VAT = detail.ProcVAT,
                            Details = detail.Element + ", " + item.ThirdParty.FullName + ", " + item.InvoiceSeries + " " + item.InvoiceNumber + " / " + item.InvoiceDate.ToShortDateString() + ", " +
                                  (item.StartDatePeriod.HasValue && item.EndDatePeriod.HasValue ? item.StartDatePeriod.Value.ToShortDateString() + " - " + item.EndDatePeriod.Value.ToShortDateString()
                                                                                                : "")
                        };
                        sum += (detail.Value + detail.VAT);

                        var activityTypeId = item.ActivityTypeId ?? GetMainActivityType();
                        // iau pozitia de schimb valutar
                        try
                        {

                            pozitieSchimbAccountId = _accountRepository.GetAll().FirstOrDefault(f => f.AccountFuncType == AccountFuncType.PozitieSchimbCheltuieli && f.CurrencyId == item.CurrencyId
                                                                                                && f.ActivityTypeId == activityTypeId).Id;
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Nu am identificat contul pentru pozitia de schimb valutar");
                        }
                        // iau contravaloarea pozitiei de schimb valutar
                        try
                        {
                            var denumireMoneda = _currencyRepository.GetCurrencyById(item.CurrencyId).CurrencyCode;
                            contravaloarePozitieSchimbAccountId = _accountRepository.GetAll().FirstOrDefault(f => f.AccountFuncType == AccountFuncType.ContravaloarePozitieSchimbCheltuieli
                                                                                                             && f.ActivityTypeId == activityTypeId
                                                                                                             && f.AccountName.IndexOf(denumireMoneda) >= 0).Id;
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Nu am identificat contul pentru contravaloarea pozitiei de schimb valutar");
                        }

                        if (item.ThirdPartyQuality == ThirdPartyQuality.Client) // Facturi de la clienti
                        {
                            operationDetailChild.DebitId = thirdPartyAccountId;
                            operationDetailChild.CreditId = pozitieSchimbAccountId;
                            operationDetail.DebitId = contravaloarePozitieSchimbAccountId;
                            operationDetail.CreditId = directAccountId;
                        }
                        else
                        {
                            operationDetailChild.DebitId = pozitieSchimbAccountId;
                            operationDetailChild.CreditId = thirdPartyAccountId;
                            operationDetail.DebitId = directAccountId;
                            operationDetail.CreditId = contravaloarePozitieSchimbAccountId;
                        }
                        operationDetailChild.OperationId = operationChild.Id;
                        Context.OperationsDetails.Add(operationDetailChild);

                    }
                    else
                    {
                        if (item.ThirdPartyQuality == ThirdPartyQuality.Client) // Facturi de la clienti
                        {
                            operationDetail.DebitId = thirdPartyAccountId;
                            operationDetail.CreditId = directAccountId;
                        }
                        else
                        {
                            operationDetail.DebitId = directAccountId;
                            operationDetail.CreditId = thirdPartyAccountId;
                        }
                    }

                    operationDetail.OperationId = operation.Id;

                    bool okAddNededOper = true;
                    var nededOperDetail = GenerateNededOperDetail(ref operationDetail, operationDate, directAccountId, out okAddNededOper);


                    detailList.Add(operationDetail);
                    Context.OperationsDetails.Add(operationDetail);

                    if (okAddNededOper)
                    {
                        detailList.Add(nededOperDetail);
                        Context.OperationsDetails.Add(nededOperDetail);
                    }

                    Context.SaveChanges();
                    detail.ContaOperationDetailId = operationDetail.Id;
                    //detail.Value = sum;
                }
            }

            // contez diurna
            if (decont.TotalDiurnaAcordata != 0)
            {
                int pozitieSchimbAccountId = 0;
                int contravaloarePozitieSchimbAccountId = 0;

                exchangeRate = _exchangeRatesRepository.GetExchangeRateForOper(decont.DecontDate, decont.CurrencyId, localCurrencyId);
                var operationDetail = new OperationDetails
                {
                    ValueCurr = (localCurrencyId != decont.CurrencyId) ? decont.TotalDiurnaAcordata : 0,
                    Value = decont.TotalDiurnaAcordata * exchangeRate,
                    VAT = 0,
                    Details = "Cheltuieli cu deplasarea in perioada " + LazyMethods.DateToString(decont.DateStart.Value) + " - " + LazyMethods.DateToString(decont.DateEnd.Value)
                              + ", Decont " + decont.ThirdParty.FullName + ", Total diurna acordata: " + decont.TotalDiurnaAcordata.ToString("N2") + " " + decont.Currency.CurrencyName
                              + (decont.CurrencyId != localCurrencyId ? (", Curs valutar: " + exchangeRate.ToString()) : "")
                };
                int directAccountId = 0;
                var scopDeplasare = decont.ScopDeplasareType;
                var diurnaType = DiurnaType.Interna;
                var country = Context.Country.FirstOrDefault(f => f.Id == decont.DiurnaLegala.CountryId).CountryAbrv;
                if (country != "ROM")
                {
                    diurnaType = DiurnaType.Externa;
                }
                var expenseAccountList = Context.AccountConfig.Where(f => f.Symbol == "6345" && f.ScopDeplasareType == scopDeplasare);
                if (scopDeplasare == ScopDeplasareType.Deplasare)
                {
                    expenseAccountList = expenseAccountList.Where(f => f.DiurnaType == diurnaType);
                }
                var expenseAccount = expenseAccountList.FirstOrDefault();
                var expenseSymbol = expenseAccount.AccountRad + "." + expenseAccount.Sufix;
                directAccountId = Context.Account.FirstOrDefault(f => f.Symbol == expenseSymbol).Id;
                var operationDetailChild = new OperationDetails();

                if (decont.CurrencyId != localCurrencyId)
                {
                    operationDetailChild = new OperationDetails
                    {
                        ValueCurr = (localCurrencyId != decont.CurrencyId) ? decont.TotalDiurnaAcordata : 0,
                        Value = decont.TotalDiurnaAcordata,
                        VAT = 0,
                        Details = "Cheltuieli cu deplasarea in perioada " + LazyMethods.DateToString(decont.DateStart.Value) + " - " + LazyMethods.DateToString(decont.DateEnd.Value)
                                  + ", Decont " + decont.ThirdParty.FullName + ", Total diurna acordata: " + decont.TotalDiurnaAcordata.ToString("N2") + " " + decont.Currency.CurrencyName
                    };
                    sum += decont.TotalDiurnaAcordata;

                    var activityTypeId = GetMainActivityType();
                    // iau pozitia de schimb valutar
                    try
                    {
                        pozitieSchimbAccountId = _accountRepository.GetAll().FirstOrDefault(f => f.AccountFuncType == AccountFuncType.PozitieSchimbCheltuieli && f.CurrencyId == decont.CurrencyId
                                                                                            && f.ActivityTypeId == activityTypeId).Id;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Nu am identificat contul pentru pozitia de schimb valutar");
                    }
                    // iau contravaloarea pozitiei de schimb valutar
                    try
                    {
                        var denumireMoneda = _currencyRepository.GetCurrencyById(decont.CurrencyId).CurrencyCode;
                        contravaloarePozitieSchimbAccountId = _accountRepository.GetAll().FirstOrDefault(f => f.AccountFuncType == AccountFuncType.ContravaloarePozitieSchimbCheltuieli
                                                                                                         && f.ActivityTypeId == activityTypeId
                                                                                                         && f.AccountName.IndexOf(denumireMoneda) >= 0).Id;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Nu am identificat contul pentru contravaloarea pozitiei de schimb valutar");
                    }

                    operationDetailChild.DebitId = pozitieSchimbAccountId;
                    operationDetailChild.CreditId = thirdPartyAccountId;
                    operationDetail.DebitId = directAccountId;
                    operationDetail.CreditId = contravaloarePozitieSchimbAccountId;
                    operationDetailChild.OperationId = operationChild.Id;
                    Context.OperationsDetails.Add(operationDetailChild);

                }
                else
                {
                    operationDetail.DebitId = directAccountId;
                    operationDetail.CreditId = thirdPartyAccountId;
                }

                operationDetail.OperationId = operation.Id;

                bool okAddNededOper = true;
                var nededOperDetail = GenerateNededOperDetail(ref operationDetail, operationDate, directAccountId, out okAddNededOper);

                Context.OperationsDetails.Add(operationDetail);

                if (okAddNededOper)
                {
                    Context.OperationsDetails.Add(nededOperDetail);
                }
                Context.SaveChanges();
            }

            var operationEnd = Context.Operations.Include(f => f.OperationsDetails).FirstOrDefault(f => f.Id == operation.Id);
            if (operationEnd.OperationsDetails.Count == 0)
            {
                operationEnd.State = State.Inactive;
                operationChild.State = State.Inactive;
            }

            if (invoices.Count() == 0) // nu am facturi inregistrate in decont => adaug id-ul operatiei contabile in decont
            {
                decont.OperationId = operation.Id;
            }

            Context.SaveChanges();
        }

        public int GetAutoAccountForDecont(string symbol, int? thirdPartyId, DateTime operationDate, int currencyId, DecontType decontType, int? imoAssetStorage)
        {
            var ret = new Account();

            var count = Context.AccountConfig.Count(f => f.Symbol == symbol && f.ValabilityDate <= operationDate);

            if (count == 0) // daca nu e definit in configurarea conturilor automate => caut contul dupa simbol direct in planul de conturi
            {
                ret = Context.Account.FirstOrDefault(f => f.Status == State.Active && f.Symbol == symbol);
                if (ret == null)
                {
                    throw new Exception("Contul cu simbolul " + symbol + " nu a fost gasit in planul de conturi.");
                }
                if (!ret.ComputingAccount)
                {
                    throw new Exception("Contul cu simbolul " + symbol + " nu este marcat in planul de conturi ca un cont in care se pot inregistra operatii contabile. Verificati configurarea conturilor pentru contarile automate si planul de conturi.");
                }
            }
            else // este definit in configurare => reconstitui simbolul contului
            {
                var autoAccountList = Context.AccountConfig
                                             .Where(f => f.Symbol == symbol && f.ValabilityDate <= operationDate)
                                             .OrderByDescending(f => f.ValabilityDate)
                                             .ToList();
                if (imoAssetStorage != null)
                {
                    autoAccountList = autoAccountList.Where(f => f.ImoAssetStorageId == imoAssetStorage).ToList();
                }

                var autoAccount = autoAccountList.OrderByDescending(f => f.ValabilityDate)
                                                  .FirstOrDefault();
                if (autoAccount == null)
                {
                    throw new Exception("Nu a fost identificata nici o configurare pentru contul " + symbol + " care sa indeplineasca conditiile");
                }

                string newSymbol = "";
                newSymbol = autoAccount.AccountRad;

                // e cont de tert => caut contul cu sinteticul accountRad si tertul specificat
                if (autoAccount.ThirdPartyAccount)
                {
                    var tipCont = (decontType == DecontType.Card ? AccountFuncType.DecontCard : AccountFuncType.DecontCasa);
                    var accountThirdParty = Context.Account.Include(f => f.SyntheticAccount)
                                                           .FirstOrDefault(f => f.SyntheticAccount.Symbol == autoAccount.AccountRad && f.AccountStatus && f.ComputingAccount
                                                                                && f.ThirdPartyId == thirdPartyId && f.CurrencyId == currencyId && f.Status == State.Active
                                                                                && f.AccountFuncType == tipCont);
                    if (accountThirdParty != null)
                    {
                        return accountThirdParty.Id;
                    }
                }

                ret = GenerateAccount(autoAccount, currencyId, thirdPartyId);

            }
            return ret.Id;
        }


        public void DeleteInvoicesFromDecont(int decontId, int tenantId)
        {
            var invoice = Context.Invoices.Include(f => f.InvoiceDetails).ThenInclude(f => f.InvoiceElementsDetails).Where(f => f.DecontId == decontId && f.State == State.Active/* && f.InvoiceDetails.Any(g => g.ContaOperationDetailId != null)*/);
            var lastBalanceDate = Context.Balance.Where(f => f.TenantId == tenantId && f.Status == State.Active).Max(g => g.BalanceDate);
            foreach (var item in invoice.ToList())
            {
                InvoiceDeleteConta(item.Id, lastBalanceDate, item.InvoiceDetails.Select(f => (int)f.InvoiceElementsDetails.InvoiceElementsType).FirstOrDefault());
                item.State = State.Inactive;
            }
        }

        public void RemoveContaInvoicesFromDecont(int decontId, int tenantId)
        {
            var invoice = Context.Invoices.Include(f => f.InvoiceDetails).ThenInclude(f => f.InvoiceElementsDetails).Where(f => f.DecontId == decontId && f.State == State.Active/* && f.InvoiceDetails.Any(g => g.ContaOperationDetailId != null)*/);
            var lastBalanceDate = Context.Balance.Where(f => f.TenantId == tenantId && f.Status == State.Active).Max(g => g.BalanceDate);
            foreach (var item in invoice.ToList())
            {
                InvoiceDeleteConta(item.Id, lastBalanceDate, item.InvoiceDetails.Select(f => (int)f.InvoiceElementsDetails.InvoiceElementsType).FirstOrDefault());
            }
        }
        #endregion

        //public void TaxProfitOperationAdd(int compId, int documentTypeId, string documentNumber, DateTime documentDate, int appClientId)
        //{
        //    try
        //    {
        //        var compItem = Context.TaxProfitComp.FirstOrDefault(f => f.Id == compId);
        //        DateTime compDate = new DateTime(compItem.ComputeDate.Year, compItem.ComputeDate.Month, compItem.ComputeDate.Day);

        //        var lastBalanceDate = Context.Balance.Where(f => f.AppClientId == appClientId && f.Status == State.Active).Max(g => g.BalanceDate);

        //        var localCurrecyId = Context.Clients.FirstOrDefault(f => f.Id == appClientId).LocalCurrencyId;

        //        if (compDate <= lastBalanceDate)
        //        {
        //            throw new Exception("Nu puteti sa generati note contabile pentru data " + LazyMethods.DateToString(compDate) + " deoarece luna contabila este inchisa");
        //        }

        //        var monog = Context.AutoOperationConfig
        //                                   .Where(f => f.State == State.Active && compDate >= f.StartDate && compDate <= (f.EndDate ?? compDate)
        //                                          && f.AutoOperType == AutoOperationType.ImpozitPeProfit && f.OperationType == 0 && f.AppClientId == appClientId)
        //                                   .OrderBy(f => f.EntryOrder)
        //                                   .ToList();
        //        if (monog.Count == 0)
        //        {
        //            throw new Exception("Nu exista o monografie definita pentru aceasta operatie");
        //        }

        //        var individualOper = monog.FirstOrDefault().IndividualOperation;

        //        var operation = new Operation
        //        {
        //            AppClientId = appClientId,
        //            CurrencyId = localCurrecyId,
        //            OperationDate = compDate,
        //            DocumentTypeId = documentTypeId,
        //            DocumentNumber = documentNumber,
        //            DocumentDate = documentDate,
        //            OperationStatus = OperationStatus.Unchecked,
        //            State = State.Active,
        //            ExternalOperation = true
        //        };

        //        var detailList = new List<OperationDetails>();

        //        foreach (var item in monog)
        //        {
        //            decimal value = 0;
        //            var debitId = GetAutoAccount(item.DebitAccount, null, compDate, appClientId, localCurrecyId, null);
        //            var creditId = GetAutoAccount(item.CreditAccount, null, compDate, appClientId, localCurrecyId, null);
        //            var taxProfitElement = (TaxProfitElement)item.ElementId;

        //            var compDetItem = Context.TaxProfitCompDet.Include(f => f.TaxProfitConfigDet)
        //                                                      .FirstOrDefault(f => f.TaxProfitCompId == compId && f.TaxProfitConfigDet.AutoOperationElement == taxProfitElement);
        //            if (compDetItem != null)
        //            {
        //                value = (taxProfitElement == TaxProfitElement.ImpozitPeProfitDeclarat ? -1 : 1) * compDetItem.Col1.Value;
        //            }
        //            if (value != 0)
        //            {
        //                var operDetail = new OperationDetails
        //                {
        //                    DebitId = debitId,
        //                    CreditId = creditId,
        //                    Value = value,
        //                    ValueCurr = 0,
        //                    VAT = 0,
        //                    Details = item.Details,
        //                    DetailNr = item.EntryOrder
        //                };
        //                detailList.Add(operDetail);
        //            }
        //        }

        //        operation.OperationsDetails = detailList;

        //        Context.Operations.Add(operation);
        //        Context.SaveChanges();

        //        compItem.ContaOperationId = operation.Id;
        //        Context.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        // Contari automate dispozitii de plata
        public void DispositionToConta(int dispositionId, DateTime dispositionDate, int localCurrencyId)
        {
            var disposition = Context.Dispositions.Include(f => f.DispositionInvoices).Include(f => f.InvoiceElementsDetails).ThenInclude(f => f.InvoiceElementsDetailsCategory)
                                                  .FirstOrDefault(f => f.Id == dispositionId);

            var documentType = Context.DocumentType.FirstOrDefault(f => f.TypeNameShort == (disposition.OperationType == Models.Economic.Casierii.OperationType.Incasare ? "DI" : "DP"));

            if (documentType == null)
            {
                throw new Exception("Nu a fost identificat tipul de document pentru inregistrarea dispozitiilor.");
            }

            var operation = new Operation
            {
                CurrencyId = disposition.CurrencyId,
                OperationDate = dispositionDate,
                DocumentTypeId = documentType.Id,
                DocumentNumber = disposition.DispositionNumber.ToString(),
                DocumentDate = disposition.DispositionDate,
                OperationStatus = OperationStatus.Unchecked,
                State = State.Active,
                ExternalOperation = true
            };

            Context.Operations.Add(operation);
            Context.SaveChanges();

            var detailList = new List<OperationDetails>();

            var operationDetail = new OperationDetails
            {
                Value = disposition.Value,
                Details = disposition.Description
            };

            var element = Context.InvoiceElementsDetails.FirstOrDefault(f => f.Id == disposition.InvoiceElementsDetailsId);
            if (element.ThirdPartyAccount == null)
                throw new Exception("Conturile corespondente trebuie completate. Verificati daca elementele dispozitiei sunt completate in Conta -> Nomenclatoare -> Tipuri de cheltuieli");

            var casierieAccountId = Context.Account.FirstOrDefault(f => f.AccountFuncType == AccountFuncType.Casierie && f.CurrencyId == disposition.CurrencyId && f.Status == State.Active && f.ComputingAccount).Id;
            var thirdPartyAccountId = GetAutoAccount(element.ThirdPartyAccount, disposition.ThirdPartyId, dispositionDate, disposition.CurrencyId, null);

            if (disposition.OperationType == Models.Economic.Casierii.OperationType.Incasare)
            {
                operationDetail.DebitId = casierieAccountId;
                operationDetail.CreditId = thirdPartyAccountId;
            }
            else
            {
                operationDetail.DebitId = thirdPartyAccountId;
                operationDetail.CreditId = casierieAccountId;
            }

            operationDetail.OperationId = operation.Id;

            bool okAddNededOper = true;
            var nededOperDetail = GenerateNededOperDetail(ref operationDetail, dispositionDate, thirdPartyAccountId, out okAddNededOper);

            detailList.Add(operationDetail);
            Context.OperationsDetails.Add(operationDetail);

            if (okAddNededOper)
            {
                detailList.Add(nededOperDetail);
                Context.OperationsDetails.Add(nededOperDetail);
            }

            Context.SaveChanges();

            disposition.OperationId = operation.Id;
            Context.Dispositions.Update(disposition);
            Context.SaveChanges();
        }

        public void RepartizareCheltuieli(DateTime dataEnd, int appClientId, int localCurrencyId, int operGenId)
        {
            try
            {
                var dataStart = LazyMethods.LastDayOfMonth(dataEnd.AddMonths(-1)).AddDays(1);

                // iau veniturile
                var incomeList = Context.OperationsDetails.Include(f => f.Operation).Include(f => f.Credit)
                                                          .Where(f => f.Operation.OperationDate <= dataEnd && f.Operation.OperationDate >= dataStart && f.Operation.State == State.Active
                                                                 && f.Operation.TenantId == appClientId && f.Credit.Symbol.IndexOf("7") == 0)
                                                          .GroupBy(f => f.Credit.ActivityTypeId)
                                                          .Select(f => new { ActivityType = f.Key, Value = f.Sum(g => g.Value) })
                                                          .ToList();
                var totalIncome = incomeList.Sum(f => f.Value);

                //--------------------
                var activityTypesList = incomeList.Where(f => f.ActivityType != null).Select(f => f.ActivityType).Distinct().ToList();
                var nrActivityType = activityTypesList.Count;
                int[] vActivityType = new int[nrActivityType];
                decimal[] vIncomeProc = new decimal[nrActivityType];
                decimal incomeProcSum = 0;
                decimal[] vExpenseValue = new decimal[nrActivityType];
                var mainActivityId = GetMainActivityType();

                int index = 0;
                // calculez procentul veniturilor pentru activitatile neprincipale
                foreach (var activityType in activityTypesList.Where(f => f != mainActivityId))
                {
                    vActivityType[index] = activityType.Value;
                    decimal incomeProc = incomeList.FirstOrDefault(f => f.ActivityType == activityType.Value).Value / totalIncome;
                    incomeProcSum += incomeProc;
                    vIncomeProc[index] = incomeProc;
                    index++;
                }
                //calculez procentul veniturilor pentru activitatea principala
                vActivityType[index] = mainActivityId;
                vIncomeProc[index] = 1 - incomeProcSum;

                // iau cheltuielile care nu au activitate stabilita
                var expenseList = Context.OperationsDetails.Include(f => f.Operation).Include(f => f.Debit)
                                                           .Where(f => f.Operation.OperationDate <= dataEnd && f.Operation.OperationDate >= dataStart && f.Operation.State == State.Active
                                                                  && f.Operation.TenantId == appClientId && f.Debit.Symbol.IndexOf("6") == 0 && f.Debit.ActivityTypeId == null)
                                                           .ToList();
                // parcurg cheltuielile si le impart in functie de procentul veniturilor pe fiecare activitate in parte
                foreach (var operDetail in expenseList)
                {
                    // stornez operatia inregistrata
                    var operDetailStorno = new OperationDetails
                    {
                        OperationId = operDetail.OperationId,
                        DebitId = operDetail.DebitId,
                        CreditId = operDetail.CreditId,
                        Details = "Storno - " + operDetail.Details,
                        Value = -1 * operDetail.Value,
                        OperGenerateId = operGenId
                    };
                    Context.OperationsDetails.Add(operDetailStorno);

                    decimal totalValue = operDetail.Value;
                    decimal remainingValue = totalValue;
                    for (int i = 0; i <= index; i++)
                    {
                        decimal value = 0;
                        if (index != i) // nu e activitatea principala
                        {
                            value = Math.Round(vIncomeProc[i] * totalValue, 2);
                            remainingValue -= value;
                        }
                        else
                        {
                            value = remainingValue;
                        }

                        var activityType = Context.ActivityTypes.FirstOrDefault(f => f.Id == vActivityType[i]);
                        var account = Context.Account.Include(f => f.SyntheticAccount)
                                                     .Where(f => f.TenantId == appClientId && f.AccountStatus && f.SyntheticAccount.Id == operDetail.DebitId && f.ActivityTypeId == vActivityType[i]
                                                            && f.ComputingAccount && f.AccountStatus && f.Status == State.Active)
                                                     .FirstOrDefault();
                        int accountId = 0;
                        if (account == null)
                        {
                            var sinteticAccount = Context.Account.FirstOrDefault(f => f.Id == operDetail.DebitId);

                            // construiesc simbolul contului nou
                            var maxAccount = Context.Account.Where(f => f.SyntheticAccountId == sinteticAccount.Id).OrderByDescending(f => f.Symbol).FirstOrDefault();
                            string newSymbol = sinteticAccount.Symbol;
                            if (maxAccount == null)
                            {
                                newSymbol += "." + "1".PadLeft(2, '0');
                            }
                            else
                            {
                                var maxSymbol = maxAccount.Symbol;
                                var substrSymbol = maxSymbol.Substring(newSymbol.Length + 1);
                                if (substrSymbol.IndexOf(".") >= 0)
                                {
                                    substrSymbol = substrSymbol.Substring(0, substrSymbol.IndexOf("."));
                                    var nextItem = int.Parse(substrSymbol);
                                    nextItem++;
                                    newSymbol += "." + nextItem.ToString().PadLeft(2, '0');
                                }
                            }

                            var accountNew = new Account
                            {
                                Symbol = newSymbol,
                                AccountName = sinteticAccount.AccountName + " - " + activityType.ActivityName,
                                CurrencyId = sinteticAccount.CurrencyId,
                                AccountFuncType = sinteticAccount.AccountFuncType,
                                AccountStatus = true,
                                AccountTypes = sinteticAccount.AccountTypes,
                                ActivityTypeId = activityType.Id,
                                ComputingAccount = true,
                                Status = State.Active,
                                SyntheticAccountId = sinteticAccount.Id,
                                TaxStatus = sinteticAccount.TaxStatus,
                                TenantId = sinteticAccount.TenantId,
                                NivelRand = (sinteticAccount.NivelRand ?? 0) + 1
                            };
                            Context.Account.Add(accountNew);
                            Context.SaveChanges();
                            accountId = accountNew.Id;
                        }
                        else
                        {
                            accountId = account.Id;
                        }

                        var operDetailActivity = new OperationDetails
                        {
                            OperationId = operDetail.OperationId,
                            DebitId = accountId,
                            CreditId = operDetail.CreditId,
                            Details = activityType.ActivityName + " - " + operDetail.Details,
                            Value = value,
                            OperGenerateId = operGenId
                        };
                        Context.OperationsDetails.Add(operDetailActivity);
                    }

                }

                Context.SaveChanges();

            }
            catch (Exception ex)
            {
                throw new Exception("Eroare repartizare cheltuieli " + ex.ToString());
            }
        }

        public void InchidereVenituriCheltuieli(DateTime dataEnd, int appClientId, int localCurrencyId, int operGenId)
        {
            try
            {
                var dataStart = LazyMethods.LastDayOfMonth(dataEnd.AddMonths(-1)).AddDays(1);

                string docNumber = "";
                // definesc operatia
                var operation = new Operation();
                operation.CurrencyId = localCurrencyId;
                var doc = Context.DocumentType.Where(x => x.ClosingMonth).FirstOrDefault();
                if (doc == null)
                {
                    throw new Exception("Nu ati specificat tipul documentului folosit la operatiile de inchidere prin contul de profit sau pierdere");
                }
                operation.DocumentType = doc;
                operation.DocumentNumber = docNumber;
                operation.DocumentDate = dataEnd;
                operation.OperationDate = dataEnd;
                operation.OperationStatus = OperationStatus.Unchecked;
                operation.State = State.Active;
                operation.OperGenerateId = operGenId;
                operation.ExternalOperation = true;

                // preiau operatiile
                //Select operations between dates, with local currency
                var listCredit = new List<OperationDetails>();
                listCredit = Context.OperationsDetails.Include(f => f.Operation).Include(f => f.Credit)
                                                      .Where(x => x.Operation.OperationDate >= dataStart && x.Operation.OperationDate <= dataEnd && x.Operation.State == State.Active)
                                                      .Where(x => x.Credit.Symbol.StartsWith("7")).ToList();

                var listDebit = Context.OperationsDetails.Include(f => f.Operation).Include(f => f.Debit)
                                                      .Where(x => x.Operation.OperationDate >= dataStart && x.Operation.OperationDate <= dataEnd && x.Operation.State == State.Active)
                                                      .Where(x => x.Debit.Symbol.StartsWith("6")).ToList();

                if (listCredit.Count == 0 && listDebit.Count == 0)
                    return;

                int? detailNr = 1;

                var detailsCredit = listCredit.Where(x => x.Credit.Symbol.StartsWith("7")).GroupBy(p => new { p.Credit.Symbol }).Select(g =>
                                  new
                                  {
                                      Value = g.Sum(x => x.Value),
                                      ValueCurr = g.Sum(x => x.ValueCurr),
                                      Symbol = g.Key.Symbol
                                  }).ToList();

                var detailsDebit = listDebit.Where(x => x.Debit.Symbol.StartsWith("6")).GroupBy(p => new { p.Debit.Symbol }).Select(g =>
                                 new
                                 {
                                     Value = g.Sum(x => x.Value),
                                     ValueCurr = g.Sum(x => x.ValueCurr),
                                     Symbol = g.Key.Symbol
                                 }).ToList();

                var OpDetList = new List<OperationDetails>();

                // venituri
                foreach (var opD in detailsCredit)
                {

                    if (opD.Symbol.StartsWith("7"))
                    {
                        if (opD.Value != 0)
                        {
                            var opNew = new OperationDetails();

                            opNew.Debit = _accountRepository.GetAccountBySymbol(opD.Symbol);
                            opNew.DebitId = opNew.Debit.Id;
                            opNew.Credit = Context.Account.FirstOrDefault(f => f.ComputingAccount && f.AccountFuncType == AccountFuncType.ProfitSauPierdere
                                                                          && f.ActivityTypeId == opNew.Debit.ActivityTypeId && f.CurrencyId == localCurrencyId && f.Status == State.Active);
                            if (opNew.Credit == null)
                            {
                                throw new Exception("Nu am identificat contul de profit sau pierdere pornind de la simbolul " + opNew.Credit.Symbol);
                            }

                            opNew.CreditId = opNew.Credit.Id;
                            opNew.Details = "INCHIDERE CONT " + opD.Symbol;
                            opNew.Value = opD.Value;
                            opNew.ValueCurr = opD.ValueCurr;
                            if (doc.AutoNumber)
                            {
                                opNew.DetailNr = detailNr;
                                detailNr++;
                            }
                            OpDetList.Add(opNew);
                        }
                    }
                }

                // cheltuieli
                foreach (var opD in detailsDebit)
                {

                    if (opD.Symbol.StartsWith("6"))
                    {
                        if (opD.Value != 0)
                        {
                            var opNew = new OperationDetails();

                            opNew.Credit = _accountRepository.GetAccountBySymbol(opD.Symbol);
                            opNew.CreditId = opNew.Credit.Id;
                            opNew.Debit = Context.Account.FirstOrDefault(f => f.ComputingAccount && f.AccountFuncType == AccountFuncType.ProfitSauPierdere
                                                                              && f.ActivityTypeId == opNew.Credit.ActivityTypeId && f.CurrencyId == localCurrencyId);
                            if (opNew.Debit == null)
                            {
                                throw new Exception("Nu am identificat contul de profit sau pierdere pornind de la simbolul " + opNew.Credit.Symbol);
                            }

                            opNew.DebitId = opNew.Debit.Id;
                            opNew.Details = "INCHIDERE CONT " + opD.Symbol;
                            opNew.Value = opD.Value;
                            opNew.ValueCurr = opD.ValueCurr;
                            if (doc.AutoNumber)
                            {
                                opNew.DetailNr = detailNr;
                                detailNr++;
                            }
                            OpDetList.Add(opNew);
                        }
                    }
                }

                operation.OperationsDetails = OpDetList;
                if (OpDetList.Count > 0)
                    Context.Operations.Add(operation);
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Eroare inchidere venituri si cheltuieli " + ex.ToString());
            }
        }

        private void GetNededForAccount(DateTime date, int accountId, out int nededAccountId, out decimal procentDeduc)
        {
            nededAccountId = 0;
            procentDeduc = 0;

            var accountTax = Context.AccountTaxProperties.Where(f => f.AccountId == accountId && f.PropertyDate <= date)
                                                         .OrderByDescending(f => f.PropertyDate).ThenByDescending(f => f.Id)
                                                         .FirstOrDefault();
            if (accountTax != null)
            {
                nededAccountId = accountTax.AccountNededId ?? 0;
                procentDeduc = accountTax.PropertyValue;
            }
        }

        public OperationDetails GenerateNededOperDetail(ref OperationDetails operDetail, DateTime date, int accountId, out bool okAdd)
        {
            var ret = new OperationDetails();
            okAdd = true;

            int nededAccountId = 0; decimal dedProc = 0;
            GetNededForAccount(date, accountId, out nededAccountId, out dedProc);

            ret = new OperationDetails();

            if (nededAccountId == 0)
            {
                okAdd = false;
            }
            else
            {
                ret.VAT = operDetail.VAT;
                ret.Details = operDetail.Details;


                if (nededAccountId != 0) // exista un cont nedeductibil pentru acest contul corespondent => impart sumele
                {
                    decimal valueCurr = operDetail.ValueCurr;
                    decimal value = operDetail.Value;

                    operDetail.ValueCurr = Math.Round(valueCurr * dedProc, 2, MidpointRounding.AwayFromZero);
                    operDetail.Value = Math.Round(value * dedProc, 2, MidpointRounding.AwayFromZero);

                    ret.ValueCurr = valueCurr - operDetail.ValueCurr;
                    ret.Value = value - operDetail.Value;

                    ret.DebitId = (operDetail.DebitId == accountId) ? nededAccountId : operDetail.DebitId;
                    ret.CreditId = (operDetail.CreditId == accountId) ? nededAccountId : operDetail.CreditId;
                    ret.OperationId = operDetail.OperationId;
                }
            }

            return ret;
        }

        public int GetAccount(string symbol, int? thirdPartyId, DateTime operationDate, int currencyId)
        {
            var ret = new Account();

            if (symbol == "")
            {
                throw new Exception("Pentru elementul selectat nu a fost definit un cont de cheltuiala.");
            }

            var count = Context.AccountConfig.Count(f => f.Symbol == symbol && f.ValabilityDate <= operationDate);

            if (count == 0) // daca nu e definit in configurarea conturilor automate => caut contul dupa simbol direct in planul de conturi
            {
                ret = Context.Account.FirstOrDefault(f => f.Status == State.Active && f.Symbol == symbol);
                if (ret == null)
                {
                    throw new Exception("Contul cu simbolul " + symbol + " nu a fost gasit in planul de conturi.");
                }
            }
            else // este definit in configurare => reconstitui simbolul contului
            {
                var autoAccountList = Context.AccountConfig
                                             .Where(f => f.Symbol == symbol && f.ValabilityDate <= operationDate)
                                             .OrderByDescending(f => f.ValabilityDate)
                                             .ToList();

                var autoAccount = autoAccountList.OrderByDescending(f => f.ValabilityDate)
                                                  .FirstOrDefault();
                if (autoAccount == null)
                {
                    throw new Exception("Nu a fost identificata nici o configurare pentru contul " + symbol + " care sa indeplineasca conditiile");
                }

                string newSymbol = "";
                newSymbol = autoAccount.AccountRad;

                // e cont de tert => caut contul cu sinteticul accountRad si tertul specificat
                if (autoAccount.ThirdPartyAccount)
                {
                    var accountThirdParty = Context.Account.Include(f => f.SyntheticAccount)
                                                           .FirstOrDefault(f => f.SyntheticAccount.Symbol == autoAccount.AccountRad && f.AccountStatus && f.ComputingAccount
                                                                                && f.ThirdPartyId == thirdPartyId && f.CurrencyId == currencyId && f.Status == State.Active);
                    if (accountThirdParty != null)
                    {
                        return accountThirdParty.Id;
                    }
                }

                ret = GenerateAccount(autoAccount, currencyId, thirdPartyId);

            }
            return ret.Id;
        }

        public int GetMainActivityType()
        {
            int rez = 0;
            rez = Context.ActivityTypes.FirstOrDefault(f => f.MainActivity && f.Status == State.Active).Id;
            return rez;
        }

        public void SaveExchangeToConta(int exchangeId, DateTime operationDate, int localCurrencyId)
        {
            var exchange = Context.Exchange.Include(f => f.Curency).Include(f => f.BankAccountValuta).Include(f => f.BankAccountLei).FirstOrDefault(f => f.Id == exchangeId && f.State == State.Active);
            string documentTypeShort = exchange.ExchangeOperType == ExchangeOperType.CumparValuta ? "OC" : "OV";

            var documentType = Context.DocumentType.FirstOrDefault(f => f.TypeNameShort == documentTypeShort);

            if (documentType == null)
            {
                documentType = Context.DocumentType.FirstOrDefault(f => f.TypeNameShort == "NC");
            }
            if (documentType == null)
                throw new Exception("Nu a fost identificat tipul de document pentru inregistrarea facturilor sau bonurilor. Acesta trebuie sa aiba codul FF sau BF!");

            // generez urmatorul numar
            int nrDoc = GetNextNumberForOperContab(exchange.ExchangeDate, documentType.Id);

            var operationChild = new Operation(); // e folosit in cazul in care avem valuta

            var operation = new Operation
            {
                CurrencyId = localCurrencyId,
                OperationDate = operationDate,
                DocumentTypeId = documentType.Id,
                DocumentNumber = nrDoc.ToString(),
                DocumentDate = operationDate,
                OperationStatus = OperationStatus.Unchecked,
                State = State.Active,
                ExternalOperation = true
            };

            Context.Operations.Add(operation);
            Context.SaveChanges();

            if (exchange.CurrencyId != localCurrencyId)
            {
                operationChild = new Operation // pentru operatiile in valuta
                {
                    CurrencyId = exchange.CurrencyId,
                    OperationDate = operationDate,
                    DocumentTypeId = documentType.Id,
                    DocumentNumber = nrDoc.ToString(),
                    DocumentDate = operationDate,
                    OperationStatus = OperationStatus.Unchecked,
                    State = State.Active,
                    ExternalOperation = true,
                    OperationParentId = operation.Id
                };

                Context.Operations.Add(operationChild);
                Context.SaveChanges();
            }

            int pozitieSchimbAccountId = 0;
            int contravaloarePozitieSchimbAccountId = 0;

            var operationDetail = new OperationDetails();
            if (exchange.ExchangeOperType == ExchangeOperType.CumparValuta)
            {
                operationDetail = new OperationDetails
                {
                    ValueCurr = (localCurrencyId != exchange.CurrencyId) ? exchange.Value : 0,
                    Value = exchange.Value,
                    Details = "Schimb valutar in " + exchange.Curency.CurrencyName + " la data " + LazyMethods.DateToString(operationDate) + " " + (exchange.CurrencyId != localCurrencyId ? (", curs valutar: " + exchange.ExchangeRate.ToString()) : "")
                };
            }
            else
            {
                operationDetail = new OperationDetails
                {
                    ValueCurr = (localCurrencyId != exchange.CurrencyId) ? exchange.Value : 0,
                    Value = exchange.Value * exchange.ExchangeRate,
                    Details = "Schimb valutar din " + exchange.Curency.CurrencyName + " la data " + LazyMethods.DateToString(operationDate) + " " + (exchange.CurrencyId != localCurrencyId ? (", curs valutar: " + exchange.ExchangeRate.ToString()) : "")
                };
            }
            var accountLeiId = 0;

            var localCurrencyName = Context.Currency.FirstOrDefault(f => f.Id == localCurrencyId && f.Status == State.Active).CurrencyName;
            try
            {
                accountLeiId = Context.Account.Include(f => f.Currency).Include(f => f.BankAccount).Include(f => f.BankAccount.Bank).FirstOrDefault(f => f.BankAccountId == exchange.BankAccountLeiId && f.Status == State.Active && f.CurrencyId == localCurrencyId).Id;
            }
            catch (Exception ex)
            {

                throw new Exception("Nu am identificat contul asociat IBAN-ului " + exchange.BankAccountLei.IBAN + " in " + localCurrencyName);
            }

            var accountValutaId = 0;
            try
            {
                accountValutaId = Context.Account.Include(f => f.Currency).Include(f => f.BankAccount).Include(f => f.BankAccount.Bank).FirstOrDefault(f => f.BankAccountId == exchange.BankAccountValutaId && f.Status == State.Active && f.CurrencyId == exchange.CurrencyId).Id;
            }
            catch (Exception ex)
            {
                throw new Exception("Nu am identificat contul asociat IBAN-ului " + exchange.BankAccountValuta.IBAN + " in " + exchange.Curency.CurrencyCode);
            }

            var operationDetailChild = new OperationDetails();

            operationDetailChild = new OperationDetails
            {

                ValueCurr = (localCurrencyId != exchange.CurrencyId) ? exchange.Value : 0,
                Value = exchange.ExchangeOperType == ExchangeOperType.CumparValuta ? Math.Round(exchange.Value / exchange.ExchangeRate, 2) : exchange.Value,
                Details = "Schimb valutar in " + exchange.Curency.CurrencyName + " la data " + LazyMethods.DateToString(operationDate)
            };

            // iau pozitia de schimb valutar
            try
            {
                var pozitieSchimb = exchange.ExchangeType == ExchangeType.Cheltuieli ? AccountFuncType.PozitieSchimbCheltuieli :
                                                            (exchange.ExchangeType == ExchangeType.Operatiuni ? AccountFuncType.PozitieSchimbOperatiuni : AccountFuncType.PozitieSchimbImprumuturi);
                pozitieSchimbAccountId = _accountRepository.GetAll().FirstOrDefault(f => f.AccountFuncType == pozitieSchimb && f.ActivityTypeId == exchange.ActivityTypeId && f.CurrencyId == exchange.CurrencyId).Id;
            }
            catch (Exception ex)
            {
                throw new Exception("Nu am identificat contul pentru pozitia de schimb valutar");
            }

            //iau contra valoarea pozitiei de schimb valutar
            try
            {
                var denumireMoneda = _currencyRepository.GetCurrencyById(exchange.CurrencyId).CurrencyCode;
                var contravaloarePozitieSchimb = exchange.ExchangeType == ExchangeType.Cheltuieli ? AccountFuncType.ContravaloarePozitieSchimbCheltuieli :
                (exchange.ExchangeType == ExchangeType.Operatiuni ? AccountFuncType.ContravaloarePozitieSchimbOperatiuni : AccountFuncType.ContravaloarePozitieImprumuturi);
                contravaloarePozitieSchimbAccountId = _accountRepository.GetAll().FirstOrDefault(f => f.AccountFuncType == contravaloarePozitieSchimb &&
                                                                                                 f.ActivityTypeId == exchange.ActivityTypeId &&
                                                                                                 f.AccountName.IndexOf(denumireMoneda) >= 0).Id;
            }
            catch (Exception ex)
            {
                throw new Exception("Nu am identificat contul pentru contravaloarea pozitiei de schimb valutar");
            }

            if (exchange.ExchangeOperType == ExchangeOperType.CumparValuta)
            {
                operationDetailChild.DebitId = accountValutaId;
                operationDetailChild.CreditId = pozitieSchimbAccountId;
                operationDetail.DebitId = contravaloarePozitieSchimbAccountId;
                operationDetail.CreditId = accountLeiId;
                operationDetailChild.OperationId = operationChild.Id;
                Context.OperationsDetails.Add(operationDetailChild);

            }
            else
            {
                operationDetailChild.DebitId = pozitieSchimbAccountId;
                operationDetailChild.CreditId = accountValutaId;
                operationDetail.DebitId = accountLeiId;
                operationDetail.CreditId = contravaloarePozitieSchimbAccountId;
                operationDetailChild.OperationId = operationChild.Id;
                Context.OperationsDetails.Add(operationDetailChild);

            }

            operationDetail.OperationId = operation.Id;
            exchange.ContaOperationId = operation.Id;


            bool okAddNededOper = true;
            var nededOperDetail = GenerateNededOperDetail(ref operationDetail, operationDate, accountLeiId, out okAddNededOper);

            Context.OperationsDetails.Add(operationDetail);

            if (okAddNededOper)
            {
                Context.OperationsDetails.Add(nededOperDetail);
            }


            Context.SaveChanges();
        }

        public void DeleteContaForExchange(int exchangeId, int appClientId)
        {
            var exchange = Context.Exchange.FirstOrDefault(f => f.Id == exchangeId && f.State == State.Active);
            var lastBalanceDate = Context.Balance.Where(f => f.TenantId == appClientId && f.Status == State.Active).Max(g => g.BalanceDate);

            var operationDetails = Context.OperationsDetails.Include(f => f.Operation).Where(f => f.OperationId == exchange.ContaOperationId).ToList();
            foreach (var opDetail in operationDetails)
            {
                var operation = Context.Operations.FirstOrDefault(f => f.Id == opDetail.OperationId);
                if (operation.OperationDate <= lastBalanceDate)
                {
                    throw new Exception("Nu puteti sterge notele deoarece luna contabila este inchisa.");
                }
                operation.State = State.Inactive;

                // sterg si eventuala operatie copil provenita din valuta
                var operationChildList = Context.Operations.Where(f => f.OperationParentId == operation.Id).ToList();
                foreach (var operationChild in operationChildList)
                {
                    operationChild.State = State.Inactive;
                }
            }
        }

        public void ReevaluarePozitieValutara(DateTime dataEnd, int appClientId, int localCurrencyId, int operGenId)
        {
            try
            {
                string docNumber = "";
                // definesc operatia
                var operation = new Operation();
                operation.CurrencyId = localCurrencyId;
                var doc = Context.DocumentType.Where(x => x.TypeNameShort == "NC").FirstOrDefault();
                if (doc == null)
                {
                    throw new Exception("Nu am identificat tipul documentului");
                }
                operation.DocumentType = doc;
                operation.DocumentNumber = docNumber;
                operation.DocumentDate = dataEnd;
                operation.OperationDate = dataEnd;
                operation.OperationStatus = OperationStatus.Unchecked;
                operation.State = State.Active;
                operation.OperGenerateId = operGenId;
                operation.ExternalOperation = true;

                // preiau operatiile
                //Select operations between dates, with local currency

                var OpDetList = new List<OperationDetails>();

                // conturi pozitie de schimb
                var accountPozitieList = Context.Account.Include(f => f.AnalyticAccounts).Include(f => f.SyntheticAccount)
                                                        .Where(f => f.TenantId == appClientId && f.ComputingAccount && (f.AccountFuncType == AccountFuncType.PozitieSchimbCheltuieli || f.AccountFuncType == AccountFuncType.PozitieSchimbOperatiuni || f.AccountFuncType == AccountFuncType.PozitieSchimbImprumuturi))
                                                        .OrderBy(f => f.Symbol)
                                                        .ToList();

                int detailNr = 1;

                foreach (var pozitie in accountPozitieList) // pentru toate conturile de pozitie de schimb
                {
                    var soldPozitie = _balanceRepository.GetSoldTypeAccount(dataEnd, pozitie.Id, appClientId, pozitie.CurrencyId, localCurrencyId, false, "");
                    if (soldPozitie.Sold != 0)
                    {
                        var currency = Context.Currency.FirstOrDefault(f => f.Id == pozitie.CurrencyId);
                        // identific tipul functiei contului pentru contravaloare
                        var contravaloareFuncType = AccountFuncType.ContravaloarePozitieSchimbCheltuieli;
                        if (pozitie.AccountFuncType == AccountFuncType.PozitieSchimbOperatiuni)
                        {
                            contravaloareFuncType = AccountFuncType.ContravaloarePozitieSchimbOperatiuni;
                        }
                        else if (pozitie.AccountFuncType == AccountFuncType.PozitieSchimbImprumuturi)
                        {
                            contravaloareFuncType = AccountFuncType.ContravaloarePozitieImprumuturi;
                        }
                        // identific contul de contravaloare
                        var contravaloare = Context.Account.FirstOrDefault(f => f.TenantId == appClientId && f.ComputingAccount && f.AccountFuncType == contravaloareFuncType
                                                                                && f.ActivityTypeId == pozitie.ActivityTypeId && f.AccountName.IndexOf(currency.CurrencyCode) >= 0);
                        if (contravaloare == null)
                        {
                            throw new Exception("Nu am identificat contravaloarea in RON pentru contul " + pozitie.Symbol + " - " + pozitie.AccountName);
                        }
                        // iau soldul contravalorii
                        var soldContravaloare = _balanceRepository.GetSoldTypeAccount(dataEnd, contravaloare.Id, appClientId, contravaloare.CurrencyId, localCurrencyId, false, "");

                        var exchangeRates = _exchangeRatesRepository.GetExchangeRate(dataEnd, pozitie.CurrencyId, localCurrencyId);
                        // sold pozitie in localcurrency
                        var soldPozitieLocalCurrency = Math.Round(soldPozitie.Sold * exchangeRates, 2);

                        if (soldContravaloare.Sold != soldPozitieLocalCurrency)
                        {
                            var operDetail = new OperationDetails
                            {
                                DetailNr = detailNr,
                                Value = Math.Abs(soldPozitieLocalCurrency - soldContravaloare.Sold)
                            };
                            int debitId = 0, creditId = 0;

                            bool amVenit = true;
                            if (soldPozitie.TipSold == "C")
                            {
                                if (soldPozitieLocalCurrency > soldContravaloare.Sold)
                                {
                                    amVenit = true;
                                }
                                else
                                {
                                    amVenit = false;
                                }
                            }
                            else
                            {
                                if (soldPozitieLocalCurrency > soldContravaloare.Sold)
                                {
                                    amVenit = false;
                                }
                                else
                                {
                                    amVenit = true;
                                }
                            }


                            if (amVenit) // am diferenta pozitiva => am venit
                            {
                                // identific tipul functiei contului pentru venit
                                var venitFuncType = AccountFuncType.VenituriDiferenteCursValutarCheltuieli;
                                if (pozitie.AccountFuncType == AccountFuncType.PozitieSchimbOperatiuni)
                                {
                                    venitFuncType = AccountFuncType.VenituriDiferenteCursValutarOperatiuni;
                                }
                                else if (pozitie.AccountFuncType == AccountFuncType.PozitieSchimbImprumuturi)
                                {
                                    venitFuncType = AccountFuncType.VenituriCheltuieliDiferenteCursValutarImprumuturi;
                                }
                                // identific contul de venit
                                var venitAccount = Context.Account.FirstOrDefault(f => f.TenantId == appClientId && f.ComputingAccount && f.AccountFuncType == venitFuncType && f.ActivityTypeId == pozitie.ActivityTypeId);

                                debitId = contravaloare.Id;
                                creditId = venitAccount.Id;
                            }
                            else
                            {
                                // identific tipul functiei contului pentru cheltuiala
                                var cheltuialatFuncType = AccountFuncType.CheltuieliDiferenteCursValutarCheltuieli;
                                if (pozitie.AccountFuncType == AccountFuncType.PozitieSchimbOperatiuni)
                                {
                                    cheltuialatFuncType = AccountFuncType.CheltuieliDiferenteCursValutarOperatiuni;
                                }
                                else if (pozitie.AccountFuncType == AccountFuncType.PozitieSchimbImprumuturi)
                                {
                                    cheltuialatFuncType = AccountFuncType.VenituriCheltuieliDiferenteCursValutarImprumuturi;
                                }
                                // identific contul de venit
                                var cheltuialaAccount = Context.Account.FirstOrDefault(f => f.TenantId == appClientId && f.ComputingAccount && f.AccountFuncType == cheltuialatFuncType && f.ActivityTypeId == pozitie.ActivityTypeId);

                                debitId = cheltuialaAccount.Id;
                                creditId = contravaloare.Id;
                            }
                            operDetail.DebitId = debitId;
                            operDetail.CreditId = creditId;
                            operDetail.Details = "Reevaluare pozitie valutara";

                            detailNr++;
                            OpDetList.Add(operDetail);
                        }
                    }
                }

                operation.OperationsDetails = OpDetList;
                if (OpDetList.Count > 0)
                    Context.Operations.Add(operation);
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Eroare reevaluare pozitie valutara " + ex.Message);
            }
        }

        public int GetNextNumberForOperContab(DateTime operDate, int documentTypeId)
        {
            int ret = 0;
            int year = operDate.Year;
            var count = Context.Operations.Count(f => f.State == State.Active && f.OperationDate.Year == year && f.DocumentTypeId == documentTypeId);
            if (count == 0)
            {
                ret = 1;
            }
            else
            {
                var nrStrList = Context.Operations.Where(f => f.State == State.Active && f.OperationDate.Year == year && f.DocumentTypeId == documentTypeId)
                                               .Select(f => f.DocumentNumber).ToList();
                var nrList = new List<int>();
                foreach (var item in nrStrList)
                {
                    try
                    {
                        int nr = int.Parse(item);
                        nrList.Add(nr);
                    }
                    catch
                    {

                    }
                }
                ret = nrList.Max();
                ret++;
            }

            return ret;
        }

        public void ImprumutToConta(int imprumutId, int localCurrencyId)
        {
            var imprumut = Context.Imprumuturi.Include(f => f.ActivityType).Include(f => f.Currency).Include(f => f.DocumentType).Include(f => f.ImprumutStateList).Include(f => f.ThirdParty)
                                              .FirstOrDefault(f => f.Id == imprumutId);

            // TO DO - stergem si generam din nou notele
            var contaOperationDetailIds = imprumut.ImprumutStateList.Where(f => f.ContaOperationDetailId != null).Select(f => f.ContaOperationDetailId).ToList();

            var contaOperationDetails = Context.OperationsDetails.Include(f => f.Operation).Where(f => contaOperationDetailIds.Contains(f.Id)).ToList();

            foreach (var det in contaOperationDetails)
            {
                var oper = Context.Operations.FirstOrDefault(f => f.Id == det.OperationId);

                foreach (var child in Context.Operations.Where(f => f.OperationParentId == oper.Id).ToList())
                {
                    Context.Operations.Remove(child);
                }

                Context.Operations.Remove(oper);
            }

            Context.SaveChanges();

            var imprumutStateList = imprumut.ImprumutStateList.Where(f => f.ContaOperationDetailId == null && f.ImprumutId == imprumutId).OrderBy(f => f.ImprumuturiStare).ToList();

            if (imprumut.TipCreditare == TipCreditare.LinieDeCredit)
            {
                // pentru linie de credit avem o singura monografie de aplicat cea de inregistrare fara cea de primire care se declaseaza daca imprumutStateList contine ImprumuturiStare.Acordat
                imprumutStateList = imprumutStateList.Where(f => f.ImprumuturiStare == ImprumuturiStare.Inregistrat).ToList();


            }

            var documentType = Context.DocumentType.FirstOrDefault(f => f.Id == imprumut.DocumentTypeId);
            if (documentType == null)
                throw new Exception("Nu a fost identificat tipul de document pentru inregistrarea imprumutului!");


            foreach (var state in imprumutStateList)
            {
                /*var operationChild = new Operation();*/ // e folosit in cazul in care avem documente in valuta

                var operation = new Operation
                {
                    CurrencyId = imprumut.CurrencyId,
                    OperationDate = imprumut.DocumentDate,
                    DocumentTypeId = documentType.Id,
                    DocumentNumber = imprumut.DocumentNr.ToString(),
                    DocumentDate = state.OperationDate,
                    OperationStatus = OperationStatus.Unchecked,
                    State = State.Active,
                    ExternalOperation = true
                };

                Context.Operations.Add(operation);
                Context.SaveChanges();

                //decimal exchangeRate = 1;
                //if (imprumut.CurrencyId != localCurrencyId)
                //{
                //    exchangeRate = _exchangeRatesRepository.GetExchangeRateForOper(imprumut.DocumentDate, imprumut.CurrencyId, localCurrencyId);

                //    operationChild = new Operation // pentr operatiile in valuta... in operation am valorile in lei, iar in operationChild am valorile in valuta
                //    {
                //        CurrencyId = imprumut.CurrencyId,
                //        OperationDate = DateTime.Now,
                //        DocumentTypeId = documentType.Id,
                //        DocumentNumber = imprumut.DocumentNr.ToString(),
                //        DocumentDate = state.OperationDate,
                //        OperationStatus = OperationStatus.Unchecked,
                //        State = State.Active,
                //        ExternalOperation = true,
                //        OperationParentId = operation.Id
                //    };
                //    Context.Operations.Add(operationChild);
                //    Context.SaveChanges();

                //}

                var detailList = new List<OperationDetails>();
                /*  var operationDetailChild = new OperationDetails();*/ // pentru operatiile in valuta
                var operationDetail = new OperationDetails();
                int pozitieSchimbAccountId = 0;
                int contravaloarePozitieSchimbAccountId = 0;
                var creditAccountId = 0;
                var debitAccountId = 0;

                operationDetail = new OperationDetails
                {
                    ValueCurr = 0,
                    Value = imprumut.Suma,
                    VAT = 0,
                    Details = "Imprumut acordat" + ", " + imprumut.DocumentNr + " / " + imprumut.DocumentDate.ToShortDateString()
                };

                //if (imprumut.CurrencyId != localCurrencyId)
                //{
                //operationDetail = new OperationDetails
                //{
                //    ValueCurr = imprumut.Suma,
                //    Value = imprumut.Suma * exchangeRate,
                //    VAT = 0,
                //    Details = "Imprumut acordat" + ", " + imprumut.DocumentNr + " / " + imprumut.DocumentDate.ToShortDateString() + ", "
                //       + (imprumut.CurrencyId != localCurrencyId ? (", Curs valutar: " + exchangeRate) : "")
                //};
                //}
                //else
                //{
                //    operationDetail = new OperationDetails
                //    {
                //        ValueCurr = 0,
                //        Value = imprumut.Suma,
                //        VAT = 0,
                //        Details = "Imprumut acordat" + ", " + imprumut.DocumentNr + " / " + imprumut.DocumentDate.ToShortDateString()
                //    };
                //}

                var monog = Context.AutoOperationConfig.Where(f => f.AutoOperType == AutoOperationType.Imprumuturi)
                                                           .Where(f => f.State == State.Active && imprumut.DocumentDate >= f.StartDate && imprumut.DocumentDate <= (f.EndDate ?? imprumut.DocumentDate)
                                                           && f.OperationType == (state.ImprumuturiStare == ImprumuturiStare.Inregistrat ? (int)ImprumuturiOperType.InregistrareaAngajamentuluiDeFinantare : (int)ImprumuturiOperType.PrimireaImprumuturilor))
                                                           .OrderBy(f => f.EntryOrder)
                                                           .ToList();



                if (monog.Count == 0)
                {
                    throw new Exception("Nu exista o monografie definita pentru aceasta operatie");
                }

                foreach (var monogItem in monog)
                {
                    if (monogItem.CreditAccount == "AP")
                    {
                        var imprumutDetaliu = Context.ImprumutTipDetalii.FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.CreditAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                        creditAccountId = GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, imprumut.DocumentDate, imprumut.CurrencyId, null);
                        //imprumut.ContContabil = Context.Account.FirstOrDefault(f => f.Id == creditAccountId).Symbol;


                    }
                    else if (monogItem.CreditAccount == "CI")
                    {
                        var imprumutDetaliu = Context.ImprumutTipDetalii.FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.CreditAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                        creditAccountId = GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, imprumut.DocumentDate, imprumut.CurrencyId, null);
                    }
                    else if (monogItem.CreditAccount == "CB")
                    {
                        creditAccountId = Context.Account.Where(f => f.Status == State.Active && f.AccountFuncType == AccountFuncType.ContBancar && f.BankAccountId == imprumut.LoanAccountId).FirstOrDefault().Id;
                    }
                    else
                    {
                        creditAccountId = GetAutoAccount(monogItem.CreditAccount, imprumut.ThirdPartyId, DateTime.Now, imprumut.CurrencyId, null);
                    }

                    if (monogItem.DebitAccount == "CB")
                    {
                        debitAccountId = Context.Account.Where(f => f.Status == State.Active && f.AccountFuncType == AccountFuncType.ContBancar && f.BankAccountId == imprumut.LoanAccountId).FirstOrDefault().Id;
                    }
                    else
                    {
                        debitAccountId = GetAutoAccount(monogItem.DebitAccount, imprumut.ThirdPartyId, DateTime.Now, imprumut.CurrencyId, null);
                    }
                    //    if (imprumut.CurrencyId != localCurrencyId)
                    //    {

                    //        //operationDetailChild = new OperationDetails
                    //        //{
                    //        //    ValueCurr = (localCurrencyId != imprumut.CurrencyId) ? imprumut.Suma : 0,
                    //        //    Value = imprumut.Suma,
                    //        //    VAT = 0,
                    //        //    Details = "Acordare credit, Nr." + imprumut.DocumentNr + ", Data " + state.OperationDate
                    //        //};

                    //        //// iau pozitia de schimb valutar
                    //        //try
                    //        //{
                    //        //    pozitieSchimbAccountId = _accountRepository.GetAll().FirstOrDefault(f => f.AccountFuncType == AccountFuncType.PozitieSchimbImprumuturi && f.CurrencyId == imprumut.CurrencyId
                    //        //                                                                        && f.ActivityTypeId == imprumut.ActivityTypeId).Id;
                    //        //}
                    //        //catch (Exception ex)
                    //        //{
                    //        //    throw new Exception("Nu am identificat contul pentru pozitia de schimb valutar");
                    //        //}
                    //        //// iau contravaloarea pozitiei de schimb valutar
                    //        //try
                    //        //{
                    //        //    var denumireMoneda = _currencyRepository.GetCurrencyById(imprumut.CurrencyId).CurrencyCode;
                    //        //    contravaloarePozitieSchimbAccountId = _accountRepository.GetAll().FirstOrDefault(f => f.AccountFuncType == AccountFuncType.ContravaloarePozitieImprumuturi
                    //        //                                                                                     && f.ActivityTypeId == imprumut.ActivityTypeId
                    //        //                                                                                     && f.AccountName.IndexOf(denumireMoneda) >= 0).Id;
                    //        //}
                    //        //catch (Exception ex)
                    //        //{
                    //        //    throw new Exception("Nu am identificat contul pentru contravaloarea pozitiei de schimb valutar");
                    //        //}

                    //        //operationDetailChild.DebitId = pozitieSchimbAccountId;
                    //        //operationDetailChild.CreditId = creditAccountId;
                    //        //operationDetail.DebitId = debitAccountId;
                    //        //operationDetail.CreditId = contravaloarePozitieSchimbAccountId;

                    //        //operationDetailChild.OperationId = operationChild.Id;
                    //        //Context.OperationsDetails.Add(operationDetailChild);
                    //    }
                    //    else
                    //    {
                    //        operationDetail.DebitId = debitAccountId;
                    //        operationDetail.CreditId = creditAccountId;

                    //        operationDetail.Value = imprumut.Suma;
                    //    }
                    //}

                    operationDetail.DebitId = debitAccountId;
                    operationDetail.CreditId = creditAccountId;

                    operationDetail.Value = imprumut.Suma;

                    operationDetail.OperationId = operation.Id;

                    bool okAddNededOper = true;
                    var nededOperDetail = GenerateNededOperDetail(ref operationDetail, DateTime.Now, debitAccountId, out okAddNededOper);


                    detailList.Add(operationDetail);
                    Context.OperationsDetails.Add(operationDetail);

                    if (okAddNededOper)
                    {
                        detailList.Add(nededOperDetail);
                        Context.OperationsDetails.Add(nededOperDetail);
                    }

                    Context.SaveChanges();
                    state.ContaOperationDetailId = operationDetail.Id;
                }

                Context.SaveChanges();
            }
        }

        public void DobandaToConta(int DobandaId, int localCurrencyId, int operGenId)
        {
            var dobanda = Context.Dobanda.Include(f => f.Rata).FirstOrDefault(f => f.Id == DobandaId);
            var imprumut = Context.Imprumuturi.Where(f => f.Id == dobanda.Rata.ImprumutId).FirstOrDefault();
            // TO DO - stergem si generam din nou notele


            var contaOperation = Context.Operations.Where(f => f.Id == dobanda.ContaOperationId).FirstOrDefault();

            if (contaOperation != null)
            {
                var details = Context.OperationsDetails.Where(f => f.OperationId == contaOperation.Id).FirstOrDefault();
                dobanda.ContaOperationDetailId = null;
                dobanda.ContaOperationId = null;
                Context.OperationsDetails.Remove(details);
                Context.Operations.Remove(contaOperation);
            }

            Context.SaveChanges();


            
            var documentType = Context.DocumentType.FirstOrDefault(f => f.TypeNameShort == "NC");
            var documentNumber = GetNextNumberForOperContab(dobanda.OperationDate, documentType.Id).ToString();

            if (documentType == null)
                throw new Exception("Nu a fost identificat tipul de document pentru inregistrarea imprumutului!");



            var operationChild = new Operation(); // e folosit in cazul in care avem documente in valuta

            var operation = new Operation
            {
                CurrencyId = localCurrencyId,
                OperationDate = dobanda.OperationDate,
                DocumentTypeId = documentType.Id,
                DocumentNumber = documentNumber,
                DocumentDate = dobanda.OperationDate,
                OperationStatus = OperationStatus.Unchecked,
                State = State.Active,
                ExternalOperation = true,
                OperGenerateId = operGenId,
            };

            Context.Operations.Add(operation);
            Context.SaveChanges();

            decimal exchangeRate = 1;
            if (dobanda.Rata.CurrencyId != localCurrencyId)
            {
                exchangeRate = _exchangeRatesRepository.GetExchangeRateForOper(imprumut.DocumentDate, imprumut.CurrencyId, localCurrencyId);

                operationChild = new Operation // pentr operatiile in valuta... in operation am valorile in lei, iar in operationChild am valorile in valuta
                {
                    CurrencyId = dobanda.Rata.CurrencyId,
                    OperationDate = DateTime.Now,
                    DocumentTypeId = documentType.Id,
                    DocumentNumber = documentNumber,
                    DocumentDate = dobanda.OperationDate,
                    OperationStatus = OperationStatus.Unchecked,
                    State = State.Active,
                    ExternalOperation = true,
                    OperationParentId = operation.Id,
                    OperGenerateId = operGenId,
                };
                Context.Operations.Add(operationChild);
                Context.SaveChanges();

            }



            var detailList = new List<OperationDetails>();
            var operationDetailChild = new OperationDetails(); // pentru operatiile in valuta
            var operationDetail = new OperationDetails();
            int pozitieSchimbAccountId = 0;
            int contravaloarePozitieSchimbAccountId = 0;
            var creditAccountId = 0;
            var debitAccountId = 0;

            var monog = Context.AutoOperationConfig.Where(f => f.AutoOperType == AutoOperationType.Imprumuturi)
                                                   .Where(f => f.State == State.Active && dobanda.OperationDate >= f.StartDate && dobanda.OperationDate <= (f.EndDate ?? dobanda.OperationDate)
                                                   && f.OperationType == (int)ImprumuturiOperType.InregistrareDobandaDatorata)
                                                   .OrderBy(f => f.EntryOrder)
                                                   .ToList();

            if (monog.Count == 0)
            {
                throw new Exception("Nu exista o monografie definita pentru aceasta operatie");
            }

            foreach (var monogItem in monog)
            {


                if (dobanda.Rata.CurrencyId != localCurrencyId)
            {
                operationDetail = new OperationDetails
                {
                    ValueCurr = dobanda.ValoareDobanda,
                    Value = dobanda.ValoareDobanda * exchangeRate,
                    VAT = 0,
                    Details = "Inregistrare Dobanda" + ", " + documentNumber + " / " + dobanda.OperationDate.ToShortDateString() + ", "
                       + (dobanda.Rata.CurrencyId != localCurrencyId ? (", Curs valutar: " + exchangeRate) : "")
                };
            }
            else
            {
                operationDetail = new OperationDetails
                {
                    ValueCurr = 0,
                    Value = dobanda.ValoareDobanda,
                    VAT = 0,
                    Details = "Inregistrare Dobanda" + ", " + documentNumber + " / " + dobanda.OperationDate.ToShortDateString()
                };
            }

            //var monog = Context.AutoOperationConfig.Where(f => f.AutoOperType == AutoOperationType.Dobanda)
            //                                       .Where(f => f.State == State.Active && dobanda.OperationDate >= f.StartDate && dobanda.OperationDate <= (f.EndDate ?? dobanda.OperationDate)
            //                                       && f.OperationType == (state.ImprumuturiStare == ImprumuturiStare.Inregistrat ? (int)ImprumuturiOperType.InregistrareaAngajamentuluiDeFinantare : (int)ImprumuturiOperType.PrimireaImprumuturilor))
            //                                       .OrderBy(f => f.EntryOrder)
            //                                       .ToList();

            
                // debit
                if (monogItem.DebitAccount == "FDC")
                {
                    var imprumutDetaliu = Context.ImprumutTipDetalii.FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.DebitAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    debitAccountId = GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, imprumut.DocumentDate, imprumut.CurrencyId, null);
                }
                else if (monogItem.DebitAccount == "DA")
                {
                    var imprumutDetaliu = Context.ImprumutTipDetalii.FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.DebitAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    debitAccountId = GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, imprumut.DocumentDate, imprumut.CurrencyId, null);
                }
                else
                {
                    debitAccountId = GetAutoAccount(monogItem.DebitAccount, imprumut.ThirdPartyId, DateTime.Now, imprumut.CurrencyId, null);
                }

                // credit
                if (monogItem.CreditAccount == "FDC")
                {
                    var imprumutDetaliu = Context.ImprumutTipDetalii.FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.CreditAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    creditAccountId = GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, imprumut.DocumentDate, imprumut.CurrencyId, null);
                }
                else if (monogItem.CreditAccount == "DA")
                {
                    var imprumutDetaliu = Context.ImprumutTipDetalii.FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.CreditAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    creditAccountId = GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, imprumut.DocumentDate, imprumut.CurrencyId, null);
                }
                else
                {
                    creditAccountId = GetAutoAccount(monogItem.CreditAccount, imprumut.ThirdPartyId, DateTime.Now, imprumut.CurrencyId, null);
                }

                if (imprumut.CurrencyId != localCurrencyId)
                {

                    operationDetailChild = new OperationDetails
                    {
                        ValueCurr = (localCurrencyId != imprumut.CurrencyId) ? dobanda.ValoareDobanda : 0,
                        Value = dobanda.ValoareDobanda,
                        VAT = 0,
                        Details = "Acordare dobanda, Nr." + documentNumber + ", Data " + dobanda.OperationDate
                    };

                    // iau pozitia de schimb valutar
                    try
                    {
                        pozitieSchimbAccountId = _accountRepository.GetAll().FirstOrDefault(f => f.AccountFuncType == AccountFuncType.PozitieSchimbImprumuturi && f.CurrencyId == imprumut.CurrencyId
                                                                                            && f.ActivityTypeId == imprumut.ActivityTypeId).Id;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Nu am identificat contul pentru pozitia de schimb valutar");
                    }
                    // iau contravaloarea pozitiei de schimb valutar
                    try
                    {
                        var denumireMoneda = _currencyRepository.GetCurrencyById(imprumut.CurrencyId).CurrencyCode;
                        contravaloarePozitieSchimbAccountId = _accountRepository.GetAll().FirstOrDefault(f => f.AccountFuncType == AccountFuncType.ContravaloarePozitieImprumuturi
                                                                                                         && f.ActivityTypeId == imprumut.ActivityTypeId
                                                                                                         && f.AccountName.IndexOf(denumireMoneda) >= 0).Id;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Nu am identificat contul pentru contravaloarea pozitiei de schimb valutar");
                    }

                    operationDetailChild.DebitId = pozitieSchimbAccountId;
                    operationDetailChild.CreditId = creditAccountId;
                    operationDetail.DebitId = debitAccountId;
                    operationDetail.CreditId = contravaloarePozitieSchimbAccountId;

                    operationDetailChild.OperationId = operationChild.Id;
                    Context.OperationsDetails.Add(operationDetailChild);
                }
                else
                {
                    operationDetail.DebitId = debitAccountId;
                    operationDetail.CreditId = creditAccountId;

                    operationDetail.Value = dobanda.ValoareDobanda;
                }


                operationDetail.OperationId = operation.Id;

                bool okAddNededOper = true;
                var nededOperDetail = GenerateNededOperDetail(ref operationDetail, DateTime.Now, debitAccountId, out okAddNededOper);


                detailList.Add(operationDetail);
                Context.OperationsDetails.Add(operationDetail);

                if (okAddNededOper)
                {
                    detailList.Add(nededOperDetail);
                    Context.OperationsDetails.Add(nededOperDetail);
                }

                Context.SaveChanges();
                dobanda.ContaOperationDetailId = operationDetail.Id;
                dobanda.ContaOperationId = operation.Id;
                dobanda.ContaOperationDetailId = operationDetail.Id;
            }

            


            Context.SaveChanges();
        }

        public void PlataToConta(int DobandaId, int localCurrencyId, int operGenId)
        {
            var dobanda = Context.Dobanda.Include(f => f.Rata).FirstOrDefault(f => f.Id == DobandaId);
            var imprumut = Context.Imprumuturi.Where(f => f.Id == dobanda.Rata.ImprumutId).FirstOrDefault();
            // TO DO - stergem si generam din nou notele


            //var contaOperation = Context.Operations.Where(f => f.Id == dobanda.ContaOperationId).FirstOrDefault();

            //if (contaOperation != null)
            //{
            //    var details = Context.OperationsDetails.Where(f => f.OperationId == contaOperation.Id).FirstOrDefault();
            //    dobanda.ContaOperationDetailId = null;
            //    dobanda.ContaOperationId = null;
            //    Context.OperationsDetails.Remove(details);
            //    Context.Operations.Remove(contaOperation);
            //}

            Context.SaveChanges();



            var documentType = Context.DocumentType.FirstOrDefault(f => f.TypeNameShort == "NC");
            var documentNumber = GetNextNumberForOperContab(dobanda.OperationDate, documentType.Id).ToString();

            if (documentType == null)
                throw new Exception("Nu a fost identificat tipul de document pentru inregistrarea imprumutului!");



            var operationChild = new Operation(); // e folosit in cazul in care avem documente in valuta

            var operation = new Operation
            {
                CurrencyId = localCurrencyId,
                OperationDate = dobanda.OperationDate,
                DocumentTypeId = documentType.Id,
                DocumentNumber = documentNumber,
                DocumentDate = dobanda.OperationDate,
                OperationStatus = OperationStatus.Unchecked,
                State = State.Active,
                ExternalOperation = true,
                OperGenerateId = operGenId,
            };

            Context.Operations.Add(operation);
            Context.SaveChanges();

            decimal exchangeRate = 1;
            if (dobanda.Rata.CurrencyId != localCurrencyId)
            {
                exchangeRate = _exchangeRatesRepository.GetExchangeRateForOper(imprumut.DocumentDate, imprumut.CurrencyId, localCurrencyId);

                operationChild = new Operation // pentr operatiile in valuta... in operation am valorile in lei, iar in operationChild am valorile in valuta
                {
                    CurrencyId = dobanda.Rata.CurrencyId,
                    OperationDate = DateTime.Now,
                    DocumentTypeId = documentType.Id,
                    DocumentNumber = documentNumber,
                    DocumentDate = dobanda.OperationDate,
                    OperationStatus = OperationStatus.Unchecked,
                    State = State.Active,
                    ExternalOperation = true,
                    OperationParentId = operation.Id,
                    OperGenerateId = operGenId,
                };
                Context.Operations.Add(operationChild);
                Context.SaveChanges();

            }



            var detailList = new List<OperationDetails>();
            var operationDetailChild = new OperationDetails(); // pentru operatiile in valuta
            var operationDetail = new OperationDetails();
            int pozitieSchimbAccountId = 0;
            int contravaloarePozitieSchimbAccountId = 0;
            var creditAccountId = 0;
            var debitAccountId = 0;

            var monog = Context.AutoOperationConfig.Where(f => f.AutoOperType == AutoOperationType.Imprumuturi)
                                                   .Where(f => f.State == State.Active && dobanda.OperationDate >= f.StartDate && dobanda.OperationDate <= (f.EndDate ?? dobanda.OperationDate)
                                                   && f.OperationType == (int)ImprumuturiOperType.PlataDobandaDatorata)
                                                   .OrderBy(f => f.EntryOrder)
                                                   .ToList();

            if (monog.Count == 0)
            {
                throw new Exception("Nu exista o monografie definita pentru aceasta operatie");
            }

            foreach (var monogItem in monog)
            {


                if (dobanda.Rata.CurrencyId != localCurrencyId)
            {
                operationDetail = new OperationDetails
                {
                    ValueCurr = dobanda.Rata.SumaDobanda,
                    Value = dobanda.Rata.SumaDobanda * exchangeRate,
                    VAT = 0,
                    Details = "Plata dobanda" + ", " + documentNumber + " / " + dobanda.OperationDate.ToShortDateString() + ", "
                       + (dobanda.Rata.CurrencyId != localCurrencyId ? (", Curs valutar: " + exchangeRate) : "")
                };
            }
            else
            {
                operationDetail = new OperationDetails
                {
                    ValueCurr = 0,
                    Value = dobanda.Rata.SumaDobanda,
                    VAT = 0,
                    Details = "Plata dobanda" + ", " + documentNumber + " / " + dobanda.OperationDate.ToShortDateString()
                };
            }

            //var monog = Context.AutoOperationConfig.Where(f => f.AutoOperType == AutoOperationType.Dobanda)
            //                                       .Where(f => f.State == State.Active && dobanda.OperationDate >= f.StartDate && dobanda.OperationDate <= (f.EndDate ?? dobanda.OperationDate)
            //                                       && f.OperationType == (state.ImprumuturiStare == ImprumuturiStare.Inregistrat ? (int)ImprumuturiOperType.InregistrareaAngajamentuluiDeFinantare : (int)ImprumuturiOperType.PrimireaImprumuturilor))
            //                                       .OrderBy(f => f.EntryOrder)
            //                                       .ToList();

            
                // debit
                if (monogItem.DebitAccount == "CB")
                {

                    debitAccountId = Context.Account.Where(f => f.BankAccountId == imprumut.PaymentAccountId).FirstOrDefault().Id;

                }
                else if (monogItem.DebitAccount == "DA")
                {
                    var imprumutDetaliu = Context.ImprumutTipDetalii.FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.DebitAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    debitAccountId = GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, imprumut.DocumentDate, imprumut.CurrencyId, null);
                }
                else
                {
                    debitAccountId = GetAutoAccount(monogItem.CreditAccount, imprumut.ThirdPartyId, DateTime.Now, imprumut.CurrencyId, null);
                }

                // credit
                if (monogItem.CreditAccount == "CB")
                {

                    creditAccountId = Context.Account.Where(f => f.BankAccountId == imprumut.PaymentAccountId).FirstOrDefault().Id;
                }
                else if (monogItem.CreditAccount == "DA")
                {
                    var imprumutDetaliu = Context.ImprumutTipDetalii.FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.CreditAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    creditAccountId = GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, imprumut.DocumentDate, imprumut.CurrencyId, null);
                }
                else
                {
                    creditAccountId = GetAutoAccount(monogItem.CreditAccount, imprumut.ThirdPartyId, DateTime.Now, imprumut.CurrencyId, null);
                }

                if (imprumut.CurrencyId != localCurrencyId)
                {

                    operationDetailChild = new OperationDetails
                    {
                        ValueCurr = (localCurrencyId != imprumut.CurrencyId) ? dobanda.ValoareDobanda : 0,
                        Value = dobanda.Rata.SumaDobanda,
                        VAT = 0,
                        Details = "Acordare dobanda, Nr." + documentNumber + ", Data " + dobanda.OperationDate
                    };

                    // iau pozitia de schimb valutar
                    try
                    {
                        pozitieSchimbAccountId = _accountRepository.GetAll().FirstOrDefault(f => f.AccountFuncType == AccountFuncType.PozitieSchimbImprumuturi && f.CurrencyId == imprumut.CurrencyId
                                                                                            && f.ActivityTypeId == imprumut.ActivityTypeId).Id;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Nu am identificat contul pentru pozitia de schimb valutar");
                    }
                    // iau contravaloarea pozitiei de schimb valutar
                    try
                    {
                        var denumireMoneda = _currencyRepository.GetCurrencyById(imprumut.CurrencyId).CurrencyCode;
                        contravaloarePozitieSchimbAccountId = _accountRepository.GetAll().FirstOrDefault(f => f.AccountFuncType == AccountFuncType.ContravaloarePozitieImprumuturi
                                                                                                         && f.ActivityTypeId == imprumut.ActivityTypeId
                                                                                                         && f.AccountName.IndexOf(denumireMoneda) >= 0).Id;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Nu am identificat contul pentru contravaloarea pozitiei de schimb valutar");
                    }

                    operationDetailChild.DebitId = pozitieSchimbAccountId;
                    operationDetailChild.CreditId = creditAccountId;
                    operationDetail.DebitId = debitAccountId;
                    operationDetail.CreditId = contravaloarePozitieSchimbAccountId;

                    operationDetailChild.OperationId = operationChild.Id;
                    Context.OperationsDetails.Add(operationDetailChild);
                }
                else
                {
                    operationDetail.DebitId = debitAccountId;
                    operationDetail.CreditId = creditAccountId;

                    operationDetail.Value = dobanda.Rata.SumaDobanda;
                }

                operationDetail.OperationId = operation.Id;

                bool okAddNededOper = true;
                var nededOperDetail = GenerateNededOperDetail(ref operationDetail, DateTime.Now, debitAccountId, out okAddNededOper);


                detailList.Add(operationDetail);
                Context.OperationsDetails.Add(operationDetail);

                if (okAddNededOper)
                {
                    detailList.Add(nededOperDetail);
                    Context.OperationsDetails.Add(nededOperDetail);
                }

                Context.SaveChanges();
                


            }




            Context.SaveChanges();


        }

        public void ComisionToConta(int DataComisionId, int localCurrencyId)
        {
            var comision = Context.DateComision.Include(f => f.Comision).FirstOrDefault(f => f.Id == DataComisionId);
            var imprumut = Context.Imprumuturi.Where(f => f.Id == comision.ImprumutId).FirstOrDefault();
            // TO DO - stergem si generam din nou notele


            var contaOperation = Context.Operations.Where(f => f.Id == comision.ContaOperationId).FirstOrDefault();

            if (contaOperation != null)
            {
                var details = Context.OperationsDetails.Where(f => f.OperationId == contaOperation.Id).FirstOrDefault();
                comision.ContaOperationDetailId = null;
                comision.ContaOperationId = null;
                Context.SaveChanges();
                Context.OperationsDetails.Remove(details);
                Context.Operations.Remove(contaOperation);
            }

            Context.SaveChanges();



            var documentType = Context.DocumentType.FirstOrDefault(f => f.TypeNameShort == "NC");
            var documentNumber = GetNextNumberForOperContab(comision.DataPlataComision, documentType.Id).ToString();

            if (documentType == null)
                throw new Exception("Nu a fost identificat tipul de document pentru inregistrarea imprumutului!");



            var operationChild = new Operation(); // e folosit in cazul in care avem documente in valuta

            var operation = new Operation
            {
                CurrencyId = localCurrencyId,
                OperationDate = comision.DataPlataComision,
                DocumentTypeId = documentType.Id,
                DocumentNumber = documentNumber,
                DocumentDate = comision.DataPlataComision,
                OperationStatus = OperationStatus.Unchecked,
                State = State.Active,
                ExternalOperation = true,

            };

            Context.Operations.Add(operation);
            Context.SaveChanges();

            decimal exchangeRate = 1;
            if (imprumut.CurrencyId != localCurrencyId)
            {
                exchangeRate = _exchangeRatesRepository.GetExchangeRateForOper(imprumut.DocumentDate, imprumut.CurrencyId, localCurrencyId);

                operationChild = new Operation // pentr operatiile in valuta... in operation am valorile in lei, iar in operationChild am valorile in valuta
                {
                    CurrencyId = imprumut.CurrencyId,
                    OperationDate = comision.DataPlataComision,
                    DocumentTypeId = documentType.Id,
                    DocumentNumber = documentNumber,
                    DocumentDate = comision.DataPlataComision,
                    OperationStatus = OperationStatus.Unchecked,
                    State = State.Active,
                    ExternalOperation = true,
                    OperationParentId = operation.Id,

                };
                Context.Operations.Add(operationChild);
                Context.SaveChanges();

            }



            var detailList = new List<OperationDetails>();
            var operationDetailChild = new OperationDetails(); // pentru operatiile in valuta
            var operationDetail = new OperationDetails();
            int pozitieSchimbAccountId = 0;
            int contravaloarePozitieSchimbAccountId = 0;
            var creditAccountId = 0;
            var debitAccountId = 0;

            var OperationType = (comision.Comision.ModCalculComision == ModCalculComision.Anual || comision.Comision.ModCalculComision == ModCalculComision.Semestrial || comision.Comision.ModCalculComision == ModCalculComision.Trimestrial) ? (int)ImprumuturiOperType.PlataComisionPeriodic : (int)ImprumuturiOperType.PlataComisionLunar;

            var monog = Context.AutoOperationConfig.Where(f => f.AutoOperType == AutoOperationType.Imprumuturi)
                                                   .Where(f => f.State == State.Active && comision.DataPlataComision >= f.StartDate && comision.DataPlataComision <= (f.EndDate ?? comision.DataPlataComision)
                                                   && f.OperationType == OperationType)
                                                   .OrderBy(f => f.EntryOrder)
                                                   .ToList();

            if (monog.Count == 0)
            {
                throw new Exception("Nu exista o monografie definita pentru aceasta operatie");
            }

            foreach (var monogItem in monog)
            {

                if (imprumut.CurrencyId != localCurrencyId)
                {
                    operationDetail = new OperationDetails
                    {
                        ValueCurr = comision.SumaComision,
                        Value = comision.SumaComision * exchangeRate,
                        VAT = 0,
                        Details = "Comision" + ", " + documentNumber + " / " + comision.DataPlataComision.ToShortDateString() + ", "
                           + (imprumut.CurrencyId != localCurrencyId ? (", Curs valutar: " + exchangeRate) : "")
                    };
                }
                else
                {
                    operationDetail = new OperationDetails
                    {
                        ValueCurr = 0,
                        Value = comision.SumaComision,
                        VAT = 0,
                        Details = "Comision" + ", " + documentNumber + " / " + comision.DataPlataComision.ToShortDateString()
                    };
                }


                // debit
                if (monogItem.DebitAccount == "CB")
                {

                    debitAccountId = Context.Account.Where(f => f.BankAccountId == imprumut.PaymentAccountId).FirstOrDefault().Id;

                }
                else if (monogItem.DebitAccount == "CMI")
                {
                    var imprumutDetaliu = Context.ImprumutTipDetalii.FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.DebitAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    debitAccountId = GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, comision.DataPlataComision, imprumut.CurrencyId, null);
                }
                else if (monogItem.DebitAccount == "CPP")
                {
                    var imprumutDetaliu = Context.ImprumutTipDetalii.FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.DebitAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    debitAccountId = GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, comision.DataPlataComision, imprumut.CurrencyId, null);
                }
                else if (monogItem.DebitAccount == "CPL")
                {
                    var imprumutDetaliu = Context.ImprumutTipDetalii.FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.DebitAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    debitAccountId = GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, comision.DataPlataComision, imprumut.CurrencyId, null);
                }
                else
                {
                    debitAccountId = GetAutoAccount(monogItem.DebitAccount, imprumut.ThirdPartyId, DateTime.Now, imprumut.CurrencyId, null);
                }

                // credit
                if (monogItem.CreditAccount == "CB")
                {

                    creditAccountId = Context.Account.Where(f => f.BankAccountId == imprumut.PaymentAccountId).FirstOrDefault().Id;
                }
                else if (monogItem.CreditAccount == "CMI")
                {
                    var imprumutDetaliu = Context.ImprumutTipDetalii.FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.CreditAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    creditAccountId = GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, comision.DataPlataComision, imprumut.CurrencyId, null);
                }
                else if (monogItem.CreditAccount == "CPP")
                {
                    var imprumutDetaliu = Context.ImprumutTipDetalii.FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.CreditAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    creditAccountId = GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, comision.DataPlataComision, imprumut.CurrencyId, null);
                }
                else if (monogItem.CreditAccount == "CPL")
                {
                    var imprumutDetaliu = Context.ImprumutTipDetalii.FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.CreditAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    creditAccountId = GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, comision.DataPlataComision, imprumut.CurrencyId, null);
                }
                else
                {
                    creditAccountId = GetAutoAccount(monogItem.CreditAccount, imprumut.ThirdPartyId, DateTime.Now, imprumut.CurrencyId, null);
                }

                if (imprumut.CurrencyId != localCurrencyId)
                {

                    operationDetailChild = new OperationDetails
                    {
                        ValueCurr = (localCurrencyId != imprumut.CurrencyId) ? comision.SumaComision : 0,
                        Value = comision.SumaComision,
                        VAT = 0,
                        Details = "Acordare Comision, Nr." + documentNumber + ", Data " + comision.DataPlataComision
                    };

                    // iau pozitia de schimb valutar
                    try
                    {
                        pozitieSchimbAccountId = _accountRepository.GetAll().FirstOrDefault(f => f.AccountFuncType == AccountFuncType.PozitieSchimbImprumuturi && f.CurrencyId == imprumut.CurrencyId
                                                                                            && f.ActivityTypeId == imprumut.ActivityTypeId).Id;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Nu am identificat contul pentru pozitia de schimb valutar");
                    }
                    // iau contravaloarea pozitiei de schimb valutar
                    try
                    {
                        var denumireMoneda = _currencyRepository.GetCurrencyById(imprumut.CurrencyId).CurrencyCode;
                        contravaloarePozitieSchimbAccountId = _accountRepository.GetAll().FirstOrDefault(f => f.AccountFuncType == AccountFuncType.ContravaloarePozitieImprumuturi
                                                                                                         && f.ActivityTypeId == imprumut.ActivityTypeId
                                                                                                         && f.AccountName.IndexOf(denumireMoneda) >= 0).Id;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Nu am identificat contul pentru contravaloarea pozitiei de schimb valutar");
                    }

                    operationDetailChild.DebitId = pozitieSchimbAccountId;
                    operationDetailChild.CreditId = creditAccountId;
                    operationDetail.DebitId = debitAccountId;
                    operationDetail.CreditId = contravaloarePozitieSchimbAccountId;

                    operationDetailChild.OperationId = operationChild.Id;
                    Context.OperationsDetails.Add(operationDetailChild);
                }
                else
                {
                    operationDetail.DebitId = debitAccountId;
                    operationDetail.CreditId = creditAccountId;

                    operationDetail.Value = comision.SumaComision;
                }

                operationDetail.OperationId = operation.Id;

                bool okAddNededOper = true;
                var nededOperDetail = GenerateNededOperDetail(ref operationDetail, DateTime.Now, debitAccountId, out okAddNededOper);


                detailList.Add(operationDetail);
                Context.OperationsDetails.Add(operationDetail);

                if (okAddNededOper)
                {
                    detailList.Add(nededOperDetail);
                    Context.OperationsDetails.Add(nededOperDetail);
                }

                Context.SaveChanges();
                comision.ContaOperationDetailId = operationDetail.Id;
                comision.ContaOperationId = operation.Id;
            }

            Context.SaveChanges();


        }

        public void DiminuareRambursareToConta(int RataId, int localCurrencyId)
        {
            var rata = Context.Rate.FirstOrDefault(f => f.Id == RataId);
            var imprumut = Context.Imprumuturi.Where(f => f.Id == rata.ImprumutId).FirstOrDefault();
            // TO DO - stergem si generam din nou notele


            var contaOperation = Context.OperationsDetails.Where(f => f.Id == rata.ContaOperationDetailId).FirstOrDefault();

            if (contaOperation != null)
            {
                var oper = Context.Operations.Where(f => f.Id == contaOperation.OperationId).FirstOrDefault();
                rata.ContaOperationDetailId = null;
                
                Context.SaveChanges();
                Context.OperationsDetails.Remove(contaOperation);
                Context.Operations.Remove(oper);
            }

            Context.SaveChanges();



            var documentType = Context.DocumentType.FirstOrDefault(f => f.TypeNameShort == "NC");
            var documentNumber = GetNextNumberForOperContab(rata.DataPlataRata, documentType.Id).ToString();

            if (documentType == null)
                throw new Exception("Nu a fost identificat tipul de document pentru inregistrarea imprumutului!");



            var operationChild = new Operation(); // e folosit in cazul in care avem documente in valuta

            var operation = new Operation
            {
                CurrencyId = localCurrencyId,
                OperationDate = rata.DataPlataRata,
                DocumentTypeId = documentType.Id,
                DocumentNumber = documentNumber,
                DocumentDate = rata.DataPlataRata,
                OperationStatus = OperationStatus.Unchecked,
                State = State.Active,
                ExternalOperation = true,

            };

            Context.Operations.Add(operation);
            Context.SaveChanges();

            decimal exchangeRate = 1;
            if (imprumut.CurrencyId != localCurrencyId)
            {
                exchangeRate = _exchangeRatesRepository.GetExchangeRateForOper(imprumut.DocumentDate, imprumut.CurrencyId, localCurrencyId);

                operationChild = new Operation // pentr operatiile in valuta... in operation am valorile in lei, iar in operationChild am valorile in valuta
                {
                    CurrencyId = imprumut.CurrencyId,
                    OperationDate = rata.DataPlataRata,
                    DocumentTypeId = documentType.Id,
                    DocumentNumber = documentNumber,
                    DocumentDate = rata.DataPlataRata,
                    OperationStatus = OperationStatus.Unchecked,
                    State = State.Active,
                    ExternalOperation = true,
                    OperationParentId = operation.Id,

                };
                Context.Operations.Add(operationChild);
                Context.SaveChanges();

            }



            var detailList = new List<OperationDetails>();
            var operationDetailChild = new OperationDetails(); // pentru operatiile in valuta
            var operationDetail = new OperationDetails();
            int pozitieSchimbAccountId = 0;
            int contravaloarePozitieSchimbAccountId = 0;
            var creditAccountId = 0;
            var debitAccountId = 0;

           

            var monog = Context.AutoOperationConfig.Where(f => f.AutoOperType == AutoOperationType.Imprumuturi)
                                                   .Where(f => f.State == State.Active && rata.DataPlataRata >= f.StartDate && rata.DataPlataRata <= (f.EndDate ?? rata.DataPlataRata)
                                                   && f.OperationType == (int)ImprumuturiOperType.DiminuareaAngajamentului)
                                                   .OrderBy(f => f.EntryOrder)
                                                   .ToList();

            if (monog.Count == 0)
            {
                throw new Exception("Nu exista o monografie definita pentru aceasta operatie");
            }

            foreach (var monogItem in monog)
            {

                if (imprumut.CurrencyId != localCurrencyId)
                {
                    operationDetail = new OperationDetails
                    {
                        ValueCurr = rata.SumaPrincipal,
                        Value = rata.SumaPrincipal * exchangeRate,
                        VAT = 0,
                        Details = monogItem.Details + ", " + documentNumber + " / " + rata.DataPlataRata.ToShortDateString() + ", "
                           + (imprumut.CurrencyId != localCurrencyId ? (", Curs valutar: " + exchangeRate) : "")
                    };
                }
                else
                {
                    operationDetail = new OperationDetails
                    {
                        ValueCurr = 0,
                        Value = rata.SumaPrincipal,
                        VAT = 0,
                        Details = monogItem.Details + ", " + documentNumber + " / " + rata.DataPlataRata.ToShortDateString()
                    };
                }


                // debit
                if (monogItem.DebitAccount == "AP")
                {
                    var imprumutDetaliu = Context.ImprumutTipDetalii.FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.DebitAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    debitAccountId = GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, imprumut.DocumentDate, imprumut.CurrencyId, null);
                }
                else if (monogItem.DebitAccount == "CI")
                {
                    var imprumutDetaliu = Context.ImprumutTipDetalii.FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.DebitAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    debitAccountId = GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, imprumut.DocumentDate, imprumut.CurrencyId, null);
                }
                else
                {
                    debitAccountId = GetAutoAccount(monogItem.DebitAccount, imprumut.ThirdPartyId, DateTime.Now, imprumut.CurrencyId, null);
                }

                // credit
                if (monogItem.CreditAccount == "CB")
                {

                    creditAccountId = Context.Account.Where(f => f.BankAccountId == imprumut.PaymentAccountId).FirstOrDefault().Id;
                }
                
                else
                {
                    creditAccountId = GetAutoAccount(monogItem.CreditAccount, imprumut.ThirdPartyId, DateTime.Now, imprumut.CurrencyId, null);
                }

                if (imprumut.CurrencyId != localCurrencyId)
                {

                    operationDetailChild = new OperationDetails
                    {
                        ValueCurr = (localCurrencyId != imprumut.CurrencyId) ? rata.SumaPrincipal : 0,
                        Value = rata.SumaPrincipal,
                        VAT = 0,
                        Details = monogItem.Details + " Nr." + documentNumber + ", Data " + rata.DataPlataRata
                    };

                    // iau pozitia de schimb valutar
                    try
                    {
                        pozitieSchimbAccountId = _accountRepository.GetAll().FirstOrDefault(f => f.AccountFuncType == AccountFuncType.PozitieSchimbImprumuturi && f.CurrencyId == imprumut.CurrencyId
                                                                                            && f.ActivityTypeId == imprumut.ActivityTypeId).Id;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Nu am identificat contul pentru pozitia de schimb valutar");
                    }
                    // iau contravaloarea pozitiei de schimb valutar
                    try
                    {
                        var denumireMoneda = _currencyRepository.GetCurrencyById(imprumut.CurrencyId).CurrencyCode;
                        contravaloarePozitieSchimbAccountId = _accountRepository.GetAll().FirstOrDefault(f => f.AccountFuncType == AccountFuncType.ContravaloarePozitieImprumuturi
                                                                                                         && f.ActivityTypeId == imprumut.ActivityTypeId
                                                                                                         && f.AccountName.IndexOf(denumireMoneda) >= 0).Id;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Nu am identificat contul pentru contravaloarea pozitiei de schimb valutar");
                    }

                    operationDetailChild.DebitId = pozitieSchimbAccountId;
                    operationDetailChild.CreditId = creditAccountId;
                    operationDetail.DebitId = debitAccountId;
                    operationDetail.CreditId = contravaloarePozitieSchimbAccountId;

                    operationDetailChild.OperationId = operationChild.Id;
                    Context.OperationsDetails.Add(operationDetailChild);
                }
                else
                {
                    operationDetail.DebitId = debitAccountId;
                    operationDetail.CreditId = creditAccountId;

                    operationDetail.Value = rata.SumaPrincipal;
                }

                operationDetail.OperationId = operation.Id;

                bool okAddNededOper = true;
                var nededOperDetail = GenerateNededOperDetail(ref operationDetail, DateTime.Now, debitAccountId, out okAddNededOper);


                detailList.Add(operationDetail);
                Context.OperationsDetails.Add(operationDetail);

                if (okAddNededOper)
                {
                    detailList.Add(nededOperDetail);
                    Context.OperationsDetails.Add(nededOperDetail);
                }

                Context.SaveChanges();
                rata.ContaOperationDetailId = operationDetail.Id;
                
            }

            Context.SaveChanges();


        }

        public void LinieDeCreditPentruLuna(Imprumut imprumut, DateTime operData, int operGenId, int localCurrencyId)
        {
            // verificare luna inchisa (exista balanta)
            var count = Context.Balance.Count(f => f.Status == State.Active && f.BalanceDate.Month == operData.Month && f.BalanceDate.Year == operData.Year);
            if (count != 0)
            {
                throw new Exception("Nu se poate inregistra operatia deoarece luna contabila este inchisa");
            }

            var debitId = Context.Account.FirstOrDefault(f => f.Status == State.Active && f.Symbol == "999").Id;
            var contImprumut = Context.Account.FirstOrDefault(f => f.Status == State.Active && f.Id == imprumut.ContContabilId).Id;
            //    var lastBalance = Context.BalanceDetails.Include(f => f.Account).Include(f => f.Balance).OrderByDescending(f => f.Balance.BalanceDate).FirstOrDefault(f => f.Account.Symbol == imprumut.ContContabil); // cea mai recenta balanta asociata contului
            var lastBalance = Context.Balance.Where(f=>f.Status == State.Active && f.BalanceDate < operData).OrderByDescending(f => f.BalanceDate).FirstOrDefault(); // cea mai recenta balanta

            var soldInitialItem = Context.BalanceDetails.FirstOrDefault(f => f.AccountId == contImprumut && f.BalanceId == lastBalance.Id);
            decimal sumaImprumutataLastBalance = 0;
            if (soldInitialItem != null)
            {
                sumaImprumutataLastBalance = soldInitialItem.CrValueF - soldInitialItem.DbValueF;
            }

            var contaOperationTragere = Context.OperationsDetails.Include(f => f.Operation).Where(f => f.Operation.OperationDate > lastBalance.BalanceDate && f.Operation.OperationDate <= operData && f.CreditId == contImprumut && f.Operation.State == State.Active).ToList(); // de adaugat al cui imprumut este operatia contabila
            var contaOperationRambursare = Context.OperationsDetails.Include(f => f.Operation).Where(f => f.Operation.OperationDate > lastBalance.BalanceDate && f.Operation.OperationDate <= operData && f.DebitId == contImprumut && f.Operation.State == State.Active).ToList();

            decimal calculDobanda = 0;
            decimal calculComision = 0;

            decimal soldDisponibil = imprumut.Suma - sumaImprumutataLastBalance;
            decimal valoareImprumutata = sumaImprumutataLastBalance;

            var firstDayOperation = new DateTime(operData.Year, operData.Month, 1);

            var contaOperationTragereToFirstDay = contaOperationTragere.Where(f => f.Operation.OperationDate < firstDayOperation);
            var contaOperationRambursareToFirstDay = contaOperationRambursare.Where(f => f.Operation.OperationDate < firstDayOperation);


            soldDisponibil = soldDisponibil - contaOperationTragereToFirstDay.Sum(f=>f.Value);

            soldDisponibil = soldDisponibil + contaOperationRambursareToFirstDay.Sum(f => f.Value);

            var comisionList = Context.Comisioane.Where(f => f.ImprumutId == imprumut.Id && f.TipComision != TipComision.Acordare && f.State == State.Active).ToList();
            decimal[] comisioneCalculate = new decimal[comisionList.Count];

            var operatieDC = Context.OperatieDobandaComision.Where(f => f.ImprumutId == imprumut.Id && f.State == State.Active);

            
            for (int i = 1; i <= operData.Day; i++)
            {
                var operTragere = contaOperationTragere.Where(f => f.Operation.OperationDate == new DateTime(operData.Year, operData.Month, i));
                var operRambursare = contaOperationRambursare.Where(f => f.Operation.OperationDate == new DateTime(operData.Year, operData.Month, i));

                foreach (var value in operTragere)
                {
                    soldDisponibil = soldDisponibil - value.Value;
                }

                foreach (var value in operRambursare)
                {
                    soldDisponibil = soldDisponibil + value.Value;
                }

                valoareImprumutata = imprumut.Suma - soldDisponibil;

                calculDobanda = calculDobanda + valoareImprumutata * imprumut.ProcentDobanda / 100 / 360;

                

                for(int j = 0; j < comisionList.Count; j++)
                {
                    var comision = comisionList[j];
                    decimal _calculComision = comision.ValoareComision / comision.BazaDeCalcul;

                    if (comision.TipValoareComision == TipValoareComision.Procent)
                    {
                        if (comision.TipSumaComision == TipSumaComision.Sold)
                        {
                            _calculComision = soldDisponibil * comision.ValoareComision / 100  / comision.BazaDeCalcul;
                        }
                        else if (comision.TipSumaComision == TipSumaComision.ValoareImprumut)
                        {
                            _calculComision = imprumut.Suma * comision.ValoareComision / 100 / comision.BazaDeCalcul;
                        }
                        else if (comision.TipSumaComision == TipSumaComision.SumaTrasa)
                        {
                            _calculComision = valoareImprumutata * comision.ValoareComision / 100 / comision.BazaDeCalcul;
                        }
                    }

                    comisioneCalculate[j] = comisioneCalculate[j] + _calculComision;
                }


            }


            var documentTypeComision = Context.DocumentType.FirstOrDefault(f => f.TypeNameShort == "NC");
            var documentNumberComision = GetNextNumberForOperContab(operData, documentTypeComision.Id).ToString();

            if (documentTypeComision == null)
                throw new Exception("Nu a fost identificat tipul de document pentru inregistrarea imprumutului!");



            var operationChildComision = new Operation(); // e folosit in cazul in care avem documente in valuta

            var operationComision = new Operation
            {
                CurrencyId = localCurrencyId,
                OperationDate = operData,
                DocumentTypeId = documentTypeComision.Id,
                DocumentNumber = documentNumberComision,
                DocumentDate = operData,
                OperationStatus = OperationStatus.Unchecked,
                State = State.Active,
                ExternalOperation = true,
                OperGenerateId = operGenId
            };

            Context.Operations.Add(operationComision);
            Context.SaveChanges();

            decimal exchangeRateComision = 1;
            if (imprumut.CurrencyId != localCurrencyId)
            {
                exchangeRateComision = _exchangeRatesRepository.GetExchangeRateForOper(imprumut.DocumentDate, imprumut.CurrencyId, localCurrencyId);

                operationChildComision = new Operation // pentr operatiile in valuta... in operation am valorile in lei, iar in operationChild am valorile in valuta
                {
                    CurrencyId = imprumut.CurrencyId,
                    OperationDate = operData,
                    DocumentTypeId = documentTypeComision.Id,
                    DocumentNumber = documentNumberComision,
                    DocumentDate = operData,
                    OperationStatus = OperationStatus.Unchecked,
                    State = State.Active,
                    ExternalOperation = true,
                    OperationParentId = operationComision.Id,
                    OperGenerateId = operGenId

                };
                Context.Operations.Add(operationChildComision);
                Context.SaveChanges();

            }

            


            for (int i = 0; i < comisionList.Count; i++)
            {
                ComisionLinieCredit(exchangeRateComision, documentNumberComision, localCurrencyId, operData, comisioneCalculate[i], imprumut, comisionList[i], operationComision, operationChildComision);
            }
            


            //var contaOperation = Context.Operations.Where(f => f.Id == dobanda.ContaOperationId).FirstOrDefault();

            //if (contaOperation != null)
            //{
            //    var details = Context.OperationsDetails.Where(f => f.OperationId == contaOperation.Id).FirstOrDefault();
            //    dobanda.ContaOperationDetailId = null;
            //    dobanda.ContaOperationId = null;
            //    Context.OperationsDetails.Remove(details);
            //    Context.Operations.Remove(contaOperation);
            //}

            //Context.SaveChanges();



            var documentType = Context.DocumentType.FirstOrDefault(f => f.TypeNameShort == "NC");
            var documentNumber = GetNextNumberForOperContab(operData, documentType.Id).ToString();

            if (documentType == null)
                throw new Exception("Nu a fost identificat tipul de document pentru inregistrarea imprumutului!");



            var operationChild = new Operation(); // e folosit in cazul in care avem documente in valuta

            var operation = new Operation
            {
                CurrencyId = localCurrencyId,
                OperationDate = operData,
                DocumentTypeId = documentType.Id,
                DocumentNumber = documentNumber,
                DocumentDate = operData,
                OperationStatus = OperationStatus.Unchecked,
                State = State.Active,
                ExternalOperation = true,
                OperGenerateId = operGenId,
            };

            Context.Operations.Add(operation);
            Context.SaveChanges();



            decimal exchangeRate = 1;
            if (imprumut.CurrencyId != localCurrencyId)
            {
                exchangeRate = _exchangeRatesRepository.GetExchangeRateForOper(imprumut.DocumentDate, imprumut.CurrencyId, localCurrencyId);

                operationChild = new Operation // pentr operatiile in valuta... in operation am valorile in lei, iar in operationChild am valorile in valuta
                {
                    CurrencyId = imprumut.CurrencyId,
                    OperationDate = operData,
                    DocumentTypeId = documentType.Id,
                    DocumentNumber = documentNumber,
                    DocumentDate = operData,
                    OperationStatus = OperationStatus.Unchecked,
                    State = State.Active,
                    ExternalOperation = true,
                    OperationParentId = operation.Id,
                    OperGenerateId = operGenId,
                };
                Context.Operations.Add(operationChild);
                Context.SaveChanges();

            }



            var detailList = new List<OperationDetails>();
            var operationDetailChild = new OperationDetails(); // pentru operatiile in valuta
            var operationDetail = new OperationDetails();
            int pozitieSchimbAccountId = 0;
            int contravaloarePozitieSchimbAccountId = 0;
            var creditAccountId = 0;
            var debitAccountId = 0;

            var monog = Context.AutoOperationConfig.Where(f => f.AutoOperType == AutoOperationType.Imprumuturi)
                                                   .Where(f => f.State == State.Active && operData >= f.StartDate && operData <= (f.EndDate ?? operData)
                                                   && f.OperationType == (int)ImprumuturiOperType.InregistrareDobandaDatorata)
                                                   .OrderBy(f => f.EntryOrder)
                                                   .ToList();

            if (monog.Count == 0)
            {
                throw new Exception("Nu exista o monografie definita pentru aceasta operatie");
            }

            foreach (var monogItem in monog)
            {


                if (imprumut.CurrencyId != localCurrencyId)
                {
                    operationDetail = new OperationDetails
                    {
                        ValueCurr = calculDobanda,
                        Value = calculDobanda * exchangeRate,
                        VAT = 0,
                        Details = "Inregistrare Dobanda" + ", " + documentNumber + " / " + operData.ToShortDateString() + ", "
                           + (imprumut.CurrencyId != localCurrencyId ? (", Curs valutar: " + exchangeRate) : "")
                    };
                }
                else
                {
                    operationDetail = new OperationDetails
                    {
                        ValueCurr = 0,
                        Value = calculDobanda,
                        VAT = 0,
                        Details = "Inregistrare Dobanda" + ", " + documentNumber + " / " + operData.ToShortDateString()
                    };
                }

                //var monog = Context.AutoOperationConfig.Where(f => f.AutoOperType == AutoOperationType.Dobanda)
                //                                       .Where(f => f.State == State.Active && dobanda.OperationDate >= f.StartDate && dobanda.OperationDate <= (f.EndDate ?? dobanda.OperationDate)
                //                                       && f.OperationType == (state.ImprumuturiStare == ImprumuturiStare.Inregistrat ? (int)ImprumuturiOperType.InregistrareaAngajamentuluiDeFinantare : (int)ImprumuturiOperType.PrimireaImprumuturilor))
                //                                       .OrderBy(f => f.EntryOrder)
                //                                       .ToList();


                // debit
                if (monogItem.DebitAccount == "FDC")
                {
                    var imprumutDetaliu = Context.ImprumutTipDetalii.FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.DebitAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    debitAccountId = GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, imprumut.DocumentDate, localCurrencyId, null);
                }
                else if (monogItem.DebitAccount == "DA")
                {
                    var imprumutDetaliu = Context.ImprumutTipDetalii.FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.DebitAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    debitAccountId = GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, imprumut.DocumentDate, imprumut.CurrencyId, null);
                }
                else
                {
                    debitAccountId = GetAutoAccount(monogItem.DebitAccount, imprumut.ThirdPartyId, DateTime.Now, imprumut.CurrencyId, null);
                }

                // credit
                if (monogItem.CreditAccount == "FDC")
                {
                    var imprumutDetaliu = Context.ImprumutTipDetalii.FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.CreditAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    creditAccountId = GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, imprumut.DocumentDate, localCurrencyId, null);
                }
                else if (monogItem.CreditAccount == "DA")
                {
                    var imprumutDetaliu = Context.ImprumutTipDetalii.FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.CreditAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    creditAccountId = GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, imprumut.DocumentDate, imprumut.CurrencyId, null);
                }
                else
                {
                    creditAccountId = GetAutoAccount(monogItem.CreditAccount, imprumut.ThirdPartyId, DateTime.Now, imprumut.CurrencyId, null);
                }

                if (imprumut.CurrencyId != localCurrencyId)
                {

                    operationDetailChild = new OperationDetails
                    {
                        ValueCurr = (localCurrencyId != imprumut.CurrencyId) ? calculDobanda : 0,
                        Value = calculDobanda,
                        VAT = 0,
                        Details = "Acordare dobanda, Nr." + documentNumber + ", Data " + operData
                    };

                    // iau pozitia de schimb valutar
                    try
                    {
                        pozitieSchimbAccountId = _accountRepository.GetAll().FirstOrDefault(f => f.AccountFuncType == AccountFuncType.PozitieSchimbImprumuturi && f.CurrencyId == imprumut.CurrencyId
                                                                                            && f.ActivityTypeId == imprumut.ActivityTypeId).Id;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Nu am identificat contul pentru pozitia de schimb valutar");
                    }
                    // iau contravaloarea pozitiei de schimb valutar
                    try
                    {
                        var denumireMoneda = _currencyRepository.GetCurrencyById(imprumut.CurrencyId).CurrencyCode;
                        contravaloarePozitieSchimbAccountId = _accountRepository.GetAll().FirstOrDefault(f => f.AccountFuncType == AccountFuncType.ContravaloarePozitieImprumuturi
                                                                                                         && f.ActivityTypeId == imprumut.ActivityTypeId
                                                                                                         && f.AccountName.IndexOf(denumireMoneda) >= 0).Id;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Nu am identificat contul pentru contravaloarea pozitiei de schimb valutar");
                    }

                    operationDetailChild.DebitId = pozitieSchimbAccountId;
                    operationDetailChild.CreditId = creditAccountId;
                    operationDetail.DebitId = debitAccountId;
                    operationDetail.CreditId = contravaloarePozitieSchimbAccountId;

                    operationDetailChild.OperationId = operationChild.Id;
                    Context.OperationsDetails.Add(operationDetailChild);
                }
                else
                {
                    operationDetail.DebitId = debitAccountId;
                    operationDetail.CreditId = creditAccountId;

                    operationDetail.Value = calculDobanda;
                }


                operationDetail.OperationId = operation.Id;

                bool okAddNededOper = true;
                var nededOperDetail = GenerateNededOperDetail(ref operationDetail, DateTime.Now, debitAccountId, out okAddNededOper);


                detailList.Add(operationDetail);
                Context.OperationsDetails.Add(operationDetail);

                if (okAddNededOper)
                {
                    detailList.Add(nededOperDetail);
                    Context.OperationsDetails.Add(nededOperDetail);
                }

                Context.SaveChanges();

            }




            Context.SaveChanges();

        }

        

        public void ComisionLinieCredit(decimal exchangeRate ,string documentNumber, int localCurrencyId, DateTime OperDate, decimal ValoareComision,Imprumut imprumut, Comision Comision , Operation operation, Operation operationChild)
        {


            var documentType = Context.DocumentType.FirstOrDefault(f => f.TypeNameShort == "NC");




            var detailList = new List<OperationDetails>();
            var operationDetailChild = new OperationDetails(); // pentru operatiile in valuta
            var operationDetail = new OperationDetails();
            int pozitieSchimbAccountId = 0;
            int contravaloarePozitieSchimbAccountId = 0;
            var creditAccountId = 0;
            var debitAccountId = 0;

            var OperationType = (Comision.ModCalculComision == ModCalculComision.Anual || Comision.ModCalculComision == ModCalculComision.Semestrial || Comision.ModCalculComision == ModCalculComision.Trimestrial) ? (int)ImprumuturiOperType.PlataComisionPeriodic : (int)ImprumuturiOperType.PlataComisionLunar;

            var monog = Context.AutoOperationConfig.Where(f => f.AutoOperType == AutoOperationType.Imprumuturi)
                                                   .Where(f => f.State == State.Active && OperDate >= f.StartDate && OperDate <= (f.EndDate ?? OperDate)
                                                   && f.OperationType == OperationType)
                                                   .OrderBy(f => f.EntryOrder)
                                                   .ToList();

            if (monog.Count == 0)
            {
                throw new Exception("Nu exista o monografie definita pentru aceasta operatie");
            }

            foreach (var monogItem in monog)
            {

                if (imprumut.CurrencyId != localCurrencyId)
                {
                    operationDetail = new OperationDetails
                    {
                        ValueCurr = ValoareComision,
                        Value = ValoareComision * exchangeRate,
                        VAT = 0,
                        Details = "Comision" + ", " + documentNumber + " / " + OperDate.ToShortDateString() + ", "
                           + (imprumut.CurrencyId != localCurrencyId ? (", Curs valutar: " + exchangeRate) : "")
                    };
                }
                else
                {
                    operationDetail = new OperationDetails
                    {
                        ValueCurr = 0,
                        Value = ValoareComision,
                        VAT = 0,
                        Details = "Comision" + ", " + documentNumber + " / " + OperDate.ToShortDateString()
                    };
                }


                // debit
                if (monogItem.DebitAccount == "CB")
                {

                    debitAccountId = Context.Account.Where(f => f.BankAccountId == imprumut.PaymentAccountId).FirstOrDefault().Id;

                }
                else if (monogItem.DebitAccount == "CMI")
                {
                    var imprumutDetaliu = Context.ImprumutTipDetalii.FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.DebitAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    debitAccountId = GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, OperDate, localCurrencyId, null);
                }
                else if (monogItem.DebitAccount == "CPP")
                {
                    var imprumutDetaliu = Context.ImprumutTipDetalii.FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.DebitAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    debitAccountId = GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, OperDate, imprumut.CurrencyId, null);
                }
                else if (monogItem.DebitAccount == "CPL")
                {
                    var imprumutDetaliu = Context.ImprumutTipDetalii.FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.DebitAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    debitAccountId = GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, OperDate, imprumut.CurrencyId, null);
                }
                else
                {
                    debitAccountId = GetAutoAccount(monogItem.DebitAccount, imprumut.ThirdPartyId, DateTime.Now, imprumut.CurrencyId, null);
                }

                // credit
                if (monogItem.CreditAccount == "CB")
                {

                    creditAccountId = Context.Account.Where(f => f.BankAccountId == imprumut.PaymentAccountId).FirstOrDefault().Id;
                }
                else if (monogItem.CreditAccount == "CMI")
                {
                    var imprumutDetaliu = Context.ImprumutTipDetalii.FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.CreditAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    creditAccountId = GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, OperDate, localCurrencyId, null);
                }
                else if (monogItem.CreditAccount == "CPP")
                {
                    var imprumutDetaliu = Context.ImprumutTipDetalii.FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.CreditAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    creditAccountId = GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, OperDate, imprumut.CurrencyId, null);
                }
                else if (monogItem.CreditAccount == "CPL")
                {
                    var imprumutDetaliu = Context.ImprumutTipDetalii.FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.CreditAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    creditAccountId = GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, OperDate, imprumut.CurrencyId, null);
                }
                else
                {
                    creditAccountId = GetAutoAccount(monogItem.CreditAccount, imprumut.ThirdPartyId, DateTime.Now, imprumut.CurrencyId, null);
                }

                if (imprumut.CurrencyId != localCurrencyId)
                {

                    operationDetailChild = new OperationDetails
                    {
                        ValueCurr = (localCurrencyId != imprumut.CurrencyId) ? ValoareComision : 0,
                        Value = ValoareComision,
                        VAT = 0,
                        Details = "Acordare Comision, Nr." + documentNumber + ", Data " + OperDate.ToShortDateString()
                    };

                    // iau pozitia de schimb valutar
                    try
                    {
                        pozitieSchimbAccountId = _accountRepository.GetAll().FirstOrDefault(f => f.AccountFuncType == AccountFuncType.PozitieSchimbImprumuturi && f.CurrencyId == imprumut.CurrencyId
                                                                                            && f.ActivityTypeId == imprumut.ActivityTypeId).Id;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Nu am identificat contul pentru pozitia de schimb valutar");
                    }
                    // iau contravaloarea pozitiei de schimb valutar
                    try
                    {
                        var denumireMoneda = _currencyRepository.GetCurrencyById(imprumut.CurrencyId).CurrencyCode;
                        contravaloarePozitieSchimbAccountId = _accountRepository.GetAll().FirstOrDefault(f => f.AccountFuncType == AccountFuncType.ContravaloarePozitieImprumuturi
                                                                                                         && f.ActivityTypeId == imprumut.ActivityTypeId
                                                                                                         && f.AccountName.IndexOf(denumireMoneda) >= 0).Id;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Nu am identificat contul pentru contravaloarea pozitiei de schimb valutar");
                    }

                    operationDetailChild.DebitId = pozitieSchimbAccountId;
                    operationDetailChild.CreditId = creditAccountId;
                    operationDetail.DebitId = debitAccountId;
                    operationDetail.CreditId = contravaloarePozitieSchimbAccountId;

                    operationDetailChild.OperationId = operationChild.Id;
                    Context.OperationsDetails.Add(operationDetailChild);
                }
                else
                {
                    operationDetail.DebitId = debitAccountId;
                    operationDetail.CreditId = creditAccountId;

                    operationDetail.Value = ValoareComision;
                }

                operationDetail.OperationId = operation.Id;

                bool okAddNededOper = true;
                var nededOperDetail = GenerateNededOperDetail(ref operationDetail, DateTime.Now, debitAccountId, out okAddNededOper);


                detailList.Add(operationDetail);
                Context.OperationsDetails.Add(operationDetail);

                if (okAddNededOper)
                {
                    detailList.Add(nededOperDetail);
                    Context.OperationsDetails.Add(nededOperDetail);
                }

                Context.SaveChanges();
               
            }

            Context.SaveChanges();

        }

        public void OperatieGarantieToConta(int OperatieGarantieId, int localCurrencyId)
        {
            var operatieGarantie = Context.OperatieGarantie.Include(f => f.Garantie).ThenInclude(f => f.GarantieTip).FirstOrDefault(f => f.Id == OperatieGarantieId);
            var imprumut = Context.Imprumuturi.Where(f => f.Id == operatieGarantie.Garantie.ImprumutId).FirstOrDefault();
            // TO DO - stergem si generam din nou notele


            var documentType = Context.DocumentType.FirstOrDefault(f => f.TypeNameShort == "NC");
            var documentNumber = GetNextNumberForOperContab(operatieGarantie.DataOperatiei, documentType.Id).ToString();

            if (documentType == null)
                throw new Exception("Nu a fost identificat tipul de document pentru inregistrarea imprumutului!");



            var operationChild = new Operation(); // e folosit in cazul in care avem documente in valuta

            var operation = new Operation
            {
                CurrencyId = operatieGarantie.Garantie.CurrencyId,
                OperationDate = operatieGarantie.DataOperatiei,
                DocumentTypeId = documentType.Id,
                DocumentNumber = documentNumber,
                DocumentDate = operatieGarantie.DataOperatiei,
                OperationStatus = OperationStatus.Unchecked,
                State = State.Active,
                ExternalOperation = true,

            };

            Context.Operations.Add(operation);
            Context.SaveChanges();


            var detailList = new List<OperationDetails>();
            var operationDetailChild = new OperationDetails(); // pentru operatiile in valuta
            var operationDetail = new OperationDetails();
            int pozitieSchimbAccountId = 0;
            int contravaloarePozitieSchimbAccountId = 0;
            var creditAccountId = 0;
            var debitAccountId = 0;

            var OperationType = new ImprumuturiOperType();

            if ( operatieGarantie.Garantie.TipGarantiePrimitaDataEnum == TipGarantiePrimitaDataEnum.Primita)
            {
                if(operatieGarantie.TipOperatieGarantieEnum == TipOperatieGarantieEnum.Majorare)
                {
                    if(operatieGarantie.Garantie.GarantieTip.Description == "Scrisoare de garantie")
                    {
                        OperationType = ImprumuturiOperType.MajorareScrisoareDeGarantiePrimita;
                    }
                    else if(operatieGarantie.Garantie.GarantieTip.Description == "Gajuri fără deposedare")
                    {
                        OperationType = ImprumuturiOperType.MajorareGajFaraDeposedarePrimit;
                    }
                    
                    else if(operatieGarantie.Garantie.GarantieTip.Description == "Gajuri cu deposedare")
                    {
                        OperationType = ImprumuturiOperType.MajorareGajCuDeposedarePrimit;
                    }
                }
                else
                {
                    if (operatieGarantie.Garantie.GarantieTip.Description == "Scrisoare de garantie")
                    {
                        OperationType = ImprumuturiOperType.DiminuareScrisoareDeGarantiePrimita;
                    }
                    else if (operatieGarantie.Garantie.GarantieTip.Description == "Gajuri fără deposedare")
                    {
                        OperationType = ImprumuturiOperType.DiminuareGajFaraDeposedarePrimit;
                    }

                    else if (operatieGarantie.Garantie.GarantieTip.Description == "Gajuri cu deposedare")
                    {
                        OperationType = ImprumuturiOperType.DiminuareGajCuDeposedarePrimit;
                    }
                }
            }
            else
            {
                if (operatieGarantie.TipOperatieGarantieEnum == TipOperatieGarantieEnum.Majorare)
                {
                    if (operatieGarantie.Garantie.GarantieTip.Description == "Scrisoare de garantie")
                    {
                        OperationType = ImprumuturiOperType.MajorareScrisoareDeGarantieData;
                    }
                    else if (operatieGarantie.Garantie.GarantieTip.Description == "Gajuri fără deposedare")
                    {
                        OperationType = ImprumuturiOperType.MajorareGajFaraDeposedareDat;
                    }

                    else if (operatieGarantie.Garantie.GarantieTip.Description == "Gajuri cu deposedare")
                    {
                        OperationType = ImprumuturiOperType.MajorareGajCuDeposedareDat;
                    }
                }
                else
                {
                    if (operatieGarantie.Garantie.GarantieTip.Description == "Scrisoare de garantie")
                    {
                        OperationType = ImprumuturiOperType.DiminuareScrisoareDeGarantieData;
                    }
                    else if (operatieGarantie.Garantie.GarantieTip.Description == "Gajuri fără deposedare")
                    {
                        OperationType = ImprumuturiOperType.DiminuareGajFaraDeposedareDat;
                    }

                    else if (operatieGarantie.Garantie.GarantieTip.Description == "Gajuri cu deposedare")
                    {
                        OperationType = ImprumuturiOperType.DiminuareGajCuDeposedareDat;
                    }
                }
            }

           

            var monog = Context.AutoOperationConfig.Where(f => f.AutoOperType == AutoOperationType.Imprumuturi)
                                                   .Where(f => f.State == State.Active && operatieGarantie.DataOperatiei >= f.StartDate && operatieGarantie.DataOperatiei <= (f.EndDate ?? operatieGarantie.DataOperatiei)
                                                   && f.OperationType == (int)OperationType)
                                                   .OrderBy(f => f.EntryOrder)
                                                   .ToList();

            if (monog.Count == 0)
            {
                throw new Exception("Nu exista o monografie definita pentru aceasta operatie");
            }

            foreach (var monogItem in monog)
            {

                
                    operationDetail = new OperationDetails
                    {
                        ValueCurr = 0,
                        Value = operatieGarantie.Suma,
                        VAT = 0,
                        Details = "Operatie Garantie" + ", " + documentNumber + " / " + operatieGarantie.DataOperatiei.ToShortDateString()
                    };
                


                // debit
                if (monogItem.DebitAccount == "CB")
                {

                    debitAccountId = Context.Account.Where(f => f.BankAccountId == imprumut.PaymentAccountId).FirstOrDefault().Id;

                }
                else if (monogItem.DebitAccount == "GFDP")
                {
                    var imprumutDetaliu = Context.ImprumutTipDetalii.FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.DebitAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    debitAccountId = GetAutoAccount(imprumutDetaliu.ContImprumut, operatieGarantie.Garantie.LegalPersonId, operatieGarantie.DataOperatiei, operatieGarantie.Garantie.CurrencyId, null);
                }
                else if (monogItem.DebitAccount == "GCDP")
                {
                    var imprumutDetaliu = Context.ImprumutTipDetalii.FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.DebitAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    debitAccountId = GetAutoAccount(imprumutDetaliu.ContImprumut, operatieGarantie.Garantie.LegalPersonId, operatieGarantie.DataOperatiei, operatieGarantie.Garantie.CurrencyId, null);
                }
                else if (monogItem.DebitAccount == "GFP")
                {
                    var imprumutDetaliu = Context.ImprumutTipDetalii.FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.DebitAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    debitAccountId = GetAutoAccount(imprumutDetaliu.ContImprumut, operatieGarantie.Garantie.LegalPersonId, operatieGarantie.DataOperatiei, operatieGarantie.Garantie.CurrencyId, null);
                }
                else if (monogItem.DebitAccount == "GFDD")
                {
                    var imprumutDetaliu = Context.ImprumutTipDetalii.FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.DebitAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    debitAccountId = GetAutoAccount(imprumutDetaliu.ContImprumut, operatieGarantie.Garantie.LegalPersonId, operatieGarantie.DataOperatiei, operatieGarantie.Garantie.CurrencyId, null);
                }
                else if (monogItem.DebitAccount == "GCDD")
                {
                    var imprumutDetaliu = Context.ImprumutTipDetalii.FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.DebitAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    debitAccountId = GetAutoAccount(imprumutDetaliu.ContImprumut, operatieGarantie.Garantie.LegalPersonId, operatieGarantie.DataOperatiei, operatieGarantie.Garantie.CurrencyId, null);
                }
                else if (monogItem.DebitAccount == "GFD")
                {
                    var imprumutDetaliu = Context.ImprumutTipDetalii.FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.DebitAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    debitAccountId = GetAutoAccount(imprumutDetaliu.ContImprumut, operatieGarantie.Garantie.LegalPersonId, operatieGarantie.DataOperatiei, operatieGarantie.Garantie.CurrencyId, null);
                }
                else
                {
                    debitAccountId = GetAutoAccount(monogItem.DebitAccount, operatieGarantie.Garantie.LegalPersonId, DateTime.Now, operatieGarantie.Garantie.CurrencyId, null);
                }

                // credit
                if (monogItem.CreditAccount == "CB")
                {

                    creditAccountId = Context.Account.Where(f => f.BankAccountId == imprumut.PaymentAccountId).FirstOrDefault().Id;
                }
                else if (monogItem.CreditAccount == "GFDP")
                {
                    var imprumutDetaliu = Context.ImprumutTipDetalii.FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.CreditAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    creditAccountId = GetAutoAccount(imprumutDetaliu.ContImprumut, operatieGarantie.Garantie.LegalPersonId, operatieGarantie.DataOperatiei, operatieGarantie.Garantie.CurrencyId, null);
                }
                else if (monogItem.CreditAccount == "GCDP")
                {
                    var imprumutDetaliu = Context.ImprumutTipDetalii.FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.CreditAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    creditAccountId = GetAutoAccount(imprumutDetaliu.ContImprumut, operatieGarantie.Garantie.LegalPersonId, operatieGarantie.DataOperatiei, operatieGarantie.Garantie.CurrencyId, null);
                }
                else if (monogItem.CreditAccount == "GFP")
                {
                    var imprumutDetaliu = Context.ImprumutTipDetalii.FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.CreditAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    creditAccountId = GetAutoAccount(imprumutDetaliu.ContImprumut, operatieGarantie.Garantie.LegalPersonId, operatieGarantie.DataOperatiei, operatieGarantie.Garantie.CurrencyId, null);
                }
                else if (monogItem.CreditAccount == "GFDD")
                {
                    var imprumutDetaliu = Context.ImprumutTipDetalii.FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.CreditAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    creditAccountId = GetAutoAccount(imprumutDetaliu.ContImprumut, operatieGarantie.Garantie.LegalPersonId, operatieGarantie.DataOperatiei, operatieGarantie.Garantie.CurrencyId, null);
                }
                else if (monogItem.CreditAccount == "GCDD")
                {
                    var imprumutDetaliu = Context.ImprumutTipDetalii.FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.CreditAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    creditAccountId = GetAutoAccount(imprumutDetaliu.ContImprumut, operatieGarantie.Garantie.LegalPersonId, operatieGarantie.DataOperatiei, operatieGarantie.Garantie.CurrencyId, null);
                }
                else if (monogItem.CreditAccount == "GFD")
                {
                    var imprumutDetaliu = Context.ImprumutTipDetalii.FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.CreditAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    creditAccountId = GetAutoAccount(imprumutDetaliu.ContImprumut, operatieGarantie.Garantie.LegalPersonId, operatieGarantie.DataOperatiei, operatieGarantie.Garantie.CurrencyId, null);
                }
                else
                {
                    creditAccountId = GetAutoAccount(monogItem.CreditAccount, operatieGarantie.Garantie.LegalPersonId, DateTime.Now, operatieGarantie.Garantie.CurrencyId, null);
                }

                
                    
                
                    operationDetail.DebitId = debitAccountId;
                    operationDetail.CreditId = creditAccountId;

                    operationDetail.Value = operatieGarantie.Suma;
                

                operationDetail.OperationId = operation.Id;

                bool okAddNededOper = true;
                var nededOperDetail = GenerateNededOperDetail(ref operationDetail, DateTime.Now, debitAccountId, out okAddNededOper);


                detailList.Add(operationDetail);
                Context.OperationsDetails.Add(operationDetail);

                if (okAddNededOper)
                {
                    detailList.Add(nededOperDetail);
                    Context.OperationsDetails.Add(nededOperDetail);
                }

                Context.SaveChanges();
                operatieGarantie.ContaOperationDetailId = operationDetail.Id;
                operatieGarantie.ContaOperationId = operation.Id;
            }

            Context.SaveChanges();


        }


    }
}
