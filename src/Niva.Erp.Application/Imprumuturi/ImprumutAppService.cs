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
using Abp.Domain.Uow;
using Niva.Erp.Repositories.Conta.AutoOperation;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Repositories.Conta.Nomenclatures;
using Niva.Conta.Nomenclatures;

namespace Niva.Erp.Imprumuturi
{

    public interface IImprumutAppService : IApplicationService
    {
        List<ImprumutDto> ImprumutList();
        ImprumutDto ImprumutId(int id);
        ImprumutEditDto GetImprumutId(int id);
        void SaveImprumut(ImprumutEditDto imprumut);
        void UpdateRateVariabile();

        void DeleteImprumut(int id);
        void GenerateRateOrientativeRataTotalaEgala(Imprumut imprumut);
        void GenerateRateOrientativeRataDescrescatoare(Imprumut imprumut);
        void DeleteAllRateId(int imprumutId);
        void UpdateRateVariabileId(Imprumut imprumut);
        List<ImprumutStateDto> ImprumutStateList(int imprumutId);
        List<ImprumutStateDto> SaveImprumutStateList(ImprumutStateDto imprumutState);
    }

    public class ImprumutAppService : ErpAppServiceBase, IImprumutAppService
    {
        IRepository<Imprumut> _ImprumutRepository;
        IRepository<Rata> _RataRepository;
        IRepository<Dobanda> _DobandaRepository;
        IRepository<DateDobanziReferinta> _DateDobanziReferintaRepository;
        IRepository<Comision> _ComisionRepository;
        IRepository<DataComision> _DataComisionRepository;
        IRepository<Tragere> _TragereRepository;
        IRepository<ImprumutState> _ImprumutStateRepository;
        IRepository<Garantie> _GarantieRepository;
        IRepository<Operation> _OperationsRepository;
        IAutoOperationRepository _autoOperationRepository;
        IRepository<OperationDetails> _operationDetailsRepository;
        IAccountRepository _accountRepository;

        public ImprumutAppService(IAccountRepository accountRepository , IRepository<OperationDetails> operationDetailsRepository, IRepository<Dobanda> DobandaRepository, IRepository<Operation> OperationsRepository, IRepository<Garantie> GarantieRepository, IRepository<ImprumutState> ImprumutStateRepository, IRepository<Tragere> TragereRepository, IRepository<Imprumut> ImprumutRepository, IRepository<Rata> RataRepository, 
                                  IRepository<DateDobanziReferinta> DateDobanziReferintaRepository, IRepository<Comision> ComisionRepository, IRepository<DataComision> DataComisionRepository, IAutoOperationRepository autoOperationRepository)
        {
            _accountRepository = accountRepository;
            _DobandaRepository = DobandaRepository;
            _GarantieRepository = GarantieRepository;
            _OperationsRepository = OperationsRepository;
            _DateDobanziReferintaRepository = DateDobanziReferintaRepository;
            _ImprumutRepository = ImprumutRepository;
            _RataRepository = RataRepository;
            _ComisionRepository = ComisionRepository;
            _DataComisionRepository = DataComisionRepository;
            _TragereRepository = TragereRepository;
            _ImprumutStateRepository = ImprumutStateRepository;
            _autoOperationRepository = autoOperationRepository;
            _operationDetailsRepository = operationDetailsRepository;
        }
        
        public List<ImprumutDto> ImprumutList()
        {
            var appClient = GetCurrentTenant();
            var _imprumut = _ImprumutRepository.GetAllIncluding(f => f.ImprumuturiTipuri, f => f.ImprumuturiTermen, f => f.LoanAccount, f => f.PaymentAccount, f => f.DocumentType,
                                                                f => f.DobanziReferinta, f => f.Currency, f => f.Bank, f => f.Bank.LegalPerson, f => f.ImprumutStateList)
                                               .Where(f => f.State == State.Active)
                                               .ToList();
            //var _imprumut = _ImprumutRepository.GetAll().Include(e => e.ImprumuturiTipuri)
            //                                            .Include(e => e.ImprumuturiTermen)
            //                                            .Include(e => e.LoanAccount)
            //                                            .Include(e => e.PaymentAccount)
            //                                            .Include(e => e.DocumentType)
            //                                            .Include(e => e.DobanziReferinta)
            //                                            .Include(e => e.Currency)
                                                        
            //                                            .Include(e => e.Bank)
            //                                            .ThenInclude(e => e.LegalPerson)
                                                        
                                                        
            //                                        .Where(f => f.State == Models.Conta.Enums.State.Active);
            

            var ret = ObjectMapper.Map<List<ImprumutDto>>(_imprumut);
            return ret;
        }

