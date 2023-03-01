using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Niva.EntityFramework.Repositories.Nomenclatures;
using Niva.Erp.ModelObjects;
using Niva.Erp.Models.Conta;
using Niva.Erp.Repositories.Conta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abp.ObjectMapping;

namespace Niva.Erp.EntityFrameworkCore.Repositories.Conta
{
    public class SavedBalanceRepository : ErpRepositoryBase<SavedBalance, int>, ISavedBalanceRepository
    {
        AccountRepository _accountRepository;
        BalanceRepository _balanceRepository;
        ExchangeRatesRepository _exchangeRatesRepository;

        public SavedBalanceRepository(IDbContextProvider<ErpDbContext> context) : base(context)
        {
            _accountRepository = new AccountRepository(context);
            _balanceRepository = new BalanceRepository(context);
            _exchangeRatesRepository = new ExchangeRatesRepository(context);
        }
        public SavedBalance AddSavedBalance(Balance balance, string name, bool balantaZilnica)
        {
            var localCurrencyId = Context.Tenants.FirstOrDefault(f => f.Id == 1).LocalCurrencyId.Value;

            var savedBalance = new SavedBalance();
            savedBalance.BalanceName = name;
            savedBalance.SaveDate = balance.BalanceDate;
            savedBalance.IsDaily = balantaZilnica;

            if (Context.SavedBalance.Where(x => x.SaveDate == balance.BalanceDate && x.BalanceName == name).Count() > 0)
                throw new Exception("Exista o alta balanta salvata cu aceasta denumire pentru aceeasi data");

            var balanceTemp = balance.BalanceDetails.Select(f => new SavedBalanceTempDto
            {
                AccountId = f.AccountId,
                DbI = f.DbValueI,
                CrI = f.CrValueI,
                DbM = f.DbValueM,
                CrM = f.CrValueM,
                DbY = f.DbValueY,
                CrY = f.CrValueY,
                DbF = f.DbValueF,
                CrF = f.CrValueF,
                CurrencyId = f.CurrencyId,
                ExchangeRate = 1
            }).ToList();

            foreach (var currency in balanceTemp.Where(f => f.CurrencyId != localCurrencyId).Select(f => f.CurrencyId).Distinct())
            {
                var exchangeRate = _exchangeRatesRepository.GetExchangeRate(savedBalance.SaveDate, currency, localCurrencyId);
                foreach (var item in balanceTemp.Where(f => f.CurrencyId == currency))
                {
                    item.DbI = Math.Round(item.DbI * exchangeRate, 2);
                    item.CrI = Math.Round(item.CrI * exchangeRate, 2);
                    item.DbM = Math.Round(item.DbM * exchangeRate, 2);
                    item.CrM = Math.Round(item.CrM * exchangeRate, 2);
                    item.DbY = Math.Round(item.DbY * exchangeRate, 2);
                    item.CrY = Math.Round(item.CrY * exchangeRate, 2);
                    item.DbF = Math.Round(item.DbF * exchangeRate, 2);
                    item.CrF = Math.Round(item.CrF * exchangeRate, 2);
                    item.ExchangeRate = exchangeRate;
                }

            }

            savedBalance.SavedBalanceDetails = new List<SavedBalanceDetails>();

            foreach (var b in balanceTemp)
            {
                savedBalance.SavedBalanceDetails.Add(new SavedBalanceDetails
                {
                    AccountId = b.AccountId,
                    CrValueF = b.CrF,
                    CrValueI = b.CrI,
                    CrValueM = b.CrM,
                    CrValueY = b.CrY,
                    CurrencyId = b.CurrencyId,
                    DbValueF = b.DbF,
                    DbValueI = b.DbI,
                    DbValueM = b.DbM,
                    DbValueY = b.DbY,
                    ExhangeRate = b.ExchangeRate
                });
            }
            //var status = SaveBalanceWebService(balance);
            //if (!status.Success)
            //    return StatusConta.FromCode(ErrorCodeConta.WebServiceFailed);

            //OriginalBalance.SavedBalance = new List<SavedBalance>();
            //OriginalBalance.SavedBalance.Add(balance);

            Context.SavedBalance.Add(savedBalance);
            Context.SaveChanges();

            // save balance to SavedBalanceDetailsCurrencys table
            SavedBalanceDetCurrency(balance, localCurrencyId, savedBalance.Id);

            // salvez balance in tabela SavedBalanceViewDet => moneda RON si echivalent RON
            AddToSavedBalanceDetView(balance, savedBalance.Id, localCurrencyId);

            return savedBalance;
        }
        //public SavedBalance AddSavedBalance(int id, string Name, bool externalSave)
        //{
        //    var OriginalBalance = _balanceRepository.GetBalanceById(id);
        //    var localCurrencyId = Context.Tenants.FirstOrDefault(f => f.Id == 1).LocalCurrencyId.Value;

