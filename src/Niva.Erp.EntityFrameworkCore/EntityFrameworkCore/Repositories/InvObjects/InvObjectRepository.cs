using Abp.EntityFrameworkCore;
using Niva.Erp.Models.InvObjects;
using Niva.Erp.Repositories.InvObjects;
using System.Linq;

namespace Niva.Erp.EntityFrameworkCore.Repositories.InvObjects
{
    public class InvObjectRepository : ErpRepositoryBase<InvObjectItem, int>, IInvObjectRepository
    {
        public InvObjectRepository(IDbContextProvider<ErpDbContext> context) : base(context)
        {

        }

        public void UpdateInvObject(InvObjectItem inventoryObj)
        {
            var _invObjectDb = Context.InvObjectItem.FirstOrDefault(f => f.Id == inventoryObj.Id);
            Context.Entry(_invObjectDb).CurrentValues.SetValues(inventoryObj);
            Context.SaveChanges();
        }
    }
}