        public ImprumutDto ImprumutId(int id)
        {
            var _imprumut = _ImprumutRepository.GetAll().Include(e => e.ImprumuturiTipuri)
                                                        .Include(e => e.ImprumuturiTermen)
                                                        .Include(e => e.LoanAccount)
                                                        .Include(e => e.PaymentAccount)
                                                        .Include(e => e.DocumentType)
                                                        .Include(e => e.DobanziReferinta)
                                                        .Include(e => e.Currency)
                                                        .Include(e => e.Bank)
                                                        .ThenInclude(e => e.LegalPerson)
                                                        .Include(e => e.ImprumutStateList)                                                      
                                                        .FirstOrDefault(e => e.Id == id);
            
                                                    

            var ret = ObjectMapper.Map<ImprumutDto>(_imprumut);
            return ret;
        }

        public ImprumutEditDto GetImprumutId(int id)
        {
            var ret = ObjectMapper.Map<ImprumutEditDto>(_ImprumutRepository.Get(id)); 

            if (ret.ContContabilId != null)
            {
                var acc = _accountRepository.GetAccountById(ret.ContContabilId);

                var item = new AccountListDDDto()
                {
                    Id = acc.Id,
                    Name = acc.Symbol + "-" + acc.AccountName
                };

                ret.AccountName = item;
            }
            
            return ret;
        }

        public void GenerateRateOrientativeRataDescrescatoare(Imprumut imprumut)
        {
            


            if (imprumut.ImprumuturiTipDurata == ImprumuturiTipDurata.Zile) // de revizuit
            {
                //imprumut.Durata = imprumut.Durata / 30;
                imprumut.Durata = ((imprumut.EndDate.Year - imprumut.StartDate.Year) * 12) + imprumut.EndDate.Month - imprumut.StartDate.Month;
                imprumut.ImprumuturiTipDurata = ImprumuturiTipDurata.Luni;
            }
            if (imprumut.PerioadaTipDurata == PerioadaTipDurata.Zile)
            {
             //   imprumut.Periodicitate = imprumut.Periodicitate / 30;
                imprumut.Periodicitate = ((imprumut.EndDate.Year - imprumut.StartDate.Year) * 12) + imprumut.EndDate.Month - imprumut.StartDate.Month;
                imprumut.PerioadaTipDurata = PerioadaTipDurata.Luni;
            }

            var rata = new RataEditDto()
            {
                ImprumutId = imprumut.Id,
                TipRata = imprumut.TipRata,
                DataPlataRata = imprumut.StartDate,
                Sold = imprumut.Suma,
                IsValid = true,
                TipDobanda = imprumut.TipDobanda,
                ProcentDobanda = imprumut.ProcentDobanda,
                SumaPrincipal = imprumut.Suma / imprumut.Durata * imprumut.Periodicitate,
                CurrencyId = imprumut.CurrencyId
            };
            
         //   var rataLunara = (double)imprumut.Suma * ((double)imprumut.ProcentDobanda / 100 / 12 * imprumut.Periodicitate) / (1 - Math.Pow((1 + (double)imprumut.ProcentDobanda / 100 / 12 * imprumut.Periodicitate), -imprumut.Durata / imprumut.Periodicitate));
            var _procentDobanda = imprumut.ProcentDobanda / 100;
            DateTime LastDate = imprumut.StartDate;
            var conventieDobanda = 365;

            for (int i = 0; i <= imprumut.Durata / imprumut.Periodicitate; i++)
            {

                var _rata = ObjectMapper.Map<Rata>(rata);
                _RataRepository.Insert(_rata);
                CurrentUnitOfWork.SaveChanges();

                if (imprumut.isFinalDeLuna)
                {
                    rata.DataPlataRata = imprumut.StartDate.AddMonths((i+1) * imprumut.Periodicitate);

                    var firstDayOfMonth = new DateTime(rata.DataPlataRata.Year, rata.DataPlataRata.Month, 1);
                    var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                    rata.DataPlataRata = lastDayOfMonth;
                }
                else
                {
                    rata.DataPlataRata = imprumut.StartDate.AddMonths((i+1) * imprumut.Periodicitate);
                }

                


                rata.SumaDobanda = rata.Sold * _procentDobanda * (decimal)(rata.DataPlataRata - LastDate).TotalDays/conventieDobanda;
                rata.SumaPlatita = rata.SumaPrincipal + rata.SumaDobanda;
                rata.Sold -= rata.SumaPrincipal;
                rata.IsValid = false;
                rata.Index = i + 1;

                

                LastDate = rata.DataPlataRata;
                //rata.DataPlataRata = imprumut.StartDate.AddMonths((i + 1) * imprumut.Periodicitate);

            }
        }
       
