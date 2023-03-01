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

namespace Niva.Erp.Imprumuturi
{
    public interface IRataAppService : IApplicationService
    {
        List<RataDto> RataList();

        List<RataDto> RataListId(int imprumutId);

        RataEditDto GetRataId(int id);
        
        void SaveRata(RataEditDto rata);
        
        void DeleteRata(int id);
        void DeleteAllRateIdDate(int imprumutId, DateTime dataPlataRata);
        void GenerateRateOrientativeRataTotalaEgala(RataEditDto rata);
        void RegenerareScadentarDeLaRandCurent(RataEditDto rata);

        void GenerateRateOrientativeRataDescrescatoare(RataEditDto rata);
    }

    public class RataAppService : ErpAppServiceBase,  IRataAppService
    {
        IRepository<Imprumut> _ImprumutRepository;
        IRepository<Rata> _RataRepository;
        IRepository<Operatie> _OpereatieRepository;
        IRepository<Dobanda> _DobandaRepository;
        IAutoOperationRepository _autoOperationRepository;

        public RataAppService(IAutoOperationRepository autoOperationRepository , IRepository<Dobanda> DobandaRepository, IRepository<Imprumut> ImprumutRepository, IRepository<Rata> RataRepository, IRepository<Operatie> OpereatieRepository)
        {
            _autoOperationRepository = autoOperationRepository;
            _DobandaRepository = DobandaRepository;
            _ImprumutRepository = ImprumutRepository;
            _RataRepository = RataRepository;
            _OpereatieRepository = OpereatieRepository;
        }

        public List<RataDto> RataListId(int imprumutId)
        {
            var _rata = _RataRepository.GetAll().Include(e => e.Currency)
                                                .Where(f => f.ImprumutId == imprumutId && f.State == Models.Conta.Enums.State.Active)
                                                .OrderBy(f => f.DataPlataRata); // sau .OrderBy(f => f.Index)




            var ret = ObjectMapper.Map<List<RataDto>>(_rata).ToList();

            return ret;
        }
        public List<RataDto> RataList()
        {
            var _rata = _RataRepository.GetAll().Where(f => f.State == Models.Conta.Enums.State.Active);
            


            var ret = ObjectMapper.Map<List<RataDto>>(_rata).ToList();
            return ret;
        }

        public RataEditDto GetRataId(int id)
        {
            var ret = _RataRepository.Get(id);
            return ObjectMapper.Map<RataEditDto>(ret);
        }


        public void RegenerareScadentarDeLaRandCurent(RataEditDto rata)
        {
            
            DeleteAllRateIdDate(rata.ImprumutId, rata.DataPlataRata);
            if (rata.TipRata == TipRata.RataTotalaEgala)
            {
                GenerateRateOrientativeRataTotalaEgala(rata);
            }
            else if(rata.TipRata == TipRata.RateDescrescatoare)
            {
                GenerateRateOrientativeRataDescrescatoare(rata);
            }
            
        }

        public void SaveRata(RataEditDto rata)
        {
            var _rata = ObjectMapper.Map<Rata>(rata);
         //   var _imprumut = _ImprumutRepository.Get(rata.ImprumutId);

            if (_rata.Id == 0)
            {

                //  int _chk = _GarantieRepository.GetAll().Where(f => f.State == Models.Conta.Enums.State.Active).Count();

                //  if (_chk > 0) throw new UserFriendlyException("Eroare", "Termen existent!");

                _RataRepository.Insert(_rata);
            }
            else
            {
                var appClient = GetCurrentTenant();
                _rata.TenantId = appClient.Id;
                _RataRepository.Update(_rata);
                CurrentUnitOfWork.SaveChanges();

                if (_rata.IsValid)
                {
                    //var operatie = new Operatie()
                    //{
                    //    DataOperatie = _rata.DataPlataRata,
                    //    TipOperatie = TipOperatie.Imprumut,
                    //    Valoare = _rata.SumaPlatita,
                    //    TenantId = appClient.Id,
                    //};

                    //_OpereatieRepository.Insert(operatie);
                    //CurrentUnitOfWork.SaveChanges();
                    // _OpereatieRepository;
                    // ce e mai sus ar trebui sters dar de verificat

                    GeneratePlataConta(_rata.Id);
                    CurrentUnitOfWork.SaveChanges();
                    _autoOperationRepository.DiminuareRambursareToConta(_rata.Id, GetCurrentTenant().LocalCurrencyId.Value);
                    CurrentUnitOfWork.SaveChanges();

                }
                
                
            }
        }