        //    var savedBalance = new SavedBalance();
        //    savedBalance.BalanceName = Name;
        //    savedBalance.SaveDate = OriginalBalance.BalanceDate;
        //    savedBalance.ExternalSave = externalSave;

        //    if (Context.SavedBalance.Where(x => x.SaveDate == OriginalBalance.BalanceDate && x.BalanceName == Name).Count() > 0)
        //        throw new Exception("Exista o alta balanta salvata cu aceasta denumire pentru aceeasi data");

        //    var balanceTemp = OriginalBalance.BalanceDetails.Select(f => new SavedBalanceTempDto
        //    {
        //        AccountId = f.AccountId,
        //        DbI = f.DbValueI,
        //        CrI = f.CrValueI,
        //        DbM = f.DbValueM,
        //        CrM = f.CrValueM,
        //        DbY = f.DbValueY,
        //        CrY = f.CrValueY,
        //        DbF = f.DbValueF,
        //        CrF = f.CrValueF,
        //        CurrencyId = f.CurrencyId,
        //        ExchangeRate = 1
        //    }).ToList();

        //    foreach (var currency in balanceTemp.Where(f => f.CurrencyId != localCurrencyId).Select(f => f.CurrencyId).Distinct())
        //    {
        //        var exchangeRate = _exchangeRatesRepository.GetExchangeRate(savedBalance.SaveDate, currency, localCurrencyId);
        //        foreach (var item in balanceTemp.Where(f => f.CurrencyId == currency))
        //        {
        //            item.DbI = Math.Round(item.DbI * exchangeRate, 2);
        //            item.CrI = Math.Round(item.CrI * exchangeRate, 2);
        //            item.DbM = Math.Round(item.DbM * exchangeRate, 2);
        //            item.CrM = Math.Round(item.CrM * exchangeRate, 2);
        //            item.DbY = Math.Round(item.DbY * exchangeRate, 2);
        //            item.CrY = Math.Round(item.CrY * exchangeRate, 2);
        //            item.DbF = Math.Round(item.DbF * exchangeRate, 2);
        //            item.CrF = Math.Round(item.CrF * exchangeRate, 2);
        //            item.ExchangeRate = exchangeRate;
        //        }

        //    }

        //    savedBalance.SavedBalanceDetails = new List<SavedBalanceDetails>();

        //    foreach (var b in balanceTemp)
        //    {
        //        savedBalance.SavedBalanceDetails.Add(new SavedBalanceDetails
        //        {
        //            AccountId = b.AccountId,
        //            CrValueF = b.CrF,
        //            CrValueI = b.CrI,
        //            CrValueM = b.CrM,
        //            CrValueY = b.CrY,
        //            CurrencyId = b.CurrencyId,
        //            DbValueF = b.DbF,
        //            DbValueI = b.DbI,
        //            DbValueM = b.DbM,
        //            DbValueY = b.DbY,
        //            ExhangeRate = b.ExchangeRate
        //        });
        //    }
        //    //var status = SaveBalanceWebService(balance);
        //    //if (!status.Success)
        //    //    return StatusConta.FromCode(ErrorCodeConta.WebServiceFailed);

        //    //OriginalBalance.SavedBalance = new List<SavedBalance>();
        //    //OriginalBalance.SavedBalance.Add(balance);

        //    Context.SavedBalance.Add(savedBalance);
        //    Context.SaveChanges();

        //    // save balance to SavedBalanceDetailsCurrencys table
        //    SavedBalanceDetCurrency(OriginalBalance, localCurrencyId, savedBalance.Id);

