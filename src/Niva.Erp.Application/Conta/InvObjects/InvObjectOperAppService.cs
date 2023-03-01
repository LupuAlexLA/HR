using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Niva.Erp.Conta.ImoAsset;
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
using System.Linq.Dynamic.Core;

namespace Niva.Erp.Conta.InvObjects
{
    public interface IInvObjectOperAppService : IApplicationService
    {
        List<InventoryDDList> InvObjectDate();
        InvObjectOperListDto InitOperList();

        InvObjectOperListDto OperList(InvObjectOperListDto list);

        List<OperTypeListDto> OperTypeList();

        List<DocumentTypeListDto> DocumentTypeList(int? operTypeId);

        int GetNextDocumentNumber(int? documentTypeId);

        InvObjectOperEditDto ShowFormOper(InvObjectOperEditDto operation, int formNr);

        InvObjectOperEditDto InitOperation(int? operationId);

        InvObjectOperEditDto ChangeOperation(InvObjectOperEditDto operation);
        List<InvObjectsDto> InvObjectsList(InvObjectOperEditDto operation, int? storageId);
        List<InvObjectsDto> InvObjectsDtoList(InvObjectOperEditDto operation, int operationType);

        InvObjectOperEditDto InitDetails(InvObjectOperEditDto operation);

        InvObjectOperEditDto DetailChangeInvObject(InvObjectOperEditDto operation, int idOrd);

        InvObjectOperEditDto AddRow(InvObjectOperEditDto operation);

        List<InvObjectOperDetailEditDto> DeleteRow(List<InvObjectOperDetailEditDto> details, int IdOrd);

        InvObjectOperEditDto Summary(InvObjectOperEditDto operation);

        InvObjectOperEditDto SaveOperation(InvObjectOperEditDto operation);

        void DeleteOperation(int operationId);
    }
    public class InvObjectOperAppService : ErpAppServiceBase, IInvObjectOperAppService
    {
        IInvOperationRepository _invOperationRepository;
        IOperationRepository _operationRepository;
        IInvObjectRepository _invObjectRepository;
        IBalanceRepository _balanceRepository;
        IAutoOperationRepository _autoOperationRepository;
        IRepository<InvObjectOperDocType> _invObjectOperDocTypeRepository;
        IRepository<InvStorage> _invObjectStorageRepository;
        IRepository<InvObjectOperDetail> _invObjectOperDetRepository;
        IRepository<InvObjectStock> _invGestRepository;

        public InvObjectOperAppService(IInvOperationRepository invOperationRepository, IOperationRepository operationRepository, IRepository<InvObjectOperDocType> invObjectOperDocTypeRepository,
                                       IInvObjectRepository invObjectRepository, IBalanceRepository balanceRepository, IAutoOperationRepository autoOperationRepository, IRepository<InvStorage> invObjectStorageRepository,
                                        IRepository<InvObjectStock> invGestRepository, IRepository<InvObjectOperDetail> invObjectOperDetRepository)
        {
            _invOperationRepository = invOperationRepository;
            _operationRepository = operationRepository;
            _invObjectOperDocTypeRepository = invObjectOperDocTypeRepository;
            _invObjectRepository = invObjectRepository;
            _balanceRepository = balanceRepository;
            _autoOperationRepository = autoOperationRepository;
            _invObjectStorageRepository = invObjectStorageRepository;
            _invGestRepository = invGestRepository;
            _invObjectOperDetRepository = invObjectOperDetRepository;
        }


        public List<InventoryDDList> InvObjectDate()
        {
            try
            {
                var invObjectList = _invOperationRepository.GetAll().Where(f => f.State == State.Active)
                                                            .OrderByDescending(f => f.OperationDate)
                                                            .ToList()
                                                            .Select(f => new InventoryDDList { Id = f.Id, InvObjectDate = f.OperationDate.ToShortDateString() })
                                                            .ToList();
                return invObjectList;

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }

        }

        public InvObjectOperListDto InitOperList()
        {
            var ret = new InvObjectOperListDto
            {
                DataStart = LazyMethods.Now().AddMonths(-1),
                DataEnd = LazyMethods.Now(),
                ListDetail = new List<InvObjectOperListDetailDto>()
            };

            return ret;
        }

