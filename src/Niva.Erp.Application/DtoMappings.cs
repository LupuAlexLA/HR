using AutoMapper;
using Niva.Conta.Nomenclatures;
using Niva.Erp.Buget;
using Niva.Erp.Buget.Dto;
using Niva.Erp.Conta.AutoOperation;
using Niva.Erp.Conta.Balance;
using Niva.Erp.Conta.ConfigurareRapoarte.Dto;
using Niva.Erp.Conta.ImoAsset;
using Niva.Erp.Conta.ImoAsset.Dto;
using Niva.Erp.Conta.InvObjects.Dto;
using Niva.Erp.Conta.Lichiditate.Dto;
using Niva.Erp.Conta.Nomenclatures;
using Niva.Erp.Conta.Nomenclatures.Dto;
using Niva.Erp.Conta.Operations;
using Niva.Erp.Conta.Operations.Dto;
using Niva.Erp.Conta.Prepayments;
using Niva.Erp.Conta.RegistruInventar.Dto;
using Niva.Erp.Conta.Reports.Dto;
using Niva.Erp.Conta.SituatiiFinanciare;
using Niva.Erp.Conta.TranzactiiDto.Dto;
using Niva.Erp.Deconturi.Dto;
using Niva.Erp.Economic;
using Niva.Erp.Economic.Dto;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.ExternalApi;
using Niva.Erp.HR.Dto;
using Niva.Erp.Imprumuturi.Dto;
using Niva.Erp.ModelObjects;
using Niva.Erp.Models.AutoOperation;
using Niva.Erp.Models.Buget;
using Niva.Erp.Models.Buget.BVC;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.ConfigurareRapoarte;
using Niva.Erp.Models.Conta.Lichiditate;
using Niva.Erp.Models.Conta.RegistruInventar;
using Niva.Erp.Models.Conta.SituatiiFinanciare;
using Niva.Erp.Models.Contracts;
using Niva.Erp.Models.Deconturi;
using Niva.Erp.Models.Economic.Casierii;
using Niva.Erp.Models.Economic.Casierii.Cupiuri;
using Niva.Erp.Models.Emitenti;
using Niva.Erp.Models.Filedoc;
using Niva.Erp.Models.HR;
using Niva.Erp.Models.ImoAsset;
using Niva.Erp.Models.ImoAssets;
using Niva.Erp.Models.Imprumuturi;
using Niva.Erp.Models.InvObjects;
using Niva.Erp.Models.PrePayments;
using Niva.Erp.Models.SectoareBnr;
using Niva.Erp.SectoareBnr.Dto;
using System;
using System.Linq;

