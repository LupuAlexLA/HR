using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Niva.Erp.Imprumuturi.Dto;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Imprumuturi;
using Niva.Erp.Repositories.Conta.AutoOperation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Niva.Erp.Imprumuturi
{
    public interface IComisioaneAppService : IApplicationService
    {
        List<ComisionDto> ComisionList();

        List<ComisionDto> ComisionListId(int imprumutId);

        ComisionEditDto GetComisionId(int id);
        ComisionDto ComisionId(int id);
        void SaveComision(ComisionEditDto comision);

        void DeleteComision(int id);

        void SaveComisionLinieDeCredit(ComisionEditDto comision);
        void SaveDataComisionNeutilizare(DataComisionEditDto dataComision);
        ComisionEditDto ComisionEditNeutilizatId(int imprumutId);
        List<ComisionV2Dto> ComisionV2List();
        void DeleteComisionV2(int id);
        List<ComisionV2Dto> ComisionV2ListId(int imprumutId);
        ComisionV2EditDto GetComisionV2Id(int id);
    }
    public class ComisioaneAppService : ErpAppServiceBase, IComisioaneAppService
    {
        IRepository<Comision> _comisionRepository;
        IRepository<ComisionV2> _comisionV2Repository;
        IRepository<DataComision> _DataComisionRepository;
        IRepository<Imprumut> _ImprumutRepository;
        IRepository<Rata> _RataRepository;
        IRepository<Tragere> _TragereRepository;
        IRepository<Imprumut> _imprumutRepository;
        IRepository<Account> _accountRepository;
        IRepository<Balance> _balanceRepository;
        IRepository<BalanceDetails> _balanceDetailsRepository;
        IRepository<OperationDetails> _operationDetailsRepository;
        IAutoOperationRepository _autoOperationRepository;
       

        public ComisioaneAppService(IRepository<OperationDetails> operationDetailsRepository, IRepository<BalanceDetails> balanceDetailsRepository, IRepository<Balance> balanceRepository, IRepository<Account> accountRepository,IRepository<Imprumut> imprumutRepository, IRepository<ComisionV2> comisionV2Repository, IAutoOperationRepository autoOperationRepository, IRepository<Comision> ComisionRepository , IRepository<DataComision> DataComisionRepository, IRepository<Imprumut> ImprumutRepository, IRepository<Rata> RataRepository, IRepository<Tragere> TragereRepository)
        {
            _operationDetailsRepository = operationDetailsRepository;
            _balanceRepository = balanceRepository;
            _balanceDetailsRepository = balanceDetailsRepository;
            _accountRepository = accountRepository;
            _imprumutRepository = imprumutRepository;
            _comisionV2Repository = comisionV2Repository;
            _autoOperationRepository = autoOperationRepository;
            _TragereRepository = TragereRepository;
            _comisionRepository = ComisionRepository;
            _DataComisionRepository = DataComisionRepository;
            _ImprumutRepository = ImprumutRepository;
            _RataRepository = RataRepository;
    }

        public List<ComisionDto> ComisionListId(int imprumutId)
        {
            var _comision = _comisionRepository.GetAll()     
                                                    .Where(f => f.ImprumutId == imprumutId && f.State == Models.Conta.Enums.State.Active);
            // 
            //


            var ret = ObjectMapper.Map<List<ComisionDto>>(_comision).ToList();

            return ret;
        }

        public List<ComisionV2Dto> ComisionV2ListId(int imprumutId)
        {
            var _comision = _comisionV2Repository.GetAll()
                                                    .Where(f => f.ImprumutId == imprumutId && f.State == Models.Conta.Enums.State.Active);
            // 
            //


            var ret = ObjectMapper.Map<List<ComisionV2Dto>>(_comision).ToList();

            return ret;
        }

        public ComisionEditDto ComisionEditNeutilizatId(int imprumutId)
        {
            var _comision = _comisionRepository.GetAll()
                                                    .Where(f => f.ImprumutId == imprumutId && f.State == Models.Conta.Enums.State.Active && f.TipComision == TipComision.Neutilizare).FirstOrDefault();
            // 
            //


            var ret = ObjectMapper.Map<ComisionEditDto>(_comision);

            return ret;
        }

        public List<ComisionDto> ComisionList()
        {
            var _comision = _comisionRepository.GetAll()
                                                    
                                                    .Where(f => f.State == Models.Conta.Enums.State.Active);
            


            var ret = ObjectMapper.Map<List<ComisionDto>>(_comision).ToList();
            return ret;
        }

        public List<ComisionV2Dto> ComisionV2List()
        {
            var _comision = _comisionV2Repository.GetAll()
                                                    .Where(f => f.State == Models.Conta.Enums.State.Active);



            var ret = ObjectMapper.Map<List<ComisionV2Dto>>(_comision).ToList();
            return ret;
        }
        public ComisionDto ComisionId(int id)
        {
            var _comision = _comisionRepository.GetAll()
                                                        .FirstOrDefault(e => e.Id == id);



            var ret = ObjectMapper.Map<ComisionDto>(_comision);
            return ret;
        }

        public ComisionEditDto GetComisionId(int id)
        {
            var ret = _comisionRepository.Get(id);
            return ObjectMapper.Map<ComisionEditDto>(ret);
        }

        public ComisionV2EditDto GetComisionV2Id(int id)
        {
            var ret = _comisionV2Repository.Get(id);
            return ObjectMapper.Map<ComisionV2EditDto>(ret);
        }

        public void SaveComision(ComisionEditDto comision)
        {
            var _comision = ObjectMapper.Map<Comision>(comision);


            if (_comision.Id == 0)
            {

                //  int _chk = _GarantieRepository.GetAll().Where(f => f.State == Models.Conta.Enums.State.Active).Count();

                //  if (_chk > 0) throw new UserFriendlyException("Eroare", "Termen existent!");

                _comisionRepository.InsertAndGetId(_comision);
                CurrentUnitOfWork.SaveChanges();

                var __comision = ObjectMapper.Map<ComisionEditDto>(_comision);

                if(__comision.ModCalculComision == ModCalculComision.LaAcordare)
                {
                    GenerateDateComisionLaAcordare(ObjectMapper.Map<Comision>(__comision));
                }
                else
                {
                    GenerateDateComisionPeriodic(ObjectMapper.Map<Comision>(__comision));
                }
            }
            else
            {
                var appClient = GetCurrentTenant();
                _comision.TenantId = appClient.Id;
                _comisionRepository.Update(_comision);
            }


        }

        public void SaveComisionV2(ComisionV2EditDto comision)
        {
            var _comision = ObjectMapper.Map<ComisionV2>(comision);


            if (_comision.Id == 0)
            {

                //  int _chk = _GarantieRepository.GetAll().Where(f => f.State == Models.Conta.Enums.State.Active).Count();

                //  if (_chk > 0) throw new UserFriendlyException("Eroare", "Termen existent!");


                if(_comision.TipComision == TipComisionV2.cheltuialaInAvans)
                {
                    _comision.NrLuni = Math.Abs(12 * (_comision.DataStart.Year - _comision.DataEnd.Year) + _comision.DataStart.Month - _comision.DataEnd.Month);

                    if(_comision.TipValoareComision == TipValoareComision.ValoareFixa)
                    {

                        _comision.ValoareCalculata = Math.Round(_comision.ValoareComision / _comision.NrLuni, 2);

                    }
                    else if(_comision.TipValoareComision == TipValoareComision.Procent)
                    {


                        var imprumut = _imprumutRepository.Get((int)_comision.ImprumutId);

                        //var count = _balanceRepository.GetAll().ToList().Count(f => f.Status == State.Active && f.BalanceDate.Month == Operatie.DataEnd.Month && f.BalanceDate.Year == Operatie.DataEnd.Year);
                        //if (count != 0)
                        //{
                        //    throw new Exception("Nu se poate inregistra operatia deoarece luna contabila este inchisa");
                        //}

                        var debitId = _accountRepository.FirstOrDefault(f => f.Status == State.Active && f.Symbol == "999").Id;
                        var contImprumut = _accountRepository.GetAll().ToList().FirstOrDefault(f => f.Status == State.Active && f.Id == imprumut.ContContabilId).Id;
                        //    var lastBalance = Context.BalanceDetails.Include(f => f.Account).Include(f => f.Balance).OrderByDescending(f => f.Balance.BalanceDate).FirstOrDefault(f => f.Account.Symbol == imprumut.ContContabil); // cea mai recenta balanta asociata contului
                        var lastBalance = _balanceRepository.GetAll().ToList().Where(f => f.Status == State.Active && f.BalanceDate <= _comision.DataStart).OrderByDescending(f => f.BalanceDate).FirstOrDefault(); // cea mai recenta balanta

                        var soldInitialItem = _balanceDetailsRepository.GetAll().ToList().FirstOrDefault(f => f.AccountId == contImprumut && f.BalanceId == lastBalance.Id);
                        decimal sumaImprumutataLastBalance = 0;
                        if (soldInitialItem != null)
                        {
                            sumaImprumutataLastBalance = soldInitialItem.CrValueF - soldInitialItem.DbValueF;
                        }

                        var contaOperationTragere = _operationDetailsRepository.GetAllIncluding(f => f.Operation).ToList().Where(f => f.Operation.OperationDate > lastBalance.BalanceDate && f.Operation.OperationDate <= _comision.DataStart && f.CreditId == contImprumut && f.Operation.State == State.Active).ToList(); // de adaugat al cui imprumut este operatia contabila
                        var contaOperationRambursare = _operationDetailsRepository.GetAllIncluding(f => f.Operation).ToList().Where(f => f.Operation.OperationDate > lastBalance.BalanceDate && f.Operation.OperationDate <= _comision.DataStart && f.DebitId == contImprumut && f.Operation.State == State.Active).ToList();


                        decimal calculComision = 0;

                        decimal soldDisponibil = imprumut.Suma - sumaImprumutataLastBalance;
                        decimal valoareImprumutata = sumaImprumutataLastBalance;

                        var firstDayOperation = _comision.DataStart;

                        var contaOperationTragereToFirstDay = contaOperationTragere.Where(f => f.Operation.OperationDate < firstDayOperation);
                        var contaOperationRambursareToFirstDay = contaOperationRambursare.Where(f => f.Operation.OperationDate < firstDayOperation);


                        soldDisponibil = soldDisponibil - contaOperationTragereToFirstDay.Sum(f => f.Value);

                        soldDisponibil = soldDisponibil + contaOperationRambursareToFirstDay.Sum(f => f.Value);

                        valoareImprumutata = imprumut.Suma - soldDisponibil;


                        if(_comision.TipSumaComision == TipSumaComision.ValoareImprumut)
                        {
                            _comision.ValoareCalculata = Math.Round((imprumut.Suma * _comision.ValoareComision / 100 / _comision.BazaDeCalcul) / _comision.NrLuni, 2);
                        }
                        else if(_comision.TipSumaComision == TipSumaComision.Sold)
                        {
                            _comision.ValoareCalculata = Math.Round((soldDisponibil * _comision.ValoareComision / 100 / _comision.BazaDeCalcul) / _comision.NrLuni, 2);
                        }
                        else if (_comision.TipSumaComision == TipSumaComision.SumaTrasa)
                        {
                            _comision.ValoareCalculata = Math.Round((valoareImprumutata * _comision.ValoareComision / 100 / _comision.BazaDeCalcul) / _comision.NrLuni, 2);
                        }


                    }
                    
                }

                _comisionV2Repository.InsertAndGetId(_comision);
                CurrentUnitOfWork.SaveChanges();

                
            }
            else
            {
                var appClient = GetCurrentTenant();
                _comision.TenantId = appClient.Id;
                _comisionV2Repository.Update(_comision);
            }


        }

        public void SaveComisionLinieDeCredit(ComisionEditDto comision)
        {
            var _comision = ObjectMapper.Map<Comision>(comision);
            var _imprumut = _ImprumutRepository.Get((int)comision.ImprumutId);

            if (_comision.Id == 0)
            {

                //  int _chk = _GarantieRepository.GetAll().Where(f => f.State == Models.Conta.Enums.State.Active).Count();

                //  if (_chk > 0) throw new UserFriendlyException("Eroare", "Termen existent!");

                
                if(_comision.TipComision != TipComision.Acordare)
                {
                    _comision.ModCalculComision = ModCalculComision.Tragere;
                }

                _comisionRepository.InsertAndGetId(_comision);
                CurrentUnitOfWork.SaveChanges();

                //  this.tragere.sumaDisponibila * comNeut.valoareComision / 100 * this.tragere.dataTragere.daysInMonth() / comNeut.bazaDeCalcul;

                if (_comision.TipComision == TipComision.Acordare)
                {
                  
                    decimal sumaComision = 0;

                    if (_comision.TipValoareComision == TipValoareComision.Procent)
                    {
                      sumaComision = _imprumut.Suma * comision.ValoareComision / 100 /comision.BazaDeCalcul; // de revizuit
                    }
                    else
                    {
                        sumaComision = comision.ValoareComision;
                    }
                        



                    var dataComision = new DataComision()
                    {
                        ImprumutId = comision.ImprumutId,
                        ComisionId = _comision.Id,
                        DataPlataComision = _imprumut.DocumentDate,
                        IsValid = false,
                        TipValoareComision = comision.TipValoareComision,
                        ValoareComision = comision.ValoareComision,
                        SumaComision = sumaComision,
                        TipSumaComision = comision.TipSumaComision,
                        
                    };
                    _DataComisionRepository.Insert(dataComision);
                    CurrentUnitOfWork.SaveChanges();

                    int localCurrencyId = GetCurrentTenant().LocalCurrencyId.Value;
                    _autoOperationRepository.ComisionToConta(dataComision.Id, localCurrencyId);
                }
                
            }
            else
            {
                var appClient = GetCurrentTenant();
                _comision.TenantId = appClient.Id;
                _comisionRepository.Update(_comision);
            }
        }




        public void SaveDataComisionNeutilizare(DataComisionEditDto dataComision)
        {
            _DataComisionRepository.Insert(ObjectMapper.Map<DataComision>(dataComision));
        }

            public void GenerateDateComisionPeriodic(Comision comision)
        {
            var _rateImprumut = _RataRepository.GetAll().Where(f => f.ImprumutId == comision.ImprumutId && f.State == Models.Conta.Enums.State.Active);
            var _imprumut = _ImprumutRepository.Get((int)comision.ImprumutId);
            int periodicitate = 0;
            decimal suma = 0;
            if(comision.TipSumaComision == TipSumaComision.ValoareImprumut)
            {
                suma = _imprumut.Suma;
            }
            if(comision.ModCalculComision == ModCalculComision.Lunar)
            {
                periodicitate = 1;
            }
            else if (comision.ModCalculComision == ModCalculComision.Trimestrial)
            {
                periodicitate = 3;
            }
            else if(comision.ModCalculComision == ModCalculComision.Semestrial)
            {
                periodicitate = 6;
            }
            else if(comision.ModCalculComision == ModCalculComision.Anual)
            {
                periodicitate = 12;
            }
            int index = -1;
            foreach(var rata in _rateImprumut)
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
                if(comision.TipValoareComision == TipValoareComision.Procent)
                {
                    if (comision.TipSumaComision == TipSumaComision.Sold)
                    {
                        dateComision.SumaComision = rata.Sold * dateComision.ValoareComision / 100 * System.DateTime.DaysInMonth(rata.DataPlataRata.Year, rata.DataPlataRata.Month) / comision.BazaDeCalcul;
                    }
                    else if (comision.TipSumaComision == TipSumaComision.ValoareImprumut)
                    {
                        dateComision.SumaComision = _imprumut.Suma * dateComision.ValoareComision / 100 * System.DateTime.DaysInMonth(dateComision.DataPlataComision.Year, dateComision.DataPlataComision.Month) / comision.BazaDeCalcul;
                    }
                }
                if(index % periodicitate == 0 && index != 0)
                {
                    _DataComisionRepository.Insert(ObjectMapper.Map<DataComision>(dateComision));
                }
            }
        }

        public void GenerateDateComisionLaAcordare(Comision comision)
        {
            var _rateImprumut = _RataRepository.GetAll().Where(f => f.ImprumutId == comision.ImprumutId && f.State == Models.Conta.Enums.State.Active);
            var _imprumut = _ImprumutRepository.Get((int)comision.ImprumutId);

            var dateComision = new DataComisionEditDto()
            {
                ImprumutId = comision.ImprumutId,
                ComisionId = comision.Id,
                DataPlataComision = _imprumut.StartDate,
                Index = 0,
                IsValid = false,
                TipValoareComision = comision.TipValoareComision,
                ValoareComision = comision.ValoareComision,
                SumaComision = comision.ValoareComision,
                TipSumaComision = comision.TipSumaComision,
            };
            if(dateComision.TipValoareComision == TipValoareComision.Procent)
            {
                dateComision.SumaComision = _imprumut.Suma * comision.ValoareComision / 100;
            }

            _DataComisionRepository.Insert(ObjectMapper.Map<DataComision>(dateComision));
            
        }
        
        

        public void DeleteComision(int id)
        {
            var _comision = _comisionRepository.GetAll().FirstOrDefault(f => f.Id == id);
            _comision.State = Models.Conta.Enums.State.Inactive;
            _comisionRepository.Update(_comision);
        }

        public void DeleteComisionV2(int id)
        {
            var _comision = _comisionV2Repository.GetAll().FirstOrDefault(f => f.Id == id);
            _comision.State = Models.Conta.Enums.State.Inactive;
            _comisionV2Repository.Update(_comision);
        }
    }
}
