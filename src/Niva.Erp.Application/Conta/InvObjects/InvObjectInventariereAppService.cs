using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.Conta.InvObjects.Dto;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.InvObjects;
using Niva.Erp.Repositories;

namespace Niva.Erp.Conta.InvObjects
{
    public interface IInvObjectInventariereAppService : IApplicationService
    {
        List<InventoryDDList> InvDateList();
        InvObjectInventariereInitDto InitForm();
        InvObjectInventariereInitDto SearchInvObjects(InvObjectInventariereInitDto invObject);
        InvObjectInventariereEditDto GetInvObjectInvetariere(int? invObjectId);
        List<InvObjectInventariereDetDto> SearchComputeInvObject(DateTime dateStart);
        void SaveInvObjectInventariere(InvObjectInventariereEditDto invObject);
        void DeleteInvObjectInventariere(int invObjectId);
    }

    public class InvObjectInventariereAppService : ErpAppServiceBase, IInvObjectInventariereAppService
    {
        IRepository<InvObjectInventariere> _invObjInventariereRepository;
        IRepository<InvObjectStock> _invObjectStockRepository;
        IRepository<InvObjectInventariereDet> _invObjInventariereDetailsRepository;
        IOperationRepository _operationRepository;

        public InvObjectInventariereAppService(IRepository<InvObjectInventariere> invObjInventariereRepository, IRepository<InvObjectStock> invObjectStockRepository,
             IRepository<InvObjectInventariereDet> invObjInventariereDetailsRepository, IOperationRepository operationRepository)
        {
            _invObjInventariereRepository = invObjInventariereRepository;
            _invObjectStockRepository = invObjectStockRepository;
            _invObjInventariereDetailsRepository = invObjInventariereDetailsRepository;
            _operationRepository = operationRepository;
        }

