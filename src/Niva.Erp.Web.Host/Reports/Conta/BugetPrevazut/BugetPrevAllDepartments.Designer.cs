//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Niva.Erp.Web.Host.Reports.Conta.BugetPrevazut {
    
    public partial class BugetPrevAllDepartments : DevExpress.XtraReports.UI.XtraReport {
        private void InitializeComponent() {
            DevExpress.XtraReports.ReportInitializer reportInitializer = new DevExpress.XtraReports.ReportInitializer(this, "Niva.Erp.Web.Host.Reports.Conta.BugetPrevazut.BugetPrevAllDepartments.vsrepx");

            // Controls
            this.Detail = reportInitializer.GetControl<DevExpress.XtraReports.UI.DetailBand>("Detail");
            this.TopMargin = reportInitializer.GetControl<DevExpress.XtraReports.UI.TopMarginBand>("TopMargin");
            this.BottomMargin = reportInitializer.GetControl<DevExpress.XtraReports.UI.BottomMarginBand>("BottomMargin");
            this.DetailReport = reportInitializer.GetControl<DevExpress.XtraReports.UI.DetailReportBand>("DetailReport");
            this.crossTab1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRCrossTab>("crossTab1");
            this.crossTabCell1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell>("crossTabCell1");
            this.crossTabCell2 = reportInitializer.GetControl<DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell>("crossTabCell2");
            this.crossTabCell3 = reportInitializer.GetControl<DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell>("crossTabCell3");
            this.crossTabCell4 = reportInitializer.GetControl<DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell>("crossTabCell4");
            this.crossTabCell5 = reportInitializer.GetControl<DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell>("crossTabCell5");
            this.crossTabCell6 = reportInitializer.GetControl<DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell>("crossTabCell6");
            this.crossTabCell7 = reportInitializer.GetControl<DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell>("crossTabCell7");
            this.crossTabCell8 = reportInitializer.GetControl<DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell>("crossTabCell8");
            this.crossTabCell9 = reportInitializer.GetControl<DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell>("crossTabCell9");
            this.crossTabCell10 = reportInitializer.GetControl<DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell>("crossTabCell10");
            this.crossTabCell11 = reportInitializer.GetControl<DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell>("crossTabCell11");
            this.crossTabCell12 = reportInitializer.GetControl<DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell>("crossTabCell12");
            this.crossTabCell13 = reportInitializer.GetControl<DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell>("crossTabCell13");
            this.crossTabCell14 = reportInitializer.GetControl<DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell>("crossTabCell14");
            this.label2 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRLabel>("label2");
            this.xrLabel2 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRLabel>("xrLabel2");
            this.label3 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRLabel>("label3");
            this.label4 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRLabel>("label4");
            this.pageInfo1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRPageInfo>("pageInfo1");
            this.pageInfo2 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRPageInfo>("pageInfo2");
            this.Detail1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.DetailBand>("Detail1");
            this.crossTab2 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRCrossTab>("crossTab2");
            this.crossTabHeaderCell1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell>("crossTabHeaderCell1");
            this.crossTabDataCell1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell>("crossTabDataCell1");
            this.crossTabHeaderCell2 = reportInitializer.GetControl<DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell>("crossTabHeaderCell2");
            this.crossTabHeaderCell3 = reportInitializer.GetControl<DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell>("crossTabHeaderCell3");
            this.crossTabHeaderCell4 = reportInitializer.GetControl<DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell>("crossTabHeaderCell4");
            this.crossTabTotalCell1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell>("crossTabTotalCell1");
            this.crossTabHeaderCell5 = reportInitializer.GetControl<DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell>("crossTabHeaderCell5");
            this.crossTabTotalCell2 = reportInitializer.GetControl<DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell>("crossTabTotalCell2");
            this.crossTabTotalCell3 = reportInitializer.GetControl<DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell>("crossTabTotalCell3");

            // Parameters
            this.departmentId = reportInitializer.GetParameter("departmentId");
            this.formularId = reportInitializer.GetParameter("formularId");
            this.activityType = reportInitializer.GetParameter("activityType");
            this.bugetPrevId = reportInitializer.GetParameter("bugetPrevId");

            // Data Sources
            this.objectDataSource1 = reportInitializer.GetDataSource<DevExpress.DataAccess.ObjectBinding.ObjectDataSource>("objectDataSource1");
            this.objectDataSource1.DataSource = typeof(Niva.Erp.Web.Host.Reports.Conta.TestReportDataSource);

            // Styles
            this.crossTabTotalStyle1 = reportInitializer.GetStyle("crossTabTotalStyle1");
            this.crossTabHeaderStyle1 = reportInitializer.GetStyle("crossTabHeaderStyle1");
            this.crossTabDataStyle1 = reportInitializer.GetStyle("crossTabDataStyle1");
            this.crossTabGeneralStyle1 = reportInitializer.GetStyle("crossTabGeneralStyle1");
            this.TitleStyle = reportInitializer.GetStyle("TitleStyle");
            this.crossTabGeneralStyle = reportInitializer.GetStyle("crossTabGeneralStyle");
            this.crossTabHeaderStyle = reportInitializer.GetStyle("crossTabHeaderStyle");
            this.crossTabDataStyle = reportInitializer.GetStyle("crossTabDataStyle");
            this.crossTabTotalStyle = reportInitializer.GetStyle("crossTabTotalStyle");
            this.crossTabGeneralStyle2 = reportInitializer.GetStyle("crossTabGeneralStyle2");
            this.crossTabHeaderStyle2 = reportInitializer.GetStyle("crossTabHeaderStyle2");
            this.crossTabDataStyle2 = reportInitializer.GetStyle("crossTabDataStyle2");
            this.crossTabTotalStyle2 = reportInitializer.GetStyle("crossTabTotalStyle2");
        }
        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.DetailReportBand DetailReport;
        private DevExpress.XtraReports.UI.XRCrossTab crossTab1;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabCell1;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabCell2;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabCell3;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabCell4;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabCell5;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabCell6;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabCell7;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabCell8;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabCell9;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabCell10;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabCell11;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabCell12;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabCell13;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabCell14;
        private DevExpress.DataAccess.ObjectBinding.ObjectDataSource objectDataSource1;
        private DevExpress.XtraReports.UI.XRLabel label2;
        private DevExpress.XtraReports.UI.XRLabel xrLabel2;
        private DevExpress.XtraReports.UI.XRLabel label3;
        private DevExpress.XtraReports.UI.XRLabel label4;
        private DevExpress.XtraReports.UI.XRPageInfo pageInfo1;
        private DevExpress.XtraReports.UI.XRPageInfo pageInfo2;
        private DevExpress.XtraReports.UI.DetailBand Detail1;
        private DevExpress.XtraReports.UI.XRCrossTab crossTab2;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabHeaderCell1;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabDataCell1;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabHeaderCell2;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabHeaderCell3;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabHeaderCell4;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabTotalCell1;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabHeaderCell5;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabTotalCell2;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabTotalCell3;
        private DevExpress.XtraReports.UI.XRControlStyle crossTabTotalStyle1;
        private DevExpress.XtraReports.UI.XRControlStyle crossTabHeaderStyle1;
        private DevExpress.XtraReports.UI.XRControlStyle crossTabDataStyle1;
        private DevExpress.XtraReports.UI.XRControlStyle crossTabGeneralStyle1;
        private DevExpress.XtraReports.UI.XRControlStyle TitleStyle;
        private DevExpress.XtraReports.UI.XRControlStyle crossTabGeneralStyle;
        private DevExpress.XtraReports.UI.XRControlStyle crossTabHeaderStyle;
        private DevExpress.XtraReports.UI.XRControlStyle crossTabDataStyle;
        private DevExpress.XtraReports.UI.XRControlStyle crossTabTotalStyle;
        private DevExpress.XtraReports.UI.XRControlStyle crossTabGeneralStyle2;
        private DevExpress.XtraReports.UI.XRControlStyle crossTabHeaderStyle2;
        private DevExpress.XtraReports.UI.XRControlStyle crossTabDataStyle2;
        private DevExpress.XtraReports.UI.XRControlStyle crossTabTotalStyle2;
        private DevExpress.XtraReports.Parameters.Parameter departmentId;
        private DevExpress.XtraReports.Parameters.Parameter formularId;
        private DevExpress.XtraReports.Parameters.Parameter activityType;
        private DevExpress.XtraReports.Parameters.Parameter bugetPrevId;
    }
}