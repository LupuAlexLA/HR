using Abp.Domain.Repositories;
using Niva.Erp.Models.SectoareBnr;
using System.Collections.Generic;

namespace Niva.Erp.Repositories.SectoareBNR
{
   public interface IBNR_RaportareRepository: IRepository<BNR_Raportare, int>
    {
        void SaveRaportareRows(int savedBalanceId);
        void SaveRaportareRowDetails(int savedBalanceId);
        void InsertRanduriDetailList(List<BNR_RaportareRandDetail> randDetails, int savedBalanceId);

        void CalculTotaluri(List<BNR_AnexaDetail> anexaDetails);
        void CalculTotaluriRaportare(int savedBalanceId);
        decimal CalculFormulaAnexa3(string formula, int savedBalanceId);
        decimal CalculFormulaAnexa4(string formula, int savedBalanceId);
        void DeleteRaportare(int savedBalanceId);
        void DesfacFormula(string formula, out List<string> semneList, out List<string> splitItem);
    }
}
