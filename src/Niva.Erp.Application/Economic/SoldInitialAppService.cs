using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.Economic.Dto;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Economic.Casierii;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Economic
{
    public interface ISoldInitialAppService : IApplicationService
    {
        //sold initial
        List<SoldInitialDto> SoldList(DateTime dataStart, DateTime dataEnd);
        SoldInitialEditDto GetSold(int soldId);
        void SaveSold(SoldInitialEditDto sold);
        void DeleteSold(int soldId);
    }
    public class SoldInitialAppService : ErpAppServiceBase, ISoldInitialAppService
    {
        IRepository<Disposition> _dispositionRepository;

        public SoldInitialAppService(IRepository<Disposition> dispositionRepository)
        {
            _dispositionRepository = dispositionRepository;
        }

        //[AbpAuthorize("Casierie.Numerar.SoldInitial.Acces")]
        public List<SoldInitialDto> SoldList(DateTime dataStart, DateTime dataEnd)
        {
            var list = _dispositionRepository.GetAllIncluding(f => f.Currency).Where(f => f.State == State.Active &&
                                                                                          f.OperationType == OperationType.SoldInitial &&
                                                                                          f.DispositionDate >= dataStart && f.DispositionDate <= dataEnd)
                                                                              .OrderByDescending(f => f.DispositionDate)
                                                                              .ToList();

            var ret = ObjectMapper.Map<List<SoldInitialDto>>(list);
            return ret;
        }

        //[AbpAuthorize("Casierie.Numerar.SoldInitial.Acces")]
        public SoldInitialEditDto GetSold(int soldId)
        {
            SoldInitialEditDto sold;
            if (soldId == 0)
            {
                sold = new SoldInitialEditDto
                {
                    DispositionDate = DateTime.Now,
                    OperationType = OperationType.SoldInitial.ToString()
                };
            }
            else
            {
                var ret = _dispositionRepository.Get(soldId);
                sold = ObjectMapper.Map<SoldInitialEditDto>(ret);
            }
            return sold;
        }

        [AbpAuthorize("Casierie.Numerar.SoldInitial.Modificare")]
        public void SaveSold(SoldInitialEditDto sold)
        {
            // verific daca exista un sold initial cu acceasi moneda
            try
            {
                ValidateSold(sold);
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare!", ex.Message);
            }

            var appClient = GetCurrentTenant();
            sold.SumOper = sold.Value;
            var _sold = ObjectMapper.Map<Disposition>(sold);

            try
            {
                if (_sold.Id == 0)
                {
                    _sold.TenantId = appClient.Id;

                    _dispositionRepository.Insert(_sold);
                    CurrentUnitOfWork.SaveChanges();
                }
                else
                {
                    _dispositionRepository.Update(_sold);
                    CurrentUnitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        [AbpAuthorize("Casierie.Numerar.SoldInitial.Modificare")]
        public void DeleteSold(int soldId)
        {
            try
            {
                var sold = _dispositionRepository.Get(soldId);
                sold.State = State.Inactive;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare!", ex.ToString());
            }
        }

        [AbpAuthorize("Casierie.Numerar.SoldInitial.Modificare")]
        private void ValidateSold(SoldInitialEditDto sold)
        {
            var existingSold = _dispositionRepository.GetAllIncluding(f => f.Currency)
                                                     .Where(f => f.State == State.Active && f.CurrencyId == sold.CurrencyId && f.OperationType == OperationType.SoldInitial)
                                                     .OrderByDescending(f => f.DispositionDate)
                                                     .FirstOrDefault();
            if (existingSold != null)
                throw new Exception($"Exista un sold initial pentru moneda {existingSold.Currency.CurrencyName}");

        }
    }
}