        public void GenerateRateOrientativeRataTotalaEgala(Imprumut imprumut)
        {
            
            
            var _date = new DateDobanziReferinta();
            decimal dobandaVariabila = imprumut.ProcentDobanda;

            //var _imprumut = new Imprumut()
            //{
            //    ProcentDobanda = imprumut.ProcentDobanda,
            //    Durata = imprumut.Durata ,
            //    Periodicitate = imprumut.Periodicitate
                
            //};
            

            if (imprumut.TipDobanda == TipDobanda.Variabila)
            {
                try
                {
                    _date = _DateDobanziReferintaRepository.GetAll()
                                                       .Include(e => e.DobanziReferinta)
                                                       .Where(f => f.DobanziReferintaId == imprumut.DobanziReferintaId && f.State == Models.Conta.Enums.State.Active && f.Data <= imprumut.StartDate).OrderBy(f => f.Data).Last();
                }
                catch
                {
                    throw new UserFriendlyException("Eroare", "Dobanda de referinta neactualizata!");
                }
                dobandaVariabila = imprumut.MarjaFixa + _date.Valoare;
            }


            if (imprumut.ImprumuturiTipDurata == ImprumuturiTipDurata.Zile) // de revizuit
            {
                //imprumut.Durata = imprumut.Durata / 30;
                imprumut.Durata = ((imprumut.EndDate.Year - imprumut.StartDate.Year) * 12) + imprumut.EndDate.Month - imprumut.StartDate.Month;
                imprumut.ImprumuturiTipDurata = ImprumuturiTipDurata.Luni;
            }
            if (imprumut.PerioadaTipDurata == PerioadaTipDurata.Zile)
            {
                //   imprumut.Periodicitate = imprumut.Periodicitate / 30;
                imprumut.Periodicitate = ((imprumut.EndDate.Year - imprumut.StartDate.Year) * 12) + imprumut.EndDate.Month - imprumut.StartDate.Month;
                imprumut.PerioadaTipDurata = PerioadaTipDurata.Luni;
            }

            var rata = new RataEditDto()
            {
                ImprumutId = imprumut.Id,
                TipRata = imprumut.TipRata,
                DataPlataRata = imprumut.StartDate,
                IsValid = true,
                TipDobanda = imprumut.TipDobanda,
                ProcentDobanda = dobandaVariabila,
                Sold = imprumut.Suma,
                CurrencyId = imprumut.CurrencyId
            };
            
          
            var rataLunara = (double)imprumut.Suma * ((double)dobandaVariabila / 100/12*imprumut.Periodicitate)/(1-Math.Pow((1+(double)dobandaVariabila / 100/12*imprumut.Periodicitate),-imprumut.Durata/imprumut.Periodicitate));
            var _procentDobanda = dobandaVariabila / 100 / 12 * imprumut.Periodicitate;

            for (int i = 0; i <= imprumut.Durata/imprumut.Periodicitate ; i++)
            {

                var _rata = ObjectMapper.Map<Rata>(rata);
                _RataRepository.Insert(_rata);
                CurrentUnitOfWork.SaveChanges();


                if (imprumut.isFinalDeLuna)
                {
                    rata.DataPlataRata = imprumut.StartDate.AddMonths((i+1) * imprumut.Periodicitate);

                    var firstDayOfMonth = new DateTime(rata.DataPlataRata.Year, rata.DataPlataRata.Month, 1);
                    var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                    rata.DataPlataRata = lastDayOfMonth;
                }
                else
                {
                    rata.DataPlataRata = imprumut.StartDate.AddMonths((i+1)  * imprumut.Periodicitate);
                }

              
                rata.SumaPlatita =(decimal) rataLunara;
                rata.SumaDobanda = rata.Sold * _procentDobanda;
                rata.SumaPrincipal = (decimal) rataLunara - rata.Sold * _procentDobanda;
                rata.Sold -= rata.SumaPrincipal;
                rata.Index = i + 1;
                rata.IsValid = false;

             

            }    
        }

