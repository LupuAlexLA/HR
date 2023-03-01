using Abp.EntityFrameworkCore;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Deconturi;
using Niva.Erp.Repositories.Doconturi;
using System.Linq;

namespace Niva.Erp.EntityFrameworkCore.Repositories.Deconturi
{
    public class DiurnaLegalaRepository : ErpRepositoryBase<DiurnaLegala, int>, IDiurnaLegalaRepository
    {
        public DiurnaLegalaRepository(IDbContextProvider<ErpDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public int GetDiurnalLegalaValue(int diurnaId)
        {

            var diurnaLegala = Context.DiurnaLegala.FirstOrDefault(f => f.State == State.Active && f.Id == diurnaId).Value;

            var ret = (int)(diurnaLegala * 2.5);
            return ret;
        }
    }
}