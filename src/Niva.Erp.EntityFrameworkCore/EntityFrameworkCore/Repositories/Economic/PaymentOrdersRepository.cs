using Abp.EntityFrameworkCore;
using Niva.Erp.Models.Economic;
using Niva.Erp.Repositories.Economic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Niva.Erp.EntityFrameworkCore.Repositories.Economic
{
    public class PaymentOrdersRepository : ErpRepositoryBase<PaymentOrders, int>, IPaymentOrdersRepository
    {
        public PaymentOrdersRepository(IDbContextProvider<ErpDbContext> context) : base(context)
        {

        }
    }
}
