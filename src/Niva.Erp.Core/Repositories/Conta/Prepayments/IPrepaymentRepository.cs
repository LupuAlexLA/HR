using Abp.Domain.Repositories;
using Niva.Erp.Models.PrePayments;
using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Repositories.Conta.Prepayments
{
    public interface IPrepaymentRepository : IRepository<Prepayment, int>
    {
        DateTime UnprocessedDate(PrepaymentType prepaymentType);

        DateTime LastProcessedDate(PrepaymentType prepaymentType);

        void GestPrepaymentsComputing(DateTime operationDate, PrepaymentType prepaymentType, DateTime lastBalanceDate);

        void GestPrepaymentDelComputing(DateTime operationDate, PrepaymentType prepaymentType);

        DateTime LastProcessedDateForPrepayment(PrepaymentType prepaymentType, int? prepaymentId);
        void GestComputingForPrepayment(DateTime operationDate, PrepaymentType prepaymentType, DateTime lastBalanceDate, int prepaymentId);
        void GestDelComputingForPrepayment(DateTime operationDate, PrepaymentType prepaymentType, int prepaymentId);
        void InsertOrUpdateV(Prepayment prepayment);
        void GestPrepayments(DateTime dataEnd, PrepaymentType prepaymentType, DateTime lastBalanceDate);

        List<Prepayment> PrepaymentList(PrepaymentType prepaymentType, DateTime computeDate);
    }
}