        public void GeneratePlataConta(int rataId)
        {
            var rata = _RataRepository.GetAll().Where(f => f.Id == rataId).AsNoTracking().FirstOrDefault();
            var dobanda = _DobandaRepository.GetAll().Where(f => f.RataId == rata.Id && f.State == Models.Conta.Enums.State.Active).AsNoTracking().FirstOrDefault();
            int localCurrencyId = GetCurrentTenant().LocalCurrencyId.Value;

            if (dobanda == null)
            {
                throw new UserFriendlyException("Eroare", "Nu a fost generata dobanda din ecranul Sfarsit de Luna");
            }
            else
            {
                var ret = ObjectMapper.Map<Dobanda>(dobanda);
                ret.ValoareDobanda = rata.SumaDobanda - ret.ValoareDobanda;
                ret.OperationDate = rata.DataPlataRata;
                ret.ValoarePrincipal = rata.SumaPrincipal;
                ret.Id = 0;
                ret.ContaOperationId = null;
                ret.ContaOperationDetailId = null;
                ret.RataId = rata.Id;
                _DobandaRepository.Insert(ret);
                CurrentUnitOfWork.SaveChanges();
                _autoOperationRepository.DobandaToConta(ret.Id, localCurrencyId, (int)ret.OperGenerateId);
                CurrentUnitOfWork.SaveChanges();
               _autoOperationRepository.PlataToConta(ret.Id, localCurrencyId, (int)ret.OperGenerateId);
                CurrentUnitOfWork.SaveChanges();
            }

            CurrentUnitOfWork.SaveChanges();

        }

        public void GenerateRateOrientativeRataDescrescatoare(RataEditDto _rata)
        {
            var imprumut = _ImprumutRepository.Get(_rata.ImprumutId);
            var rata = new RataEditDto();


            if (imprumut.ImprumuturiTipDurata == ImprumuturiTipDurata.Zile)
            {
                imprumut.Durata = imprumut.Durata / 30;
            }
            if (imprumut.PerioadaTipDurata == PerioadaTipDurata.Zile)
            {
                imprumut.Periodicitate = imprumut.Periodicitate / 30;
            }


            rata.ImprumutId = imprumut.Id;
            rata.TipRata = imprumut.TipRata;
            rata.CurrencyId = imprumut.CurrencyId;
            rata.TipDobanda = imprumut.TipDobanda;
            rata.DataPlataRata = imprumut.StartDate;



            rata.Sold = _rata.Sold;

            rata.SumaPrincipal = _rata.SumaPrincipal;
       //   var rataLunara = (double)imprumut.Suma * ((double)imprumut.ProcentDobanda / 100 / 12 * imprumut.Periodicitate) / (1 - Math.Pow((1 + (double)imprumut.ProcentDobanda / 100 / 12 * imprumut.Periodicitate), -imprumut.Durata / imprumut.Periodicitate));
            var _procentDobanda = imprumut.ProcentDobanda / 100;
            DateTime LastDate = _rata.DataPlataRata;
            var conventieDobanda = 365;

            for (int i = _rata.Index; i < imprumut.Durata / imprumut.Periodicitate; i++)
            {


                if (imprumut.isFinalDeLuna)
                {
                    rata.DataPlataRata = _rata.DataPlataRata.AddMonths((i - _rata.Index + 1) * imprumut.Periodicitate);

                    var firstDayOfMonth = new DateTime(rata.DataPlataRata.Year, rata.DataPlataRata.Month, 1);
                    var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                    rata.DataPlataRata = lastDayOfMonth;
                }
                else
                {
                    rata.DataPlataRata = _rata.DataPlataRata.AddMonths((i - _rata.Index + 1) * imprumut.Periodicitate);
                }


                rata.SumaDobanda = rata.Sold * _procentDobanda * (decimal)(rata.DataPlataRata - LastDate).TotalDays / conventieDobanda;
                rata.SumaPlatita = rata.SumaPrincipal + rata.SumaDobanda;
                rata.Sold -= rata.SumaPrincipal;

                rata.Index = i + 1;

                var __rata = ObjectMapper.Map<Rata>(rata);
                _RataRepository.Insert(__rata);
                CurrentUnitOfWork.SaveChanges();

                LastDate = rata.DataPlataRata;
                //rata.DataPlataRata = imprumut.StartDate.AddMonths((i + 1) * imprumut.Periodicitate);

            }
        }