        public void AdaugaStateInitial(ImprumutEditDto _imprumut)
        {
            var appClient = GetCurrentTenant();

            var _imprumutState = new ImprumutState { 
                ImprumutId = _imprumut.Id,
                OperationDate = _imprumut.DocumentDate,
                ImprumuturiStare = ImprumuturiStare.Inregistrat,
                TenantId = appClient.Id,
            };
            
            _ImprumutStateRepository.Insert(_imprumutState); // inregistrat
            CurrentUnitOfWork.SaveChanges();
            
        }

        public void SaveImprumut(ImprumutEditDto imprumut)
        {
            int localCurrencyId = GetCurrentTenant().LocalCurrencyId.Value;
            var _imprumut = ObjectMapper.Map<Imprumut>(imprumut);

            if (_imprumut.Id == 0)
            {
                int _chk = _ImprumutRepository.GetAll().Where(f => f.DocumentNr == _imprumut.DocumentNr && f.State == Models.Conta.Enums.State.Active).Count();
                if (_chk > 0) throw new UserFriendlyException("Eroare", "Termen existent!");

         
                _ImprumutRepository.InsertAndGetId(_imprumut);
                CurrentUnitOfWork.SaveChanges();


                var __imprumut = ObjectMapper.Map<ImprumutEditDto>(_imprumut); // Copie a imprumutului pentru a evita situatia in care EF updateaza Db daca este editat obiectul de alta functie
                

                if (__imprumut.TipCreditare == TipCreditare.Credit)
                {
                    if (__imprumut.TipRata == TipRata.RataTotalaEgala)
                    {
                        GenerateRateOrientativeRataTotalaEgala(ObjectMapper.Map<Imprumut>(__imprumut));
                        AdaugaStateInitial(__imprumut);
                    }
                    else
                    {
                        GenerateRateOrientativeRataDescrescatoare(ObjectMapper.Map<Imprumut>(__imprumut));
                        AdaugaStateInitial(__imprumut);
                    }
                }
                else
                {
                    GenerateTragereInitiala(ObjectMapper.Map<Imprumut>(__imprumut));
                    AdaugaStateInitial(__imprumut);
                }
                
            }
            else
            {


                var appClient = GetCurrentTenant();
                _imprumut.TenantId = appClient.Id;
                _ImprumutRepository.Update(_imprumut);
                CurrentUnitOfWork.SaveChanges();

                var _comisioane = _ComisionRepository.GetAll().Where(f => f.State == Models.Conta.Enums.State.Active && f.ImprumutId == _imprumut.Id).Count();

                var _garantie = _GarantieRepository.GetAll().Where(f => f.State == Models.Conta.Enums.State.Active && f.ImprumutId == _imprumut.Id).Count();

                // de implementat verificarea pentru operatii contabile
                if (_imprumut.TipCreditare == TipCreditare.Credit)
                {
                    if (_comisioane == 0 && _garantie == 0)
                    {
                        // DeleteAllOperationsDocNum(_imprumut.DocumentNr.ToString());
                        DeleteAllRateOnEdit(_imprumut.Id);
                        // DeleteAllImprumutState(_imprumut.Id);

                        if (_imprumut.TipRata == TipRata.RataTotalaEgala)
                        {
                            GenerateRateOrientativeRataTotalaEgala(_imprumut);
                            //   AdaugaStateInitial(ObjectMapper.Map<ImprumutEditDto>(_imprumut));
                        }
                        else
                        {
                            GenerateRateOrientativeRataDescrescatoare(_imprumut);
                            //  AdaugaStateInitial(ObjectMapper.Map<ImprumutEditDto>(_imprumut));
                        }
                    }
                    else
                    {
                        throw new UserFriendlyException("Eroare", "Comision/Garantie Definite!");
                    }
                }


            }

            //generare nota contabila
            _autoOperationRepository.ImprumutToConta(_imprumut.Id, localCurrencyId);
        }

