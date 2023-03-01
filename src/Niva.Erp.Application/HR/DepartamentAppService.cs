using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.HR.Dto;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.HR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.HR
{
    public interface IDepartamentAppService: IApplicationService
    {
        List<DepartamentListDto> GetDepartamentList();
        List<DepartamentListDto> GetSalariatDepartamentList();
        List<string> GetDepartamentNumeList();
    }

    public class DepartamentAppService : ErpAppServiceBase, IDepartamentAppService
    {
        IRepository<Departament> _departamentRepository;
        IRepository<SalariatiDepartamente> _salariatiDepartamentRepository;

        public DepartamentAppService(IRepository<Departament> departamentRepository, IRepository<SalariatiDepartamente> salariatiDepartamentRepository)
        {
            _departamentRepository = departamentRepository;
            _salariatiDepartamentRepository = salariatiDepartamentRepository;
        }
        public List<DepartamentListDto> GetDepartamentList()
        {
            try
            {
                var list = _departamentRepository.GetAll().Where(f => f.State == State.Active).OrderBy(f => f.Name).ToList();
                var ret = ObjectMapper.Map<List<DepartamentListDto>>(list);
                return ret;
            }
            catch (System.Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public List<DepartamentListDto> GetSalariatDepartamentList()
        {
            try
            {
                var departamentList = _salariatiDepartamentRepository.GetAllIncluding(f => f.Departament).Where(f => f.State == State.Active)
                                                                     .Select(f => new DepartamentListDto
                                                                     {
                                                                         Id = f.DepartamentId,
                                                                         Name = f.Departament.Name
                                                                     })
                                                                     .Distinct()
                                                                     .ToList();

                return departamentList;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public List<string> GetDepartamentNumeList()
        {
            try
            {
                var list = _departamentRepository.GetAll().Where(f => f.State == State.Active).OrderBy(f => f.Name).Select(f=>f.Name).ToList();
                return list;
            }
            catch (System.Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }
    }
}
