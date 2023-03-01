using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Models.Buget;
using Niva.Erp.Models.Buget.BVC;
using Niva.Erp.Repositories.Buget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Niva.Erp.EntityFrameworkCore.Repositories.Buget
{
    public class BVC_VenituriRepository : ErpRepositoryBase<BVC_VenitTitlu, int>, IBVC_VenituriRepository
    {
        public BVC_VenituriRepository(IDbContextProvider<ErpDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public void AplicaBVCsiCashFlow(int bugetPrevId)
        {
            try
            {
                var randuriDeSters = Context.BVC_BugetPrevRandValue.Include(f => f.BugetPrevRand).Where(f => f.BugetPrevRand.BugetPrevId == bugetPrevId && f.EsteDinVenituri);
                Context.BVC_BugetPrevRandValue.RemoveRange(randuriDeSters);

                var bvcPrev = Context.BVC_BugetPrev.Include(f => f.Formular).FirstOrDefault(f => f.Id == bugetPrevId);
                var startDate = new DateTime(bvcPrev.Formular.AnBVC, 1, 1);
                var endDate = new DateTime(bvcPrev.Formular.AnBVC, 12, 31);
                var dept = Context.BVC_BugetPrevAutoValue.FirstOrDefault(f => f.TipRand == BVC_RowType.Venituri && f.TipRandVenit == BVC_RowTypeIncome.Dobanzi);
                if (dept == null)
                {
                    throw new Exception("Nu ati specificat departamentul pentru care se vor inregistra veniturile din dobanzi");
                }
                var deptId = dept.DepartamentId;
                if (bvcPrev.BVC_Tip == BVC_Tip.BVC)
                {
                    AplicaBVC(bvcPrev, deptId);
                }
                else
                {
                    AplicaCF(bvcPrev, deptId);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SaveVenitTitluParams(int formularBVCId, int monthStart, int monthEnd)
        {
            try
            {
                var venitTitluParams = new BVC_VenitTitluParams
                {
                    FormularId = formularBVCId,
                    MonthEnd = monthEnd,
                    MonthStart = monthStart
                };

                Context.BVC_VenitTitluParams.Add(venitTitluParams);
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void AplicaBVC(BVC_BugetPrev bvcPrev, int deptId)
        {
            try
            {
                var dataStartBVC = new DateTime(bvcPrev.Formular.AnBVC, bvcPrev.MonthStart, 1);
                var dataEndBVC = LazyMethods.LastDayOfMonth(new DateTime(bvcPrev.Formular.AnBVC, bvcPrev.MonthEnd, 1));

                var randFormVenit = Context.BVC_FormRand.FirstOrDefault(f => f.FormularId == bvcPrev.FormularId && f.TipRand == BVC_RowType.Venituri && f.TipRandVenit == BVC_RowTypeIncome.Dobanzi
                                                                         && !f.IsTotal && f.AvailableBVC);

                if (randFormVenit == null) // nu am venituri din dobanzi definite => am terminat
                {
                    return;
                }

                var randVenit = Context.BVC_BugetPrevRand.Include(f => f.ValueList)
                                                                    .FirstOrDefault(f => f.BugetPrevId == bvcPrev.Id && f.FormRandId == randFormVenit.Id && f.DepartamentId == deptId);

                // titluri din trezo si titluri preliminate
                var titluri = Context.BVC_VenitTitluBVC.Include(f => f.BVC_VenitTitlu).Where(f => f.BVC_VenitTitlu.FormularId == bvcPrev.FormularId).ToList();
                var valoriBVC = Context.BVC_BugetPrevRandValue.Include(f => f.BugetPrevRand)
                                                              .Where(f => f.BugetPrevRandId == randVenit.Id && f.ValueType == BVC_PrevValueType.Dobanzi)
                                                              .Select(f => new BVC_BugetPrevRandValueDto
                                                              {
                                                                  //Id = f.Id,
                                                                  DataLuna = f.DataLuna,
                                                                  DataOper = f.DataOper,
                                                                  Description = f.Description,
                                                                  ActivityTypeId = f.ActivityTypeId,
                                                                  BugetPrevRandId = f.BugetPrevRandId,
                                                                  Value = 0,
                                                                  ValueType = f.ValueType
                                                              })
                                                              .ToList();

                var monedeList = titluri.GroupBy(f => f.BVC_VenitTitlu.CurrencyId).Select(f => f.Key).ToList();

                foreach (var moneda in monedeList)
                {
                    decimal exchangeRate = 1;
                    if (moneda != 1)
                    {
                        var currencyItem = Context.Currency.FirstOrDefault(f => f.Id == moneda);
                        var exchangeRateItem = Context.ExchangeRateForecasts.FirstOrDefault(f => f.State == Models.Conta.Enums.State.Active && f.Year == bvcPrev.Formular.AnBVC
                                                                                            && f.CurrencyId == currencyItem.Id);
                        if (exchangeRateItem == null)
                        {
                            throw new Exception("Nu ati completat cursul valutar estimat pentru " + moneda + " pentru anul " + bvcPrev.Formular.AnBVC.ToString());
                        }
                        exchangeRate = exchangeRateItem.ValoareEstimata;
                    }

                    var titluriMoneda = titluri.Where(f => f.BVC_VenitTitlu.CurrencyId == moneda).ToList();
                    foreach (var titlu in titluriMoneda)
                    {
                        var dataLuna = LazyMethods.LastDayOfMonth(titlu.DataDobanda);
                        var valueItem = valoriBVC.FirstOrDefault(f => f.DataLuna == dataLuna && f.ActivityTypeId == titlu.BVC_VenitTitlu.ActivityTypeId);

                        if (valueItem != null)
                        {
                            valueItem.Value += Math.Round(titlu.DobandaLuna * exchangeRate, 2);
                        }
                        else
                        {
                            valueItem = new BVC_BugetPrevRandValueDto
                            {
                                DataLuna = dataLuna,
                                DataOper = dataLuna,
                                Description = "Dobanzi titluri",
                                ActivityTypeId = titlu.BVC_VenitTitlu.ActivityTypeId,
                                BugetPrevRandId = randVenit.Id,
                                Value = Math.Round(titlu.DobandaLuna * exchangeRate, 2),
                                ValueType = BVC_PrevValueType.Dobanzi
                            };
                            valoriBVC.Add(valueItem);
                        }
                    }
                }

                foreach (var item in valoriBVC)
                {
                    var valueDB = Context.BVC_BugetPrevRandValue.FirstOrDefault(f => f.Id == item.Id);
                    if (valueDB == null)
                    {
                        valueDB = new BVC_BugetPrevRandValue
                        {
                            DataLuna = item.DataLuna,
                            DataOper = item.DataOper,
                            BugetPrevRandId = item.BugetPrevRandId,
                            ActivityTypeId = item.ActivityTypeId,
                            Description = item.Description,
                            Value = item.Value,
                            ValueType = item.ValueType,
                            EsteDinVenituri = true
                        };
                        Context.BVC_BugetPrevRandValue.Add(valueDB);
                    }
                    else
                    {
                        valueDB.Value = item.Value;
                    }
                }
                Context.SaveChanges();

                // venituri reinvestite
                var valoriBVCReinv = new List<BVC_BugetPrevRandValueDto>();
                var titluriReinv = Context.BVC_VenitTitluCFReinv.Include(f => f.BVC_VenitTitluCF).Include(f => f.BVC_VenitTitluCF.BVC_VenitTitlu)
                                                                .Include(f => f.BVC_VenitTitluCF.BVC_VenitTitlu.Formular)
                                                                .Where(f => f.BVC_VenitTitluCF.BVC_VenitTitlu.FormularId == bvcPrev.FormularId && f.BVC_VenitTitluCF.BVC_VenitTitlu.Selectat == true)
                                                                .ToList();

                var monedeReinvList = titluriReinv.GroupBy(f => f.BVC_VenitTitluCF.BVC_VenitTitlu.CurrencyId).Select(f => f.Key).ToList();

                foreach (var moneda in monedeReinvList)
                {
                    decimal exchangeRate = 1;
                    if (moneda != 1)
                    {
                        var currencyItem = Context.Currency.FirstOrDefault(f => f.Id == moneda);
                        var exchangeRateItem = Context.ExchangeRateForecasts.FirstOrDefault(f => f.State == Models.Conta.Enums.State.Active && f.Year == bvcPrev.Formular.AnBVC
                                                                                            && f.CurrencyId == currencyItem.Id);
                        if (exchangeRateItem == null)
                        {
                            throw new Exception("Nu ati completat cursul valutar estimat pentru " + moneda + " pentru anul " + bvcPrev.Formular.AnBVC.ToString());
                        }
                        exchangeRate = exchangeRateItem.ValoareEstimata;
                    }

                    var titluriMoneda = titluriReinv.Where(f => f.BVC_VenitTitluCF.BVC_VenitTitlu.CurrencyId == moneda).ToList();
                    foreach (var titlu in titluriMoneda)
                    {
                        decimal nrZileTitlu = (decimal)(titlu.DataReinvestire.AddMonths(12) - titlu.DataReinvestire).TotalDays;
                        var procentDobanda = titlu.ProcDobanda;
                        var dobandaTotala = Math.Round(titlu.SumaReinvestita * procentDobanda / 100, 2);

                        var currDate = LazyMethods.LastDayOfMonth((dataStartBVC <= titlu.DataReinvestire) ? titlu.DataReinvestire : dataStartBVC);
                        while (currDate <= LazyMethods.LastDayOfMonth(dataEndBVC))
                        {
                            var dataLuna = LazyMethods.LastDayOfMonth(currDate);
                            int nrZileLuna = 0;
                            if (titlu.DataReinvestire <= dataLuna && titlu.DataReinvestire.Month == dataLuna.Month)
                            {
                                nrZileLuna = (int)(dataLuna - titlu.DataReinvestire).TotalDays + 1;
                            }
                            else
                            {
                                nrZileLuna = dataLuna.Day;
                            }
                            var dobandaLuna = Math.Round(dobandaTotala * nrZileLuna / nrZileTitlu, 2);
                            var valueItem = valoriBVCReinv.FirstOrDefault(f => f.DataLuna == dataLuna && f.ActivityTypeId == titlu.BVC_VenitTitluCF.BVC_VenitTitlu.ActivityTypeId);
                            if (valueItem != null)
                            {
                                valueItem.Value += Math.Round(dobandaLuna * exchangeRate, 2);
                            }
                            else
                            {
                                valueItem = new BVC_BugetPrevRandValueDto
                                {
                                    DataLuna = dataLuna,
                                    DataOper = dataLuna,
                                    Description = "Dobanzi titluri reinvestite",
                                    ActivityTypeId = titlu.BVC_VenitTitluCF.BVC_VenitTitlu.ActivityTypeId,
                                    BugetPrevRandId = randVenit.Id,
                                    Value = Math.Round(dobandaLuna * exchangeRate, 2),
                                    ValueType = BVC_PrevValueType.Dobanzi
                                };
                                valoriBVCReinv.Add(valueItem);
                            }

                            currDate = LazyMethods.LastDayOfMonth(currDate.AddMonths(1));
                        }

                    }
                }

                foreach (var item in valoriBVCReinv)
                {
                    var valueDB = Context.BVC_BugetPrevRandValue.FirstOrDefault(f => f.Id == item.Id);
                    if (valueDB == null)
                    {
                        valueDB = new BVC_BugetPrevRandValue
                        {
                            DataLuna = item.DataLuna,
                            DataOper = item.DataOper,
                            BugetPrevRandId = item.BugetPrevRandId,
                            ActivityTypeId = item.ActivityTypeId,
                            Description = item.Description,
                            Value = item.Value,
                            ValueType = item.ValueType,
                            EsteDinVenituri = true
                        };
                        Context.BVC_BugetPrevRandValue.Add(valueDB);
                    }
                    else
                    {
                        valueDB.Value = item.Value;
                    }
                }
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private void AplicaCF(BVC_BugetPrev bvcPrev, int deptId)
        {
            try
            {
                var dataEndBVC = LazyMethods.LastDayOfMonth(new DateTime(bvcPrev.Formular.AnBVC, bvcPrev.MonthEnd, 1));
                var randFormVenit = Context.BVC_FormRand.FirstOrDefault(f => f.FormularId == bvcPrev.FormularId && f.TipRand == BVC_RowType.Venituri && f.TipRandVenit == BVC_RowTypeIncome.Dobanzi
                                                                         && !f.IsTotal && f.AvailableCashFlow);

                if (randFormVenit != null) // nu am venituri din dobanzi definite => am terminat
                {


                    var randVenit = Context.BVC_BugetPrevRand.Include(f => f.ValueList)
                                                                        .FirstOrDefault(f => f.BugetPrevId == bvcPrev.Id && f.FormRandId == randFormVenit.Id && f.DepartamentId == deptId);

                    var titluri = Context.BVC_VenitTitluCF.Include(f => f.BVC_VenitTitlu).Where(f => f.BVC_VenitTitlu.FormularId == bvcPrev.FormularId).ToList();
                    var valoriCF = Context.BVC_BugetPrevRandValue.Include(f => f.BugetPrevRand)
                                                                  .Where(f => f.BugetPrevRandId == randVenit.Id && f.ValueType == BVC_PrevValueType.Dobanzi)
                                                                  .Select(f => new BVC_BugetPrevRandValueDto
                                                                  {
                                                                      //Id = f.Id,
                                                                      DataLuna = f.DataLuna,
                                                                      DataOper = f.DataOper,
                                                                      Description = f.Description,
                                                                      ActivityTypeId = f.ActivityTypeId,
                                                                      BugetPrevRandId = f.BugetPrevRandId,
                                                                      Value = 0,
                                                                      ValueType = f.ValueType
                                                                  })
                                                                  .ToList();

                    var monedeList = titluri.GroupBy(f => f.BVC_VenitTitlu.CurrencyId).Select(f => f.Key).ToList();

                    foreach (var moneda in monedeList)
                    {
                        decimal exchangeRate = 1;
                        if (moneda != 1)
                        {
                            var currencyItem = Context.Currency.FirstOrDefault(f => f.Id == moneda);
                            var exchangeRateItem = Context.ExchangeRateForecasts.FirstOrDefault(f => f.State == Models.Conta.Enums.State.Active && f.Year == bvcPrev.Formular.AnBVC
                                                                                                && f.CurrencyId == currencyItem.Id);
                            if (exchangeRateItem == null)
                            {
                                throw new Exception("Nu ati completat cursul valutar estimat pentru " + moneda + " pentru anul " + bvcPrev.Formular.AnBVC.ToString());
                            }
                            exchangeRate = exchangeRateItem.ValoareEstimata;
                        }

                        var titluriMoneda = titluri.Where(f => f.BVC_VenitTitlu.CurrencyId == moneda).ToList();
                        foreach (var titlu in titluriMoneda)
                        {
                            var dataLuna = LazyMethods.LastDayOfMonth(titlu.DataIncasare);
                            var valueItem = valoriCF.FirstOrDefault(f => f.DataLuna == dataLuna && f.ActivityTypeId == titlu.BVC_VenitTitlu.ActivityTypeId);

                            if (valueItem != null)
                            {
                                valueItem.Value += Math.Round(titlu.DobandaTotala + titlu.ValoarePlasament * exchangeRate, 2);
                            }
                            else
                            {
                                valueItem = new BVC_BugetPrevRandValueDto
                                {
                                    DataLuna = dataLuna,
                                    DataOper = dataLuna,
                                    Description = "Dobanzi titluri",
                                    ActivityTypeId = titlu.BVC_VenitTitlu.ActivityTypeId,
                                    BugetPrevRandId = randVenit.Id,
                                    Value = Math.Round(titlu.DobandaTotala + titlu.ValoarePlasament * exchangeRate, 2),
                                    ValueType = BVC_PrevValueType.Dobanzi
                                };
                                valoriCF.Add(valueItem);
                            }
                        }
                    }

                    foreach (var item in valoriCF)
                    {
                        var valueDB = Context.BVC_BugetPrevRandValue.FirstOrDefault(f => f.Id == item.Id);
                        if (valueDB == null)
                        {
                            valueDB = new BVC_BugetPrevRandValue
                            {
                                DataLuna = item.DataLuna,
                                DataOper = item.DataOper,
                                BugetPrevRandId = item.BugetPrevRandId,
                                ActivityTypeId = item.ActivityTypeId,
                                Description = item.Description,
                                Value = item.Value,
                                ValueType = item.ValueType,
                                EsteDinVenituri = true
                            };
                            Context.BVC_BugetPrevRandValue.Add(valueDB);
                        }
                        else
                        {
                            valueDB.Value = item.Value;
                        }
                    }

                }


                // adaug sumele reinvestite pentru a le folosi la generarea bugetului
                Context.BVC_BugetPrevSumeReinvest.RemoveRange(Context.BVC_BugetPrevSumeReinvest.Where(f => f.BugetPrevId == bvcPrev.Id));
                var sumeReinvestite = Context.BVC_VenitTitluCFReinv.Include(f => f.BVC_VenitTitluCF).ThenInclude(f => f.BVC_VenitTitlu)
                                             .Where(f => f.BVC_VenitTitluCF.BVC_VenitTitlu.FormularId == bvcPrev.FormularId)
                                             .ToList();
                foreach (var suma in sumeReinvestite)
                {
                    var newSuma = new BVC_BugetPrevSumeReinvest
                    {
                        BugetPrevId = bvcPrev.Id,
                        CurrencyId = /*suma.BVC_VenitTitluCF.BVC_VenitTitlu.CurrencyId*/ suma.CurrencyId.Value,
                        IdPlasament = suma.BVC_VenitTitluCF.BVC_VenitTitlu.IdPlasament + " - reinvestit",
                        StartDate = suma.DataReinvestire,
                        MaturityDate = suma.DataReinvestire.AddMonths(12),
                        ProcentDobanda = suma.ProcDobanda,
                        TipPlasament = suma.BVC_VenitTitluCF.BVC_VenitTitlu.TipPlasament,
                        ActivityTypeId = suma.BVC_VenitTitluCF.BVC_VenitTitlu.ActivityTypeId,
                        State = Models.Conta.Enums.State.Active,
                        ValoarePlasament = suma.SumaReinvestita,
                        VenitType = suma.BVC_VenitTitluCF.BVC_VenitTitlu.VenitType
                    };
                    Context.BVC_BugetPrevSumeReinvest.Add(newSuma);
                }

                // adaug titlurile care au scadenta dupa data end => le folosesc la calculul resurselor
                Context.BVC_BugetPrevTitluriValab.RemoveRange(Context.BVC_BugetPrevTitluriValab.Where(f => f.BugetPrevId == bvcPrev.Id));
                var titluriValab = Context.BVC_VenitTitlu.Where(f => f.FormularId == bvcPrev.FormularId && f.MaturityDate > dataEndBVC)
                                                         .ToList();
                foreach (var suma in titluriValab)
                {
                    var newSuma = new BVC_BugetPrevTitluriValab
                    {
                        BugetPrevId = bvcPrev.Id,
                        CurrencyId = /*suma.BVC_VenitTitluCF.BVC_VenitTitlu.CurrencyId*/ suma.CurrencyId,
                        IdPlasament = suma.IdPlasament,
                        StartDate = suma.StartDate,
                        MaturityDate = suma.MaturityDate,
                        ProcentDobanda = suma.ProcentDobanda,
                        TipPlasament = suma.TipPlasament,
                        ActivityTypeId = suma.ActivityTypeId,
                        State = Models.Conta.Enums.State.Active,
                        ValoarePlasament = suma.ValoarePlasament,
                        VenitType = suma.VenitType
                    };
                    Context.BVC_BugetPrevTitluriValab.Add(newSuma);
                }


                Context.SaveChanges();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public decimal RetDobRefEstimat(int BVCFormularId, BVC_PlasamentType tipPlasament, DateTime dataConstituire, int currencyId, int? activityTypeId)
        {
            decimal rez = 0;
            var dobanziReferinta = Context.BVC_DobandaReferinta.Where(f => f.FormularId == BVCFormularId && f.State == Models.Conta.Enums.State.Active).ToList();
            rez = RetDobRefEstimatCuLista(dobanziReferinta, tipPlasament, dataConstituire, currencyId, activityTypeId);
            return rez;
        }

        public decimal RetDobRefEstimatCuLista(List<BVC_DobandaReferinta> dobanziReferinta, BVC_PlasamentType TipPlasament, DateTime dataConstituire, int currencyId, int? activityTypeId)
        {
            decimal rez = 0;
            // caut dobanda din data constituirii
            var dobandaItem = dobanziReferinta.FirstOrDefault(f => f.PlasamentType == TipPlasament && f.State == Models.Conta.Enums.State.Active
                                                              && f.DataStart == dataConstituire && dataConstituire == f.DataEnd && f.CurrencyId == currencyId && f.ActivityTypeId == activityTypeId);
            if (dobandaItem == null)
            {
                dobandaItem = dobanziReferinta.FirstOrDefault(f => f.PlasamentType == null && f.State == Models.Conta.Enums.State.Active
                                                             && f.DataStart == dataConstituire && dataConstituire == f.DataEnd && f.CurrencyId == currencyId && f.ActivityTypeId == activityTypeId);
                // caut dobanda definita in perioada
                if (dobandaItem == null)
                {
                    dobandaItem = dobanziReferinta.FirstOrDefault(f => f.PlasamentType == TipPlasament && f.State == Models.Conta.Enums.State.Active
                                                                 && f.DataStart <= dataConstituire && dataConstituire <= f.DataEnd && f.CurrencyId == currencyId && f.ActivityTypeId == activityTypeId);

                    if (dobandaItem == null)
                    {
                        dobandaItem = dobanziReferinta.FirstOrDefault(f => f.PlasamentType == null && f.State == Models.Conta.Enums.State.Active
                                                                      && f.DataStart <= dataConstituire && dataConstituire <= f.DataEnd && f.CurrencyId == currencyId && f.ActivityTypeId == activityTypeId);
                        if (dobandaItem == null)
                        {
                            throw new Exception("Nu am identificat dobanda estimata pentru venitul din data " + LazyMethods.DateToString(dataConstituire));
                        }
                    }
                }
            }
            rez = dobandaItem.Procent;

            return rez;
        }

        public void CalculResurse(int bugetPrevBVCId, int bugetPrevCFId)
        {
            Context.BVC_PrevResurse.RemoveRange(Context.BVC_PrevResurse.Where(f => f.BugetPrevId == bugetPrevCFId));

            var bvcCashFlow = Context.BVC_BugetPrev.Include(f => f.Formular).FirstOrDefault(f => f.Id == bugetPrevCFId);
            var startDate = new DateTime(bvcCashFlow.Formular.AnBVC, bvcCashFlow.MonthStart, 1);
            var endDate = LazyMethods.LastDayOfMonth(new DateTime(bvcCashFlow.Formular.AnBVC, bvcCashFlow.MonthEnd, 1));
            var endPerPrec = startDate.AddDays(-1);


            var activityTypes = Context.ActivityTypes.Where(f => f.Status == Models.Conta.Enums.State.Active).ToList();
            var plasamenteList = Context.BVC_VenitTitlu.Where(f => f.FormularId == bvcCashFlow.FormularId && f.Selectat).ToList();

            var plasamenteIncasDob = Context.BVC_VenitTitluCF.Include(f => f.BVC_VenitTitlu)
                                                             .Where(f => f.BVC_VenitTitlu.FormularId == bvcCashFlow.FormularId && f.BVC_VenitTitlu.Selectat && f.DobandaTotala != 0)
                                                             .ToList();

            var cursValutarList = Context.ExchangeRateForecasts.Where(f => f.Year == bvcCashFlow.Formular.AnBVC && f.State == Models.Conta.Enums.State.Active).ToList();

            foreach (var activity in activityTypes)
            {
                // Resurse la inceputul anului
                decimal valoare = 0;
                var resResurse = new BVC_PrevResurse
                {
                    ActivityTypeId = activity.Id,
                    BugetPrevId = bugetPrevCFId,
                    OrderView = 1,
                    Suma = 0,
                    TenantId = bvcCashFlow.TenantId,
                    Descriere = "Resurse la " + (startDate.AddDays(-1).ToString("dd.MM.yyyy"))
                };
                Context.BVC_PrevResurse.Add(resResurse);
                Context.SaveChanges();

                var currencyList = plasamenteList.Where(f => f.ActivityTypeId == activity.Id).Select(f => f.CurrencyId).Distinct().ToList();
                foreach (var currencyId in currencyList)
                {
                    decimal cursValutar = 1;
                    if (currencyId != 1)
                    {
                        var cursValutarItem = Context.ExchangeRates.Where(f => f.CurrencyId == currencyId && f.ExchangeDate <= endPerPrec).OrderByDescending(f => f.ExchangeDate).FirstOrDefault();

                        if (cursValutarItem == null)
                        {
                            var currency = Context.Currency.FirstOrDefault(f => f.Id == currencyId);
                            throw new Exception("Nu ati completat cursul valutar estimat pentru " + currency.CurrencyCode + " pentru data " + endPerPrec.ToString("dd.MM.yyyy"));
                        }
                        cursValutar = cursValutarItem.Value;
                    }
                    Context.BVC_PrevResurseDetalii.AddRange(plasamenteList.Where(f => f.ActivityTypeId == activity.Id && f.CurrencyId == currencyId)
                                                                          .Select(f => new BVC_PrevResurseDetalii
                                                                          {
                                                                              BVC_PrevResurseId = resResurse.Id,
                                                                              CurrencyId = currencyId,
                                                                              CursValutar = cursValutar,
                                                                              DataStart = f.StartDate,
                                                                              IdPlasament = f.IdPlasament,
                                                                              DataEnd = f.MaturityDate,
                                                                              NrZilePlasament = (decimal)(LazyMethods.MinDate(f.MaturityDate, LazyMethods.LastDayOfMonth(endDate)) - LazyMethods.MaxDate(f.StartDate, startDate)).TotalDays,
                                                                              ProcentDobanda = f.ProcentDobanda,
                                                                              SumaInvestita = f.ValoarePlasament,
                                                                              SumaInRon = f.ValoarePlasament * cursValutar,
                                                                              TenantId = bvcCashFlow.TenantId
                                                                          }));
                }
                Context.SaveChanges();

                valoare = Context.BVC_PrevResurseDetalii.Where(f => f.BVC_PrevResurseId == resResurse.Id).Sum(f => f.SumaInRon);
                resResurse.Suma = valoare;
                Context.SaveChanges();

                // cupoane + dobanzi
                valoare = 0;
                var resCupoane = new BVC_PrevResurse
                {
                    ActivityTypeId = activity.Id,
                    BugetPrevId = bugetPrevCFId,
                    OrderView = 2,
                    Suma = 0,
                    TenantId = bvcCashFlow.TenantId,
                    Descriere = "cupoane + dobanzi"
                };
                Context.BVC_PrevResurse.Add(resCupoane);
                Context.SaveChanges();
                var currencyIncasList = plasamenteIncasDob.Where(f => f.BVC_VenitTitlu.ActivityTypeId == activity.Id).Select(f => f.BVC_VenitTitlu.CurrencyId).Distinct().ToList();

                foreach (var currencyId in currencyIncasList)
                {
                    decimal cursValutar = 1;
                    if (currencyId != 1)
                    {
                        var cursValutarItem = cursValutarList.FirstOrDefault(f => f.CurrencyId == currencyId);
                        if (cursValutarItem == null)
                        {
                            var currency = Context.Currency.FirstOrDefault(f => f.Id == currencyId);
                            throw new Exception("Nu ati completat cursul valutar estimat pentru " + currency.CurrencyCode + " pentru anul " + bvcCashFlow.Formular.AnBVC.ToString());
                        }
                        cursValutar = cursValutarItem.ValoareEstimata;
                    }


                    Context.BVC_PrevResurseDetalii.AddRange(plasamenteIncasDob.Where(f => f.BVC_VenitTitlu.ActivityTypeId == activity.Id && f.BVC_VenitTitlu.CurrencyId == currencyId)
                                                                              .Select(f => new BVC_PrevResurseDetalii
                                                                              {
                                                                                  BVC_PrevResurseId = resCupoane.Id,
                                                                                  CurrencyId = currencyId,
                                                                                  CursValutar = cursValutar,
                                                                                  DataStart = f.BVC_VenitTitlu.StartDate,
                                                                                  IdPlasament = f.BVC_VenitTitlu.IdPlasament,
                                                                                  DataEnd = f.BVC_VenitTitlu.MaturityDate,
                                                                                  NrZilePlasament = 0,
                                                                                  ProcentDobanda = f.BVC_VenitTitlu.ProcentDobanda,
                                                                                  SumaInvestita = f.DobandaTotala,
                                                                                  SumaInRon = f.DobandaTotala * cursValutar,
                                                                                  TenantId = bvcCashFlow.TenantId
                                                                              }));
                }

                Context.SaveChanges();

                valoare = Context.BVC_PrevResurseDetalii.Where(f => f.BVC_PrevResurseId == resCupoane.Id).Sum(f => f.SumaInRon);
                resCupoane.Suma = valoare;
                Context.SaveChanges();

                // Diferenta curs valutar
                valoare = 0;
                var resDifCurs = new BVC_PrevResurse
                {
                    ActivityTypeId = activity.Id,
                    BugetPrevId = bugetPrevCFId,
                    OrderView = 3,
                    Suma = 0,
                    TenantId = bvcCashFlow.TenantId,
                    Descriere = "diferenta curs valutar"
                };
                Context.BVC_PrevResurse.Add(resDifCurs);
                Context.SaveChanges();

                var titluriValuta = Context.BVC_PrevResurseDetalii.Where(f => f.BVC_PrevResurseId == resResurse.Id && f.CurrencyId != 1).ToList();

                var currencyIncasTitluList = titluriValuta.Select(f => f.CurrencyId).Distinct().ToList();

                foreach (var currencyId in currencyIncasTitluList)
                {
                    decimal cursValutar = 1;
                    if (currencyId != 1)
                    {
                        var cursValutarItem = cursValutarList.FirstOrDefault(f => f.CurrencyId == currencyId);
                        if (cursValutarItem == null)
                        {
                            var currency = Context.Currency.FirstOrDefault(f => f.Id == currencyId);
                            throw new Exception("Nu ati completat cursul valutar estimat pentru " + currency.CurrencyCode + " pentru anul " + bvcCashFlow.Formular.AnBVC.ToString());
                        }
                        cursValutar = cursValutarItem.ValoareEstimata;
                    }

                    foreach (var item in titluriValuta.Where(f => f.CurrencyId == currencyId))
                    {
                        decimal valoareItem = cursValutar * item.SumaInvestita - item.SumaInRon;

                        var detaliiItem = new BVC_PrevResurseDetalii
                        {
                            BVC_PrevResurseId = resDifCurs.Id,
                            CurrencyId = currencyId,
                            CursValutar = cursValutar,
                            DataStart = item.DataStart,
                            IdPlasament = item.IdPlasament,
                            DataEnd = item.DataEnd,
                            NrZilePlasament = 0,
                            ProcentDobanda = item.ProcentDobanda,
                            SumaInvestita = valoareItem,
                            SumaInRon = valoareItem,
                            TenantId = bvcCashFlow.TenantId
                        };
                        Context.BVC_PrevResurseDetalii.Add(detaliiItem);
                    }
                }

                Context.SaveChanges();

                valoare = Context.BVC_PrevResurseDetalii.Where(f => f.BVC_PrevResurseId == resDifCurs.Id).Sum(f => f.SumaInRon);
                resDifCurs.Suma = valoare;
                Context.SaveChanges();

                // Contributii
                valoare = 0;
                var contributii = Context.BVC_BugetPrevContributie.Where(f => f.DataIncasare >= startDate && f.DataIncasare <= endDate && f.ActivityTypeId == activity.Id
                                                                         && f.TipIncasare == BVC_BugetPrevContributieTipIncasare.Contributii).ToList();
                foreach (var item in contributii)
                {
                    decimal cursValutar = 1;
                    if (item.CurrencyId != 1)
                    {
                        var cursValutarItem = cursValutarList.FirstOrDefault(f => f.CurrencyId == item.CurrencyId);
                        if (cursValutarItem == null)
                        {
                            var currency = Context.Currency.FirstOrDefault(f => f.Id == item.CurrencyId);
                            throw new Exception("Nu ati completat cursul valutar estimat pentru " + currency.CurrencyCode + " pentru anul " + bvcCashFlow.Formular.AnBVC.ToString());
                        }
                        cursValutar = cursValutarItem.ValoareEstimata;
                    }

                    valoare += item.Value * cursValutar;

                }

                var resContib = new BVC_PrevResurse
                {
                    ActivityTypeId = activity.Id,
                    BugetPrevId = bugetPrevCFId,
                    OrderView = 4,
                    Suma = valoare,
                    TenantId = bvcCashFlow.TenantId,
                    Descriere = "Contributii"
                };
                Context.BVC_PrevResurse.Add(resContib);
                Context.SaveChanges();

                // creante si comision lichidator
                valoare = 0;
                var creanteComisLichid = Context.BVC_BugetPrevContributie.Where(f => f.DataIncasare >= startDate && f.DataIncasare <= endDate && f.ActivityTypeId == activity.Id
                                                                         && (f.TipIncasare == BVC_BugetPrevContributieTipIncasare.Creante || f.TipIncasare == BVC_BugetPrevContributieTipIncasare.ComisionLichidator))
                                                                         .ToList();
                foreach (var item in creanteComisLichid)
                {
                    decimal cursValutar = 1;
                    if (item.CurrencyId != 1)
                    {
                        var cursValutarItem = cursValutarList.FirstOrDefault(f => f.CurrencyId == item.CurrencyId);
                        if (cursValutarItem == null)
                        {
                            var currency = Context.Currency.FirstOrDefault(f => f.Id == item.CurrencyId);
                            throw new Exception("Nu ati completat cursul valutar estimat pentru " + currency.CurrencyCode + " pentru anul " + bvcCashFlow.Formular.AnBVC.ToString());
                        }
                        cursValutar = cursValutarItem.ValoareEstimata;
                    }

                    valoare += item.Value * cursValutar;

                }

                var resCreanteComisLichid = new BVC_PrevResurse
                {
                    ActivityTypeId = activity.Id,
                    BugetPrevId = bugetPrevCFId,
                    OrderView = 5,
                    Suma = valoare,
                    TenantId = bvcCashFlow.TenantId,
                    Descriere = "Creante si comision lichidator"
                };
                Context.BVC_PrevResurse.Add(resCreanteComisLichid);
                Context.SaveChanges();

                // cheltuieli
                valoare = 0;
                valoare = Context.BVC_BugetPrevRandValue.Include(f => f.BugetPrevRand).ThenInclude(f => f.FormRand)
                                                        .Where(f => f.ActivityTypeId == activity.Id && f.BugetPrevRand.BugetPrevId == bugetPrevCFId
                                                               && !f.BugetPrevRand.FormRand.IsTotal
                                                               && (f.BugetPrevRand.FormRand.TipRand == BVC_RowType.Cheltuieli || f.BugetPrevRand.FormRand.TipRand == BVC_RowType.Investitii
                                                                  || f.BugetPrevRand.FormRand.TipRand == BVC_RowType.Salarizare))
                                                        .Sum(f => f.Value);

                var resCheltuieli = new BVC_PrevResurse
                {
                    ActivityTypeId = activity.Id,
                    BugetPrevId = bugetPrevCFId,
                    OrderView = 6,
                    Suma = valoare,
                    TenantId = bvcCashFlow.TenantId,
                    Descriere = "Cheltuieli"
                };
                Context.BVC_PrevResurse.Add(resCheltuieli);
                Context.SaveChanges();

                // Resurse la sfarsitul anului
                valoare = resResurse.Suma + resCupoane.Suma + resDifCurs.Suma + resContib.Suma + resCreanteComisLichid.Suma - resCheltuieli.Suma;
                var resResurseEnd = new BVC_PrevResurse
                {
                    ActivityTypeId = activity.Id,
                    BugetPrevId = bugetPrevCFId,
                    OrderView = 7,
                    Suma = valoare,
                    TenantId = bvcCashFlow.TenantId,
                    Descriere = "Resurse la " + (endDate.ToString("dd.MM.yyyy"))
                };
                Context.BVC_PrevResurse.Add(resResurseEnd);
                Context.SaveChanges();
            }

        }
    }

    public class BVC_BugetPrevRandValueDto
    {
        public int Id { get; set; }
        public int BugetPrevRandId { get; set; }
        public DateTime DataOper { get; set; }
        public DateTime DataLuna { get; set; }
        public string Description { get; set; }
        public BVC_PrevValueType ValueType { get; set; }
        public decimal Value { get; set; }
        public int ActivityTypeId { get; set; }
    }
}
