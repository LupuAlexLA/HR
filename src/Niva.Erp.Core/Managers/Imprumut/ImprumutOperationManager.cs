using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.EntityFrameworkCore;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Imprumuturi;
using Niva.Erp.Repositories;
using Niva.Erp.Repositories.Conta;
using Niva.Erp.Repositories.Conta.Nomenclatures;
using Niva.Erp.Models.Imprumuturi;
using System;
using System.Collections.Generic;
using System.Text;
using Niva.Erp.Models.Conta.Enums;
using System.Linq;
using Niva.Erp.Repositories.Conta.AutoOperation;
using Niva.Erp.Models.AutoOperation;
using Niva.Erp.Models.Contracts;
using Microsoft.EntityFrameworkCore;
using Abp.UI;

namespace Niva.Erp.Managers.Imprumut
{
    public class ImprumutOperationManager : DomainService
    {
        IAccountRepository _accountRepository;
        IExchangeRatesRepository _exchangeRatesRepository;
        ICurrencyRepository _currencyRepository;
        IBalanceRepository _balanceRepository;
        IOperationRepository _operationRepository;
        IAutoOperationRepository _autoOperationRepository;
        IRepository<Models.Imprumuturi.Imprumut> _imprumutRepository;
        IRepository<BalanceDetails> _balanceDetailsRepository;
        IRepository<OperationDetails> _operationDetailsRepository;
        IRepository<ComisionV2> _comisionV2Repository;
        IRepository<DocumentType> _documentTypeRepository; 
        IRepository<AutoOperationConfig> _autoOperationConfigRepository; 
        IRepository<ImprumutTipDetaliu> _imprumutTipDetalii;
        IRepository<DateDobanziReferinta> _DateDobanziReferintaRepository;
        IRepository<OperatieDobandaComision> _operatieDobandaComisionRepository;
        public ImprumutOperationManager(
        IRepository<ImprumutTipDetaliu> imprumutTipDetalii, 
        IAccountRepository accountRepository,
        IExchangeRatesRepository exchangeRatesRepository,
        ICurrencyRepository currencyRepository,
        IBalanceRepository balanceRepository,
        IRepository<Models.Imprumuturi.Imprumut> imprumutRepository,
        IRepository<BalanceDetails> balanceDetailsRepository,
        IRepository<OperationDetails> operationDetailsRepository,
        IRepository<ComisionV2> comisionV2Repository,
        IAutoOperationRepository autoOperationRepository,
        IRepository<DocumentType> documentTypeRepository,
        IOperationRepository operationRepository,
        IRepository<AutoOperationConfig> autoOperationConfigRepository,
        IRepository<DateDobanziReferinta> DateDobanziReferintaRepository,
        IRepository<OperatieDobandaComision> operatieDobandaComisionRepository)
        {
            _DateDobanziReferintaRepository = DateDobanziReferintaRepository;
            _autoOperationConfigRepository = autoOperationConfigRepository;
            _documentTypeRepository = documentTypeRepository;
            _autoOperationRepository = autoOperationRepository;
            _comisionV2Repository = comisionV2Repository;
            _operationDetailsRepository = operationDetailsRepository;
            _balanceDetailsRepository = balanceDetailsRepository;
            _imprumutRepository = imprumutRepository;
            _accountRepository = accountRepository;
            _exchangeRatesRepository = exchangeRatesRepository;
            _currencyRepository = currencyRepository;
            _balanceRepository = balanceRepository;
            _operationRepository = operationRepository;
            _imprumutTipDetalii = imprumutTipDetalii;
            _operatieDobandaComisionRepository = operatieDobandaComisionRepository;
        }

