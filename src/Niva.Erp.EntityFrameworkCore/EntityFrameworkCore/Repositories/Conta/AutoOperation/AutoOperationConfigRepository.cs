using Abp.EntityFrameworkCore;
using Niva.Erp.Models.AutoOperation;
using Niva.Erp.Repositories.Conta.AutoOperation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Niva.Erp.EntityFrameworkCore.Repositories.Conta.AutoOperation
{
    public class AutoOperationConfigRepository : ErpRepositoryBase<AutoOperationConfig, int>, IAutoOperationConfigRepository
    {
        public AutoOperationConfigRepository(IDbContextProvider<ErpDbContext> context) : base(context)
        {

        }

        public void SaveConfigToDb(List<AutoOperationConfig> configList)
        {
            try
            {
                // Update and Insert
                foreach (var detail in configList)
                {
                    if (detail.Id != 0)
                    {
                        var existingDetail = Context.AutoOperationConfig
                            .Where(c => c.Id == detail.Id)
                            .SingleOrDefault();

                        detail.TenantId = existingDetail.TenantId;

                        // Update child
                        Context.Entry(existingDetail).CurrentValues.SetValues(detail);
                    }
                    else
                    {
                        // Insert child
                        Context.AutoOperationConfig.Add(detail);
                    }
                }
                //Context.SaveChanges();
                UnitOfWorkManager.Current.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
