using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.PrePayments;
using Niva.Erp.Repositories.Conta.Prepayments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Niva.Erp.EntityFrameworkCore.Repositories.Conta.Prepayments
{
    public class PrepaymentRepository : ErpRepositoryBase<Prepayment, int>, IPrepaymentRepository
    {
        public PrepaymentRepository(IDbContextProvider<ErpDbContext> context) : base(context)
        {

        }

        public DateTime UnprocessedDate(PrepaymentType prepaymentType)
        {
            DateTime ret;
            DateTime closingMonth = new DateTime();
            var prepaymentDate = Context.Prepayment
                                            .Where(f => f.State == State.Active && f.Processed == false)
                                            .OrderBy(f => f.PaymentDate)
                                            .FirstOrDefault();
            DateTime gestDate;
            var countStock = Context.PrepaymentBalance.Include(f => f.Prepayment).Count(f => f.Prepayment.PrepaymentType == prepaymentType);
            if (countStock == 0)
            {
                gestDate = (prepaymentDate == null) ? LazyMethods.Now() : prepaymentDate.PaymentDate;
            }
            else
            {
                gestDate = Context.PrepaymentBalance.Include(f => f.Prepayment)
                                                .Where(f => f.Prepayment.PrepaymentType == prepaymentType).Max(f => f.ComputeDate).AddDays(1);
            }
            ret = (prepaymentDate == null) ? LazyMethods.Now() : prepaymentDate.PaymentDate;

            DateTime currDate = (ret < gestDate) ? ret : gestDate;
            DateTime endDate = (ret < gestDate) ? gestDate : ret;
            closingMonth = currDate;
            while (currDate <= endDate)
            {
                if (currDate.Day == DateTime.DaysInMonth(currDate.Year, currDate.Month))
                {
                    closingMonth = currDate;
                    break;
                }
                currDate = currDate.AddDays(1);
            }
            closingMonth = (closingMonth == null) ? ret : closingMonth;
            ret = (ret < gestDate) ? gestDate : ret;
            ret = (ret > closingMonth) ? closingMonth : ret;

            return ret;
        }

        public DateTime LastProcessedDate(PrepaymentType prepaymentType)
        {
            DateTime ret;

            var count = Context.PrepaymentBalance.Include(f => f.Prepayment).Where(f => f.Prepayment.PrepaymentType == prepaymentType).Count();
            if (count != 0)
            {
                ret = Context.PrepaymentBalance.Include(f => f.Prepayment).Where(f => f.Prepayment.PrepaymentType == prepaymentType).Max(f => f.ComputeDate);
            }
            else
            {
                var prepaymentDate = Context.Prepayment
                                            .Where(f => f.State == State.Active && f.Processed == true)
                                            .OrderBy(f => f.PaymentDate)
                                            .FirstOrDefault();
                ret = (prepaymentDate == null) ? LazyMethods.Now().AddYears(-10) : prepaymentDate.PaymentDate.AddDays(-1);

            }


            return ret;
        }

        public void GestPrepaymentsComputing(DateTime operationDate, PrepaymentType prepaymentType, DateTime lastBalanceDate)
        {
            try
            {
                var unprocessedDate = UnprocessedDate(prepaymentType);
                var lastProcessedDate = LastProcessedDate(prepaymentType);
                var operList = OperationList(unprocessedDate, operationDate, prepaymentType);

                foreach (var item in operList)
                {
                    if (lastProcessedDate >= item.OperationDate)
                        throw new Exception("Data operatiunii trebuie sa fie mai mare decat ultima data procesata in gestiune: " + LazyMethods.DateToString(lastProcessedDate));


                    if (lastBalanceDate >= item.OperationDate)
                        throw new Exception("Data operatiunii trebuie sa fie mai mare decat data ultimei balante calculate: " + LazyMethods.DateToString(lastBalanceDate));

                    ComputeOperation(item, prepaymentType);
                    Context.SaveChanges();
                }

                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PrepaymentOperationForComputing> OperationList(DateTime lastProcessedDate, DateTime operationDate, PrepaymentType prepaymentType)
        {
            var ret = new List<PrepaymentOperationForComputing>();

            var prepaymentstDateList = Context.Prepayment
                                  .Where(f => f.State == State.Active && f.PaymentDate <= operationDate && !f.Processed)
                                  .ToList()
                                  .Select(f => new PrepaymentOperationForComputing
                                  {
                                      Id = f.Id,
                                      OperationDate = f.PaymentDate,
                                      OperationSource = PrepaymentOperationSource.Prepayments,
                                      OperationOrd = 1,
                                      OperationDateSort = new DateTime(f.PaymentDate.Year, f.PaymentDate.Month, f.PaymentDate.Day)
                                  })
                                  .ToList();

            var prepaymentstExitDateList = Context.Prepayment
                                  .Where(f => f.State == State.Active && f.EndDate <= operationDate && !f.ProcessedOut && f.EndDate != null)
                                  .ToList()
                                  .Select(f => new PrepaymentOperationForComputing
                                  {
                                      Id = f.Id,
                                      OperationDate = f.EndDate ?? LazyMethods.Now(),
                                      OperationSource = PrepaymentOperationSource.PrepaymentsOut,
                                      OperationOrd = 2,
                                      OperationDateSort = new DateTime(f.EndDate.Value.Year, f.EndDate.Value.Month, f.EndDate.Value.Day)
                                  })
                                  .ToList();

            var deprecDateList = GetDeprecList(lastProcessedDate, operationDate, prepaymentType);

            foreach (var item in prepaymentstDateList)
            {
                ret.Add(item);
            }

            foreach (var item in prepaymentstExitDateList)
            {
                ret.Add(item);
            }


            foreach (var item in deprecDateList)
            {
                ret.Add(item);
            }

            ret = ret.Distinct().OrderBy(f => f.OperationDateSort).ThenBy(f => f.OperationOrd).ToList();

            return ret;
        }

        public List<PrepaymentOperationForComputing> GetDeprecList(DateTime lastProcessedDate, DateTime operationDate, PrepaymentType prepaymentType)
        {
            var ret = new List<PrepaymentOperationForComputing>();
            DateTime currDate;
            var firstAssetDay = Context.Prepayment.Where(f => f.State == State.Active && f.PrepaymentType == prepaymentType).OrderBy(f => f.PaymentDate).FirstOrDefault();

            if (firstAssetDay == null)
            {
                return ret;
            }
            else
            {
                currDate = (lastProcessedDate > firstAssetDay.PaymentDate) ? lastProcessedDate : firstAssetDay.PaymentDate;
            }

            while (currDate <= operationDate)
            {
                if (currDate.Day == DateTime.DaysInMonth(currDate.Year, currDate.Month))
                {
                    ret.Add(new PrepaymentOperationForComputing
                    {
                        OperationDate = currDate,
                        OperationSource = PrepaymentOperationSource.AmortizareLunara,
                        OperationOrd = 3,
                        OperationDateSort = new DateTime(currDate.Year, currDate.Month, currDate.Day)
                    });
                }
                currDate = currDate.AddDays(1);
            }
            return ret;
        }

        public void ComputeOperation(PrepaymentOperationForComputing operation, PrepaymentType prepaymentType)
        {
            try
            {
                switch (operation.OperationSource)
                {
                    case PrepaymentOperationSource.Prepayments:
                        ComputePrepayment(operation, prepaymentType);
                        break;
                    case PrepaymentOperationSource.AmortizareLunara:
                        GestDeprecComputing(operation, prepaymentType);
                        break;
                    case PrepaymentOperationSource.PrepaymentsOut:
                        ComputeExitPrepayment(operation, prepaymentType);
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // procesare din Prepayment
        public void ComputePrepayment(PrepaymentOperationForComputing operation, PrepaymentType prepaymentType)
        {
            var prepayment = Context.Prepayment.FirstOrDefault(f => f.Id == operation.Id);
            int tranzQuantity = 0, quantity = 0, tranzDuration = 0, duration = 0;
            decimal tranzPaymentValue = 0, paymentValue = 0, tranzDeprec = 0, deprec = 0;
            decimal monthlyDepreciation;
            decimal tranzPaymentVAT = 0, paymentVAT = 0, tranzDeprecVAT = 0, deprecVAT = 0;
            decimal monthlyDepreciationVAT;
            var modCalcul = RetModCalcul(prepaymentType);


            tranzQuantity = 1;
            tranzDuration = prepayment.RemainingDuration ?? prepayment.DurationInMonths;
            tranzPaymentValue = prepayment.PrepaymentValue - (prepayment.Depreciation ?? 0);
            tranzDeprec = prepayment.Depreciation ?? 0;
            tranzPaymentVAT = prepayment.PrepaymentVAT - (prepayment.DepreciationVAT ?? 0);
            tranzDeprecVAT = prepayment.DepreciationVAT ?? 0;

            quantity += tranzQuantity;
            duration += tranzDuration;
            paymentValue += tranzPaymentValue;
            deprec += tranzDeprec;
            paymentVAT += tranzPaymentVAT;
            deprecVAT += tranzDeprecVAT;

            int decimalAmort = 2;
            var decDeprecSetup = Context.PrepaymentsDecDeprecSetup.FirstOrDefault(f => f.PrepaymentType == prepaymentType);
            if (decDeprecSetup != null)
            {
                decimalAmort = decDeprecSetup.DecimalAmort;
            }

            monthlyDepreciation = (prepayment.MontlyDepreciation ?? (tranzDuration != 0 ? decimal.Round(tranzPaymentValue / tranzDuration, decimalAmort) : 0));
            monthlyDepreciationVAT = (prepayment.MontlyDepreciationVAT ?? (tranzDuration != 0 ? decimal.Round(tranzPaymentVAT / tranzDuration, decimalAmort) : 0));


            var stock = new PrepaymentBalance
            {
                PrepaymentId = prepayment.Id,
                PrepaymentPFId = prepayment.Id,
                ComputeDate = new DateTime(prepayment.PaymentDate.Year, prepayment.PaymentDate.Month, prepayment.PaymentDate.Day),
                TranzQuantity = tranzQuantity,
                Quantity = quantity,
                TranzDuration = tranzDuration,
                Duration = duration,
                TranzPrepaymentValue = tranzPaymentValue,
                PrepaymentValue = paymentValue,
                TranzDeprec = tranzDeprec,
                Deprec = deprec,
                OperType = PrepaymentOperType.Constituire,
                MontlyCharge = monthlyDepreciation,
                TranzPrepaymentVAT = tranzPaymentVAT,
                PrepaymentVAT = paymentVAT,
                TranzDeprecVAT = tranzDeprecVAT,
                DeprecVAT = deprecVAT,
                MontlyChargeVAT = monthlyDepreciationVAT
            };

            Context.PrepaymentBalance.Add(stock);
            prepayment.Processed = true;
        }

        // calculez amortizarea
        public void GestDeprecComputing(PrepaymentOperationForComputing operation, PrepaymentType prepaymentType)
        {
            try
            {
                var modCalcul = RetModCalcul(prepaymentType);

                var prepaymentList = Context.PrepaymentBalance.Include(f => f.Prepayment)
                                                     .Where(f => f.Prepayment.PrepaymentType == prepaymentType)
                                                     .GroupBy(f => f.PrepaymentId)
                                                     .Select(f => new { AssetId = f.Key, Id = f.Max(x => x.Id) }).ToList();

                foreach (var item in prepaymentList)
                {
                    var stockItem = Context.PrepaymentBalance.Include(f => f.Prepayment).FirstOrDefault(f => f.Id == item.Id);

                    if (stockItem.Prepayment.DepreciationStartDate <= operation.OperationDate)
                    {
                        int tranzQuantity = 0, quantity = stockItem.Quantity, tranzDuration = 0, duration = stockItem.Duration;
                        decimal tranzPrepaymentValue = 0, prepaymentValue = stockItem.PrepaymentValue;
                        decimal tranzDeprec = 0, deprec = stockItem.Deprec;
                        decimal monthlyDepreciation = monthlyDepreciation = stockItem.MontlyCharge;
                        decimal tranzPrepaymentVAT = 0, prepaymentVAT = stockItem.PrepaymentVAT;
                        decimal tranzDeprecVAT = 0, deprecVAT = stockItem.DeprecVAT;
                        decimal monthlyDepreciationVAT = monthlyDepreciationVAT = stockItem.MontlyChargeVAT;


                        if (quantity != 0)
                        {
                            if (modCalcul == PrepaymentDurationCalc.Lunar) // calcul lunar
                            {
                                if (duration != 0)
                                {
                                    tranzDuration = -1;
                                    tranzDeprec = (duration == 1 ? prepaymentValue : monthlyDepreciation);
                                    tranzPrepaymentValue = -1 * tranzDeprec;
                                    tranzDeprecVAT = (duration == 1 ? prepaymentVAT : monthlyDepreciationVAT);
                                    tranzPrepaymentVAT = -1 * tranzDeprecVAT;
                                }
                                else
                                {
                                    tranzDuration = 0;
                                    tranzDeprec = 0;
                                    tranzPrepaymentValue = 0;
                                    tranzDeprecVAT = 0;
                                    tranzPrepaymentVAT = 0;
                                }
                            }
                            else // calcul zilnic
                            {
                                int nrZile = 0;
                                if (operation.OperationDate.Month == stockItem.Prepayment.DepreciationStartDate.Month
                                    && operation.OperationDate.Year == stockItem.Prepayment.DepreciationStartDate.Year)
                                {
                                    nrZile = operation.OperationDate.Day - stockItem.Prepayment.DepreciationStartDate.Day + 1;
                                }
                                else
                                {
                                    nrZile = operation.OperationDate.Day;
                                }
                                if (duration != 0)
                                {
                                    tranzDuration = -1 * ((duration < nrZile) ? duration : nrZile);
                                    tranzDeprec = (duration < nrZile ? prepaymentValue : Math.Round(nrZile * monthlyDepreciation, 2));
                                    tranzPrepaymentValue = -1 * tranzDeprec;
                                    tranzDeprecVAT = (duration < nrZile ? prepaymentVAT : Math.Round(nrZile * monthlyDepreciationVAT, 2));
                                    tranzPrepaymentVAT = -1 * tranzDeprecVAT;
                                }
                                else
                                {
                                    tranzDuration = 0;
                                    tranzDeprec = 0;
                                    tranzPrepaymentValue = 0;
                                    tranzDeprecVAT = 0;
                                    tranzPrepaymentVAT = 0;
                                }
                            }

                            quantity += tranzQuantity;
                            duration += tranzDuration;
                            prepaymentValue += tranzPrepaymentValue;
                            deprec += tranzDeprec;
                            prepaymentVAT += tranzPrepaymentVAT;
                            deprecVAT += tranzDeprecVAT;

                            var stock = new PrepaymentBalance
                            {
                                PrepaymentId = stockItem.PrepaymentId,
                                ComputeDate = new DateTime(operation.OperationDate.Year, operation.OperationDate.Month, operation.OperationDate.Day),
                                TranzQuantity = tranzQuantity,
                                Quantity = quantity,
                                TranzDuration = tranzDuration,
                                Duration = duration,
                                TranzPrepaymentValue = tranzPrepaymentValue,
                                PrepaymentValue = prepaymentValue,
                                TranzDeprec = tranzDeprec,
                                Deprec = deprec,
                                OperType = PrepaymentOperType.AmortizareLunara,
                                MontlyCharge = monthlyDepreciation,
                                TranzPrepaymentVAT = tranzPrepaymentVAT,
                                PrepaymentVAT = prepaymentVAT,
                                TranzDeprecVAT = tranzDeprecVAT,
                                DeprecVAT = deprecVAT,
                                MontlyChargeVAT = monthlyDepreciationVAT
                            };

                            Context.PrepaymentBalance.Add(stock);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // procesare din Prepayment
        public void ComputeExitPrepayment(PrepaymentOperationForComputing operation, PrepaymentType prepaymentType)
        {
            try
            {
                var prepaymentList = Context.PrepaymentBalance.Include(f => f.Prepayment)
                                                     .Where(f => f.Prepayment.PrepaymentType == prepaymentType && f.PrepaymentId == operation.Id)
                                                     .GroupBy(f => f.PrepaymentId)
                                                     .Select(f => new { AssetId = f.Key, Id = f.Max(x => x.Id) }).ToList();

                foreach (var item in prepaymentList)
                {
                    var prepayment = Context.Prepayment.FirstOrDefault(f => f.Id == operation.Id);

                    var stockItem = Context.PrepaymentBalance.FirstOrDefault(f => f.Id == item.Id);

                    int tranzQuantity = -1 * stockItem.Quantity, quantity = stockItem.Quantity, tranzDuration = -1 * stockItem.Duration, duration = 0;
                    decimal tranzPrepaymentValue = 0, prepaymentValue = stockItem.PrepaymentValue;
                    decimal tranzDeprec = 0, deprec = stockItem.Deprec;
                    decimal monthlyDepreciation = stockItem.MontlyCharge;
                    decimal tranzPrepaymentVAT = 0, prepaymentVAT = stockItem.PrepaymentVAT;
                    decimal tranzDeprecVAT = 0, deprecVAT = stockItem.DeprecVAT;
                    decimal monthlyDepreciationVAT = stockItem.MontlyChargeVAT;

                    if (tranzQuantity != 0)
                    {
                        tranzDeprec = -1 * deprec;
                        tranzPrepaymentValue = -1 * prepaymentValue;
                        tranzDeprec = -1 * deprecVAT;
                        tranzPrepaymentVAT = -1 * prepaymentVAT;


                        quantity += tranzQuantity;
                        duration += tranzDuration;
                        prepaymentValue += tranzPrepaymentValue;
                        deprec += tranzDeprec;
                        prepaymentVAT += tranzPrepaymentVAT;
                        deprecVAT += tranzDeprecVAT;

                        var stock = new PrepaymentBalance
                        {
                            PrepaymentId = stockItem.PrepaymentId,
                            PrepaymentPFId = prepayment.Id,
                            ComputeDate = new DateTime(operation.OperationDate.Year, operation.OperationDate.Month, operation.OperationDate.Day),
                            TranzQuantity = tranzQuantity,
                            Quantity = quantity,
                            TranzDuration = tranzDuration,
                            Duration = duration,
                            TranzPrepaymentValue = tranzPrepaymentValue,
                            PrepaymentValue = prepaymentValue,
                            TranzDeprec = tranzDeprec,
                            Deprec = deprec,
                            OperType = PrepaymentOperType.Iesire,
                            MontlyCharge = monthlyDepreciation,
                            TranzPrepaymentVAT = tranzPrepaymentVAT,
                            PrepaymentVAT = prepaymentVAT,
                            TranzDeprecVAT = tranzDeprecVAT,
                            DeprecVAT = deprecVAT,
                            MontlyChargeVAT = monthlyDepreciationVAT
                        };

                        Context.PrepaymentBalance.Add(stock);
                        prepayment.ProcessedOut = true;

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GestPrepaymentDelComputing(DateTime operationDate, PrepaymentType prepaymentType)
        {
            var _gestList = Context.PrepaymentBalance.Include(f => f.Prepayment)
                                .Where(f => f.ComputeDate >= operationDate && f.Prepayment.PrepaymentType == prepaymentType)
                                .OrderByDescending(f => f.ComputeDate).ThenByDescending(f => f.Id).ToList();
            foreach (var gest in _gestList)
            {
                //var processedOper = Context.AutoOperationDetail.Include(f => f.AutoOper)
                //                       .Where(f => f.AutoOper.AutoOperType == AutoOperationType.CheltuieliInAvans || f.AutoOper.AutoOperType == AutoOperationType.VenituriInAvans)
                //                       .Where(f => f.OperationalId == gest.Id)
                //                       .Count();
                //if (processedOper != 0)
                //{
                //    throw new Exception("Pentru aceasta operatie s-au generat notele contabile!");
                //}

                var prepaymentId = gest.PrepaymentId;
                if (gest.OperType == PrepaymentOperType.Constituire)
                {
                    var assetPF = Context.Prepayment.FirstOrDefault(f => f.Id == gest.PrepaymentPFId);
                    assetPF.Processed = false;
                }
                else if (gest.OperType == PrepaymentOperType.Iesire)
                {
                    var assetPF = Context.Prepayment.FirstOrDefault(f => f.Id == gest.PrepaymentPFId);
                    assetPF.ProcessedOut = false;
                }

                Context.PrepaymentBalance.Remove(gest);
            }
            Context.SaveChanges();
        }

        public PrepaymentDurationCalc RetModCalcul(PrepaymentType prepaymentType)
        {
            PrepaymentDurationCalc ret;
            var setup = Context.PrepaymentsDurationSetup.FirstOrDefault(f => f.PrepaymentType == prepaymentType);
            if (setup == null)
            {
                ret = PrepaymentDurationCalc.Lunar;
            }
            else
            {
                ret = setup.PrepaymentDurationCalc;
            }
            return ret;
        }

        public void InsertOrUpdateV(Prepayment prepayment)
        {
            var existingPrepayment = Context.Prepayment
                                                       .Where(f => f.InvoiceDetailsId == prepayment.InvoiceDetailsId &&
                                                              f.InvoiceDetails.InvoicesId == prepayment.InvoiceDetails.InvoicesId && f.State == State.Active).FirstOrDefault();

            if (existingPrepayment != null)
            {
                prepayment.Id = existingPrepayment.Id;
                Context.Entry(existingPrepayment).CurrentValues.SetValues(prepayment);
            }
            else
            {
                Insert(prepayment);
            }
            UnitOfWorkManager.Current.SaveChanges();
        }

        // Procesare cheltuieli in avans din Modulul Facturi
        #region Cheltuieli in avans

        public DateTime LastProcessedDateForPrepayment(PrepaymentType prepaymentType, int? prepaymentId)
        {
            DateTime ret;

            var count = Context.PrepaymentBalance.Include(f => f.Prepayment).Where(f => f.Prepayment.PrepaymentType == prepaymentType && f.PrepaymentId == prepaymentId).Count();
            if (count != 0)
            {
                ret = Context.PrepaymentBalance.Include(f => f.Prepayment).Where(f => f.Prepayment.PrepaymentType == prepaymentType && f.PrepaymentId == prepaymentId).Max(f => f.ComputeDate);
            }
            else
            {
                var prepaymentDate = Context.Prepayment
                                            .Where(f => f.State == State.Active && f.Processed == true && f.Id == prepaymentId)
                                            .OrderBy(f => f.PaymentDate)
                                            .FirstOrDefault();
                ret = (prepaymentDate == null) ? LazyMethods.Now().AddYears(-10) : prepaymentDate.PaymentDate.AddDays(-1);

            }


            return ret;
        }

        public void ComputeOperationForPrepayment(PrepaymentOperationForComputing operation, PrepaymentType prepaymentType, int prepaymentId)
        {
            try
            {
                switch (operation.OperationSource)
                {
                    case PrepaymentOperationSource.Prepayments:
                        ComputePrepayment(operation, prepaymentType);
                        break;
                    case PrepaymentOperationSource.AmortizareLunara:
                        GestDeprecComputingForPrepayment(operation, prepaymentType, prepaymentId);
                        break;
                    case PrepaymentOperationSource.PrepaymentsOut:
                        ComputeExitPrepayment(operation, prepaymentType);
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GestComputingForPrepayment(DateTime operationDate, PrepaymentType prepaymentType, DateTime lastBalanceDate, int prepaymentId)
        {
            try
            {
                var lastProcessedDate = LastProcessedDateForPrepayment(prepaymentType, prepaymentId);//.AddDays(1);
                var operList = OperationListForPrepayment(lastProcessedDate, operationDate, prepaymentType, prepaymentId);

                foreach (var item in operList)
                {
                    if (lastProcessedDate >= item.OperationDate)
                        throw new Exception("Data operatiunii trebuie sa fie mai mare decat ultima data procesata in gestiune: " + LazyMethods.DateToString(lastProcessedDate));


                    if (lastBalanceDate >= item.OperationDate)
                        throw new Exception("Data operatiunii trebuie sa fie mai mare decat data ultimei balante calculate: " + LazyMethods.DateToString(lastBalanceDate));

                    ComputeOperationForPrepayment(item, prepaymentType, prepaymentId);
                    Context.SaveChanges();
                }

                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Calculez gestiunea pentru toate cheltuielile
        public void GestPrepayments(DateTime dataEnd, PrepaymentType prepaymentType, DateTime lastBalanceDate)
        {
            try
            {
                var prepayments = PrepaymentList(prepaymentType, dataEnd);


                foreach (var prepayment in prepayments)
                {
                    var lastProcessedDate = new DateTime();
                    lastProcessedDate = LastProcessedDateForPrepayment(prepayment.PrepaymentType, prepayment.Id).AddDays(1);

                    if (lastBalanceDate != null)
                    {
                        if (lastBalanceDate >= lastProcessedDate) 
                            lastProcessedDate = lastBalanceDate.AddDays(1);
                    }
                    
                    var operList = OperationListForPrepayment(lastProcessedDate, dataEnd, prepayment.PrepaymentType, prepayment.Id);

                    foreach (var item in operList)
                    {

                        if (lastProcessedDate > item.OperationDate)
                            throw new Exception("Data operatiunii trebuie sa fie mai mare decat ultima data procesata in gestiune: " + LazyMethods.DateToString(lastProcessedDate));


                        if (lastBalanceDate >= item.OperationDate)
                            throw new Exception("Data operatiunii trebuie sa fie mai mare decat data ultimei balante calculate: " + LazyMethods.DateToString(lastBalanceDate));

                        ComputeOperationForPrepayment(item, prepayment.PrepaymentType, prepayment.Id);
                        Context.SaveChanges();
                    }

                    Context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PrepaymentOperationForComputing> OperationListForPrepayment(DateTime lastProcessedDate, DateTime operationDate, PrepaymentType prepaymentType, int prepaymentId)
        {
            var ret = new List<PrepaymentOperationForComputing>();

            var prepaymentstDateList = Context.Prepayment
                                  .Where(f => f.State == State.Active && f.PaymentDate <= operationDate && !f.Processed && f.Id == prepaymentId)
                                  .ToList()
                                  .Select(f => new PrepaymentOperationForComputing
                                  {
                                      Id = f.Id,
                                      OperationDate = f.PaymentDate,
                                      OperationSource = PrepaymentOperationSource.Prepayments,
                                      OperationOrd = 1,
                                      OperationDateSort = new DateTime(f.PaymentDate.Year, f.PaymentDate.Month, f.PaymentDate.Day)
                                  })
                                  .ToList();

            var prepaymentstExitDateList = Context.Prepayment
                                  .Where(f => f.State == State.Active && f.EndDate <= operationDate && !f.ProcessedOut && f.EndDate != null && f.Id == prepaymentId)
                                  .ToList()
                                  .Select(f => new PrepaymentOperationForComputing
                                  {
                                      Id = f.Id,
                                      OperationDate = f.EndDate ?? LazyMethods.Now(),
                                      OperationSource = PrepaymentOperationSource.PrepaymentsOut,
                                      OperationOrd = 2,
                                      OperationDateSort = new DateTime(f.EndDate.Value.Year, f.EndDate.Value.Month, f.EndDate.Value.Day)
                                  })
                                  .ToList();

            var deprecDateList = GetDeprecListForPrepayment(lastProcessedDate, operationDate, prepaymentType, prepaymentId);

            foreach (var item in prepaymentstDateList)
            {
                ret.Add(item);
            }

            foreach (var item in prepaymentstExitDateList)
            {
                ret.Add(item);
            }


            foreach (var item in deprecDateList)
            {
                ret.Add(item);
            }

            ret = ret.Distinct().OrderBy(f => f.OperationDateSort).ThenBy(f => f.OperationOrd).ToList();

            return ret;
        }

        public List<PrepaymentOperationForComputing> GetDeprecListForPrepayment(DateTime lastProcessedDate, DateTime operationDate, PrepaymentType prepaymentType, int prepaymentId)
        {
            var ret = new List<PrepaymentOperationForComputing>();
            DateTime currDate;
            var firstAssetDay = Context.Prepayment.Where(f => f.State == State.Active && f.PrepaymentType == prepaymentType && f.Id == prepaymentId).OrderBy(f => f.PaymentDate).FirstOrDefault();

            if (firstAssetDay == null)
            {
                return ret;
            }
            else
            {
                currDate = (lastProcessedDate > firstAssetDay.PaymentDate) ? lastProcessedDate : firstAssetDay.PaymentDate;
            }

            while (currDate <= operationDate)
            {
                if (currDate.Day == DateTime.DaysInMonth(currDate.Year, currDate.Month))
                {
                    ret.Add(new PrepaymentOperationForComputing
                    {
                        OperationDate = currDate,
                        OperationSource = PrepaymentOperationSource.AmortizareLunara,
                        OperationOrd = 3,
                        OperationDateSort = new DateTime(currDate.Year, currDate.Month, currDate.Day)
                    });
                }
                currDate = currDate.AddDays(1);
            }
            return ret;
        }


        // sterg gestiunea pana la data operatiunii pt cheltuiala afectata
        public void GestDelComputingForPrepayment(DateTime operationDate, PrepaymentType prepaymentType, int prepaymentId)
        {
            var _gestList = Context.PrepaymentBalance.Include(f => f.Prepayment)
                    .Where(f => f.ComputeDate >= operationDate && f.Prepayment.PrepaymentType == prepaymentType && f.PrepaymentId == prepaymentId)
                    .OrderByDescending(f => f.ComputeDate).ThenByDescending(f => f.Id).ToList();
            foreach (var gest in _gestList)
            {
                if (gest.OperType == PrepaymentOperType.Constituire)
                {
                    var assetPF = Context.Prepayment.FirstOrDefault(f => f.Id == gest.PrepaymentPFId);
                    assetPF.Processed = false;
                }
                else if (gest.OperType == PrepaymentOperType.Iesire)
                {
                    var assetPF = Context.Prepayment.FirstOrDefault(f => f.Id == gest.PrepaymentPFId);
                    assetPF.ProcessedOut = false;
                }

                Context.PrepaymentBalance.Remove(gest);
            }
            Context.SaveChanges();
        }

        public void GestDeprecComputingForPrepayment(PrepaymentOperationForComputing operation, PrepaymentType prepaymentType, int prepaymentId)
        {
            try
            {
                var modCalcul = RetModCalcul(prepaymentType);

                var prepaymentList = Context.PrepaymentBalance.Include(f => f.Prepayment)
                                                     .Where(f => f.Prepayment.PrepaymentType == prepaymentType && f.PrepaymentId == prepaymentId)
                                                     .GroupBy(f => f.PrepaymentId)
                                                     .Select(f => new { AssetId = f.Key, Id = f.Max(x => x.Id) }).ToList();

                foreach (var item in prepaymentList)
                {
                    var stockItem = Context.PrepaymentBalance.Include(f => f.Prepayment).FirstOrDefault(f => f.Id == item.Id);

                    if (stockItem.Prepayment.DepreciationStartDate <= operation.OperationDate)
                    {
                        int tranzQuantity = 0, quantity = stockItem.Quantity, tranzDuration = 0, duration = stockItem.Duration;
                        decimal tranzPrepaymentValue = 0, prepaymentValue = stockItem.PrepaymentValue;
                        decimal tranzDeprec = 0, deprec = stockItem.Deprec;
                        decimal monthlyDepreciation = monthlyDepreciation = stockItem.MontlyCharge;
                        decimal tranzPrepaymentVAT = 0, prepaymentVAT = stockItem.PrepaymentVAT;
                        decimal tranzDeprecVAT = 0, deprecVAT = stockItem.DeprecVAT;
                        decimal monthlyDepreciationVAT = monthlyDepreciationVAT = stockItem.MontlyChargeVAT;


                        if (quantity != 0)
                        {
                            if (modCalcul == PrepaymentDurationCalc.Lunar) // calcul lunar
                            {
                                if (duration != 0)
                                {
                                    tranzDuration = -1;
                                    tranzDeprec = (duration == 1 ? prepaymentValue : monthlyDepreciation);
                                    tranzPrepaymentValue = -1 * tranzDeprec;
                                    tranzDeprecVAT = (duration == 1 ? prepaymentVAT : monthlyDepreciationVAT);
                                    tranzPrepaymentVAT = -1 * tranzDeprecVAT;
                                }
                                else
                                {
                                    tranzDuration = 0;
                                    tranzDeprec = 0;
                                    tranzPrepaymentValue = 0;
                                    tranzDeprecVAT = 0;
                                    tranzPrepaymentVAT = 0;
                                }
                            }
                            else // calcul zilnic
                            {
                                int nrZile = 0;
                                if (operation.OperationDate.Month == stockItem.Prepayment.DepreciationStartDate.Month
                                    && operation.OperationDate.Year == stockItem.Prepayment.DepreciationStartDate.Year)
                                {
                                    nrZile = operation.OperationDate.Day - stockItem.Prepayment.DepreciationStartDate.Day + 1;
                                }
                                else
                                {
                                    nrZile = operation.OperationDate.Day;
                                }
                                if (duration != 0)
                                {
                                    tranzDuration = -1 * ((duration < nrZile) ? duration : nrZile);
                                    tranzDeprec = (duration < nrZile ? prepaymentValue : Math.Round(nrZile * monthlyDepreciation, 2));
                                    tranzPrepaymentValue = -1 * tranzDeprec;
                                    tranzDeprecVAT = (duration < nrZile ? prepaymentVAT : Math.Round(nrZile * monthlyDepreciationVAT, 2));
                                    tranzPrepaymentVAT = -1 * tranzDeprecVAT;
                                }
                                else
                                {
                                    tranzDuration = 0;
                                    tranzDeprec = 0;
                                    tranzPrepaymentValue = 0;
                                    tranzDeprecVAT = 0;
                                    tranzPrepaymentVAT = 0;
                                }
                            }

                            quantity += tranzQuantity;
                            duration += tranzDuration;
                            prepaymentValue += tranzPrepaymentValue;
                            deprec += tranzDeprec;
                            prepaymentVAT += tranzPrepaymentVAT;
                            deprecVAT += tranzDeprecVAT;

                            var stock = new PrepaymentBalance
                            {
                                PrepaymentId = stockItem.PrepaymentId,
                                ComputeDate = new DateTime(operation.OperationDate.Year, operation.OperationDate.Month, operation.OperationDate.Day),
                                TranzQuantity = tranzQuantity,
                                Quantity = quantity,
                                TranzDuration = tranzDuration,
                                Duration = duration,
                                TranzPrepaymentValue = tranzPrepaymentValue,
                                PrepaymentValue = prepaymentValue,
                                TranzDeprec = tranzDeprec,
                                Deprec = deprec,
                                OperType = PrepaymentOperType.AmortizareLunara,
                                MontlyCharge = monthlyDepreciation,
                                TranzPrepaymentVAT = tranzPrepaymentVAT,
                                PrepaymentVAT = prepaymentVAT,
                                TranzDeprecVAT = tranzDeprecVAT,
                                DeprecVAT = deprecVAT,
                                MontlyChargeVAT = monthlyDepreciationVAT
                            };

                            Context.PrepaymentBalance.Add(stock);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Prepayment> PrepaymentList(PrepaymentType prepaymentType, DateTime computeDate)
        {
            var prepayments = new List<Prepayment>();
            // cheltuieli neprocesate in gestiune
            var prepaymentsList = Context.Prepayment
                                        .Where(f => f.PrepaymentType == prepaymentType && f.Processed == false && f.State == State.Active)
                                        .ToList();
            // cheltuieli cu sold != 0 la data computeDate
            var prepaymentInStock = Context.PrepaymentBalance.Include(f => f.Prepayment)
                                                    .Where(f => f.ComputeDate <= computeDate && f.Prepayment.PrepaymentType == prepaymentType)
                                                    .GroupBy(f => f.PrepaymentId)
                                                    .Select(f => f.Max(x => x.Id))
                                                    .ToList();

            var stockList = Context.PrepaymentBalance.Include(f => f.Prepayment)
                                                    .Where(f => f.Quantity != 0 && prepaymentInStock.Contains(f.Id))
                                                    .ToList();

            foreach (var item in prepaymentsList)
            {
                prepayments.Add(item);
            }

            foreach (var item in stockList)
            {
                prepayments.Add(item.Prepayment);
            }

            return prepayments;
        }


        #endregion

    }

    public class PrepaymentOperationForComputing
    {
        public int? Id { get; set; }

        public DateTime OperationDate { get; set; }

        public PrepaymentOperationSource OperationSource { get; set; }

        public int OperationOrd { get; set; }

        public DateTime OperationDateSort { get; set; }

    }

    public enum PrepaymentOperationSource
    {
        Prepayments,
        PrepaymentsOut,
        AmortizareLunara
    }
}
