using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Conta.Lichiditate;
using Niva.Erp.Models.PrePayments;
using Niva.Erp.Repositories.Conta.Lichiditate;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Niva.Erp.EntityFrameworkCore.Repositories.Conta
{
    public class LichidCalcRepository : ErpRepositoryBase<LichidCalc, int>, ILichidCalcRepository
    {
        private readonly char separatorExpresie = '$';
        private readonly char separator = '#';
        CurrencyRepository _currencyRepository;

        public LichidCalcRepository(IDbContextProvider<ErpDbContext> dbContextProvider) : base(dbContextProvider)
        {
            _currencyRepository = new CurrencyRepository(dbContextProvider);
        }

        public void DeleteLichidCalc(int savedBalanceId)
        {
            var lichidCalcList = Context.LichidCalc.Include(f => f.SavedBalance)
                                                   .Where(f => f.SavedBalanceId == savedBalanceId)
                                                   .ToList();
            Context.LichidCalc.RemoveRange(lichidCalcList);
        }

        public void DeleteLichidCalcCurr(int savedBalanceId)
        {
            var lichidCalcCurrList = Context.LichidCalcCurr.Include(f => f.SavedBalance)
                                                           .Where(f => f.SavedBalanceId == savedBalanceId)
                                                           .ToList();

            Context.LichidCalcCurr.RemoveRange(lichidCalcCurrList);
        }

        public void DeleteLichidCalcCurrTotal(int savedBalanceId)
        {
            try
            {
                var calcRowList = Context.LichidCalcCurr.Include(f => f.LichidConfig)
                                                    .Where(f => f.SavedBalanceId == savedBalanceId && f.LichidConfig.FormulaTotal != null && f.LichidConfig.FormulaTotal != "")
                                                    .Select(f => f.Id)
                                                    .ToList();
                Context.LichidCalcCurr.RemoveRange(Context.LichidCalcCurr.Where(f => calcRowList.Contains(f.Id)));
                Context.SaveChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void DeleteLichidCalcTotal(int savedBalanceId)
        {
            try
            {
                var calcRowList = Context.LichidCalc.Include(f => f.LichidConfig)
                                                    .Where(f => f.SavedBalanceId == savedBalanceId && f.LichidConfig.FormulaTotal != null && f.LichidConfig.FormulaTotal != "")
                                                    .Select(f => f.Id)
                                                    .ToList();
                Context.LichidCalc.RemoveRange(Context.LichidCalc.Where(f => calcRowList.Contains(f.Id)));
                Context.SaveChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void LichidCalc(int savedBalanceId, List<PlasamentLichiditateDto> plasamenteLichiditate)
        {
            var appClient = 1;
            var saveBalance = Context.SavedBalance.FirstOrDefault(f => f.Id == savedBalanceId);
            var savedBalanceRows = Context.SavedBalanceDetails.Include(f => f.Account).Where(f => f.SavedBalanceId == savedBalanceId);
            // var prepaymentsBalanceList = Context.PrepaymentBalance.Include(f => f.Prepayment).Where(f => f.ComputeDate == saveBalance.SaveDate && f.TenantId == appClient).ToList();

            var prepaymentsList = Context.PrepaymentBalance.Include(f => f.Prepayment)
                                                    .Where(f => f.ComputeDate <= saveBalance.SaveDate && f.Prepayment.PrepaymentType == PrepaymentType.CheltuieliInAvans)
                                                    .GroupBy(f => f.PrepaymentId)
                                                    .Select(f => f.Max(x => x.Id))
                                                    .ToList();

            var prepaymentsBalanceList = Context.PrepaymentBalance.Include(f => f.Prepayment)
                                                    .Where(f => prepaymentsList.Contains(f.Id) && f.Quantity != 0)
                                                    .ToList();

            var rowList = Context.LichidConfig.Where(f => f.State == State.Active && f.FormulaTotal == null || f.FormulaTotal == "").OrderBy(f => f.OrderView).ToList();


            var lichidBenziList = Context.LichidBenzi.ToList();

            var calcRows = new List<LichidCalc>();

            foreach (var banda in lichidBenziList)
            {
                calcRows.AddRange(rowList.Select(f => new LichidCalc
                {
                    LichidBenziId = banda.Id,
                    LichidBenzi = banda,
                    LichidConfigId = f.Id,
                    LichidConfig = f,
                    Valoare = 0,
                    SavedBalanceId = savedBalanceId,
                    TenantId = appClient
                }));
            }

            Context.LichidCalc.AddRange(calcRows);
            Context.SaveChanges();
            var lichidDetList = new List<LichidCalcDet>();

            try
            {
                foreach (var calcItem in calcRows)
                {
                    if (calcItem.LichidConfig.EDinConta && calcItem.LichidConfig.FormulaConta != null && calcItem.LichidConfig.FormulaConta != "")
                    {
                        if ((calcItem.LichidConfig.LichidBenziId ?? 0) == calcItem.LichidBenziId)
                        {
                            string formula = calcItem.LichidConfig.FormulaConta;
                            decimal value = 0;

                            while (formula.IndexOf(separatorExpresie) >= 0)
                            {
                                int indexStart = formula.IndexOf(separatorExpresie);
                                int indexEnd = formula.IndexOf(separatorExpresie, indexStart + 1);
                                string item = formula.Substring(indexStart + 1, indexEnd - indexStart - 1);
                                string[] splitItem = item.Split(separator);

                                string tipItem = splitItem[0];
                                string contItem = splitItem[1];

                                if (tipItem == "OSFD")
                                {
                                    value = savedBalanceRows.Where(f => f.Account.Symbol.IndexOf(contItem) == 0).Sum(f => f.DbValueF);
                                }

                                if (tipItem == "OSFC")
                                {
                                    value = savedBalanceRows.Where(f => f.Account.Symbol.IndexOf(contItem) == 0).Sum(f => f.CrValueF);
                                }

                                IFormatProvider formatProvider = new System.Globalization.CultureInfo("en-US", true);
                                string valueF = value.ToString(formatProvider);
                                if (valueF.IndexOf(".") < 0) valueF = valueF + ".0";
                                formula = formula.Replace("$" + item + "$", valueF);

                                lichidDetList.Add(new LichidCalcDet
                                {
                                    Valoare = value,
                                    Descriere = item,
                                    TenantId = appClient,
                                    LichidCalcId = calcItem.Id
                                });
                            }
                            calcItem.Valoare = Convert.ToDecimal(new DataTable().Compute(formula, null));
                        }
                    }
                    else
                    {
                        if (calcItem.LichidConfig.TipInstrument != null && calcItem.LichidConfig.TipInstrument != "")
                        {
                            string[] splittedTipInstrument = calcItem.LichidConfig.TipInstrument.Split("#").Select(f => f.Trim()).Where(f => !string.IsNullOrWhiteSpace(f)).ToArray();

                            foreach (var split in splittedTipInstrument)
                            {
                                if (split == "CA")
                                {
                                    var valCheltAvans = prepaymentsBalanceList.Where(f => calcItem.LichidBenzi.DurataInLuniMinima < f.Duration && f.Duration <= calcItem.LichidBenzi.DurataInLuniMaxima)
                                                                              .Sum(f => f.PrepaymentValue);
                                    calcItem.Valoare += valCheltAvans;
                                    var chelAvansDet = prepaymentsBalanceList.Where(f => calcItem.LichidBenzi.DurataInLuniMinima < f.Duration && f.Duration <= calcItem.LichidBenzi.DurataInLuniMaxima)
                                                                             .Select(f => new LichidCalcDet
                                                                             {
                                                                                 Descriere = f.Prepayment.Description,
                                                                                 Valoare = f.PrepaymentValue,
                                                                                 TenantId = appClient,
                                                                                 LichidCalcId = calcItem.Id
                                                                             })
                                                                             .ToList();
                                    lichidDetList.AddRange(chelAvansDet);
                                }

                                if (split == "DP")
                                {
                                    var valPlasament = plasamenteLichiditate.Where(f => f.tipPlasament == "D" && calcItem.LichidBenzi.DurataMinima < (f.maturityDate - saveBalance.SaveDate).Days &&
                                                                                       (f.maturityDate - saveBalance.SaveDate).Days <= calcItem.LichidBenzi.DurataMaxima)
                                                                            .Sum(f => f.valoareInvestita);


                                    calcItem.Valoare += valPlasament;
                                    var plasamentList = plasamenteLichiditate.Where(f => f.tipPlasament == "D" && calcItem.LichidBenzi.DurataMinima < (f.maturityDate - saveBalance.SaveDate).Days &&
                                                                                       (f.maturityDate - saveBalance.SaveDate).Days <= calcItem.LichidBenzi.DurataMaxima)
                                                                             .Select(f => new LichidCalcDet
                                                                             {
                                                                                 Descriere = f.idplasament + " - " + f.tipPlasament,
                                                                                 Valoare = f.valoareInvestita,
                                                                                 TenantId = calcItem.TenantId,
                                                                                 LichidCalcId = calcItem.Id
                                                                             })
                                                                            .ToList();
                                    lichidDetList.AddRange(plasamentList);
                                }

                                if (split == "DD")
                                {
                                    var valPlasament = plasamenteLichiditate.Where(f => f.tipPlasament == "D" && f.maturitateCreanta != null && calcItem.LichidBenzi.DurataMinima < (f.maturitateCreanta.Value - saveBalance.SaveDate).Days &&
                                                                                       (f.maturitateCreanta.Value - saveBalance.SaveDate).Days <= calcItem.LichidBenzi.DurataMaxima)
                                                                            .Sum(f => f.valoareCreanta);
                                    calcItem.Valoare += valPlasament;
                                    var plasamentList = plasamenteLichiditate.Where(f => f.tipPlasament == "D" && f.maturitateCreanta != null && calcItem.LichidBenzi.DurataMinima < (f.maturitateCreanta.Value - saveBalance.SaveDate).Days &&
                                                                                       (f.maturitateCreanta.Value - saveBalance.SaveDate).Days <= calcItem.LichidBenzi.DurataMaxima)
                                                                             .Select(f => new LichidCalcDet
                                                                             {
                                                                                 Descriere = f.idplasament + " - " + f.tipPlasament,
                                                                                 Valoare = f.valoareCreanta,
                                                                                 TenantId = calcItem.TenantId,
                                                                                 LichidCalcId = calcItem.Id
                                                                             })
                                                                            .ToList();
                                    lichidDetList.AddRange(plasamentList);
                                }

                                if (split == "TP")
                                {
                                    var valPlasament = plasamenteLichiditate.Where(f => f.tipPlasament == "T" && calcItem.LichidBenzi.DurataMinima < (f.maturityDate - saveBalance.SaveDate).Days &&
                                                                                       (f.maturityDate - saveBalance.SaveDate).Days <= calcItem.LichidBenzi.DurataMaxima)
                                                                            .Sum(f => (f.valoareContabila - f.valoareDepreciere));
                                    calcItem.Valoare += valPlasament;
                                    var plasamentList = plasamenteLichiditate.Where(f => f.tipPlasament == "T" && calcItem.LichidBenzi.DurataMinima < (f.maturityDate - saveBalance.SaveDate).Days &&
                                                                                       (f.maturityDate - saveBalance.SaveDate).Days <= calcItem.LichidBenzi.DurataMaxima)
                                                                             .Select(f => new LichidCalcDet
                                                                             {
                                                                                 Descriere = f.idplasament + " - " + f.tipPlasament,
                                                                                 Valoare = (f.valoareContabila - f.valoareDepreciere),
                                                                                 TenantId = calcItem.TenantId,
                                                                                 LichidCalcId = calcItem.Id
                                                                             })
                                                                            .ToList();
                                    lichidDetList.AddRange(plasamentList);
                                }

                                if (split == "TD")
                                {
                                    var valPlasament = plasamenteLichiditate.Where(f => f.tipPlasament == "T" && f.maturitateCreanta != null && calcItem.LichidBenzi.DurataMinima < (f.maturitateCreanta.Value - saveBalance.SaveDate).Days &&
                                                                                       (f.maturitateCreanta.Value - saveBalance.SaveDate).Days <= calcItem.LichidBenzi.DurataMaxima)
                                                                            .Sum(f => f.valoareCreanta);
                                    calcItem.Valoare += valPlasament;
                                    var plasamentList = plasamenteLichiditate.Where(f => f.tipPlasament == "T" && f.maturitateCreanta != null && calcItem.LichidBenzi.DurataMinima < (f.maturitateCreanta.Value - saveBalance.SaveDate).Days &&
                                                                                       (f.maturitateCreanta.Value - saveBalance.SaveDate).Days <= calcItem.LichidBenzi.DurataMaxima)
                                                                             .Select(f => new LichidCalcDet
                                                                             {
                                                                                 Descriere = f.idplasament + " - " + f.tipPlasament,
                                                                                 Valoare = f.valoareCreanta,
                                                                                 TenantId = calcItem.TenantId,
                                                                                 LichidCalcId = calcItem.Id
                                                                             })
                                                                            .ToList();
                                    lichidDetList.AddRange(plasamentList);
                                }

                                if (split == "OP")
                                {
                                    var valPlasament = plasamenteLichiditate.Where(f => f.tipPlasament == "O" && calcItem.LichidBenzi.DurataMinima < (f.maturityDate - saveBalance.SaveDate).Days &&
                                                                                       (f.maturityDate - saveBalance.SaveDate).Days <= calcItem.LichidBenzi.DurataMaxima)
                                                                            .Sum(f => (f.valoareContabila - f.valoareDepreciere));
                                    calcItem.Valoare += valPlasament;
                                    var plasamentList = plasamenteLichiditate.Where(f => f.tipPlasament == "O" && calcItem.LichidBenzi.DurataMinima < (f.maturityDate - saveBalance.SaveDate).Days &&
                                                                                       (f.maturityDate - saveBalance.SaveDate).Days <= calcItem.LichidBenzi.DurataMaxima)
                                                                             .Select(f => new LichidCalcDet
                                                                             {
                                                                                 Descriere = f.idplasament + " - " + f.tipPlasament,
                                                                                 Valoare = (f.valoareContabila - f.valoareDepreciere),
                                                                                 TenantId = calcItem.TenantId,
                                                                                 LichidCalcId = calcItem.Id
                                                                             })
                                                                            .ToList();
                                    lichidDetList.AddRange(plasamentList);
                                }

                                if (split == "OD")
                                {
                                    var valPlasament = plasamenteLichiditate.Where(f => f.tipPlasament == "O" && f.maturitateCreanta != null && calcItem.LichidBenzi.DurataMinima < (f.maturitateCreanta.Value - saveBalance.SaveDate).Days &&
                                                                                       (f.maturitateCreanta.Value - saveBalance.SaveDate).Days <= calcItem.LichidBenzi.DurataMaxima)
                                                                            .Sum(f => (f.valoareCreanta));
                                    calcItem.Valoare += valPlasament;
                                    var plasamentList = plasamenteLichiditate.Where(f => f.tipPlasament == "O" && f.maturitateCreanta != null && calcItem.LichidBenzi.DurataMinima < (f.maturitateCreanta.Value - saveBalance.SaveDate).Days &&
                                                                                       (f.maturitateCreanta.Value - saveBalance.SaveDate).Days <= calcItem.LichidBenzi.DurataMaxima)
                                                                             .Select(f => new LichidCalcDet
                                                                             {
                                                                                 Descriere = f.idplasament + " - " + f.tipPlasament,
                                                                                 Valoare = f.valoareCreanta,
                                                                                 TenantId = calcItem.TenantId,
                                                                                 LichidCalcId = calcItem.Id
                                                                             })
                                                                            .ToList();
                                    lichidDetList.AddRange(plasamentList);
                                }
                            }
                        }

                    }
                }

                Context.LichidCalcDet.AddRange(lichidDetList);
                Context.SaveChanges();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            #region Commented code

            //try
            //{
            //    foreach (var row in rowList)
            //    {
            //        var lichidDetList = new List<LichidCalcDet>();
            //        if (row.EDinConta && row.FormulaConta != null && row.FormulaConta != "")
            //        {
            //            string formula = row.FormulaConta;
            //            decimal value = 0;

            //            while (formula.IndexOf(separatorExpresie) >= 0)
            //            {
            //                int indexStart = formula.IndexOf(separatorExpresie);
            //                int indexEnd = formula.IndexOf(separatorExpresie, indexStart + 1);
            //                string item = formula.Substring(indexStart + 1, indexEnd - indexStart - 1);
            //                string[] splitItem = item.Split(separator);

            //                string tipItem = splitItem[0];
            //                string contItem = splitItem[1];

            //                if (tipItem == "OSFD")
            //                {
            //                    value = savedBalanceRows.Where(f => f.Account.Symbol.IndexOf(contItem) == 0).Sum(f => f.DbValueF);
            //                }

            //                if (tipItem == "OSFC")
            //                {
            //                    value = savedBalanceRows.Where(f => f.Account.Symbol.IndexOf(contItem) == 0).Sum(f => f.CrValueF);
            //                }

            //                IFormatProvider formatProvider = new System.Globalization.CultureInfo("en-US", true);
            //                string valueF = value.ToString(formatProvider);
            //                if (valueF.IndexOf(".") < 0) valueF = valueF + ".0";
            //                formula = formula.Replace("$" + item + "$", valueF);

            //                lichidDetList.Add(new LichidCalcDet
            //                {
            //                    Valoare = value,
            //                    Descriere = item,
            //                    TenantId = appClient
            //                });
            //            }

            //            foreach (var item in calcRows)
            //            {
            //                if (row.LichidBenziId == item.LichidBenziId && row.Id == item.LichidConfigId)
            //                {
            //                    item.Valoare = Convert.ToDecimal(new DataTable().Compute(formula, null));
            //                    item.LichidCalcDet = lichidDetList;
            //                }
            //            }
            //        }
            //        else
            //        {
            //            string[] splittedTipInstrument = row.TipInstrument.Split("#").Select(f => f.Trim()).Where(f => !string.IsNullOrWhiteSpace(f)).ToArray();

            //            foreach (var split in splittedTipInstrument)
            //            {
            //                if (split == "CA")
            //                {

            //                    foreach (var gest in prepaymentsBalanceList)
            //                    {
            //                        lichidDetList.Add(new LichidCalcDet
            //                        {
            //                            Valoare = gest.PrepaymentValue,
            //                            Descriere = gest.Prepayment.Description,
            //                            TenantId = appClient
            //                        });

            //                        foreach (var calcRow in calcRows)
            //                        {
            //                            foreach (var banda in lichidBenziList)
            //                            {
            //                                if (banda.Id == calcRow.LichidBenziId && row.Id == calcRow.LichidConfigId && banda.DurataMinima <= gest.Duration && gest.Duration <= banda.DurataMaxima)
            //                                {
            //                                    calcRow.Valoare = gest.PrepaymentValue;
            //                                    calcRow.LichidCalcDet = lichidDetList;
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    Context.LichidCalc.AddRange(calcRows);
            //    Context.SaveChanges();
            //}
            //catch (System.Exception ex)
            //{
            //    throw ex;
            //} 
            #endregion
        }

        public void LichidCalcCurr(int savedBalanceId, List<PlasamentLichiditateDto> plasamenteLichiditate)
        {
            try
            {
                var appClient = 1;
                var saveBalance = Context.SavedBalance.FirstOrDefault(f => f.Id == savedBalanceId);
                var savedBalanceRows = Context.SavedBalanceDetails.Include(f => f.Account).Include(f => f.Currency).Where(f => f.SavedBalanceId == savedBalanceId);

                var prepaymentsList = Context.PrepaymentBalance.Include(f => f.Prepayment)
                                                        .Where(f => f.ComputeDate <= saveBalance.SaveDate && f.Prepayment.PrepaymentType == PrepaymentType.CheltuieliInAvans)
                                                        .GroupBy(f => f.PrepaymentId)
                                                        .Select(f => f.Max(x => x.Id))
                                                        .ToList();

                var prepaymentsBalanceList = Context.PrepaymentBalance.Include(f => f.Prepayment).Include(f => f.Prepayment.InvoiceDetails.Invoices)
                                                        .Where(f => prepaymentsList.Contains(f.Id) && f.Quantity != 0)
                                                        .ToList();

                var rowList = Context.LichidConfig.Where(f => f.State == State.Active && f.FormulaTotal == null || f.FormulaTotal == "").OrderBy(f => f.OrderView).ToList();

                var lichidBenziCurrList = Context.LichidBenziCurr.ToList();

                var calcRows = new List<LichidCalcCurr>();

                foreach (var banda in lichidBenziCurrList)
                {
                    calcRows.AddRange(rowList.Select(f => new LichidCalcCurr
                    {
                        LichidBenziCurrId = banda.Id,
                        LichidBenziCurr = banda,
                        LichidConfigId = f.Id,
                        LichidConfig = f,
                        Valoare = 0,
                        SavedBalanceId = savedBalanceId,
                        TenantId = appClient
                    }));
                }

                Context.LichidCalcCurr.AddRange(calcRows);
                Context.SaveChanges();
                var lichidDetList = new List<LichidCalcCurrDet>();
                var currenciesList = lichidBenziCurrList.Select(f => f.CurrencyId).Distinct().ToList();

                try
                {
                    foreach (var calcItem in calcRows)
                    {
                        if (calcItem.LichidConfig.EDinConta && calcItem.LichidConfig.FormulaConta != null && calcItem.LichidConfig.FormulaConta != "")
                        {
                                string formula = calcItem.LichidConfig.FormulaConta;
                                decimal value = 0;

                                while (formula.IndexOf(separatorExpresie) >= 0)
                                {
                                    int indexStart = formula.IndexOf(separatorExpresie);
                                    int indexEnd = formula.IndexOf(separatorExpresie, indexStart + 1);
                                    string item = formula.Substring(indexStart + 1, indexEnd - indexStart - 1);
                                    string[] splitItem = item.Split(separator);

                                    string tipItem = splitItem[0];
                                    string contItem = splitItem[1];

                                    if (tipItem == "OSFD")
                                    {
                                        value = savedBalanceRows.Where(f => f.Account.Symbol.IndexOf(contItem) >= 0 &&
                                                                            (calcItem.LichidBenziCurr.CurrencyId == null ? !currenciesList.Contains(f.CurrencyId) :
                                                                            f.CurrencyId == calcItem.LichidBenziCurr.CurrencyId))
                                                                .Sum(f => f.DbValueF);
                                    }

                                    if (tipItem == "OSFC")
                                    {
                                        value = savedBalanceRows.Where(f => f.Account.Symbol.IndexOf(contItem) >= 0 &&
                                                                            (calcItem.LichidBenziCurr.CurrencyId == null ? !currenciesList.Contains(f.CurrencyId) :
                                                                            f.CurrencyId == calcItem.LichidBenziCurr.CurrencyId)).Sum(f => f.CrValueF);
                                    }

                                    IFormatProvider formatProvider = new System.Globalization.CultureInfo("en-US", true);
                                    string valueF = value.ToString(formatProvider);
                                    if (valueF.IndexOf(".") < 0) valueF = valueF + ".0";
                                    formula = formula.Replace("$" + item + "$", valueF);

                                    lichidDetList.Add(new LichidCalcCurrDet
                                    {
                                        Valoare = value,
                                        Descriere = item,
                                        TenantId = appClient,
                                        LichidCalcCurrId = calcItem.Id
                                    });
                                }
                                calcItem.Valoare = Convert.ToDecimal(new DataTable().Compute(formula, null));
                        }else
                        {
                            if (calcItem.LichidConfig.TipInstrument != null && calcItem.LichidConfig.TipInstrument != "")
                            {
                                string[] splittedTipInstrument = calcItem.LichidConfig.TipInstrument.Split("#").Select(f => f.Trim()).Where(f => !string.IsNullOrWhiteSpace(f)).ToArray();
                                var localCurrencyId = 1;
                                foreach (var split in splittedTipInstrument)
                                {
                                    if (split == "CA")
                                    {
                                        var valCheltAvans = prepaymentsBalanceList.Where(f => (f.Prepayment.InvoiceDetails !=null ? (calcItem.LichidBenziCurr.CurrencyId == null ? !currenciesList.Contains(f.Prepayment.InvoiceDetails.Invoices.CurrencyId) :
                                                                                               calcItem.LichidBenziCurr.CurrencyId == localCurrencyId) : calcItem.LichidBenziCurr.CurrencyId == localCurrencyId))
                                                                                  .Sum(f => f.PrepaymentValue);
                                        calcItem.Valoare += valCheltAvans;
                                        var chelAvansDet = prepaymentsBalanceList.Where(f => (f.Prepayment.InvoiceDetails != null ? (calcItem.LichidBenziCurr.CurrencyId == null ? !currenciesList.Contains(f.Prepayment.InvoiceDetails.Invoices.CurrencyId) :
                                                                                               calcItem.LichidBenziCurr.CurrencyId == localCurrencyId) : calcItem.LichidBenziCurr.CurrencyId == localCurrencyId))
                                                                                 .Select(f => new LichidCalcCurrDet
                                                                                 {
                                                                                     Descriere = f.Prepayment.Description,
                                                                                     Valoare = f.PrepaymentValue,
                                                                                     TenantId = appClient,
                                                                                     LichidCalcCurrId = calcItem.Id
                                                                                 })
                                                                                 .ToList();
                                        lichidDetList.AddRange(chelAvansDet);
                                    }
                                    if (split == "DP")
                                    {
                                        var valPlasament = plasamenteLichiditate.Where(f => f.tipPlasament == "D" && calcItem.LichidBenziCurr.CurrencyId == _currencyRepository.GetByCode(f.moneda).Id)
                                                                                .Sum(f => f.valoareInvestita);


                                        calcItem.Valoare += valPlasament;
                                        var plasamentList = plasamenteLichiditate.Where(f => f.tipPlasament == "D" && calcItem.LichidBenziCurr.CurrencyId == _currencyRepository.GetByCode(f.moneda).Id)
                                                                                 .Select(f => new LichidCalcCurrDet
                                                                                 {
                                                                                     Descriere = f.idplasament + " - " + f.tipPlasament,
                                                                                     Valoare = f.valoareInvestita,
                                                                                     TenantId = calcItem.TenantId,
                                                                                     LichidCalcCurrId = calcItem.Id
                                                                                 })
                                                                                .ToList();
                                        lichidDetList.AddRange(plasamentList);
                                    }

                                    if (split == "DD")
                                    {
                                        var valPlasament = plasamenteLichiditate.Where(f => f.tipPlasament == "D" && f.maturitateCreanta != null && calcItem.LichidBenziCurr.CurrencyId == _currencyRepository.GetByCode(f.moneda).Id)
                                                                                .Sum(f => f.valoareCreanta);
                                        calcItem.Valoare += valPlasament;
                                        var plasamentList = plasamenteLichiditate.Where(f => f.tipPlasament == "D" && f.maturitateCreanta != null && calcItem.LichidBenziCurr.CurrencyId == _currencyRepository.GetByCode(f.moneda).Id)
                                                                                 .Select(f => new LichidCalcCurrDet
                                                                                 {
                                                                                     Descriere = f.idplasament + " - " + f.tipPlasament,
                                                                                     Valoare = f.valoareCreanta,
                                                                                     TenantId = calcItem.TenantId,
                                                                                     LichidCalcCurrId = calcItem.Id
                                                                                 })
                                                                                .ToList();
                                        lichidDetList.AddRange(plasamentList);
                                    }

                                    if (split == "TP")
                                    {
                                        var valPlasament = plasamenteLichiditate.Where(f => f.tipPlasament == "T" && calcItem.LichidBenziCurr.CurrencyId == _currencyRepository.GetByCode(f.moneda).Id)
                                                                                .Sum(f => (f.valoareContabila - f.valoareDepreciere));
                                        calcItem.Valoare += valPlasament;
                                        var plasamentList = plasamenteLichiditate.Where(f => f.tipPlasament == "T" && calcItem.LichidBenziCurr.CurrencyId == _currencyRepository.GetByCode(f.moneda).Id)
                                                                                 .Select(f => new LichidCalcCurrDet
                                                                                 {
                                                                                     Descriere = f.idplasament + " - " + f.tipPlasament,
                                                                                     Valoare = (f.valoareContabila - f.valoareDepreciere),
                                                                                     TenantId = calcItem.TenantId,
                                                                                     LichidCalcCurrId = calcItem.Id
                                                                                 })
                                                                                .ToList();
                                        lichidDetList.AddRange(plasamentList);
                                    }

                                    if (split == "TD")
                                    {
                                        var valPlasament = plasamenteLichiditate.Where(f => f.tipPlasament == "T" && f.maturitateCreanta != null && calcItem.LichidBenziCurr.CurrencyId == _currencyRepository.GetByCode(f.moneda).Id)
                                                                                .Sum(f => f.valoareCreanta);
                                        calcItem.Valoare += valPlasament;
                                        var plasamentList = plasamenteLichiditate.Where(f => f.tipPlasament == "T" && f.maturitateCreanta != null && calcItem.LichidBenziCurr.CurrencyId == _currencyRepository.GetByCode(f.moneda).Id)
                                                                                 .Select(f => new LichidCalcCurrDet
                                                                                 {
                                                                                     Descriere = f.idplasament + " - " + f.tipPlasament,
                                                                                     Valoare = f.valoareCreanta,
                                                                                     TenantId = calcItem.TenantId,
                                                                                     LichidCalcCurrId = calcItem.Id
                                                                                 })
                                                                                .ToList();
                                        lichidDetList.AddRange(plasamentList);
                                    }

                                    if (split == "OP")
                                    {
                                        var valPlasament = plasamenteLichiditate.Where(f => f.tipPlasament == "O" && calcItem.LichidBenziCurr.CurrencyId == _currencyRepository.GetByCode(f.moneda).Id)
                                                                                .Sum(f => (f.valoareContabila - f.valoareDepreciere));
                                        calcItem.Valoare += valPlasament;
                                        var plasamentList = plasamenteLichiditate.Where(f => f.tipPlasament == "O" && calcItem.LichidBenziCurr.CurrencyId == _currencyRepository.GetByCode(f.moneda).Id)
                                                                                 .Select(f => new LichidCalcCurrDet
                                                                                 {
                                                                                     Descriere = f.idplasament + " - " + f.tipPlasament,
                                                                                     Valoare = (f.valoareContabila - f.valoareDepreciere),
                                                                                     TenantId = calcItem.TenantId,
                                                                                     LichidCalcCurrId = calcItem.Id
                                                                                 })
                                                                                .ToList();
                                        lichidDetList.AddRange(plasamentList);
                                    }

                                    if (split == "OD")
                                    {
                                        var valPlasament = plasamenteLichiditate.Where(f => f.tipPlasament == "O" && f.maturitateCreanta != null && calcItem.LichidBenziCurr.CurrencyId == _currencyRepository.GetByCode(f.moneda).Id)
                                                                                .Sum(f => (f.valoareCreanta));
                                        calcItem.Valoare += valPlasament;
                                        var plasamentList = plasamenteLichiditate.Where(f => f.tipPlasament == "O" && f.maturitateCreanta != null && calcItem.LichidBenziCurr.CurrencyId == _currencyRepository.GetByCode(f.moneda).Id)
                                                                                 .Select(f => new LichidCalcCurrDet
                                                                                 {
                                                                                     Descriere = f.idplasament + " - " + f.tipPlasament,
                                                                                     Valoare = f.valoareCreanta,
                                                                                     TenantId = calcItem.TenantId,
                                                                                     LichidCalcCurrId = calcItem.Id
                                                                                 })
                                                                                .ToList();
                                        lichidDetList.AddRange(plasamentList);
                                    }
                                }
                            }
                        }
                    }
                    Context.LichidCalcCurrDet.AddRange(lichidDetList);
                    Context.SaveChanges();
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void LichidCalcCurrTotaluri(int savedBalanceId)
        {
            try
            {
                DeleteLichidCalcCurrTotal(savedBalanceId);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            var appClient = 1;

            var lichidCalcRowList = Context.LichidCalcCurr.Include(f => f.LichidConfig).Where(f => f.SavedBalanceId == savedBalanceId).OrderBy(f => f.LichidConfig.OrderView).ToList();
            var calcRows = new List<LichidCalcCurrDto>();

            calcRows.AddRange(lichidCalcRowList.Select(f => new LichidCalcCurrDto
            {
                LichidBenziCurrId = f.LichidBenziCurrId,
                LichidConfigId = f.LichidConfigId,
                Valoare = f.Valoare,
                CodRand = f.LichidConfig.CodRand,
                SavedBalanceId = savedBalanceId,
                TenantId = appClient,
                Calculat = true,
                Formula = f.LichidConfig.FormulaConta,
                RandTotal = false
            }));

            var randuriTotal = Context.LichidConfig.Where(f => f.FormulaTotal != null && f.FormulaTotal != "" && f.State == State.Active).OrderBy(f => f.OrderView).ToList();

            foreach (var item in randuriTotal)
            {
                var benziList = calcRows.Where(f => f.SavedBalanceId == savedBalanceId).Select(f => f.LichidBenziCurrId).Distinct().ToList();
                foreach (var banda in benziList)
                {
                    var randTotalCalc = new LichidCalcCurrDto
                    {
                        LichidBenziCurrId = banda,
                        LichidConfigId = item.Id,
                        Valoare = 0,
                        CodRand = item.CodRand,
                        SavedBalanceId = savedBalanceId,
                        TenantId = appClient,
                        Calculat = false,
                        Formula = item.FormulaTotal,
                        RandTotal = true
                    };
                    calcRows.Add(randTotalCalc);
                }

            }

            bool calculatOk = false;
            int contor = 0;

            while (!calculatOk && contor <= 50)
            {
                foreach (var rand in calcRows.Where(f => !f.Calculat))
                {
                    string formula = rand.Formula;
                    var semneList = new List<string>();
                    var splitItem = new List<string>();
                    DesfacFormula(formula, out semneList, out splitItem);

                    decimal sum = 0;
                    var count = calcRows.Count(f => splitItem.Contains(f.CodRand) && !f.Calculat && f.LichidBenziCurrId == rand.LichidBenziCurrId);
                    if (count == 0) // sunt toate componentele calculate
                    {
                        for (int i = 0; i < semneList.Count; i++)
                        {
                            var codRand = splitItem[i];
                            var semn = semneList[i];

                            var valoareRand = calcRows.FirstOrDefault(f => f.CodRand == codRand && f.LichidBenziCurrId == rand.LichidBenziCurrId);
                            if (valoareRand != null)
                            {
                                sum += (semn == "+" ? 1 : -1) * valoareRand.Valoare;
                            }
                        }
                        rand.Valoare = sum;
                        rand.Calculat = true;
                    }
                }

                var countOK = calcRows.Count(f => !f.Calculat);
                if (countOK == 0)
                {
                    calculatOk = true;
                }

                contor++;
            }

            if (!calculatOk)
            {
                throw new Exception("Nu am calculat totalurile pentru lichiditate. Verificati formulele pentru totaluri!");
            }

            foreach (var rand in calcRows.Where(f => f.RandTotal))
            {
                var lichidRandTotal = new LichidCalcCurr
                {
                    LichidBenziCurrId = rand.LichidBenziCurrId,
                    LichidConfigId = rand.LichidConfigId,
                    SavedBalanceId = rand.SavedBalanceId,
                    Valoare = rand.Valoare,
                    TenantId = rand.TenantId,
                };

                Context.LichidCalcCurr.Add(lichidRandTotal);
            }
            Context.SaveChanges();
        }

        public void LichidCalcTotaluri(int savedBalanceId)
        {
            try
            {
                DeleteLichidCalcTotal(savedBalanceId);
            }
            catch (Exception ex)
            {

                throw ex;
            }

            var appClient = 1;

            var lichidCalcRowList = Context.LichidCalc.Include(f => f.LichidConfig).Where(f => f.SavedBalanceId == savedBalanceId).OrderBy(f => f.LichidConfig.OrderView).ToList();
            var calcRows = new List<LichidCalcDto>();

            calcRows.AddRange(lichidCalcRowList.Select(f => new LichidCalcDto
            {
                LichidBenziId = f.LichidBenziId,
                LichidConfigId = f.LichidConfigId,
                Valoare = f.Valoare,
                CodRand = f.LichidConfig.CodRand,
                SavedBalanceId = savedBalanceId,
                TenantId = appClient,
                Calculat = true,
                Formula = f.LichidConfig.FormulaConta,
                RandTotal = false
            }));

            var randuriTotal = Context.LichidConfig.Where(f => f.FormulaTotal != null && f.FormulaTotal != "" && f.State == State.Active).OrderBy(f => f.OrderView).ToList();

            foreach (var item in randuriTotal)
            {
                var benziList = calcRows.Where(f => f.SavedBalanceId == savedBalanceId).Select(f => f.LichidBenziId).Distinct().ToList();
                foreach (var banda in benziList)
                {
                    var randTotalCalc = new LichidCalcDto
                    {
                        LichidBenziId = banda,
                        LichidConfigId = item.Id,
                        Valoare = 0,
                        CodRand = item.CodRand,
                        SavedBalanceId = savedBalanceId,
                        TenantId = appClient,
                        Calculat = false,
                        Formula = item.FormulaTotal,
                        RandTotal = true
                    };
                    calcRows.Add(randTotalCalc);
                }

            }

            bool calculatOk = false;
            int contor = 0;

            while (!calculatOk && contor <= 50)
            {
                foreach (var rand in calcRows.Where(f => !f.Calculat))
                {
                    string formula = rand.Formula;
                    var semneList = new List<string>();
                    var splitItem = new List<string>();
                    DesfacFormula(formula, out semneList, out splitItem);

                    decimal sum = 0;
                    var count = calcRows.Count(f => splitItem.Contains(f.CodRand) && !f.Calculat && f.LichidBenziId == rand.LichidBenziId);
                    if (count == 0) // sunt toate componentele calculate
                    {
                        for (int i = 0; i < semneList.Count; i++)
                        {
                            var codRand = splitItem[i];
                            var semn = semneList[i];

                            var valoareRand = calcRows.FirstOrDefault(f => f.CodRand == codRand && f.LichidBenziId == rand.LichidBenziId);
                            if (valoareRand != null)
                            {
                                sum += (semn == "+" ? 1 : -1) * valoareRand.Valoare;
                            }
                        }
                        rand.Valoare = sum;
                        rand.Calculat = true;
                    }
                }

                var countOK = calcRows.Count(f => !f.Calculat);
                if (countOK == 0)
                {
                    calculatOk = true;
                }

                contor++;
            }

            if (!calculatOk)
            {
                throw new Exception("Nu am calculat totalurile pentru lichiditate. Verificati formulele pentru totaluri!");
            }

            foreach (var rand in calcRows.Where(f => f.RandTotal))
            {
                var lichidRandTotal = new LichidCalc
                {
                    LichidBenziId = rand.LichidBenziId,
                    LichidConfigId = rand.LichidConfigId,
                    SavedBalanceId = rand.SavedBalanceId,
                    Valoare = rand.Valoare,
                    TenantId = rand.TenantId,
                };

                Context.LichidCalc.Add(lichidRandTotal);
            }
            Context.SaveChanges();
        }

        private void DesfacFormula(string formula, out List<string> semneList, out List<string> splitItem)
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
    }

    public class LichidCalcDto
    {
        public int LichidBenziId { get; set; }
        public int LichidConfigId { get; set; }
        public decimal Valoare { get; set; }
        public string CodRand { get; set; }
        public int SavedBalanceId { get; set; }
        public int TenantId { get; set; }
        public bool Calculat { get; set; }
        public string Formula { get; set; }
        public bool RandTotal { get; set; }
    }

    public class LichidCalcCurrDto
    {
        public int LichidBenziCurrId { get; set; }  
        public int LichidConfigId { get; set; }
        public decimal Valoare { get; set; }
        public string CodRand { get; set; }
        public int SavedBalanceId { get; set; }
        public int TenantId { get; set; }
        public bool Calculat { get; set; }
        public string Formula { get; set; }
        public bool RandTotal { get; set; }
    }
}
