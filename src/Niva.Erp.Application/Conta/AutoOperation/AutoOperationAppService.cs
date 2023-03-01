using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using AutoMapper;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Models.AutoOperation;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.ImoAsset;
using Niva.Erp.Models.InvObjects;
using Niva.Erp.Models.PrePayments;
using Niva.Erp.Repositories.Conta;
using Niva.Erp.Repositories.Conta.AutoOperation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Niva.Erp.Conta.AutoOperation
{
    public interface IAutoOperationAppService : IApplicationService
    {
        AutoOperationDto InitForm();

        AutoOperationDto SearchAutoOper(AutoOperationDto form);

        AutoOperationDto PrepareAdd(AutoOperationDto form);

        AutoOperationDto OperationsAdd(AutoOperationDto form);

        void DeleteOperations(int operationId);
    }

    public class AutoOperationAppService : ErpAppServiceBase, IAutoOperationAppService
    {
        IAutoOperationRepository _autoOperationRepository;
        IBalanceRepository _balanceRepository;
        IRepository<ImoAssetStock> _imoAssetStockRepository;
        IRepository<PrepaymentBalance> _prepaymentBalanceRepository;
        IRepository<AutoOperationDetail> _autoOperationDetailRepository;
        IRepository<InvObjectStock> _invObjectStockRepository;


        public AutoOperationAppService(IAutoOperationRepository autoOperationRepository, IRepository<AutoOperationDetail> autoOperationDetailRepository, IBalanceRepository balanceRepository,
                                       IRepository<ImoAssetStock> imoAssetStockRepository, IRepository<PrepaymentBalance> prepaymentBalanceRepository, IRepository<InvObjectStock> invObjectStockRepository)
        {
            _autoOperationRepository = autoOperationRepository;
            _autoOperationDetailRepository = autoOperationDetailRepository;
            _balanceRepository = balanceRepository;
            _imoAssetStockRepository = imoAssetStockRepository;
            _prepaymentBalanceRepository = prepaymentBalanceRepository;
            _invObjectStockRepository = invObjectStockRepository;
        }
        //[AbpAuthorize("Admin.Conta.ContariAutomate.Acces")]
        public AutoOperationDto InitForm()
        {
            var currDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            var ret = new AutoOperationDto
            {
                AutoOperType = 0,
                StartDate = _balanceRepository.BalanceDateNextDay(),
                EndDate = currDate,
                ShowSearchForm = true,
                ShowCompute = false,
                ShowResults = false,
                ShowSummary = false,
                AddStartDate = _balanceRepository.BalanceDateNextDay(),
                AddEndDate = currDate,
            };
            ret = SearchAutoOper(ret);

            return ret;
        }
        //[AbpAuthorize("Admin.Conta.ContariAutomate.Acces")]
        public AutoOperationDto SearchAutoOper(AutoOperationDto form)
        {
            var list = _autoOperationRepository.GetAllIncluding(f => f.Currency, f => f.DocumentType)
                                               .Where(f => f.State == State.Active && f.OperationDate <= form.EndDate && f.OperationDate >= form.StartDate
                                                      && f.AutoOperType == (AutoOperationType)(form.AutoOperType ?? -1))
                                               .OrderBy(f => f.OperationDate)
                                               .ToList()
                                               .Select(f => new AutoOperationSummaryDto
                                               {
                                                   Id = f.Id,
                                                   OperationDate = f.OperationDate,
                                                   AutoOperType = (int)f.AutoOperType,
                                                   OperationType = ((AutoOperationType)form.AutoOperType == AutoOperationType.MijloaceFixe) ?
                                                                   LazyMethods.EnumValueToDescription((ImoAssetOperType)((int)f.OperationType)) :
                                                                   ((AutoOperationType)form.AutoOperType == AutoOperationType.ObiecteDeInventar) ?
                                                                   LazyMethods.EnumValueToDescription((InvObjectOperType)((int)f.OperationType)) :
                                                                     LazyMethods.EnumValueToDescription(((PrepaymentOperType)((int)f.OperationType))),
                                                   OperationTypeId = (int)f.OperationType,
                                                   CurrencyId = f.CurrencyId,
                                                   Currency = f.Currency.CurrencyCode,
                                                   DocumentType = f.DocumentType.TypeNameShort,
                                                   DocumentDate = f.DocumentDate,
                                                   DocumentNumber = f.DocumentNumber,
                                                   ShowDetail = false
                                               })
                                               .ToList();

            foreach (var item in list)
            {
                var detailList = _autoOperationDetailRepository.GetAllIncluding(f => f.DebitAccount, f => f.CreditAccount)
                                                               .Where(f => f.AutoOperId == item.Id)
                                                               .OrderBy(f => f.Id)
                                                               .ToList()
                                                               .Select(f => new AutoOperationSummaryDetailDto
                                                               {
                                                                   OperationDetailId = f.OperationDetailId,
                                                                   DebitAccount = f.DebitAccount.Symbol + " - " + f.DebitAccount.AccountName,
                                                                   CreditAccount = f.CreditAccount.Symbol + " - " + f.CreditAccount.AccountName,
                                                                   Details = f.Details,
                                                                   Value = f.Value,
                                                                   ValueCurr = f.ValueCurr,
                                                                   OperationalId = f.OperationalId
                                                               })
                                                               .ToList();

                foreach (var detItem in detailList)
                {
                    if ((AutoOperationType)form.AutoOperType == AutoOperationType.MijloaceFixe)
                    {
                        var assetItem = _imoAssetStockRepository.GetAllIncluding(f => f.ImoAssetItem).FirstOrDefault(f => f.Id == detItem.OperationalId);
                        if (assetItem != null)
                        {
                            detItem.AssetName = assetItem.ImoAssetItem.Name;
                        }
                    }
                    else if ((AutoOperationType)form.AutoOperType == AutoOperationType.CheltuieliInAvans || (AutoOperationType)form.AutoOperType == AutoOperationType.VenituriInAvans)
                    {
                        var prepayment = _prepaymentBalanceRepository.GetAllIncluding(f => f.Prepayment).FirstOrDefault(f => f.Id == detItem.OperationalId);
                        if (prepayment != null)
                        {
                            detItem.AssetName = prepayment.Prepayment.Description;
                        }
                    }
                    else if ((AutoOperationType)form.AutoOperType == AutoOperationType.ObiecteDeInventar)
                    {
                        var invObjectItem = _invObjectStockRepository.GetAllIncluding(f => f.InvObjectItem).FirstOrDefault(f => f.Id == detItem.OperationalId);
                        if (invObjectItem != null)
                        {
                            detItem.AssetName = invObjectItem.InvObjectItem.Name;
                        }
                    }
                }

                item.SummaryDetail = detailList;
            }

            form.Summary = list;
            form.ShowComputeDetails = false;
            form.ShowSearchForm = true;
            form.ShowCompute = false;
            return form;
        }
        //[AbpAuthorize("Admin.Conta.ContariAutomate.Acces")]
        public AutoOperationDto PrepareAdd(AutoOperationDto form)
        {
            try
            {
                var localCurrencyId = GetCurrentTenant().LocalCurrencyId.Value;
                if (form.AutoOperType == null)
                {
                    throw new UserFriendlyException("Eroare", "Nu ati selectat categoria de obiecte!");
                }
                var autoOperType = (AutoOperationType)form.AutoOperType;

                // mijloace fixe
                if (autoOperType == AutoOperationType.MijloaceFixe)
                {
                    var mfList = _autoOperationRepository.ImoAssetPrepareAdd(form.AddStartDate, form.AddEndDate, localCurrencyId);
                    var prepareOper = ObjectMapper.Map<List<AutoOperationComputeDto>>(mfList)/*.OrderBy(f => f.OperationDate).ThenBy(f => f.GestId)*/;
                    form.PrepareCompute = prepareOper.ToList();
                }
                // prepayments
                else if (autoOperType == AutoOperationType.CheltuieliInAvans || autoOperType == AutoOperationType.VenituriInAvans)
                {
                    var prepaymentType = (autoOperType == AutoOperationType.CheltuieliInAvans) ? PrepaymentType.CheltuieliInAvans : PrepaymentType.VenituriInAvans;
                    var prepList = _autoOperationRepository.PrepaymentsPrepareAdd(form.AddStartDate, form.AddEndDate, localCurrencyId, prepaymentType);
                    var prepareOper = ObjectMapper.Map<List<AutoOperationComputeDto>>(prepList)/*.OrderBy(f => f.OperationDate).ThenBy(f => f.GestId)*/;
                    form.PrepareCompute = prepareOper.ToList();
                }
                form.ShowComputeDetails = true;
                form.ShowSearchForm = false;

                return form;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }
        //[AbpAuthorize("Admin.Conta.ContariAutomate.Acces")]
        public AutoOperationDto OperationsAdd(AutoOperationDto form)
        {
            try
            {
                var localCurrencyId = GetCurrentTenant().LocalCurrencyId.Value;
                var autoOperType = (AutoOperationType)form.AutoOperType;

                var lastBalanceDate = _balanceRepository.GetAll().Where(f => f.Status == State.Active)
                                                             .OrderByDescending(f => f.BalanceDate)
                                                             .FirstOrDefault().BalanceDate;

                // mijloace fixe
                if (autoOperType == AutoOperationType.MijloaceFixe)
                {
                    var prepareOper = ObjectMapper.Map<List<AutoOperationCompute>>(form.PrepareCompute).OrderBy(f => f.OperationDate).ThenBy(f => f.AccountSort).ThenBy(f => f.GestId).ToList();
                    _autoOperationRepository.ImoAssetOperationAdd(prepareOper, lastBalanceDate, autoOperType, localCurrencyId, null);
                }
                else if (autoOperType == AutoOperationType.CheltuieliInAvans || autoOperType == AutoOperationType.VenituriInAvans)
                {
                    var prepareOper = ObjectMapper.Map<List<AutoOperationCompute>>(form.PrepareCompute).OrderBy(f => f.OperationDate).ThenBy(f => f.AccountSort).ThenBy(f => f.GestId).ToList();
                    _autoOperationRepository.PrepaymentsOperationAdd(prepareOper, lastBalanceDate, autoOperType, localCurrencyId, null);
                }

                form = SearchAutoOper(form);
                return form;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }

        }
        //[AbpAuthorize("Admin.Conta.ContariAutomate.Acces")]
        public void DeleteOperations(int operationId)
        {
            var lastBalanceDate = _balanceRepository.GetAll().Where(f => f.Status == State.Active)
                                                             .OrderByDescending(f => f.BalanceDate)
                                                             .FirstOrDefault().BalanceDate;
            try
            {
                _autoOperationRepository.DeleteAutoOperation(operationId, lastBalanceDate);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare stergere note - " + ex.Message);
            }
        }
    }

}