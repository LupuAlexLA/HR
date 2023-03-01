using Abp.Domain.Repositories;
using Niva.Erp.Models.Conta.Lichiditate;
using System;
using System.Collections.Generic;

namespace Niva.Erp.Repositories.Conta.Lichiditate
{
    public interface ILichidCalcRepository : IRepository<LichidCalc, int>
    {
        void LichidCalc(int savedBalanceId, List<PlasamentLichiditateDto> plasamenteLichiditate);
        void LichidCalcTotaluri(int savedBalanceId);
        void DeleteLichidCalcTotal(int savedBalanceId);
        void DeleteLichidCalc(int savedBalanceId);

        void LichidCalcCurr(int savedBalanceId, List<PlasamentLichiditateDto> plasamenteLichiditate);
        void DeleteLichidCalcCurr(int savedBalanceId);
        void LichidCalcCurrTotaluri(int savedBalanceId);
        void DeleteLichidCalcCurrTotal(int savedBalanceId);
    }


    public class PlasamentLichiditateDto
    {
        public string idplasament { get; set; }
        public DateTime dataRulareRaport { get; set; }
        public decimal valoareContabila { get; set; }
        public DateTime maturityDate { get; set; }
        public decimal valoareCreanta { get; set; }
        public DateTime? maturitateCreanta { get; set; }
        public decimal valoareDepreciere { get; set; }
        public string tipPlasament { get; set; }
        public string tipFond { get; set; }
        public string moneda { get; set; }
        public DateTime startDate { get; set; }
        public decimal procentDobanda { get; set; }
        public string emitent { get; set; }
        public decimal valoareInvestita { get; set; }
        public string intervalLichiditate { get; set; }
        public string termen { get; set; }
    }


}
