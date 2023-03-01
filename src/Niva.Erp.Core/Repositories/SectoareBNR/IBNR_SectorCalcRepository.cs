using Abp.Domain.Repositories;
using Niva.Erp.Models.SectoareBnr;

namespace Niva.Erp.Repositories.SectoareBNR
{
    public interface IBNR_SectorCalcRepository: IRepository<BNR_Conturi, int>
    {
        void CalcConturi(int balanceId);
    }
}
