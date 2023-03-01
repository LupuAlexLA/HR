using Abp.Application.Services;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Niva.Erp.Imprumuturi.Dto;
using Niva.Erp.Models.Imprumuturi;
using Niva.Erp.Repositories.Conta.AutoOperation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Niva.Erp.Imprumuturi
{
    public interface IDataComisionAppService : IApplicationService
    {
        List<DataComisionDto> DataComisionList();

        List<DataComisionDto> DataComisionListImprumutId(int imprumutId);
        List<DataComisionDto> DataComisionListId(int comisionId);
        List<DataComisionEditDto> DataComisionEditListId(int comisionId);

        DataComisionEditDto GetDataComisionId(int id);

        void SaveDataComision(DataComisionEditDto DataComision);

        void DeleteDataComision(int id);
        
    }
    public class DataComisionAppService : ErpAppServiceBase, IDataComisionAppService
    {
        IRepository<DataComision> _DataComisionRepository;
        IRepository<Comision> _ComisionRepository;
        IAutoOperationRepository _autoOperationRepository;


        public DataComisionAppService(IAutoOperationRepository autoOperationRepository, IRepository<DataComision> DataComisionRepository, IRepository<Comision> ComisionRepository)
        {
            _DataComisionRepository = DataComisionRepository;
            _ComisionRepository = ComisionRepository;
            _autoOperationRepository = autoOperationRepository;
        }
        public List<DataComisionDto> DataComisionList()
        {
            var _comision = _DataComisionRepository.GetAll().Where(f => f.State == Models.Conta.Enums.State.Active);



            var ret = ObjectMapper.Map<List<DataComisionDto>>(_comision).ToList();
            return ret;
        }

        public List<DataComisionDto> DataComisionListImprumutId(int imprumutId)
        {
            var _comision = _DataComisionRepository.GetAll()
                                                .Where(f => f.ImprumutId == imprumutId && f.State == Models.Conta.Enums.State.Active)
                                                .OrderBy(f => f.DataPlataComision); // sau .OrderBy(f => f.Index)




            var ret = ObjectMapper.Map<List<DataComisionDto>>(_comision).ToList();

            return ret;
        }

        public List<DataComisionDto> DataComisionListId(int comisionId)
        {
            var _comision = _DataComisionRepository.GetAll()
                                                .Where(f => f.ComisionId == comisionId && f.State == Models.Conta.Enums.State.Active)
                                                .OrderBy(f => f.DataPlataComision); // sau .OrderBy(f => f.Index)




            var ret = ObjectMapper.Map<List<DataComisionDto>>(_comision).ToList();

            return ret;
        }

        public List<DataComisionEditDto> DataComisionEditListId(int comisionId)
        {
            var _comision = _DataComisionRepository.GetAll()
                                                .Where(f => f.ComisionId == comisionId && f.State == Models.Conta.Enums.State.Active)
                                                .OrderBy(f => f.DataPlataComision); // sau .OrderBy(f => f.Index)



            
            var ret = ObjectMapper.Map<List<DataComisionEditDto>>(_comision).ToList();

            return ret;
        }

        public DataComisionEditDto GetDataComisionId(int id)
        {
            var ret = _DataComisionRepository.Get(id);
            return ObjectMapper.Map<DataComisionEditDto>(ret);
        }

        public void SaveDataComision(DataComisionEditDto dataComision)
        {
            var _dataComision = ObjectMapper.Map<DataComision>(dataComision);
            int localCurrencyId = GetCurrentTenant().LocalCurrencyId.Value;

            if (_dataComision.Id == 0)
            {

                //  int _chk = _GarantieRepository.GetAll().Where(f => f.State == Models.Conta.Enums.State.Active).Count();

                //  if (_chk > 0) throw new UserFriendlyException("Eroare", "Termen existent!");

                _DataComisionRepository.Insert(_dataComision);
            }
            else
            {
                var appClient = GetCurrentTenant();      
                _dataComision.TenantId = appClient.Id;
                
                _DataComisionRepository.Update(_dataComision);
                CurrentUnitOfWork.SaveChanges();

                if (_dataComision.IsValid)
                {


                   _autoOperationRepository.ComisionToConta(_dataComision.Id, localCurrencyId);
                    CurrentUnitOfWork.SaveChanges();
                    
                }

                
            }
        }
        public void DeleteDataComision(int id)
        {
            var _dataComision = _DataComisionRepository.FirstOrDefault(f => f.Id == id);
            _dataComision.State = Models.Conta.Enums.State.Inactive;
            _DataComisionRepository.Update(_dataComision);
        }
    }
}