        //[AbpAuthorize("Conta.ObInventar.Operatii.Acces")]
        public InvObjectOperListDto OperList(InvObjectOperListDto list)
        {
            try
            {
                var _details = _invOperationRepository.GetAllIncluding(f => f.DocumentType)
                                                        .Where(f => f.State == State.Active && f.OperationDate >= list.DataStart && f.OperationDate <= list.DataEnd)
                                                        .OrderBy(f => f.OperationDate).ThenBy(f => f.DocumentNr)
                                                        .ToList();

                var _listDetail = ObjectMapper.Map<List<InvObjectOperListDetailDto>>(_details);
                list.ListDetail = _listDetail;
            }
            catch (System.Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex);
            }

            return list;
        }

        [AbpAuthorize("Conta.ObInventar.Operatii.Modificare")]
        public void DeleteOperation(int operationId)
        {
            var operation = _invOperationRepository.GetAllIncluding(f => f.OperDetails).FirstOrDefault(f => f.Id == operationId);

            if (!_operationRepository.VerifyClosedMonth(operation.OperationDate))
                throw new UserFriendlyException("Eroare", "Nu se poate sterge operatia deoarece luna contabila este inchisa");

            // sterg gestiunea
            try
            {
                DeleteGestInvObject(operation);
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.Message);
            }

