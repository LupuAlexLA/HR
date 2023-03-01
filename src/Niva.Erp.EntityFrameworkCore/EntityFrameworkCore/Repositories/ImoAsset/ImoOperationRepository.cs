using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.ImoAsset;
using Niva.Erp.Repositories.ImoAsset;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.EntityFrameworkCore.Repositories.ImoAsset
{
    public class ImoOperationRepository : ErpRepositoryBase<ImoAssetOper, int>, IImoOperationRepository
    {
        public ImoOperationRepository(IDbContextProvider<ErpDbContext> context) : base(context)
        {

        }

        public DateTime UnprocessedDate()
        {
            DateTime ret;
            var assetDate = Context.ImoAssetItem
                                            .Where(f => f.State == State.Active && f.ProcessedIn == false && f.ProcessedInUse == false )
                                            .OrderBy(f => f.UseStartDate)
                                           // .OrderBy(f => f.OperationDate)
                                            .FirstOrDefault();
            var operation = Context.ImoAssetOper
                                            .Where(f => f.State == State.Active && f.Processed == false)
                                            .OrderBy(f => f.OperUseStartDate)
                                            .FirstOrDefault();
            DateTime gestDate;
            var countStock = Context.ImoAssetStock.Count();
            if (countStock == 0)
            {
                gestDate = DateTime.Now;
            }
            else
            {
                gestDate = Context.ImoAssetStock.Max(f => f.StockDate).AddDays(1);
            }
            ret = (assetDate == null) ? DateTime.Now : (assetDate.UseStartDate.HasValue ? assetDate.UseStartDate.Value : assetDate.OperationDate);
            if (operation != null)
            {
                var operDate = operation.OperUseStartDate;
                ret = (ret < operDate) ? ret : operDate;
            }
            ret = (ret < gestDate) ? ret : gestDate;

            return ret;
        }

        public DateTime LastProcessedDate()
        {
            DateTime ret;

            var count = Context.ImoAssetStock.Count();
            if (count != 0)
            {
                ret = Context.ImoAssetStock.Max(f => f.StockDate);
            }
            else
            {
                var assetDate = Context.ImoAssetItem
                                            .Where(f => f.State == State.Active && f.Processed == true)
                                            .OrderBy(f => f.UseStartDate)
                                            .FirstOrDefault();
                var operation = Context.ImoAssetOper
                                                .Where(f => f.State == State.Active && f.Processed == true)
                                                .OrderBy(f => f.OperUseStartDate)
                                                .FirstOrDefault();
                ret = (assetDate == null) ? DateTime.Now.AddYears(-10) : assetDate.UseStartDate.Value.AddDays(-1);
                if (operation != null)
                {
                    var operDate = operation.OperUseStartDate.AddDays(-1);
                    ret = (ret < operDate) ? ret : operDate;
                }
            }


            return ret;
        }

        public DateTime LastProcessedDateAdd()
        {
            DateTime ret;

            var count = Context.ImoAssetStock.Count();
            if (count != 0)
            {
                ret = Context.ImoAssetStock.Max(f => f.StockDate);
            }
            else
            {
                ret = DateTime.Now.AddYears(-10);
                //var assetDate = Context.ImoAssetItem
                //                            .Where(f => f.AppClientId == appClientId && f.State == State.Active && f.Processed == false)
                //                            .OrderBy(f => f.UseStartDate)
                //                            .FirstOrDefault();
                //var operation = Context.ImoAssetOper
                //                                .Where(f => f.AppClientId == appClientId && f.State == State.Active && f.Processed == false)
                //                                .OrderBy(f => f.OperUseStartDate)
                //                                .FirstOrDefault();
                //ret = (assetDate == null) ? DateTime.Now.AddYears(-10) : assetDate.UseStartDate.AddDays(-1);
                //if (operation != null)
                //{
                //    var operDate = operation.OperUseStartDate.AddDays(-1);
                //    ret = (ret < operDate) ? ret : operDate;
                //}
            }


            return ret;
        }

        public void GestAssetComputing(DateTime operationDate)
        {
            try
            {
                var lastProcessedDate = UnprocessedDate();
                var operList = OperationList(lastProcessedDate, operationDate);

                foreach (var item in operList)
                {
                    ComputeOperation(item);
                    Context.SaveChanges();
                }

                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // calculez gestiunea pentru toate mijloacele fixe la data primita ca parametru
        public void GestImoAssetsComputing(DateTime operationDate)
        {
            try
            {
                var assetList = ImoAssetsList(operationDate.Date);

                foreach (var asset in assetList)
                {
                    var lastProcessedDate = LastProcessedDateForAsset(asset.Id).AddDays(1);
                    var operList = OperListForAsset(lastProcessedDate, operationDate, asset.Id);

                    foreach (var item in operList)
                    {
                        ComputeOperationForAsset(item, asset.Id);
                        Context.SaveChanges();
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        // Returnez lista mijloacelor fixe pana la data primita ca parametru
        private List<ImoAssetItem> ImoAssetsList(DateTime operationDate)
        {
            var assetList = new List<ImoAssetItem>();

            // mijloace fixe neprocesate in gestiune
            var assests = Context.ImoAssetItem.Where(f => f.State == State.Active && !f.ProcessedInUse && !f.ProcessedIn).ToList();

            // mijloace fixe cu sold != 0 la data specificata ca parametru
            var assetsInStock = Context.ImoAssetStock.Include(f => f.ImoAssetItem)
                                                     .Where(f => f.StockDate <= operationDate)
                                                     .GroupBy(f => f.ImoAssetItemId)
                                                     .Select(f => f.Max(f => f.Id))
                                                     .ToList();

            var stockList = Context.ImoAssetStock.Include(f => f.ImoAssetItem)
                                                 .Where(f => f.Quantity != 0 && assetsInStock.Contains(f.Id))
                                                 .ToList();

            var assetOper = Context.ImoAssetOper.Where(f => f.State == State.Active && !f.Processed).ToList();

            foreach (var item in assests)
            {
                assetList.Add(item);
            }

            foreach (var item in stockList)
            {
                assetList.Add(item.ImoAssetItem);
            }

            return assetList;
        }

        public List<OperationForComputing> OperationList(DateTime lastProcessedDate, DateTime operationDate)
        {
            var ret = new List<OperationForComputing>();

            var assetDateList = Context.ImoAssetItem
                                  .Where(f => f.State == State.Active && f.UseStartDate <= operationDate.Date && !f.Processed)
                                  .ToList()
                                  .Select(f => new OperationForComputing
                                  {
                                      Id = f.Id,
                                      OperationDate = f.UseStartDate.Value,
                                      OperationSource = OperationSource.ImoAsset,
                                      OperationOrd = 1,
                                      OperationDateSort = new DateTime(f.UseStartDate.Value.Year, f.UseStartDate.Value.Month, f.UseStartDate.Value.Day)
                                  })
                                  .ToList();
            var operDateList = Context.ImoAssetOper
                                  .Where(f => f.State == State.Active && f.OperUseStartDate <= operationDate && !f.Processed)
                                  .ToList()
                                  .Select(f => new OperationForComputing
                                  {
                                      Id = f.Id,
                                      OperationDate = f.OperUseStartDate,
                                      OperationSource = OperationSource.ImoOper,
                                      OperationOrd = (((f.AssetsOperType == ImoAssetOperType.Reevaluare) || (f.AssetsOperType == ImoAssetOperType.Casare) || (f.AssetsOperType == ImoAssetOperType.Iesire)) ? 4 : 2),
                                      OperationDateSort = new DateTime(f.OperUseStartDate.Year, f.OperUseStartDate.Month, f.OperUseStartDate.Day)
                                  })
                                  .ToList();

            var deprecDateList = GetDeprecList(lastProcessedDate, operationDate);

            foreach (var item in assetDateList)
            {
                ret.Add(item);
            }

            foreach (var item in operDateList)
            {
                ret.Add(item);
            }

            foreach (var item in deprecDateList)
            {
                ret.Add(item);
            }

            ret = ret.Distinct().OrderBy(f => f.OperationDateSort).ThenBy(f => f.OperationOrd).ToList();

            return ret;
        }

        public List<OperationForComputing> GetDeprecList(DateTime lastProcessedDate, DateTime operationDate)
        {
            var ret = new List<OperationForComputing>();
            DateTime currDate;
            var firstAssetDay = Context.ImoAssetItem.Where(f => f.State == State.Active).OrderBy(f => f.UseStartDate).FirstOrDefault();

            if (firstAssetDay == null)
            {
                return ret;
            }
            else
            {
                currDate = (lastProcessedDate > firstAssetDay.UseStartDate) ? lastProcessedDate : firstAssetDay.UseStartDate.Value;
            }

            while (currDate <= operationDate)
            {
                if (currDate.Day == DateTime.DaysInMonth(currDate.Year, currDate.Month))
                {
                    ret.Add(new OperationForComputing
                    {
                        OperationDate = currDate,
                        OperationSource = OperationSource.AmortizareLunara,
                        OperationOrd = 3,
                        OperationDateSort = new DateTime(currDate.Year, currDate.Month, currDate.Day)
                    });
                }
                currDate = currDate.AddDays(1);
            }
            return ret;
        }

        public void ComputeOperation(OperationForComputing operation)
        {
            try
            {
                switch (operation.OperationSource)
                {
                    case OperationSource.ImoAsset:
                        ComputeAsset(operation);
                        break;
                    case OperationSource.ImoOper:
                        ComputeOper(operation);
                        break;
                    case OperationSource.AmortizareLunara:
                        GestDeprecComputing(operation);
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        // procesare din ImoAssetItem
        public void ComputeAsset(OperationForComputing operation)
        {
            var asset = Context.ImoAssetItem.FirstOrDefault(f => f.Id == operation.Id);
            int tranzQuantity = 0, quantity = 0, tranzDuration = 0, duration = 0;
            decimal tranzInventoryValue = 0, inventoryValue = 0, tranzFiscalInventoryValue = 0, fiscalInventoryValue = 0, tranzDeprec = 0, deprec = 0, tranzFiscalDeprec = 0, fiscalDeprec = 0;
            decimal monthlyDepreciation = 0, monthlyFiscalDepreciation = 0;
            bool inConservare = false;

            if (operation.ImoAssetOperType == ImoAssetOperType.Intrare)
            {
                tranzQuantity = 1;
                tranzDuration = asset.RemainingDuration ?? asset.DurationInMonths;
                tranzInventoryValue = asset.InventoryValue - (asset.Depreciation ?? 0);
                tranzFiscalInventoryValue = asset.FiscalInventoryValue - (asset.FiscalDepreciation ?? 0);
                tranzDeprec = asset.Depreciation ?? 0;
                tranzFiscalDeprec = asset.FiscalDepreciation ?? 0;

                quantity += tranzQuantity;
                duration += tranzDuration;
                inventoryValue += tranzInventoryValue;
                fiscalInventoryValue += tranzFiscalInventoryValue;
                deprec += tranzDeprec;
                fiscalDeprec += tranzFiscalDeprec;

                monthlyDepreciation = (asset.MonthlyDepreciation ?? (tranzDuration != 0 ? Math.Round(tranzInventoryValue / tranzDuration, 2) : 0));
                monthlyFiscalDepreciation = (asset.MonthlyFiscalDepreciation ?? (tranzDuration != 0 ? Math.Round(tranzFiscalInventoryValue / tranzDuration, 2) : 0));
            }
            else // punere in functiune
            {
                quantity += asset.Quantity;
                duration += asset.RemainingDuration ?? asset.DurationInMonths;
                inventoryValue += asset.InventoryValue - (asset.Depreciation ?? 0);
                fiscalInventoryValue += asset.FiscalInventoryValue - (asset.FiscalDepreciation ?? 0);
                deprec += asset.Depreciation ?? 0;
                fiscalDeprec += asset.FiscalDepreciation ?? 0;

                monthlyDepreciation = (asset.MonthlyDepreciation ?? (duration != 0 ? Math.Round(inventoryValue / duration, 2) : 0));
                monthlyFiscalDepreciation = (asset.MonthlyFiscalDepreciation ?? (duration != 0 ? Math.Round(fiscalInventoryValue / duration, 2) : 0));
            }

            var stock = new ImoAssetStock
            {
                ImoAssetItemId = asset.Id,
                ImoAssetItemPFId = asset.Id,
                ImoAssetOperDetId = null,
                StockDate = (asset.UseStartDate.HasValue && asset.ProcessedIn) ? new DateTime(asset.UseStartDate.Value.Year, asset.UseStartDate.Value.Month, asset.UseStartDate.Value.Day)
                                                        : new DateTime(asset.OperationDate.Year, asset.OperationDate.Month, asset.OperationDate.Day),
                InConservare = inConservare,
                TranzQuantity = tranzQuantity,
                Quantity = quantity,
                TranzDuration = tranzDuration,
                Duration = duration,
                TranzInventoryValue = tranzInventoryValue,
                InventoryValue = inventoryValue,
                TranzFiscalInventoryValue = tranzFiscalInventoryValue,
                FiscalInventoryValue = fiscalInventoryValue,
                TranzDeprec = tranzDeprec,
                Deprec = deprec,
                TranzFiscalDeprec = tranzFiscalDeprec,
                FiscalDeprec = fiscalDeprec,
                StorageId = asset.ImoAssetStorageId ?? 0,
                OperType = asset.ProcessedIn == true ? ImoAssetOperType.PunereInFunctiune : ImoAssetOperType.Intrare,
                MonthlyDepreciation = monthlyDepreciation,
                MonthlyFiscalDepreciation = monthlyFiscalDepreciation,
                AssetAccountId = asset.AssetAccountInUseId ?? asset.AssetAccountId
            };

            Context.ImoAssetStock.Add(stock);
            asset.Processed = true;

            asset.ProcessedInUse = stock.OperType == ImoAssetOperType.PunereInFunctiune ? true : false;
            asset.ProcessedIn = true;
            Context.SaveChanges();

        }

        // calculez amortizarea
        public void GestDeprecComputing(OperationForComputing operation)
        {
            try
            {
                var _reserveDeprec = false;
                var setupItem = Context.ImoAssetSetup.FirstOrDefault();
                if (setupItem != null)
                {
                    _reserveDeprec = setupItem.ReserveDepreciation;
                }

                var assetList = Context.ImoAssetStock.GroupBy(f => f.ImoAssetItemId)
                                                     .Select(f => new { AssetId = f.Key, Id = f.Max(x => x.Id) }).ToList();

                foreach (var item in assetList)
                {
                    var stockItem = Context.ImoAssetStock.Include(f=>f.ImoAssetItem).FirstOrDefault(f => f.Id == item.Id);

                    int tranzQuantity = 0, quantity = stockItem.Quantity, tranzDuration = 0, duration = stockItem.Duration;
                    decimal tranzInventoryValue = 0, inventoryValue = stockItem.InventoryValue, tranzFiscalInventoryValue = 0, fiscalInventoryValue = stockItem.FiscalInventoryValue;
                    decimal tranzDeprec = 0, deprec = stockItem.Deprec, tranzFiscalDeprec = 0, fiscalDeprec = stockItem.FiscalDeprec;
                    decimal monthlyDepreciation = stockItem.MonthlyDepreciation, monthlyFiscalDepreciation = stockItem.MonthlyFiscalDepreciation;
                    bool inConservare = stockItem.InConservare;

                    if (duration != 0)
                    {
                        tranzDuration = (inConservare && duration != 0) ? 0 : -1;
                        tranzDeprec = ((inConservare && duration != 0) ? 0 : (duration == 1 ? inventoryValue : monthlyDepreciation));// Math.Round(inventoryValue / duration, 2));
                        tranzFiscalDeprec = ((inConservare && duration != 0) ? 0 : (duration == 1 ? fiscalInventoryValue : monthlyFiscalDepreciation)); //Math.Round(fiscalInventoryValue / duration, 2));
                        tranzInventoryValue = -1 * tranzDeprec;
                        tranzFiscalInventoryValue = -1 * tranzFiscalDeprec;


                        quantity += tranzQuantity;
                        duration += tranzDuration;
                        inventoryValue += tranzInventoryValue;
                        fiscalInventoryValue += tranzFiscalInventoryValue;
                        deprec += tranzDeprec;
                        fiscalDeprec += tranzFiscalDeprec;

                        var stock = new ImoAssetStock
                        {
                            ImoAssetItemId = stockItem.ImoAssetItemId,
                            ImoAssetItemPFId = null,
                            ImoAssetOperDetId = null,
                            StockDate = new DateTime(operation.OperationDate.Year, operation.OperationDate.Month, operation.OperationDate.Day),
                            InConservare = inConservare,
                            TranzQuantity = tranzQuantity,
                            Quantity = quantity,
                            TranzDuration = tranzDuration,
                            Duration = duration,
                            TranzInventoryValue = tranzInventoryValue,
                            InventoryValue = inventoryValue,
                            TranzFiscalInventoryValue = tranzFiscalInventoryValue,
                            FiscalInventoryValue = fiscalInventoryValue,
                            TranzDeprec = tranzDeprec,
                            Deprec = deprec,
                            TranzFiscalDeprec = tranzFiscalDeprec,
                            FiscalDeprec = fiscalDeprec,
                            StorageId = stockItem.StorageId,
                            OperType = ImoAssetOperType.AmortizareLunara,
                            MonthlyDepreciation = monthlyDepreciation,
                            MonthlyFiscalDepreciation = monthlyFiscalDepreciation,
                            AssetAccountId = stockItem.ImoAssetItem.AssetAccountInUseId ?? stockItem.ImoAssetItem.AssetAccountId
                        };

                        Context.ImoAssetStock.Add(stock);

                        // reserve depreciation
                        var reserveStock = Context.ImoAssetStockReserve.Where(f => f.ImoAssetStockId == item.Id);
                        foreach (var reserveItem in reserveStock)
                        {
                            decimal tranzReserve = 0, tranzReserveDeprec = 0, reserve = reserveItem.Reserve, reserveDeprec = reserveItem.DeprecReserve;
                            tranzReserveDeprec = ((inConservare && duration != 0 && !_reserveDeprec) ? 0 : Math.Round(reserve / stockItem.Duration, 2));
                            tranzReserve = -1 * tranzReserveDeprec;
                            reserveDeprec += tranzReserveDeprec;
                            reserve += tranzReserve;

                            var reserveItemDb = new ImoAssetStockReserve
                            {
                                ImoAssetStock = stock,
                                TranzDeprecReserve = tranzReserveDeprec,
                                TranzReserve = tranzReserve,
                                DeprecReserve = reserveDeprec,
                                Reserve = reserve,
                                ExpenseReserve = 0,
                                ImoAssetOperDetailId = reserveItem.ImoAssetOperDetailId
                            };
                            Context.ImoAssetStockReserve.Add(reserveItemDb);
                        }

                        // moderniz depreciation
                        var modernizStock = Context.ImoAssetStockModerniz.Where(f => f.ImoAssetStockId == item.Id);
                        foreach (var modernizItem in modernizStock)
                        {
                            decimal tranzModerniz = 0, tranzModernizDeprec = 0, moderniz = modernizItem.Moderniz, modernizDeprec = modernizItem.DeprecModerniz;
                            tranzModernizDeprec = ((inConservare && duration != 0 && !_reserveDeprec) ? 0 : Math.Round(moderniz / stockItem.Duration, 2));
                            tranzModerniz = -1 * tranzModernizDeprec;
                            modernizDeprec += tranzModernizDeprec;
                            moderniz += tranzModerniz;

                            var modernizItemDb = new ImoAssetStockModerniz
                            {
                                ImoAssetStock = stock,
                                TranzDeprecModerniz = tranzModernizDeprec,
                                TranzModerniz = tranzModerniz,
                                DeprecModerniz = modernizDeprec,
                                Moderniz = moderniz,
                                ExpenseModerniz = 0,
                                ImoAssetOperDetailId = modernizItem.ImoAssetOperDetailId
                            };
                            Context.ImoAssetStockModerniz.Add(modernizItemDb);
                        }
                    }
                    Context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // procesare operatie
        public void ComputeOper(OperationForComputing operation)
        {
            var imoOper = Context.ImoAssetOper.Include(f => f.OperDetails).FirstOrDefault(f => f.Id == operation.Id);
            // reevaluare
            if (imoOper.AssetsOperType == ImoAssetOperType.Reevaluare)
            {
                ComputeReevaluare(imoOper);
            }
            // modernizare
            if (imoOper.AssetsOperType == ImoAssetOperType.Modernizare)
            {
                ComputeModernizare(imoOper);
            }
            // modernizari, intrari/iesiri in conservare
            else if ((imoOper.AssetsOperType == ImoAssetOperType.IntrareInConservare) || (imoOper.AssetsOperType == ImoAssetOperType.IesireDinConservare))
            {
                ComputeOperation(imoOper);
            }
            // transfer / bonuri de miscare
            else if ((imoOper.AssetsOperType == ImoAssetOperType.BonMiscare) || (imoOper.AssetsOperType == ImoAssetOperType.Transfer))
            {
                ComputeTransfer(imoOper);
            }
            // vanzare
            else if ((imoOper.AssetsOperType == ImoAssetOperType.Vanzare) || (imoOper.AssetsOperType == ImoAssetOperType.Casare) || (imoOper.AssetsOperType == ImoAssetOperType.Iesire))
            {
                ComputeSale(imoOper);
            }
            // modificare conturi cu inregistrare contabila de miscare a soldului
            else if((imoOper.AssetsOperType == ImoAssetOperType.ModificareConturiCuInregistrareNotaContabila))
            {
                ComputeAccountModif(imoOper);
            }
            imoOper.Processed = true;
        }

        public void ComputeAccountModif(ImoAssetOper operation)
        {
            foreach (var item in operation.OperDetails)
            {
                var stockItem = Context.ImoAssetStock.Include(f => f.ImoAssetItem)
                                       .Where(f => f.ImoAssetItemId == item.ImoAssetItemId)
                                       .OrderByDescending(f => f.Id)
                                       .FirstOrDefault();

                var stock = new ImoAssetStock
                {
                    ImoAssetItemId = stockItem.ImoAssetItemId,
                    ImoAssetItemPFId = null,
                    ImoAssetOperDetId = item.Id,
                    StockDate = new DateTime(operation.OperationDate.Year, operation.OperationDate.Month, operation.OperationDate.Day),
                    InConservare = stockItem.InConservare,
                    TranzQuantity = 0,
                    Quantity = stockItem.Quantity,
                    TranzDuration = 0,
                    Duration = stockItem.Duration,
                    TranzInventoryValue = 0,
                    InventoryValue = stockItem.InventoryValue,
                    TranzFiscalInventoryValue = 0,
                    FiscalInventoryValue = stockItem.FiscalInventoryValue,
                    TranzDeprec = 0,
                    Deprec = stockItem.Deprec,
                    TranzFiscalDeprec = 0,
                    FiscalDeprec = stockItem.FiscalDeprec,
                    StorageId = stockItem.StorageId,
                    OperType = operation.AssetsOperType,
                    MonthlyDepreciation = stockItem.MonthlyDepreciation,
                    MonthlyFiscalDepreciation = stockItem.MonthlyFiscalDepreciation,
                    AssetAccountId = item.NewAssetAccountId,
                    ImoAssetOperDet = item
                };
                Context.ImoAssetStock.Add(stock);

                //stockItem.AssetAccountId = item.NewAssetAccountId;

                Context.SaveChanges();
            }
        }

        public void ComputeReevaluare(ImoAssetOper operation)
        {
            foreach (var item in operation.OperDetails)
            {
                var stockItem = Context.ImoAssetStock.Include(f=>f.ImoAssetItem)
                                       .Where(f => f.ImoAssetItemId == item.ImoAssetItemId)
                                       .OrderByDescending(f => f.Id)
                                       .FirstOrDefault();

                int tranzQuantity = 0, quantity = stockItem.Quantity, tranzDuration = 0, duration = stockItem.Duration;
                decimal tranzInventoryValue = 0, inventoryValue = stockItem.InventoryValue, tranzFiscalInventoryValue = 0, fiscalInventoryValue = stockItem.FiscalInventoryValue;
                decimal tranzDeprec = 0, deprec = stockItem.Deprec, tranzFiscalDeprec = 0, fiscalDeprec = stockItem.FiscalDeprec;
                decimal monthlyDepreciation = 0, monthlyFiscalDepreciation = 0;
                bool inConservare = stockItem.InConservare;

                tranzDuration = item.DurationModif;
                tranzInventoryValue = item.InvValueModif;
                tranzFiscalInventoryValue = item.FiscalValueModif;

                quantity += tranzQuantity;
                duration += tranzDuration;
                inventoryValue += tranzInventoryValue;
                fiscalInventoryValue += tranzFiscalInventoryValue;
                deprec += tranzDeprec;
                fiscalDeprec += tranzFiscalDeprec;
                monthlyDepreciation = Math.Round(inventoryValue / duration, 2);
                monthlyFiscalDepreciation = Math.Round(fiscalInventoryValue / duration, 2);

                var stock = new ImoAssetStock
                {
                    ImoAssetItemId = stockItem.ImoAssetItemId,
                    ImoAssetItemPFId = null,
                    ImoAssetOperDetId = item.Id,
                    StockDate = new DateTime(operation.OperUseStartDate.Year, operation.OperUseStartDate.Month, operation.OperUseStartDate.Day),
                    InConservare = inConservare,
                    TranzQuantity = tranzQuantity,
                    Quantity = quantity,
                    TranzDuration = tranzDuration,
                    Duration = duration,
                    TranzInventoryValue = tranzInventoryValue,
                    InventoryValue = inventoryValue,
                    TranzFiscalInventoryValue = tranzFiscalInventoryValue,
                    FiscalInventoryValue = fiscalInventoryValue,
                    TranzDeprec = tranzDeprec,
                    Deprec = deprec,
                    TranzFiscalDeprec = tranzFiscalDeprec,
                    FiscalDeprec = fiscalDeprec,
                    StorageId = stockItem.StorageId,
                    OperType = operation.AssetsOperType,
                    MonthlyDepreciation = monthlyDepreciation,
                    MonthlyFiscalDepreciation = monthlyFiscalDepreciation,
                    AssetAccountId = stockItem.ImoAssetItem.AssetAccountInUseId ?? stockItem.ImoAssetItem.AssetAccountId
                };
                Context.ImoAssetStock.Add(stock);

                var reserveList = Context.ImoAssetStockReserve.Where(f => f.ImoAssetStockId == stockItem.Id).OrderBy(f => f.Id);
                if (item.InvValueModif > 0)
                {
                    // insert din istoric
                    foreach (var reserveItem in reserveList)
                    {
                        var reserveToDB = new ImoAssetStockReserve
                        {
                            ImoAssetStock = stock,
                            DeprecReserve = reserveItem.DeprecReserve,
                            Reserve = reserveItem.Reserve,
                            ExpenseReserve = reserveItem.ExpenseReserve,
                            TranzDeprecReserve = 0,
                            TranzReserve = 0,
                            ImoAssetOperDetailId = reserveItem.ImoAssetOperDetailId
                        };
                        Context.ImoAssetStockReserve.Add(reserveToDB);
                    }
                    // inregistrarea noua
                    var reserveNew = new ImoAssetStockReserve
                    {
                        ImoAssetStock = stock,
                        TranzDeprecReserve = 0,
                        TranzReserve = item.InvValueModif,
                        DeprecReserve = 0,
                        Reserve = item.InvValueModif,
                        ExpenseReserve = 0,
                        ImoAssetOperDetailId = item.Id
                    };
                    Context.ImoAssetStockReserve.Add(reserveNew);
                }
                else
                {
                    // am rezerva negativa => diminuez din rezerva pozitiva
                    var rezNegativa = Math.Abs(item.InvValueModif);
                    var count = reserveList.Count();
                    int currCount = 1;
                    decimal expenseReserve = 0;

                    foreach (var reserveItem in reserveList)
                    {
                        decimal rezItemDim = 0;
                        if (reserveItem.Reserve > rezNegativa)
                        {
                            rezItemDim = rezNegativa;
                            rezNegativa = 0;
                        }
                        else
                        {
                            rezItemDim = reserveItem.Reserve;
                            rezNegativa = rezNegativa - rezItemDim;
                            if (currCount == count)
                            {
                                expenseReserve = rezNegativa;
                            }
                        }
                        var reserveToDB = new ImoAssetStockReserve
                        {
                            ImoAssetStock = stock,
                            DeprecReserve = reserveItem.DeprecReserve,
                            Reserve = reserveItem.Reserve - rezItemDim,
                            ExpenseReserve = expenseReserve,
                            TranzDeprecReserve = 0,
                            TranzReserve = -1 * rezItemDim,
                            ImoAssetOperDetailId = reserveItem.ImoAssetOperDetailId
                        };
                        currCount++;
                        Context.ImoAssetStockReserve.Add(reserveToDB);
                    }
                }

                // modernizari
                var modernizList = Context.ImoAssetStockModerniz.Where(f => f.ImoAssetStockId == stockItem.Id).OrderBy(f => f.Id);
                foreach (var modernizItem in modernizList)
                {
                    var modernizToDB = new ImoAssetStockModerniz
                    {
                        ImoAssetStock = stock,
                        DeprecModerniz = modernizItem.DeprecModerniz,
                        Moderniz = modernizItem.Moderniz,
                        ExpenseModerniz = modernizItem.ExpenseModerniz,
                        TranzDeprecModerniz = 0,
                        TranzModerniz = 0,
                        ImoAssetOperDetailId = modernizItem.ImoAssetOperDetailId
                    };
                    Context.ImoAssetStockModerniz.Add(modernizToDB);
                }
                Context.SaveChanges();
            }
        }

        public void ComputeModernizare(ImoAssetOper operation)
        {
            foreach (var item in operation.OperDetails)
            {
                var stockItem = Context.ImoAssetStock.Include(f => f.ImoAssetItem)
                                       .Where(f => f.ImoAssetItemId == item.ImoAssetItemId)
                                       .OrderByDescending(f => f.Id)
                                       .FirstOrDefault();

                int tranzQuantity = 0, quantity = stockItem.Quantity, tranzDuration = 0, duration = stockItem.Duration;
                decimal tranzInventoryValue = 0, inventoryValue = stockItem.InventoryValue, tranzFiscalInventoryValue = 0, fiscalInventoryValue = stockItem.FiscalInventoryValue;
                decimal tranzDeprec = 0, deprec = stockItem.Deprec, tranzFiscalDeprec = 0, fiscalDeprec = stockItem.FiscalDeprec;
                decimal monthlyDepreciation = 0, monthlyFiscalDepreciation = 0;
                bool inConservare = stockItem.InConservare;

                tranzDuration = item.DurationModif;
                tranzInventoryValue = item.InvValueModif;
                tranzFiscalInventoryValue = item.FiscalValueModif;
                
                quantity += tranzQuantity;
                duration += tranzDuration;
                inventoryValue += tranzInventoryValue;
                fiscalInventoryValue += tranzFiscalInventoryValue;
                deprec += tranzDeprec;
                fiscalDeprec += tranzFiscalDeprec;
                monthlyDepreciation = Math.Round(inventoryValue / duration, 2);
                monthlyFiscalDepreciation = Math.Round(fiscalInventoryValue / duration, 2);

                var stock = new ImoAssetStock
                {
                    ImoAssetItemId = stockItem.ImoAssetItemId,
                    ImoAssetItemPFId = null,
                    ImoAssetOperDetId = item.Id,
                    StockDate = new DateTime(operation.OperUseStartDate.Year, operation.OperUseStartDate.Month, operation.OperUseStartDate.Day),
                    InConservare = inConservare,
                    TranzQuantity = tranzQuantity,
                    Quantity = quantity,
                    TranzDuration = tranzDuration,
                    Duration = duration,
                    TranzInventoryValue = tranzInventoryValue,
                    InventoryValue = inventoryValue,
                    TranzFiscalInventoryValue = tranzFiscalInventoryValue,
                    FiscalInventoryValue = fiscalInventoryValue,
                    TranzDeprec = tranzDeprec,
                    Deprec = deprec,
                    TranzFiscalDeprec = tranzFiscalDeprec,
                    FiscalDeprec = fiscalDeprec,
                    StorageId = stockItem.StorageId,
                    OperType = operation.AssetsOperType,
                    MonthlyDepreciation = monthlyDepreciation,
                    MonthlyFiscalDepreciation = monthlyFiscalDepreciation,
                    AssetAccountId = stockItem.ImoAssetItem.AssetAccountInUseId ?? stockItem.ImoAssetItem.AssetAccountId
                };
                Context.ImoAssetStock.Add(stock);

                var modernizList = Context.ImoAssetStockModerniz.Where(f => f.ImoAssetStockId == stockItem.Id).OrderBy(f => f.Id);
                if (item.InvValueModif > 0)
                {
                    // insert din istoric
                    foreach (var modernizItem in modernizList)
                    {
                        var modernizToDB = new ImoAssetStockModerniz
                        {
                            ImoAssetStock = stock,
                            DeprecModerniz = modernizItem.DeprecModerniz,
                            Moderniz = modernizItem.Moderniz,
                            ExpenseModerniz = modernizItem.ExpenseModerniz,
                            TranzDeprecModerniz = 0,
                            TranzModerniz = 0,
                            ImoAssetOperDetailId = modernizItem.ImoAssetOperDetailId
                        };
                        Context.ImoAssetStockModerniz.Add(modernizToDB);
                    }
                    // inregistrarea noua
                    var modernizNew = new ImoAssetStockModerniz
                    {
                        ImoAssetStock = stock,
                        TranzDeprecModerniz = 0,
                        TranzModerniz = item.InvValueModif,
                        DeprecModerniz = 0,
                        Moderniz = item.InvValueModif,
                        ExpenseModerniz = 0,
                        ImoAssetOperDetailId = item.Id
                    };
                    Context.ImoAssetStockModerniz.Add(modernizNew);
                }
                Context.SaveChanges();
            }
        }

        public void ComputeOperation(ImoAssetOper operation) // intrari/iesiri in conservare
        {
            foreach (var item in operation.OperDetails)
            {
                var stockItem = Context.ImoAssetStock.Include(f=>f.ImoAssetItem)
                                       .Where(f => f.ImoAssetItemId == item.ImoAssetItemId)
                                       .OrderByDescending(f => f.Id)
                                       .FirstOrDefault();

                int tranzQuantity = 0, quantity = stockItem.Quantity, tranzDuration = 0, duration = stockItem.Duration;
                decimal tranzInventoryValue = 0, inventoryValue = stockItem.InventoryValue, tranzFiscalInventoryValue = 0, fiscalInventoryValue = stockItem.FiscalInventoryValue;
                decimal tranzDeprec = 0, deprec = stockItem.Deprec, tranzFiscalDeprec = 0, fiscalDeprec = stockItem.FiscalDeprec;
                decimal monthlyDepreciation = 0, monthlyFiscalDepreciation = 0;
                bool inConservare = (operation.AssetsOperType == ImoAssetOperType.Modernizare)
                                    ? stockItem.InConservare
                                    : (operation.AssetsOperType == ImoAssetOperType.IntrareInConservare ? true : false);

                tranzDuration = item.DurationModif;
                tranzInventoryValue = item.InvValueModif;
                tranzFiscalInventoryValue = item.FiscalValueModif;

                quantity += tranzQuantity;
                duration += tranzDuration;
                inventoryValue += tranzInventoryValue;
                fiscalInventoryValue += tranzFiscalInventoryValue;
                deprec += tranzDeprec;
                fiscalDeprec += tranzFiscalDeprec;
                monthlyDepreciation = Math.Round(inventoryValue / duration, 2);
                monthlyFiscalDepreciation = Math.Round(fiscalInventoryValue / duration, 2);

                var stock = new ImoAssetStock
                {
                    ImoAssetItemId = stockItem.ImoAssetItemId,
                    ImoAssetItemPFId = null,
                    ImoAssetOperDetId = item.Id,
                    StockDate = new DateTime(operation.OperUseStartDate.Year, operation.OperUseStartDate.Month, operation.OperUseStartDate.Day),
                    InConservare = inConservare,
                    TranzQuantity = tranzQuantity,
                    Quantity = quantity,
                    TranzDuration = tranzDuration,
                    Duration = duration,
                    TranzInventoryValue = tranzInventoryValue,
                    InventoryValue = inventoryValue,
                    TranzFiscalInventoryValue = tranzFiscalInventoryValue,
                    FiscalInventoryValue = fiscalInventoryValue,
                    TranzDeprec = tranzDeprec,
                    Deprec = deprec,
                    TranzFiscalDeprec = tranzFiscalDeprec,
                    FiscalDeprec = fiscalDeprec,
                    StorageId = stockItem.StorageId,
                    OperType = operation.AssetsOperType,
                    MonthlyDepreciation = monthlyDepreciation,
                    MonthlyFiscalDepreciation = monthlyFiscalDepreciation,
                    AssetAccountId = stockItem.ImoAssetItem.AssetAccountInUseId ?? stockItem.ImoAssetItem.AssetAccountId
                };
                Context.ImoAssetStock.Add(stock);

                // rezerve
                var reserveList = Context.ImoAssetStockReserve.Where(f => f.ImoAssetStockId == stockItem.Id).OrderBy(f => f.Id);
                foreach (var reserveItem in reserveList)
                {
                    var reserveToDB = new ImoAssetStockReserve
                    {
                        ImoAssetStock = stock,
                        DeprecReserve = reserveItem.DeprecReserve,
                        Reserve = reserveItem.Reserve,
                        ExpenseReserve = reserveItem.ExpenseReserve,
                        TranzDeprecReserve = reserveItem.TranzDeprecReserve,
                        TranzReserve = reserveItem.TranzReserve,
                        ImoAssetOperDetailId = reserveItem.ImoAssetOperDetailId
                    };
                    Context.ImoAssetStockReserve.Add(reserveToDB);
                }

                // modernizari
                var modernizList = Context.ImoAssetStockModerniz.Where(f => f.ImoAssetStockId == stockItem.Id).OrderBy(f => f.Id);
                foreach (var modernizItem in modernizList)
                {
                    var modernizToDB = new ImoAssetStockModerniz
                    {
                        ImoAssetStock = stock,
                        DeprecModerniz = modernizItem.DeprecModerniz,
                        Moderniz = modernizItem.Moderniz,
                        ExpenseModerniz = modernizItem.ExpenseModerniz,
                        TranzDeprecModerniz = 0,
                        TranzModerniz = 0,
                        ImoAssetOperDetailId = modernizItem.ImoAssetOperDetailId
                    };
                    Context.ImoAssetStockModerniz.Add(modernizToDB);
                }
                Context.SaveChanges();
            }
        }

        public void ComputeTransfer(ImoAssetOper operation) // transfer / bonuri de miscare
        {
            foreach (var item in operation.OperDetails)
            {
                var stockItem = Context.ImoAssetStock.Include(f=>f.ImoAssetItem)
                                       .Where(f => f.ImoAssetItemId == item.ImoAssetItemId)
                                       .OrderByDescending(f => f.Id)
                                       .FirstOrDefault();
                // iesire din gestiune
                var stockOut = new ImoAssetStock
                {
                    ImoAssetItemId = stockItem.ImoAssetItemId,
                    ImoAssetItemPFId = null,
                    ImoAssetOperDetId = item.Id,
                    StockDate = new DateTime(operation.OperUseStartDate.Year, operation.OperUseStartDate.Month, operation.OperUseStartDate.Day),
                    InConservare = stockItem.InConservare,
                    TranzQuantity = -1 * stockItem.Quantity,
                    Quantity = 0,
                    TranzDuration = -1 * stockItem.Duration,
                    Duration = 0,
                    TranzInventoryValue = -1 * stockItem.InventoryValue,
                    InventoryValue = 0,
                    TranzFiscalInventoryValue = -1 * stockItem.FiscalInventoryValue,
                    FiscalInventoryValue = 0,
                    TranzDeprec = -1 * stockItem.Deprec,
                    Deprec = 0,
                    TranzFiscalDeprec = -1 * stockItem.FiscalDeprec,
                    FiscalDeprec = 0,
                    StorageId = stockItem.StorageId,
                    OperType = operation.AssetsOperType,
                    MonthlyDepreciation = stockItem.MonthlyDepreciation,
                    MonthlyFiscalDepreciation = stockItem.MonthlyFiscalDepreciation,
                    AssetAccountId = stockItem.ImoAssetItem.AssetAccountInUseId ?? stockItem.ImoAssetItem.AssetAccountId
                };
                Context.ImoAssetStock.Add(stockOut);

                // rezerve
                var reserveList = Context.ImoAssetStockReserve.Where(f => f.ImoAssetStockId == stockItem.Id).OrderBy(f => f.Id);
                foreach (var reserveItem in reserveList)
                {
                    var reserveToDB = new ImoAssetStockReserve
                    {
                        ImoAssetStock = stockOut,
                        DeprecReserve = 0,
                        Reserve = 0,
                        ExpenseReserve = 0,
                        TranzDeprecReserve = -1 * reserveItem.DeprecReserve,
                        TranzReserve = -1 * reserveItem.Reserve,
                        ImoAssetOperDetailId = reserveItem.ImoAssetOperDetailId
                    };
                    Context.ImoAssetStockReserve.Add(reserveToDB);
                }
                Context.SaveChanges();

                // modernizari
                var modernizList = Context.ImoAssetStockModerniz.Where(f => f.ImoAssetStockId == stockItem.Id).OrderBy(f => f.Id);
                foreach (var modernizItem in modernizList)
                {
                    var modernizToDB = new ImoAssetStockModerniz
                    {
                        ImoAssetStock = stockOut,
                        DeprecModerniz = 0,
                        Moderniz = 0,
                        ExpenseModerniz = 0,
                        TranzDeprecModerniz = -1 * modernizItem.DeprecModerniz,
                        TranzModerniz = -1 * modernizItem.Moderniz,
                        ImoAssetOperDetailId = modernizItem.ImoAssetOperDetailId
                    };
                    Context.ImoAssetStockModerniz.Add(modernizToDB);
                }

                // intrare in gestiune
                var stockIn = new ImoAssetStock
                {
                    ImoAssetItemId = stockItem.ImoAssetItemId,
                    ImoAssetItemPFId = null,
                    ImoAssetOperDetId = item.Id,
                    StockDate = new DateTime(operation.OperUseStartDate.Year, operation.OperUseStartDate.Month, operation.OperUseStartDate.Day),
                    InConservare = stockItem.InConservare,
                    TranzQuantity = stockItem.Quantity,
                    Quantity = stockItem.Quantity,
                    TranzDuration = stockItem.Duration,
                    Duration = stockItem.Duration,
                    TranzInventoryValue = stockItem.InventoryValue,
                    InventoryValue = stockItem.InventoryValue,
                    TranzFiscalInventoryValue = stockItem.FiscalInventoryValue,
                    FiscalInventoryValue = stockItem.FiscalInventoryValue,
                    TranzDeprec = stockItem.Deprec,
                    Deprec = stockItem.Deprec,
                    TranzFiscalDeprec = stockItem.FiscalDeprec,
                    FiscalDeprec = stockItem.FiscalDeprec,
                    StorageId = operation.AssetsStoreInId ?? 0,
                    OperType = operation.AssetsOperType,
                    MonthlyDepreciation = stockItem.MonthlyDepreciation,
                    MonthlyFiscalDepreciation = stockItem.MonthlyFiscalDepreciation,
                    AssetAccountId = stockItem.ImoAssetItem.AssetAccountInUseId ?? stockItem.ImoAssetItem.AssetAccountId
                };
                Context.ImoAssetStock.Add(stockIn);

                // rezerve
                foreach (var reserveItem in reserveList)
                {
                    var reserveToDB = new ImoAssetStockReserve
                    {
                        ImoAssetStock = stockIn,
                        DeprecReserve = reserveItem.DeprecReserve,
                        Reserve = reserveItem.Reserve,
                        ExpenseReserve = 0,
                        TranzDeprecReserve = reserveItem.DeprecReserve,
                        TranzReserve = reserveItem.Reserve,
                        ImoAssetOperDetailId = reserveItem.ImoAssetOperDetailId
                    };
                    Context.ImoAssetStockReserve.Add(reserveToDB);
                }

                // modernizari
                foreach (var modernizItem in modernizList)
                {
                    var modernizToDB = new ImoAssetStockModerniz
                    {
                        ImoAssetStock = stockIn,
                        DeprecModerniz = modernizItem.DeprecModerniz,
                        Moderniz = modernizItem.Moderniz,
                        ExpenseModerniz = 0,
                        TranzDeprecModerniz = modernizItem.DeprecModerniz,
                        TranzModerniz = modernizItem.Moderniz,
                        ImoAssetOperDetailId = modernizItem.ImoAssetOperDetailId
                    };
                    Context.ImoAssetStockModerniz.Add(modernizToDB);
                }

                Context.SaveChanges();
            }
        }

        public void ComputeSale(ImoAssetOper operation) // vanzare
        {
            foreach (var item in operation.OperDetails)
            {
                var stockItem = Context.ImoAssetStock.Include(f=>f.ImoAssetItem)
                                       .Where(f => f.ImoAssetItemId == item.ImoAssetItemId)
                                       .OrderByDescending(f => f.Id)
                                       .FirstOrDefault();
                // iesire din gestiune
                var stockOut = new ImoAssetStock
                {
                    ImoAssetItemId = stockItem.ImoAssetItemId,
                    ImoAssetItemPFId = null,
                    ImoAssetOperDetId = item.Id,
                    StockDate = new DateTime(operation.OperUseStartDate.Year, operation.OperUseStartDate.Month, operation.OperUseStartDate.Day),
                    InConservare = stockItem.InConservare,
                    TranzQuantity = -1 * stockItem.Quantity,
                    Quantity = 0,
                    TranzDuration = -1 * stockItem.Duration,
                    Duration = 0,
                    TranzInventoryValue = -1 * stockItem.InventoryValue,
                    InventoryValue = 0,
                    TranzFiscalInventoryValue = -1 * stockItem.FiscalInventoryValue,
                    FiscalInventoryValue = 0,
                    TranzDeprec = -1 * stockItem.Deprec,
                    Deprec = 0,
                    TranzFiscalDeprec = -1 * stockItem.FiscalDeprec,
                    FiscalDeprec = 0,
                    StorageId = stockItem.StorageId,
                    OperType = operation.AssetsOperType,
                    MonthlyDepreciation = stockItem.MonthlyDepreciation,
                    MonthlyFiscalDepreciation = stockItem.MonthlyFiscalDepreciation,
                    AssetAccountId = stockItem.ImoAssetItem.AssetAccountInUseId ?? stockItem.ImoAssetItem.AssetAccountId
                };
                Context.ImoAssetStock.Add(stockOut);

                // rezerve
                var reserveList = Context.ImoAssetStockReserve.Where(f => f.ImoAssetStockId == stockItem.Id).OrderBy(f => f.Id);
                foreach (var reserveItem in reserveList)
                {
                    var reserveToDB = new ImoAssetStockReserve
                    {
                        ImoAssetStock = stockOut,
                        DeprecReserve = 0,
                        Reserve = 0,
                        ExpenseReserve = 0,
                        TranzDeprecReserve = -1 * reserveItem.DeprecReserve,
                        TranzReserve = -1 * reserveItem.Reserve,
                        ImoAssetOperDetailId = reserveItem.ImoAssetOperDetailId
                    };
                    Context.ImoAssetStockReserve.Add(reserveToDB);
                }

                // modernizari
                var modernizList = Context.ImoAssetStockModerniz.Where(f => f.ImoAssetStockId == stockItem.Id).OrderBy(f => f.Id);
                foreach (var modernizItem in modernizList)
                {
                    var modernizToDB = new ImoAssetStockModerniz
                    {
                        ImoAssetStock = stockOut,
                        DeprecModerniz = 0,
                        Moderniz = 0,
                        ExpenseModerniz = 0,
                        TranzDeprecModerniz = -1 * modernizItem.DeprecModerniz,
                        TranzModerniz = -1 * modernizItem.Moderniz,
                        ImoAssetOperDetailId = modernizItem.ImoAssetOperDetailId
                    };
                    Context.ImoAssetStockModerniz.Add(modernizToDB);
                }

                Context.SaveChanges();
            }
        }

        public void GestAssetDelComputing(DateTime operationDate)
        {
            var _gestList = Context.ImoAssetStock
                                .Where(f => f.StockDate >= operationDate)
                                .OrderByDescending(f => f.StockDate).ThenByDescending(f => f.Id).ToList();
            foreach (var gest in _gestList)
            {
                //TODO
                //var processedOper = Context.AutoOperationDetail.Include(f => f.AutoOper)
                //                       .Where(f => f.AutoOper.AutoOperType == AutoOperationType.MijloaceFixe)
                //                       .Where(f => f.OperationalId == gest.Id)
                //                       .Count();
                //if (processedOper != 0)
                //{
                //    throw new Exception("Pentru aceasta operatie s-au generat notele contabile!");
                //}

                var assetId = gest.ImoAssetItemId;
                if (gest.ImoAssetItemPFId != null)
                {
                    var assetPF = Context.ImoAssetItem.FirstOrDefault(f => f.Id == gest.ImoAssetItemPFId);
                    assetPF.Processed = false;
                    if (gest.OperType == ImoAssetOperType.Intrare)
                    {
                        assetPF.ProcessedIn = false;
                    }

                    if (gest.OperType == ImoAssetOperType.PunereInFunctiune)
                    {
                        assetPF.ProcessedInUse = false;
                    }
                }
                if (gest.ImoAssetOperDetId != null)
                {
                    var operDetail = Context.ImoAssetOperDetail.FirstOrDefault(f => f.Id == gest.ImoAssetOperDetId);
                    var oper = Context.ImoAssetOper.FirstOrDefault(f => f.Id == operDetail.ImoAssetOperId);
                    oper.Processed = false;
                }

                // elimin din rezerve
                var reserveStock = Context.ImoAssetStockReserve.Where(f => f.ImoAssetStockId == gest.Id);
                foreach (var reserve in reserveStock)
                {
                    Context.ImoAssetStockReserve.Remove(reserve);
                }

                // elimin din modernizari
                var modernizStock = Context.ImoAssetStockModerniz.Where(f => f.ImoAssetStockId == gest.Id);
                foreach(var moderniz in modernizStock)
                {
                    Context.ImoAssetStockModerniz.Remove(moderniz);
                }

                // verific daca are definite obiecte de inventar
                var imoInventariere = Context.ImoInventariereDet.Include(f => f.ImoInventariere).Where(f => f.ImoAssetStockId == gest.Id && f.ImoInventariere.State == State.Active).ToList();
                if(imoInventariere.Count > 0)
                {
                    throw new Exception("au fost definite obiecte de inventar. Stergeti obiectele de inventar din modulul Conta -> Mijloace Fixe -> Inventariere");
                }

                Context.ImoAssetStock.Remove(gest);
            }
            Context.SaveChanges();
        }

        public void UpdateAssetOperation(ImoAssetOper operation)
        {
            var _operation = Context.ImoAssetOper.Include(f => f.OperDetails).FirstOrDefault(f => f.Id == operation.Id);

            Context.Entry(_operation).CurrentValues.SetValues(operation);

            // Delete children
            foreach (var detail in _operation.OperDetails.ToList())
            {
                if (!operation.OperDetails.Any(c => c.Id == detail.Id))
                    Context.ImoAssetOperDetail.Remove(detail);
            }

            // Update and Insert children
            foreach (var detail in operation.OperDetails)
            {
                var existingDetail = _operation.OperDetails
                    .Where(c => c.Id == detail.Id)
                    .SingleOrDefault();

                if (existingDetail != null)
                    // Update child
                    Context.Entry(existingDetail).CurrentValues.SetValues(detail);
                else
                {
                    // Insert child
                    _operation.OperDetails.Add(detail);
                }
            }
            //Context.SaveChanges();
            UnitOfWorkManager.Current.SaveChanges();
        }

        public ImoAssetStock GetGestDetailForAsset(int assetId, DateTime gestDate)
        {
            var gest = Context.ImoAssetStock.Where(f => f.ImoAssetItemId == assetId && f.StockDate <= gestDate)
                                            .OrderByDescending(f => f.StockDate).ThenByDescending(f => f.Id)
                                            .FirstOrDefault();
            return gest;
        }

        public int NextDocumentNumber(int documentTypeId)
        {
            int ret = GetNextNumber(documentTypeId);
            return ret;
        }

        public int GetNextNumber(int documentTypeId)
        {
            int ret = 0, invObjOper = 0, imoAsset = 0, imoOper = 0;
            int count = 0;

            //// operatii obiecte de inventar
            // count = Context.InvOperation
            //                 .Count(f => f.AppClientId == selectedAppClientId && f.State == 0 && f.DocumentTypeId == documentTypeId);
            //if (count != 0)
            //{
            //    invObjOper = Context.InvOperation
            //                  .Where(f => f.AppClientId == selectedAppClientId && f.State == 0 && f.DocumentTypeId == documentTypeId)
            //                  .Max(f => f.DocumentNr);
            //}

            // incarcari mijloace fixe
            count = Context.ImoAssetItem
                             .Count(f => f.State == 0 && f.DocumentTypeId == documentTypeId);
            if (count != 0)
            {
                imoAsset = Context.ImoAssetItem
                              .Where(f => f.State == 0 && f.DocumentTypeId == documentTypeId)
                              .Max(f => f.DocumentNr);
            }

            // operratii mijloace fixe
            count = Context.ImoAssetOper
                             .Count(f => f.State == 0 && f.DocumentTypeId == documentTypeId);
            if (count != 0)
            {
                imoOper = Context.ImoAssetOper
                              .Where(f => f.State == 0 && f.DocumentTypeId == documentTypeId)
                              .Max(f => f.DocumentNr);
            }

            ret = (invObjOper > imoAsset) ? invObjOper : imoAsset;
            ret = (ret > imoOper) ? ret : imoOper;

            ret++;
            return ret;
        }

        // Procesare gestiune pornind de la mijloc fix
        public DateTime LastProcessedDateForAsset(int imoAssetId)
        {
            DateTime ret;

            var count = Context.ImoAssetStock.Where(f => f.ImoAssetItemId == imoAssetId).Count();
            if (count != 0)
            {
                ret = Context.ImoAssetStock.Include(f => f.ImoAssetItem).Where(f => f.ImoAssetItemId == imoAssetId).Max(f => f.StockDate);
            }
            else
            {
                var assetDate = Context.ImoAssetItem
                            .Where(f => f.State == State.Active && f.ProcessedIn == true && f.Id == imoAssetId)
                            .OrderBy(f => f.OperationDate)
                            .FirstOrDefault();
                var operation = Context.ImoAssetOper
                                                .Where(f => f.State == State.Active && f.Processed == true)
                                                .OrderBy(f => f.OperUseStartDate)
                                                .FirstOrDefault();
                ret = (assetDate == null) ? DateTime.Now.AddYears(-10) : assetDate.OperationDate.AddDays(-1);
                if (operation != null)
                {
                    var operDate = operation.OperUseStartDate.AddDays(-1);
                    ret = (ret < operDate) ? ret : operDate;
                }
            }

            var lastBalanceDate = Context.Balance.Where(f => f.Status == State.Active).Max(f => f.BalanceDate);
            if (ret < lastBalanceDate)
            {
                ret = lastBalanceDate;
            }
            return ret;
        }

        // Returnez lista operatiilor pentru mijlocul fix
        public List<OperationForComputing> OperListForAsset(DateTime lastProcessedDate, DateTime operationDate, int imoAssetId)
        {
            var ret = new List<OperationForComputing>();
            var assetDateList = new List<ImoAssetItem>();

            // operatii de PunereInfunctiune neprocesate in gestiune
            var assetDateInUseList = Context.ImoAssetItem
                      .Where(f => f.State == State.Active && f.UseStartDate <= operationDate && !f.ProcessedInUse && f.UseStartDate != null && f.Id == imoAssetId)
                      .ToList()
                      .Select(f => new OperationForComputing
                      {
                          Id = f.Id,
                          OperationDate = f.UseStartDate.Value,
                          OperationSource = OperationSource.ImoAsset,
                          ImoAssetOperType = ImoAssetOperType.PunereInFunctiune,
                          OperationOrd = 1,
                          OperationDateSort = new DateTime(f.UseStartDate.Value.Year, f.UseStartDate.Value.Month, f.UseStartDate.Value.Day)
                      })
                      .ToList();

            // operatii de intrare neprocesate in gestiune
            var assetDateInList = Context.ImoAssetItem
                      .Where(f => f.State == State.Active && f.OperationDate <= operationDate && !f.ProcessedIn && f.Id == imoAssetId)
                      .ToList()
                      .Select(f => new OperationForComputing
                      {
                          Id = f.Id,
                          OperationDate = f.OperationDate,
                          OperationSource = OperationSource.ImoAsset,
                          ImoAssetOperType = ImoAssetOperType.Intrare,
                          OperationOrd = 1,
                          OperationDateSort = new DateTime(f.OperationDate.Year, f.OperationDate.Month, f.OperationDate.Day)
                      })
                      .ToList();

            var operDateList = Context.ImoAssetOper
                                  .Where(f => f.State == State.Active && f.OperUseStartDate <= operationDate && !f.Processed && f.OperDetails.Select(f => f.ImoAssetItemId == imoAssetId).FirstOrDefault()/* && f.AssetsStoreInId == imoAssetId*/)
                                  .ToList()
                                  .Select(f => new OperationForComputing
                                  {
                                      Id = f.Id,
                                      OperationDate = f.OperUseStartDate,
                                      OperationSource = OperationSource.ImoOper,
                                      OperationOrd = (((f.AssetsOperType == ImoAssetOperType.Reevaluare) || (f.AssetsOperType == ImoAssetOperType.Casare) || (f.AssetsOperType == ImoAssetOperType.Iesire)) ? 4 : 2),
                                      OperationDateSort = new DateTime(f.OperUseStartDate.Year, f.OperUseStartDate.Month, f.OperUseStartDate.Day),
                                      ImoAssetOperType = f.AssetsOperType
                                  })
                                  .ToList();

            var deprecDateList = GetDeprecListForAsset(lastProcessedDate, operationDate, imoAssetId);

                foreach (var item in assetDateInList)
                {
                    ret.Add(item);
                }

                foreach (var item in assetDateInUseList)
                {
                    ret.Add(item);
                }



            foreach (var item in operDateList)
            {
                ret.Add(item);
            }

            foreach (var item in deprecDateList)
            {
                ret.Add(item);
            }

            ret = ret.Distinct().OrderBy(f => f.OperationDateSort).ThenBy(f => f.OperationOrd).ToList();

            return ret;
        }

        private List<OperationForComputing> GetDeprecListForAsset(DateTime lastProcessedDate, DateTime operationDate, int imoAssetId)
        {
            var ret = new List<OperationForComputing>();
            DateTime currDate;
            var firstAssetDay = Context.ImoAssetItem.Where(f => f.State == State.Active && f.Id == imoAssetId && f.ProcessedInUse == true).OrderBy(f => f.UseStartDate).FirstOrDefault();

            if (firstAssetDay == null)
            {
                return ret;
            }
            else
            {
                currDate = (lastProcessedDate > firstAssetDay.UseStartDate) ? lastProcessedDate : (firstAssetDay.UseStartDate ?? firstAssetDay.OperationDate);
            }

            while (currDate <= operationDate)
            {
                if (currDate.Day == DateTime.DaysInMonth(currDate.Year, currDate.Month))
                {
                    ret.Add(new OperationForComputing
                    {
                        OperationDate = currDate,
                        OperationSource = OperationSource.AmortizareLunara,
                        OperationOrd = 3,
                        OperationDateSort = new DateTime(currDate.Year, currDate.Month, currDate.Day)
                    });
                }
                currDate = currDate.AddDays(1);
            }
            return ret;
        }

        public void GestComputingForAsset(DateTime operationDate, int imoAssetId)
        {
            try
            {
                if (operationDate == null)
                {
                    throw new Exception("Data darii in folosinta trebuie completata");
                }
                var lastProcessedDate = LastProcessedDateForAsset(imoAssetId).AddDays(1);
                var operList = OperListForAsset(lastProcessedDate, operationDate, imoAssetId);

                foreach (var item in operList)
                {
                    ComputeOperationForAsset(item, imoAssetId); 
                    Context.SaveChanges();
                }
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public void ComputeOperationForAsset(OperationForComputing operation, int imoAssetId)
        {
            try
            {
                switch (operation.OperationSource)
                {
                    case OperationSource.ImoAsset:
                        ComputeAsset(operation);
                        break;
                    case OperationSource.ImoOper:
                        ComputeOper(operation);
                        break;
                    case OperationSource.AmortizareLunara:
                        GestDeprecComputingForAsset(operation, imoAssetId);
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Calculez amortizarea mijlocului fix pentru care operatia este de punere in functiune
        public void GestDeprecComputingForAsset(OperationForComputing operation, int imoAssetId)
        {
            try
            {
                var _reserveDeprec = false;
                var setupItem = Context.ImoAssetSetup.FirstOrDefault();
                if (setupItem != null)
                {
                    _reserveDeprec = setupItem.ReserveDepreciation;
                }

                var assetList = Context.ImoAssetStock.Include(f => f.ImoAssetItem)
                                                     .Where(f => f.ImoAssetItemId == imoAssetId && f.ImoAssetItem.DepreciationStartDate <= operation.OperationDate)
                                                     .GroupBy(f => f.ImoAssetItemId)
                                                     .Select(f => new { AssetId = f.Key, Id = f.Max(x => x.Id) }).ToList();

                foreach (var item in assetList)
                {
                    var stockItem = Context.ImoAssetStock.FirstOrDefault(f => f.Id == item.Id);

                    int tranzQuantity = 0, quantity = stockItem.Quantity, tranzDuration = 0, duration = stockItem.Duration;
                    decimal tranzInventoryValue = 0, inventoryValue = stockItem.InventoryValue, tranzFiscalInventoryValue = 0, fiscalInventoryValue = stockItem.FiscalInventoryValue;
                    decimal tranzDeprec = 0, deprec = stockItem.Deprec, tranzFiscalDeprec = 0, fiscalDeprec = stockItem.FiscalDeprec;
                    decimal monthlyDepreciation = stockItem.MonthlyDepreciation, monthlyFiscalDepreciation = stockItem.MonthlyFiscalDepreciation;
                    bool inConservare = stockItem.InConservare;

                    if (duration != 0)
                    {
                        if (stockItem.OperType == ImoAssetOperType.PunereInFunctiune)
                        {

                            tranzDuration = (inConservare && duration != 0) ? 0 : -1;
                            tranzDeprec =  Math.Round(inventoryValue / duration, 2);
                            tranzFiscalDeprec = Math.Round(fiscalInventoryValue / duration, 2);
                            tranzInventoryValue = -1 * tranzDeprec;
                            tranzFiscalInventoryValue = -1 * tranzFiscalDeprec;


                            quantity += tranzQuantity;
                            duration += tranzDuration;
                            inventoryValue += tranzInventoryValue;
                            fiscalInventoryValue += tranzFiscalInventoryValue;
                            deprec += tranzDeprec;
                            fiscalDeprec += tranzFiscalDeprec;
                        }
                        else
                        {
                            tranzDuration = (inConservare && duration != 0) ? 0 : -1;
                            tranzDeprec = ((inConservare && duration != 0) ? 0 : (duration == 1 ? inventoryValue : monthlyDepreciation));// Math.Round(inventoryValue / duration, 2));
                            tranzFiscalDeprec = ((inConservare && duration != 0) ? 0 : (duration == 1 ? fiscalInventoryValue : monthlyFiscalDepreciation)); //Math.Round(fiscalInventoryValue / duration, 2));
                            tranzInventoryValue = -1 * tranzDeprec;
                            tranzFiscalInventoryValue = -1 * tranzFiscalDeprec;


                            quantity += tranzQuantity;
                            duration += tranzDuration;
                            inventoryValue += tranzInventoryValue;
                            fiscalInventoryValue += tranzFiscalInventoryValue;
                            deprec += tranzDeprec;
                            fiscalDeprec += tranzFiscalDeprec;
                        }
                        var stock = new ImoAssetStock
                        {
                            ImoAssetItemId = stockItem.ImoAssetItemId,
                            ImoAssetItemPFId = null,
                            ImoAssetOperDetId = null,
                            StockDate = new DateTime(operation.OperationDate.Year, operation.OperationDate.Month, operation.OperationDate.Day),
                            InConservare = inConservare,
                            TranzQuantity = tranzQuantity,
                            Quantity = quantity,
                            TranzDuration = tranzDuration,
                            Duration = duration,
                            TranzInventoryValue = tranzInventoryValue,
                            InventoryValue = inventoryValue,
                            TranzFiscalInventoryValue = tranzFiscalInventoryValue,
                            FiscalInventoryValue = fiscalInventoryValue,
                            TranzDeprec = tranzDeprec,
                            Deprec = deprec,
                            TranzFiscalDeprec = tranzFiscalDeprec,
                            FiscalDeprec = fiscalDeprec,
                            StorageId = stockItem.StorageId,
                            OperType = ImoAssetOperType.AmortizareLunara,
                            MonthlyDepreciation = monthlyDepreciation,
                            MonthlyFiscalDepreciation = monthlyFiscalDepreciation,
                            AssetAccountId = /*stockItem.ImoAssetItem.AssetAccountInUseId ??*/ stockItem.AssetAccountId
                        };

                        Context.ImoAssetStock.Add(stock);

                        // reserve depreciation
                        var reserveStock = Context.ImoAssetStockReserve.Where(f => f.ImoAssetStockId == item.Id);
                        foreach (var reserveItem in reserveStock)
                        {
                            decimal tranzReserve = 0, tranzReserveDeprec = 0, reserve = reserveItem.Reserve, reserveDeprec = reserveItem.DeprecReserve;
                            tranzReserveDeprec = ((inConservare && duration != 0 && !_reserveDeprec) ? 0 : Math.Round(reserve / stockItem.Duration, 2));
                            tranzReserve = -1 * tranzReserveDeprec;
                            reserveDeprec += tranzReserveDeprec;
                            reserve += tranzReserve;

                            var reserveItemDb = new ImoAssetStockReserve
                            {
                                ImoAssetStock = stock,
                                TranzDeprecReserve = tranzReserveDeprec,
                                TranzReserve = tranzReserve,
                                DeprecReserve = reserveDeprec,
                                Reserve = reserve,
                                ExpenseReserve = 0,
                                ImoAssetOperDetailId = reserveItem.ImoAssetOperDetailId
                            };
                            Context.ImoAssetStockReserve.Add(reserveItemDb);
                        }


                        // modernizari depreciation
                        var modernizStock = Context.ImoAssetStockModerniz.Where(f => f.ImoAssetStockId == item.Id);
                        foreach (var modernizItem in modernizStock)
                        {
                            decimal tranzModerniz = 0, tranzModernizDeprec = 0, moderniz = modernizItem.Moderniz, modernizDeprec = modernizItem.DeprecModerniz;
                            tranzModernizDeprec = ((inConservare && duration != 0 && !_reserveDeprec) ? 0 : Math.Round(moderniz / stockItem.Duration, 2));
                            tranzModerniz = -1 * tranzModernizDeprec;
                            modernizDeprec += tranzModernizDeprec;
                            moderniz += tranzModerniz;

                            var modernizItemDb = new ImoAssetStockModerniz
                            {
                                ImoAssetStock = stock,
                                TranzDeprecModerniz = tranzModernizDeprec,
                                TranzModerniz = tranzModerniz,
                                DeprecModerniz = modernizDeprec,
                                Moderniz = moderniz,
                                ExpenseModerniz = 0,
                                ImoAssetOperDetailId = modernizItem.ImoAssetOperDetailId
                            };
                            Context.ImoAssetStockModerniz.Add(modernizItemDb);
                        }

                        Context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GestDelComputingForAsset(DateTime operationDate, int imoAssetId)
        {
            var _gestList = Context.ImoAssetStock
                               .Where(f => f.StockDate >= operationDate && f.ImoAssetItemId == imoAssetId)
                               .OrderByDescending(f => f.StockDate).ThenByDescending(f => f.Id).ToList();
            foreach (var gest in _gestList)
            {
                var assetId = gest.ImoAssetItemId;
                if (gest.ImoAssetItemPFId != null)
                {
                    var assetPF = Context.ImoAssetItem.FirstOrDefault(f => f.Id == gest.ImoAssetItemPFId);
                    assetPF.Processed = false;
                    if (gest.OperType == ImoAssetOperType.Intrare)
                    {
                        assetPF.ProcessedIn = false;
                    }

                    if (gest.OperType == ImoAssetOperType.PunereInFunctiune)
                    {
                        assetPF.ProcessedInUse = false;
                    }
                }

                if (gest.ImoAssetOperDetId != null)
                {
                    var operDetail = Context.ImoAssetOperDetail.FirstOrDefault(f => f.Id == gest.ImoAssetOperDetId);
                    var oper = Context.ImoAssetOper.FirstOrDefault(f => f.Id == operDetail.ImoAssetOperId);
                    oper.Processed = false;
                }

                // elimin din rezerve
                var reserveStock = Context.ImoAssetStockReserve.Where(f => f.ImoAssetStockId == gest.Id);
                foreach (var reserve in reserveStock)
                {
                    Context.ImoAssetStockReserve.Remove(reserve);
                }

                // elimin din modernizari
                var modernizStock = Context.ImoAssetStockModerniz.Where(f => f.ImoAssetStockId == gest.Id);
                foreach (var moderniz in modernizStock)
                {
                    Context.ImoAssetStockModerniz.Remove(moderniz);
                }

                //verific daca au fost definite obiecte de inventar
                var imoInventariere = Context.ImoInventariereDet.Include(f => f.ImoInventariere).Where(f => f.ImoAssetStockId == gest.Id && f.ImoInventariere.State == State.Active).ToList();
                if (imoInventariere.Count > 0)
                {
                    throw new Exception("Operatia nu poate fi inregistrata, deoarece au fost definite obiecte de inventar. Stergeti obiectele de inventar din modulul Conta -> Mijloace Fixe -> Inventariere");
                }

                Context.ImoAssetStock.Remove(gest);
            }
            Context.SaveChanges();
        }

        public List<ImoAssetOperDetail> GetImoAssetOperDetails(int imoAssetId, int appClientId)
        {
            var imoAssetOperDetail = Context.ImoAssetOperDetail.Include(f => f.ImoAssetItem).Where(f => f.State == State.Active && f.TenantId == appClientId && f.ImoAssetItemId == imoAssetId && 
                                                                                                   f.ImoAssetOper.State == State.Active/* && f.ImoAssetOper.Processed == true*/).ToList();
            return imoAssetOperDetail;
        }
    }


    public class OperationForComputing
    {
        public int? Id { get; set; }

        public DateTime OperationDate { get; set; }

        public OperationSource OperationSource { get; set; }

        public int OperationOrd { get; set; }

        public int AppClientId { get; set; }

        public DateTime OperationDateSort { get; set; }
        public ImoAssetOperType ImoAssetOperType { get; set; }

    }

    public enum OperationSource
    {
        ImoAsset,
        ImoOper,
        AmortizareLunara
    }
}