        //    // salvez balance in tabela SavedBalanceViewDet => moneda RON si echivalent RON
        //    AddToSavedBalanceDetView(OriginalBalance, savedBalance.Id, localCurrencyId);

        //    return savedBalance;
        //}

        private void AddToSavedBalanceDetView(Balance originalBalance, int savedBalanceId, int localCurrencyId)
        {
            try
            {
                AddToSavedBalanceDetViewRonEchiv(originalBalance, savedBalanceId, CurrencyType.RonEchivalentRon, localCurrencyId);
                AddToSavedBalanceDetViewRon(originalBalance, savedBalanceId, localCurrencyId);
                AddToSavedBalanceDetViewValuta(originalBalance, savedBalanceId, CurrencyType.Valuta, localCurrencyId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void AddToSavedBalanceDetViewRon(Balance originalBalance, int savedBalanceId, int localCurrencyId)
        {
            var balView = _balanceRepository.ConvertBalanceToType(originalBalance, "", localCurrencyId, false, true, localCurrencyId);
            Context.SavedBalanceViewDet.AddRange(balView.Details
                                                 .Select(f => new SavedBalanceViewDet
                                                 {
                                                     AccountId = f.AccountId,
                                                     SavedBalanceId = savedBalanceId,
                                                     Cont = f.Symbol,
                                                     NivelRand = f.NivelRand,
                                                     Denumire = f.Name,
                                                     CurrencyId = f.CurrencyId,
                                                     CrValueF = f.CrValueF,
                                                     CrValueI = f.CrValueI,
                                                     CrValueM = f.CrValueM,
                                                     CrValueP = f.CrValueP,
                                                     CrValueY = f.CrValueY,
                                                     DbValueF = f.DbValueF,
                                                     DbValueI = f.DbValueI,
                                                     DbValueM = f.DbValueM,
                                                     DbValueP = f.DbValueP,
                                                     DbValueY = f.DbValueY,
                                                     IsSynthetic = f.IsSynthetic,
                                                     IsConverted = false,
                                                     TenantId = 1
                                                 })
                                                 .ToList());
            Context.SaveChanges();
        }

        private void AddToSavedBalanceDetViewValuta(Balance originalBalance, int savedBalanceId, CurrencyType currencyTypeId, int localCurrencyId)
        {
            bool convertToLocalCurrency = (currencyTypeId != 0);
            bool convertAllCurrencies = (currencyTypeId == 0);

            foreach (var item in originalBalance.BalanceDetails.Where(f => f.CurrencyId != localCurrencyId).Select(f => f.CurrencyId).Distinct())
            {
                var balView = _balanceRepository.ConvertBalanceToType(originalBalance, "", item, convertToLocalCurrency, convertAllCurrencies, localCurrencyId);

                var ret = balView.Details.Select(f => new SavedBalanceViewDet
                {
                    AccountId = f.AccountId,
                    SavedBalanceId = savedBalanceId,
                    Cont = f.Symbol,
                    NivelRand = f.NivelRand,
                    Denumire = f.Name,
                    CurrencyId = f.CurrencyId,
                    CrValueF = f.CrValueF,
                    CrValueI = f.CrValueI,
                    CrValueM = f.CrValueM,
                    CrValueP = f.CrValueP,
                    CrValueY = f.CrValueY,
                    DbValueF = f.DbValueF,
                    DbValueI = f.DbValueI,
                    DbValueM = f.DbValueM,
                    DbValueP = f.DbValueP,
                    DbValueY = f.DbValueY,
                    IsSynthetic = f.IsSynthetic,
                    IsConverted = (currencyTypeId == 0) ? true : false,
                    TenantId = 1
                })
                .ToList();

                Context.SavedBalanceViewDet.AddRange(ret);
                Context.SaveChanges();
            }
        }

        private void AddToSavedBalanceDetViewRonEchiv(Balance originalBalance, int savedBalanceId, CurrencyType currencyTypeId, int localCurrencyId)
        {
            bool convertAllCurrencies = (currencyTypeId == 0);
            var balView = _balanceRepository.ConvertBalanceToType(originalBalance, "", (int)currencyTypeId, false, convertAllCurrencies, localCurrencyId);
            Context.SavedBalanceViewDet.AddRange(balView.Details
                                                 .Select(f => new SavedBalanceViewDet
                                                 {
                                                     AccountId = f.AccountId,
                                                     SavedBalanceId = savedBalanceId,
                                                     Cont = f.Symbol,
                                                     NivelRand = f.NivelRand,
                                                     Denumire = f.Name,
                                                     CurrencyId = f.CurrencyId,
                                                     CrValueF = f.CrValueF,
                                                     CrValueI = f.CrValueI,
                                                     CrValueM = f.CrValueM,
                                                     CrValueP = f.CrValueP,
                                                     CrValueY = f.CrValueY,
                                                     DbValueF = f.DbValueF,
                                                     DbValueI = f.DbValueI,
                                                     DbValueM = f.DbValueM,
                                                     DbValueP = f.DbValueP,
                                                     DbValueY = f.DbValueY,
                                                     IsSynthetic = f.IsSynthetic,
                                                     IsConverted = (currencyTypeId == 0) ? true : false,
                                                     TenantId = 1
                                                 })
                                                 .ToList());
            Context.SaveChanges();
        }

        private void SavedBalanceDetCurrency(Balance originalBalance, int localCurrencyId, int savedBalanceId)
        {
            var savedBalanceDetCurr = new List<SavedBalanceDetailsCurrency>();

            savedBalanceDetCurr = originalBalance.BalanceDetails.Select(f => new SavedBalanceDetailsCurrency
            {
                AccountId = f.AccountId,
                CrValueF = f.CrValueF,
                DbValueF = f.DbValueF,
                CrValueM = f.CrValueM,
                DbValueM = f.DbValueM,
                CrValueY = f.CrValueY,
                DbValueY = f.DbValueY,
                CrValueI = f.CrValueI,
                DbValueI = f.DbValueI,
                CurrencyId = f.CurrencyId,
                SavedBalanceId = savedBalanceId
            }).ToList();

            Context.SavedBalanceDetailsCurrencies.AddRange(savedBalanceDetCurr);
            Context.SaveChanges();
        }

        public SavedBalance GetSavedBalanceById(int id)
        {
            var balanceList = Context.SavedBalance.Include(f => f.SavedBalanceDetails).FirstOrDefault(f => f.Id == id);

            return balanceList;
        }

        public BalanceView GetBalanceDetails(int balanceId, Boolean addTotals, string searchData, int idCurrency, int? nivelRand)
        {
            var balance = GetSavedBalanceById(balanceId);
            var ret = ConvertSavedBalance(balance, addTotals, searchData, idCurrency);

            if (nivelRand != null)
            {
                ret.Details = ret.Details.Where(f => f.NivelRand <= nivelRand).ToList();
            }
            return ret;
        }

        public BalanceView ConvertSavedBalance(SavedBalance b, Boolean AddTotals, string SearchData, int IdCurrency)
        {
            var a = new BalanceView();
            var det = new List<BalanceDetailsView>();

            a.BalanceDate = b.SaveDate;
            a.StartDate = b.SaveDate;
            a.BalanceName = b.BalanceName;
            a.Id = b.Id;

            var tenant = Context.Tenants.FirstOrDefault(f => f.Id == b.TenantId);
            var person = Context.Persons.Where(x => x.Id == tenant.LegalPersonId).FirstOrDefault();
            a.AppClientId1 = person.Id1;
            a.AppClientId2 = person.Id2;
            a.AppClientName = person.FullName;

            var list = new List<SavedBalanceDetails>();
            var rows = b.SavedBalanceDetails.Count();
            var accountList = _accountRepository.AccountList().ToArray();



            list = b.SavedBalanceDetails/*.Where(x => x.CurrencyId == IdCurrency)*/.ToList();
            if (SearchData != "")
                list = list.Where(x => x.Account.Symbol.Contains(SearchData)).ToList();

            //Group by synthetic
            var details = list.GroupBy(p => new { p.AccountId, p.Account.Symbol, p.CurrencyId }).Select(g =>
                             new
                             {
                                 DbI = g.Sum(x => x.DbValueI),
                                 CrI = g.Sum(x => x.CrValueI),
                                 DbM = g.Sum(x => x.DbValueM),
                                 CrM = g.Sum(x => x.CrValueM),
                                 DbY = g.Sum(x => x.DbValueY),
                                 CrY = g.Sum(x => x.CrValueY),
                                 DbF = g.Sum(x => x.DbValueF),
                                 CrF = g.Sum(x => x.CrValueF),
                                 AccountId = g.Key.AccountId,
                                 Symbol = g.Key.Symbol,
                                 CurrencyId = g.Key.CurrencyId
                             }).ToArray();


            var count = details.Count();


            for (int i = 0; i < count; ++i)
            {
                var row = new BalanceDetailsView();
                var account = _balanceRepository.GetAccountBySymbol(details[i].Symbol, accountList.ToList());

                row.Symbol = account.Symbol;
                row.Name = account.AccountName;
                row.NivelRand = account.NivelRand;

                row.DbValueI = details[i].DbI;
                row.CrValueI = details[i].CrI;

                _balanceRepository.OrganizeSolds(row, account.AccountTypes, true);

                row.CrValueM = details[i].CrM;
                row.CrValueY = details[i].CrY;
                row.CurrencyId = details[i].CurrencyId;
                row.DbValueM = details[i].DbM;
                row.DbValueY = details[i].DbY;

                _balanceRepository.OrganizeSolds(row, account.AccountTypes, false);

                row.Id = i;
                //row.Synthetic = details[i].Synthetic;
                row.accountType = account.AccountTypes;

                if (Math.Abs(row.CrValueI) + Math.Abs(row.DbValueI) + Math.Abs(row.CrValueM) + Math.Abs(row.DbValueM) + Math.Abs(row.CrValueY) + Math.Abs(row.CrValueY) +
                    Math.Abs(row.CrValueF) + Math.Abs(row.DbValueF) > 0)
                    det.Add(row);


            }

            a.Details = det.OrderBy(x => x.Symbol).ToList();
            // a.Type = type;

            #region Totals
            if (AddTotals)
            {
                //adding subtotals
                if (a.Type == BalanceType.Analythic)
                {
                    var subTotals = a.Details.GroupBy(p => new { p.Synthetic, p.CurrencyId/*, p.accountType*/ }).Select(g =>
                             new
                             {
                                 DbI = g.Sum(x => x.DbValueI),
                                 CrI = g.Sum(x => x.CrValueI),
                                 DbM = g.Sum(x => x.DbValueM),
                                 CrM = g.Sum(x => x.CrValueM),
                                 DbY = g.Sum(x => x.DbValueY),
                                 CrY = g.Sum(x => x.CrValueY),
                                 DbF = g.Sum(x => x.DbValueF),
                                 CrF = g.Sum(x => x.CrValueF),
                                 Symbol = g.Key.Synthetic,
                                 CurrencyId = g.Key.CurrencyId,
                                 Name = g.Key.Synthetic//,
                                 //AccountTypes = g.Key.accountType
                             });

                    foreach (var d in subTotals)
                    {
                        var syntheticAccount = accountList.FirstOrDefault(f => f.Symbol == d.Symbol);

                        a.Details.Add(new BalanceDetailsView
                        {
                            Name = d.Name,
                            CrValueF = d.CrF,
                            CrValueI = d.CrI,
                            CrValueM = d.CrM,
                            CrValueY = d.CrY,
                            DbValueF = d.DbF,
                            DbValueI = d.DbI,
                            DbValueM = d.DbM,
                            DbValueY = d.DbY,
                            Synthetic = d.Symbol,
                            Symbol = d.Symbol,
                            TotalSum = true,
                            accountType = syntheticAccount.AccountTypes
                        });
                    }

                    foreach (var d in a.Details.Where(x => x.TotalSum))
                    {
                        _balanceRepository.OrganizeSolds(d, d.accountType, true);

                        _balanceRepository.OrganizeSolds(d, d.accountType, false);
                    }
                }


                ///Adding general total
                var Total = a.Details.Where(x => x.TotalSum == (a.Type == BalanceType.Analythic ? true : false)).GroupBy(x => x.accountType).Select(g => new
                {
                    DbI = g.Sum(x => x.DbValueI),
                    CrI = g.Sum(x => x.CrValueI),
                    DbM = g.Sum(x => x.DbValueM),
                    CrM = g.Sum(x => x.CrValueM),
                    DbY = g.Sum(x => x.DbValueY),
                    CrY = g.Sum(x => x.CrValueY),
                    DbF = g.Sum(x => x.DbValueF),
                    CrF = g.Sum(x => x.CrValueF),
                    Name = "TOTAL GENERAL "
                }).ToArray()[0];

                a.Details.Add(new BalanceDetailsView
                {
                    Name = Total.Name,
                    CrValueF = Total.CrF,
                    CrValueI = Total.CrI,
                    CrValueM = Total.CrM,
                    CrValueY = Total.CrY,
                    DbValueF = Total.DbF,
                    DbValueI = Total.DbI,
                    DbValueM = Total.DbM,
                    DbValueY = Total.DbY,
                    Synthetic = "99999999999999",
                    Symbol = Total.Name,
                    TotalSum = true
                });
            }

            #endregion

            a.Details = a.Details.OrderBy(x => x.Synthetic).ThenByDescending(x => x.TotalSum).ThenBy(x => x.Symbol).ToList();

            return a;
        }

        public void DeleteSavedBalance(int id)
        {
            try
            {
                // verific daca pot sa sterg
                // sit finan
                var count = Context.SitFinanCalc.Count(f => f.SavedBalanceId == id);
                if (count != 0)
                {
                    throw new Exception("Nu puteti sa stergeti aceasta balanta. Este folosita la calculul situatiilor financiare");
                }
                // bnr
                count = Context.BNR_Conturi.Count(f => f.SavedBalanceId == id);
                if (count != 0)
                {
                    throw new Exception("Nu puteti sa stergeti aceasta balanta. Este folosita la calculul raportarii BNR");
                }
                // bnr
                count = Context.BNR_Raportare.Count(f => f.SavedBalanceId == id);
                if (count != 0)
                {
                    throw new Exception("Nu puteti sa stergeti aceasta balanta. Este folosita la calculul raportarii BNR");
                }
                // bvc realizat
                count = Context.BVC_Realizat.Count(f => f.SavedBalanceId == id);
                if (count != 0)
                {
                    throw new Exception("Nu puteti sa stergeti aceasta balanta. Este folosita la calculul BVC-ului realizat");
                }


                var balance = Context.SavedBalance.Include(f => f.SavedBalanceDetails).FirstOrDefault(f => f.Id == id);
                Context.SavedBalanceDetails.RemoveRange(balance.SavedBalanceDetails);
                Context.SavedBalance.Remove(balance);
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SavedBalanceViewDet> GetSavedBalanceViewDetByNivelRand(int savedBalanceId, int? nivelRand)
        {
            var savedBalanceViewDetList = new List<SavedBalanceViewDet>();

            if (nivelRand == null)
            {
                savedBalanceViewDetList = Context.SavedBalanceViewDet.Include(f => f.Currency).Include(f => f.SavedBalance)
                                    .Where(f => f.SavedBalanceId == savedBalanceId && f.IsConverted == true)
                                    .OrderBy(f => f.Cont)
                                    .ThenByDescending(f => f.IsSynthetic)
                                    .ToList();
            }
            else
            {
                savedBalanceViewDetList = Context.SavedBalanceViewDet.Include(f => f.Currency).Include(f => f.SavedBalance)
                                    .Where(f => f.SavedBalanceId == savedBalanceId && f.NivelRand <= nivelRand && f.IsConverted == true)
                                    .OrderBy(f => f.Cont)
                                    .ThenByDescending(f => f.IsSynthetic)
                                    .ToList();
            }
            return savedBalanceViewDetList;
        }
    }

    public class SavedBalanceTempDto
    {
        public int AccountId { get; set; }
        public virtual decimal DbI { get; set; }
        public virtual decimal CrI { get; set; }
        public virtual decimal DbM { get; set; }
        public virtual decimal CrM { get; set; }
        public virtual decimal DbY { get; set; }
        public virtual decimal CrY { get; set; }
        public virtual decimal DbF { get; set; }
        public virtual decimal CrF { get; set; }
        public int CurrencyId { get; set; }
        public decimal ExchangeRate { get; set; }
    }

}

