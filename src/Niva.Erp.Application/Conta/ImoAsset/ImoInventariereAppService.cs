using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.Conta.ImoAsset.Dto;
using Niva.Erp.Conta.InvObjects.Dto;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.ImoAsset;
using Niva.Erp.Models.ImoAssets;
using Niva.Erp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Conta.ImoAsset
{
    public interface IImoInventariereAppService : IApplicationService
    {
        List<InventoryDDList> ImoOperDateList();
        ImoInventariereInitDto InitForm();
        List<ImoInventariereListDto> SearchImoInventariere(DateTime dataStart, DateTime dateEnd);
        List<ImoInventariereDetailDto> GetImoInvDetail(int imoInvId, DateTime operationDate);
        List<ImoInventariereDetailDto> SearchComputeImoInv(DateTime dateStart);
        void SaveImoInv(ImoInventariereEditDto imoInv);
        ImoInventariereEditDto GetImoInventariere(int? imoInvId);
        void DeleteImoInventariere(int imoInvId);
    }

    public class ImoInventariereAppService : ErpAppServiceBase, IImoInventariereAppService
    {
        IRepository<ImoInventariere> _imoInventariereRepository;
        IRepository<ImoAssetStock> _imoAssetStockRepository;
        IRepository<ImoInventariereDet> _imoInventariereDetailsRepository;
        IOperationRepository _operationRepository;

        public ImoInventariereAppService(IRepository<ImoInventariere> imoInventariereRepository, IRepository<ImoAssetStock> imoAssetStockRepository, IRepository<ImoInventariereDet> imoInventariereDetailsRepository,
            IOperationRepository operationRepository)
        {

            _imoInventariereRepository = imoInventariereRepository;
            _imoAssetStockRepository = imoAssetStockRepository;
            _imoInventariereDetailsRepository = imoInventariereDetailsRepository;
            _operationRepository = operationRepository;
        }
        //[AbpAuthorize("Conta.MF.Inventar.Acces")]
        public ImoInventariereInitDto InitForm()
        {
            var ret = new ImoInventariereInitDto
            {
                DateStart = LazyMethods.Now().AddMonths(-1),
                DateEnd = LazyMethods.Now(),
                ImoInventariereList = new List<ImoInventariereListDto>()
            };

            ret.ImoInventariereList = SearchImoInventariere(ret.DateStart, ret.DateEnd);

            return ret;
        }
        //[AbpAuthorize("Conta.MF.Inventar.Acces")]
        public List<ImoInventariereListDto> SearchImoInventariere(DateTime dateStart, DateTime dateEnd)
        {
            var imoInventarieres = _imoInventariereRepository.GetAll().Where(f => f.DataInventariere >= dateStart && f.DataInventariere <= dateEnd && f.State == State.Active)
                .Select(f => new ImoInventariereListDto
                {
                    Id = f.Id,
                    DateStart = f.DataInventariere,
                    State = f.State
                }).OrderByDescending(f => f.DateStart).ToList();

            return imoInventarieres;
        }

        //[AbpAuthorize("Conta.MF.Inventar.Acces")]
        public List<ImoInventariereDetailDto> GetImoInvDetail(int imoInvId, DateTime operationDate)
        {
            try
            {
                var appClient = GetCurrentTenant();
                var imoDetails = _imoAssetStockRepository.GetAllIncluding(f => f.ImoAssetItem).Where(f => f.ImoAssetItemId == imoInvId && f.StockDate <= operationDate && f.Quantity != 0 && f.TenantId == appClient.Id)
                                                         .Select(g => new ImoInventariereDetailDto
                                                         {
                                                             Description = g.ImoAssetItem.Name,
                                                             UseStartDate = g.ImoAssetItem.UseStartDate,
                                                             InventoryNumber = g.ImoAssetItem.InventoryNr,
                                                             InventoryValue = g.InventoryValue + g.Deprec,
                                                             RemainingValue = g.InventoryValue,
                                                             StockScriptic = g.InventoryValue,
                                                             ImoAssetItemId = g.ImoAssetItemId,
                                                             TenantId = g.TenantId

                                                         }).OrderBy(f => f.InventoryNumber).ToList();

                return imoDetails;
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }
        //[AbpAuthorize("Conta.MF.Inventar.Acces")]
        public ImoInventariereEditDto GetImoInventariere(int? imoInvId)
        {
            var ret = new ImoInventariereEditDto();
            if (imoInvId == null)
            {
                ret.DateStart = LazyMethods.Now();
                ret.ImoInventariereDetails = new List<ImoInventariereDetailDto>();
                return ret;
            }

            var imoInvList = _imoInventariereRepository.GetAllIncluding(f => f.ImoInventariereDetails).Where(f => f.Id == imoInvId && f.State == State.Active).FirstOrDefault();

            var imoInvDetails = _imoInventariereDetailsRepository.GetAllIncluding(f => f.ImoAssetItem, f => f.ImoAssetStock, f => f.ImoInventariere, f => f.ImoAssetStock.Storage)
                                                                 .Where(f => f.ImoInventariereId == imoInvId && f.ImoInventariere.State == State.Active)
                                                                 .OrderBy(f => f.ImoAssetItem.InventoryNr)
                                                                 .ToList();
            imoInvList.ImoInventariereDetails = imoInvDetails;
            ret = ObjectMapper.Map<ImoInventariereEditDto>(imoInvList);
            return ret;

        }
        //[AbpAuthorize("Conta.MF.Inventar.Acces")]
        public List<ImoInventariereDetailDto> SearchComputeImoInv(DateTime dateStart)
        {
            //if (!_operationRepository.VerifyClosedMonth(dateStart))
            //    throw new UserFriendlyException("Eroare", "Nu se poate inregistra operatia deoarece luna contabila este inchisa");

            var assetList = _imoAssetStockRepository.GetAllIncluding(f => f.ImoAssetItem).Where(f => f.StockDate <= dateStart).GroupBy(f => f.ImoAssetItemId).Select(f => f.Max(x => x.Id)).ToList();
            var stockList = _imoAssetStockRepository.GetAllIncluding(f => f.ImoAssetItem, f => f.ImoAssetItem.ImoAssetStorage).Where(f => assetList.Contains(f.Id) && f.Quantity != 0)
               .Select(f => new ImoInventariereDetailDto
               {
                   Description = f.ImoAssetItem.Name,
                   UseStartDate = f.ImoAssetItem.UseStartDate,
                   StockScriptic = f.Quantity,
                   StockFaptic = f.Quantity,
                   StorageIn = f.Storage.StorageName,
                   StorageInId = f.StorageId,
                   ImoAssetItemId = f.ImoAssetItemId,
                   InventoryNumber = f.ImoAssetItem.InventoryNr,
                   InventoryValue = f.InventoryValue + f.Deprec,
                   RemainingValue = f.InventoryValue,
                   ImoAssetStockId = f.Id,
                   TenantId = f.TenantId
               })
               .ToList();


            return stockList;
        }
        [AbpAuthorize("Conta.MF.Inventar.Modificare")]
        public void SaveImoInv(ImoInventariereEditDto imoInv)
        {
            //if (!_operationRepository.VerifyClosedMonth(imoInv.DateStart))
            //    throw new UserFriendlyException("Eroare", "Nu se poate inregistra operatia deoarece luna contabila este inchisa");


            try
            {
                var newImoInv = ObjectMapper.Map<ImoInventariere>(imoInv);

                var appClient = GetCurrentTenant();
                newImoInv.TenantId = appClient.Id;

                var _imoInvDetails = new List<ImoInventariereDet>();
                foreach (var item in imoInv.ImoInventariereDetails)
                {
                    var _imoInvDetail = ObjectMapper.Map<ImoInventariereDet>(item);
                    _imoInvDetails.Add(_imoInvDetail);

                }

                if (imoInv.Id == 0)
                {
                    newImoInv.ImoInventariereDetails = _imoInvDetails;
                    _imoInventariereRepository.Insert(newImoInv);
                    CurrentUnitOfWork.SaveChanges();
                }
                else
                {
                    _imoInventariereRepository.Update(newImoInv);
                    CurrentUnitOfWork.SaveChanges();
                    // sterg detaliile anterioare
                    var _oldDetails = _imoInventariereDetailsRepository.GetAll().Where(f => f.ImoInventariereId == newImoInv.Id);
                    foreach (var item in _oldDetails)
                    {
                        _imoInventariereDetailsRepository.Delete(item);
                    }
                    CurrentUnitOfWork.SaveChanges();
                    // adaug detaliile noi
                    foreach (var item in _imoInvDetails)
                    {
                        item.ImoInventariereId = newImoInv.Id;
                        item.Id = 0;
                        _imoInventariereDetailsRepository.Insert(item);
                    }
                    CurrentUnitOfWork.SaveChanges();
                }

            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }
        [AbpAuthorize("Conta.MF.Inventar.Modificare")]
        public void DeleteImoInventariere(int imoInvId)
        {
            var imoInv = _imoInventariereRepository.FirstOrDefault(f => f.Id == imoInvId);

            //if (!_operationRepository.VerifyClosedMonth(imoInv.DataInventariere))
            //    throw new UserFriendlyException("Eroare", "Nu se poate inregistra operatia deoarece luna contabila este inchisa");


            try
            {
                //imoInv.State = State.Inactive;
                _imoInventariereRepository.Delete(imoInv);
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }
        //[AbpAuthorize("Conta.MF.Inventar.Acces")]
        public List<InventoryDDList> ImoOperDateList()
        {
            try
            {
                var imoAssetList = _imoInventariereRepository.GetAll().Where(f => f.State == State.Active)
                                                            .OrderByDescending(f => f.DataInventariere)
                                                            .ToList()
                                                            .Select(f => new InventoryDDList { Id = f.Id, InvObjectDate = f.DataInventariere.ToShortDateString() })
                                                            .ToList();
                return imoAssetList;

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }
    }
}
