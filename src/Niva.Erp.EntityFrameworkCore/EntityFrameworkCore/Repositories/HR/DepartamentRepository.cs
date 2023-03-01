using Abp.EntityFrameworkCore;
using Niva.Erp.Models.HR;
using Niva.Erp.Repositories.HR;
using System.Linq;

namespace Niva.Erp.EntityFrameworkCore.Repositories.HR
{
    public class DepartamentRepository : ErpRepositoryBase<Departament, int>, IDepartamentRepository
    {
        public DepartamentRepository(IDbContextProvider<ErpDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
        public void InsertOrUpdateV(Departament compartiment)
        {
            var existingDepartament = Context.Departament.FirstOrDefault(f => f.IdExtern == compartiment.IdExtern);

            if (existingDepartament == null)
            {
                Insert(compartiment);
            }
            else
            {
                compartiment.Id = existingDepartament.Id;
                Context.Entry(existingDepartament).CurrentValues.SetValues(compartiment);
            }
            UnitOfWorkManager.Current.SaveChanges();
        }
    }
}