            operation.State = State.Inactive;
            try
            {
                _invOperationRepository.Update(operation);
                CurrentUnitOfWork.SaveChanges();
            }
            catch (System.Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public List<OperTypeListDto> OperTypeList()
        {
            //var ret = _invObjectOperDocTypeRepository.GetAll().Where(f => f.AppOperation).ToList().Select(f => new OperTypeListDto { Id = (int)f.OperType, Name = f.OperType.ToString() }).OrderBy(f => f.Name).ToList();
            //return ret;

            var ret = new List<OperTypeListDto>();
            var _operType = Enum.GetValues(typeof(InvObjectOperType)).Cast<InvObjectOperType>().ToList();
            foreach (var e in _operType)
            {
                var item = new OperTypeListDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                ret.Add(item);
            }
            var operDocTypeList = _invObjectOperDocTypeRepository.GetAll()
                                                                .Where(f => f.AppOperation)
                                                                .ToList()
                                                                .Select(f => (int)f.OperType);
            ret = ret.Where(f => operDocTypeList.Contains(f.Id)).OrderBy(f => f.Name).ToList();
            return ret;
        }

        public List<DocumentTypeListDto> DocumentTypeList(int? operTypeId)
        {
            var ret = _invObjectOperDocTypeRepository.GetAllIncluding(f => f.DocumentType).Where(f => (int)f.OperType == (operTypeId ?? -1)).Select(f => new DocumentTypeListDto { Id = f.DocumentType.Id, Name = f.DocumentType.TypeName })
                .OrderBy(f => f.Name).ToList();

            return ret;
        }

        public InvObjectOperEditDto ShowFormOper(InvObjectOperEditDto operation, int formNr)
        {
            operation.ShowForm1 = (formNr == 1);
            operation.ShowForm2 = (formNr == 2);
            operation.ShowForm3 = (formNr == 3);
            return operation;
        }

        public int GetNextDocumentNumber(int? documentTypeId)
        {
            int ret;
            ret = (documentTypeId != null) ? _invOperationRepository.NextDocumentNumber(documentTypeId ?? 0) : 0;
            return ret;
        }

        //[AbpAuthorize("Conta.ObInventar.Operatii.Acces")]
        public InvObjectOperEditDto InitOperation(int? operationId)
        {
            InvObjectOperEditDto operation;

            if ((operationId ?? 0) == 0)
            {
                operation = new InvObjectOperEditDto
                {
                    OperationDate = LazyMethods.Now(),
                    DocumentDate = LazyMethods.Now(),
                    DocumentTypeId = null,
                    DocumentNr = 0,
                    FinishAdd = false,
                    ShowStorage = false
                };
            }
            else
            {
                var oper = _invOperationRepository.GetAllIncluding(f => f.InvObjectsStoreIn, f => f.InvObjectsStoreOut, f => f.DocumentType, f => f.PersonStoreOut, f => f.PersonStoreIn)
                                                    .Include(f => f.OperDetails)
                                                    .ThenInclude(f => f.InvObjectItem)
                                                    .Include(f => f.Invoice)
                                                    .ThenInclude(f => f.InvoiceDetails)
                                                  .FirstOrDefault(f => f.Id == operationId);
                operation = ObjectMapper.Map<InvObjectOperEditDto>(oper);
                var operationDetail = ObjectMapper.Map<List<InvObjectOperDetailEditDto>>(oper.OperDetails);
                var idOrd = 1;

                if (operation.OperationTypeId == (int)InvObjectOperType.Reevaluare)
                {
                    foreach (var item in operationDetail.OrderBy(f => f.Id))
                    {
                        item.IdOrd = idOrd;
                        idOrd++;
                        var gest = _invOperationRepository.GetGestDetailForInvObject(item.InvObjectItemId ?? 0, operation.OperationDate);
                        item.InvValueOld = gest.InventoryValue;
                        item.FiscalValueOld = gest.FiscalInventoryValue;
                    }
                }
                operation.Details = operationDetail;
            }

            operation = ShowFormOper(operation, 1);
            return operation;
        }

        public InvObjectOperEditDto ChangeOperation(InvObjectOperEditDto operation)
        {
            try
            {
                operation.ShowModifValues = true;
                switch ((InvObjectOperType)operation.OperationTypeId)
                {
                    case InvObjectOperType.BonMiscare:
                        operation.ShowStorage = true;
                        operation.ShowValues = false;
                        break;
                    case InvObjectOperType.Transfer:
                        operation.ShowStorage = true;
                        operation.ShowValues = false;
                        break;
                    case InvObjectOperType.DareInConsum:
                        operation.ShowStorage = true;
                        operation.ShowValues = false;
                        break;
                    case InvObjectOperType.Reevaluare:
                        operation.ShowStorage = false;
                        operation.ShowValues = true;
                        operation.ShowModifValues = false;
                        break;
                    case InvObjectOperType.Casare:
                        operation.ShowStorage = false;
                        operation.ShowValues = false;
                        break;
                    case InvObjectOperType.Iesire:
                        operation.ShowStorage = false;
                        operation.ShowValues = false;
                        break;
                    default:
                        operation.ShowStorage = false;
                        operation.ShowValues = true;
                        break;
                }
                //operation.Details = null;
            }
            catch
            {
                operation.ShowStorage = false;
                operation.ShowValues = true;
            }
            return operation;
        }

        public List<InvObjectsDto> InvObjectsList(InvObjectOperEditDto operation, int? storageId)
        {
            var ret = _invObjectRepository.GetAll().Where(f => f.State == State.Active)
                                                  .Select(f => new InvObjectsDto { Id = f.Id, Name = f.InventoryNr + ". " + f.Name, InventoryNr = f.InventoryNr })
                                                  .OrderBy(f => f.InventoryNr)
                                                  .ToList();
            return ret;
        }

        public List<InvObjectsDto> InvObjectsDtoList(InvObjectOperEditDto operation, int operationType)
        {
            var _operationType = (InvObjectOperType)operationType;
            var ret = new List<InvObjectsDto>();

            if (operation.Id == 0)
            {
                var invObjectsList = _invGestRepository.GetAllIncluding(f => f.InvObjectItem).GroupBy(f => f.InvObjectItemId).Select(f => f.Max(x => x.Id)).ToList();

                var list = _invGestRepository.GetAllIncluding(f => f.InvObjectItem).Where(f => invObjectsList.Contains(f.Id) && f.StockDate <= operation.OperationDate && f.Quantity != 0);


                if ((_operationType == InvObjectOperType.BonMiscare || _operationType == InvObjectOperType.Transfer) && operation.InvObjectsStoreOutId != null)
                {
                    list = list.Where(f => f.StorageId == operation.InvObjectsStoreOutId);
                }

                ret = list.Select(f => new InvObjectsDto { Id = f.InvObjectItemId, Name = f.InvObjectItem.InventoryNr + ". " + f.InvObjectItem.Name, InventoryNr = f.InvObjectItem.InventoryNr })
                                                              .OrderBy(f => f.InventoryNr)
                                                              .ToList();
            }
            else
            {
                var invObjectsList = _invGestRepository.GetAllIncluding(f => f.InvObjectItem).GroupBy(f => f.InvObjectItemId).Select(f => f.Max(x => x.Id)).ToList();

                var list = _invGestRepository.GetAllIncluding(f => f.InvObjectItem, f => f.InvObjectOperDet).Where(f => invObjectsList.Contains(f.Id) && f.StockDate <= operation.OperationDate);
                var invObjectOperDetIds = _invObjectOperDetRepository.GetAllIncluding(f => f.InvObjectOper).Where(f => f.State == State.Active && f.InvObjectOperId == operation.Id)
                                                                                                        .Select(f => f.InvObjectOperId).Distinct().ToList();
                if ((_operationType == InvObjectOperType.BonMiscare || _operationType == InvObjectOperType.Transfer) && operation.InvObjectsStoreOutId != null)
                {
                    list = list.Where(f => f.StorageId == operation.InvObjectsStoreOutId || invObjectOperDetIds.Contains(f.InvObjectOperDet.InvObjectOperId));
                }

                ret = list.Select(f => new InvObjectsDto { Id = f.InvObjectItemId, Name = f.InvObjectItem.InventoryNr + ". " + f.InvObjectItem.Name, InventoryNr = f.InvObjectItem.InventoryNr })
                                                              .OrderBy(f => f.InventoryNr)
                                                              .ToList();
            }



            return ret;
        }

        public InvObjectOperEditDto InitDetails(InvObjectOperEditDto operation)
        {
            if (!_operationRepository.VerifyClosedMonth(operation.OperationDate))
                throw new UserFriendlyException("Eroare", "Nu se poate inregistra operatia deoarece luna contabila este inchisa");

            if (operation.Details == null)
            {
                var details = new List<InvObjectOperDetailEditDto>();
                var detail = new InvObjectOperDetailEditDto
                {
                    IdOrd = 1,
                    Id = 0,
                    InvObjectItemId = null,
                    InvValueModif = 0,
                    Quantity = 0,
                    InvoiceDetailId = null,
                    IsSelectedInvObjectItem = false
                };
                details.Add(detail);
                operation.Details = details;
            }
            operation = ShowFormOper(operation, 2);
            return operation;
        }

        public InvObjectOperEditDto DetailChangeInvObject(InvObjectOperEditDto operation, int idOrd)
        {
            try
            {
                if ((operation.OperationTypeId ?? 0) == (int)InvObjectOperType.Reevaluare)
                {
                    var detail = operation.Details.FirstOrDefault(f => f.IdOrd == idOrd);
                    var gest = _invOperationRepository.GetGestDetailForInvObject(detail.InvObjectItemId ?? 0, operation.OperationDate);
                    detail.InvValueOld = gest.InventoryValue;
                    detail.FiscalValueOld = gest.FiscalInventoryValue;
                }
                else if ((operation.OperationTypeId ?? 0) == (int)InvObjectOperType.Casare || (operation.OperationTypeId ?? 0) == (int)InvObjectOperType.Iesire || (operation.OperationTypeId ?? 0) == (int)InvObjectOperType.Vanzare)
                {
                    var detail = operation.Details.FirstOrDefault(f => f.IdOrd == idOrd);
                    var gest = _invOperationRepository.GetGestDetailForInvObject(detail.InvObjectItemId ?? 0, operation.OperationDate);
                    detail.InvValueModif = -1 * gest.InventoryValue;
                }
                return operation;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public InvObjectOperEditDto AddRow(InvObjectOperEditDto operation)
        {
            var maxOrd = operation.Details.Max(f => f.IdOrd);
            maxOrd++;
            var detail = new InvObjectOperDetailEditDto
            {
                IdOrd = maxOrd,
                Id = 0,
                InvObjectItemId = null,
                InvValueModif = 0,
                Quantity = 0,
                InvoiceDetailId = 0
            };
            operation.Details.Add(detail);

            operation = ShowFormOper(operation, 2);
            return operation;
        }

        public List<InvObjectOperDetailEditDto> DeleteRow(List<InvObjectOperDetailEditDto> details, int IdOrd)
        {
            var detail = details.FirstOrDefault(f => f.IdOrd == IdOrd);
            details.Remove(detail);

            return details.ToList();
        }

        public void CheckForDuplicateActiva(InvObjectOperEditDto operation)
        {
            if (operation.Details.Count != operation.Details.GroupBy(x => x.InvObjectItemId).Select(g => g.First()).ToList().Count())
            {
                throw new UserFriendlyException("Activa introdusa de mai multe ori");
            }
        }

        public InvObjectOperEditDto Summary(InvObjectOperEditDto operation)
        {


            CheckForDuplicateActiva(operation);

            if (operation.InvObjectOperType == InvObjectOperType.Vanzare || operation.InvObjectOperType == InvObjectOperType.Modernizare)
            {
                var count = operation.Details.Count(f => f.InvoiceDetailId == null);
                if (count != 0)
                {
                    throw new UserFriendlyException("Eroare", "Nu ati selectat detaliul facturii pentru activul/activele selectate");
                }
            }

            operation.OperationType = ((InvObjectOperType)operation.OperationTypeId).ToString();
            operation.InvObjectOperType = (InvObjectOperType)operation.OperationTypeId;
            operation.OperationDateStr = operation.OperationDate.ToShortDateString();
            operation.DocumentDateStr = operation.DocumentDate.ToShortDateString();
            var documentType = _invObjectOperDocTypeRepository.GetAllIncluding(f => f.DocumentType)
                                        .FirstOrDefault(f => f.DocumentTypeId == operation.DocumentTypeId && f.OperType == (InvObjectOperType)operation.OperationTypeId);
            operation.DocumentType = documentType.DocumentType.TypeName;
            if (operation.InvObjectsStoreInId != null)
            {
                var storageIn = _invObjectStorageRepository.FirstOrDefault(f => f.Id == operation.InvObjectsStoreInId);
                operation.InvObjectsStoreIn = storageIn.StorageName;
            }
            if (operation.InvObjectsStoreOutId != null)
            {
                var storageOut = _invObjectStorageRepository.FirstOrDefault(f => f.Id == operation.InvObjectsStoreOutId);
                operation.InvObjectsStoreOut = storageOut.StorageName;
            }

            foreach (var detail in operation.Details)
            {
                var invObject = _invObjectRepository.FirstOrDefault(f => f.Id == detail.InvObjectItemId);
                detail.InvObjectItem = invObject.InventoryNr + ". " + invObject.Name;
            }

            operation = ShowFormOper(operation, 3);
            return operation;
        }

        [AbpAuthorize("Conta.ObInventar.Operatii.Modificare")]
        public InvObjectOperEditDto SaveOperation(InvObjectOperEditDto operation)
        {
            try
            {
                InvObjectOper oper = ObjectMapper.Map<InvObjectOper>(operation);
                oper.OperationDate = (operation.InvObjectOperType == InvObjectOperType.Reevaluare) ? LazyMethods.LastDayOfMonth(oper.OperationDate) : oper.OperationDate;

                var appClient = GetCurrentTenant();
                var operDetails = new List<InvObjectOperDetail>();
                int invObjectItemId = 0;
                foreach (var detail in operation.Details)
                {
                    var operDetail = ObjectMapper.Map<InvObjectOperDetail>(detail);
                    operDetail.InvObjectOperId = oper.Id;
                    operDetail.InvoiceDetailId = detail.InvoiceDetailId != 0 ? detail.InvoiceDetailId : (int?)null;
                    operDetail.TenantId = appClient.Id;
                    invObjectItemId = operDetail.InvObjectItemId;
                    operDetails.Add(operDetail);
                }

                oper.TenantId = appClient.Id;
                if (oper.Id == 0)
                {
                    oper.OperDetails = operDetails;
                    _invOperationRepository.Insert(oper);
                    CurrentUnitOfWork.SaveChanges();

                }
                else
                {
                    oper.OperDetails = operDetails;
                    _invOperationRepository.UpdateInvObjectOperation(oper);

                    //sterg gestiunea
                    DeleteGestInvObject(oper);
                }

                //generare gestiune
                foreach (var item in operDetails)
                {
                    // _imoAssetOperRepository.GestComputingForAsset(oper.OperationDate, item.ImoAssetItemId);
                    _invOperationRepository.GestComputingForInvObject(item.InvObjectItemId, oper.OperationDate);
                }


                if (oper.InvObjectsOperType != InvObjectOperType.BonMiscare && oper.InvObjectsOperType != InvObjectOperType.Transfer)
                {
                    var lastBalanceDate = _balanceRepository.GetAll().Where(f => f.Status == State.Active).OrderByDescending(f => f.BalanceDate).FirstOrDefault().BalanceDate;

                    // generez note contabile din gestiune
                    _autoOperationRepository.AutoInvObjectOperationAdd(oper.OperationDate, appClient.Id, appClient.LocalCurrencyId.Value, lastBalanceDate, null);
                }
                operation.FinishAdd = true;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
            return operation;
        }

        private void DeleteGestInvObject(InvObjectOper oper)
        {
            foreach (var item in oper.OperDetails)
            {
                var invObjectStock = _invGestRepository.GetAll().FirstOrDefault(f => f.InvObjectItemId == item.InvObjectItemId);

                if (invObjectStock != null)
                {
                    _autoOperationRepository.DeleteInvObjectDetailOperation(oper.OperationDate, invObjectStock.InvObjectItemId);
                    var lastProcessedDateForInvObjectItem = _invOperationRepository.LastProcessedDateForInvObject(invObjectStock.InvObjectItemId);

                    if (oper.OperationDate.Date <= lastProcessedDateForInvObjectItem)
                    {
                        _invOperationRepository.GestDelComputingForInvObject(oper.OperationDate.Date, invObjectStock.InvObjectItemId);
                    }
                }


            }
        }
    }
}
