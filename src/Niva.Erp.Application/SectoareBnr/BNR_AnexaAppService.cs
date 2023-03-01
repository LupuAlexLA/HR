using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Niva.Erp.Models.SectoareBnr;
using Niva.Erp.SectoareBnr.Dto;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.SectoareBnr
{

    public interface IBNR_AnexaAppService : IApplicationService
    {
        List<BNR_AnexaDto> AnexaList();
    }

    public class BNR_AnexaAppService : ErpAppServiceBase, IBNR_AnexaAppService
    {
        IRepository<BNR_Anexa> _bnrAnexaRepository;
        public BNR_AnexaAppService(IRepository<BNR_Anexa> bnrAnexaRepository)
        {
            _bnrAnexaRepository = bnrAnexaRepository;
        }
        
        public List<BNR_AnexaDto> AnexaList()
        {
            var _bnrAnexaList = _bnrAnexaRepository.GetAll();
            var ret = ObjectMapper.Map<List<BNR_AnexaDto>>(_bnrAnexaList).ToList();
            return ret;
        }
    }
}
