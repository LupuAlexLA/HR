using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.Conta.Lichiditate.Dto;
using Niva.Erp.ExternalApi;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Conta.Lichiditate;
using Niva.Erp.Repositories;
using Niva.Erp.Repositories.Conta;
using Niva.Erp.Repositories.Conta.Lichiditate;
using Niva.Erp.Repositories.Conta.Nomenclatures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Conta.Lichiditate
{
    public interface ILichidCalAppService : IApplicationService
    {
        void LichidCalc(int savedBalanceId);
        LichidCalcInitDto GetLichidCalcList();
        List<LichidCalcListDetDto> GetLichidCalcDetList(int savedBalanceId);
        void DeleteLichidCalc(int savedBalanceId);
        List<LichidBenziDto> GetLichidBenziList();
        List<LichidCalcDetDto> GetLichidCalcFormulaDet(int columnId, int savedBalanceId, int rowId);
        List<LichidCalcListDetDto> UpdateLichidCalc(List<LichidCalcListDetDto> lichidCalcDet, int savedBalanceId);
        void RecalculLichiditate(int savedBalanceId);
    }


    public class LichidCalcAppService : ErpAppServiceBase, ILichidCalAppService
    {
        ILichidCalcRepository _lichidCalcRepository;
        IRepository<LichidCalcDet> _lichidCalcDetRepository;
        IRepository<LichidBenzi> _lichidBenziRepository;
        IRepository<LichidConfig> _lichidConfigRepository;
        IPlasamentLichiditateManager _plasamentLichiditateManager;
        ISavedBalanceRepository _savedBalanceRepository;
        ICurrencyRepository _currencyRepository;
        IExchangeRatesRepository _exchangeRatesRepository;

        public LichidCalcAppService(ILichidCalcRepository lichidCalcRepository, IRepository<LichidCalcDet> lichidCaldDetRepository, 
                                    IRepository<LichidBenzi> lichidBenziRepository, IRepository<LichidConfig> lichidConfigRepository,
                                    IPlasamentLichiditateManager plasamentLichiditateManager, ISavedBalanceRepository savedBalanceRepository,
                                    ICurrencyRepository currencyRepository, IExchangeRatesRepository exchangeRatesRepository)
        {
            _lichidCalcRepository = lichidCalcRepository;
            _lichidCalcDetRepository = lichidCaldDetRepository;
            _lichidBenziRepository = lichidBenziRepository;
            _lichidConfigRepository = lichidConfigRepository;
            _plasamentLichiditateManager = plasamentLichiditateManager;
            _savedBalanceRepository = savedBalanceRepository;
            _currencyRepository = currencyRepository;
            _exchangeRatesRepository = exchangeRatesRepository;
        }

        // Sterg calculul in finctie de balanta salvata selectata
        public void DeleteLichidCalc(int savedBalanceId)
        {
            try
            {
                // sterg calculele din LichidCalc
                _lichidCalcRepository.DeleteLichidCalc(savedBalanceId);

                // sterg calculele din LichidCalcCurr
                _lichidCalcRepository.DeleteLichidCalcCurr(savedBalanceId);

                CurrentUnitOfWork.SaveChanges();
            }
            catch (System.Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex);
            }
        }

        // Returnez lista benzilor pentru a construi tabelul detaliilor in mod dinamic
        public List<LichidBenziDto> GetLichidBenziList()
        {
            try
            {
                var lichidBenziList = _lichidBenziRepository.GetAll()
                                                            .Where(f => f.State == State.Active)
                                                            .Select(f => new LichidBenziDto
                                                            {
                                                                Id = f.Id,
                                                                Descriere = f.Descriere
                                                            })
                                                            .ToList();

                return lichidBenziList;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        // Returnez detaliile unui calcul in functie de balanta salvata
        public List<LichidCalcListDetDto> GetLichidCalcDetList(int savedBalanceId)
        {
            try
            {
                var lichidCalcDetList = new List<LichidCalcListDetDto>();

                var lichidCalcIdsList = _lichidCalcRepository.GetAllIncluding(f => f.LichidConfig).Where(f => f.SavedBalanceId == savedBalanceId).Select(f => f.LichidConfigId).Distinct().ToList();
                var lichidConfigList = _lichidConfigRepository.GetAllIncluding(f=>f.LichidBenzi).Where(f => lichidCalcIdsList.Contains(f.Id)).ToList();
                var lichidBenziRows = _lichidBenziRepository.GetAll().ToList();

                foreach (var conf in lichidConfigList)
                {

                    var lichidCalcDet = new LichidCalcListDetDto
                    {
                        Descriere = conf.DenumireRand,
                        LichidConfigId = conf.Id,
                        RandTotal = (conf.FormulaTotal != null && conf.FormulaTotal != "") ? true: false,
                        TenantId = conf.TenantId

                    };
                    foreach (var banda in lichidBenziRows)
                    {
                        var lichidCalcRow = _lichidCalcRepository.GetAllIncluding(f => f.LichidConfig, f => f.LichidBenzi, f => f.SavedBalance)
                                           .FirstOrDefault(f => f.SavedBalanceId == savedBalanceId && f.LichidConfigId == conf.Id && f.LichidBenziId == banda.Id);

                        switch (banda.DurataInLuniMaxima)
                        {
                            case 3:
                                lichidCalcDet.ValoareBanda1 = lichidCalcRow.Valoare;
                                break;
                            case 12:
                                lichidCalcDet.ValoareBanda2 = lichidCalcRow.Valoare;
                                break;
                            case 60:
                                lichidCalcDet.ValoareBanda3 = lichidCalcRow.Valoare;
                                break;
                            case 9999:
                                lichidCalcDet.ValoareBanda4 = lichidCalcRow.Valoare;
                                break;
                            case 0:
                                lichidCalcDet.ValoareBanda5 = lichidCalcRow.Valoare;
                                break;
                        }
                        lichidCalcDet.TotalInit += lichidCalcRow.Valoare;
                        lichidCalcDet.TotalActualiz += lichidCalcRow.Valoare;
                    }
                    lichidCalcDetList.Add(lichidCalcDet);
                }
                    return lichidCalcDetList;
            }
            catch (System.Exception ex)
            {
                throw new  UserFriendlyException("Eroare", ex);
            }
        }

        public List<LichidCalcDetDto> GetLichidCalcFormulaDet(int columnId, int savedBalanceId, int lichidCalcConfigId)
        {
            try
            {
                var lichidDetList = _lichidCalcDetRepository.GetAllIncluding(f => f.LichidCalc, f => f.LichidCalc.LichidConfig, f => f.LichidCalc.SavedBalance)
                                                            .Where(f => f.LichidCalc.LichidConfigId == lichidCalcConfigId && f.LichidCalc.SavedBalanceId == savedBalanceId && f.LichidCalc.LichidBenziId == columnId)
                                                            .Select(f => new LichidCalcDetDto
                                                            {
                                                                LichidCalcId = f.LichidCalcId,
                                                                Descriere = f.Descriere,
                                                                Valoare  = f.Valoare
                                                            })
                                                            .ToList();

                return lichidDetList;
            }
            catch (System.Exception ex)
            {

                throw ex;
            }
        }

        // Returnez lista lichiditatilor calculate
        public LichidCalcInitDto GetLichidCalcList()
        {
            try
            {
                var appClientId = GetCurrentTenant().Id;

                var list = _lichidCalcRepository.GetAllIncluding(f => f.SavedBalance, f => f.LichidConfig)
                                                .Where(f => f.TenantId == appClientId)
                                                .GroupBy(f => new { SavedBalanceId = f.SavedBalanceId, SavedBalanceDate = f.SavedBalance.SaveDate, SavedBalanceDesc = f.SavedBalance.BalanceName, TenantId = f.TenantId })
                                                .Select(f => new LichidCalcSavedBalancetDto
                                                {

                                                    SavedBalanceId = f.Key.SavedBalanceId,
                                                    SavedBalanceDate = f.Key.SavedBalanceDate,
                                                    SavedBalanceDesc = f.Key.SavedBalanceDesc,
                                                    TenantId = f.Key.TenantId
                                                })
                                                .Distinct()
                                                .OrderByDescending(f => f.SavedBalanceDate)
                                                .ToList();

                var lichidCalcInit = new LichidCalcInitDto
                {
                    ShowDetails = false,
                    ShowLichidCalcSavedBalanceList = true,
                    LichidCalcSavedBalanceList = list,
                    TenantId = appClientId
                };
                return lichidCalcInit;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        // Calculez lichiditatile pentru balanta salvata selectata
        public void LichidCalc(int savedBalanceId)
        {
            try
            {
                var tenant = GetCurrentTenant();
                var localCurrencyId = tenant.LocalCurrencyId.Value;
                var currency = _currencyRepository.GetCurrencyById(localCurrencyId);
                var savedBalance = _savedBalanceRepository.FirstOrDefault(f => f.Id == savedBalanceId);

                var plasamentLichiditateList = _plasamentLichiditateManager.PlasamenteLichiditateList(savedBalance.SaveDate).ToList();

                var plasamentCurrencyList = plasamentLichiditateList.Where(f => f.moneda != currency.CurrencyCode).Select(f => f.moneda).Distinct().ToList();

                foreach (var item in plasamentCurrencyList)
                {
                    var fromCurrency = _currencyRepository.GetByCode(item);
                    var exchangeRate = _exchangeRatesRepository.GetExchangeRate(savedBalance.SaveDate, fromCurrency.Id, localCurrencyId);
                    foreach (var plasament in plasamentLichiditateList.Where(f => f.moneda == item))
                    {
                        plasament.valoareContabila = Math.Round(plasament.valoareContabila * exchangeRate, 2);
                        plasament.valoareCreanta = Math.Round(plasament.valoareCreanta * exchangeRate, 2);
                        plasament.valoareInvestita = Math.Round(plasament.valoareInvestita * exchangeRate, 2);
                        plasament.valoareDepreciere = Math.Round(plasament.valoareDepreciere * exchangeRate, 2);
                    }
                }

                _lichidCalcRepository.LichidCalc(savedBalanceId, plasamentLichiditateList);
                _lichidCalcRepository.LichidCalcTotaluri(savedBalanceId);

                _lichidCalcRepository.LichidCalcCurr(savedBalanceId, plasamentLichiditateList);
                _lichidCalcRepository.LichidCalcCurrTotaluri(savedBalanceId);
            }
            catch (System.Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public void RecalculLichiditate(int savedBalanceId)
        {
            try
            {
              // Sterg lichiditatile curente pentru balanta primita ca parametru
                _lichidCalcRepository.DeleteLichidCalc(savedBalanceId);

                // Sterg lichiditatile curente in valuta pentru balanta primita ca parametru
                _lichidCalcRepository.DeleteLichidCalcCurr(savedBalanceId);

                // recalculez lichiditatile pentru balanta primita ca parametru
                LichidCalc(savedBalanceId);




            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex);
            }
        }

        public List<LichidCalcListDetDto> UpdateLichidCalc(List<LichidCalcListDetDto> lichidCalcDet, int savedBalanceId)
        {
            try
            {
                foreach (var item in lichidCalcDet)
                {

                    var lichidBenziList = _lichidBenziRepository.GetAll().ToList();

                    foreach (var banda in lichidBenziList)
                    {
                        var lichidCalc = _lichidCalcRepository.GetAllIncluding(f => f.SavedBalance).FirstOrDefault(f => f.SavedBalanceId == savedBalanceId && f.LichidConfigId == item.LichidConfigId && f.LichidBenziId == banda.Id);

                            switch (banda.DurataInLuniMaxima)
                            {
                                case 3:
                                    lichidCalc.Valoare = item.ValoareBanda1;
                                    break;
                                case 12:
                                    lichidCalc.Valoare = item.ValoareBanda2;
                                    break;
                                case 60:
                                    lichidCalc.Valoare = item.ValoareBanda3;
                                    break;
                                case 9999:
                                    lichidCalc.Valoare = item.ValoareBanda4;
                                    break;
                                case 0:
                                    lichidCalc.Valoare = item.ValoareBanda5;
                                    break;
                            }

                            _lichidCalcRepository.Update(lichidCalc);
                            CurrentUnitOfWork.SaveChanges();

                        
                    }
                    item.TotalActualiz = _lichidCalcRepository.GetAllIncluding(f => f.SavedBalance)
                                                              .Where(f => f.SavedBalanceId == savedBalanceId && f.LichidConfigId == item.LichidConfigId)
                                                              .Sum(f => f.Valoare);

                    item.TotalInit = _lichidCalcRepository.GetAllIncluding(f => f.SavedBalance)
                                                              .Where(f => f.SavedBalanceId == savedBalanceId && f.LichidConfigId == item.LichidConfigId)
                                                              .Sum(f => f.Valoare);
                }

                //calculez randuri total
                _lichidCalcRepository.LichidCalcTotaluri(savedBalanceId);

                lichidCalcDet = GetLichidCalcDetList(savedBalanceId);

                return lichidCalcDet;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}