namespace Niva.Erp
{
    class DtoMappings : Profile
    {
        public DtoMappings()
        {


            #region ImoAssets
            CreateMap<ImoAssetItem, ImoAssetListDto>()
                .ForMember(t => t.DocumentType, opts => opts.MapFrom(d => d.DocumentType.TypeName))
                .ForMember(t => t.ClassCode, opts => opts.MapFrom(d => (d.AssetClassCodes.Code + " " + d.AssetClassCodes.Name).Length > 50 ? (d.AssetClassCodes.Code + " " + d.AssetClassCodes.Name).Substring(0, 50) + "..." : d.AssetClassCodes.Code + " " + d.AssetClassCodes.Name))
                .ForMember(t => t.ThirdParty, opts => opts.MapFrom(d => d.ThirdParty.FullName))//(d.ThirdParty != null ? d.ThirdParty.Person.FullName : "")))
                .ForMember(t => t.Invoice, opts => opts.MapFrom(d => (d.PrimDocumentType != null ? d.PrimDocumentType.TypeNameShort : "") + " " + d.PrimDocumentNr + " / "
                            + (d.PrimDocumentDate != null ? (d.PrimDocumentDate ?? DateTime.Now).ToShortDateString() : "")))
                .ForMember(t => t.UseStartDate, opts => opts.MapFrom(d => (d.UseStartDate != null ? d.UseStartDate.Value.ToShortDateString() : "")))
                .ForMember(t => t.DepreciationStartDate, opts => opts.MapFrom(d => (d.DepreciationStartDate != null ? d.DepreciationStartDate.Value.ToShortDateString() : "")));

            CreateMap<ImoAssetStorage, ImoAssetStorageDto>();

            CreateMap<ImoAssetStorageDto, ImoAssetStorage>();

            CreateMap<ImoAssetClassCode, ImoAssetClassCodeListDto>()
                .ForMember(t => t.AssetAccount, opts => opts.MapFrom(d => d.AssetAccount.Symbol.ToString()))
                .ForMember(t => t.DepreciationAccount, opts => opts.MapFrom(d => d.DepreciationAccount.Symbol.ToString()))
                .ForMember(t => t.ExpenseAccount, opts => opts.MapFrom(d => d.ExpenseAccount.Symbol.ToString()))
                .ForMember(t => t.ClassCodeParrent, opts => opts.MapFrom(d => d.ClassCodeParrent.Name.ToString()))
                .ForMember(t => t.Name, opts => opts.MapFrom(d => d.Code + " " + d.Name));

            CreateMap<ImoAssetClassCode, ImoAssetClassCodeListDDDto>()
                .ForMember(t => t.Name, opts => opts.MapFrom(d => d.Code + " " + d.Name));

            CreateMap<ImoAssetClassCodeListDto, ImoAssetClassCode>();

            CreateMap<ImoAssetClassCode, ImoAssetClassCodeEditDto>();

            CreateMap<ImoAssetClassCodeEditDto, ImoAssetClassCode>();

            CreateMap<ImoAssetOperDocType, ImoAssetOperDocTypeListDto>()
                .ForMember(t => t.DocumentType, opts => opts.MapFrom(d => d.DocumentType.TypeNameShort))
                .ForMember(t => t.OperType, opts => opts.MapFrom(d => LazyMethods.EnumValueToDescription(d.OperType)));

            CreateMap<ImoAssetOperDocType, ImoAssetOperDocTypeEditDto>()
                .ForMember(t => t.OperTypeId, opts => opts.MapFrom(d => (int)d.OperType));

            CreateMap<ImoAssetOperDocTypeEditDto, ImoAssetOperDocType>()
                .ForMember(t => t.OperType, opts => opts.MapFrom(d => (ImoAssetOperType)d.OperTypeId));

            CreateMap<ImoAssetOper, ImoAssetOperListDetailDto>()
                .ForMember(t => t.OperationType, opts => opts.MapFrom(d => LazyMethods.EnumValueToDescription(d.AssetsOperType)))
                .ForMember(t => t.OperationTypeId, opts => opts.MapFrom(d => d.AssetsOperType))
                .ForMember(t => t.DocumentType, opts => opts.MapFrom(d => d.DocumentType.TypeName));


            CreateMap<ImoAssetOperEditDto, ImoAssetOper>()
                 .ForMember(t => t.AssetsStoreIn, opts => opts.Ignore())
                 .ForMember(t => t.AssetsStoreOut, opts => opts.Ignore())
                 .ForMember(t => t.DocumentType, opts => opts.Ignore())
                 .ForMember(t => t.OperUseStartDate, opts => opts.Ignore());

            CreateMap<ImoAssetOperDetailEditDto, ImoAssetOperDetail>()
                 .ForMember(t => t.ImoAssetItem, opts => opts.Ignore())
                 .ForMember(t => t.InvoiceDetail, opts => opts.Ignore())
                 .ForMember(t => t.ImoAssetOperId, opts => opts.MapFrom(d => d.Id));

            CreateMap<ImoAssetOperDetailEditModifAccountDto, ImoAssetOperDetail>()
                 .ForMember(t => t.ImoAssetItem, opts => opts.Ignore())
                 .ForMember(t => t.InvoiceDetail, opts => opts.Ignore())
                 .ForMember(t => t.ImoAssetOperId, opts => opts.MapFrom(d => d.Id))
                 .ForMember(t => t.OldAssetAccountInUseId, opts => opts.MapFrom(d => d.OldAssetAccountInUseId.Value))
                 .ForMember(t => t.OldDepreciationAccountId, opts => opts.MapFrom(d => d.OldDepreciationAccountId.Value))
                 .ForMember(t => t.OldExpenseAccountId, opts => opts.MapFrom(d => d.OldExpenseAccountId.Value))
                 .ForMember(t => t.NewAssetAccountId, opts => opts.MapFrom(d => d.NewAssetAccountId.Value))
                 .ForMember(t => t.NewAssetAccountInUseId, opts => opts.MapFrom(d => d.NewAssetAccountInUseId.Value))
                 .ForMember(t => t.NewDepreciationAccountId, opts => opts.MapFrom(d => d.NewDepreciationAccountId.Value))
                 .ForMember(t => t.OldAssetAccountId, opts => opts.MapFrom(d => d.OldAssetAccountId.Value))
                 .ForMember(t => t.NewExpenseAccountId, opts => opts.MapFrom(d => d.NewExpenseAccountId.Value));

            CreateMap<ImoAssetOper, ImoAssetOperEditDto>()
                 .ForMember(t => t.AssetsStoreIn, opts => opts.MapFrom(d => d.AssetsStoreIn.StorageName))
                 .ForMember(t => t.AssetsStoreOut, opts => opts.MapFrom(d => d.AssetsStoreOut.StorageName))
                 .ForMember(t => t.DocumentType, opts => opts.MapFrom(d => d.DocumentType.TypeName))
                 .ForMember(t => t.OperationType, opts => opts.MapFrom(d => LazyMethods.EnumValueToDescription(d.AssetsOperType)))
                 .ForMember(t => t.OperationTypeId, opts => opts.MapFrom(d => (int)d.AssetsOperType))
                 .ForMember(t => t.PersonStoreInName, opts => opts.MapFrom(d => d.PersonStoreIn.FullName.ToString()))
                 .ForMember(t => t.PersonStoreOutName, opts => opts.MapFrom(d => d.PersonStoreOut.FullName.ToString()));

            CreateMap<ImoAssetOperDetail, ImoAssetOperDetailEditDto>()
                 .ForMember(t => t.ImoAssetItem, opts => opts.MapFrom(d => d.ImoAssetItem.InventoryNr + ". " + d.ImoAssetItem.Name));

            CreateMap<ImoAssetOperDetail, ImoAssetOperDetailEditModifAccountDto>()
                  .ForMember(t => t.ImoAssetItem, opts => opts.MapFrom(d => d.ImoAssetItem.InventoryNr + ". " + d.ImoAssetItem.Name))
                  .ForMember(t => t.OldAssetAccountInUseId, opts => opts.MapFrom(d => d.OldAssetAccountInUseId.Value))
                 .ForMember(t => t.OldDepreciationAccountId, opts => opts.MapFrom(d => d.OldDepreciationAccountId.Value))
                 .ForMember(t => t.OldExpenseAccountId, opts => opts.MapFrom(d => d.OldExpenseAccountId.Value))
                 .ForMember(t => t.NewAssetAccountId, opts => opts.MapFrom(d => d.NewAssetAccountId.Value))
                 .ForMember(t => t.NewAssetAccountInUseId, opts => opts.MapFrom(d => d.NewAssetAccountInUseId.Value))
                 .ForMember(t => t.NewDepreciationAccountId, opts => opts.MapFrom(d => d.NewDepreciationAccountId.Value))
                 .ForMember(t => t.OldAssetAccountId, opts => opts.MapFrom(d => d.OldAssetAccountId.Value))
                 .ForMember(t => t.NewExpenseAccountId, opts => opts.MapFrom(d => d.NewExpenseAccountId.Value))
                 .ForMember(t => t.OldAssetAccount, opts => opts.MapFrom(d => d.OldAssetAccount.AccountName))
                 .ForMember(t => t.OldAssetAccountInUse, opts => opts.MapFrom(d => d.OldAssetAccountInUse.AccountName))
                 .ForMember(t => t.OldDepreciationAccount, opts => opts.MapFrom(d => d.OldDepreciationAccount.AccountName))
                 .ForMember(t => t.OldExpenseAccount, opts => opts.MapFrom(d => d.OldExpenseAccount.AccountName))
                 .ForMember(t => t.NewAssetAccount, opts => opts.MapFrom(d => d.NewAssetAccount.AccountName))
                 .ForMember(t => t.NewAssetAccountInUse, opts => opts.MapFrom(d => d.NewAssetAccountInUse.AccountName))
                 .ForMember(t => t.NewDepreciationAccount, opts => opts.MapFrom(d => d.NewDepreciationAccount.AccountName))
                 .ForMember(t => t.NewExpenseAccount, opts => opts.MapFrom(d => d.NewExpenseAccount.AccountName));

            CreateMap<ImoAssetOperDetailEditModifAccountDto, ImoAssetOperDetail>()
                 .ForMember(t => t.ImoAssetItem, opts => opts.Ignore())
                 .ForMember(t => t.OldAssetAccount, opts => opts.Ignore())
                 .ForMember(t => t.OldAssetAccountInUse, opts => opts.Ignore())
                 .ForMember(t => t.OldDepreciationAccount, opts => opts.Ignore())
                 .ForMember(t => t.OldExpenseAccount, opts => opts.Ignore())
                 .ForMember(t => t.NewAssetAccount, opts => opts.Ignore())
                 .ForMember(t => t.NewAssetAccountInUse, opts => opts.Ignore())
                 .ForMember(t => t.NewDepreciationAccount, opts => opts.Ignore())
                 .ForMember(t => t.NewExpenseAccount, opts => opts.Ignore())
                 .ForMember(t => t.OldAssetAccountInUseId, opts => opts.MapFrom(d => d.OldAssetAccountInUseId.Value))
                 .ForMember(t => t.OldDepreciationAccountId, opts => opts.MapFrom(d => d.OldDepreciationAccountId.Value))
                 .ForMember(t => t.OldExpenseAccountId, opts => opts.MapFrom(d => d.OldExpenseAccountId.Value))
                 .ForMember(t => t.NewAssetAccountId, opts => opts.MapFrom(d => d.NewAssetAccountId.Value))
                 .ForMember(t => t.NewAssetAccountInUseId, opts => opts.MapFrom(d => d.NewAssetAccountInUseId.Value))
                 .ForMember(t => t.NewDepreciationAccountId, opts => opts.MapFrom(d => d.NewDepreciationAccountId.Value))
                 .ForMember(t => t.OldAssetAccountId, opts => opts.MapFrom(d => d.OldAssetAccountId.Value))
                 .ForMember(t => t.NewExpenseAccountId, opts => opts.MapFrom(d => d.NewExpenseAccountId.Value));

            CreateMap<ImoAssetSetup, ImoAssetSetupDto>();

            CreateMap<ImoAssetSetupDto, ImoAssetSetup>();

            CreateMap<ImoAssetStock, ImoGestRowDto>();

            CreateMap<ImoInventariereDetailDto, ImoAssetStock>();
            CreateMap<ImoAssetStock, ImoInventariereDetailDto>()
                .ForMember(t => t.Description, opts => opts.MapFrom(d => d.ImoAssetItem.Name));

            CreateMap<ImoInventariereInitDto, ImoInventariere>()
                .ForMember(t => t.DataInventariere, opts => opts.MapFrom(d => d.DateStart))
                .ForMember(t => t.ImoInventariereDetails, opts => opts.Ignore());
            CreateMap<ImoInventariere, ImoInventariereInitDto>();

            CreateMap<ImoInventariereListDto, ImoInventariere>()
                .ForMember(f => f.DataInventariere, opts => opts.MapFrom(d => d.DateStart));
            CreateMap<ImoInventariere, ImoInventariereListDto>();

            CreateMap<ImoInventariereDetailDto, ImoInventariereDet>();
            CreateMap<ImoInventariereDet, ImoInventariereDetailDto>()
                .ForMember(t => t.Description, opts => opts.MapFrom(d => d.ImoAssetItem.Name))
                .ForMember(t => t.InventoryNumber, opts => opts.MapFrom(d => d.ImoAssetItem.InventoryNr))
                .ForMember(t => t.StockScriptic, opts => opts.MapFrom(d => d.ImoAssetStock.Quantity))
                .ForMember(t => t.StorageIn, opts => opts.MapFrom(d => d.ImoAssetStock.Storage.StorageName))
                .ForMember(t => t.InventoryValue, opts => opts.MapFrom(d => (d.ImoAssetStock.InventoryValue + d.ImoAssetStock.Deprec)))
                .ForMember(t => t.RemainingValue, opts => opts.MapFrom(d => d.ImoAssetStock.InventoryValue))
                 .ForMember(t => t.UseStartDate, opts => opts.MapFrom(d => d.ImoAssetItem.UseStartDate));


            CreateMap<ImoInventariereEditDto, ImoInventariere>()
                 .ForMember(t => t.DataInventariere, opts => opts.MapFrom(d => d.DateStart));
            CreateMap<ImoInventariere, ImoInventariereEditDto>()
                .ForMember(t => t.DateStart, opts => opts.MapFrom(d => d.DataInventariere));
            #endregion

            #region Prepayments
            CreateMap<Prepayment, PrepaymentsListDto>()
                .ForMember(t => t.ThirdParty, opts => opts.MapFrom(d => d.ThirdParty.FullName))//(d.ThirdParty != null ? d.ThirdParty.Person.FullName : "")))
                .ForMember(t => t.Invoice, opts => opts.MapFrom(d => (d.PrimDocumentType != null ? d.PrimDocumentType.TypeNameShort : "") + " " + d.PrimDocumentNr + " / "
                            + (d.PrimDocumentDate != null ? (d.PrimDocumentDate ?? DateTime.Now).ToShortDateString() : "")));

            CreateMap<PrepaymentDocType, PrepaymentsOperDocTypeListDto>()
                .ForMember(t => t.DocumentType, opts => opts.MapFrom(d => d.DocumentType.TypeNameShort))
                .ForMember(t => t.OperType, opts => opts.MapFrom(d => LazyMethods.EnumValueToDescription(d.OperType)));

            CreateMap<PrepaymentDocType, PrepaymentsOperDocTypeEditDto>()
                .ForMember(t => t.OperTypeId, opts => opts.MapFrom(d => (int)d.OperType));

            CreateMap<PrepaymentsOperDocTypeEditDto, PrepaymentDocType>()
                .ForMember(t => t.OperType, opts => opts.MapFrom(d => (PrepaymentOperType)d.OperTypeId));

            CreateMap<PrepaymentsDurationSetup, PrepaymentsDurationSetupDto>()
                .ForMember(t => t.PrepaymentDurationCalcId, opts => opts.MapFrom(d => d.PrepaymentDurationCalc))
                .ForMember(t => t.PrepaymentTypeId, opts => opts.MapFrom(d => d.PrepaymentType));

            CreateMap<PrepaymentsDurationSetupDto, PrepaymentsDurationSetup>()
                .ForMember(t => t.PrepaymentDurationCalc, opts => opts.MapFrom(d => (PrepaymentDurationCalc)d.PrepaymentDurationCalcId))
                .ForMember(t => t.PrepaymentType, opts => opts.MapFrom(d => (PrepaymentType)d.PrepaymentTypeId));

            CreateMap<PrepaymentsDecDeprecSetup, PrepaymentsDecDeprecSetupDto>()
                .ForMember(t => t.PrepaymentTypeId, opts => opts.MapFrom(d => d.PrepaymentType));

            CreateMap<PrepaymentsDecDeprecSetupDto, PrepaymentsDecDeprecSetup>()
                .ForMember(t => t.PrepaymentType, opts => opts.MapFrom(d => (PrepaymentType)d.PrepaymentTypeId));

            #endregion

            #region InvObjects
            CreateMap<InvObjectItem, InvObjectAddDirectDto>()
                .ForMember(t => t.InvCategoryId, opts => opts.MapFrom(d => d.InvCateg));
            CreateMap<InvObjectAddDirectDto, InvObjectItem>();

            CreateMap<InvObjectItem, InvObjectListDto>()
                .ForMember(t => t.ThirdParty, opts => opts.MapFrom(d => d.ThirdParty.FullName))
                .ForMember(t => t.Invoice, opts => opts.MapFrom(d => (d.PrimDocumentType != null ? d.PrimDocumentType.TypeNameShort : "") + " " + d.PrimDocumentNr + " / "
                            + (d.PrimDocumentDate != null ? (d.PrimDocumentDate ?? DateTime.Now).ToShortDateString() : "")))
                .ForMember(t => t.CategoryName, opts => opts.MapFrom(d => d.InvCateg.Name.ToString()));

            CreateMap<InvObjectListDto, InvObjectItem>()
                .ForMember(t => t.InvCategId, opts => opts.MapFrom(d => d.CategoryId));

            CreateMap<InvStorage, InvObjectStorageDto>();
            CreateMap<InvObjectStorageDto, InvStorage>();

            CreateMap<InvCateg, InvObjectCategoryListDto>()
                .ForMember(f => f.CategoryName, opts => opts.MapFrom(d => d.Name.ToString()));
            CreateMap<InvObjectCategoryListDto, InvCateg>()
                .ForMember(f => f.Name, opts => opts.MapFrom(d => d.CategoryName));
            CreateMap<InvCateg, InvObjectCategoryEditDto>()
                .ForMember(f => f.CategoryName, opts => opts.MapFrom(d => d.Name));
            CreateMap<InvObjectCategoryEditDto, InvCateg>()
                .ForMember(f => f.Name, opts => opts.MapFrom(d => d.CategoryName));

            CreateMap<InvObjectOperDocTypeEditDto, InvObjectOperDocType>()
                .ForMember(t => t.OperType, opts => opts.MapFrom(d => (InvObjectOperType)d.OperTypeId));

            CreateMap<InvObjectOperDocType, InvObjectOperDocTypeEditDto>()
                 .ForMember(t => t.OperTypeId, opts => opts.MapFrom(d => (int)d.OperType));

            CreateMap<InvObjectOperDocType, InvObjectOperDocTypeDto>()
                .ForMember(t => t.DocumentType, opts => opts.MapFrom(d => d.DocumentType.TypeNameShort))
                .ForMember(t => t.OperType, opts => opts.MapFrom(d => LazyMethods.EnumValueToDescription(d.OperType)));

            CreateMap<InvObjectOperDocTypeDto, InvObjectOperDocType>();

            CreateMap<InvObjectOper, InvObjectOperListDetailDto>()
                .ForMember(t => t.OperationType, opts => opts.MapFrom(d => LazyMethods.EnumValueToDescription(d.InvObjectsOperType)))
                .ForMember(t => t.OperationTypeId, opts => opts.MapFrom(d => d.InvObjectsOperType))
                .ForMember(t => t.DocumentType, opts => opts.MapFrom(d => d.DocumentType.TypeName));


            CreateMap<InvObjectOperEditDto, InvObjectOper>()
                 .ForMember(t => t.InvObjectsStoreIn, opts => opts.Ignore())
                 .ForMember(t => t.InvObjectsStoreOut, opts => opts.Ignore())
                 .ForMember(t => t.DocumentType, opts => opts.Ignore())
                 .ForMember(t => t.Invoice, opts => opts.Ignore())
                 .ForMember(t => t.InvObjectsOperType, opts => opts.MapFrom(d => d.OperationType.ToString()));

            CreateMap<InvObjectOperDetailEditDto, InvObjectOperDetail>()
                 .ForMember(t => t.InvObjectItem, opts => opts.Ignore())
                 .ForMember(t => t.InvoiceDetail, opts => opts.Ignore())
                 .ForMember(t => t.InvObjectOperId, opts => opts.MapFrom(d => d.Id));

            CreateMap<InvObjectOper, InvObjectOperEditDto>()
                 .ForMember(t => t.InvObjectsStoreIn, opts => opts.MapFrom(d => d.InvObjectsStoreIn.StorageName))
                 .ForMember(t => t.InvObjectsStoreOut, opts => opts.MapFrom(d => d.InvObjectsStoreOut.StorageName))
                 .ForMember(t => t.DocumentType, opts => opts.MapFrom(d => d.DocumentType.TypeName))
                 .ForMember(t => t.OperationType, opts => opts.MapFrom(d => LazyMethods.EnumValueToDescription(d.InvObjectsOperType)))
                 .ForMember(t => t.OperationTypeId, opts => opts.MapFrom(d => (int)d.InvObjectsOperType))
                 .ForMember(t => t.PersonStoreInName, opts => opts.MapFrom(d => d.PersonStoreIn.FullName))
                 .ForMember(t => t.PersonStoreOutName, opts => opts.MapFrom(d => d.PersonStoreOut.FullName));

            CreateMap<InvObjectOperDetail, InvObjectOperDetailEditDto>()
                 .ForMember(t => t.InvObjectItem, opts => opts.MapFrom(d => d.InvObjectItem.InventoryNr + ". " + d.InvObjectItem.Name));

            CreateMap<InvObjectOperDetailEditDto, InvObjectOperDetail>()
                     .ForMember(t => t.InvObjectItem, opts => opts.Ignore())
                 .ForMember(t => t.InvoiceDetail, opts => opts.Ignore())
                 .ForMember(t => t.InvObjectOperId, opts => opts.MapFrom(d => d.Id));

            CreateMap<InvObjectInventariereListDto, InvObjectInventariere>();
            CreateMap<InvObjectInventariere, InvObjectInventariereListDto>();

            CreateMap<InvObjectInventariereDetDto, InvObjectInventariereDet>();
            CreateMap<InvObjectInventariereDet, InvObjectInventariereDetDto>()
                .ForMember(t => t.Description, opts => opts.MapFrom(d => d.InvObjectItem.Name))
                .ForMember(t => t.InventoryNumber, opts => opts.MapFrom(d => d.InvObjectItem.InventoryNr))
                .ForMember(t => t.StockScriptic, opts => opts.MapFrom(d => d.InvObjectStock.Quantity))
                .ForMember(t => t.StorageIn, opts => opts.MapFrom(d => d.InvObjectStock.Storage.StorageName))
                .ForMember(t => t.InventoryValue, opts => opts.MapFrom(d => (d.InvObjectStock.InventoryValue + d.InvObjectStock.Deprec)))
                .ForMember(t => t.OperationDate, opts => opts.MapFrom(d => d.InvObjectItem.OperationDate));

            CreateMap<InvObjectInventariereEditDto, InvObjectInventariere>();
            CreateMap<InvObjectInventariere, InvObjectInventariereEditDto>();



            //CreateMap<InvStorage, InvStorageListDto>()
            //    .ForMember(t => t.InvAccount, opts => opts.MapFrom(d => d.InvAccount.Symbol))
            //    .ForMember(t => t.ExpenseAccount, opts => opts.MapFrom(d => d.ExpenseAccount.Symbol))
            //    .ForMember(t => t.ExtraAccount, opts => opts.MapFrom(d => d.ExtraAccount.Symbol));

            //CreateMap<InvStorageEditDto, InvStorage>();

            //CreateMap<Account, AccountListDDDto>()
            //    .ForMember(t => t.Name, opts => opts.MapFrom(d => d.Symbol + " - " + d.AccountName));

            //CreateMap<InvCategDto, InvCateg>();

            //CreateMap<InvCateg, InvCategDto>();

            //CreateMap<InvUMDto, InvUM>();

            //CreateMap<InvUM, InvUMDto>();

            //CreateMap<InvProduct, InvProductListDto>()
            //    .ForMember(t => t.InvCateg, opts => opts.MapFrom(d => d.InvCateg.Name))
            //    .ForMember(t => t.InvUM, opts => opts.MapFrom(d => d.InvUM.Name));

            //CreateMap<InvGestObjects, InvProductGestDetailsDto>()
            //    .ForMember(t => t.InvProduct, opts => opts.MapFrom(d => d.InvProduct.Name.ToString()))
            //    .ForMember(t => t.InvStorage, opts => opts.MapFrom(d => d.InvStorage.Name.ToString()))
            //    .ForMember(t => t.InvOperation, opts => opts.MapFrom(d => d.InvOperation.DocumentNr.ToString()));

            //CreateMap<InvProductEditDto, InvProduct>();

            //CreateMap<InvOperationDocType, InvOperationDocTypeListDto>()
            //    .ForMember(t => t.DocumentType, opts => opts.MapFrom(d => d.DocumentType.TypeNameShort))
            //    .ForMember(t => t.OperationType, opts => opts.MapFrom(d => d.OperationType.ToString()));

            //CreateMap<InvOperationDocType, InvOperationDocTypeEditDto>()
            //    .ForMember(t => t.OperationTypeId, opts => opts.MapFrom(d => (int)d.OperationType));

            //CreateMap<InvOperationDocTypeEditDto, InvOperationDocType>()
            //    .ForMember(t => t.OperationType, opts => opts.MapFrom(d => (InvOperationType)d.OperationTypeId));


            //CreateMap<InvOperation, InvNIRListDto>()
            //   .ForMember(t => t.OperationType, opts => opts.MapFrom(d => d.OperationType.ToString()))
            //   .ForMember(t => t.StorageIn, opts => opts.MapFrom(d => d.StorageIn.Name))
            //   .ForMember(t => t.ThirdParty, opts => opts.MapFrom(d => d.Invoice.ThirdParty.Person.FullName))
            //   .ForMember(t => t.InvoiceNr, opts => opts.MapFrom(d => d.Invoice.InvoiceNumber))
            //   .ForMember(t => t.InvoiceDate, opts => opts.MapFrom(d => d.Invoice.InvoiceDate));

            //CreateMap<InvOperation, InvOperationListDto>()
            //   .ForMember(t => t.OperationType, opts => opts.MapFrom(d => d.OperationType.ToString()))
            //   .ForMember(t => t.StorageIn, opts => opts.MapFrom(d => d.StorageIn.Name))
            //   .ForMember(t => t.StorageOut, opts => opts.MapFrom(d => d.StorageOut.Name))
            //   .ForMember(t => t.DocumentType, opts => opts.MapFrom(d => d.DocumentType.TypeName));

            #endregion

            #region ContaNomenclatoare
            CreateMap<Account, AccountListDto>()
                 .ForMember(t => t.SyntheticAccount, opts => opts.MapFrom(d => (d.SyntheticAccount == null ? null : d.SyntheticAccount.Symbol)))
                 .ForMember(t => t.AccountType, opts => opts.MapFrom(d => d.AccountTypes.ToString()))
                 .ForMember(t => t.Currency, opts => opts.MapFrom(d => d.Currency.CurrencyName))
                 .ForMember(t => t.ThirdParty, opts => opts.MapFrom(d => d.ThirdParty.FullName))
                 .ForMember(t => t.ActivityType, opts => opts.MapFrom(d => d.ActivityType.ActivityName.ToString()))
                 .ForMember(t => t.AccountFuncTypeName, opts => opts.MapFrom(d => LazyMethods.EnumValueToDescription(d.AccountFuncType)))
                 .ForMember(t => t.DenumireSector, opts => opts.MapFrom(d => d.BNR_Sector.Denumire));

            CreateMap<Account, AccountEditDto>()
                 .ForMember(t => t.AccountType, opts => opts.MapFrom(d => d.AccountTypes))
                 .ForMember(t => t.CurrencyId, opts => opts.MapFrom(d => d.CurrencyId))
                 .ForMember(t => t.AccountFuncType, opts => opts.MapFrom(d => d.AccountFuncType))
                 .ForMember(t => t.TaxStatus, opts => opts.MapFrom(d => d.TaxStatus))
                 .ForMember(t => t.ActivityTypeId, opts => opts.MapFrom(d => d.ActivityTypeId))
                 .ForMember(t => t.SyntheticAccount, opts => opts.MapFrom(d => (d.SyntheticAccount == null ? null : d.SyntheticAccount.Symbol)));

            CreateMap<AccountEditDto, Account>()
                 .ForMember(t => t.AccountTypes, opts => opts.MapFrom(d => d.AccountType))
                 .ForMember(t => t.CurrencyId, opts => opts.MapFrom(d => d.CurrencyId))
                 .ForMember(t => t.ThirdPartyId, opts => opts.MapFrom(d => d.ThirdPartyId))
                 .ForMember(t => t.AccountFuncType, opts => opts.MapFrom(d => d.AccountFuncType))
                 .ForMember(t => t.TaxStatus, opts => opts.MapFrom(d => d.TaxStatus))
                 .ForMember(t => t.ActivityTypeId, opts => opts.MapFrom(d => d.ActivityTypeId))
                 .ForMember(t => t.SyntheticAccountId, opts => opts.Ignore())
                 .ForMember(t => t.SyntheticAccount, opts => opts.Ignore());

            CreateMap<AccountDivConfig, AccountDivConfigDto>()
                .ForMember(t => t.AccountTypeStr, opts => opts.MapFrom(d => d.AccountType.ToString()))
                .ForMember(t => t.PersTypeStr, opts => opts.MapFrom(d => d.PersType.ToString()))
                .ForMember(t => t.ResidenceTypeStr, opts => opts.MapFrom(d => d.ResidenceType))
                .ForMember(t => t.AccountName, opts => opts.MapFrom(d => d.Account.Symbol + " - " + d.Account.AccountName));

            CreateMap<AccountDivConfigDto, AccountDivConfig>();

            CreateMap<AccountConfig, AccountConfigDto>()
                .ForMember(t => t.ImoAssetStorageId, opts => opts.MapFrom(d => d.ImoAssetStorageId))
                .ForMember(t => t.ActivityTypeId, opts => opts.MapFrom(d => d.ActivityTypeId))
                .ForMember(t => t.ActivityType, opts => opts.MapFrom(d => (d.ActivityTypeId == null ? null : d.ActivityType.ActivityName)));

            CreateMap<AccountConfigDto, AccountConfig>()
                .ForMember(t => t.ImoAssetStorage, opts => opts.Ignore())
                .ForMember(t => t.ActivityType, opts => opts.Ignore());

            CreateMap<AccountRelation, AccountRelationDto>();

            CreateMap<AccountRelationDto, AccountRelation>();

            CreateMap<Account, AccountListDDDto>()
            .ForMember(t => t.Name, opts => opts.MapFrom(d => d.Symbol + " - " + d.AccountName))
            .ForMember(t => t.CurrencyId, opts => opts.MapFrom(d => d.CurrencyId));

            //CreateMap<AccountTaxProperty, Account>();

            CreateMap<DocumentTypeListDDDto, DocumentType>();

            CreateMap<DocumentType, DocumentTypeListDDDto>()
                 .ForMember(t => t.Name, opts => opts.MapFrom(d => d.TypeName));

            CreateMap<DocumentTypeEditDto, DocumentType>()
                .ForMember(t => t.TenantId, opts => opts.MapFrom(d => d.AppClientId));
            CreateMap<DocumentType, DocumentTypeEditDto>()
                .ForMember(t => t.AppClientId, opts => opts.MapFrom(d => d.TenantId));

            CreateMap<AutoOperationConfigDto, AutoOperationConfig>();
            //.ForMember(t => t.DocumentType, opts => opts.Ignore());

            //CreateMap<AutoOperationConfig, AutoOperationConfigDto>()
            //    .ForMember(t => t.DocumentTypeId, opts => opts.MapFrom(d => d.DocumentType.Id));

            CreateMap<AutoOperationConfig, AutoOperationConfigDetailsDto>()
                .ForMember(t => t.AutoOperType, opts => opts.MapFrom(d => (int)d.AutoOperType));
            // .ForMember(t => t.DocumentTypeId, opts => opts.MapFrom(d => d.DocumentType.Id));

            CreateMap<AutoOperationConfigDetailsDto, AutoOperationConfig>()
                .ForMember(t => t.AutoOperType, opts => opts.MapFrom(d => (AutoOperationType)d.AutoOperType));
            //    .ForMember(t => t.DocumentTypeId, opts => opts.MapFrom(d => d.DocumentTypeId));

            CreateMap<AutoOperationSearchConfig, AutoOperationConfigDetailsDto>()
                .ForMember(t => t.AutoOperSearchConfigId, opts => opts.MapFrom(d => d.Id));
            CreateMap<AutoOperationConfigDetailsDto, AutoOperationSearchConfig>();

            CreateMap<AutoOperationConfigDto, AutoOperationSearchConfig>();
            CreateMap<AutoOperationSearchConfig, AutoOperationConfigDto>()
                .ForMember(t => t.AutoOperSearchConfigId, opts => opts.MapFrom(d => d.Id));


            CreateMap<AutoOperationCompute, AutoOperationComputeDto>();

            CreateMap<AutoOperationComputeDto, AutoOperationCompute>();

            CreateMap<OperationTypesEditDto, OperationTypes>();
            CreateMap<OperationTypes, OperationTypesEditDto>();

            CreateMap<OperationTypesListDto, OperationTypes>();
            CreateMap<OperationTypes, OperationTypesListDto>();

            #endregion

            #region Decont
            CreateMap<Decont, DecontListDto>()
                .ForMember(t => t.ThirdParty, opts => opts.MapFrom(d => d.ThirdParty.FullName.ToString()))
                .ForMember(t => t.DecontType, opts => opts.MapFrom(d => LazyMethods.EnumValueToDescription(d.DecontType)))
                .ForMember(t => t.DocumentName, opts => opts.MapFrom(d => d.DocumentType.TypeName.ToString()))
                .ForMember(t => t.ScopDeplasareTypeName, opts => opts.MapFrom(d => LazyMethods.EnumValueToDescription(d.ScopDeplasareType)));
            CreateMap<DecontListDto, Decont>();

            CreateMap<Decont, DecontEditDto>()
                .ForMember(t => t.ThirdPartyName, opts => opts.MapFrom(d => d.ThirdParty.FullName.ToString()))
                .ForMember(t => t.DecontTypeId, opts => opts.MapFrom(d => (int)d.DecontType))
                .ForMember(t => t.DiurnaLegala, opts => opts.MapFrom(d => d.DiurnaLegalaValue))
                .ForMember(t => t.ScopDeplasareTypeId, opts => opts.MapFrom(d => (int)d.ScopDeplasareType))
                .ForMember(t => t.DocumentType, opts => opts.MapFrom(d => d.DocumentType.TypeNameShort.ToString()));
            CreateMap<DecontEditDto, Decont>();

            CreateMap<DiurnaEditDto, DiurnaLegala>()
                .ForMember(t => t.DiurnaType, opts => opts.MapFrom(d => d.DiurnaTypeId));

            CreateMap<DiurnaLegala, DiurnaEditDto>()
                .ForMember(t => t.DiurnaTypeId, opts => opts.MapFrom(d => (int)d.DiurnaType))
                .ForMember(t => t.CountryId, opts => opts.MapFrom(d => d.Country.Id));

            CreateMap<DiurnaLegala, DiurnaListDto>()
                .ForMember(t => t.CountryName, opts => opts.MapFrom(d => d.Country.CountryName))
                .ForMember(t => t.CurrencyName, opts => opts.MapFrom(d => d.Currency.CurrencyName))
                .ForMember(t => t.DiurnaTypeName, opts => opts.MapFrom(d => d.DiurnaType.ToString()));

            CreateMap<DiurnaEditDto, DiurnaZi>()
                  .ForMember(t => t.DiurnaType, opts => opts.MapFrom(d => d.DiurnaTypeId));
            CreateMap<DiurnaZi, DiurnaEditDto>()
                .ForMember(t => t.DiurnaTypeId, opts => opts.MapFrom(d => (int)d.DiurnaType));

            CreateMap<DiurnaZi, DiurnaListDto>()
                .ForMember(t => t.CountryName, opts => opts.MapFrom(d => d.Country.CountryName))
                .ForMember(t => t.CurrencyName, opts => opts.MapFrom(d => d.Currency.CurrencyName))
                .ForMember(t => t.DiurnaTypeName, opts => opts.MapFrom(d => d.DiurnaType.ToString()));
            #endregion

            #region ContaBalance
            CreateMap<BalanceDetailsView, BalanceDetailDto>();
            //.ForMember(t => t, opts => opts.MapFrom(d => d.AccountType));

            CreateMap<SavedBalanceViewDet, BalanceDetailsView>()
                .ForMember(t => t.Symbol, opts => opts.MapFrom(d => d.Cont))
                .ForMember(t => t.Name, opts => opts.MapFrom(d => d.Denumire))
                .ForMember(t => t.DbValueSum, opts => opts.MapFrom(d => d.DbValueI + d.DbValueM + d.DbValueP))
                .ForMember(t => t.CrValueSum, opts => opts.MapFrom(d => d.CrValueI + d.CrValueM + d.CrValueP));

            CreateMap<SavedBalanceDetails, SavedBalanceItemDetailsDto>()
                .ForMember(t => t.AccountName, opts => opts.MapFrom(d => d.Account.AccountName))
                .ForMember(t => t.Symbol, opts => opts.MapFrom(d => d.Account.Symbol));

            CreateMap<BalanceDetailDto, SavedBalanceViewDet>();
            CreateMap<SavedBalanceViewDet, BalanceDetailDto>()
                .ForMember(t => t.Symbol, opts => opts.MapFrom(d => d.Cont))
                .ForMember(t => t.Name, opts => opts.MapFrom(d => d.Denumire));


            CreateMap<BalanceCompSummary, BalanceCompSummaryDto>();
            CreateMap<BalanceCompValid, BalanceCompValidDto>();
            #endregion

            #region Operations
            CreateMap<Operation, OperationDTO>()
                .ForMember(t => t.CurrencyName, opts => opts.MapFrom(d => d.Currency.CurrencyName));

            CreateMap<OperationDTO, Operation>();

            CreateMap<OperationDetails, OperationDetailsDTO>();

            CreateMap<OperationDetailsDTO, OperationDetails>()
                 .ForMember(t => t.CreditValueDeduct, opts => opts.Ignore())
                 .ForMember(t => t.DebitValueDeduct, opts => opts.Ignore());

            CreateMap<Operation, OperationEditDto>()
                .ForMember(t => t.LocalCurrencyId, opts => opts.Ignore())
                .ForMember(t => t.ExchangeRate, opts => opts.Ignore());

            CreateMap<OperationEditDto, Operation>();

            CreateMap<OperationDetails, OperationEditDetailsDto>()
                .ForMember(t => t.IdOrd, opts => opts.Ignore())
                .ForMember(t => t.CreditName, opts => opts.MapFrom(d => d.Credit.Symbol + " " + d.Credit.AccountName))
                .ForMember(t => t.DebitName, opts => opts.MapFrom(d => d.Debit.Symbol + " " + d.Debit.AccountName));

            CreateMap<OperationEditDetailsDto, OperationDetails>()
                .ForMember(t => t.Details, opts => opts.MapFrom(d => d.Details));

            CreateMap<ForeignOperation, ForeignOperationDto>();
            CreateMap<ForeignOperationDto, ForeignOperation>();

            CreateMap<ForeignOperationList, ForeignOperationsDetails>();
            CreateMap<ForeignOperationsDetails, ForeignOperationList>();

            CreateMap<ForeignOperationDictionary, FODictionaryEditDto>()
                .ForMember(t => t.AccountName, opts => opts.MapFrom(d => d.Account.Symbol.ToString()));
            CreateMap<FODictionaryEditDto, ForeignOperationDictionary>();

            CreateMap<ForeignOperationDictionary, FODictionaryListDto>()
                .ForMember(t => t.AccountName, opts => opts.MapFrom(d => d.Account.Symbol.ToString()))
                .ForMember(t => t.DictionaryType, opts => opts.MapFrom(d => d.FODictionaryType.ToString()));
            CreateMap<FODictionaryListDto, ForeignOperationDictionary>();

            CreateMap<OperationDefinition, OperationDefinitionDto>()
                .ForMember(t => t.OperationDetails, opts => opts.MapFrom(d => d.OperationDefinitionDetails));
            CreateMap<OperationDefinitionDto, OperationDefinition>()
                .ForMember(t => t.OperationDefinitionDetails, opts => opts.MapFrom(d => d.OperationDetails));
              
            CreateMap<OperationDefinitionDetails, OperationDefinitionDetailsDto>()
                .ForMember(t => t.CreditName, opts => opts.MapFrom(d => d.Credit.Symbol + " " + d.Credit.AccountName))
                .ForMember(t => t.DebitName, opts => opts.MapFrom(d => d.Debit.Symbol + " " + d.Debit.AccountName));

            CreateMap<OperationDefinitionDetailsDto, OperationDefinitionDetails>();

            #endregion Operations

            #region TaxProfit
            //CreateMap<TaxProfitConfigDet, TaxProfitConfigDetDto>()
            //    .BeforeMap((s, d) => d.Deleted = false);

            //CreateMap<TaxProfitComp, TaxProfitCompListDto>()
            //    .ForMember(t => t.OperationGenerated, opts => opts.MapFrom(d => (d.ContaOperationId != null)))
            //     .ForMember(t => t.BalanceComputed, opts => opts.Ignore());

            //CreateMap<TaxProfitCompExpense, TaxProfitCompExpenseDto>()
            //     .ForMember(t => t.AccountSymbol, opts => opts.MapFrom(d => d.Account.Symbol))
            //     .ForMember(t => t.AccountName, opts => opts.MapFrom(d => d.Account.AccountName));

            //CreateMap<TaxProfitCompExpenseDto, TaxProfitCompExpense>();

            //CreateMap<ExpenseDetails, ExpenseDetailsDto>();

            //CreateMap<TaxProfitCompDet, TaxProfitCompDetDto>()
            //     .ForMember(t => t.RowDescription, opts => opts.MapFrom(d => d.TaxProfitConfigDet.Description))
            //     .ForMember(t => t.RowNr, opts => opts.MapFrom(d => d.TaxProfitConfigDet.RowNr))
            //     .ForMember(t => t.OrderView, opts => opts.MapFrom(d => d.TaxProfitConfigDet.OrderView))
            //     .ForMember(t => t.TotalRow, opts => opts.MapFrom(d => d.TaxProfitConfigDet.TotalRow))
            //     .ForMember(t => t.IdRow, opts => opts.MapFrom(d => d.TaxProfitConfigDet.IdRow));

            #endregion

            #region Economic
            CreateMap<Invoices, NirDetailDTO>()
                .ForMember(t => t.Name, opts => opts.MapFrom(d => d.ThirdParty.FullName + " - " + d.InvoiceNumber + " / " + d.InvoiceDate.ToShortDateString()));

            //CreateMap<Invoices, InvoiceDTO>()
            //    .ForMember(t => t.CurrencyName, opts => opts.MapFrom(d => d.Currency.CurrencyName))
            //    .ForMember(t => t.ThirdPartyName, opts => opts.MapFrom(d => d.ThirdParty.Person.FullName))
            //    .ForMember(t => t.ThirdPartyQualityStr, opts => opts.MapFrom(d => d.ThirdPartyQuality.ToString()))
            //    .ForMember(t => t.EnableEdit, opts => opts.Ignore());

            CreateMap<InvoiceDTO, Invoices>()
                .ForMember(t => t.DocumentType, opts => opts.Ignore());
            CreateMap<Invoices, InvoiceDTO>()
                .ForMember(t => t.CurrencyName, opts => opts.MapFrom(d => d.Currency.CurrencyName))
                .ForMember(t => t.ThirdPartyName, opts => opts.MapFrom(d => d.ThirdParty.FullName))
                .ForMember(t => t.ThirdPartyQualityStr, opts => opts.MapFrom(d => d.ThirdPartyQuality.ToString()))
                .ForMember(t => t.DocumentType, opts => opts.MapFrom(d => d.DocumentType.TypeName))
                .ForMember(t => t.DocumentTypeShortName, opts => opts.MapFrom(d => d.DocumentType.TypeNameShort))
                .ForMember(t => t.ContaOperationStatus, opts => opts.MapFrom(d => d.ContaOperation.OperationStatus))
                .ForMember(t => t.ContractNumber, opts => opts.MapFrom(d => d.Contracts.ContractNr + " / " + d.Contracts.ContractDate.ToShortDateString()))
                .ForMember(t => t.DecontNumber, opts => opts.MapFrom(d => d.Decont.DecontNumber));

            CreateMap<InvoiceDetails, InvoiceDetailsDTO>()
                .ForMember(t => t.UnitValue, opts => opts.MapFrom(d => d.Value / d.Quantity))
                .ForMember(t => t.CotaTVA_Id, opts => opts.MapFrom(d => d.CotaTVA_Id));

            CreateMap<InvoiceDetailsDTO, InvoiceDetails>()
                .ForMember(t => t.InvoiceElementsDetails, opts => opts.Ignore())
                .ForMember(t => t.CotaTVA_Id, opts => opts.MapFrom(d => d.CotaTVA_Id));

            CreateMap<InvoiceElementsDetails, InvoiceElementsDetailsDTO>()
                .ForMember(t => t.InvoiceElementsTypeStr, opts => opts.MapFrom(d => LazyMethods.EnumValueToDescription(d.InvoiceElementsType)))
                .ForMember(t => t.InvoiceElementsType, opts => opts.MapFrom(d => d.InvoiceElementsType))
                .ForMember(t => t.InvoiceElementsDetailsCategory, opts => opts.MapFrom(d => d.InvoiceElementsDetailsCategory.CategoryElementDetName));

            CreateMap<InvoiceElementsDetailsDTO, InvoiceElementsDetails>()
                .ForMember(t => t.InvoiceElementsType, opts => opts.MapFrom(d => (InvoiceElementsType)d.InvoiceElementsType))
                .ForMember(t => t.InvoiceElementsDetailsCategory, opts => opts.Ignore());

            CreateMap<InvoiceElementsDetailsCategoryEditDTO, InvoiceElementsDetailsCategory>()
                .ForMember(t => t.CategoryType, opts => opts.MapFrom(d => d.CategoryTypeId));
            CreateMap<InvoiceElementsDetailsCategory, InvoiceElementsDetailsCategoryEditDTO>()
                .ForMember(t => t.CategoryTypeId, opts => opts.MapFrom(d => (int)d.CategoryType));

            CreateMap<InvoiceElementsDetailsCategoryListDTO, InvoiceElementsDetailsCategory>();
            CreateMap<InvoiceElementsDetailsCategory, InvoiceElementsDetailsCategoryListDTO>()
                .ForMember(t => t.CategoryType, opts => opts.MapFrom(d => LazyMethods.EnumValueToDescription(d.CategoryType)));

            CreateMap<InvoiceElementsDetailsCategory, InvoiceElementsDetailsDTO>()
                .ForMember(t => t.InvoiceElementsDetailsCategory, opts => opts.Ignore());
            CreateMap<InvoiceElementsDetailsDTO, InvoiceElementsDetailsCategory>()
                .ForMember(t => t.CategoryElementDetName, opts => opts.MapFrom(d => d.InvoiceElementsDetailsCategory));

            CreateMap<InvoiceElementAccountsDTO, InvoiceElementAccounts>();
            CreateMap<InvoiceElementAccounts, InvoiceElementAccountsDTO>()
                .ForMember(t => t.AccountName, opts => opts.MapFrom(d => d.Account.Symbol + " - " + d.Account.AccountName))
                .ForMember(t => t.InvoiceElementAccountTypeStr, opts => opts.MapFrom(d => d.InvoiceElementAccountType.ToString()));

            CreateMap<Contracts, ContractDto>()
                .ForMember(t => t.ThirdPartyName, opts => opts.MapFrom(d => d.ThirdParty.FullName))
                //.ForMember(t => t.MasterContractId, opts => opts.MapFrom(d => d.MasterContract.Id))
                .ForMember(t => t.CurrencyId, opts => opts.MapFrom(d => d.Currency.Id))
                .ForMember(t => t.CurrencyName, opts => opts.MapFrom(d => d.Currency.CurrencyName))
                .ForMember(t => t.Contract_State, opts => opts.MapFrom(d => d.GetContractState.ToString()))
                //.ForMember(t => t.ContractsStatusStr, opts => opts.MapFrom(d => d.ContractsStatus.ToString()))
                .ForMember(t => t.ContractsTypeStr, opts => opts.MapFrom(d => LazyMethods.EnumValueToDescription(d.ContractsType)))
                .ForMember(t => t.ThirdPartyQualityStr, opts => opts.MapFrom(d => d.ThirdPartyQuality.ToString()))
                .ForMember(t => t.AditionalContracts, opts => opts.Ignore())
                .ForMember(t => t.Contract_State, opts => opts.MapFrom(d => LazyMethods.EnumValueToDescription(d.GetContractState)));

            CreateMap<ContractDto, Contracts>()
                .ForMember(t => t.Currency, opts => opts.Ignore())
                .ForMember(t => t.Currency_Id, opts => opts.MapFrom(d => d.CurrencyId))
                .ForMember(t => t.ThirdParty, opts => opts.Ignore())
                .ForMember(t => t.ContractsInstalments, opts => opts.Ignore())
                .ForMember(t => t.ContractsCategory, opts => opts.Ignore());

            CreateMap<ContractsCategory, ContractCategoryListDto>()
                 .ForMember(t => t.Name, opt => opt.MapFrom(d => d.CategoryName)); ;
            CreateMap<ContractCategoryListDto, ContractsCategory>();

            CreateMap<ContractsCategory, ContractCategoryEditDto>()
                .ForMember(t => t.Name, opt => opt.MapFrom(d => d.CategoryName));
            CreateMap<ContractCategoryEditDto, ContractsCategory>()
                .ForMember(t => t.CategoryName, opt => opt.MapFrom(d => d.Name));
            CreateMap<Contracts_State, ContractStateListDto>()
                .ForMember(t => t.Contract_State, opt => opt.MapFrom(d => LazyMethods.EnumValueToDescription( d.Contract_State)));
            CreateMap<ContractStateListDto, Contracts_State>();


            CreateMap<IssuerDto, Issuer>();
            CreateMap<Issuer, IssuerDto>()
                .ForMember(t => t.LegalPersonId, opts => opts.MapFrom(d => d.LegalPersonId))
                .ForMember(t => t.IssuerType, opts => opts.MapFrom(d => d.IssuerType));

            CreateMap<IssuerListDto, Issuer>();
            CreateMap<Issuer, IssuerListDto>()
                .ForMember(t => t.IssuerType, opts => opts.MapFrom(d => d.IssuerType));

            CreateMap<IssuerListDto, Person>();
            CreateMap<Person, IssuerListDto>()
                .ForMember(t => t.Id1, opts => opts.MapFrom(d => d.Id1))
                .ForMember(t => t.Id2, opts => opts.MapFrom(d => d.Id2))
                .ForMember(t => t.FullName, opts => opts.MapFrom(d => d.FullName));

            CreateMap<Issuer, PersonIssuerEditDto>();
            CreateMap<PersonIssuerEditDto, Issuer>();

            CreateMap<Person, PersonIssuerEditDto>()
                .ForMember(t => t.PersonType, opts => opts.Ignore());
            CreateMap<PersonIssuerEditDto, Person>();

            CreateMap<LegalPerson, PersonIssuerEditDto>()
                .ForMember(t => t.FirstName, opts => opts.Ignore())
                .ForMember(t => t.LastName, opts => opts.Ignore())
                .ForMember(t => t.PersonType, opts => opts.Ignore())
                .ForMember(t => t.DefinedById, opts => opts.MapFrom(d => d.CreatorUserId));

            CreateMap<PersonIssuerEditDto, LegalPerson>()
                .ForMember(t => t.CreatorUserId, opts => opts.MapFrom(d => d.DefinedById));

            CreateMap<IssuerDetailsDto, Issuer>();
            CreateMap<Issuer, IssuerDetailsDto>();

            CreateMap<Invoices, AutoInvNInvoices>()
                .ForMember(t => t.CurrencyName, opts => opts.MapFrom(d => d.Currency.CurrencyName))
                .ForMember(t => t.ThirdParty, opts => opts.MapFrom(d => d.ThirdParty.FullName))
                .ForMember(t => t.ThirdPartyQualityStr, opts => opts.MapFrom(d => d.ThirdPartyQuality.ToString()))
                .ForMember(t => t.OperationDate, opts => opts.MapFrom(d => d.InvoiceDate))
                .ForMember(t => t.Selected, opts => opts.MapFrom(d => true));

            CreateMap<Invoices, AutoInvPInvoices>()
                .ForMember(t => t.CurrencyName, opts => opts.MapFrom(d => d.Currency.CurrencyName))
                .ForMember(t => t.ThirdParty, opts => opts.MapFrom(d => d.ThirdParty.FullName))
                .ForMember(t => t.ThirdPartyQualityStr, opts => opts.MapFrom(d => d.ThirdPartyQuality.ToString()));

            CreateMap<Invoices, AutoInvDInvoices>()
                .ForMember(t => t.CurrencyName, opts => opts.MapFrom(d => d.Currency.CurrencyName))
                .ForMember(t => t.ThirdParty, opts => opts.MapFrom(d => d.ThirdParty.FullName))
                .ForMember(t => t.ThirdPartyQualityStr, opts => opts.MapFrom(d => d.ThirdPartyQuality.ToString()));

            CreateMap<Disposition, DispositionListDto>()
                .ForMember(t => t.ThirdPartyName, opts => opts.MapFrom(d => d.ThirdParty.FullName))
                .ForMember(t => t.OperationType, opts => opts.MapFrom(d => d.OperationType.ToString()))
                .ForMember(t => t.OperationTypeId, opts => opts.MapFrom(d => (int)d.OperationType))
                .ForMember(t => t.CurrencyName, opts => opts.MapFrom(d => d.Currency.CurrencyName))
                .ForMember(t => t.DocumentTypeName, opts => opts.MapFrom(d => d.DocumentType.TypeName))
                .ForMember(t => t.DispositionOperStatus, opts => opts.MapFrom(d => d.Operation.OperationStatus));
            CreateMap<DispositionListDto, Disposition>();

            CreateMap<Disposition, DispositionEditDto>()
                .ForMember(t => t.ThirdPartyName, opts => opts.MapFrom(d => d.ThirdParty.FullName))
                .ForMember(t => t.CategoryElementId, opts => opts.MapFrom(d => d.InvoiceElementsDetails.InvoiceElementsDetailsCategoryId))
                .ForMember(t => t.ElementId, opts => opts.MapFrom(d => d.InvoiceElementsDetailsId));
            CreateMap<DispositionEditDto, Disposition>()
                .ForMember(t => t.InvoiceElementsDetailsId, opts => opts.MapFrom(d => d.ElementId))
                .ForMember(t => t.InvoiceElementsDetails, opts => opts.Ignore());

            CreateMap<DepositListDto, Disposition>();
            CreateMap<Disposition, DepositListDto>()
                .ForMember(t => t.CurrencyName, opts => opts.MapFrom(d => d.Currency.CurrencyName))
                .ForMember(t => t.BankAcount, opts => opts.MapFrom(d => d.BankAccount.IBAN))
                .ForMember(t => t.BankName, opts => opts.MapFrom(d => d.BankAccount.Bank.LegalPerson.FullName));

            CreateMap<InvoiceListSelectableDto, Invoices>();
            CreateMap<Invoices, InvoiceListSelectableDto>();

            CreateMap<DepositEditDto, Disposition>();
            CreateMap<Disposition, DepositEditDto>();

            CreateMap<SoldInitialDto, Disposition>();
            CreateMap<Disposition, SoldInitialDto>()
                .ForMember(t => t.CurrencyName, opts => opts.MapFrom(d => d.Currency.CurrencyName));

            CreateMap<SoldInitialEditDto, Disposition>()
                .ForMember(t => t.CurrencyId, opts => opts.MapFrom(d => d.CurrencyId));
            CreateMap<Disposition, SoldInitialEditDto>();

            CreateMap<TranzactiiFonduriDto, TranzactiiFonduri>();
            CreateMap<TranzactiiFonduri, TranzactiiFonduriDto>();

            //CreateMap<PaymentOrderForForeignOperationDto, PaymentOrders>();
            //CreateMap<PaymentOrders, PaymentOrderForForeignOperationDto>()
            //    .ForMember(f => f.PayerBank, opts => opts.MapFrom(d => d.PayerBankAccount.IBAN))
            //    .ForMember(f => f.Id, opts => opts.MapFrom(d => d.PayerBankAccountId));

            //CreateMap<PaymentOrders, ForeignOperationsDetails>()
            //    .ForMember(t => t.PaymentOrderId, opts => opts.MapFrom(d => d.Id));

            #endregion

            #region Nomenclatures
            CreateMap<PersonListDto, Person>();
            CreateMap<PersonEditDto, Person>();

            //CreateMap<Bank, BankListDto>()
            //    .ForMember(t => t.BankName, opts => opts.MapFrom(d => d.LegalPerson.Name));

            CreateMap<BankAccount, ThirdPartyAccListDto>()
                .ForMember(t => t.BankName, opts => opts.MapFrom(d => d.Bank.LegalPerson.FullName))
                .ForMember(t => t.Currency, opts => opts.MapFrom(d => d.Currency.CurrencyCode));

            CreateMap<Person, ThirdPartyListDto>()
                 .ForMember(t => t.FullName, opts => opts.MapFrom(d => d.FullName))
                 .ForMember(t => t.PersonId, opts => opts.MapFrom(d => d.Id));

            //CreateMap<ThirdParty, ThirdPartyListDDDto>()
            //     .ForMember(t => t.Id, opts => opts.MapFrom(d => d.PersonId))
            //     .ForMember(t => t.Name, opts => opts.MapFrom(d => d.Person.FullName));


            //CreateMap<ThirdPartyEditDto, ThirdParty>();
            //CreateMap<ThirdParty, ThirdPartyEditDto>();
            #endregion

            #region Nomenclatures
            CreateMap<PersonListDto, Person>();

            CreateMap<PersonEditDto, Person>();

            CreateMap<NaturalPerson, PersonEditDto>()
                .ForMember(t => t.Name, opts => opts.Ignore())
                .ForMember(t => t.PersonType, opts => opts.Ignore());

            CreateMap<LegalPerson, PersonEditDto>()
                .ForMember(t => t.FirstName, opts => opts.Ignore())
                .ForMember(t => t.LastName, opts => opts.Ignore())
                .ForMember(t => t.PersonType, opts => opts.Ignore());


            CreateMap<PersonEditDto, LegalPerson>();

            CreateMap<PersonEditDto, NaturalPerson>();
            CreateMap<Issuer, BankListDto>()
                    .ForMember(t => t.BankName, opts => opts.MapFrom(d => d.LegalPerson.Name))
                    .ForMember(t => t.IssuerId, opts => opts.MapFrom(d => d.Id));
            CreateMap<BankListDto, Issuer>();

            //CreateMap<BankListDto, Bank>();
            //CreateMap<Bank, BankListDto>()
            //    .ForMember(t => t.BankName, opts => opts.MapFrom(d => d.LegalPerson.Name));

            //CreateMap<Bank, BankEditDto>()
            //    .ForMember(t => t.BankName, opts => opts.MapFrom(d => d.LegalPerson.Name));
            ////.ForMember(t => t.Add, opts => opts.MapFrom(d => (d.LegalPersonId == 0) ? true : false));

            //CreateMap<BankEditDto, Bank>();

            CreateMap<BankAccount, ThirdPartyAccListDto>()
                .ForMember(t => t.BankName, opts => opts.MapFrom(d => d.Bank.LegalPerson.FullName))
                .ForMember(t => t.Currency, opts => opts.MapFrom(d => d.Currency.CurrencyCode))
                .ForMember(t => t.ThirdPartyId, opts => opts.MapFrom(d => d.PersonId));

            //CreateMap<ThirdParty, ThirdPartyListDto>()
            //     .ForMember(t => t.FullName, opts => opts.MapFrom(d => d.Person.FullName));


            //CreateMap<ThirdPartyEditDto, ThirdParty>();

            CreateMap<ThirdPartyAccEditDto, BankAccount>();
            CreateMap<ThirdPartyAccEditDto, Currency>();
            CreateMap<CurrencyListDto, Currency>();

            CreateMap<PersonEditDto, Country>();
            CreateMap<CountryListDto, Country>();
            CreateMap<CountryListDto, Region>()
                .ForMember(t => t.Country, opts => opts.MapFrom(d => d.Id));

            CreateMap<Currency, CurrencyDto>()
                .ForMember(t => t.Name, opts => opts.MapFrom(d => d.CurrencyCode));

            //CreateMap<ThirdParty, ThirdPartyEditDto>();

            CreateMap<ActivityType, ActivityTypeDto>()
                .ForMember(t => t.Name, opts => opts.MapFrom(d => d.ActivityName));
            CreateMap<ActivityTypeDto, ActivityType>()
                .ForMember(t => t.ActivityName, opts => opts.MapFrom(d => d.Name));
            #endregion

            #region Tokens
            //CreateMap<Tokens, TokensListDto>();
            //CreateMap<TokensListDto, Tokens>();
            //CreateMap<TokensEditDto, Tokens>();

            //CreateMap<ApiLinks, ApiLinksListDto>();
            //CreateMap<ApiLinksListDto, ApiLinks>();
            //CreateMap<ApiLinksEditDto, ApiLinks>();

            #endregion

            #region SituatiiFinanciare
            CreateMap<SitFinan, SitFinanDto>();

            CreateMap<SitFinan, SitFinanConfigDto>()
                .ForMember(t => t.PrevDateId, opts => opts.Ignore())
                .ForMember(t => t.CopyReport, opts => opts.Ignore());

            CreateMap<SitFinanRap, SitFinanRapDto>();

            CreateMap<SitFinanRapConfig, SitFinanRapConfigDto>();

            CreateMap<SitFinanRapConfigDto, SitFinanRapConfig>();

            CreateMap<SitFinanRapConfigCol, SitFinanRapConfigColDto>();

            CreateMap<SitFinanRapFluxConfig, SitFinanRapFluxConfigDto>();

            CreateMap<SitFinanRapFluxConfigDto, SitFinanRapFluxConfig>();

            #endregion

            #region Imprumuturi
            //ImprumuturiTermen
            CreateMap<ImprumutTermen, ImprumuturiTermenDto>();
            CreateMap<ImprumuturiTermenDto, ImprumutTermen>();
            CreateMap<ImprumuturiTermenEditDto, ImprumutTermen>();
            CreateMap<ImprumutTermen, ImprumuturiTermenEditDto>();

            //ImprumuturiTipuri
            CreateMap<ImprumutTip, ImprumuturiTipuriDto>();
            CreateMap<ImprumuturiTipuriDto, ImprumutTip>();
            CreateMap<ImprumuturiTipuriEditDto, ImprumutTip>();
            CreateMap<ImprumutTip, ImprumuturiTipuriEditDto>();

            CreateMap<ImprumutTipDetaliu, ImprumutTipDetaliuEditDto>();
            CreateMap<ImprumutTipDetaliuEditDto, ImprumutTipDetaliu>();

            CreateMap<ImprumutTipDetaliu, ImprumutTipDetaliuDto>()
                .ForMember(f => f.Description, opts => opts.MapFrom(d => LazyMethods.EnumValueToDescription(d.Description)));
            CreateMap<ImprumutTipDetaliuDto, ImprumutTipDetaliu>();

            //ImprumutState
            CreateMap<ImprumutState, ImprumutStateDto>()
                .ForMember(t => t.ImprumuturiStareString , opts => opts.MapFrom(d => d.ImprumuturiStare.ToString()));
            CreateMap<ImprumutStateDto, ImprumutState>();



            //Imprumut
            CreateMap<Imprumut, ImprumutDto>()
                .ForMember(t => t.LoanAccount, opts => opts.MapFrom(d => d.LoanAccount.IBAN))
                .ForMember(t => t.PaymentAccount, opts => opts.MapFrom(d => d.PaymentAccount.IBAN))
                .ForMember(t => t.ImprumuturiTermen, opts => opts.MapFrom(d => d.ImprumuturiTermen.Description))
                .ForMember(t => t.ImprumuturiTipuri, opts => opts.MapFrom(d => d.ImprumuturiTipuri.Description))
                .ForMember(t => t.DocumentType, opts => opts.MapFrom(d => d.DocumentType.TypeName))
                .ForMember(t => t.DobanziReferinta, opts => opts.MapFrom(d => d.DobanziReferinta.Dobanda))
                .ForMember(t => t.Currency, opts => opts.MapFrom(d => d.Currency.CurrencyName))
                .ForMember(t => t.Bank, opts => opts.MapFrom(d => d.Bank.LegalPerson.Name))
                .ForMember(t => t.TipCreditare, opts => opts.MapFrom(d => d.TipCreditare.ToString()))
                .ForMember(t => t.ThirdParty, opts => opts.MapFrom(d => d.ThirdParty.FullName))
                .ForMember(t => t.ImprumuturiStare, opts => opts.MapFrom(d => d.GetImprumuturiState.ToString()));
                

            CreateMap<ImprumutDto, Imprumut>();

            CreateMap<ImprumutEditDto, Imprumut>();
            CreateMap<Imprumut, ImprumutEditDto>();

            CreateMap<ImprumutEditDto, ImprumutDto>();

            #endregion

            #region Garantii
            CreateMap<Garantie, GarantieEditDto>();



            CreateMap<Garantie, GarantieDto>()
                .ForMember(t => t.GarantieAccount, opts => opts.MapFrom(d => d.GarantieAccount.IBAN))
                .ForMember(t => t.Currency, opts => opts.MapFrom(d => d.Currency.CurrencyName))
                .ForMember(t => t.LegalPerson, opts => opts.MapFrom(d => d.LegalPerson.Name))
                .ForMember(t => t.GarantieTip, opts => opts.MapFrom(d => d.GarantieTip.Description))
                .ForMember(t => t.GarantieCeGaranteaza, opts => opts.MapFrom(d => d.GarantieCeGaranteaza.Description))
                .ForMember(t => t.Sold, opts => opts.MapFrom(d => d.OperatiiGarantie.Where(f => f.State == Models.Conta.Enums.State.Active).OrderByDescending(f => f.Id).First().Sold));


            CreateMap<GarantieDto, Garantie>();

            CreateMap<GarantieEditDto, Garantie>();

            CreateMap<GarantieEditDto, GarantieDto>();

            CreateMap<OperatieGarantie, OperatieGarantieDto>();

            CreateMap<OperatieGarantieDto, OperatieGarantie>();
            #endregion

            #region Comisioane
            CreateMap<Comision, ComisionEditDto>();

            CreateMap<Comision, ComisionDto>();

            CreateMap<ComisionDto, Comision>();

            CreateMap<ComisionEditDto, Comision>();

            CreateMap<ComisionEditDto, ComisionDto>();
            #endregion

            #region DateComisioane
            CreateMap<DataComision, DataComisionEditDto>();

            CreateMap<DataComision, DataComisionDto>();

            CreateMap<DataComisionDto, DataComision>();

            CreateMap<DataComisionEditDto, DataComision>();

            CreateMap<DataComisionEditDto, DataComisionDto>();
            #endregion

            #region Operatie Comision Dobanda
            CreateMap<OperatieDobandaComision, OperatieDobandaComisionDto>();

            CreateMap<OperatieDobandaComisionDto, OperatieDobandaComision>();

            #endregion

            #region ComisionV2
            CreateMap<ComisionV2, ComisionV2EditDto>();

            CreateMap<ComisionV2EditDto, ComisionV2>();

            CreateMap<ComisionV2, ComisionV2Dto>();

            CreateMap<ComisionV2Dto, ComisionV2>();

            #endregion

            #region Rate


            //Rata


            CreateMap<Rata, RataEditDto>();


            CreateMap<Rata, RataDto>()
                .ForMember(t => t.Currency, opts => opts.MapFrom(d => d.Currency.CurrencyName));

            CreateMap<RataDto, Rata>();

            CreateMap<RataEditDto, Rata>();

            CreateMap<RataEditDto, RataDto>();

            #endregion
            CreateMap<Tragere, TragereDto>()
                .ForMember(t => t.CurrencyS, opts => opts.MapFrom(d => d.Currency.CurrencyName))
                .ForMember(t => t.TipTragereString, opts => opts.MapFrom(d => d.TipTragere.ToString()));

            CreateMap<TragereDto, Tragere>();
            #region Tragere

            #endregion

            #region GarantieTip

            CreateMap<GarantieTip, GarantieTipEditDto>();


            CreateMap<GarantieTip, GarantieTipDto>();


            CreateMap<GarantieTipDto, GarantieTip>();

            CreateMap<GarantieTipEditDto, GarantieTip>();

            CreateMap<GarantieTipEditDto, GarantieTipDto>();

            #endregion

            #region GarantieCeGaranteaza

            CreateMap<GarantieCeGaranteaza, GarantieCeGaranteazaEditDto>();


            CreateMap<GarantieCeGaranteaza, GarantieCeGaranteazaDto>();


            CreateMap<GarantieCeGaranteazaDto, GarantieCeGaranteaza>();

            CreateMap<GarantieCeGaranteazaEditDto, GarantieCeGaranteaza>();

            CreateMap<GarantieCeGaranteazaEditDto, GarantieCeGaranteazaDto>();

            #endregion

            #region DobanziReferinta


            CreateMap<DobanziReferinta, DobanziReferintaDto>();
            CreateMap<DobanziReferintaDto, DobanziReferinta>();
            CreateMap<DobanziReferintaEditDto, DobanziReferinta>();
            CreateMap<DobanziReferinta, DobanziReferintaEditDto>();
            #endregion

            #region DateDobanziReferinta
            CreateMap<DateDobanziReferinta, DateDobanziReferintaEditDto>();

            CreateMap<DateDobanziReferinta, DateDobanziReferintaDto>()
                .ForMember(t => t.DobanziReferinta, opts => opts.MapFrom(d => d.DobanziReferinta.Dobanda));


            CreateMap<DateDobanziReferintaDto, DateDobanziReferinta>();

            CreateMap<DateDobanziReferintaEditDto, DateDobanziReferinta>();

            CreateMap<DateDobanziReferintaEditDto, DateDobanziReferintaDto>();
            #endregion

            #region Configurare rapoarte
            CreateMap<ReportInit, RepConfigInitDto>();
            CreateMap<RepConfigInitDto, ReportInit>();

            CreateMap<ReportInit, RepConfigEditDto>();
            CreateMap<RepConfigEditDto, ReportInit>();

            CreateMap<Report, RepConfigDto>()
                .ForMember(t => t.RepConfigId, opts => opts.MapFrom(d => d.ReportInitId));

            CreateMap<ReportConfig, ConfigFormulaDto>();
            CreateMap<ConfigFormulaDto, ReportConfig>();
            #endregion

            #region RegistruInventar
            CreateMap<RegInventarExceptii, RegInventarExceptiiListDto>()
                .ForMember(t => t.AccountName, opts => opts.MapFrom(d => d.Account.Symbol + " - " + d.Account.AccountName));

            CreateMap<RegInventarExceptiiListDto, RegInventarExceptii>();

            CreateMap<RegInventarExceptiiEliminare, ExceptEliminareRegInventarListDto>()
                .ForMember(t => t.AccountName, opts => opts.MapFrom(d => d.Account.Symbol + " - " + d.Account.AccountName));

            CreateMap<ExceptEliminareRegInventarListDto, RegInventarExceptiiEliminare>();

            CreateMap<RegInventar, RegistruInventarItem>()
                .ForMember(f => f.AccountName, opts => opts.MapFrom(d => d.Account.Symbol + " - " + d.Account.AccountName));
            CreateMap<RegistruInventarItem, RegInventar>();

            #endregion

            #region Schimb valutar
            CreateMap<Exchange, ExchangeEditDto>()
                .ForMember(t => t.ActivityTypeId, opts => opts.MapFrom(d => d.ActivityTypeId))
                .ForMember(t => t.OperationType, opts => opts.MapFrom(d => d.ExchangeType));
            CreateMap<ExchangeEditDto, Exchange>()
                .ForMember(t => t.ActivityTypeId, opts => opts.MapFrom(d => d.ActivityTypeId))
                .ForMember(t => t.BankAccountLeiId, opts => opts.MapFrom(d => d.BankAccountLeiId))
                .ForMember(t => t.BankAccountValutaId, opts => opts.MapFrom(d => d.BankAccountValutaId))
                .ForMember(t => t.ExchangeOperType, opts => opts.MapFrom(d => (int)d.ExchangeOperType))
                .ForMember(t => t.ExchangeType, opts => opts.MapFrom(d => (int)d.OperationType));

            CreateMap<Exchange, ExchangeListDto>()
                .ForMember(t => t.BankAccountLei, opts => opts.MapFrom(d => d.BankAccountLei.IBAN))
                .ForMember(t => t.BankAccountValuta, opts => opts.MapFrom(d => d.BankAccountValuta.IBAN))
                .ForMember(t => t.ExchangeOperType, opts => opts.MapFrom(d => LazyMethods.EnumValueToDescription(d.ExchangeOperType)))
                .ForMember(t => t.OperationType, opts => opts.MapFrom(d => LazyMethods.EnumValueToDescription(d.ExchangeType)))
                .ForMember(t => t.CurrencyName, opts => opts.MapFrom(d => d.Curency.CurrencyName.ToString()));
            CreateMap<ExchangeListDto, Exchange>();

            CreateMap<ExchangeRates, ExchangeRateDto>();

            CreateMap<ExchangeRateDto, ExchangeRates>();

            #endregion

            #region Cupiuri
            CreateMap<CupiuriListDto, CupiuriInit>();
            CreateMap<CupiuriInit, CupiuriListDto>();

            CreateMap<CupiuriForm, CupiuriItem>();
            CreateMap<CupiuriItem, CupiuriForm>();

            CreateMap<CupiuriDetails, CupiuriDetailsDto>();
            CreateMap<CupiuriDetailsDto, CupiuriDetails>();

            CreateMap<CupiuriItem, CupiuriItemDto>();
            CreateMap<CupiuriItemDto, CupiuriItem>();

            CreateMap<CupiuriDeclaratieCaserie, CupiuriItem>();
            CreateMap<CupiuriItem, CupiuriDeclaratieCaserie>();

            CreateMap<CupiuriDeclaratieCaserieDetails, CupiuriDetails>();
            CreateMap<CupiuriDetails, CupiuriDeclaratieCaserieDetails>();

            #endregion

            #region BVC_PAAP
            CreateMap<BVC_PAAP, PaapEditDto>()
                .ForMember(t => t.SursaFinantareId, opts => opts.MapFrom(d => d.SursaFinatare))
                .ForMember(t => t.ModalitateDerulareId, opts => opts.MapFrom(d => d.ModalitateDerulare))
                .ForMember(t => t.ContractsPaymentInstalmentFreqId, opts => opts.MapFrom(d => d.ContractsPaymentInstalmentFreq))
                .ForMember(t => t.InvoiceElementsDetailsCategoryId, opts => opts.MapFrom(d => d.InvoiceElementsDetailsCategory.Id))
                .ForMember(t => t.InvoiceElementsDetailsId, opts => opts.MapFrom(d => d.InvoiceElementsDetails.Id))
                .ForMember(t => t.ObiectTranzactieId, opts => opts.MapFrom(d => (int)d.ObiectTranzactie))
                .ForMember(t => t.ImoAssetClassCodeId, opts => opts.MapFrom(d => d.AssetClassCodesId))
                .ForMember(t => t.CotaTVA_Id, opts => opts.MapFrom(d => d.CotaTVA_Id));

            CreateMap<PaapEditDto, BVC_PAAP>()
                .ForMember(t => t.ContractsPaymentInstalmentFreq, opts => opts.MapFrom(d => d.ContractsPaymentInstalmentFreqId))
                .ForMember(t => t.ModalitateDerulare, opts => opts.MapFrom(d => d.ModalitateDerulareId))
                .ForMember(t => t.SursaFinatare, opts => opts.MapFrom(d => d.SursaFinantareId))
                .ForMember(t => t.InvoiceElementsDetailsId, opts => opts.MapFrom(d => d.InvoiceElementsDetailsId))
                .ForMember(t => t.AssetClassCodesId, opts => opts.MapFrom(d => d.ImoAssetClassCodeId))
                .ForMember(t => t.DepartamentId, opts => opts.MapFrom(d => d.DepartamentId))
                .ForMember(t => t.ObiectTranzactie, opts => opts.MapFrom(d => d.ObiectTranzactieId))
                .ForMember(t => t.Transe, opts => opts.MapFrom(d => d.Transe));

            CreateMap<BVC_PAAPTranseDto, BVC_PAAPTranse>();

            CreateMap<BVC_PAAPTranse, BVC_PAAPTranseDto>();

            CreateMap<PaapTranseListDto, BVC_PAAPTranse>();
            CreateMap<BVC_PAAPTranse, PaapTranseListDto>()
                .ForMember(t => t.StatePAAP, opts => opts.MapFrom(d =>d.BVC_PAAP.GetPaapState.ToString()));

            CreateMap<BVC_PAAP, PaapDto>()
                .ForMember(t => t.InvoiceElementsDetailsCategoryName, opts => opts.MapFrom(d => d.InvoiceElementsDetailsCategory.CategoryElementDetName))
                .ForMember(t => t.InvoiceElementsDetailsName, opts => opts.MapFrom(d => d.InvoiceElementsDetails.Description))
                .ForMember(t => t.InvoiceElementsDetailsCategoryId, opts => opts.MapFrom(d => d.InvoiceElementsDetailsCategory.Id))
                .ForMember(t => t.InvoiceElementsDetailsId, opts => opts.MapFrom(d => d.InvoiceElementsDetails.Id))
                .ForMember(t => t.SursaFinantare, opts => opts.MapFrom(d => d.SursaFinatare.ToString()))
                .ForMember(t => t.ContractsPaymentInstalmentFreq, opts => opts.MapFrom(d => d.ContractsPaymentInstalmentFreq.ToString()))
                .ForMember(t => t.ModDerulare, opts => opts.MapFrom(d => d.ModalitateDerulare.ToString()))
                .ForMember(t => t.Value, opts => opts.MapFrom(d => d.ValoareEstimataFaraTvaLei))
                .ForMember(t => t.ObiectTranzactie, opts => opts.MapFrom(d => d.ObiectTranzactie.ToString()))
                .ForMember(t => t.StatePAAP, opts => opts.MapFrom(d => d.GetPaapState.ToString()))
                .ForMember(t => t.StatePAAPId, opts => opts.MapFrom(d => (int)d.GetPaapState))
                .ForMember(t => t.DepartamentName, opts => opts.MapFrom(d => d.Departament.Name))
                .ForMember(t => t.DepartamentId, opts => opts.MapFrom(d => d.Departament.Id))
                .ForMember(t => t.CotaTVA_Id, opts => opts.MapFrom(d => d.CotaTVA.Id))
                .ForMember(t => t.CurrencyName, opts => opts.MapFrom(d => d.Currency.CurrencyName))
                .ForMember(t => t.ValoareRealizata, opts => opts.Ignore())
                .ForMember(t => t.Transe, opts => opts.MapFrom(d => d.Transe));

            CreateMap<PaapDto, BVC_PAAP>()
                .ForMember(t => t.InvoiceElementsDetailsCategoryId, opts => opts.MapFrom(d => d.InvoiceElementsDetailsCategoryId))
                .ForMember(t => t.ValoareEstimataFaraTvaLei, opts => opts.MapFrom(d => d.Value));

            CreateMap<BVC_PAAP_State, PaapStateEditDto>()
                .ForMember(t => t.PaapId, opts => opts.MapFrom(d => d.BVC_PAAP_Id));
            CreateMap<PaapStateEditDto, BVC_PAAP_State>()
                .ForMember(t => t.BVC_PAAP_Id, opts => opts.MapFrom(d => d.PaapId));
            CreateMap<BVC_PAAP_State, PaapStateListDto>()
                .ForMember(f => f.CurrencyName, opts => opts.MapFrom(d => d.BVC_PAAP.Currency.CurrencyName))
                .ForMember(f => f.TVA, opts => opts.MapFrom(d => d.CotaTVA.VAT))
                .ForMember(f => f.DataEnd, opts => opts.MapFrom(d => d.BVC_PAAP.DataEnd));

            CreateMap<BVC_PAAP_ApprovedYear, PaapApprovedYearDto>();
            CreateMap<PaapApprovedYearDto, BVC_PAAP_ApprovedYear>();

            CreateMap<BVC_PAAP_InvoiceDetails, BVC_PAAP_InvoiceDetailsDto>();
            CreateMap<BVC_PAAP_InvoiceDetailsDto, BVC_PAAP_InvoiceDetails>();

            CreateMap<BVC_Cheltuieli, BugetCheltuieliDto>()
                .ForMember(f => f.BVC_FormRand_Descriere, opts => opts.MapFrom(d => d.BVC_FormRand.Descriere))
                .ForMember(f => f.Currency, opts => opts.MapFrom(d => d.Currency.CurrencyName))
                .ForMember(f => f.ActivityTypeName, opts => opts.MapFrom(d => d.ActivityType.ActivityName)) 
                .ForMember(f => f.DepartamentName, opts => opts.MapFrom(d => d.Departament.Name));

            CreateMap<BugetCheltuieliDto, BVC_Cheltuieli>();

            CreateMap<BugetCheltuieliEditDto, BVC_Cheltuieli>();
            CreateMap<BVC_Cheltuieli,BugetCheltuieliEditDto>();



            // Curs Valutar Estimat

            CreateMap<ExchangeRateForecastDto, ExchangeRateForecast>();
            CreateMap<ExchangeRateForecastEditDto, ExchangeRateForecast>();
            CreateMap<ExchangeRateForecast, ExchangeRateForecastEditDto>();
            CreateMap<ExchangeRateForecast, ExchangeRateForecastDto>()
                 .ForMember(t => t.Currency, opts => opts.MapFrom(d => d.Currency.CurrencyName));

            // Buget Config 


            CreateMap<BugetConfigDto, BVC_Formular>();
            CreateMap<BugetConfigEditDto, BVC_Formular>();
            CreateMap<BVC_Formular, BugetConfigEditDto>();
            CreateMap<BVC_Formular, BugetConfigDto>();



            CreateMap<BVC_PAAP_Referat, PaapReferatDto>();
            CreateMap<PaapReferatDto, BVC_PAAP_Referat>();

            CreateMap<BVC_PAAP_Referat, PaapReferatEditDto>();
            CreateMap<PaapReferatEditDto, BVC_PAAP_Referat>();

            CreateMap<BVC_FormRand, BugetFormRandDto>();
            CreateMap<BugetFormRandDto, BVC_FormRand>();

            CreateMap<BVC_FormRandDetails, BugetFormRandDetailDto>();
            CreateMap<BugetFormRandDetailDto, BVC_FormRandDetails>();

            CreateMap<BVC_BugetPrev, BugetPrevGenerateDto>();
            CreateMap<BugetPrevGenerateDto, BVC_BugetPrev>();

            CreateMap<BVC_BugetPrev, BugetPrevListDto>()
                .ForMember(t => t.AnBuget, opts => opts.MapFrom(d => d.Formular.AnBVC))
                .ForMember(t => t.BVC_TipId, opts => opts.MapFrom(d => (int)d.BVC_Tip));
            CreateMap<BugetPrevListDto, BVC_BugetPrev>();

            CreateMap<BVC_BugetPrevAutoValue, BugetPrevAutoValueListDto>()
                .ForMember(t => t.DepartamentName, opts => opts.MapFrom(d => d.Departament.Name))
                .ForMember(t => t.TipRandName, opts => opts.MapFrom(d => LazyMethods.EnumValueToDescription(d.TipRand)))
                .ForMember(t => t.TipRandVenitName, opts => opts.MapFrom(d => LazyMethods.EnumValueToDescription(d.TipRandVenit)));
            CreateMap<BugetPrevAutoValueListDto, BVC_BugetPrevAutoValue>();

            CreateMap<BVC_BugetPrevAutoValue, BugetPrevAutoValueAddDto>()
                .ForMember(t => t.TipRandId, opts => opts.MapFrom(d => (int)d.TipRand))
                .ForMember(t => t.TipRandVenitId, opts => opts.MapFrom(d => (int?)d.TipRandVenit));
            CreateMap<BugetPrevAutoValueAddDto, BVC_BugetPrevAutoValue>()
                .ForMember(t => t.TipRand, opts => opts.MapFrom(d => d.TipRandId))
                .ForMember(t => t.TipRandVenit, opts => opts.MapFrom(d => d.TipRandVenitId));

            CreateMap<BVC_BugetPrevContributie, BugetPrevContribListDto>()
                .ForMember(t => t.CurrencyName, opts => opts.MapFrom(d => d.Currency.CurrencyName))
                .ForMember(t => t.ActivityTypeName, opts => opts.MapFrom(d => d.ActivityType.ActivityName))
                .ForMember(t => t.BankName, opts => opts.MapFrom(d => d.BankAccount.LegalPerson.FullName))
                .ForMember(t => t.TipIncasare, opts => opts.MapFrom(d => LazyMethods.EnumValueToDescription(d.TipIncasare)));
            CreateMap<BugetPrevContribListDto, BVC_BugetPrevContributie>();

            CreateMap<BVC_BugetPrevContributie, BugetPrevContribAddDto>();
            CreateMap<BugetPrevContribAddDto, BVC_BugetPrevContributie>();

            CreateMap<BVC_BugetPrevRandValue, BugetPrevDetailRandValueDto>()
                .ForMember(t => t.ActivityTypeId, opts => opts.MapFrom(d => d.ActivityType.Id))
                .ForMember(t => t.ActivityTypeName, opts => opts.MapFrom(d => d.ActivityType.ActivityName));
            CreateMap<BugetPrevDetailRandValueDto, BVC_BugetPrevRandValue>()
                .ForMember(t => t.ActivityTypeId, opts => opts.MapFrom(d => d.ActivityTypeId))
                .ForMember(t => t.Value, opts => opts.MapFrom(d => d.Valoare))
                .ForMember(t => t.Description, opts => opts.MapFrom(d => d.Descriere));

            CreateMap<BVC_DobandaReferinta, BugetPrevDobandaReferintaListDto>()
                .ForMember(t => t.PlasamentName, opts => opts.MapFrom(d => LazyMethods.EnumValueToDescription(d.PlasamentType)))
                .ForMember(t => t.CurrencyName, opts => opts.MapFrom(d => d.Currency.CurrencyName))
                .ForMember(t => t.An, opts => opts.MapFrom(d => d.Formular.AnBVC))
                .ForMember(t => t.ActivityTypeName, opts => opts.MapFrom(d => d.ActivityType.ActivityName));

            CreateMap<BugetPrevDobandaReferintaListDto, BVC_DobandaReferinta>();

            CreateMap<BVC_DobandaReferinta, BugetPrevDobandaReferintaEditDto>();
            CreateMap<BugetPrevDobandaReferintaEditDto, BVC_DobandaReferinta>();

            CreateMap<BVC_VenitTitluCFReinv, BVC_VenitTitluCFReinvDto>()
                .ForMember(t => t.CursValutar, opts=> opts.MapFrom(d => d.CursValutarEstimat));
            CreateMap<BVC_VenitTitluCFReinvDto, BVC_VenitTitluCFReinv>()
                .ForMember(t => t.CursValutarEstimat, opts => opts.MapFrom(d => d.CursValutar));

            CreateMap<BVC_RealizatRandDetails, BugetRealizatRowDetailDto>()
                .ForMember(t => t.CurrencyName, opts => opts.MapFrom(d => d.Currency.CurrencyName));
            CreateMap<BugetRealizatRowDetailDto, BVC_RealizatRandDetails>();

            CreateMap<BVC_VenitProcRepartiz, BugetRepartizatDto>()

                .ForMember(t => t.ActivityName, opts => opts.MapFrom(d => d.ActivityType.ActivityName));
            CreateMap<BugetRepartizatDto, BVC_VenitProcRepartiz>();

            CreateMap<BVC_Formular, BugetFormularListDto>();
            CreateMap<BugetFormularListDto, BVC_Formular>();

            CreateMap<PaapPrimesteDto, BVC_PAAP>();
            CreateMap<BVC_PAAP, PaapPrimesteDto>()
                .ForMember(t => t.Denumire, opts => opts.MapFrom(d => d.Departament.Name + " - " + d.InvoiceElementsDetailsCategory.CategoryElementDetName + " - " + d.Descriere + " - " +
                                                                 d.ValoareTotalaLei))
                .ForMember(t => t.CategorieId, opts => opts.MapFrom(d => d.InvoiceElementsDetailsCategoryId))
                .ForMember(t => t.ValoareInitiala, opts => opts.MapFrom(d => d.ValoareTotalaLei))
                .ForMember(t => t.PaapPrimesteId, opts =>opts.MapFrom(d => d.Id));

            CreateMap<PaapPierdeDto, BVC_PAAP>();
            CreateMap<BVC_PAAP, PaapPierdeDto>()
                .ForMember(t => t.Denumire, opts => opts.MapFrom(d => d.Departament.Name + " - " + 
                                                                 d.InvoiceElementsDetailsCategory.CategoryElementDetName + " - " + d.Descriere + " - " +
                                                                 d.ValoareTotalaLei))
                .ForMember(t => t.CategorieId, opts => opts.MapFrom(d => d.InvoiceElementsDetailsCategoryId))
                .ForMember(t => t.ValoareDisponibila, opts =>opts.MapFrom(d => d.ValoareTotalaLei))
                .ForMember(t => t.PaapPierdeId, opts =>opts.MapFrom(d => d.Id));

            CreateMap<BVC_PaapRedistribuireDetalii, PaapPierdeDto>();
            CreateMap<PaapPierdeDto, BVC_PaapRedistribuireDetalii>()
                .ForMember(t => t.SumaPierduta, opts => opts.MapFrom(d => d.ValoareRedistribuita))
                .ForMember(t => t.PaapCarePierdeId, opts => opts.MapFrom(d => d.PaapPierdeId));

            CreateMap<BVC_PaapRedistribuireDetalii, PaapRedistribuireDetaliiDto>()
                .ForMember(t => t.Denumire, opts => opts.MapFrom(d => d.PaapCarePierde.Departament.Name + " - " + 
                                                                 d.PaapCarePierde.InvoiceElementsDetailsCategory.CategoryElementDetName + " - " + 
                                                                 d.PaapCarePierde.Descriere));
            CreateMap<PaapRedistribuireDetaliiDto, BVC_PaapRedistribuireDetalii>();

            CreateMap<BVC_PaapRedistribuire, PaapPrimesteDto>();
            CreateMap<PaapPrimesteDto, BVC_PaapRedistribuire>()
                .ForMember(t => t.PaapCarePrimesteId, opts => opts.MapFrom(d => d.PaapPrimesteId))
                .ForMember(t => t.SumaPlatita, opts => opts.MapFrom(d => d.SumaPrimita));

            CreateMap<BVC_PaapRedistribuire, PaapRedistribuireListDto>()
                .ForMember(t => t.SumaPrimita, opts => opts.MapFrom(d => d.SumaPlatita))
                .ForMember(t => t.DenumireAchizitie, opts => opts.MapFrom(d => d.PaapCarePrimeste.Descriere))
                .ForMember(t => t.NumeCategorie, opts => opts.MapFrom(d => d.PaapCarePrimeste.InvoiceElementsDetailsCategory.CategoryElementDetName))
                .ForMember(t => t.NumeCompartiment, opts => opts.MapFrom(d => d.PaapCarePrimeste.Departament.Name));
            CreateMap<PaapRedistribuireListDto, BVC_PaapRedistribuire>();

            CreateMap<BVC_PaapRedistribuire, PaapRedistribuireDto>()
                .ForMember(t=>t.CategorieId, opts =>opts.MapFrom(d =>d.PaapCarePrimeste.InvoiceElementsDetailsCategoryId))
                .ForMember(t =>t.Denumire, opts =>opts.MapFrom(d => d.PaapCarePrimeste.Departament.Name + " - " +
                                                                 d.PaapCarePrimeste.InvoiceElementsDetailsCategory.CategoryElementDetName + " - " +
                                                                 d.PaapCarePrimeste.Descriere))
                .ForMember(t =>t.ValoareInitiala, opts => opts.MapFrom(d =>d.PaapCarePrimeste.ValoareTotalaLei))
                .ForMember(t => t.SumaPrimita, opts=>opts.MapFrom(d => d.SumaPlatita))
                .ForMember(t => t.PaapPrimesteId, opts =>opts.MapFrom(d =>d.PaapCarePrimesteId))
                .ForMember(t=>t.PaapRedistribuireDetaliiList, opts => opts.MapFrom(d => d.PaapRedistribuireDetalii));
            CreateMap<PaapRedistribuireDto, BVC_PaapRedistribuire>()
                .ForMember(t => t.PaapCarePrimesteId, opts=>opts.MapFrom(d => d.PaapPrimesteId))
                .ForMember(t => t.SumaPlatita, opts =>opts.MapFrom(d => d.SumaPrimita));

            CreateMap<Notificare, NotificareDto>()
                .ForMember(t => t.NotificareId, opts =>opts.MapFrom(d => d.Id));
            CreateMap<NotificareDto, Notificare>();

            #endregion

            #region HR
            CreateMap<Departament, DepartamentListDto>();
            CreateMap<DepartamentListDto, Departament>();
            #endregion

            #region CotaTVA
            CreateMap<CotaTVA, CotaTVAListDto>();
            //    .ForMember(t => t.VAT, opts => opts.MapFrom(d => d.VAT.ToString()));
            CreateMap<CotaTVAListDto, CotaTVA>();

            CreateMap<CotaTVA, CotaTVAEditDto>();
            CreateMap<CotaTVAEditDto, CotaTVA>();
            #endregion

            #region BNR
            CreateMap<BNR_Sector, BNR_SectorListDto>();
            CreateMap<BNR_Sector, BNR_SectorEditDto>();
            CreateMap<BNR_SectorEditDto, BNR_Sector>();

            CreateMap<BNR_RaportareRand, BNR_RaportareRowDto>()
                .ForMember(t => t.AnexaDetailName, opts => opts.MapFrom(d => d.BNR_AnexaDetail.DenumireRand))
                .ForMember(t => t.SectorName, opts => opts.MapFrom(d => d.BNR_Sector.Denumire));
            CreateMap<BNR_RaportareRowDto, BNR_RaportareRand>();

            CreateMap<BNR_RaportareRandDetail, BNR_RaportareRowDetailsDto>();
            CreateMap<BNR_RaportareRowDetailsDto, BNR_RaportareRandDetail>();
            #endregion

            #region AnexaBnr
            CreateMap<BNR_Anexa, BNR_AnexaDto>();
            CreateMap<BNR_AnexaDto, BNR_Anexa>();

            CreateMap<BNR_AnexaDetail, BNR_AnexaDetailDto>()
                .ForMember(t => t.TipTitlu, opts => opts.MapFrom(d => d.TipTitlu.ToString()));
            CreateMap<BNR_AnexaDetailDto, BNR_AnexaDetail>();
            #endregion

            #region Lichiditate
            CreateMap<LichidConfig, LichidConfigDto>();
            CreateMap<LichidConfigDto, LichidConfig>();

            CreateMap<LichidBenzi, LichidBenziDto>();
            CreateMap<LichidBenziDto, LichidBenzi>();

            CreateMap<LichidCalc, LichidCalcSavedBalancetDto>();
            CreateMap<LichidCalcSavedBalancetDto, LichidCalc>();

            CreateMap<LichidCalcDet, LichidCalcListDetDto>();
            CreateMap<LichidCalcListDetDto, LichidCalcDet>();
            #endregion

            

            //CreateMap<FileDocErrorDto, FileDocError>();
            CreateMap<FileDocError, FileDocErrorDto>();

        }
    }
}
