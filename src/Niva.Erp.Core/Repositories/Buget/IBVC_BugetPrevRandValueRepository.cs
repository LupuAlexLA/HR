using Abp.Domain.Repositories;
using Niva.Erp.Models.Buget;

namespace Niva.Erp.Repositories.Buget
{
    public interface IBVC_BugetPrevRandValueRepository: IRepository<BVC_BugetPrevRandValue, int>
    {
        void InsertList(BVC_BugetPrevRandValue randValueList);
    }
}
