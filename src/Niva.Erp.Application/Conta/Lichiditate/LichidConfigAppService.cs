using Abp.Application.Services;
using Abp.Domain.Repositories;
using Niva.Erp.Conta.Lichiditate.Dto;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Conta.Lichiditate;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Conta.Lichiditate
{
    public interface ILichidConfigAppService : IApplicationService
    {
        List<LichidConfigDto> LichidConfigList();
        void SaveLichidConfig(List<LichidConfigDto> lichidConfigList);
        List<LichidBenziDto> LichidBenziList();
    }

    public class LichidConfigAppService : ErpAppServiceBase, ILichidConfigAppService
    {
        IRepository<LichidConfig> _lichidConfigRepository;
        IRepository<LichidBenzi> _lichidBenziRepository;

        public LichidConfigAppService(IRepository<LichidConfig> lichidConfigRepository, IRepository<LichidBenzi> lichidBenziRepository)
        {
            _lichidConfigRepository = lichidConfigRepository;
            _lichidBenziRepository = lichidBenziRepository;
        }

        public List<LichidBenziDto> LichidBenziList()
        {
            try
            {
                var list = _lichidBenziRepository.GetAll().Where(f => f.State == State.Active).ToList();

                var ret = ObjectMapper.Map<List<LichidBenziDto>>(list);
                return ret;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public List<LichidConfigDto> LichidConfigList()
        {
            try
            {
                var list = _lichidConfigRepository.GetAll().Where(f => f.State == State.Active).ToList();

                var ret = ObjectMapper.Map<List<LichidConfigDto>>(list);
                return ret;
            }
            catch (System.Exception ex)
            {

                throw ex;
            }
        }

        public void SaveLichidConfig(List<LichidConfigDto> lichidConfigList)
        {
            try
            {
                foreach (var item in lichidConfigList)
                {
                    var lichidConfig = _lichidConfigRepository.FirstOrDefault(f => f.Id == item.Id && f.State == State.Active);
                    lichidConfig.FormulaConta = item.FormulaConta;
                    lichidConfig.LichidBenziId = item.LichidBenziId ?? null;
                    _lichidConfigRepository.Update(lichidConfig);
                }
                CurrentUnitOfWork.SaveChanges();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}