        public int OperatieToConta(OperatieDobandaComision Operatie, int localCurrencyId, int? operGenId)
        {

            
            var imprumut = _imprumutRepository.Get((int)Operatie.ImprumutId);

            var count = _balanceRepository.GetAll().ToList().Count(f => f.Status == State.Active && f.BalanceDate.Month == Operatie.DataEnd.Month && f.BalanceDate.Year == Operatie.DataEnd.Year);
            if (count != 0)
            {
                throw new Exception("Nu se poate inregistra operatia deoarece luna contabila este inchisa");
            }

            var debitId = _accountRepository.FirstOrDefault(f => f.Status == State.Active && f.Symbol == "999").Id;
            var contImprumut = _accountRepository.GetAll().ToList().FirstOrDefault(f => f.Status == State.Active && f.Id == imprumut.ContContabilId).Id;
            //    var lastBalance = Context.BalanceDetails.Include(f => f.Account).Include(f => f.Balance).OrderByDescending(f => f.Balance.BalanceDate).FirstOrDefault(f => f.Account.Symbol == imprumut.ContContabil); // cea mai recenta balanta asociata contului
            var lastBalance = _balanceRepository.GetAll().ToList().Where(f => f.Status == State.Active && f.BalanceDate < Operatie.DataStart).OrderByDescending(f => f.BalanceDate).FirstOrDefault(); // cea mai recenta balanta

            var soldInitialItem = _balanceDetailsRepository.GetAll().ToList().FirstOrDefault(f => f.AccountId == contImprumut && f.BalanceId == lastBalance.Id);
            decimal sumaImprumutataLastBalance = 0;
            if (soldInitialItem != null)
            {
                sumaImprumutataLastBalance = soldInitialItem.CrValueF - soldInitialItem.DbValueF;
            }

            var contaOperationTragere = _operationDetailsRepository.GetAllIncluding(f => f.Operation).ToList().Where(f => f.Operation.OperationDate > lastBalance.BalanceDate && f.Operation.OperationDate <= Operatie.DataEnd && f.CreditId == contImprumut && f.Operation.State == State.Active).ToList(); // de adaugat al cui imprumut este operatia contabila
            var contaOperationRambursare = _operationDetailsRepository.GetAllIncluding(f => f.Operation).ToList().Where(f => f.Operation.OperationDate > lastBalance.BalanceDate && f.Operation.OperationDate <= Operatie.DataEnd && f.DebitId == contImprumut && f.Operation.State == State.Active).ToList();

            
            decimal calculComision = 0;

            decimal soldDisponibil = imprumut.Suma - sumaImprumutataLastBalance;
            decimal valoareImprumutata = sumaImprumutataLastBalance;

            var firstDayOperation = Operatie.DataStart;

            var contaOperationTragereToFirstDay = contaOperationTragere.Where(f => f.Operation.OperationDate < firstDayOperation);
            var contaOperationRambursareToFirstDay = contaOperationRambursare.Where(f => f.Operation.OperationDate < firstDayOperation);


            soldDisponibil = soldDisponibil - contaOperationTragereToFirstDay.Sum(f => f.Value);

            soldDisponibil = soldDisponibil + contaOperationRambursareToFirstDay.Sum(f => f.Value);

            


            //for (var i = Operatie.DataStart ; i <= Operatie.DataEnd; i.AddDays(1))
            //{
            //    var operTragere = contaOperationTragere.Where(f => f.Operation.OperationDate == i);
            //    var operRambursare = contaOperationRambursare.Where(f => f.Operation.OperationDate == i);

            //    foreach (var value in operTragere)
            //    {
            //        soldDisponibil = soldDisponibil - value.Value;
            //    }

            //    foreach (var value in operRambursare)
            //    {
            //        soldDisponibil = soldDisponibil + value.Value;
            //    }

            //    valoareImprumutata = imprumut.Suma - soldDisponibil;

            //    calculDobanda = calculDobanda + valoareImprumutata * imprumut.ProcentDobanda / 100 / 360;

            //}

            if (Operatie.TipOperatieDobandaComision == TipOperatieDobandaComision.dobanda)
            {
                decimal calculDobanda = 0;
                decimal? dobandaVariabila = null;

                if (imprumut.TipDobanda == TipDobanda.Variabila)
                {
                    
                        var _date = _DateDobanziReferintaRepository.GetAll()
                                                           .Include(e => e.DobanziReferinta)
                                                           .Where(f => f.DobanziReferintaId == imprumut.DobanziReferintaId && f.State == Models.Conta.Enums.State.Active && f.Data <= Operatie.DataStart).OrderBy(f => f.Data).Last();
                        if (_date != null)
                        {
                            dobandaVariabila = imprumut.MarjaFixa + _date.Valoare;
                        }
                        else
                        {
                            throw new UserFriendlyException("Eroare", "Dobanda de referinta neactualizata!");
                        }

                }

                for (var i = Operatie.DataStart.Date; i <= Operatie.DataEnd.Date; i = i.AddDays(1))
                {
                    var operTragere = contaOperationTragere.Where(f => f.Operation.OperationDate == i);
                    var operRambursare = contaOperationRambursare.Where(f => f.Operation.OperationDate == i);

                    foreach (var value in operTragere)
                    {
                        soldDisponibil = soldDisponibil - value.Value;
                    }

                    foreach (var value in operRambursare)
                    {
                        soldDisponibil = soldDisponibil + value.Value;
                    }

                    valoareImprumutata = imprumut.Suma - soldDisponibil;

                    calculDobanda = calculDobanda + valoareImprumutata * (dobandaVariabila ?? imprumut.ProcentDobanda) / 100 / 360;

                }




                var documentType = _documentTypeRepository.GetAll().FirstOrDefault(f => f.TypeNameShort == "NC");
                var documentNumber = _autoOperationRepository.GetNextNumberForOperContab(Operatie.DataEnd, documentType.Id).ToString();
                
                if (documentType == null)
                    throw new Exception("Nu a fost identificat tipul de document pentru inregistrarea imprumutului!");



                var operationChild = new Operation(); // e folosit in cazul in care avem documente in valuta

                var operation = new Operation
                {
                    CurrencyId = localCurrencyId,
                    OperationDate = Operatie.DataEnd,
                    DocumentTypeId = documentType.Id,
                    DocumentNumber = documentNumber,
                    DocumentDate = Operatie.DataEnd,
                    OperationStatus = OperationStatus.Unchecked,
                    State = State.Active,
                    ExternalOperation = true,
                    OperGenerateId = operGenId,
                };

                _operationRepository.Insert(operation);
                CurrentUnitOfWork.SaveChanges();



                decimal exchangeRate = 1;
                if (imprumut.CurrencyId != localCurrencyId)
                {
                    exchangeRate = _exchangeRatesRepository.GetExchangeRateForOper(Operatie.DataEnd, imprumut.CurrencyId, localCurrencyId);

                    operationChild = new Operation // pentr operatiile in valuta... in operation am valorile in lei, iar in operationChild am valorile in valuta
                    {
                        CurrencyId = imprumut.CurrencyId,
                        OperationDate = Operatie.DataEnd,
                        DocumentTypeId = documentType.Id,
                        DocumentNumber = documentNumber,
                        DocumentDate = Operatie.DataEnd,
                        OperationStatus = OperationStatus.Unchecked,
                        State = State.Active,
                        ExternalOperation = true,
                        OperationParentId = operation.Id,
                        OperGenerateId = operGenId,
                    };
                    _operationRepository.Insert(operationChild);
                    CurrentUnitOfWork.SaveChanges();
                    
                }

                DobandaLinieDeCredit(exchangeRate, documentNumber, localCurrencyId, Operatie.DataEnd, calculDobanda, imprumut, operation, operationChild);
                return operation.Id;
            }

            else if (Operatie.TipOperatieDobandaComision == TipOperatieDobandaComision.comision)
            {
                var comisionList = _comisionV2Repository.GetAll().Where(f => f.ImprumutId == imprumut.Id && f.TipComision != TipComisionV2.cheltuialaInAvans && f.TipComision != TipComisionV2.cheltuialaIntegrala && f.State == State.Active && f.DataEnd >= Operatie.DataStart.Date && f.DataStart <= Operatie.DataEnd.Date).ToList();
                                                     
                decimal[] comisioneCalculate = new decimal[comisionList.Count];

                for (var i = Operatie.DataStart.Date; i <= Operatie.DataEnd.Date; i = i.AddDays(1))
                {
                    var operTragere = contaOperationTragere.Where(f => f.Operation.OperationDate == i);
                    var operRambursare = contaOperationRambursare.Where(f => f.Operation.OperationDate == i);

                    foreach (var value in operTragere)
                    {
                        soldDisponibil = soldDisponibil - value.Value;
                    }

                    foreach (var value in operRambursare)
                    {
                        soldDisponibil = soldDisponibil + value.Value;
                    }

                    valoareImprumutata = imprumut.Suma - soldDisponibil;

                    for (int j = 0; j < comisionList.Count; j++)
                    {
                        var comision = comisionList[j];
                        decimal _calculComision = comision.ValoareComision ;

                        if (comision.TipValoareComision == TipValoareComision.Procent)
                        {
                            if (comision.TipSumaComision == TipSumaComision.Sold)
                            {
                                _calculComision = soldDisponibil * comision.ValoareComision / 100 / comision.BazaDeCalcul;
                            }
                            else if (comision.TipSumaComision == TipSumaComision.ValoareImprumut)
                            {
                                _calculComision = imprumut.Suma * comision.ValoareComision / 100 / comision.BazaDeCalcul;
                            }
                            else if (comision.TipSumaComision == TipSumaComision.SumaTrasa)
                            {
                                _calculComision = valoareImprumutata * comision.ValoareComision / 100 / comision.BazaDeCalcul;
                            }
                        }

                        comisioneCalculate[j] = comisioneCalculate[j] + _calculComision;
                    }

                }

                var documentTypeComision = _documentTypeRepository.GetAll().ToList().FirstOrDefault(f => f.TypeNameShort == "NC");
                var documentNumberComision = _autoOperationRepository.GetNextNumberForOperContab(Operatie.DataEnd, documentTypeComision.Id).ToString();

                if (documentTypeComision == null)
                    throw new Exception("Nu a fost identificat tipul de document pentru inregistrarea imprumutului!");



                var operationChildComision = new Operation(); // e folosit in cazul in care avem documente in valuta

                var operationComision = new Operation
                {
                    CurrencyId = localCurrencyId,
                    OperationDate = Operatie.DataEnd,
                    DocumentTypeId = documentTypeComision.Id,
                    DocumentNumber = documentNumberComision,
                    DocumentDate = Operatie.DataEnd,
                    OperationStatus = OperationStatus.Unchecked,
                    State = State.Active,
                    ExternalOperation = true,
                    OperGenerateId = operGenId
                };

                _operationRepository.Insert(operationComision);
                CurrentUnitOfWork.SaveChanges();

                decimal exchangeRateComision = 1;
                if (imprumut.CurrencyId != localCurrencyId)
                {
                    exchangeRateComision = _exchangeRatesRepository.GetExchangeRateForOper(Operatie.DataEnd, imprumut.CurrencyId, localCurrencyId);

                    operationChildComision = new Operation // pentr operatiile in valuta... in operation am valorile in lei, iar in operationChild am valorile in valuta
                    {
                        CurrencyId = imprumut.CurrencyId,
                        OperationDate = Operatie.DataEnd,
                        DocumentTypeId = documentTypeComision.Id,
                        DocumentNumber = documentNumberComision,
                        DocumentDate = Operatie.DataEnd,
                        OperationStatus = OperationStatus.Unchecked,
                        State = State.Active,
                        ExternalOperation = true,
                        OperationParentId = operationComision.Id,
                        OperGenerateId = operGenId

                    };
                    _operationRepository.Insert(operationChildComision);
                    CurrentUnitOfWork.SaveChanges();

                }

                for (int i = 0; i < comisionList.Count; i++)
                {
                    ComisionLinieCredit(exchangeRateComision, documentNumberComision, localCurrencyId, Operatie.DataEnd, comisioneCalculate[i], imprumut, comisionList[i], operationComision, operationChildComision);
                }
                return operationComision.Id;
            }

            return 0;

        }

