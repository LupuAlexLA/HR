using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.ExternalApi;
using Niva.Erp.Models.SectoareBnr;
using Niva.Erp.Repositories.Conta;
using Niva.Erp.Repositories.SectoareBNR;
using Niva.Erp.SectoareBnr.Dto;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.SectoareBnr
{
    public interface IBNR_RaportareAppService : IApplicationService
    {
        List<BNR_RaportareDto> RaportareList();
        void ComputeRaportare(int savedBalanceId);
        void DeleteRaportare(int savedBalanceId);
        List<BNR_RaportareRowDto> RaportareRows(int savedBalanceId, int anexaId);
        List<BNR_RaportareRowDetailsDto> RaportareRowDetails(int raportareRandId);
        List<BNR_Detalii> RaportareDetailsList(int saveBalanceId, int anexaId);
    }
    public class BNR_RaportareAppService : ErpAppServiceBase, IBNR_RaportareAppService
    {
        IBNR_RaportareRepository _bnrRaportareRepository;
        IRepository<BNR_RaportareRand> _bnrRaportareRandRepository;
        IRepository<BNR_RaportareRandDetail> _bnrRaportareRandDetailRepository;
        IRepository<BNR_Conturi> _bnrConturiRepository;
        IRepository<BNR_AnexaDetail> _bnrAnexaDetailRepository;
        IPlasamentBNRManager _plasamentBNRManager;
        ISavedBalanceRepository _savedBalanceRepository;
        IRepository<BNR_Sector> _bnrSectorRepository;
        public BNR_RaportareAppService(IBNR_RaportareRepository bnrRaportareRepository, IRepository<BNR_RaportareRand> bnrRaportareRandRepository,
                                       IRepository<BNR_RaportareRandDetail> bnrRaportareRandDetailRepository, IRepository<BNR_Conturi> bnrConturiRepository,
                                       IRepository<BNR_AnexaDetail> bnrAnexaDetailRepository, IPlasamentBNRManager plasamentBNRManager,
                                       ISavedBalanceRepository savedBalanceRepository, IRepository<BNR_Sector> bnrSectorRepository)
        {
            _bnrRaportareRepository = bnrRaportareRepository;
            _bnrRaportareRandRepository = bnrRaportareRandRepository;
            _bnrRaportareRandDetailRepository = bnrRaportareRandDetailRepository;
            _bnrConturiRepository = bnrConturiRepository;
            _bnrAnexaDetailRepository = bnrAnexaDetailRepository;
            _plasamentBNRManager = plasamentBNRManager;
            _savedBalanceRepository = savedBalanceRepository;
            _bnrSectorRepository = bnrSectorRepository;
        }
        //[AbpAuthorize("Conta.BNR.Raportare.Acces")]
        public void ComputeRaportare(int savedBalanceId)
        {
            try
            {
                // sterg raportarea pentru balanta selectata
                _bnrRaportareRepository.DeleteRaportare(savedBalanceId);

                var randuriDetailList = new List<BNR_RaportareRandDetail>();
                var appClient = GetCurrentTenant();

                var savedBalance = _savedBalanceRepository.GetAll().FirstOrDefault(f => f.Id == savedBalanceId);
                var saveBalanceDate = savedBalance.SaveDate;

                // lista titlurilor din api
                var plasamenteBNRList = _plasamentBNRManager.PlasamenteBNRList(saveBalanceDate).ToList();

                // lista conturilor din bnrConturi
                var bnrConturiList = _bnrConturiRepository.GetAllIncluding(f => f.SavedBalance, f => f.BNR_AnexaDetail, f => f.Account).Where(f => f.SavedBalanceId == savedBalanceId).ToList();

                var anexaIdsList = _bnrAnexaDetailRepository.GetAllIncluding(f => f.BNR_Anexa)
                                                               .Where(f => f.TenantId == appClient.Id)
                                                               .Select(f => f.AnexaId)
                                                               .Distinct()
                                                               .ToList();
                foreach (var anexaId in anexaIdsList) // pentru fiecare anexa
                {
                    var raportare = new BNR_Raportare
                    {
                        AnexaId = anexaId.Value,
                        SavedBalanceId = savedBalanceId,
                        TenantId = appClient.Id
                    };
                    _bnrRaportareRepository.Insert(raportare);
                    CurrentUnitOfWork.SaveChanges();

                    var anexaDetailList = _bnrAnexaDetailRepository.GetAllIncluding(f => f.BNR_Anexa).Where(f => f.AnexaId == anexaId && f.State == Models.Conta.Enums.State.Active
                                                                                                            && f.FormulaTotal == null)
                                                                                                     .ToList();
                    var raportareRandList = new List<BNR_RaportareRand>();

                    foreach (var item in anexaDetailList)
                    {
                        if (item.EDinConta)
                        {
                            foreach (var cont in bnrConturiList.Where(f => f.AnexaDetailId == item.Id))
                            {
                                var raportareRand = raportareRandList.FirstOrDefault(f => f.BNR_RaportareId == raportare.Id && f.AnexaDetailId == cont.AnexaDetailId && f.SectorId == cont.BNR_SectorId);

                                if (raportareRand == null)
                                {
                                    raportareRand = new BNR_RaportareRand
                                    {
                                        BNR_RaportareId = raportare.Id,
                                        AnexaDetailId = cont.AnexaDetailId,
                                        SectorId = cont.BNR_SectorId,
                                        Valoare = cont.Value,
                                        BNR_RaportareRandDetails = new List<BNR_RaportareRandDetail>()
                                    };
                                    raportareRandList.Add(raportareRand);
                                }
                                else
                                {
                                    raportareRand.Valoare += cont.Value;
                                }

                                var randDetail = new BNR_RaportareRandDetail
                                {
                                    Valoare = cont.Value,
                                    BNR_RaportareRandId = raportareRand.Id,
                                    Descriere = cont.Account.Symbol
                                };
                                raportareRand.BNR_RaportareRandDetails.Add(randDetail);
                            }
                        }
                        else
                        {
                            var randuriDetPlasament = new List<BNR_RaportareRandDetail>();

                            string[] tipPlasament = item.TipTitlu.Split("#");

                            var plasamenteAnexaDetail = plasamenteBNRList.Where(f => item.TipTitlu.Contains("#" + f.tipPlasament + "#") && item.DurataMinima < f.durataAnEmis && f.durataAnEmis <= item.DurataMaxima)
                                                                         .ToList();
                            var sectoareList = plasamenteAnexaDetail.Select(f => f.codStatistica).Distinct().ToList();

                            var plasamenteFaraCodStatistica = plasamenteAnexaDetail.Where(f => f.codStatistica == null).Select(f => f.idplasament).Distinct().ToList();
                            string messagePlasamenteIds = string.Join(", " , plasamenteAnexaDetail.Where(f => f.codStatistica == null).Select(f => f.idplasament));

                            if (plasamenteFaraCodStatistica.Count > 0)
                            {
                                throw new Exception($"Lista plasamentelor identificate fara cod statistica este {messagePlasamenteIds}");
                            }

                            foreach (var sector in sectoareList)
                            {
                                var sectorBNR = _bnrSectorRepository.FirstOrDefault(f => f.Sector == sector);
                                var raportareRand = raportareRandList.FirstOrDefault(f => f.BNR_RaportareId == raportare.Id && f.AnexaDetailId == item.Id && f.SectorId == sectorBNR.Id);
                                if (raportareRand == null)
                                {
                                    raportareRand = new BNR_RaportareRand
                                    {
                                        BNR_RaportareId = raportare.Id,
                                        AnexaDetailId = item.Id,
                                        SectorId = sectorBNR.Id,
                                        Valoare = 0,
                                        BNR_RaportareRandDetails = new List<BNR_RaportareRandDetail>()
                                    };
                                    raportareRandList.Add(raportareRand);
                                }
                                var valoare = plasamenteAnexaDetail.Where(f => f.codStatistica == sector).Sum(f => f.valoarePlasament);
                                raportareRand.Valoare += valoare;

                                var detailList = plasamenteAnexaDetail.Where(f => f.codStatistica == sector).Select(f => new BNR_RaportareRandDetail
                                {
                                    Descriere = f.idplasament,
                                    Valoare = f.valoarePlasament
                                }).ToList();
                                raportareRand.BNR_RaportareRandDetails.AddRange(detailList);
                            }
                        }
                    }

                    foreach (var rand in raportareRandList)
                    {
                        var randDb = new BNR_RaportareRand
                        {
                            AnexaDetailId = rand.AnexaDetailId,
                            BNR_RaportareId = rand.BNR_RaportareId,
                            SectorId = rand.SectorId,
                            Valoare = Math.Round(rand.Valoare, 0)
                        };
                        _bnrRaportareRandRepository.Insert(randDb);
                        CurrentUnitOfWork.SaveChanges();

                        foreach (var detail in rand.BNR_RaportareRandDetails)
                        {
                            var detailDb = new BNR_RaportareRandDetail
                            {
                                Descriere = detail.Descriere,
                                Valoare = detail.Valoare,
                                BNR_RaportareRandId = randDb.Id
                            };
                            _bnrRaportareRandDetailRepository.Insert(detailDb);
                        }
                        CurrentUnitOfWork.SaveChanges();
                    }
                }

                // calcul totaluri
                //var anexaDetailsTotal = _bnrAnexaDetailRepository.GetAllIncluding(f => f.BNR_Anexa).Where(f => anexaIdsList.Contains(f.AnexaId) && f.FormulaTotal != null && f.FormulaTotal != "").ToList();
                //_bnrRaportareRepository.CalculTotaluri(anexaDetailsTotal);
                _bnrRaportareRepository.CalculTotaluriRaportare(savedBalanceId);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public void DeleteRaportare(int savedBalanceId)
        {
            try
            {
                _bnrRaportareRepository.DeleteRaportare(savedBalanceId);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        //[AbpAuthorize("Conta.BNR.Raportare.Acces")]
        public List<BNR_RaportareDto> RaportareList()
        {
            try
            {
                var raportareList = _bnrRaportareRepository.GetAllIncluding(f => f.SavedBalance, f => f.BNR_Anexa)
                                                          .GroupBy(f => new { f.SavedBalanceId, f.SavedBalance.SaveDate, f.SavedBalance.BalanceName })
                                                           .Select(f => new BNR_RaportareDto
                                                           {

                                                               SavedBalanceId = f.Key.SavedBalanceId,
                                                               SavedDate = f.Key.SaveDate,
                                                               BalanceName = f.Key.BalanceName
                                                           })
                                                           .OrderByDescending(f => f.SavedDate)
                                                           .ToList();

                return raportareList;
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        //[AbpAuthorize("Conta.BNR.Raportare.Acces")]
        public List<BNR_RaportareRowDetailsDto> RaportareRowDetails(int raportareRandId)
        {
            try
            {
                var raportareRowDetailsList = _bnrRaportareRandDetailRepository.GetAllIncluding(f => f.BNR_RaportareRand)
                                                                           .Where(f => f.BNR_RaportareRandId == raportareRandId && f.Valoare != 0)
                                                                           .Select(f => new BNR_RaportareRowDetailsDto
                                                                           {
                                                                               Id = f.Id,
                                                                               Valoare = f.Valoare,
                                                                               BNR_RaportareRandId = f.BNR_RaportareRandId,
                                                                               Descriere = f.Descriere
                                                                           })
                                                                           .ToList();

                return raportareRowDetailsList;
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        //[AbpAuthorize("Conta.BNR.Raportare.Acces")]
        public List<BNR_Detalii> RaportareDetailsList(int saveBalanceId, int anexaId)
        {
            try
            {
                var raportareRanduriIdsList = _bnrRaportareRandRepository.GetAllIncluding(f => f.BNR_Sector, f => f.BNR_Raportare, f => f.BNR_AnexaDetail, f => f.BNR_AnexaDetail.BNR_Anexa)
                                                      .Where(f => f.BNR_Raportare.SavedBalanceId == saveBalanceId && f.Valoare != 0 && f.BNR_AnexaDetail.AnexaId == anexaId /*&& f.SectorId != null*/)
                                                      .GroupBy(f => new { f.AnexaDetailId, f.SectorId })
                                                      .Select(f => f.Max(x => x.BNR_RaportareId))
                                                      .Distinct()
                                                      .ToList();

                var raportareRowDetailsList = _bnrRaportareRandDetailRepository.GetAllIncluding(f => f.BNR_RaportareRand, f => f.BNR_RaportareRand.BNR_AnexaDetail)
                                                                           .Where(f => f.Valoare != 0 && f.BNR_RaportareRand.BNR_Raportare.SavedBalanceId == saveBalanceId && f.BNR_RaportareRand.BNR_AnexaDetail.AnexaId == anexaId &&
                                                                                  raportareRanduriIdsList.Contains(f.BNR_RaportareRand.BNR_RaportareId) /*&& f.BNR_RaportareRand.SectorId != null*/)
                                                                           .OrderBy(f => f.BNR_RaportareRand.BNR_AnexaDetail.OrderView)
                                                                           //.ThenBy(f => f.BNR_RaportareRand.BNR_AnexaDetail.DenumireRand)
                                                                           //.ThenBy(f => f.BNR_RaportareRand.BNR_Sector.Denumire)
                                                                           .Select(f => new BNR_Detalii
                                                                           {
                                                                               AnexaDetailName = f.BNR_RaportareRand.BNR_AnexaDetail.DenumireRand,
                                                                               SectorName = f.BNR_RaportareRand.BNR_Sector.Sector,
                                                                               Valoare = f.Valoare,
                                                                               Descriere = f.Descriere,
                                                                               OrderView = f.BNR_RaportareRand.BNR_AnexaDetail.OrderView
                                                                           })
                                                                           .OrderBy(f => f.OrderView)
                                                                           .Distinct()
                                                                           .ToList();

                return raportareRowDetailsList;
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        //[AbpAuthorize("Conta.BNR.Raportare.Acces")]
        public List<BNR_RaportareRowDto> RaportareRows(int saveBalanceId, int anexaId)
        {
            try
            {
                var appClient = GetCurrentTenant();

                var raportareRanduriIdsList = _bnrRaportareRandRepository.GetAllIncluding(f => f.BNR_Sector, f => f.BNR_Raportare, f => f.BNR_AnexaDetail, f => f.BNR_AnexaDetail.BNR_Anexa)
                                                      .Where(f => f.BNR_Raportare.SavedBalanceId == saveBalanceId && f.Valoare != 0 && f.BNR_AnexaDetail.AnexaId == anexaId)
                                                      .GroupBy(f => new { f.AnexaDetailId, f.SectorId })
                                                      .Select(f => f.Max(x => x.BNR_RaportareId))
                                                      .Distinct()
                                                      .ToList();

                var raportareRanduriList = _bnrRaportareRandRepository.GetAllIncluding(f => f.BNR_Sector, f => f.BNR_Raportare, f => f.BNR_AnexaDetail,
                                                                                            f => f.BNR_AnexaDetail.BNR_Anexa)
                                                                      .Where(f => raportareRanduriIdsList.Contains(f.BNR_RaportareId) && f.Valoare != 0
                                                                       && f.BNR_AnexaDetail.AnexaId == anexaId)
                                                                  .OrderBy(f => f.BNR_AnexaDetail.OrderView)
                                                                  .Select(f => new BNR_RaportareRowDto
                                                                  {
                                                                      Id = f.Id,
                                                                      OrderView = f.BNR_AnexaDetail.OrderView,
                                                                      AnexaDetailId = f.AnexaDetailId,
                                                                      BNR_RaportareId = f.BNR_RaportareId,
                                                                      AnexaDetailName = f.BNR_AnexaDetail.DenumireRand,
                                                                      SectorId = f.SectorId,
                                                                      SectorName = f.BNR_Sector.Denumire,
                                                                      Valoare = f.Valoare
                                                                  })
                                                                  .OrderBy(f => f.OrderView)
                                                                  .Distinct()
                                                                  .ToList();

                return raportareRanduriList;
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }
    }
}
