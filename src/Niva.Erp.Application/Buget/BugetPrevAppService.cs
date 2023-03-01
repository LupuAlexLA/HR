using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.Buget.Dto;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.ExternalApi;
using Niva.Erp.Models.Buget;
using Niva.Erp.Models.Buget.BVC;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.HR;
using Niva.Erp.Repositories.Buget;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Buget
{
    public interface IBugetPrevAppService : IApplicationService
    {
        BugetPrevGenerateDto BVC_PrevGenerateInit();

        List<BugetPrevListDto> BugetPrevList();

        List<BugetPrevListDto> SearchBugetPrev(int? anBugetId);

        //BugetPrevGenerateDto GenerateBugetPrev(BugetPrevGenerateDto form);
        BugetPrevGenerateDto GenerateBugetPrevV2(BugetPrevGenerateDto form);

        //BugetPrevItemDto GetBugetPrevDetails(int? departamentId, int bugetPrevId, int month);
        BugetPrevItemDto GetBugetPrevDetailsV2(int? departamentId, int bugetPrevId, string month, bool values0);

        List<BugetPrevMonthsDto> GetBugetPrevDataLunaList(int bugetPrevId);

        void Delete(int bugetPrevId);

        void Save(BugetPrevByDepartmentDto buget, int bugetPrevId, int departamentId, string month);

        void ValidateAll(int bugetPrevId);

        void ValidateByDepartament(int bugetPrevId, int departamentId);

        void CancelAll(int formularId);

        void CancelByDepartament(int bugetPrevId, int departamentId);

        void ChangeBugetPrevState(BugetPrevStatusDto bugetPRevStatus);

        int DuplicateBugetPrev(int bugetPrevId, DateTime dataBuget, string descriere);

        List<BugetPrevDDDto> BugetPrevDDList(int formularId, int bvcTip);

        List<BugetPrevStatCalculDto> BugetPrevStatCalcList(int bugetPrevId);

        List<BugetPrevDDDto> BugetPreliminatLastYear(int formularId);

        List<BugetPrevDDDto> BugetPreliminatCFLastYear(int formularId);

        void CalculResurse(int bugetPrevId, int cashFlowId, int formularId, int tipBVC);
    }

    public class BugetPrevAppService : ErpAppServiceBase, IBugetPrevAppService
    {
        private IBVC_BugetPrevRepository _bugetPrevRepository;
        private IRepository<BVC_FormRand> _bvcFormRandRepository;
        private IRepository<BVC_BugetPrevRand> _bugetPrevRandRepository;
        private IRepository<BVC_BugetPrevRandValue> _bugetPrevRandValueRepository;
        private IRepository<BVC_BugetPrevStatus> _bugetPrevStatusRepository;
        private IBVC_PaapRepository _bvcPaapRepository;
        private IRepository<SalariatiDepartamente> _salariatiDepartamentRepository;
        private IRepository<ActivityType> _activityTypeRepository;
        private IActiveBugetBVCManager _activeBugetBVCManager;
        private IRepository<BVC_VenitProcRepartiz> _venitProcRepartzRepository;
        private IRepository<BVC_BugetPrevStatCalcul> _bugetPrevStatCalculRepository;
        private IRepository<BVC_VenitTitluCF> _bugetVenitTitluCFRepository;
        private IRepository<BVC_Formular> _bvcFormularRepository;
        private IBVC_VenituriRepository _bugetVenitTitluRepository;
        private IRepository<BVC_VenitTitluCFReinv> _bugetVenitTitluCFReinvRepository;
        private IRepository<BVC_VenitTitluBVC> _bugetVenitTitluBVCRepository;
        private IRepository<BVC_PrevResurseDetalii> _bugetPrevResurseDetaliiRepository;
        private IRepository<BVC_PrevResurse> _bugetPrevResurseRepository;
        private IRepository<BVC_BugetPrevContributie> _bugetPrevContribRepository;
        private IRepository<ExchangeRateForecast> _exchangeRateForecastRepository;
        private IRepository<Currency> _currencyRepository;
        private IRepository<BVC_PrevResurse> _prevResurseRepository;
        private IRepository<BVC_VenitBugetPrelim> _venitBugetPrelim;

        public BugetPrevAppService(IBVC_BugetPrevRepository bugetPrevRepository, IRepository<BVC_FormRand> bvcFormRandRepository, IRepository<BVC_BugetPrevRand> bugetPrevRandRepository,
                   IBVC_PaapRepository bvcPaapRepository, IRepository<SalariatiDepartamente> salariatiDepartamentRepository, IRepository<BVC_BugetPrevRandValue> bugetPrevRandValueRepository,
                   IRepository<ActivityType> activityTypeRepository, IRepository<BVC_BugetPrevStatus> bugetPrevStatusRepository, IActiveBugetBVCManager activeBugetBVCManager,
                   IRepository<BVC_VenitProcRepartiz> venitProcRepartzRepository, IRepository<BVC_BugetPrevStatCalcul> bugetPrevStatCalculRepository,
                   IRepository<BVC_VenitTitluCF> bugetVenitTitluCFRepository, IRepository<BVC_Formular> bvcFormularRepository, IBVC_VenituriRepository bugetVenitTitluRepository, IRepository<BVC_VenitTitluCFReinv> bugetVenitTitluCFReinvRepository,
                   IRepository<BVC_VenitTitluBVC> bugetVenitTitluBVCRepository, IRepository<BVC_PrevResurseDetalii> bugetPrevResurseDetaliiRepository, IRepository<BVC_PrevResurse> bugetPrevResurseRepository,
                   IRepository<BVC_BugetPrevContributie> bugetPrevContribRepository, IRepository<ExchangeRateForecast> exchangeRateForecastRepository, IRepository<Currency> currencyRepository,
                   IRepository<BVC_PrevResurse> prevResurseRepository, IRepository<BVC_VenitBugetPrelim> venitBugetPrelim)
        {
            _bugetPrevRepository = bugetPrevRepository;
            _bvcFormRandRepository = bvcFormRandRepository;
            _bugetPrevRandRepository = bugetPrevRandRepository;
            _bugetPrevRandValueRepository = bugetPrevRandValueRepository;
            _bugetPrevStatusRepository = bugetPrevStatusRepository;
            _bvcPaapRepository = bvcPaapRepository;
            _salariatiDepartamentRepository = salariatiDepartamentRepository;
            _activityTypeRepository = activityTypeRepository;
            _activeBugetBVCManager = activeBugetBVCManager;
            _venitProcRepartzRepository = venitProcRepartzRepository;
            _bugetPrevStatCalculRepository = bugetPrevStatCalculRepository;
            _bugetVenitTitluCFRepository = bugetVenitTitluCFRepository;
            _bvcFormularRepository = bvcFormularRepository;
            _bugetVenitTitluRepository = bugetVenitTitluRepository;
            _bugetVenitTitluCFReinvRepository = bugetVenitTitluCFReinvRepository;
            _bugetVenitTitluBVCRepository = bugetVenitTitluBVCRepository;
            _bugetPrevResurseDetaliiRepository = bugetPrevResurseDetaliiRepository;
            _bugetPrevResurseRepository = bugetPrevResurseRepository;
            _bugetPrevContribRepository = bugetPrevContribRepository;
            _exchangeRateForecastRepository = exchangeRateForecastRepository;
            _currencyRepository = currencyRepository;
            _prevResurseRepository = prevResurseRepository;
            _venitBugetPrelim = venitBugetPrelim;
        }

        public List<BugetPrevListDto> SearchBugetPrev(int? anBugetId)
        {
            try
            {
                var bugetPrevList = _bugetPrevRepository.GetAllIncluding(f => f.Formular, f => f.StatusList).Where(f => f.State == State.Active);
                if (anBugetId != 0)
                {
                    bugetPrevList = bugetPrevList.Where(f => f.FormularId == anBugetId).OrderByDescending(f => f.FormularId);
                }

                var ret = ObjectMapper.Map<List<BugetPrevListDto>>(bugetPrevList);
                ret = CheckBugetValidation(ret);

                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        //[AbpAuthorize("Buget.BVC.Prevazut.Acces")]
        public List<BugetPrevListDto> BugetPrevList()
        {
            try
            {
                var list = _bugetPrevRepository.GetAllIncluding(f => f.Formular, f => f.StatusList).Where(f => f.State == State.Active).OrderByDescending(f => f.FormularId).ToList();
                var ret = ObjectMapper.Map<List<BugetPrevListDto>>(list);
                ret = CheckBugetValidation(ret);
                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        [AbpAuthorize("Buget.BVC.Prevazut.Modificare")]
        public BugetPrevGenerateDto BVC_PrevGenerateInit()
        {
            try
            {
                var ret = new BugetPrevGenerateDto
                {
                    DataBuget = LazyMethods.Now(),
                    FormularId = null,
                    BVC_Tip = null,
                    Status = 1,
                    MonthStart = 1,
                    MonthEnd = 12
                };

                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        #region Commented code

        //public BugetPrevGenerateDto GenerateBugetPrev(BugetPrevGenerateDto form)
        //{
        //    try
        //    {
        //        var bugetPrev = ObjectMapper.Map<BVC_BugetPrev>(form);
        //        var appClient = GetCurrentTenant();

        //        if (bugetPrev.Id == 0) // INSERT
        //        {
        //            bugetPrev.TenantId = appClient.Id;
        //            _bugetPrevRepository.Insert(bugetPrev);
        //        }

        //        var bvc_BugetPrevStatus = new BVC_BugetPrevStatus()
        //        {
        //            BugetPrev = bugetPrev,
        //            StatusDate = bugetPrev.DataBuget,
        //            Status = BVC_Status.Preliminat
        //        };
        //        _bugetPrevStatusRepository.Insert(bvc_BugetPrevStatus);

        //        // iau repartizarea veniturilor pentru anul bugetului
        //        var venitProcRepartiz = _venitProcRepartzRepository.GetAll().Where(f => f.FormularId == form.FormularId)
        //                                                           .Select(f => new VenitRepartizProc
        //                                                           {
        //                                                               ActivityTypeId = f.ActivityTypeId,
        //                                                               ProcRepartizat = f.ProcRepartiz
        //                                                           }).ToList();
        //        if (venitProcRepartiz.Count == 0)
        //        {
        //            throw new Exception("Nu a fost realizata repartizarea veniturilor pentru anul de buget selectat");
        //        }

        //        var departamentList = _salariatiDepartamentRepository.GetAll().Where(f => f.State == State.Active).Select(f => f.DepartamentId).Distinct().ToList();
        //        var bvcFormRanduriList = _bvcFormRandRepository.GetAllIncluding(f => f.Formular).Where(f => f.FormularId == form.FormularId).ToList();
        //        if (bugetPrev.BVC_Tip == BVC_Tip.BVC)
        //        {
        //            bvcFormRanduriList = bvcFormRanduriList.Where(f => f.AvailableBVC).ToList();
        //        }
        //        if (bugetPrev.BVC_Tip == BVC_Tip.CashFlow)
        //        {
        //            bvcFormRanduriList = bvcFormRanduriList.Where(f => f.AvailableCashFlow).ToList();
        //        }
        //        var anBVC = bvcFormRanduriList.FirstOrDefault().Formular.AnBVC;
        //        var activityTypeList = _activityTypeRepository.GetAll().Where(f => f.Status == State.Active && f.TenantId == appClient.Id);

        //        if (form.BVC_Tip == (int)BVC_Tip.BVC)
        //        {
        //            bvcFormRanduriList = bvcFormRanduriList.Where(f => f.AvailableBVC).ToList();
        //        }
        //        if (form.BVC_Tip == (int)BVC_Tip.CashFlow)
        //        {
        //            bvcFormRanduriList = bvcFormRanduriList.Where(f => f.AvailableCashFlow).ToList();
        //        }

        //        foreach (var dep in departamentList)
        //        {
        //            foreach (var rand in bvcFormRanduriList)
        //            {
        //                var bvcPrevRand = new BVC_BugetPrevRand
        //                {
        //                    BugetPrev = bugetPrev,
        //                    FormRandId = rand.Id,
        //                    DepartamentId = dep,
        //                    State = State.Active,
        //                    TenantId = appClient.Id
        //                };

        //                _bugetPrevRandRepository.Insert(bvcPrevRand);
        //                //CurrentUnitOfWork.SaveChanges();
        //                var lastDayFirstMonth = LazyMethods.LastDayOfMonth(new DateTime(anBVC, 1, 1));
        //                var lastDayLastMonth = LazyMethods.LastDayOfMonth(new DateTime(anBVC, 12, 1));
        //                for (DateTime date = lastDayFirstMonth; date <= lastDayLastMonth; date = LazyMethods.LastDayOfMonth(date.AddMonths(1)))
        //                {
        //                    foreach (var activity in activityTypeList.ToList())
        //                    {
        //                        var bvcPrevRandValue = new BVC_BugetPrevRandValue
        //                        {
        //                            Value = 0,
        //                            ValueType = BVC_PrevValueType.Manual,
        //                            BugetPrevRand = bvcPrevRand,
        //                            ActivityTypeId = activity.Id,
        //                            DataLuna = date,
        //                            DataOper = date
        //                        };
        //                        _bugetPrevRandValueRepository.Insert(bvcPrevRandValue);
        //                        //CurrentUnitOfWork.SaveChanges();
        //                    }
        //                }
        //            }
        //        }
        //        CurrentUnitOfWork.SaveChanges();
        //        form.Id = bugetPrev.Id;

        //        if (bugetPrev.BVC_Tip == BVC_Tip.BVC)
        //        {
        //            //contributii
        //            try
        //            {
        //                _bugetPrevRepository.BVC_PrevContributii(bugetPrev.Id);
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new Exception("Eroare generare venituri contributii - " + ex.ToString());
        //            }

        //            // salarizare
        //            try
        //            {
        //                var salarizareBVC = _activeBugetBVCManager.SalarizareBVCList(anBVC);
        //                _bugetPrevRepository.BVC_PrevSalarizare(salarizareBVC, bugetPrev.Id, venitProcRepartiz);
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new Exception("Eroare generare cheltuieli salarizare - " + ex.ToString());
        //            }

        //            // inserez amortizarile MF
        //            try
        //            {
        //                _bugetPrevRepository.BVC_PrevAmortizariMF(bugetPrev.Id, venitProcRepartiz);
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new Exception("Eroare generare amortizari mijloace fixe - " + ex.ToString());
        //            }

        //            // inserez amortizarile CA
        //            try
        //            {
        //                _bugetPrevRepository.BVC_PrevAmortizariCA(bugetPrev.Id, venitProcRepartiz);
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new Exception("Eroare generare amortizari chelt avans - " + ex.ToString());
        //            }
        //            // inserez inregistrarile din PAAP
        //            try
        //            {
        //                _bugetPrevRepository.BVC_PrevPAAP(bugetPrev.Id, venitProcRepartiz);
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new Exception("Eroare generare cheltuieli PAAP - " + ex.ToString());
        //            }

        //            // totaluri
        //            try
        //            {
        //                _bugetPrevRepository.BVC_PrevTotaluri(bugetPrev.Id);
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new Exception("Eroare calcul totaluri - " + ex.ToString());
        //            }

        //        }
        //        else // cash flow
        //        {
        //            // contributii
        //            try
        //            {
        //                _bugetPrevRepository.BVC_CashFlowContributii(bugetPrev.Id);
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new Exception("Eroare generare cashflow contributii - " + ex.ToString());
        //            }

        //            // salarizare
        //            try
        //            {
        //                var salarizareCF = _activeBugetBVCManager.SalarizareCFList(anBVC);
        //                _bugetPrevRepository.BVC_CashFlowSalarizare(salarizareCF, bugetPrev.Id, venitProcRepartiz);
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new Exception("Eroare generare cheltuieli salarizare - " + ex.ToString());
        //            }

        //            //paap
        //            try
        //            {
        //                _bugetPrevRepository.BVC_CashFlowPAAP(bugetPrev.Id, venitProcRepartiz);
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new Exception("Eroare generare cashflow PAAP - " + ex.ToString());
        //            }

        //            // totaluri
        //            try
        //            {
        //                _bugetPrevRepository.BVC_PrevTotaluri(bugetPrev.Id);
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new Exception("Eroare calcul totaluri - " + ex.ToString());
        //            }

        //        }

        //        return form;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new UserFriendlyException("Eroare", ex.ToString());
        //    }
        //}

        #endregion Commented code

        [AbpAuthorize("Buget.BVC.Prevazut.Modificare")]
        public BugetPrevGenerateDto GenerateBugetPrevV2(BugetPrevGenerateDto form)
        {
            try
            {
                var bugetPrev = ObjectMapper.Map<BVC_BugetPrev>(form);
                var appClient = GetCurrentTenant();

                if (bugetPrev.Id == 0) // INSERT
                {
                    bugetPrev.TenantId = appClient.Id;
                    _bugetPrevRepository.Insert(bugetPrev);
                }

                var bvc_BugetPrevStatus = new BVC_BugetPrevStatus()
                {
                    BugetPrev = bugetPrev,
                    StatusDate = bugetPrev.DataBuget,
                    Status = (BVC_Status)form.Status
                };
                _bugetPrevStatusRepository.Insert(bvc_BugetPrevStatus);

                // iau repartizarea veniturilor pentru anul bugetului
                var venitProcRepartiz = _venitProcRepartzRepository.GetAllIncluding(f => f.BVC_VenitBugetPrelim).Where(f => f.BVC_VenitBugetPrelim.FormularId == form.FormularId)
                                                                   .Select(f => new VenitRepartizProc
                                                                   {
                                                                       ActivityTypeId = f.ActivityTypeId,
                                                                       ProcRepartizat = f.ProcRepartiz / 100
                                                                   }).ToList();
                if (venitProcRepartiz.Count == 0)
                {
                    throw new Exception("Nu a fost realizata repartizarea veniturilor pentru anul de buget selectat");
                }

                var departamentList = _salariatiDepartamentRepository.GetAll().Where(f => f.State == State.Active).Select(f => f.DepartamentId).Distinct().ToList();
                // verific daca exista alte departamente in PAAP decat cele din salariati
                var bugetFormular = _bvcFormRandRepository.GetAllIncluding(f => f.Formular).Where(f => f.FormularId == form.FormularId).Select(f => f.Formular).FirstOrDefault();
                var dataStart = new DateTime(bugetFormular.AnBVC, 1, 1);
                var dataEnd = new DateTime(bugetFormular.AnBVC, 12, 31);
                var deptPaap = _bvcPaapRepository.GetAll().Where(f => f.State == State.Active && dataStart <= f.DataEnd && f.DataEnd <= dataEnd).Select(f => f.DepartamentId.Value).Distinct().ToList();
                var newDeptPaap = deptPaap.Where(f => !departamentList.Contains(f)).ToList();
                if (newDeptPaap.Count != 0)
                {
                    departamentList.AddRange(newDeptPaap);
                }

                var bvcFormRanduriList = _bvcFormRandRepository.GetAllIncluding(f => f.Formular).Where(f => f.FormularId == form.FormularId).ToList();
                if (bugetPrev.BVC_Tip == BVC_Tip.BVC)
                {
                    bvcFormRanduriList = bvcFormRanduriList.Where(f => f.AvailableBVC).ToList();
                }
                if (bugetPrev.BVC_Tip == BVC_Tip.CashFlow)
                {
                    bvcFormRanduriList = bvcFormRanduriList.Where(f => f.AvailableCashFlow).ToList();
                }
                var anBVC = bvcFormRanduriList.FirstOrDefault().Formular.AnBVC;
                var activityTypeList = _activityTypeRepository.GetAll().Where(f => f.Status == State.Active && f.TenantId == appClient.Id);

                if (form.BVC_Tip == (int)BVC_Tip.BVC)
                {
                    bvcFormRanduriList = bvcFormRanduriList.Where(f => f.AvailableBVC).ToList();
                }
                if (form.BVC_Tip == (int)BVC_Tip.CashFlow)
                {
                    bvcFormRanduriList = bvcFormRanduriList.Where(f => f.AvailableCashFlow).ToList();
                }

                foreach (var dep in departamentList)
                {
                    foreach (var rand in bvcFormRanduriList)
                    {
                        var bvcPrevRand = new BVC_BugetPrevRand
                        {
                            BugetPrev = bugetPrev,
                            FormRandId = rand.Id,
                            DepartamentId = dep,
                            State = State.Active,
                            TenantId = appClient.Id
                        };

                        _bugetPrevRandRepository.Insert(bvcPrevRand);
                        //CurrentUnitOfWork.SaveChanges();
                    }
                }
                CurrentUnitOfWork.SaveChanges();
                form.Id = bugetPrev.Id;

                if (bugetPrev.BVC_Tip == BVC_Tip.BVC)
                {
                    //contributii
                    try
                    {
                        //_bugetPrevRepository.BVC_PrevContributii(bugetPrev.Id);
                        //AddPrevStatusCalc(bugetPrev.Id, BVC_BugetPrevElemCalc.Contributii, true, "OK", appClient.Id);
                        _bugetPrevRepository.BVC_PrevIncasari(bugetPrev.Id, BVC_BugetPrevContributieTipIncasare.Contributii);
                        AddPrevStatusCalc(bugetPrev.Id, BVC_BugetPrevElemCalc.Contributii, true, "OK", appClient.Id);
                    }
                    catch (Exception ex)
                    {
                        AddPrevStatusCalc(bugetPrev.Id, BVC_BugetPrevElemCalc.Contributii, false, "Eroare generare venituri contributii - " + ex.ToString(), appClient.Id);
                    }
                    //creante
                    try
                    {
                        //_bugetPrevRepository.BVC_PrevCreante(bugetPrev.Id);
                        //AddPrevStatusCalc(bugetPrev.Id, BVC_BugetPrevElemCalc.Creante, true, "OK", appClient.Id);
                        _bugetPrevRepository.BVC_PrevIncasari(bugetPrev.Id, BVC_BugetPrevContributieTipIncasare.Creante);
                        AddPrevStatusCalc(bugetPrev.Id, BVC_BugetPrevElemCalc.Creante, true, "OK", appClient.Id);
                    }
                    catch (Exception ex)
                    {
                        AddPrevStatusCalc(bugetPrev.Id, BVC_BugetPrevElemCalc.Creante, false, "Eroare generare venituri creante - " + ex.ToString(), appClient.Id);
                    }

                    //altele
                    try
                    {
                        //_bugetPrevRepository.BVC_PrevCreante(bugetPrev.Id);
                        //AddPrevStatusCalc(bugetPrev.Id, BVC_BugetPrevElemCalc.Creante, true, "OK", appClient.Id);
                        _bugetPrevRepository.BVC_PrevIncasari(bugetPrev.Id, BVC_BugetPrevContributieTipIncasare.Altele);
                        AddPrevStatusCalc(bugetPrev.Id, BVC_BugetPrevElemCalc.Altele, true, "OK", appClient.Id);
                    }
                    catch (Exception ex)
                    {
                        AddPrevStatusCalc(bugetPrev.Id, BVC_BugetPrevElemCalc.Altele, false, "Eroare generare venituri altele - " + ex.ToString(), appClient.Id);
                    }

                    // salarizare
                    try
                    {
                        var salarizareBVC = _activeBugetBVCManager.SalarizareBVCList(anBVC);
                        _bugetPrevRepository.BVC_PrevSalarizare(salarizareBVC, bugetPrev.Id, venitProcRepartiz);
                        AddPrevStatusCalc(bugetPrev.Id, BVC_BugetPrevElemCalc.Salarizare, true, "OK", appClient.Id);
                    }
                    catch (Exception ex)
                    {
                        AddPrevStatusCalc(bugetPrev.Id, BVC_BugetPrevElemCalc.Salarizare, false, "Eroare generare cheltuieli salarizare - " + ex.ToString(), appClient.Id);
                    }

                    // inserez amortizarile MF
                    try
                    {
                        _bugetPrevRepository.BVC_PrevAmortizariMF(bugetPrev.Id, venitProcRepartiz);
                        AddPrevStatusCalc(bugetPrev.Id, BVC_BugetPrevElemCalc.AmortizariMijFixe, true, "OK", appClient.Id);
                    }
                    catch (Exception ex)
                    {
                        AddPrevStatusCalc(bugetPrev.Id, BVC_BugetPrevElemCalc.AmortizariMijFixe, false, "Eroare generare amortizari mijloace fixe - " + ex.ToString(), appClient.Id);
                    }

                    // inserez amortizarile CA
                    try
                    {
                        _bugetPrevRepository.BVC_PrevAmortizariCA(bugetPrev.Id, venitProcRepartiz);
                        AddPrevStatusCalc(bugetPrev.Id, BVC_BugetPrevElemCalc.AmortizariCheltAvans, true, "OK", appClient.Id);
                    }
                    catch (Exception ex)
                    {
                        AddPrevStatusCalc(bugetPrev.Id, BVC_BugetPrevElemCalc.AmortizariCheltAvans, false, "Eroare generare amortizari chelt avans - " + ex.ToString(), appClient.Id);
                    }
                    // inserez inregistrarile din PAAP
                    try
                    {
                        _bugetPrevRepository.BVC_PrevPAAP(bugetPrev.Id, venitProcRepartiz);
                        AddPrevStatusCalc(bugetPrev.Id, BVC_BugetPrevElemCalc.PAAP, true, "OK", appClient.Id);
                    }
                    catch (Exception ex)
                    {
                        AddPrevStatusCalc(bugetPrev.Id, BVC_BugetPrevElemCalc.PAAP, false, "Eroare generare cheltuieli PAAP - " + ex.ToString(), appClient.Id);
                    }

                    //cheltuieli
                    try
                    {
                        _bugetPrevRepository.BVC_PrevCheltuieli(bugetPrev.Id);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Eroare generare bvc cheltuieli -" + ex.ToString());
                    }

                    // totaluri
                    try
                    {
                        _bugetPrevRepository.BVC_PrevTotaluri(bugetPrev.Id);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Eroare calcul totaluri - " + ex.ToString());
                    }
                }
                else // cash flow
                {
                    // contributii
                    try
                    {
                        //_bugetPrevRepository.BVC_CashFlowContributii(bugetPrev.Id);
                        //AddPrevStatusCalc(bugetPrev.Id, BVC_BugetPrevElemCalc.Contributii, true, "OK", appClient.Id);
                        _bugetPrevRepository.BVC_CashFlowIncasari(bugetPrev.Id, BVC_BugetPrevContributieTipIncasare.Contributii);
                        AddPrevStatusCalc(bugetPrev.Id, BVC_BugetPrevElemCalc.Contributii, true, "OK", appClient.Id);
                    }
                    catch (Exception ex)
                    {
                        AddPrevStatusCalc(bugetPrev.Id, BVC_BugetPrevElemCalc.Contributii, false, "Eroare generare cashflow contributii - " + ex.ToString(), appClient.Id);
                    }

                    // creante
                    try
                    {
                        //_bugetPrevRepository.BVC_CashFlowCreante(bugetPrev.Id);
                        _bugetPrevRepository.BVC_CashFlowIncasari(bugetPrev.Id, BVC_BugetPrevContributieTipIncasare.Creante);
                        AddPrevStatusCalc(bugetPrev.Id, BVC_BugetPrevElemCalc.Creante, true, "OK", appClient.Id);
                    }
                    catch (Exception ex)
                    {
                        AddPrevStatusCalc(bugetPrev.Id, BVC_BugetPrevElemCalc.Creante, false, "Eroare generare cashflow creante - " + ex.ToString(), appClient.Id);
                    }

                    // Altele
                    try
                    {
                        //_bugetPrevRepository.BVC_CashFlowCreante(bugetPrev.Id);
                        _bugetPrevRepository.BVC_CashFlowIncasari(bugetPrev.Id, BVC_BugetPrevContributieTipIncasare.Altele);
                        AddPrevStatusCalc(bugetPrev.Id, BVC_BugetPrevElemCalc.Altele, true, "OK", appClient.Id);
                    }
                    catch (Exception ex)
                    {
                        AddPrevStatusCalc(bugetPrev.Id, BVC_BugetPrevElemCalc.Altele, false, "Eroare generare cashflow altele - " + ex.ToString(), appClient.Id);
                    }

                    // salarizare
                    try
                    {
                        var salarizareCF = _activeBugetBVCManager.SalarizareCFList(anBVC);
                        _bugetPrevRepository.BVC_CashFlowSalarizare(salarizareCF, bugetPrev.Id, venitProcRepartiz);
                        AddPrevStatusCalc(bugetPrev.Id, BVC_BugetPrevElemCalc.Salarizare, true, "OK", appClient.Id);
                    }
                    catch (Exception ex)
                    {
                        AddPrevStatusCalc(bugetPrev.Id, BVC_BugetPrevElemCalc.Salarizare, false, "Eroare generare cheltuieli salarizare - " + ex.ToString(), appClient.Id);
                    }

                    //paap
                    try
                    {
                        _bugetPrevRepository.BVC_CashFlowPAAP(bugetPrev.Id, venitProcRepartiz);
                        AddPrevStatusCalc(bugetPrev.Id, BVC_BugetPrevElemCalc.PAAP, true, "OK", appClient.Id);
                    }
                    catch (Exception ex)
                    {
                        AddPrevStatusCalc(bugetPrev.Id, BVC_BugetPrevElemCalc.PAAP, false, "Eroare generare cashflow PAAP - " + ex.ToString(), appClient.Id);
                    }

                    // cheltuieli
                    try
                    {
                        _bugetPrevRepository.BVC_CashFlowCheltuieli(bugetPrev.Id);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Eroare generare cashflow cheltuieli - " + ex.ToString());
                    }

                    // totaluri
                    try
                    {
                        _bugetPrevRepository.BVC_PrevTotaluri(bugetPrev.Id);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Eroare calcul totaluri - " + ex.ToString());
                    }
                }

                return form;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        //public BugetPrevItemDto GetBugetPrevDetails(int? departamentId, int bugetPrevId, int month)
        //{
        //    try
        //    {
        //        var formularId = _bugetPrevRepository.FirstOrDefault(f => f.Id == bugetPrevId).FormularId;
        //        var status = _bugetPrevRepository.GetAllIncluding(f => f.StatusList).Where(f => f.Id == bugetPrevId).FirstOrDefault().Status.ToString();
        //        var bugetPrev = new BugetPrevItemDto();
        //        bugetPrev.Status = status;

        //        if (departamentId != null && departamentId != 0)
        //        {
        //            var bugetPrevByDep = new BugetPrevByDepartmentDto();
        //            var departament = _salariatiDepartamentRepository.GetAllIncluding(f => f.Departament).FirstOrDefault(f => f.DepartamentId == departamentId);
        //            //var bugetPrevDetailList = _bugetPrevRandValueRepository.GetAllIncluding(f => f.BugetPrevRand.Departament, f => f.BugetPrevRand.FormRand, f => f.BugetPrevRand,
        //            //                                                    f => f.BugetPrevRand.BugetPrev.Formular, f => f.BugetPrevRand.ValueList,
        //            //                                                    f => f.ActivityType)
        //            //                                   .Where(f => f.BugetPrevRand.BugetPrev.Id == bugetPrevId && f.BugetPrevRand.DepartamentId == departament.DepartamentId
        //            //                                          && f.DataLuna.Month == (month != 0 ? month : f.DataLuna.Month))
        //            //                                   .OrderBy(f => f.BugetPrevRand.FormRand.OrderView)
        //            //                                   .ToList();

        //            var bugetPrevItem = new BugetPrevByDepartmentDto
        //            {
        //                DepartamentId = departament.DepartamentId,
        //                DepartamentName = departament.Departament.Name,
        //                Details = new List<BugetPrevDetailDto>()
        //            };

        //            //foreach (var det in bugetPrevDetailList.GroupBy(f => f.BugetPrevRandId).Distinct())
        //            //{
        //            //    var bugetRandValue = _bugetPrevRandValueRepository.GetAllIncluding(f => f.BugetPrevRand, f => f.BugetPrevRand.FormRand)
        //            //                                                      .Where(f => f.BugetPrevRandId == det.Key && f.DataLuna.Month == (month != 0 ? month : f.DataLuna.Month))
        //            //                                                      .OrderBy(f => f.BugetPrevRand.FormRand.OrderView)
        //            //                                                      .ToList();
        //            //    //if (month != 0)
        //            //    //{
        //            //    //    bugetRandValue = bugetRandValue.Where(f => f.DataLuna.Month == month).ToList();
        //            //    //}

        //            //    var bugetRand = _bugetPrevRandRepository.GetAllIncluding(f => f.FormRand).FirstOrDefault(f => f.Id == det.Key);
        //            //    var detail = new BugetPrevDetailDto
        //            //    {
        //            //        Descriere = bugetRand.FormRand.Descriere,
        //            //        Validat = bugetRand.Validat,
        //            //        Valoare = bugetRandValue.Where(f => f.BugetPrevRandId == bugetRand.Id).Sum(f => f.Value),
        //            //        IsTotal = bugetRand.FormRand.IsTotal,
        //            //        RandValueDetails = new List<BugetPrevDetailRandValueDto>()
        //            //    };

        //            //    foreach (var value in bugetRandValue.OrderBy(f => f.DataLuna).ThenBy(f => f.DataOper).ThenBy(f => f.ActivityTypeId))
        //            //    {
        //            //        var randValueDetail = new BugetPrevDetailRandValueDto()
        //            //        {
        //            //            ActivityType = value.ActivityType.ActivityName,
        //            //            Descriere = value.Description,
        //            //            Valoare = value.Value,
        //            //            BugetPrevRandId = value.BugetPrevRandId,
        //            //            DataLuna = value.DataLuna,
        //            //            DataOper = value.DataOper,
        //            //            ValueType = (int)value.ValueType
        //            //        };
        //            //        detail.RandValueDetails.Add(randValueDetail);
        //            //    }
        //            //    bugetPrevItem.Details.Add(detail);
        //            //}
        //            //bugetPrevByDep = bugetPrevItem;
        //            //bugetPrev.BugetPrevByDepartmentList = bugetPrevByDep;
        //            //bugetPrev.IsValidated = bugetPrevByDep.Details.All(f => f.Validat == true);

        //            var retList = _bugetPrevRandValueRepository.GetAllIncluding(f => f.BugetPrevRand, f => f.BugetPrevRand.FormRand, f=>f.ActivityType)
        //                                                       .Where(f => f.BugetPrevRand.BugetPrevId == bugetPrevId && f.DataLuna.Month == (month != 0 ? month : f.DataLuna.Month)
        //                                                              && f.BugetPrevRand.DepartamentId == departamentId)
        //                                                       .ToList()
        //                                                       .GroupBy(f => new
        //                                                       {
        //                                                           Id = f.BugetPrevRand.FormRand.Id,
        //                                                           Descriere = f.BugetPrevRand.FormRand.Descriere,
        //                                                           OrderView = f.BugetPrevRand.FormRand.OrderView,
        //                                                           Validat = f.BugetPrevRand.Validat,
        //                                                           IsTotal = f.BugetPrevRand.FormRand.IsTotal
        //                                                       })
        //                                                       .OrderBy(f => f.Key.OrderView)
        //                                                       .Select(f => new BugetPrevDetailDto
        //                                                       {
        //                                                           Descriere = f.Key.Descriere,
        //                                                           Validat = f.Key.Validat,
        //                                                           Valoare = f.Sum(g => g.Value),
        //                                                           IsTotal = f.Key.IsTotal,
        //                                                           RandValueDetails = f.Select(g=> new BugetPrevDetailRandValueDto
        //                                                           {
        //                                                               Id = g.Id,
        //                                                               ActivityType = g.ActivityType.ActivityName,
        //                                                               Descriere = g.Description,
        //                                                               Valoare = g.Value,
        //                                                               BugetPrevRandId = g.BugetPrevRandId,
        //                                                               DataLuna = g.DataLuna,
        //                                                               DataOper = g.DataOper,
        //                                                               ValueType = (int)g.ValueType
        //                                                           }).OrderBy(f=>f.DataOper).ThenBy(f=>f.ValueType).ThenBy(f=>f.ActivityType).ToList()
        //                                                       })
        //                                                       .ToList();
        //            bugetPrevItem.Details = retList;
        //            bugetPrevByDep = bugetPrevItem;
        //            bugetPrev.BugetPrevByDepartmentList = bugetPrevByDep;
        //            bugetPrev.IsValidated = bugetPrevByDep.Details.All(f => f.Validat == true);
        //        }

        //        if (departamentId == 0)
        //        {
        //            //var bugetPrevAllDep = new List<BugetPrevAllDepartmentsDto>();
        //            //var bvcFormRandList = _bvcFormRandRepository.GetAllIncluding(f => f.Formular).Where(f => f.FormularId == formularId).OrderBy(f => f.OrderView).Select(f => f.Id);
        //            //var bugetPrevDetailList = _bugetPrevRandValueRepository.GetAllIncluding(f => f.BugetPrevRand.Departament, f => f.BugetPrevRand.FormRand, f => f.BugetPrevRand,
        //            //                                f => f.BugetPrevRand.BugetPrev.Formular, f => f.BugetPrevRand.ValueList,
        //            //                                f => f.ActivityType)
        //            //               .Where(f => f.BugetPrevRand.BugetPrevId == bugetPrevId && bvcFormRandList.Contains(f.BugetPrevRand.FormRandId) && f.DataLuna.Month == (month != 0 ? month : f.DataLuna.Month))
        //            //               .OrderBy(f => f.BugetPrevRand.FormRand.OrderView)
        //            //               .ToList();
        //            ////if (month != 0)
        //            ////{
        //            ////    bugetPrevDetailList = bugetPrevDetailList.Where(f => f.DataLuna.Month == month).ToList();
        //            ////}

        //            //foreach (var item in bugetPrevDetailList.GroupBy(f => f.BugetPrevRand.FormRandId).Distinct())
        //            //{
        //            //    var detail = bugetPrevDetailList.Where(f => f.BugetPrevRand.FormRandId == item.Key).GroupBy(f => new
        //            //    {
        //            //        Id = f.BugetPrevRand.FormRandId,
        //            //        Descriere = f.BugetPrevRand.FormRand.Descriere,
        //            //        Validat = bugetPrevDetailList.All(f => f.BugetPrevRand.Validat == true),
        //            //    }).Select(f => new BugetPrevAllDepartmentsDto
        //            //    {
        //            //        FormRandId = f.Key.Id,
        //            //        Descriere = f.Key.Descriere,
        //            //        Validat = f.Key.Validat,
        //            //        Valoare = f.Sum(g => g.Value)
        //            //    }).Distinct().ToList();

        //            //    bugetPrevAllDep.AddRange(detail);
        //            //}
        //            //bugetPrev.BugetPrevAllDepartmentsList = bugetPrevAllDep;
        //            //bugetPrev.IsValidated = bugetPrevAllDep.All(f => f.Validat == true);

        //            var retList = _bugetPrevRandValueRepository.GetAllIncluding(f => f.BugetPrevRand, f => f.BugetPrevRand.FormRand)
        //                                                       .Where(f=>f.BugetPrevRand.BugetPrevId == bugetPrevId && f.DataLuna.Month == (month != 0 ? month : f.DataLuna.Month))
        //                                                       .ToList()
        //                                                       .GroupBy(f => new
        //                                                       {
        //                                                           Id = f.BugetPrevRand.FormRand.Id,
        //                                                           Descriere = f.BugetPrevRand.FormRand.Descriere,
        //                                                           OrderView = f.BugetPrevRand.FormRand.OrderView
        //                                                       })
        //                                                       .OrderBy(f=>f.Key.OrderView)
        //                                                       .Select(f=> new BugetPrevAllDepartmentsDto
        //                                                       {
        //                                                           FormRandId = f.Key.Id,
        //                                                           Descriere = f.Key.Descriere,
        //                                                           Valoare = f.Sum(g=>g.Value),
        //                                                           Validat = (f.Min(g=> (g.BugetPrevRand.Validat ? 1 : 0)) == 0 ? false : true)
        //                                                       })
        //                                                       .ToList();
        //            bugetPrev.BugetPrevAllDepartmentsList = retList;
        //            bugetPrev.IsValidated = retList.All(f => f.Validat == true);

        //        }

        //        return bugetPrev;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new UserFriendlyException("Eroare", ex.ToString());
        //    }
        //}

        public BugetPrevItemDto GetBugetPrevDetailsV2(int? departamentId, int bugetPrevId, string month, bool values0)
        {
            try
            {
                var appClient = GetCurrentTenant();
                var activityTypeList = _activityTypeRepository.GetAll().Where(f => f.Status == State.Active && f.TenantId == appClient.Id).ToList();
                var bugetPrevDb = _bugetPrevRepository.GetAllIncluding(f => f.Formular).FirstOrDefault(f => f.Id == bugetPrevId);
                var formularId = bugetPrevDb.FormularId;
                var anBVC = bugetPrevDb.Formular.AnBVC;
                var status = _bugetPrevRepository.GetAllIncluding(f => f.StatusList).Where(f => f.Id == bugetPrevId).FirstOrDefault().Status.ToString();
                var bugetPrev = new BugetPrevItemDto();
                bugetPrev.Status = status;
                var dataLunaSel = new DateTime();
                if (month != "all")
                {
                    dataLunaSel = DateTime.ParseExact(month, "ddMMyyyy", null);
                }

                if (departamentId != null && departamentId != 0)
                {
                    var bugetPrevByDep = new BugetPrevByDepartmentDto();
                    var departament = _salariatiDepartamentRepository.GetAllIncluding(f => f.Departament).FirstOrDefault(f => f.DepartamentId == departamentId);

                    var bugetPrevItem = new BugetPrevByDepartmentDto
                    {
                        DepartamentId = departament.DepartamentId,
                        DepartamentName = departament.Departament.Name,
                        Details = new List<BugetPrevDetailDto>()
                    };

                    var retList = _bugetPrevRandValueRepository.GetAllIncluding(f => f.BugetPrevRand, f => f.BugetPrevRand.FormRand, f => f.ActivityType)
                                                           .Where(f => f.BugetPrevRand.BugetPrevId == bugetPrevId && f.DataLuna == (month != "all" ? dataLunaSel : f.DataLuna)
                                                                  && f.BugetPrevRand.DepartamentId == departamentId/* && f.ValueType == BVC_PrevValueType.Manual*/)
                                                           .ToList()
                                                           .GroupBy(f => new
                                                           {
                                                               Id = f.BugetPrevRand.FormRand.Id,
                                                               Descriere = f.BugetPrevRand.FormRand.Descriere,
                                                               OrderView = f.BugetPrevRand.FormRand.OrderView,
                                                               Validat = f.BugetPrevRand.Validat,
                                                               IsTotal = f.BugetPrevRand.FormRand.IsTotal
                                                           })
                                                           .OrderBy(f => f.Key.OrderView)
                                                           .Select(f => new BugetPrevDetailDto
                                                           {
                                                               Id = f.Key.Id,
                                                               Descriere = f.Key.Descriere,
                                                               OrderView = f.Key.OrderView,
                                                               Validat = f.Key.Validat,
                                                               Valoare = f.Sum(g => g.Value),
                                                               IsTotal = f.Key.IsTotal,
                                                               RandValueDetails = f.Select(g => new BugetPrevDetailRandValueDto
                                                               {
                                                                   Id = g.Id,
                                                                   ActivityTypeName = g.ActivityType.ActivityName,
                                                                   Descriere = g.Description,
                                                                   Valoare = g.Value,
                                                                   BugetPrevRandId = g.BugetPrevRandId,
                                                                   DataLuna = g.DataLuna,
                                                                   DataOper = g.DataOper,
                                                                   ValueType = (int)g.ValueType
                                                               }).OrderBy(f => f.DataOper).ThenBy(f => f.ValueType).ThenBy(f => f.ActivityTypeName).ToList()
                                                           })
                                                           .ToList();

                    /*  var retListManual = retList.Where(f => f.RandValueDetails.Any(g => g.ValueType == (int)BVC_PrevValueType.Manual)).ToList()*/
                    ;
                    var randList = _bugetPrevRandRepository.GetAllIncluding(f => f.Departament, f => f.BugetPrev, f => f.FormRand)
                                                            .Where(f => f.BugetPrevId == bugetPrevId && f.DepartamentId == departamentId)
                                                            .ToList();
                    //.Where(f => !retList.Any(g => g.Descriere == f.FormRand.Descriere)).ToList();

                    //var retListManual = new List<BugetPrevDetailDto>();
                    DateTime startDate = (month != "all" ? dataLunaSel : new DateTime(bugetPrevDb.Formular.AnBVC, bugetPrevDb.MonthStart, 1));
                    DateTime endDate = (month != "all" ? dataLunaSel : LazyMethods.LastDayOfMonth(new DateTime(bugetPrevDb.Formular.AnBVC, bugetPrevDb.MonthEnd, 1)));

                    for (DateTime i = startDate; i <= endDate; i = LazyMethods.LastDayOfMonth(i.AddMonths(1)))
                    {
                        var dataLuna = i;// LazyMethods.LastDayOfMonth(new DateTime(anBVC, i, 1));

                        foreach (var rand in randList)
                        {
                            foreach (var activity in activityTypeList)
                            {
                                var existingRow = retList.FirstOrDefault(f => f.Id == rand.FormRandId);

                                if (existingRow == null)
                                {
                                    existingRow = new BugetPrevDetailDto
                                    {
                                        Id = rand.FormRandId,
                                        Descriere = rand.FormRand.Descriere,
                                        OrderView = rand.FormRand.OrderView,
                                        Validat = false,
                                        Valoare = 0,
                                        IsTotal = rand.FormRand.IsTotal,
                                        RandValueDetails = new List<BugetPrevDetailRandValueDto>()
                                    };
                                    retList.Add(existingRow);
                                }

                                var existingValue = existingRow.RandValueDetails
                                                               .FirstOrDefault(f => f.ActivityTypeName == activity.ActivityName
                                                                && (BVC_PrevValueType)f.ValueType == BVC_PrevValueType.Manual);
                                if (existingValue == null)
                                {
                                    var randValueDetails = new BugetPrevDetailRandValueDto
                                    {
                                        Id = 0,
                                        Descriere = null,
                                        Valoare = 0,
                                        BugetPrevRandId = rand.Id,
                                        DataLuna = dataLuna,
                                        DataOper = dataLuna,
                                        ValueType = (int)BVC_PrevValueType.Manual,
                                        ActivityTypeName = activity.ActivityName,
                                        ActivityTypeId = activity.Id
                                    };
                                    existingRow.RandValueDetails.Add(randValueDetails);
                                }
                            }
                        }
                    }

                    bugetPrevItem.Details = retList.OrderBy(f => f.OrderView).ToList();
                    // bugetPrevItem.Details = retListManual;
                    bugetPrevByDep = bugetPrevItem;
                    bugetPrev.BugetPrevByDepartmentList = bugetPrevByDep;
                    bugetPrev.IsValidated = bugetPrevByDep.Details.All(f => f.Validat == true);
                }

                if (departamentId == 0)
                {
                    var retList = _bugetPrevRepository.GetBugetPrevDetails(departamentId, bugetPrevId, month);
                    bugetPrev.BugetPrevAllDepartmentsList = retList;
                    bugetPrev.IsValidated = retList.All(f => f.Validat == true);
                }

                if (!values0)
                {
                    if (bugetPrev.BugetPrevAllDepartmentsList != null)
                    {
                        bugetPrev.BugetPrevAllDepartmentsList = bugetPrev.BugetPrevAllDepartmentsList.Where(f => f.Valoare != 0).ToList();
                    }
                    if (bugetPrev.BugetPrevByDepartmentList != null)
                    {
                        if (bugetPrev.BugetPrevByDepartmentList.Details != null)
                        {
                            bugetPrev.BugetPrevByDepartmentList.Details = bugetPrev.BugetPrevByDepartmentList.Details.Where(f => f.Valoare != 0).ToList();
                        }
                    }
                }
                return bugetPrev;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public List<BugetPrevMonthsDto> GetBugetPrevDataLunaList(int bugetPrevId)
        {
            var list = _bugetPrevRandValueRepository.GetAllIncluding(f => f.BugetPrevRand)
                                                    .Where(f => f.BugetPrevRand.BugetPrevId == bugetPrevId)
                                                    .Select(f => new { f.DataLuna })
                                                    .GroupBy(f => f.DataLuna)
                                                    .Select(f => f.Key)
                                                    .OrderBy(f => f)
                                                    .ToList();
            var ret = new List<BugetPrevMonthsDto>();
            ret.Add(new BugetPrevMonthsDto { Month = "all", MonthDisplay = "Toate" });
            foreach (var item in list)
            {
                ret.Add(new BugetPrevMonthsDto
                {
                    Month = item.ToString("ddMMyyyy"),
                    MonthDisplay = item.ToString("dd.MM.yyyy")
                });
            }
            return ret;
        }

        [AbpAuthorize("Buget.BVC.Prevazut.Modificare")]
        public void Delete(int bugetPrevId)
        {
            try
            {
                var bugetPrev = _bugetPrevRepository.FirstOrDefault(f => f.Id == bugetPrevId);
                //var bugetPrevRand = _bugetPrevRandRepository.GetAllIncluding(f => f.BugetPrev).Where(f => f.BugetPrevId == bugetPrevId);
                //var bugetPrevStatus = _bugetPrevStatusRepository.GetAllIncluding(f => f.BugetPrev).Where(f => f.BugetPrevId == bugetPrevId);

                //foreach (var bugetStatus in bugetPrevStatus.ToList())
                //{
                //    _bugetPrevStatusRepository.Delete(bugetStatus);
                //    //CurrentUnitOfWork.SaveChanges();
                //}

                //foreach (var rand in bugetPrevRand.ToList())
                //{
                //    var bugetPrevRandValue = _bugetPrevRandValueRepository.GetAllIncluding(f => f.BugetPrevRand).Where(f => f.BugetPrevRandId == rand.Id);
                //    foreach (var randValue in bugetPrevRandValue.ToList())
                //    {
                //        _bugetPrevRandValueRepository.Delete(randValue);
                //        //CurrentUnitOfWork.SaveChanges();
                //    }

                //    _bugetPrevRandRepository.Delete(rand);
                //    //CurrentUnitOfWork.SaveChanges();
                //}

                var venitBugetList = _venitBugetPrelim.GetAll().Where(f => f.BVC_BugetPrevId == bugetPrevId).ToList();
                foreach (var item in venitBugetList)
                {
                    _venitBugetPrelim.Delete(item);
                }

                var prevResurseList = _prevResurseRepository.GetAll().Where(f => f.BugetPrevId == bugetPrevId).ToList();
                foreach (var item in prevResurseList)
                {
                    _prevResurseRepository.Delete(item);
                }

                var statusCalcList = _bugetPrevStatCalculRepository.GetAll().Where(f => f.BugetPrevId == bugetPrevId).ToList();
                foreach (var item in statusCalcList)
                {
                    _bugetPrevStatCalculRepository.Delete(item.Id);
                }

                _bugetPrevRepository.Delete(bugetPrev);
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        [AbpAuthorize("Buget.BVC.Prevazut.Modificare")]
        public void Save(BugetPrevByDepartmentDto buget, int bugetPrevId, int departamentId, string month)
        {
            try
            {
                if (departamentId != 0) // pentru un departament selectat
                {
                    //foreach (var detail in buget.Details)
                    //{
                    //    foreach (var det in detail.RandValueDetails)
                    //    {
                    //        var activityTypeId = _activityTypeRepository.FirstOrDefault(f => f.ActivityName == det.ActivityType).Id;
                    //        var bugetRandValue = _bugetPrevRandValueRepository.GetAll().Where(f => f.BugetPrevRandId == det.BugetPrevRandId &&
                    //                                                                               f.ActivityTypeId == activityTypeId && f.DataLuna.Month == month);
                    //        var bugetPrev = _bugetPrevRandRepository.GetAll().FirstOrDefault(f => f.Id == det.BugetPrevRandId);
                    //        bugetPrevId = bugetPrev.BugetPrevId;

                    //        foreach (var randVal in bugetRandValue.ToList())
                    //        {
                    //            var randValue = _bugetPrevRandValueRepository.FirstOrDefault(f => f.Id == randVal.Id);
                    //            randValue.Description = det.Descriere;
                    //            randValue.Value = det.Valoare;
                    //            _bugetPrevRandValueRepository.Update(randValue);
                    //            CurrentUnitOfWork.SaveChanges();
                    //        }
                    //    }
                    //}

                    var appClient = GetCurrentTenant();
                    foreach (var detail in buget.Details)
                    {
                        foreach (var det in detail.RandValueDetails.Where(f => f.Valoare != 0))
                        {
                            var activityType = _activityTypeRepository.FirstOrDefault(f => f.ActivityName == det.ActivityTypeName);
                            det.ActivityTypeId = activityType.Id;
                            var bugetRandValue = ObjectMapper.Map<BVC_BugetPrevRandValue>(det)
                                ;/*_bugetPrevRandValueRepository.GetAll().FirstOrDefault(f => f.Id == det.Id)*/
                            ;
                            bugetRandValue.TenantId = appClient.Id;
                            if (bugetRandValue.Id == 0)
                            {
                                _bugetPrevRandValueRepository.Insert(bugetRandValue);
                            }
                            else
                            {
                                bugetRandValue.Description = det.Descriere;
                                bugetRandValue.Value = det.Valoare;
                                bugetRandValue.DataOper = det.DataOper;
                                _bugetPrevRandValueRepository.Update(bugetRandValue);
                            }
                            //var bugetRandValue = _bugetPrevRandValueRepository.GetAll().FirstOrDefault(f => f.Id == det.Id);
                            //bugetRandValue.Description = det.Descriere;
                            //bugetRandValue.Value = det.Valoare;
                            //bugetRandValue.DataOper = det.DataOper;
                            //_bugetPrevRandValueRepository.Update(bugetRandValue);
                        }
                        CurrentUnitOfWork.SaveChanges();
                    }

                    // recalculez totalurile
                    _bugetPrevRepository.BVC_PrevTotaluri(bugetPrevId);
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        [AbpAuthorize("Buget.BVC.Prevazut.Validare")]
        public void ValidateAll(int bugetPrevId)
        {
            try
            {
                //var formularId = _bugetPrevRepository.FirstOrDefault(f => f.Id == bugetPrevId).FormularId;
                //var departamentList = _salariatiDepartamentRepository.GetAll().Where(f => f.State == State.Active).Select(f => f.DepartamentId).Distinct().ToList();
                //var bvcFormRandList = _bvcFormRandRepository.GetAllIncluding(f => f.Formular).Where(f => f.FormularId == formularId).Select(f => f.Id);
                //var bugetPrevRandValueList = _bugetPrevRandValueRepository.GetAllIncluding(f => f.BugetPrevRand, f => f.BugetPrevRand.FormRand)
                //                   .Where(f => bvcFormRandList.Contains(f.BugetPrevRand.FormRandId)).Select(f => f.BugetPrevRandId);

                //var bugetRandList = _bugetPrevRandRepository.GetAllIncluding(f => f.Departament).Where(f => f.Validat == false && bugetPrevRandValueList.Contains(f.Id) && f.BugetPrevId == bugetPrevId);
                //foreach (var dep in departamentList)
                //{
                //    foreach (var rand in bugetRandList.Where(f => f.DepartamentId == dep))
                //    {
                //        rand.Validat = true;
                //    }
                //    CurrentUnitOfWork.SaveChanges();
                //}
                var bugetRows = _bugetPrevRandRepository.GetAll().Where(f => f.BugetPrevId == bugetPrevId).ToList();
                foreach (var item in bugetRows)
                {
                    item.Validat = true;
                }
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        [AbpAuthorize("Buget.BVC.Prevazut.Validare")]
        public void ValidateByDepartament(int bugetPrevId, int departamentId)
        {
            try
            {
                var appClient = GetCurrentTenant();
                //var formularId = _bugetPrevRepository.FirstOrDefault(f => f.Id == bugetPrevId).FormularId;
                //var formRandList = _bvcFormRandRepository.GetAllIncluding(f => f.Formular).Where(f => f.FormularId == formularId).Select(f => f.Id);

                //foreach (var formRand in formRandList.ToList())
                //{
                //    var bugetPrevRandValueList = _bugetPrevRandValueRepository.GetAllIncluding(f => f.ActivityType, f => f.BugetPrevRand).Where(f => f.BugetPrevRand.DepartamentId == departamentId &&
                //                                                                               f.BugetPrevRand.FormRandId == formRand && f.BugetPrevRand.BugetPrevId == bugetPrevId).Select(f => f.BugetPrevRandId).Distinct();
                //    var bugetRand = _bugetPrevRandRepository.GetAll().Where(f => bugetPrevRandValueList.Contains(f.Id) && f.State == State.Active).FirstOrDefault();
                //    bugetRand.Validat = true;
                //    CurrentUnitOfWork.SaveChanges();
                //}

                var bugetRows = _bugetPrevRandRepository.GetAll().Where(f => f.BugetPrevId == bugetPrevId && f.DepartamentId == departamentId).ToList();
                foreach (var item in bugetRows)
                {
                    item.Validat = true;
                }
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        [AbpAuthorize("Buget.BVC.Prevazut.Validare")]
        public void CancelAll(int bugetPrevId)
        {
            try
            {
                //var formularId = _bugetPrevRepository.FirstOrDefault(f => f.Id == bugetPrevId).FormularId;
                //var bvcFormRandList = _bvcFormRandRepository.GetAllIncluding(f => f.Formular).Where(f => f.FormularId == formularId).Select(f => f.Id);
                //var bugetRandList = _bugetPrevRandRepository.GetAll().Where(f => bvcFormRandList.Contains(f.FormRandId) && f.Validat == true && f.BugetPrevId == bugetPrevId);

                //foreach (var rand in bugetRandList.ToList())
                //{
                //    rand.Validat = false;
                //    CurrentUnitOfWork.SaveChanges();
                //}
                var bugetRows = _bugetPrevRandRepository.GetAll().Where(f => f.BugetPrevId == bugetPrevId).ToList();
                foreach (var item in bugetRows)
                {
                    item.Validat = false;
                }
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            };
        }

        [AbpAuthorize("Buget.BVC.Prevazut.Validare")]
        public void CancelByDepartament(int bugetPrevId, int departamentId)
        {
            try
            {
                //var formularId = _bugetPrevRepository.FirstOrDefault(f => f.Id == bugetPrevId).FormularId;
                //var bvcFormRandList = _bvcFormRandRepository.GetAllIncluding(f => f.Formular).Where(f => f.FormularId == formularId).Select(f => f.Id);
                //var bugetPrevRandValueList = _bugetPrevRandValueRepository.GetAllIncluding(f => f.ActivityType, f => f.BugetPrevRand)
                //                                                          .Where(f => f.BugetPrevRand.DepartamentId == departamentId && bvcFormRandList.Contains(f.BugetPrevRand.FormRandId))
                //                                                          .Select(f => f.BugetPrevRandId).Distinct();
                //var bugetRandList = _bugetPrevRandRepository.GetAll().Where(f => bugetPrevRandValueList.Contains(f.Id) && f.BugetPrevId == bugetPrevId).ToList();
                //foreach (var rand in bugetRandList)
                //{
                //    rand.Validat = false;
                //    CurrentUnitOfWork.SaveChanges();
                //}
                var bugetRows = _bugetPrevRandRepository.GetAll().Where(f => f.BugetPrevId == bugetPrevId && f.DepartamentId == departamentId).ToList();
                foreach (var item in bugetRows)
                {
                    item.Validat = false;
                }
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        private List<BugetPrevListDto> CheckBugetValidation(List<BugetPrevListDto> bugetPrevList)
        {
            try
            {
                foreach (var item in bugetPrevList)
                {
                    item.IsValidated = _bugetPrevRandRepository.GetAll().Where(f => f.BugetPrevId == item.Id).All(f => f.Validat == true);
                }

                return bugetPrevList;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        [AbpAuthorize("Buget.BVC.Prevazut.SchimbareStare")]
        public void ChangeBugetPrevState(BugetPrevStatusDto bugetPrevStatus)
        {
            try
            {
                _bugetPrevRepository.InsertBVC_BugetPrevStatus(bugetPrevStatus.BugetPrevId, bugetPrevStatus.StatusDate, (BVC_Status)bugetPrevStatus.Status);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public int DuplicateBugetPrev(int bugetPrevId, DateTime dataBuget, string descriere)
        {
            try
            {
                int newBugetId = 0;
                var bugetPrevOld = _bugetPrevRepository.GetAll().FirstOrDefault(f => f.Id == bugetPrevId);
                var appClient = GetCurrentTenant();

                var bugetPrevNew = new BVC_BugetPrev
                {
                    FormularId = bugetPrevOld.FormularId,
                    BVC_Tip = bugetPrevOld.BVC_Tip,
                    DataBuget = dataBuget,
                    Descriere = descriere,
                    State = State.Active,
                    TenantId = appClient.Id
                };
                _bugetPrevRepository.Insert(bugetPrevNew);

                var bvc_BugetPrevStatus = new BVC_BugetPrevStatus()
                {
                    BugetPrev = bugetPrevNew,
                    StatusDate = bugetPrevNew.DataBuget,
                    Status = BVC_Status.Preliminat
                };
                _bugetPrevStatusRepository.Insert(bvc_BugetPrevStatus);

                var randuriOld = _bugetPrevRandRepository.GetAllIncluding(f => f.ValueList).Where(f => f.BugetPrevId == bugetPrevId).ToList();
                foreach (var rand in randuriOld)
                {
                    var randNew = new BVC_BugetPrevRand
                    {
                        BugetPrev = bugetPrevNew,
                        DepartamentId = rand.DepartamentId,
                        FormRandId = rand.FormRandId,
                        State = rand.State,
                        TenantId = appClient.Id
                    };
                    _bugetPrevRandRepository.Insert(randNew);

                    foreach (var value in rand.ValueList)
                    {
                        var valueNew = new BVC_BugetPrevRandValue
                        {
                            BugetPrevRand = randNew,
                            ActivityTypeId = value.ActivityTypeId,
                            DataLuna = value.DataLuna,
                            DataOper = value.DataOper,
                            TenantId = appClient.Id,
                            Value = value.Value,
                            Description = value.Description,
                            ValueType = value.ValueType
                        };
                        _bugetPrevRandValueRepository.Insert(valueNew);
                    }
                }
                CurrentUnitOfWork.SaveChanges();

                return newBugetId;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            };
        }

        //[AbpAuthorize("Buget.BVC.Venituri.Acces")]
        public List<BugetPrevDDDto> BugetPrevDDList(int formularId, int bvcTip)
        {
            try
            {
                var _bvcTip = (BVC_Tip)bvcTip;
                var ret = _bugetPrevRepository.GetAll().Where(f => f.FormularId == formularId && f.State == State.Active && f.BVC_Tip == _bvcTip)
                                              .OrderByDescending(f => f.DataBuget)
                                              .ToList()
                                              .Select(f => new BugetPrevDDDto
                                              {
                                                  Id = f.Id,
                                                  Descriere = LazyMethods.DateToString(f.DataBuget) + " - " + f.Descriere
                                              })
                                              .ToList();
                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        private void AddPrevStatusCalc(int bugetPrevId, BVC_BugetPrevElemCalc elemCalc, bool statusCalc, string message, int tenantId)
        {
            var status = new BVC_BugetPrevStatCalcul
            {
                BugetPrevId = bugetPrevId,
                ElemCalc = elemCalc,
                StatusCalc = statusCalc,
                Message = message,
                TenantId = tenantId
            };
            _bugetPrevStatCalculRepository.Insert(status);
            CurrentUnitOfWork.SaveChanges();
        }

        public List<BugetPrevStatCalculDto> BugetPrevStatCalcList(int bugetPrevId)
        {
            var ret = _bugetPrevStatCalculRepository.GetAll().Where(f => f.BugetPrevId == bugetPrevId)
                                                             .ToList()
                                                             .Select(f => new BugetPrevStatCalculDto
                                                             {
                                                                 Id = f.Id,
                                                                 BugetPrevId = f.BugetPrevId,
                                                                 ElemCalc = f.ElemCalc,
                                                                 ElemCalcStr = f.ElemCalc.ToString(),
                                                                 Message = f.Message,
                                                                 StatusCalc = f.StatusCalc
                                                             })
                                                             .OrderBy(f => f.ElemCalcStr)
                                                             .ToList();
            return ret;
        }

        public List<BugetPrevDDDto> BugetPreliminatLastYear(int formularId)
        {
            try
            {
                var anBvc = _bvcFormRandRepository.GetAllIncluding(f => f.Formular)
                                                  .Where(f => f.FormularId == formularId)
                                                  .Select(f => f.Formular.AnBVC)
                                                  .FirstOrDefault();
                anBvc = anBvc - 1;
                var ret = _bugetPrevRepository.GetAllIncluding(f => f.Formular, f => f.StatusList)
                                              .ToList()
                                              .Where(f => f.Formular.AnBVC == anBvc && f.BVC_Tip == BVC_Tip.BVC && f.Status == BVC_Status.Preliminat)
                                              .Select(f => new BugetPrevDDDto
                                              {
                                                  Id = f.Id,
                                                  Descriere = f.Descriere
                                              })
                                              .OrderBy(f => f.Descriere)
                                              .ToList();
                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public List<BugetPrevDDDto> BugetPreliminatCFLastYear(int formularId)
        {
            try
            {
                var anBvc = _bvcFormRandRepository.GetAllIncluding(f => f.Formular)
                                                  .Where(f => f.FormularId == formularId)
                                                  .Select(f => f.Formular.AnBVC)
                                                  .FirstOrDefault();
                anBvc = anBvc - 1;
                var ret = _bugetPrevRepository.GetAllIncluding(f => f.Formular, f => f.StatusList)
                                              .ToList()
                                              .Where(f => (f.Formular.AnBVC == anBvc) && f.BVC_Tip == BVC_Tip.CashFlow && f.Status == BVC_Status.Preliminat)
                                              .Select(f => new BugetPrevDDDto
                                              {
                                                  Id = f.Id,
                                                  Descriere = f.Descriere
                                              })
                                              .OrderBy(f => f.Descriere)
                                              .ToList();
                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public void CalculResurse(int bugetPrevId, int cashFlowId, int formularId, int tipBVC)
        {
            try
            {
                // sterg inregistrarile pentru bugetul prevazut
                var bugetPrevResurseList = _bugetPrevResurseRepository.GetAll().Where(f => f.BugetPrevId == bugetPrevId);

                foreach (var item in bugetPrevResurseList)
                {
                    _bugetPrevResurseRepository.Delete(item);
                }
                CurrentUnitOfWork.SaveChanges();

                var appClientId = GetCurrentTenant().Id;
                var bugetFormular = _bvcFormularRepository.GetAll().FirstOrDefault(f => f.Id == formularId);
                int anBVC = bugetFormular.AnBVC;
                var startDate = new DateTime(anBVC, 1, 1);
                var endDate = LazyMethods.LastDayOfMonth(new DateTime(anBVC, 12, 1));
                var titluriTrezorerie = _bugetVenitTitluRepository.GetAllIncluding(f => f.Currency, f => f.ActivityType)
                                                          .Where(f => f.FormularId == formularId && f.StartDate >= startDate && f.StartDate <= endDate &&
                                                                      f.Reinvestit == false)
                                                          .OrderBy(f => f.StartDate)
                                                          .ToList()
                                                          .Select(f => new BVC_PrevResurseDetaliiDto
                                                          {
                                                              CurrencyId = f.CurrencyId,
                                                              CurrencyName = f.Currency.CurrencyName,
                                                              IdPlasament = f.IdPlasament,
                                                              NrZilePlasament = (decimal)(LazyMethods.MinDate(f.MaturityDate, endDate) - LazyMethods.MaxDate(f.StartDate, startDate)).TotalDays,
                                                              ProcentDobanda = f.ProcentDobanda,
                                                              SumaInvestita = f.ValoarePlasament,
                                                              ActivityTypeId = f.ActivityTypeId,
                                                              DataStart = f.StartDate,
                                                              DataEnd = f.MaturityDate,
                                                              ActivityTypeName = f.ActivityType.ActivityName,
                                                              DobandaCuvenita = _bugetVenitTitluBVCRepository.GetAll().Where(f => f.BVC_VenitTitluId == f.Id)
                                                                                                                     .ToList().Sum(f => f.DobandaLuna),
                                                              Incasari = _bugetVenitTitluCFRepository.GetAll().Where(f => f.BVC_VenitTitluId == f.Id).ToList().Sum(f => f.DobandaTotala),
                                                              TenantId = f.TenantId,
                                                              TipResursa = "T"
                                                          })
                                                          .ToList();

                var titluriReinvPreliminat = _bugetVenitTitluRepository.GetAllIncluding(f => f.Currency, f => f.ActivityType)
                                                          .Where(f => f.FormularId == formularId && f.StartDate >= startDate && f.StartDate <= endDate &&
                                                                      f.Reinvestit == true)
                                                          .OrderBy(f => f.StartDate)
                                                          .ToList()
                                                          .Select(f => new BVC_PrevResurseDetaliiDto
                                                          {
                                                              CurrencyId = f.CurrencyId,
                                                              CurrencyName = f.Currency.CurrencyName,
                                                              IdPlasament = f.IdPlasament,
                                                              NrZilePlasament = (decimal)(LazyMethods.MinDate(f.MaturityDate, LazyMethods.LastDayOfMonth(endDate)) - LazyMethods.MaxDate(f.StartDate, startDate)).TotalDays,
                                                              ProcentDobanda = f.ProcentDobanda,
                                                              SumaInvestita = f.ValoarePlasament,
                                                              ActivityTypeId = f.ActivityTypeId,
                                                              DataStart = f.StartDate,
                                                              DataEnd = f.MaturityDate,
                                                              ActivityTypeName = f.ActivityType.ActivityName,
                                                              DobandaCuvenita = _bugetVenitTitluBVCRepository.GetAll().Where(f => f.BVC_VenitTitluId == f.Id)
                                                                                                                     .ToList().Sum(f => f.DobandaLuna),
                                                              Incasari = _bugetVenitTitluCFRepository.GetAll().Where(f => f.BVC_VenitTitluId == f.Id).ToList().Sum(f => f.DobandaTotala),
                                                              TenantId = f.TenantId,
                                                              TipResursa = "P"
                                                          })
                                                          .ToList();

                var titluriReinvestite = _bugetVenitTitluCFReinvRepository.GetAllIncluding(f => f.BVC_VenitTitluCF, f => f.BVC_VenitTitluCF.BVC_VenitTitlu, f => f.BVC_VenitTitluCF.BVC_VenitTitlu.ActivityType,
                                                                                            f => f.BVC_VenitTitluCF.BVC_VenitTitlu.Currency)
                                                                            .Where(f => f.BVC_VenitTitluCF.BVC_VenitTitlu.FormularId == formularId && f.BVC_VenitTitluCF.DataIncasare >= startDate &&
                                                                                        f.BVC_VenitTitluCF.DataIncasare <= endDate)
                                                                            .OrderBy(f => f.BVC_VenitTitluCF.DataIncasare)
                                                                            .ToList()
                                                                            .Select(f => new BVC_PrevResurseDetaliiDto
                                                                            {
                                                                                CurrencyId = f.BVC_VenitTitluCF.BVC_VenitTitlu.CurrencyId,
                                                                                CurrencyName = f.Currency.CurrencyName,
                                                                                IdPlasament = f.BVC_VenitTitluCF.BVC_VenitTitlu.IdPlasament,
                                                                                NrZilePlasament = (decimal)(LazyMethods.MinDate(f.BVC_VenitTitluCF.DataReinvestire, LazyMethods.LastDayOfMonth(endDate)) - LazyMethods.MaxDate(f.BVC_VenitTitluCF.DataReinvestire, startDate)).TotalDays,
                                                                                ProcentDobanda = f.ProcDobanda,
                                                                                SumaInvestita = f.SumaReinvestita,
                                                                                ActivityTypeId = f.BVC_VenitTitluCF.BVC_VenitTitlu.ActivityTypeId,
                                                                                DataStart = f.BVC_VenitTitluCF.BVC_VenitTitlu.StartDate,
                                                                                DataEnd = f.BVC_VenitTitluCF.BVC_VenitTitlu.MaturityDate,
                                                                                ActivityTypeName = f.BVC_VenitTitluCF.BVC_VenitTitlu.ActivityType.ActivityName,
                                                                                DobandaCuvenita = 0,
                                                                                Incasari = 0,
                                                                                TenantId = f.BVC_VenitTitluCF.BVC_VenitTitlu.TenantId,
                                                                                TipResursa = "R"
                                                                            })
                                                                            .ToList();

                foreach (var item in titluriReinvestite)
                {
                    decimal nrZileTotalTitlu = (decimal)(item.DataEnd - item.DataStart).TotalDays;
                    decimal nrZilePlasAn = (decimal)(endDate - item.DataStart).TotalDays;

                    var dobandaTotala = item.SumaInvestita * item.ProcentDobanda / 100;
                    item.DobandaCuvenita = dobandaTotala * nrZilePlasAn / nrZileTotalTitlu;
                }

                var plasamente = new List<BVC_PrevResurseDetaliiDto>();
                plasamente.AddRange(titluriTrezorerie);
                plasamente.AddRange(titluriReinvPreliminat);
                plasamente.AddRange(titluriReinvestite);

                var bugetPrevResurseDetList = new List<BVC_PrevResurseDetalii>();

                var activityTypeList = plasamente.Select(f => f.ActivityTypeId).Distinct().ToList();

                foreach (var activityTypeId in activityTypeList)
                {
                    RandResurse(bugetPrevId, activityTypeId, endDate, plasamente, appClientId, out bugetPrevResurseDetList);

                    RandIncasari(bugetPrevId, activityTypeId, plasamente, appClientId, out bugetPrevResurseDetList);

                    RandDiferentaCursValutar(bugetPrevId, activityTypeId, appClientId);

                    RandContributii(bugetPrevId, activityTypeId, startDate, endDate, appClientId, anBVC);

                    RandCreante(bugetPrevId, activityTypeId, startDate, endDate, appClientId, anBVC);

                    RandCheltuieli(bugetPrevId, activityTypeId, anBVC, appClientId);

                    RandAnUrmator(bugetPrevId, activityTypeId, anBVC, endDate, appClientId);

                    RandVenituri(bugetPrevId, activityTypeId, anBVC, endDate, appClientId);

                    RandRataMedieDobanda(bugetPrevId, activityTypeId, appClientId);

                    RandCapitalInvestit(bugetPrevId, activityTypeId, appClientId);
                }

                foreach (var item in bugetPrevResurseDetList)
                {
                    _bugetPrevResurseDetaliiRepository.Insert(item);
                }
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        private void RandResurse(int bugetPrevId, int activityTypeId, DateTime endDate, List<BVC_PrevResurseDetaliiDto> plasamente, int appClientId, out List<BVC_PrevResurseDetalii> bugetPrevResurseDetList)
        {
            bugetPrevResurseDetList = new List<BVC_PrevResurseDetalii>();
            var resurseRand = new BVC_PrevResurse
            {
                BugetPrevId = bugetPrevId,
                ActivityTypeId = activityTypeId,
                Descriere = "Resurse la " + endDate,
                OrderView = 1,
                Suma = plasamente.Where(f => f.ActivityTypeId == activityTypeId && "TP".Contains(f.TipResursa)).Sum(f => f.SumaInvestita),
                TenantId = appClientId
            };

            _bugetPrevResurseRepository.Insert(resurseRand);
            CurrentUnitOfWork.SaveChanges();

            bugetPrevResurseDetList.AddRange(plasamente.Where(f => f.ActivityTypeId == activityTypeId && "TP".Contains(f.TipResursa)).Select(f => new BVC_PrevResurseDetalii
            {
                BVC_PrevResurseId = resurseRand.Id,
                CurrencyId = f.CurrencyId,
                DataEnd = f.DataEnd,
                DataStart = f.DataStart,
                IdPlasament = f.IdPlasament,
                NrZilePlasament = f.NrZilePlasament,
                ProcentDobanda = f.ProcentDobanda,
                SumaInvestita = f.SumaInvestita,
                TenantId = f.TenantId
            }).ToList());
        }

        private void RandIncasari(int bugetPrevId, int activityTypeId, List<BVC_PrevResurseDetaliiDto> plasamente, int appClientId, out List<BVC_PrevResurseDetalii> bugetPrevResurseDetList)
        {
            bugetPrevResurseDetList = new List<BVC_PrevResurseDetalii>();
            var incasariRand = new BVC_PrevResurse
            {
                BugetPrevId = bugetPrevId,
                ActivityTypeId = activityTypeId,
                Descriere = "cupoane+dobanzi",
                OrderView = 2,
                Suma = plasamente.Where(f => f.ActivityTypeId == activityTypeId && "TP".Contains(f.TipResursa)).Sum(f => f.Incasari),
                TenantId = appClientId
            };
            _bugetPrevResurseRepository.Insert(incasariRand);
            CurrentUnitOfWork.SaveChanges();

            bugetPrevResurseDetList.AddRange(plasamente.Where(f => f.ActivityTypeId == activityTypeId && "TP".Contains(f.TipResursa)).Select(f => new BVC_PrevResurseDetalii
            {
                BVC_PrevResurseId = incasariRand.Id,
                CurrencyId = f.CurrencyId,
                DataEnd = f.DataEnd,
                DataStart = f.DataStart,
                IdPlasament = f.IdPlasament,
                NrZilePlasament = f.NrZilePlasament,
                ProcentDobanda = f.ProcentDobanda,
                SumaInvestita = f.SumaInvestita,
                TenantId = f.TenantId
            }).ToList());
        }

        private void RandDiferentaCursValutar(int bugetPrevId, int activityTypeId, int appClientId)
        {
            var diferentaCursValutarRand = new BVC_PrevResurse
            {
                BugetPrevId = bugetPrevId,
                ActivityTypeId = activityTypeId,
                Descriere = "diferenta curs valutar",
                OrderView = 3,
                Suma = 0,
                TenantId = appClientId
            };
            _bugetPrevResurseRepository.Insert(diferentaCursValutarRand);
            CurrentUnitOfWork.SaveChanges();
        }

        private void RandContributii(int bugetPrevId, int activityTypeId, DateTime startDate, DateTime endDate, int appClientId, int anBVC)
        {
            var localCurrencyId = GetCurrentTenant().LocalCurrencyId.Value;
            var bugetPrevContribList = _bugetPrevContribRepository.GetAllIncluding(f => f.ActivityType, f => f.Currency)
                                                       .Where(f => f.ActivityTypeId == activityTypeId && f.TipIncasare == BVC_BugetPrevContributieTipIncasare.Contributii &&
                                                                   f.DataIncasare >= startDate && f.DataIncasare <= endDate)
                                                       .ToList();
            decimal sum = 0;
            var monedeList = bugetPrevContribList.Select(f => f.CurrencyId).Distinct().ToList();

            foreach (var item in monedeList)
            {
                if (item == localCurrencyId)
                {
                    sum += bugetPrevContribList.Where(f => f.CurrencyId == item).Sum(f => f.Value);
                }
                else
                {
                    var exchangeRate = _exchangeRateForecastRepository.GetAll().FirstOrDefault(f => f.Year == anBVC && f.CurrencyId == item && f.State == State.Active);
                    if (exchangeRate == null)
                    {
                        var currencyName = _currencyRepository.GetAll().FirstOrDefault(f => f.Id == item).CurrencyName;
                        throw new Exception("Nu am identificat cursul valutar estimat pentru " + currencyName + ", anul " + anBVC.ToString());
                    }

                    var value = bugetPrevContribList.Where(f => f.CurrencyId == item).Sum(f => f.Value);
                    sum += Math.Round(value * exchangeRate.ValoareEstimata, 2);
                }
            }

            var contributiiRand = new BVC_PrevResurse
            {
                BugetPrevId = bugetPrevId,
                ActivityTypeId = activityTypeId,
                Descriere = "Contributii",
                OrderView = 4,
                Suma = sum,
                TenantId = appClientId
            };
            _bugetPrevResurseRepository.Insert(contributiiRand);
            CurrentUnitOfWork.SaveChanges();
        }

        private void RandCreante(int bugetPrevId, int activityTypeId, DateTime startDate, DateTime endDate, int appClientId, int anBVC)
        {
            var localCurrencyId = GetCurrentTenant().LocalCurrencyId.Value;

            var bugetPrevCreanteList = _bugetPrevContribRepository.GetAllIncluding(f => f.ActivityType, f => f.Currency)
                                       .Where(f => f.ActivityTypeId == activityTypeId && (f.TipIncasare == BVC_BugetPrevContributieTipIncasare.Creante || f.TipIncasare == BVC_BugetPrevContributieTipIncasare.ComisionLichidator) &&
                                                   f.DataIncasare >= startDate && f.DataIncasare <= endDate)
                                       .ToList();
            decimal sum = 0;
            var monedeList = bugetPrevCreanteList.Select(f => f.CurrencyId).Distinct().ToList();
            foreach (var item in monedeList)
            {
                if (item == localCurrencyId)
                {
                    sum += bugetPrevCreanteList.Where(f => f.CurrencyId == item).Sum(f => f.Value);
                }
                else
                {
                    var exchangeRate = _exchangeRateForecastRepository.GetAll().FirstOrDefault(f => f.Year == anBVC && f.CurrencyId == item && f.State == State.Active);
                    if (exchangeRate == null)
                    {
                        var currencyName = _currencyRepository.GetAll().FirstOrDefault(f => f.Id == item).CurrencyName;
                        throw new Exception("Nu am identificat cursul valutar estimat pentru " + currencyName + ", anul " + anBVC.ToString());
                    }

                    var value = bugetPrevCreanteList.Where(f => f.CurrencyId == item).Sum(f => f.Value);
                    sum += Math.Round(value * exchangeRate.ValoareEstimata, 2);
                }
            }

            var creanteRand = new BVC_PrevResurse
            {
                BugetPrevId = bugetPrevId,
                ActivityTypeId = activityTypeId,
                Descriere = "creante si comision lichidator",
                OrderView = 5,
                Suma = sum,
                TenantId = appClientId
            };
            _bugetPrevResurseRepository.Insert(creanteRand);
            CurrentUnitOfWork.SaveChanges();
        }

        private void RandCheltuieli(int bugetPrevId, int activityTypeId, int anBVC, int appClientId)
        {
            var cheltList = _bugetPrevRandValueRepository.GetAllIncluding(f => f.BugetPrevRand, f => f.BugetPrevRand.BugetPrev, f => f.BugetPrevRand.FormRand, f => f.BugetPrevRand.FormRand.Formular)
                                             .Where(f => f.BugetPrevRand.FormRand.Formular.AnBVC == anBVC && f.BugetPrevRand.FormRand.NivelRand == 1 && f.BugetPrevRand.FormRand.IsTotal == true &&
                                                         f.BugetPrevRand.FormRand.TipRand == BVC_RowType.Cheltuieli && f.BugetPrevRand.BugetPrevId == bugetPrevId)
                                             .OrderBy(f => f.BugetPrevRand.FormRand.OrderView)
                                             .ToList();
            var cheltuieliRand = new BVC_PrevResurse
            {
                BugetPrevId = bugetPrevId,
                ActivityTypeId = activityTypeId,
                Descriere = "cheltuieli",
                OrderView = 6,
                Suma = cheltList.Where(f => f.ActivityTypeId == activityTypeId).Sum(f => f.Value),
                TenantId = appClientId
            };
            _bugetPrevResurseRepository.Insert(cheltuieliRand);
            CurrentUnitOfWork.SaveChanges();
        }

        private void RandAnUrmator(int bugetPrevId, int activityTypeId, int anBVC, DateTime endDate, int appClientId)
        {
            var resurseAnUrmatorRand = new BVC_PrevResurse
            {
                BugetPrevId = bugetPrevId,
                ActivityTypeId = activityTypeId,
                Descriere = "RESURSE LA " + new DateTime(anBVC + 1, endDate.Month, endDate.Day),
                OrderView = 7,
                Suma = _bugetPrevResurseRepository.GetAllIncluding(f => f.ActivityType).Where(f => f.ActivityTypeId == activityTypeId).Sum(f => f.Suma),
                TenantId = appClientId
            };
            _bugetPrevResurseRepository.Insert(resurseAnUrmatorRand);
            CurrentUnitOfWork.SaveChanges();
        }

        private void RandVenituri(int bugetPrevId, int activityTypeId, int anBVC, DateTime endDate, int appClientId)
        {
            var venituriRand = new BVC_PrevResurse
            {
                BugetPrevId = bugetPrevId,
                ActivityTypeId = activityTypeId,
                Descriere = "Venituri " + anBVC + 1,
                OrderView = 8,
                Suma = 0,
                TenantId = appClientId
            };
            _bugetPrevResurseRepository.Insert(venituriRand);
            CurrentUnitOfWork.SaveChanges();
        }

        private void RandRataMedieDobanda(int bugetPrevId, int activityTypeId, int appClientId)
        {
            var rataMedieRand = new BVC_PrevResurse
            {
                BugetPrevId = bugetPrevId,
                ActivityTypeId = activityTypeId,
                Descriere = "Rata medie a dobanzii",
                OrderView = 9,
                Suma = 0,
                TenantId = appClientId
            };
            _bugetPrevResurseRepository.Insert(rataMedieRand);
            CurrentUnitOfWork.SaveChanges();
        }

        private void RandCapitalInvestit(int bugetPrevId, int activityTypeId, int appClientId)
        {
            var capitalMediuRand = new BVC_PrevResurse
            {
                BugetPrevId = bugetPrevId,
                ActivityTypeId = activityTypeId,
                Descriere = "Capital mediu investit",
                OrderView = 10,
                Suma = 0,
                TenantId = appClientId
            };
            _bugetPrevResurseRepository.Insert(capitalMediuRand);
            CurrentUnitOfWork.SaveChanges();
        }
    }
}