using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.SectoareBnr;
using Niva.Erp.SectoareBnr.Dto;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.SectoareBnr
{
    public interface IBNR_SectorAppService : IApplicationService
    {
        List<BNR_SectorListDto> GetBNRSectorList();
        void Save(BNR_SectorEditDto sectorEdit);
    }
    public class BNR_SectorAppService : ErpAppServiceBase, IBNR_SectorAppService
    {
        IRepository<BNR_Sector> _bnrSectorRepository;

        public BNR_SectorAppService(IRepository<BNR_Sector> bnrSectorRepository)
        {
            _bnrSectorRepository = bnrSectorRepository;
        }

        //[AbpAuthorize("Admin.Conta.SectoareBNR.Acces")]
        public List<BNR_SectorListDto> GetBNRSectorList()
        {
            try
            {
                var list = _bnrSectorRepository.GetAll().Where(f => f.State == State.Active);
                var ret = ObjectMapper.Map<List<BNR_SectorListDto>>(list);
                return ret;
            }
            catch (System.Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        //[AbpAuthorize("Admin.Conta.SectoareBNR.Acces")]
        public void Save(BNR_SectorEditDto sectorEdit)
        {
            try
            {
                var sectorDb = ObjectMapper.Map<BNR_Sector>(sectorEdit);
                var appClient = GetCurrentTenant();

                if (sectorDb.Id == 0) //INSERT
                {
                    sectorDb.TenantId = appClient.Id;
                    _bnrSectorRepository.Insert(sectorDb);
                }
                else
                {
                    _bnrSectorRepository.Update(sectorDb);
                }

                CurrentUnitOfWork.SaveChanges();
            }
            catch (System.Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }
    }
}