        public List<InventoryDDList> InvDateList()
        {
            try
            {
                var invObjectDateList = _invObjInventariereRepository.GetAll().Where(f => f.State == State.Active)
                                                            .OrderByDescending(f => f.DataInventariere)
                                                            .ToList()
                                                            .Select(f => new InventoryDDList { Id = f.Id, InvObjectDate = f.DataInventariere.ToShortDateString() })
                                                            .ToList();
                return invObjectDateList;

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        [AbpAuthorize("Conta.ObInventar.Inventar.Modificare")]
        public void DeleteInvObjectInventariere(int invObjectId)
        {
            var invObject = _invObjInventariereRepository.FirstOrDefault(f => f.Id == invObjectId);

            //if (!_operationRepository.VerifyClosedMonth(invObject.DataInventariere))
            //    throw new UserFriendlyException("Eroare", "Nu se poate inregistra operatia deoarece luna contabila este inchisa");

            try
            {
                //invObject.State = State.Inactive;
                _invObjInventariereRepository.Delete(invObject);
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        //[AbpAuthorize("Conta.ObInventar.Inventar.Acces")]
        public InvObjectInventariereEditDto GetInvObjectInvetariere(int? invObjectId)
        {
            InvObjectInventariereEditDto ret;
            if (invObjectId == null)
            {
                ret = new InvObjectInventariereEditDto
                {
                    DataInventariere = LazyMethods.Now(),
                    InvObjectInventariereDetails = new List<InvObjectInventariereDetDto>()
                };

            }
            else
            {
                var invObject = _invObjInventariereRepository.GetAllIncluding(f => f.InvObjectInventariereDetails).FirstOrDefault(f => f.Id == invObjectId);

                var invObjectDetails = _invObjInventariereDetailsRepository.GetAllIncluding(f => f.InvObjectItem, f => f.InvObjectStock, f => f.InvObjectInventariere, f => f.InvObjectStock.Storage)
                                                      .Where(f => f.InvObjectInventariereId == invObjectId && f.InvObjectInventariere.State == State.Active)
                                                      .OrderBy(f => f.InvObjectItem.InventoryNr)
                                                      .ToList();

                ret = ObjectMapper.Map<InvObjectInventariereEditDto>(invObject);
                var invObjDet = ObjectMapper.Map<List<InvObjectInventariereDetDto>>(invObjectDetails);

                ret.InvObjectInventariereDetails = invObjDet;


            }
            return ret;
        }

        //[AbpAuthorize("Conta.ObInventar.Inventar.Acces")]
        public InvObjectInventariereInitDto InitForm()
        {
            var ret = new InvObjectInventariereInitDto
            {
                DateStart = LazyMethods.Now().AddMonths(-1),
                DateEnd = LazyMethods.Now(),
                InvObjectInventariereList = new List<InvObjectInventariereListDto>()
            };

            ret = SearchInvObjects(ret);

            return ret;
        }

        [AbpAuthorize("Conta.ObInventar.Inventar.Modificare")]
        public void SaveInvObjectInventariere(InvObjectInventariereEditDto invObject)
        {
            //if (!_operationRepository.VerifyClosedMonth(invObject.DataInventariere))
            //    throw new UserFriendlyException("Eroare", "Nu se poate inregistra operatia deoarece luna contabila este inchisa");

            try
            {
                var newInvObjectInventariere = ObjectMapper.Map<InvObjectInventariere>(invObject);

                var appClient = GetCurrentTenant();
                newInvObjectInventariere.TenantId = appClient.Id;

                var _invObjectInventariereDetails = new List<InvObjectInventariereDet>();
                foreach (var item in invObject.InvObjectInventariereDetails)
                {
                    var _invObjectDetail = ObjectMapper.Map<InvObjectInventariereDet>(item);
                    _invObjectInventariereDetails.Add(_invObjectDetail);

                }

                if (invObject.Id == 0)
                {
                    newInvObjectInventariere.InvObjectInventariereDetails = _invObjectInventariereDetails;
                    _invObjInventariereRepository.Insert(newInvObjectInventariere);
                    CurrentUnitOfWork.SaveChanges();
                }
                else
                {
                    _invObjInventariereRepository.Update(newInvObjectInventariere);
                    CurrentUnitOfWork.SaveChanges();
                    // sterg detaliile anterioare
                    var _oldDetails = _invObjInventariereDetailsRepository.GetAll().Where(f => f.InvObjectInventariereId == newInvObjectInventariere.Id);
                    foreach (var item in _oldDetails)
                    {
                        _invObjInventariereDetailsRepository.Delete(item);
                    }
                    CurrentUnitOfWork.SaveChanges();
                    // adaug detaliile noi
                    foreach (var item in _invObjectInventariereDetails)
                    {
                        item.InvObjectInventariereId = newInvObjectInventariere.Id;
                        item.Id = 0;
                        _invObjInventariereDetailsRepository.Insert(item);
                    }
                    CurrentUnitOfWork.SaveChanges();
                }

            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public List<InvObjectInventariereDetDto> SearchComputeInvObject(DateTime dateStart)
        {
            try
            {
                //if (!_operationRepository.VerifyClosedMonth(dateStart))
                //    throw new Exception("Nu se poate inregistra operatia deoarece luna contabila este inchisa");

                var invObjectList = _invObjectStockRepository.GetAllIncluding(f => f.InvObjectItem).Where(f => f.StockDate <= dateStart && f.InvObjectItem.State == State.Active).GroupBy(f => f.InvObjectItemId).Select(f => f.Max(x => x.Id)).ToList();
                var stockList = _invObjectStockRepository.GetAllIncluding(f => f.InvObjectItem, f => f.InvObjectItem.InvStorage).Where(f => invObjectList.Contains(f.Id) && f.Quantity != 0)
                   .Select(f => new InvObjectInventariereDetDto
                   {
                       Description = f.InvObjectItem.Name,
                       OperationDate = f.InvObjectItem.OperationDate,
                       StockScriptic = f.Quantity,
                       StockFaptic = f.Quantity,
                       StorageIn = f.Storage.StorageName,
                       StorageInId = f.StorageId,
                       InvObjectItemId = f.InvObjectItemId,
                       InventoryNumber = f.InvObjectItem.InventoryNr,
                       InventoryValue = f.InventoryValue + f.Deprec,
                       InvObjectStockId = f.Id,
                       TenantId = f.TenantId
                   })
                   .ToList();


                return stockList;
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.Message);
            }

        }

        public InvObjectInventariereInitDto SearchInvObjects(InvObjectInventariereInitDto invObject)
        {
            try
            {
                var invObjectListDb = _invObjInventariereRepository.GetAll().Where(f => f.DataInventariere >= invObject.DateStart &&
                                                                                   f.DataInventariere <= invObject.DateEnd && f.State == State.Active)
                                                                             .ToList();

                var invObjectList = ObjectMapper.Map<List<InvObjectInventariereListDto>>(invObjectListDb);

                invObject.InvObjectInventariereList = invObjectList;
                return invObject;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }
    }
}
