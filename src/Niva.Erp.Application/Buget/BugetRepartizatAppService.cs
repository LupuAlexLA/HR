using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Models.Buget;
using Niva.Erp.Models.Conta;
using Niva.Erp.Repositories.Buget;
using Niva.Erp.Repositories.Conta;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Buget
{
    public interface IBugetRepartizatAppService : IApplicationService
    {
        List<BugetPreliminatListDto> BugetPreliminatList();
        List<BugetFormularListDto> BugetPreliminatNewList();
        List<BugetRepartizatDto> BugetRepartizatAddManual(int formularId);
        List<BugetRepartizatDto> BugetRepartizatManualDetails(int preliminatId, int formularId);
        void BugetRepartizatAddManualSave(int preliminatId, int formularId, List<BugetRepartizatDto> manualList);
        void BugetRepartizatDelete(int preliminatId);
        int BugetRepartizatAddBalanta(int formularId);
        List<BugetRepartizatDto> BugetRepartizatBalantaDetails(int preliminatId, int formularId);
        void BugetRepartizatAddBalantaSave(List<BugetRepartizatDto> venituriList);
        int BugetRepartizatAddPrelimVenit(int formularId, int bugetPrevId);
    }

    public class BugetRepartizatAppService : ErpAppServiceBase, IBugetRepartizatAppService
    {

        IRepository<BVC_VenitProcRepartiz> _BugetRepartizatRepository;
        IRepository<BVC_Formular> _BugetFormularRepository;
        IRepository<ActivityType> _ActivityRepository;
        IRepository<BVC_VenitBugetPrelim> _bugetPrelimRepository;
        IBalanceRepository _balanceRepository;
        IRepository<BalanceDetails> _balanceDetailsRepository;
        IBVC_BugetPrevRepository _bugetPrevRepository;
        IRepository<BVC_BugetPrevRand> _bugetPrevRandRepository;
        IRepository<BVC_BugetPrevRandValue> _bugetPrevRandValueRepository;

        public BugetRepartizatAppService(IRepository<BVC_VenitProcRepartiz> BugetRepartizatRepository, IRepository<BVC_Formular> BugetFormularRepository, IRepository<ActivityType> ActivityRepository,
                                         IRepository<BVC_VenitBugetPrelim> bugetPrelimRepository, IBalanceRepository balanceRepository, IRepository<BalanceDetails> balanceDetailsRepository,
                                         IBVC_BugetPrevRepository bugetPrevRepository, IRepository<BVC_BugetPrevRand> bugetPrevRandRepository, IRepository<BVC_BugetPrevRandValue> bugetPrevRandValueRepository)
        {
            _BugetRepartizatRepository = BugetRepartizatRepository;
            _BugetFormularRepository = BugetFormularRepository;
            _ActivityRepository = ActivityRepository;
            _bugetPrelimRepository = bugetPrelimRepository;
            _balanceRepository = balanceRepository;
            _balanceDetailsRepository = balanceDetailsRepository;
            _bugetPrevRepository = bugetPrevRepository;
            _bugetPrevRandRepository = bugetPrevRandRepository;
            _bugetPrevRandValueRepository = bugetPrevRandValueRepository;
        }

        [AbpAuthorize("Buget.BVC.Preliminat")]
        public List<BugetPreliminatListDto> BugetPreliminatList()
        {
            try
            {
                var ret = _bugetPrelimRepository.GetAllIncluding(f => f.Formular, f => f.BVC_BugetPrev)
                                                .ToList()
                                                .Select(f => new BugetPreliminatListDto
                                                {
                                                    Id = f.Id,
                                                    AnBVC = f.Formular.AnBVC,
                                                    PreliminatCalculType = (int)f.PreliminatCalculType,
                                                    PreliminatCalculTypeStr = LazyMethods.EnumValueToDescription(f.PreliminatCalculType),
                                                    DataUltBalanta = f.DataUltBalanta,
                                                    FormularId = f.FormularId,
                                                    BVC_BugetPrevStr = f.BVC_BugetPrev == null ? "" : f.BVC_BugetPrev.Descriere
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

        public List<BugetFormularListDto> BugetPreliminatNewList()
        {
            try
            {
                var bugetPrelimIdList = _bugetPrelimRepository.GetAll().Select(f => f.FormularId);
                var _bugetFormular = _BugetFormularRepository.GetAll().Where(f => f.State == Models.Conta.Enums.State.Active && !bugetPrelimIdList.Contains(f.Id)).OrderByDescending(f => f.AnBVC);

                var ret = ObjectMapper.Map<List<BugetFormularListDto>>(_bugetFormular).ToList();
                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public List<BugetRepartizatDto> BugetRepartizatAddManual(int formularId)
        {
            try
            {
                var ret = _ActivityRepository.GetAll().Where(f => f.Status == Models.Conta.Enums.State.Active)
                                                              .Select(f => new BugetRepartizatDto
                                                              {
                                                                  FormularId = formularId,
                                                                  ActivityTypeId = f.Id,
                                                                  ActivityName = f.ActivityName,
                                                                  ProcRepartiz = 0
                                                              })
                    .OrderBy(f => f.ActivityName)
                    .ToList();
                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public List<BugetRepartizatDto> BugetRepartizatManualDetails(int preliminatId, int formularId)
        {
            try
            {


                var ret = _BugetRepartizatRepository.GetAllIncluding(f => f.ActivityType)
                                                    .Where(f => f.BVC_VenitBugetPrelimId == preliminatId)
                                                    .Select(f => new BugetRepartizatDto
                                                    {
                                                        Id = f.Id,
                                                        FormularId = formularId,
                                                        ActivityTypeId = f.ActivityTypeId,
                                                        ActivityName = f.ActivityType.ActivityName,
                                                        ProcRepartiz = f.ProcRepartiz
                                                    })
                                                    .OrderBy(f => f.ActivityName)
                                                    .ToList();
                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public void BugetRepartizatAddManualSave(int preliminatId, int formularId, List<BugetRepartizatDto> manualList)
        {
            try
            {
                if (preliminatId == 0)
                {
                    var bugetPrelim = new BVC_VenitBugetPrelim
                    {
                        FormularId = formularId,
                        PreliminatCalculType = PreliminatCalculType.Manual
                    };
                    _bugetPrelimRepository.Insert(bugetPrelim);
                    CurrentUnitOfWork.SaveChanges();
                    preliminatId = bugetPrelim.Id;
                }
                foreach (var item in manualList)
                {
                    if (item.Id == 0)
                    {
                        var repartizat = new BVC_VenitProcRepartiz
                        {
                            ActivityTypeId = item.ActivityTypeId,
                            BVC_VenitBugetPrelimId = preliminatId,
                            ProcRepartiz = item.ProcRepartiz
                        };
                        _BugetRepartizatRepository.Insert(repartizat);
                    }
                    else
                    {
                        var itemDb = _BugetRepartizatRepository.GetAll().FirstOrDefault(f => f.Id == item.Id);
                        itemDb.ProcRepartiz = item.ProcRepartiz;
                    }
                }
                CurrentUnitOfWork.SaveChanges();

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public void BugetRepartizatDelete(int preliminatId)
        {
            try
            {
                var repartizatList = _BugetRepartizatRepository.GetAll().Where(f => f.BVC_VenitBugetPrelimId == preliminatId).ToList();
                foreach (var item in repartizatList)
                {
                    _BugetRepartizatRepository.Delete(item.Id);
                }
                _bugetPrelimRepository.Delete(preliminatId);
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public int BugetRepartizatAddBalanta(int formularId)
        {
            try
            {
                int preliminatId = 0;
                var lastBalance = _balanceRepository.GetAll().Where(f => f.Status == Models.Conta.Enums.State.Active)
                                                    .OrderByDescending(f => f.BalanceDate)
                                                    .FirstOrDefault();
                var lastBalanceDay = lastBalance.BalanceDate;

                var balanceDetails = _balanceDetailsRepository.GetAllIncluding(f => f.Account).Where(f => f.BalanceId == lastBalance.Id);

                var bugetPrelim = new BVC_VenitBugetPrelim
                {
                    FormularId = formularId,
                    DataUltBalanta = lastBalanceDay,
                    PreliminatCalculType = PreliminatCalculType.UltimaBalanta
                };
                _bugetPrelimRepository.Insert(bugetPrelim);
                CurrentUnitOfWork.SaveChanges();
                preliminatId = bugetPrelim.Id;

                var venitList = balanceDetails.Where(f => f.Account.Symbol.IndexOf("7") == 0)
                                              .GroupBy(f => f.Account.ActivityTypeId)
                                              .Select(f => new BugetRepartizatDto
                                              {
                                                  ActivityTypeId = f.Key.Value,
                                                  VenitRepartiz = f.Sum(g => g.CrValueY)
                                              })
                                              .ToList();
                var totalVenituri = venitList.Sum(f => f.VenitRepartiz);
                if (totalVenituri != 0)
                {
                    foreach (var item in venitList)
                    {
                        item.ProcRepartiz = Math.Round(item.VenitRepartiz / totalVenituri * 100, 2);
                    }
                }

                foreach (var item in venitList)
                {
                    var repartizat = new BVC_VenitProcRepartiz
                    {
                        ActivityTypeId = item.ActivityTypeId,
                        BVC_VenitBugetPrelimId = preliminatId,
                        ProcRepartiz = item.ProcRepartiz,
                        VenitRepartiz = item.VenitRepartiz
                    };
                    _BugetRepartizatRepository.Insert(repartizat);
                }
                CurrentUnitOfWork.SaveChanges();

                return preliminatId;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public List<BugetRepartizatDto> BugetRepartizatBalantaDetails(int preliminatId, int formularId)
        {
            try
            {
                var ret = _BugetRepartizatRepository.GetAllIncluding(f => f.ActivityType)
                                                    .Where(f => f.BVC_VenitBugetPrelimId == preliminatId)
                                                    .Select(f => new BugetRepartizatDto
                                                    {
                                                        Id = f.Id,
                                                        FormularId = formularId,
                                                        ActivityTypeId = f.ActivityTypeId,
                                                        ActivityName = f.ActivityType.ActivityName,
                                                        ProcRepartiz = f.ProcRepartiz,
                                                        VenitRepartiz = f.VenitRepartiz,
                                                        VenitRepartizBVC = f.VenitRepartizBVC
                                                    })
                                                    .OrderBy(f => f.ActivityName)
                                                    .ToList();
                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public void BugetRepartizatAddBalantaSave(List<BugetRepartizatDto> venituriList)
        {
            try
            {
                foreach (var item in venituriList)
                {
                    var itemDb = _BugetRepartizatRepository.GetAll().FirstOrDefault(f => f.Id == item.Id);
                    itemDb.ProcRepartiz = item.ProcRepartiz;
                }
                CurrentUnitOfWork.SaveChanges();

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public int BugetRepartizatAddPrelimVenit(int formularId, int bugetPrevId)
        {
            try
            {
                if (bugetPrevId == 0)
                {
                    throw new Exception("Nu ati selectat bugetul preliminat aferent anului anterior");
                }
                var preliminatId = BugetRepartizatAddBalanta(formularId);
                var bugetVenitRepartiz = _bugetPrelimRepository.GetAllIncluding(f => f.BVC_BugetPrev, f => f.VenitProcRepartiz).FirstOrDefault(f => f.FormularId == formularId);

                bugetVenitRepartiz.PreliminatCalculType = PreliminatCalculType.PreliminareVenituri;
                bugetVenitRepartiz.BVC_BugetPrevId = bugetPrevId;
                CurrentUnitOfWork.SaveChanges();

                var venitRowList = _bugetPrevRandRepository.GetAllIncluding(f => f.ValueList, f => f.FormRand)
                                                               .Where(f => f.BugetPrevId == bugetPrevId && f.FormRand.TipRand == BVC_RowType.Venituri)
                                                               .Select(f => f.Id)
                                                               .ToList();

                var valueList = _bugetPrevRandValueRepository.GetAll()
                                                             .Where(f => venitRowList.Contains(f.BugetPrevRandId))
                                                             .GroupBy(f => f.ActivityTypeId)
                                                             .Select(f => new { ActivityTypeId = f.Key, Value = f.Sum(g => g.Value) })
                                                             .ToList();
                foreach (var item in valueList.Where(f => f.Value != 0))
                {
                    var activityValue = bugetVenitRepartiz.VenitProcRepartiz.FirstOrDefault(f => f.ActivityTypeId == item.ActivityTypeId);
                    activityValue.VenitRepartizBVC = item.Value;
                }

                var totalVenituri = bugetVenitRepartiz.VenitProcRepartiz.Sum(f => f.VenitRepartiz + f.VenitRepartizBVC);

                if (totalVenituri != 0)
                {
                    foreach (var item in bugetVenitRepartiz.VenitProcRepartiz)
                    {
                        item.ProcRepartiz = Math.Round((item.VenitRepartiz + item.VenitRepartizBVC) * 100 / totalVenituri, 2);
                    }
                }

                CurrentUnitOfWork.SaveChanges();
                return bugetVenitRepartiz.Id;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

    }
}

