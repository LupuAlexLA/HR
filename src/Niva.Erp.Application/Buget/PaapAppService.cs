using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.Buget.Dto;
using Niva.Erp.Economic;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Models.Buget;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Contracts;
using Niva.Erp.Models.HR;
using Niva.Erp.Repositories.Buget;
using Niva.Erp.Repositories.Conta.Nomenclatures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Buget
{
    public interface IPaapAppService : IApplicationService
    {
        List<PaapDto> GetPAAPList(); // returnez lista achizitiilor

        void PaapSave(PaapEditDto paapEdit); // salvare achizitie

        PaapEditDto GetPaap(int paapId);

        void DeletePaap(int paapId);

        List<PaapStateListDto> getPaapStateListByPaapId(int PaapId);

        List<PaapDto> GetPAAPListByYear(int year);

        List<PaapTranseListDto> GetPAAPListTranseByYear(int year);

        List<PaapDepartamentListDto> GetPAAPByDepartament(int year);

        List<int> GetPAAPYearList();

        void ApprovePaapList(List<PaapDepartamentListDto> paapList, int selectedYear);

        PaapStateEditDto GetPaapStateByPaapId(int paapId);

        void CancelApprovedPaap(PaapStateEditDto paapState);

        decimal? GetAvailableSum(int categoryId, int departamentId, DateTime dataEnd);

        void CancelAllApprovedPaap(int year);

        InvoiceDetailsForPAAPDto GetInvoiceDetails(int paapId);

        PaapDto GetSinglePaap(int paapId);

        void ApproveInvoices(InvoiceDetailsForPAAPDto invoices, int paapId); // Aprob cheltuielile/facturile alocate

        void DeleteAllocatedInvoice(int pappId, int invoiceDetailId); // Sterg facturile/cheltuielile alocate

        void FinalizePAAP(int paapId); // Finalizez alocarea cheltuielilor

        void CancelFinalizePAAP(int paapId); // Anulez finalizarea cheltuielilor pentru achizitia selectata

        void AmanarePAAP(AmanarePaapEditDto amanarePaap);

        void RealocarePaap(RealocarePaapDto realocarePaap);

        PaapEditDto GenerateTranse(PaapEditDto paap);

        PaapEditDto AddTransa(PaapEditDto paap);

        PaapEditDto ReglareTranse(PaapEditDto paap, int index);

        List<PaapDto> CheckEqualSumPAAPTranse(List<PaapDto> paapList);

        List<PaapTranseListDto> GetTranseByPaapId(int paapId);

        List<InvoiceDetailPAAPWithInvoiceElementsDto> GetInvoiceDetailsWithoutPaap();

        bool ApproveInvoicesPentruAlocareFacturi(int invoiceId, int paapId);

        int GetInvoiceDetailsWithoutPaapCount();

        int Nefinalizat30zilePaapListCount();

        int NefinalizatDepasitPaapListCount();

        int GetUserDeptId();

        string GetUserDeptName();
    }

    public class PaapAppService : ErpAppServiceBase, IPaapAppService
    {
        private IBVC_PaapRepository _bvcPaapRepository;
        private IRepository<BVC_PAAP_State> _bvcPaapStateRepository;
        private IRepository<Departament> _departamentRepository;
        private IRepository<BVC_PAAP_AvailableSum> _bvcPaapAvbSumRepository;
        private IRepository<BVC_PAAP_ApprovedYear> _bvcPaapApprovedYearRepository;
        private IRepository<BVC_PAAP_InvoiceDetails> _bvcPaapInvoiceDetailsRepository;
        private IRepository<InvoiceDetails> _invoiceDetailsRepository;
        private IRepository<CotaTVA> _cotaTVARepository;
        private IExchangeRatesRepository _exchangeRatesRepository;
        private IRepository<BVC_PAAPTranse> _bvcPaapTranseRepository;
        private IPersonRepository _personRepository;

        public PaapAppService(IBVC_PaapRepository bvcPaapRepository, IRepository<BVC_PAAP_State> bvcPaapStateRepository, IRepository<Departament> departamentRepository, IRepository<BVC_PAAP_AvailableSum> bvcPaapAvbSumRepository,
                              IRepository<BVC_PAAP_ApprovedYear> bvcPaapApprovedYearRepository, IRepository<BVC_PAAP_InvoiceDetails> bvcPaapInvoiceDetailsRepository,
                              IRepository<InvoiceDetails> invoiceDetailsRepository, IRepository<CotaTVA> cotaTVARepository, IExchangeRatesRepository exchangeRatesRepository,
                              IRepository<BVC_PAAPTranse> bvcPaapTranseRepository, IPersonRepository personRepository)
        {
            _bvcPaapRepository = bvcPaapRepository;
            _bvcPaapStateRepository = bvcPaapStateRepository;
            _departamentRepository = departamentRepository;
            _bvcPaapAvbSumRepository = bvcPaapAvbSumRepository;
            _bvcPaapApprovedYearRepository = bvcPaapApprovedYearRepository;
            _bvcPaapInvoiceDetailsRepository = bvcPaapInvoiceDetailsRepository;
            _invoiceDetailsRepository = invoiceDetailsRepository;
            _cotaTVARepository = cotaTVARepository;
            _exchangeRatesRepository = exchangeRatesRepository;
            _bvcPaapTranseRepository = bvcPaapTranseRepository;
            _personRepository = personRepository;
        }

        public void ApprovePaapList(List<PaapDepartamentListDto> paapList, int selectedYear)
        {
            try
            {
                var operationDate = DateTime.Now;
                var starePaapId = (int)PAAP_State.Inregistrat;
                var list = new List<PaapDto>();

                // modific state-ul achizitiilor => Aprobat
                foreach (var paapDep in paapList)
                {
                    foreach (var item in paapDep.PaapListDetails.Where(f => f.State == State.Active && f.StatePAAPId == starePaapId && f.DataEnd.Year == selectedYear))
                    {
                        _bvcPaapRepository.InsertPAAPState(item.Id, operationDate, PAAP_State.Aprobat, null);
                        list.Add(item);
                    }
                }

                // salvez sumele alocate in functie de categoria de cheltuiala
                var paapDb = ObjectMapper.Map<List<BVC_PAAP>>(list);
                _bvcPaapRepository.SaveAvailableSum(paapDb, selectedYear);

                // salvez anul pentru in care au fost salvate achizitiile
                var approvedYear = _bvcPaapApprovedYearRepository.FirstOrDefault(f => f.Year == selectedYear);
                if (approvedYear == null)
                {
                    var paapApprovedYear = new PaapApprovedYearDto
                    {
                        State = State.Active,
                        Year = selectedYear,
                        TenantId = GetCurrentTenant().Id
                    };

                    var paapApprovedYearDb = ObjectMapper.Map<BVC_PAAP_ApprovedYear>(paapApprovedYear);
                    _bvcPaapApprovedYearRepository.Insert(paapApprovedYearDb);
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public void DeletePaap(int paapId)
        {
            try
            {
                var paapDB = _bvcPaapRepository.GetAll().FirstOrDefault(f => f.Id == paapId && f.State == State.Active);
                paapDB.State = State.Inactive;
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public PaapDto GetSinglePaap(int paapId)
        {
            try
            {
                var paapDB = _bvcPaapRepository.GetAllIncluding(f => f.InvoiceElementsDetails, f => f.InvoiceElementsDetailsCategory, f => f.AssetClassCodes, f => f.Departament, f => f.PaapStateList).FirstOrDefault(f => f.Id == paapId && f.State == State.Active);
                return ObjectMapper.Map<PaapDto>(paapDB);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public PaapEditDto GetPaap(int paapId)
        {
            try
            {
                var cotaTVA = _cotaTVARepository.GetAll().FirstOrDefault(f => f.VAT == 19);
                var appClient = GetCurrentTenant();
                PaapEditDto paap;
                if (paapId == 0)
                {
                    paap = new PaapEditDto
                    {
                        State = State.Active,
                        DataStart = DateTime.Now.AddMonths(-1),
                        DataEnd = DateTime.Now,
                        FirstInstalmentDate = DateTime.Now.AddMonths(-1),
                        CurrencyId = appClient.LocalCurrencyId, // RON by default
                        CotaTVA_Id = cotaTVA?.Id, // 19% by default
                        InvoiceElementsDetailsCategoryId = null,
                        InvoiceElementsDetailsId = null,
                        DepartamentId = null,
                        ModalitateDerulareId = null,
                        LocalCurrencyId = appClient.LocalCurrencyId,
                        NrTranse = 1
                    };

                    var userDeptId = GetUserDeptId();
                    if (userDeptId != 0)
                    {
                        paap.DepartamentId = userDeptId;
                    }

                    var transe = new List<BVC_PAAPTranseDto>();
                    paap.Transe = transe;
                }
                else
                {
                    var paapDB = _bvcPaapRepository.GetAllIncluding(f => f.InvoiceElementsDetails, f => f.InvoiceElementsDetailsCategory, f => f.AssetClassCodes, f => f.Departament, f => f.CotaTVA, f => f.Transe)
                                                   .FirstOrDefault(f => f.Id == paapId && f.State == State.Active);

                    paap = ObjectMapper.Map<PaapEditDto>(paapDB);
                    paap.Transe = paap.Transe.OrderBy(f => f.DataTransa).ToList();
                }
                return paap;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public List<PaapStateListDto> getPaapStateListByPaapId(int PaapId)
        {
            try
            {
                var listDb = _bvcPaapStateRepository.GetAllIncluding(f => f.BVC_PAAP, f => f.BVC_PAAP.Currency, f => f.CotaTVA)
                                                    .Where(f => f.BVC_PAAP_Id == PaapId)
                                                    .OrderByDescending(f => f.OperationDate)
                                                    .ToList();
                var ret = ObjectMapper.Map<List<PaapStateListDto>>(listDb);
                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public List<int> GetPAAPYearList()
        {
            try
            {
                var years = _bvcPaapRepository.GetAll()
                                              .Where(f => f.State == State.Active)
                                              .Select(f => f.DataEnd.Year)
                                              .Distinct()
                                              .OrderByDescending(f => f)
                                              .ToList();
                return years;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public List<PaapDepartamentListDto> GetPAAPByDepartament(int year)
        {
            try
            {
                var list = _bvcPaapRepository.GetAllIncluding(f => f.InvoiceElementsDetails, f => f.InvoiceElementsDetailsCategory, f => f.PaapStateList, f => f.Departament)
                                             .Where(f => f.State == State.Active && f.DataEnd.Year == year)
                                             .ToList();
                var paapList = new List<PaapDepartamentListDto>();
                foreach (var item in list.GroupBy(f => f.DepartamentId))
                {
                    var departament = _departamentRepository.GetAll().FirstOrDefault(f => f.Id == item.Key && f.State == State.Active);
                    var paapDep = new PaapDepartamentListDto
                    {
                        DepartamentName = departament.Name,
                        PaapListDetails = new List<PaapDto>()
                    };

                    var paapDb = list.Where(f => f.DepartamentId == departament.Id).ToList();
                    var paapDto = ObjectMapper.Map<List<PaapDto>>(paapDb);
                    paapDep.PaapListDetails.AddRange(paapDto);

                    paapList.Add(paapDep);
                }
                return paapList;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public List<PaapDto> GetPAAPList()
        {
            try
            {
                var listDb = _bvcPaapRepository.GetAllIncluding(f => f.PaapStateList, f => f.Departament, f => f.Currency)
                                               .Where(f => f.State == State.Active)
                                               .ToList();
                var ret = ObjectMapper.Map<List<PaapDto>>(listDb);
                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        //[AbpAuthorize("Buget.PAAP.Acces")]
        public List<PaapDto> GetPAAPListByYear(int year)
        {
            try
            {
                var tenant = GetCurrentTenant();
                var localCurrencyId = tenant.LocalCurrencyId.Value;
                var listDb = _bvcPaapRepository.GetAllIncluding(f => f.PaapStateList, f => f.Departament, f => f.InvoiceElementsDetailsCategory, f => f.InvoiceElementsDetails,
                                                                f => f.CotaTVA, f => f.Currency, f => f.Transe)
                                               .Where(f => f.State == State.Active && f.DataEnd.Year == year)
                                               .OrderBy(f => f.InvoiceElementsDetailsCategory.CategoryElementDetName)
                                               .ThenBy(f => f.InvoiceElementsDetails.Description)
                                               .ToList();
                var ret = ObjectMapper.Map<List<PaapDto>>(listDb);
                foreach (var item in ret)
                {
                    var count = _bvcPaapInvoiceDetailsRepository.GetAll().Count(f => f.BVC_PAAPId == item.Id);
                    if (count != 0)
                    {
                        var valoareRealizata = _bvcPaapInvoiceDetailsRepository.GetAllIncluding(f => f.InvoiceDetails, f => f.InvoiceDetails.Invoices)
                                                                               .Where(f => f.BVC_PAAPId == item.Id).ToList()
                                                                               .Sum(f => f.InvoiceDetails.Value * _exchangeRatesRepository.GetExchangeRateForOper(f.InvoiceDetails.Invoices.InvoiceDate, f.InvoiceDetails.Invoices.CurrencyId, localCurrencyId));
                        item.ValoareRealizata = valoareRealizata;
                    }
                }
                return ret;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public PaapStateEditDto GetPaapStateByPaapId(int paapId)
        {
            try
            {
                var paapState = _bvcPaapStateRepository.GetAllIncluding(f => f.BVC_PAAP)
                                                       .FirstOrDefault(f => f.BVC_PAAP_Id == paapId && f.State == State.Active);
                var paapStateDto = new PaapStateEditDto
                {
                    OperationDate = null,
                    Id = paapState.Id,
                    PaapId = paapState.BVC_PAAP_Id
                };
                return paapStateDto;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        [AbpAuthorize("Buget.PAAP.Modificare")]
        public void PaapSave(PaapEditDto paapEdit)
        {
            try
            {
                var operationDate = DateTime.Now;
                var appClient = GetCurrentTenant();
                paapEdit.DataStart = new DateTime(paapEdit.DataStart.Year, paapEdit.DataStart.Month, paapEdit.DataStart.Day);
                paapEdit.DataEnd = new DateTime(paapEdit.DataEnd.Year, paapEdit.DataEnd.Month, paapEdit.DataEnd.Day);

                ValidatePaap(paapEdit);

                if (paapEdit.Id != 0)
                {
                    var _transeDb = _bvcPaapTranseRepository.GetAll().Where(f => f.BVC_PAAPId == paapEdit.Id).ToList();
                    foreach (var item in _transeDb)
                    {
                        _bvcPaapTranseRepository.Delete(item.Id);
                    }
                }

                var newPaap = ObjectMapper.Map<BVC_PAAP>(paapEdit);
                newPaap.TenantId = appClient.Id;
                newPaap.NrTranse = newPaap.ContractsPaymentInstalmentFreq == ContractsPaymentInstalmentFreq.Ocazional ? 1 : newPaap.NrTranse;
                newPaap.Transe = null;

                var paapApprovedYear = _bvcPaapApprovedYearRepository.GetAll().FirstOrDefault(f => f.Year == paapEdit.DataEnd.Year && f.State == State.Active);
                newPaap.IsAddedAfterApproval = paapApprovedYear != null ? true : false;

                _bvcPaapRepository.InsertOrUpdate(newPaap);
                CurrentUnitOfWork.SaveChanges();

                foreach (var transa in paapEdit.Transe)
                {
                    transa.BVC_PAAPId = newPaap.Id;
                    var newTransa = ObjectMapper.Map<BVC_PAAPTranse>(transa);
                    newTransa.Id = 0;
                    _bvcPaapTranseRepository.Insert(newTransa);
                }

                _bvcPaapRepository.InsertPAAPState(newPaap.Id, operationDate, PAAP_State.Inregistrat, paapEdit.Comentarii);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message/*ex.ToString()*/);
            }
        }

        private void ValidatePaap(PaapEditDto paapEdit)
        {
            try
            {
                if (paapEdit.DepartamentId == null)
                {
                    throw new Exception("Nu ati specificat departamentul");
                }

                if (paapEdit.ObiectTranzactieId == null)
                {
                    throw new Exception("Nu ati specificat obiectul tranzactiei");
                }

                if (paapEdit.InvoiceElementsDetailsCategoryId == null || paapEdit.InvoiceElementsDetailsId == null)
                {
                    throw new Exception("Nu ati specificat categoria de cheltuiala / cheltuiala");
                }

                if (paapEdit.ValoareTotalaLei == 0)
                {
                    throw new Exception("Valoare totala nu poate fi 0");
                }

                if (paapEdit.AvailableValue == 0)
                {
                    throw new Exception("Achizitia nu poate fi efectuata, deoarece planul a fost aprobat pentru anul " + paapEdit.DataEnd.Year);
                }

                if (paapEdit.ValoareEstimataFaraTvaLei > paapEdit.AvailableValue)
                {
                    throw new Exception("Achizitia nu poate fi efectuata, deoarece suma estimata depaseste valoarea disponibila");
                }

                if (paapEdit.Transe.Count == 0)
                {
                    throw new Exception("Nu ati specificat transele");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CancelApprovedPaap(PaapStateEditDto paapState)
        {
            try
            {
                var appClient = GetCurrentTenant();
                var operationDate = new DateTime(paapState.OperationDate.Value.Year, paapState.OperationDate.Value.Month, paapState.OperationDate.Value.Day);

                //Anulez achizitia si modific state-ul
                _bvcPaapRepository.InsertPAAPState(paapState.PaapId, paapState.OperationDate.Value, PAAP_State.Anulat, paapState.Comentarii);

                // recalculez suma alocata pentru categoria din care face parte achizitia anulata
                var paap = _bvcPaapRepository.GetAllIncluding(f => f.InvoiceElementsDetailsCategory).FirstOrDefault(f => f.Id == paapState.PaapId);

                _bvcPaapRepository.UpdatePaapAvailableValues(paap);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public decimal? GetAvailableSum(int categoryId, int departamentId, DateTime dataEnd)
        {
            try
            {
                var paapAvbSum = _bvcPaapAvbSumRepository.GetAllIncluding(f => f.InvoiceElementsDetailsCategory, f => f.Departament)
                                                         .FirstOrDefault(f => f.InvoiceElementsDetailsCategoryId == categoryId && f.DepartamentId == departamentId && f.State == State.Active && f.ApprovedYear == dataEnd.Year);
                if (paapAvbSum == null)
                {
                    return null;
                }
                return paapAvbSum.Rest;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void CancelAllApprovedPaap(int year)
        {
            try
            {
                var operationDate = DateTime.Now;
                // sterg anul selectat din tabel BVC_PAAP_ApprovedYear
                var paapApprovedYearDb = _bvcPaapApprovedYearRepository.GetAll().FirstOrDefault(f => f.Year == year && f.State == State.Active);

                if (paapApprovedYearDb == null)
                {
                    throw new Exception($"Planul pentru anul {year} nu a fost aprobat");
                }
                _bvcPaapApprovedYearRepository.Delete(paapApprovedYearDb);

                // sterg inregistrarile don BVC_PAAP_AvailableSum
                _bvcPaapRepository.DeleteAllPaapAvbSumForYear(year);

                // schimb state-ul din 'Aprobat' in 'Inregistrat'
                //var paapStateList = _bvcPaapStateRepository.GetAllIncluding(f => f.BVC_PAAP)
                //           .Where(f => f.BVC_PAAP.DataEnd.Year == year && f.BVC_PAAP.State == State.Active).OrderByDescending(f => f.OperationDate)
                //           .ToList();
                //foreach (var item in paapStateList) // aici nu e bine => ar trebui sa faca insert in tabela de stari cu data zilei sau in cazul anularii cu data anularii
                //{
                //    if (item.Paap_State == PAAP_State.Aprobat) // doar pe cele aprobate le modific
                //    {
                //        _bvcPaapRepository.InsertPAAPState(item.BVC_PAAP_Id, operationDate, PAAP_State.Inregistrat, item.Comentarii);
                //    }
                //}

                var paapList = _bvcPaapRepository.GetAllIncluding(f => f.PaapStateList)
                                                 .ToList()
                                                 .Where(f => f.DataEnd.Year == year && f.State == State.Active && f.GetPaapState == PAAP_State.Aprobat)
                                                 .ToList();
                foreach (var item in paapList) // aici nu e bine => ar trebui sa faca insert in tabela de stari cu data zilei sau in cazul anularii cu data anularii
                {
                    _bvcPaapRepository.InsertPAAPState(item.Id, operationDate, PAAP_State.Inregistrat, "Anulare aprobare");
                }

                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        //returnez lista facturilor alocate si disponibile pentru PAAP-ul selectat
        public InvoiceDetailsForPAAPDto GetInvoiceDetails(int paapId)
        {
            try
            {
                int localCurrecyId = GetCurrentTenant().LocalCurrencyId.Value;
                var paap = _bvcPaapRepository.GetAllIncluding(f => f.InvoiceElementsDetails, f => f.InvoiceElementsDetailsCategory).FirstOrDefault(f => f.Id == paapId && f.State == State.Active);
                var paapYear = paap.DataEnd.Year;

                var invoiceDetailsForPaapIdList = _bvcPaapInvoiceDetailsRepository.GetAll().Where(f => f.BVC_PAAPId == paapId).Select(f => f.InvoiceDetailsId).ToList();
                var paapInvoiceDetailsList = _bvcPaapInvoiceDetailsRepository.GetAll().Where(f => !invoiceDetailsForPaapIdList.Contains(f.Id)).Select(f => f.InvoiceDetailsId).ToList();

                var invoiceDetailsAllocatedToPAAP = _invoiceDetailsRepository.GetAllIncluding(f => f.InvoiceElementsDetails, f => f.InvoiceElementsDetails.InvoiceElementsDetailsCategory, f => f.Invoices, f => f.Invoices.Currency)
                                                                             .Where(f => invoiceDetailsForPaapIdList.Contains(f.Id) && f.State == State.Active && f.Invoices.State == State.Active && f.InvoiceElementsDetails.InvoiceElementsDetailsCategoryId == paap.InvoiceElementsDetailsCategoryId)
                                                                             .Select(g => new InvoiceDetailPAAPDto
                                                                             {
                                                                                 Id = g.Id,
                                                                                 CurrencyId = g.Invoices.CurrencyId,
                                                                                 CurrencyName = g.Invoices.Currency.CurrencyCode,
                                                                                 DetailValue = g.Value * g.Quantity,
                                                                                 DetailValueLocalCurr = g.Invoices.Currency.CurrencyCode != "RON" ? g.Invoices.ValueLocalCurr : (g.Value * g.Quantity),
                                                                                 InvoiceDate = g.Invoices.InvoiceDate,
                                                                                 InvoiceNumber = g.Invoices.InvoiceNumber,
                                                                                 InvoiceSeries = g.Invoices.InvoiceSeries,
                                                                                 Selected = true,
                                                                                 ThirdPartyAccount = g.Invoices.ThirdParty.FullName,
                                                                                 Value = g.Invoices.Value,
                                                                             })
                                                                             .ToList();

                var invoiceDetailAvailableForPAAP = _invoiceDetailsRepository.GetAllIncluding(f => f.InvoiceElementsDetails, f => f.InvoiceElementsDetails.InvoiceElementsDetailsCategory)
                                                                             .Where(f => !paapInvoiceDetailsList.Contains(f.Id) && f.State == State.Active && f.Invoices.State == State.Active
                                                                                    && f.InvoiceElementsDetailsId == paap.InvoiceElementsDetailsId && f.Invoices.OperationDate.Year == paapYear)
                                                                               .Select(g => new InvoiceDetailPAAPDto
                                                                               {
                                                                                   Id = g.Id,
                                                                                   CurrencyId = g.Invoices.CurrencyId,
                                                                                   CurrencyName = g.Invoices.Currency.CurrencyCode,
                                                                                   DetailValue = g.Value * g.Quantity,
                                                                                   DetailValueLocalCurr = g.Invoices.Currency.CurrencyCode != "RON" ? g.Invoices.ValueLocalCurr : (g.Value * g.Quantity),
                                                                                   InvoiceDate = g.Invoices.InvoiceDate,
                                                                                   InvoiceNumber = g.Invoices.InvoiceNumber,
                                                                                   InvoiceSeries = g.Invoices.InvoiceSeries,
                                                                                   Selected = false,
                                                                                   ThirdPartyAccount = g.Invoices.ThirdParty.FullName,
                                                                                   Value = g.Invoices.Value,
                                                                               })
                                                                             .ToList();

                var invoiceDetailsPAAPDto = new InvoiceDetailsForPAAPDto
                {
                    InvoiceDetailsAllocatedForPAAP = invoiceDetailsAllocatedToPAAP,
                    InvoiceDetailsAvailableForPAAP = invoiceDetailAvailableForPAAP
                };

                return invoiceDetailsPAAPDto;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public void ApproveInvoices(InvoiceDetailsForPAAPDto invoices, int paapId)
        {
            try
            {
                var bvcPaap = _bvcPaapRepository.FirstOrDefault(f => f.Id == paapId && f.State == State.Active);
                if (invoices.InvoiceDetailsAvailableForPAAP.Where(f => f.Selected == true).Sum(f => f.DetailValueLocalCurr).Value > bvcPaap.ValoareEstimataFaraTvaLei)
                {
                    throw new Exception("Valoarea facturilor selectate depaseste valoarea achizitiei");
                }
                foreach (var item in invoices.InvoiceDetailsAvailableForPAAP.Where(f => f.Selected == true))
                {
                    // salvez id-urile in table BVC_PAAP_invoiceDetails
                    var bvcPaapInvoiceDetailsDto = new BVC_PAAP_InvoiceDetailsDto
                    {
                        BVC_PAAPId = paapId,
                        InvoiceDetailsId = item.Id,
                        TenantId = GetCurrentTenant().Id
                    };

                    var paapInvoiceDetails = ObjectMapper.Map<BVC_PAAP_InvoiceDetails>(bvcPaapInvoiceDetailsDto);

                    _bvcPaapInvoiceDetailsRepository.Insert(paapInvoiceDetails);
                    CurrentUnitOfWork.SaveChanges();
                }

                if (invoices.InvoiceDetailsAvailableForPAAP.Where(f => f.Selected == true).Sum(f => f.DetailValueLocalCurr).Value == bvcPaap.ValoareEstimataFaraTvaLei)
                {
                    FinalizePAAP(paapId);
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public void DeleteAllocatedInvoice(int pappId, int invoiceDetailId)
        {
            try
            {
                var operationDate = DateTime.Now;
                // sterg legatura dintre PAAP si InvoiceDetails
                var paapInvoiceDetail = _bvcPaapInvoiceDetailsRepository.FirstOrDefault(f => f.BVC_PAAPId == pappId && f.InvoiceDetailsId == invoiceDetailId);
                _bvcPaapInvoiceDetailsRepository.Delete(paapInvoiceDetail);
                CurrentUnitOfWork.SaveChanges();

                // schimb state-ul PAAP-ului daca nu are nicio factura selectata
                var countPaapInvoiceDetails = _bvcPaapInvoiceDetailsRepository.GetAll().Where(f => f.BVC_PAAPId == pappId).Count();
                if (countPaapInvoiceDetails == 0)
                {
                    _bvcPaapRepository.InsertPAAPState(pappId, operationDate, PAAP_State.Aprobat, null);
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public void FinalizePAAP(int paapId)
        {
            try
            {
                var operationDate = DateTime.Now;
                _bvcPaapRepository.InsertPAAPState(paapId, operationDate, PAAP_State.Finalizat, null);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public void CancelFinalizePAAP(int paapId)
        {
            try
            {
                var operationDate = DateTime.Now;
                _bvcPaapRepository.InsertPAAPState(paapId, operationDate, PAAP_State.Aprobat, null);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public void AmanarePAAP(AmanarePaapEditDto amanarePaap)
        {
            try
            {
                // schimb data de sfarsit a PAAP-ului
                var paap = _bvcPaapRepository.GetAll().FirstOrDefault(f => f.State == State.Active && f.Id == amanarePaap.PaapId);
                var origDataEnd = paap.DataEnd;
                paap.DataEnd = amanarePaap.DataEnd;
                paap.FirstInstalmentDate = paap.FirstInstalmentDate.AddDays((amanarePaap.DataEnd - origDataEnd).TotalDays);
                _bvcPaapRepository.Update(paap);
                CurrentUnitOfWork.SaveChanges();

                // salvez starea 'Amanat' in BVC_PAAP_State
                var operationDate = DateTime.Now;
                _bvcPaapRepository.InsertPAAPState(amanarePaap.PaapId, operationDate, PAAP_State.Amanat, amanarePaap.Comentarii);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public void RealocarePaap(RealocarePaapDto realocarePaap)
        {
            try
            {
                // schimb data de sfarsit a PAAP-ului
                var paap = _bvcPaapRepository.GetAllIncluding(f => f.PaapStateList).FirstOrDefault(f => f.State == State.Active && f.Id == realocarePaap.PaapId);

                var origDataEnd = paap.DataEnd;
                paap.DataEnd = realocarePaap.DataEnd;
                paap.FirstInstalmentDate = paap.FirstInstalmentDate.AddDays((paap.DataEnd - origDataEnd).TotalDays);

                decimal sumaLei = 0;
                decimal sumaLeiFaraTVA = 0;

                // updatez valorile din transe
                foreach (var item in realocarePaap.Transe)
                {
                    var cotaTVA = _cotaTVARepository.FirstOrDefault(f => f.Id == paap.CotaTVA_Id);
                    var transeDB = ObjectMapper.Map<BVC_PAAPTranse>(item);
                    transeDB.ValoareLeiFaraTVA = Convert.ToDecimal(String.Format("{0:0.00}", transeDB.ValoareLei / (100 + cotaTVA.VAT) * 100));
                    sumaLeiFaraTVA += transeDB.ValoareLeiFaraTVA;
                    sumaLei += transeDB.ValoareLei;
                    if (transeDB.Id == 0)
                    {
                        _bvcPaapTranseRepository.Insert(transeDB);
                    }
                    else
                    {
                        _bvcPaapTranseRepository.Update(transeDB);
                    }
                }

                paap.ValoareTotalaLei = sumaLei;
                paap.ValoareEstimataFaraTvaLei = sumaLeiFaraTVA;
                _bvcPaapRepository.Update(paap);

                // salvez restul in BVC_PAAP_AvailableSum
                InsertOrUpdateAvailableSumForPaap(paap, realocarePaap, paap.DataEnd.Year);

                // salvez starea achizitiei in  in BVC_PAAP_State
                var operationDate = DateTime.Now;
                _bvcPaapRepository.InsertPAAPState(realocarePaap.PaapId, operationDate, paap.GetPaapState, realocarePaap.Comentarii);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public PaapEditDto GenerateTranse(PaapEditDto paap)
        {
            try
            {
                var transe = new List<BVC_PAAPTranseDto>();

                if ((ContractsPaymentInstalmentFreq)paap.ContractsPaymentInstalmentFreqId == ContractsPaymentInstalmentFreq.Ocazional)
                {
                    var transa = new BVC_PAAPTranseDto
                    {
                        DataTransa = paap.FirstInstalmentDate,
                        ValoareLei = paap.ValoareTotalaLei,
                        ValoareLeiFaraTVA = paap.ValoareEstimataFaraTvaLei
                    };
                    transe.Add(transa);
                }
                else
                {
                    int nrTranse = paap.NrTranse;
                    int pasTranse = 0;
                    var dataTransa = paap.FirstInstalmentDate;
                    switch ((ContractsPaymentInstalmentFreq)paap.ContractsPaymentInstalmentFreqId)
                    {
                        case ContractsPaymentInstalmentFreq.Lunar:
                            pasTranse = 1;
                            break;

                        case ContractsPaymentInstalmentFreq.Trimestrial:
                            pasTranse = 3;
                            break;

                        case ContractsPaymentInstalmentFreq.Semestrial:
                            pasTranse = 6;
                            break;
                    }

                    while (nrTranse != 0)
                    {
                        var valLei = Math.Round(paap.ValoareTotalaLei / paap.NrTranse, 2);
                        var valLeiFaraTva = Math.Round(paap.ValoareEstimataFaraTvaLei / paap.NrTranse, 2);

                        var transa = new BVC_PAAPTranseDto
                        {
                            DataTransa = dataTransa,
                            ValoareLei = valLei,
                            ValoareLeiFaraTVA = valLeiFaraTva
                        };
                        transe.Add(transa);
                        nrTranse--;
                        dataTransa = dataTransa.AddMonths(pasTranse);
                    }
                }
                paap.Transe = transe;
                return paap;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public PaapEditDto AddTransa(PaapEditDto paap)
        {
            try
            {
                var transa = new BVC_PAAPTranseDto
                {
                    DataTransa = DateTime.Now,
                    ValoareLei = 0,
                    ValoareLeiFaraTVA = 0
                };

                paap.Transe.Add(transa);
                return paap;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public PaapEditDto ReglareTranse(PaapEditDto paap, int index)
        {
            try
            {
                decimal sumLeiCuTVA = 0;
                decimal sumLeiFaraTVA = 0;
                for (int i = 0; i < paap.NrTranse; i++)
                {
                    if (i != index)
                    {
                        sumLeiCuTVA += paap.Transe[i].ValoareLei;
                        sumLeiFaraTVA += paap.Transe[i].ValoareLeiFaraTVA;
                    }
                }

                paap.Transe[index].ValoareLei = paap.ValoareTotalaLei - sumLeiCuTVA;
                paap.Transe[index].ValoareLeiFaraTVA = paap.ValoareEstimataFaraTvaLei - sumLeiFaraTVA;

                return paap;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        //[AbpAuthorize("Buget.PAAP.Acces")]
        public List<PaapTranseListDto> GetPAAPListTranseByYear(int year)
        {
            try
            {
                var tenant = GetCurrentTenant();
                var localCurrencyId = tenant.LocalCurrencyId.Value;
                var paapIdsList = _bvcPaapRepository.GetAllIncluding(f => f.PaapStateList, f => f.Departament, f => f.PaapStateList, f => f.InvoiceElementsDetailsCategory, f => f.InvoiceElementsDetails,
                                                                f => f.CotaTVA, f => f.Currency)
                                               .Where(f => f.State == State.Active && f.DataEnd.Year == year)
                                               .OrderBy(f => f.InvoiceElementsDetailsCategory.CategoryElementDetName)
                                               .ThenBy(f => f.InvoiceElementsDetails.Description)
                                               .Select(f => f.Id)
                                               .ToList();
                var transeList = _bvcPaapTranseRepository.GetAllIncluding(f => f.BVC_PAAP, f => f.BVC_PAAP.PaapStateList, f => f.BVC_PAAP.InvoiceElementsDetailsCategory, f => f.BVC_PAAP.InvoiceElementsDetails)
                                                         .Where(f => paapIdsList.Contains(f.BVC_PAAPId) && f.DataTransa.Year == year)
                                                         .Select(f => new PaapTranseListDto
                                                         {
                                                             Id = f.Id,
                                                             BVC_PAAPId = f.BVC_PAAPId,
                                                             DataTransa = f.DataTransa,
                                                             ValoareLei = f.ValoareLei,
                                                             ValoareLeiFaraTVA = f.ValoareLeiFaraTVA,
                                                             InvoiceElementsDetailsCategoryId = f.BVC_PAAP.InvoiceElementsDetailsCategoryId,
                                                             InvoiceElementsDetailsId = f.BVC_PAAP.InvoiceElementsDetailsId,
                                                             InvoiceElementsDetailsCategoryName = f.BVC_PAAP.InvoiceElementsDetailsCategory.CategoryElementDetName,
                                                             InvoiceElementsDetailsName = f.BVC_PAAP.InvoiceElementsDetails.Description,
                                                             StatePAAP = f.BVC_PAAP.GetPaapState.ToString()
                                                         })
                                                         .ToList();

                return transeList;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }

        public List<PaapDto> CheckEqualSumPAAPTranse(List<PaapDto> paapList)
        {
            try
            {
                foreach (var paap in paapList)
                {
                    var transeList = _bvcPaapTranseRepository.GetAllIncluding(f => f.BVC_PAAP).Where(f => f.BVC_PAAPId == paap.Id).ToList();
                    if (transeList.Count > 0)
                    {
                        paap.IsValueEqualToSumTranse = transeList.Sum(f => f.ValoareLei) == paap.ValoareTotalaLei;
                        paap.HasTranse = true;
                    }
                }
                return paapList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PaapTranseListDto> GetTranseByPaapId(int paapId)
        {
            try
            {
                var transeList = _bvcPaapTranseRepository.GetAllIncluding(f => f.BVC_PAAP)
                                                         .Where(f => f.BVC_PAAPId == paapId)
                                                         .Select(f => new PaapTranseListDto
                                                         {
                                                             Id = f.Id,
                                                             DataTransa = f.DataTransa,
                                                             BVC_PAAPId = f.BVC_PAAPId,
                                                             ValoareLeiFaraTVA = f.ValoareLeiFaraTVA,
                                                             ValoareLei = f.ValoareLei
                                                         })
                                                         .OrderBy(f => f.DataTransa)
                                                         .ToList();
                return transeList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<InvoiceDetailPAAPWithInvoiceElementsDto> GetInvoiceDetailsWithoutPaap()
        {
            var associatedpaap = _bvcPaapInvoiceDetailsRepository.GetAllIncluding(f => f.InvoiceDetails).ToList();
            var year = LazyMethods.Now().Year;

            var invoiceDetailAvailable = _invoiceDetailsRepository.GetAllIncluding(f => f.InvoiceElementsDetails, f => f.InvoiceElementsDetails.InvoiceElementsDetailsCategory, f => f.Invoices, f => f.Invoices.ThirdParty, f => f.Invoices.Currency).ToList()
                                                                             .Where(f => !associatedpaap.Any(y => y.InvoiceDetailsId == f.Id) && f.State == State.Active && f.Invoices.State == State.Active && f.Invoices.InvoiceDate.Year == year)
                                                                               .Select(g => new InvoiceDetailPAAPWithInvoiceElementsDto
                                                                               {
                                                                                   Id = g.Id,
                                                                                   CurrencyId = g.Invoices.CurrencyId,
                                                                                   CurrencyName = g.Invoices.Currency.CurrencyCode,
                                                                                   DetailValue = g.Value * g.Quantity,
                                                                                   DetailValueLocalCurr = g.Invoices.Currency.CurrencyCode != "RON" ? g.Invoices.ValueLocalCurr : (g.Value * g.Quantity),
                                                                                   InvoiceDate = g.Invoices.InvoiceDate,
                                                                                   InvoiceNumber = g.Invoices.InvoiceNumber,
                                                                                   InvoiceSeries = g.Invoices.InvoiceSeries,
                                                                                   Selected = false,
                                                                                   ThirdPartyAccount = g.Invoices.ThirdParty.FullName,
                                                                                   Value = g.Invoices.Value,
                                                                                   InvoiceElementsDetailsId = g.InvoiceElementsDetailsId,
                                                                                   DetailDescription = g.Invoices.Description + " - " + g.Element
                                                                               }).OrderBy(f => f.InvoiceDate).ThenBy(f => f.ThirdPartyAccount)
                                                                             .ToList();

            foreach (var value in invoiceDetailAvailable)
            {
                var paapAvailableForInvoiceDetail = _bvcPaapRepository.GetAllIncluding(f => f.InvoiceElementsDetails, f => f.InvoiceElementsDetails.InvoiceElementsDetailsCategory)
                                                                             .Where(f => f.State == State.Active
                                                                                    && f.InvoiceElementsDetailsId == value.InvoiceElementsDetailsId && f.DataEnd.Year == value.InvoiceDate.Year)
                                                                             .Select(f => new BVC_PAAP_Description_Date
                                                                             {
                                                                                 BVC_PAAP_Id = f.Id,
                                                                                 DataEnd = f.DataEnd,
                                                                                 Descriere = f.Descriere,
                                                                                 Rest = f.ValoareEstimataFaraTvaLei,
                                                                             })
                                                                             .ToList();

                foreach (var item in paapAvailableForInvoiceDetail)
                {
                    item.Rest = item.Rest - (associatedpaap.Where(g => g.BVC_PAAPId == item.BVC_PAAP_Id).Any() ? associatedpaap.Where(g => g.BVC_PAAPId == item.BVC_PAAP_Id).Select(g => g.InvoiceDetails.Value).DefaultIfEmpty(0).Sum() : 0);
                }

                value.PossiblePaap = paapAvailableForInvoiceDetail;
            }

            return invoiceDetailAvailable;
        }

        public int GetInvoiceDetailsWithoutPaapCount()
        {
            var year = LazyMethods.Now().Year;
            var associatedpaap = _bvcPaapInvoiceDetailsRepository.GetAllIncluding(f => f.InvoiceDetails).ToList();
            var invoiceDetailAvailableCount = _invoiceDetailsRepository.GetAllIncluding(f => f.InvoiceElementsDetails, f => f.InvoiceElementsDetails.InvoiceElementsDetailsCategory, f => f.Invoices, f => f.Invoices.ThirdParty, f => f.Invoices.Currency).ToList()
                                                                             .Where(f => !associatedpaap.Any(y => y.InvoiceDetailsId == f.Id) && f.State == State.Active && f.Invoices.State == State.Active && f.Invoices.InvoiceDate.Year == year).Count();
            return invoiceDetailAvailableCount;
        }

        public bool ApproveInvoicesPentruAlocareFacturi(int invoiceId, int paapId)
        {
            try
            {
                var bvcPaap = _bvcPaapRepository.FirstOrDefault(f => f.Id == paapId && f.State == State.Active);
                var totalFacturiAlocate = _bvcPaapInvoiceDetailsRepository.GetAllIncluding(f => f.InvoiceDetails).Where(f => f.BVC_PAAPId == paapId).Sum(f => f.InvoiceDetails.Value);
                totalFacturiAlocate += _invoiceDetailsRepository.GetAll().FirstOrDefault(f => f.Id == invoiceId).Value;
                if (totalFacturiAlocate > bvcPaap.ValoareEstimataFaraTvaLei)
                {
                    throw new Exception("Valoarea facturilor selectate depaseste valoarea achizitiei");
                }

                // salvez id-urile in table BVC_PAAP_invoiceDetails
                var bvcPaapInvoiceDetailsDto = new BVC_PAAP_InvoiceDetailsDto
                {
                    BVC_PAAPId = paapId,
                    InvoiceDetailsId = invoiceId,
                    TenantId = GetCurrentTenant().Id
                };

                var paapInvoiceDetails = ObjectMapper.Map<BVC_PAAP_InvoiceDetails>(bvcPaapInvoiceDetailsDto);

                _bvcPaapInvoiceDetailsRepository.Insert(paapInvoiceDetails);
                CurrentUnitOfWork.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public int Nefinalizat30zilePaapListCount()
        {
            return GetPAAPListByYear(LazyMethods.Now().Year).Where(f => (f.DataEnd - LazyMethods.Now()).TotalDays < 30 && (f.DataEnd - LazyMethods.Now()).TotalDays > 0 && f.StatePAAP != "Finalizat").Count();
        }

        public int NefinalizatDepasitPaapListCount()
        {
            return GetPAAPListByYear(LazyMethods.Now().Year).Where(f => (f.DataEnd - LazyMethods.Now()).TotalDays < 0 && f.StatePAAP != "Finalizat").Count();
        }

        public int GetUserDeptId()
        {
            var rez = 0;
            try
            {
                long? userId = AbpSession.UserId;
                var dept = _personRepository.GetUserDeptId(userId.Value);
                if (dept != null)
                {
                    rez = dept.Id;
                }
            }
            catch
            {
            }
            return rez;
        }

        public string GetUserDeptName()
        {
            string rez = "";
            try
            {
                long? userId = AbpSession.UserId;
                var dept = _personRepository.GetUserDeptId(userId.Value);
                if (dept != null)
                {
                    rez = dept.Name;
                }
            }
            catch
            {
            }
            return rez;
        }

        private void InsertOrUpdateAvailableSumForPaap(BVC_PAAP paap, RealocarePaapDto realocarePaap, int selectedYear)
        {
            var tenantId = 1;

            var paapAvbSum = _bvcPaapAvbSumRepository
                                        .FirstOrDefault(f => f.InvoiceElementsDetailsCategoryId == paap.InvoiceElementsDetailsCategoryId && f.DepartamentId == paap.DepartamentId && f.State == State.Active && f.ApprovedYear == paap.DataEnd.Year);
            if (paapAvbSum == null)
            {
                paapAvbSum = new BVC_PAAP_AvailableSum();
                paapAvbSum.InvoiceElementsDetailsCategoryId = paap.InvoiceElementsDetailsCategoryId;
                paapAvbSum.DepartamentId = paap.DepartamentId;
                paapAvbSum.TenantId = tenantId;

                if (paapAvbSum.Id == 0)
                {
                    paapAvbSum.SumApproved += paap.ValoareEstimataFaraTvaLei;
                }
                paapAvbSum.SumAllocated += paap.ValoareEstimataFaraTvaLei;

                paapAvbSum.Rest = paapAvbSum.SumApproved - paapAvbSum.SumAllocated;
                paapAvbSum.ApprovedYear = selectedYear;
            }

            var sumTranse = realocarePaap.Transe.Sum(f => f.ValoareLei);
            if (realocarePaap.ValoareTotala < sumTranse)
            {
                paapAvbSum.Rest = (realocarePaap.ValoareTotala - sumTranse);
            }
            else
            {
                paapAvbSum.Rest = (realocarePaap.ValoareTotala - sumTranse);
            }

            var paapAvbSumToDb = ObjectMapper.Map<BVC_PAAP_AvailableSum>(paapAvbSum);

            if (paapAvbSum.Id == 0)
            {
                // insert
                _bvcPaapAvbSumRepository.Insert(paapAvbSum);
            }
            else
            { // update
                _bvcPaapAvbSumRepository.Update(paapAvbSum);
            }
        }
    }
}