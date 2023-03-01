using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Niva.Erp.Conta.InvObjects.Dto;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.InvObjects;
using Niva.Erp.Repositories;
using Niva.Erp.Repositories.Conta;
using Niva.Erp.Repositories.Conta.AutoOperation;
using Niva.Erp.Repositories.InvObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Conta.InvObjects
{
    public interface IInvObjectGestAppService : IApplicationService
    {
        InvObjectGestListDto InitForm();
        InvObjectGestListDto SearchGest(InvObjectGestListDto gest);
        List<InvObjectListDto> InvObjectListDD(DateTime? dataStart, DateTime? dataEnd);
        List<InvObjectStorageDto> StorageListDD(DateTime? dataStart, DateTime? dataEnd);
        InvObjectGestComputeListDto InitFormCompute();
        InvObjectGestComputeListDto SearchCompute(InvObjectGestComputeListDto gestCompute);
        InvObjectGestComputeListDto ComputeGest(InvObjectGestComputeListDto gestCompute);
        InvObjectDelListDto InitFormDel();
        InvObjectDelListDto SearchGestDel(InvObjectDelListDto gestDel);
        void DeleteGest(DateTime deleteDate);
    }

    public class InvObjectGestAppService : ErpAppServiceBase, IInvObjectGestAppService
    {
        IBalanceRepository _balanceRepository;
        IInvOperationRepository _invOperationRepository;
        IOperationRepository _operationRepository;
        IInvObjectRepository _invObjectRepository;
        IAutoOperationRepository _autoOperationRepository;
        IRepository<InvObjectStock> _invObjectGestRepository;


        public InvObjectGestAppService(IBalanceRepository balanceRepository, IRepository<InvObjectStock> invObjectGestRepository, IInvOperationRepository invOperationRepository, IOperationRepository operationRepository,
            IInvObjectRepository invObjectRepository, IAutoOperationRepository autoOperationRepository
            )
        {
            _balanceRepository = balanceRepository;
            _invObjectGestRepository = invObjectGestRepository;
            _invOperationRepository = invOperationRepository;
            _operationRepository = operationRepository;
            _invObjectRepository = invObjectRepository;
            _autoOperationRepository = autoOperationRepository;
        }
        //[AbpAuthorize("Administrare.ObInventar.CalculGestiune.Acces")]
        public InvObjectGestListDto InitForm()
        {
            var ret = new InvObjectGestListDto
            {
                DataStart = _balanceRepository.BalanceDateNextDay(),
                DataEnd = LazyMethods.Now(),
                GestDetail = new List<InvObjectGestDetailListDto>()
            };
            ret = SearchGest(ret);
            return ret;
        }

        public InvObjectGestListDto SearchGest(InvObjectGestListDto gest)
        {
            try
            {
                var _dataStart = (gest.DataStart ?? LazyMethods.Now().AddMonths(-1));
                var _dataEnd = (gest.DataEnd ?? LazyMethods.Now());

                var gestDetailsDB = _invObjectGestRepository.GetAllIncluding(f => f.InvObjectItem, f => f.Storage)
                                                      .Include(f => f.InvObjectStockReserve)
                                                      .ThenInclude(f => f.InvObjectOperDetail)
                                                      .ThenInclude(f => f.InvObjectOper)
                                                      .Where(f => f.StockDate >= _dataStart && f.StockDate <= _dataEnd
                                               && f.InvObjectItemId == (gest.InvObjectId ?? f.InvObjectItemId)
                                               && f.StorageId == (gest.StorageId ?? f.StorageId))
                                         .OrderBy(f => f.StockDate).ThenBy(f => f.InvObjectItem.InventoryNr)
                                         .ToList();
                var gestDetails = new List<InvObjectGestDetailListDto>();

                foreach (var item in gestDetailsDB)
                {
                    var gestDetail = new InvObjectGestDetailListDto
                    {
                        Id = item.Id,
                        StockDate = item.StockDate,
                        StockDateStr = item.StockDate.ToShortDateString(),
                        InvObjectItem = item.InvObjectItem.InventoryNr + ". " + item.InvObjectItem.Name,
                        OperType = LazyMethods.EnumValueToDescription(item.OperType),
                        Storage = item.Storage.StorageName,
                        TranzQuantity = item.TranzQuantity,
                        Quantity = item.Quantity,
                        TranzInventoryValue = item.TranzInventoryValue,
                        InventoryValue = item.InventoryValue,
                        ShowReserve = false
                    };

                    var reserveDetails = new List<InvObjectGestReserveDto>();
                    foreach (var itemReserve in item.InvObjectStockReserve)
                    {
                        var reserveDetail = new InvObjectGestReserveDto
                        {
                            InvObjectStockId = itemReserve.InvObjectStockId ?? 0,
                            TranzDeprecReserve = itemReserve.TranzDeprecReserve,
                            TranzReserve = itemReserve.TranzReserve,
                            DeprecReserve = itemReserve.DeprecReserve,
                            Reserve = itemReserve.Reserve,
                            ExpenseReserve = itemReserve.ExpenseReserve,
                            OperationDate = itemReserve.InvObjectOperDetail.InvObjectOper.OperationDate.ToShortDateString()
                        };
                        reserveDetails.Add(reserveDetail);
                    }
                    gestDetail.ReserveDetail = reserveDetails;
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

        public List<InvObjectListDto> InvObjectListDD(DateTime? dataStart, DateTime? dataEnd)
        {
            var _dataStart = (dataStart ?? LazyMethods.Now().AddMonths(-1));
            var _dataEnd = (dataEnd ?? LazyMethods.Now());

            var ret = _invObjectGestRepository.GetAllIncluding(f => f.InvObjectItem)
                                    .Where(f => f.StockDate >= _dataStart && f.StockDate <= _dataEnd)
                                    .OrderByDescending(f => f.StockDate)
                                    .Select(f => new InvObjectListDto { Id = f.InvObjectItemId, Name = f.InvObjectItem.InventoryNr + ". " + f.InvObjectItem.Name })
                                    .Distinct()
                                    .OrderBy(f => f.Name)
                                    .ToList();

            return ret;
        }

        public List<InvObjectStorageDto> StorageListDD(DateTime? dataStart, DateTime? dataEnd)
        {
            var _dataStart = (dataStart ?? LazyMethods.Now().AddMonths(-1));
            var _dataEnd = (dataEnd ?? LazyMethods.Now());

            var ret = _invObjectGestRepository.GetAllIncluding(f => f.Storage)
                                    .Where(f => f.StockDate >= _dataStart && f.StockDate <= _dataEnd)
                                    .OrderByDescending(f => f.StockDate)
                                    .Select(f => new InvObjectStorageDto { Id = f.StorageId, StorageName = f.Storage.StorageName })
                                    .Distinct()
                                    .OrderBy(f => f.StorageName)
                                    .ToList();

            return ret;
        }
        //[AbpAuthorize("Administrare.ObInventar.CalculGestiune.Acces")]
        public InvObjectGestComputeListDto InitFormCompute()
        {
            var ret = new InvObjectGestComputeListDto
            {
                UnprocessedDate = _invOperationRepository.UnprocessedDate().AddDays(1),
                ComputeDate = LazyMethods.Now(),
                ShowCompute = false,
                OperationList = new List<InvObjectOperationListDto>()
            };
            ret = SearchCompute(ret);

            return ret;
        }
        //[AbpAuthorize("Administrare.ObInventar.CalculGestiune.Acces")]
        public InvObjectGestComputeListDto SearchCompute(InvObjectGestComputeListDto gestCompute)
        {
            var _computeDate = (gestCompute.ComputeDate ?? LazyMethods.Now());
            var _unprocessedDate = gestCompute.UnprocessedDate;

            if (!_operationRepository.VerifyClosedMonth(_computeDate))
                throw new UserFriendlyException("Eroare", "Nu se poate inregistra operatia deoarece luna contabila este inchisa");

            var gestDetail = new List<InvObjectOperationListDto>();

            // operatii de intrare neprocesate din invObjectItem
            var invObjectItemInDetail = _invObjectRepository.GetAllIncluding(f => f.InvStorage, f => f.PrimDocumentType)
                                     .Where(f => f.OperationDate <= _computeDate && f.Processed == false && f.State == State.Active)
                                     .OrderBy(f => f.OperationDate)
                                     .ToList()
                                     .Select(f => new InvObjectOperationListDto
                                     {
                                         DocumentNr = f.DocumentNr,
                                         DocumentDate = f.DocumentDate,
                                         OperationType = (InvObjectOperType.Intrare).ToString(),
                                         StorageIn = f.InvStorage.StorageName,
                                         StorageOut = null,
                                         DocumentType = (f.PrimDocumentType != null) ? f.PrimDocumentType.TypeName : "",
                                         OperationDate = f.OperationDate,
                                         Id = f.Id,
                                         OrdProcess = 1,
                                         OperationDateSort = new DateTime(f.OperationDate.Year, f.OperationDate.Month, f.OperationDate.Day)
                                     })
                                     .Distinct()
                                     .ToList();

            // operatii neprocesate din invObjectOper
            var operDetail = _invOperationRepository.GetAllIncluding(f => f.InvObjectsStoreIn, f => f.InvObjectsStoreOut, f => f.DocumentType)
                                     .Where(f => f.OperationDate <= _computeDate && f.Processed == false && f.State == State.Active)
                                     .OrderBy(f => f.OperationDate)
                                     .ToList()
                                     .Select(f => new InvObjectOperationListDto
                                     {
                                         DocumentNr = f.DocumentNr,
                                         DocumentDate = f.DocumentDate,
                                         OperationType = LazyMethods.EnumValueToDescription(f.InvObjectsOperType),
                                         StorageIn = (f.InvObjectsStoreIn == null) ? null : f.InvObjectsStoreIn.StorageName,
                                         StorageOut = (f.InvObjectsStoreOut == null) ? null : f.InvObjectsStoreOut.StorageName,
                                         DocumentType = f.DocumentType.TypeName,
                                         OperationDate = f.OperationDate,
                                         Id = f.Id,
                                         OrdProcess = ((f.InvObjectsOperType == InvObjectOperType.Reevaluare) || (f.InvObjectsOperType == InvObjectOperType.Casare) || (f.InvObjectsOperType == InvObjectOperType.Iesire)) ? 3 : 1,
                                         OperationDateSort = new DateTime(f.OperationDate.Year, f.OperationDate.Month, f.OperationDate.Day)
                                     })
                                     .Distinct()
                                     .ToList();


            foreach (var item in invObjectItemInDetail)
            {
                gestDetail.Add(item);
            }

            foreach (var item in operDetail)
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
        //[AbpAuthorize("Administrare.ObInventar.CalculGestiune.Acces")]
        public InvObjectGestComputeListDto ComputeGest(InvObjectGestComputeListDto gestCompute)
        {
            if (!_operationRepository.VerifyClosedMonth(gestCompute.ComputeDate ?? LazyMethods.Now()))
                throw new UserFriendlyException("Eroare", "Nu se poate inregistra operatia deoarece luna contabila este inchisa");

            try
            {
                _invOperationRepository.GestInvObjectsComputing(gestCompute.ComputeDate ?? LazyMethods.Now());
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.Message);
            }

            // generez note contabile din gestiune
            try
            {
                var lastBalanceDate = _balanceRepository.GetAll().Where(f => f.Status == State.Active).OrderByDescending(f => f.BalanceDate).FirstOrDefault().BalanceDate;
                var appClient = GetCurrentTenant();
                _autoOperationRepository.AutoInvObjectOperationAdd(gestCompute.ComputeDate ?? LazyMethods.Now(), appClient.Id, appClient.LocalCurrencyId.Value, lastBalanceDate, null);
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.Message);
            }
            gestCompute = SearchCompute(gestCompute);
            gestCompute.ShowCompute = false;
            return gestCompute;
        }

        public InvObjectDelListDto InitFormDel()
        {
            var ret = new InvObjectDelListDto
            {
                DataStart = LazyMethods.Now().AddMonths(-1),//DateTime.Now.AddMonths(-1),
                DataEnd = LazyMethods.Now(),
                GestDelDetail = new List<InvObjectGestDelDetailDto>()
            };

            ret = SearchGestDel(ret);

            return ret;
        }

        public InvObjectDelListDto SearchGestDel(InvObjectDelListDto gestDel)
        {
            var _dataStart = (gestDel.DataStart ?? LazyMethods.Now().AddMonths(-1));
            var _dataEnd = (gestDel.DataEnd ?? LazyMethods.Now());

            var gestDetail = _invObjectGestRepository.GetAll()
                                     .Where(f => f.StockDate >= _dataStart && f.StockDate <= _dataEnd)
                                     .Select(f => new InvObjectGestDelDetailDto
                                     {
                                         DateGest = f.StockDate
                                     })
                                     .Distinct()
                                     .OrderByDescending(f => f.DateGest)
                                     .ToList();
            gestDel.GestDelDetail = gestDetail;
            return gestDel;
        }
        //[AbpAuthorize("Administrare.ObInventar.CalculGestiune.Acces")]
        public void DeleteGest(DateTime deleteDate)
        {
            if (!_operationRepository.VerifyClosedMonth(deleteDate))
                throw new UserFriendlyException("Eroare", "Nu se poate sterge operatia deoarece luna contabila este inchisa");

            // sterg notele contabile
            try
            {
                _autoOperationRepository.DeleteInvObjectOperations(deleteDate);
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.Message);
            }

            try
            {
                _invOperationRepository.GestInvObjectDelComputing(deleteDate);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }
    }
}