        public int OperatieToContaCredit(OperatieDobandaComision Operatie, int localCurrencyId, int? operGenId)
        {


            var imprumut = _imprumutRepository.GetAllIncluding( f => f.Rate ).FirstOrDefault(f => f.Id == (int)Operatie.ImprumutId);

            var count = _balanceRepository.GetAll().Count(f => f.Status == State.Active && f.BalanceDate.Month == Operatie.DataEnd.Month && f.BalanceDate.Year == Operatie.DataEnd.Year);
            if (count != 0)
            {
                throw new Exception("Nu se poate inregistra operatia deoarece luna contabila este inchisa");
            }


            if (Operatie.TipOperatieDobandaComision == TipOperatieDobandaComision.dobanda)
            {
                decimal calculDobanda = 0;
                Rata rataAferentaOperatiei = imprumut.Rate.OrderBy(f => f.DataPlataRata).FirstOrDefault(f => f.DataPlataRata >= Operatie.DataEnd);
                DateTime dataRataPrecedenta = imprumut.Rate.OrderBy(f => f.DataPlataRata).LastOrDefault(f => f.DataPlataRata < rataAferentaOperatiei.DataPlataRata)?.DataPlataRata ?? imprumut.DocumentDate;
                Operatie.RataId = rataAferentaOperatiei.Id;
                CurrentUnitOfWork.SaveChanges();

                int totalZileOperatie = (int)(Operatie.DataEnd - Operatie.DataStart).TotalDays;

                int totalZileRate = (int)(rataAferentaOperatiei.DataPlataRata - dataRataPrecedenta).TotalDays;
                decimal dobandaPerZi = rataAferentaOperatiei.SumaDobanda / totalZileRate;

                if(Operatie.DataEnd == rataAferentaOperatiei.DataPlataRata)
                {
                    var sumaOperatiiDobandaComisionPrecedente = _operatieDobandaComisionRepository.GetAll()
                        .Where(f => f.RataId == rataAferentaOperatiei.Id && f.TipOperatieDobandaComision == TipOperatieDobandaComision.dobanda && f.State == State.Active).Sum(f => f.Suma);
                    if(sumaOperatiiDobandaComisionPrecedente == null || sumaOperatiiDobandaComisionPrecedente == 0)
                    {
                        calculDobanda = rataAferentaOperatiei.SumaDobanda;
                    }
                    else
                    {
                        calculDobanda = (decimal)(rataAferentaOperatiei.SumaDobanda - sumaOperatiiDobandaComisionPrecedente);

                    }

                }
                else
                {
                    calculDobanda = totalZileOperatie * dobandaPerZi;
                }

                Operatie.Suma = calculDobanda;
                CurrentUnitOfWork.SaveChanges();





                var documentType = _documentTypeRepository.GetAll().FirstOrDefault(f => f.TypeNameShort == "NC");
                var documentNumber = _autoOperationRepository.GetNextNumberForOperContab(Operatie.DataEnd, documentType.Id).ToString();

                if (documentType == null)
                    throw new Exception("Nu a fost identificat tipul de document pentru inregistrarea imprumutului!");



                var operationChild = new Operation(); // e folosit in cazul in care avem documente in valuta

                var operation = new Operation
                {
                    CurrencyId = localCurrencyId,
                    OperationDate = Operatie.DataEnd,
                    DocumentTypeId = documentType.Id,
                    DocumentNumber = documentNumber,
                    DocumentDate = Operatie.DataEnd,
                    OperationStatus = OperationStatus.Unchecked,
                    State = State.Active,
                    ExternalOperation = true,
                    OperGenerateId = operGenId,
                };

                _operationRepository.Insert(operation);
                CurrentUnitOfWork.SaveChanges();



                decimal exchangeRate = 1;
                if (imprumut.CurrencyId != localCurrencyId)
                {
                    exchangeRate = _exchangeRatesRepository.GetExchangeRateForOper(Operatie.DataEnd, imprumut.CurrencyId, localCurrencyId);

                    operationChild = new Operation // pentr operatiile in valuta... in operation am valorile in lei, iar in operationChild am valorile in valuta
                    {
                        CurrencyId = imprumut.CurrencyId,
                        OperationDate = Operatie.DataEnd,
                        DocumentTypeId = documentType.Id,
                        DocumentNumber = documentNumber,
                        DocumentDate = Operatie.DataEnd,
                        OperationStatus = OperationStatus.Unchecked,
                        State = State.Active,
                        ExternalOperation = true,
                        OperationParentId = operation.Id,
                        OperGenerateId = operGenId,
                    };
                    _operationRepository.Insert(operationChild);
                    CurrentUnitOfWork.SaveChanges();

                }

                DobandaLinieDeCredit(exchangeRate, documentNumber, localCurrencyId, Operatie.DataEnd, calculDobanda, imprumut, operation, operationChild);
                return operation.Id;
            }

            else if (Operatie.TipOperatieDobandaComision == TipOperatieDobandaComision.comision)
            {
                var comisionList = _comisionV2Repository.GetAll().Where(f => f.ImprumutId == imprumut.Id && f.TipComision != TipComisionV2.cheltuialaInAvans && f.TipComision != TipComisionV2.cheltuialaIntegrala && f.State == State.Active && f.DataEnd >= Operatie.DataStart.Date && f.DataStart <= Operatie.DataEnd.Date).ToList();

                decimal[] comisioneCalculate = new decimal[comisionList.Count];

                

                for (var i = Operatie.DataStart.Date; i <= Operatie.DataEnd.Date; i = i.AddDays(1))
                {
                    var soldDisponibil = imprumut.Rate.OrderBy(f => f.DataPlataRata).Where(f => f.DataPlataRata <= i).FirstOrDefault().Sold;
                    var valoareImprumutata = imprumut.Rate.OrderBy(f => f.DataPlataRata).Where(f => f.DataPlataRata <= i).FirstOrDefault().SumaPrincipal;

                    for (int j = 0; j < comisionList.Count; j++)
                    {
                        var comision = comisionList[j];
                        decimal _calculComision = comision.ValoareComision;

                        if (comision.TipValoareComision == TipValoareComision.Procent)
                        {
                            if (comision.TipSumaComision == TipSumaComision.Sold)
                            {
                                _calculComision = soldDisponibil * comision.ValoareComision / 100 / comision.BazaDeCalcul;
                            }
                            else if (comision.TipSumaComision == TipSumaComision.ValoareImprumut)
                            {
                                _calculComision = imprumut.Suma * comision.ValoareComision / 100 / comision.BazaDeCalcul;
                            }
                            else if (comision.TipSumaComision == TipSumaComision.SumaTrasa)
                            {
                                _calculComision = valoareImprumutata * comision.ValoareComision / 100 / comision.BazaDeCalcul;
                            }
                        }

                        comisioneCalculate[j] = comisioneCalculate[j] + _calculComision;
                    }

                }

                var documentTypeComision = _documentTypeRepository.GetAll().ToList().FirstOrDefault(f => f.TypeNameShort == "NC");
                var documentNumberComision = _autoOperationRepository.GetNextNumberForOperContab(Operatie.DataEnd, documentTypeComision.Id).ToString();

                if (documentTypeComision == null)
                    throw new Exception("Nu a fost identificat tipul de document pentru inregistrarea imprumutului!");



                var operationChildComision = new Operation(); // e folosit in cazul in care avem documente in valuta

                var operationComision = new Operation
                {
                    CurrencyId = localCurrencyId,
                    OperationDate = Operatie.DataEnd,
                    DocumentTypeId = documentTypeComision.Id,
                    DocumentNumber = documentNumberComision,
                    DocumentDate = Operatie.DataEnd,
                    OperationStatus = OperationStatus.Unchecked,
                    State = State.Active,
                    ExternalOperation = true,
                    OperGenerateId = operGenId
                };

                _operationRepository.Insert(operationComision);
                CurrentUnitOfWork.SaveChanges();

                decimal exchangeRateComision = 1;
                if (imprumut.CurrencyId != localCurrencyId)
                {
                    exchangeRateComision = _exchangeRatesRepository.GetExchangeRateForOper(Operatie.DataEnd, imprumut.CurrencyId, localCurrencyId);

                    operationChildComision = new Operation // pentr operatiile in valuta... in operation am valorile in lei, iar in operationChild am valorile in valuta
                    {
                        CurrencyId = imprumut.CurrencyId,
                        OperationDate = Operatie.DataEnd,
                        DocumentTypeId = documentTypeComision.Id,
                        DocumentNumber = documentNumberComision,
                        DocumentDate = Operatie.DataEnd,
                        OperationStatus = OperationStatus.Unchecked,
                        State = State.Active,
                        ExternalOperation = true,
                        OperationParentId = operationComision.Id,
                        OperGenerateId = operGenId

                    };
                    _operationRepository.Insert(operationChildComision);
                    CurrentUnitOfWork.SaveChanges();

                }

                for (int i = 0; i < comisionList.Count; i++)
                {
                    ComisionLinieCredit(exchangeRateComision, documentNumberComision, localCurrencyId, Operatie.DataEnd, comisioneCalculate[i], imprumut, comisionList[i], operationComision, operationChildComision);
                }
                return operationComision.Id;
            }

            return 0;

        }
        public void ComisionLinieCredit(decimal exchangeRate, string documentNumber, int localCurrencyId, DateTime OperDate, decimal ValoareComision, Models.Imprumuturi.Imprumut imprumut, ComisionV2 Comision, Operation operation, Operation operationChild)
        {


            var detailList = new List<OperationDetails>();
            var operationDetailChild = new OperationDetails(); // pentru operatiile in valuta
            var operationDetail = new OperationDetails();
            int pozitieSchimbAccountId = 0;
            int contravaloarePozitieSchimbAccountId = 0;
            var creditAccountId = 0;
            var debitAccountId = 0;

            var OperationType = (Comision.ModCalculComision == ModCalculComision.Anual || Comision.ModCalculComision == ModCalculComision.Semestrial || Comision.ModCalculComision == ModCalculComision.Trimestrial) ? (int)ImprumuturiOperType.PlataComisionPeriodic : (int)ImprumuturiOperType.PlataComisionLunar;

            var monog = _autoOperationConfigRepository.GetAll().Where(f => f.AutoOperType == AutoOperationType.Imprumuturi)
                                                   .Where(f => f.State == State.Active && OperDate >= f.StartDate && OperDate <= (f.EndDate ?? OperDate)
                                                   && f.OperationType == OperationType)
                                                   .OrderBy(f => f.EntryOrder)
                                                   .ToList();

            if (monog.Count == 0)
            {
                throw new Exception("Nu exista o monografie definita pentru aceasta operatie");
            }

            foreach (var monogItem in monog)
            {

                if (imprumut.CurrencyId != localCurrencyId)
                {
                    operationDetail = new OperationDetails
                    {
                        ValueCurr = ValoareComision,
                        Value = ValoareComision * exchangeRate,
                        VAT = 0,
                        Details = "Comision" + ", " + documentNumber + " / " + OperDate.ToShortDateString() + ", "
                           + (imprumut.CurrencyId != localCurrencyId ? (", Curs valutar: " + exchangeRate) : "")
                    };
                }
                else
                {
                    operationDetail = new OperationDetails
                    {
                        ValueCurr = 0,
                        Value = ValoareComision,
                        VAT = 0,
                        Details = "Comision" + ", " + documentNumber + " / " + OperDate.ToShortDateString()
                    };
                }


                // debit
                if (monogItem.DebitAccount == "CB")
                {

                    debitAccountId = _accountRepository.GetAll().Where(f => f.BankAccountId == imprumut.PaymentAccountId).FirstOrDefault().Id;

                }
                else if (monogItem.DebitAccount == "CMI")
                {
                    var imprumutDetaliu = _imprumutTipDetalii.GetAll().FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.DebitAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    debitAccountId = _autoOperationRepository.GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, OperDate, localCurrencyId, null);
                }
                else if (monogItem.DebitAccount == "CPP")
                {
                    var imprumutDetaliu = _imprumutTipDetalii.GetAll().FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.DebitAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    debitAccountId = _autoOperationRepository.GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, OperDate, imprumut.CurrencyId, null);
                }
                else if (monogItem.DebitAccount == "CPL")
                {
                    var imprumutDetaliu = _imprumutTipDetalii.GetAll().FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.DebitAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    debitAccountId = _autoOperationRepository.GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, OperDate, imprumut.CurrencyId, null);
                }
                else
                {
                    debitAccountId = _autoOperationRepository.GetAutoAccount(monogItem.DebitAccount, imprumut.ThirdPartyId, DateTime.Now, imprumut.CurrencyId, null);
                }

