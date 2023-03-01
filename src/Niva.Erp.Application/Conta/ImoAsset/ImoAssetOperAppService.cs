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
    public interface IImoAssetOperAppService : IApplicationService
    {

        ImoAssetOperListDto InitOperList();

        ImoAssetOperListDto OperList(ImoAssetOperListDto list);

        List<OperTypeListDto> OperTypeList();

        List<DocumentTypeListDto> DocumentTypeList(int? operTypeId);

        int GetNextDocumentNumber(int? documentTypeId);

        ImoAssetOperEditDto ShowFormOper(ImoAssetOperEditDto operation, int formNr);

        ImoAssetOperEditDto InitOperation(int? operationId);

        ImoAssetOperEditDto ChangeOperation(ImoAssetOperEditDto operation);

        List<ImoAssetsDDDto> AssetsDDDList(ImoAssetOperEditDto operation, int? storageId);

        List<ImoAssetsDDDto> AssetsDDList(ImoAssetOperEditDto operation, int operationType);

        ImoAssetOperEditDto InitDetails(ImoAssetOperEditDto operation);

        ImoAssetOperEditDto DetailChangeAsset(ImoAssetOperEditDto operation, int idOrd);

        ImoAssetOperEditDto AddRow(ImoAssetOperEditDto operation);

        ImoAssetOperEditDto OperDeleteRow(ImoAssetOperEditDto operation, int IdOrd);

        ImoAssetOperEditDto Summary(ImoAssetOperEditDto operation);

        ImoAssetOperEditDto SaveOperation(ImoAssetOperEditDto operation);

        void DeleteOperation(int operationId);

        ImoAssetOperEditDto ViewOperation(int operationId);

        public ImoAssetOperEditDto DetailAccountAsset(ImoAssetOperEditDto operation, int idOrd);
        public void UpateImoAssetAccounts(ImoAssetOperEditDto operation);
    }

    public class ImoAssetOperAppService : ErpAppServiceBase, IImoAssetOperAppService
    {
        //IRepository<ImoAssetOper> _imoAssetOperRepository;
        IRepository<ImoAssetOperDocType> _imoAssetOperDocTypeRepository;
        IImoAssetRepository _imoAssetRepository;
        IRepository<ImoAssetStorage> _imoAssetStorageRepository;
        IImoOperationRepository _imoAssetOperRepository;
        IRepository<ImoAssetOperDetail> _imoAssetOperDetRepository;
        IOperationRepository _operationRepository;
        IRepository<ImoAssetOperForType> _imoAssetOperForTypeRepository;
        IBalanceRepository _balanceRepository;
        IAutoOperationRepository _autoOperationRepository;
        IRepository<ImoAssetStock> _imoGestRepository;

        public ImoAssetOperAppService(IRepository<ImoAssetOperDocType> imoAssetOperDocTypeRepository,
                                      IImoAssetRepository imoAssetRepository, IRepository<ImoAssetStorage> imoAssetStorageRepository, IImoOperationRepository imoAssetOperRepository,
                                      IOperationRepository operationRepository, IRepository<ImoAssetOperForType> imoAssetOperForTypeRepository, IBalanceRepository balanceRepository,
                                      IAutoOperationRepository autoOperationRepository, IRepository<ImoAssetStock> imoGestRepository, IRepository<ImoAssetOperDetail> imoAssetOperDetRepository)
        {
            _imoAssetOperDocTypeRepository = imoAssetOperDocTypeRepository;
            _imoAssetRepository = imoAssetRepository;
            _imoAssetStorageRepository = imoAssetStorageRepository;
            _imoAssetOperRepository = imoAssetOperRepository;
            _operationRepository = operationRepository;
            _imoAssetOperForTypeRepository = imoAssetOperForTypeRepository;
            _balanceRepository = balanceRepository;
            _autoOperationRepository = autoOperationRepository;
            _imoGestRepository = imoGestRepository;
            _imoAssetOperDetRepository = imoAssetOperDetRepository;
        }

        //[AbpAuthorize("Conta.MF.Operatii.Acces")]
        public ImoAssetOperListDto InitOperList()
        {
            var ret = new ImoAssetOperListDto
            {
                DataStart = LazyMethods.Now().AddMonths(-1),
                DataEnd = LazyMethods.Now(),
                ListDetail = new List<ImoAssetOperListDetailDto>()
            };

            return ret;
        }

        //[AbpAuthorize("Conta.MF.Operatii.Acces")]
        public ImoAssetOperListDto OperList(ImoAssetOperListDto list)
        {
            try
            {
                var _details = _imoAssetOperRepository.GetAllIncluding(f => f.DocumentType)
                                                      .Where(f => f.State == State.Active && f.OperationDate >= list.DataStart && f.OperationDate <= list.DataEnd)
                                                      .OrderBy(f => f.OperationDate).ThenBy(f => f.DocumentNr)
                                                      .ToList();
                var _listDetail = ObjectMapper.Map<List<ImoAssetOperListDetailDto>>(_details);
                list.ListDetail = _listDetail;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
            return list;
        }

        //[AbpAuthorize("Conta.MF.Operatii.Acces")]
        public List<OperTypeListDto> OperTypeList()
        {
            //var ret = _imoAssetOperDocTypeRepository.GetAll()
            //                .Where(f => f.AppOperation)
            //                .ToList()
            //                .Select(f => new OperTypeListDto { Id = (int)f.OperType, Name = f.OperType.ToString() })
            //                .OrderBy(f => f.Name).ToList()
            //                .Distinct()
            //                .ToList();
            var ret = new List<OperTypeListDto>();
            var _operType = Enum.GetValues(typeof(ImoAssetOperType)).Cast<ImoAssetOperType>().ToList();
            foreach (var e in _operType)
            {
                var item = new OperTypeListDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                ret.Add(item);
            }
            var operDocTypeList = _imoAssetOperDocTypeRepository.GetAll()
                                                                .Where(f => f.AppOperation)
                                                                .ToList()
                                                                .Select(f => (int)f.OperType);
            ret = ret.Where(f => operDocTypeList.Contains(f.Id)).OrderBy(f => f.Name).ToList();
            return ret;
        }

        //[AbpAuthorize("Conta.MF.Operatii.Acces")]
        public List<DocumentTypeListDto> DocumentTypeList(int? operTypeId)
        {
            var ret = _imoAssetOperDocTypeRepository.GetAllIncluding(f => f.DocumentType)
                            .Where(f => (int)f.OperType == (operTypeId ?? -1))
                            .Select(f => new DocumentTypeListDto { Id = f.DocumentType.Id, Name = f.DocumentType.TypeName })
                            .OrderBy(f => f.Name).ToList();
            return ret;
        }

        //[AbpAuthorize("Conta.MF.Operatii.Acces")]
        public int GetNextDocumentNumber(int? documentTypeId)
        {
            int ret;
            ret = (documentTypeId != null) ? _imoAssetOperRepository.NextDocumentNumber(documentTypeId ?? 0) : 0;
            return ret;
        }

        //[AbpAuthorize("Conta.MF.Operatii.Acces")]
        public ImoAssetOperEditDto ShowFormOper(ImoAssetOperEditDto operation, int formNr)
        {
            operation.ShowForm1 = (formNr == 1);
            operation.ShowForm2 = (formNr == 2);
            operation.ShowForm3 = (formNr == 3);
            return operation;
        }

        //[AbpAuthorize("Conta.MF.Operatii.Acces")]
        public ImoAssetOperEditDto InitOperation(int? operationId)
        {
            ImoAssetOperEditDto operation;

            if ((operationId ?? 0) == 0)
            {
                operation = new ImoAssetOperEditDto
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
                var oper = _imoAssetOperRepository.GetAllIncluding(f => f.AssetsStoreIn, f => f.AssetsStoreOut, f => f.DocumentType, f => f.Invoice.InvoiceDetails, f => f.PersonStoreIn, f => f.PersonStoreOut)
                                                    .Include(f => f.OperDetails)
                                                    .ThenInclude(f => f.ImoAssetItem)
                                                  .FirstOrDefault(f => f.Id == operationId);
                if (oper.AssetsOperType == ImoAssetOperType.ModificareConturiFaraInregistrareNotaContabila || oper.AssetsOperType == ImoAssetOperType.ModificareConturiCuInregistrareNotaContabila)
                {
                    operation = ObjectMapper.Map<ImoAssetOperEditDto>(oper);
                    var operationDetail = ObjectMapper.Map<List<ImoAssetOperDetailEditModifAccountDto>>(oper.OperDetails);
                    var idOrd = 1;

                    //if (operation.OperationTypeId == (int)ImoAssetOperType.Reevaluare)
                    //{
                    foreach (var item in operationDetail.OrderBy(f => f.Id))
                    {
                        item.IdOrd = idOrd;
                        idOrd++;
                        var assetOperDetail = _imoAssetOperDetRepository.GetAllIncluding(f => f.ImoAssetItem, f => f.OldAssetAccount, f => f.OldAssetAccountInUse, f => f.OldDepreciationAccount, f => f.OldExpenseAccount, f => f.NewAssetAccount, f => f.NewAssetAccountInUse,
                                                                              f => f.NewDepreciationAccount, f => f.NewExpenseAccount)
                                                             .Where(f => f.Id == item.Id)
                                                             .FirstOrDefault();

                        item.OldAssetAccount = assetOperDetail.OldAssetAccount != null ? assetOperDetail.OldAssetAccount.AccountName : "";
                        item.OldAssetAccountInUse = assetOperDetail.OldAssetAccountInUse != null ? assetOperDetail.OldAssetAccountInUse.AccountName : "";
                        item.OldDepreciationAccount = assetOperDetail.OldDepreciationAccount.AccountName;
                        item.OldExpenseAccount = assetOperDetail.OldExpenseAccount.AccountName;
                        item.NewAssetAccount = assetOperDetail.NewAssetAccount.AccountName;
                        item.NewAssetAccountInUse = assetOperDetail.NewAssetAccountInUse.AccountName;
                        item.NewDepreciationAccount = assetOperDetail.NewDepreciationAccount.AccountName;
                        item.NewExpenseAccount = assetOperDetail.NewExpenseAccount.AccountName;
                    }
                    //}
                    operation.AssetAccountDetails = operationDetail;
                }
                else
                {
                    operation = ObjectMapper.Map<ImoAssetOperEditDto>(oper);
                    var operationDetail = ObjectMapper.Map<List<ImoAssetOperDetailEditDto>>(oper.OperDetails);
                    var idOrd = 1;

                    //if (operation.OperationTypeId == (int)ImoAssetOperType.Reevaluare)
                    //{
                    foreach (var item in operationDetail.OrderBy(f => f.Id))
                    {
                        item.IdOrd = idOrd;
                        idOrd++;
                        var gest = _imoAssetOperRepository.GetGestDetailForAsset(item.ImoAssetItemId ?? 0, operation.OperationDate);
                        item.InvValueOld = gest.InventoryValue;
                        item.InvValueNew = item.InvValueOld + item.InvValueModif;
                        item.FiscalValueOld = gest.FiscalInventoryValue;
                        item.FiscalValueNew = item.FiscalValueOld + item.FiscalValueModif;
                    }
                    //}
                    operation.Details = operationDetail;
                }
            }

            operation = ShowFormOper(operation, 1);
            return operation;
        }

        [AbpAuthorize("Conta.MF.Operatii.Modificare")]
        public ImoAssetOperEditDto ChangeOperation(ImoAssetOperEditDto operation)
        {
            try
            {
                operation.ShowModifValues = true;
                switch ((ImoAssetOperType)operation.OperationTypeId)
                {
                    case ImoAssetOperType.BonMiscare:
                        operation.ShowStorage = true;
                        operation.ShowValues = false;
                        break;
                    case ImoAssetOperType.Transfer:
                        operation.ShowStorage = true;
                        operation.ShowValues = false;
                        break;
                    case ImoAssetOperType.IntrareInConservare:
                        operation.ShowStorage = false;
                        operation.ShowValues = false;
                        break;
                    case ImoAssetOperType.IesireDinConservare:
                        operation.ShowStorage = false;
                        operation.ShowValues = false;
                        break;
                    case ImoAssetOperType.Reevaluare:
                        operation.ShowStorage = false;
                        operation.ShowValues = true;
                        operation.ShowModifValues = false;
                        break;
                    case ImoAssetOperType.Casare:
                        operation.ShowStorage = false;
                        operation.ShowValues = false;
                        break;
                    case ImoAssetOperType.Iesire:
                        operation.ShowStorage = false;
                        operation.ShowValues = false;
                        break;
                    case ImoAssetOperType.ModificareConturiFaraInregistrareNotaContabila:
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

        [AbpAuthorize("Conta.MF.Operatii.Modificare")]
        public List<ImoAssetsDDDto> AssetsDDDList(ImoAssetOperEditDto operation, int? storageId)
        {
            var ret = _imoAssetRepository.GetAll().Where(f => f.State == State.Active)
                                                  .Select(f => new ImoAssetsDDDto { Id = f.Id, Name = f.InventoryNr + ". " + f.Name, InventoryNr = f.InventoryNr })
                                                  .OrderBy(f => f.InventoryNr)
                                                  .ToList();
            return ret;
        }

        [AbpAuthorize("Conta.MF.Operatii.Modificare")]
        public List<ImoAssetsDDDto> AssetsDDList(ImoAssetOperEditDto operation, int operationType)
        {
            var _operationType = (ImoAssetOperType)operationType;
            var assetTypes = _imoAssetOperForTypeRepository.GetAll().Where(f => f.ImoAssetOperType == _operationType).Select(f => f.ImoAssetType).ToList();


            var assetsList = _imoGestRepository.GetAllIncluding(f => f.ImoAssetItem, f => f.ImoAssetOperDet)
                                               .Where(f => f.StockDate <= operation.OperationDate)
                                               .GroupBy(f => f.ImoAssetItemId).Select(f => f.Max(x => x.Id)).ToList();

            var list = _imoGestRepository.GetAllIncluding(f => f.ImoAssetItem, f => f.ImoAssetOperDet).Where(f => assetsList.Contains(f.Id) && f.StockDate <= operation.OperationDate && f.Quantity != 0);

            if ((_operationType == ImoAssetOperType.BonMiscare || _operationType == ImoAssetOperType.Transfer) && operation.AssetsStoreOutId != null)
            {
                var imoAssetOperDetIds = _imoAssetOperDetRepository.GetAllIncluding(f => f.ImoAssetOper).Where(f => f.State == State.Active && f.ImoAssetOperId == operation.Id)
                    .Select(f => f.ImoAssetOperId).Distinct().ToList();
                if (operation.Id != 0)
                {
                    list = list.Where(f => f.StorageId == operation.AssetsStoreOutId || imoAssetOperDetIds.Contains(f.ImoAssetOperDet.ImoAssetOperId));
                }
                else
                {
                    list = list.Where(f => f.StorageId == operation.AssetsStoreOutId);
                }

            }

            if (assetTypes.Contains(ImoAssetType.DreptDeUtilizare) && assetTypes.Contains(ImoAssetType.MijlocFix))
            {

            }
            else
            {
                if (assetTypes.Contains(ImoAssetType.DreptDeUtilizare))
                {
                    list = list.Where(f => f.ImoAssetItem.ImoAssetType == ImoAssetType.DreptDeUtilizare);
                }
                else if (assetTypes.Contains(ImoAssetType.MijlocFix))
                {
                    list = list.Where(f => f.ImoAssetItem.ImoAssetType == ImoAssetType.MijlocFix);
                }
            }

            var ret = list.Select(f => new ImoAssetsDDDto { Id = f.ImoAssetItem.Id, Name = f.ImoAssetItem.InventoryNr + ". " + f.ImoAssetItem.Name, InventoryNr = f.ImoAssetItem.InventoryNr })
                                                  .Distinct()
                                                  .OrderBy(f => f.InventoryNr)
                                                  .ToList();

            return ret;
        }

        //[AbpAuthorize("Conta.MF.Operatii.Acces")]
        public ImoAssetOperEditDto InitDetails(ImoAssetOperEditDto operation)
        {
            DateTime lastProcessedDate = _imoAssetOperRepository.LastProcessedDate();

            if (!_operationRepository.VerifyClosedMonth(operation.OperationDate))
                throw new UserFriendlyException("Eroare", "Nu se poate inregistra operatia deoarece luna contabila este inchisa");

            //if (lastProcessedDate >= operation.OperationDate)
            //    throw new UserFriendlyException("Eroare data operatie", "Data operatiei nu poate fi mai mica decat data ultimei operatii procesate in gestiune " + lastProcessedDate.ToShortDateString());
            if (operation.OperationTypeId != (int)ImoAssetOperType.ModificareConturiFaraInregistrareNotaContabila && operation.OperationTypeId != (int)ImoAssetOperType.ModificareConturiCuInregistrareNotaContabila)
            {
                if (operation.Details == null)
                {
                    var details = new List<ImoAssetOperDetailEditDto>();
                    var detail = new ImoAssetOperDetailEditDto
                    {
                        IdOrd = 1,
                        Id = 0,
                        DeprecModif = 0,
                        DurationModif = 0,
                        FiscalDeprecModif = 0,
                        FiscalValueModif = 0,
                        ImoAssetItemId = null,
                        InvValueModif = 0,
                        Quantity = 0,
                        InvoiceDetailId = null
                    };
                    details.Add(detail);
                    operation.Details = details;
                }
            }
            else
            {
                if (operation.AssetAccountDetails == null)
                {
                    var assetAccountDetails = new List<ImoAssetOperDetailEditModifAccountDto>();
                    var assetAccountDetail = new ImoAssetOperDetailEditModifAccountDto
                    {
                        IdOrd = 1,
                        Id = 0,
                        OldAssetAccountId = null,
                        OldAssetAccountInUseId = null,
                        OldDepreciationAccountId = null,
                        OldExpenseAccountId = null,
                        NewAssetAccountId = null,
                        NewAssetAccountInUseId = null,
                        NewDepreciationAccountId = null,
                        NewExpenseAccountId = null
                    };
                    assetAccountDetails.Add(assetAccountDetail);
                    operation.AssetAccountDetails = assetAccountDetails;
                }
            }
            operation = ShowFormOper(operation, 2);
            return operation;
        }

        [AbpAuthorize("Conta.MF.Operatii.Modificare")]
        public ImoAssetOperEditDto DetailChangeAsset(ImoAssetOperEditDto operation, int idOrd)
        {
            try
            {
                if ((operation.OperationTypeId ?? 0) == (int)ImoAssetOperType.Reevaluare)
                {
                    var detail = operation.Details.FirstOrDefault(f => f.IdOrd == idOrd);
                    var gest = _imoAssetOperRepository.GetGestDetailForAsset(detail.ImoAssetItemId ?? 0, operation.OperationDate);
                    detail.InvValueOld = gest.InventoryValue;
                    detail.FiscalValueOld = gest.FiscalInventoryValue;
                }
                else if ((operation.OperationTypeId ?? 0) == (int)ImoAssetOperType.Casare || (operation.OperationTypeId ?? 0) == (int)ImoAssetOperType.Iesire || (operation.OperationTypeId ?? 0) == (int)ImoAssetOperType.Vanzare)
                {
                    var detail = operation.Details.FirstOrDefault(f => f.IdOrd == idOrd);
                    var gest = _imoAssetOperRepository.GetGestDetailForAsset(detail.ImoAssetItemId ?? 0, operation.OperationDate);
                    detail.InvValueModif = -1 * gest.InventoryValue;
                    detail.FiscalValueModif = -1 * gest.FiscalInventoryValue;
                }
                return operation;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        [AbpAuthorize("Conta.MF.Operatii.Modificare")]
        public ImoAssetOperEditDto AddRow(ImoAssetOperEditDto operation)
        {
            var maxOrd = operation.Details.Max(f => f.IdOrd);
            maxOrd++;
            var detail = new ImoAssetOperDetailEditDto
            {
                IdOrd = maxOrd,
                Id = 0,
                DeprecModif = 0,
                DurationModif = 0,
                FiscalDeprecModif = 0,
                FiscalValueModif = 0,
                ImoAssetItemId = null,
                InvValueModif = 0,
                Quantity = 0,
                InvoiceDetailId = 0
            };
            operation.Details.Add(detail);

            operation = ShowFormOper(operation, 2);
            return operation;
        }

        [AbpAuthorize("Conta.MF.Operatii.Modificare")]
        public ImoAssetOperEditDto OperDeleteRow(ImoAssetOperEditDto operation, int IdOrd)
        {
            if (operation.OperationTypeId == (int)ImoAssetOperType.ModificareConturiFaraInregistrareNotaContabila || operation.OperationTypeId == (int)ImoAssetOperType.ModificareConturiCuInregistrareNotaContabila)
            {
                var detail = operation.AssetAccountDetails.FirstOrDefault(f => f.IdOrd == IdOrd);

                var imoAssetItem = _imoAssetRepository.GetAllIncluding(f => f.AssetAccount, f => f.AssetAccountInUse, f => f.DepreciationAccount, f => f.ExpenseAccount)
                                                      .Where(f => f.Id == operation.AssetAccountDetails[IdOrd - 1].ImoAssetItemId && f.State == State.Active).FirstOrDefault();

                imoAssetItem.AssetAccountId = detail.OldAssetAccountId;
                imoAssetItem.AssetAccountInUseId = detail.OldAssetAccountInUseId;
                imoAssetItem.DepreciationAccountId = detail.OldDepreciationAccountId;
                imoAssetItem.ExpenseAccountId = detail.OldExpenseAccountId;

                _imoAssetRepository.Update(imoAssetItem);
                operation.AssetAccountDetails.Remove(detail);
            }
            else
            {
                var detail = operation.Details.FirstOrDefault(f => f.IdOrd == IdOrd);
                operation.Details.Remove(detail);
            }
            return operation;
        }

        [AbpAuthorize("Conta.MF.Operatii.Modificare")]
        public ImoAssetOperEditDto Summary(ImoAssetOperEditDto operation)
        {
            if (operation.AssetsOperType == ImoAssetOperType.Vanzare || operation.OperationTypeId == (int)ImoAssetOperType.Modernizare)
            {
                var count = operation.Details.Count(f => f.InvoiceDetailId == 0);
                if (count != 0)
                {
                    throw new UserFriendlyException("Eroare", "Nu ati selectat detaliul facturii pentru activul/activele selectate");
                }
            }

            operation.OperationType = ((ImoAssetOperType)operation.OperationTypeId).ToString();
            operation.AssetsOperType = (ImoAssetOperType)operation.OperationTypeId;
            operation.OperationDateStr = operation.OperationDate.ToShortDateString();
            operation.DocumentDateStr = operation.DocumentDate.ToShortDateString();
            var documentType = _imoAssetOperDocTypeRepository.GetAllIncluding(f => f.DocumentType)
                                        .FirstOrDefault(f => f.DocumentTypeId == operation.DocumentTypeId && f.OperType == (ImoAssetOperType)operation.OperationTypeId);
            operation.DocumentType = documentType.DocumentType.TypeName;
            if (operation.AssetsStoreInId != null)
            {
                var storageIn = _imoAssetStorageRepository.FirstOrDefault(f => f.Id == operation.AssetsStoreInId);
                operation.AssetsStoreIn = storageIn.StorageName;
            }
            if (operation.AssetsStoreOutId != null)
            {
                var storageOut = _imoAssetStorageRepository.FirstOrDefault(f => f.Id == operation.AssetsStoreOutId);
                operation.AssetsStoreOut = storageOut.StorageName;
            }

            foreach (var detail in operation.Details)
            {
                var asset = _imoAssetRepository.FirstOrDefault(f => f.Id == detail.ImoAssetItemId);
                detail.ImoAssetItem = asset.InventoryNr + ". " + asset.Name;
            }

            operation = ShowFormOper(operation, 3);
            return operation;
        }

        [AbpAuthorize("Conta.MF.Operatii.Modificare")]
        public ImoAssetOperEditDto SaveOperation(ImoAssetOperEditDto operation)
        {
            try
            {
                ImoAssetOper oper = ObjectMapper.Map<ImoAssetOper>(operation);
                oper.OperUseStartDate = (operation.AssetsOperType == ImoAssetOperType.Reevaluare) ? LazyMethods.LastDayOfMonth(oper.OperationDate) : oper.OperationDate;

                var appClient = GetCurrentTenant();
                var operDetails = new List<ImoAssetOperDetail>();
                int imoAssetItemId = 0;
                foreach (var detail in operation.Details)
                {
                    var operDetail = ObjectMapper.Map<ImoAssetOperDetail>(detail);
                    operDetail.ImoAssetOperId = oper.Id;
                    operDetail.InvoiceDetailId = (oper.AssetsOperType == ImoAssetOperType.Vanzare || oper.AssetsOperType == ImoAssetOperType.Modernizare) ? detail.InvoiceDetailId : oper.InvoiceId;
                    operDetail.TenantId = appClient.Id;
                    imoAssetItemId = operDetail.ImoAssetItemId;
                    operDetails.Add(operDetail);
                }

                oper.TenantId = appClient.Id;
                if (oper.Id == 0)
                {
                    oper.OperDetails = operDetails;
                    _imoAssetOperRepository.Insert(oper);
                    CurrentUnitOfWork.SaveChanges();

                }
                else
                {
                    oper.OperDetails = operDetails;
                    _imoAssetOperRepository.UpdateAssetOperation(oper);
                    //delete childred

                    DeleteGestAsset(oper);
                }

                //generare gestiune
                foreach (var item in operDetails)
                {
                    _imoAssetOperRepository.GestComputingForAsset(oper.OperationDate, item.ImoAssetItemId);
                }


                if (oper.AssetsOperType != ImoAssetOperType.BonMiscare && oper.AssetsOperType != ImoAssetOperType.Transfer)
                {
                    var lastBalanceDate = _balanceRepository.GetAll().Where(f => f.Status == State.Active).OrderByDescending(f => f.BalanceDate).FirstOrDefault().BalanceDate;

                    // generez note contabile din gestiune
                    _autoOperationRepository.AutoImoAssetOperationAdd(oper.OperationDate, appClient.Id, appClient.LocalCurrencyId.Value, ImoAssetType.MijlocFix, lastBalanceDate, null);

                }
                operation.FinishAdd = true;

                //   CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
            return operation;
        }

        [AbpAuthorize("Conta.MF.Operatii.Modificare")]
        public void DeleteOperation(int operationId)
        {
            var _operation = _imoAssetOperRepository.GetAllIncluding(f => f.OperDetails).FirstOrDefault(f => f.Id == operationId);


            if (!_operationRepository.VerifyClosedMonth(_operation.OperationDate))
                throw new UserFriendlyException("Eroare", "Nu se poate sterge operatia deoarece luna contabila este inchisa");

            if (_operation.AssetsOperType ==  ImoAssetOperType.ModificareConturiFaraInregistrareNotaContabila || _operation.AssetsOperType ==  ImoAssetOperType.ModificareConturiCuInregistrareNotaContabila)
            {
                foreach (var detail in _operation.OperDetails)
                {
                    var imoAssetItem = _imoAssetRepository.GetAllIncluding(f => f.AssetAccount, f => f.AssetAccountInUse, f => f.DepreciationAccount, f => f.ExpenseAccount)
                                                     .Where(f => f.Id == detail.ImoAssetItemId && f.State == State.Active).FirstOrDefault();

                    imoAssetItem.AssetAccountId = detail.OldAssetAccountId;
                    imoAssetItem.AssetAccountInUseId = detail.OldAssetAccountInUseId;
                    imoAssetItem.DepreciationAccountId = detail.OldDepreciationAccountId;
                    imoAssetItem.ExpenseAccountId = detail.OldExpenseAccountId;

                    _imoAssetRepository.Update(imoAssetItem);
                    CurrentUnitOfWork.SaveChanges();
                }
            }else
            {

                try
                {
                    DeleteGestAsset(_operation);
                }
                catch (Exception ex)
                {
                    throw new UserFriendlyException("Eroare", ex.Message);
                }
            }


            _operation.State = State.Inactive;
            _imoAssetOperRepository.Update(_operation);
            try
            {
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }

        }

        [AbpAuthorize("Conta.MF.Operatii.Modificare")]
        private void DeleteGestAsset(ImoAssetOper imoAssetOper)
        {
            foreach (var item in imoAssetOper.OperDetails)
            {
                var imoAssetStock = _imoGestRepository.GetAll().FirstOrDefault(f => f.ImoAssetItemId == item.ImoAssetItemId);

                if (imoAssetStock != null)
                {
                    _autoOperationRepository.DeleteUncheckedAssetDetailOperation(imoAssetOper.OperationDate.Date, imoAssetStock.ImoAssetItemId);
                    var lastProcessedDateForImoAssetItem = _imoAssetOperRepository.LastProcessedDateForAsset(imoAssetStock.ImoAssetItemId);

                    if (imoAssetOper.OperationDate.Date <= lastProcessedDateForImoAssetItem)
                    {
                        _imoAssetOperRepository.GestDelComputingForAsset(imoAssetOper.OperationDate.Date, imoAssetStock.ImoAssetItemId);
                    }
                }


            }
        }

        [AbpAuthorize("Conta.MF.Operatii.Modificare")]
        public ImoAssetOperEditDto ViewOperation(int operationId)
        {
            var operation = _imoAssetOperRepository.GetAllIncluding(f => f.AssetsStoreIn, f => f.AssetsStoreOut, f => f.DocumentType)
                                                    .Include(f => f.OperDetails)
                                                    .ThenInclude(f => f.ImoAssetItem)
                                                   .FirstOrDefault(f => f.Id == operationId);

            var ret = new ImoAssetOperEditDto
            {
                Id = operation.Id,
                DocumentNr = operation.DocumentNr,
                DocumentDate = operation.DocumentDate,
                DocumentDateStr = operation.DocumentDate.ToShortDateString(),
                DocumentType = operation.DocumentType.TypeName,
                AssetsStoreIn = (operation.AssetsStoreIn == null) ? "" : operation.AssetsStoreIn.StorageName,
                AssetsStoreOut = (operation.AssetsStoreOut == null) ? "" : operation.AssetsStoreOut.StorageName,
                AssetsOperType = operation.AssetsOperType,
                OperationDate = operation.OperationDate,
                OperationDateStr = operation.OperationDate.ToShortDateString(),
                OperationType = operation.AssetsOperType.ToString(),
                OperationTypeId = (int)operation.AssetsOperType
            };

            var details = new List<ImoAssetOperDetailEditDto>();

            foreach (var item in operation.OperDetails)
            {
                var detail = new ImoAssetOperDetailEditDto
                {
                    ImoAssetItem = item.ImoAssetItem.InventoryNr + ". " + item.ImoAssetItem.Name,
                    InvValueModif = item.InvValueModif,
                    FiscalDeprecModif = item.FiscalDeprecModif,
                    DurationModif = item.DurationModif,
                    DeprecModif = item.DeprecModif,
                    FiscalValueModif = item.FiscalValueModif,
                    Quantity = item.Quantity
                };

                details.Add(detail);
            }

            ret.Details = details.OrderBy(f => f.ImoAssetItem).ToList();

            ret = ChangeOperation(ret);
            return ret;
        }

        public ImoAssetOperEditDto DetailAccountAsset(ImoAssetOperEditDto operation, int idOrd)
        {
            try
            {
                var imoAssetItem = _imoAssetRepository.GetAllIncluding(f => f.AssetAccount, f => f.AssetAccountInUse, f => f.DepreciationAccount, f => f.ExpenseAccount)
                                                      .Where(f => f.Id == operation.AssetAccountDetails[idOrd - 1].ImoAssetItemId && f.State == State.Active).FirstOrDefault();

                operation.AssetAccountDetails[idOrd - 1].OldAssetAccountId = imoAssetItem.AssetAccountId;
                operation.AssetAccountDetails[idOrd - 1].OldAssetAccountInUseId = imoAssetItem.AssetAccountInUseId;
                operation.AssetAccountDetails[idOrd - 1].OldDepreciationAccountId = imoAssetItem.DepreciationAccountId;
                operation.AssetAccountDetails[idOrd - 1].OldExpenseAccountId = imoAssetItem.ExpenseAccountId;


                return operation;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public void UpateImoAssetAccounts(ImoAssetOperEditDto operation)
        {
            try
            {
                ImoAssetOper oper = ObjectMapper.Map<ImoAssetOper>(operation);
                operation.OperationType = ((ImoAssetOperType)operation.OperationTypeId).ToString();
                oper.AssetsOperType = (ImoAssetOperType)operation.OperationTypeId;
                oper.OperUseStartDate = (operation.AssetsOperType == ImoAssetOperType.Reevaluare) ? LazyMethods.LastDayOfMonth(oper.OperationDate) : oper.OperationDate;

                var appClient = GetCurrentTenant();
                var operDetails = new List<ImoAssetOperDetail>();
                int imoAssetItemId = 0;

                if (operation.AssetAccountDetails != null)
                {
                    foreach (var detail in operation.AssetAccountDetails)
                    {
                        var operDetail = ObjectMapper.Map<ImoAssetOperDetail>(detail);
                        operDetail.ImoAssetOperId = oper.Id;
                        operDetail.TenantId = appClient.Id;
                        imoAssetItemId = operDetail.ImoAssetItemId;
                        operDetails.Add(operDetail);

                        // actualizez ImoAssetItem cu noile conturi
                        var imoAssetItem = _imoAssetRepository.GetAllIncluding(f => f.AssetAccount, f => f.AssetAccountInUse, f => f.DepreciationAccount, f => f.ExpenseAccount)
                                          .Where(f => f.Id == detail.ImoAssetItemId && f.State == State.Active).FirstOrDefault();

                        imoAssetItem.AssetAccountId = detail.NewAssetAccountId;
                        imoAssetItem.AssetAccountInUseId = detail.NewAssetAccountInUseId;
                        imoAssetItem.DepreciationAccountId = detail.NewDepreciationAccountId;
                        imoAssetItem.ExpenseAccountId = detail.NewExpenseAccountId;

                        _imoAssetRepository.Update(imoAssetItem);
                    }
                }

                oper.TenantId = appClient.Id;
                if (oper.Id == 0)
                {
                    oper.OperDetails = operDetails;
                    _imoAssetOperRepository.Insert(oper);
                    CurrentUnitOfWork.SaveChanges();
                }
                else
                {
                    oper.OperDetails = operDetails;
                    _imoAssetOperRepository.UpdateAssetOperation(oper);
                    DeleteGestAsset(oper);
                }

                //generare gestiune
                foreach (var item in operDetails)
                {
                    if ((item.OldAssetAccountId != item.NewAssetAccountId) || (item.OldAssetAccountInUseId != item.NewAssetAccountInUseId) || (item.OldDepreciationAccountId != item.NewDepreciationAccountId) || (item.OldExpenseAccountId != item.NewExpenseAccountId))
                    {
                        _imoAssetOperRepository.GestComputingForAsset(oper.OperationDate, item.ImoAssetItemId);
                    }
                }


                if (oper.AssetsOperType == ImoAssetOperType.ModificareConturiCuInregistrareNotaContabila)
                {
                    var lastBalanceDate = _balanceRepository.GetAll().Where(f => f.Status == State.Active).OrderByDescending(f => f.BalanceDate).FirstOrDefault().BalanceDate;

                    // generez note contabile din gestiune
                    _autoOperationRepository.AutoImoAssetOperationAdd(oper.OperationDate, appClient.Id, appClient.LocalCurrencyId.Value, ImoAssetType.MijlocFix, lastBalanceDate, null);

                }


                operation.FinishAdd = true;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
