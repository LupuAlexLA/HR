using Abp.Domain.Repositories;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.ConfigurareRapoarte;
using System;
using System.Collections.Generic;

namespace Niva.Erp.Repositories.Conta.ConfigurareRapoarte
{
    public interface IConfigReportRepository: IRepository<ReportConfig, int>
    {
        List<ReportCalcItem> CalcReportValue(ReportConfig item, int coloana, int reportId, List<ReportCalcItem> calcRap, DateTime reportDate, int tenantId,int currencyId, int localCurencyId, List<Report> rapoarteList, bool rulaj, List<ContaOperationDetail> contaOperList, List<BalanceDetailsDto> balanta);
        decimal CalcValCol(string formula, int coloana, int reportId, int id, List<ReportCalcItem> calcRap, DateTime reportDate, int tenantId,int currencyId, int localCurencyId, List<Report> rapoarteList, bool rulaj, List<ContaOperationDetail> contaOperList, List<BalanceDetailsDto> balanta, out bool calculat);
        decimal CalcFormula(string formula, int reportId, DateTime reportDate, int tenantId, int currencyId, int localCurencyId, List<ReportCalcItem> calcRap, List<Report> rapoarteList, bool rulaj, List<ContaOperationDetail> contaOperList, List<BalanceDetailsDto> balanta, out bool calculat);

    }
}
