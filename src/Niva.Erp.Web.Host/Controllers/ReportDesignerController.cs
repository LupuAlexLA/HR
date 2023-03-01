using DevExpress.Compatibility.System.Web;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.ReportDesigner;
using Microsoft.AspNetCore.Mvc;
using Niva.Erp.Conta.Reports;
using Niva.Erp.Controllers;
using Niva.Erp.Web.Host.Reports.Conta;
using System;
using System.IO;

namespace Niva.Erp.Web.Host.Controllers
{
    //public enum ReportExportType
    //{
    //    Pdf, Word, Excel
    //}

    //[Route("api/[controller]")]
    //public class ReportDesignerController : ErpControllerBase
    //{

    //    IReportsAppService _reportService;

    //    public ReportDesignerController(IReportsAppService reportService)
    //    {
    //        _reportService = reportService;
    //    }

    //    [HttpGet]
    //    public FileContentResult BalanceReport(int balanceId, ReportExportType reportExportType)
    //    {
    //        var Model = _reportService.BalantaReport(balanceId);
    //        var rap = new XtraReport1(Model);
    //        //var FileContentResult = memstream.ToArray();
    //        var ret = GetReportBytes(rap, reportExportType);
    //        return ret;
    //      //  return null;
    //    }


    //    private FileContentResult GetReportBytes(XtraReport xtraReport, ReportExportType reportExportType)
    //    {
    //        xtraReport.CreateDocument();
    //        var memstream = new MemoryStream();

    //        string mime = "";

    //        FileContentResult ret;

    //        switch (reportExportType)
    //        {
    //            case ReportExportType.Pdf:
    //                {
    //                    xtraReport.ExportToPdf(memstream);
    //                    mime = "application/pdf";
    //                    break;
    //                }
    //        }
    //        var FileContentResult = memstream.ToArray();
    //        ret = new FileContentResult(FileContentResult, mime);
    //        return ret;
    //    }

    //}
}
