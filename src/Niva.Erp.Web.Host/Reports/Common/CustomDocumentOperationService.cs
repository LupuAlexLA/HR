using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Runtime.Session;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Web.WebDocumentViewer;
using DevExpress.XtraReports.Web.WebDocumentViewer.DataContracts;
using Niva.Erp.Managers.Reporting;
using wsfiledoc;

namespace DocumentOperationServiceSample.Services
{
    
    public class CustomDocumentOperationService : DocumentOperationService
    {
        public FileDocReportData repData { get; set; }
        public CustomDocumentOperationService(FileDocReportData repData) :base()
        {
            this.repData = repData;
        }
        public override bool CanPerformOperation(DocumentOperationRequest request)
        {
            return true;
        }

        public override Task<DocumentOperationResponse> PerformOperationAsync(DocumentOperationRequest request, PrintingSystemBase initialPrintingSystem, PrintingSystemBase printingSystemWithEditingFields)
        {
            //return base.PerformOperationAsync(request, initialPrintingSystem, printingSystemWithEditingFields);
            var reportName = initialPrintingSystem.Document.Bookmark;
            using (var stream = new MemoryStream())
            {
                printingSystemWithEditingFields.ExportToPdf(stream);
                stream.Position = 0;

                var info = repData.Calc(request.CustomData);
                return SaveToFileDocAsync(stream, info);
            }
        }
         

        async Task<DocumentOperationResponse> SaveToFileDocAsync(MemoryStream stream, FileDocReportData info)
        {
            try
            {
                DocumentArhiva docNew = new DocumentArhiva();
                docNew.tipDocument = info.TipDocument;
                docNew.observatii = info.Subiect;                 
                docNew.tipInregistrare = "3";                 
                docNew.emitentArhivare = "simona.nitu";// "Simona Nitu";// info.Emitent;
                docNew.destinatarArhivare = "simona.nitu";//"Simona Nitu";// info.Destinatar;
                docNew.dataArhivare =  DateTime.Now.ToString("dd.MM.yyyy");
                docNew.subiect = info.Subiect;
                docNew.atributeDocument = new ArrayOfAtributDocument();
                docNew.atributeDocument.Add(new AtributDocument() { simbol = "Numar", valoare = "1" });
                docNew.atributeDocument.Add(new AtributDocument() { simbol = "Data", valoare = "02/02/2022" });  // MM/dd/yyyy
                var filedocSoapClient = new FiledocSoapClient(FiledocSoapClient.EndpointConfiguration.FiledocSoap);

                try
                {

                    if (filedocSoapClient.State != System.ServiceModel.CommunicationState.Faulted)
                    {
                        await filedocSoapClient.OpenAsync();
                        var adaugareDocumentResponse = await filedocSoapClient.adaugareDocumentAsync("12345678", "", docNew/*,"ionut.gavrila", "12345678"*/);

                        if (adaugareDocumentResponse.Body.adaugareDocumentResult != null)
                        {
                            var adaugareDocumentResult = adaugareDocumentResponse.Body.adaugareDocumentResult;
                            //var adaugareAtasamentResponse = await filedocSoapClient.adaugareAtasamentDocumentAsync("12345678", adaugareDocumentResult.ID.ToString(), reportName+".pdf", stream.ToArray());
                            return new DocumentOperationResponse
                            {
                                Succeeded = true,
                                Message = "Document was successfully saved to FileDoc."
                            };
                        }

                    }
                }
                catch (Exception ex)
                {
                    filedocSoapClient.Abort();
                    throw new Exception("Eroare salvare", ex);
                }

                return new DocumentOperationResponse
                {
                    Succeeded = true,
                    Message = "Document was successfully saved to FileDoc."
                };
            }
            catch
            {
                return new DocumentOperationResponse
                {
                    Succeeded = false,
                    Message = "Document was not saved to FileDoc."
                };
            }
        }

        protected string RemoveNewLineSymbols(string value)
        {
            return value;
        }
    }
}