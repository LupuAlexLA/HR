using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Niva.Erp.Imprumuturi.Dto;
using Niva.Erp.Models.Imprumuturi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Niva.Erp.Imprumuturi
{
    public interface IDateDobanziReferintaAppService : IApplicationService
    {
        List<DateDobanziReferintaDto> DateDobanziReferintaList();

        List<DateDobanziReferintaDto> DateDobanziReferintaListId(int dobanziReferintaId);

        DateDobanziReferintaEditDto GetDateDobanziReferintaId(int id);
        void SaveDateDobanziReferinta(DateDobanziReferintaEditDto dobandaRef);

        void DeleteDateDobanziReferinta(int id);
    }
    public class DateDobanziReferintaAppService  : ErpAppServiceBase, IDateDobanziReferintaAppService
    {
        IRepository<DateDobanziReferinta> _DateDobanziReferintaRepository;
        IRepository<DobanziReferinta> _DobanziReferintaRepository;

        public DateDobanziReferintaAppService(IRepository<DateDobanziReferinta> DateDobanziReferintaRepository, IRepository<DobanziReferinta> DobanziReferintaRepository)
        {
            _DateDobanziReferintaRepository = DateDobanziReferintaRepository;
            _DobanziReferintaRepository = DobanziReferintaRepository;
        }

        public List<DateDobanziReferintaDto> DateDobanziReferintaListId(int dobanziReferintaId)
        {
            var _date = _DateDobanziReferintaRepository.GetAll()
                                                    .Include(e => e.DobanziReferinta)
                                                    .Where(f => f.DobanziReferintaId == dobanziReferintaId && f.State == Models.Conta.Enums.State.Active).OrderBy(f => f.Data);
             
            


            var ret = ObjectMapper.Map<List<DateDobanziReferintaDto>>(_date).ToList();

            return ret;
        }

        public List<DateDobanziReferintaDto> DateDobanziReferintaList()
        {
            var _date  = _DateDobanziReferintaRepository.GetAll()
                                                    .Include(e => e.DobanziReferinta)
                                                    .Where(f => f.State == Models.Conta.Enums.State.Active);

            var ret = ObjectMapper.Map<List<DateDobanziReferintaDto>>(_date).ToList();
            
            return ret;
        }

        public DateDobanziReferintaEditDto GetDateDobanziReferintaId(int id)
        {
            var ret = _DateDobanziReferintaRepository.Get(id);
            return ObjectMapper.Map<DateDobanziReferintaEditDto>(ret);
        }

        public void SaveDateDobanziReferinta(DateDobanziReferintaEditDto dobandaRef)
        {
            var _dobandaRef = ObjectMapper.Map<DateDobanziReferinta>(dobandaRef);
            
            

            if (_dobandaRef.Id == 0)
            {

                int _chk = _DateDobanziReferintaRepository.GetAll().Where(f => f.State == Models.Conta.Enums.State.Active && f.DobanziReferintaId == dobandaRef.DobanziReferintaId && f.Data.Month == _dobandaRef.Data.Month && f.Data.Year == _dobandaRef.Data.Year).Count();
                

                if (_chk > 0) throw new UserFriendlyException("Eroare", "Termen existent!");

                
                _DateDobanziReferintaRepository.Insert(_dobandaRef);
                

                
            }
            else
            {
                var appClient = GetCurrentTenant();
                _dobandaRef.TenantId = appClient.Id;
                _DateDobanziReferintaRepository.Update(_dobandaRef);
            }
        }

            public void DeleteDateDobanziReferinta(int id)
            {
            var _dobandaRef = _DateDobanziReferintaRepository.FirstOrDefault(f => f.Id == id);
            _dobandaRef.State = Models.Conta.Enums.State.Inactive;
            _DateDobanziReferintaRepository.Update(_dobandaRef);
            }

        
    }
}
