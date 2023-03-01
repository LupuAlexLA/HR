using Abp.Domain.Repositories;
using Niva.Erp.Models.Deconturi;

namespace Niva.Erp.Repositories.Doconturi
{
    public interface IDiurnaLegalaRepository: IRepository<DiurnaLegala, int>
    {
        int GetDiurnalLegalaValue(int diurnaId);
    }
}
    