        public void GenerateRateOrientativeRataTotalaEgala(RataEditDto _rata)
        {
           
            var imprumut = _ImprumutRepository.Get(_rata.ImprumutId);

            var rata = new RataEditDto();
            


            if (imprumut.ImprumuturiTipDurata == ImprumuturiTipDurata.Zile)
            {
                imprumut.Durata = imprumut.Durata / 30;
            }
            if (imprumut.PerioadaTipDurata == PerioadaTipDurata.Zile)
            {
                imprumut.Periodicitate = imprumut.Periodicitate / 30;
            }

            //  rata.DataPlataRata = imprumut.StartDate;
            rata.ImprumutId = imprumut.Id;
            rata.TipRata = imprumut.TipRata;
            rata.CurrencyId = imprumut.CurrencyId;
            rata.TipDobanda = imprumut.TipDobanda;
            rata.DataPlataRata = _rata.DataPlataRata;

            rata.Sold = _rata.Sold;
            //  rata.DataPlataRata = imprumut.StartDate;
            var rataLunara = _rata.SumaPlatita;
            var _procentDobanda = imprumut.ProcentDobanda / 100 / 12 * imprumut.Periodicitate;

            for (int i = _rata.Index; i < imprumut.Durata/imprumut.Periodicitate; i++)
            {


                if (imprumut.isFinalDeLuna)
                {
                    rata.DataPlataRata = _rata.DataPlataRata.AddMonths((i - _rata.Index + 1) * imprumut.Periodicitate);

                    var firstDayOfMonth = new DateTime(rata.DataPlataRata.Year, rata.DataPlataRata.Month, 1);
                    var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                    rata.DataPlataRata = lastDayOfMonth;
                }
                else
                {
                    rata.DataPlataRata = _rata.DataPlataRata.AddMonths((i - _rata.Index + 1) * imprumut.Periodicitate);
                }

                //rata.DataPlataRata = _rata.DataPlataRata.AddMonths((i-_rata.Index+1)*imprumut.Periodicitate);
                rata.SumaPlatita = (decimal)rataLunara;
                rata.SumaDobanda = rata.Sold * _procentDobanda;
                rata.SumaPrincipal = (decimal)rataLunara - rata.Sold * _procentDobanda;
                rata.Sold -= rata.SumaPrincipal;
                rata.Index = i + 1;

                

                
                    var __rata = ObjectMapper.Map<Rata>(rata);
                    _RataRepository.Insert(__rata);
                    CurrentUnitOfWork.SaveChanges();

                

            }
        }


        public void DeleteAllRateIdDate(int imprumutId, DateTime dataPlataRata)
        {
            var _rate = _RataRepository.GetAll().Where(f =>  f.State == Models.Conta.Enums.State.Active && f.ImprumutId == imprumutId && f.IsValid == false && f.DataPlataRata > dataPlataRata);
            var ret = ObjectMapper.Map<List<Rata>>(_rate).ToList();
            ret.ForEach(rata =>
            {
                rata.State = Models.Conta.Enums.State.Inactive;
                _RataRepository.Update(rata);
            });
            
        }

        public void DeleteRata(int id)
        {
            var _rata = _RataRepository.FirstOrDefault(f => f.Id == id);
            _rata.State = Models.Conta.Enums.State.Inactive;
            _RataRepository.Update(_rata);
        }

        
    }
}
