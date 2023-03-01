using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.PrePayments;
using Niva.Erp.Repositories;
using Niva.Erp.Repositories.Conta;
using Niva.Erp.Repositories.Conta.AutoOperation;
using Niva.Erp.Repositories.Conta.Prepayments;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Conta.Prepayments
{
    public interface IPrepaymentBalanceAppService : IApplicationService
    {
        PrepaymentsBalanceListDto InitForm(int prepaymentType);

        List<PrepaymentsDDDto> PrepaymentsListDD(DateTime? dataStart, DateTime? dataEnd, int prepaymentType);

        PrepaymentsBalanceListDto SerchGest(PrepaymentsBalanceListDto gest);

        PrepaymentsBalanceComputeListDto InitFormCompute(int prepaymentType);

        PrepaymentsBalanceComputeListDto SerchComputeOper(PrepaymentsBalanceComputeListDto gestCompute);

        PrepaymentsBalanceComputeListDto ComputeDateGest(PrepaymentsBalanceComputeListDto gestCompute);

        PrepaymentsBalanceDelListDto InitFormDel(int prepaymentType);

        PrepaymentsBalanceDelListDto SerchDateGest(PrepaymentsBalanceDelListDto gestDel);

        void DeleteDateGest(PrepaymentType prepaymentType, DateTime deleteDate);

    }

    public class PrepaymentBalanceAppService : ErpAppServiceBase, IPrepaymentBalanceAppService
    {
        IRepository<PrepaymentBalance> _prepaymentBalanceRepository;
        IOperationRepository _operationRepository;
        IBalanceRepository _balanceRepository;
        IPrepaymentRepository _prepaymentRepository;
        IRepository<PrepaymentDocType> _prepaymentOperDocTypeRepository;
        IAutoOperationRepository _autoOperationRepository;

        public PrepaymentBalanceAppService(IOperationRepository operationRepository, IBalanceRepository balanceRepository, IRepository<PrepaymentBalance> prepaymentBalanceRepository,
                                           IPrepaymentRepository prepaymentRepository, IRepository<PrepaymentDocType> prepaymentOperDocTypeRepository,
                                           IAutoOperationRepository autoOperationRepository)
        {
            _operationRepository = operationRepository;
            _balanceRepository = balanceRepository;
            _prepaymentBalanceRepository = prepaymentBalanceRepository;
            _prepaymentRepository = prepaymentRepository;
            _prepaymentOperDocTypeRepository = prepaymentOperDocTypeRepository;
            _autoOperationRepository = autoOperationRepository;
        }
        //[AbpAuthorize("Administrare.CheltAvans.CalculGestiune.Acces")]
        //[AbpAuthorize("Administrare.VenitAvans.CalculGestiune.Acces")]
        public PrepaymentsBalanceListDto InitForm(int prepaymentType)
        {
            var _prepaymentType = (PrepaymentType)prepaymentType;
            var ret = new PrepaymentsBalanceListDto
            {
                DataStart = _balanceRepository.BalanceDateNextDay(),
                DataEnd = LazyMethods.Now(),
                PrepaymentType = _prepaymentType,
                GestDetail = new List<PrepaymentsBalanceDetailListDto>()
            };
            ret = SerchGest(ret);

            return ret;
        }

        //[AbpAuthorize("Administrare.CheltAvans.CalculGestiune.Acces")]
        //[AbpAuthorize("Administrare.VenitAvans.CalculGestiune.Acces")]
        public List<PrepaymentsDDDto> PrepaymentsListDD(DateTime? dataStart, DateTime? dataEnd, int prepaymentType)
        {
            var _dataStart = (dataStart ?? LazyMethods.Now().AddMonths(-1));
            var _dataEnd = (dataEnd ?? LazyMethods.Now());
            var _prepaymentType = (PrepaymentType)prepaymentType;

            var ret = _prepaymentBalanceRepository.GetAllIncluding(f => f.Prepayment)
                                    .Where(f => f.ComputeDate >= _dataStart && f.ComputeDate <= _dataEnd && f.Prepayment.PrepaymentType == _prepaymentType)
                                    .OrderByDescending(f => f.ComputeDate)
                                    .ToList()
                                    .Select(f => new PrepaymentsDDDto
                                    {
                                        Id = f.PrepaymentId,
                                        Name = f.Prepayment.Description + " - " + f.Prepayment.PrimDocumentNr + " / " + f.Prepayment.PrimDocumentDate.Value.ToShortDateString(),
                                        DocumentNumber = f.Prepayment.PrimDocumentNr,
                                        DocumentDate = f.Prepayment.PrimDocumentDate
                                    })
                                    .Distinct()
                                    .OrderBy(f => f.Name)
                                    .ToList();
            ret = ret.GroupBy(f => f.Id).Select(f => f.First()).ToList();

            return ret;
        }

        //[AbpAuthorize("Administrare.CheltAvans.CalculGestiune.Acces")]
        //[AbpAuthorize("Administrare.VenitAvans.CalculGestiune.Acces")]
        public PrepaymentsBalanceListDto SerchGest(PrepaymentsBalanceListDto gest)
        {
            try
            {
                var _dataStart = (gest.DataStart ?? LazyMethods.Now().AddMonths(-1));
                var _dataEnd = (gest.DataEnd ?? LazyMethods.Now());

                var gestDetails = _prepaymentBalanceRepository.GetAllIncluding(f => f.Prepayment)
                                        .Where(f => f.ComputeDate >= _dataStart && f.ComputeDate <= _dataEnd && f.PrepaymentId == (gest.PrepaymentId ?? f.PrepaymentId)
                                               && f.Prepayment.PrepaymentType == gest.PrepaymentType)
                                         .OrderBy(f => f.ComputeDate).ThenBy(f => f.Prepayment.Description)
                                         .ToList()
                                         .Select(f => new PrepaymentsBalanceDetailListDto
                                         {
                                             Id = f.Id,
                                             GestDate = f.ComputeDate,
                                             Prepayment = f.Prepayment.Description + " - " + f.Prepayment.PrimDocumentNr + " / " + f.Prepayment.PrimDocumentDate.Value.ToShortDateString(),
                                             OperType = LazyMethods.EnumValueToDescription(f.OperType),
                                             TranzDuration = f.TranzDuration,
                                             TranzQuantity = f.TranzQuantity,
                                             TranzPaymentValue = f.TranzPrepaymentValue,
                                             TranzDeprec = f.TranzDeprec,
                                             Duration = f.Duration,
                                             Quantity = f.Quantity,
                                             PaymentValue = f.PrepaymentValue,
                                             Deprec = f.Deprec,
                                             TranzPaymentVAT = f.TranzPrepaymentVAT,
                                             PaymentVAT = f.PrepaymentVAT,
                                             TranzDeprecVAT = f.TranzDeprecVAT,
                                             DeprecVAT = f.DeprecVAT
                                         })
                                         .ToList();
                gest.GestDetail = gestDetails;
                return gest;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        //[AbpAuthorize("Administrare.CheltAvans.CalculGestiune.Acces")]
        //[AbpAuthorize("Administrare.VenitAvans.CalculGestiune.Acces")]
        public PrepaymentsBalanceComputeListDto InitFormCompute(int prepaymentType)
        {
            var _prepaymentType = (PrepaymentType)prepaymentType;
            var ret = new PrepaymentsBalanceComputeListDto
            {
                UnprocessedDate = _prepaymentRepository.UnprocessedDate(_prepaymentType),
                ComputeDate = LazyMethods.Now(),
                ShowCompute = false,
                PrepaymentType = _prepaymentType,
                OperationList = new List<PrepaymentsOperationListDto>()
            };
            ret = SerchComputeOper(ret);

            return ret;
        }

        //[AbpAuthorize("Administrare.CheltAvans.CalculGestiune.Acces")]
        //[AbpAuthorize("Administrare.VenitAvans.CalculGestiune.Acces")]
        public PrepaymentsBalanceComputeListDto SerchComputeOper(PrepaymentsBalanceComputeListDto gestCompute)
        {
            var _computeDate = (gestCompute.ComputeDate ?? LazyMethods.Now());
            var _unprocessedDate = gestCompute.UnprocessedDate;

            if (!_operationRepository.VerifyClosedMonth(_computeDate))
                throw new UserFriendlyException("Eroare", "Nu se poate inregistra operatia deoarece luna contabila este inchisa");

            var gestDetail = new List<PrepaymentsOperationListDto>();
            // operatii neprocesate din prepayment
            var operTypeInregistrare = (PrepaymentOperType)0;
            var operTypeExit = (PrepaymentOperType)2;
            var prepaymentDetail = _prepaymentRepository.GetAllIncluding(f => f.PrimDocumentType)
                                     .Where(f => f.PaymentDate <= _computeDate && f.Processed == false && f.State == State.Active
                                            && f.PrepaymentType == gestCompute.PrepaymentType)
                                     .OrderBy(f => f.PaymentDate)
                                     .ToList()
                                     .Select(f => new PrepaymentsOperationListDto
                                     {
                                         Prepayment = f.Description,
                                         DocumentNr = f.PrimDocumentNr,
                                         DocumentDate = f.PrimDocumentDate ?? LazyMethods.Now(),
                                         OperationType = LazyMethods.EnumValueToDescription(operTypeInregistrare),
                                         DocumentType = (f.PrimDocumentType != null) ? f.PrimDocumentType.TypeName : "",
                                         OperationDate = f.PaymentDate,
                                         Id = f.Id,
                                         OrdProcess = 1,
                                         OperationDateSort = new DateTime(f.PaymentDate.Year, f.PaymentDate.Month, f.PaymentDate.Day)
                                     })
                                     .Distinct()
                                     .ToList();

            var prepaymentExitDetail = _prepaymentRepository.GetAllIncluding(f => f.PrimDocumentType)
                                     .Where(f => f.EndDate <= _computeDate && f.ProcessedOut == false && f.State == State.Active && f.EndDate != null && f.PrepaymentType == gestCompute.PrepaymentType)
                                     .OrderBy(f => f.PaymentDate)
                                     .ToList()
                                     .Select(f => new PrepaymentsOperationListDto
                                     {
                                         Prepayment = f.Description,
                                         DocumentNr = f.PrimDocumentNr,
                                         DocumentDate = f.PrimDocumentDate ?? LazyMethods.Now(),
                                         OperationType = LazyMethods.EnumValueToDescription(operTypeExit),
                                         DocumentType = (f.PrimDocumentType != null) ? f.PrimDocumentType.TypeName : "",
                                         OperationDate = f.EndDate ?? LazyMethods.Now(),
                                         Id = f.Id,
                                         OrdProcess = 2,
                                         OperationDateSort = new DateTime(f.EndDate.Value.Year, f.EndDate.Value.Month, f.EndDate.Value.Day)
                                     })
                                     .Distinct()
                                     .ToList();

            // date de sfarsit de luna pentru amortizare
            var deprecDetail = new List<PrepaymentsOperationListDto>();

            var prepaymentsList = _prepaymentBalanceRepository.GetAllIncluding(f => f.Prepayment)
                                        .Where(f => f.Prepayment.PrepaymentType == gestCompute.PrepaymentType)
                                        .GroupBy(f => f.PrepaymentId)
                                        .Select(f => f.Max(x => x.Id))
                                        .ToList();

            var stockList = _prepaymentBalanceRepository.GetAllIncluding(f => f.Prepayment, f => f.Prepayment.PrepaymentAccount, f => f.Prepayment.PrepaymentAccountVAT,
                                                                          f => f.Prepayment.PrimDocumentType, f => f.Prepayment.ThirdParty, f => f.Prepayment.ThirdParty)
                                                    .Where(f => prepaymentsList.Contains(f.Id) && f.Quantity != 0 && f.Prepayment.PaymentDate <= gestCompute.ComputeDate)
                                                    .ToList();

            var minLastProcessedDate = (stockList.Count > 0 ? stockList.Min(f => f.ComputeDate) : _unprocessedDate);

            var currDate = minLastProcessedDate.AddDays(1);

            while (currDate <= _computeDate)
            {
                if (currDate.Day == DateTime.DaysInMonth(currDate.Year, currDate.Month))
                {
                    var docType = _prepaymentOperDocTypeRepository.GetAllIncluding(f => f.DocumentType)
                                                                  .FirstOrDefault(f => f.OperType == PrepaymentOperType.AmortizareLunara);
                    var deprec = new PrepaymentsOperationListDto
                    {
                        Prepayment = "",
                        DocumentType = docType.DocumentType.TypeName,
                        DocumentDate = currDate,
                        OperationDate = currDate,
                        OperationType = LazyMethods.EnumValueToDescription(PrepaymentOperType.AmortizareLunara),
                        OrdProcess = 3,
                        OperationDateSort = currDate
                    };
                    deprecDetail.Add(deprec);
                }
                currDate = currDate.AddDays(1);
            }

            foreach (var item in prepaymentDetail)
            {
                gestDetail.Add(item);
            }
            foreach (var item in prepaymentExitDetail)
            {
                gestDetail.Add(item);
            }
            foreach (var item in deprecDetail)
            {
                gestDetail.Add(item);
            }

            gestDetail = gestDetail.OrderBy(f => f.OperationDateSort).ThenBy(f => f.OrdProcess).ToList();

            gestCompute.OperationList = gestDetail;
            gestCompute.ShowCompute = true;
            return gestCompute;
        }

        //[AbpAuthorize("Administrare.CheltAvans.CalculGestiune.Acces")]
        //[AbpAuthorize("Administrare.VenitAvans.CalculGestiune.Acces")]
        public PrepaymentsBalanceComputeListDto ComputeDateGest(PrepaymentsBalanceComputeListDto gestCompute)
        {
            try
            {
                if (!_operationRepository.VerifyClosedMonth(gestCompute.ComputeDate ?? LazyMethods.Now()))
                    throw new UserFriendlyException("Eroare", "Nu se poate inregistra operatia deoarece luna contabila este inchisa");

                var lastBalanceDate = _balanceRepository.GetAll().Where(f => f.Status == State.Active)
                                                                 .OrderByDescending(f => f.BalanceDate)
                                                                 .FirstOrDefault().BalanceDate;

                var localCurrencyId = GetCurrentTenant().LocalCurrencyId.Value;

                _prepaymentRepository.GestPrepayments(gestCompute.ComputeDate ?? LazyMethods.Now(), gestCompute.PrepaymentType, lastBalanceDate);

                //Salvez nota contabila din gestiune
                _autoOperationRepository.AutoPrepaymentsOperationAdd(gestCompute.ComputeDate ?? LazyMethods.Now(), localCurrencyId, gestCompute.PrepaymentType, lastBalanceDate, null);
                gestCompute = SerchComputeOper(gestCompute);
                gestCompute.ShowCompute = false;
                return gestCompute;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        //[AbpAuthorize("Administrare.CheltAvans.CalculGestiune.Acces")]
        //[AbpAuthorize("Administrare.VenitAvans.CalculGestiune.Acces")]
        public PrepaymentsBalanceDelListDto InitFormDel(int prepaymentType)
        {
            var _prepaymentType = (PrepaymentType)prepaymentType;
            var ret = new PrepaymentsBalanceDelListDto
            {
                DataStart = LazyMethods.Now().AddMonths(-1),
                DataEnd = LazyMethods.Now(),
                PrepaymentType = _prepaymentType,
                GestDelDetail = new List<PrepaymentsBalanceDelDetailDto>()
            };
            ret = SerchDateGest(ret);

            return ret;
        }

        //[AbpAuthorize("Administrare.CheltAvans.CalculGestiune.Acces")]
        //[AbpAuthorize("Administrare.VenitAvans.CalculGestiune.Acces")]
        public PrepaymentsBalanceDelListDto SerchDateGest(PrepaymentsBalanceDelListDto gestDel)
        {
            var _dataStart = (gestDel.DataStart ?? LazyMethods.Now().AddMonths(-1));
            var _dataEnd = (gestDel.DataEnd ?? LazyMethods.Now());

            var gestDetail = _prepaymentBalanceRepository.GetAll()
                                     .Where(f => f.ComputeDate >= _dataStart && f.ComputeDate <= _dataEnd && f.Prepayment.PrepaymentType == gestDel.PrepaymentType)
                                     .Select(f => new PrepaymentsBalanceDelDetailDto
                                     {
                                         DateGest = f.ComputeDate
                                     })
                                     .Distinct()
                                     .OrderByDescending(f => f.DateGest)
                                     .ToList();
            gestDel.GestDelDetail = gestDetail;
            return gestDel;
        }

        //[AbpAuthorize("Administrare.CheltAvans.CalculGestiune.Acces")]
        //[AbpAuthorize("Administrare.VenitAvans.CalculGestiune.Acces")]
        public void DeleteDateGest(PrepaymentType prepaymentType, DateTime deleteDate)
        {
            if (!_operationRepository.VerifyClosedMonth(deleteDate))
                throw new UserFriendlyException("Eroare", "Nu se poate sterge operatia deoarece luna contabila este inchisa");

            try
            {
                _autoOperationRepository.DeleteUncheckedAutoOperation(deleteDate, prepaymentType);
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare", "Gestiunea nu poate fi stearsa, deoarece " + ex.Message);
            }

            try
            {
                _prepaymentRepository.GestPrepaymentDelComputing(deleteDate, prepaymentType);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", LazyMethods.GetErrMessage(ex));
            }
        }
    }
}
