using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using Castle.Core.Logging;
using Microsoft.Extensions.Configuration;
using Niva.Erp.ExternalApi;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Contracts;
using Niva.Erp.Models.Deconturi;
using Niva.Erp.Models.Filedoc;
using Niva.Erp.Models.HR;
using Niva.Erp.Repositories.Economic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wsfiledoc;

namespace Niva.Erp.BackgroudWorkers
{
    public class PreluareFileDocBackgroundWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private readonly IInvoiceRepository invoiceRepository;
        private readonly IRepository<Person> personRepository;
        private readonly IRepository<Currency> currencyRepository;
        private readonly IRepository<FileDocError> fileDocErrorRepository;
        private readonly IRepository<Departament> departamentRepository;
        private readonly IRepository<Contracts> contractRepository;
        private readonly FileDocApi fileDocApi;
        private readonly AnafAPI anafAPI;
        private readonly ILogger logger;
        private bool IsActive;
        private readonly IConfiguration configuration;
        public PreluareFileDocBackgroundWorker(AnafAPI anafAPI, ILogger logger, AbpTimer timer, IInvoiceRepository invoiceRepository, IRepository<Currency> currencyRepository, IRepository<Person> personRepository, FileDocApi fileDocApi, IRepository<FileDocError> fileDocErrorRepository, IConfiguration configuration, IRepository<Departament> departamentRepository, IRepository<Contracts> contractRepository) : base(timer)
        {
            
            this.fileDocApi = fileDocApi;
            this.anafAPI = anafAPI;
            this.invoiceRepository = invoiceRepository;
            this.currencyRepository = currencyRepository;
            this.personRepository = personRepository;
            this.fileDocErrorRepository = fileDocErrorRepository;
            this.departamentRepository = departamentRepository;
            this.contractRepository = contractRepository;
            this.logger = logger;
            this.configuration = configuration;
            Timer.Period = 60000;
            Timer.RunOnStart = true;
            IsActive = bool.Parse( configuration["App:FileDocJobActive"]);
        }
        [UnitOfWork]
        protected override void DoWork()
        {
            if (IsActive)
                using (UnitOfWorkManager.Current.SetTenantId(1))
                {
                    var docFacturi = GetFacturiFromFiledoc().Result;
                    SaveFacturi(docFacturi);

                    var docContracteAchizitie = GetContracteAchizitieFiledoc().Result;
                    SaveContracteAchizitie(docContracteAchizitie);

                    var docBonuriFiscale = GetBonuriFiscaleFromFiledoc().Result;
                    SaveBonuriFiscale(docBonuriFiscale);

                    //var docOrdineDeplasare = GetOrdineDeplasareFiledoc().Result;
                    //SaveOrdineDeplasare(docOrdineDeplasare);
                }
        }
        public async Task<List<DocumentArhiva>> GetFacturiFromFiledoc()
        {
            var docFacturi = await fileDocApi.GetFacturiForImport();
            return docFacturi;
        }
        public async Task<List<DocumentArhiva>> GetContracteAchizitieFiledoc()
        {
            var docFacturi = await fileDocApi.GetContracteAchizitieForImport();
            return docFacturi;
        }
        public async Task<List<DocumentArhiva>> GetBonuriFiscaleFromFiledoc()
        {
            var docFacturi = await fileDocApi.GetBonuriFiscaleForImport();
            return docFacturi;
        }
        public async Task<List<DocumentArhiva>> GetOrdineDeplasareFiledoc()
        {
            var docFacturi = await fileDocApi.GetOrdineDeplasareForImport();
            return docFacturi;
        }
        [UnitOfWork]
        public void SaveFacturi(List<DocumentArhiva> docFacturi)
        {
            foreach (var d in docFacturi)
            {
                var errrors = "";
                var hasErrors = false;
                var currentField = "";
                using (var uow = UnitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
                {
                    try
                    {
                        
                        var factura = new Invoices();
                        factura.ThirdPartyQuality = ThirdPartyQuality.Furnizor;
                        factura.State = Models.Conta.Enums.State.Active;
                        currentField = "Suma factura";
                        factura.FileDocValue = decimal.Parse(d.atributeDocument.Single(s => s.simbol == "Suma factura").valoare, CultureInfo.GetCultureInfo("ro-RO"));
                        currentField = "Valuta";
                        var currencyCode = d.atributeDocument.Single(s => s.simbol == "Valuta").valoare;
                        factura.CurrencyId = currencyRepository.GetAll().Single(c => c.CurrencyCode == currencyCode).Id;
                        factura.MonedaFacturaId = currencyRepository.GetAll().Single(c => c.CurrencyCode == currencyCode).Id;
                        currentField = "Serie factura";
                        factura.InvoiceSeries = d.atributeDocument.Single(s => s.simbol == "Serie factura").valoare;
                        currentField = "Numar factura";
                        factura.InvoiceNumber = d.atributeDocument.Single(s => s.simbol == "Numar factura").valoare;
                        currentField = "Data";
                        factura.InvoiceDate = DateTime.ParseExact(d.atributeDocument.Single(s => s.simbol == "Data factura").valoare, "dd/MM/yyyy", null);
                        currentField = "Scadenta factura";
                        var dataArhivare = DateTime.ParseExact(d.dataArhivare, "yyyy-MM-dd", null);
                        if (!string.IsNullOrEmpty(d.dataArhivare))
                        {
                            factura.RegDate = dataArhivare;
                            factura.OperationDate = dataArhivare;
                        }
                        DateTime dataScadenta;
                        if (DateTime.TryParseExact(d.atributeDocument.Single(s => s.simbol == "Scadenta factura").valoare, "dd/MM/yyyy", null, DateTimeStyles.None, out dataScadenta))
                            factura.DueDate = dataScadenta;
                        else
                            factura.DueDate = dataArhivare;

                        factura.FileDocId = d.ID.Value;
                        factura.RegNumber = d.numarArhivare;
                        currentField = "Suma factura";
                        
                        factura.DocumentType = invoiceRepository.GetDocumentType("FF");
                        currentField = "Emitent";
                        var emitentCui = invoiceRepository.GetFileDocEmitentCui(d.ID.Value);
                        emitentCui = emitentCui.ToUpper().Replace("RO", "").Trim();
                        if (!string.IsNullOrEmpty(emitentCui))
                        {
                            var person = personRepository.GetAll().SingleOrDefault(p => p.Id1 == emitentCui) as LegalPerson;
                            if (person != null)
                            {
                                factura.ThirdPartyId = person.Id;
                            }
                            else
                            {
                                var firma = anafAPI.AnafAPIAsync(int.Parse(emitentCui)).Result;
                                if (!string.IsNullOrEmpty(firma.denumire))
                                {
                                    person = new LegalPerson();
                                    person.Id1 = firma.cui.ToString();
                                    person.AddressStreet = firma.adresa;
                                    person.Id2 = firma.nrRegCom;
                                    person.Name = firma.denumire;
                                    personRepository.Insert(person);
                                    factura.ThirdParty = person;
                                }
                                else
                                {
                                    hasErrors = true;
                                    errrors += Environment.NewLine + "Eroare preluare cui";
                                }
                            }
                        }
                        else
                        {
                            hasErrors = true;
                            errrors += Environment.NewLine + "Eroare preluare cui";
                        }
                        if (!hasErrors)
                        { 
                            invoiceRepository.Insert(factura);
                            invoiceRepository.SetAsPreluatInConta(d.ID.Value);
                            uow.Complete();
                        }
                    }
                    catch (Exception ex)
                    {
                        hasErrors = true;
                        errrors += Environment.NewLine + currentField +"- " + ex.Message;
                        logger.Error("FileDocApi: Eroare preluare factura: " + d.numarArhivare + ": " + ex);
                    }
                }
                if (hasErrors)
                    AddToErrors(d, errrors);
                else
                    RemoveFromErrors(d);
            }
        }
        private void SaveOrdineDeplasare(List<DocumentArhiva> docOrdineDeplasare)
        {
            
            //foreach (var d in docOrdineDeplasare)
            //{
            //    using (var uow = UnitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            //    {
            //        try
            //        {
            //            var emitentCui = invoiceRepository.GetFileDocEmitentCui(d.ID.Value);
            //            var decont = new Decont();
            //            if (!string.IsNullOrEmpty(emitentCui))
            //            {
            //                var person = personRepository.GetAll().SingleOrDefault(p => p.Id1 == emitentCui) as LegalPerson;
            //                if (person != null)
            //                {
            //                    decont.ThirdPartyId = person.Id;
            //                }
            //                else
            //                {
            //                    var firma = anafAPI.AnafAPIAsync(int.Parse(emitentCui)).Result;
            //                    if (!string.IsNullOrEmpty(firma.denumire))
            //                    {
            //                        person = new LegalPerson();
            //                        person.Id1 = firma.cui.ToString();
            //                        person.AddressStreet = firma.adresa;
            //                        person.Id2 = firma.nrRegCom;
            //                        person.Name = firma.denumire;
            //                        personRepository.Insert(person);
            //                        decont.ThirdParty = person;
            //                    }
            //                }
            //            }
            //            //decont.ThirdPartyQuality = ThirdPartyQuality.Furnizor;
            //            decont.State = Models.Conta.Enums.State.Active;
            //            decont.FileDocValue = decimal.Parse(d.atributeDocument.Single(s => s.simbol == "Suma").valoare);

            //            var currencyCode = d.atributeDocument.Single(s => s.simbol == "Valuta").valoare;
            //            decont.CurrencyId = currencyRepository.GetAll().Single(c => c.CurrencyCode == currencyCode).Id;
            //            decont.InvoiceSeries = d.atributeDocument.Single(s => s.simbol == "Serie factura").valoare;
            //            decont.DecontNumber = int.Parse( d.atributeDocument.Single(s => s.simbol == "Numar").valoare);
            //            decont.InvoiceDate = DateTime.ParseExact(d.atributeDocument.Single(s => s.simbol == "Data").valoare, "dd/MM/yyyy", null);
            //            decont.DueDate = DateTime.ParseExact(d.atributeDocument.Single(s => s.simbol == "Scadenta factura").valoare, "dd/MM/yyyy", null);
            //            decont.FileDocId = d.ID.Value;
            //            decont.RegNumber = d.numarArhivare;
            //            if (!string.IsNullOrEmpty(d.dataArhivare))
            //            {
            //                decont.RegDate = DateTime.ParseExact(d.dataArhivare, "yyyy-MM-dd", null);
            //                decont.OperationDate = DateTime.ParseExact(d.dataArhivare, "yyyy-MM-dd", null);
            //            }
            //            decont.DocumentType = invoiceRepository.GetDocumentType("FF");
            //            decontRepository.Insert(decont);
            //            invoiceRepository.SetAsPreluatInConta(d.ID.Value);
            //            //CurrentUnitOfWork.SaveChanges();
            //            uow.Complete();
            //        }
            //        catch (Exception ex)
            //        {
            //            logger.Error("FileDocApi: Eroare preluare factura: " + d.numarArhivare + ": " + ex);
            //        }
            //    }
            //}
        }
        private void SaveBonuriFiscale(List<DocumentArhiva> docBonuriFiscale)
        {
            
            
            foreach (var d in docBonuriFiscale)
            {
                var errrors = "";
                var hasErrors = false;
                var currentField = "";
                using (var uow = UnitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
                {
                    try
                    {
                        var bonFiscal = new Invoices();                        
                        bonFiscal.ThirdPartyQuality = ThirdPartyQuality.Furnizor;
                        bonFiscal.State = Models.Conta.Enums.State.Active;
                        currentField = "Suma bon fiscal";
                        bonFiscal.FileDocValue = decimal.Parse(d.atributeDocument.Single(s => s.simbol == "Suma bon fiscal").valoare, CultureInfo.GetCultureInfo("ro-RO"));
                        currentField = "Valuta";
                        var currencyCode = d.atributeDocument.Single(s => s.simbol == "Valuta").valoare;
                        bonFiscal.CurrencyId = currencyRepository.GetAll().Single(c => c.CurrencyCode == currencyCode).Id;
                        bonFiscal.MonedaFacturaId = currencyRepository.GetAll().Single(c => c.CurrencyCode == currencyCode).Id;
                        currentField = "Serie bon fiscal";
                        bonFiscal.InvoiceSeries = d.atributeDocument.Single(s => s.simbol == "Serie bon fiscal").valoare;
                        currentField = "Numar bon fiscal";
                        bonFiscal.InvoiceNumber = d.atributeDocument.Single(s => s.simbol == "Numar bon fiscal").valoare;
                        currentField = "Data bon fiscal";
                        bonFiscal.InvoiceDate = DateTime.ParseExact(d.atributeDocument.Single(s => s.simbol == "Data bon fiscal").valoare, "dd/MM/yyyy", null);
                        currentField = "Scadenta bon fiscal";
                        var dataScadentaStr = d.atributeDocument.Single(s => s.simbol == "Scadenta bon fiscal").valoare;
                        if (string.IsNullOrEmpty(dataScadentaStr))
                        {
                            bonFiscal.DueDate = bonFiscal.InvoiceDate;
                        }
                        else
                        {
                            bonFiscal.DueDate = DateTime.ParseExact(dataScadentaStr, "dd/MM/yyyy", null);
                        }
                       
                        bonFiscal.FileDocId = d.ID.Value;
                        bonFiscal.RegNumber = d.numarArhivare;
                        currentField = "Data arhivare";
                        if (!string.IsNullOrEmpty(d.dataArhivare))
                        {
                            bonFiscal.RegDate = DateTime.ParseExact(d.dataArhivare, "yyyy-MM-dd", null);
                            bonFiscal.OperationDate = DateTime.ParseExact(d.dataArhivare, "yyyy-MM-dd", null);
                        }
                        bonFiscal.DocumentType = invoiceRepository.GetDocumentType("BF");
                        currentField = "Cui emitent";
                        var emitentCui = invoiceRepository.GetFileDocEmitentCui(d.ID.Value);
                        emitentCui = emitentCui.ToUpper().Replace("RO", "").Trim();

                        if (!string.IsNullOrEmpty(emitentCui))
                        {
                            var person = personRepository.GetAll().SingleOrDefault(p => p.Id1 == emitentCui) as LegalPerson;
                            if (person != null)
                            {
                                bonFiscal.ThirdPartyId = person.Id;
                            }
                            else
                            {
                                var firma = anafAPI.AnafAPIAsync(int.Parse(emitentCui)).Result;
                                if (!string.IsNullOrEmpty(firma.denumire))
                                {
                                    person = new LegalPerson();
                                    person.Id1 = firma.cui.ToString();
                                    person.AddressStreet = firma.adresa;
                                    person.Id2 = firma.nrRegCom;
                                    person.Name = firma.denumire;
                                    personRepository.Insert(person);
                                    bonFiscal.ThirdParty = person;
                                }
                                else
                                {
                                    hasErrors = true;
                                    errrors += Environment.NewLine + "Eroare preluare cui";
                                }
                            }
                        }
                        else
                        {
                            hasErrors = true;
                            errrors += Environment.NewLine + "Eroare preluare cui";
                        }
                        if (!hasErrors)
                        {
                            invoiceRepository.Insert(bonFiscal);
                            invoiceRepository.SetAsPreluatInConta(d.ID.Value);
                            uow.Complete();
                        }
                    }
                    catch (Exception ex)
                    {
                        hasErrors = true;
                        errrors += Environment.NewLine + currentField + "- " + ex.Message;
                        logger.Error("FileDocApi: Eroare preluare bon fiscal: " + d.numarArhivare + ": " + ex);
                    }
                }
                if (hasErrors)
                    AddToErrors(d, errrors);
                else
                    RemoveFromErrors(d);
            }
        }
        [UnitOfWork]
        private void SaveContracteAchizitie(List<DocumentArhiva> docContracteAchizitie)
        {
            foreach (var d in docContracteAchizitie)
            {
                var errrors = "";
                var hasErrors = false;
                var currentField = "";
                using (var uow = UnitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
                {
                    try
                    {
                        var contract = new Contracts();

                        contract.ThirdPartyQuality = ThirdPartyQuality.Furnizor;
                        contract.State = Models.Conta.Enums.State.Active;
                        currentField = "Tip entitate partenera";
                        var thirdPartyQualityString = d.atributeDocument.Single(s => s.simbol == "Tip entitate partenera").valoare;
                        switch (thirdPartyQualityString)
                        {
                            case "Client":
                                {
                                    contract.ThirdPartyQuality = ThirdPartyQuality.Client;
                                    break;
                                }
                            case "Furnizor":
                                {
                                    contract.ThirdPartyQuality = ThirdPartyQuality.Furnizor;
                                    break;
                                }
                            default:
                                {
                                    hasErrors = true;
                                    errrors += Environment.NewLine + "Eroare tip entitate partenera";
                                    break;
                                }
                        }
                        currentField = "Obiect contract";
                        contract.ContractObject = d.atributeDocument.Single(s => s.simbol == "Obiect contract").valoare;
                        currentField = "Valoare";
                        contract.Value = decimal.Parse(d.atributeDocument.Single(s => s.simbol == "Valoare").valoare, CultureInfo.GetCultureInfo("ro-RO"));
                        currentField = "Moneda";
                        var currencyCode = d.atributeDocument.Single(s => s.simbol == "Moneda").valoare;
                        var currency = currencyRepository.GetAll().SingleOrDefault(c => c.CurrencyCode == currencyCode);
                        if (currency == null)
                        {
                            hasErrors = true;
                            errrors += Environment.NewLine + "Eroare moneda";
                        }
                        else
                        {
                            contract.Currency_Id = currency.Id;
                        }
                        currentField = "Numar partener";
                        contract.ThirdPartyContractNr = d.atributeDocument.Single(s => s.simbol == "Numar partener").valoare;
                        currentField = "Data partener";
                        contract.ThirdPartyContractDate = DateTime.ParseExact(d.atributeDocument.Single(s => s.simbol == "Data partener").valoare, "dd/MM/yyyy", null);
                        currentField = "Data inceput";
                        contract.StartDate = DateTime.ParseExact(d.atributeDocument.Single(s => s.simbol == "Data inceput").valoare, "dd/MM/yyyy", null);
                        currentField = "Data sfarsit";
                        contract.EndDate = DateTime.ParseExact(d.atributeDocument.Single(s => s.simbol == "Data sfarsit").valoare, "dd/MM/yyyy", null);
                        currentField = "Data arhivare";
                        contract.ContractDate = DateTime.ParseExact(d.dataArhivare, "yyyy-MM-dd", null);
                        currentField = "document id";
                        contract.FileDocId = d.ID.Value;
                        currentField = "numar arhivare";
                        contract.ContractNr = d.numarArhivare;
                        currentField = "Frecventa plata";
                        var frecventaPlataString = d.atributeDocument.Single(s => s.simbol == "Frecventa plata").valoare;
                        switch (frecventaPlataString)
                        {
                            case "Lunar":
                                {
                                    contract.ContractsPaymentInstalmentFreq = ContractsPaymentInstalmentFreq.Lunar;
                                    break;
                                }
                            case "Ocazional":
                                {
                                    contract.ContractsPaymentInstalmentFreq = ContractsPaymentInstalmentFreq.Ocazional;
                                    break;
                                }
                            case "Semestrial":
                                {
                                    contract.ContractsPaymentInstalmentFreq = ContractsPaymentInstalmentFreq.Semestrial;
                                    break;
                                }
                            case "Trimestrial":
                                {
                                    contract.ContractsPaymentInstalmentFreq = ContractsPaymentInstalmentFreq.Trimestrial;
                                    break;
                                }
                            default:
                                {
                                    hasErrors = true;
                                    errrors += Environment.NewLine + "Eroare Frecventa plata";
                                    break;
                                }
                        }
                        currentField = "structura initiatoare";
                        var structuraInitiatoareString = d.atributeDocument.Single(s => s.simbol == "Structura initiatoare").valoare;
                        var departament = departamentRepository.GetAll().SingleOrDefault(s => s.Name == structuraInitiatoareString);
                        if (departament == null)
                        {
                            hasErrors = true;
                            errrors += Environment.NewLine + "Eroare Structura initiatoare (Departament)";
                        }
                        else
                        {
                            contract.DepartamentId = departament.Id;
                        }
                        currentField = "Emitent cui";
                        var emitentCui = invoiceRepository.GetFileDocEmitentCui(d.ID.Value);
                        emitentCui = emitentCui.ToUpper().Replace("RO", "").Trim();
                        if (!string.IsNullOrEmpty(emitentCui))
                        {
                            var person = personRepository.GetAll().SingleOrDefault(p => p.Id1 == emitentCui) as LegalPerson;
                            if (person != null)
                            {
                                contract.ThirdPartyId = person.Id;
                            }
                            else
                            {
                                var firma = anafAPI.AnafAPIAsync(int.Parse(emitentCui)).Result;
                                if (!string.IsNullOrEmpty(firma.denumire))
                                {
                                    person = new LegalPerson();
                                    person.Id1 = firma.cui.ToString();
                                    person.AddressStreet = firma.adresa;
                                    person.Id2 = firma.nrRegCom;
                                    person.Name = firma.denumire;
                                    personRepository.Insert(person);
                                    contract.ThirdParty = person;
                                }
                                else
                                {
                                    hasErrors = true;
                                    errrors += Environment.NewLine + "Eroare preluare cui";
                                }
                            }
                        }
                        else
                        {
                            hasErrors = true;
                            errrors += Environment.NewLine + "Eroare preluare cui";
                        }
                        if (!hasErrors)
                        {
                            contractRepository.Insert(contract);
                            invoiceRepository.SetAsPreluatInConta(d.ID.Value);
                            uow.Complete();
                        }
                    }
                    catch (Exception ex)
                    {
                        hasErrors = true;
                        errrors += Environment.NewLine + currentField + " - " + ex.Message;
                        logger.Error("FileDocApi: Eroare preluare contract: " + d.numarArhivare + ": " + currentField + " " + ex);
                    }
                }
                if (hasErrors)
                    AddToErrors(d, errrors);
                else
                    RemoveFromErrors(d);
            }
        }
        [UnitOfWork]
        private void AddToErrors(DocumentArhiva doc, string eroare)
        {
            var error = fileDocErrorRepository.GetAll().SingleOrDefault(w => w.DocumentId == doc.ID.Value);
            if (error == null)
            {
                error = new FileDocError()
                {
                    DocumentId = doc.ID.Value,
                    DocumentNr = doc.numarArhivare,
                    Rezolvat = false,
                };
                fileDocErrorRepository.Insert(error);
            }
            error.LastErrorDate = DateTime.Now;
            error.MesajEroare = eroare;
            UnitOfWorkManager.Current.SaveChanges();
        }
        [UnitOfWork]
        private void RemoveFromErrors(DocumentArhiva doc)
        {
            var error = fileDocErrorRepository.GetAll().SingleOrDefault(w => w.DocumentId == doc.ID.Value);
            if (error != null)
            {
                error.Rezolvat = true;
                error.RezolvatDate = DateTime.Now;
                UnitOfWorkManager.Current.SaveChanges();
            }
        }
    }
}