        public void GenerateTragereInitiala(Imprumut imprumut)
        {
            var appClient = GetCurrentTenant();
            var tragere = new TragereDto()
            {
                ImprumutId = imprumut.Id,
                DataTragere = imprumut.LoanDate,
                SumaDisponibila = imprumut.Suma,
                SumaTrasa = 0,
                Dobanda = 0,
                CurrencyId = imprumut.CurrencyId,
                TenantId = appClient.Id,
                TipTragere = TipTragere.Acordare,
                
            };
            _TragereRepository.Insert(ObjectMapper.Map<Tragere>(tragere));

        }

        public void UpdateRateVariabileId(Imprumut imprumut)
        {
            var _date = new DateDobanziReferinta(); // obiect pentru dobanda de refenrinta
            var rataLunara = (double)imprumut.Suma * ((double)imprumut.ProcentDobanda / 100 / 12 * imprumut.Periodicitate) / (1 - Math.Pow((1 + (double)imprumut.ProcentDobanda / 100 / 12 * imprumut.Periodicitate), -imprumut.Durata / imprumut.Periodicitate));
            var _procentDobanda = imprumut.ProcentDobanda / 100 / 12 * imprumut.Periodicitate;
            // se sterg ratele nevalidate pentru a regenera ratele cu noile valori ale roborului
            // iau ultima rata validata a imprumutului, sterg toate ratele care nu sunt valide si regenerez ratele tinand cont de dobanda variabila
            var _rata = _RataRepository.GetAll().Where(f => f.ImprumutId == imprumut.Id && f.State == Models.Conta.Enums.State.Active && f.IsValid == true).OrderBy(f => f.DataPlataRata).Last();
           
            

            var rata = new RataEditDto() // rata initiala
            {
                ImprumutId = imprumut.Id,
                TipRata = imprumut.TipRata,
                DataPlataRata = imprumut.StartDate,
                IsValid = false,
                TipDobanda = imprumut.TipDobanda,
                ProcentDobanda = _rata.ProcentDobanda,
                Sold = _rata.Sold,
                Index = _rata.Index,
                CurrencyId = imprumut.CurrencyId
            };

            for(int i = _rata.Index; i < imprumut.Durata / imprumut.Periodicitate; i++)
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

                try
                {

                    _date = _DateDobanziReferintaRepository.GetAll()
                                                       
                                                       .Where(f => f.DobanziReferintaId == imprumut.DobanziReferintaId && f.State == Models.Conta.Enums.State.Active && f.Data <= rata.DataPlataRata).OrderBy(f => f.Data).Last();
                }
                catch
                {
                 //   _date.Valoare = rata.ProcentDobanda - imprumut.MarjaFixa;
                }
                    rata.ProcentDobanda = imprumut.MarjaFixa + _date.Valoare;
                    rataLunara = (double)rata.Sold * ((double)rata.ProcentDobanda / 100 / 12 * imprumut.Periodicitate) / (1 - Math.Pow((1 + (double)rata.ProcentDobanda / 100 / 12 * imprumut.Periodicitate), -(imprumut.Durata / imprumut.Periodicitate - rata.Index)));
                    _procentDobanda = rata.ProcentDobanda / 100 / 12 * imprumut.Periodicitate;

                    rata.SumaPlatita = (decimal)rataLunara;
                    rata.SumaDobanda = rata.Sold * _procentDobanda;
                    rata.SumaPrincipal = (decimal)rataLunara - rata.Sold * _procentDobanda;
                    rata.Sold -= rata.SumaPrincipal;
                    rata.Index = i+1;
                    

                var __rata = ObjectMapper.Map<Rata>(rata);
                _RataRepository.Insert(__rata);
                CurrentUnitOfWork.SaveChanges();


            }
        }
        public void UpdateRateVariabile()
        {
            var imprumut = _ImprumutRepository.GetAll().Where(f => f.State == Models.Conta.Enums.State.Active && f.TipDobanda == TipDobanda.Variabila).ToList();

            var _imprumut = ObjectMapper.Map<List<Imprumut>>(imprumut);

            var imprumutRataFixa = _ImprumutRepository.GetAll().Where(f => f.State == Models.Conta.Enums.State.Active && f.TipDobanda == TipDobanda.Fixa).ToList();

            foreach (var value in _imprumut)
            {
                DeleteAllRateId(value.Id);
                DeleteDateComisionId(value.Id);
                CurrentUnitOfWork.SaveChanges();
                UpdateRateVariabileId(value);
                UpdateDateComisionRate(value);
           //     GenerateDobandaConta(value);
            }

            foreach(var value in imprumutRataFixa)
            {
           //     GenerateDobandaConta(value);
            }
        
        }

