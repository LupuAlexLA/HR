using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Niva.Erp.Buget.Dto;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Models.Buget;
using Niva.Erp.Repositories.Buget;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Buget
{

    public interface IBugetConfigAppService : IApplicationService
    {
        List<BugetConfigDto> BugetConfigList();
        BugetConfigEditDto GetBugetConfigId(int id);
        void SaveBugetConfig(BugetConfigEditDto storage);
        void DeleteBugetConfig(int id);
        BugetForm BugetRandInit(int FormularId);
        BugetForm BugetRandList(BugetForm form);
        BugetForm BugetAddRow(BugetForm form);
        BugetForm BugetRandSave(BugetForm form);
        BugetForm BugetRandSave2(BugetForm form);
        BugetForm BugetOrderRow(BugetForm form);
        void BugetClonare(int formularId, int an);

    }

    public class BugetConfigAppService : ErpAppServiceBase, IBugetConfigAppService
    {
        IRepository<BVC_Formular> _BugetConfigRepository;
        IRepository<BVC_FormRand> _BugetFormRandRepository;
        IRepository<BVC_FormRandDetails> _BugetFormRandDetailRepository;
        IRepository<BVC_PAAPTranse> _bvcPaapTranseRepository;
        IBVC_PaapRepository _bvcPaapRepository;

        public BugetConfigAppService(IRepository<BVC_Formular> BugetConfigRepository, IRepository<BVC_FormRand> BugetFormRandRepository, IRepository<BVC_FormRandDetails> BugetFormRandDetailRepository,
                    IRepository<BVC_PAAPTranse> bvcPaapTranseRepository, IBVC_PaapRepository bvcPaapRepository)
        {
            _BugetConfigRepository = BugetConfigRepository;
            _BugetFormRandRepository = BugetFormRandRepository;
            _BugetFormRandDetailRepository = BugetFormRandDetailRepository;
            _bvcPaapTranseRepository = bvcPaapTranseRepository;
            _bvcPaapRepository = bvcPaapRepository;
        }

        public List<BugetConfigDto> BugetConfigList()
        {
            try
            {
                var _bugetConfig = _BugetConfigRepository.GetAll()
                                                        .Where(f => f.State == Models.Conta.Enums.State.Active).OrderByDescending(f => f.AnBVC)
                                                        .ToList();


                var ret = ObjectMapper.Map<List<BugetConfigDto>>(_bugetConfig);
                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public BugetConfigEditDto GetBugetConfigId(int id)
        {
            try
            {


                var ret = _BugetConfigRepository.Get(id);
                return ObjectMapper.Map<BugetConfigEditDto>(ret);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public void SaveBugetConfig(BugetConfigEditDto storage)
        {
            try
            {
                var _BugetConfig = ObjectMapper.Map<BVC_Formular>(storage);

                if (_BugetConfig.Id == 0)
                {

                    int _chk = _BugetConfigRepository.GetAll().Where(f => f.AnBVC == _BugetConfig.AnBVC && f.State == Models.Conta.Enums.State.Active).Count();

                    if (_chk > 0) throw new UserFriendlyException("Eroare", "Termen existent!");

                    _BugetConfigRepository.Insert(_BugetConfig);
                }
                else
                {
                    var appClient = GetCurrentTenant();
                    _BugetConfig.TenantId = appClient.Id;
                    _BugetConfigRepository.Update(_BugetConfig);
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public void DeleteBugetConfig(int id)
        {
            try
            {


                var _BugetConfig = _BugetConfigRepository.FirstOrDefault(f => f.Id == id);
                _BugetConfig.State = Models.Conta.Enums.State.Inactive;
                _BugetConfigRepository.Update(_BugetConfig);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public BugetForm BugetRandInit(int FormularId)
        {
            try
            {



                var ret = new BugetForm
                {
                    FormularId = FormularId,
                };

                ret = BugetRandList(ret);


                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public BugetForm BugetRandList(BugetForm form)
        {

            try
            {



                var formulaList = _BugetFormRandRepository.GetAll().Include(f => f.DetaliiRand).Where(f => f.FormularId == form.FormularId)
                                                              .OrderBy(f => f.OrderView)
                                                              .ToList();
                //var ret = ObjectMapper.Map<List<BugetFormRandDto>>(formulaList);

                //foreach (var rand in ret)
                //{
                //    rand.DetaliiRand = new List<BugetFormRandDetailDto>();
                //}

                //foreach(var item in ret)
                //{
                //    var ciudat = _BugetFormRandDetailRepository.GetAll().Where(f => f.FormRandId == item.Id).FirstOrDefault();
                //    var retu = ObjectMapper.Map<BugetFormRandDetailDto>(ciudat);
                //    item.DetaliiRand.Add(retu);
                //}

                var _formulaList = ObjectMapper.Map<List<BugetFormRandDto>>(formulaList);
                form.RandList = _formulaList;

                var paapCheltList = PAAP_CheltuieliList(form.FormularId);
                form.PaapCheltList = paapCheltList;

                if (_formulaList.Count == 0)
                {
                    form = BugetAddRow(form);
                }

                return form;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        private List<PaapCheltListDto> PAAP_CheltuieliList(int formularId)
        {
            try
            {
                var formular = _BugetConfigRepository.FirstOrDefault(f => f.Id == formularId);
                var anBVC = formular.AnBVC;
                var startDate = new DateTime(anBVC, 1, 1);
                var endDate = LazyMethods.LastDayOfMonth(new DateTime(anBVC, 12, 1));
                var paapCheltList = new List<PaapCheltListDto>();

                var paapIdList = _bvcPaapTranseRepository.GetAllIncluding(f => f.BVC_PAAP).Where(f => f.DataTransa >= startDate && f.DataTransa <= endDate)
                                                    .Select(f => f.BVC_PAAPId)
                                                    .Distinct()
                                                    .ToList();

                var paapList = _bvcPaapRepository.GetAllIncluding(f => f.PaapStateList, f => f.Transe, f=> f.InvoiceElementsDetails, f => f.Departament)
                                               .Where(f => paapIdList.Contains(f.Id) && f.State == Models.Conta.Enums.State.Active)
                                               .ToList();
                foreach (var item in paapList)
                {
                    decimal valoarePaap = item.ValoareEstimataFaraTvaLei;

                    var rand = new BVC_BugetPrevRand();
                    var randDetail = new BVC_FormRandDetails();

                    randDetail = _BugetFormRandDetailRepository.GetAll().Where(f => f.TipRandCheltuialaId == item.InvoiceElementsDetailsId).FirstOrDefault();
                    if (randDetail == null)
                    {
                        paapCheltList.Add(new PaapCheltListDto
                        {
                            PaapId = item.Id,
                            TipCheltuiala = item.InvoiceElementsDetails.Description,
                            Valoare = valoarePaap,
                            Departament = item.Departament.Name,
                            Descriere = item.Descriere
                        });

                    }
                }

                return paapCheltList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public BugetForm BugetAddRow(BugetForm form)
        {
            int rowCode = 1;
            int orderView = 1;
            try
            {
                if (form.RandList.Count == 0)
                {
                    rowCode = _BugetFormRandRepository.GetAll().Where(f => f.FormularId == form.FormularId)
                                                          .Max(f => f.CodRand);
                }
                else
                {
                    rowCode = form.RandList.Max(f => f.CodRand);
                }
                rowCode++;
            }
            catch
            {

            }
            try
            {
                if (form.RandList.Count == 0)
                {
                    orderView = _BugetFormRandRepository.GetAll().Where(f => f.FormularId == form.FormularId).Max(f => f.OrderView);
                }
                else
                {
                    orderView = form.RandList.Max(f => f.OrderView);
                }
                orderView++;
            }
            catch
            {

            }
            var appClient = GetCurrentTenant();
            var Rand = new BugetFormRandDto
            {
                CodRand = rowCode,
                OrderView = orderView,
                FormularId = form.FormularId,
                Descriere = "",
                TipRand = 0,
                IsTotal = false,
                FormulaBVC = "",
                FormulaCashFlow = "",
                TipRandVenit = 0,
                AvailableBVC = false,
                AvailableCashFlow = false,
                TenantId = appClient.Id,
                DetaliiRand = new List<BugetFormRandDetailDto>() { new BugetFormRandDetailDto() },

            };
            form.RandList.Add(Rand);

            return form;
        }

        public BugetForm BugetOrderRow(BugetForm form)
        {
            try
            {


                form.RandList = form.RandList.OrderBy(f => f.OrderView).ToList();

                for (int i = 1; i < form.RandList.Count; i++)
                {
                    if (form.RandList[i - 1].OrderView == form.RandList[i].OrderView)
                    {
                        if (form.RandList[i - 1].Insert)
                        {
                            form.RandList[i - 1].Insert = false;
                            form.RandList[i].OrderView++;
                        }
                        else if (form.RandList[i].Insert)
                        {
                            form.RandList[i].Insert = false;
                            var swap = form.RandList[i - 1];
                            form.RandList[i - 1] = form.RandList[i];
                            form.RandList[i] = swap;
                            form.RandList[i].OrderView++;

                        }
                        else
                        {
                            form.RandList[i].OrderView++;
                        }
                    }
                }

                return form;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public BugetForm BugetRandSave(BugetForm form)
        {
            try
            {
                Dictionary<int, int> CodRandId = new Dictionary<int, int>();

                // prelucrez datele
                foreach (var item in form.RandList)
                {
                    if (item.TipRand != BVC_RowType.Venituri)
                    {
                        item.TipRandVenit = null;
                    }
                }


                var oldList = _BugetFormRandRepository.GetAll().Where(f => f.FormularId == form.FormularId).ToList();

                foreach (var item in oldList)
                {
                    var oldListDetails = _BugetFormRandDetailRepository.GetAll().Where(f => f.FormRandId == item.Id).ToList();

                    foreach (var rand in oldListDetails)
                    {
                        _BugetFormRandDetailRepository.Delete(rand);
                    }

                    _BugetFormRandRepository.Delete(item);
                }
                CurrentUnitOfWork.SaveChanges();


                foreach (var item in form.RandList)
                {

                    var _item = ObjectMapper.Map<BVC_FormRand>(item);

                    if (_item.RandParentIdFromUI != null && CodRandId.ContainsKey((int)_item.RandParentIdFromUI))
                    {
                        _item.RandParentId = CodRandId[(int)_item.RandParentIdFromUI];
                    }
                    _item.FormularId = form.FormularId;
                    _item.Id = 0;

                    int id = _BugetFormRandRepository.InsertAndGetId(_item);


                    CodRandId.Add(_item.CodRand, id);



                    if (item.DetaliiRand.Count == 0)
                    {
                        var s = new BVC_FormRandDetails();
                        s.Id = 0;
                        s.FormRandId = id;
                        _BugetFormRandDetailRepository.Insert(s);
                    }
                    else
                    {
                        foreach (var detail in item.DetaliiRand)
                        {
                            var _detail = ObjectMapper.Map<BVC_FormRandDetails>(detail);
                            _detail.Id = 0;
                            _detail.FormRandId = id;
                            _BugetFormRandDetailRepository.Insert(_detail);
                        }
                    }

                }
                CurrentUnitOfWork.SaveChanges();

                form = BugetRandList(form);
                return form;

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }

        }

        public BugetForm BugetRandSave2(BugetForm form)
        {


            try
            {
                // prelucrez datele
                foreach (var item in form.RandList)
                {
                    if (item.TipRand != BVC_RowType.Venituri)
                    {
                        item.TipRandVenit = null;
                    }
                }


                foreach (BugetFormRandDto item in form.RandList)
                {
                    if (item.Delete)
                    {
                        // sterg toate detaliile
                        foreach (var value in item.DetaliiRand)
                        {
                            _BugetFormRandDetailRepository.Delete(value.Id);

                        }
                        CurrentUnitOfWork.SaveChanges();
                        //sterg randul pe care utilizatorul s a decis sa l stearga
                        _BugetFormRandRepository.Delete(item.Id);
                        CurrentUnitOfWork.SaveChanges();
                    }
                    else
                    {
                        var _item = _BugetFormRandRepository.GetAll().AsNoTracking().Where(f => f.Id == item.Id).FirstOrDefault();


                        if (_item == null)
                        {
                            var __item = ObjectMapper.Map<BVC_FormRand>(item);
                            var Randparent = _BugetFormRandRepository.GetAll().AsNoTracking().Where(f => f.CodRand == __item.RandParentIdFromUI).FirstOrDefault();

                            if (Randparent == null)
                            {
                                __item.RandParentId = null;
                            }
                            else
                            {
                                __item.RandParentId = Randparent.Id;
                            }

                            __item.FormularId = form.FormularId;
                            __item.Id = 0;

                            int id = _BugetFormRandRepository.InsertAndGetId(__item);

                            if (__item.DetaliiRand.Count == 0)
                            {
                                var s = new BVC_FormRandDetails();
                                s.Id = 0;
                                s.FormRandId = id;
                                _BugetFormRandDetailRepository.Insert(s);
                                CurrentUnitOfWork.SaveChanges();
                            }
                            else
                            {
                                //foreach(var value in __item.DetaliiRand)
                                //{
                                //    var s = new BVC_FormRandDetails();
                                //    s.Id = 0;
                                //    s.FormRandId = id;
                                //    s.TipRandCheltuialaId = value.TipRandCheltuialaId;
                                //    _BugetFormRandDetailRepository.Insert(s);
                                //}
                            }



                            CurrentUnitOfWork.SaveChanges();


                        }
                        else
                        {
                            var Randparent = _BugetFormRandRepository.GetAll().AsNoTracking().Where(f => f.CodRand == item.RandParentIdFromUI).FirstOrDefault();

                            if (Randparent == null)
                            {
                                item.RandParentId = null;
                            }
                            else
                            {
                                item.RandParentId = Randparent.Id;
                            }

                            var ret = ObjectMapper.Map<BVC_FormRand>(item);

                            foreach (var value in item.DetaliiRand)
                            {
                                if (value.Id == 0)
                                {
                                    value.FormRandId = item.Id;
                                    var rett = ObjectMapper.Map<BVC_FormRandDetails>(value);
                                    _BugetFormRandDetailRepository.Insert(rett);
                                    CurrentUnitOfWork.SaveChanges();
                                    //ret.DetaliiRand = ret.DetaliiRand.Where(e => e.Id != value.Id).ToList();

                                }
                                else if (value.Delete)
                                {
                                    //ret.DetaliiRand = ret.DetaliiRand.Where(e => e.Id != value.Id).ToList();
                                    //item.DetaliiRand = item.DetaliiRand.Where(e => e.Id != value.Id).ToList();
                                    _BugetFormRandDetailRepository.Delete(value.Id);
                                    CurrentUnitOfWork.SaveChanges();
                                }
                                else
                                {
                                    var rett = ObjectMapper.Map<BVC_FormRandDetails>(value);
                                    _BugetFormRandDetailRepository.Update(rett);
                                    //ret.DetaliiRand = ret.DetaliiRand.Where(e => e.Id != value.Id).ToList();
                                    CurrentUnitOfWork.SaveChanges();
                                }

                            }

                            ret.DetaliiRand = new List<BVC_FormRandDetails>();
                            _BugetFormRandRepository.Update(ret);
                            CurrentUnitOfWork.SaveChanges();

                        }

                    }
                }

                //form.RandList = form.RandList.Where(f => f.Delete == false).ToList();
                //form.RandList.ForEach(x => x.DetaliiRand = x.DetaliiRand.Where(f => f.Delete == false).ToList());
                form = BugetRandList(form);
                return form;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public void BugetClonare(int formularId, int an)
        {
            try
            {
                var appClient = GetCurrentTenant();
                var count = _BugetConfigRepository.GetAll().Count(f => f.AnBVC == an && f.State == Models.Conta.Enums.State.Active);
                if (count != 0)
                {
                    throw new Exception("Exista un alt formular definit pentru anul " + an.ToString());
                }
                var raduriCurrList = _BugetFormRandRepository.GetAllIncluding(f => f.DetaliiRand).Where(f => f.FormularId == formularId).ToList();

                var formularNew = new BVC_Formular
                {
                    AnBVC = an,
                    State = Models.Conta.Enums.State.Active,
                    TenantId = appClient.Id
                };
                _BugetConfigRepository.Insert(formularNew);

                foreach (var item in raduriCurrList)
                {
                    var randNew = new BVC_FormRand
                    {
                        Formular = formularNew,
                        CodRand = item.CodRand,
                        Descriere = item.Descriere,
                        TipRand = item.TipRand,
                        OrderView = item.OrderView,
                        NivelRand = item.NivelRand,
                        IsTotal = item.IsTotal,
                        Bold = item.Bold,
                        FormulaBVC = item.FormulaBVC,
                        FormulaCashFlow = item.FormulaCashFlow,
                        TipRandVenit = item.TipRandVenit,
                        TipRandSalarizare = item.TipRandSalarizare,
                        TipRandCheltuialaId = item.TipRandCheltuialaId,
                        AvailableBVC = item.AvailableBVC,
                        AvailableCashFlow = item.AvailableCashFlow,
                        TenantId = item.TenantId
                    };
                    _BugetFormRandRepository.Insert(randNew);

                    foreach (var details in item.DetaliiRand)
                    {
                        var detNew = new BVC_FormRandDetails
                        {
                            FormRand = randNew,
                            TipRandCheltuialaId = details.TipRandCheltuialaId,
                            TenantId = details.TenantId
                        };
                        _BugetFormRandDetailRepository.Insert(detNew);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }

        }

    }
}

