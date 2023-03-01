using Abp.EntityFrameworkCore;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Deconturi;
using Niva.Erp.Repositories.Doconturi;
using System;
using System.Linq;

namespace Niva.Erp.EntityFrameworkCore.Repositories.Deconturi
{
    public class DiurnaZiRepository : ErpRepositoryBase<DiurnaZi, int>, IDiurnaZiRepository
    {
        public DiurnaZiRepository(IDbContextProvider<ErpDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public int GetDiurnaZiValue(int countryId)
        {
            

            var diurnaZi = Context.DiurnaZi.FirstOrDefault(f => f.CountryId == countryId && f.State == State.Active);

            if (diurnaZi == null)
            {
                throw new Exception("Pentru tara selectata nu a fost definita valoarea diurnei pe zi"); 
            }

            var ret = diurnaZi.Value;
            return ret;
        }
    }
}
