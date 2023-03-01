using Abp.Domain.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wsfiledoc;

namespace Niva.Erp.ExternalApi
{
    public class FileDocApi : DomainService
    {
        private readonly IConfiguration _configuration;
        string token;
        string companie;
        string apiUrl;
        public FileDocApi(IConfiguration configuration ) : base()
        {
            apiUrl = configuration["App:FileDocApiUrl"];
            token = configuration["App:FileDocToken"];
            companie = configuration["App:FileDocCompanie"];
        }
        public async Task<List<DocumentArhiva>> GetFacturiForImport()
        {
            var cautareDocument = new cautareDocumenteRequest();
            cautareDocument.Body = new cautareDocumenteRequestBody();
            cautareDocument.Body.token = token;
            cautareDocument.Body.document = new DocumentArhiva();
            cautareDocument.Body.companie = companie;
            cautareDocument.Body.document.tipDocument = "Factura";
            cautareDocument.Body.document.atributeDocument = new wsfiledoc.ArrayOfAtributDocument();
            cautareDocument.Body.document.atributeDocument.Add(new AtributDocument() { simbol = "Viza CFP", valoare = "1" });
            cautareDocument.Body.document.atributeDocument.Add(new AtributDocument() { simbol = "Preluata in contabilitate", valoare = "0" });
            FiledocSoapClient filedocSoapClient = new FiledocSoapClient(FiledocSoapClient.EndpointConfiguration.FiledocSoap, apiUrl);
            List<DocumentArhiva> rezultatList = new List<DocumentArhiva>();
            List<DocumentArhiva> ret = new List<DocumentArhiva>();
            try
            {
                if (filedocSoapClient.State != System.ServiceModel.CommunicationState.Faulted)
                {
                    await filedocSoapClient.OpenAsync();
                    var rezultatCautareDDocument = await filedocSoapClient.cautareDocumenteAsync(token, companie, cautareDocument.Body.document);

                    if (rezultatCautareDDocument.Body.cautareDocumenteResult.documente != null)
                    {
                        rezultatList = rezultatCautareDDocument.Body.cautareDocumenteResult.documente.ToList();

                        foreach (var r in rezultatList)
                            if (r.atributeDocument.Single(s => s.simbol == "Preluata in contabilitate").valoare == "0" && r.atributeDocument.Single(s => s.simbol == "Viza CFP").valoare == "1")
                                ret.Add(r);
                    }

                }
            }
            catch (Exception ex)
            {
                filedocSoapClient.Abort();
                throw new Exception("Erroare preluare facturi din Filedoc API", ex);
            }
            return ret;
        }
        public async Task<List<DocumentArhiva>> GetBonuriFiscaleForImport()
        {
            var cautareDocument = new cautareDocumenteRequest();
            cautareDocument.Body = new cautareDocumenteRequestBody();
            cautareDocument.Body.token = token;
            cautareDocument.Body.document = new DocumentArhiva();
            cautareDocument.Body.companie = companie;
            cautareDocument.Body.document.tipDocument = "Bon fiscal";
            cautareDocument.Body.document.atributeDocument = new wsfiledoc.ArrayOfAtributDocument();
            cautareDocument.Body.document.atributeDocument.Add(new AtributDocument() { simbol = "Viza CFP", valoare = "1" });
            cautareDocument.Body.document.atributeDocument.Add(new AtributDocument() { simbol = "Preluata in contabilitate", valoare = "0" });
            FiledocSoapClient filedocSoapClient = new FiledocSoapClient(FiledocSoapClient.EndpointConfiguration.FiledocSoap, apiUrl);
            List<DocumentArhiva> rezultatList = new List<DocumentArhiva>();
            List<DocumentArhiva> ret = new List<DocumentArhiva>();

            try
            {
                if (filedocSoapClient.State != System.ServiceModel.CommunicationState.Faulted)
                {
                    await filedocSoapClient.OpenAsync();
                    var rezultatCautareDDocument = await filedocSoapClient.cautareDocumenteAsync(token, companie, cautareDocument.Body.document);

                    if (rezultatCautareDDocument.Body.cautareDocumenteResult.documente != null)
                    {
                        rezultatList = rezultatCautareDDocument.Body.cautareDocumenteResult.documente.ToList();
                        foreach (var r in rezultatList)
                            if (r.atributeDocument.Single(s => s.simbol == "Preluata in contabilitate").valoare == "0" && r.atributeDocument.Single(s => s.simbol == "Viza CFP").valoare == "1")
                                ret.Add(r);
                    }
                }
            }
            catch (Exception ex)
            {
                filedocSoapClient.Abort();
                throw new Exception("Erroare preluare Bonuri fiscale din Filedoc API", ex);
            }
            return ret;
        }
        public async Task<List<DocumentArhiva>> GetContracteAchizitieForImport()
        {
            var cautareDocument = new cautareDocumenteRequest();
            cautareDocument.Body = new cautareDocumenteRequestBody();
            cautareDocument.Body.token = token;
            cautareDocument.Body.document = new DocumentArhiva();
            cautareDocument.Body.companie = companie;
            cautareDocument.Body.document.tipDocument = "Contract de achizitie";
            cautareDocument.Body.document.atributeDocument = new wsfiledoc.ArrayOfAtributDocument();
            cautareDocument.Body.document.atributeDocument.Add(new AtributDocument() { simbol = "STATUS CONTRACT: Semnat de ambele parti", valoare = "1" });
            cautareDocument.Body.document.atributeDocument.Add(new AtributDocument() { simbol = "Preluata in contabilitate", valoare = "0" });
            FiledocSoapClient filedocSoapClient = new FiledocSoapClient(FiledocSoapClient.EndpointConfiguration.FiledocSoap, apiUrl);
            List<DocumentArhiva> rezultatList = new List<DocumentArhiva>();

            try
            {
                if (filedocSoapClient.State != System.ServiceModel.CommunicationState.Faulted)
                {
                    await filedocSoapClient.OpenAsync();
                    var rezultatCautareDDocument = await filedocSoapClient.cautareDocumenteAsync(token, companie, cautareDocument.Body.document);

                    if (rezultatCautareDDocument.Body.cautareDocumenteResult.documente != null)
                    {
                        rezultatList = rezultatCautareDDocument.Body.cautareDocumenteResult.documente.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                filedocSoapClient.Abort();
                throw new Exception("Erroare preluare Contracte de achizitie din Filedoc API", ex);
            }
            return rezultatList;
        }
        public async Task<List<DocumentArhiva>> GetOrdineDeplasareForImport()
        {
            var cautareDocument = new cautareDocumenteRequest();
            cautareDocument.Body = new cautareDocumenteRequestBody();
            cautareDocument.Body.token = token;
            cautareDocument.Body.document = new DocumentArhiva();
            cautareDocument.Body.companie = companie;
            cautareDocument.Body.document.tipDocument = "Ordin de deplasare";
            cautareDocument.Body.document.atributeDocument = new wsfiledoc.ArrayOfAtributDocument();
            cautareDocument.Body.document.atributeDocument.Add(new AtributDocument() { simbol = "CFP", valoare = "1" });
            cautareDocument.Body.document.atributeDocument.Add(new AtributDocument() { simbol = "Preluata in contabilitate", valoare = "0" });
            FiledocSoapClient filedocSoapClient = new FiledocSoapClient(FiledocSoapClient.EndpointConfiguration.FiledocSoap, apiUrl);
            List<DocumentArhiva> rezultatList = new List<DocumentArhiva>();

            try
            {
                if (filedocSoapClient.State != System.ServiceModel.CommunicationState.Faulted)
                {
                    await filedocSoapClient.OpenAsync();
                    var rezultatCautareDDocument = await filedocSoapClient.cautareDocumenteAsync(token, companie, cautareDocument.Body.document);

                    if (rezultatCautareDDocument.Body.cautareDocumenteResult.documente != null)
                    {
                        rezultatList = rezultatCautareDDocument.Body.cautareDocumenteResult.documente.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                filedocSoapClient.Abort();
                throw new Exception("Erroare preluare ordine de deplasare din Filedoc API", ex);
            }
            return rezultatList;
        }
        public async Task<List<Atasament>> GetAtasamente( int documentId)
        {

            FiledocSoapClient filedocSoapClient = new FiledocSoapClient(FiledocSoapClient.EndpointConfiguration.FiledocSoap, apiUrl);

            try
            {
                if (filedocSoapClient.State != System.ServiceModel.CommunicationState.Faulted)
                {
                    await filedocSoapClient.OpenAsync();
                    var rezultatCautareDDocument = await filedocSoapClient.cautareAtasamenteDocumentAsync(token, documentId.ToString());

                    if (rezultatCautareDDocument.Body.cautareAtasamenteDocumentResult.atasamente != null)
                    {
                        return   rezultatCautareDDocument.Body.cautareAtasamenteDocumentResult.atasamente.ToList();
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                filedocSoapClient.Abort();
                throw new Exception("Erroare preluare facturi din Filedoc API", ex);
            }            
        }
        public async Task<DescarcareAtasamentRezultat> DownloadAtasament(int documentId)
        {

            FiledocSoapClient filedocSoapClient = new FiledocSoapClient(FiledocSoapClient.EndpointConfiguration.FiledocSoap, apiUrl);

            try
            {
                if (filedocSoapClient.State != System.ServiceModel.CommunicationState.Faulted)
                {
                    await filedocSoapClient.OpenAsync();
                    var rezultatCautareDDocument = await filedocSoapClient.descarcareAtasamentDocumentAsync(token, documentId.ToString());

                    if (rezultatCautareDDocument.Body.descarcareAtasamentDocumentResult != null && rezultatCautareDDocument.Body.descarcareAtasamentDocumentResult.continut != null)
                    {
                        return rezultatCautareDDocument.Body.descarcareAtasamentDocumentResult;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                filedocSoapClient.Abort();
                throw new Exception("Erroare preluare facturi din Filedoc API", ex);
            }

        }
    }
}
