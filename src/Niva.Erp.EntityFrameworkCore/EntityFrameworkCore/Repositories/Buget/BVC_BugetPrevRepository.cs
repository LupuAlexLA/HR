using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Models.Buget;
using Niva.Erp.Repositories.Buget;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.EntityFrameworkCore.Repositories.Buget
{
    public class BVC_BugetPrevRepository : ErpRepositoryBase<BVC_BugetPrev, int>, IBVC_BugetPrevRepository
    {
        public BVC_BugetPrevRepository(IDbContextProvider<ErpDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public void InsertBVC_BugetPrevStatus(int bugetPrevId, DateTime statusDate, BVC_Status state)
        {
            var bvc_BugetPrev = Context.BVC_BugetPrev.FirstOrDefault(f => f.Id == bugetPrevId);
           
            var bvc_BugetPrevStatus = new BVC_BugetPrevStatus()
            {
                BugetPrevId = bugetPrevId,
                StatusDate = statusDate,
                Status = state
            };
            Context.BVC_BugetPrevStatus.Add(bvc_BugetPrevStatus);
            UnitOfWorkManager.Current.SaveChanges();
        }

        public void BVC_PrevAmortizariMF(int bugetPrevId, List<VenitRepartizProc> repartizareVenituri)
        {
            try
            {

                // iau departamentul pentru care trebuie sa fac inregistrarile
                var dept = Context.BVC_BugetPrevAutoValue.FirstOrDefault(f => f.TipRand == BVC_RowType.Amortizari);
                if (dept == null)
                {
                    throw new Exception("Nu ati specificat departamentul pentru care se vor inregistra cheltuielile cu amortizarile");
                }
                var deptId = dept.DepartamentId;
                var bvcPrev = Context.BVC_BugetPrev.Include(f => f.Formular).FirstOrDefault(f => f.Id == bugetPrevId);
                if (bvcPrev.BVC_Tip == BVC_Tip.CashFlow)
                {
                    return; // nu e cazul pentru cash flow
                }

                var randAmort = Context.BVC_FormRand.FirstOrDefault(f => f.FormularId == bvcPrev.FormularId && f.TipRand == BVC_RowType.Amortizari && f.AvailableBVC && !f.IsTotal);

                if (randAmort == null) // nu am amortizari definite => am terminat
                {
                    return;
                }

                var dataStart = new DateTime(bvcPrev.Formular.AnBVC, bvcPrev.MonthStart, 1);
                // iau ultimele informatii din gestiune
                var lastStockRecord = Context.ImoAssetStock.Include(f => f.ImoAssetItem)
                                                            .Where(f => f.StockDate <= dataStart)
                                                            .GroupBy(f => f.ImoAssetItemId)
                                                            .Select(f => f.Max(x => x.Id)).ToList();
                var stockValues = Context.ImoAssetStock.Where(f => lastStockRecord.Contains(f.Id)).Where(f => f.Duration != 0).OrderBy(f => f.ImoAssetItemId).ToList();

                var bvcPrevRand = Context.BVC_BugetPrevRand.FirstOrDefault(f => f.FormRandId == randAmort.Id && f.BugetPrevId == bugetPrevId && f.DepartamentId == deptId);

                var valori = Context.BVC_BugetPrevRandValue.Where(f => f.BugetPrevRandId == bvcPrevRand.Id && f.ValueType == BVC_PrevValueType.Manual).OrderBy(f => f.DataLuna).ToList(); // iau valorile manuale
                var firstMonth = valori.FirstOrDefault();

                var gestLastDate = stockValues.Max(f => f.StockDate);

                var lastDate = new DateTime();
                if (gestLastDate == LazyMethods.LastDayOfMonth(gestLastDate))
                {
                    lastDate = gestLastDate;
                }
                else
                {
                    lastDate = new DateTime(gestLastDate.Year, gestLastDate.Month, 1).AddDays(-1);
                }
                dataStart = LazyMethods.LastDayOfMonth(dataStart);
                var monthsBetweenLastStartDate = LazyMethods.MonthsBetween(lastDate, dataStart) - 1;

                var values = new decimal[13];
                foreach (var item in stockValues)
                {
                    var remainingDurationYear = item.Duration - monthsBetweenLastStartDate;
                    if (remainingDurationYear > 0)
                    {
                        int contor = 1;
                        while (remainingDurationYear > 0 && contor <= bvcPrev.MonthEnd)
                        {
                            values[contor] += item.MonthlyDepreciation;// Math.Round(item.InventoryValue / item.Duration, 2);
                            remainingDurationYear--;
                            contor++;
                        }
                    }
                }
                for (int i = bvcPrev.MonthStart; i <= bvcPrev.MonthEnd; i++)
                {
                    var valueDate = LazyMethods.LastDayOfMonth(new DateTime(dataStart.Year, i, 1));
                    foreach (var procVenit in repartizareVenituri)
                    {
                        var randValue = new BVC_BugetPrevRandValue
                        {
                            BugetPrevRandId = bvcPrevRand.Id,
                            DataOper = valueDate,
                            DataLuna = valueDate,
                            Description = "Amortizare Mijloace fixe",
                            ValueType = BVC_PrevValueType.Amortizari,
                            Value = Math.Round(values[i] * procVenit.ProcRepartizat, 2),
                            ActivityTypeId = procVenit.ActivityTypeId
                        };
                        Context.BVC_BugetPrevRandValue.Add(randValue);
                    }
                }
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void BVC_PrevAmortizariCA(int bugetPrevId, List<VenitRepartizProc> repartizareVenituri)
        {
            try
            {
                // iau departamentul pentru care trebuie sa fac inregistrarile
                var dept = Context.BVC_BugetPrevAutoValue.FirstOrDefault(f => f.TipRand == BVC_RowType.Amortizari);
                if (dept == null)
                {
                    throw new Exception("Nu ati specificat departamentul pentru care se vor inregistra cheltuielile cu amortizarile");
                }
                var deptId = dept.DepartamentId;
                var bvcPrev = Context.BVC_BugetPrev.Include(f => f.Formular).FirstOrDefault(f => f.Id == bugetPrevId);
                if (bvcPrev.BVC_Tip == BVC_Tip.CashFlow)
                {
                    return; // nu e cazul pentru cash flow
                }

                var randAmort = Context.BVC_FormRand.FirstOrDefault(f => f.FormularId == bvcPrev.FormularId && f.TipRand == BVC_RowType.Amortizari && f.AvailableBVC && !f.IsTotal);

                if (randAmort == null) // nu am amortizari definite => am terminat
                {
                    return;
                }

                var dataStart = new DateTime(bvcPrev.Formular.AnBVC, bvcPrev.MonthStart, 1);
                // iau ultimele informatii din gestiune
                var lastStockRecord = Context.PrepaymentBalance.Include(f=>f.Prepayment)
                                                               .Where(f => f.Prepayment.PrepaymentType == Models.PrePayments.PrepaymentType.CheltuieliInAvans 
                                                                           && f.ComputeDate <= dataStart)
                                                               .GroupBy(f => f.PrepaymentId)
                                                               .Select(f => f.Max(x => x.Id)).ToList();
                var stockValues = Context.PrepaymentBalance.Where(f => lastStockRecord.Contains(f.Id)).Where(f => f.Duration != 0).OrderBy(f=>f.PrepaymentId).ToList();

                var bvcPrevRand = Context.BVC_BugetPrevRand.FirstOrDefault(f => f.FormRandId == randAmort.Id && f.BugetPrevId == bugetPrevId && f.DepartamentId == deptId);

                var valori = Context.BVC_BugetPrevRandValue.Where(f => f.BugetPrevRandId == bvcPrevRand.Id && f.ValueType == BVC_PrevValueType.Manual).OrderBy(f => f.DataLuna).ToList(); // iau valorile manuale
                var firstMonth = valori.FirstOrDefault();

                var gestLastDate = stockValues.Max(f => f.ComputeDate);

                var lastDate = new DateTime();
                if (gestLastDate == LazyMethods.LastDayOfMonth(gestLastDate))
                {
                    lastDate = gestLastDate;
                }
                else
                {
                    lastDate = new DateTime(gestLastDate.Year, gestLastDate.Month, 1).AddDays(-1);
                }
                dataStart = LazyMethods.LastDayOfMonth(dataStart);
                var monthsBetweenLastStartDate = LazyMethods.MonthsBetween(lastDate, dataStart) - 1;

                var values = new decimal[13];
                foreach (var item in stockValues)
                {
                    var remainingDurationYear = item.Duration- monthsBetweenLastStartDate;
                    if (remainingDurationYear > 0)
                    {
                        int contor = 1;
                        while (remainingDurationYear > 0 && contor <= bvcPrev.MonthEnd)
                        {
                            values[contor] += item.MontlyCharge; // Math.Round(item.PrepaymentValue / item.Duration, 2);
                            remainingDurationYear--;
                            contor++;
                        }
                    }
                }
                var activityType = Context.ActivityTypes.FirstOrDefault(f => f.Status == Models.Conta.Enums.State.Active && f.MainActivity);

                for (int i = bvcPrev.MonthStart; i <= bvcPrev.MonthEnd; i++)
                {
                    var valueDate = LazyMethods.LastDayOfMonth(new DateTime(dataStart.Year, i, 1));
                    foreach (var procVenit in repartizareVenituri)
                    {
                        var randValue = new BVC_BugetPrevRandValue
                        {
                            BugetPrevRandId = bvcPrevRand.Id,
                            DataOper = valueDate,
                            DataLuna = valueDate,
                            Description = "Amortizare Cheltuieli in avans",
                            ValueType = BVC_PrevValueType.Amortizari,
                            Value = Math.Round(values[i] * procVenit.ProcRepartizat, 2),
                            ActivityTypeId = procVenit.ActivityTypeId
                        };
                        Context.BVC_BugetPrevRandValue.Add(randValue);
                    }
                }
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        } 

        public void BVC_PrevPAAP(int bugetPrevId, List<VenitRepartizProc> repartizareVenituri)
        {
            try
            {
                bool OkAdd = true;
                var bvcPrev = Context.BVC_BugetPrev.Include(f => f.Formular).FirstOrDefault(f => f.Id == bugetPrevId);
                var bvcFormRanduri = Context.BVC_FormRand.Include(f => f.DetaliiRand).Where(f => f.FormularId == bvcPrev.FormularId && f.AvailableBVC).ToList();
                var bvcFormRandDetalii = Context.BVC_FormRandDetails.Include(f => f.FormRand).Where(f => f.FormRand.FormularId == bvcPrev.FormularId && f.FormRand.AvailableBVC).ToList();
                var startDate = new DateTime(bvcPrev.Formular.AnBVC, bvcPrev.MonthStart, 1);
                var endDate = LazyMethods.LastDayOfMonth(new DateTime(bvcPrev.Formular.AnBVC, bvcPrev.MonthEnd, 1));

                var stateNotOkPaap = new List<int>();
                stateNotOkPaap.Add((int)PAAP_State.Anulat);
                stateNotOkPaap.Add((int)PAAP_State.Inregistrat);

                var paapList = Context.BVC_PAAP.Include(f=>f.PaapStateList).Include(f=>f.Transe)
                                               .Where(f => f.DataEnd >= startDate && f.DataEnd <= endDate && f.State == Models.Conta.Enums.State.Active)
                                               .ToList();
                paapList = paapList.Where(f => !stateNotOkPaap.Contains((int)f.GetPaapState))
                                   .ToList();

                foreach (var item in paapList)
                {
                    decimal valoarePaap = item.ValoareTotalaLei;
                    decimal durata = item.DurationInMonths ?? 0;

                    var rand = new BVC_BugetPrevRand();
                    var randDetail = new BVC_FormRandDetails();

                    // daca durata e 0, inregistrez direct pe tipul de cheltuiala
                    if (durata == 0)
                    {
                        randDetail = bvcFormRandDetalii.Where(f => f.TipRandCheltuialaId == item.InvoiceElementsDetailsId && !f.FormRand.IsTotal).FirstOrDefault();
                        if (randDetail != null)
                        {
                            rand = Context.BVC_BugetPrevRand.Include(f => f.ValueList)
                                                                .FirstOrDefault(f => f.BugetPrevId == bugetPrevId && f.FormRandId == randDetail.FormRandId && f.DepartamentId == item.DepartamentId);
                        }
                        else
                        {
                            OkAdd = false;
                        }
                    }
                    else // inregistrez in randul de amortizari
                    {
                        var formRand = bvcFormRanduri.FirstOrDefault(f => f.TipRand == BVC_RowType.Amortizari && !f.IsTotal);
                        var dept = Context.BVC_BugetPrevAutoValue.FirstOrDefault(f => f.TipRand == BVC_RowType.Amortizari);
                        if (formRand != null)
                        {
                            rand = Context.BVC_BugetPrevRand.Include(f => f.ValueList)
                                                                    .FirstOrDefault(f => f.BugetPrevId == bugetPrevId && f.FormRandId == formRand.Id && f.DepartamentId == dept.DepartamentId);
                        }
                        else
                        {
                            OkAdd = false;
                        }
                    }

                    if (OkAdd)
                    {
                        if (durata == 0)
                        {
                            var monthDate = LazyMethods.LastDayOfMonth(new DateTime(bvcPrev.Formular.AnBVC, item.DataEnd.Month, 1));
                            foreach (var procVenit in repartizareVenituri)
                            {
                                BVC_BugetPrevRandValue randValue = rand.ValueList.Where(f => f.ValueType == BVC_PrevValueType.PAAP && f.DataLuna == monthDate && f.ActivityTypeId == procVenit.ActivityTypeId)
                                                                                 .FirstOrDefault();
                                decimal valueActivity = Math.Round(valoarePaap * procVenit.ProcRepartizat, 2);
                                if (randValue == null)
                                {
                                    randValue = new BVC_BugetPrevRandValue
                                    {
                                        ActivityTypeId = procVenit.ActivityTypeId,
                                        DataLuna = monthDate,
                                        DataOper = monthDate,
                                        BugetPrevRandId = rand.Id,
                                        Description = "PAAP cheltuieli",
                                        TenantId = 1,
                                        ValueType = BVC_PrevValueType.PAAP,
                                        Value = valueActivity
                                    };
                                    Context.Add(randValue);
                                }
                                else
                                {
                                    randValue.Value += valueActivity;
                                }
                            }
                        }
                        else
                        {
                            var monthDate = LazyMethods.LastDayOfMonth(new DateTime(bvcPrev.Formular.AnBVC, item.DataEnd.Month, 1).AddMonths(1));
                            var contor = 1;
                            decimal valoareLunara = Math.Round(valoarePaap / durata, 2);
                            while (monthDate <= endDate && contor <= durata)
                            {
                                foreach (var procVenit in repartizareVenituri)
                                {
                                    BVC_BugetPrevRandValue randValue = rand.ValueList.Where(f => f.ValueType == BVC_PrevValueType.PAAP && f.DataLuna == monthDate && f.ActivityTypeId == procVenit.ActivityTypeId)
                                                                                     .FirstOrDefault();
                                    decimal valueActivity = Math.Round(valoareLunara * procVenit.ProcRepartizat, 2);
                                    if (randValue == null)
                                    {
                                        randValue = new BVC_BugetPrevRandValue
                                        {
                                            ActivityTypeId = procVenit.ActivityTypeId,
                                            DataLuna = monthDate,
                                            DataOper = monthDate,
                                            BugetPrevRandId = rand.Id,
                                            Description = "PAAP cheltuieli - amortizari",
                                            TenantId = 1,
                                            ValueType = BVC_PrevValueType.PAAP,
                                            Value = valueActivity
                                        };
                                        Context.Add(randValue);
                                    }
                                    else
                                    {
                                        randValue.Value += valueActivity;
                                    }
                                }
                                contor++;
                                monthDate = LazyMethods.LastDayOfMonth(monthDate.AddMonths(1));
                            }

                        }
                    }

                }
                Context.SaveChanges();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void BVC_CashFlowPAAP(int bugetPrevId, List<VenitRepartizProc> repartizareVenituri)
        {
            try
            {
                bool OkAdd = true;
                var bvcPrev = Context.BVC_BugetPrev.Include(f => f.Formular).FirstOrDefault(f => f.Id == bugetPrevId);
                var bvcFormRanduri = Context.BVC_FormRand.Include(f => f.DetaliiRand).Where(f => f.FormularId == bvcPrev.FormularId && f.AvailableCashFlow).ToList();
                var bvcFormRandDetalii = Context.BVC_FormRandDetails.Include(f => f.FormRand).Where(f => f.FormRand.FormularId == bvcPrev.FormularId && f.FormRand.AvailableCashFlow).ToList();
                var startDate = new DateTime(bvcPrev.Formular.AnBVC, bvcPrev.MonthStart, 1);
                var endDate = LazyMethods.LastDayOfMonth(new DateTime(bvcPrev.Formular.AnBVC, bvcPrev.MonthEnd, 1));

                var stateNotOkPaap = new List<int>();
                stateNotOkPaap.Add((int)PAAP_State.Anulat);
                stateNotOkPaap.Add((int)PAAP_State.Inregistrat);

                var activityType = Context.ActivityTypes.FirstOrDefault(f => f.Status == Models.Conta.Enums.State.Active && f.MainActivity);
                var paapIdList = Context.BVC_PAAPTranse.Where(f => f.DataTransa >= startDate && f.DataTransa <= endDate)
                                                       .Select(f => f.BVC_PAAPId)
                                                       .Distinct()
                                                       .ToList();

                var paapList = Context.BVC_PAAP.Include(f => f.PaapStateList).Include(f=>f.Transe)
                                               .Where(f => paapIdList.Contains(f.Id) && f.State == Models.Conta.Enums.State.Active && f.DataEnd >= startDate && f.DataEnd <= endDate)
                                               .ToList();
                paapList = paapList.Where(f => !stateNotOkPaap.Contains((int)f.GetPaapState))
                                   .ToList();

                foreach (var item in paapList)
                {
                    decimal valoarePaap = item.ValoareEstimataFaraTvaLei;
                    
                    var rand = new BVC_BugetPrevRand();
                    var randDetail = new BVC_FormRandDetails();

                    randDetail = bvcFormRandDetalii.Where(f => f.TipRandCheltuialaId == item.InvoiceElementsDetailsId).FirstOrDefault();
                    if (randDetail != null)
                    {
                        rand = Context.BVC_BugetPrevRand.Include(f => f.ValueList)
                                                            .FirstOrDefault(f => f.BugetPrevId == bugetPrevId && f.FormRandId == randDetail.FormRandId && f.DepartamentId == item.DepartamentId);


                        int nrTranse = item.NrTranse;
                        DateTime dataTransa = item.FirstInstalmentDate;
                        DateTime dataLuna = LazyMethods.LastDayOfMonth(item.FirstInstalmentDate);
                        decimal valoareTransa = Math.Round(valoarePaap / nrTranse, 2);

                        foreach(var transa in item.Transe.Where(f=>f.DataTransa <= endDate && f.DataTransa >= startDate).ToList())
                        {
                            dataLuna = LazyMethods.LastDayOfMonth(transa.DataTransa);
                            dataTransa = transa.DataTransa;
                            valoareTransa = transa.ValoareLei;

                            foreach (var procVenit in repartizareVenituri)
                            {
                                BVC_BugetPrevRandValue randValue = rand.ValueList.Where(f => f.ValueType == BVC_PrevValueType.PAAP && f.DataLuna == dataLuna && f.DataOper == dataTransa 
                                                                                        && f.ActivityTypeId == procVenit.ActivityTypeId)
                                                                                 .FirstOrDefault();
                                var valueActivity = Math.Round(valoareTransa * procVenit.ProcRepartizat, 2);
                                if (randValue == null)
                                {
                                    randValue = new BVC_BugetPrevRandValue
                                    {
                                        ActivityTypeId = procVenit.ActivityTypeId,
                                        DataLuna = dataLuna,
                                        DataOper = dataTransa,
                                        BugetPrevRandId = rand.Id,
                                        Description = "PAAP cheltuieli",
                                        TenantId = 1,
                                        ValueType = BVC_PrevValueType.PAAP,
                                        Value = valueActivity
                                    };
                                    Context.Add(randValue);
                                }
                                else
                                {
                                    randValue.Value += valueActivity;
                                }
                            }
                        }
                    }

                }
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public void BVC_PrevActive(List<ActiveBugetBVCDto> activeBugetList, int bugetPrevId)
        //{
        //    try
        //    {
        //        var bvcPrev = Context.BVC_BugetPrev.Include(f => f.Formular).FirstOrDefault(f => f.Id == bugetPrevId);
        //        var startDate = new DateTime(bvcPrev.Formular.AnBVC, 1, 1);
        //        var endDate = new DateTime(bvcPrev.Formular.AnBVC, 12, 31);
                
        //        var valuesList = new List<ActiveBugetPrepareAddDto>();
        //        var dept = Context.BVC_BugetPrevAutoValue.FirstOrDefault(f => f.TipRand == BVC_RowType.Venituri && f.TipRandVenit == BVC_RowTypeIncome.Dobanzi);
        //        if (dept == null)
        //        {
        //            throw new Exception("Nu ati specificat departamentul pentru care se vor inregistra veniturile din dobanzi");
        //        }
        //        var deptId = dept.DepartamentId;
        //        var randFormVenit = Context.BVC_FormRand.FirstOrDefault(f => f.FormularId == bvcPrev.FormularId && f.TipRand == BVC_RowType.Venituri && f.TipRandVenit == BVC_RowTypeIncome.Dobanzi
        //                                                                && !f.IsTotal && f.AvailableBVC);

        //        if (randFormVenit == null) // nu am venituri din dobanzi definite => am terminat
        //        {
        //            return;
        //        }
                
        //        var randVenit = Context.BVC_BugetPrevRand.Include(f => f.ValueList)
        //                                                            .FirstOrDefault(f => f.BugetPrevId == bugetPrevId && f.FormRandId == randFormVenit.Id && f.DepartamentId == dept.DepartamentId);

        //        var dobanziReferinta = Context.BVC_DobandaReferinta.Where(f => f.An == bvcPrev.Formular.AnBVC && f.State == Models.Conta.Enums.State.Active).ToList();

        //        var monedeList = activeBugetList.Select(f => f.moneda).Distinct().ToList();

        //        foreach (var moneda in monedeList)
        //        {
        //            decimal exchangeRate = 1;
        //            if (moneda != "RON")
        //            {
        //                var currencyItem = Context.Currency.FirstOrDefault(f => f.CurrencyCode == moneda);
        //                var exchangeRateItem = Context.ExchangeRateForecasts.FirstOrDefault(f => f.State == Models.Conta.Enums.State.Active && f.Year == bvcPrev.Formular.AnBVC 
        //                                                                                    && f.CurrencyId == currencyItem.Id);
        //                if (exchangeRateItem == null)
        //                {
        //                    throw new Exception("Nu ati completat cursul valutar estimat pentru " + moneda + " pentru anul " + bvcPrev.Formular.AnBVC.ToString());
        //                }
        //                exchangeRate = exchangeRateItem.ValoareEstimata;
        //            }
        //            var activeIdList = activeBugetList.Where(f=>f.moneda == moneda).Select(f => new { f.idplasament, f.tipFond }).Distinct().ToList();

        //            foreach (var activ in activeIdList)
        //            {
        //                decimal valoareActualizata = 0;
        //                decimal procentDobanda = 0;
        //                DateTime maturityDate = new DateTime();
        //                DateTime currDate = new DateTime();
        //                BVC_PlasamentType tipPlasament = new BVC_PlasamentType();

                        

        //                string tipFond = "";
        //                tipFond = activ.tipFond == "SGD" ? "FGDB" : activ.tipFond;
        //                var activityType = Context.ActivityTypes.FirstOrDefault(f => f.Status == Models.Conta.Enums.State.Active && f.ActivityName == tipFond);

        //                foreach (var dataItem in activeBugetList.Where(f => f.idplasament == activ.idplasament).OrderBy(f => f.data))
        //                {
        //                    maturityDate = dataItem.maturityDate;
        //                    var startMonth = new DateTime(dataItem.data.Year, dataItem.data.Month, 1);

        //                    switch (dataItem.tipPlasament)
        //                    {
        //                        case "D":
        //                            tipPlasament = BVC_PlasamentType.DepoziteBancare;
        //                            break;
        //                        case "OLB":
        //                            tipPlasament = BVC_PlasamentType.Obligatiuni;
        //                            break;
        //                        case "T":
        //                            tipPlasament = BVC_PlasamentType.TitluriDePlasament;
        //                            break;
        //                    }

        //                    if (dataItem.maturityDate > startMonth)
        //                    {
        //                        currDate = dataItem.data;
                                
        //                        var randValue = valuesList.FirstOrDefault(f => f.ActivityType == activityType.Id && f.DataLuna == currDate);
        //                        if (randValue == null)
        //                        {
        //                            randValue = new ActiveBugetPrepareAddDto
        //                            {
        //                                ActivityType = activityType.Id,
        //                                DataLuna = currDate,
        //                                Valoare = Math.Round(dataItem.valoareDobandaLuna * exchangeRate, 2)
        //                            };
        //                            valuesList.Add(randValue);
        //                        }
        //                        else
        //                        {
        //                            randValue.Valoare += Math.Round(dataItem.valoareDobandaLuna * exchangeRate, 2);
        //                        }


        //                        valoareActualizata = dataItem.valoarePlasament + dataItem.valoareDobandaLuna + dataItem.valoareDobandaCumulataPanaLaLuna;

        //                        // daca am scadenta inainte de sfarsitul perioadei reinvenstesc suma
        //                        if (dataItem.maturityDate <= currDate)
        //                        {
        //                            var procentDobandaItem = dobanziReferinta.FirstOrDefault(f => f.PlasamentType == tipPlasament);
        //                            if (procentDobandaItem == null)
        //                            {
        //                                throw new Exception("Nu ati specificat dobanzile de referinta pentru toate tipurile de plasamente");
        //                            }
        //                            procentDobanda = procentDobandaItem.Procent;
        //                            var nrZile = (decimal)(currDate - dataItem.maturityDate).TotalDays;

        //                            if (nrZile > 0)
        //                            {
        //                                var dobandaLuna = Math.Round(valoareActualizata * procentDobanda / 100 / nrZile / 360, 2);
        //                                randValue.Valoare += Math.Round(dobandaLuna * exchangeRate, 2);                                        
        //                            }
        //                        }
        //                    }
        //                }

        //                currDate = LazyMethods.LastDayOfMonth(currDate.AddMonths(1));

        //                // daca am scadenta inainte de sfarsitul perioadei reinvenstesc suma
        //                while (currDate <= endDate)
        //                {
        //                    var startMonthCurrDate = new DateTime(currDate.Year, currDate.Month, 1);
        //                    var procentDobandaItem = dobanziReferinta.FirstOrDefault(f => f.PlasamentType == tipPlasament);
        //                    if (procentDobandaItem == null)
        //                    {
        //                        throw new Exception("Nu ati specificat dobanzile de referinta pentru toate tipurile de plasamente");
        //                    }
        //                    procentDobanda = procentDobandaItem.Procent;
        //                    var nrZile = (decimal)(currDate - startMonthCurrDate).TotalDays;

        //                    if (nrZile > 0)
        //                    {
        //                        var dobandaLuna = Math.Round(valoareActualizata * procentDobanda / 100 / nrZile / 360, 2);
        //                        var randValue = valuesList.FirstOrDefault(f => f.ActivityType == activityType.Id && f.DataLuna == currDate);
        //                        if (randValue == null)
        //                        {
        //                            randValue = new ActiveBugetPrepareAddDto
        //                            {
        //                                ActivityType = activityType.Id,
        //                                DataLuna = currDate,
        //                                Valoare = Math.Round(dobandaLuna * exchangeRate, 2)
        //                            };
        //                            valuesList.Add(randValue);
        //                        }
        //                        else
        //                        {
        //                            randValue.Valoare += Math.Round(dobandaLuna * exchangeRate, 2);
        //                        }
        //                    }
        //                    currDate = LazyMethods.LastDayOfMonth(currDate.AddMonths(1));
        //                }
        //            }
        //        }

        //        // salvez valorile in db
        //        foreach(var item in valuesList)
        //        {
        //            var randValue = Context.BVC_BugetPrevRandValue.FirstOrDefault(f => f.BugetPrevRandId == randVenit.Id && f.ValueType == BVC_PrevValueType.Dobanzi && f.DataLuna == item.DataLuna
        //                                                                          && f.ActivityTypeId == item.ActivityType);

        //            if (randValue == null)
        //            {
        //                randValue = new BVC_BugetPrevRandValue
        //                {
        //                    ActivityTypeId = item.ActivityType,
        //                    DataLuna = item.DataLuna,
        //                    DataOper = item.DataLuna,
        //                    BugetPrevRandId = randVenit.Id,
        //                    Description = "Dobanzi titluri",
        //                    TenantId = 1,
        //                    ValueType = BVC_PrevValueType.Dobanzi,
        //                    Value = item.Valoare
        //                };
        //                Context.Add(randValue);
        //            }
        //            else
        //            {
        //                randValue.Value += item.Valoare;
        //            }
        //        }

        //        Context.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //}

        public void BVC_PrevContributii(int bugetPrevId)
        {
            try
            {
                var bvcPrev = Context.BVC_BugetPrev.Include(f => f.Formular).FirstOrDefault(f => f.Id == bugetPrevId);
                var startDate = new DateTime(bvcPrev.Formular.AnBVC, bvcPrev.MonthStart, 1);
                var endDate = LazyMethods.LastDayOfMonth(new DateTime(bvcPrev.Formular.AnBVC, bvcPrev.MonthEnd, 1));

                var valuesList = new List<ActiveBugetPrepareAddDto>();
                var dept = Context.BVC_BugetPrevAutoValue.FirstOrDefault(f => f.TipRand == BVC_RowType.Venituri && f.TipRandVenit == BVC_RowTypeIncome.Contributii);
                if (dept == null)
                {
                    throw new Exception("Nu ati specificat departamentul pentru care se vor inregistra veniturile din contributii");
                }
                var deptId = dept.DepartamentId;
                var randFormVenit = Context.BVC_FormRand.FirstOrDefault(f => f.FormularId == bvcPrev.FormularId && f.TipRand == BVC_RowType.Venituri && f.TipRandVenit == BVC_RowTypeIncome.Contributii
                                                                        && !f.IsTotal);
                if (randFormVenit == null) // nu am venituri din contributii => am terminat
                {
                    return;
                }

                if (bvcPrev.BVC_Tip == BVC_Tip.BVC && !randFormVenit.AvailableBVC)
                {
                    return;
                }

                if (bvcPrev.BVC_Tip == BVC_Tip.CashFlow && !randFormVenit.AvailableCashFlow)
                {
                    return;
                }

                var randVenit = Context.BVC_BugetPrevRand.Include(f => f.ValueList)
                                                                    .FirstOrDefault(f => f.BugetPrevId == bugetPrevId && f.FormRandId == randFormVenit.Id && f.DepartamentId == dept.DepartamentId);

                var contributiiList = Context.BVC_BugetPrevContributie.Where(f => f.TipIncasare == BVC_BugetPrevContributieTipIncasare.Contributii && f.DataIncasare >= startDate && f.DataIncasare <= endDate).ToList();

                var monedeList = contributiiList.Select(f => f.CurrencyId).Distinct().ToList();

                foreach (var moneda in monedeList)
                {
                    decimal exchangeRate = 1;
                    var currencyItem = Context.Currency.FirstOrDefault(f => f.Id == moneda);
                    if (currencyItem.CurrencyCode != "RON")
                    {
                        var exchangeRateItem = Context.ExchangeRateForecasts.FirstOrDefault(f => f.State == Models.Conta.Enums.State.Active && f.Year == bvcPrev.Formular.AnBVC
                                                                                            && f.CurrencyId == currencyItem.Id);
                        if (exchangeRateItem == null)
                        {
                            throw new Exception("Nu ati completat cursul valutar estimat pentru " + currencyItem.CurrencyCode + " pentru anul " + bvcPrev.Formular.AnBVC.ToString());
                        }
                        exchangeRate = exchangeRateItem.ValoareEstimata;
                    }

                    foreach (var contributie in contributiiList.Where(f => f.CurrencyId == moneda))
                    {
                        var currDate = LazyMethods.LastDayOfMonth(contributie.DataIncasare);
                        var randValue = valuesList.FirstOrDefault(f => f.ActivityType == contributie.ActivityTypeId && f.DataLuna == currDate);
                        if (randValue == null)
                        {
                            randValue = new ActiveBugetPrepareAddDto
                            {
                                ActivityType = contributie.ActivityTypeId,
                                DataLuna = currDate,
                                Valoare = Math.Round(contributie.Value * exchangeRate, 2)
                            };
                            valuesList.Add(randValue);
                        }
                        else
                        {
                            randValue.Valoare += Math.Round(contributie.Value * exchangeRate, 2);
                        }
                    }
                }

                // salvez valorile in db
                foreach (var item in valuesList)
                {
                    var randValue = Context.BVC_BugetPrevRandValue.FirstOrDefault(f => f.BugetPrevRandId == randVenit.Id && f.ValueType == BVC_PrevValueType.Contributii && f.DataLuna == item.DataLuna
                                                                                  && f.ActivityTypeId == item.ActivityType);
                    if (randValue == null)
                    {
                        randValue = new BVC_BugetPrevRandValue
                        {
                            ActivityTypeId = item.ActivityType,
                            DataLuna = item.DataLuna,
                            DataOper = item.DataLuna,
                            BugetPrevRandId = randVenit.Id,
                            Description = "Contributii",
                            TenantId = 1,
                            ValueType = BVC_PrevValueType.Contributii,
                            Value = item.Valoare
                        };
                        Context.Add(randValue);
                    }
                    else
                    {
                        randValue.Value += item.Valoare;
                    }
                }

                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void BVC_CashFlowContributii(int bugetPrevId)
        {
            try
            {
                var bvcPrev = Context.BVC_BugetPrev.Include(f => f.Formular).FirstOrDefault(f => f.Id == bugetPrevId);
                var startDate = new DateTime(bvcPrev.Formular.AnBVC, bvcPrev.MonthStart, 1);
                var endDate = LazyMethods.LastDayOfMonth(new DateTime(bvcPrev.Formular.AnBVC, bvcPrev.MonthEnd, 1));

                var valuesList = new List<ActiveCashFlowPrepareAddDto>();
                var dept = Context.BVC_BugetPrevAutoValue.FirstOrDefault(f => f.TipRand == BVC_RowType.Venituri && f.TipRandVenit == BVC_RowTypeIncome.Contributii);
                if (dept == null)
                {
                    throw new Exception("Nu ati specificat departamentul pentru care se vor inregistra veniturile din contributii");
                }
                var deptId = dept.DepartamentId;
                var randFormVenit = Context.BVC_FormRand.FirstOrDefault(f => f.FormularId == bvcPrev.FormularId && f.TipRand == BVC_RowType.Venituri && f.TipRandVenit == BVC_RowTypeIncome.Contributii
                                                                        && !f.IsTotal);
                if (randFormVenit == null) // nu am venituri din contributii => am terminat
                {
                    return;
                }

                if (bvcPrev.BVC_Tip == BVC_Tip.BVC && !randFormVenit.AvailableBVC)
                {
                    return;
                }

                if (bvcPrev.BVC_Tip == BVC_Tip.CashFlow && !randFormVenit.AvailableCashFlow)
                {
                    return;
                }

                var randVenit = Context.BVC_BugetPrevRand.Include(f => f.ValueList)
                                                                    .FirstOrDefault(f => f.BugetPrevId == bugetPrevId && f.FormRandId == randFormVenit.Id && f.DepartamentId == dept.DepartamentId);

                var contributiiList = Context.BVC_BugetPrevContributie.Where(f => f.TipIncasare == BVC_BugetPrevContributieTipIncasare.Contributii && f.DataIncasare >= startDate && f.DataIncasare <= endDate).ToList();

                var monedeList = contributiiList.Select(f => f.CurrencyId).Distinct().ToList();

                foreach (var moneda in monedeList)
                {
                    decimal exchangeRate = 1;
                    var currencyItem = Context.Currency.FirstOrDefault(f => f.Id == moneda);
                    if (currencyItem.CurrencyCode != "RON")
                    {
                        var exchangeRateItem = Context.ExchangeRateForecasts.FirstOrDefault(f => f.State == Models.Conta.Enums.State.Active && f.Year == bvcPrev.Formular.AnBVC
                                                                                            && f.CurrencyId == currencyItem.Id);
                        if (exchangeRateItem == null)
                        {
                            throw new Exception("Nu ati completat cursul valutar estimat pentru " + currencyItem.CurrencyCode + " pentru anul " + bvcPrev.Formular.AnBVC.ToString());
                        }
                        exchangeRate = exchangeRateItem.ValoareEstimata;
                    }

                    foreach (var contributie in contributiiList.Where(f => f.CurrencyId == moneda))
                    {
                        var currDate = LazyMethods.LastDayOfMonth(contributie.DataIncasare);
                        var randValue = valuesList.FirstOrDefault(f => f.ActivityType == contributie.ActivityTypeId && f.DataLuna == currDate && f.DataOper == contributie.DataIncasare);
                        if (randValue == null)
                        {
                            randValue = new ActiveCashFlowPrepareAddDto
                            {
                                ActivityType = contributie.ActivityTypeId,
                                DataLuna = currDate,
                                DataOper = contributie.DataIncasare,
                                Valoare = Math.Round(contributie.Value * exchangeRate, 2)
                            };
                            valuesList.Add(randValue);
                        }
                        else
                        {
                            randValue.Valoare += Math.Round(contributie.Value * exchangeRate, 2);
                        }
                    }
                }

                // salvez valorile in db
                foreach (var item in valuesList)
                {
                    var randValue = Context.BVC_BugetPrevRandValue.FirstOrDefault(f => f.BugetPrevRandId == randVenit.Id && f.ValueType == BVC_PrevValueType.Contributii && f.DataLuna == item.DataLuna
                                                                                  && f.DataOper == item.DataOper && f.ActivityTypeId == item.ActivityType);
                    if (randValue == null)
                    {
                        randValue = new BVC_BugetPrevRandValue
                        {
                            ActivityTypeId = item.ActivityType,
                            DataLuna = item.DataLuna,
                            DataOper = item.DataOper,
                            BugetPrevRandId = randVenit.Id,
                            Description = "Contributii",
                            TenantId = 1,
                            ValueType = BVC_PrevValueType.Contributii,
                            Value = item.Valoare
                        };
                        Context.Add(randValue);
                    }
                    else
                    {
                        randValue.Value += item.Valoare;
                    }
                }

                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void BVC_PrevCreante(int bugetPrevId)
        {
            try
            {
                var bvcPrev = Context.BVC_BugetPrev.Include(f => f.Formular).FirstOrDefault(f => f.Id == bugetPrevId);
                var startDate = new DateTime(bvcPrev.Formular.AnBVC, bvcPrev.MonthStart, 1);
                var endDate = LazyMethods.LastDayOfMonth(new DateTime(bvcPrev.Formular.AnBVC, bvcPrev.MonthEnd, 1));

                var valuesList = new List<ActiveBugetPrepareAddDto>();
                var dept = Context.BVC_BugetPrevAutoValue.FirstOrDefault(f => f.TipRand == BVC_RowType.Venituri && f.TipRandVenit == BVC_RowTypeIncome.Creante);
                if (dept == null)
                {
                    throw new Exception("Nu ati specificat departamentul pentru care se vor inregistra veniturile din contributii");
                }
                var deptId = dept.DepartamentId;
                var randFormVenit = Context.BVC_FormRand.FirstOrDefault(f => f.FormularId == bvcPrev.FormularId && f.TipRand == BVC_RowType.Venituri && f.TipRandVenit == BVC_RowTypeIncome.Creante
                                                                        && !f.IsTotal);
                if (randFormVenit == null) // nu am venituri din contributii => am terminat
                {
                    return;
                }

                if (bvcPrev.BVC_Tip == BVC_Tip.BVC && !randFormVenit.AvailableBVC)
                {
                    return;
                }

                if (bvcPrev.BVC_Tip == BVC_Tip.CashFlow && !randFormVenit.AvailableCashFlow)
                {
                    return;
                }

                var randVenit = Context.BVC_BugetPrevRand.Include(f => f.ValueList)
                                                                    .FirstOrDefault(f => f.BugetPrevId == bugetPrevId && f.FormRandId == randFormVenit.Id && f.DepartamentId == dept.DepartamentId);

                var contributiiList = Context.BVC_BugetPrevContributie.Where(f => (f.TipIncasare == BVC_BugetPrevContributieTipIncasare.Creante || f.TipIncasare == BVC_BugetPrevContributieTipIncasare.ComisionLichidator) && f.DataIncasare >= startDate && f.DataIncasare <= endDate).ToList();

                var monedeList = contributiiList.Select(f => f.CurrencyId).Distinct().ToList();

                foreach (var moneda in monedeList)
                {
                    decimal exchangeRate = 1;
                    var currencyItem = Context.Currency.FirstOrDefault(f => f.Id == moneda);
                    if (currencyItem.CurrencyCode != "RON")
                    {
                        var exchangeRateItem = Context.ExchangeRateForecasts.FirstOrDefault(f => f.State == Models.Conta.Enums.State.Active && f.Year == bvcPrev.Formular.AnBVC
                                                                                            && f.CurrencyId == currencyItem.Id);
                        if (exchangeRateItem == null)
                        {
                            throw new Exception("Nu ati completat cursul valutar estimat pentru " + currencyItem.CurrencyCode + " pentru anul " + bvcPrev.Formular.AnBVC.ToString());
                        }
                        exchangeRate = exchangeRateItem.ValoareEstimata;
                    }

                    foreach (var contributie in contributiiList.Where(f => f.CurrencyId == moneda))
                    {
                        var currDate = LazyMethods.LastDayOfMonth(contributie.DataIncasare);
                        var randValue = valuesList.FirstOrDefault(f => f.ActivityType == contributie.ActivityTypeId && f.DataLuna == currDate);
                        if (randValue == null)
                        {
                            randValue = new ActiveBugetPrepareAddDto
                            {
                                ActivityType = contributie.ActivityTypeId,
                                DataLuna = currDate,
                                Valoare = Math.Round(contributie.Value * exchangeRate, 2)
                            };
                            valuesList.Add(randValue);
                        }
                        else
                        {
                            randValue.Valoare += Math.Round(contributie.Value * exchangeRate, 2);
                        }
                    }
                }

                // salvez valorile in db
                foreach (var item in valuesList)
                {
                    var randValue = Context.BVC_BugetPrevRandValue.FirstOrDefault(f => f.BugetPrevRandId == randVenit.Id && f.ValueType == BVC_PrevValueType.Creante && f.DataLuna == item.DataLuna
                                                                                  && f.ActivityTypeId == item.ActivityType);
                    if (randValue == null)
                    {
                        randValue = new BVC_BugetPrevRandValue
                        {
                            ActivityTypeId = item.ActivityType,
                            DataLuna = item.DataLuna,
                            DataOper = item.DataLuna,
                            BugetPrevRandId = randVenit.Id,
                            Description = "Creante",
                            TenantId = 1,
                            ValueType = BVC_PrevValueType.Creante,
                            Value = item.Valoare
                        };
                        Context.Add(randValue);
                    }
                    else
                    {
                        randValue.Value += item.Valoare;
                    }
                }

                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void BVC_CashFlowCreante(int bugetPrevId)
        {
            try
            {
                var bvcPrev = Context.BVC_BugetPrev.Include(f => f.Formular).FirstOrDefault(f => f.Id == bugetPrevId);
                var startDate = new DateTime(bvcPrev.Formular.AnBVC, bvcPrev.MonthStart, 1);
                var endDate = LazyMethods.LastDayOfMonth(new DateTime(bvcPrev.Formular.AnBVC, bvcPrev.MonthEnd, 1));

                var valuesList = new List<ActiveCashFlowPrepareAddDto>();
                var dept = Context.BVC_BugetPrevAutoValue.FirstOrDefault(f => f.TipRand == BVC_RowType.Venituri && f.TipRandVenit == BVC_RowTypeIncome.Creante);
                if (dept == null)
                {
                    throw new Exception("Nu ati specificat departamentul pentru care se vor inregistra veniturile din contributii");
                }
                var deptId = dept.DepartamentId;
                var randFormVenit = Context.BVC_FormRand.FirstOrDefault(f => f.FormularId == bvcPrev.FormularId && f.TipRand == BVC_RowType.Venituri && f.TipRandVenit == BVC_RowTypeIncome.Creante
                                                                        && !f.IsTotal);
                if (randFormVenit == null) // nu am venituri din contributii => am terminat
                {
                    return;
                }

                if (bvcPrev.BVC_Tip == BVC_Tip.BVC && !randFormVenit.AvailableBVC)
                {
                    return;
                }

                if (bvcPrev.BVC_Tip == BVC_Tip.CashFlow && !randFormVenit.AvailableCashFlow)
                {
                    return;
                }

                var randVenit = Context.BVC_BugetPrevRand.Include(f => f.ValueList)
                                                                    .FirstOrDefault(f => f.BugetPrevId == bugetPrevId && f.FormRandId == randFormVenit.Id && f.DepartamentId == dept.DepartamentId);

                var contributiiList = Context.BVC_BugetPrevContributie.Where(f => f.TipIncasare == BVC_BugetPrevContributieTipIncasare.Creante && f.DataIncasare >= startDate && f.DataIncasare <= endDate).ToList();

                var monedeList = contributiiList.Select(f => f.CurrencyId).Distinct().ToList();

                foreach (var moneda in monedeList)
                {
                    decimal exchangeRate = 1;
                    var currencyItem = Context.Currency.FirstOrDefault(f => f.Id == moneda);
                    if (currencyItem.CurrencyCode != "RON")
                    {
                        var exchangeRateItem = Context.ExchangeRateForecasts.FirstOrDefault(f => f.State == Models.Conta.Enums.State.Active && f.Year == bvcPrev.Formular.AnBVC
                                                                                            && f.CurrencyId == currencyItem.Id);
                        if (exchangeRateItem == null)
                        {
                            throw new Exception("Nu ati completat cursul valutar estimat pentru " + currencyItem.CurrencyCode + " pentru anul " + bvcPrev.Formular.AnBVC.ToString());
                        }
                        exchangeRate = exchangeRateItem.ValoareEstimata;
                    }

                    foreach (var contributie in contributiiList.Where(f => f.CurrencyId == moneda))
                    {
                        var currDate = LazyMethods.LastDayOfMonth(contributie.DataIncasare);
                        var randValue = valuesList.FirstOrDefault(f => f.ActivityType == contributie.ActivityTypeId && f.DataLuna == currDate && f.DataOper == contributie.DataIncasare);
                        if (randValue == null)
                        {
                            randValue = new ActiveCashFlowPrepareAddDto
                            {
                                ActivityType = contributie.ActivityTypeId,
                                DataLuna = currDate,
                                DataOper = contributie.DataIncasare,
                                Valoare = Math.Round(contributie.Value * exchangeRate, 2)
                            };
                            valuesList.Add(randValue);
                        }
                        else
                        {
                            randValue.Valoare += Math.Round(contributie.Value * exchangeRate, 2);
                        }
                    }
                }

                // salvez valorile in db
                foreach (var item in valuesList)
                {
                    var randValue = Context.BVC_BugetPrevRandValue.FirstOrDefault(f => f.BugetPrevRandId == randVenit.Id && f.ValueType == BVC_PrevValueType.Creante && f.DataLuna == item.DataLuna
                                                                                  && f.DataOper == item.DataOper && f.ActivityTypeId == item.ActivityType);
                    if (randValue == null)
                    {
                        randValue = new BVC_BugetPrevRandValue
                        {
                            ActivityTypeId = item.ActivityType,
                            DataLuna = item.DataLuna,
                            DataOper = item.DataOper,
                            BugetPrevRandId = randVenit.Id,
                            Description = "Creante",
                            TenantId = 1,
                            ValueType = BVC_PrevValueType.Creante,
                            Value = item.Valoare
                        };
                        Context.Add(randValue);
                    }
                    else
                    {
                        randValue.Value += item.Valoare;
                    }
                }

                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void BVC_PrevIncasari(int bugetPrevId, BVC_BugetPrevContributieTipIncasare tipIncasare)
        {
            try
            {
                var bvcPrev = Context.BVC_BugetPrev.Include(f => f.Formular).FirstOrDefault(f => f.Id == bugetPrevId);
                var startDate = new DateTime(bvcPrev.Formular.AnBVC, bvcPrev.MonthStart, 1);
                var endDate = LazyMethods.LastDayOfMonth(new DateTime(bvcPrev.Formular.AnBVC, bvcPrev.MonthEnd, 1));

                var valuesList = new List<ActiveBugetPrepareAddDto>();
                var dept = new BVC_BugetPrevAutoValue();
                var randFormVenit = new BVC_FormRand();

                switch (tipIncasare)
                {
                    case BVC_BugetPrevContributieTipIncasare.Contributii:
                        dept = Context.BVC_BugetPrevAutoValue.FirstOrDefault(f => f.TipRand == BVC_RowType.Venituri && f.TipRandVenit == BVC_RowTypeIncome.Contributii);
                        randFormVenit = Context.BVC_FormRand.FirstOrDefault(f => f.FormularId == bvcPrev.FormularId && f.TipRand == BVC_RowType.Venituri && f.TipRandVenit == BVC_RowTypeIncome.Contributii
                                                                        && !f.IsTotal);
                        break;
                    case BVC_BugetPrevContributieTipIncasare.Creante:
                        dept = Context.BVC_BugetPrevAutoValue.FirstOrDefault(f => f.TipRand == BVC_RowType.Venituri && f.TipRandVenit == BVC_RowTypeIncome.Creante);
                        randFormVenit = Context.BVC_FormRand.FirstOrDefault(f => f.FormularId == bvcPrev.FormularId && f.TipRand == BVC_RowType.Venituri && f.TipRandVenit == BVC_RowTypeIncome.Creante
                                                                        && !f.IsTotal);
                        break;
                    case BVC_BugetPrevContributieTipIncasare.Altele:
                        dept = Context.BVC_BugetPrevAutoValue.FirstOrDefault(f => f.TipRand == BVC_RowType.Venituri && f.TipRandVenit == BVC_RowTypeIncome.Altele);
                        randFormVenit = Context.BVC_FormRand.FirstOrDefault(f => f.FormularId == bvcPrev.FormularId && f.TipRand == BVC_RowType.Venituri && f.TipRandVenit == BVC_RowTypeIncome.Altele
                                                                        && !f.IsTotal);
                        break;
                }
                
                
                if (dept == null)
                {
                    throw new Exception("Nu ati specificat departamentul pentru care se vor inregistra veniturile din contributii");
                }
                var deptId = dept.DepartamentId;
                
                if (randFormVenit == null) // nu am venituri din contributii => am terminat
                {
                    return;
                }

                if (bvcPrev.BVC_Tip == BVC_Tip.BVC && !randFormVenit.AvailableBVC)
                {
                    return;
                }

                if (bvcPrev.BVC_Tip == BVC_Tip.CashFlow && !randFormVenit.AvailableCashFlow)
                {
                    return;
                }

                var randVenit = Context.BVC_BugetPrevRand.Include(f => f.ValueList)
                                                                    .FirstOrDefault(f => f.BugetPrevId == bugetPrevId && f.FormRandId == randFormVenit.Id && f.DepartamentId == dept.DepartamentId);

                var contributiiList = new List<BVC_BugetPrevContributie>();

                switch (tipIncasare)
                {
                    case BVC_BugetPrevContributieTipIncasare.Contributii:
                        contributiiList = Context.BVC_BugetPrevContributie.Where(f => f.TipIncasare == BVC_BugetPrevContributieTipIncasare.Contributii  && f.DataIncasare >= startDate && f.DataIncasare <= endDate).ToList();
                        break;
                    case BVC_BugetPrevContributieTipIncasare.Creante:
                        contributiiList = Context.BVC_BugetPrevContributie.Where(f => (f.TipIncasare == BVC_BugetPrevContributieTipIncasare.Creante || f.TipIncasare == BVC_BugetPrevContributieTipIncasare.ComisionLichidator) && f.DataIncasare >= startDate && f.DataIncasare <= endDate).ToList();
                        break;
                    case BVC_BugetPrevContributieTipIncasare.Altele:
                        contributiiList = Context.BVC_BugetPrevContributie.Where(f => f.TipIncasare == BVC_BugetPrevContributieTipIncasare.Altele && f.DataIncasare >= startDate && f.DataIncasare <= endDate).ToList();
                        break;
                }

                

                var monedeList = contributiiList.Select(f => f.CurrencyId).Distinct().ToList();

                foreach (var moneda in monedeList)
                {
                    decimal exchangeRate = 1;
                    var currencyItem = Context.Currency.FirstOrDefault(f => f.Id == moneda);
                    if (currencyItem.CurrencyCode != "RON")
                    {
                        var exchangeRateItem = Context.ExchangeRateForecasts.FirstOrDefault(f => f.State == Models.Conta.Enums.State.Active && f.Year == bvcPrev.Formular.AnBVC
                                                                                            && f.CurrencyId == currencyItem.Id);
                        if (exchangeRateItem == null)
                        {
                            throw new Exception("Nu ati completat cursul valutar estimat pentru " + currencyItem.CurrencyCode + " pentru anul " + bvcPrev.Formular.AnBVC.ToString());
                        }
                        exchangeRate = exchangeRateItem.ValoareEstimata;
                    }

                    foreach (var contributie in contributiiList.Where(f => f.CurrencyId == moneda))
                    {
                        var currDate = LazyMethods.LastDayOfMonth(contributie.DataIncasare);
                        var randValue = valuesList.FirstOrDefault(f => f.ActivityType == contributie.ActivityTypeId && f.DataLuna == currDate);
                        if (randValue == null)
                        {
                            randValue = new ActiveBugetPrepareAddDto
                            {
                                ActivityType = contributie.ActivityTypeId,
                                DataLuna = currDate,
                                Valoare = Math.Round(contributie.Value * exchangeRate, 2)
                            };
                            valuesList.Add(randValue);
                        }
                        else
                        {
                            randValue.Valoare += Math.Round(contributie.Value * exchangeRate, 2);
                        }
                    }
                }

                // salvez valorile in db
                foreach (var item in valuesList)
                {
                    var randValue = new BVC_BugetPrevRandValue();

                    switch (tipIncasare)
                    {
                        case BVC_BugetPrevContributieTipIncasare.Contributii:
                        randValue = Context.BVC_BugetPrevRandValue.FirstOrDefault(f => f.BugetPrevRandId == randVenit.Id && f.ValueType == BVC_PrevValueType.Contributii && f.DataLuna == item.DataLuna
                                                                                  && f.ActivityTypeId == item.ActivityType);
                        break;
                        case BVC_BugetPrevContributieTipIncasare.Creante:
                            randValue = Context.BVC_BugetPrevRandValue.FirstOrDefault(f => f.BugetPrevRandId == randVenit.Id && f.ValueType == BVC_PrevValueType.Creante && f.DataLuna == item.DataLuna
                                                                                      && f.ActivityTypeId == item.ActivityType);
                            break;
                        case BVC_BugetPrevContributieTipIncasare.Altele:
                            randValue = Context.BVC_BugetPrevRandValue.FirstOrDefault(f => f.BugetPrevRandId == randVenit.Id && f.ValueType == BVC_PrevValueType.Altele && f.DataLuna == item.DataLuna
                                                                                      && f.ActivityTypeId == item.ActivityType);
                            break;
                    }
                    
                    if (randValue == null)
                    {
                        switch (tipIncasare)
                        {
                            case BVC_BugetPrevContributieTipIncasare.Contributii:
                                randValue = new BVC_BugetPrevRandValue
                                {
                                    ActivityTypeId = item.ActivityType,
                                    DataLuna = item.DataLuna,
                                    DataOper = item.DataLuna,
                                    BugetPrevRandId = randVenit.Id,
                                    Description = "Contributii",
                                    TenantId = 1,
                                    ValueType = BVC_PrevValueType.Contributii,
                                    Value = item.Valoare
                                };
                                Context.Add(randValue);
                                break;

                            case BVC_BugetPrevContributieTipIncasare.Creante:
                                randValue = new BVC_BugetPrevRandValue
                                {
                                    ActivityTypeId = item.ActivityType,
                                    DataLuna = item.DataLuna,
                                    DataOper = item.DataLuna,
                                    BugetPrevRandId = randVenit.Id,
                                    Description = "Creante",
                                    TenantId = 1,
                                    ValueType = BVC_PrevValueType.Creante,
                                    Value = item.Valoare
                                };
                                Context.Add(randValue);
                                break;
                            case BVC_BugetPrevContributieTipIncasare.Altele:

                                randValue = new BVC_BugetPrevRandValue
                                {
                                    ActivityTypeId = item.ActivityType,
                                    DataLuna = item.DataLuna,
                                    DataOper = item.DataLuna,
                                    BugetPrevRandId = randVenit.Id,
                                    Description = "Altele",
                                    TenantId = 1,
                                    ValueType = BVC_PrevValueType.Altele,
                                    Value = item.Valoare
                                };
                                Context.Add(randValue);
                                break;
                        }
                        
                    }
                    else
                    {
                        randValue.Value += item.Valoare;
                    }
                }

                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void BVC_CashFlowIncasari(int bugetPrevId, BVC_BugetPrevContributieTipIncasare tipIncasare)
        {
            try
            {
                var bvcPrev = Context.BVC_BugetPrev.Include(f => f.Formular).FirstOrDefault(f => f.Id == bugetPrevId);
                var startDate = new DateTime(bvcPrev.Formular.AnBVC, bvcPrev.MonthStart, 1);
                var endDate = LazyMethods.LastDayOfMonth(new DateTime(bvcPrev.Formular.AnBVC, bvcPrev.MonthEnd, 1));

                var valuesList = new List<ActiveCashFlowPrepareAddDto>();
                var dept = new BVC_BugetPrevAutoValue();

                switch (tipIncasare)
                {
                    case BVC_BugetPrevContributieTipIncasare.Contributii:
                        dept = Context.BVC_BugetPrevAutoValue.FirstOrDefault(f => f.TipRand == BVC_RowType.Venituri && f.TipRandVenit == BVC_RowTypeIncome.Contributii);
                        break;
                    case BVC_BugetPrevContributieTipIncasare.Creante:
                        dept = Context.BVC_BugetPrevAutoValue.FirstOrDefault(f => f.TipRand == BVC_RowType.Venituri && f.TipRandVenit == BVC_RowTypeIncome.Creante);
                        break;
                    case BVC_BugetPrevContributieTipIncasare.Altele:
                        dept = Context.BVC_BugetPrevAutoValue.FirstOrDefault(f => f.TipRand == BVC_RowType.Venituri && f.TipRandVenit == BVC_RowTypeIncome.Altele);
                        break;
                }

                
                if (dept == null)
                {
                    throw new Exception("Nu ati specificat departamentul pentru care se vor inregistra veniturile din contributii");
                }
                var deptId = dept.DepartamentId;
                var randFormVenit = new BVC_FormRand();

                switch (tipIncasare)
                {
                    case BVC_BugetPrevContributieTipIncasare.Contributii:
                        randFormVenit = Context.BVC_FormRand.FirstOrDefault(f => f.FormularId == bvcPrev.FormularId && f.TipRand == BVC_RowType.Venituri && f.TipRandVenit == BVC_RowTypeIncome.Contributii
                                                                        && !f.IsTotal);
                        break;
                    case BVC_BugetPrevContributieTipIncasare.Creante:
                        randFormVenit = Context.BVC_FormRand.FirstOrDefault(f => f.FormularId == bvcPrev.FormularId && f.TipRand == BVC_RowType.Venituri && f.TipRandVenit == BVC_RowTypeIncome.Creante
                                                                        && !f.IsTotal);
                        break;
                    case BVC_BugetPrevContributieTipIncasare.Altele:
                        randFormVenit = Context.BVC_FormRand.FirstOrDefault(f => f.FormularId == bvcPrev.FormularId && f.TipRand == BVC_RowType.Venituri && f.TipRandVenit == BVC_RowTypeIncome.Altele
                                                                        && !f.IsTotal);
                        break;
                }
                
                if (randFormVenit == null) // nu am venituri din contributii => am terminat
                {
                    return;
                }

                if (bvcPrev.BVC_Tip == BVC_Tip.BVC && !randFormVenit.AvailableBVC)
                {
                    return;
                }

                if (bvcPrev.BVC_Tip == BVC_Tip.CashFlow && !randFormVenit.AvailableCashFlow)
                {
                    return;
                }

                var randVenit = Context.BVC_BugetPrevRand.Include(f => f.ValueList)
                                                                    .FirstOrDefault(f => f.BugetPrevId == bugetPrevId && f.FormRandId == randFormVenit.Id && f.DepartamentId == dept.DepartamentId);
                var contributiiList = new List<BVC_BugetPrevContributie>();

                switch (tipIncasare)
                {
                    case BVC_BugetPrevContributieTipIncasare.Contributii:
                        contributiiList = Context.BVC_BugetPrevContributie.Where(f => f.TipIncasare == BVC_BugetPrevContributieTipIncasare.Contributii && f.DataIncasare >= startDate && f.DataIncasare <= endDate).ToList();
                        break;
                    case BVC_BugetPrevContributieTipIncasare.Creante:
                        contributiiList = Context.BVC_BugetPrevContributie.Where(f => f.TipIncasare == BVC_BugetPrevContributieTipIncasare.Creante && f.DataIncasare >= startDate && f.DataIncasare <= endDate).ToList();
                        break;
                    case BVC_BugetPrevContributieTipIncasare.Altele:
                        contributiiList = Context.BVC_BugetPrevContributie.Where(f => f.TipIncasare == BVC_BugetPrevContributieTipIncasare.Altele && f.DataIncasare >= startDate && f.DataIncasare <= endDate).ToList();
                        break;
                }
                

                var monedeList = contributiiList.Select(f => f.CurrencyId).Distinct().ToList();

                foreach (var moneda in monedeList)
                {
                    decimal exchangeRate = 1;
                    var currencyItem = Context.Currency.FirstOrDefault(f => f.Id == moneda);
                    if (currencyItem.CurrencyCode != "RON")
                    {
                        var exchangeRateItem = Context.ExchangeRateForecasts.FirstOrDefault(f => f.State == Models.Conta.Enums.State.Active && f.Year == bvcPrev.Formular.AnBVC
                                                                                            && f.CurrencyId == currencyItem.Id);
                        if (exchangeRateItem == null)
                        {
                            throw new Exception("Nu ati completat cursul valutar estimat pentru " + currencyItem.CurrencyCode + " pentru anul " + bvcPrev.Formular.AnBVC.ToString());
                        }
                        exchangeRate = exchangeRateItem.ValoareEstimata;
                    }

                    foreach (var contributie in contributiiList.Where(f => f.CurrencyId == moneda))
                    {
                        var currDate = LazyMethods.LastDayOfMonth(contributie.DataIncasare);
                        var randValue = valuesList.FirstOrDefault(f => f.ActivityType == contributie.ActivityTypeId && f.DataLuna == currDate && f.DataOper == contributie.DataIncasare);
                        if (randValue == null)
                        {
                            randValue = new ActiveCashFlowPrepareAddDto
                            {
                                ActivityType = contributie.ActivityTypeId,
                                DataLuna = currDate,
                                DataOper = contributie.DataIncasare,
                                Valoare = Math.Round(contributie.Value * exchangeRate, 2)
                            };
                            valuesList.Add(randValue);
                        }
                        else
                        {
                            randValue.Valoare += Math.Round(contributie.Value * exchangeRate, 2);
                        }
                    }
                }

                // salvez valorile in db
                foreach (var item in valuesList)
                {
                    var randValue = new BVC_BugetPrevRandValue();
                    switch (tipIncasare)
                    {
                        case BVC_BugetPrevContributieTipIncasare.Contributii:
                            randValue = Context.BVC_BugetPrevRandValue.FirstOrDefault(f => f.BugetPrevRandId == randVenit.Id && f.ValueType == BVC_PrevValueType.Contributii && f.DataLuna == item.DataLuna
                                                                                  && f.DataOper == item.DataOper && f.ActivityTypeId == item.ActivityType);
                            break;
                        case BVC_BugetPrevContributieTipIncasare.Creante:
                            randValue = Context.BVC_BugetPrevRandValue.FirstOrDefault(f => f.BugetPrevRandId == randVenit.Id && f.ValueType == BVC_PrevValueType.Creante && f.DataLuna == item.DataLuna
                                                                                  && f.DataOper == item.DataOper && f.ActivityTypeId == item.ActivityType);
                            break;
                        case BVC_BugetPrevContributieTipIncasare.Altele:
                            randValue = Context.BVC_BugetPrevRandValue.FirstOrDefault(f => f.BugetPrevRandId == randVenit.Id && f.ValueType == BVC_PrevValueType.Altele && f.DataLuna == item.DataLuna
                                                                                  && f.DataOper == item.DataOper && f.ActivityTypeId == item.ActivityType);
                            break;
                    }
                    
                    if (randValue == null)
                    {
                        switch (tipIncasare)
                        {
                            case BVC_BugetPrevContributieTipIncasare.Contributii:
                                randValue = new BVC_BugetPrevRandValue
                                {
                                    ActivityTypeId = item.ActivityType,
                                    DataLuna = item.DataLuna,
                                    DataOper = item.DataOper,
                                    BugetPrevRandId = randVenit.Id,
                                    Description = "Contributii",
                                    TenantId = 1,
                                    ValueType = BVC_PrevValueType.Contributii,
                                    Value = item.Valoare
                                };
                                Context.Add(randValue);
                                break;
                            case BVC_BugetPrevContributieTipIncasare.Creante:
                                randValue = new BVC_BugetPrevRandValue
                                {
                                    ActivityTypeId = item.ActivityType,
                                    DataLuna = item.DataLuna,
                                    DataOper = item.DataOper,
                                    BugetPrevRandId = randVenit.Id,
                                    Description = "Creante",
                                    TenantId = 1,
                                    ValueType = BVC_PrevValueType.Creante,
                                    Value = item.Valoare
                                };
                                Context.Add(randValue);
                                break;
                            case BVC_BugetPrevContributieTipIncasare.Altele:
                                randValue = new BVC_BugetPrevRandValue
                                {
                                    ActivityTypeId = item.ActivityType,
                                    DataLuna = item.DataLuna,
                                    DataOper = item.DataOper,
                                    BugetPrevRandId = randVenit.Id,
                                    Description = "Altele",
                                    TenantId = 1,
                                    ValueType = BVC_PrevValueType.Altele,
                                    Value = item.Valoare
                                };
                                Context.Add(randValue);
                                break;
                        }
                        
                    }
                    else
                    {
                        randValue.Value += item.Valoare;
                    }
                }

                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void BVC_PrevSalarizare(SalarizareBVCDto salarizareBVC, int bugetPrevId, List<VenitRepartizProc> repartizareVenituri)
        {
            try
            {
                var bvcPrev = Context.BVC_BugetPrev.Include(f => f.Formular).FirstOrDefault(f => f.Id == bugetPrevId);

                var valuesList = new List<ActiveBugetPrepareAddDto>();
                var dept = Context.BVC_BugetPrevAutoValue.FirstOrDefault(f => f.TipRand == BVC_RowType.Salarizare);
                if (dept == null)
                {
                    throw new Exception("Nu ati specificat departamentul pentru care se vor inregistra veniturile din contributii");
                }
                var listToAdd = new List<BugetPrevRandValuePrepare>();

                var deptId = dept.DepartamentId;
                var okIndemnizatiiCS = true; var okFondSalarii = true; var okContrib = true; var okFondSocial = true;

                var randIndemnizatiiCS = Context.BVC_FormRand.FirstOrDefault(f => f.FormularId == bvcPrev.FormularId && f.TipRand == BVC_RowType.Salarizare
                                                                              && f.TipRandSalarizare == BVC_RowTypeSalarizare.IndemnizatiiCS
                                                                              && !f.IsTotal);
                var randFondSalarii = Context.BVC_FormRand.FirstOrDefault(f => f.FormularId == bvcPrev.FormularId && f.TipRand == BVC_RowType.Salarizare
                                                                              && f.TipRandSalarizare == BVC_RowTypeSalarizare.FondSalarii
                                                                              && !f.IsTotal);
                var randContrib = Context.BVC_FormRand.FirstOrDefault(f => f.FormularId == bvcPrev.FormularId && f.TipRand == BVC_RowType.Salarizare
                                                                              && f.TipRandSalarizare == BVC_RowTypeSalarizare.Contributii
                                                                              && !f.IsTotal);
                var randFondSocial = Context.BVC_FormRand.FirstOrDefault(f => f.FormularId == bvcPrev.FormularId && f.TipRand == BVC_RowType.Salarizare
                                                                              && f.TipRandSalarizare == BVC_RowTypeSalarizare.FondSocial
                                                                              && !f.IsTotal);
                if (randIndemnizatiiCS == null) // nu am indemnizatii
                {
                    okIndemnizatiiCS = false;
                }
                if (randFondSalarii == null) // nu am fondSalarii
                {
                    okFondSalarii = false;
                }
                if (randContrib == null) // nu am contrib
                {
                    okContrib = false;
                }
                if (randFondSocial == null) // nu am fondSocial
                {
                    okFondSocial = false;
                }

                if (okIndemnizatiiCS)
                {
                    var randBVCIndemnizatiiCS = Context.BVC_BugetPrevRand.Include(f => f.ValueList)
                                                                         .FirstOrDefault(f => f.BugetPrevId == bugetPrevId && f.FormRandId == randIndemnizatiiCS.Id && f.DepartamentId == dept.DepartamentId);
                    foreach (var item in salarizareBVC.Luni.Where(f=>bvcPrev.MonthStart <= f.Luna && f.Luna <= bvcPrev.MonthEnd))
                    {
                        DateTime dataOper = LazyMethods.LastDayOfMonth(new DateTime(bvcPrev.Formular.AnBVC, item.Luna, 1));

                        foreach (var procVenit in repartizareVenituri)
                        {
                            var indemnizatieCS = new BugetPrevRandValuePrepare
                            {
                                ActivityTypeId = procVenit.ActivityTypeId,
                                BugetPrevRandId = randBVCIndemnizatiiCS.Id,
                                DataLuna = dataOper,
                                DataOper = dataOper,
                                Description = "Indemnizatii CS",
                                Value = Math.Round(item.CheltIndemnCS * procVenit.ProcRepartizat, 2),
                                ValueType = BVC_PrevValueType.Salarii
                            };
                            listToAdd.Add(indemnizatieCS);
                        }
                    }
                }

                if (okFondSalarii)
                {
                    var randBVCFondSalarii = Context.BVC_BugetPrevRand.Include(f => f.ValueList)
                                                                      .FirstOrDefault(f => f.BugetPrevId == bugetPrevId && f.FormRandId == randFondSalarii.Id && f.DepartamentId == dept.DepartamentId);
                    foreach (var item in salarizareBVC.Luni.Where(f => bvcPrev.MonthStart <= f.Luna && f.Luna <= bvcPrev.MonthEnd))
                    {
                        DateTime dataOper = LazyMethods.LastDayOfMonth(new DateTime(bvcPrev.Formular.AnBVC, item.Luna, 1));

                        foreach (var procVenit in repartizareVenituri)
                        {
                            var fondSalarii = new BugetPrevRandValuePrepare
                            {
                                ActivityTypeId = procVenit.ActivityTypeId,
                                BugetPrevRandId = randBVCFondSalarii.Id,
                                DataLuna = dataOper,
                                DataOper = dataOper,
                                Description = "Fond salarii",
                                Value = Math.Round(item.cheltFdSalarii * procVenit.ProcRepartizat, 2),
                                ValueType = BVC_PrevValueType.Salarii
                            };
                            listToAdd.Add(fondSalarii);
                        }
                    }
                }

                if (okContrib)
                {
                    var randBVCContributii = Context.BVC_BugetPrevRand.Include(f => f.ValueList)
                                                                      .FirstOrDefault(f => f.BugetPrevId == bugetPrevId && f.FormRandId == randContrib.Id && f.DepartamentId == dept.DepartamentId);
                    foreach (var item in salarizareBVC.Luni.Where(f => bvcPrev.MonthStart <= f.Luna && f.Luna <= bvcPrev.MonthEnd))
                    {
                        DateTime dataOper = LazyMethods.LastDayOfMonth(new DateTime(bvcPrev.Formular.AnBVC, item.Luna, 1));

                        foreach (var procVenit in repartizareVenituri)
                        {
                            var contributii = new BugetPrevRandValuePrepare
                            {
                                ActivityTypeId = procVenit.ActivityTypeId,
                                BugetPrevRandId = randBVCContributii.Id,
                                DataLuna = dataOper,
                                DataOper = dataOper,
                                Description = "Contributii",
                                Value = Math.Round(item.cheltContrib * procVenit.ProcRepartizat, 2),
                                ValueType = BVC_PrevValueType.Salarii
                            };
                            listToAdd.Add(contributii);
                        }
                    }
                }

                if (okFondSocial)
                {
                    var randBVCFondSocial = Context.BVC_BugetPrevRand.Include(f => f.ValueList)
                                                                      .FirstOrDefault(f => f.BugetPrevId == bugetPrevId && f.FormRandId == randFondSocial.Id && f.DepartamentId == dept.DepartamentId);
                    foreach (var item in salarizareBVC.Luni.Where(f => bvcPrev.MonthStart <= f.Luna && f.Luna <= bvcPrev.MonthEnd))
                    {
                        DateTime dataOper = LazyMethods.LastDayOfMonth(new DateTime(bvcPrev.Formular.AnBVC, item.Luna, 1));

                        foreach (var procVenit in repartizareVenituri)
                        {
                            var contributii = new BugetPrevRandValuePrepare
                            {
                                ActivityTypeId = procVenit.ActivityTypeId,
                                BugetPrevRandId = randBVCFondSocial.Id,
                                DataLuna = dataOper,
                                DataOper = dataOper,
                                Description = "Fond social",
                                Value = Math.Round(item.cheltFdSocial * procVenit.ProcRepartizat, 2),
                                ValueType = BVC_PrevValueType.Salarii
                            };
                            listToAdd.Add(contributii);
                        }
                    }
                }

                foreach (var item in listToAdd)
                {
                    var randValue = Context.BVC_BugetPrevRandValue.FirstOrDefault(f => f.BugetPrevRandId == item.BugetPrevRandId && f.ValueType == BVC_PrevValueType.Salarii
                                                                                       && f.DataLuna == item.DataLuna && f.ActivityTypeId == item.ActivityTypeId);
                    if (randValue == null)
                    {
                        randValue = new BVC_BugetPrevRandValue
                        {
                            ActivityTypeId = item.ActivityTypeId.Value,
                            DataLuna = item.DataLuna,
                            DataOper = item.DataLuna,
                            BugetPrevRandId = item.BugetPrevRandId,
                            Description = item.Description,
                            TenantId = 1,
                            ValueType = BVC_PrevValueType.Contributii,
                            Value = item.Value
                        };
                        Context.Add(randValue);
                    }
                    else
                    {
                        randValue.Value += item.Value;
                    }
                }

                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void BVC_CashFlowSalarizare(SalarizareCFDto salarizareCF, int bugetPrevId, List<VenitRepartizProc> repartizareVenituri)
        {
            try
            {
                var bvcPrev = Context.BVC_BugetPrev.Include(f => f.Formular).FirstOrDefault(f => f.Id == bugetPrevId);

                var valuesList = new List<ActiveBugetPrepareAddDto>();
                var dept = Context.BVC_BugetPrevAutoValue.FirstOrDefault(f => f.TipRand == BVC_RowType.Salarizare);
                if (dept == null)
                {
                    throw new Exception("Nu ati specificat departamentul pentru care se vor inregistra veniturile din contributii");
                }
                var listToAdd = new List<BugetPrevRandValuePrepare>();

                var deptId = dept.DepartamentId;

                foreach(var plata in salarizareCF.Luni.Where(f => bvcPrev.MonthStart <= f.Luna && f.Luna <= bvcPrev.MonthEnd))
                {
                    var tipRandSalarizare = ReturnValueFromStr(plata.TipPlata);
                    var randSalarizare = Context.BVC_FormRand.FirstOrDefault(f => f.FormularId == bvcPrev.FormularId && f.TipRand == BVC_RowType.Salarizare
                                                                              && f.TipRandSalarizare == tipRandSalarizare && !f.IsTotal);
                    if (randSalarizare != null)
                    {
                        var randCFSalarizare = Context.BVC_BugetPrevRand.Include(f => f.ValueList)
                                                                        .FirstOrDefault(f => f.BugetPrevId == bugetPrevId && f.FormRandId == randSalarizare.Id && f.DepartamentId == dept.DepartamentId);

                        DateTime dataLuna = LazyMethods.LastDayOfMonth(new DateTime(bvcPrev.Formular.AnBVC, plata.Luna, 1));
                        
                        foreach (var procVenit in repartizareVenituri)
                        {
                            var plataSalarizare = new BugetPrevRandValuePrepare
                            {
                                ActivityTypeId = procVenit.ActivityTypeId,
                                BugetPrevRandId = randCFSalarizare.Id,
                                DataLuna = dataLuna,
                                DataOper = plata.DataPlatii,
                                Description = plata.TipPlata,
                                Value = Math.Round(plata.Valoare * procVenit.ProcRepartizat, 2),
                                ValueType = BVC_PrevValueType.Salarii
                            };
                            listToAdd.Add(plataSalarizare);
                        }
                    }
                }
               

                foreach (var item in listToAdd)
                {
                    var randValue = Context.BVC_BugetPrevRandValue.FirstOrDefault(f => f.BugetPrevRandId == item.BugetPrevRandId && f.ValueType == BVC_PrevValueType.Salarii
                                                                                       && f.DataLuna == item.DataLuna && f.ActivityTypeId == item.ActivityTypeId);
                    if (randValue == null)
                    {
                        randValue = new BVC_BugetPrevRandValue
                        {
                            ActivityTypeId = item.ActivityTypeId.Value,
                            DataLuna = item.DataLuna,
                            DataOper = item.DataOper,
                            BugetPrevRandId = item.BugetPrevRandId,
                            Description = item.Description,
                            TenantId = 1,
                            ValueType = BVC_PrevValueType.Contributii,
                            Value = item.Value
                        };
                        Context.Add(randValue);
                    }
                    else
                    {
                        randValue.Value += item.Value;
                    }
                }

                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private BVC_RowTypeSalarizare ReturnValueFromStr(string value)
        {
            var ret = new BVC_RowTypeSalarizare();
            switch (value.ToUpper())
            {
                case "SALARII":
                    ret = BVC_RowTypeSalarizare.Salarii;
                    break;
                case "PRIME CO":
                    ret = BVC_RowTypeSalarizare.PrimaCO;
                    break;
                case "PRIMA DE ZIUA FONDULUI":
                    ret = BVC_RowTypeSalarizare.PrimaZiuaFondului;
                    break;
                case "CADOU 1 IUNIE":
                    ret = BVC_RowTypeSalarizare.Cadou1Iunie;
                    break;
                case "CADOU 8 MARTIE":
                    ret = BVC_RowTypeSalarizare.Cadou8Martie;
                    break;
                case "CADOU CRACIUN":
                    ret = BVC_RowTypeSalarizare.CadouCraciun;
                    break;
                case "TINUTE VESTIMENTARE":
                    ret = BVC_RowTypeSalarizare.TinuteVestimentare;
                    break;
                case "FOND PROFIT":
                    ret = BVC_RowTypeSalarizare.FondProfit;
                    break;
                case "CONTRIBUTII SI CS":
                    ret = BVC_RowTypeSalarizare.ContibutiiSiCS;
                    break;

            }

            return ret;
        }

        public void BVC_PrevTotaluri(int bugetPrevId)
        {
            try
            {
                var bvcPrev = Context.BVC_BugetPrev.Include(f => f.Formular).FirstOrDefault(f => f.Id == bugetPrevId);
                var startDate = new DateTime(bvcPrev.Formular.AnBVC, bvcPrev.MonthStart, 1);
                var endDate = LazyMethods.LastDayOfMonth(new DateTime(bvcPrev.Formular.AnBVC, bvcPrev.MonthEnd, 1));

                var bvcFormRanduri = Context.BVC_FormRand.Include(f => f.DetaliiRand).Where(f => f.FormularId == bvcPrev.FormularId).ToList();
                if (bvcPrev.BVC_Tip == BVC_Tip.BVC)
                {
                    bvcFormRanduri = bvcFormRanduri.Where(f => f.AvailableBVC).ToList();
                }
                if (bvcPrev.BVC_Tip == BVC_Tip.CashFlow)
                {
                    bvcFormRanduri = bvcFormRanduri.Where(f => f.AvailableCashFlow).ToList();
                }

                var bvcPrevValues = Context.BVC_BugetPrevRand.Include(f => f.ValueList).Where(f => f.BugetPrevId == bugetPrevId).ToList();
                var randTotalIds = bvcFormRanduri.Where(f => f.IsTotal).Select(f => f.Id).ToList();

                var valuesList = new List<ActiveBugetCalcTotalDto>();
                
                // initializez totalurile
                foreach(var rand in bvcFormRanduri.Where(f=>f.IsTotal))
                {
                    var randTotalList = bvcPrevValues.Where(f => f.FormRandId == rand.Id);
                    foreach(var randTotal in randTotalList)
                    {
                        if (randTotal.ValueList.Count == 0) // e primul calcul si nu am valori
                        {
                            var activityTypeList = Context.ActivityTypes.ToList();
                            foreach (var item in activityTypeList)
                            {
                                for (int i = bvcPrev.MonthStart; i <= bvcPrev.MonthEnd; i++)
                                {
                                    var dataLunaValue = LazyMethods.LastDayOfMonth(new DateTime(bvcPrev.Formular.AnBVC, i, 1));
                                    var valueItem = new ActiveBugetCalcTotalDto
                                    {
                                        ActivityType = item.Id,
                                        DepartamentId = randTotal.DepartamentId,
                                        DataLuna = dataLunaValue,
                                        Valoare = 0,
                                        Calculat = false,
                                        RandId = rand.Id
                                    };
                                    valuesList.Add(valueItem);
                                }
                            }
                        }
                        else
                        {
                            foreach (var valueRand in randTotal.ValueList)
                            {
                                valueRand.Value = 0;
                                var valueItem = new ActiveBugetCalcTotalDto
                                {
                                    ActivityType = valueRand.ActivityTypeId,
                                    DepartamentId = randTotal.DepartamentId,
                                    DataLuna = valueRand.DataLuna,
                                    Valoare = 0,
                                    Calculat = false,
                                    RandId = rand.Id
                                };
                                valuesList.Add(valueItem);
                            }
                        }
                    }
                }

                //incep calculul
                int contor = 1; bool okCalcul = false;
                while (contor < 50 && !okCalcul)
                {
                    foreach(var valueItem in valuesList.Where(f=>!f.Calculat))
                    {
                        var randForm = bvcFormRanduri.FirstOrDefault(f => f.Id == valueItem.RandId);
                        string formula = (bvcPrev.BVC_Tip == BVC_Tip.BVC) ? randForm.FormulaBVC : randForm.FormulaCashFlow;
                        var semneList = new List<string>();
                        var codRandList = new List<int>();

                        decimal suma = 0;
                        bool formulaOk = true;

                        if (formula != "" && formula != null)
                        {

                            DesfacFormula(formula, out semneList, out codRandList);

                            for (int i = 0; i < semneList.Count; i++)
                            {
                                // iau randul din formular
                                var randFormCurr = bvcFormRanduri.FirstOrDefault(f => f.CodRand == codRandList[i]);

                                // verific daca e rand de total
                                if (randFormCurr.IsTotal)
                                {
                                    // il caut in valuesList
                                    var valueCurr = valuesList.FirstOrDefault(f => f.RandId == randFormCurr.Id && f.DataLuna == valueItem.DataLuna && f.ActivityType == valueItem.ActivityType
                                                                              && f.DepartamentId == valueItem.DepartamentId);
                                    if (!valueCurr.Calculat)
                                    {
                                        formulaOk = false;
                                    }
                                    else
                                    {
                                        suma += (semneList[i] == "+" ? 1 : -1) * valueCurr.Valoare;
                                    }
                                }
                                else
                                {
                                    var valueCurr = bvcPrevValues.FirstOrDefault(f => f.DepartamentId == valueItem.DepartamentId && f.FormRandId == randFormCurr.Id);
                                    var sumaCurr = valueCurr.ValueList.Where(f => f.ActivityTypeId == valueItem.ActivityType && f.DataLuna == valueItem.DataLuna).Sum(f => f.Value);
                                    suma += sumaCurr;
                                }
                            }
                        }

                        if (formulaOk)
                        {
                            valueItem.Valoare = suma;
                            valueItem.Calculat = true;
                        }

                    }
                    var count = valuesList.Count(f => !f.Calculat);
                    okCalcul = (count == 0);

                    contor++;
                }

                if (!okCalcul)
                {
                    throw new Exception("Verificati corectitudinea formulelor. Exista elemente care au fost specificate in formula si nu au fost calculate");
                }

                // inregistrez in db
                foreach (var valueItem in valuesList)
                {
                    var randFormCurr = bvcFormRanduri.FirstOrDefault(f => f.Id == valueItem.RandId);
                    var valuesCurr = bvcPrevValues.FirstOrDefault(f => f.DepartamentId == valueItem.DepartamentId && f.FormRandId == randFormCurr.Id);
                    var valueCurr = valuesCurr.ValueList.FirstOrDefault(f => f.ActivityTypeId == valueItem.ActivityType && f.DataLuna == valueItem.DataLuna);
                    if (valueCurr == null)
                    {
                        var newValue = new BVC_BugetPrevRandValue
                        {
                            ActivityTypeId = valueItem.ActivityType,
                            BugetPrevRandId = valuesCurr.Id,
                            DataLuna = valueItem.DataLuna,
                            DataOper = valueItem.DataLuna,
                            Value = valueItem.Valoare,
                            ValueType = BVC_PrevValueType.Manual
                        };
                        Context.BVC_BugetPrevRandValue.Add(newValue);
                    }
                    else
                    { 
                        valueCurr.Value = valueItem.Valoare;
                    }
                }

                Context.SaveChanges();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DesfacFormula(string formula, out List<string> semneList, out List<int> codRandList)
        {
            try
            {
                semneList = new List<string>();
                codRandList = new List<int>();
                if (!formula.StartsWith("+") && !formula.StartsWith("-"))
                {
                    formula = "+" + formula;
                }

                while (formula.Length != 0)
                {
                    string aux = "";

                    string semn = formula.Substring(0, 1);
                    semneList.Add(semn);
                    formula = formula.Substring(1);

                    int indexAdun = formula.IndexOf("+");
                    int indexScad = formula.IndexOf("-");
                    int index = 0;

                    if (indexAdun == -1 && indexScad == -1)
                    {
                        index = formula.Length;
                    }
                    else if (indexScad == -1)
                    {
                        index = indexAdun;
                    }
                    else if (indexAdun == -1)
                    {
                        index = indexScad;
                    }
                    else if (indexAdun < indexScad)
                    {
                        index = indexAdun;
                    }
                    else
                    {
                        index = indexScad;
                    }

                    aux = formula.Substring(0, index);
                    aux = aux.Replace("#", "");
                    int auxCod = int.Parse(aux);
                    codRandList.Add(auxCod);
                    formula = formula.Substring(index);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Eroare procesare formula " + formula + " - " + ex.ToString());
            }

        }

        public List<BugetPrevAllDepartmentsDto> GetBugetPrevDetails(int? departamentId, int bugetPrevId, string month)
        {
            var dataLunaSel = new DateTime();
            if (month != "all")
            {
                dataLunaSel = DateTime.ParseExact(month, "ddMMyyyy", null);
            }
            var ret = new List<BugetPrevAllDepartmentsDto>();
            var query = from f in Context.BVC_FormRand
                        join r in Context.BVC_BugetPrevRand on f.Id equals r.FormRandId
                        join v in Context.BVC_BugetPrevRandValue.Where(f=>f.DataLuna == (month != "all" ? dataLunaSel : f.DataLuna)) on r.Id equals v.BugetPrevRandId into values
                        from allValues in values.DefaultIfEmpty()
                        where r.BugetPrevId == bugetPrevId
                        select new BugetPrevAllDepartmentsDto
                        {
                            FormRandId = f.Id,
                            Descriere = f.Descriere,
                            OrderView = f.OrderView,
                            Valoare = allValues.Value,
                            Validat = r.Validat
                        };
            ret = query.GroupBy(f => new { f.FormRandId, f.Descriere, f.OrderView })
                                .Select(f => new BugetPrevAllDepartmentsDto
                                {
                                    FormRandId = f.Key.FormRandId,
                                    Descriere = f.Key.Descriere,
                                    OrderView = f.Key.OrderView,
                                    Valoare = f.Sum(g => g.Valoare),
                                    Validat = f.Min(g => g.Validat ? 1 : 0) == 0 ? false : true//(f.Min(r => (r.Validat ? 1 : 0) == 0))
                                })
                                .OrderBy(f=>f.OrderView)
                                .ToList();

            return ret;
        }

        public List<BugetPrevazutModel> GetBugetPrevValues(BVC_FormRand rand, DateTime dataStart, DateTime dataEnd, int bugetPrevId)
        {
            var ret = new List<BugetPrevazutModel>();
            var activityTypeList = Context.ActivityTypes.ToList();
            var query = from f in Context.BVC_FormRand
                        join r in Context.BVC_BugetPrevRand on f.Id equals r.FormRandId
                        join v in Context.BVC_BugetPrevRandValue.Where(f => dataStart <= f.DataLuna && f.DataLuna <= dataEnd /*&& f.ActivityTypeId == tipActivitate*/ && 
                                                                       f.BugetPrevRand.FormRand.FormularId == rand.FormularId && f.BugetPrevRand.FormRand.TipRand == rand.TipRand 
                                                                       && f.BugetPrevRand.FormRand.NivelRand == rand.NivelRand && f.BugetPrevRand.FormRand.Id == rand.Id) 
                                                                            on r.Id equals v.BugetPrevRandId
                        into values from allValues in values
                        where r.BugetPrevId == bugetPrevId
                        select new BugetPrevazutModel
                        {
                            Id = f.Id,
                            TipRand = (int)f.TipRand,
                            DataLuna = dataEnd,
                            DenumireRand = f.Descriere,
                            OrderView = f.OrderView,
                            Valoare = allValues.Value,
                            ActivityTypeId = allValues.ActivityTypeId
                        };
            if (query.Count() == 0)
            {
                foreach (var activityType in activityTypeList)
                {
                    ret.Add(new BugetPrevazutModel()
                    {
                        Id = rand.Id,
                        TipRand = (int)rand.TipRand,
                        DataLuna = dataEnd,
                        DenumireRand = rand.Descriere,
                        OrderView = rand.OrderView,
                        Valoare = 0,
                        ActivityTypeId = activityType.Id
                    });
                }
            }else
            {
                ret = query.ToList();
            }

            //ret = query.OrderBy(f => new { f.OrderView, f.DataBuget })
            //    .Select(f => new BugetPrevazutModel
            //     {
            //         FormRandId = f.Key.FormRandId,
            //         DataBuget = f.Key.DataBuget,
            //         DenumireRand = f.Key.DenumireRand,
            //         OrderView = f.Key.OrderView,
            //         Valoare = f.Sum(g => g.Valoare)
            //     }).OrderBy(f => f.OrderView).ToList();
            return ret;
        }

        public List<BugetPrevazutModel> ComputePrevazutTrimestrial(List<BugetPrevazutModel> prevazutList)
        {
           
            foreach (var item in prevazutList)
            {
                item.DataLuna = LazyMethods.GetQuarterByDate(item.DataLuna);
            }

            prevazutList = prevazutList.GroupBy(f => new { f.TipRand, f.DataLuna, f.OrderView, f.DenumireRand, f.Id }).Select(f => new BugetPrevazutModel
            {
                Id = f.Key.Id,
              
                TipRand = f.Key.TipRand,
                DataLuna = f.Key.DataLuna,
                Valoare = f.Sum(g => g.Valoare),
                DenumireRand = f.Key.DenumireRand,
                ActivityTypeId = f.FirstOrDefault().ActivityTypeId,
                OrderView = f.Key.OrderView,

            }).OrderBy(f => f.OrderView).ToList();

            return prevazutList;


        }

        public List<BugetPrevazutModel> ComputePrevazutLunar(List<BugetPrevazutModel> randuriList)
        {
            try
            {
                randuriList = randuriList.GroupBy(f => new { f.TipRand, f.DataLuna, f.Valoare, f.OrderView, f.DenumireRand }).Select(f => new BugetPrevazutModel
                {
                    TipRand = f.Key.TipRand,
                    DataLuna = f.Key.DataLuna,
                    Valoare = f.Sum(g => g.Valoare),
                    DenumireRand = f.Key.DenumireRand,
                    ActivityTypeId = f.FirstOrDefault().ActivityTypeId,
                    OrderView = f.Key.OrderView,

                }).OrderBy(f => f.OrderView).ToList();

                return randuriList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<BugetPrevazutModel> ComputePrevazutTotal(List<BugetPrevazutModel> randuriList)
        {
            try
            {
                var an = randuriList.Select(f => f.DataLuna).FirstOrDefault().Year;
                randuriList = randuriList.GroupBy(f => new { f.TipRand, f.OrderView, f.DenumireRand }).Select(f => new BugetPrevazutModel
                {
                    TipRand = f.Key.TipRand,
                    An = an,
                    Valoare = f.Sum(g => g.Valoare),
                    DenumireRand = f.Key.DenumireRand,
                    ActivityTypeId = f.FirstOrDefault().ActivityTypeId,
                    OrderView = f.Key.OrderView,

                }).OrderBy(f => f.OrderView).ToList();

                return randuriList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void BVC_PrevCheltuieli(int bugetPrevId)
        {
            try
            {
                var bvcPrev = Context.BVC_BugetPrev.Include(f => f.Formular).FirstOrDefault(f => f.Id == bugetPrevId);
                var startDate = new DateTime(bvcPrev.Formular.AnBVC, bvcPrev.MonthStart, 1);
                var endDate = LazyMethods.LastDayOfMonth(new DateTime(bvcPrev.Formular.AnBVC, bvcPrev.MonthEnd, 1));

                var valuesList = new List<ActiveBugetPrepareAddDto>();

                var bvcCheltuieliList = Context.BVC_Cheltuieli.Include(f => f.Departament).Include(f => f.BVC_FormRand).Include(f => f.Currency)
                                                              .Where(f => startDate <= f.DataIncasare && f.DataIncasare<= endDate && f.BVC_FormRand.AvailableBVC && !f.BVC_FormRand.IsTotal)
                                                              .ToList();

                var monedeList = bvcCheltuieliList.Select(f => f.CurrencyId).Distinct().ToList();

                foreach (var moneda in monedeList)
                {
                    decimal exchangeRate = 1;
                    var currencyItem = Context.Currency.FirstOrDefault(f => f.Id == moneda);
                    if (currencyItem.CurrencyCode != "RON")
                    {
                        var exchangeRateItem = Context.ExchangeRateForecasts.FirstOrDefault(f => f.State == Models.Conta.Enums.State.Active && f.Year == bvcPrev.Formular.AnBVC
                                                                                            && f.CurrencyId == currencyItem.Id);
                        if (exchangeRateItem == null)
                        {
                            throw new Exception("Nu ati completat cursul valutar estimat pentru " + currencyItem.CurrencyCode + " pentru anul " + bvcPrev.Formular.AnBVC.ToString());
                        }
                        exchangeRate = exchangeRateItem.ValoareEstimata;
                    }

                    foreach (var cheltuiala in bvcCheltuieliList.Where(f => f.CurrencyId == moneda))
                    {
                        var randCheltuiala = Context.BVC_BugetPrevRand.Include(f => f.ValueList)
                                              .FirstOrDefault(f => f.BugetPrevId == bugetPrevId && f.FormRandId == cheltuiala.BVC_FormRandId && f.DepartamentId == cheltuiala.DepartamentId);

                        var currDate = LazyMethods.LastDayOfMonth(cheltuiala.DataIncasare);
                        var randValue = valuesList.FirstOrDefault(f => f.ActivityType == cheltuiala.ActivityTypeId && f.DataLuna == currDate && f.RandCheltuialaId == randCheltuiala.Id);
                        if (randValue == null)
                        {
                            randValue = new ActiveBugetPrepareAddDto
                            {
                                ActivityType = cheltuiala.ActivityTypeId,
                                DataLuna = currDate,
                                Valoare = Math.Round(cheltuiala.Value * exchangeRate, 2),
                                RandCheltuialaId = randCheltuiala.Id
                            };
                            valuesList.Add(randValue);
                        }
                        else
                        {
                            randValue.Valoare += Math.Round(cheltuiala.Value * exchangeRate, 2);
                        }
                    }
                }

                // salvez valorile in db

                foreach (var item in valuesList)
                {
                    var randValue = Context.BVC_BugetPrevRandValue.FirstOrDefault(f => f.BugetPrevRandId == item.RandCheltuialaId && f.DataLuna == item.DataLuna
                                                                                  && f.ActivityTypeId == item.ActivityType);
                    if (randValue == null)
                    {
                        randValue = new BVC_BugetPrevRandValue
                        {
                            ActivityTypeId = item.ActivityType,
                            DataLuna = item.DataLuna,
                            DataOper = item.DataLuna,
                            BugetPrevRandId = item.RandCheltuialaId.Value,
                            Description = "Cheltuiala estimata",
                            TenantId = 1,
                            ValueType = BVC_PrevValueType.Cheltuieli,
                            Value = item.Valoare
                        };
                        Context.Add(randValue);
                    }
                    else
                    {
                        randValue.Value += item.Valoare;
                    }
                }
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void BVC_CashFlowCheltuieli(int bugetPrevId)
        {
            try
            {
                var bvcPrev = Context.BVC_BugetPrev.Include(f => f.Formular).FirstOrDefault(f => f.Id == bugetPrevId);
                var startDate = new DateTime(bvcPrev.Formular.AnBVC, bvcPrev.MonthStart, 1);
                var endDate = LazyMethods.LastDayOfMonth(new DateTime(bvcPrev.Formular.AnBVC, bvcPrev.MonthEnd, 1));

                var valuesList = new List<ActiveCashFlowPrepareAddDto>();

                var cashFlowCheltuieliList = Context.BVC_Cheltuieli.Include(f => f.Departament).Include(f => f.BVC_FormRand).Include(f => f.Currency)
                                                                   .Where(f => startDate <= f.DataIncasare && f.DataIncasare <= endDate && f.BVC_FormRand.AvailableCashFlow && !f.BVC_FormRand.IsTotal)
                                                                   .ToList();

                var monedeList = cashFlowCheltuieliList.Select(f => f.CurrencyId).Distinct().ToList();

                foreach (var moneda in monedeList)
                {
                    decimal exchangeRate = 1;
                    var currencyItem = Context.Currency.FirstOrDefault(f => f.Id == moneda);
                    if (currencyItem.CurrencyCode != "RON")
                    {
                        var exchangeRateItem = Context.ExchangeRateForecasts.FirstOrDefault(f => f.State == Models.Conta.Enums.State.Active && f.Year == bvcPrev.Formular.AnBVC
                                                                                            && f.CurrencyId == currencyItem.Id);
                        if (exchangeRateItem == null)
                        {
                            throw new Exception("Nu ati completat cursul valutar estimat pentru " + currencyItem.CurrencyCode + " pentru anul " + bvcPrev.Formular.AnBVC.ToString());
                        }
                        exchangeRate = exchangeRateItem.ValoareEstimata;
                    }

                    foreach (var cheltuiala in cashFlowCheltuieliList.Where(f => f.CurrencyId == moneda))
                    {
                        var randCheltuiala = Context.BVC_BugetPrevRand.Include(f => f.ValueList)
                                                                      .FirstOrDefault(f => f.BugetPrevId == bugetPrevId && f.FormRandId == cheltuiala.BVC_FormRandId && f.DepartamentId == cheltuiala.DepartamentId);
                        var currDate = LazyMethods.LastDayOfMonth(cheltuiala.DataIncasare);
                        var randValue = valuesList.FirstOrDefault(f => f.ActivityType == cheltuiala.ActivityTypeId && f.DataLuna == currDate && f.DataOper == cheltuiala.DataIncasare && f.RandCheltuialaId == randCheltuiala.Id);
                        if (randValue == null)
                        {
                            randValue = new ActiveCashFlowPrepareAddDto
                            {
                                ActivityType = cheltuiala.ActivityTypeId,
                                DataLuna = currDate,
                                DataOper = cheltuiala.DataIncasare,
                                Valoare = Math.Round(cheltuiala.Value * exchangeRate, 2),
                                RandCheltuialaId = randCheltuiala.Id
                            };
                            valuesList.Add(randValue);
                        }
                        else
                        {
                            randValue.Valoare += Math.Round(cheltuiala.Value * exchangeRate, 2);
                        }
                    }
                }

                    // salvez valorile in db
                    foreach (var item in valuesList)
                    {
                        var randValue = new BVC_BugetPrevRandValue();

                        randValue = Context.BVC_BugetPrevRandValue.FirstOrDefault(f => f.BugetPrevRandId == item.RandCheltuialaId && f.DataLuna == item.DataLuna
                                                                                      && f.DataOper == item.DataOper && f.ActivityTypeId == item.ActivityType);

                        if (randValue == null)
                        {
                            randValue = new BVC_BugetPrevRandValue
                            {
                                ActivityTypeId = item.ActivityType,
                                DataLuna = item.DataLuna,
                                DataOper = item.DataOper,
                                BugetPrevRandId = item.RandCheltuialaId.Value,
                                Description = "Cheltuiala estimata",
                                TenantId = 1,
                                ValueType = BVC_PrevValueType.Cheltuieli,
                                Value = item.Valoare
                            };
                            Context.Add(randValue);
                        }
                        else
                        {
                            randValue.Value += item.Valoare;
                        }
                    }

                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public class BugetPrevRandValuePrepare
    {
        public int BugetPrevRandId { get; set; }
        public DateTime DataOper { get; set; }
        public DateTime DataLuna { get; set; }
        public string Description { get; set; }
        public BVC_PrevValueType ValueType { get; set; }
        public decimal Value { get; set; }
        public int? ActivityTypeId { get; set; }
    }
}
