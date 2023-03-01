using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.SectoareBnr;
using Niva.Erp.SectoareBnr.Dto;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.SectoareBnr
{

    public interface IBNR_AnexaDetailAppService : IApplicationService
    {
        List<BNR_AnexaDetailDto> AnexaBnrList();
        AnexaBnrEditDto AnexaBnrId(int id);
        void UpdateAnexaBnr(List<BNR_AnexaDetailDto> storage);
    }

    public class BNR_AnexaDetailAppService : ErpAppServiceBase, IBNR_AnexaDetailAppService
    {
        IRepository<BNR_AnexaDetail> _bnrAnexaDetailRepository;

        public BNR_AnexaDetailAppService(IRepository<BNR_AnexaDetail> bnrAnexaDetailRepository)
        {
            _bnrAnexaDetailRepository = bnrAnexaDetailRepository;
        }

        //[AbpAuthorize("Conta.BNR.Configurare.Acces")]
        public List<BNR_AnexaDetailDto> AnexaBnrList()
        {
            var bnrAnexaDetails = _bnrAnexaDetailRepository.GetAllIncluding(f => f.BNR_Anexa).Where(f => f.State == State.Active);
            var ret = ObjectMapper.Map<List<BNR_AnexaDetailDto>>(bnrAnexaDetails).ToList();
            return ret;
        }

        public AnexaBnrEditDto AnexaBnrId(int id)
        {
            var ret = _bnrAnexaDetailRepository.Get(id);
            return ObjectMapper.Map<AnexaBnrEditDto>(ret);
        }

        public void UpdateAnexaBnr(List<BNR_AnexaDetailDto> storage)
        {
            foreach (var value in storage)
            {
                var bnrAnexaDetail = _bnrAnexaDetailRepository.FirstOrDefault(f => f.Id == value.Id);
                bnrAnexaDetail.FormulaConta = value.FormulaConta;
                bnrAnexaDetail.FormulaCresteri = value.FormulaCresteri;
                bnrAnexaDetail.FormulaReduceri = value.FormulaReduceri;
                bnrAnexaDetail.FormulaTotal = value.FormulaTotal;
                bnrAnexaDetail.Sectorizare = value.Sectorizare;
                bnrAnexaDetail.TenantId = GetCurrentTenant().Id;

                //var bnrAnexaDetail = ObjectMapper.Map<BNR_AnexaDetail>(value);
                //var appClient = GetCurrentTenant();
                //bnrAnexaDetail.TenantId = appClient.Id;

                _bnrAnexaDetailRepository.Update(bnrAnexaDetail);
            }
        }
    }
}
