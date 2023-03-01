using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.Economic.Dto;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Economic.Casierii;
using Niva.Erp.Repositories;
using Niva.Erp.Repositories.Conta.AutoOperation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Economic
{
    public interface IExchangeAppService : IApplicationService
    {
        ExchangeInitDto ExchangeInit();
        ExchangeInitDto ExchangeList(ExchangeInitDto form);
        ExchangeEditDto GetExchangeById(int? exchangeId);
        void SaveExchange(ExchangeEditDto exchange);
        void DeleteExchange(int exchangeId);
        decimal CalculateExchangeValue(decimal value, decimal? exchangeRate, int operType);
    }

    public class ExchangeAppService : ErpAppServiceBase, IExchangeAppService
    {
        IRepository<Exchange> _exchangeRepository;
        IAutoOperationRepository _autoOperationRepository;
        IOperationRepository _operationRepository;

        public ExchangeAppService(IRepository<Exchange> excangeRepository, IAutoOperationRepository autoOperationRepository, IOperationRepository operationRepository)
        {
            _exchangeRepository = excangeRepository;
            _autoOperationRepository = autoOperationRepository;
            _operationRepository = operationRepository;
        }

        //[AbpAuthorize("Casierie.SchimbValutar.Acces")]
        public decimal CalculateExchangeValue(decimal value, decimal? exchangeRate, int operType)
        {
            try
            {
                decimal exchangeValue = 0;
                if (exchangeRate != 0 && exchangeRate != null)
                {
                    exchangeValue = ((ExchangeOperType)operType == ExchangeOperType.CumparValuta) ? Math.Round((value / exchangeRate.Value), 2) : Math.Round((value * exchangeRate.Value), 2);
                }

                return exchangeValue;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        [AbpAuthorize("Casierie.SchimbValutar.Modificare")]
        public void DeleteExchange(int exchangeId)
        {
            try
            {
                var appClientId = GetCurrentTenant().Id;
                var exchange = _exchangeRepository.FirstOrDefault(f => f.Id == exchangeId && f.State == State.Active);
                _autoOperationRepository.DeleteContaForExchange(exchangeId, appClientId);
                exchange.State = State.Inactive;
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }

        }

        public ExchangeInitDto ExchangeInit()
        {
            try
            {
                var currDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                var startDate = currDate.AddMonths(-1);
                var ret = new ExchangeInitDto
                {
                   StartDate = startDate,
                   EndDate = currDate,
                   CurrencyId = null,
                   OperationTypeId = null,
                };
                ret = ExchangeList(ret);

                return ret;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        //[AbpAuthorize("Casierie.SchimbValutar.Acces")]
        public ExchangeInitDto ExchangeList(ExchangeInitDto form)
        {
            var appClient = GetCurrentTenant();
            var _list = _exchangeRepository.GetAllIncluding(f => f.ActivityType, f => f.BankAccountLei, f => f.BankAccountValuta, f => f.Curency)
                                           .Where(f => f.State == State.Active && f.TenantId == appClient.Id && form.StartDate <= f.OperationDate && f.OperationDate <= form.EndDate)
                                           .OrderBy(f => f.OperationDate)
                                           .ToList();
            if (form.OperationTypeId != null)
            {
                _list = _list.Where(f => f.ExchangeOperType ==(ExchangeOperType) form.OperationTypeId.Value).ToList();
            }

            if (form.CurrencyId != null)
            {
                _list = _list.Where(f => f.CurrencyId == form.CurrencyId).ToList();
            }

            var ret = ObjectMapper.Map<List<ExchangeListDto>>(_list);
            form.ExchangeList = ret;
            return form;
        }

        //[AbpAuthorize("Casierie.SchimbValutar.Acces")]
        public ExchangeEditDto GetExchangeById(int? exchangeId)
        {
            ExchangeEditDto exchange;
            if (exchangeId == 0)
            {
                exchange = new ExchangeEditDto()
                {
                    CurrencyId = null,
                    BankAccountLeiId = null,
                    BankAccountValutaId = null,
                    BankLeiId = null,
                    BankValutaId = null,
                    ExchangeOperType = null,
                    OperationType = null,
                    ActivityTypeId = null,
                    OperationDate = DateTime.Now,
                    ExchangeDate = DateTime.Now
                };
            }
            else
            {
                var exchangeDb = _exchangeRepository.GetAllIncluding(f => f.Curency, f => f.BankAccountLei, f => f.BankAccountValuta).FirstOrDefault(f => f.Id == exchangeId && f.State == State.Active);

                exchange = new ExchangeEditDto
                {
                    Id = exchangeDb.Id,
                    ActivityTypeId = exchangeDb.ActivityTypeId,
                    BankAccountLeiId = exchangeDb.BankAccountLeiId,
                    BankAccountValutaId = exchangeDb.BankAccountValutaId,
                    BankLeiId = exchangeDb.BankAccountLei.BankId,
                    BankValutaId = exchangeDb.BankAccountValuta.BankId,
                    CurrencyId = exchangeDb.CurrencyId,
                    ExchangeDate = exchangeDb.ExchangeDate,
                    ExchangedValue = exchangeDb.ExchangedValue,
                    ExchangeOperType = (int?)exchangeDb.ExchangeOperType,
                    ExchangeRate = exchangeDb.ExchangeRate,
                    OperationDate = exchangeDb.OperationDate,
                    OperationType = (int?)exchangeDb.ExchangeOperType,
                    Value = exchangeDb.Value,
                    CurrencyCode = (exchangeDb.ExchangeOperType == ExchangeOperType.CumparValuta) ? "RON" : exchangeDb.Curency.CurrencyCode,
                    LocalCurrencyCode = (exchangeDb.ExchangeOperType == ExchangeOperType.CumparValuta) ? exchangeDb.Curency.CurrencyCode : "RON",
                    ContaOperationId = exchangeDb.ContaOperationId
                };
            }

            return exchange;
        }

        [AbpAuthorize("Casierie.SchimbValutar.Modificare")]
        public void SaveExchange(ExchangeEditDto exchange)
        {
            exchange.OperationDate = new DateTime(exchange.OperationDate.Year, exchange.OperationDate.Month, exchange.OperationDate.Day);
            exchange.ExchangeDate = new DateTime(exchange.ExchangeDate.Year, exchange.ExchangeDate.Month, exchange.ExchangeDate.Day);
            var exchangeDb = ObjectMapper.Map<Exchange>(exchange);
            var localCurrencyId = GetCurrentTenant().LocalCurrencyId.Value;
            var appClientId = GetCurrentTenant().Id;

            if (!_operationRepository.VerifyClosedMonth(exchange.OperationDate))
                throw new UserFriendlyException("Eroare", "Nu se poate inregistra operatia deoarece luna contabila este inchisa");

            if (exchange.ExchangeRate == 0)
            {
                throw new UserFriendlyException("Eroare", "Cursul de schimb trebuie completat");
            }

            try
            {
                if (exchangeDb.Id == 0) // INSERT
                {
                    _exchangeRepository.Insert(exchangeDb);
                }
                else
                {

                    exchangeDb.TenantId = appClientId;



                    _exchangeRepository.Update(exchangeDb);
                }

                CurrentUnitOfWork.SaveChanges();
            }
            catch (System.Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }

            if (exchangeDb.ContaOperationId != null && exchangeDb.Id != 0) // sterg nota contabila pe edit
            {
                _autoOperationRepository.DeleteContaForExchange(exchangeDb.Id, appClientId);
            }

            //generez nota contabila
            try
            {
                _autoOperationRepository.SaveExchangeToConta(exchangeDb.Id, exchange.ExchangeDate, localCurrencyId);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }
    }
}
