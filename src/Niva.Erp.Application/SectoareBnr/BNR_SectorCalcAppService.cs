using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.Models.SectoareBnr;
using Niva.Erp.Repositories.Conta;
using Niva.Erp.Repositories.Conta.Nomenclatures;
using Niva.Erp.Repositories.SectoareBNR;
using Niva.Erp.SectoareBnr.Dto;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.SectoareBnr
{
    public interface IBNR_SectorCalcAppService : IApplicationService
    {
        void BNR_SectorConturiCalc(int balanceId);
        List<BNR_SectorCalculatListDto> GetSectorBalanceList();
        List<BNR_SectorRowCalcListDto> SectorBnrDetailList(int balanceId);
        List<BNR_SectorRowCalValDto> GetSectorRowDetail(int anexaDetailId, int savedBalanceId, int anexaId);
        List<BNR_SectorDetailDto> GetBNRConturiList(int anexaDetailId, int? sectorId, int savedBalanceId);
        void SaveBNRConturi(List<BNR_SectorDetailDto> bnrItem, int savedBalanceId);
        void DeleteCalculBNR(int balanceId);
    }
    public class BNR_SectorCalcAppService : ErpAppServiceBase, IBNR_SectorCalcAppService
    {
        ISavedBalanceRepository _savedBalanceRepository;
        IBNR_SectorCalcRepository _bnrSectorCalcRepository;
        IRepository<BNR_AnexaDetail> _bnrAnexaDetailRepository;
        IRepository<BNR_Sector> _bnrSectorRepository;
        IAccountRepository _accountRepository;
        IBNR_RaportareRepository _bnrRaportareRepository;

        public BNR_SectorCalcAppService(ISavedBalanceRepository savedBalanceRepository, IBNR_SectorCalcRepository bnrSectorCalcRepository,
                                        IRepository<BNR_AnexaDetail> bnrAnexaDetailRepository, IRepository<BNR_Sector> bnrSectorRepository,
                                        IAccountRepository accountRepository, IBNR_RaportareRepository bnrRaportareRepository)
        {
            _savedBalanceRepository = savedBalanceRepository;
            _bnrSectorCalcRepository = bnrSectorCalcRepository;
            _bnrAnexaDetailRepository = bnrAnexaDetailRepository;
            _bnrSectorRepository = bnrSectorRepository;
            _accountRepository = accountRepository;
            _bnrRaportareRepository = bnrRaportareRepository;
        }

        //[AbpAuthorize("Conta.BNR.Conturi.Acces")]
        public void BNR_SectorConturiCalc(int balanceId)
        {
            try
            {
                _bnrSectorCalcRepository.CalcConturi(balanceId);
            }
            catch (System.Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public List<BNR_SectorCalculatListDto> GetSectorBalanceList()
        {
            var calculatedBalanceIdList = _bnrSectorCalcRepository.GetAllIncluding(f => f.SavedBalance).Select(f => f.SavedBalanceId).Distinct().ToList();


            var calculatedList = new List<BNR_SectorCalculatListDto>();
            foreach (var balCalc in calculatedBalanceIdList)
            {
                var balance = _savedBalanceRepository.FirstOrDefault(f => f.Id == balCalc);
                var item = new BNR_SectorCalculatListDto
                {
                    BalanceDate = balance.SaveDate,
                    BalanceId = balCalc,
                    TenantId = balance.TenantId,
                    BalanceDesc = balance.BalanceName,
                    AllowCompute = false,
                    BNR_Rows = new List<BNR_SectorRowCalcListDto>()
                };

                var count = _bnrSectorCalcRepository.GetAllIncluding(f => f.BNR_AnexaDetail)
                                                    .Count(f => f.SavedBalanceId == balCalc && f.BNR_AnexaDetail.Sectorizare && f.BNR_Sector == null);
                item.AllowCompute = (count == 0);


                calculatedList.Add(item);
            }
            return calculatedList.OrderByDescending(f => f.BalanceDate).ToList();
        }

        public List<BNR_SectorRowCalcListDto> SectorBnrDetailList(int balanceId)
        {
            try
            {
                var anexaIdList = _bnrAnexaDetailRepository.GetAllIncluding(f => f.BNR_Anexa).Select(f => f.AnexaId).Distinct().ToList();
                var rows = new List<BNR_SectorRowCalcListDto>();

                var bnrCalculList = _bnrSectorCalcRepository.GetAllIncluding(f => f.SavedBalance, f => f.BNR_AnexaDetail).Where(f => f.SavedBalanceId == balanceId).Select(f => f.AnexaDetailId).Distinct().ToList();
                foreach (var anexaId in anexaIdList)
                {
                    var rowsByAnexaId = _bnrAnexaDetailRepository.GetAllIncluding(f => f.BNR_Anexa).Where(f => bnrCalculList.Contains(f.Id) && f.AnexaId == anexaId).Select(f => new BNR_SectorRowCalcListDto
                    {
                        AnexaDetailId = f.Id,
                        AnexaId = anexaId.Value,
                        Denumire = f.DenumireRand,
                        TenantId = f.TenantId,
                        BalanceId = balanceId,
                        NrCrt = f.NrCrt,
                        Sectorizare = f.Sectorizare,
                        BNR_RowDetails = new List<BNR_SectorRowCalValDto>()
                    }).Distinct().ToList();

                    foreach (var row in rowsByAnexaId)
                    {
                        var rowDetailList = GetSectorRowDetail(row.AnexaDetailId, balanceId, row.AnexaId);
                        row.BNR_RowDetails.AddRange(rowDetailList);
                    }
                    rows.AddRange(rowsByAnexaId);
                }
                return rows;
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public List<BNR_SectorDetailDto> GetBNRConturiList(int anexaDetailId, int? sectorId, int savedBalanceId)
        {
            try
            {
                var bnrConturiList = _bnrSectorCalcRepository.GetAllIncluding(f => f.Account, f => f.BNR_Sector, f => f.BNR_AnexaDetail, f => f.SavedBalance)
                                                            .Where(f => f.AnexaDetailId == anexaDetailId && f.BNR_SectorId == sectorId && f.SavedBalanceId == savedBalanceId)
                                                            .Select(f => new BNR_SectorDetailDto
                                                            {
                                                                SavedBalanceId = f.SavedBalanceId,
                                                                Account = f.Account.Symbol,
                                                                AnexaDetailId = anexaDetailId,
                                                                Denumire = f.Account.AccountName,
                                                                SectorId = f.BNR_SectorId,
                                                                TenantId = f.TenantId,
                                                                SoldCr = f.SoldCr,
                                                                SoldDb = f.SoldDb,
                                                                Value = f.Value
                                                            }).ToList();

                return bnrConturiList;
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public List<BNR_SectorRowCalValDto> GetSectorRowDetail(int anexaDetailId, int savedBalanceId, int anexaId)
        {
            try
            {
                var list = new List<BNR_SectorRowCalValDto>();

                var sectorList = _bnrSectorCalcRepository.GetAllIncluding(f => f.BNR_Sector, f => f.BNR_AnexaDetail, f => f.BNR_AnexaDetail.BNR_Anexa)
                                                         .Where(f => f.AnexaDetailId == anexaDetailId && f.BNR_SectorId != null && f.SavedBalanceId == savedBalanceId && f.BNR_AnexaDetail.AnexaId == anexaId)
                                                        .GroupBy(f => new { Denumire = f.BNR_Sector.Denumire, AnexaDetailId = f.AnexaDetailId, SectorId = f.BNR_SectorId, SectorCod = f.BNR_Sector.Sector })
                                                        .Select(f => new BNR_SectorRowCalValDto
                                                        {
                                                            Denumire = f.Key.Denumire,
                                                            RowId = f.Key.AnexaDetailId,
                                                            SectorId = f.Key.SectorId,
                                                            SectorCod = f.Key.SectorCod,
                                                            Value = f.Sum(f => f.Value)
                                                        }).Distinct().ToList();

                list.AddRange(sectorList);

                var unsectoredList = _bnrSectorCalcRepository.GetAllIncluding(f => f.BNR_Sector, f => f.BNR_AnexaDetail, f => f.BNR_AnexaDetail.BNR_Anexa, f => f.SavedBalance)
                                             .Where(f => f.AnexaDetailId == anexaDetailId && f.BNR_SectorId == null && f.SavedBalanceId == savedBalanceId && f.BNR_AnexaDetail.AnexaId == anexaId)
                                             .GroupBy(f => f.AnexaDetailId)
                                             .Select(f => new BNR_SectorRowCalValDto()
                                             {
                                                 Denumire = null,
                                                 RowId = anexaDetailId,
                                                 SectorCod = null,
                                                 SectorId = null,
                                                 Value = f.Sum(f => f.Value)
                                             }).Distinct().ToList();

                list.AddRange(unsectoredList);
                return list;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public void SaveBNRConturi(List<BNR_SectorDetailDto> bnrItem, int savedBalanceId)
        {
            try
            {
                foreach (var item in bnrItem.Where(f => f.SectorId != 0 && f.SavedBalanceId == savedBalanceId).ToList())
                {
                    var bnrCont = _bnrSectorCalcRepository.GetAllIncluding(f => f.Account, f => f.BNR_Sector).FirstOrDefault(f => f.AnexaDetailId == item.AnexaDetailId && f.SavedBalanceId == savedBalanceId && f.Account.Symbol == item.Account);
                    bnrCont.BNR_SectorId = item.SectorId;

                    var account = _accountRepository.GetAllIncluding(f => f.BNR_Sector).FirstOrDefault(f => f.Id == bnrCont.AccountId);
                    account.SectorBnrId = item.SectorId;
                }
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public void DeleteCalculBNR(int balanceId)
        {
            try
            {
                //sterg raportarile daca ua fost generate
                _bnrRaportareRepository.DeleteRaportare(balanceId);

                var bnrConturiList = _bnrSectorCalcRepository.GetAllIncluding(f => f.Account, f => f.BNR_Sector, f => f.SavedBalance).Where(f => f.SavedBalanceId == balanceId).ToList();

                foreach (var item in bnrConturiList)
                {
                    _bnrSectorCalcRepository.Delete(item);
                }
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }
    }
}