                // credit
                if (monogItem.CreditAccount == "CB")
                {

                    creditAccountId = _accountRepository.GetAll().Where(f => f.BankAccountId == imprumut.PaymentAccountId).FirstOrDefault().Id;
                }
                else if (monogItem.CreditAccount == "CMI")
                {
                    var imprumutDetaliu = _imprumutTipDetalii.GetAll().FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.CreditAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    creditAccountId = _autoOperationRepository.GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, OperDate, localCurrencyId, null);
                }
                else if (monogItem.CreditAccount == "CPP")
                {
                    var imprumutDetaliu = _imprumutTipDetalii.GetAll().FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.CreditAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    creditAccountId = _autoOperationRepository.GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, OperDate, imprumut.CurrencyId, null);
                }
                else if (monogItem.CreditAccount == "CPL")
                {
                    var imprumutDetaliu = _imprumutTipDetalii.GetAll().FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.CreditAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    creditAccountId = _autoOperationRepository.GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, OperDate, imprumut.CurrencyId, null);
                }
                else
                {
                    creditAccountId = _autoOperationRepository.GetAutoAccount(monogItem.CreditAccount, imprumut.ThirdPartyId, DateTime.Now, imprumut.CurrencyId, null);
                }

                if (imprumut.CurrencyId != localCurrencyId)
                {

                    operationDetailChild = new OperationDetails
                    {
                        ValueCurr = (localCurrencyId != imprumut.CurrencyId) ? ValoareComision : 0,
                        Value = ValoareComision,
                        VAT = 0,
                        Details = "Acordare Comision, Nr." + documentNumber + ", Data " + OperDate.ToShortDateString()
                    };

                    // iau pozitia de schimb valutar
                    try
                    {
                        pozitieSchimbAccountId = _accountRepository.GetAll().FirstOrDefault(f => f.AccountFuncType == AccountFuncType.PozitieSchimbImprumuturi && f.CurrencyId == imprumut.CurrencyId
                                                                                            && f.ActivityTypeId == imprumut.ActivityTypeId).Id;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Nu am identificat contul pentru pozitia de schimb valutar");
                    }
                    // iau contravaloarea pozitiei de schimb valutar
                    try
                    {
                        var denumireMoneda = _currencyRepository.GetCurrencyById(imprumut.CurrencyId).CurrencyCode;
                        contravaloarePozitieSchimbAccountId = _accountRepository.GetAll().FirstOrDefault(f => f.AccountFuncType == AccountFuncType.ContravaloarePozitieImprumuturi
                                                                                                         && f.ActivityTypeId == imprumut.ActivityTypeId
                                                                                                         && f.AccountName.IndexOf(denumireMoneda) >= 0).Id;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Nu am identificat contul pentru contravaloarea pozitiei de schimb valutar");
                    }

                    operationDetailChild.DebitId = pozitieSchimbAccountId;
                    operationDetailChild.CreditId = creditAccountId;
                    operationDetail.DebitId = debitAccountId;
                    operationDetail.CreditId = contravaloarePozitieSchimbAccountId;

                    operationDetailChild.OperationId = operationChild.Id;
                    _operationDetailsRepository.Insert(operationDetailChild);
                    CurrentUnitOfWork.SaveChanges();
                }
                else
                {
                    operationDetail.DebitId = debitAccountId;
                    operationDetail.CreditId = creditAccountId;

                    operationDetail.Value = ValoareComision;
                }

                operationDetail.OperationId = operation.Id;

                bool okAddNededOper = true;
                var nededOperDetail = _autoOperationRepository.GenerateNededOperDetail(ref operationDetail, DateTime.Now, debitAccountId, out okAddNededOper);
                

                detailList.Add(operationDetail);
                _operationDetailsRepository.Insert(operationDetail);
                CurrentUnitOfWork.SaveChanges();

                if (okAddNededOper)
                {
                    detailList.Add(nededOperDetail);
                    _operationDetailsRepository.Insert(nededOperDetail);
                }

                CurrentUnitOfWork.SaveChanges();

            }

            CurrentUnitOfWork.SaveChanges();

        }

        public void ComisionInAvansSfarsitDeLuna(int localCurrencyId, int operGenId, DateTime OperDate, Models.Imprumuturi.Imprumut imprumut, ComisionV2 Comision)
        {

            //var TotalLuni = Math.Abs(12 * (Comision.DataStart.Year - Comision.DataEnd.Year) + Comision.DataStart.Month - Comision.DataEnd.Month);


            

            var documentTypeComision = _documentTypeRepository.GetAll().ToList().FirstOrDefault(f => f.TypeNameShort == "NC");
            var documentNumberComision = _autoOperationRepository.GetNextNumberForOperContab(OperDate, documentTypeComision.Id).ToString();

            if (documentTypeComision == null)
                throw new Exception("Nu a fost identificat tipul de document pentru inregistrarea imprumutului!");



            var operationChildComision = new Operation(); // e folosit in cazul in care avem documente in valuta

            var operationComision = new Operation
            {
                CurrencyId = localCurrencyId,
                OperationDate = OperDate,
                DocumentTypeId = documentTypeComision.Id,
                DocumentNumber = documentNumberComision,
                DocumentDate = OperDate,
                OperationStatus = OperationStatus.Unchecked,
                State = State.Active,
                ExternalOperation = true,
                OperGenerateId = operGenId
            };
            _operationRepository.Insert(operationComision);
            CurrentUnitOfWork.SaveChanges();

            decimal exchangeRateComision = 1;

            if (imprumut.CurrencyId != localCurrencyId)
            {
                exchangeRateComision = _exchangeRatesRepository.GetExchangeRateForOper(OperDate, imprumut.CurrencyId, localCurrencyId);

                operationChildComision = new Operation // pentr operatiile in valuta... in operation am valorile in lei, iar in operationChild am valorile in valuta
                {
                    CurrencyId = imprumut.CurrencyId,
                    OperationDate = OperDate,
                    DocumentTypeId = documentTypeComision.Id,
                    DocumentNumber = documentNumberComision,
                    DocumentDate = OperDate,
                    OperationStatus = OperationStatus.Unchecked,
                    State = State.Active,
                    ExternalOperation = true,
                    OperationParentId = operationComision.Id,
                    OperGenerateId = operGenId,
                };
                _operationRepository.Insert(operationChildComision);
                CurrentUnitOfWork.SaveChanges();

            }

            var detailList = new List<OperationDetails>();
            var operationDetailChild = new OperationDetails(); // pentru operatiile in valuta
            var operationDetail = new OperationDetails();
            int pozitieSchimbAccountId = 0;
            int contravaloarePozitieSchimbAccountId = 0;
            var creditAccountId = 0;
            var debitAccountId = 0;

            var OperationType = (int)ImprumuturiOperType.PlataComisionLunar;

            var monog = _autoOperationConfigRepository.GetAll().Where(f => f.AutoOperType == AutoOperationType.Imprumuturi)
                                                   .Where(f => f.State == State.Active && OperDate >= f.StartDate && OperDate <= (f.EndDate ?? OperDate)
                                                   && f.OperationType == OperationType)
                                                   .OrderBy(f => f.EntryOrder)
                                                   .ToList();

            if (monog.Count == 0)
            {
                throw new Exception("Nu exista o monografie definita pentru aceasta operatie");
            }

            foreach (var monogItem in monog)
            {

                if (imprumut.CurrencyId != localCurrencyId)
                {
                    operationDetail = new OperationDetails
                    {
                        ValueCurr = Comision.ValoareCalculata,
                        Value = Comision.ValoareCalculata * exchangeRateComision,
                        VAT = 0,
                        Details = "Comision" + ", Nr. " + documentNumberComision + " / " + OperDate.ToShortDateString() + ", "
                           + (imprumut.CurrencyId != localCurrencyId ? (", Curs valutar: " + exchangeRateComision) : "")
                    };
                }
                else
                {
                    operationDetail = new OperationDetails
                    {
                        ValueCurr = 0,
                        Value = Comision.ValoareCalculata,
                        VAT = 0,
                        Details = "Comision" + ", " + documentNumberComision + " / " + OperDate.ToShortDateString()
                    };
                }



                // debit
                if (monogItem.DebitAccount == "CB")
                {

                    debitAccountId = _accountRepository.GetAll().Where(f => f.BankAccountId == imprumut.PaymentAccountId).FirstOrDefault().Id;

                }
                else if (monogItem.DebitAccount == "CMI")
                {
                    var imprumutDetaliu = _imprumutTipDetalii.GetAll().FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.DebitAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    debitAccountId = _autoOperationRepository.GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, OperDate, localCurrencyId, null);
                }
                else if (monogItem.DebitAccount == "CPP")
                {
                    var imprumutDetaliu = _imprumutTipDetalii.GetAll().FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.DebitAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    debitAccountId = _autoOperationRepository.GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, OperDate, imprumut.CurrencyId, null);
                }
                else if (monogItem.DebitAccount == "CPL")
                {
                    var imprumutDetaliu = _imprumutTipDetalii.GetAll().FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.DebitAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    debitAccountId = _autoOperationRepository.GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, OperDate, imprumut.CurrencyId, null);
                }
                else
                {
                    debitAccountId = _autoOperationRepository.GetAutoAccount(monogItem.DebitAccount, imprumut.ThirdPartyId, DateTime.Now, imprumut.CurrencyId, null);
                }

                // credit
                if (monogItem.CreditAccount == "CB")
                {

                    creditAccountId = _accountRepository.GetAll().Where(f => f.BankAccountId == imprumut.PaymentAccountId).FirstOrDefault().Id;
                }
                else if (monogItem.CreditAccount == "CMI")
                {
                    var imprumutDetaliu = _imprumutTipDetalii.GetAll().FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.CreditAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    creditAccountId = _autoOperationRepository.GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, OperDate, localCurrencyId, null);
                }
                else if (monogItem.CreditAccount == "CPP")
                {
                    var imprumutDetaliu = _imprumutTipDetalii.GetAll().FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.CreditAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    creditAccountId = _autoOperationRepository.GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, OperDate, imprumut.CurrencyId, null);
                }
                else if (monogItem.CreditAccount == "CPL")
                {
                    var imprumutDetaliu = _imprumutTipDetalii.GetAll().FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.CreditAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    creditAccountId = _autoOperationRepository.GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, OperDate, imprumut.CurrencyId, null);
                }
                else
                {
                    creditAccountId = _autoOperationRepository.GetAutoAccount(monogItem.CreditAccount, imprumut.ThirdPartyId, DateTime.Now, imprumut.CurrencyId, null);
                }

                
                if (imprumut.CurrencyId != localCurrencyId)
                {

                    operationDetailChild = new OperationDetails
                    {
                        ValueCurr = (localCurrencyId != imprumut.CurrencyId) ? Comision.ValoareCalculata : 0,
                        Value = Comision.ValoareCalculata,
                        VAT = 0,
                        Details = "Acordare Comision, Nr." + documentNumberComision + ", Data " + OperDate.ToShortDateString()
                    };

                    // iau pozitia de schimb valutar
                    try
                    {
                        pozitieSchimbAccountId = _accountRepository.GetAll().FirstOrDefault(f => f.AccountFuncType == AccountFuncType.PozitieSchimbImprumuturi && f.CurrencyId == imprumut.CurrencyId
                                                                                            && f.ActivityTypeId == imprumut.ActivityTypeId).Id;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Nu am identificat contul pentru pozitia de schimb valutar");
                    }
                    // iau contravaloarea pozitiei de schimb valutar
                    try
                    {
                        var denumireMoneda = _currencyRepository.GetCurrencyById(imprumut.CurrencyId).CurrencyCode;
                        contravaloarePozitieSchimbAccountId = _accountRepository.GetAll().FirstOrDefault(f => f.AccountFuncType == AccountFuncType.ContravaloarePozitieImprumuturi
                                                                                                         && f.ActivityTypeId == imprumut.ActivityTypeId
                                                                                                         && f.AccountName.IndexOf(denumireMoneda) >= 0).Id;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Nu am identificat contul pentru contravaloarea pozitiei de schimb valutar");
                    }

                    operationDetailChild.DebitId = pozitieSchimbAccountId;
                    operationDetailChild.CreditId = creditAccountId;
                    operationDetail.DebitId = debitAccountId;
                    operationDetail.CreditId = contravaloarePozitieSchimbAccountId;

                    operationDetailChild.OperationId = operationChildComision.Id;
                    _operationDetailsRepository.Insert(operationDetailChild);
                    CurrentUnitOfWork.SaveChanges();
                }
                else
                {
                    operationDetail.DebitId = debitAccountId;
                    operationDetail.CreditId = creditAccountId;

                    operationDetail.Value = Comision.ValoareCalculata; 
                }


                operationDetail.OperationId = operationComision.Id;

                bool okAddNededOper = true;
                var nededOperDetail = _autoOperationRepository.GenerateNededOperDetail(ref operationDetail, DateTime.Now, debitAccountId, out okAddNededOper);


                detailList.Add(operationDetail);
                _operationDetailsRepository.Insert(operationDetail);
                CurrentUnitOfWork.SaveChanges();

                if (okAddNededOper)
                {
                    detailList.Add(nededOperDetail);
                    _operationDetailsRepository.Insert(nededOperDetail);
                }

                CurrentUnitOfWork.SaveChanges();


               

            } 
        }

        public void DobandaLinieDeCredit(decimal exchangeRate, string documentNumber, int localCurrencyId, DateTime operData, decimal calculDobanda, Models.Imprumuturi.Imprumut imprumut, Operation operation, Operation operationChild)
        {

            
            var detailList = new List<OperationDetails>();
            var operationDetailChild = new OperationDetails(); // pentru operatiile in valuta
            var operationDetail = new OperationDetails();
            int pozitieSchimbAccountId = 0;
            int contravaloarePozitieSchimbAccountId = 0;
            var creditAccountId = 0;
            var debitAccountId = 0;

            var monog = _autoOperationConfigRepository.GetAll().Where(f => f.AutoOperType == AutoOperationType.Imprumuturi)
                                                   .Where(f => f.State == State.Active && operData >= f.StartDate && operData <= (f.EndDate ?? operData)
                                                   && f.OperationType == (int)ImprumuturiOperType.InregistrareDobandaDatorata)
                                                   .OrderBy(f => f.EntryOrder)
                                                   .ToList();

            if (monog.Count == 0)
            {
                throw new Exception("Nu exista o monografie definita pentru aceasta operatie");
            }

            foreach (var monogItem in monog)
            {


                if (imprumut.CurrencyId != localCurrencyId)
                {
                    operationDetail = new OperationDetails
                    {
                        ValueCurr = calculDobanda,
                        Value = calculDobanda * exchangeRate,
                        VAT = 0,
                        Details = "Inregistrare Dobanda" + ", " + documentNumber + " / " + operData.ToShortDateString() + ", "
                           + (imprumut.CurrencyId != localCurrencyId ? (", Curs valutar: " + exchangeRate) : "")
                    };
                }
                else
                {
                    operationDetail = new OperationDetails
                    {
                        ValueCurr = 0,
                        Value = calculDobanda,
                        VAT = 0,
                        Details = "Inregistrare Dobanda" + ", " + documentNumber + " / " + operData.ToShortDateString()
                    };
                }

                //var monog = Context.AutoOperationConfig.Where(f => f.AutoOperType == AutoOperationType.Dobanda)
                //                                       .Where(f => f.State == State.Active && dobanda.OperationDate >= f.StartDate && dobanda.OperationDate <= (f.EndDate ?? dobanda.OperationDate)
                //                                       && f.OperationType == (state.ImprumuturiStare == ImprumuturiStare.Inregistrat ? (int)ImprumuturiOperType.InregistrareaAngajamentuluiDeFinantare : (int)ImprumuturiOperType.PrimireaImprumuturilor))
                //                                       .OrderBy(f => f.EntryOrder)
                //                                       .ToList();


                // debit
                if (monogItem.DebitAccount == "FDC")
                {
                    var imprumutDetaliu = _imprumutTipDetalii.GetAll().FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.DebitAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    debitAccountId = _autoOperationRepository.GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, imprumut.DocumentDate, localCurrencyId, null);
                }
                else if (monogItem.DebitAccount == "DA")
                {
                    var imprumutDetaliu = _imprumutTipDetalii.GetAll().FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.DebitAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    debitAccountId = _autoOperationRepository.GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, imprumut.DocumentDate, imprumut.CurrencyId, null);
                }
                else
                {
                    debitAccountId = _autoOperationRepository.GetAutoAccount(monogItem.DebitAccount, imprumut.ThirdPartyId, DateTime.Now, imprumut.CurrencyId, null);
                }

                // credit
                if (monogItem.CreditAccount == "FDC")
                {
                    var imprumutDetaliu = _imprumutTipDetalii.GetAll().FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.CreditAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    creditAccountId = _autoOperationRepository.GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, imprumut.DocumentDate, localCurrencyId, null);
                }
                else if (monogItem.CreditAccount == "DA")
                {
                    var imprumutDetaliu = _imprumutTipDetalii.GetAll().FirstOrDefault(f => f.Description == (ImprumuturiTipDetaliuEnum)Enum.Parse(typeof(ImprumuturiTipDetaliuEnum), monogItem.CreditAccount) && f.ActivityTypeId == imprumut.ActivityTypeId && f.ImprumutTipId == imprumut.ImprumuturiTipuriId);
                    creditAccountId = _autoOperationRepository.GetAutoAccount(imprumutDetaliu.ContImprumut, imprumut.ThirdPartyId, imprumut.DocumentDate, imprumut.CurrencyId, null);
                }
                else
                {
                    creditAccountId = _autoOperationRepository.GetAutoAccount(monogItem.CreditAccount, imprumut.ThirdPartyId, DateTime.Now, imprumut.CurrencyId, null);
                }

                if (imprumut.CurrencyId != localCurrencyId)
                {

                    operationDetailChild = new OperationDetails
                    {
                        ValueCurr = (localCurrencyId != imprumut.CurrencyId) ? calculDobanda : 0,
                        Value = calculDobanda,
                        VAT = 0,
                        Details = "Acordare dobanda, Nr." + documentNumber + ", Data " + operData
                    };

                    // iau pozitia de schimb valutar
                    try
                    {
                        pozitieSchimbAccountId = _accountRepository.GetAll().FirstOrDefault(f => f.AccountFuncType == AccountFuncType.PozitieSchimbImprumuturi && f.CurrencyId == imprumut.CurrencyId
                                                                                            && f.ActivityTypeId == imprumut.ActivityTypeId).Id;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Nu am identificat contul pentru pozitia de schimb valutar");
                    }
                    // iau contravaloarea pozitiei de schimb valutar
                    try
                    {
                        var denumireMoneda = _currencyRepository.GetCurrencyById(imprumut.CurrencyId).CurrencyCode;
                        contravaloarePozitieSchimbAccountId = _accountRepository.GetAll().FirstOrDefault(f => f.AccountFuncType == AccountFuncType.ContravaloarePozitieImprumuturi
                                                                                                         && f.ActivityTypeId == imprumut.ActivityTypeId
                                                                                                         && f.AccountName.IndexOf(denumireMoneda) >= 0).Id;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Nu am identificat contul pentru contravaloarea pozitiei de schimb valutar");
                    }

                    operationDetailChild.DebitId = pozitieSchimbAccountId;
                    operationDetailChild.CreditId = creditAccountId;
                    operationDetail.DebitId = debitAccountId;
                    operationDetail.CreditId = contravaloarePozitieSchimbAccountId;

                    operationDetailChild.OperationId = operationChild.Id;
                    _operationDetailsRepository.Insert(operationDetailChild);
                    CurrentUnitOfWork.SaveChanges();
                    
                }
                else
                {
                    operationDetail.DebitId = debitAccountId;
                    operationDetail.CreditId = creditAccountId;

                    operationDetail.Value = calculDobanda;
                }


                operationDetail.OperationId = operation.Id;

                bool okAddNededOper = true;
                var nededOperDetail = _autoOperationRepository.GenerateNededOperDetail(ref operationDetail, DateTime.Now, debitAccountId, out okAddNededOper);
                

                detailList.Add(operationDetail);
                _operationDetailsRepository.Insert(operationDetail);
                CurrentUnitOfWork.SaveChanges();

                if (okAddNededOper)
                {
                    detailList.Add(nededOperDetail);
                    _operationDetailsRepository.Insert(nededOperDetail);
                }

                CurrentUnitOfWork.SaveChanges();

            }
        }



    }
}
