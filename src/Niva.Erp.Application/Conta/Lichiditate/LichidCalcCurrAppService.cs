using Abp.Application.Services;
using Abp.Domain.Repositories;
using Niva.Erp.Conta.Lichiditate.Dto;
using Niva.Erp.Models.Conta.Lichiditate;
using Niva.Erp.Repositories.Conta.Lichiditate;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Conta.Lichiditate
{
    public interface ILichidCalcCurrAppService : IApplicationService
    {
        List<LichidCalcCurrListDto> GetLichidCalcCurrList(int savedBalanceId);
        List<LichidCalcCurrDetDto> GetLichidCalcCurrFormulaDet(int columnId, int savedBalanceId, int lichidCalcConfigId);
        List<LichidCalcCurrListDto> UpdateLichidCalcCurr(List<LichidCalcCurrListDto> lichidCalcCurr, int savedBalanceId);
    }

    public class LichidCalcCurrAppService : ErpAppServiceBase, ILichidCalcCurrAppService
    {
        IRepository<LichidCalcCurr> _lichidCalcCurrRepository;
        IRepository<LichidConfig> _lichidConfigRepository;
        IRepository<LichidBenziCurr> _lichidBenziCurrRepository;
        IRepository<LichidCalcCurrDet> _lichidCalcCurrDetRepository;
        ILichidCalcRepository _lichidCalcRepository;

        public LichidCalcCurrAppService(IRepository<LichidCalcCurr> lichidCalcCurrRepository, IRepository<LichidConfig> lichidConfigRepository,
                                        IRepository<LichidBenziCurr> lichidBenziCurrRepository, IRepository<LichidCalcCurrDet> lichidCalcCurrDetRepository,
                                        ILichidCalcRepository lichidCalcRepository)
        {
            _lichidCalcCurrRepository = lichidCalcCurrRepository;
            _lichidCalcCurrDetRepository = lichidCalcCurrDetRepository;
            _lichidConfigRepository = lichidConfigRepository;
            _lichidBenziCurrRepository = lichidBenziCurrRepository;
            _lichidCalcRepository = lichidCalcRepository;

        }

        public List<LichidCalcCurrDetDto> GetLichidCalcCurrFormulaDet(int columnId, int savedBalanceId, int lichidCalcConfigId)
        {
            try
            {
                var lichidDetList = _lichidCalcCurrDetRepository.GetAllIncluding(f => f.LichidCalcCurr, f => f.LichidCalcCurr.LichidConfig, f => f.LichidCalcCurr.SavedBalance)
                                                            .Where(f => f.LichidCalcCurr.LichidConfigId == lichidCalcConfigId && f.LichidCalcCurr.SavedBalanceId == savedBalanceId &&
                                                                        f.LichidCalcCurr.LichidBenziCurrId == columnId)
                                                            .Select(f => new LichidCalcCurrDetDto
                                                            {
                                                                LichidCalcCurrId = f.LichidCalcCurrId,
                                                                Descriere = f.Descriere,
                                                                Valoare = f.Valoare
                                                            })
                                                            .ToList();

                return lichidDetList;
            }
            catch (System.Exception ex)
            {

                throw ex;
            }
        }

        public List<LichidCalcCurrListDto> GetLichidCalcCurrList(int savedBalanceId)
        {
            try
            {
                var lichidCalcCurrList = new List<LichidCalcCurrListDto>();

                var lichidCalcIdsList = _lichidCalcCurrRepository.GetAllIncluding(f => f.LichidConfig)
                                                                 .Where(f => f.SavedBalanceId == savedBalanceId)
                                                                 .Select(f => f.LichidConfigId)
                                                                 .Distinct()
                                                                 .ToList();

                var lichidConfigList = _lichidConfigRepository.GetAll().Where(f => lichidCalcIdsList.Contains(f.Id)).ToList();
                var lichidBenziCurrRows = _lichidBenziCurrRepository.GetAll().ToList();

                foreach (var conf in lichidConfigList)
                {

                    var lichidCalcDet = new LichidCalcCurrListDto
                    {
                        Descriere = conf.DenumireRand,
                        LichidConfigId = conf.Id,
                        RandTotal = (conf.FormulaTotal != null && conf.FormulaTotal != "") ? true : false,
                        TenantId = conf.TenantId

                    };
                    foreach (var banda in lichidBenziCurrRows)
                    {
                        var lichidCalcRow = _lichidCalcCurrRepository.GetAllIncluding(f => f.LichidConfig, f => f.LichidBenziCurr, f => f.SavedBalance)
                                           .FirstOrDefault(f => f.SavedBalanceId == savedBalanceId && f.LichidConfigId == conf.Id && f.LichidBenziCurrId == banda.Id);

                        switch (banda.Descriere)
                        {
                            case "RON":
                                lichidCalcDet.ValoareRON = lichidCalcRow.Valoare;
                                break;
                            case "EUR":
                                lichidCalcDet.ValoareEUR = lichidCalcRow.Valoare;
                                break;
                            case "USD":
                                lichidCalcDet.ValoareUSD = lichidCalcRow.Valoare;
                                break;
                            case "GBP":
                                lichidCalcDet.ValoareGBP = lichidCalcRow.Valoare;
                                break;
                            default:
                                lichidCalcDet.ValoareAlteMonede = lichidCalcRow.Valoare;
                                break;
                        }
                        lichidCalcDet.TotalInit += lichidCalcRow.Valoare;
                        lichidCalcDet.TotalActualiz += lichidCalcRow.Valoare;
                    }
                    lichidCalcCurrList.Add(lichidCalcDet);
                }
                return lichidCalcCurrList;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public List<LichidCalcCurrListDto> UpdateLichidCalcCurr(List<LichidCalcCurrListDto> lichidCalcCurrList, int savedBalanceId)
        {
            try
            {
                foreach (var item in lichidCalcCurrList)
                {

                    var lichidBenziCurrList = _lichidBenziCurrRepository.GetAll().ToList();

                    foreach (var banda in lichidBenziCurrList)
                    {
                        var lichidCalcCurr = _lichidCalcCurrRepository.GetAllIncluding(f => f.SavedBalance)
                                                                      .FirstOrDefault(f => f.SavedBalanceId == savedBalanceId && f.LichidConfigId == item.LichidConfigId &&
                                                                                           f.LichidBenziCurrId == banda.Id);

                        switch (banda.Descriere)
                        {
                            case "RON":
                                lichidCalcCurr.Valoare = item.ValoareRON;
                                break;
                            case "EUR":
                                lichidCalcCurr.Valoare = item.ValoareEUR;
                                break;
                            case "USD":
                                lichidCalcCurr.Valoare = item.ValoareUSD;
                                break;
                            case "GBP":
                                lichidCalcCurr.Valoare = item.ValoareGBP;
                                break;
                            default:
                                lichidCalcCurr.Valoare = item.ValoareAlteMonede;
                                break;
                        }
                        _lichidCalcCurrRepository.Update(lichidCalcCurr);
                        CurrentUnitOfWork.SaveChanges();


                    }
                    item.TotalActualiz = _lichidCalcCurrRepository.GetAllIncluding(f => f.SavedBalance)
                                                              .Where(f => f.SavedBalanceId == savedBalanceId && f.LichidConfigId == item.LichidConfigId)
                                                              .Sum(f => f.Valoare);

                    item.TotalInit = _lichidCalcCurrRepository.GetAllIncluding(f => f.SavedBalance)
                                                              .Where(f => f.SavedBalanceId == savedBalanceId && f.LichidConfigId == item.LichidConfigId)
                                                              .Sum(f => f.Valoare);
                }

                //calculez randuri total
                _lichidCalcRepository.LichidCalcCurrTotaluri(savedBalanceId);

                lichidCalcCurrList = GetLichidCalcCurrList(savedBalanceId);

                return lichidCalcCurrList;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}
