using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Economic.Casierii;
using Niva.Erp.Repositories.Economic;
using System;
using System.Linq;

namespace Niva.Erp.EntityFrameworkCore.Repositories.Economic
{
    public class DispositionRepository : ErpRepositoryBase<Disposition, int>, IDispositionRepository
    {
        public DispositionRepository(IDbContextProvider<ErpDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        // Returnez urmatorul numar pentru dispozitie
        public int GetNextNumber(DateTime date)
        {
            var nextNumber = 0;
            var item = Context.Dispositions.Where(f => f.State == State.Active && f.DispositionDate.Year == date.Year)
                                                        .OrderByDescending(f => f.DispositionNumber)
                                                        .FirstOrDefault();
            if (item != null)
            {
                nextNumber = item.DispositionNumber + 1;
            }
            else
            {
                nextNumber = 1;
            }

            return nextNumber;
        }

        public void InsertOrUpdateV(Disposition disp)
        {
            var existingDisposition = Context.Dispositions.FirstOrDefault(f => f.Id == disp.Id && f.State == State.Active);
            if (existingDisposition == null) // INSERT
            {
                Insert(disp);
            }else
            {
                if (disp.OperationId != null)
                {
                    var operation = Context.Operations.FirstOrDefault(f => f.Id == disp.OperationId);
                    disp.OperationId = null;
                    Context.Operations.Remove(operation);
                }
                Context.Entry(existingDisposition).CurrentValues.SetValues(disp);
            }
            UnitOfWorkManager.Current.SaveChanges();
        }

        public decimal SoldPrec(DateTime previousDate, int currencyId)
        {
            decimal value = 0;
            DateTime startDate = new DateTime();
            var soldInit = Context.Dispositions.Where(f => f.State == State.Active && f.OperationType == OperationType.SoldInitial
                                                                 && f.DispositionDate <= previousDate && f.CurrencyId == currencyId)
                                                            .OrderByDescending(f => f.DispositionDate)
                                                          .FirstOrDefault();
            if (soldInit == null)
            {
                startDate = new DateTime(2000, 1, 1);
            }
            else
            {
                value = soldInit.SumOper;
                startDate = soldInit.DispositionDate;
            }
            var operValue = Context.Dispositions.AsNoTracking().Where(f => f.State == State.Active && f.OperationType != OperationType.SoldInitial
                                                                       && f.DispositionDate >= startDate && f.DispositionDate <= previousDate && f.CurrencyId == currencyId).ToList()
                                                           .Sum(f => f.SumOper);
            value += operValue;

            return value;
        }
    }
}
