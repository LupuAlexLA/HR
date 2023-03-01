using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.ExternalApi;
using Niva.Erp.Models.Buget;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Repositories.Buget;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Buget
{
    public interface IBugetVenituriAppService : IApplicationService
    {
        void AddTitluri(int formularBVCId, int monthStart, int monthEnd, int? bugetPreliminatId);
        void DeleteTitluri(int formularBVCId);
        List<BugetTitluriDDDto> DispoTitluriAdd();
        List<BugetTitluriDDDto> TitluriAnList();
        List<BugetTitluriViewDto> TitluriViewList(int formularBVCId);
        List<BugetTitluriBVCViewList> TitluriViewBVCList(int venitTitluId);
        List<BugetTitluriCFViewList> TitluriViewCFList(int venitTitluId);
        BugetReinvest BugetReinvestStart(int formularBVCId);
        List<BugetReinvestIncasari> ReinvestIncas(int formularBVCId, DateTime startDate, DateTime endDate);
        List<BugetReinvestPlati> ReinvestPlati(int formularBVCId, DateTime startDate, DateTime endDate);
        void ReinvestSave(List<BugetReinvestIncasari> incasari);
        void AplicaBVCsiCashFlow(int bugetPrevBVCId, int bugetPrevCFId);
        void ReinvestireVenituriTitluCF(int formularBVCId);
        void UpdateVenituriTitluCF(List<BugetTitluriViewDto> bugetVenituriTitluList, int formularBVCId);
        BVC_VenitTitluCFReinvDto ChangeSumReinvest(BVC_VenitTitluCFReinvDto venitReinv, int formularBVCId);
        BVC_VenitTitluCFReinvDto ExchangeForecastByCurrency(BVC_VenitTitluCFReinvDto venitReinv, int formularBVCId);
        List<BugetTitluriCFViewCurrenciesList> TitluriViewCurrencyList(int formularBVCId, DateTime startDate, DateTime endDate);
    }

    public class BugetVenituriAppService : ErpAppServiceBase, IBugetVenituriAppService
    {
        IActiveBugetBVCManager _activeBugetBVCManager;
        IBVC_BugetPrevRepository _bugetPrevRepository;
        IRepository<BVC_Formular> _bvcFormularRepository;
        IRepository<ActivityType> _activityTypeRepository;
        IRepository<Currency> _currencyRepository;
        IBVC_VenituriRepository _bugetVenitTitluRepository;
        IRepository<BVC_VenitTitluBVC> _bugetVenitTitluBVCRepository;
        IRepository<BVC_VenitTitluCF> _bugetVenitTitluCFRepository;
        IRepository<BVC_VenitTitluCFReinv> _bugetVenitTitluCFReinvRepository;
        IRepository<ExchangeRateForecast> _exchangeRateForecastRepository;
        IRepository<BVC_DobandaReferinta> _dobandaReferintaRepository;
        IRepository<BVC_BugetPrevRandValue> _bugetPrevRandValueRepository;
        IRepository<BVC_BugetPrevContributie> _bugetPrevContributieRepository;
        IRepository<BVC_VenitCheltuieli> _venitCheltuieliRepository;
        IRepository<BVC_BugetPrevSumeReinvest> _bugetPrevSumeReinvestRepository;
        IRepository<BVC_VenitTitluParams> _bugetVenitTitluParamsRepository;

        public BugetVenituriAppService(IActiveBugetBVCManager activeBugetBVCManager, IBVC_BugetPrevRepository bugetPrevRepository, IRepository<BVC_Formular> bvcFormularRepository,
                                       IRepository<ActivityType> activityTypeRepository, IRepository<Currency> currencyRepository, IBVC_VenituriRepository bugetVenitTitluRepository,
                                       IRepository<BVC_VenitTitluBVC> bugetVenitTitluBVCRepository, IRepository<BVC_VenitTitluCF> bugetVenitTitluCFRepository,
                                       IRepository<BVC_VenitTitluCFReinv> bugetVenitTitluCFReinvRepository, IRepository<ExchangeRateForecast> exchangeRateForecastRepository,
                                       IRepository<BVC_DobandaReferinta> dobandaReferintaRepository,
                                       IRepository<BVC_BugetPrevRandValue> bugetPrevRandValueRepository, IRepository<BVC_BugetPrevContributie> bugetPrevContributieRepository,
                                       IRepository<BVC_VenitCheltuieli> venitCheltuieliRepository, IRepository<BVC_BugetPrevSumeReinvest> bugetPrevSumeReinvestRepository,
                                       IRepository<BVC_VenitTitluParams> bugetVenitTitluParamsRepository)
        {
            _activeBugetBVCManager = activeBugetBVCManager;
            _bugetPrevRepository = bugetPrevRepository;
            _bvcFormularRepository = bvcFormularRepository;
            _activityTypeRepository = activityTypeRepository;
            _currencyRepository = currencyRepository;
            _bugetVenitTitluRepository = bugetVenitTitluRepository;
            _bugetVenitTitluBVCRepository = bugetVenitTitluBVCRepository;
            _bugetVenitTitluCFRepository = bugetVenitTitluCFRepository;
            _bugetVenitTitluCFReinvRepository = bugetVenitTitluCFReinvRepository;
            _exchangeRateForecastRepository = exchangeRateForecastRepository;
            _dobandaReferintaRepository = dobandaReferintaRepository;
            _bugetPrevRandValueRepository = bugetPrevRandValueRepository;
            _bugetPrevContributieRepository = bugetPrevContributieRepository;
            _venitCheltuieliRepository = venitCheltuieliRepository;
            _bugetPrevSumeReinvestRepository = bugetPrevSumeReinvestRepository;
            _bugetVenitTitluParamsRepository = bugetVenitTitluParamsRepository;
        }

        [AbpAuthorize("Buget.BVC.Venituri.Modificare")]
        public void AddTitluri(int formularBVCId, int monthStart, int monthEnd, int? bugetPreliminatId)
        {
            try
            {
                var appClientId = GetCurrentTenant().Id;
                var bugetFormular = _bvcFormularRepository.GetAll().FirstOrDefault(f => f.Id == formularBVCId);
                int anBVC = bugetFormular.AnBVC;
                var startDate = new DateTime(anBVC, monthStart, 1);
                var endDate = LazyMethods.LastDayOfMonth(new DateTime(anBVC, monthEnd, 1));

                var zileLibereList = _activeBugetBVCManager.ZileLibere(anBVC);

                // salvez parametrii in tabela BVC_VenitTitluParams
                _bugetVenitTitluRepository.SaveVenitTitluParams(formularBVCId, monthStart, monthEnd);

                // titluri din BVC_BugetPrevSumeReinvest
                var titluriSumeReinvest = _bugetPrevSumeReinvestRepository.GetAllIncluding(f => f.BugetPrev)
                                                                          .Where(f => f.BugetPrevId == bugetPreliminatId && f.State == State.Active && f.TenantId == appClientId)
                                                                          .ToList();
                foreach (var item in titluriSumeReinvest)
                {
                    var _titlu = new BVC_VenitTitlu
                    {
                        FormularId = formularBVCId,
                        IdPlasament = item.IdPlasament,
                        CurrencyId = item.CurrencyId,
                        ActivityTypeId = item.ActivityTypeId,
                        StartDate = item.StartDate,
                        MaturityDate = item.MaturityDate,
                        TipPlasament = item.TipPlasament,
                        ValoarePlasament = item.ValoarePlasament,
                        VenitType = item.VenitType,
                        TenantId = appClientId,
                        ProcentDobanda = item.ProcentDobanda,
                        Selectat = true,
                        Reinvestit = true
                    };
                    _bugetVenitTitluRepository.Insert(_titlu);

                    decimal nrZileTitlu = (decimal)(_titlu.MaturityDate - _titlu.StartDate).TotalDays;
                    //var dobandaTotala = Math.Round((decimal)(nrZileTitlu * _titlu.ProcentDobanda) * _titlu.ValoarePlasament / 100 / 360, 2);
                    var dobandaTotala = Math.Round(_titlu.ValoarePlasament * _titlu.ProcentDobanda / 100, 2);

                    // titlu BVC
                    var dataStart = startDate;
                    decimal dobandaCumulataPrecendent = 0, dobandaLuna = 0;

                    var currentDate = startDate;
                    while (currentDate <= LazyMethods.LastDayOfMonth(_titlu.MaturityDate))
                    {
                        currentDate = LazyMethods.LastDayOfMonth(currentDate);
                        var startMonthDate = new DateTime(currentDate.Year, currentDate.Month, 1);
                        var nrZilePrecendent = (decimal)(startMonthDate.AddDays(-1) - _titlu.StartDate).TotalDays + 1;
                        if (nrZilePrecendent < 0)
                        {
                            dobandaCumulataPrecendent = 0;
                        }
                        else
                        {
                            dobandaCumulataPrecendent = Math.Round(dobandaTotala * nrZilePrecendent / nrZileTitlu, 2);
                        }

                        var nrZileLuna = (_titlu.MaturityDate <= currentDate) ? (_titlu.MaturityDate.Day - 1) : currentDate.Day;
                        dobandaLuna = Math.Round(dobandaTotala * nrZileLuna / nrZileTitlu, 2);

                        _bugetVenitTitluBVCRepository.Insert(new BVC_VenitTitluBVC
                        {
                            BVC_VenitTitlu = _titlu,
                            DataDobanda = currentDate,
                            ValoarePlasament = item.ValoarePlasament,
                            DobandaCumulataPrec = dobandaCumulataPrecendent,
                            DobandaLuna = dobandaLuna
                        });
                        currentDate = currentDate.AddMonths(1);
                    }

                    var dataReinvestire = GetZiLucratoare(zileLibereList, (item.TipPlasament == BVC_PlasamentType.Obligatiuni ? item.MaturityDate.AddDays(1) : item.MaturityDate));

                    var titluCF = new BVC_VenitTitluCF
                    {
                        BVC_VenitTitlu = _titlu,
                        DataIncasare = item.MaturityDate,
                        ValoarePlasament = item.ValoarePlasament,
                        DobandaTotala = dobandaCumulataPrecendent + dobandaLuna,
                        DataReinvestire = dataReinvestire,
                        SumaReinvestita = item.ValoarePlasament
                    };
                    _bugetVenitTitluCFRepository.Insert(titluCF);

                }

                // titluri de plasament
                var activeListBVC = new List<ActiveBugetBVCDto>();
                for (int i = monthStart; i <= monthEnd; i++)
                {
                    var currDate = LazyMethods.LastDayOfMonth(new DateTime(anBVC, i, 1));
                    var activeListTemp = _activeBugetBVCManager.ActiveBugetBVCList(currDate);
                    foreach (var item in activeListTemp)
                    {
                        item.data = currDate;
                    }
                    activeListBVC.AddRange(activeListTemp);
                }

                var activeListCF = new List<ActiveBugetBVCDto>();
                for (int i = monthStart; i <= monthEnd; i++)
                {
                    var currDate = LazyMethods.LastDayOfMonth(new DateTime(anBVC, i, 1));
                    var activeListTemp = _activeBugetBVCManager.ActiveBugetCFList(currDate);
                    foreach (var item in activeListTemp)
                    {
                        item.data = currDate;
                    }
                    activeListCF.AddRange(activeListTemp);
                }

                var allTitluri = new List<ActiveBugetBVCDto>();
                allTitluri.AddRange(activeListBVC);
                var idPlasBVC = activeListBVC.Select(f => f.idplasament).ToList();
                allTitluri.AddRange(activeListCF.Where(f => !idPlasBVC.Contains(f.idplasament)).ToList());

                var titluri = allTitluri.GroupBy(f => new { f.idplasament, f.tipPlasament, f.tipFond, f.moneda, f.maturityDate, f.valoarePlasament, f.startDate, f.procentDobanda })
                                        .ToList()
                                        .Select(f => new BugetTitluriDto
                                        {
                                            IdPlasament = f.Key.idplasament,
                                            TipPlasamentStr = f.Key.tipPlasament,
                                            TipFond = (f.Key.tipFond == "SGD" ? "FGDB" : f.Key.tipFond),
                                            Currency = f.Key.moneda,
                                            MaturityDate = f.Key.maturityDate,
                                            StartDate = f.Key.startDate,
                                            ValoarePlasament = f.Key.valoarePlasament,
                                            ProcentDobanda = f.Key.procentDobanda
                                        }).ToList().ToList();

                var activityTypes = _activityTypeRepository.GetAll().Where(f => f.Status == Models.Conta.Enums.State.Active).ToList();

                foreach (var item in titluri)
                {
                    var activityType = activityTypes.FirstOrDefault(f => f.ActivityName == item.TipFond);
                    if (activityType == null)
                    {
                        throw new Exception("Nu am identificat tipul activitatii " + item.TipFond + " pentru plasamentul " + item.IdPlasament + " data" + LazyMethods.DateToString(item.MaturityDate));
                    }
                    item.ActivityTypeId = activityType.Id;

                    switch (item.TipPlasamentStr)
                    {
                        case "OBL":
                            item.TipPlasament = BVC_PlasamentType.Obligatiuni;
                            break;
                        case "O":
                            item.TipPlasament = BVC_PlasamentType.Obligatiuni;
                            break;
                        case "D":
                            item.TipPlasament = BVC_PlasamentType.DepoziteBancare;
                            break;
                        case "T":
                            item.TipPlasament = BVC_PlasamentType.TitluriDePlasament;
                            break;

                    }

                    var currency = _currencyRepository.GetAll().FirstOrDefault(f => f.CurrencyCode == item.Currency);
                    item.CurrencyId = currency.Id;

                    var _titlu = new BVC_VenitTitlu
                    {
                        FormularId = formularBVCId,
                        IdPlasament = item.IdPlasament,
                        CurrencyId = item.CurrencyId,
                        ActivityTypeId = item.ActivityTypeId,
                        StartDate = item.StartDate,
                        MaturityDate = item.MaturityDate,
                        TipPlasament = item.TipPlasament,
                        ValoarePlasament = item.ValoarePlasament,
                        VenitType = VenitType.Plasamente,
                        TenantId = appClientId,
                        ProcentDobanda = item.ProcentDobanda,
                        Selectat = true,
                        Reinvestit = false
                    };
                    _bugetVenitTitluRepository.Insert(_titlu);

                    // intorduc plasamentele pentru bvc
                    var titluBVCList = activeListBVC.Where(f => f.idplasament == item.IdPlasament);
                    foreach (var itemBVC in titluBVCList)
                    {
                        var titluBVC = new BVC_VenitTitluBVC
                        {
                            BVC_VenitTitlu = _titlu,
                            DataDobanda = itemBVC.data,
                            ValoarePlasament = itemBVC.valoarePlasament,
                            DobandaCumulataPrec = itemBVC.valoareDobandaCumulataPanaLaLuna,
                            DobandaLuna = itemBVC.valoareDobandaLuna
                        };
                        _bugetVenitTitluBVCRepository.Insert(titluBVC);
                    }

                    // intorduc plasamentele pentru CF
                    var titluCFList = activeListCF.Where(f => f.idplasament == item.IdPlasament);
                    foreach (var itemBVC in titluCFList)
                    {
                        var dataReinvestire = GetZiLucratoare(zileLibereList, (item.TipPlasament == BVC_PlasamentType.Obligatiuni ? itemBVC.maturityDate.AddDays(1) : itemBVC.maturityDate));

                        var titluCF = new BVC_VenitTitluCF
                        {
                            BVC_VenitTitlu = _titlu,
                            DataIncasare = itemBVC.maturityDate,
                            ValoarePlasament = itemBVC.valoarePlasament,
                            DobandaTotala = itemBVC.valoareDobandaCumulataPanaLaLuna,
                            DataReinvestire = dataReinvestire,
                            SumaReinvestita = itemBVC.valoarePlasament + itemBVC.valoareDobandaCumulataPanaLaLuna
                        };
                        _bugetVenitTitluCFRepository.Insert(titluCF);

                        // determin dobanda de referinta
                        #region determin dobanda de referinta
                        //decimal dobandaReferinta = 0;
                        //var dobandaItem = dobanziReferintaList.FirstOrDefault(f => f.PlasamentType == item.TipPlasament && f.DataStart <= titluCF.DataIncasare && titluCF.DataIncasare <= f.DataEnd);
                        //if (dobandaItem == null)
                        //{
                        //    dobandaItem = dobanziReferintaList.FirstOrDefault(f => f.PlasamentType == null && f.DataStart <= titluCF.DataIncasare && titluCF.DataIncasare <= f.DataEnd);
                        //    if (dobandaItem == null)
                        //    {
                        //        throw new Exception("Nu am identificat dobanda estimata pentru titlul cu codul " + _titlu.IdPlasament + " cu data incasarii " + LazyMethods.DateToString(titluCF.DataIncasare));
                        //    }
                        //}
                        //dobandaReferinta = dobandaItem.Procent;

                        //var titluCFReinv = new BVC_VenitTitluCFReinv
                        //{
                        //    BVC_VenitTitluCF = titluCF,
                        //    DataReinvestire = dataReinvestire,
                        //    MainValue = true,
                        //    ProcDobanda = dobandaReferinta,
                        //    SumaReinvestita = itemBVC.valoarePlasament + itemBVC.valoareDobandaCumulataPanaLaLuna
                        //};
                        //_bugetVenitTitluCFReinvRepository.Insert(titluCFReinv); 
                        #endregion
                    }
                }

                //contributii
                var contributiiList = _bugetPrevContributieRepository.GetAll().Where(f => startDate <= f.DataIncasare && f.DataIncasare <= endDate).ToList();

                foreach (var contributie in contributiiList)
                {
                    var contrib = new BVC_VenitTitlu
                    {
                        FormularId = formularBVCId,
                        IdPlasament = "",
                        CurrencyId = contributie.CurrencyId,
                        ActivityTypeId = contributie.ActivityTypeId,
                        StartDate = contributie.DataIncasare,
                        MaturityDate = contributie.DataIncasare,
                        TipPlasament = BVC_PlasamentType.TitluriDePlasament,
                        ValoarePlasament = contributie.Value,
                        VenitType = VenitType.Contributii,
                        TenantId = appClientId,
                        Selectat = true,
                        Reinvestit = false
                    };
                    _bugetVenitTitluRepository.Insert(contrib);

                    var contribCF = new BVC_VenitTitluCF
                    {
                        BVC_VenitTitlu = contrib,
                        DataIncasare = contributie.DataIncasare,
                        ValoarePlasament = contributie.Value,
                        DobandaTotala = 0,
                        DataReinvestire = contributie.DataIncasare,
                        SumaReinvestita = contributie.Value
                    };
                    _bugetVenitTitluCFRepository.Insert(contribCF);

                    // determin dobanda de referinta
                    #region determin dobanda de referinta
                    //decimal dobandaReferinta = 0;
                    //var dobandaItem = dobanziReferintaList.FirstOrDefault(f => f.PlasamentType == contrib.TipPlasament && f.DataStart <= contribCF.DataIncasare && contribCF.DataIncasare <= f.DataEnd);
                    //if (dobandaItem == null)
                    //{
                    //    dobandaItem = dobanziReferintaList.FirstOrDefault(f => f.PlasamentType == null && f.DataStart <= contribCF.DataIncasare && contribCF.DataIncasare <= f.DataEnd);
                    //    if (dobandaItem == null)
                    //    {
                    //        throw new Exception("Nu am identificat dobanda estimata pentru contributia din data " + LazyMethods.DateToString(contribCF.DataIncasare));
                    //    }
                    //}
                    //dobandaReferinta = dobandaItem.Procent;

                    //var contribCFReinv = new BVC_VenitTitluCFReinv
                    //{
                    //    BVC_VenitTitluCF = contribCF,
                    //    DataReinvestire = contribCF.DataIncasare,
                    //    MainValue = true,
                    //    ProcDobanda = dobandaReferinta,
                    //    SumaReinvestita = contribCF.ValoarePlasament
                    //};
                    //_bugetVenitTitluCFReinvRepository.Insert(contribCFReinv); 
                    #endregion
                }

                CurrentUnitOfWork.SaveChanges();

                ReinvestireVenituriTitluCF(formularBVCId);

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        private DateTime GetZiLucratoare(List<ZileLibereDto> zileLibere, DateTime zi)
        {
            try
            {


                DateTime ret = zi;
                bool ziLucratoare = false;

                while (!ziLucratoare)
                {
                    if (ret.DayOfWeek == DayOfWeek.Saturday && ret.DayOfWeek == DayOfWeek.Sunday && zileLibere.Select(f => f.Data).Contains(ret))
                    {
                        ziLucratoare = false;
                    }
                    else
                    {
                        ziLucratoare = true;
                    }
                    if (!ziLucratoare)
                    {
                        ret.AddDays(1);
                    }
                }

                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        [AbpAuthorize("Buget.BVC.Venituri.Modificare")]
        public void DeleteTitluri(int formularBVCId)
        {
            try
            {
                var alocareInregList = _venitCheltuieliRepository.GetAll().Where(f => f.FormularId == formularBVCId).ToList();
                foreach (var item in alocareInregList)
                {
                    _venitCheltuieliRepository.Delete(item.Id);
                }

                var titluri = _bugetVenitTitluRepository.GetAll().Where(f => f.FormularId == formularBVCId).ToList();
                foreach (var item in titluri)
                {
                    _bugetVenitTitluRepository.Delete(item.Id);
                }

                var bugetVenitTitluParams = _bugetVenitTitluParamsRepository.GetAllIncluding(f => f.Formular).Where(f => f.FormularId == formularBVCId).FirstOrDefault();

                if (bugetVenitTitluParams != null)
                {
                    _bugetVenitTitluParamsRepository.Delete(bugetVenitTitluParams);
                }


                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public List<BugetTitluriDDDto> DispoTitluriAdd()
        {
            try
            {


                var ret = new List<BugetTitluriDDDto>();

                var existingAn = _bugetVenitTitluRepository.GetAll().Select(f => f.FormularId).Distinct().ToList();

                ret = _bvcFormularRepository.GetAll().Where(f => !existingAn.Contains(f.Id) && f.State == Models.Conta.Enums.State.Active)
                                                     .Select(f => new BugetTitluriDDDto
                                                     {
                                                         FormularBVCId = f.Id,
                                                         AnBVC = f.AnBVC
                                                     })
                                                     .OrderByDescending(f => f.AnBVC)
                                                     .ToList();

                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        //[AbpAuthorize("Buget.BVC.Venituri.Acces")]
        public List<BugetTitluriDDDto> TitluriAnList()
        {
            try
            {
                var ret = new List<BugetTitluriDDDto>();

                var existingAn = _bugetVenitTitluRepository.GetAll().Select(f => f.FormularId).Distinct().ToList();

                ret = _bvcFormularRepository.GetAll().Where(f => existingAn.Contains(f.Id) && f.State == Models.Conta.Enums.State.Active)
                                                     .Select(f => new BugetTitluriDDDto
                                                     {
                                                         FormularBVCId = f.Id,
                                                         AnBVC = f.AnBVC
                                                     })
                                                     .OrderByDescending(f => f.AnBVC)
                                                     .ToList();
                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        //[AbpAuthorize("Buget.BVC.Venituri.Acces")]
        public List<BugetTitluriViewDto> TitluriViewList(int formularBVCId)
        {
            try
            {
                var ret = new List<BugetTitluriViewDto>();
                ret = _bugetVenitTitluRepository.GetAllIncluding(f => f.Currency, f => f.ActivityType)
                                                .Where(f => f.FormularId == formularBVCId)
                                                .ToList()
                                                .Select(f => new BugetTitluriViewDto
                                                {
                                                    Id = f.Id,
                                                    IdPlasament = f.IdPlasament,
                                                    ActivityTypeId = f.ActivityTypeId,
                                                    ActivityType = f.ActivityType.ActivityName,
                                                    VenitType = f.VenitType,
                                                    VenitTypeStr = LazyMethods.EnumValueToDescription(f.VenitType),
                                                    CurrencyId = f.CurrencyId,
                                                    Currency = f.Currency.CurrencyCode,
                                                    TipPlasament = f.TipPlasament,
                                                    TipPlasamentStr = (f.VenitType == VenitType.Contributii ? "" : LazyMethods.EnumValueToDescription(f.TipPlasament)),
                                                    ValoarePlasament = f.ValoarePlasament,
                                                    MaturityDate = f.MaturityDate,
                                                    StartDate = f.StartDate,
                                                    ProcentDobanda = f.ProcentDobanda,
                                                    Selectat = f.Selectat,
                                                    Reinvestit = f.Reinvestit
                                                })
                                                .OrderBy(f => f.MaturityDate)
                                                .ToList();

                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public List<BugetTitluriBVCViewList> TitluriViewBVCList(int venitTitluId)
        {
            try
            {
                var ret = new List<BugetTitluriBVCViewList>();
                ret = _bugetVenitTitluBVCRepository.GetAll().Where(f => f.BVC_VenitTitluId == venitTitluId)
                                                            .ToList()
                                                            .Select(f => new BugetTitluriBVCViewList
                                                            {
                                                                DataDobanda = f.DataDobanda,
                                                                DobandaCumulataPrec = f.DobandaCumulataPrec,
                                                                DobandaLuna = f.DobandaLuna,
                                                                Id = f.Id,
                                                                ValoarePlasament = f.ValoarePlasament
                                                            })
                                                            .OrderBy(f => f.DataDobanda)
                                                            .ToList();
                return ret;
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public List<BugetTitluriCFViewList> TitluriViewCFList(int venitTitluId)
        {
            try
            {
                var ret = new List<BugetTitluriCFViewList>();
                ret = _bugetVenitTitluCFRepository.GetAll().Where(f => f.BVC_VenitTitluId == venitTitluId)
                                                            .ToList()
                                                            .Select(f => new BugetTitluriCFViewList
                                                            {
                                                                DataIncasare = f.DataIncasare,
                                                                DataReinvestire = f.DataReinvestire,
                                                                DobandaTotala = f.DobandaTotala,
                                                                SumaReinvestita = f.SumaReinvestita,
                                                                Id = f.Id,
                                                                ValoarePlasament = f.ValoarePlasament
                                                            })
                                                            .OrderBy(f => f.DataIncasare)
                                                            .ToList();
                return ret;
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        //[AbpAuthorize("Buget.BVC.Venituri.Acces")]
        public BugetReinvest BugetReinvestStart(int formularBVCId)
        {
            try
            {
                var ret = new BugetReinvest();
                var formularBVC = _bvcFormularRepository.GetAll().FirstOrDefault(f => f.Id == formularBVCId);
                var startDate = new DateTime(formularBVC.AnBVC, 1, 1);
                var endDate = new DateTime(formularBVC.AnBVC, 12, 31);
                ret.StartDate = startDate;
                ret.EndDate = endDate;
                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public List<BugetReinvestIncasari> ReinvestIncas(int formularBVCId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var appClient = GetCurrentTenant();
                var localCurrencyId = appClient.LocalCurrencyId.Value;
                var ret = _bugetVenitTitluCFRepository.GetAllIncluding(f => f.BVC_VenitTitlu, f => f.BVC_VenitTitlu.Currency, f => f.BVC_VenitTitlu.ActivityType, f => f.BVC_VenitTitluCFReinv)
                                                      .Where(f => f.BVC_VenitTitlu.FormularId == formularBVCId && f.DataIncasare >= startDate && f.DataIncasare <= endDate && f.BVC_VenitTitlu.Selectat == true)
                                                      .OrderBy(f => f.DataIncasare)
                                                      .ToList()
                                                      .Select(f => new BugetReinvestIncasari
                                                      {
                                                          Id = f.Id,
                                                          IdPlasament = f.BVC_VenitTitlu.IdPlasament,
                                                          ActivityTypeId = f.BVC_VenitTitlu.ActivityTypeId,
                                                          ActivityType = f.BVC_VenitTitlu.ActivityType.ActivityName,
                                                          VenitType = f.BVC_VenitTitlu.VenitType,
                                                          VenitTypeStr = f.BVC_VenitTitlu.VenitType.ToString(),
                                                          CurrencyId = f.BVC_VenitTitlu.CurrencyId,
                                                          Currency = f.BVC_VenitTitlu.Currency.CurrencyCode,
                                                          TipPlasament = f.BVC_VenitTitlu.TipPlasament,
                                                          TipPlasamentStr = (f.BVC_VenitTitlu.VenitType == VenitType.Contributii ? "" : f.BVC_VenitTitlu.TipPlasament.ToString()),
                                                          ValoarePlasament = f.ValoarePlasament,
                                                          DobandaTotala = f.DobandaTotala,
                                                          ValoareIncasata = f.ValoarePlasament + f.DobandaTotala,
                                                          ValoareIncasataRon = f.ValoarePlasament + f.DobandaTotala,
                                                          DataIncasare = f.DataIncasare,
                                                          DataReinvestire = f.DataReinvestire,
                                                          SumaReinvestita = f.SumaReinvestita,
                                                          BVC_VenitTitluCFReinv = f.BVC_VenitTitluCFReinv.Select(f => new BVC_VenitTitluCFReinvDto
                                                          {
                                                              Id = f.Id,
                                                              BVC_VenitTitluCFId = f.BVC_VenitTitluCFId,
                                                              DataReinvestire = f.DataReinvestire,
                                                              SumaReinvestita = f.SumaReinvestita,
                                                              ProcDobanda = f.ProcDobanda,
                                                              MainValue = f.MainValue,
                                                              CurrencyId = f.CurrencyId.Value,
                                                              CurrencyName = f.BVC_VenitTitluCF.BVC_VenitTitlu.Currency.CurrencyName,
                                                              CursValutar = f.CursValutarEstimat,
                                                              SumaIncasata = f.SumaIncasata

                                                          }).OrderByDescending(f => f.MainValue).ThenBy(f => f.DataReinvestire).ToList()
                                                      }).ToList();

                var currencyList = ret.Where(f => f.CurrencyId != localCurrencyId).GroupBy(f => f.CurrencyId).Select(f => f.Key).ToList();
                if (currencyList.Count != 0)
                {
                    var formularBVC = _bvcFormularRepository.GetAll().FirstOrDefault(f => f.Id == formularBVCId);
                    var anBVC = formularBVC.AnBVC;

                    foreach (var currency in currencyList)
                    {
                        var exchangeRate = _exchangeRateForecastRepository.GetAll().FirstOrDefault(f => f.Year == anBVC && f.CurrencyId == currency && f.State == Models.Conta.Enums.State.Active);
                        if (exchangeRate == null)
                        {
                            var currencyName = _currencyRepository.GetAll().FirstOrDefault(f => f.Id == currency).CurrencyName;
                            throw new Exception("Nu am identificat cursul valutar estimat pentru " + currencyName + ", anul " + anBVC.ToString());
                        }

                        foreach (var item in ret.Where(f => f.CurrencyId == currency))
                        {
                            item.ValoareIncasataRon = Math.Round(item.ValoareIncasata * exchangeRate.ValoareEstimata, 2);
                        }
                    }
                }

                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public List<TabelIncasari> TableDate(int formularBVCId, DateTime startDate, DateTime endDate)
        {
            var ret = _bugetVenitTitluCFRepository.GetAllIncluding(f => f.BVC_VenitTitlu, f => f.BVC_VenitTitlu.Currency, f => f.BVC_VenitTitlu.ActivityType, f => f.BVC_VenitTitluCFReinv)
                                                      .Where(f => f.BVC_VenitTitlu.FormularId == formularBVCId && f.DataIncasare >= startDate && f.DataIncasare <= endDate && f.BVC_VenitTitlu.Selectat == true)
                                                      .OrderBy(f => f.DataIncasare)
                                                      .ToList()
                                                      .Select(f => new 
                                                      {
                                                          ActivityType = f.BVC_VenitTitlu.ActivityType.ActivityName,
                                                          Currency = f.BVC_VenitTitlu.Currency.CurrencyName,
                                                          IdPlasament = f.BVC_VenitTitlu.IdPlasament,
                                                          VenitTypeStr = f.BVC_VenitTitlu.VenitType.ToString(),
                                                          TipPlasamentStr = f.BVC_VenitTitlu.TipPlasament.ToString(),
                                                          Valoare = f.ValoarePlasament + f.DobandaTotala,
                                                          Date = f.DataIncasare,
                                                          BVC_VenitTitluCFReinv = f.BVC_VenitTitluCFReinv.Select(g => new 
                                                          {                                                            
                                                              Date = g.DataReinvestire,
                                                              Valoare = g.SumaReinvestita,
                                                              ProcDobanda = g.ProcDobanda,
                                                              ActivityType = f.BVC_VenitTitlu.ActivityType.ActivityName,
                                                              IdPlasament = f.BVC_VenitTitlu.IdPlasament,
                                                              VenitTypeStr = f.BVC_VenitTitlu.VenitType.ToString(),
                                                              TipPlasamentStr = f.BVC_VenitTitlu.TipPlasament.ToString(),
                                                              Currency = g.BVC_VenitTitluCF.BVC_VenitTitlu.Currency.CurrencyName,
                                                              
                                                          }).ToList()

                                                      }).ToList();

            var venitTitlu = _bugetVenitTitluRepository.GetAll().Where(f => f.StartDate >= startDate && f.StartDate <= endDate && f.FormularId == formularBVCId && f.Selectat == true);

            var dateTabel = new List<TabelIncasari>();


            foreach (var value in ret)
            {
                var item = new TabelIncasari()
                {
                    Date = value.Date,
                    ActivityType = value.ActivityType,
                    Currency = value.Currency,
                    IdPlasament = value.IdPlasament,
                    TipPlasamentStr = value.TipPlasamentStr,
                    ValoareIncasata = value.Valoare,
                    VenitTypeStr = value.VenitTypeStr,
                    
                };

                dateTabel.Add(item);

                foreach (var value2 in value.BVC_VenitTitluCFReinv)
                {
                    var item2 = new TabelIncasari()
                    {
                        Date = value2.Date,
                        Reinvestit = true,
                        ActivityType = value2.ActivityType,
                        Currency = value2.Currency,
                        IdPlasament = value2.IdPlasament,
                        TipPlasamentStr = value2.TipPlasamentStr ,
                        ValoareReinvestita = value2.Valoare,
                        VenitTypeStr = value2.VenitTypeStr,
                        ProcDobanda = value2.ProcDobanda,
                        
                    };

                    dateTabel.Add(item2);
                }

            }

            foreach(var value in venitTitlu)
            {
                var item2 = new TabelIncasari()
                {
                    Date = value.StartDate,
                    ActivityType = value.ActivityType.ActivityName.ToString(),
                    Currency = value.Currency.CurrencyName.ToString(),
                    IdPlasament = value.IdPlasament ,
                    TipPlasamentStr = value.TipPlasament.ToString(),
                    ValoareReinvestita = value.ValoarePlasament,
                    VenitTypeStr = value.VenitType.ToString(),
                    ProcDobanda = value.ProcentDobanda
                    

                };
                dateTabel.Add(item2);
            }

            return dateTabel.OrderBy(f => f.Date).ThenBy(f => f.IdPlasament).ToList();
        }

            public List<BugetReinvestPlati> ReinvestPlati(int formularBVCId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var appClient = GetCurrentTenant();
                var localCurrencyId = appClient.LocalCurrencyId.Value;
                var ret = new List<BugetReinvestPlati>();
                var formularBVC = _bvcFormularRepository.GetAll().FirstOrDefault(f => f.Id == formularBVCId);
                var formStartDate = new DateTime(formularBVC.AnBVC, 1, 1);
                var formEndDate = new DateTime(formularBVC.AnBVC, 12, 31);

                var bvcPrevCF = _bugetPrevRepository.GetAll().Where(f => f.FormularId == formularBVCId && f.BVC_Tip == BVC_Tip.CashFlow).OrderByDescending(f => f.Id).FirstOrDefault();

                if (bvcPrevCF != null)
                {
                    ret = _bugetPrevRandValueRepository.GetAllIncluding(f => f.BugetPrevRand, f => f.BugetPrevRand.FormRand)
                                                       .Where(f => f.BugetPrevRand.BugetPrevId == bvcPrevCF.Id
                                                                   && startDate <= f.DataOper && f.DataOper <= endDate && f.Value != 0 && !f.BugetPrevRand.FormRand.IsTotal
                                                                   && (f.BugetPrevRand.FormRand.TipRand == BVC_RowType.Cheltuieli || f.BugetPrevRand.FormRand.TipRand == BVC_RowType.Investitii
                                                                       || f.BugetPrevRand.FormRand.TipRand == BVC_RowType.Salarizare))
                                                       .OrderBy(f => f.DataOper)
                                                       .ToList()
                                                       .Select(f => new BugetReinvestPlati
                                                       {
                                                           Id = f.Id,
                                                           DataPlatii = f.DataOper,
                                                           Descriere = f.BugetPrevRand.FormRand.Descriere,
                                                           ValoarePlata = f.Value
                                                       })
                                                       .ToList();
                    foreach (var item in ret)
                    {
                        var valPlatita = _venitCheltuieliRepository.GetAll().Where(f => f.BVC_BugetPrevRandValueId == item.Id).Sum(f => f.Value);
                        item.ValoarePlatita = valPlatita;
                    }
                }
                return ret;
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        [AbpAuthorize("Buget.BVC.Venituri.Modificare")]
        public void ReinvestSave(List<BugetReinvestIncasari> incasari)
        {
            try
            {
                foreach (var incasare in incasari)
                {
                    var item = _bugetVenitTitluCFRepository.GetAll().FirstOrDefault(f => f.Id == incasare.Id);
                    item.DataReinvestire = incasare.DataReinvestire;
                    item.SumaReinvestita = incasare.SumaReinvestita;

                    foreach (var reinv in incasare.BVC_VenitTitluCFReinv)
                    {

                        if (reinv.Id == 0 && !reinv.Delete)
                        {
                            var ret = ObjectMapper.Map<BVC_VenitTitluCFReinv>(reinv);
                            _bugetVenitTitluCFReinvRepository.Insert(ret);
                        }
                        else if (reinv.Id != 0 && !reinv.Delete)
                        {
                            var ret = ObjectMapper.Map<BVC_VenitTitluCFReinv>(reinv);
                            _bugetVenitTitluCFReinvRepository.Update(ret);
                        }
                        else if (reinv.Id != 0 && reinv.Delete)
                        {
                            _bugetVenitTitluCFReinvRepository.Delete(reinv.Id);
                        }
                    }
                }

                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }

        }

        public void AlocaCheltuieli(int formularBVCId, int monthStart, int monthEnd)
        {
            try
            {
                var alocareInregList = _venitCheltuieliRepository.GetAll().Where(f => f.FormularId == formularBVCId).ToList();
                foreach (var item in alocareInregList)
                {
                    _venitCheltuieliRepository.Delete(item.Id);
                }

                var appClient = GetCurrentTenant();
                var localCurrencyId = appClient.LocalCurrencyId.Value;
                var incasari = _bugetVenitTitluCFReinvRepository.GetAllIncluding(f => f.BVC_VenitTitluCF, f => f.BVC_VenitTitluCF.BVC_VenitTitlu)
                                                                .Where(f => f.BVC_VenitTitluCF.BVC_VenitTitlu.FormularId == formularBVCId && f.BVC_VenitTitluCF.BVC_VenitTitlu.CurrencyId == localCurrencyId &&
                                                                            f.BVC_VenitTitluCF.BVC_VenitTitlu.Selectat == true)
                                                                .ToList();

                var formularBVC = _bvcFormularRepository.GetAll().FirstOrDefault(f => f.Id == formularBVCId);
                var formStartDate = new DateTime(formularBVC.AnBVC, monthStart, 1);
                var formEndDate = LazyMethods.LastDayOfMonth(new DateTime(formularBVC.AnBVC, monthEnd, 1));

                var cheltuieli = ReinvestPlati(formularBVCId, formStartDate, formEndDate);
                var dataPrimeiIncasari = incasari.Min(f => f.BVC_VenitTitluCF.DataIncasare);

                foreach (var chelt in cheltuieli.Where(f => f.DataPlatii >= dataPrimeiIncasari))
                {
                    var sumaCheltuiala = chelt.ValoarePlata;

                    while (sumaCheltuiala != 0)
                    {
                        var incasare = incasari.Where(f => f.SumaReinvestita != 0 && f.BVC_VenitTitluCF.DataIncasare <= chelt.DataPlatii)
                                               .OrderByDescending(f => f.BVC_VenitTitluCF.DataIncasare).ThenBy(f => f.SumaReinvestita).FirstOrDefault();
                        if (incasare == null)
                        {
                            throw new Exception("Nu am identificat nici un venit disponibil pentru alocarea cheltuielii in valoare de " + chelt.ValoarePlata.ToString("N4") + " din " + LazyMethods.DateToString(chelt.DataPlatii));
                        }
                        else
                        {
                            decimal valAlocata = 0;
                            if (incasare.SumaReinvestita < sumaCheltuiala)
                            {
                                valAlocata = incasare.SumaReinvestita;
                                sumaCheltuiala = sumaCheltuiala - incasare.SumaReinvestita;
                                incasare.SumaReinvestita = 0;
                            }
                            else
                            {
                                valAlocata = sumaCheltuiala;
                                incasare.SumaReinvestita = incasare.SumaReinvestita - sumaCheltuiala;
                                sumaCheltuiala = 0;
                            }

                            var venitCheltItem = new BVC_VenitCheltuieli
                            {
                                FormularId = formularBVCId,
                                BVC_BugetPrevRandValueId = chelt.Id,
                                BVC_VenitTitluCFReinvId = incasare.Id,
                                Value = valAlocata
                            };
                            _venitCheltuieliRepository.Insert(venitCheltItem);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [AbpAuthorize("Buget.BVC.Venituri.Modificare")]
        public void AplicaBVCsiCashFlow(int bugetPrevBVCId, int bugetPrevCFId)
        {
            try
            {
                // BVC
                _bugetVenitTitluRepository.AplicaBVCsiCashFlow(bugetPrevBVCId);

                _bugetPrevRepository.BVC_PrevTotaluri(bugetPrevBVCId);

                // CF
                _bugetVenitTitluRepository.AplicaBVCsiCashFlow(bugetPrevCFId);

                _bugetPrevRepository.BVC_PrevTotaluri(bugetPrevCFId);

                _bugetVenitTitluRepository.CalculResurse(bugetPrevBVCId, bugetPrevCFId);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public void ReinvestireVenituriTitluCF(int formularBVCId)
        {
            try
            {
                var formularBVC = _bvcFormularRepository.FirstOrDefault(f => f.Id == formularBVCId);
                var localCurrencyId = GetCurrentTenant().LocalCurrencyId;

                var bugetVenitTitluParams = _bugetVenitTitluParamsRepository.GetAllIncluding(f => f.Formular).Where(f => f.FormularId == formularBVCId).FirstOrDefault();

                var dobanziReferintaList = _dobandaReferintaRepository.GetAll().Where(f => f.FormularId == formularBVCId && f.State == State.Active).ToList();
                var bugetVenitTitluCFList = _bugetVenitTitluCFRepository.GetAllIncluding(f => f.BVC_VenitTitlu, f => f.BVC_VenitTitlu.Formular, f => f.BVC_VenitTitluCFReinv)
                                                                        .Where(f => f.BVC_VenitTitlu.FormularId == formularBVCId && f.BVC_VenitTitlu.Selectat == true)
                                                                        .ToList();

                // sterg titlurile reinvestite
                DeleteBugetVenitTitluCFReinv(formularBVCId);

                foreach (var item in bugetVenitTitluCFList)
                {
                    var dobandaReferinta = _bugetVenitTitluRepository.RetDobRefEstimatCuLista(dobanziReferintaList, item.BVC_VenitTitlu.TipPlasament, item.DataReinvestire, item.BVC_VenitTitlu.CurrencyId, item.BVC_VenitTitlu.ActivityTypeId);


                    //var bugetVenitTitluCFReinv = _bugetVenitTitluCFReinvRepository.GetAllIncluding(f => f.BVC_VenitTitluCF).Where(f => f.BVC_VenitTitluCFId == item.Id).FirstOrDefault();

                    //if (bugetVenitTitluCFReinv == null) // NU EXISTA
                    //{
                    //    _bugetVenitTitluCFReinvRepository.Insert(new BVC_VenitTitluCFReinv
                    //    {
                    //        BVC_VenitTitluCFId = item.Id,
                    //        DataReinvestire = item.DataReinvestire,
                    //        MainValue = true,
                    //        ProcDobanda = dobandaReferinta,
                    //        SumaReinvestita = item.ValoarePlasament + item.DobandaTotala
                    //    });
                    //}
                    //else
                    //{
                    //    bugetVenitTitluCFReinv.ProcDobanda = dobandaReferinta;
                    //    bugetVenitTitluCFReinv.SumaReinvestita = item.ValoarePlasament + item.DobandaTotala;
                    //    bugetVenitTitluCFReinv.DataReinvestire = item.DataReinvestire;
                    //    _bugetVenitTitluCFReinvRepository.Update(bugetVenitTitluCFReinv);
                    //}

                        _bugetVenitTitluCFReinvRepository.Insert(new BVC_VenitTitluCFReinv
                        {
                            BVC_VenitTitluCFId = item.Id,
                            DataReinvestire = item.DataReinvestire,
                            MainValue = true,
                            ProcDobanda = dobandaReferinta,
                            SumaReinvestita = item.ValoarePlasament + item.DobandaTotala,
                            SumaIncasata = item.ValoarePlasament + item.DobandaTotala,
                            CurrencyId = item.BVC_VenitTitlu.CurrencyId,
                            CursValutarEstimat = 1
                        });

                }
                CurrentUnitOfWork.SaveChanges();

                AlocaCheltuieli(formularBVCId, bugetVenitTitluParams.MonthStart, bugetVenitTitluParams.MonthEnd);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void DeleteBugetVenitTitluCFReinv(int formularBVCId)
        {
            try
            {
                var alocareInregList = _venitCheltuieliRepository.GetAll().Where(f => f.FormularId == formularBVCId).ToList();
                foreach (var item in alocareInregList)
                {
                    _venitCheltuieliRepository.Delete(item.Id);
                }

                var bugetVenitTitluCFReinvList = _bugetVenitTitluCFReinvRepository.GetAllIncluding(f => f.BVC_VenitTitluCF, f => f.BVC_VenitTitluCF.BVC_VenitTitlu,
                                                                                                   f => f.BVC_VenitTitluCF.BVC_VenitTitlu.Formular,
                                                                                                   f => f.VenitCheltuieli)
                                                                                  .Where(f => f.BVC_VenitTitluCF.BVC_VenitTitlu.FormularId == formularBVCId)
                                                                                  .ToList();
                foreach (var item in bugetVenitTitluCFReinvList)
                {
                    _bugetVenitTitluCFReinvRepository.Delete(item);
                }

                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateVenituriTitluCF(List<BugetTitluriViewDto> bugetVenituriTitluList, int formularBVCId)
        {
            try
            {
                foreach (var item in bugetVenituriTitluList)
                {
                    var bugetVenitTitlu = new BVC_VenitTitlu
                    {
                        ActivityTypeId = item.ActivityTypeId,
                        CurrencyId = item.CurrencyId,
                        FormularId = formularBVCId,
                        IdPlasament = item.IdPlasament,
                        MaturityDate = item.MaturityDate,
                        Id = item.Id,
                        ProcentDobanda = item.ProcentDobanda,
                        Reinvestit = item.Reinvestit,
                        Selectat = item.Selectat,
                        StartDate = item.StartDate,
                        TenantId = 1,
                        TipPlasament = item.TipPlasament,
                        ValoarePlasament = item.ValoarePlasament,
                        VenitType = item.VenitType
                    };
                    _bugetVenitTitluRepository.Update(bugetVenitTitlu);
                }

                CurrentUnitOfWork.SaveChanges();

                ReinvestireVenituriTitluCF(formularBVCId);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public BVC_VenitTitluCFReinvDto ChangeSumReinvest(BVC_VenitTitluCFReinvDto venitReinv, int formularBVCId)
        {
            try
            {
                var localCurrencyId = GetCurrentTenant().LocalCurrencyId;
                var formularBVC = _bvcFormularRepository.FirstOrDefault(f => f.Id == formularBVCId);

                var venitTitluCF = _bugetVenitTitluCFRepository.GetAllIncluding(f => f.BVC_VenitTitluCFReinv, f => f.BVC_VenitTitlu)
                                                               .FirstOrDefault(f => f.Id == venitReinv.BVC_VenitTitluCFId && f.BVC_VenitTitlu.FormularId == formularBVCId);


                var dobanziReferintaList = _dobandaReferintaRepository.GetAll().Where(f => f.FormularId == formularBVCId && f.State == State.Active).ToList();
                var dobandaReferinta = _bugetVenitTitluRepository.RetDobRefEstimatCuLista(dobanziReferintaList, venitTitluCF.BVC_VenitTitlu.TipPlasament, venitReinv.DataReinvestire, venitReinv.CurrencyId, venitTitluCF.BVC_VenitTitlu.ActivityTypeId);
                var exchangeRateForecastValuta = _exchangeRateForecastRepository.GetAllIncluding(f => f.Currency)
                                                .FirstOrDefault(f => f.State == State.Active && f.Year == formularBVC.AnBVC && f.CurrencyId == venitReinv.CurrencyId);

                if (venitTitluCF.BVC_VenitTitlu.CurrencyId == localCurrencyId)
                {
                    if (venitTitluCF.BVC_VenitTitlu.CurrencyId == venitReinv.CurrencyId)
                    {
                        venitReinv.SumaReinvestita = venitReinv.SumaIncasata;
                        venitReinv.CursValutar = 1;
                        venitReinv.ProcDobanda = dobandaReferinta;
                    }else {

                        venitReinv.SumaReinvestita = Math.Round(venitReinv.SumaIncasata / exchangeRateForecastValuta.ValoareEstimata, 2);
                        venitReinv.CursValutar = exchangeRateForecastValuta.ValoareEstimata;
                        venitReinv.ProcDobanda = dobandaReferinta;
                    }
                }else
                {
                    if (venitTitluCF.BVC_VenitTitlu.CurrencyId == venitReinv.CurrencyId)
                    {
                        venitReinv.SumaReinvestita = venitReinv.SumaIncasata;
                        venitReinv.CursValutar = 1;
                        venitReinv.ProcDobanda = dobandaReferinta;
                    }
                    else
                    {
                        if (venitReinv.CurrencyId == localCurrencyId)
                        {
                            exchangeRateForecastValuta = _exchangeRateForecastRepository.GetAllIncluding(f => f.Currency)
                                                .FirstOrDefault(f => f.State == State.Active && f.Year == formularBVC.AnBVC && f.CurrencyId == venitTitluCF.BVC_VenitTitlu.CurrencyId);
                            venitReinv.SumaReinvestita = Math.Round(venitReinv.SumaReinvestita * exchangeRateForecastValuta.ValoareEstimata, 2);
                            venitReinv.CursValutar = exchangeRateForecastValuta.ValoareEstimata;
                            venitReinv.ProcDobanda = dobandaReferinta;
                        }else
                        {
                            throw new Exception($"Nu este implementata optiunea de reinvestire a unei sume in valuta in alta valuta.");
                        }
                    }  
                }
                return venitReinv;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public BVC_VenitTitluCFReinvDto ExchangeForecastByCurrency(BVC_VenitTitluCFReinvDto venitReinv, int formularBVCId)
        {
            try
            {
                var localCurrencyId = GetCurrentTenant().LocalCurrencyId;
                var formularBVC = _bvcFormularRepository.FirstOrDefault(f => f.Id == formularBVCId);

                var venitTitluCF = _bugetVenitTitluCFRepository.GetAllIncluding(f => f.BVC_VenitTitluCFReinv, f => f.BVC_VenitTitlu)
                                                               .FirstOrDefault(f => f.Id == venitReinv.BVC_VenitTitluCFId && f.BVC_VenitTitlu.FormularId == formularBVCId);

                var exchangeRateForecast = _exchangeRateForecastRepository.GetAllIncluding(f => f.Currency)
                                                                          .FirstOrDefault(f => f.State == State.Active && f.Year == formularBVC.AnBVC && f.CurrencyId == venitReinv.CurrencyId);

                if (venitTitluCF.BVC_VenitTitlu.CurrencyId == localCurrencyId)
                {
                    if (venitTitluCF.BVC_VenitTitlu.CurrencyId == venitReinv.CurrencyId)
                    {
                        venitReinv.SumaReinvestita = venitReinv.SumaIncasata;
                        venitReinv.CursValutar = 1;
                    }
                    else
                    {
                        venitReinv.SumaReinvestita = Math.Round(venitReinv.SumaIncasata / exchangeRateForecast.ValoareEstimata, 2);
                        venitReinv.CursValutar = exchangeRateForecast.ValoareEstimata;
                    }
                }
                else
                {
                    if (venitTitluCF.BVC_VenitTitlu.CurrencyId == venitReinv.CurrencyId)
                    {
                        venitReinv.SumaReinvestita = venitReinv.SumaIncasata;
                        venitReinv.CursValutar = 1;
                    }
                    else
                    {
                        if (venitReinv.CurrencyId == localCurrencyId)
                        {
                            exchangeRateForecast = _exchangeRateForecastRepository.GetAllIncluding(f => f.Currency)
                                                .FirstOrDefault(f => f.State == State.Active && f.Year == formularBVC.AnBVC && f.CurrencyId == venitTitluCF.BVC_VenitTitlu.CurrencyId);
                            venitReinv.SumaReinvestita = Math.Round(venitReinv.SumaIncasata * exchangeRateForecast.ValoareEstimata, 2);
                            venitReinv.CursValutar = exchangeRateForecast.ValoareEstimata;
                        }
                        else
                        {
                            throw new Exception($"Nu este implementata optiunea de reinvestire a unei sume in valuta in alta valuta.");
                        }
                    }
                }
                //if (venitReinv.CurrencyId == localCurrencyId)
                //{
                //    venitReinv.CursValutar = 1;
                //    venitReinv.SumaReinvestita = venitReinv.SumaIncasata;
                //}else
                //{
                //    var exchangeRateForecast = _exchangeRateForecastRepository.GetAllIncluding(f => f.Currency)
                //                          .FirstOrDefault(f => f.State == State.Active && f.Year == formularBVC.AnBVC && f.CurrencyId == venitReinv.CurrencyId);

                //    venitReinv.CursValutar = exchangeRateForecast.ValoareEstimata;
                //    venitReinv.SumaReinvestita = Math.Round(venitReinv.SumaIncasata * exchangeRateForecast.ValoareEstimata, 2);
                //}

                return venitReinv;

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public List<BugetTitluriCFViewCurrenciesList> TitluriViewCurrencyList(int formularBVCId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var ret = new List<BugetTitluriCFViewCurrenciesList>();
                  var appClient = GetCurrentTenant();
                var localCurrencyId = appClient.LocalCurrencyId.Value;
                var formularBVC = _bvcFormularRepository.FirstOrDefault(f => f.Id == formularBVCId);
                var bugetVenitTitluList = _bugetVenitTitluCFRepository.GetAllIncluding(f => f.BVC_VenitTitlu, f => f.BVC_VenitTitlu.Currency, f => f.BVC_VenitTitlu.ActivityType, f => f.BVC_VenitTitluCFReinv)
                                                      .Where(f => f.BVC_VenitTitlu.FormularId == formularBVCId && f.DataIncasare >= startDate && f.DataIncasare <= endDate && f.BVC_VenitTitlu.Selectat == true)
                                                      .OrderBy(f => f.DataIncasare)
                                                      .ToList();
                var venitTitlu = _bugetVenitTitluRepository.GetAllIncluding(f => f.Currency , f => f.ActivityType).Where(f => f.StartDate >= startDate && f.StartDate <= endDate && f.FormularId == formularBVCId).Select(f => new BugetTitluriCFViewCurrenciesList {
                    ActivityTypeId = f.ActivityTypeId,
                    CurrencyId = f.CurrencyId,
                    Currency = f.Currency.CurrencyName,
                    ActivityType = f.ActivityType.ActivityName,
                    ValoarePlasament = f.ValoarePlasament,   
                });

                ret.AddRange(venitTitlu);


                var currencyList = bugetVenitTitluList.GroupBy(f => f.BVC_VenitTitlu.CurrencyId).Select(f => f.Key).ToList();
                foreach (var item in currencyList)
                {

                        ret.AddRange(bugetVenitTitluList.Where(f => f.BVC_VenitTitlu.CurrencyId == item)
                                                        .GroupBy(f => new { CurrencyId = f.BVC_VenitTitlu.CurrencyId, CurrencyName = f.BVC_VenitTitlu.Currency.CurrencyName,
                                                                            ActivityTypeId = f.BVC_VenitTitlu.ActivityTypeId, ActivityTypeName = f.BVC_VenitTitlu.ActivityType.ActivityName})
                                                        .Select(f => new BugetTitluriCFViewCurrenciesList
                                                        {
                                                            ActivityTypeId = f.Key.ActivityTypeId,
                                                            ActivityType = f.Key.ActivityTypeName,
                                                            CurrencyId = f.Key.CurrencyId,
                                                            Currency = f.Key.CurrencyName,
                                                            ValoareIncasata = f.Sum(g => g.ValoarePlasament+g.DobandaTotala),
                                                            ValoareReinvestita = f.Sum(g => g.BVC_VenitTitluCFReinv.Sum(f => f.SumaReinvestita))
                                                        })
                                                        .ToList()
                                                        );
                }


                ret = ret.GroupBy(f => new
                {
                    CurrencyId = f.CurrencyId,
                    CurrencyName = f.Currency,
                    ActivityTypeId = f.ActivityTypeId,
                    ActivityTypeName = f.ActivityType
                })
                                                       .Select(f => new BugetTitluriCFViewCurrenciesList
                                                       {
                                                           ActivityTypeId = f.Key.ActivityTypeId,
                                                           ActivityType = f.Key.ActivityTypeName,
                                                           CurrencyId = f.Key.CurrencyId,
                                                           Currency = f.Key.CurrencyName,
                                                           ValoareIncasata = f.Sum(g => g.ValoareIncasata),
                                                           ValoareReinvestita = f.Sum(g => g.ValoareReinvestita),
                                                           ValoarePlasament = f.Sum(g => g.ValoarePlasament)
                                                       }).ToList();

                ret.ForEach(e => {
                    if (e.CurrencyId != localCurrencyId)
                    {
                        var exchangeRateForecast = _exchangeRateForecastRepository.GetAllIncluding(f => f.Currency)
                                                                          .FirstOrDefault(f => f.State == State.Active && f.Year == formularBVC.AnBVC && f.CurrencyId == e.CurrencyId);
                        e.ValoareReinvestitaLei =  Math.Round(e.ValoareReinvestita * exchangeRateForecast.ValoareEstimata, 2);
                        e.ValoareIncasataLei = Math.Round(e.ValoareIncasata * exchangeRateForecast.ValoareEstimata, 2);
                        e.ValoarePlasamentLei = Math.Round(e.ValoarePlasament * exchangeRateForecast.ValoareEstimata, 2);
                    }
                    else
                    {
                        e.ValoareIncasataLei = e.ValoareIncasata;
                        e.ValoareReinvestitaLei = e.ValoareReinvestita;
                        e.ValoarePlasamentLei = e.ValoarePlasament;
                    }
                    
                    
                });


               


                return ret;
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }
    }
}
