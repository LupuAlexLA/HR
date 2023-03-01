using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Niva.Erp.Buget.Dto;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Models.Buget;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Contracts;
using Niva.Erp.Repositories.Buget;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Buget
{
    public interface IPaapRedistribuireAppService : IApplicationService
    {
        List<PaapRedistribuireListDto> GetPaapRedistribuireList(int year);

        List<PaapPrimesteDto> GetPaapPrimList(int year);

        List<PaapPierdeDto> GetPaapPierdeList(int year);

        List<PaapRedistribuireDetaliiDto> GetPaapRedistribDetalii(int paapPrimesteId);

        void DeleteAchizitieRedistribuita(int achizRedistribId);

        void Save(PaapRedistribuireDto redistribuire);

        PaapRedistribuireDto GetPaapRedistribuire(int paapRedistribuireId);
    }

    public class PaapRedistribuireAppService : ErpAppServiceBase, IPaapRedistribuireAppService
    {
        private IRepository<BVC_PaapRedistribuire> _paapRedistribuireRepository;
        private IRepository<BVC_PAAP_AvailableSum> _bvcPaapAvbSumRepository;
        private IBVC_PaapRepository _bvcPaapRepository;
        private IRepository<BVC_PaapRedistribuireDetalii> _paapRedistribuireDetaliiRepository;
        private IRepository<BVC_PAAPTranse> _bvcPaapTranseRepository;
        private IBVC_Notificare _bvcNotificare;

        public PaapRedistribuireAppService(IRepository<BVC_PaapRedistribuire> paapRedistribuireRepository,
            IRepository<BVC_PaapRedistribuireDetalii> paapRedistribuireDetaliiRepository,
            IRepository<BVC_PAAP_AvailableSum> bvcPaapAvbSumRepository, IBVC_PaapRepository bvcPaapRepository,
            IRepository<BVC_PAAPTranse> bvcPaapTranseRepository, IBVC_Notificare bvcNotificare)
        {
            _paapRedistribuireRepository = paapRedistribuireRepository;
            _bvcPaapAvbSumRepository = bvcPaapAvbSumRepository;
            _bvcPaapRepository = bvcPaapRepository;
            _paapRedistribuireDetaliiRepository = paapRedistribuireDetaliiRepository;
            _bvcPaapTranseRepository = bvcPaapTranseRepository;
            _bvcNotificare = bvcNotificare;
        }

        public List<PaapPierdeDto> GetPaapPierdeList(int year)
        {
            var appClientId = 1;
            var paapPierdeList = _bvcPaapRepository
                  .GetAllIncluding(f => f.PaapStateList, f => f.InvoiceElementsDetailsCategory, f => f.Departament, f => f.Currency)
                  .ToList()
                  .Where(f => f.DataEnd.Year == year && (f.GetPaapState == PAAP_State.Aprobat ||
                  f.GetPaapState == PAAP_State.Amanat) &&
                              f.ValoareTotalaLei - f.ValoareEstimataFaraTvaLei > 0 &&
                              f.State == State.Active && f.TenantId == appClientId)
                  .OrderBy(f => f.Departament.Name)
                 .ThenBy(f => f.InvoiceElementsDetailsCategory.CategoryElementDetName)
                 .ThenBy(f => f.Descriere)
                 .ToList();

            var paapPierdeListDto = ObjectMapper.Map<List<PaapPierdeDto>>(paapPierdeList);

            return paapPierdeListDto;
        }

        public List<PaapPrimesteDto> GetPaapPrimList(int year)
        {
            var paapPrimList = _bvcPaapRepository
                 .GetAllIncluding(f => f.PaapStateList, f => f.Departament, f => f.InvoiceElementsDetailsCategory, f => f.Currency)
                 .ToList()
                 .Where(f => f.DataEnd.Year == year && (f.GetPaapState == PAAP_State.Aprobat || f.GetPaapState == PAAP_State.Amanat))
                 .OrderBy(f => f.Departament.Name)
                 .ThenBy(f => f.InvoiceElementsDetailsCategory.CategoryElementDetName)
                 .ThenBy(f => f.Descriere)
                 .ToList();
            var paapPrimListDto = ObjectMapper.Map<List<PaapPrimesteDto>>(paapPrimList);
            return paapPrimListDto;
        }

        public List<PaapRedistribuireDetaliiDto> GetPaapRedistribDetalii(int paapPrimesteId)
        {
            var paapRedistribDetaliiListDb = _paapRedistribuireDetaliiRepository
                .GetAllIncluding(f => f.PaapCarePierde.Departament, f => f.PaapCarePierde.InvoiceElementsDetailsCategory, f => f.BVC_PaapRedistribuire)
                .Where(f => f.BVC_PaapRedistribuire.PaapCarePrimesteId == paapPrimesteId)
                .ToList();
            var paapRedistribDetaliiListDto = ObjectMapper.Map<List<PaapRedistribuireDetaliiDto>>(paapRedistribDetaliiListDb);
            return paapRedistribDetaliiListDto;
        }

        public PaapRedistribuireDto GetPaapRedistribuire(int paapRedistribuireId)
        {
            try
            {
                var paapRdistribuire = _paapRedistribuireRepository
                    .GetAllIncluding(f => f.PaapCarePrimeste, f => f.PaapCarePrimeste.Departament, f => f.PaapCarePrimeste.InvoiceElementsDetailsCategory)
                    .Include(f => f.PaapRedistribuireDetalii)
                    .ThenInclude(f => f.PaapCarePierde)
                    .ThenInclude(f => f.InvoiceElementsDetailsCategory)
                    .Include(f => f.PaapRedistribuireDetalii)
                    .ThenInclude(f => f.PaapCarePierde.Departament)
                    .FirstOrDefault(f => f.Id == paapRedistribuireId);

                var paapRdistribuireDto = ObjectMapper.Map<PaapRedistribuireDto>(paapRdistribuire);
                return paapRdistribuireDto;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public void DeleteAchizitieRedistribuita(int achizRedistribId)
        {
            try
            {
                var achizRedistrib = _paapRedistribuireRepository
                    .GetAllIncluding(f => f.PaapRedistribuireDetalii)
                    .FirstOrDefault(f => f.Id == achizRedistribId);

                _paapRedistribuireRepository.Delete(achizRedistrib);
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public void Save(PaapRedistribuireDto redistribuire)
        {
            try
            {
                //salvez BVC_PaapRedistribuire
                var paapRedistribuireDB = ObjectMapper.Map<BVC_PaapRedistribuire>(redistribuire);

                paapRedistribuireDB.DataRedistribuire = LazyMethods.Now();
                paapRedistribuireDB.TenantId = 1;
                _paapRedistribuireRepository.Insert(paapRedistribuireDB);
                CurrentUnitOfWork.SaveChanges();
                UpdatePaapPrimeste(redistribuire);

                // Salvez noitificarea pentru PAAP care primeste
                _bvcNotificare.SaveNotificarePaapPrimeste(paapRedistribuireDB);

                //salvez in BVC_PaapRedistribuireDetalii
                var paapRedistribuireDetalii = redistribuire.PaapRedistribuireDetaliiList;
                SavePaapRedistribuireDetalii(paapRedistribuireDB.Id, paapRedistribuireDetalii);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.ToString());
            }
        }

        public List<PaapRedistribuireListDto> GetPaapRedistribuireList(int year)
        {
            var redistribuireList = _paapRedistribuireRepository
                .GetAllIncluding(f => f.PaapCarePrimeste, f => f.PaapCarePrimeste.Departament, f => f.PaapCarePrimeste.InvoiceElementsDetailsCategory)
                .Where(f => f.PaapCarePrimeste.DataEnd.Year == year)
                .OrderByDescending(f => f.DataRedistribuire)
                .ToList();
            var redistribuireListDto = ObjectMapper.Map<List<PaapRedistribuireListDto>>(redistribuireList);
            return redistribuireListDto;
        }

        private void UpdatePaapPrimeste(PaapRedistribuireDto paapPrimeste)
        {
            var paapDb = _bvcPaapRepository.GetAllIncluding(f => f.CotaTVA, f => f.Transe).FirstOrDefault(f => f.Id == paapPrimeste.PaapPrimesteId);
            paapDb.ValoareTotalaLei += paapPrimeste.SumaPrimita;
            paapDb.ValoareEstimataFaraTvaLei = Math.Round(paapDb.ValoareTotalaLei / (100 + paapDb.CotaTVA.VAT) * 100, 2);
            var availableSum = _bvcPaapAvbSumRepository
                    .GetAllIncluding(f => f.InvoiceElementsDetailsCategory, f => f.Departament)
                    .FirstOrDefault(f => f.DepartamentId == paapDb.DepartamentId &&
                           f.InvoiceElementsDetailsCategoryId == paapDb.InvoiceElementsDetailsCategoryId &&
                           f.TenantId == paapDb.TenantId && f.ApprovedYear == paapDb.DataEnd.Year &&
                           f.State == State.Active);
            //availableSum.SumAllocated += paapPrimeste.SumaPrimita;

            if (availableSum == null)
            {
                availableSum = new BVC_PAAP_AvailableSum();
                availableSum.InvoiceElementsDetailsCategoryId = paapDb.InvoiceElementsDetailsCategoryId;
                availableSum.DepartamentId = paapDb.DepartamentId;
            }

            var paapList = _bvcPaapRepository.GetAllIncluding(f => f.Departament, f => f.InvoiceElementsDetailsCategory).Where(f => f.DataEnd.Year == paapDb.DataEnd.Year).ToList();
            foreach (var paap in paapList.Where(f => f.InvoiceElementsDetailsCategoryId == paapDb.InvoiceElementsDetailsCategoryId && f.DepartamentId == paapDb.DepartamentId))
            {
                if (availableSum.Id == 0)
                {
                    availableSum.SumApproved += paap.ValoareEstimataFaraTvaLei;
                }
                availableSum.SumAllocated += paap.ValoareEstimataFaraTvaLei;
            }

            availableSum.SumAllocated += paapPrimeste.SumaPrimita;
            availableSum.Rest = availableSum.SumApproved - availableSum.SumAllocated;
            availableSum.ApprovedYear = paapDb.DataEnd.Year;

            if (availableSum.Id == 0)
            {
                _bvcPaapAvbSumRepository.Insert(availableSum);
            }
            CurrentUnitOfWork.SaveChanges();

            _bvcPaapRepository.InsertOrUpdate(paapDb);
            CurrentUnitOfWork.SaveChanges();

            _bvcPaapRepository.InsertPAAPState(paapDb.Id, LazyMethods.Now(), PAAP_State.Aprobat, "Redistribuire suma -" + LazyMethods.Now());
        }

        private void UpdatePaapPierde(IList<PaapRedistribuireDetaliiDto> paapRedistribDetList)
        {
            foreach (var paapRedistrib in paapRedistribDetList)
            {
                var paapPierde = _bvcPaapRepository
                    .GetAllIncluding(f => f.CotaTVA, f => f.Transe)
                    .Where(f => f.Id == paapRedistrib.PaapCarePierdeId && f.State == State.Active)
                    .FirstOrDefault();
                paapPierde.ValoareTotalaLei -= paapRedistrib.SumaPierduta;
                paapPierde.ValoareEstimataFaraTvaLei = paapPierde.ValoareTotalaLei / (100 + paapPierde.CotaTVA.VAT) * 100;

                var paapAvailableSum = _bvcPaapAvbSumRepository
                    .GetAllIncluding(f => f.InvoiceElementsDetailsCategory, f => f.Departament)
                    .FirstOrDefault(f => f.DepartamentId == paapPierde.DepartamentId &&
                           f.InvoiceElementsDetailsCategoryId == paapPierde.InvoiceElementsDetailsCategoryId &&
                           f.TenantId == paapPierde.TenantId && f.ApprovedYear == paapPierde.DataEnd.Year &&
                           f.State == State.Active);

                if (paapAvailableSum == null)
                {
                    paapAvailableSum = new BVC_PAAP_AvailableSum();
                    paapAvailableSum.InvoiceElementsDetailsCategoryId = paapPierde.InvoiceElementsDetailsCategoryId;
                    paapAvailableSum.DepartamentId = paapPierde.DepartamentId;
                }

                var paapList = _bvcPaapRepository.GetAllIncluding(f => f.Departament, f => f.InvoiceElementsDetailsCategory).Where(f => f.DataEnd.Year == paapPierde.DataEnd.Year).ToList();
                foreach (var paap in paapList.Where(f => f.InvoiceElementsDetailsCategoryId == paapPierde.InvoiceElementsDetailsCategoryId && f.DepartamentId == paapPierde.DepartamentId))
                {
                    if (paapAvailableSum.Id == 0)
                    {
                        paapAvailableSum.SumApproved += paap.ValoareEstimataFaraTvaLei;
                    }
                    paapAvailableSum.SumAllocated += paap.ValoareEstimataFaraTvaLei;
                }

                paapAvailableSum.SumAllocated -= paapRedistrib.SumaPierduta;
                paapAvailableSum.Rest = paapAvailableSum.SumApproved - paapAvailableSum.SumAllocated;
                paapAvailableSum.ApprovedYear = paapPierde.DataEnd.Year;

                if (paapAvailableSum.Id == 0)
                {
                    _bvcPaapAvbSumRepository.Insert(paapAvailableSum);
                }
                CurrentUnitOfWork.SaveChanges();

                _bvcPaapRepository.InsertOrUpdate(paapPierde);
                CurrentUnitOfWork.SaveChanges();
                _bvcPaapRepository.InsertPAAPState(paapPierde.Id, LazyMethods.Now(), PAAP_State.Aprobat, "Redistribuire suma -" + LazyMethods.Now());
            }
        }

        private void SavePaapRedistribuireDetalii(int paapRedistribuireId, IList<PaapRedistribuireDetaliiDto> paapRedistribuireDetalii)
        {
            foreach (var paapRedistrib in paapRedistribuireDetalii)
            {
                var paapRedistribDetaliuDb = ObjectMapper.Map<BVC_PaapRedistribuireDetalii>(paapRedistrib);
                paapRedistribDetaliuDb.BVC_PaapRedistribuireId = paapRedistribuireId;
                _paapRedistribuireDetaliiRepository.Insert(paapRedistribDetaliuDb);

                _bvcNotificare.SaveNotificarePaapPierde(paapRedistribDetaliuDb);

                CurrentUnitOfWork.SaveChanges();
            }

            UpdatePaapPierde(paapRedistribuireDetalii);
        }
    }
}