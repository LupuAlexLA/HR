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

namespace DocumentOperationServiceSample.Services
{
    
    public class CustomDocumentOperationService : DocumentOperationService
    {
        
         


        protected string RemoveNewLineSymbols(string value)
        {
            return value;
        }
    }
}