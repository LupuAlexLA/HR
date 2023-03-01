using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Niva.EntityFramework.Repositories.Nomenclatures;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Managers.Reporting;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Conta.SituatiiFinanciare;
using Niva.Erp.Repositories.Conta;
using Niva.Erp.Repositories.Conta.Lichiditate;
using Niva.Erp.Repositories.Conta.SituatiiFinanciare;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Niva.Erp.EntityFrameworkCore.Repositories.Conta.SituatiiFinanciare
{
    public class SitFinanCalcRepository : ErpRepositoryBase<SitFinanCalc, int>, ISitFinanCalcRepository
    {
        string[] dateFormula = { "$PCI$", "$PCF$", "$PCA$", "$PPI$", "$PPF$", "$PPA$" };
        private readonly char separatorExpresie = '$';
        private readonly char separator = '#';
        string[] coloaneBalanta = { "OSID", "OSIC", "ORLD", "ORLC", "OSFD", "OSFC", "ORD", "ORC" };
        BalanceRepository _balanceRepository;
        ExchangeRatesRepository _exchangeRatesRepository;
        CurrencyRepository _currencyRepository;

        public SitFinanCalcRepository(IDbContextProvider<ErpDbContext> context) : base(context)
        {
            _balanceRepository = new BalanceRepository(context);
            _exchangeRatesRepository = new ExchangeRatesRepository(context);
            _currencyRepository = new CurrencyRepository(context);
        }

        public void CalcRapoarte(int balantaId, List<PlasamentLichiditateDto> plasamenteList, List<SitFinanExternalValues> externalValues)
        {
            try
            {
                DeleteCalcRapoarte(balantaId);
                var balanta = Context.SavedBalance.FirstOrDefault(f => f.Id == balantaId);
                var balanceDetailList = GetBalance(balantaId);

                var startDate = new DateTime(balanta.SaveDate.Year, 1, 1);
                var contaOperList = _balanceRepository.ContaOperationList(startDate, balanta.SaveDate, balanta.TenantId, 0, 1, true);

                var config = Context.SitFinan.Where(f => f.State == State.Active && f.RapDate <= balanta.SaveDate)
                                             .OrderByDescending(f => f.RapDate).FirstOrDefault();
                var rapoarteList = Context.SitFinanRap.Where(f => f.State == State.Active && f.SitFinanId == config.Id /*&& f.Id == 45*/).OrderBy(f => f.OrderView).ToList();

                foreach (var raport in rapoarteList)
                {
                    // prelucrez contaOperList in functie de conturile definite in flux
                    var accountFluxList = Context.SitFinanRapFluxConfig.Where(f => f.TenantId == balanta.TenantId && f.SitFinanRapId == raport.Id).ToList();
                    var operationDetailList = new List<ContaOperationDetail>();
                    ComputeOperAccountByFlux(accountFluxList, contaOperList, out operationDetailList);

                    CalcRaportNoTotal(balantaId, balanceDetailList, operationDetailList, contaOperList, raport.Id, plasamenteList, externalValues);
                    CalculRaportTotal(balantaId, balanceDetailList, operationDetailList, contaOperList, raport.Id, plasamenteList, externalValues);
                    CalcRaportNota(balantaId, balanceDetailList, operationDetailList, contaOperList, raport.Id, plasamenteList);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void CalcRaport(int balanceId, int raportId, List<PlasamentLichiditateDto> plasamenteList, List<SitFinanExternalValues> externalValues)
        {
            try
            {
                DeleteCalcRaport(balanceId, raportId);

                var balanta = Context.SavedBalance.FirstOrDefault(f => f.Id == balanceId);
                var balanceDetailList = GetBalance(balanta.Id);
                var startDate = new DateTime(balanta.SaveDate.Year, 1, 1);
                var contaOperList = _balanceRepository.ContaOperationList(startDate, balanta.SaveDate, balanta.TenantId, 0, 1, true);

                // prelucrez contaOperList in functie de conturile definite in flux
                var accountFluxList = Context.SitFinanRapFluxConfig.Where(f => f.TenantId == balanta.TenantId && f.SitFinanRapId == raportId).ToList();
                var operationDetailList = new List<ContaOperationDetail>();
                ComputeOperAccountByFlux(accountFluxList, contaOperList, out operationDetailList);

                CalcRaportNoTotal(balanceId, balanceDetailList, operationDetailList, contaOperList, raportId, plasamenteList, externalValues);

                CalculRaportTotal(balanceId, balanceDetailList, operationDetailList, contaOperList, raportId, plasamenteList, externalValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CalcRaportNoTotal(int balantaId, List<BalanceSitFinanCalc> balanceDetailList, List<ContaOperationDetail> operationDetailList, List<ContaOperationDetail> contaOperList, int raportId, List<PlasamentLichiditateDto> plasamenteList, List<SitFinanExternalValues> externalValues)
        {
            var raport = Context.SitFinanRap.FirstOrDefault(f => f.Id == raportId);
            var rowList = Context.SitFinanRapConfig.Where(f => f.SitFinanRapId == raport.Id).OrderBy(f => f.OrderView).ToList();
            var calcList = Context.SitFinanCalc.Where(f => f.SavedBalanceId == balantaId).ToList();
            var rapoarteList = Context.SitFinanRap.Where(f => f.State == State.Active && f.SitFinanId == raport.SitFinanId).ToList();
            var calcRap = new List<SitFinanCalc>();

            // initializez tabela de calcul
            try
            {
                calcRap = rowList.Select(f => new SitFinanCalc
                {
                    SitFinanRapRowId = f.Id,
                    SavedBalanceId = balantaId,
                    SitFinanRapId = raport.Id,
                    Calculated = false,
                    Val1 = 0,
                    Val2 = 0,
                    Val3 = 0,
                    Val4 = 0,
                    Val5 = 0,
                    Val6 = 0,
                    Validated = false
                })
                .ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            // calculez valorile
            try
            {
                var calculatOK = false;
                int contor = 1;

                while (!calculatOK && contor < 50)
                {

                    // parcurg lista din configurare fara totaluri si incep sa calculez
                    foreach (var item in rowList.Where(f => f.TotalRow == false).OrderBy(f => f.OrderView))
                    {
                        bool ok = true;
                        try
                        {
                            calcRap = CalcRaportValue(item, calcList, calcRap, balanceDetailList, operationDetailList, plasamenteList, contaOperList, externalValues, rapoarteList, balantaId, 1, true, out ok);
                            calcRap = CalcRaportValue(item, calcList, calcRap, balanceDetailList, operationDetailList, plasamenteList, contaOperList, externalValues, rapoarteList, balantaId, 2, true, out ok);
                            calcRap = CalcRaportValue(item, calcList, calcRap, balanceDetailList, operationDetailList, plasamenteList, contaOperList, externalValues, rapoarteList, balantaId, 3, true, out ok);
                            calcRap = CalcRaportValue(item, calcList, calcRap, balanceDetailList, operationDetailList, plasamenteList, contaOperList, externalValues, rapoarteList, balantaId, 4, true, out ok);
                            calcRap = CalcRaportValue(item, calcList, calcRap, balanceDetailList, operationDetailList, plasamenteList, contaOperList, externalValues, rapoarteList, balantaId, 5, true, out ok);
                            calcRap = CalcRaportValue(item, calcList, calcRap, balanceDetailList, operationDetailList, plasamenteList, contaOperList, externalValues, rapoarteList, balantaId, 6, true, out ok);

                            var calcRow = calcRap.FirstOrDefault(f => f.SitFinanRapRowId == item.Id);
                            if (ok)
                            {
                                calcRow.Calculated = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Eroare calcul rand: " + item.SitFinanRap.ReportName + " cod rand: " + item.RowCode + " - " + item.RowName + " - " + LazyMethods.GetErrMessage(ex));
                        }
                    }

                    var count = calcRap.Where(f => f.Calculated == false && (rowList.Where(g => g.TotalRow == false).Select(g => g.Id).ToList().Contains(f.SitFinanRapRowId)))
                                        .ToList().Count;
                    if (count == 0)
                    {
                        calculatOK = true;
                    }

                    contor++;
                }

                if (!calculatOK)
                {
                    throw new Exception("Verificati corectitudinea formulelor. Exista elemente care au fost specificate in formula si nu au fost calculate");
                }

                //salvez datele calculate in baza de date
                foreach (var item in calcRap)
                {
                    Context.SitFinanCalc.Add(item);
                }
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ComputeOperAccountByFlux(List<SitFinanRapFluxConfig> accountFluxList, List<ContaOperationDetail> contaOperList, out List<ContaOperationDetail> operList)
        {
            operList = new List<ContaOperationDetail>();
            //conturi care se iau in considerare in calculul fluxului
            foreach (var cont in accountFluxList.Where(f => f.SitFinanFluxRowType == SitFinanFluxRowType.ContCash))
            {
                var listAdd = contaOperList.Where(f => (cont.Debit != null && cont.Debit != "" && f.DebitSymbol.IndexOf(cont.Debit) == 0)
                                                        || (cont.Credit != null && cont.Credit != "" && f.CreditSymbol.IndexOf(cont.Credit) == 0)).ToList();
                var operDetailIdList = operList.Select(f => f.OperationDetailId).ToList();

                listAdd = listAdd.Where(f => !operDetailIdList.Contains(f.OperationDetailId)).ToList();

                operList.AddRange(listAdd);
            }

            //conturi care nu se iau in considerare in calculul fluxului
            foreach (var cont in accountFluxList.Where(f => f.SitFinanFluxRowType == SitFinanFluxRowType.Exceptii))
            {
                var listRemove = operList.Where(f => (cont.Debit != null && cont.Debit != "" && f.DebitSymbol.IndexOf(cont.Debit) == 0) &&
                                                     (cont.Credit != null && cont.Credit != "" && f.CreditSymbol.IndexOf(cont.Credit) == 0)).ToList();
                operList.RemoveAll(f => listRemove.Select(g => g.OperationDetailId).ToList().Contains(f.OperationDetailId));
            }
        }

        public void CalculRaportTotal(int balantaId, List<BalanceSitFinanCalc> balanceDetailList, List<ContaOperationDetail> operationDetailList, List<ContaOperationDetail> contaOperList, 
                                      int raportId, List<PlasamentLichiditateDto> plasamenteList, List<SitFinanExternalValues> externalValues)
        {

            var raport = Context.SitFinanRap.FirstOrDefault(f => f.Id == raportId);
            var itemList = Context.SitFinanRapConfig.Where(f => f.SitFinanRapId == raportId).OrderBy(f => f.OrderView).ToList();
            var calcList = Context.SitFinanCalc.Include(f => f.SitFinanRap).Where(f => f.SavedBalanceId == balantaId && f.SitFinanRapId != raportId).ToList();
            var calcRap = Context.SitFinanCalc.Include(f => f.SitFinanRap).Where(f => f.SavedBalanceId == balantaId && f.SitFinanRapId == raportId).ToList();

            var rapoarteList = Context.SitFinanRap.Where(f => f.State == State.Active && f.SitFinanId == raport.SitFinanId).OrderBy(f => f.OrderView).ToList();

            try
            {
                var calculatOK = false;
                int contor = 1;

                // pun campurile necalculate pe N
                foreach (var item in calcRap.Where(f => f.SitFinanRapRow.TotalRow == true).OrderBy(f => f.SitFinanRapRow.OrderView))
                {
                    item.Calculated = false;
                    item.Val1 = 0;
                    item.Val2 = 0;
                    item.Val3 = 0;
                    item.Val4 = 0;
                    item.Val5 = 0;
                    item.Val6 = 0;

                    Context.SitFinanCalcFormulaDet.RemoveRange(Context.SitFinanCalcFormulaDet.Where(f => f.SavedBalanceId == balantaId && f.SitFinanRapRowId == item.SitFinanRapRowId));
                    Context.SitFinanCalcDetails.RemoveRange(Context.SitFinanCalcDetails.Where(f => f.SavedBalanceId == balantaId && f.SitFinanRapRowId == item.SitFinanRapRowId));
                }

                while (!calculatOK && contor < 50)
                {

                    // parcurg lista din configurare fara totaluri si incep sa calculez
                    foreach (var item in calcRap.Where(f => f.SitFinanRapRow.TotalRow == true && f.Calculated == false).OrderBy(f => f.SitFinanRapRow.OrderView))
                    {
                        try
                        {
                            bool ok1 = true, ok2 = true, ok3 = true, ok4 = true, ok5 = true, ok6 = true;
                            calcRap = CalcRaportValue(item.SitFinanRapRow, calcList, calcRap, balanceDetailList, operationDetailList, plasamenteList, contaOperList, externalValues, rapoarteList, balantaId, 1, false, out ok1);
                            calcRap = CalcRaportValue(item.SitFinanRapRow, calcList, calcRap, balanceDetailList, operationDetailList, plasamenteList, contaOperList, externalValues, rapoarteList, balantaId, 2, false, out ok2);
                            calcRap = CalcRaportValue(item.SitFinanRapRow, calcList, calcRap, balanceDetailList, operationDetailList, plasamenteList, contaOperList, externalValues, rapoarteList, balantaId, 3, false, out ok3);
                            calcRap = CalcRaportValue(item.SitFinanRapRow, calcList, calcRap, balanceDetailList, operationDetailList, plasamenteList, contaOperList, externalValues, rapoarteList, balantaId, 4, false, out ok4);
                            calcRap = CalcRaportValue(item.SitFinanRapRow, calcList, calcRap, balanceDetailList, operationDetailList, plasamenteList, contaOperList, externalValues, rapoarteList, balantaId, 5, false, out ok5);
                            calcRap = CalcRaportValue(item.SitFinanRapRow, calcList, calcRap, balanceDetailList, operationDetailList, plasamenteList, contaOperList, externalValues, rapoarteList, balantaId, 6, false, out ok6);

                            var calcRow = calcRap.FirstOrDefault(f => f.Id == item.Id);
                            if (ok1 && ok2 && ok3 && ok4 && ok5 && ok6)
                            {
                                calcRow.Calculated = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Eroare calcul rand: " + item.SitFinanRap.ReportName + " cod rand: " + item.SitFinanRapRow.RowCode + " - " + item.SitFinanRapRow.RowName + " - " + LazyMethods.GetErrMessage(ex));
                        }
                    }

                    var count = calcRap.Where(f => f.Calculated == false && (itemList.Where(g => g.TotalRow == true).Select(g => g.Id).ToList().Contains(f.SitFinanRapRowId)))
                                        .ToList().Count;
                    if (count == 0)
                    {
                        calculatOK = true;
                    }

                    contor++;
                }

                if (!calculatOK)
                {
                    throw new Exception("Verificati corectitudinea formulelor. Exista elemente care au fost specificate in formula si nu au fost calculate");
                }

                //salvez datele calculate in baza de date
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Eroare calcul totaluri raport: " + raport.ReportName + " " + LazyMethods.GetErrMessage(ex));
            }
        }


        public List<SitFinanCalc> CalcRaportValue(SitFinanRapConfig itemConfig, List<SitFinanCalc> calcList, List<SitFinanCalc> calcRap, List<BalanceSitFinanCalc> balanceDetailList, 
                                                  List<ContaOperationDetail> operationDetailList, List<PlasamentLichiditateDto> plasamenteList, List<ContaOperationDetail> contaOperList, 
                                                  List<SitFinanExternalValues> externalValues, List<SitFinanRap> rapoarteList, int balantaId, int coloana, bool insertDb, out bool ok)
        {
            decimal rez = 0;

            string formula = "";
            switch (coloana)
            {
                case 1:
                    formula = itemConfig.Col1;
                    break;
                case 2:
                    formula = itemConfig.Col2;
                    break;
                case 3:
                    formula = itemConfig.Col3;
                    break;
                case 4:
                    formula = itemConfig.Col4;
                    break;
                case 5:
                    formula = itemConfig.Col5;
                    break;
                case 6:
                    formula = itemConfig.Col6;
                    break;
            }

            foreach (var item in calcRap)
            {
                var itemCalc = calcList.FirstOrDefault(f => f.SitFinanRapRowId == item.SitFinanRapRowId);
                if (itemCalc != null)
                {
                    calcList.Remove(itemCalc);
                }
                calcList.Add(item);
            }

            rez = CalcValCol(formula, balantaId, calcList, balanceDetailList, operationDetailList, contaOperList, plasamenteList, externalValues, rapoarteList, coloana, itemConfig.Id, insertDb, out ok);
            if (itemConfig.NegativeValue == false && rez < 0)
            {
                rez = 0;
            }
            rez = Math.Round(rez, itemConfig.DecimalNr);
            var calcItem = calcRap.FirstOrDefault(f => f.SitFinanRapRowId == itemConfig.Id);
            switch (coloana)
            {
                case 1:
                    calcItem.Val1 = rez;
                    break;
                case 2:
                    calcItem.Val2 = rez;
                    break;
                case 3:
                    calcItem.Val3 = rez;
                    break;
                case 4:
                    calcItem.Val4 = rez;
                    break;
                case 5:
                    calcItem.Val5 = rez;
                    break;
                case 6:
                    calcItem.Val6 = rez;
                    break;
            }

            return calcRap;
        }

        public decimal CalcValCol(string formula, int balantaId, List<SitFinanCalc> calcList, List<BalanceSitFinanCalc> balanceDetailList, List<ContaOperationDetail> operationDetailList,
                       List<ContaOperationDetail> contaOperList, List<PlasamentLichiditateDto> plasamenteList, List<SitFinanExternalValues> externalValues, List<SitFinanRap> rapoarteList, 
                       int coloanaRap, int codRandRap, bool insertDb, out bool ok)
        {
            decimal rez = 0;
            ok = true;

            var raportRand = Context.SitFinanRapConfig.FirstOrDefault(f => f.Id == codRandRap);
            var coloana = Context.SitFinanRapConfigCol.FirstOrDefault(f => f.SitFinanRapId == raportRand.SitFinanRapId && f.ColumnNr == coloanaRap);

            decimal? modCalc = null;
            if (coloana != null)
            {
                modCalc = coloana.ColumnModCalc;
            }

            if (modCalc != null)
            {
                var raport = Context.SitFinanRap.FirstOrDefault(f => f.Id == raportRand.SitFinanRapId);
                DateTime dataPrec, balanceDate;

                balanceDate = balanceDetailList.FirstOrDefault().BalanceDate;

                if (raport.PerioadaEchivalenta == true)
                {
                    dataPrec = balanceDate.AddMonths(-12);
                    dataPrec = dataPrec.AddMonths(1);
                    dataPrec = new DateTime(dataPrec.Year, dataPrec.Month, 1).AddDays(-1);
                }
                else
                {
                    dataPrec = new DateTime(balanceDate.Year - 1, 12, 31);
                }
                var sitFinanPrec = Context.SitFinan.Where(f => f.State == State.Active && f.RapDate <= dataPrec).OrderByDescending(f => f.RapDate).FirstOrDefault();
                if (sitFinanPrec != null)
                {
                    var raportRandPrec = Context.SitFinanRapConfig.Include(f => f.SitFinanRap)
                                                .FirstOrDefault(f => f.SitFinanRap.SitFinanId == sitFinanPrec.Id && f.RowCode == raportRand.RowCode && f.SitFinanRap.ReportSymbol == raport.ReportSymbol);
                    if (raportRandPrec != null)
                    {
                        var balancePrec = Context.SitFinanCalc.Include(f => f.SavedBalance)
                                                              .Where(f => f.SavedBalance.SaveDate == dataPrec)
                                                              .OrderByDescending(f => f.SavedBalance.SaveDate).OrderByDescending(f => f.SavedBalance.CreationTime)
                                                              .FirstOrDefault();
                        if (balancePrec != null)
                        {
                            int balancePrecId = balancePrec.SavedBalanceId;

                            var randCalcPrec = Context.SitFinanCalc.Where(f => f.SavedBalanceId == balancePrecId && f.SitFinanRapRowId == raportRandPrec.Id).FirstOrDefault();
                            if (randCalcPrec != null)
                            {
                                decimal _modCalc = modCalc ?? 0;

                                if (_modCalc == 1)
                                {
                                    rez = randCalcPrec.Val1 ?? 0;
                                }
                                else if (_modCalc == 2)
                                {
                                    rez = randCalcPrec.Val2 ?? 0;
                                }
                                else if (_modCalc == 3)
                                {
                                    rez = randCalcPrec.Val3 ?? 0;
                                }
                                else if (_modCalc == 4)
                                {
                                    rez = randCalcPrec.Val4 ?? 0;
                                }
                                else if (_modCalc == 5)
                                {
                                    rez = randCalcPrec.Val5 ?? 0;
                                }
                                else if (_modCalc == 6)
                                {
                                    rez = randCalcPrec.Val6 ?? 0;
                                }

                                Context.SitFinanCalcFormulaDet.Add(new SitFinanCalcFormulaDet
                                {
                                    SavedBalanceId = balantaId,
                                    SitFinanRapRowId = codRandRap,
                                    ColumnId = coloanaRap,
                                    Formula = "Valoare preluata din raportarea precedenta",
                                    FormulaVal = rez.ToString()
                                });
                            }
                        }
                    }
                }
                else
                {
                    rez = CalcFormula(formula, balantaId, calcList, balanceDetailList, operationDetailList, contaOperList, plasamenteList, externalValues, (int?)raportRand.SitFinanRowModCalc, rapoarteList, coloanaRap, codRandRap, insertDb, out ok);
                }
            }
            else
            {
                rez = CalcFormula(formula, balantaId, calcList, balanceDetailList, operationDetailList, contaOperList, plasamenteList, externalValues, (int?)raportRand.SitFinanRowModCalc, rapoarteList, coloanaRap, codRandRap, insertDb, out ok);
            }
            return rez;
        }

        public decimal CalcFormula(string formula, int balantaId, List<SitFinanCalc> calcList, List<BalanceSitFinanCalc> balanceDetailList, List<ContaOperationDetail> operationDetailList,
                       List<ContaOperationDetail> contaOperList, List<PlasamentLichiditateDto> plasamenteList, List<SitFinanExternalValues> externalValues, int? modCalc, List<SitFinanRap> rapoarteList, 
                       int? coloanaRap, int? codRandRap, bool insertDb, out bool ok)
        { // ok => true = ok; false = exista elemente necalculate
            decimal rez = 0;
            ok = true;
            var dataRap = Context.SavedBalance.FirstOrDefault(f => f.Id == balantaId).SaveDate;
            if (formula == "" || formula == null)
            {
                return 0;
            }

            if (insertDb) // sterg detalierea formulelor
            {
                Context.SitFinanCalcDetails.RemoveRange(Context.SitFinanCalcDetails.Where(f => f.SavedBalanceId == balantaId && f.SitFinanRapRowId == codRandRap && f.ColumnId == coloanaRap));
                Context.SitFinanCalcFormulaDet.RemoveRange(Context.SitFinanCalcFormulaDet.Where(f => f.SavedBalanceId == balantaId && f.SitFinanRapRowId == codRandRap && f.ColumnId == coloanaRap));
            }

            if (formula == "$EfectCursValutar$")
            {
               
                var startDateRap = new DateTime(dataRap.Year, 1, 1);
                //var operEfectValuta = _balanceRepository.ContaOperationList(startDateRap, dataRap, 1, 1, 1, true);

                var operEfectValuta = operationDetailList.Where(f => f.CurrencyOrigId != 0 && f.CurrencyOrigId != 1).ToList();
                decimal platiValoareLei = 0, platiValoareEval = 0, incasariValoareLei = 0, incasariValoareEval = 0;

                Context.SitFinanCalcFormulaDet.Add(new SitFinanCalcFormulaDet
                {
                    SavedBalanceId = balantaId,
                    SitFinanRapRowId = codRandRap.Value,
                    ColumnId = coloanaRap.Value,
                    Formula = formula,
                    FormulaVal = ""
                });

                foreach (var currencyId in operEfectValuta.Select(f => f.CurrencyOrigId).Distinct().ToList())
                {
                    decimal valIncasEval = 0, valIncasLei = 0, valPlatEval = 0, valPlatLei = 0;
                    var moneda = _currencyRepository.FirstOrDefault(f => f.Id == currencyId);
                    var cursValutar = _exchangeRatesRepository.GetExchangeRate(dataRap, currencyId, 1);
                    valIncasEval = 0;
                    valIncasEval = operEfectValuta.Where(f => (f.DebitSymbol.IndexOf("101") == 0 || f.DebitSymbol.IndexOf("2211") == 0) && f.CurrencyOrigId == currencyId).Sum(f => f.ValoareValuta);
                    valIncasEval = Math.Round(valIncasEval * cursValutar, 2);
                    incasariValoareEval += valIncasEval;

                    valIncasLei = 0;
                    valIncasLei = operEfectValuta.Where(f => (f.DebitSymbol.IndexOf("101") == 0 || f.DebitSymbol.IndexOf("2211") == 0) && f.CurrencyOrigId == currencyId).Sum(f => f.Valoare);
                    incasariValoareLei += valIncasLei;

                    valPlatEval = 0;
                    valPlatEval = operEfectValuta.Where(f => (f.CreditSymbol.IndexOf("101") == 0 || f.CreditSymbol.IndexOf("2211") == 0) && f.CurrencyOrigId == currencyId).Sum(f => f.ValoareValuta);
                    valPlatEval = Math.Round(valPlatEval * cursValutar, 2);
                    platiValoareEval += valPlatEval;

                    valPlatLei = 0;
                    valPlatLei = operEfectValuta.Where(f => (f.CreditSymbol.IndexOf("101") == 0 || f.CreditSymbol.IndexOf("2211") == 0) && f.CurrencyOrigId == currencyId).Sum(f => f.Valoare);
                    platiValoareLei += valPlatLei;

                    if (insertDb) // inserez in baza de date
                    {
                        if (valIncasEval != 0)
                        {
                            Context.SitFinanCalcDetails.Add(new SitFinanCalcDetails
                            {
                                SavedBalanceId = balantaId,
                                Val = valIncasEval,
                                ElementDet = moneda.CurrencyCode + " - incasari in valuta evaluate la data de calcul",
                                SitFinanRapRowId = codRandRap.Value,
                                ColumnId = coloanaRap.Value
                            });
                        }
                        if (valIncasLei != 0)
                        {
                            Context.SitFinanCalcDetails.Add(new SitFinanCalcDetails
                            {
                                SavedBalanceId = balantaId,
                                Val = valIncasLei,
                                ElementDet = moneda.CurrencyCode + " - incasari in valuta evaluate la data de incasarii",
                                SitFinanRapRowId = codRandRap.Value,
                                ColumnId = coloanaRap.Value
                            });
                        }
                        if (valPlatEval != 0)
                        {
                            Context.SitFinanCalcDetails.Add(new SitFinanCalcDetails   
                            {
                                SavedBalanceId = balantaId,
                                Val = valPlatEval,
                                ElementDet = moneda.CurrencyCode + " - plati in valuta evaluate la data de calcul",
                                SitFinanRapRowId = codRandRap.Value,
                                ColumnId = coloanaRap.Value
                            });
                        }
                        if (valPlatLei != 0)
                        {
                            Context.SitFinanCalcDetails.Add(new SitFinanCalcDetails
                            {
                                SavedBalanceId = balantaId,
                                Val = valPlatLei,
                                ElementDet = moneda.CurrencyCode + " - plati in valuta evaluate la data de incasarii",
                                SitFinanRapRowId = codRandRap.Value,
                                ColumnId = coloanaRap.Value
                            });
                        }
                    }
                }

                incasariValoareEval = -1 * incasariValoareEval;
                incasariValoareLei = -1 * incasariValoareLei;

                var diffIncasari = incasariValoareEval - incasariValoareEval;
                var diffPlati = platiValoareEval - platiValoareLei;
                var valEfectCurs = diffIncasari + diffPlati;

                // schimb curs valutar
                var schimbValutar = Context.Exchange.Where(f => f.OperationDate >= startDateRap && f.OperationDate <= dataRap && f.State == State.Active).ToList();
                foreach(var item in schimbValutar)
                {
                    decimal valoareLeiOper = 0;
                    decimal valoareLeiRap = 0;
                    decimal valFlux = 0;

                    if (item.ExchangeOperType == Models.Economic.Casierii.ExchangeOperType.CumparLei)
                    {
                        valoareLeiOper = item.ExchangedValue;
                        valoareLeiRap = item.Value;
                        var cursValutar = _exchangeRatesRepository.GetExchangeRate(dataRap, item.CurrencyId, 1);
                        valoareLeiRap = Math.Round(valoareLeiRap * cursValutar, 2);
                        valFlux = valoareLeiOper - valoareLeiRap;
                    }
                    else
                    {
                        valoareLeiOper = item.Value;
                        valoareLeiRap = item.ExchangedValue;
                        var cursValutar = _exchangeRatesRepository.GetExchangeRate(dataRap, item.CurrencyId, 1);
                        valoareLeiRap = Math.Round(valoareLeiRap * cursValutar, 2);
                        valFlux = valoareLeiRap - valoareLeiOper;
                    }

                    Context.SitFinanCalcDetails.Add(new SitFinanCalcDetails
                    {
                        SavedBalanceId = balantaId,
                        Val = valFlux,
                        ElementDet = "Schimb valutar la data " + LazyMethods.DateToString(item.ExchangeDate) + "; valoare lei la data schimb: " + valoareLeiOper.ToString("N2") + "; valoare lei la data raport: " 
                                     + valoareLeiRap.ToString("N2") + "; diferenta curs: " + valFlux.ToString("N2"),
                        SitFinanRapRowId = codRandRap.Value,
                        ColumnId = coloanaRap.Value
                    });

                    valEfectCurs += valFlux;
                }

                return valEfectCurs;

            }
            else if (formula == "$NrSalPer$")
            {
                var nrSalPer = externalValues.FirstOrDefault(f => f.ValueType == SitFinanExternalValuesType.NrMediuSalariatiPerioada);
                if (nrSalPer == null)
                    return 0;
                return nrSalPer.Value;
            }
            else if (formula == "$NrSalData$")
            {
                var nrSalData = externalValues.FirstOrDefault(f => f.ValueType == SitFinanExternalValuesType.NrSalariatiData);
                if (nrSalData == null)
                    return 0;
                return nrSalData.Value;
            }

            string expresie = formula;

            while (expresie.IndexOf(separatorExpresie) >= 0)
            {
                int indexStart = expresie.IndexOf(separatorExpresie);
                int indexEnd = expresie.IndexOf(separatorExpresie, indexStart + 1);
                string item = expresie.Substring(indexStart + 1, indexEnd - indexStart - 1);
                string[] splitItem = item.Split(separator);

                string tipItem = splitItem[0];
                string contItem = splitItem[1];
                decimal? value = 0;

                // e din contabilitate
                if (coloaneBalanta.Contains(tipItem))
                {
                    if (modCalc == (int?)SitFinanRowModCalc.ValoriBalanta)
                    {
                        value = (decimal)balanceDetailList.Where(f => f.AccountSymbol.IndexOf(contItem) == 0)
                                                .Sum(f => (tipItem == "OSID") ? f.DbValueI :
                                                          (tipItem == "OSIC") ? f.CrValueI :
                                                          (tipItem == "ORLD") ? f.DbValueM :
                                                          (tipItem == "ORLC") ? f.CrValueM :
                                                          (tipItem == "OSFD") ? f.DbValueF :
                                                          (tipItem == "OSFC") ? f.CrValueF :
                                                          (tipItem == "ORD") ? f.DbValueY : f.CrValueY);
                    }
                    else
                    { // Rulaje

                        if (tipItem == "OSID" || tipItem == "ORLD" || tipItem == "OSFD" || tipItem == "ORD")
                        {
                            value = operationDetailList.Where(f => f.DebitSymbol.IndexOf(contItem) == 0 && f.CurrencyId == 1)
                                                       .Sum(f => f.Valoare);
                        }

                        if (tipItem == "OSIC" || tipItem == "ORLC" || tipItem == "OSFC" || tipItem == "ORC")
                        {
                            value = operationDetailList.Where(f => f.CreditSymbol.IndexOf(contItem) == 0 && f.CurrencyId == 1)
                                                     .Sum(f => f.Valoare);
                        }
                    }

                }
                else if (tipItem == "TITLU")
                {
                    var tipPlasament = splitItem[1];
                  //var termen = splitItem[2] == "L" ? "lung" : (splitItem[3] == "M" ? "mediu" : "scurt");
                    var termen = splitItem[2] == "L" ? "lung" : (splitItem[2] == "M" ? "mediu" : "scurt");
                    var tip = splitItem[3];

                    // nr zile min si max 
                    var nrZileMin = termen == "lung" ? 1825 : termen == "mediu" ? 365 : 0;
                    var nrZileMax = termen == "lung" ? 9999 : termen == "mediu" ? 1825 : 365;
                    
                    if (tip == "D") // durata creantei = maturitateCreanta - data calcul
                    {
                        if (tipPlasament == "D") // depozit
                        {
                            value = plasamenteList.Where(f => f.tipPlasament == tipPlasament.Substring(0, 1) && (f.maturityDate - dataRap).Days > nrZileMin && (f.maturityDate - dataRap).Days <= nrZileMax )
                                                  .Sum(f => f.valoareCreanta);
                        }

                        if (tipPlasament == "T") // titlu de plasament
                        {
                            value = plasamenteList.Where(f => f.tipPlasament == tipPlasament.Substring(0, 1) && (f.maturitateCreanta.Value - dataRap).Days > nrZileMin && (f.maturitateCreanta.Value - dataRap).Days <= nrZileMax)
                                                 .Sum(f => f.valoareCreanta);
                        }

                        if ((tipPlasament == "OBL") || (tipPlasament == "O")) // obligatiuni
                        {
                            value = plasamenteList.Where(f => f.tipPlasament == tipPlasament.Substring(0, 1) && (f.maturitateCreanta.Value - dataRap).Days > nrZileMin && (f.maturitateCreanta.Value - dataRap).Days <= nrZileMax)
                                                  .Sum(f => f.valoareCreanta);
                        }
                    }

                    else if (tip == "P") // durata titlului
                    {
                        //if (tipPlasament == "D") // depozit
                        //{
                        //    value = plasamenteList.Where(f => f.tipPlasament == tipPlasament && f.termen.Contains(termen)).Sum(f => f.valoareContabila);
                        //}

                        //if (tipPlasament == "T") // titlu de plasament
                        //{
                        //    value = plasamenteList.Where(f => f.tipPlasament == tipPlasament && f.termen.Contains(termen)).Sum(f => f.valoareContabila);
                        //}

                        //if ((tipPlasament == "OBL") || (tipPlasament == "O")) // obligatiuni
                        //{
                        //    value = plasamenteList.Where(f => f.tipPlasament == tipPlasament.Substring(0, 1) && f.termen.Contains(termen)).Sum(f => f.valoareContabila);
                        //}
                        if (tipPlasament == "D") // depozit
                        {
                            value = plasamenteList.Where(f => f.tipPlasament == tipPlasament.Substring(0, 1) && (f.maturityDate - dataRap).Days > nrZileMin && (f.maturityDate - dataRap).Days <= nrZileMax)
                                                  .Sum(f => f.valoareContabila);
                        }

                        if (tipPlasament == "T") // titlu de plasament
                        {
                            value = plasamenteList.Where(f => f.tipPlasament == tipPlasament.Substring(0, 1) && (f.maturityDate - dataRap).Days > nrZileMin && (f.maturityDate - dataRap).Days <= nrZileMax)
                                                 .Sum(f => f.valoareContabila);
                        }

                        if ((tipPlasament == "OBL") || (tipPlasament == "O")) // obligatiuni
                        {
                            value = plasamenteList.Where(f => f.tipPlasament == tipPlasament.Substring(0, 1) && (f.maturityDate - dataRap).Days > nrZileMin && (f.maturityDate - dataRap).Days <= nrZileMax)
                                                  .Sum(f => f.valoareContabila);
                        }
                    }

                    else if (tip == "A") // durata titlului
                    {
                        //if (tipPlasament == "D") // depozit
                        //{
                        //    value = plasamenteList.Where(f => f.tipPlasament == tipPlasament && f.termen.Contains(termen)).Sum(f => f.valoareDepreciere);
                        //}

                        //if (tipPlasament == "T") // titlu de plasament
                        //{
                        //    value = plasamenteList.Where(f => f.tipPlasament == tipPlasament && f.termen.Contains(termen)).Sum(f => f.valoareDepreciere);
                        //}

                        //if ((tipPlasament == "OBL") || (tipPlasament == "O")) // obligatiuni
                        //{
                        //    value = plasamenteList.Where(f => f.tipPlasament == tipPlasament.Substring(0, 1) && f.termen.Contains(termen)).Sum(f => f.valoareDepreciere);
                        //}
                        if (tipPlasament == "D") // depozit
                        {
                            value = plasamenteList.Where(f => f.tipPlasament == tipPlasament.Substring(0, 1) && (f.maturityDate - dataRap).Days > nrZileMin && (f.maturityDate - dataRap).Days <= nrZileMax)
                                                  .Sum(f => f.valoareDepreciere);
                        }

                        if (tipPlasament == "T") // titlu de plasament
                        {
                            value = plasamenteList.Where(f => f.tipPlasament == tipPlasament.Substring(0, 1) && (f.maturityDate - dataRap).Days > nrZileMin && (f.maturityDate - dataRap).Days <= nrZileMax)
                                                 .Sum(f => f.valoareDepreciere);
                        }

                        if ((tipPlasament == "OBL") || (tipPlasament == "O")) // obligatiuni
                        {
                            value = plasamenteList.Where(f => f.tipPlasament == tipPlasament.Substring(0, 1) && (f.maturityDate - dataRap).Days > nrZileMin && (f.maturityDate - dataRap).Days <= nrZileMax)
                                                  .Sum(f => f.valoareDepreciere);
                        }
                    }
                }
                else if (tipItem == "RUL") // rulaje cont debitor in relatie cu cont creditor
                {
                    string[] conturiRulaje = contItem.Split('*');
                    string[] contDeb = new string[2];
                    string[] contCred = new string[2];
                    contDeb[0] = conturiRulaje[0].Substring(0, 1);
                    contDeb[1] = conturiRulaje[0].Substring(1);
                    contCred[0] = conturiRulaje[1].Substring(0, 1);
                    contCred[1] = conturiRulaje[1].Substring(1);

                    value = contaOperList.Where(f => f.DebitSymbol.IndexOf(contDeb[1]) == 0 && f.CreditSymbol.IndexOf(contCred[1]) == 0).Sum(f => f.Valoare);
                }
                else // e din rapoarte
                {
                    var codRaport = tipItem.Substring(0, 1);
                    var coloana = tipItem.Substring(1);
                    decimal codRand = decimal.Parse(contItem);

                    var raport = rapoarteList.FirstOrDefault(f => f.ReportSymbol == codRaport);
                    if (raport == null)
                    {
                        throw new Exception("Raportul cu codul " + codRaport + " nu exista. Revizuiti formula " + formula);
                    }
                    var rand = Context.SitFinanRapConfig.FirstOrDefault(f => f.RowCode == codRand && f.SitFinanRapId == raport.Id);
                    if (rand == null)
                    {
                        throw new Exception("Randul cu codul " + codRand + " nu exista. Revizuiti formula " + formula);
                    }
                    var calcRow = calcList.FirstOrDefault(f => f.SitFinanRapRowId == rand.Id);
                    if (calcRow.Calculated == false)
                    {
                        ok = false;
                        return 0;
                    }
                    value = (coloana == "1") ? calcRow.Val1 :
                            (coloana == "2") ? calcRow.Val2 :
                            (coloana == "3") ? calcRow.Val3 :
                            (coloana == "4") ? calcRow.Val4 :
                            (coloana == "5") ? calcRow.Val5 : calcRow.Val6;
                }
                if (insertDb) // inserez in baza de date
                {
                    Context.SitFinanCalcDetails.Add(new SitFinanCalcDetails
                    {
                        SavedBalanceId = balantaId,
                        Val = value ?? 0,
                        ElementDet = item,
                        SitFinanRapRowId = codRandRap.Value,
                        ColumnId = coloanaRap.Value
                    });
                }

                IFormatProvider formatProvider = new System.Globalization.CultureInfo("en-US", true);
                string valueF = value.Value.ToString(formatProvider);
                if (valueF.IndexOf(".") < 0) valueF = valueF + ".0";
                expresie = expresie.Replace("$" + item + "$", valueF);
            }

            if (insertDb) // inserez in baza de date
            {
                Context.SitFinanCalcFormulaDet.Add(new SitFinanCalcFormulaDet
                {
                    SavedBalanceId = balantaId,
                    SitFinanRapRowId = codRandRap.Value,
                    ColumnId = coloanaRap.Value,
                    Formula = formula,
                    FormulaVal = expresie
                });
            }

            try
            {
                rez = Convert.ToDecimal(new DataTable().Compute(expresie, null));
            }
            catch (Exception ex)
            {
                rez = 0;
            }

            return rez;
        }


        private List<BalanceSitFinanCalc> GetBalance(int balanceId)
        {
            var tenantId = Context.SavedBalance.FirstOrDefault(f => f.Id == balanceId).TenantId;
            var localCurrencyId = Context.Tenants.FirstOrDefault(f => f.Id == tenantId).LocalCurrencyId;

            var balance = Context.SavedBalanceDetails.Include(f => f.SavedBalance).Include(f => f.Account)
                                              .Where(f => f.SavedBalanceId == balanceId)
                                              .ToList()
                                              .Select(f => new BalanceSitFinanCalc
                                              {
                                                  BalanceDate = f.SavedBalance.SaveDate,
                                                  BalanceName = f.SavedBalance.BalanceName,
                                                  AccountId = f.AccountId,
                                                  AccountName = f.Account.AccountName,
                                                  AccountSymbol = f.Account.Symbol,
                                                  CrValueF = f.CrValueF,
                                                  CrValueI = f.CrValueI,
                                                  CrValueM = f.CrValueM,
                                                  CrValueY = f.CrValueY,
                                                  DbValueF = f.DbValueF,
                                                  DbValueI = f.DbValueI,
                                                  DbValueM = f.DbValueM,
                                                  DbValueY = f.DbValueY
                                              })
                                              .OrderBy(f => f.AccountSymbol)
                                              .ToList();
            return balance;
        }

        public void DeleteCalcRapoarte(int balanceId)
        {
            try
            {
                Context.SitFinanCalcNote.RemoveRange(Context.SitFinanCalcNote.Where(f => f.SavedBalanceId == balanceId));
                Context.SitFinanCalcFormulaDet.RemoveRange(Context.SitFinanCalcFormulaDet.Where(f => f.SavedBalanceId == balanceId));
                Context.SitFinanCalcDetails.RemoveRange(Context.SitFinanCalcDetails.Where(f => f.SavedBalanceId == balanceId));
                Context.SitFinanCalc.RemoveRange(Context.SitFinanCalc.Where(f => f.SavedBalanceId == balanceId));
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteCalcRaport(int balanceId, int raportId)
        {
            try
            {
                var rowList = Context.SitFinanRapConfig.Where(f => f.SitFinanRapId == raportId).Select(f => f.Id).ToList();

                Context.SitFinanCalcFormulaDet.RemoveRange(Context.SitFinanCalcFormulaDet.Where(f => f.SavedBalanceId == balanceId && rowList.Contains(f.SitFinanRapRowId)));
                Context.SitFinanCalcDetails.RemoveRange(Context.SitFinanCalcDetails.Where(f => f.SavedBalanceId == balanceId && rowList.Contains(f.SitFinanRapRowId)));
                Context.SitFinanCalc.RemoveRange(Context.SitFinanCalc.Where(f => f.SavedBalanceId == balanceId && f.SitFinanRapId == raportId));
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetDateFromFormula(string formula, DateTime rapDate)
        {
            string rez = "";
            DateTime dateForm = new DateTime();
            if (formula == "$PCI$")
            {
                dateForm = new DateTime(rapDate.Year, 1, 1);
            }
            else if (formula == "$PCA$")
            {
                dateForm = new DateTime(rapDate.Year, 12, 31);
            }
            else if (formula == "$PCF$")
            {
                dateForm = rapDate;
            }
            else if (formula == "$PPI$")
            {
                dateForm = new DateTime(rapDate.Year - 1, 1, 1);
            }
            else if (formula == "$PPA$")
            {
                dateForm = new DateTime(rapDate.Year - 1, 12, 31);
            }
            else if (formula == "$PPF$")
            {
                dateForm = rapDate.AddMonths(-12);
            }

            rez = LazyMethods.DateToString(dateForm);

            return rez;
        }


        public string CalcNotaFormula(string formula, int balanceId, List<SitFinanCalc> calcList, List<BalanceSitFinanCalc> balanta,
                                     List<ContaOperationDetail> operationDetailList, List<ContaOperationDetail> contaOperList, List<SitFinanRap> rapoarteList, List<PlasamentLichiditateDto> plasamenteList)
        {
            string expresie = formula;
            int contor = 1;

            while (expresie.IndexOf(separatorExpresie) >= 0 && contor < 50)
            {
                int indexStart = expresie.IndexOf(separatorExpresie);
                int indexEnd = expresie.IndexOf(separatorExpresie, indexStart + 1);
                string item = expresie.Substring(indexStart, indexEnd - indexStart + 1);

                if (dateFormula.Contains(item)) // e camp de data
                {
                    var dataBalance = Context.SavedBalance.FirstOrDefault(f => f.Id == balanceId).SaveDate;
                    string dataText = GetDateFromFormula(item, dataBalance);
                    expresie = expresie.Replace(item, dataText);
                }
                else // e formula
                {
                    bool ok;
                    decimal rezFormNr = CalcFormula(item, balanceId, calcList, balanta, operationDetailList, contaOperList, plasamenteList, null, null, rapoarteList, null, null, false, out ok);
                    string rezForm = rezFormNr.ToString("N2");
                    expresie = expresie.Replace(item, rezForm);
                }
                contor++;
            }
            return expresie;
        }


        public void CalcRaportNota(int balantaId, List<BalanceSitFinanCalc> balanceDetailList, List<ContaOperationDetail> operationDetailList, List<ContaOperationDetail> contaOperList, int raportId, List<PlasamentLichiditateDto> plasamenteList)
        {
            try
            {
                var notaBd = Context.SitFinanCalcNote.FirstOrDefault(f => f.SitFinanRapId == raportId && f.SavedBalanceId == balantaId);
                if (notaBd != null)
                {
                    Context.SitFinanCalcNote.Remove(notaBd);
                }

                var raport = Context.SitFinanRap.FirstOrDefault(f => f.Id == raportId);
                var rowList = Context.SitFinanRapConfig.Where(f => f.SitFinanRapId == raportId).OrderBy(f => f.OrderView).ToList();
                var calcList = Context.SitFinanCalc.Where(f => f.SavedBalanceId == balantaId).ToList();
                var balance = GetBalance(balantaId);
                var rapoarteList = Context.SitFinanRap.Where(f => f.State == State.Active && f.SitFinanId == raport.SitFinanId).ToList();

                var nota = Context.SitFinanRapConfigNote.FirstOrDefault(f => f.SitFinanRapId == raportId);
                if (nota != null)
                {
                    var notaBefore = CalcNotaFormula(nota.BeforeNote, balantaId, calcList, balance, operationDetailList, contaOperList, rapoarteList, plasamenteList);
                    var notaAfter = CalcNotaFormula(nota.AfterNote, balantaId, calcList, balance, operationDetailList, contaOperList, rapoarteList, plasamenteList);
                    var notaItem = new SitFinanCalcNote
                    {
                        SavedBalanceId = balantaId,
                        SitFinanRapId = raportId,
                        BeforeNote = notaBefore,
                        AfterNote = notaAfter
                    };
                    Context.SitFinanCalcNote.Add(notaItem);
                    Context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string GetCalcColumnText(string formula, int balanceId, int raportId)
        {
            string ret = "";
            var calcList = Context.SitFinanCalc.Include(f => f.SitFinanRap).Where(f => f.SavedBalanceId == balanceId).ToList();
            var balanta = GetBalance(balanceId);
            var raport = Context.SitFinanRap.FirstOrDefault(f => f.Id == raportId);
            var rapoarteList = Context.SitFinanRap.Where(f => f.State == State.Active && f.SitFinanId == raport.SitFinanId).OrderBy(f => f.OrderView).ToList();

            ret = CalcNotaFormulaOld(formula, balanceId, calcList, balanta, rapoarteList);

            return ret;
        }


        public void CalcRaportNoTotalOld(int balanceId, int raportId)
        {
            var raport = Context.SitFinanRap.FirstOrDefault(f => f.Id == raportId);
            var rowList = Context.SitFinanRapConfig.Where(f => f.SitFinanRapId == raportId).OrderBy(f => f.OrderView).ToList();
            var calcList = Context.SitFinanCalc.Where(f => f.SavedBalanceId == balanceId).ToList();
            var balance = GetBalance(balanceId);
            var rapoarteList = Context.SitFinanRap.Where(f => f.State == State.Active && f.SitFinanId == raport.SitFinanId).ToList();

            var calcRap = new List<SitFinanCalc>();

            // initializez tabela de calcul
            try
            {
                calcRap = rowList.Select(f => new SitFinanCalc
                {
                    SitFinanRapRowId = f.Id,
                    SavedBalanceId = balanceId,
                    SitFinanRapId = raportId,
                    Calculated = false,
                    Val1 = 0,
                    Val2 = 0,
                    Val3 = 0,
                    Val4 = 0,
                    Val5 = 0,
                    Val6 = 0,
                    Validated = false
                })
                .ToList();


            }
            catch (Exception ex)
            {
                throw ex;
            }


            // calculez valorile
            try
            {
                var calculatOK = false;
                int contor = 1;

                while (!calculatOK && contor < 50)
                {

                    // parcurg lista din configurare fara totaluri si incep sa calculez
                    foreach (var item in rowList.Where(f => f.TotalRow == false).OrderBy(f => f.OrderView))
                    {
                        bool ok = true;
                        try
                        {
                            calcRap = CalcRaportValueOld(item, calcList, calcRap, balance, rapoarteList, balanceId, 1, true, out ok);
                            calcRap = CalcRaportValueOld(item, calcList, calcRap, balance, rapoarteList, balanceId, 2, true, out ok);
                            calcRap = CalcRaportValueOld(item, calcList, calcRap, balance, rapoarteList, balanceId, 3, true, out ok);
                            calcRap = CalcRaportValueOld(item, calcList, calcRap, balance, rapoarteList, balanceId, 4, true, out ok);
                            calcRap = CalcRaportValueOld(item, calcList, calcRap, balance, rapoarteList, balanceId, 5, true, out ok);
                            calcRap = CalcRaportValueOld(item, calcList, calcRap, balance, rapoarteList, balanceId, 6, true, out ok);

                            var calcRow = calcRap.FirstOrDefault(f => f.SitFinanRapRowId == item.Id);
                            if (ok)
                            {
                                calcRow.Calculated = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Eroare calcul rand: " + item.RowName + " - " + LazyMethods.GetErrMessage(ex));
                        }
                    }

                    var count = calcRap.Where(f => f.Calculated == false && (rowList.Where(g => g.TotalRow == false).Select(g => g.Id).ToList().Contains(f.SitFinanRapRowId)))
                                        .ToList().Count;
                    if (count == 0)
                    {
                        calculatOK = true;
                    }

                    contor++;
                }

                if (!calculatOK)
                {
                    throw new Exception("Verificati corectitudinea formulelor. Exista elemente care au fost specificate in formula si nu au fost calculate");
                }

                //salvez datele calculate in baza de date
                foreach (var item in calcRap)
                {
                    Context.SitFinanCalc.Add(item);
                }
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CalculRaportTotalOld(int balanceId, int raportId)
        {
            var raport = Context.SitFinanRap.FirstOrDefault(f => f.Id == raportId);
            var itemList = Context.SitFinanRapConfig.Where(f => f.SitFinanRapId == raportId).OrderBy(f => f.OrderView).ToList();
            var calcList = Context.SitFinanCalc.Include(f => f.SitFinanRap).Where(f => f.SavedBalanceId == balanceId && f.SitFinanRapId != raportId).ToList();
            var calcRap = Context.SitFinanCalc.Include(f => f.SitFinanRap).Where(f => f.SavedBalanceId == balanceId && f.SitFinanRapId == raportId).ToList();
            var balanta = GetBalance(balanceId);
            var rapoarteList = Context.SitFinanRap.Where(f => f.State == State.Active && f.SitFinanId == raport.SitFinanId).OrderBy(f => f.OrderView).ToList();

            try
            {
                var calculatOK = false;
                int contor = 1;

                // pun campurile necalculate pe N
                foreach (var item in calcRap.Where(f => f.SitFinanRapRow.TotalRow == true).OrderBy(f => f.SitFinanRapRow.OrderView))
                {
                    item.Calculated = false;
                    item.Val1 = 0;
                    item.Val2 = 0;
                    item.Val3 = 0;
                    item.Val4 = 0;
                    item.Val5 = 0;
                    item.Val6 = 0;

                    Context.SitFinanCalcFormulaDet.RemoveRange(Context.SitFinanCalcFormulaDet.Where(f => f.SavedBalanceId == balanceId && f.SitFinanRapRowId == item.SitFinanRapRowId));
                    Context.SitFinanCalcDetails.RemoveRange(Context.SitFinanCalcDetails.Where(f => f.SavedBalanceId == balanceId && f.SitFinanRapRowId == item.SitFinanRapRowId));
                }

                while (!calculatOK && contor < 50)
                {

                    // parcurg lista din configurare fara totaluri si incep sa calculez
                    foreach (var item in calcRap.Where(f => f.SitFinanRapRow.TotalRow == true && f.Calculated == false).OrderBy(f => f.SitFinanRapRow.OrderView))
                    {
                        try
                        {
                            bool ok1 = true, ok2 = true, ok3 = true, ok4 = true, ok5 = true, ok6 = true;
                            calcRap = CalcRaportValueOld(item.SitFinanRapRow, calcList, calcRap, balanta, rapoarteList, balanceId, 1, false, out ok1);
                            calcRap = CalcRaportValueOld(item.SitFinanRapRow, calcList, calcRap, balanta, rapoarteList, balanceId, 2, false, out ok2);
                            calcRap = CalcRaportValueOld(item.SitFinanRapRow, calcList, calcRap, balanta, rapoarteList, balanceId, 3, false, out ok3);
                            calcRap = CalcRaportValueOld(item.SitFinanRapRow, calcList, calcRap, balanta, rapoarteList, balanceId, 4, false, out ok4);
                            calcRap = CalcRaportValueOld(item.SitFinanRapRow, calcList, calcRap, balanta, rapoarteList, balanceId, 5, false, out ok5);
                            calcRap = CalcRaportValueOld(item.SitFinanRapRow, calcList, calcRap, balanta, rapoarteList, balanceId, 6, false, out ok6);

                            var calcRow = calcRap.FirstOrDefault(f => f.Id == item.Id);
                            if (ok1 && ok2 && ok3 && ok4 && ok5 && ok6)
                            {
                                calcRow.Calculated = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Eroare calcul rand: " + item.SitFinanRapRow.RowName + " - " + LazyMethods.GetErrMessage(ex));
                        }
                    }

                    var count = calcRap.Where(f => f.Calculated == false && (itemList.Where(g => g.TotalRow == true).Select(g => g.Id).ToList().Contains(f.SitFinanRapRowId)))
                                        .ToList().Count;
                    if (count == 0)
                    {
                        calculatOK = true;
                    }

                    contor++;
                }

                if (!calculatOK)
                {
                    throw new Exception("Verificati corectitudinea formulelor. Exista elemente care au fost specificate in formula si nu au fost calculate");
                }

                //salvez datele calculate in baza de date
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Eroare calcul totaluri raport: " + raport.ReportName + " " + LazyMethods.GetErrMessage(ex));
            }
        }

        public List<SitFinanCalc> CalcRaportValueOld(SitFinanRapConfig itemConfig, List<SitFinanCalc> calcList, List<SitFinanCalc> calcRap, List<BalanceSitFinanCalc> balanta,
                                                  List<SitFinanRap> rapoarteList, int balanceId, int coloana, bool insertDb, out bool ok)
        {
            decimal rez = 0;

            string formula = "";
            switch (coloana)
            {
                case 1:
                    formula = itemConfig.Col1;
                    break;
                case 2:
                    formula = itemConfig.Col2;
                    break;
                case 3:
                    formula = itemConfig.Col3;
                    break;
                case 4:
                    formula = itemConfig.Col4;
                    break;
                case 5:
                    formula = itemConfig.Col5;
                    break;
                case 6:
                    formula = itemConfig.Col6;
                    break;
            }

            foreach (var item in calcRap)
            {
                var itemCalc = calcList.FirstOrDefault(f => f.SitFinanRapRowId == item.SitFinanRapRowId);
                if (itemCalc != null)
                {
                    calcList.Remove(itemCalc);
                }
                calcList.Add(item);
            }

            rez = CalcValColOld(formula, balanceId, calcList, balanta, rapoarteList, coloana, itemConfig.Id, insertDb, out ok);
            if (itemConfig.NegativeValue == false && rez < 0)
            {
                rez = 0;
            }
            rez = Math.Round(rez, itemConfig.DecimalNr);
            var calcItem = calcRap.FirstOrDefault(f => f.SitFinanRapRowId == itemConfig.Id);
            switch (coloana)
            {
                case 1:
                    calcItem.Val1 = rez;
                    break;
                case 2:
                    calcItem.Val2 = rez;
                    break;
                case 3:
                    calcItem.Val3 = rez;
                    break;
                case 4:
                    calcItem.Val4 = rez;
                    break;
                case 5:
                    calcItem.Val5 = rez;
                    break;
                case 6:
                    calcItem.Val6 = rez;
                    break;
            }

            return calcRap;
        }

        public decimal CalcValColOld(string formula, int balanceId, List<SitFinanCalc> calcList, List<BalanceSitFinanCalc> balanta,
                               List<SitFinanRap> rapoarteList, int coloanaRap, int codRandRap, bool insertDb, out bool ok)
        {
            decimal rez = 0;
            ok = true;

            var raportRand = Context.SitFinanRapConfig.FirstOrDefault(f => f.Id == codRandRap);
            var coloana = Context.SitFinanRapConfigCol.FirstOrDefault(f => f.SitFinanRapId == raportRand.SitFinanRapId && f.ColumnNr == coloanaRap);

            decimal? modCalc = null;
            if (coloana != null)
            {
                modCalc = coloana.ColumnModCalc;
            }

            if (modCalc != null)
            {
                var raport = Context.SitFinanRap.FirstOrDefault(f => f.Id == raportRand.SitFinanRapId);
                DateTime dataPrec, balanceDate;

                balanceDate = balanta.FirstOrDefault().BalanceDate;

                if (raport.PerioadaEchivalenta == true)
                {
                    dataPrec = balanceDate.AddMonths(-12);
                    dataPrec = dataPrec.AddMonths(1);
                    dataPrec = new DateTime(dataPrec.Year, dataPrec.Month, 1).AddDays(-1);
                }
                else
                {
                    dataPrec = new DateTime(balanceDate.Year - 1, 12, 31);
                }
                var sitFinanPrec = Context.SitFinan.Where(f => f.State == State.Active && f.RapDate == dataPrec).OrderByDescending(f => f.RapDate).FirstOrDefault();
                if (sitFinanPrec != null)
                {
                    var raportRandPrec = Context.SitFinanRapConfig.Include(f => f.SitFinanRap)
                                                .FirstOrDefault(f => f.SitFinanRap.SitFinanId == sitFinanPrec.Id && f.RowCode == raportRand.RowCode);
                    if (raportRandPrec != null)
                    {
                        var balancePrec = Context.SitFinanCalc.Include(f => f.SavedBalance)
                                                              .Where(f => f.SavedBalance.SaveDate == dataPrec)
                                                              .OrderByDescending(f => f.SavedBalance.SaveDate).OrderByDescending(f => f.SavedBalance.CreationTime)
                                                              .FirstOrDefault();
                        if (balancePrec != null)
                        {
                            int balancePrecId = balancePrec.SavedBalanceId;

                            var randCalcPrec = Context.SitFinanCalc.Where(f => f.SavedBalanceId == balancePrecId && f.SitFinanRapRowId == raportRandPrec.Id).FirstOrDefault();
                            if (randCalcPrec != null)
                            {
                                decimal _modCalc = modCalc ?? 0;

                                if (_modCalc == 1)
                                {
                                    rez = randCalcPrec.Val1 ?? 0;
                                }
                                else if (_modCalc == 2)
                                {
                                    rez = randCalcPrec.Val2 ?? 0;
                                }
                                else if (_modCalc == 3)
                                {
                                    rez = randCalcPrec.Val3 ?? 0;
                                }
                                else if (_modCalc == 4)
                                {
                                    rez = randCalcPrec.Val4 ?? 0;
                                }
                                else if (_modCalc == 5)
                                {
                                    rez = randCalcPrec.Val5 ?? 0;
                                }
                                else if (_modCalc == 6)
                                {
                                    rez = randCalcPrec.Val6 ?? 0;
                                }

                                Context.SitFinanCalcFormulaDet.Add(new SitFinanCalcFormulaDet
                                {
                                    SavedBalanceId = balanceId,
                                    SitFinanRapRowId = codRandRap,
                                    ColumnId = coloanaRap,
                                    Formula = "Valoare preluata din raportarea precedenta",
                                    FormulaVal = rez.ToString()
                                });
                            }
                        }
                    }
                }
                else
                {
                    rez = CalcFormulaOld(formula, balanceId, calcList, balanta, rapoarteList, coloanaRap, codRandRap, insertDb, out ok);
                }
            }
            else
            {
                rez = CalcFormulaOld(formula, balanceId, calcList, balanta, rapoarteList, coloanaRap, codRandRap, insertDb, out ok);
            }
            return rez;
        }


        public decimal CalcFormulaOld(string formula, int balanceId, List<SitFinanCalc> calcList, List<BalanceSitFinanCalc> balanta,
                               List<SitFinanRap> rapoarteList, int? coloanaRap, int? codRandRap, bool insertDb, out bool ok)
        { // ok => true = ok; false = exista elemente necalculate
            decimal rez = 0;
            ok = true;

            if (formula == "" || formula == null)
            {
                return 0;
            }

            if (insertDb) // sterg detalierea formulelor
            {
                Context.SitFinanCalcDetails.RemoveRange(Context.SitFinanCalcDetails.Where(f => f.SavedBalanceId == balanceId && f.SitFinanRapRowId == codRandRap && f.ColumnId == coloanaRap));
                Context.SitFinanCalcFormulaDet.RemoveRange(Context.SitFinanCalcFormulaDet.Where(f => f.SavedBalanceId == balanceId && f.SitFinanRapRowId == codRandRap && f.ColumnId == coloanaRap));
            }

            string expresie = formula;

            while (expresie.IndexOf(separatorExpresie) >= 0)
            {
                int indexStart = expresie.IndexOf(separatorExpresie);
                int indexEnd = expresie.IndexOf(separatorExpresie, indexStart + 1);
                string item = expresie.Substring(indexStart + 1, indexEnd - indexStart - 1);
                string[] splitItem = item.Split(separator);

                string tipItem = splitItem[0];
                string contItem = splitItem[1];
                decimal? value = 0;

                // e din contabilitate
                if (coloaneBalanta.Contains(tipItem))
                {
                    value = (decimal)balanta.Where(f => f.AccountSymbol.IndexOf(contItem) == 0)
                                            .Sum(f => (tipItem == "OSID") ? f.DbValueI :
                                                      (tipItem == "OSIC") ? f.CrValueI :
                                                      (tipItem == "ORLD") ? f.DbValueM :
                                                      (tipItem == "ORLC") ? f.CrValueM :
                                                      (tipItem == "OSFD") ? f.DbValueF :
                                                      (tipItem == "OSFC") ? f.CrValueF :
                                                      (tipItem == "ORD") ? f.DbValueY : f.CrValueY);
                }
                else // e din rapoarte
                {
                    var codRaport = tipItem.Substring(0, 1);
                    var coloana = tipItem.Substring(1);
                    decimal codRand = decimal.Parse(contItem);

                    var raport = rapoarteList.FirstOrDefault(f => f.ReportSymbol == codRaport);
                    if (raport == null)
                    {
                        throw new Exception("Raportul cu codul " + codRaport + " nu exista. Revizuiti formula " + formula);
                    }
                    var rand = Context.SitFinanRapConfig.FirstOrDefault(f => f.RowCode == codRand && f.SitFinanRapId == raport.Id);
                    if (rand == null)
                    {
                        throw new Exception("Randul cu codul " + codRand + " nu exista. Revizuiti formula " + formula);
                    }
                    var calcRow = calcList.FirstOrDefault(f => f.SitFinanRapRowId == rand.Id);
                    if (calcRow.Calculated == false)
                    {
                        ok = false;
                        return 0;
                    }
                    value = (coloana == "1") ? calcRow.Val1 :
                            (coloana == "2") ? calcRow.Val2 :
                            (coloana == "3") ? calcRow.Val3 :
                            (coloana == "4") ? calcRow.Val4 :
                            (coloana == "5") ? calcRow.Val5 : calcRow.Val6;
                }
                if (insertDb) // inserez in baza de date
                {
                    Context.SitFinanCalcDetails.Add(new SitFinanCalcDetails
                    {
                        SavedBalanceId = balanceId,
                        Val = value ?? 0,
                        ElementDet = item,
                        SitFinanRapRowId = codRandRap.Value,
                        ColumnId = coloanaRap.Value
                    });
                }

                IFormatProvider formatProvider = new System.Globalization.CultureInfo("en-US", true);
                string valueF = value.Value.ToString(formatProvider);
                if (valueF.IndexOf(".") < 0) valueF = valueF + ".0";
                expresie = expresie.Replace("$" + item + "$", valueF);
            }

            if (insertDb) // inserez in baza de date
            {
                Context.SitFinanCalcFormulaDet.Add(new SitFinanCalcFormulaDet
                {
                    SavedBalanceId = balanceId,
                    SitFinanRapRowId = codRandRap.Value,
                    ColumnId = coloanaRap.Value,
                    Formula = formula,
                    FormulaVal = expresie
                });
            }

            try
            {
                rez = Convert.ToDecimal(new DataTable().Compute(expresie, null));
            }
            catch (Exception ex)
            {
                rez = 0;
            }

            return rez;
        }


        public string CalcNotaFormulaOld(string formula, int balanceId, List<SitFinanCalc> calcList, List<BalanceSitFinanCalc> balanta,
                                  List<SitFinanRap> rapoarteList)
        {
            string expresie = formula;
            int contor = 1;

            while (expresie.IndexOf(separatorExpresie) >= 0 && contor < 50)
            {
                int indexStart = expresie.IndexOf(separatorExpresie);
                int indexEnd = expresie.IndexOf(separatorExpresie, indexStart + 1);
                string item = expresie.Substring(indexStart, indexEnd - indexStart + 1);

                if (dateFormula.Contains(item)) // e camp de data
                {
                    var dataBalance = Context.SavedBalance.FirstOrDefault(f => f.Id == balanceId).SaveDate;
                    string dataText = GetDateFromFormula(item, dataBalance);
                    expresie = expresie.Replace(item, dataText);
                }
                else // e formula
                {
                    bool ok;
                    decimal rezFormNr = CalcFormulaOld(item, balanceId, calcList, balanta, rapoarteList, null, null, false, out ok);
                    string rezForm = rezFormNr.ToString("N2");
                    expresie = expresie.Replace(item, rezForm);
                }
                contor++;
            }
            return expresie;
        }

        public void CalcRaportNotaOld(int balantaId, int raportId)
        {
            try
            {
                var notaBd = Context.SitFinanCalcNote.FirstOrDefault(f => f.SitFinanRapId == raportId && f.SavedBalanceId == balantaId);
                if (notaBd != null)
                {
                    Context.SitFinanCalcNote.Remove(notaBd);
                }

                var raport = Context.SitFinanRap.FirstOrDefault(f => f.Id == raportId);
                var rowList = Context.SitFinanRapConfig.Where(f => f.SitFinanRapId == raportId).OrderBy(f => f.OrderView).ToList();
                var calcList = Context.SitFinanCalc.Where(f => f.SavedBalanceId == balantaId).ToList();
                var balance = GetBalance(balantaId);
                var rapoarteList = Context.SitFinanRap.Where(f => f.State == State.Active && f.SitFinanId == raport.SitFinanId).ToList();

                var nota = Context.SitFinanRapConfigNote.FirstOrDefault(f => f.SitFinanRapId == raportId);
                if (nota != null)
                {
                    var notaBefore = CalcNotaFormulaOld(nota.BeforeNote, balantaId, calcList, balance, rapoarteList);
                    var notaAfter = CalcNotaFormulaOld(nota.AfterNote, balantaId, calcList, balance, rapoarteList);
                    var notaItem = new SitFinanCalcNote
                    {
                        SavedBalanceId = balantaId,
                        SitFinanRapId = raportId,
                        BeforeNote = notaBefore,
                        AfterNote = notaAfter
                    };
                    Context.SitFinanCalcNote.Add(notaItem);
                    Context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SitFinanCalcDetailTemp> GetSitFinanRapRows(int columnId, int balanceId, int reportId)
        {
            var query = from det in Context.SitFinanCalcDetails
                        join conf in Context.SitFinanRapConfig on det.SitFinanRapRowId equals conf.Id
                        where det.SavedBalanceId == balanceId && conf.SitFinanRapId == reportId && det.ColumnId == columnId
                        select new SitFinanCalcDetailTemp
                        {
                            Id = conf.Id,
                            RowName = conf.RowName,
                            Col1 = conf.Col1,
                            Col2 = conf.Col2,
                            Col3 = conf.Col3,
                            Col4 = conf.Col4,
                            Col5 = conf.Col5,
                            Col6 = conf.Col6,
                            ElementDet = det.ElementDet,
                            Val = det.Val,
                            OrderView = conf.OrderView
                        };
            return query.OrderBy(f => f.OrderView).ToList();
        }

        public void ComputeReportDetails(DateTime startDate, DateTime endDate, bool isDailyBalance, int raportId, SitFinanRapConfigCol column, out SitFinanRaport ret)
        {
            try
            {
                ret = new SitFinanRaport();
                ret.Details = new List<SitFinanReportDetailList>();
                var savedBalanceIdsList = Context.SavedBalance.Where(f => f.SaveDate >= startDate && f.SaveDate <= endDate && f.IsDaily == isDailyBalance).GroupBy(f => f.SaveDate).Select(f => f.Max(x => x.Id)).ToList();
                var sitFinanCalcBalanceIdsList = Context.SitFinanCalc.Include(f => f.SavedBalance).Where(f => savedBalanceIdsList.Contains(f.SavedBalanceId)).OrderBy(f => f.SavedBalanceId).Select(f => f.SavedBalanceId).Distinct().ToList();
                foreach (var balanceId in sitFinanCalcBalanceIdsList)
                {
                    var balance = Context.SitFinanCalc.Include(f => f.SavedBalance).FirstOrDefault(f => f.SavedBalanceId == balanceId);


                    var reportDetail = new List<SitFinanReportDetailList>();

                    reportDetail = Context.SitFinanCalc.Include(f => f.SitFinanRapRow).ThenInclude(f => f.SitFinanRap)
                                                          .Where(f => f.SavedBalanceId == balanceId && f.SitFinanRapId == raportId)
                                                          .Select(f => new SitFinanReportDetailList
                                                          {
                                                              CalcRowId = f.Id,
                                                              RowId = f.SitFinanRapRowId,
                                                              RowName = f.SitFinanRapRow.RowName,
                                                              RowNr = f.SitFinanRapRow.RowNr,
                                                              RowNota = f.SitFinanRapRow.RowNota,
                                                              OrderView = f.SitFinanRapRow.OrderView,
                                                              TotalRow = f.SitFinanRapRow.TotalRow,
                                                              Bold = f.SitFinanRapRow.Bold,
                                                              NegativeValue = f.SitFinanRapRow.NegativeValue,
                                                              DecimalNr = f.SitFinanRapRow.DecimalNr,
                                                              Val = (column.ColumnNr == 1 ? f.Val1 : (column.ColumnNr == 2 ? f.Val2 : (column.ColumnNr == 3 ? f.Val3 : (column.ColumnNr == 4 ? f.Val4 : (column.ColumnNr == 5 ? f.Val5 : f.Val6))))),
                                                              BalanceDate = balance.SavedBalance.SaveDate
                                                          })
                                                          .ToList()
                                                          .OrderBy(f => f.OrderView)
                                                          .ToList();

                    ret.Details.AddRange(reportDetail);


                    // note
                    var nota = Context.SitFinanCalcNote.FirstOrDefault(f => f.SavedBalanceId == balanceId && f.SitFinanRapId == raportId);
                    if (nota != null)
                    {
                        ret.NotaBefore = nota.BeforeNote;
                        ret.NotaAfter = nota.AfterNote;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public class BalanceSitFinanCalc
    {
        public DateTime BalanceDate { get; set; }

        public string BalanceName { get; set; }

        public int AccountId { get; set; }

        public virtual string AccountName { get; set; }

        public virtual string AccountSymbol { get; set; }

        public virtual string Synthetic { get; set; }

        public virtual string Analythic { get; set; }

        public virtual decimal CrValueF { get; set; }

        public virtual decimal CrValueI { get; set; }

        public virtual decimal CrValueM { get; set; }

        public virtual decimal CrValueY { get; set; }

        public virtual decimal DbValueF { get; set; }

        public virtual decimal DbValueI { get; set; }

        public virtual decimal DbValueM { get; set; }

        public virtual decimal DbValueY { get; set; }

    }
}