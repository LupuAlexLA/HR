using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.EntityFrameworkCore.Repositories.Conta;
using Niva.Erp.EntityFrameworkCore.Repositories.imprumut;
using Niva.Erp.Imprumuturi;
using Niva.Erp.Imprumuturi.Dto;
using Niva.Erp.Managers.Imprumut;
using Niva.Erp.Models.AutoOperation;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.ImoAsset;
using Niva.Erp.Models.Imprumuturi;
using Niva.Erp.Models.PrePayments;
using Niva.Erp.Repositories.Conta;
using Niva.Erp.Repositories.Conta.AutoOperation;
using Niva.Erp.Repositories.Conta.Prepayments;
using Niva.Erp.Repositories.ImoAsset;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Conta.AutoOperation
{
    public interface IOperGenerateService : IApplicationService
    {
        List<OperGenerateListDto> OperGenerateList(DateTime startDate, DateTime endDate);
        OperGenerateAddDto OperGenerateAddInit();
        OperGenerateAddDto OperGenerateAddSearchOper(OperGenerateAddDto form);
        void OperGenerateAdd(OperGenerateAddDto form);
        void OperGenerateDelete(int Id);
    }

    public class OperGenerateService : ErpAppServiceBase, IOperGenerateService
    {
        IOperGenerateRepository _operGenerateRepository;
        IBalanceRepository _balanceRepository;
        IRepository<OperGenerateTipuri> _operGenerateTipuriRepository;
        OperationRepository _operationRepository;
        ImprumutRepository _imprumuturiRepository;
        IAutoOperationRepository _autoOperationRepository;
        IPrepaymentRepository _prepaymentRepository;
        IRepository<OperationDetails> _operationDetailsRepository;
        IRepository<Dobanda> _dobandaRepository;
        IImoOperationRepository _imoOperationRepository;
        IOperatieComisionDobandaAppService _OperatieComisionDobandaAppService;
        ImprumutOperationManager _imprumutOperationManager;
        IRepository<ComisionV2> _comisionRepository;
        IRepository<OperatieDobandaComision> _operatieDobandaComisionRepository;

        public OperGenerateService(IRepository<OperatieDobandaComision> operatieDobandaComisionRepository, IRepository<ComisionV2> comisionRepository, ImprumutOperationManager imprumutOperationManager, IOperatieComisionDobandaAppService OperatieComisionDobandaAppService, IRepository<Dobanda> dobandaRepository, ImprumutRepository imprumuturiRepository, IOperGenerateRepository operGenerateRepository, IBalanceRepository balanceRepository, IRepository<OperGenerateTipuri> operGenerateTipuriRepository,
                                   OperationRepository operationRepository, IAutoOperationRepository autoOperationRepository, IPrepaymentRepository prepaymentRepository,
                                   IRepository<OperationDetails> operationDetailsRepository, IImoOperationRepository imoOperationRepository)
        {
            _operatieDobandaComisionRepository = operatieDobandaComisionRepository;
            _comisionRepository = comisionRepository;
            _imprumutOperationManager = imprumutOperationManager;
            _OperatieComisionDobandaAppService = OperatieComisionDobandaAppService;
            _dobandaRepository = dobandaRepository;
            _imprumuturiRepository = imprumuturiRepository;
            _operGenerateRepository = operGenerateRepository;
            _balanceRepository = balanceRepository;
            _operGenerateTipuriRepository = operGenerateTipuriRepository;
            _operationRepository = operationRepository;
            _autoOperationRepository = autoOperationRepository;
            _prepaymentRepository = prepaymentRepository;
            _operationDetailsRepository = operationDetailsRepository;
            _imoOperationRepository = imoOperationRepository;
        }

        //[AbpAuthorize("Conta.Balanta.SfarsitLuna.Acces")]
        public List<OperGenerateListDto> OperGenerateList(DateTime startDate, DateTime endDate)
        {
            try
            {
                var ret = new List<OperGenerateListDto>();

                ret = _operGenerateRepository.GetAllIncluding(f => f.TipOperatie, f => f.TipOperatie.Categ)
                                             .Where(f => f.DataOperatie >= startDate && f.DataOperatie <= endDate && f.State == State.Active)
                                             .Select(f => new OperGenerateListDto
                                             {
                                                 Id = f.Id,
                                                 CategorieOperatie = f.TipOperatie.Categ.CategType,
                                                 TipOperatie = f.TipOperatie.Descriere,
                                                 DataOperatie = f.DataOperatie
                                             })
                                             .OrderByDescending(f => f.DataOperatie)
                                             .ToList();
                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public OperGenerateAddDto OperGenerateAddInit()
        {
            var ret = new OperGenerateAddDto
            {
                StartDate = _balanceRepository.BalanceDateNextDay(),
                EndDate = LazyMethods.Now(),
                OperatiiPropuse = new List<OperGenerateAddListDto>()
            };
            return ret;
        }

        public OperGenerateAddDto OperGenerateAddSearchOper(OperGenerateAddDto form)
        {
            try
            {
                var listPropus = new List<OperGenerateAddListDto>();
                var currDate = form.StartDate;

                // iau doar operatiile care se realizeaza pentru sfarsitul de luna
                while (currDate <= form.EndDate)
                {
                    if (currDate.Day == DateTime.DaysInMonth(currDate.Year, currDate.Month))
                    {
                        var listInreg = _operGenerateRepository.GetAllIncluding(f => f.TipOperatie)
                                                               .Where(f => f.State == State.Active && f.DataOperatie == currDate)
                                                               .Select(f => f.TipOperatie.Tip)
                                                               .ToList();
                        var list = _operGenerateTipuriRepository.GetAllIncluding(f => f.Categ)
                                                                .Where(f => f.State == State.Active && f.Categ.State == State.Active && f.SfarsitLuna
                                                                       && !listInreg.Contains(f.Tip))
                                                                .Select(f => new OperGenerateAddListDto
                                                                {
                                                                    Selected = false,
                                                                    CategorieOperatie = f.Categ.CategType,
                                                                    TipOperatieId = f.Id,
                                                                    TipOperatie = f.Descriere,
                                                                    TipOperatieShort = f.Tip,
                                                                    DataOperatie = currDate,
                                                                    ExecOrder = f.ExecOrder
                                                                })
                                                                .OrderBy(f => f.ExecOrder)
                                                                .ToList();

                        foreach (var item in list)
                        {
                            listPropus.Add(item);
                        }
                    }
                    currDate = currDate.AddDays(1);
                }
                form.OperatiiPropuse = listPropus;

                return form;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public void OperGenerateAdd(OperGenerateAddDto form)
        {
            //try
            //{
                foreach (var item in form.OperatiiPropuse.Where(f => f.Selected).OrderBy(f => f.ExecOrder))
                {
                    if (!_operationRepository.VerifyClosedMonth(item.DataOperatie))
                    {
                        throw new Exception("Nu se poate inregistra operatia deoarece luna contabila este inchisa");
                    }

                    OperGenerateAddItem(item);
                }
                CurrentUnitOfWork.SaveChanges();
            //}
            //catch (Exception ex)
            //{
            //    throw new UserFriendlyException("Eroare", ex.Message);
            //}
        }

        private void OperGenerateAddItem(OperGenerateAddListDto item)
        {
            try
            {
                DateTime balanceDateNextDay = _balanceRepository.BalanceDateNextDay();
                DateTime lastBalance = _balanceRepository.GetAll().Where(f => f.Status == State.Active).Max(f => f.BalanceDate);
                var operGen = new OperGenerate
                {
                    DataOperatie = item.DataOperatie,
                    TipOperatieId = item.TipOperatieId,
                    State = State.Active
                };
                _operGenerateRepository.Insert(operGen);
                CurrentUnitOfWork.SaveChanges();

                var appClient = GetCurrentTenant();
                if (item.TipOperatieShort == "ACA") // amortizare cheltuieli in avans
                {
                    _prepaymentRepository.GestPrepayments(operGen.DataOperatie, PrepaymentType.CheltuieliInAvans, lastBalance);

                    _autoOperationRepository.AutoPrepaymentsOperationAdd(operGen.DataOperatie, appClient.LocalCurrencyId.Value, PrepaymentType.CheltuieliInAvans, lastBalance, operGen.Id);
                }
                else if (item.TipOperatieShort == "RC") // repartizare cheltuieli pe fonduri
                {
                    _autoOperationRepository.RepartizareCheltuieli(operGen.DataOperatie, appClient.Id, appClient.LocalCurrencyId.Value, operGen.Id);
                }
                else if (item.TipOperatieShort == "VC") // Inchidere Venituri si Cheltuieli
                {
                    _autoOperationRepository.InchidereVenituriCheltuieli(operGen.DataOperatie, appClient.Id, appClient.LocalCurrencyId.Value, operGen.Id);
                }
                else if (item.TipOperatieShort == "AMF") // amortizare mijloace fixe
                {
                    _imoOperationRepository.GestImoAssetsComputing(operGen.DataOperatie);
                    _autoOperationRepository.AutoImoAssetOperationAdd(operGen.DataOperatie, appClient.Id, appClient.LocalCurrencyId.Value, ImoAssetType.MijlocFix, lastBalance, operGen.Id);
                }
                else if (item.TipOperatieShort == "RPV") // Reevaluare pozitie valutara
                {
                    _autoOperationRepository.ReevaluarePozitieValutara(operGen.DataOperatie, appClient.Id, appClient.LocalCurrencyId.Value, operGen.Id);
                }
                else if (item.TipOperatieShort == "D")
                {
                    int localCurrencyId = GetCurrentTenant().LocalCurrencyId.Value;
                    //_imprumuturiRepository.GenerateDobandaOper(operGen.DataOperatie, operGen.Id, localCurrencyId);
                    var imprumuturi = _imprumuturiRepository.GetAll().Where(f => f.State == State.Active && f.TipCreditare == TipCreditare.LinieDeCredit).ToList();

                    foreach (var imprumut in imprumuturi)
                    {
                        var DobandaComisionList = _OperatieComisionDobandaAppService.OperatieComisionDobandaByImprumutIdList(imprumut.Id);
                        var LastComision = DobandaComisionList.OperatieComision.LastOrDefault();
                        var LastDobanda = DobandaComisionList.OperatieDobanda.LastOrDefault();

                        var lastComisionDate = (LastComision == null) ? imprumut.DocumentDate : LastComision.DataEnd;
                        var lastDobandaDate = (LastDobanda == null) ? imprumut.DocumentDate : LastDobanda.DataEnd;

                        var operatieComision = new OperatieDobandaComisionDto()
                        {
                            Id = 0,
                            ImprumutId = imprumut.Id,
                            OperGenerateId = operGen.Id,
                            DataStart = lastComisionDate,
                            DataEnd = operGen.DataOperatie,
                            TipOperatieDobandaComision = TipOperatieDobandaComision.comision,
                            State = State.Active,
                        };

                        var operatieDobanda = new OperatieDobandaComisionDto()
                        {
                            Id = 0,
                            ImprumutId = imprumut.Id,
                            OperGenerateId = operGen.Id,
                            DataStart = lastDobandaDate,
                            DataEnd = operGen.DataOperatie,
                            TipOperatieDobandaComision = TipOperatieDobandaComision.dobanda,
                            State = State.Active,
                        };


                        _OperatieComisionDobandaAppService.OperatieComisionDobandaSave(operatieComision);
                        _OperatieComisionDobandaAppService.OperatieComisionDobandaSave(operatieDobanda);

                        //plata comision in avans

                        var comisionInAvansList = _comisionRepository.GetAll().Where(f => f.ImprumutId == imprumut.Id && f.TipComision == TipComisionV2.cheltuialaInAvans && operGen.DataOperatie > f.DataStart && operGen.DataOperatie <= f.DataEnd).ToList();

                        foreach (var comisionInAvans in comisionInAvansList)
                        {
                            _imprumutOperationManager.ComisionInAvansSfarsitDeLuna(localCurrencyId, operGen.Id, operGen.DataOperatie, imprumut, comisionInAvans);
                        }


                    }



                }
            }
            catch(Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        [AbpAuthorize("Conta.Balanta.SfarsitLuna.Modificare")]
        public void OperGenerateDelete(int Id)
        {
            try
            {
                var item = _operGenerateRepository.GetAllIncluding(f => f.TipOperatie).FirstOrDefault(f => f.Id == Id);

                if (!_operationRepository.VerifyClosedMonth(item.DataOperatie))
                {
                    throw new Exception("Nu se poate sterge operatia deoarece luna contabila este inchisa");
                }

                item.State = State.Inactive;
                CurrentUnitOfWork.SaveChanges();

                var operGenerateTipuri = _operGenerateTipuriRepository.GetAll().FirstOrDefault(f => f.Id == item.TipOperatieId);

                if (operGenerateTipuri.Tip == "ACA")
                {
                    try
                    {
                        _autoOperationRepository.DeleteUncheckedAutoOperation(item.DataOperatie, PrepaymentType.CheltuieliInAvans);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Operatia nu poate fi stearsa, deoarece " + ex.Message);
                    }

                    try
                    {
                        _prepaymentRepository.GestPrepaymentDelComputing(item.DataOperatie, PrepaymentType.CheltuieliInAvans);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Operatia nu poate fi stearsa, deoarece " + ex.Message);
                    }
                }
                else if (operGenerateTipuri.Tip == "RC")
                {
                    var detailList = _operationDetailsRepository.GetAll().Where(f => f.OperGenerateId == Id).ToList();
                    foreach (var detail in detailList)
                    {
                        _operationDetailsRepository.Delete(detail);
                    }
                    CurrentUnitOfWork.SaveChanges();
                }
                else if (operGenerateTipuri.Tip == "VC")
                {
                    var operation = _operationRepository.GetAll().FirstOrDefault(f => f.OperGenerateId == Id);
                    if (operation != null)
                    {
                        if (operation.OperationStatus == OperationStatus.Checked)
                        {
                            throw new Exception("Operatia nu poate fi stearsa, deoarece este validata");
                        }
                        operation.State = State.Inactive;
                        CurrentUnitOfWork.SaveChanges();
                    }
                }
                else if (operGenerateTipuri.Tip == "AMF")
                {
                    try
                    {
                        _autoOperationRepository.DeleteAssetOperations(item.DataOperatie);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Operatia nu poate fi stearsa, deoarece " + ex.Message);
                    }

                    try
                    {
                        _imoOperationRepository.GestAssetDelComputing(item.DataOperatie);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Operatia nu poate fi stearsa, deoarece " + ex.Message);
                    }
                }
                else if (operGenerateTipuri.Tip == "RPV") // Reevaluare pozitie valutara
                {
                    var operation = _operationRepository.GetAll().FirstOrDefault(f => f.OperGenerateId == Id);
                    if (operation != null)
                    {
                        operation.State = State.Inactive;
                        CurrentUnitOfWork.SaveChanges();
                    }
                }
                else if (operGenerateTipuri.Tip == "D") 
                {
                    //var dobandaList = _dobandaRepository.GetAll().Where(f => f.OperGenerateId == Id).ToList();

                    //foreach(var dobanda in dobandaList)
                    //{

                    //    dobanda.State = State.Inactive;
                    //    var contaOperation = _operationRepository.GetAll().Where(f => f.Id == dobanda.ContaOperationId).FirstOrDefault();

                    //    if (contaOperation != null)
                    //    {
                    //        var details = _operationDetailsRepository.GetAll().Where(f => f.OperationId == contaOperation.Id).FirstOrDefault();
                    //        _operationDetailsRepository.Delete(details);
                    //        _operationRepository.Delete(contaOperation);
                    //    }

                    //}


                    var operationList = _operationRepository.GetAll().Where(f => f.OperGenerateId == Id).ToList();
                    var operatieDobandaComision = _operatieDobandaComisionRepository.GetAll().Where(f => f.OperGenerateId == Id).ToList();

                    foreach (var operation in operationList)
                    {
                        operation.State = State.Inactive;
                        CurrentUnitOfWork.SaveChanges();
                    }

                    foreach(var operDC in operatieDobandaComision)
                    {
                        operDC.State = State.Inactive;
                        CurrentUnitOfWork.SaveChanges();
                    }
                }

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", LazyMethods.GetErrMessage(ex));
            }
        }
    }
}
