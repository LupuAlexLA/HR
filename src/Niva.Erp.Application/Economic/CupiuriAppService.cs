using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.Economic.Dto;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Economic.Casierii;
using Niva.Erp.Models.Economic.Casierii.Cupiuri;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Economic
{
    public interface ICupiuriAppService : IApplicationService
    {
        List<CupiuriListDto> InitList();
        List<CupiuriListDto> SearchCupiuri(DateTime operationDate);
        CupiuriForm CupiuriFormInit(int cupiuriId);
        CupiuriForm AddDetailsRow(CupiuriForm form, int index);
        CupiuriForm AddRow(CupiuriForm form);
        void Delete(int cupiuriId);
        void Save(CupiuriForm form);
        CupiuriForm UpdateTotal(CupiuriForm form, int rowIndex);

        decimal SoldForCurrencyId(int currencyId, DateTime operationDate);
    }
    public class CupiuriAppService : ErpAppServiceBase, ICupiuriAppService
    {
        IRepository<CupiuriInit> _cupiuriInitRepository;
        IRepository<CupiuriItem> _cupiuriItemRepository;
        IRepository<CupiuriDetails> _cupiuriDetailsRepository;
        IRepository<Disposition> _dispositionRepository;
        IRepository<Currency> _currencyRepository;

        public CupiuriAppService(IRepository<CupiuriItem> cupiuriItemRepository, IRepository<CupiuriInit> cupiuriInitRepository,
             IRepository<Disposition> dispositionRepository, IRepository<Currency> currencyRepository, IRepository<CupiuriDetails> cupiuriDetailsRepository)
        {
            _cupiuriInitRepository = cupiuriInitRepository;
            _cupiuriItemRepository = cupiuriItemRepository;
            _dispositionRepository = dispositionRepository;
            _currencyRepository = currencyRepository;
            _cupiuriDetailsRepository = cupiuriDetailsRepository;
        }

        /// <summary>
        /// Adaugare rand in tabela CupiuriDetails
        /// </summary>
        /// <param name="form"></param>
        /// <param name="index">numarul randului din lista Cupiurilor </param>
        /// <returns></returns>

        [AbpAuthorize("Casierie.Numerar.Cupiuri.Modificare")]
        public CupiuriForm AddDetailsRow(CupiuriForm form, int index)
        {
            try
            {
                var details = new CupiuriDetailsDto
                {
                    Quantity = null
                };

                form.Cupiuri[index].CupiuriDetails.Add(details);

                return form;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        //Adaugare rand nou pentru moneda
        [AbpAuthorize("Casierie.Numerar.Cupiuri.Modificare")]
        public CupiuriForm AddRow(CupiuriForm form)
        {
            try
            {
                var cupiuri = new CupiuriItemDto
                {
                    CurrencyId = null,
                    CupiuriDetails = new List<CupiuriDetailsDto>()
                };

                cupiuri = CupiuriDetailsInit(cupiuri);
                form.Cupiuri.Add(cupiuri);
                return form;
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        //Stergere cupiuri la o anumita data
        [AbpAuthorize("Casierie.Numerar.Cupiuri.Modificare")]
        public void Delete(int cupiuriId)
        {
            try
            {
                var cupiuraDetails = _cupiuriItemRepository.GetAllIncluding(f => f.CupiuriDetails).Where(f => f.CupiuriInitId == cupiuriId).ToList();

                foreach(var det in cupiuraDetails)
                {
                    foreach(var x in det.CupiuriDetails)
                    {
                        _cupiuriDetailsRepository.Delete(x.Id);
                    }
                    _cupiuriItemRepository.Delete(det.Id);
                }

                var item = _cupiuriInitRepository.Get(cupiuriId);
                _cupiuriInitRepository.Delete(item);
                
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        //[AbpAuthorize("Casierie.Numerar.Cupiuri.Acces")]
        public CupiuriForm CupiuriFormInit(int cupiuriId)
        {
            CupiuriForm ret;
            var cupiuiInit = _cupiuriInitRepository.FirstOrDefault(f => f.Id == cupiuriId);

            if (cupiuiInit == null)
            {
                ret = new CupiuriForm
                {
                    OperationDate = DateTime.Now
                };
            }
            else
            {
                ret = new CupiuriForm
                {
                    OperationDate = cupiuiInit.OperationDate,
                    CupiuriInitId = cupiuiInit.Id
                };
            }

            ret = CupiuriFormList(ret);
            return ret;
        }

        //[AbpAuthorize("Casierie.Numerar.Cupiuri.Acces")]
        private CupiuriForm CupiuriFormList(CupiuriForm form)
        {
            var list = _cupiuriItemRepository.GetAllIncluding(f => f.CupiuriDetails, f => f.Currency).Where(f => f.CupiuriInitId == form.CupiuriInitId && f.State == State.Active);
            var _list = ObjectMapper.Map<List<CupiuriItemDto>>(list);

            foreach (var item in _list)
            {
                var sold = _dispositionRepository.GetAllIncluding(f => f.Currency).Where(f => f.State == State.Active &&
                                                                                         f.OperationType == OperationType.SoldInitial &&
                                                                                         f.DispositionDate >= form.OperationDate && f.CurrencyId == item.CurrencyId).FirstOrDefault();
                item.Sold = sold?.Value ?? 0;
                item.Total = (decimal)item.CupiuriDetails.Where(f => f.State == State.Active).Sum(f => f.Value * f.Quantity);
            }

            form.Cupiuri = _list;
            form = AddRow(form);

            return form;
        }

        //[AbpAuthorize("Casierie.Numerar.SoldInitial.Acces")]
        public List<CupiuriListDto> InitList()
        {
            try
            {
                var ret = new List<CupiuriListDto>();
                var list = new CupiuriListDto
                {
                    OperationDate = DateTime.Now
                };

                ret = SearchCupiuri(list.OperationDate);

                return ret;
            }
            catch (System.Exception ex)
            {

                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        [AbpAuthorize("Casierie.Numerar.Cupiuri.Modificare")]
        public void Save(CupiuriForm form)
        {
            try
            {
                //ValidateSold(form);
                var oldList = _cupiuriItemRepository.GetAll().Where(f => f.CupiuriInitId == form.CupiuriInitId);

                foreach (var item in oldList)
                {
                    _cupiuriItemRepository.Delete(item);
                }

                if (oldList.Count() == 0)
                {
                    form.CupiuriInitId = AddCupiuriInit(form.OperationDate);
                }

                CurrentUnitOfWork.SaveChanges();

                foreach (var item in form.Cupiuri)
                {

                    var _item = ObjectMapper.Map<CupiuriItem>(item);
                    _item.CupiuriInitId = form.CupiuriInitId;
                    _cupiuriItemRepository.Insert(_item);

                }
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        [AbpAuthorize("Casierie.Numerar.Cupiuri.Acces")]
        public List<CupiuriListDto> SearchCupiuri(DateTime operationDate)
        {
            var date = new DateTime(operationDate.Year, operationDate.Month, operationDate.Day);
            var list = _cupiuriInitRepository.GetAll().Where(f => f.State == State.Active && f.OperationDate == date).ToList();

            var ret = ObjectMapper.Map<List<CupiuriListDto>>(list);
            return ret;
        }

        /// <summary>
        /// Actualizez valoarea totala pentru moneda selectata la adugare/stergere/editare rand
        /// </summary>
        /// <param name="form"></param>
        /// <param name="rowIndex"> rand pentru clasa CupiuriDetails</param>
        /// <returns></returns>
        [AbpAuthorize("Casierie.Numerar.Cupiuri.Modificare")]
        public CupiuriForm UpdateTotal(CupiuriForm form, int rowIndex)
        {
            form.Cupiuri[rowIndex].Total = (decimal)form.Cupiuri[rowIndex].CupiuriDetails.Where(f => f.State == State.Active).Sum(f => f.Value * f.Quantity);
            return form;
        }

        public decimal SoldForCurrencyId(int currencyId, DateTime operationDate)
        {
            try
            {

                var sold = _dispositionRepository.GetAllIncluding(f => f.Currency).Where(f => f.State == State.Active &&
                                                                                         f.OperationType == OperationType.SoldInitial &&
                                                                                         f.DispositionDate >= operationDate && f.CurrencyId == currencyId).FirstOrDefault();

                return sold == null ? 0 : sold.Value;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        //[AbpAuthorize("Casierie.Numerar.Cupiuri.Acces")]
        private int AddCupiuriInit(DateTime operationDate)
        {
            var appClientId = GetCurrentTenant().Id;
            var cupiuriniInit = new CupiuriInit
            {
                OperationDate = operationDate,
                TenantId = appClientId
            };

            _cupiuriInitRepository.Insert(cupiuriniInit);
            CurrentUnitOfWork.SaveChanges();
            return cupiuriniInit.Id;
        }

        [AbpAuthorize("Casierie.Numerar.Cupiuri.Modificare")]
        private void ValidateSold(CupiuriForm form)
        {
            foreach (var item in form.Cupiuri)
            {
                var sold = _dispositionRepository.GetAllIncluding(f => f.Currency).Where(f => f.State == State.Active &&
                                                                                          f.OperationType == OperationType.SoldInitial &&
                                                                                          f.DispositionDate >= form.OperationDate && f.CurrencyId == item.CurrencyId).FirstOrDefault();
                var currencyName = _currencyRepository.GetAll().FirstOrDefault(f => f.Id == item.CurrencyId && f.Status == State.Active).CurrencyName;

                if (sold == null) throw new Exception("Nu a fost definit soldul pentru moneda " + currencyName + " la data " + LazyMethods.DateToString(form.OperationDate));

                if (item.Total != sold.Value) throw new Exception("Valorea cupiurilor pentru moneda " + currencyName + " este diferita de soldul  " + sold.Value + " la data " + LazyMethods.DateToString(form.OperationDate));
            }
        }

        //[AbpAuthorize("Casierie.Numerar.Cupiuri.Acces")]
        private CupiuriItemDto CupiuriDetailsInit(CupiuriItemDto cupiuri)
        {
            var cupiuriDetails = new CupiuriDetailsDto
            {
                Quantity = null
            };

            cupiuri.CupiuriDetails.Add(cupiuriDetails);
            return cupiuri;
        }
    }
}
