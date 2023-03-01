using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.SectoareBnr;
using Niva.Erp.Repositories.SectoareBNR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.EntityFrameworkCore.Repositories.SectoareBNR
{
    public class BNR_SectorCalcRepository : ErpRepositoryBase<BNR_Conturi, int>, IBNR_SectorCalcRepository
    {
        public BNR_SectorCalcRepository(IDbContextProvider<ErpDbContext> dbContextProvider) : base(dbContextProvider)
        { }

        public void CalcConturi(int balanceId)
        {
            var appClient = 1;
            var balance = Context.SavedBalance.FirstOrDefault(f => f.Id == balanceId);
            var balanceRows = Context.SavedBalanceDetails.Include(f => f.Account).Where(f => f.SavedBalanceId == balanceId);
            var rowList = Context.BNR_AnexaDetails.Where(f => f.FormulaConta != null && f.FormulaConta != "" && f.State == State.Active
                                                         && f.EDinConta)
                                                  .OrderBy(f => f.NrCrt)
                                                  .ToList();

            var calcRows = new List<BNR_Conturi>();

            // initializez tabela de calcul
            try
            {
                foreach (var row in rowList)
                {
                    string formula = row.FormulaConta;
                    var semneList = new List<string>();
                    var splitItem = new List<KeyValuePair<string, string>>();
                    DesfacFormula(formula, out semneList, out splitItem);

                    for (int i = 0; i < semneList.Count; i++)
                    {
                        var dictItem = splitItem.ElementAt(i);
                        var tipItem = dictItem.Key;
                        var contItem = dictItem.Value;

                        var balantaAccountList = balanceRows.Where(f => f.Account.Symbol.IndexOf(contItem) == 0).ToList();

                        foreach (var balRow in balantaAccountList)
                        {
                            decimal value = 0;
                            switch (tipItem)
                            {
                                case "OFD":
                                    value = balRow.DbValueF;
                                    break;
                                case "OFC":
                                    value = balRow.CrValueF;
                                    break;
                                case "ORD":
                                    value = balRow.DbValueY;
                                    break;
                                case "ORC":
                                    value = balRow.CrValueY;
                                    break;
                            }

                            value = (semneList[i] == "+" ? 1 : -1) * value;

                            if (value != 0)
                            {
                                var calcRow = new BNR_Conturi
                                {
                                    AccountId = balRow.AccountId,
                                    AnexaDetailId = row.Id,
                                    ContBNR = contItem,
                                    SavedBalanceId = balanceId,
                                    BNR_SectorId = balRow.Account.SectorBnrId,
                                    SoldDb = balRow.DbValueF,
                                    SoldCr = balRow.CrValueF,
                                    TenantId = appClient,
                                    Value = value
                                };
                                calcRows.Add(calcRow);
                            }


                        }
                        #region Commented code
                        //foreach (var bal in balance.Where(f => accountList.Contains(f.AccountId) && (f.CrValueF - f.DbValueF) != 0))
                        //{
                        //    var value = (tipItem == "OFD") ? bal.DbValueF : bal.CrValueF;

                        //    var valueCurrent = calcRows.FirstOrDefault(f => f.AnexaDetailId == item.AnexaDetailId && f.CodRand == item.CodRand);
                        //    if (valueCurrent.Calculat)
                        //    {
                        //        formulaOK = false;
                        //    }
                        //    else
                        //    {
                        //        suma += (semneList[i] == "+" ? 1 : -1) * value;
                        //    }

                        //    Context.BNR_Conturi.Add(new BNR_Conturi
                        //    {
                        //        AccountId = bal.AccountId,
                        //        AnexaDetailId = item.AnexaDetailId,
                        //        BNR_SectorId = item.SectorId,
                        //        ContBNR = contItem,
                        //        CodRand = item.CodRand,
                        //        SavedBalanceId = item.SavedBalanceId,
                        //        SoldCr = (tipItem == "OFC") ? value : 0,
                        //        SoldDb = (tipItem == "OFD") ? value : 0,
                        //        TenantId = item.TenantId,
                        //        Value = suma
                        //    });
                        //} 
                        #endregion
                    }
                }
                #region Commented code
                //calcRows = rowList.Select(f => new BNR_SectorDetailDto
                //{
                //    AccountId = 0,
                //    AnexaDetailId = f.Id,
                //    SectorId = 1,
                //    Calculat = false,
                //    ContBNR = null,
                //    SavedBalanceId = balanceId,
                //    SoldCr = 0,
                //    SoldDb = 0,
                //    Value = 0,
                //    TenantId = appClient,
                //    CodRand = f.CodRand

                //}).ToList(); 
                #endregion

                Context.BNR_Conturi.AddRange(calcRows);
                Context.SaveChanges();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            #region Commented code
            //calculez valorile
            //try
            //{
            //    var calculatOK = false;
            //    int contor = 1;
            //    while (!calculatOK && contor < 50)
            //    {
            //        foreach (var item in calcRows)
            //        {
            //            var row = rowList.FirstOrDefault(f => f.Id == item.AnexaDetailId && f.CodRand == item.CodRand);
            //            string formula = row.FormulaConta;
            //            var semneList = new List<string>();
            //            var splitItem = new List<KeyValuePair<string, string>>();
            //            DesfacFormula(formula, out semneList, out splitItem);

            //            bool formulaOK = true;
            //            decimal suma = 0;
            //            for (int i = 0; i < semneList.Count; i++)
            //            {

            //                var dictItem = splitItem.ElementAt(i);
            //                var tipItem = dictItem.Key;
            //                var contItem = dictItem.Value;

            //                var account = _accountRepository.GetAccountBySymbol(contItem);
            //                var accountList = _accountRepository.GetAllAnalythics(account.Id).Select(f => f.Id).Distinct().ToList();

            //                foreach (var bal in balance.Where(f => accountList.Contains(f.AccountId) && (f.CrValueF - f.DbValueF) != 0))
            //                {
            //                    var value = (tipItem == "OFD") ? bal.DbValueF : bal.CrValueF;

            //                    var valueCurrent = calcRows.FirstOrDefault(f => f.AnexaDetailId == item.AnexaDetailId && f.CodRand == item.CodRand);
            //                    if (valueCurrent.Calculat)
            //                    {
            //                        formulaOK = false;
            //                    }
            //                    else
            //                    {
            //                        suma += (semneList[i] == "+" ? 1 : -1) * value;
            //                    }

            //                    Context.BNR_Conturi.Add(new BNR_Conturi
            //                    {
            //                        AccountId = bal.AccountId,
            //                        AnexaDetailId = item.AnexaDetailId,
            //                        BNR_SectorId = item.SectorId,
            //                        ContBNR = contItem,
            //                        CodRand = item.CodRand,
            //                        SavedBalanceId = item.SavedBalanceId,
            //                        SoldCr = (tipItem == "OFC") ? value : 0,
            //                        SoldDb = (tipItem == "OFD") ? value : 0,
            //                        TenantId = item.TenantId,
            //                        Value = suma
            //                    });
            //                }
            //            }

            //            if (formulaOK)
            //            {
            //                item.Calculat = true;
            //                item.Value = suma;
            //            }
            //        }
            //        var count = calcRows.Count(f => !f.Calculat);
            //        calculatOK = (count == 0);
            //        contor++;
            //    }

            //    //foreach (var item in calcRows)
            //    //{
            //    //    Context.BNR_Conturi.AddRange(new BNR_Conturi
            //    //    {
            //    //        AccountId = item.AccountId,
            //    //        AnexaDetailId = item.AnexaDetailId,
            //    //        BNR_SectorId = item.SectorId,
            //    //        ContBNR = item.ContBNR,
            //    //        SavedBalanceId = item.SavedBalanceId,
            //    //        SoldCr = item.SoldCr,
            //    //        SoldDb = item.SoldDb,
            //    //        TenantId = item.TenantId,
            //    //        Value = item.Value
            //    //    });
            //    //}
            //    // Context.BNR_Conturi.AddRange(calcRows);
            //    Context.SaveChanges();
            //}
            //catch (System.Exception ex)
            //{

            //    throw ex;
            //} 
            #endregion
        }

        private void DesfacFormula(string formula, out List<string> semneList, out List<KeyValuePair<string, string>> splitItem)
        {
            try
            {
                semneList = new List<string>();
                splitItem = new List<KeyValuePair<string, string>>();
                formula = formula.Replace("$", "");
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
                    string[] splitForm = aux.Split("#");
                    splitItem.Add(new KeyValuePair<string, string>(splitForm[0], splitForm[1]));
                    formula = formula.Substring(index);
                }
            }
            catch (System.Exception ex)
            {
                throw new Exception("Eroare procesare formula " + formula + " - " + ex.ToString());
            }
        }
    }

    public class BNR_SectorDetailDto
    {
        public int AnexaDetailId { get; set; }
        public int SavedBalanceId { get; set; }
        public string ContBNR { get; set; }
        public string Denumire { get; set; }
        public int AccountId { get; set; }
        public int? SectorId { get; set; }
        public decimal SoldDb { get; set; }
        public decimal SoldCr { get; set; }
        public decimal Value { get; set; }
        public int TenantId { get; set; }
        public string CodRand { get; set; }

    }
}
