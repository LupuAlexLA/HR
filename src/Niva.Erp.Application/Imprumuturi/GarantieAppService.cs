using Abp.Application.Services;
using Abp.Domain.Repositories;
using System.Collections.Generic;
using Niva.Erp.Models.Imprumuturi;
using Niva.Erp.Imprumuturi.Dto;
using System.Linq;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Abp.EntityFrameworkCore.Extensions;
using System;
using Niva.Erp.MultiTenancy;
using Niva.Erp.Repositories.Conta.AutoOperation;
using Niva.Erp.Models.Conta;

namespace Niva.Erp.Imprumuturi
{
    public interface IGarantieAppService : IApplicationService
    {
        List<GarantieDto> GarantieList();

        List<GarantieDto> GarantieListId(int imprumutId);

        GarantieEditDto GetGarantieId(int id);
        void SaveGarantie(GarantieEditDto garantie);

        void DeleteGarantie(int id);

        List<OperatieGarantieDto> getOperatieGarantieList(int garantieId);
        List<OperatieGarantieDto> SaveOperatieGarantie(OperatieGarantieDto operatieGarantieDto);
        decimal getSoldGarantie(int garantieId);
    }

    public class GarantieAppService : ErpAppServiceBase,  IGarantieAppService
    {
        TenantManager _tenantManager;
        IRepository<Garantie> _garantieRepository;
        IRepository<OperatieGarantie> _operatieGarantieRepository;
        IAutoOperationRepository _IAutoOperationRepository;
        IRepository<Operation> _contaOperationsRepository;
        IRepository<OperationDetails> _contaOperationsDetailsRepository;

        public GarantieAppService(IRepository<Operation> contaOperationsRepository, IRepository<OperationDetails> contaOperationsDetailsRepository ,IAutoOperationRepository IAutoOperationRepository, IRepository<OperatieGarantie> operatieGarantieRepository, IRepository<Garantie> garantieRepository, TenantManager tenantManager)
        {
            _contaOperationsRepository = contaOperationsRepository;
            _contaOperationsDetailsRepository = contaOperationsDetailsRepository;
            _IAutoOperationRepository = IAutoOperationRepository;
            _operatieGarantieRepository = operatieGarantieRepository;
            _tenantManager = tenantManager;
            _garantieRepository = garantieRepository;
        }

        public List<GarantieDto> GarantieListId(int imprumutId)
        {
            var _garantie = _garantieRepository.GetAllIncluding(e => e.GarantieAccount, e => e.Currency, e => e.GarantieCeGaranteaza, e => e.GarantieTip, e => e.LegalPerson, e => e.OperatiiGarantie)
                                                        .Where(f => f.ImprumutId == imprumutId && f.State == Models.Conta.Enums.State.Active);
            //.Include(e => e.GarantieAccount)
            //.Include(e => e.Currency)
            //.Include(e => e.LegalPerson)
            //.Include(e => e.GarantieCeGaranteaza)
            //.Include(e => e.GarantieTip)
            // , , 
            // e => e.LegalPerson
            //
            var _garantietest = _garantieRepository.GetAll()
                                                  
                                                   .Where(f => f.ImprumutId == imprumutId && f.State == Models.Conta.Enums.State.Active);

            var ret = ObjectMapper.Map<List<GarantieDto>>(_garantie).ToList();
            
            return ret;
        }

        public List<GarantieDto> GarantieList()
        {
            var _garantie = _garantieRepository.GetAll()
                                                    .Include(e => e.GarantieAccount)
                                                    .Include(e => e.Currency)
                                                    .Include(e => e.LegalPerson)
                                                    .Include(e => e.GarantieCeGaranteaza)
                                                    .Include(e => e.GarantieTip)
                                                    .Include(e => e.OperatiiGarantie)
                                                    .Where(f => f.State == Models.Conta.Enums.State.Active);
            // f.ImprumutId == imprumutId && 
            //


            var ret = ObjectMapper.Map<List<GarantieDto>>(_garantie).ToList();
            
            return ret;
        }

        public GarantieEditDto GetGarantieId(int id)
        {
            var ret = _garantieRepository.Get(id);
            return ObjectMapper.Map<GarantieEditDto>(ret);
        }

