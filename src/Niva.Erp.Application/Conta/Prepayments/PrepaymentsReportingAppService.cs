using Abp.Application.Services;
using Abp.Domain.Repositories;
using Niva.Erp.Models.PrePayments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Niva.Erp.Conta.Prepayments
{
    public interface IPrepaymentsReportingAppService : IApplicationService
    {
        PrepaymentsRegistruReport PrepaymentsRegReport(DateTime repDate, int prepaymentType);
    }

    public class PrepaymentsReportingAppService : ErpAppServiceBase, IPrepaymentsReportingAppService
    {
        IRepository<PrepaymentBalance> _prepaymentsBalanceRepository;
        IRepository<PrepaymentsDurationSetup> _prepaymentsDurationSetupRepository;

        public PrepaymentsReportingAppService(IRepository<PrepaymentBalance> prepaymentsBalanceRepository, IRepository<PrepaymentsDurationSetup> prepaymentsDurationSetupRepository)
        {
            _prepaymentsBalanceRepository = prepaymentsBalanceRepository;
            _prepaymentsDurationSetupRepository = prepaymentsDurationSetupRepository;
        }

        public PrepaymentsRegistruReport PrepaymentsRegReport(DateTime repDate, int prepaymentType)
        {
            var appClient = GetCurrentTenant();
            var _prepaymentType = (PrepaymentType)prepaymentType;

            var ret = new PrepaymentsRegistruReport();
            ret.AppClientName = appClient.LegalPerson.Name;
            ret.AppClientId1 = appClient.LegalPerson.Id1;
            ret.AppClientId2 = appClient.LegalPerson.Id2;
            ret.RegDate = repDate;
            ret.PrepaymentType = _prepaymentType;
            ret.ReportName = ((_prepaymentType == PrepaymentType.CheltuieliInAvans) ? " Cheltuieli " : " Venituri ") + " inregistrate in avans la data " + repDate.ToShortDateString();

            var setup = _prepaymentsDurationSetupRepository.GetAll().FirstOrDefault();
            PrepaymentDurationCalc modCalcul = (setup == null) ? PrepaymentDurationCalc.Lunar : setup.PrepaymentDurationCalc;
            ret.ModCalc = (int)modCalcul;


            ret.RegDetails = new List<PrepaymentsRegistruDetails>();

            var prepaymentsList = _prepaymentsBalanceRepository.GetAllIncluding(f => f.Prepayment)
                                                    .Where(f => f.ComputeDate <= repDate && f.Prepayment.PrepaymentType == _prepaymentType)
                                                    .GroupBy(f => f.PrepaymentId)
                                                    .Select(f => f.Max(x => x.Id))
                                                    .ToList();

            var stockList = _prepaymentsBalanceRepository.GetAllIncluding(f => f.Prepayment, f => f.Prepayment.PrepaymentAccount, f => f.Prepayment.PrepaymentAccountVAT, 
                                                                          f => f.Prepayment.PrimDocumentType, f => f.Prepayment.ThirdParty, f => f.Prepayment.ThirdParty)
                                                    .Where(f => prepaymentsList.Contains(f.Id) && f.Quantity != 0)
                                                    .ToList();
            var repList = new List<PrepaymentsRegistruDetails>();
            foreach (var item in stockList)
            {
                var listItem = new PrepaymentsRegistruDetails
                {
                    PrepaymentName = item.Prepayment.Description,
                    SyntheticAccount = item.Prepayment.PrepaymentAccount.Symbol,
                    AccountForGroup = item.Prepayment.PrepaymentAccount.Symbol + " - " + item.Prepayment.PrepaymentAccount.AccountName,
                    AccountName = item.Prepayment.PrepaymentAccount.AccountName,
                    PrepaymentValue = item.PrepaymentValue + item.Deprec,
                    RemainingPrepaymentValue = item.PrepaymentValue,
                    MonthlyDepreciation = item.MontlyCharge,
                    Depreciation = item.Deprec,
                    Duration = item.Prepayment.DurationInMonths,
                    RemainingDuration = item.Duration,
                    DocumentType = (item.Prepayment.PrimDocumentType != null) ? item.Prepayment.PrimDocumentType.TypeNameShort : "",
                    DocumentNr = item.Prepayment.PrimDocumentNr,
                    DocumentDate = item.Prepayment.PrimDocumentDate,
                    DepreciationStartDate = item.Prepayment.DepreciationStartDate,
                    ThirdParty = (item.Prepayment.ThirdParty != null) ? item.Prepayment.ThirdParty.FullName : ""
                };
                repList.Add(listItem);
                if (item.PrepaymentVAT != 0 || item.Prepayment.PrepaymentVAT != 0)
                {
                    var listItemVAT = new PrepaymentsRegistruDetails
                    {
                        PrepaymentName = item.Prepayment.Description,
                        SyntheticAccount = item.Prepayment.PrepaymentAccountVAT.Symbol,
                        AccountForGroup = item.Prepayment.PrepaymentAccountVAT.Symbol + " - " + item.Prepayment.PrepaymentAccountVAT.AccountName,
                        AccountName = item.Prepayment.PrepaymentAccountVAT.AccountName,
                        PrepaymentValue = item.PrepaymentVAT + item.DeprecVAT,
                        RemainingPrepaymentValue = item.PrepaymentVAT,
                        MonthlyDepreciation = item.MontlyChargeVAT,
                        Depreciation = item.DeprecVAT,
                        Duration = item.Prepayment.DurationInMonths,
                        RemainingDuration = item.Duration,
                        DocumentType = (item.Prepayment.PrimDocumentType != null) ? item.Prepayment.PrimDocumentType.TypeNameShort : "",
                        DocumentNr = item.Prepayment.PrimDocumentNr,
                        DocumentDate = item.Prepayment.PrimDocumentDate,
                        DepreciationStartDate = item.Prepayment.DepreciationStartDate,
                        ThirdParty = (item.Prepayment.ThirdParty != null) ? item.Prepayment.ThirdParty.FullName : ""
                    };
                    repList.Add(listItemVAT);
                }
            }



            ret.RegDetails = repList;

            return ret;
        }
    }
}
