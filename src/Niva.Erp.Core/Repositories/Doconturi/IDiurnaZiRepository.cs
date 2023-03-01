using Abp.Domain.Repositories;
using Niva.Erp.Models.Deconturi;

namespace Niva.Erp.Repositories.Doconturi
{
    public interface IDiurnaZiRepository: IRepository<DiurnaZi, int>
    {
        int GetDiurnaZiValue(int countryId);    
    }
}
