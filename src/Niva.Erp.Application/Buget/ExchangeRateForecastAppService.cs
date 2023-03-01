using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Niva.Erp.Buget.Dto;
using Niva.Erp.Models.Buget;
using Niva.Erp.Models.Conta.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Buget
{
    public interface IExchangeRateForecastAppService : IApplicationService
    {
        List<ExchangeRateForecastDto> ExchangeRateForecastDtoList();
        ExchangeRateForecastEditDto GetExchangeRateForecastId(int id);
        void SaveExchangeRateForecast(ExchangeRateForecastEditDto storage);
        List<ExchangeRateForecastDto> ExchangeRateForecastDtoListYear(int year);
        decimal GetExchangeRateForecastCurrency(int CurrencyId, int year);
        List<int> GetExchangeRateYearList();
        void DeleteExchangeRateForecast(int id);
    }

    public class ExchangeRateForecastAppService : ErpAppServiceBase, IExchangeRateForecastAppService
    {
        IRepository<ExchangeRateForecast> _exchangeRateForecastRepository;

        public ExchangeRateForecastAppService(IRepository<ExchangeRateForecast> exchangeRateForecastRepository)
        {
            _exchangeRateForecastRepository = exchangeRateForecastRepository;
        }

        //[AbpAuthorize("Buget.CursValutarEstimat.Acces")]
        public List<int> GetExchangeRateYearList()
        {
            try
            {
                var years = _exchangeRateForecastRepository.GetAll().Where(f => f.State == State.Active).Select(f => f.Year).Distinct().ToList();
                return years;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public List<ExchangeRateForecastDto> ExchangeRateForecastDtoList()
        {
            try
            {
                var _exchangeRateForecastTermen = _exchangeRateForecastRepository.GetAll().Include(f => f.Currency)
                                                        .Where(f => f.State == Models.Conta.Enums.State.Active);

                var ret = ObjectMapper.Map<List<ExchangeRateForecastDto>>(_exchangeRateForecastTermen).ToList();
                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public List<ExchangeRateForecastDto> ExchangeRateForecastDtoListYear(int year)
        {
            try
            {
                var _exchangeRateForecastTermen = _exchangeRateForecastRepository.GetAll().Include(f => f.Currency)
                                                        .Where(f => f.State == Models.Conta.Enums.State.Active && f.Year == year);

                var ret = ObjectMapper.Map<List<ExchangeRateForecastDto>>(_exchangeRateForecastTermen).ToList();
                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        [AbpAuthorize("Buget.CursValutarEstimat.Modificare")]
        public ExchangeRateForecastEditDto GetExchangeRateForecastId(int id)
        {
            try
            {
                ExchangeRateForecastEditDto exchangeRateForecastDto;
                var appClient = GetCurrentTenant();
                if (id == 0)
                {
                    exchangeRateForecastDto = new ExchangeRateForecastEditDto
                    {
                        CurrencyId = null,
                        ValoareEstimata = 0
                    };
                }
                else
                {
                    var exchangeRateForecastDb = _exchangeRateForecastRepository.GetAllIncluding(f => f.Currency).FirstOrDefault(f => f.Id == id && f.State == State.Active && f.TenantId == appClient.Id);
                    exchangeRateForecastDto = ObjectMapper.Map<ExchangeRateForecastEditDto>(exchangeRateForecastDb);
                }
                return exchangeRateForecastDto;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public decimal GetExchangeRateForecastCurrency(int CurrencyId, int year)
        {
            try
            {
                var ret = _exchangeRateForecastRepository.GetAll().Where(d => d.CurrencyId == CurrencyId && d.Year == year).FirstOrDefault();
                if (ret == null)
                {
                    return 1;
                }
                return ret.ValoareEstimata;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        [AbpAuthorize("Buget.CursValutarEstimat.Modificare")]
        public void SaveExchangeRateForecast(ExchangeRateForecastEditDto storage)
        {
            try
            {


                var _exhangeRate = ObjectMapper.Map<ExchangeRateForecast>(storage);

                if (_exhangeRate.Id == 0)
                {

                    int _chk = _exchangeRateForecastRepository.GetAll().Where(f => f.CurrencyId == _exhangeRate.CurrencyId && f.Year == _exhangeRate.Year && f.State == Models.Conta.Enums.State.Active).Count();

                    if (_chk > 0) throw new UserFriendlyException("Eroare", "Termen existent!");

                    _exchangeRateForecastRepository.Insert(_exhangeRate);
                }
                else
                {
                    var appClient = GetCurrentTenant();
                    _exhangeRate.TenantId = appClient.Id;
                    _exchangeRateForecastRepository.Update(_exhangeRate);
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        [AbpAuthorize("Buget.CursValutarEstimat.Modificare")]
        public void DeleteExchangeRateForecast(int id)
        {
            try
            {
                var _exhangeRate = _exchangeRateForecastRepository.Get(id);
                _exhangeRate.State = Models.Conta.Enums.State.Inactive;
                _exchangeRateForecastRepository.Update(_exhangeRate);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }
    }
}
