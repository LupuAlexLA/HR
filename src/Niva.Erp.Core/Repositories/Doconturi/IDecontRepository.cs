using Abp.Domain.Repositories;
using Niva.Erp.Models.Deconturi;
using System.Linq;

namespace Niva.Erp.Repositories.Doconturi
{
    public interface IDecontRepository : IRepository<Decont, int>
    {
        int GetNextDecontNumber(int appClientId);
        public IQueryable<Decont> GetAllIncludeElemDet();
    }
}
