//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Niva.Erp.Web.Host.Reports.Conta.SitFinan {
    
    public partial class SitFinanRapC2 : DevExpress.XtraReports.UI.XtraReport {
        private void InitializeComponent() {
            DevExpress.XtraReports.ReportInitializer reportInitializer = new DevExpress.XtraReports.ReportInitializer(this, "Niva.Erp.Web.Host.Reports.Conta.SitFinan.SitFinanRapC2.vsrepx");

            // Controls
            this.topMarginBand1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.TopMarginBand>("topMarginBand1");
            this.detailBand1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.DetailBand>("detailBand1");
            this.bottomMarginBand1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.BottomMarginBand>("bottomMarginBand1");
            this.ReportHeader = reportInitializer.GetControl<DevExpress.XtraReports.UI.ReportHeaderBand>("ReportHeader");
            this.GroupHeader1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.GroupHeaderBand>("GroupHeader1");
            this.GroupHeader2 = reportInitializer.GetControl<DevExpress.XtraReports.UI.GroupHeaderBand>("GroupHeader2");
            this.DetailReport = reportInitializer.GetControl<DevExpress.XtraReports.UI.DetailReportBand>("DetailReport");
            this.GroupFooter2 = reportInitializer.GetControl<DevExpress.XtraReports.UI.GroupFooterBand>("GroupFooter2");
            this.label6 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRLabel>("label6");
            this.label3 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRLabel>("label3");
            this.label2 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRLabel>("label2");
            this.label1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRLabel>("label1");
            this.pageInfo2 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRPageInfo>("pageInfo2");
            this.pageInfo1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRPageInfo>("pageInfo1");
            this.table1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTable>("table1");
            this.tableRow3 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableRow>("tableRow3");
            this.tableRow1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableRow>("tableRow1");
            this.tableCell11 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell11");
            this.tableCell12 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell12");
            this.tableCell13 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell13");
            this.tableCell15 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell15");
            this.tableCell1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell1");
            this.tableCell4 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell4");
            this.tableCell10 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell10");
            this.tableCell2 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell2");
            this.tableCell3 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell3");
            this.label4 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRLabel>("label4");
            this.Detail = reportInitializer.GetControl<DevExpress.XtraReports.UI.DetailBand>("Detail");
            this.GroupFooter1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.GroupFooterBand>("GroupFooter1");
            this.table2 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTable>("table2");
            this.tableRow2 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableRow>("tableRow2");
            this.tableCell5 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell5");
            this.tableCell6 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell6");
            this.tableCell9 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell9");
            this.tableCell7 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell7");
            this.tableCell8 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell8");
            this.label5 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRLabel>("label5");

            // Parameters
            this.balanceId = reportInitializer.GetParameter("balanceId");
            this.raportId = reportInitializer.GetParameter("raportId");
            this.isDailyBalance = reportInitializer.GetParameter("isDailyBalance");
            this.isDateRange = reportInitializer.GetParameter("isDateRange");
            this.startDate = reportInitializer.GetParameter("startDate");
            this.endDate = reportInitializer.GetParameter("endDate");
            this.colNumber = reportInitializer.GetParameter("colNumber");

            // Data Sources
            this.objectDataSource1 = reportInitializer.GetDataSource<DevExpress.DataAccess.ObjectBinding.ObjectDataSource>("objectDataSource1");
            this.objectDataSource1.DataSource = typeof(Niva.Erp.Web.Host.Reports.Conta.TestReportDataSource);

            // Styles
            this.DetailData3_Odd = reportInitializer.GetStyle("DetailData3_Odd");
        }
        private DevExpress.XtraReports.UI.TopMarginBand topMarginBand1;
        private DevExpress.XtraReports.UI.DetailBand detailBand1;
        private DevExpress.XtraReports.UI.BottomMarginBand bottomMarginBand1;
        private DevExpress.XtraReports.UI.ReportHeaderBand ReportHeader;
        private DevExpress.XtraReports.UI.GroupHeaderBand GroupHeader1;
        private DevExpress.XtraReports.UI.GroupHeaderBand GroupHeader2;
        private DevExpress.XtraReports.UI.DetailReportBand DetailReport;
        private DevExpress.XtraReports.UI.GroupFooterBand GroupFooter2;
        private DevExpress.XtraReports.UI.XRLabel label6;
        private DevExpress.XtraReports.UI.XRLabel label3;
        private DevExpress.XtraReports.UI.XRLabel label2;
        private DevExpress.XtraReports.UI.XRLabel label1;
        private DevExpress.XtraReports.UI.XRPageInfo pageInfo2;
        private DevExpress.XtraReports.UI.XRPageInfo pageInfo1;
        private DevExpress.XtraReports.UI.XRTable table1;
        private DevExpress.XtraReports.UI.XRTableRow tableRow3;
        private DevExpress.XtraReports.UI.XRTableRow tableRow1;
        private DevExpress.XtraReports.UI.XRTableCell tableCell11;
        private DevExpress.XtraReports.UI.XRTableCell tableCell12;
        private DevExpress.XtraReports.UI.XRTableCell tableCell13;
        private DevExpress.XtraReports.UI.XRTableCell tableCell15;
        private DevExpress.XtraReports.UI.XRTableCell tableCell1;
        private DevExpress.XtraReports.UI.XRTableCell tableCell4;
        private DevExpress.XtraReports.UI.XRTableCell tableCell10;
        private DevExpress.XtraReports.UI.XRTableCell tableCell2;
        private DevExpress.XtraReports.UI.XRTableCell tableCell3;
        private DevExpress.XtraReports.UI.XRLabel label4;
        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.GroupFooterBand GroupFooter1;
        private DevExpress.XtraReports.UI.XRTable table2;
        private DevExpress.XtraReports.UI.XRTableRow tableRow2;
        private DevExpress.XtraReports.UI.XRTableCell tableCell5;
        private DevExpress.XtraReports.UI.XRTableCell tableCell6;
        private DevExpress.XtraReports.UI.XRTableCell tableCell9;
        private DevExpress.XtraReports.UI.XRTableCell tableCell7;
        private DevExpress.XtraReports.UI.XRTableCell tableCell8;
        private DevExpress.DataAccess.ObjectBinding.ObjectDataSource objectDataSource1;
        private DevExpress.XtraReports.UI.XRLabel label5;
        private DevExpress.XtraReports.UI.XRControlStyle DetailData3_Odd;
        private DevExpress.XtraReports.Parameters.Parameter balanceId;
        private DevExpress.XtraReports.Parameters.Parameter raportId;
        private DevExpress.XtraReports.Parameters.Parameter isDailyBalance;
        private DevExpress.XtraReports.Parameters.Parameter isDateRange;
        private DevExpress.XtraReports.Parameters.Parameter startDate;
        private DevExpress.XtraReports.Parameters.Parameter endDate;
        private DevExpress.XtraReports.Parameters.Parameter colNumber;
    }
}
