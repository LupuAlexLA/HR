using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Deconturi;
using Niva.Erp.Repositories.Doconturi;
using System.Linq;

namespace Niva.Erp.EntityFrameworkCore.Repositories.Deconturi
{
    public class DecontRepository : ErpRepositoryBase<Decont, int>, IDecontRepository
    {
        public DecontRepository(IDbContextProvider<ErpDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        public IQueryable<Decont> GetAllIncludeElemDet()
        {
            var ret = Context.Decont.Include(f => f.InvoiceDetails).ThenInclude(f => f.Invoices).Include(f => f.InvoiceDetails).ThenInclude(f => f.InvoiceElementsDetails).ThenInclude(f => f.InvoiceElementsDetailsCategory);
            return ret;
        }

        public int GetNextDecontNumber(int appClientId)
        {
            var nextNumber = 0;
            var decont = Context.Decont.Where(f => f.State == State.Active && f.TenantId == appClientId).OrderByDescending(f => f.DecontNumber).FirstOrDefault();

            if(decont != null)
            {
                nextNumber = decont.DecontNumber + 1;
            }
            else
            {
                nextNumber++;
            }

            return nextNumber;
        }
    }
}
