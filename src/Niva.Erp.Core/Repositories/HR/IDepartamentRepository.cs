using Abp.Domain.Repositories;
using Niva.Erp.Models.HR;

namespace Niva.Erp.Repositories.HR
{
    public interface IDepartamentRepository : IRepository<Departament, int>
    {
        void InsertOrUpdateV(Departament compartiment);
    }
}
