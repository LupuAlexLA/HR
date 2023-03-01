using Abp.Domain.Repositories;
using Niva.Erp.Models.InvObjects;

namespace Niva.Erp.Repositories.InvObjects
{
    public interface IInvObjectRepository : IRepository<InvObjectItem, int>
    {
        void UpdateInvObject(InvObjectItem inventoryObj);
    }
}
