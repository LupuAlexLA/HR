using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Niva.Erp.Imprumuturi.Dto;
using Niva.Erp.Models.Imprumuturi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Niva.Erp.Imprumuturi
{

    public interface ITragereAppService : IApplicationService
    {
        void SaveTragere(TragereDto tragere);
        void DeleteTragere(int id);
        List<TragereDto> TragereListId(int imprumutId);
        List<DataTragereDto> DeleteDobandaByDate(DateTime Date);
        List<DataTragereDto> CalculatorDobanda(DateTime Date);

        List<DataTragereDto> DateTragereList();

        TragereDto SimulateCalculatorDobandaSiComision(DateTime Date, int ImprumutId);

    }


    public class TragereAppService : ErpAppServiceBase, ITragereAppService
    {
        IRepository<Tragere> _TragereRepository;
        IRepository<DateDobanziReferinta> _DateDobanziReferintaRepository;
        IRepository<DataComision> _DataComisionRepository;
        IRepository<Comision> _ComisionRepository;

        public TragereAppService(IRepository<Tragere> TragereRepository , IRepository<DateDobanziReferinta> DateDobanziReferintaRepository, IRepository<Comision> ComisionRepository, IRepository<DataComision> DataComisionRepository)
        {
            _DataComisionRepository = DataComisionRepository;
            _TragereRepository = TragereRepository;
            _DateDobanziReferintaRepository = DateDobanziReferintaRepository;
            _ComisionRepository = ComisionRepository;
        }

        public void SaveTragere(TragereDto tragere)
        {
            var _tragere = ObjectMapper.Map<Tragere>(tragere);
           

            if (_tragere.Id == 0)
            {

                //  int _chk = _GarantieRepository.GetAll().Where(f => f.State == Models.Conta.Enums.State.Active).Count();

                    //  if (_chk > 0) throw new UserFriendlyException("Eroare", "Termen existent!");

                


                _TragereRepository.Insert(_tragere);
            }
            else
            {
                var appClient = GetCurrentTenant();



                _tragere.TenantId = appClient.Id;
                _TragereRepository.Update(_tragere);
                CurrentUnitOfWork.SaveChanges();
            }
        }

        public void DeleteTragere(int id)
        {
            var _tragere = _TragereRepository.FirstOrDefault(f => f.Id == id);
            _tragere.State = Models.Conta.Enums.State.Inactive;
            _TragereRepository.Update(_tragere);
        }

        public List<TragereDto> TragereListId(int imprumutId)
        {
            var _tragere = _TragereRepository.GetAll().Include(e => e.Comisions).Include(e => e.Currency)
                                                .Where(f => f.ImprumutId == imprumutId && f.State == Models.Conta.Enums.State.Active)
                                                .OrderBy(f => f.DataTragere); 




            var ret = ObjectMapper.Map<List<TragereDto>>(_tragere).ToList();

            return ret;
        }

        public List<DataTragereDto> DateTragereList()
        {
            var _tragere = _TragereRepository.GetAll().Include(e => e.Currency).Include(e => e.Imprumut)
                                                .Where(f => f.TipTragere == TipTragere.Dobanda && f.State == Models.Conta.Enums.State.Active && f.Imprumut.State == Models.Conta.Enums.State.Active).ToList().GroupBy(f => f.DataTragere).Select(f => new DataTragereDto {Id = f.Last().Id, DataTragere = f.Last().DataTragere });

            return _tragere.ToList();
        }

        public List<DataTragereDto> DeleteDobandaByDate(DateTime Date)
        {
            var _tragere = _TragereRepository.GetAll().Where(f => f.TipTragere == TipTragere.Dobanda && f.DataTragere == Date).ToList();
            var _dateComision = _DataComisionRepository.GetAll().Where(f => f.DataPlataComision == Date).ToList();


            foreach(var date in _dateComision)
            {
                _DataComisionRepository.Delete(date.Id);
                CurrentUnitOfWork.SaveChanges();
            }

            foreach (var value in _tragere)
            {       
               _TragereRepository.Delete(value.Id);
                CurrentUnitOfWork.SaveChanges();
            }


            CurrentUnitOfWork.SaveChanges();
            return DateTragereList();
        }

        public List<DataTragereDto> CalculatorDobanda(DateTime Date)
        {
            DeleteDobandaByDate(Date);

            var _tragere = _TragereRepository.GetAll().Include(f => f.Imprumut).Where(f => f.TipTragere == TipTragere.Acordare || f.TipTragere == TipTragere.Tragere || f.TipTragere == TipTragere.Rambursare).OrderBy(f => f.DataTragere).ToList().GroupBy(e => e.ImprumutId).Select(g => g.Last());

            foreach (var value in _tragere)
            {
                SaveTragere(SimulateCalculatorDobandaSiComision(Date,(int)value.ImprumutId));             
            }

            CurrentUnitOfWork.SaveChanges();
            return DateTragereList();

        }

        //public List<DataTragereDto> CalculatorDobanda(DateTime Date)
        //{
        //    DeleteDobandaByDate(Date);

        //    var _tragere = _TragereRepository.GetAll().Include(f => f.Imprumut).Where(f => f.TipTragere == TipTragere.Acordare || f.TipTragere == TipTragere.Tragere || f.TipTragere == TipTragere.Rambursare).OrderBy(f => f.DataTragere).ToList().GroupBy(e => e.ImprumutId).Select(g => g.Last());



        //    foreach (var value in _tragere)
        //    {
        //        var _date = _DateDobanziReferintaRepository.GetAll()
        //                                           .Include(e => e.DobanziReferinta)
        //                                           .Where(f => f.DobanziReferintaId == value.Imprumut.DobanziReferintaId && f.State == Models.Conta.Enums.State.Active && f.Data <= Date).OrderBy(f => f.Data).LastOrDefault();
        //        var _comision = _ComisionRepository.GetAll()
        //                                        .Where(f => f.ImprumutId == value.ImprumutId && f.State == Models.Conta.Enums.State.Active && f.TipComision == TipComision.Neutilizare).FirstOrDefault();

        //        if (value.Imprumut.TipDobanda == TipDobanda.Variabila)
        //        {
        //            if (Date > value.Imprumut.StartDate)
        //            {
        //                if (_date != null && _comision != null)
        //                {
        //                    // this.tragere.sumaDisponibila * comNeut.valoareComision / 100 * this.tragere.dataTragere.daysInMonth() / comNeut.bazaDeCalcul
        //                    var _newTragere = new Tragere()
        //                    {
        //                        Id = 0,
        //                        CurrencyId = value.CurrencyId,
        //                        DataTragere = Date,
        //                        SumaDisponibila = value.SumaDisponibila,
        //                        SumaImprumutata = value.SumaImprumutata,
        //                        TipTragere = TipTragere.Dobanda,
        //                        Dobanda = Math.Round(value.SumaImprumutata * (value.Imprumut.MarjaFixa + _date.Valoare) / 100 * DateTime.DaysInMonth(Date.Year, Date.Month) / 365, 2),
        //                        Comision = Math.Round(value.SumaDisponibila * _comision.ValoareComision / 100 * DateTime.DaysInMonth(Date.Year, Date.Month) / _comision.BazaDeCalcul, 2),
        //                        TenantId = value.TenantId,
        //                        ImprumutId = value.ImprumutId,
        //                    };

        //                    _TragereRepository.Insert(_newTragere);
        //                }
        //                else
        //                {
        //                    throw new UserFriendlyException("Eroare", "DOBANZI DE REFERINTA/COMISIOANE NEACTUALIZATE!");
        //                }
        //            }



        //        }
        //        else
        //        {
        //            if (Date > value.Imprumut.StartDate)
        //            {
        //                if (_comision != null)
        //                {

        //                    var _newTragere = new Tragere()
        //                    {
        //                        Id = 0,
        //                        CurrencyId = value.CurrencyId,
        //                        DataTragere = Date,
        //                        SumaDisponibila = value.SumaDisponibila,
        //                        SumaImprumutata = value.SumaImprumutata,
        //                        TipTragere = TipTragere.Dobanda,
        //                        Dobanda = Math.Round(value.SumaImprumutata * value.Imprumut.ProcentDobanda / 100 * DateTime.DaysInMonth(Date.Year, Date.Month) / 365, 2),
        //                        Comision = Math.Round(value.SumaDisponibila * _comision.ValoareComision / 100 * DateTime.DaysInMonth(Date.Year, Date.Month) / _comision.BazaDeCalcul, 2),
        //                        TenantId = value.TenantId,
        //                        ImprumutId = value.ImprumutId,

        //                    };

        //                    _TragereRepository.Insert(_newTragere);
        //                }
        //                else
        //                {
        //                    throw new UserFriendlyException("Eroare", "COMISIOANE NEDEFINITE!");
        //                }
        //            }
        //        }

        //    }
        //    CurrentUnitOfWork.SaveChanges();
        //    return DateTragereList();

        //}

        public TragereDto SimulateCalculatorDobandaSiComision(DateTime Date,int ImprumutId)
        {
            var _tragere = _TragereRepository.GetAll().Include(f => f.Imprumut).Where(f => (f.TipTragere == TipTragere.Acordare || f.TipTragere == TipTragere.Tragere || f.TipTragere == TipTragere.Rambursare) && f.ImprumutId == ImprumutId).OrderBy(f => f.DataTragere).LastOrDefault();
            _tragere.Comision = 0;
            var _newTragere = new Tragere() { CurrencyId = _tragere.CurrencyId,TipTragere = TipTragere.Dobanda,ImprumutId = ImprumutId,DataTragere = Date,SumaDisponibila = _tragere.SumaDisponibila,SumaImprumutata = _tragere.SumaImprumutata } ;
            var _comisioane = _ComisionRepository.GetAll()
                                                .Where(f => f.ImprumutId == ImprumutId && f.State == Models.Conta.Enums.State.Active).ToList();
            _newTragere.Comisions = new List<DataComision>();
            foreach (var value in _comisioane)
            {
                
                if(value.TipComision != TipComision.Acordare)
                {
                    

                    var _dateComision = new DataComision()
                    {
                        ComisionId = value.Id,
                        DataPlataComision = Date,
                        ImprumutId = ImprumutId,
                        SumaComision = calculComision(value,_tragere,Date),
                        TipSumaComision = value.TipSumaComision,
                        TipValoareComision = value.TipValoareComision,
                        TragereId = 0,
                        ValoareComision = value.ValoareComision,

                    };

                    _newTragere.Comisions.Add(_dateComision);
                }
               // _newTragere.Comision += calculComision; calculata de getter din model tragereDto

                
            }

            //    _newTragere.Dobanda = Math.Round(_tragere.SumaImprumutata * _tragere.Imprumut.ProcentDobanda / 100 * DateTime.DaysInMonth(Date.Year, Date.Month) / 365, 2);
            _newTragere.Dobanda = Math.Round(_tragere.SumaImprumutata * getDobanda(_tragere,Date) / 100 * DateTime.DaysInMonth(Date.Year, Date.Month) / 365, 2);

            var ret = ObjectMapper.Map<TragereDto>(_newTragere);

            return ret;

        }

        public decimal calculComision(Comision value, Tragere _tragere , DateTime Date)
        {
            //Calculeaza comisionul in methoda SimulateCalculatorDobandaSiComision
            decimal calculComision = value.ValoareComision;

            if (value.TipValoareComision == TipValoareComision.Procent)
            {

                switch (value.TipSumaComision)
                {
                    case TipSumaComision.ValoareImprumut:
                        calculComision = Math.Round(_tragere.Imprumut.Suma * value.ValoareComision / 100 * DateTime.DaysInMonth(Date.Year, Date.Month) / value.BazaDeCalcul, 2);
                        break;
                    case TipSumaComision.SumaTrasa:
                        calculComision = Math.Round(_tragere.SumaImprumutata * value.ValoareComision / 100 * DateTime.DaysInMonth(Date.Year, Date.Month) / value.BazaDeCalcul, 2);
                        break;
                    case TipSumaComision.Sold:
                        calculComision = Math.Round(_tragere.SumaDisponibila * value.ValoareComision / 100 * DateTime.DaysInMonth(Date.Year, Date.Month) / value.BazaDeCalcul, 2);
                        break;
                }
                


            }
            

            return calculComision;
        }

        private decimal getDobanda(Tragere tragere, DateTime Date)
        {
            // Obtine Dobanda variabila pentru creditele cu dobanda variabila in methoda SimulateCalculatorDobandaSiComision
            var _date = new DateDobanziReferinta();
            decimal dobandaVariabila = tragere.Imprumut.ProcentDobanda;

            if (tragere.Imprumut.TipDobanda == TipDobanda.Variabila)
            {
                try
                {
                   _date = _DateDobanziReferintaRepository.GetAll()
                                                       .Include(e => e.DobanziReferinta)
                                                       .Where(f => f.DobanziReferintaId == tragere.Imprumut.DobanziReferintaId && f.State == Models.Conta.Enums.State.Active && f.Data <= Date).OrderBy(f => f.Data).Last();
                }
                catch
                {
                    throw new UserFriendlyException("Eroare", "Dobanda de referinta neactualizata!");
                }
                dobandaVariabila = tragere.Imprumut.MarjaFixa + _date.Valoare;
            }
            
            return dobandaVariabila;
        }
        
    }
}
