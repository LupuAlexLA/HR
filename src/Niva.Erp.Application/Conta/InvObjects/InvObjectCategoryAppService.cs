using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.Conta.InvObjects.Dto;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.InvObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Conta.InvObjects
{
    public interface IInvObjectCategoryAppService: IApplicationService
    {
        GetInvCategoryOutput CategoryList();
        InvObjectCategoryEditDto GetCategory(int categoryId);
        void SaveCategory(InvObjectCategoryEditDto category);
        void DeleteCategory(int categoryId);
    }

    public class GetInvCategoryOutput
    {
        public List<InvObjectCategoryListDto> GetCategoryList { get; set; }
    }

    public class InvObjectCategoryAppService : ErpAppServiceBase, IInvObjectCategoryAppService
    {
        IRepository<InvCateg> _invCategRepository;

        public InvObjectCategoryAppService(IRepository<InvCateg> invCategRepository)
        {
            _invCategRepository = invCategRepository;
        }

        public GetInvCategoryOutput CategoryList()
        {
            var _categoryList = _invCategRepository.GetAll().Where(f => f.State == State.Active).OrderBy(f => f.Name);
            var ret = new GetInvCategoryOutput { GetCategoryList = ObjectMapper.Map<List<InvObjectCategoryListDto>>(_categoryList) };
            return ret;
        }

        public void DeleteCategory(int categoryId)
        {
            try
            {
                var _category = _invCategRepository.GetAll().FirstOrDefault(f => f.Id == categoryId);
                _category.State = State.Inactive;
                _invCategRepository.Update(_category);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", LazyMethods.GetErrMessage(ex));
            }
        }

        public InvObjectCategoryEditDto GetCategory(int categoryId)
        {
            var ret = new InvObjectCategoryEditDto();
            InvCateg _category;
            if (categoryId == 0)
            {
                _category = new InvCateg();
            }
            else
            {
                _category = _invCategRepository.GetAll().FirstOrDefault(f => f.Id == categoryId);
            }
            ret = ObjectMapper.Map<InvObjectCategoryEditDto>(_category);
            return ret;
        }

        public void SaveCategory(InvObjectCategoryEditDto category)
        {
            var _category = ObjectMapper.Map<InvCateg>(category);

            if (_category.Id == 0)
            {

                int count = _invCategRepository.GetAll().Where(f => f.Name == _category.Name && f.State == State.Active).Count();

                if (count > 0)
                    throw new UserFriendlyException("Eroare", "Categorie existenta!");

                _invCategRepository.Insert(_category);
            }
            else
            {
                var appClient = GetCurrentTenant();
                _category.TenantId = appClient.Id;
                _invCategRepository.Update(_category);
            }
        }
    }
}
