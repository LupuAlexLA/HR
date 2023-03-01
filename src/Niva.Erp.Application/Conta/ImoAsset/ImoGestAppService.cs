using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.ImoAsset;
using Niva.Erp.Repositories;
using Niva.Erp.Repositories.Conta;
using Niva.Erp.Repositories.Conta.AutoOperation;
using Niva.Erp.Repositories.ImoAsset;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Conta.ImoAsset
{
    public interface IImoGestAppService : IApplicationService
    {
        ImoGestListDto InitForm();

        List<ImoAssetsDDDto> AssetListDD(DateTime? dataStart, DateTime? dataEnd);

        List<ImoAssetStorageDDDto> StorageListDD(DateTime? dataStart, DateTime? dataEnd);

        ImoGestComputeListDto InitFormCompute();

        ImoGestListDto SerchGest(ImoGestListDto gest);

        ImoGestComputeListDto SerchComputeOper(ImoGestComputeListDto gestCompute);

        ImoGestComputeListDto ComputeDateGest(ImoGestComputeListDto gestCompute);

        ImoGestDelListDto InitFormDel();

        ImoGestDelListDto SerchDateGest(ImoGestDelListDto gestDel);

        void DeleteDateGest(DateTime deleteDate);

        ImoGestRowDto GetGestDetailForAsset(int assetId, DateTime gestDate);
    }

    public class ImoGestAppService : ErpAppServiceBase, IImoGestAppService
    {
        IRepository<ImoAssetStock> _imoGestRepository;
        IRepository<ImoAssetItem> _imoAssetItemRepository;
        IImoOperationRepository _imoOperationRepository;
        IRepository<ImoAssetStock> _imoAssetStockRepository;
        IRepository<ImoAssetOperDocType> _imoAssetOperDocTypeRepository;
        IOperationRepository _operationRepository;
        IBalanceRepository _balanceRepository;
        IAutoOperationRepository _autoOperationRepository;

        public ImoGestAppService(IRepository<ImoAssetStock> imoGestRepository, IImoOperationRepository imoOperationRepository, IRepository<ImoAssetItem> imoAssetItemRepository,
                                 IRepository<ImoAssetOperDocType> imoAssetOperDocTypeRepository, IOperationRepository operationRepository, IBalanceRepository balanceRepository,
                                 IAutoOperationRepository autoOperationRepository, IRepository<ImoAssetStock> imoAssetStockRepository)
        {
            _imoGestRepository = imoGestRepository;
            _imoOperationRepository = imoOperationRepository;
            _imoAssetItemRepository = imoAssetItemRepository;
            _imoAssetOperDocTypeRepository = imoAssetOperDocTypeRepository;
            _imoAssetStockRepository = imoAssetStockRepository;
            _operationRepository = operationRepository;
            _balanceRepository = balanceRepository;
            _autoOperationRepository = autoOperationRepository;

        }

        //[AbpAuthorize("Administrare.MF.CalculGestiune.Acces")]
        public ImoGestListDto InitForm()
        {
            var ret = new ImoGestListDto
            {
                DataStart = _balanceRepository.BalanceDateNextDay(),
                DataEnd = LazyMethods.Now(),
                GestDetail = new List<ImoGestDetailListDto>()
            };
            ret = SerchGest(ret);

            return ret;
        }

        public List<ImoAssetsDDDto> AssetListDD(DateTime? dataStart, DateTime? dataEnd)
        {
            var _dataStart = (dataStart ?? LazyMethods.Now().AddMonths(-1));
            var _dataEnd = (dataEnd ?? LazyMethods.Now());

            var ret = _imoGestRepository.GetAllIncluding(f => f.ImoAssetItem)
                                    .Where(f => f.StockDate >= _dataStart && f.StockDate <= _dataEnd)
                                    .OrderByDescending(f => f.StockDate)
                                    .Select(f => new ImoAssetsDDDto { Id = f.ImoAssetItemId, Name = f.ImoAssetItem.InventoryNr + ". " + f.ImoAssetItem.Name })
                                    .Distinct()
                                    .OrderBy(f => f.Name)
                                    .ToList();

            return ret;
        }

        public List<ImoAssetStorageDDDto> StorageListDD(DateTime? dataStart, DateTime? dataEnd)
        {
            var _dataStart = (dataStart ?? LazyMethods.Now().AddMonths(-1));
            var _dataEnd = (dataEnd ?? LazyMethods.Now());

            var ret = _imoGestRepository.GetAllIncluding(f => f.Storage)
                                    .Where(f => f.StockDate >= _dataStart && f.StockDate <= _dataEnd)
                                    .OrderByDescending(f => f.StockDate)
                                    .Select(f => new ImoAssetStorageDDDto { Id = f.StorageId, Name = f.Storage.StorageName })
                                    .Distinct()
                                    .OrderBy(f => f.Name)
                                    .ToList();

            return ret;
        }

        public ImoGestListDto SerchGest(ImoGestListDto gest)

        {
            try
            {
                var _dataStart = (gest.DataStart ?? LazyMethods.Now().AddMonths(-1));
                var _dataEnd = (gest.DataEnd ?? LazyMethods.Now());

                var gestDetailsDB = _imoGestRepository.GetAllIncluding(f => f.ImoAssetItem, f => f.Storage)
                                                      .Include(f => f.ImoAssetStockReserve)
                                                      .Include(f => f.ImoAssetStockModerniz)
                                                      .ThenInclude(f => f.ImoAssetOperDetail)
                                                      .ThenInclude(f => f.ImoAssetOper)
                                                      .Where(f => f.StockDate >= _dataStart && f.StockDate <= _dataEnd
                                               && f.ImoAssetItemId == (gest.AssetId ?? f.ImoAssetItemId)
                                               && f.StorageId == (gest.StorageId ?? f.StorageId))
                                         .OrderBy(f => f.StockDate).ThenBy(f => f.ImoAssetItem.InventoryNr)
                                         .ToList();
                var gestDetails = new List<ImoGestDetailListDto>();

                foreach (var item in gestDetailsDB)
                {
                    var gestDetail = new ImoGestDetailListDto
                    {
                        Id = item.Id,
                        StockDate = item.StockDate,
                        StockDateStr = item.StockDate.ToShortDateString(),
                        ImoAssetItem = item.ImoAssetItem.InventoryNr + ". " + item.ImoAssetItem.Name,
                        OperType = LazyMethods.EnumValueToDescription(item.OperType),
                        Storage = item.Storage.StorageName,
                        TranzDuration = item.TranzDuration,
                        TranzQuantity = item.TranzQuantity,
                        TranzInventoryValue = item.TranzInventoryValue,
                        TranzFiscalInventoryValue = item.TranzFiscalInventoryValue,
                        TranzDeprec = item.TranzDeprec,
                        TranzFiscalDeprec = item.TranzFiscalDeprec,
                        Duration = item.Duration,
                        Quantity = item.Quantity,
                        InventoryValue = item.InventoryValue,
                        FiscalInventoryValue = item.FiscalInventoryValue,
                        Deprec = item.Deprec,
                        FiscalDeprec = item.FiscalDeprec,
                        ShowReserve = false,
                        ShowModerniz = false
                    };

                    var reserveDetails = new List<ImoGestReserveDto>();
                    foreach (var itemReserve in item.ImoAssetStockReserve)
                    {
                        var reserveDetail = new ImoGestReserveDto
                        {
                            ImoAssetStockId = itemReserve.ImoAssetStockId ?? 0,
                            TranzDeprecReserve = itemReserve.TranzDeprecReserve,
                            TranzReserve = itemReserve.TranzReserve,
                            DeprecReserve = itemReserve.DeprecReserve,
                            Reserve = itemReserve.Reserve,
                            ExpenseReserve = itemReserve.ExpenseReserve,
                            OperationDate = itemReserve.ImoAssetOperDetail.ImoAssetOper.OperationDate.ToShortDateString()  // DateTime.Now.ToShortDateString()// 
                        };
                        reserveDetails.Add(reserveDetail);
                    }
                    gestDetail.ReserveDetail = reserveDetails;



                    var modernizDetails = new List<ImoGestModernizDto>();
                    foreach (var itemModerniz in item.ImoAssetStockModerniz)
                    {
                        var modernizDetail = new ImoGestModernizDto
                        {
                            ImoAssetStockId = itemModerniz.ImoAssetStockId ?? 0,
                            TranzDeprecModerniz = itemModerniz.TranzDeprecModerniz,
                            TranzModerniz = itemModerniz.TranzModerniz,
                            DeprecModerniz = itemModerniz.DeprecModerniz,
                            Moderniz = itemModerniz.Moderniz,
                            ExpenseModerniz = itemModerniz.ExpenseModerniz,
                            OperationDate = itemModerniz.ImoAssetOperDetail.ImoAssetOper.OperationDate.ToShortDateString()  // DateTime.Now.ToShortDateString()// 
                        };
                        modernizDetails.Add(modernizDetail);
                    }
                    gestDetail.ModernizDetail = modernizDetails.OrderByDescending(f => f.OperationDate).ToList();

                    gestDetails.Add(gestDetail);

                }

                gest.GestDetail = gestDetails;
                return gest;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public ImoGestComputeListDto InitFormCompute()
        {
            var ret = new ImoGestComputeListDto
            {
                UnprocessedDate = _imoOperationRepository.UnprocessedDate().AddDays(1),
                ComputeDate = LazyMethods.Now(),
                ShowCompute = false,
                OperationList = new List<ImoOperationListDto>()
            };
            ret = SerchComputeOper(ret);

            return ret;
        }

        public ImoGestComputeListDto SerchComputeOper(ImoGestComputeListDto gestCompute)
        {
            var _computeDate = (gestCompute.ComputeDate ?? LazyMethods.Now());
            var _unprocessedDate = gestCompute.UnprocessedDate;

            if (!_operationRepository.VerifyClosedMonth(_computeDate))
                throw new UserFriendlyException("Eroare", "Nu se poate inregistra operatia deoarece luna contabila este inchisa");

            var gestDetail = new List<ImoOperationListDto>();

            // operatii de intrare neprocesate din imoAssetItem
            var assetInDetail = _imoAssetItemRepository.GetAllIncluding(f => f.ImoAssetStorage, f => f.DocumentType)
                                     .Where(f => f.OperationDate <= _computeDate && f.ProcessedIn == false && f.State == State.Active)
                                     .OrderBy(f => f.OperationDate)
                                     .ToList()
                                     .Select(f => new ImoOperationListDto
                                     {
                                         DocumentNr = f.DocumentNr,
                                         DocumentDate = f.DocumentDate,
                                         OperationType = (ImoAssetOperType.Intrare).ToString(),
                                         StorageIn = f.ImoAssetStorage.StorageName,
                                         StorageOut = null,
                                         DocumentType = f.DocumentType.TypeName,
                                         OperationDate = f.OperationDate,
                                         Id = f.Id,
                                         OrdProcess = 1,
                                         OperationDateSort = new DateTime(f.OperationDate.Year, f.OperationDate.Month, f.OperationDate.Day)
                                     })
                                     .Distinct()
                                     .ToList();

            // operatii de punere in functiune neprocesate din imoAssetItem
            var assetInUseDetail = _imoAssetItemRepository.GetAllIncluding(f => f.ImoAssetStorage, f => f.DocumentType)
                                     .Where(f => f.UseStartDate <= _computeDate && f.ProcessedInUse == false && f.UseStartDate != null && f.State == State.Active)
                                     .OrderBy(f => f.OperationDate)
                                     .ToList()
                                     .Select(f => new ImoOperationListDto
                                     {
                                         DocumentNr = f.DocumentNr,
                                         DocumentDate = f.DocumentDate,
                                         OperationType = LazyMethods.EnumValueToDescription((ImoAssetOperType.PunereInFunctiune)),
                                         StorageIn = f.ImoAssetStorage.StorageName,
                                         StorageOut = null,
                                         DocumentType = f.DocumentType.TypeName,
                                         OperationDate = f.UseStartDate.Value,
                                         Id = f.Id,
                                         OrdProcess = 1,
                                         OperationDateSort = new DateTime(f.UseStartDate.Value.Year, f.UseStartDate.Value.Month, f.UseStartDate.Value.Day)
                                     })
                                     .Distinct()
                                     .ToList();

            // operatii neprocesate din imoAssetOper
            var operDetail = _imoOperationRepository.GetAllIncluding(f => f.AssetsStoreIn, f => f.AssetsStoreOut, f => f.DocumentType)
                                     .Where(f => f.OperUseStartDate <= _computeDate && f.Processed == false && f.State == State.Active)
                                     .OrderBy(f => f.OperationDate)
                                     .ToList()
                                     .Select(f => new ImoOperationListDto
                                     {
                                         DocumentNr = f.DocumentNr,
                                         DocumentDate = f.DocumentDate,
                                         OperationType = LazyMethods.EnumValueToDescription(f.AssetsOperType),
                                         StorageIn = (f.AssetsStoreIn == null) ? null : f.AssetsStoreIn.StorageName,
                                         StorageOut = (f.AssetsStoreOut == null) ? null : f.AssetsStoreOut.StorageName,
                                         DocumentType = f.DocumentType.TypeName,
                                         OperationDate = f.OperUseStartDate,
                                         Id = f.Id,
                                         OrdProcess = ((f.AssetsOperType == ImoAssetOperType.Reevaluare) || (f.AssetsOperType == ImoAssetOperType.Casare) || (f.AssetsOperType == ImoAssetOperType.Iesire)) ? 3 : 1,
                                         OperationDateSort = new DateTime(f.OperUseStartDate.Year, f.OperUseStartDate.Month, f.OperUseStartDate.Day)
                                     })
                                     .Distinct()
                                     .ToList();
            // date de sfarsit de luna pentru amortizare
            var deprecDetail = new List<ImoOperationListDto>();

            var assetList = _imoAssetStockRepository.GetAllIncluding(f => f.ImoAssetItem).GroupBy(f => f.ImoAssetItemId).Select(f => f.Max(x => x.Id)).ToList();
            var stockList = _imoAssetStockRepository.GetAllIncluding(f => f.ImoAssetItem)
                                                    .Where(f => assetList.Contains(f.Id) && f.Quantity != 0 && f.ImoAssetItem.OperationDate <= gestCompute.ComputeDate)
                                                    .ToList();
            var minProcessedDate = (stockList.Count > 0 ? stockList.Min(f => f.StockDate) : _unprocessedDate);

            var currDate = minProcessedDate.AddDays(1);
            while (currDate <= _computeDate)
            {
                if (currDate.Day == DateTime.DaysInMonth(currDate.Year, currDate.Month))
                {
                    var docType = _imoAssetOperDocTypeRepository.GetAllIncluding(f => f.DocumentType)
                                                                .FirstOrDefault(f => f.OperType == ImoAssetOperType.AmortizareLunara);
                    if (docType != null)
                    {
                        var deprec = new ImoOperationListDto
                        {
                            DocumentType = docType.DocumentType.TypeName,
                            DocumentDate = currDate,
                            OperationDate = currDate,
                            OperationType = LazyMethods.EnumValueToDescription(ImoAssetOperType.AmortizareLunara),
                            OrdProcess = 2,
                            OperationDateSort = currDate
                        };
                        deprecDetail.Add(deprec);
                    }

                }
                currDate = currDate.AddDays(1);
            }

            foreach (var item in assetInDetail)
            {
                gestDetail.Add(item);
            }
            foreach (var item in assetInUseDetail)
            {
                gestDetail.Add(item);
            }
            foreach (var item in operDetail)
            {
                gestDetail.Add(item);
            }
            foreach (var item in deprecDetail)
            {
                gestDetail.Add(item);
            }

            foreach (var item in gestDetail)
            {
                item.OperationDateStr = item.OperationDate.ToShortDateString();
                item.DocumentDateStr = item.DocumentDate.ToShortDateString();
            }

            gestDetail = gestDetail.OrderBy(f => f.OperationDateSort).ThenBy(f => f.OrdProcess).ToList();

            gestCompute.OperationList = gestDetail;
            gestCompute.ShowCompute = true;
            return gestCompute;
        }
        //[AbpAuthorize("Administrare.MF.CalculGestiune.Acces")]
        public ImoGestComputeListDto ComputeDateGest(ImoGestComputeListDto gestCompute)
        {
            if (!_operationRepository.VerifyClosedMonth(gestCompute.ComputeDate ?? LazyMethods.Now()))
                throw new UserFriendlyException("Eroare", "Nu se poate inregistra operatia deoarece luna contabila este inchisa");

            //_imoOperationRepository.GestAssetComputing(gestCompute.ComputeDate ?? DateTime.Now);

            _imoOperationRepository.GestImoAssetsComputing(gestCompute.ComputeDate ?? LazyMethods.Now());

            var appClient = GetCurrentTenant();
            var lastBalanceDate = _balanceRepository.GetAll().Where(f => f.Status == State.Active).OrderByDescending(f => f.BalanceDate).FirstOrDefault().BalanceDate;

            // generez note contabile din gestiune
            _autoOperationRepository.AutoImoAssetOperationAdd(gestCompute.ComputeDate ?? LazyMethods.Now(), appClient.Id, appClient.LocalCurrencyId.Value, ImoAssetType.MijlocFix, lastBalanceDate, null);

            // gestCompute.UnprocessedDate = _imoOperationRepository.LastProcessedDate().AddDays(1);
            gestCompute = SerchComputeOper(gestCompute);
            gestCompute.ShowCompute = false;
            return gestCompute;
        }

        public ImoGestDelListDto InitFormDel()
        {
            var ret = new ImoGestDelListDto
            {
                DataStart = LazyMethods.Now().AddMonths(-1),
                DataEnd = LazyMethods.Now(),
                GestDelDetail = new List<ImoGestDelDetailDto>()
            };
            ret = SerchDateGest(ret);

            return ret;
        }

        public ImoGestDelListDto SerchDateGest(ImoGestDelListDto gestDel)
        {
            var _dataStart = (gestDel.DataStart ?? LazyMethods.Now().AddMonths(-1));
            var _dataEnd = (gestDel.DataEnd ?? LazyMethods.Now());

            var gestDetail = _imoGestRepository.GetAll()
                                     .Where(f => f.StockDate >= _dataStart && f.StockDate <= _dataEnd)
                                     .Select(f => new ImoGestDelDetailDto
                                     {
                                         DateGest = f.StockDate
                                     })
                                     .Distinct()
                                     .OrderByDescending(f => f.DateGest)
                                     .ToList();
            gestDel.GestDelDetail = gestDetail;
            return gestDel;
        }
        //[AbpAuthorize("Administrare.MF.CalculGestiune.Acces")]
        public void DeleteDateGest(DateTime deleteDate)
        {
            if (!_operationRepository.VerifyClosedMonth(deleteDate))
                throw new UserFriendlyException("Eroare", "Nu se poate sterge operatia deoarece luna contabila este inchisa");

            //sterg notele contabile nevalidate
            try
            {
                _autoOperationRepository.DeleteAssetOperations(deleteDate);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", "Gestiunea nu poate fi stearsa, deoarece " + ex.Message);
            }

            try
            {
                _imoOperationRepository.GestAssetDelComputing(deleteDate);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", "Gestiunea nu poate fi stearsa, deoarece " + ex.Message);
            }
        }

        public ImoGestRowDto GetGestDetailForAsset(int assetId, DateTime gestDate)
        {
            var gest = _imoGestRepository.GetAll().Where(f => f.ImoAssetItemId == assetId && f.StockDate <= gestDate)
                                         .OrderByDescending(f => f.StockDate).ThenByDescending(f => f.Id)
                                         .FirstOrDefault();
            var ret = ObjectMapper.Map<ImoGestRowDto>(gest);
            return ret;
        }
    }
}
