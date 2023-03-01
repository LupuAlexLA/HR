using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.Conta.Lichiditate.Dto;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Managers.Conta;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.SectoareBnr;
using Niva.Erp.Repositories.Conta;
using Niva.Erp.Repositories.Conta.Lichiditate;
using Niva.Erp.Repositories.Conta.Nomenclatures;
using Niva.Erp.SectoareBnr.Dto;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Conta.Balance
{
    public interface ISavedBalanceAppService : IApplicationService
    {
        SavedBalanceDto InitBalanceForm();

        SavedBalanceDto SavedBalanceList(SavedBalanceDto balanceInit);

        SavedBalanceDto SaveBalance(SavedBalanceDto balanceInit);

        void DeleteBalance(int balanceId);

        SavedBalanceDto BalanceDetailsInit(int balanceId, SavedBalanceDto balanceInit, int currencyId);

        SavedBalanceDto BalanceDetails(SavedBalanceDto balanceInit);

        List<BNR_BalanceListDto> BalanceListForBNR();
        List<LichidSavedBalanceListDto> LichidSavedBalanceList();

    }

    public class SavedBalanceAppService : ErpAppServiceBase, ISavedBalanceAppService
    {
        ISavedBalanceRepository _savedBalanceRepository;
        IBalanceRepository _balanceRepository;
        IAccountRepository _accountRepository;
        IRepository<BNR_Conturi> _bnrSectorCalcRepository;
        IRepository<SavedBalanceDetails> _savedBalanceDetailsRepository;
        IRepository<SavedBalanceDetailsCurrency> _savedBalanceDetailsCurrencyRepository;
        BalanceManager _balanceManager;
        ILichidCalcRepository _lichidCalcRepository;

        public SavedBalanceAppService(ISavedBalanceRepository savedBalanceRepository, IBalanceRepository balanceRepository, IAccountRepository accountRepository,
                                      IRepository<BNR_Conturi> bnrSectorCalcRepository, IRepository<SavedBalanceDetails> savedBalanceDetailsRepository, IRepository<SavedBalanceDetailsCurrency> savedBalanceDetailsCurrencyRepository,
                                      BalanceManager balanceManager, ILichidCalcRepository lichidCalcRepository)
        {
            _savedBalanceRepository = savedBalanceRepository;
            _balanceRepository = balanceRepository;
            _accountRepository = accountRepository;
            _bnrSectorCalcRepository = bnrSectorCalcRepository;
            _savedBalanceDetailsRepository = savedBalanceDetailsRepository;
            _savedBalanceDetailsCurrencyRepository = savedBalanceDetailsCurrencyRepository;
            _balanceManager = balanceManager;
            _lichidCalcRepository = lichidCalcRepository;
        }

        //[AbpAuthorize("Conta.Balanta.BalanteSalvate.Acces")]
        public SavedBalanceDto InitBalanceForm()
        {
            try
            {
                var _currentDate = LazyMethods.Now();
                var ret = new SavedBalanceDto
                {
                    SearchStartDate = _currentDate.AddYears(-3),
                    SearchEndDate = _currentDate,
                    ExternalSave = false,
                    SavedBalanceForm = new SaveBalanceFormDto()
                };
                ret = SavedBalanceList(ret);

                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public SavedBalanceDto SavedBalanceList(SavedBalanceDto balanceInit)
        {
            try
            {
                var balanceList = _savedBalanceRepository.GetAll()
                                                    .Where(f => f.SaveDate >= balanceInit.SearchStartDate && f.SaveDate <= balanceInit.SearchEndDate && f.IsDaily == balanceInit.IsDaily)
                                                    .OrderByDescending(f => f.SaveDate)
                                                    .ToList()
                                                    .Select(f => new SavedBalanceListDto { Id = f.Id, BalanceDate = f.SaveDate.ToShortDateString(), BalanceName = f.BalanceName, OkDelete = true })
                                                    .ToList();

                try
                {
                    balanceList[0].OkDelete = true;
                }
                catch
                {

                }
                balanceInit.BalanceList = balanceList;
                return balanceInit;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        [AbpAuthorize("Conta.Balanta.BalanteSalvate.Modificare")]
        public SavedBalanceDto SaveBalance(SavedBalanceDto balanceInit)
        {
            try
            {
                Models.Conta.Balance computedBalance = new Models.Conta.Balance();
                var currencyId = GetCurrentTenant().LocalCurrencyId;
                var appClientId = GetCurrentTenant().Id;

                if (balanceInit.SavedBalanceForm.IsDaily)
                {
                    computedBalance = _balanceRepository.BalantaZilnicaCalc(balanceInit.SavedBalanceForm.DailyBalanceDate, appClientId);
                }
                else
                {
                    if (balanceInit.SavedBalanceForm.CalcBalanceId == 0)
                        throw new UserFriendlyException("Eroare", "Trebuie sa selectati o balanta");
                    if (balanceInit.SavedBalanceForm.BalanceName == "" || balanceInit.SavedBalanceForm.BalanceName == null)
                        throw new UserFriendlyException("Eroare", "Trebuie sa completati denumirea balantei");

                    computedBalance = _balanceRepository.GetAllIncluding(f => f.BalanceDetails).FirstOrDefault(f => f.Id == balanceInit.SavedBalanceForm.CalcBalanceId);
                }
                var savedBalance = _balanceManager.AddSavedBalance(computedBalance, balanceInit.SavedBalanceForm.BalanceName, balanceInit.SavedBalanceForm.IsDaily);

                if (balanceInit.ExternalSave)
                {
                    var details = ObjectMapper.Map<List<SavedBalanceItemDetailsDto>>(savedBalance.SavedBalanceDetails).Where(f => f.CurrencyId == currencyId).ToList();

                    var balance = new SavedBalanceItemDto
                    {
                        Id = savedBalance.Id,
                        BalanceName = savedBalance.BalanceName,
                        SaveDate = savedBalance.SaveDate,
                        ExternalSave = savedBalance.ExternalSave
                    };

                    var totals = details.GroupBy(p => new { p.Symbol, p.CurrencyId }).Select(g =>
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
                                Symbol = g.Key.Symbol,
                                CurrencyId = g.Key.CurrencyId,
                                NrInreg = g.Count()
                            }).ToArray();

                    var accountList = _accountRepository.AccountList().ToArray();

                    foreach (var item in totals.ToList())
                    {
                        int okSynthetic = 0; // inserez sinteticul
                        var count = details.Count(f => f.Symbol == item.Symbol);
                        okSynthetic = (count != 0) ? 0 : 1;

                        if (item.NrInreg != 1 || okSynthetic == 0)
                        {
                            var row = new SavedBalanceItemDetailsDto();
                            var account = _balanceRepository.GetAccountBySymbol(item.Symbol, accountList.ToList());

                            row.Symbol = account.Symbol;
                            row.AccountName = account.AccountName;

                            decimal debit = 0, credit = 0;
                            debit = (item.DbI - item.CrI > 0) ? (item.DbI - item.CrI) : 0;
                            credit = (item.DbI - item.CrI > 0) ? 0 : (item.CrI - item.DbI);

                            row.DbValueI = debit;
                            row.CrValueI = credit;

                            row.CrValueM = item.CrM;
                            row.CrValueY = item.CrY;
                            row.CurrencyId = item.CurrencyId;
                            row.DbValueM = item.DbM;
                            row.DbValueY = item.DbY;

                            debit = (item.DbF - item.CrF > 0) ? (item.DbF - item.CrF) : 0;
                            credit = (item.DbF - item.CrF > 0) ? 0 : (item.CrF - item.DbF);

                            row.DbValueF = debit;
                            row.CrValueF = credit;

                            details.Add(row);
                        }
                    }

                    balance.SavedBalanceDetails = details;

                    //SaveBalanceWebService(balance);
                }

                balanceInit = SavedBalanceList(balanceInit);
                return balanceInit;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        //public void SaveBalanceWebService(SavedBalanceItemDto savedbalance)
        //{
        //    var currencyId = _appClientRepository.FirstOrDefault(f => f.Id == selectedAppClientId).LocalCurrencyId;
        //    //integration
        //    var token = _tokenRepository.GetAll().FirstOrDefault(x => x.AppClient.Id == SelectedClientId && x.User.Id == UserManager.AbpSession.UserId);

        //    var webService = new wBalanceGestSoapClient();
        //    var balance = new List<Sif4WebService.Balance>();

        //    foreach (var b in savedbalance.SavedBalanceDetails)
        //    {
        //        balance.Add(new Sif4WebService.Balance
        //        {
        //            AccountSymbol = b.SyntheticAccount + "." + b.AnalyticAccount,
        //            AccountName = b.AccountName,
        //            CrValueF = b.CrValueF,
        //            CrValueI = b.CrValueI,
        //            CrValueM = b.CrValueM,
        //            CrValueY = b.CrValueY,
        //            DbValueF = b.DbValueF,
        //            DbValueI = b.DbValueI,
        //            DbValueM = b.DbValueM,
        //            DbValueY = b.DbValueY
        //        });
        //    }

        //    var ccc = webService.SaveBalance(token.Token, savedbalance.SaveDate, savedbalance.BalanceName, balance);
        //    if (!ccc.Succes)
        //    {
        //        throw new Exception(ccc.Message);
        //    }


        //}

        [AbpAuthorize("Conta.Balanta.BalanteSalvate.Modificare")]
        public void DeleteBalance(int balanceId)
        {
            try
            {
                var balance = _savedBalanceRepository.GetAll().FirstOrDefault(f => f.Id == balanceId);

                _savedBalanceRepository.DeleteSavedBalance(balanceId);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public SavedBalanceDto BalanceDetailsInit(int balanceId, SavedBalanceDto balanceInit, int currencyId)
        {
            try
            {
                var balanceDetailForm = new ViewSavedBalanceDetailDto
                {
                    Id = balanceId,
                    //BalanceTypeStr = _balanceType.ToString(),
                    //BalanceType = _balanceType,
                    CurrencyId = currencyId,
                    SearchAccount = ""
                };
                balanceInit.ViewBalanceDetail = balanceDetailForm;
                balanceInit = BalanceDetails(balanceInit);
                balanceInit.ShowBalanceDetails = true;
                return balanceInit;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public SavedBalanceDto BalanceDetails(SavedBalanceDto balanceInit)
        {
            try
            {
                var _balanceType = balanceInit.ViewBalanceDetail.BalanceType;
                var savedBalance = _savedBalanceRepository.FirstOrDefault(f => f.Id == balanceInit.ViewBalanceDetail.Id);
                var savedBalanceList = new List<BalanceDetailDto>();


                if (balanceInit.ViewBalanceDetail.CurrencyId == 0) // RON si Echivalent RON
                {
                    savedBalanceList = _savedBalanceDetailsRepository.GetAllIncluding(f => f.SavedBalance, f => f.Currency, f => f.Account)
                                                                                .Where(f => f.SavedBalanceId == savedBalance.Id)
                                                                                .OrderBy(f => f.Account.Symbol)
                                                                                .Select(f => new BalanceDetailDto
                                                                                {
                                                                                    CrValueI = f.CrValueI,
                                                                                    DbValueI = f.DbValueI,
                                                                                    CrValueM = f.CrValueM,
                                                                                    DbValueM = f.DbValueM,
                                                                                    CrValueY = f.CrValueF,
                                                                                    DbValueY = f.DbValueY,
                                                                                    CrValueF = f.CrValueF,
                                                                                    DbValueF = f.DbValueF,
                                                                                    Id = f.Id,
                                                                                    Symbol = f.Account.Symbol,
                                                                                    Name = f.Account.AccountName,
                                                                                    CurrencyId = f.CurrencyId,
                                                                                    NivelRand = f.Account.NivelRand
                                                                                })
                                                                                .ToList();
                }
                else
                {
                    savedBalanceList = _savedBalanceDetailsCurrencyRepository.GetAllIncluding(f => f.Account, f => f.Currency, f => f.SavedBalance)
                                                               .Where(f => f.SavedBalanceId == balanceInit.ViewBalanceDetail.Id && f.CurrencyId == balanceInit.ViewBalanceDetail.CurrencyId)
                                                               .OrderBy(f => f.Account.Symbol)
                                                               .Select(f => new BalanceDetailDto
                                                               {
                                                                   CrValueI = f.CrValueI,
                                                                   DbValueI = f.DbValueI,
                                                                   CrValueM = f.CrValueM,
                                                                   DbValueM = f.DbValueM,
                                                                   CrValueY = f.CrValueF,
                                                                   DbValueY = f.DbValueY,
                                                                   CrValueF = f.CrValueF,
                                                                   DbValueF = f.DbValueF,
                                                                   Id = f.Id,
                                                                   Symbol = f.Account.Symbol,
                                                                   Name = f.Account.AccountName,
                                                                   CurrencyId = f.CurrencyId,
                                                                   NivelRand = f.Account.NivelRand
                                                               })
                                                                .ToList();
                }
                balanceInit.ViewBalanceDetail.BalanceDetail = savedBalanceList;

                if (balanceInit.ViewBalanceDetail.NivelRand != null)
                {
                    balanceInit.ViewBalanceDetail.BalanceDetail = savedBalanceList.Where(f => f.NivelRand <= balanceInit.ViewBalanceDetail.NivelRand).ToList();
                }

                if (balanceInit.ViewBalanceDetail.SearchAccount != null && balanceInit.ViewBalanceDetail.SearchAccount != "")
                {
                    balanceInit.ViewBalanceDetail.BalanceDetail = savedBalanceList.Where(f => f.Symbol.IndexOf(balanceInit.ViewBalanceDetail.SearchAccount) >= 0).ToList();
                }

                balanceInit.ViewBalanceDetail.BalanceDate = savedBalance.SaveDate.ToShortDateString();
                balanceInit.ViewBalanceDetail.BalanceName = savedBalance.BalanceName;

                return balanceInit;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        // Returnez balantele necalculate pentru BNR_Conturi
        public List<BNR_BalanceListDto> BalanceListForBNR()
        {
            try
            {
                var calculatedBalanceIds = _bnrSectorCalcRepository.GetAllIncluding(f => f.SavedBalance).Select(f => f.SavedBalanceId);
                var balanceList = _savedBalanceRepository.GetAll().Where(f => !calculatedBalanceIds.Contains(f.Id) && f.IsDaily == false)
                                                         .ToList()
                                                         .Select(f => new BNR_BalanceListDto
                                                         {
                                                             Id = f.Id,
                                                             BalanceDate = f.SaveDate,
                                                             BalanceDesc = LazyMethods.DateToString(f.SaveDate) + " - " + f.BalanceName,
                                                             TenantId = f.TenantId
                                                         }).Distinct().ToList().OrderByDescending(f => f.BalanceDate).ToList();

                return balanceList;
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public List<LichidSavedBalanceListDto> LichidSavedBalanceList()
        {
            try
            {
                var calculatedSavedBalanceIds = _lichidCalcRepository.GetAllIncluding(f => f.SavedBalance).Select(f => f.SavedBalanceId).Distinct().ToList();
                var savedBalanceList = _savedBalanceRepository.GetAll().Where(f => !calculatedSavedBalanceIds.Contains(f.Id) && f.IsDaily == false)
                                                         .ToList()
                                                         .Select(f => new LichidSavedBalanceListDto
                                                         {
                                                             Id = f.Id,
                                                             SavedBalanceDate = f.SaveDate,
                                                             SavedBalanceDesc = LazyMethods.DateToString(f.SaveDate) + " - " + f.BalanceName,
                                                             TenantId = f.TenantId
                                                         }).Distinct().ToList().OrderByDescending(f => f.SavedBalanceDate).ToList();

                return savedBalanceList;
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }
    }
}