        //public void GenerateDobandaConta(Imprumut Imprumut)
        //{
        //    var _rate = _RataRepository.GetAll().Where(f => f.ImprumutId == Imprumut.Id && f.State == Models.Conta.Enums.State.Active && f.DataPlataRata.Month == LazyMethods.Now().Month && f.DataPlataRata.Year == LazyMethods.Now().Year && f.DataPlataRata < Imprumut.EndDate).ToList();
        //    var rateImprumut = _RataRepository.GetAll().Where(f => f.ImprumutId == Imprumut.Id && f.State == Models.Conta.Enums.State.Active).ToList();

        //    foreach (var value in _rate)
        //    {
                
        //        var nextRata = rateImprumut.Where(f => f.Index == value.Index + 1).FirstOrDefault();
        //        var dobanda = _DobandaRepository.GetAll().Where(f => f.RataId == nextRata.Id).ToList();

        //        var _newdobanda = new Dobanda()
        //        {
        //            OperationDate = LazyMethods.LastDayOfMonth(value.DataPlataRata),
        //            RataId = nextRata.Id,
        //            TenantId = value.TenantId,
        //            ValoareDobanda = Math.Round((LazyMethods.LastDayOfMonth(value.DataPlataRata) - value.DataPlataRata).Days * nextRata.SumaDobanda / (nextRata.DataPlataRata - value.DataPlataRata).Days,2),
        //            ValoarePrincipal = nextRata.SumaPrincipal,
        //        };

        //        if (dobanda.Count() == 0)
        //        {
        //            _DobandaRepository.Insert(_newdobanda);
        //            CurrentUnitOfWork.SaveChanges();
        //        }
        //        else
        //        {
        //            var _dobanda = dobanda.FirstOrDefault();
        //            _dobanda.OperationDate = _newdobanda.OperationDate;
        //            _dobanda.RataId = _newdobanda.RataId;
        //            _dobanda.ValoareDobanda = _newdobanda.ValoareDobanda;
        //            _dobanda.ValoarePrincipal = _newdobanda.ValoarePrincipal;

        //            //_DobandaRepository.Update(_dobanda);
        //            //CurrentUnitOfWork.SaveChanges();
        //        }

        //    }

        //}

