//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Niva.Erp.Web.Host.Reports.Conta {
    
    public partial class SoldContCurentReport : DevExpress.XtraReports.UI.XtraReport {
        private void InitializeComponent() {
            DevExpress.XtraReports.ReportInitializer reportInitializer = new DevExpress.XtraReports.ReportInitializer(this, "Niva.Erp.Web.Host.Reports.Conta.SoldContCurentReport.vsrepx");

            // Controls
            this.TopMargin = reportInitializer.GetControl<DevExpress.XtraReports.UI.TopMarginBand>("TopMargin");
            this.BottomMargin = reportInitializer.GetControl<DevExpress.XtraReports.UI.BottomMarginBand>("BottomMargin");
            this.ReportHeader = reportInitializer.GetControl<DevExpress.XtraReports.UI.ReportHeaderBand>("ReportHeader");
            this.Detail = reportInitializer.GetControl<DevExpress.XtraReports.UI.DetailBand>("Detail");
            this.DetailReport = reportInitializer.GetControl<DevExpress.XtraReports.UI.DetailReportBand>("DetailReport");
            this.GroupHeader4 = reportInitializer.GetControl<DevExpress.XtraReports.UI.GroupHeaderBand>("GroupHeader4");
            this.GroupHeader5 = reportInitializer.GetControl<DevExpress.XtraReports.UI.GroupHeaderBand>("GroupHeader5");
            this.GroupFooter4 = reportInitializer.GetControl<DevExpress.XtraReports.UI.GroupFooterBand>("GroupFooter4");
            this.label9 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRLabel>("label9");
            this.label8 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRLabel>("label8");
            this.label14 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRLabel>("label14");
            this.label15 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRLabel>("label15");
            this.label16 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRLabel>("label16");
            this.pageInfo2 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRPageInfo>("pageInfo2");
            this.pageInfo1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRPageInfo>("pageInfo1");
            this.GroupHeader2 = reportInitializer.GetControl<DevExpress.XtraReports.UI.GroupHeaderBand>("GroupHeader2");
            this.Detail1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.DetailBand>("Detail1");
            this.GroupFooter3 = reportInitializer.GetControl<DevExpress.XtraReports.UI.GroupFooterBand>("GroupFooter3");
            this.table3 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTable>("table3");
            this.tableRow4 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableRow>("tableRow4");
            this.tableCell10 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell10");
            this.table1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTable>("table1");
            this.tableRow1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableRow>("tableRow1");
            this.tableCell1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell1");
            this.tableCell2 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell2");
            this.tableCell3 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell3");
            this.tableCell5 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell5");
            this.tableCell6 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell6");
            this.tableCell7 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell7");
            this.tableCell4 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell4");
            this.panel2 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRPanel>("panel2");
            this.label5 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRLabel>("label5");
            this.label6 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRLabel>("label6");
            this.label7 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRLabel>("label7");
            this.table5 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTable>("table5");
            this.tableRow6 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableRow>("tableRow6");
            this.tableCell13 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell13");
            this.tableCell14 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell14");
            this.tableCell15 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell15");
            this.tableCell18 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell18");
            this.tableCell19 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell19");
            this.tableCell20 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell20");
            this.tableCell21 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell21");
            this.table4 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTable>("table4");
            this.tableRow5 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableRow>("tableRow5");
            this.tableCell12 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell12");

            // Parameters
            this.currencyId = reportInitializer.GetParameter("currencyId");
            this.accountId = reportInitializer.GetParameter("accountId");
            this.dataStart = reportInitializer.GetParameter("dataStart");
            this.dataEnd = reportInitializer.GetParameter("dataEnd");
            this.periodTypeId = reportInitializer.GetParameter("periodTypeId");
            this.isDateRange = reportInitializer.GetParameter("isDateRange");

            // Data Sources
            this.objectDataSource1 = reportInitializer.GetDataSource<DevExpress.DataAccess.ObjectBinding.ObjectDataSource>("objectDataSource1");
            this.objectDataSource1.DataSource = typeof(Niva.Erp.Web.Host.Reports.Conta.TestReportDataSource);

            // Styles
            this.Title = reportInitializer.GetStyle("Title");
            this.DetailCaption1 = reportInitializer.GetStyle("DetailCaption1");
            this.DetailData1 = reportInitializer.GetStyle("DetailData1");
            this.GroupCaption2 = reportInitializer.GetStyle("GroupCaption2");
            this.GroupData2 = reportInitializer.GetStyle("GroupData2");
            this.DetailCaption2 = reportInitializer.GetStyle("DetailCaption2");
            this.DetailData2 = reportInitializer.GetStyle("DetailData2");
            this.GroupFooterBackground3 = reportInitializer.GetStyle("GroupFooterBackground3");
            this.DetailData3_Odd = reportInitializer.GetStyle("DetailData3_Odd");
            this.TotalCaption2 = reportInitializer.GetStyle("TotalCaption2");
            this.TotalData2 = reportInitializer.GetStyle("TotalData2");
            this.TotalBackground2 = reportInitializer.GetStyle("TotalBackground2");
            this.GrandTotalCaption2 = reportInitializer.GetStyle("GrandTotalCaption2");
            this.GrandTotalData2 = reportInitializer.GetStyle("GrandTotalData2");
            this.GrandTotalBackground2 = reportInitializer.GetStyle("GrandTotalBackground2");
            this.PageInfo = reportInitializer.GetStyle("PageInfo");
        }
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.ReportHeaderBand ReportHeader;
        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.DetailReportBand DetailReport;
        private DevExpress.XtraReports.UI.GroupHeaderBand GroupHeader4;
        private DevExpress.XtraReports.UI.GroupHeaderBand GroupHeader5;
        private DevExpress.XtraReports.UI.GroupFooterBand GroupFooter4;
        private DevExpress.XtraReports.UI.XRLabel label9;
        private DevExpress.XtraReports.UI.XRLabel label8;
        private DevExpress.XtraReports.UI.XRLabel label14;
        private DevExpress.XtraReports.UI.XRLabel label15;
        private DevExpress.XtraReports.UI.XRLabel label16;
        private DevExpress.XtraReports.UI.XRPageInfo pageInfo2;
        private DevExpress.XtraReports.UI.XRPageInfo pageInfo1;
        private DevExpress.XtraReports.UI.GroupHeaderBand GroupHeader2;
        private DevExpress.XtraReports.UI.DetailBand Detail1;
        private DevExpress.XtraReports.UI.GroupFooterBand GroupFooter3;
        private DevExpress.XtraReports.UI.XRTable table3;
        private DevExpress.XtraReports.UI.XRTableRow tableRow4;
        private DevExpress.XtraReports.UI.XRTableCell tableCell10;
        private DevExpress.XtraReports.UI.XRTable table1;
        private DevExpress.XtraReports.UI.XRTableRow tableRow1;
        private DevExpress.XtraReports.UI.XRTableCell tableCell1;
        private DevExpress.XtraReports.UI.XRTableCell tableCell2;
        private DevExpress.XtraReports.UI.XRTableCell tableCell3;
        private DevExpress.XtraReports.UI.XRTableCell tableCell5;
        private DevExpress.XtraReports.UI.XRTableCell tableCell6;
        private DevExpress.XtraReports.UI.XRTableCell tableCell7;
        private DevExpress.XtraReports.UI.XRTableCell tableCell4;
        private DevExpress.XtraReports.UI.XRPanel panel2;
        private DevExpress.XtraReports.UI.XRLabel label5;
        private DevExpress.XtraReports.UI.XRLabel label6;
        private DevExpress.XtraReports.UI.XRLabel label7;
        private DevExpress.DataAccess.ObjectBinding.ObjectDataSource objectDataSource1;
        private DevExpress.XtraReports.UI.XRTable table5;
        private DevExpress.XtraReports.UI.XRTableRow tableRow6;
        private DevExpress.XtraReports.UI.XRTableCell tableCell13;
        private DevExpress.XtraReports.UI.XRTableCell tableCell14;
        private DevExpress.XtraReports.UI.XRTableCell tableCell15;
        private DevExpress.XtraReports.UI.XRTableCell tableCell18;
        private DevExpress.XtraReports.UI.XRTableCell tableCell19;
        private DevExpress.XtraReports.UI.XRTableCell tableCell20;
        private DevExpress.XtraReports.UI.XRTableCell tableCell21;
        private DevExpress.XtraReports.UI.XRTable table4;
        private DevExpress.XtraReports.UI.XRTableRow tableRow5;
        private DevExpress.XtraReports.UI.XRTableCell tableCell12;
        private DevExpress.XtraReports.UI.XRControlStyle Title;
        private DevExpress.XtraReports.UI.XRControlStyle DetailCaption1;
        private DevExpress.XtraReports.UI.XRControlStyle DetailData1;
        private DevExpress.XtraReports.UI.XRControlStyle GroupCaption2;
        private DevExpress.XtraReports.UI.XRControlStyle GroupData2;
        private DevExpress.XtraReports.UI.XRControlStyle DetailCaption2;
        private DevExpress.XtraReports.UI.XRControlStyle DetailData2;
        private DevExpress.XtraReports.UI.XRControlStyle GroupFooterBackground3;
        private DevExpress.XtraReports.UI.XRControlStyle DetailData3_Odd;
        private DevExpress.XtraReports.UI.XRControlStyle TotalCaption2;
        private DevExpress.XtraReports.UI.XRControlStyle TotalData2;
        private DevExpress.XtraReports.UI.XRControlStyle TotalBackground2;
        private DevExpress.XtraReports.UI.XRControlStyle GrandTotalCaption2;
        private DevExpress.XtraReports.UI.XRControlStyle GrandTotalData2;
        private DevExpress.XtraReports.UI.XRControlStyle GrandTotalBackground2;
        private DevExpress.XtraReports.UI.XRControlStyle PageInfo;
        private DevExpress.XtraReports.Parameters.Parameter currencyId;
        private DevExpress.XtraReports.Parameters.Parameter accountId;
        private DevExpress.XtraReports.Parameters.Parameter dataStart;
        private DevExpress.XtraReports.Parameters.Parameter dataEnd;
        private DevExpress.XtraReports.Parameters.Parameter periodTypeId;
        private DevExpress.XtraReports.Parameters.Parameter isDateRange;
    }
}