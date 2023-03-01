using Abp.EntityFrameworkCore;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Niva.Erp.Models.SectoareBnr;
using Niva.Erp.Repositories.SectoareBNR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Niva.Erp.EntityFrameworkCore.Repositories.SectoareBNR
{
    public class BNR_RaportareRepository : ErpRepositoryBase<BNR_Raportare, int>, IBNR_RaportareRepository
    {
        private readonly char separatorExpresie = '$';
        private readonly char separatorExpresieSpecial = '&';
        private readonly char separator = '#';
        public BNR_RaportareRepository(IDbContextProvider<ErpDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public decimal CalculFormulaAnexa3(string formula, int savedBalanceId)
        {
            try
            {
                decimal rez = 0;
                if (formula == "" || formula == null)
                {
                    return 0;
                }

                string expresie = formula;
                while (expresie.IndexOf(separatorExpresie) >= 0)
                {
                    int indexStartFormula = expresie.IndexOf(separatorExpresieSpecial);
                    int indexEndFormula = expresie.IndexOf(separatorExpresieSpecial, indexStartFormula + 1);

                    if (indexStartFormula > 0 && indexEndFormula > 0)
                    {
                        string splitFormula = expresie.Substring(indexStartFormula + 1, indexEndFormula - indexStartFormula - 1);
                        string formulaSpeciala = splitFormula;
                        while (splitFormula.IndexOf(separatorExpresie) >= 0)
                        {
                            splitFormula = CalculFormula(splitFormula, savedBalanceId);
                        }

                        var rezultat = Convert.ToDecimal(new DataTable().Compute(splitFormula, null));
                        rezultat = rezultat > 0 ? rezultat : 0;
                        expresie = expresie.Replace("&" + formulaSpeciala + "&", rezultat.ToString());
                    }
                    else
                    {
                        expresie = CalculFormula(expresie, savedBalanceId);
                    }
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private string CalculFormula(string formula, int savedBalanceId)
        {
            var balance = Context.SavedBalanceDetails.Include(f => f.Account).Where(f => f.SavedBalanceId == savedBalanceId).ToList();
            int startIndex = formula.IndexOf(separatorExpresie);
            int endIndex = formula.IndexOf(separatorExpresie, startIndex + 1);
            string item = formula.Substring(startIndex + 1, endIndex - startIndex - 1);
            string[] splitItem = item.Split(separator);

            string tipItem = splitItem[0];
            string contItem = splitItem[1];
            decimal? value = 0;
            value = balance.Where(f => f.Account.Symbol.IndexOf(contItem) == 0)
                           .Sum(f => (tipItem == "ORC") ? f.CrValueY : f.DbValueY);

            IFormatProvider formatProvider = new System.Globalization.CultureInfo("en-US", true);
            string valueF = value.Value.ToString(formatProvider);
            if (valueF.IndexOf(".") < 0) valueF = valueF + ".0";
            formula = formula.Replace("$" + item + "$", valueF);
            return formula;
        }

        public void CalculTotaluri(List<BNR_AnexaDetail> anexaDetails)
        {
            try
            {
                foreach (var row in anexaDetails)
                {
                    string formula = row.FormulaTotal;
                    var semneList = new List<string>();
                    var splitItem = new List<string>();
                    DesfacFormula(formula, out semneList, out splitItem);

                    decimal sum = 0;

                    for (int i = 0; i < semneList.Count; i++)
                    {
                        var codRand = splitItem[i];
                        var raportareList = Context.BNR_RaportareRand.Include(f => f.BNR_AnexaDetail).ThenInclude(f => f.BNR_Anexa).Include(f => f.BNR_Sector)
                                                                 .Where(f => f.Valoare != 0 && f.SectorId != null && f.BNR_AnexaDetail.CodRand == codRand)
                                                                 .GroupBy(f => new { f.AnexaDetailId, f.SectorId, f.BNR_RaportareId })
                                                                 .Select(f => new BNR_RaportareRand
                                                                 {
                                                                     AnexaDetailId = row.Id,
                                                                     BNR_RaportareId = f.Key.BNR_RaportareId,
                                                                     Valoare = f.Sum(f => f.Valoare),
                                                                     SectorId = f.Key.SectorId
                                                                 })
                                                                 .Distinct()
                                                                 .ToList();
                        Context.BNR_RaportareRand.AddRange(raportareList);
                        Context.SaveChanges();
                    }
                }
            }
            catch (System.Exception ex)
            {

                throw new System.Exception(ex.ToString());
            }
        }

        public void CalculTotaluriRaportare(int savedBalanceId)
        {
            try
            {
                var raportariCalc = Context.BNR_Raportare.Where(f => f.SavedBalanceId == savedBalanceId).ToList();

                foreach (var raportareRow in raportariCalc.ToList())
                {
                    var raportareId = raportareRow.Id;
                    var calculRows = Context.BNR_RaportareRand.Include(f => f.BNR_Raportare).Include(f => f.BNR_AnexaDetail)
                                                          .Where(f => f.BNR_Raportare.SavedBalanceId == savedBalanceId && f.BNR_RaportareId == raportareId)
                                                          .Select(f => new CalcTotalBnrDto
                                                          {
                                                              RaportareId = f.BNR_RaportareId,
                                                              AnexaDetailId = f.AnexaDetailId,
                                                              SectorId = f.SectorId,
                                                              CodRand = f.BNR_AnexaDetail.CodRand,
                                                              Valoare = f.Valoare,
                                                              Calculat = true,
                                                              RandTotal = false
                                                          })
                                                          .ToList();

                    var raportare = raportariCalc.FirstOrDefault(f => f.Id == raportareId);
                    var randuriTotal = Context.BNR_AnexaDetails.Where(f => f.AnexaId == raportare.AnexaId && f.State == Models.Conta.Enums.State.Active && f.FormulaTotal != null)
                                                               .OrderBy(f => f.OrderView)
                                                               .ToList();
                    foreach (var item in randuriTotal)
                    {
                        var sectoareRows = calculRows.Where(f => f.RaportareId == raportareId).Select(f => f.SectorId).Distinct().ToList();
                        foreach (var sector in sectoareRows)
                        {
                            var randTotalCalc = new CalcTotalBnrDto
                            {
                                RaportareId = raportareId,
                                AnexaDetailId = item.Id,
                                CodRand = item.CodRand,
                                Formula = item.FormulaTotal,
                                Calculat = false,
                                SectorId = sector,
                                RandTotal = true
                            };
                            calculRows.Add(randTotalCalc);
                        }
                    }

                    bool calculatOk = false;
                    int contor = 0;

                    while (!calculatOk && contor <= 50)
                    {
                        foreach (var rand in calculRows.Where(f => !f.Calculat))
                        {
                            string formula = rand.Formula;
                            var semneList = new List<string>();
                            var splitItem = new List<string>();
                            DesfacFormula(formula, out semneList, out splitItem);

                            decimal sum = 0;
                            var count = calculRows.Count(f => splitItem.Contains(f.CodRand) && !f.Calculat && f.SectorId == rand.SectorId);
                            if (count == 0) // sunt toate componentele calculate
                            {
                                for (int i = 0; i < semneList.Count; i++)
                                {
                                    var codRand = splitItem[i];
                                    var semn = semneList[i];

                                    var valoareRand = calculRows.FirstOrDefault(f => f.CodRand == codRand && f.SectorId == rand.SectorId);
                                    if (valoareRand != null)
                                    {
                                        sum += (semn == "+" ? 1 : -1) * valoareRand.Valoare;
                                    }
                                }
                                rand.Valoare = sum;
                                rand.Calculat = true;
                            }
                        }

                        var countOK = calculRows.Count(f => !f.Calculat);
                        if (countOK == 0)
                        {
                            calculatOk = true;
                        }

                        contor++;
                    }

                    if (!calculatOk)
                    {
                        var anexa = Context.BNR_Anexa.FirstOrDefault(f => f.Id == raportare.AnexaId);
                        throw new Exception("Nu am calculat totalurile pentru anexa " + anexa.Denumire + ". Varificati formulele pentru totaluri!");
                    }

                    foreach(var rand in calculRows.Where(f=>f.RandTotal))
                    {
                        var raportareRandTotal = new BNR_RaportareRand
                        {
                            BNR_RaportareId = raportareId,
                            AnexaDetailId = rand.AnexaDetailId,
                            SectorId = rand.SectorId,
                            Valoare = rand.Valoare
                        };

                        Context.BNR_RaportareRand.Add(raportareRandTotal);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void InsertRanduriDetailList(List<BNR_RaportareRandDetail> randDetails, int savedBalanceId)
        {
            try
            {
                Context.BNR_RaportareRandDetail.AddRange(randDetails);
                Context.SaveChanges();
            }
            catch (System.Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public void SaveRaportareRowDetails(int savedBalanceId)
        {
            var raportareRandList = Context.BNR_RaportareRand.Include(f => f.BNR_Raportare)
                                                             .Include(f => f.BNR_Raportare.SavedBalance)
                                                             .Include(f => f.BNR_AnexaDetail)
                                                             .Include(f => f.BNR_Sector)
                                            .Where(f => f.BNR_Raportare.SavedBalanceId == savedBalanceId).ToList();

            foreach (var item in raportareRandList)
            {
                var bnrConturi = Context.BNR_Conturi.Include(f => f.SavedBalance).Include(f => f.BNR_AnexaDetail).Include(f => f.BNR_Sector).Include(f => f.Account)
                                                        .Where(f => f.BNR_SectorId == item.SectorId && f.AnexaDetailId == item.AnexaDetailId &&
                                                               f.SavedBalanceId == item.BNR_Raportare.SavedBalanceId)
                                                        .Select(f => new BNR_RaportareRandDetail
                                                        {
                                                            BNR_RaportareRandId = item.Id,
                                                            Descriere = f.Account.Symbol,
                                                            Valoare = f.Value
                                                        }).ToList();

                Context.BNR_RaportareRandDetail.AddRange(bnrConturi);
            }
            UnitOfWorkManager.Current.SaveChanges();
        }

        public void SaveRaportareRows(int savedBalanceId)
        {
            var raportareRandList = Context.BNR_RaportareRand.Include(f => f.BNR_Raportare).Include(f => f.BNR_Sector).Include(f => f.BNR_AnexaDetail).Where(f => f.BNR_Raportare.SavedBalanceId == savedBalanceId).ToList();

            foreach (var item in raportareRandList)
            {
                var bnrConturi = Context.BNR_Conturi.Include(f => f.SavedBalance).Include(f => f.BNR_AnexaDetail).Include(f => f.BNR_Sector)
                                                    .Where(f => f.SavedBalanceId == savedBalanceId && f.BNR_SectorId == item.SectorId && item.SectorId != null && f.AnexaDetailId == item.AnexaDetailId)
                                                    .GroupBy(f => new { f.AnexaDetailId, f.BNR_SectorId, f.SavedBalanceId })
                                                     .Select(f => new BNR_RaportareRand
                                                     {
                                                         Id = item.Id,
                                                         AnexaDetailId = f.Key.AnexaDetailId,
                                                         BNR_RaportareId = item.BNR_RaportareId,
                                                         SectorId = f.Key.BNR_SectorId.Value,
                                                         Valoare = item.Valoare + f.Sum(g => g.Value)
                                                     })
                                                    .ToList();
                foreach (var ret in bnrConturi)
                {
                    //update rand
                    Context.Entry(item).CurrentValues.SetValues(ret);
                }
            }

            UnitOfWorkManager.Current.SaveChanges();
        }
        public void DesfacFormula(string formula, out List<string> semneList, out List<string> splitItem)
        {
            semneList = new List<string>();
            splitItem = new List<string>();
            if (!formula.StartsWith("+") || !formula.StartsWith("-"))
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
                string replaceForm = aux.Replace("#", "");
                splitItem.Add(replaceForm);
                formula = formula.Substring(index);
            }
        }


        public void DeleteRaportare(int savedBalanceId)
        {
            try
            {
                var raportareList = Context.BNR_Raportare.Where(f => f.SavedBalanceId == savedBalanceId).ToList();
                Context.BNR_Raportare.RemoveRange(raportareList);
                Context.SaveChanges();
            }
            catch (System.Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public decimal CalculFormulaAnexa4(string formula, int savedBalanceId)
        {
            try
            {
                decimal rez = 0;
                if (formula == "" || formula == null)
                {
                    return 0;
                }
                var balance = Context.SavedBalanceDetails.Include(f => f.Account).Where(f => f.SavedBalanceId == savedBalanceId).ToList();

                string expresie = formula;
                while (expresie.IndexOf(separatorExpresie) >= 0)
                {
                    int startIndex = expresie.IndexOf(separatorExpresie);
                    int endIndex = expresie.IndexOf(separatorExpresie, startIndex + 1);
                    string item = expresie.Substring(startIndex + 1, endIndex - startIndex - 1);
                    string[] splitItem = item.Split(separator);

                    string tipItem = splitItem[0];
                    string contItem = splitItem[1];
                    decimal? value = 0;
                    value = balance.Where(f => f.Account.Symbol.IndexOf(contItem) >= 0)
                                   .Sum(f => (tipItem == "ORC") ? f.CrValueY : f.DbValueY);

                    IFormatProvider formatProvider = new System.Globalization.CultureInfo("en-US", true);
                    string valueF = value.Value.ToString(formatProvider);
                    if (valueF.IndexOf(".") < 0) valueF = valueF + ".0";
                    expresie = expresie.Replace("$" + item + "$", valueF);
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }

    public class CalcTotalBnrDto
    {
        public int? SectorId { get; set; }
        public decimal Valoare { get; set; }
        public int AnexaDetailId { get; set; }
        public int RaportareId { get; set; }
        public string CodRand { get; set; }
        public bool Calculat { get; set; }
        public string Formula { get; set; }
        public bool RandTotal { get; set; }
    }
}