        public void UpdateDateComisionRate(Imprumut imprumut)
        {
            var _comisioane = _ComisionRepository.GetAll().Where(f => f.State == Models.Conta.Enums.State.Active && f.ImprumutId == imprumut.Id).ToList();

            foreach(var comision in _comisioane) {

                


                var check = _DataComisionRepository.GetAll().Where(f => f.ComisionId == comision.Id && f.IsValid == true);
                // .OrderBy(f => f.DataPlataComision).Last().Index;
                var indexLastValid = -1;
                if (check.Count() > 0){
                      indexLastValid = _DataComisionRepository.GetAll().Where(f => f.ComisionId == comision.Id && f.IsValid == true).OrderBy(f => f.DataPlataComision).Last().Index + 1;
                }
                

                var _rateImprumut = _RataRepository.GetAll().Where(f => f.ImprumutId == comision.ImprumutId && f.State == Models.Conta.Enums.State.Active);
                var _imprumut = _ImprumutRepository.Get((int)comision.ImprumutId);
                int periodicitate = 0;
                decimal suma = 0;

                if (comision.ModCalculComision == ModCalculComision.LaAcordare)
                {
                    continue;
                }
                if (comision.ModCalculComision == ModCalculComision.Lunar)
                {
                    periodicitate = 1;
                }
                else if (comision.ModCalculComision == ModCalculComision.Trimestrial)
                {
                    periodicitate = 3;
                }
                else if (comision.ModCalculComision == ModCalculComision.Semestrial)
                {
                    periodicitate = 6;
                }
                else if (comision.ModCalculComision == ModCalculComision.Anual)
                {
                    periodicitate = 12;
                }
                int index = indexLastValid ;
                if(index > -1)
                {
                    _rateImprumut = _rateImprumut.Skip(index);
                }
                
                foreach (var rata in _rateImprumut)
                {
                    index++;
                    var dateComision = new DataComisionEditDto()
                    {
                        ImprumutId = rata.ImprumutId,
                        ComisionId = comision.Id,
                        DataPlataComision = rata.DataPlataRata,
                        Index = rata.Index,
                        IsValid = false,
                        TipValoareComision = comision.TipValoareComision,
                        ValoareComision = comision.ValoareComision,
                        SumaComision = comision.ValoareComision,
                        TipSumaComision = comision.TipSumaComision,
                    };
                    if (comision.TipValoareComision == TipValoareComision.Procent)
                    {
                        if (comision.TipSumaComision == TipSumaComision.Sold)
                        {
                            dateComision.SumaComision = rata.Sold * dateComision.ValoareComision / 100 * System.DateTime.DaysInMonth(rata.DataPlataRata.Year, rata.DataPlataRata.Month) / comision.BazaDeCalcul ;
                        }
                        else if (comision.TipSumaComision == TipSumaComision.ValoareImprumut)
                        {
                            dateComision.SumaComision = _imprumut.Suma * dateComision.ValoareComision / 100 * System.DateTime.DaysInMonth(dateComision.DataPlataComision.Year, dateComision.DataPlataComision.Month) / comision.BazaDeCalcul;
                        }
                        else if (comision.TipSumaComision == TipSumaComision.SumaTrasa)
                        {
                            dateComision.SumaComision = rata.SumaPlatita * dateComision.ValoareComision / 100 * System.DateTime.DaysInMonth(dateComision.DataPlataComision.Year, dateComision.DataPlataComision.Month) / comision.BazaDeCalcul;
                        }
                    }
                    if (index != 0 && periodicitate != 0 && index % periodicitate == 0  )
                    {
                        _DataComisionRepository.Insert(ObjectMapper.Map<DataComision>(dateComision));
                    }
                }
            }
            
        }

