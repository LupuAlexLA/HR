using Abp.EntityFrameworkCore;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Niva.EntityFramework.Repositories.Nomenclatures;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.EntityFrameworkCore.Repositories.Conta.AutoOperation;
using Niva.Erp.Models.Administration;
using Niva.Erp.Models.AutoOperation;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Imprumuturi;
using Niva.Erp.Repositories;
using Niva.Erp.Repositories.Imprumuturi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Niva.Erp.EntityFrameworkCore.Repositories.imprumut
{


    public class ImprumutRepository : ErpRepositoryBase<Imprumut, int>, IImprumutRepository
    {

        AutoOperationRepository _autoOperationRepository;
        ExchangeRatesRepository _exchangeRatesRepository;

        public ImprumutRepository(IDbContextProvider<ErpDbContext> context) : base(context)
        {
            _autoOperationRepository = new AutoOperationRepository(context);
            _exchangeRatesRepository = new ExchangeRatesRepository(context);

        }

        public void GenerateDobandaOper(DateTime Data, int operGenId, int localCurrencyId)
        {
            var imprumuturi = Context.Imprumuturi.Where(f => f.State == Models.Conta.Enums.State.Active).ToList();

            foreach (var value in imprumuturi)
            {
                if (value.TipCreditare == TipCreditare.Credit)
                {
                    GenerateDobandaConta(value, Data, operGenId, localCurrencyId);
                }
                if (value.TipCreditare == TipCreditare.LinieDeCredit)
                {
                    _autoOperationRepository.LinieDeCreditPentruLuna(value, Data, operGenId, localCurrencyId);
                }



            }
        }

        public void GenerateDobandaConta(Imprumut Imprumut, DateTime Data, int operGenId, int localCurrencyId)
        {
            var _rate = Context.Rate.Where(f => f.ImprumutId == Imprumut.Id && f.State == Models.Conta.Enums.State.Active && f.DataPlataRata.Month == Data.Month && f.DataPlataRata.Year == Data.Year && f.DataPlataRata < Imprumut.EndDate).ToList();
            var rateImprumut = Context.Rate.Where(f => f.ImprumutId == Imprumut.Id && f.State == Models.Conta.Enums.State.Active).ToList();

            // a ramas foreach dintr-o implementare mai veche chiar daca _rate este mereu o lista cu un singur element
            // se imbunatateste printr-un firstordefault si check in situatia in care e null
            foreach (var value in _rate)
            {

                var nextRata = rateImprumut.Where(f => f.Index == value.Index + 1).FirstOrDefault();
                var dobanda = Context.Dobanda.Where(f => f.RataId == nextRata.Id && f.ValoarePrincipal == 0).ToList();

                var _newdobanda = new Dobanda()
                {
                    OperationDate = LazyMethods.LastDayOfMonth(value.DataPlataRata),
                    RataId = nextRata.Id,
                    TenantId = value.TenantId,
                    ValoareDobanda = Math.Round((LazyMethods.LastDayOfMonth(value.DataPlataRata) - value.DataPlataRata).Days * nextRata.SumaDobanda / (nextRata.DataPlataRata - value.DataPlataRata).Days, 2),
                    OperGenerateId = operGenId,
                    State = State.Active,
                };

                if (dobanda.Count() == 0)
                {
                    Context.Dobanda.Add(_newdobanda);
                    Context.SaveChanges();
                    _autoOperationRepository.DobandaToConta(_newdobanda.Id, localCurrencyId, operGenId);
                }
                else
                {
                    var _dobanda = dobanda.FirstOrDefault();
                    _dobanda.OperationDate = _newdobanda.OperationDate;
                    _dobanda.RataId = _newdobanda.RataId;
                    _dobanda.ValoareDobanda = _newdobanda.ValoareDobanda;
                    _dobanda.ValoarePrincipal = _newdobanda.ValoarePrincipal;
                    _dobanda.OperGenerateId = _newdobanda.OperGenerateId;
                    _dobanda.State = _newdobanda.State;

                    _autoOperationRepository.DobandaToConta(_dobanda.Id, localCurrencyId, operGenId);
                    //_DobandaRepository.Update(_dobanda);
                    //CurrentUnitOfWork.SaveChanges();
                }

            }

        }
    }

}