        public void SaveGarantie(GarantieEditDto garantie)
        {
            var _garantie = ObjectMapper.Map<Garantie>(garantie);

            
                if (_garantie.Id == 0)
                {

                //  int _chk = _GarantieRepository.GetAll().Where(f => f.State == Models.Conta.Enums.State.Active).Count();

                //  if (_chk > 0) throw new UserFriendlyException("Eroare", "Termen existent!");

                _garantieRepository.Insert(_garantie);
                CurrentUnitOfWork.SaveChanges();

                var operatieGarantie = new OperatieGarantieDto()
                {   
                    TipOperatieGarantieEnum = TipOperatieGarantieEnum.Majorare,
                    DataOperatiei = _garantie.DocumentDate,
                    GarantieId = _garantie.Id,
                    Sold = 0,
                    Suma = _garantie.SumaGarantiei,
                    TenantId = GetCurrentTenant().Id
                };

                SaveOperatieGarantie(operatieGarantie);

                }
                else
                {
                    var appClient = GetCurrentTenant();
                    _garantie.TenantId = appClient.Id;
                _garantieRepository.Update(_garantie);
                }
            
            
        }

        public void DeleteGarantie(int id)
        {
            var _garantie = _garantieRepository.FirstOrDefault(f => f.Id == id);
            _garantie.State = Models.Conta.Enums.State.Inactive;
            _garantieRepository.Update(_garantie);
        }

        protected virtual Tenant GetCurrentTenant()
        {
            return _tenantManager.GetById(AbpSession.TenantId.Value);
        }

        public List<OperatieGarantieDto> SaveOperatieGarantie(OperatieGarantieDto operatieGarantieDto)
        {
            var operatiiGarantiiAnterioare = _operatieGarantieRepository.GetAll().Where(f => f.GarantieId == operatieGarantieDto.GarantieId && f.State == Models.Conta.Enums.State.Active).OrderBy(f => f.Id);
            var operatieGarantieDb = ObjectMapper.Map<OperatieGarantie>(operatieGarantieDto);

            // Daca nu exista operatii anterioare inseamnca ca functia este apelata de SaveGarantie() si introduce prima operatie care majoreaza sold-ul
            if (operatiiGarantiiAnterioare.Count() == 0)
            {
                operatieGarantieDb.Sold = operatieGarantieDto.Suma;
            }
            else
            {
                var ultimaOperatieGarantie = operatiiGarantiiAnterioare.LastOrDefault();

                if(operatieGarantieDb.TipOperatieGarantieEnum == TipOperatieGarantieEnum.Majorare)
                {
                    operatieGarantieDb.Sold = ultimaOperatieGarantie.Sold + operatieGarantieDb.Suma;
                }
                else
                {
                    operatieGarantieDb.Sold = ultimaOperatieGarantie.Sold - operatieGarantieDb.Suma;
                }
            }

            _operatieGarantieRepository.Insert(operatieGarantieDb);
            CurrentUnitOfWork.SaveChanges();

            _IAutoOperationRepository.OperatieGarantieToConta(operatieGarantieDb.Id, GetCurrentTenant().LocalCurrencyId.Value);
            CurrentUnitOfWork.SaveChanges();

            return getOperatieGarantieListTest(operatieGarantieDb.GarantieId);
        }

        public List<OperatieGarantieDto> DeleteOperatieGarantie(int operatieGarantieId)
        {
            var operGarantie = _operatieGarantieRepository.Get(operatieGarantieId);
            operGarantie.State = Models.Conta.Enums.State.Inactive;
            CurrentUnitOfWork.SaveChanges();

            var conta = _contaOperationsRepository.Get((int)operGarantie.ContaOperationId);
            conta.State = Models.Conta.Enums.State.Inactive;
            CurrentUnitOfWork.SaveChanges();

            return getOperatieGarantieListTest(operGarantie.GarantieId);
        }

        public List<OperatieGarantieDto> getOperatieGarantieList(int garantieId)
        {
            return getOperatieGarantieListTest(garantieId);
        }

        private List<OperatieGarantieDto> getOperatieGarantieListTest(int garantieId)
        {
            var opGarantieList = _operatieGarantieRepository.GetAll().Where(f => f.State == Models.Conta.Enums.State.Active && f.GarantieId == garantieId);

            return ObjectMapper.Map<List<OperatieGarantieDto>>(opGarantieList);
        }

        public decimal getSoldGarantie(int garantieId)
        {
            return _operatieGarantieRepository.GetAll().Where(f => f.State == Models.Conta.Enums.State.Active && f.GarantieId == garantieId).OrderBy(f => f.Id).Last().Sold;
        }
    }
}