        public void DeleteDateComisionId(int imprumutId)
        {
            var _comisioane = _ComisionRepository.GetAll().Where(f => f.State == Models.Conta.Enums.State.Active  && f.ImprumutId == imprumutId).ToList();

            foreach(var value in _comisioane)
            {
                var _date = _DataComisionRepository.GetAll().Where(f => f.State == Models.Conta.Enums.State.Active && f.ComisionId == value.Id && f.IsValid == false);
                var ret = ObjectMapper.Map<List<DataComision>>(_date).ToList();
                ret.ForEach(data =>
                {
                    //rata.State = Models.Conta.Enums.State.Inactive;
                    _DataComisionRepository.Delete(data);
                });
            }
            
        }

        public void DeleteAllRateId(int imprumutId)
        {
            var _rate = _RataRepository.GetAll().Where(f => f.State == Models.Conta.Enums.State.Active && f.ImprumutId == imprumutId && f.IsValid == false ).ToList();
            
            _rate.ForEach(rata =>
            {
                var dobanda = _DobandaRepository.GetAll().Where(f => f.RataId == rata.Id).FirstOrDefault();
                if(dobanda != null)
                {
                    _DobandaRepository.Delete(dobanda);
                }
                //rata.State = Models.Conta.Enums.State.Inactive;
                _RataRepository.Delete(rata);
            });

        }

        public void DeleteAllRateOnEdit(int imprumutId)
        {
            var _rate = _RataRepository.GetAll().Where(f => f.State == Models.Conta.Enums.State.Active && f.ImprumutId == imprumutId).ToList();

            _rate.ForEach(rata =>
            {
                //rata.State = Models.Conta.Enums.State.Inactive;
                _RataRepository.Delete(rata);
            });

        }

        public void DeleteImprumut(int id)
        {
            var imprumut = _ImprumutRepository.GetAllIncluding(f => f.ImprumutStateList).FirstOrDefault(f => f.Id == id);
            imprumut.State = Models.Conta.Enums.State.Inactive;
            _ImprumutRepository.Update(imprumut);

            // sterg notele contabile ale imprumutului
            var contaOperationDetailIds = imprumut.ImprumutStateList.Where(f => f.ContaOperationDetailId != null).Select(f => f.ContaOperationDetailId).ToList();

            var contaOperationDetails =_operationDetailsRepository.GetAllIncluding(f => f.Operation).Where(f => contaOperationDetailIds.Contains(f.Id)).ToList();

            foreach (var det in contaOperationDetails)
            {
                var oper = _OperationsRepository.FirstOrDefault(f => f.Id == det.OperationId);
                _OperationsRepository.Delete(oper);
            }

            CurrentUnitOfWork.SaveChanges();
        }

        public List<ImprumutStateDto> ImprumutStateList(int imprumutId)
        {
            var appClient = GetCurrentTenant();
            var _imprumut = _ImprumutStateRepository.GetAll()


                                                    .Where(f => f.ImprumutId == imprumutId);


            var ret = ObjectMapper.Map<List<ImprumutStateDto>>(_imprumut).ToList();
            return ret;
        }

        public List<ImprumutStateDto> SaveImprumutStateList(ImprumutStateDto imprumutState)
        {
            try
            {
                var appClient = GetCurrentTenant();
                var _imprumutState = ObjectMapper.Map<ImprumutState>(imprumutState);
                _imprumutState.TenantId = appClient.Id;
                _ImprumutStateRepository.Insert(_imprumutState);
                CurrentUnitOfWork.SaveChanges();
            }
            catch(Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }

            return ImprumutStateList((int)imprumutState.ImprumutId);

        }

        public void DeleteAllOperationsDocNum(string imprumutDocumentNumber)
        {
            var _Operations = _OperationsRepository.GetAll().Where(f =>  f.DocumentNumber == imprumutDocumentNumber).ToList();

            _Operations.ForEach(operation =>
            {
                //rata.State = Models.Conta.Enums.State.Inactive;
                _OperationsRepository.Delete(operation);
            });
        }

        public void DeleteAllImprumutState(int imprumutId)
        {
            var state = _ImprumutStateRepository.GetAll().Where(f => f.ImprumutId == imprumutId).ToList();

            state.ForEach(state =>
            {
                //rata.State = Models.Conta.Enums.State.Inactive;
                _ImprumutStateRepository.Delete(state);
            });
        }
    }

    
}
