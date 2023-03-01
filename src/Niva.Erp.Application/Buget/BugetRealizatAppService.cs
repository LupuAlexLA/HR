using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.ExternalApi;
using Niva.Erp.Models.Buget;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Contracts;
using Niva.Erp.Models.Economic;
using Niva.Erp.Models.Economic.Casierii;
using Niva.Erp.Models.ImoAsset;
using Niva.Erp.Models.PrePayments;
using Niva.Erp.Repositories.Buget;
using Niva.Erp.Repositories.Conta;
using Niva.Erp.Repositories.Conta.Nomenclatures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Buget
{
    public interface IBugetRealizatAppService : IApplicationService
    {
        List<BugetRealizatDto> BugetRealizatList(int formularId, int? bvcTip);
        List<BugetRealizatSavedBalanceDateDto> BugetRealizatSavedBalanceDateList(int formularId, int bvcTip);
        List<RealizatAddDispoDto> RealizatAddDisponibil();
        List<BugetRealizatRowDto> RealizatRows(int realizatId);
        List<BugetRealizatRowDetailDto> RealizatRowDetails(int rowId);
        void BugetRealizatCalcul(int savedBalanceId);
        void BugetRealizatCalculTip(int savedBalanceId, int bvcTip);
        void BugetRealizatDelete(int realizatId, int? bugetBalRealizatId);
        List<BugetBalRealizatRowDetailDto> BalRealizatRowDetails(int rowId);
        List<BugetBalRealizatRowDto> BalRealizatRows(int balRealizatId);
        List<BugetBalRealizatSavedBalanceDateDto> BugetBalRealizatSavedBalanceDateList(int formularId, int bvcTip);
    }

    public class BugetRealizatAppService : ErpAppServiceBase, IBugetRealizatAppService
    {
        IBVC_BugetRealizatRepository _bugetRealizatRepository;
        ISavedBalanceRepository _savedBalanceRepository;
        IRepository<BVC_Formular> _bugetConfigRepository;
        IRepository<BVC_FormRand> _bugetFormRandRepository;
        IRepository<BVC_RealizatRand> _bugetRealizatRandRepository;
        IRepository<BVC_RealizatRandDetails> _bugetRealizatRandDetRepository;
        IRepository<InvoiceDetails> _invoiceDetailsRepository;
        IRepository<BVC_FormRandDetails> _bugetFormRandDetailsRepository;
        IExchangeRatesRepository _exchangeRatesRepository;
        IRepository<BVC_RealizatExceptii> _bugetRealizatExceptii;
        IRepository<PaymentOrderInvoice> _paymentOrderInvoiceRepository;
        IRepository<ImoAssetStock> _imoGestRepository;
        IRepository<PrepaymentBalance> _prepaymentBalanceRepository;
        IRepository<DispositionInvoice> _dispositionInvoiceRepository;
        IActiveBugetBVCManager _activeBugetBVCManager;
        IRepository<Currency> _currencyRepository;
        IBVC_BugetPrevRepository _bugetPrevRepository;
        IRepository<BVC_PAAP_Referat> _bvcPaapReferatRepository;
        IRepository<BVC_PAAP_InvoiceDetails> _bvcPAAPInvocieDetRepository;
        IBVC_BugetBalRealizatRepository _bugetBalRealizatRepository;
        IRepository<ActivityType> _activityTypeRepository;
        IRepository<BVC_BalRealizatRand> _bugetBalRealizatRandRepository;
        IRepository<BVC_BalRealizatRandDetails> _bugetBalRealizatRandDetailsRepository;
        IRepository<SavedBalanceDetails> _savedBalanceDetailsRepository;


        public BugetRealizatAppService(IBVC_BugetRealizatRepository bugetRealizatRepository, ISavedBalanceRepository savedBalanceRepository, IRepository<BVC_Formular> bugetConfigRepository,
                                       IRepository<BVC_FormRand> bugetFormRandRepository, IRepository<BVC_RealizatRand> bugetRealizatRandRepository, IRepository<BVC_RealizatRandDetails> bugetRealizatRandDetRepository,
                                       IRepository<InvoiceDetails> invoiceDetailsRepository, IRepository<BVC_FormRandDetails> bugetFormRandDetailsRepository,
                                       IExchangeRatesRepository exchangeRatesRepository, IRepository<BVC_RealizatExceptii> bugetRealizatExceptii, IRepository<PaymentOrderInvoice> paymentOrderInvoiceRepository,
                                       IRepository<ImoAssetStock> imoGestRepository, IRepository<PrepaymentBalance> prepaymentBalanceRepository,
                                       IRepository<DispositionInvoice> dispositionInvoiceRepository, IActiveBugetBVCManager activeBugetBVCManager, IRepository<Currency> currencyRepository,
                                       IBVC_BugetPrevRepository bugetPrevRepository, IRepository<BVC_PAAP_Referat> bvcPaapReferatRepository,
                                       IRepository<BVC_PAAP_InvoiceDetails> bvcPAAPInvocieDetRepository, IBVC_BugetBalRealizatRepository bugetBalRealizatRepository,
                                       IRepository<ActivityType> activityTypeRepository, IRepository<BVC_BalRealizatRand> bugetBalRealizatRandRepository,
                                       IRepository<BVC_BalRealizatRandDetails> bugetBalRealizatRandDetailsRepository, IRepository<SavedBalanceDetails> savedBalanceDetailsRepository)
        {
            _bugetRealizatRepository = bugetRealizatRepository;
            _savedBalanceRepository = savedBalanceRepository;
            _bugetConfigRepository = bugetConfigRepository;
            _bugetFormRandRepository = bugetFormRandRepository;
            _bugetRealizatRandRepository = bugetRealizatRandRepository;
            _bugetRealizatRandDetRepository = bugetRealizatRandDetRepository;
            _invoiceDetailsRepository = invoiceDetailsRepository;
            _bugetFormRandDetailsRepository = bugetFormRandDetailsRepository;
            _exchangeRatesRepository = exchangeRatesRepository;
            _bugetRealizatExceptii = bugetRealizatExceptii;
            _paymentOrderInvoiceRepository = paymentOrderInvoiceRepository;
            _imoGestRepository = imoGestRepository;
            _prepaymentBalanceRepository = prepaymentBalanceRepository;
            _dispositionInvoiceRepository = dispositionInvoiceRepository;
            _activeBugetBVCManager = activeBugetBVCManager;
            _currencyRepository = currencyRepository;
            _bugetPrevRepository = bugetPrevRepository;
            _bvcPaapReferatRepository = bvcPaapReferatRepository;
            _bvcPAAPInvocieDetRepository = bvcPAAPInvocieDetRepository;
            _bugetBalRealizatRepository = bugetBalRealizatRepository;
            _activityTypeRepository = activityTypeRepository;
            _bugetBalRealizatRandRepository = bugetBalRealizatRandRepository;
            _bugetBalRealizatRandDetailsRepository = bugetBalRealizatRandDetailsRepository;
            _savedBalanceDetailsRepository = savedBalanceDetailsRepository;
        }

        //[AbpAuthorize("Buget.BVC.Realizat.Acces")]
        public List<BugetRealizatDto> BugetRealizatList(int formularId, int? bvcTip)
        {
            try
            {
                var appClient = GetCurrentTenant();
                var ret = new List<BugetRealizatDto>();

                var list = _bugetRealizatRepository.GetAllIncluding(f => f.SavedBalance, f => f.BVC_Formular).Where(f => f.BVC_FormularId == formularId && f.TenantId == appClient.Id && f.BVC_Formular.State == State.Active);

                if (bvcTip != null)
                {
                    var _bvcTip = (BVC_Tip)bvcTip;
                    list = list.Where(f => f.BVC_Tip == _bvcTip);
                }

                foreach (var bugetRealizat in list.ToList())
                {
                    var bvc_BalRealizatId = _bugetBalRealizatRepository.GetAllIncluding(f => f.SavedBalance, f => f.BVC_Formular)
                                                                       .FirstOrDefault(f => f.BVC_FormularId == bugetRealizat.BVC_FormularId && f.TenantId == appClient.Id && f.BVC_Formular.State == State.Active && 
                                                                                       f.BVC_Tip == bugetRealizat.BVC_Tip && f.SavedBalanceId == bugetRealizat.SavedBalanceId)?.Id;
                    ret.Add(new BugetRealizatDto
                    {
                        Id = bugetRealizat.Id,
                        BVC_BalRealizatId = bvc_BalRealizatId ?? null,
                        SavedBalanceDate = bugetRealizat.SavedBalance.SaveDate,
                        SavedBalanceDescription = bugetRealizat.SavedBalance.BalanceName,
                        BVC_Tip = (int)bugetRealizat.BVC_Tip,
                        BVC_TipStr = bugetRealizat.BVC_Tip.ToString()
                    });
                }

                ret = ret.OrderByDescending(f => f.SavedBalanceDate).ThenBy(f => f.BVC_Tip).ToList();

                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        [AbpAuthorize("Buget.BVC.Realizat.Modificare")]
        public List<RealizatAddDispoDto> RealizatAddDisponibil()
        {
            try
            {
                var appClient = GetCurrentTenant();

                var calcRealizatIds = _bugetRealizatRepository.GetAll().Select(f => f.SavedBalanceId).ToList();
                var savedBalanceList = _savedBalanceRepository.GetAll().Where(f => f.TenantId == appClient.Id && !f.IsDaily && !calcRealizatIds.Contains(f.Id)).OrderBy(f => f.SaveDate)
                                                              .ToList()
                                                              .Select(f => new RealizatAddDispoDto
                                                              {
                                                                  Id = f.Id,
                                                                  Descriere = LazyMethods.DateToString(f.SaveDate) + " - " + f.BalanceName
                                                              })
                                                              .ToList();
                return savedBalanceList;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        //[AbpAuthorize("Buget.BVC.Realizat.Acces")]
        public List<BugetRealizatRowDto> RealizatRows(int realizatId)
        {
            try
            {
                var realizat = _bugetRealizatRepository.GetAll().FirstOrDefault(f => f.Id == realizatId);

                var list = _bugetRealizatRandRepository.GetAllIncluding(f => f.BVC_FormRand)
                                                      .Where(f => f.BVC_RealizatId == realizatId)
                                                      .ToList();
                if (realizat.BVC_Tip == BVC_Tip.BVC)
                {
                    list = list.Where(f => f.BVC_FormRand.AvailableBVC).ToList();
                }
                else
                {
                    list = list.Where(f => f.BVC_FormRand.AvailableCashFlow).ToList();
                }

                var ret = list.OrderBy(f => f.BVC_FormRand.OrderView)
                              .Select(f => new BugetRealizatRowDto
                              {
                                  Id = f.Id,
                                  DenumireRand = f.BVC_FormRand.Descriere,
                                  ValoareCuReferat = f.ValoareCuReferat,
                                  ValoareFaraReferat = f.ValoareFaraReferat,
                                  Bold = f.BVC_FormRand.Bold
                              })
                              .ToList();
                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        //[AbpAuthorize("Buget.BVC.Realizat.Acces")]
        public List<BugetRealizatRowDetailDto> RealizatRowDetails(int rowId)
        {
            try
            {
                var ret = _bugetRealizatRandDetRepository.GetAllIncluding(f => f.Currency)
                                                         .Where(f => f.BVC_RealizatRandId == rowId)
                                                         .Select(f => new BugetRealizatRowDetailDto
                                                         {
                                                             Descriere = f.Descriere,
                                                             Valoare = f.Valoare,
                                                             CurrencyName = f.Currency.CurrencyName
                                                         })
                                                         .ToList();
                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        // Detalii randuri Buget Realziat cu formule din Balanta
        public List<BugetBalRealizatRowDto> BalRealizatRows(int balRealizatId)
        {
            try
            {
                var appClient = GetCurrentTenant();
                var balRealizat = _bugetBalRealizatRepository.GetAll().FirstOrDefault(f => f.Id == balRealizatId);

                var list = _bugetBalRealizatRandRepository.GetAllIncluding(f => f.BVC_FormRand, f => f.ActivityType)
                                                      .Where(f => f.BVC_BalRealizatId == balRealizatId)
                                                      .ToList();

                if (balRealizat.BVC_Tip == BVC_Tip.BVC)
                {
                    list = list.Where(f => f.BVC_FormRand.AvailableBVC).ToList();
                }

                var ret = list.OrderBy(f => f.BVC_FormRand.OrderView)
                              .Select(g => new BugetBalRealizatRowDto
                              {
                                  Id = g.Id,
                                  DenumireRand = g.BVC_FormRand.Descriere,
                                  Valoare = g.Valoare,
                                  Bold = g.BVC_FormRand.Bold,
                                  ActivityTypeName = g.ActivityType.ActivityName,
                                  ActivityTypeId = g.ActivityTypeId
                              })
                              .ToList();
                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public List<BugetBalRealizatRowDetailDto> BalRealizatRowDetails(int rowId)
        {
            var localCurrencyId = GetCurrentTenant().LocalCurrencyId.Value;
            var currency = _currencyRepository.FirstOrDefault(f => f.Id == localCurrencyId);
            var ret = _bugetBalRealizatRandDetailsRepository.GetAll()
                                         .Where(f => f.BVC_BalRealizatRandId == rowId)
                                         .Select(f => new BugetBalRealizatRowDetailDto
                                         {
                                             Descriere = f.Descriere,
                                             Valoare = f.Valoare,
                                             CurrencyName = currency.CurrencyName
                                         })
                                         .ToList();
            return ret;
        }

        [AbpAuthorize("Buget.BVC.Realizat.Modificare")]
        public void BugetRealizatCalcul(int savedBalanceId)
        {
            try
            {
                var bvcTipList = Enum.GetValues(typeof(BVC_Tip)).Cast<BVC_Tip>().ToList();

                foreach (var item in bvcTipList)
                {
                    BugetRealizatCalculTip(savedBalanceId, (int)item);

                    if (item == BVC_Tip.BVC)
                    {
                        BugetBalRealizatCalculTip(savedBalanceId, (int)item);
                    }
                }

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        [AbpAuthorize("Buget.BVC.Realizat.Modificare")]
        public void BugetRealizatDelete(int realizatId, int? bugetBalRealizatId)
        {
            try
            {
                _bugetRealizatRepository.Delete(realizatId);

                if (bugetBalRealizatId != null)
                {
                    _bugetBalRealizatRepository.Delete(bugetBalRealizatId.Value);
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public void BugetRealizatCalculTip(int savedBalanceId, int bvcTip)
        {
            try
            {
                var _bvcTip = (BVC_Tip)bvcTip;
                var appClient = GetCurrentTenant();
                var localCurrencyId = appClient.LocalCurrencyId.Value;
                var savedBalance = _savedBalanceRepository.GetAll().FirstOrDefault(f => f.Id == savedBalanceId);
                var anBVC = savedBalance.SaveDate.Year;
                var formular = _bugetConfigRepository.GetAll().FirstOrDefault(f => f.AnBVC == anBVC && f.State == State.Active);
                var formularRanduri = _bugetFormRandRepository.GetAllIncluding(f => f.DetaliiRand, f => f.Formular).Where(f => f.FormularId == formular.Id && f.Formular.State == State.Active).OrderBy(f => f.OrderView).ToList();
                if (_bvcTip == BVC_Tip.BVC)
                {
                    formularRanduri = formularRanduri.Where(f => f.AvailableBVC).ToList();
                }
                else
                {
                    formularRanduri = formularRanduri.Where(f => f.AvailableCashFlow).ToList();
                }

                var realizat = new BVC_Realizat
                {
                    SavedBalanceId = savedBalanceId,
                    BVC_FormularId = formular.Id,
                    BVC_Tip = _bvcTip,
                    TenantId = appClient.Id
                };

                _bugetRealizatRepository.Insert(realizat);
                CurrentUnitOfWork.SaveChanges();

                var randuriCalc = new List<BVC_RealizatRandDto>();
                foreach (var rand in formularRanduri)
                {
                    var realizatRand = new BVC_RealizatRandDto
                    {
                        BVC_FormRandId = rand.Id,
                        BVC_RealizatId = realizat.Id,
                        ValoareCuReferat = 0,
                        ValoareFaraReferat = 0,
                        BVC_RealizatRandDetails = new List<BVC_RealizatRandDetailsDto>()
                    };
                    randuriCalc.Add(realizatRand);
                }

                var calcul = new BVC_RealizatCalcDto
                {
                    RanduriCalc = randuriCalc,
                    Exceptii = new List<BVC_RealizatExceptiiDto>()
                };


                if (_bvcTip == BVC_Tip.BVC) // -- scos pentru ca nu mai calculam buget realizat din operational
                {
                    //// facturi fara mijloace fixe si cheltuieli in avans
                    //calcul = BVC_InvoiceCalc(calcul, savedBalance.SaveDate, appClient.Id, localCurrencyId);

                    ////facturi cu mijloace fixe
                    //calcul = BVC_InvoiceCalcMF(calcul, savedBalance.SaveDate, appClient.Id);

                    ////facturi cu cheltuieli in avans
                    //calcul = BVC_InvoiceCalcCA(calcul, savedBalance.SaveDate, appClient.Id);

                    //// dobanzi din plasamente
                    //calcul = BVC_PlasamenteCalc(calcul, savedBalance.SaveDate, appClient.Id, localCurrencyId);

                    //// calcul formule din Conta
                    //calcul = FormulaConta(calcul, savedBalance.SaveDate, appClient.Id, localCurrencyId, _bvcTip);

                    //// calcul totaluri
                    //calcul = BVC_RealizatTotaluri(calcul, savedBalance.SaveDate, appClient.Id, localCurrencyId, _bvcTip);

                    ////calcul din PAAP Referate
                    //calcul = BVC_PAAP_Referate(calcul, savedBalance.SaveDate, appClient.Id, localCurrencyId);
                }

                if (_bvcTip == BVC_Tip.CashFlow)
                {
                    //plati din OP, Casierii, Decont
                    calcul = BVC_CashFlowInvoiceCalc(calcul, savedBalance.SaveDate, appClient.Id, localCurrencyId);

                    // dobanzi din plasamente
                    calcul = BVC_CashFlowPlasamenteCalc(calcul, savedBalance.SaveDate, appClient.Id, localCurrencyId);

                    // calcul formule din Conta
                    //calcul = FormulaConta(calcul, savedBalance.SaveDate, appClient.Id, localCurrencyId, _bvcTip); -- scos pentru ca nu mai calculam venituri pentru cash flow

                    // calcul totaluri
                    calcul = BVC_RealizatTotaluri(calcul, savedBalance.SaveDate, appClient.Id, localCurrencyId, _bvcTip);

                    //calcul din PAAP Referate
                    calcul = BVC_CashFlowPAAP_Referate(calcul, savedBalance.SaveDate, appClient.Id, localCurrencyId);

                    // calcul din Salarizare
                    calcul = BVC_SalarizareCashFlowCalc(calcul, savedBalance.SaveDate, appClient.Id, localCurrencyId, realizat.Id);
                }

                // aici salvez in baza de date
                foreach (var randCalc in calcul.RanduriCalc)
                {

                    var rand = new BVC_RealizatRand
                    {
                        BVC_FormRandId = randCalc.BVC_FormRandId,
                        BVC_RealizatId = randCalc.BVC_RealizatId,
                        ValoareCuReferat = randCalc.ValoareCuReferat + randCalc.ValoareFaraReferat,
                        ValoareFaraReferat = randCalc.ValoareFaraReferat
                    };
                    _bugetRealizatRandRepository.Insert(rand);
                    CurrentUnitOfWork.SaveChanges();

                    if (randCalc.BVC_RealizatRandDetails.Count != 0)
                    {
                        foreach (var detail in randCalc.BVC_RealizatRandDetails)
                        {
                            var randDetail = new BVC_RealizatRandDetails
                            {
                                BVC_RealizatRandId = rand.Id,
                                Descriere = detail.Descriere,
                                Valoare = detail.Valoare,
                                CurrencyId = detail.CurrencyId
                            };

                            _bugetRealizatRandDetRepository.Insert(randDetail);
                        }
                    }
                }

                foreach (var exceptie in calcul.Exceptii)
                {
                    var except = new BVC_RealizatExceptii
                    {
                        Valoare = exceptie.Valoare,
                        Descriere = exceptie.Descriere
                    };
                    _bugetRealizatExceptii.Insert(except);
                }
                CurrentUnitOfWork.SaveChanges();

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        #region BVC_BalRealizat
        private void BugetBalRealizatCalculTip(int savedBalanceId, int bvcTip)
        {
            try
            {
                var _bvcTip = (BVC_Tip)bvcTip;
                var appClient = GetCurrentTenant();
                var localCurrencyId = appClient.LocalCurrencyId.Value;
                var savedBalance = _savedBalanceRepository.GetAll().FirstOrDefault(f => f.Id == savedBalanceId);
                var anBVC = savedBalance.SaveDate.Year;
                var formular = _bugetConfigRepository.GetAll().FirstOrDefault(f => f.AnBVC == anBVC && f.State == State.Active);
                var formularRanduri = _bugetFormRandRepository.GetAllIncluding(f => f.DetaliiRand, f => f.Formular).Where(f => f.FormularId == formular.Id && f.Formular.State == State.Active).OrderBy(f => f.OrderView).ToList();
                var activityTypeList = _activityTypeRepository.GetAll().Where(f => f.Status == State.Active && f.TenantId == appClient.Id).ToList();

                var realizat = new BVC_BalRealizat
                {
                    SavedBalanceId = savedBalanceId,
                    BVC_FormularId = formular.Id,
                    BVC_Tip = _bvcTip,
                    TenantId = appClient.Id
                };

                _bugetBalRealizatRepository.Insert(realizat);
                CurrentUnitOfWork.SaveChanges();

                var randuriCalc = new List<BVC_BalRealizatRandDto>();
                foreach (var activityType in activityTypeList)
                {
                    foreach (var rand in formularRanduri)
                    {
                        var realizatRand = new BVC_BalRealizatRandDto
                        {
                            BVC_FormRandId = rand.Id,
                            BVC_BalRealizatId = realizat.Id,
                            ActivityTypeId = activityType.Id,
                            BVC_BalRealizatRandDetails = new List<BVC_BalRealizatRandDetailsDto>()
                        };
                        randuriCalc.Add(realizatRand);
                    }
                }


                var calcul = new BVC_BalRealizatCalcDto
                {
                    RanduriCalc = randuriCalc,
                    Exceptii = new List<BVC_RealizatExceptiiDto>()
                };


                if (_bvcTip == BVC_Tip.BVC)
                {
                    // calcul formule din Balante
                    calcul = FormulaBalanta(calcul, savedBalance.SaveDate, appClient.Id, localCurrencyId, _bvcTip, activityTypeList, savedBalance);
                    // calcul totaluri
                    calcul = BVC_BalRealizatTotaluri(calcul, savedBalance.SaveDate, appClient.Id, localCurrencyId, _bvcTip, activityTypeList);

                }

                // aici salvez in baza de date
                foreach (var randCalc in calcul.RanduriCalc)
                {

                    var rand = new BVC_BalRealizatRand
                    {
                        BVC_FormRandId = randCalc.BVC_FormRandId,
                        BVC_BalRealizatId = randCalc.BVC_BalRealizatId,
                        ActivityTypeId = randCalc.ActivityTypeId,
                        Valoare = randCalc.Valoare
                    };
                    _bugetBalRealizatRandRepository.Insert(rand);
                    CurrentUnitOfWork.SaveChanges();

                    if (randCalc.BVC_BalRealizatRandDetails.Count != 0)
                    {
                        foreach (var detail in randCalc.BVC_BalRealizatRandDetails)
                        {
                            var randDetail = new BVC_BalRealizatRandDetails
                            {
                                BVC_BalRealizatRandId = rand.Id,
                                Descriere = detail.Descriere,
                                Valoare = detail.Valoare,

                            };

                            _bugetBalRealizatRandDetailsRepository.Insert(randDetail);
                        }
                    }
                }

                foreach (var exceptie in calcul.Exceptii)
                {
                    var except = new BVC_RealizatExceptii
                    {
                        Valoare = exceptie.Valoare,
                        Descriere = exceptie.Descriere
                    };
                    _bugetRealizatExceptii.Insert(except);
                }
                CurrentUnitOfWork.SaveChanges();

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        private BVC_BalRealizatCalcDto BVC_BalRealizatTotaluri(BVC_BalRealizatCalcDto calcul, DateTime calcDate, int appClientId, int localCurrencyId, BVC_Tip bvc_Tip, List<ActivityType> activityTypes)
        {

            try
            {
                var formRandList = _bugetFormRandRepository.GetAllIncluding(f => f.Formular)
                                                                     .Where(f => f.Formular.AnBVC == calcDate.Year && f.Formular.State == State.Active
                                                                            && f.TenantId == appClientId)
                                                                     .ToList();


                if (bvc_Tip == BVC_Tip.BVC)
                {
                    formRandList = formRandList.Where(f => f.AvailableBVC).OrderBy(f => f.OrderView).ToList();
                }

                var randValues = calcul.RanduriCalc.ToList();
                var randTotalIdsList = formRandList.Where(f => f.BalIsTotal).Select(f => f.Id).ToList();


                var valuesList = new List<BugetBalRealizatCalcTotalDto>();

                // initializez totalurile
                foreach (var activityType in activityTypes)
                {
                    foreach (var rand in formRandList.Where(f => f.BalIsTotal))
                    {
                        var randTotalList = randValues.Where(f => f.BVC_FormRandId == rand.Id && f.ActivityTypeId == activityType.Id);
                        foreach (var randTotal in randTotalList)
                        {
                            var valueItem = new BugetBalRealizatCalcTotalDto
                            {
                                Calculat = false,
                                RandId = randTotal.BVC_FormRandId,
                                Valoare = 0,
                                ActivityTypeId = activityType.Id
                            };
                            valuesList.Add(valueItem);
                        }
                    }
                }


                // incep calculul
                int contor = 1; bool okCalcul = false;
                while (contor < 50 && !okCalcul)
                {
                    foreach (var activityType in activityTypes)
                    {
                        foreach (var valueItem in valuesList.Where(f => !f.Calculat && f.ActivityTypeId == activityType.Id))
                        {
                            var randForm = formRandList.FirstOrDefault(f => f.Id == valueItem.RandId);
                            string formula = (bvc_Tip == BVC_Tip.BVC) ? randForm.BalFormulaBVC : randForm.BalFormulaCashFlow;
                            var semneList = new List<string>();
                            var codRandList = new List<int>();

                            decimal suma = 0;
                            bool formulaOk = true;

                            var balRealizDetails = new List<BVC_BalRealizatRandDetailsDto>();

                            if (formula != "" && formula != null)
                            {

                                _bugetPrevRepository.DesfacFormula(formula, out semneList, out codRandList);

                                for (int i = 0; i < semneList.Count; i++)
                                {
                                    // iau randul din formular
                                    var randFormCurr = formRandList.FirstOrDefault(f => f.CodRand == codRandList[i]);

                                    // verific daca e rand de total
                                    if (randFormCurr.BalIsTotal)
                                    {
                                        // il caut in valuesList
                                        var valueCurr = valuesList.FirstOrDefault(f => f.RandId == randFormCurr.Id && f.ActivityTypeId == activityType.Id);
                                        if (!valueCurr.Calculat)
                                        {
                                            formulaOk = false;
                                        }
                                        else
                                        {
                                            suma += (semneList[i] == "+" ? 1 : -1) * valueCurr.Valoare;
                                        }
                                    }
                                    else
                                    {
                                        var valueCurr = randValues.FirstOrDefault(f => f.BVC_FormRandId == randFormCurr.Id && f.ActivityTypeId == activityType.Id);
                                        suma += valueCurr.Valoare;
                                    }

                                }
                            }

                            if (formulaOk)
                            {
                                valueItem.Valoare = suma;
                                valueItem.Calculat = true;
                            }

                        }
                    }

                    var count = valuesList.Count(f => !f.Calculat);
                    okCalcul = (count == 0);

                    contor++;
                }



                if (!okCalcul)
                {
                    throw new Exception("Verificati corectitudinea formulelor. Exista elemente care au fost specificate in formula si nu au fost calculate");
                }

                // actualizez valorile pe fiecare rand
                foreach (var valueItem in valuesList)
                {
                    var realizatRand = calcul.RanduriCalc.FirstOrDefault(f => f.BVC_FormRandId == valueItem.RandId && f.ActivityTypeId == valueItem.ActivityTypeId);
                    realizatRand.Valoare = valueItem.Valoare;
                }
                CurrentUnitOfWork.SaveChanges();
                return calcul;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private BVC_BalRealizatCalcDto FormulaBalanta(BVC_BalRealizatCalcDto calcul, DateTime calcDate, int appClientId, int localCurrencyId, BVC_Tip bvc_Tip, List<ActivityType> activityTypeList, SavedBalance savedBalance)
        {
            try
            {
                var formRandList = _bugetFormRandRepository.GetAllIncluding(f => f.Formular)
                                                    .Where(f => f.Formular.AnBVC == calcDate.Year && f.Formular.State == State.Active
                                                                    && f.TenantId == appClientId)
                                                    .ToList();


                var savedBalanceDetails = _savedBalanceDetailsRepository.GetAllIncluding(f => f.SavedBalance, f => f.Account).Where(f => f.SavedBalanceId == savedBalance.Id &&
                                                                                                                         f.SavedBalance.TenantId == appClientId).ToList();

                if (bvc_Tip == BVC_Tip.BVC)
                {
                    formRandList = formRandList.Where(f => f.AvailableBVC).OrderBy(f => f.OrderView).ToList();
                }


                foreach (var activityType in activityTypeList)
                {
                    foreach (var rand in formRandList.Where(f => !f.BalIsTotal && f.BalFormulaBVC != null))
                    {
                        var balRealizatDetails = new List<BVC_BalRealizatRandDetails>();
                        string formula = (bvc_Tip == BVC_Tip.BVC) ? rand.BalFormulaBVC : rand.BalFormulaCashFlow;
                        decimal rez = _bugetBalRealizatRepository.FormulaBalantaCalc(formula, calcDate, appClientId, localCurrencyId, activityType.Id, savedBalanceDetails, out balRealizatDetails);
                        var balRealizatRand = calcul.RanduriCalc.FirstOrDefault(f => f.BVC_FormRandId == rand.Id && f.ActivityTypeId == activityType.Id);
                        balRealizatRand.Valoare += rez;
                        balRealizatRand.BVC_BalRealizatRandDetails.AddRange(balRealizatDetails.Select(f => new BVC_BalRealizatRandDetailsDto
                        {
                            Descriere = f.Descriere,
                            Valoare = f.Valoare
                        }));
                    }
                }
                CurrentUnitOfWork.SaveChanges();
                return calcul;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion

        #region BVC_Realizat
        private BVC_RealizatCalcDto BVC_RealizatTotaluri(BVC_RealizatCalcDto calcul, DateTime calcDate, int appClientId, int localCurrencyId, BVC_Tip bvc_Tip)
        {
            try
            {
                var formRandDetails = _bugetFormRandRepository.GetAllIncluding(f => f.Formular)
                                                                     .Where(f => f.Formular.AnBVC == calcDate.Year && f.Formular.State == State.Active
                                                                            && f.TenantId == appClientId)
                                                                     .ToList();

                if (bvc_Tip == BVC_Tip.BVC)
                {
                    formRandDetails = formRandDetails.Where(f => f.AvailableBVC).ToList();
                }

                if (bvc_Tip == BVC_Tip.CashFlow)
                {
                    formRandDetails = formRandDetails.Where(f => f.AvailableCashFlow).ToList();
                }

                var randValues = calcul.RanduriCalc.ToList();
                var randTotalIdsList = formRandDetails.Where(f => f.IsTotal).Select(f => f.Id).ToList();

                var valuesList = new List<BugetRealizatCalcTotalDto>();

                // initializez totalurile
                foreach (var rand in formRandDetails.Where(f => f.IsTotal))
                {
                    var randTotalList = randValues.Where(f => f.BVC_FormRandId == rand.Id);
                    foreach (var randTotal in randTotalList)
                    {
                        var valueItem = new BugetRealizatCalcTotalDto
                        {
                            Calculat = false,
                            RandId = randTotal.BVC_FormRandId,
                            Valoare = 0
                        };
                        valuesList.Add(valueItem);
                    }
                }

                // incep calculul
                int contor = 1; bool okCalcul = false;
                while (contor < 50 && !okCalcul)
                {
                    foreach (var valueItem in valuesList.Where(f => !f.Calculat))
                    {
                        var randForm = formRandDetails.FirstOrDefault(f => f.Id == valueItem.RandId);
                        string formula = (bvc_Tip == BVC_Tip.BVC) ? randForm.FormulaBVC : randForm.FormulaCashFlow;
                        var semneList = new List<string>();
                        var codRandList = new List<int>();

                        decimal suma = 0;
                        bool formulaOk = true;

                        if (formula != "" && formula != null)
                        {

                            _bugetPrevRepository.DesfacFormula(formula, out semneList, out codRandList);

                            for (int i = 0; i < semneList.Count; i++)
                            {
                                // iau randul din formular
                                var randFormCurr = formRandDetails.FirstOrDefault(f => f.CodRand == codRandList[i]);

                                // verific daca e rand de total
                                if (randFormCurr.IsTotal)
                                {
                                    // il caut in valuesList
                                    var valueCurr = valuesList.FirstOrDefault(f => f.RandId == randFormCurr.Id);
                                    if (valueCurr == null)
                                    {
                                        formulaOk = false;
                                    }
                                    if (!valueCurr.Calculat)
                                    {
                                        formulaOk = false;
                                    }
                                    else
                                    {
                                        suma += (semneList[i] == "+" ? 1 : -1) * valueCurr.Valoare;
                                    }
                                }
                                else
                                {
                                    var valueCurr = randValues.FirstOrDefault(f => f.BVC_FormRandId == randFormCurr.Id);
                                    suma += valueCurr.ValoareFaraReferat;
                                }
                            }
                        }

                        if (formulaOk)
                        {
                            valueItem.Valoare = suma;
                            valueItem.Calculat = true;
                        }

                    }
                    var count = valuesList.Count(f => !f.Calculat);
                    okCalcul = (count == 0);

                    contor++;
                }

                if (!okCalcul)
                {
                    throw new Exception("Verificati corectitudinea formulelor. Exista elemente care au fost specificate in formula si nu au fost calculate");
                }

                // actualizez valorile pe fiecare rand
                foreach (var valueItem in valuesList)
                {
                    var realizatRand = calcul.RanduriCalc.FirstOrDefault(f => f.BVC_FormRandId == valueItem.RandId);
                    realizatRand.ValoareFaraReferat = valueItem.Valoare;
                }
                CurrentUnitOfWork.SaveChanges();
                return calcul;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private BVC_RealizatCalcDto FormulaConta(BVC_RealizatCalcDto calcul, DateTime calcDate, int appClientId, int localCurrencyId, BVC_Tip bvc_Tip)
        {
            try
            {
                var formRandDetails = _bugetFormRandDetailsRepository.GetAllIncluding(f => f.FormRand, f => f.FormRand.Formular)
                                                    .Where(f => f.FormRand.Formular.AnBVC == calcDate.Year && f.FormRand.Formular.State == State.Active
                                                                    && f.FormRand.TenantId == appClientId)
                                                    .ToList();

                if (bvc_Tip == BVC_Tip.BVC)
                {
                    formRandDetails = formRandDetails.Where(f => f.FormRand.AvailableBVC).ToList();
                    formRandDetails = formRandDetails.Where(f => !f.FormRand.IsTotal).ToList();
                    formRandDetails = formRandDetails.Where(f => (f.FormRand.FormulaBVC ?? "") != "").ToList();
                }

                if (bvc_Tip == BVC_Tip.CashFlow)
                {
                    formRandDetails = formRandDetails.Where(f => f.FormRand.AvailableCashFlow).ToList();
                    formRandDetails = formRandDetails.Where(f => !f.FormRand.IsTotal).ToList();
                    formRandDetails = formRandDetails.Where(f => (f.FormRand.FormulaCashFlow ?? "") != "").ToList();
                }


                foreach (var rand in formRandDetails)
                {
                    string formula = (bvc_Tip == BVC_Tip.BVC) ? rand.FormRand.FormulaBVC : rand.FormRand.FormulaCashFlow;
                    decimal rez = _bugetRealizatRepository.FormulaContaCalc(formula, calcDate, appClientId, localCurrencyId);
                    var realizatRand = calcul.RanduriCalc.FirstOrDefault(f => f.BVC_FormRandId == rand.FormRandId);
                    realizatRand.ValoareFaraReferat += rez;
                }
                CurrentUnitOfWork.SaveChanges();
                return calcul;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private BVC_RealizatCalcDto BVC_CashFlowPlasamenteCalc(BVC_RealizatCalcDto calcul, DateTime calcDate, int appClientId, int localCurrencyId)
        {
            try
            {
                var activeListCashFlow = _activeBugetBVCManager.ActiveBugetCFList(calcDate);
                foreach (var item in activeListCashFlow)
                {
                    var formRandDetail = _bugetFormRandDetailsRepository.GetAllIncluding(f => f.FormRand, f => f.FormRand.Formular, f => f.TipRandCheltuiala)
                                                                        .FirstOrDefault(f => f.FormRand.Formular.AnBVC == calcDate.Year && f.FormRand.Formular.State == State.Active
                                                                                        && f.FormRand.TenantId == appClientId && f.FormRand.TipRandVenit == BVC_RowTypeIncome.Dobanzi
                                                                                        && f.FormRand.TipRand == BVC_RowType.Venituri && f.FormRand.AvailableCashFlow);
                    if (formRandDetail != null)
                    {
                        var realizatRand = calcul.RanduriCalc.FirstOrDefault(f => f.BVC_FormRandId == formRandDetail.FormRandId);
                        var currency = _currencyRepository.FirstOrDefault(f => f.Id == localCurrencyId);
                        var itemCurrency = _currencyRepository.FirstOrDefault(f => f.CurrencyCode == item.moneda);

                        if (item.moneda == currency.CurrencyCode)
                        {
                            realizatRand.ValoareFaraReferat += item.valoareDobandaCumulataPanaLaLuna + item.valoarePlasament;
                        }
                        else
                        {

                            var exchangeRate = _exchangeRatesRepository.GetExchangeRateForOper(item.maturityDate, itemCurrency.Id, localCurrencyId);
                            realizatRand.ValoareFaraReferat += Math.Round(item.valoareDobandaCumulataPanaLaLuna + item.valoarePlasament, 2);
                        }
                        var realizatRandDet = new BVC_RealizatRandDetailsDto
                        {
                            Descriere = item.idplasament,
                            Valoare = item.valoareDobandaCumulataPanaLaLuna + item.valoarePlasament,
                            CurrencyId = itemCurrency.Id

                        };
                        realizatRand.BVC_RealizatRandDetails.Add(realizatRandDet);
                    }
                }
                CurrentUnitOfWork.SaveChanges();
                return calcul;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private BVC_RealizatCalcDto BVC_SalarizareCashFlowCalc(BVC_RealizatCalcDto calcul, DateTime calcDate, int appClientId, int localCurrencyId, int bugetRealizatId)
        {
            try
            {
                var salarizareCashFlowRealizat = _activeBugetBVCManager.SalarizareCFRealizatList(calcDate);
                foreach (var item in salarizareCashFlowRealizat)
                {
                    var tipRandSalarizare = ReturnValueFromStr(item.TipPlata);

                    foreach (var calculRand in calcul.RanduriCalc)
                    {
                        var randSalarizare = _bugetFormRandRepository.FirstOrDefault(f => f.TipRand == BVC_RowType.Salarizare && f.TipRandSalarizare == tipRandSalarizare &&
                                                                  f.Id == calculRand.BVC_FormRandId && !f.IsTotal);
                        if (randSalarizare != null)
                        {
                            calculRand.ValoareFaraReferat += item.Valoare;
                        }
                    }

                }
                CurrentUnitOfWork.SaveChanges();
                return calcul;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private BVC_RowTypeSalarizare ReturnValueFromStr(string value)
        {
            var ret = new BVC_RowTypeSalarizare();
            switch (value.ToUpper())
            {
                case "SALARII":
                    ret = BVC_RowTypeSalarizare.Salarii;
                    break;
                case "PRIME CO":
                    ret = BVC_RowTypeSalarizare.PrimaCO;
                    break;
                case "PRIMA DE ZIUA FONDULUI":
                    ret = BVC_RowTypeSalarizare.PrimaZiuaFondului;
                    break;
                case "CADOU 1 IUNIE":
                    ret = BVC_RowTypeSalarizare.Cadou1Iunie;
                    break;
                case "CADOU 8 MARTIE":
                    ret = BVC_RowTypeSalarizare.Cadou8Martie;
                    break;
                case "CADOU CRACIUN":
                    ret = BVC_RowTypeSalarizare.CadouCraciun;
                    break;
                case "TINUTE VESTIMENTARE":
                    ret = BVC_RowTypeSalarizare.TinuteVestimentare;
                    break;
                case "FOND PROFIT":
                    ret = BVC_RowTypeSalarizare.FondProfit;
                    break;
                case "CONTRIBUTII SI CS":
                    ret = BVC_RowTypeSalarizare.ContibutiiSiCS;
                    break;

            }

            return ret;
        }

        private BVC_RealizatCalcDto BVC_PlasamenteCalc(BVC_RealizatCalcDto calcul, DateTime calcDate, int appClientId, int localCurrencyId)
        {
            try
            {
                var activeListBVC = _activeBugetBVCManager.ActiveBugetBVCList(calcDate);
                foreach (var item in activeListBVC)
                {
                    var formRandDetail = _bugetFormRandDetailsRepository.GetAllIncluding(f => f.FormRand, f => f.FormRand.Formular, f => f.TipRandCheltuiala)
                                                                        .FirstOrDefault(f => f.FormRand.Formular.AnBVC == calcDate.Year && f.FormRand.Formular.State == State.Active
                                                                                        && f.FormRand.TenantId == appClientId && f.FormRand.TipRandVenit == BVC_RowTypeIncome.Dobanzi
                                                                                        && f.FormRand.TipRand == BVC_RowType.Venituri && f.FormRand.AvailableBVC);
                    var realizatRand = calcul.RanduriCalc.FirstOrDefault(f => f.BVC_FormRandId == formRandDetail.FormRandId);
                    var currency = _currencyRepository.FirstOrDefault(f => f.Id == localCurrencyId);
                    var itemCurrency = _currencyRepository.FirstOrDefault(f => f.CurrencyCode == item.moneda);

                    if (item.moneda == currency.CurrencyCode)
                    {
                        realizatRand.ValoareFaraReferat += item.valoareDobandaLuna;
                    }
                    else
                    {

                        var exchangeRate = _exchangeRatesRepository.GetExchangeRateForOper(calcDate, itemCurrency.Id, localCurrencyId);
                        realizatRand.ValoareFaraReferat += Math.Round(item.valoareDobandaLuna, 2);
                    }

                    var realizatRandDet = new BVC_RealizatRandDetailsDto
                    {
                        Descriere = item.idplasament,
                        Valoare = item.valoareDobandaLuna,
                        CurrencyId = itemCurrency.Id

                    };
                    realizatRand.BVC_RealizatRandDetails.Add(realizatRandDet);
                }
                CurrentUnitOfWork.SaveChanges();
                return calcul;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private BVC_RealizatCalcDto BVC_InvoiceCalcCA(BVC_RealizatCalcDto calcul, DateTime calcDate, int appClientId)
        {
            try
            {
                var prepaymentsDetails = _prepaymentBalanceRepository.GetAllIncluding(f => f.Prepayment, f => f.Prepayment.InvoiceDetails, f => f.Prepayment.InvoiceDetails.InvoiceElementsDetails,
                                                                                      f => f.Prepayment.InvoiceDetails.Invoices, f => f.Prepayment.InvoiceDetails.Invoices.Currency)
                                                                     .Where(f => f.Prepayment.InvoiceDetails.Invoices.State == State.Active && f.Prepayment.InvoiceDetails.Invoices.TenantId == appClientId &&
                                                                            f.ComputeDate == calcDate && f.OperType == PrepaymentOperType.AmortizareLunara)
                                                                     .ToList();

                foreach (var detail in prepaymentsDetails)
                {
                    var formRandDetail = _bugetFormRandDetailsRepository.GetAllIncluding(f => f.FormRand, f => f.FormRand.Formular, f => f.TipRandCheltuiala)
                                                                        .FirstOrDefault(f => f.FormRand.Formular.AnBVC == calcDate.Year && f.FormRand.Formular.State == State.Active
                                                                                        && f.TipRandCheltuialaId == detail.Prepayment.InvoiceDetails.InvoiceElementsDetailsId && f.FormRand.AvailableBVC);

                    if (formRandDetail == null)
                    {
                        var exceptie = new BVC_RealizatExceptiiDto
                        {
                            Descriere = detail.Prepayment.InvoiceDetails.Invoices.InvoiceNumber + " " + LazyMethods.DateToString(detail.Prepayment.InvoiceDetails.Invoices.InvoiceDate) + " - " +
                                        detail.Prepayment.InvoiceDetails.Element,
                            Valoare = detail.TranzDeprec
                        };
                        calcul.Exceptii.Add(exceptie);
                    }
                    else
                    {
                        var realizatRand = calcul.RanduriCalc.FirstOrDefault(f => f.BVC_FormRandId == formRandDetail.FormRandId);
                        realizatRand.ValoareFaraReferat += detail.TranzDeprec;

                        var realizatRandDet = new BVC_RealizatRandDetailsDto
                        {
                            Descriere = detail.Prepayment.InvoiceDetails.Invoices.InvoiceNumber + " " + LazyMethods.DateToString(detail.Prepayment.InvoiceDetails.Invoices.InvoiceDate) + " - " + detail.Prepayment.InvoiceDetails.Element,
                            Valoare = detail.TranzDeprec,
                            CurrencyId = detail.Prepayment.InvoiceDetails.Invoices.CurrencyId
                        };

                        realizatRand.BVC_RealizatRandDetails.Add(realizatRandDet);
                    }

                }
                CurrentUnitOfWork.SaveChanges();
                return calcul;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private BVC_RealizatCalcDto BVC_InvoiceCalcMF(BVC_RealizatCalcDto calcul, DateTime calcDate, int appClientId)
        {
            try
            {
                var imoAssetDetails = _imoGestRepository.GetAllIncluding(f => f.ImoAssetItem, f => f.ImoAssetItem.InvoiceDetails, f => f.ImoAssetItem.InvoiceDetails.InvoiceElementsDetails,
                                                                         f => f.ImoAssetItem.InvoiceDetails.Invoices, f => f.ImoAssetItem.InvoiceDetails.Invoices.Currency)
                                                        .Where(f => f.ImoAssetItem.InvoiceDetails.Invoices.State == State.Active && f.ImoAssetItem.InvoiceDetails.Invoices.TenantId == appClientId &&
                                                               f.StockDate == calcDate && f.OperType == ImoAssetOperType.AmortizareLunara)
                                                        .ToList();
                foreach (var detail in imoAssetDetails)
                {
                    var formRandDetail = _bugetFormRandDetailsRepository.GetAllIncluding(f => f.FormRand, f => f.FormRand.Formular, f => f.TipRandCheltuiala)
                                                                         .FirstOrDefault(f => f.FormRand.Formular.AnBVC == calcDate.Year && f.FormRand.Formular.State == State.Active
                                                                                         && f.TipRandCheltuialaId == detail.ImoAssetItem.InvoiceDetails.InvoiceElementsDetailsId && f.FormRand.AvailableBVC);

                    if (formRandDetail == null)
                    {
                        var exceptie = new BVC_RealizatExceptiiDto
                        {
                            Descriere = detail.ImoAssetItem.InvoiceDetails.Invoices.InvoiceNumber + " " + LazyMethods.DateToString(detail.ImoAssetItem.InvoiceDetails.Invoices.InvoiceDate) + " - " +
                                        detail.ImoAssetItem.InvoiceDetails.Element,
                            Valoare = detail.TranzDeprec
                        };
                        calcul.Exceptii.Add(exceptie);
                    }
                    else
                    {
                        var realizatRand = calcul.RanduriCalc.FirstOrDefault(f => f.BVC_FormRandId == formRandDetail.FormRandId);
                        realizatRand.ValoareFaraReferat += detail.TranzDeprec;

                        var realizatRandDet = new BVC_RealizatRandDetailsDto
                        {
                            Descriere = detail.ImoAssetItem.InvoiceDetails.Invoices.InvoiceNumber + " " + LazyMethods.DateToString(detail.ImoAssetItem.InvoiceDetails.Invoices.InvoiceDate) + " - " + detail.ImoAssetItem.InvoiceDetails.Element,
                            Valoare = detail.TranzDeprec,
                            CurrencyId = detail.ImoAssetItem.InvoiceDetails.Invoices.CurrencyId

                        };
                        realizatRand.BVC_RealizatRandDetails.Add(realizatRandDet);
                    }
                }
                CurrentUnitOfWork.SaveChanges();
                return calcul;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private BVC_RealizatCalcDto BVC_CashFlowInvoiceCalc(BVC_RealizatCalcDto calcul, DateTime calcDate, int appClientId, int localCurrencyId)
        {
            try
            {
                DateTime startDate = new DateTime(calcDate.Year, calcDate.Month, 1);
                var invoiceDetails = new List<InvoiceDetails>();

                var invoiceDetailsFromDecont = _invoiceDetailsRepository.GetAllIncluding(f => f.InvoiceElementsDetails, f => f.InvoiceElementsDetails.InvoiceElementsDetailsCategory, f => f.Invoices, f => f.Invoices.Decont, f => f.Invoices.Currency)
                                              .Where(f => f.State == State.Active && f.Invoices.TenantId == appClientId
                                                     && f.Invoices.HasDecont == true && f.Invoices.Decont.State == State.Active
                                                     && startDate <= f.Invoices.Decont.DecontDate && f.Invoices.Decont.DecontDate <= calcDate)
                                              .ToList();
                invoiceDetails.AddRange(invoiceDetailsFromDecont);


                var opInvoices = _paymentOrderInvoiceRepository.GetAllIncluding(f => f.Invoice, f => f.Invoice.InvoiceDetails, f => f.PaymentOrder, f => f.Invoice.Currency, f => f.Invoice.PaymentOrderInvoices, f => f.Invoice.DispositionInvoices)
                                                               .Where(f => f.State == State.Active && f.TenantId == appClientId && startDate <= f.PaymentOrder.OrderDate && f.PaymentOrder.OrderDate <= calcDate)
                                                               .ToList();

                foreach (var item in opInvoices)
                {
                    foreach (var detail in item.Invoice.InvoiceDetails)
                    {
                        invoiceDetails.Add(detail);
                    }
                }


                var dispositionInvoiceList = _dispositionInvoiceRepository.GetAllIncluding(f => f.Disposition, f => f.Invoice, f => f.Invoice.Currency, f => f.Invoice.PaymentOrderInvoices, f => f.Invoice.DispositionInvoices)
                                                                   .Where(f => f.State == State.Active && f.TenantId == appClientId && startDate <= f.Disposition.DispositionDate && f.Disposition.DispositionDate <= calcDate)
                                                                   .ToList();
                foreach (var dispo in dispositionInvoiceList)
                {
                    if (dispo.Invoice.InvoiceDetails != null)
                    {
                        foreach (var det in dispo.Invoice.InvoiceDetails)
                        {
                            invoiceDetails.Add(det);
                        }
                    }
                }


                foreach (var detail in invoiceDetails)
                {
                    var formRandDetail = _bugetFormRandDetailsRepository.GetAllIncluding(f => f.FormRand, f => f.FormRand.Formular, f => f.TipRandCheltuiala)
                                                                        .FirstOrDefault(f => f.FormRand.Formular.AnBVC == calcDate.Year && f.FormRand.Formular.State == State.Active
                                                                                        && f.TipRandCheltuialaId == detail.InvoiceElementsDetailsId && f.FormRand.AvailableCashFlow);
                    if (formRandDetail == null)
                    {
                        var exceptie = new BVC_RealizatExceptiiDto
                        {
                            Descriere = detail.Invoices.InvoiceNumber + " " + LazyMethods.DateToString(detail.Invoices.InvoiceDate) + " - " + detail.Element,
                            Valoare = detail.Invoices.HasDecont == true ? detail.Value : detail.Invoices.TotalPlatit
                        };
                        calcul.Exceptii.Add(exceptie);
                    }
                    else
                    {
                        var realizatRand = calcul.RanduriCalc.FirstOrDefault(f => f.BVC_FormRandId == formRandDetail.FormRandId);
                        if (detail.Invoices.CurrencyId == localCurrencyId)
                        {
                            realizatRand.ValoareFaraReferat += detail.Invoices.HasDecont == true ? detail.Value : detail.Invoices.TotalPlatit;
                        }
                        else
                        {
                            var exchangeRate = _exchangeRatesRepository.GetExchangeRateForOper(detail.Invoices.InvoiceDate, detail.Invoices.CurrencyId, localCurrencyId);
                            realizatRand.ValoareFaraReferat += Math.Round(detail.Invoices.HasDecont == true ? detail.Value : detail.Invoices.TotalPlatit * exchangeRate, 2);
                        }

                        var realizatRandDet = new BVC_RealizatRandDetailsDto
                        {
                            Descriere = detail.Invoices.InvoiceNumber + " " + LazyMethods.DateToString(detail.Invoices.InvoiceDate) + " - " + detail.Element,
                            Valoare = detail.Invoices.HasDecont == true ? detail.Value : detail.Invoices.TotalPlatit,
                            CurrencyId = detail.Invoices.CurrencyId

                        };
                        realizatRand.BVC_RealizatRandDetails.Add(realizatRandDet);
                    }
                }
                CurrentUnitOfWork.SaveChanges();
                return calcul;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private BVC_RealizatCalcDto BVC_InvoiceCalc(BVC_RealizatCalcDto calcul, DateTime calcDate, int appClientId, int localCurrencyId)
        {
            try
            {
                DateTime startDate = new DateTime(calcDate.Year, calcDate.Month, 1);
                var invoiceDetails = _invoiceDetailsRepository.GetAllIncluding(f => f.InvoiceElementsDetails, f => f.InvoiceElementsDetails.InvoiceElementsDetailsCategory, f => f.Invoices, f => f.Invoices.Currency)
                                                              .Where(f => f.State == State.Active && f.Invoices.TenantId == appClientId
                                                                     && startDate <= f.Invoices.OperationDate && f.Invoices.OperationDate <= calcDate
                                                                     && f.DurationInMonths == 0 && f.InvoiceElementsDetails.InvoiceElementsType != InvoiceElementsType.MijloaceFixe)
                                                              .ToList();
                foreach (var detail in invoiceDetails)
                {
                    var formRandDetail = _bugetFormRandDetailsRepository.GetAllIncluding(f => f.FormRand, f => f.FormRand.Formular, f => f.TipRandCheltuiala)
                                                                        .FirstOrDefault(f => f.FormRand.Formular.AnBVC == calcDate.Year && f.FormRand.Formular.State == State.Active
                                                                                        && f.TipRandCheltuialaId == detail.InvoiceElementsDetailsId && f.FormRand.AvailableBVC);
                    if (formRandDetail == null)
                    {
                        var exceptie = new BVC_RealizatExceptiiDto
                        {
                            Descriere = detail.Invoices.InvoiceNumber + " " + LazyMethods.DateToString(detail.Invoices.InvoiceDate) + " - " + detail.Element,
                            Valoare = detail.Value
                        };
                        calcul.Exceptii.Add(exceptie);
                    }
                    else
                    {
                        var realizatRand = calcul.RanduriCalc.FirstOrDefault(f => f.BVC_FormRandId == formRandDetail.FormRandId);
                        if (detail.Invoices.CurrencyId == localCurrencyId)
                        {
                            realizatRand.ValoareFaraReferat += detail.Value;
                        }
                        else
                        {
                            var exchangeRate = _exchangeRatesRepository.GetExchangeRateForOper(detail.Invoices.InvoiceDate, detail.Invoices.CurrencyId, localCurrencyId);
                            realizatRand.ValoareFaraReferat += Math.Round(detail.Value * exchangeRate, 2);
                        }

                        var realizatRandDet = new BVC_RealizatRandDetailsDto
                        {
                            Descriere = detail.Invoices.InvoiceNumber + " " + LazyMethods.DateToString(detail.Invoices.InvoiceDate) + " - " + detail.Element,
                            Valoare = detail.Value,
                            CurrencyId = detail.Invoices.CurrencyId

                        };
                        realizatRand.BVC_RealizatRandDetails.Add(realizatRandDet);
                    }
                }
                CurrentUnitOfWork.SaveChanges();
                return calcul;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private BVC_RealizatCalcDto BVC_PAAP_Referate(BVC_RealizatCalcDto calcul, DateTime calcDate, int appClientId, int localCurrencyId)
        {
            try
            {
                DateTime startDate = new DateTime(calcDate.Year, calcDate.Month, 1);
                var paapInvoiceDetailsIdsList = _bvcPAAPInvocieDetRepository.GetAllIncluding(f => f.BVC_PAAP, f => f.InvoiceDetails)
                                                                            .Where(f => f.TenantId == appClientId && f.BVC_PAAP.State == State.Active && f.InvoiceDetails.State == State.Active)
                                                                            .GroupBy(f => f.BVC_PAAPId)
                                                                            .Select(g => g.Key)
                                                                            .ToList();

                var referatePAAPList = _bvcPaapReferatRepository.GetAllIncluding(f => f.BVC_PAAP, f => f.BVC_PAAP.InvoiceElementsDetails)
                                                            .Where(f => f.State == State.Active && f.TenantId == appClientId &&
                                                                   startDate <= f.OperationDate && f.OperationDate <= calcDate &&
                                                                   !paapInvoiceDetailsIdsList.Contains(f.BVC_PAAP_Id))
                                                            .ToList();

                foreach (var item in referatePAAPList)
                {
                    var formRandDetail = _bugetFormRandDetailsRepository.GetAllIncluding(f => f.FormRand, f => f.FormRand.Formular, f => f.TipRandCheltuiala)
                                                                        .FirstOrDefault(f => f.FormRand.Formular.AnBVC == calcDate.Year && f.FormRand.Formular.State == State.Active
                                                                                        && f.TipRandCheltuialaId == item.BVC_PAAP.InvoiceElementsDetailsId && f.FormRand.AvailableBVC);
                    if (formRandDetail == null)
                    {
                        var exceptie = new BVC_RealizatExceptiiDto
                        {
                            Descriere = LazyMethods.DateToString(item.OperationDate) + " - " + item.Name,
                            Valoare = item.Suma
                        };
                        calcul.Exceptii.Add(exceptie);
                    }
                    else
                    {
                        var realizatRand = calcul.RanduriCalc.FirstOrDefault(f => f.BVC_FormRandId == formRandDetail.FormRandId);
                        realizatRand.ValoareCuReferat += item.Suma;
                        var realizatRandDet = new BVC_RealizatRandDetailsDto
                        {
                            Descriere = LazyMethods.DateToString(item.OperationDate) + " - " + item.Name,
                            Valoare = item.Suma,
                            CurrencyId = item.BVC_PAAP.CurrencyId.Value

                        };
                        realizatRand.BVC_RealizatRandDetails.Add(realizatRandDet);
                    }
                }
                CurrentUnitOfWork.SaveChanges();
                return calcul;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        private BVC_RealizatCalcDto BVC_CashFlowPAAP_Referate(BVC_RealizatCalcDto calcul, DateTime calcDate, int appClientId, int localCurrencyId)
        {
            try
            {
                DateTime startDate = new DateTime(calcDate.Year, calcDate.Month, 1);
                var paapInvoiceDetailsIdsList = _bvcPAAPInvocieDetRepository.GetAllIncluding(f => f.BVC_PAAP, f => f.InvoiceDetails)
                                                                            .Where(f => f.TenantId == appClientId && f.BVC_PAAP.State == State.Active && f.InvoiceDetails.State == State.Active)
                                                                            .GroupBy(f => f.BVC_PAAPId)
                                                                            .Select(g => g.Key)
                                                                            .ToList();

                var referatePAAPList = _bvcPaapReferatRepository.GetAllIncluding(f => f.BVC_PAAP, f => f.BVC_PAAP.InvoiceElementsDetails)
                                                            .Where(f => f.State == State.Active && f.TenantId == appClientId &&
                                                                   startDate <= f.OperationDate && f.OperationDate <= calcDate &&
                                                                   !paapInvoiceDetailsIdsList.Contains(f.BVC_PAAP_Id))
                                                            .ToList();

                foreach (var item in referatePAAPList)
                {
                    var formRandDetail = _bugetFormRandDetailsRepository.GetAllIncluding(f => f.FormRand, f => f.FormRand.Formular, f => f.TipRandCheltuiala)
                                                                        .FirstOrDefault(f => f.FormRand.Formular.AnBVC == calcDate.Year && f.FormRand.Formular.State == State.Active
                                                                                        && f.TipRandCheltuialaId == item.BVC_PAAP.InvoiceElementsDetailsId && f.FormRand.AvailableCashFlow);
                    if (formRandDetail == null)
                    {
                        var exceptie = new BVC_RealizatExceptiiDto
                        {
                            Descriere = LazyMethods.DateToString(item.OperationDate) + " - " + item.Name,
                            Valoare = item.Suma
                        };
                        calcul.Exceptii.Add(exceptie);
                    }
                    else
                    {
                        var realizatRand = calcul.RanduriCalc.FirstOrDefault(f => f.BVC_FormRandId == formRandDetail.FormRandId);
                        realizatRand.ValoareCuReferat += item.Suma;
                        var realizatRandDet = new BVC_RealizatRandDetailsDto
                        {
                            Descriere = LazyMethods.DateToString(item.OperationDate) + " - " + item.Name,
                            Valoare = item.Suma,
                            CurrencyId = item.BVC_PAAP.CurrencyId.Value

                        };
                        realizatRand.BVC_RealizatRandDetails.Add(realizatRandDet);
                    }
                }
                CurrentUnitOfWork.SaveChanges();
                return calcul;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public List<BugetBalRealizatSavedBalanceDateDto> BugetBalRealizatSavedBalanceDateList(int formularId, int bvcTip)
        {
            // BugetBalRealizatSavedBalanceDateDto
            try
            {
                var appClient = GetCurrentTenant();
                var ret = new List<BugetBalRealizatSavedBalanceDateDto>();
                var bugetbalRealizatList = _bugetBalRealizatRepository.GetAllIncluding(f => f.SavedBalance)
                                                  .Where(f => f.TenantId == appClient.Id && f.BVC_FormularId == formularId && f.BVC_Tip == (BVC_Tip)bvcTip && f.BVC_Formular.State == State.Active)
                                                  .OrderByDescending(f => f.SavedBalance.SaveDate)
                                                  .ToList();
                foreach (var item in bugetbalRealizatList)
                {

                    ret.Add(new BugetBalRealizatSavedBalanceDateDto
                    {
                        Id = item.Id,
                        Descriere = LazyMethods.DateToString(item.SavedBalance.SaveDate) + " - " + item.SavedBalance.BalanceName
                    });
                }

                return ret;

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public List<BugetRealizatSavedBalanceDateDto> BugetRealizatSavedBalanceDateList(int formularId, int bvcTip)
        {
            try
            {
                var appClient = GetCurrentTenant();
                var ret = new List<BugetRealizatSavedBalanceDateDto>();
                var bugetRealizatList = _bugetRealizatRepository.GetAllIncluding(f => f.SavedBalance)
                                                  .Where(f => f.TenantId == appClient.Id && f.BVC_FormularId == formularId && f.BVC_Tip == (BVC_Tip)bvcTip)
                                                  .OrderByDescending(f => f.SavedBalance.SaveDate)
                                                  .ToList();
                foreach (var item in bugetRealizatList)
                {
                    var bvc_BalRealizatId = _bugetBalRealizatRepository.GetAllIncluding(f => f.SavedBalance, f => f.BVC_Formular)
                                                   .FirstOrDefault(f => f.BVC_FormularId == item.BVC_FormularId && f.TenantId == appClient.Id && f.BVC_Formular.State == State.Active && f.BVC_Tip == item.BVC_Tip)?.Id;
                    ret.Add(new BugetRealizatSavedBalanceDateDto
                    {
                        Id = item.Id,
                        BalRealizatId = bvc_BalRealizatId,
                        Descriere = LazyMethods.DateToString(item.SavedBalance.SaveDate) + " - " + item.SavedBalance.BalanceName
                    });
                }

                return ret;

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }
        #endregion
    }
}
