//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Niva.Erp.Web.Host.Reports.Conta.BVC {
    
    public partial class BVC_PrevResurse : DevExpress.XtraReports.UI.XtraReport {
        private void InitializeComponent() {
            DevExpress.XtraReports.ReportInitializer reportInitializer = new DevExpress.XtraReports.ReportInitializer(this, "Niva.Erp.Web.Host.Reports.Conta.BVC.BVC_PrevResurse.vsrepx");

            // Controls
            this.TopMargin = reportInitializer.GetControl<DevExpress.XtraReports.UI.TopMarginBand>("TopMargin");
            this.BottomMargin = reportInitializer.GetControl<DevExpress.XtraReports.UI.BottomMarginBand>("BottomMargin");
            this.Detail = reportInitializer.GetControl<DevExpress.XtraReports.UI.DetailBand>("Detail");
            this.DetailReport = reportInitializer.GetControl<DevExpress.XtraReports.UI.DetailReportBand>("DetailReport");
            this.label3 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRLabel>("label3");
            this.label1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRLabel>("label1");
            this.label2 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRLabel>("label2");
            this.pageInfo2 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRPageInfo>("pageInfo2");
            this.pageInfo1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRPageInfo>("pageInfo1");
            this.Detail1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.DetailBand>("Detail1");
            this.crossTab1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRCrossTab>("crossTab1");
            this.crossTabHeaderCell1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell>("crossTabHeaderCell1");
            this.crossTabDataCell1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell>("crossTabDataCell1");
            this.crossTabHeaderCell2 = reportInitializer.GetControl<DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell>("crossTabHeaderCell2");
            this.crossTabHeaderCell3 = reportInitializer.GetControl<DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell>("crossTabHeaderCell3");
            this.crossTabHeaderCell4 = reportInitializer.GetControl<DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell>("crossTabHeaderCell4");
            this.crossTabTotalCell1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell>("crossTabTotalCell1");
            this.crossTabHeaderCell5 = reportInitializer.GetControl<DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell>("crossTabHeaderCell5");
            this.crossTabTotalCell2 = reportInitializer.GetControl<DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell>("crossTabTotalCell2");
            this.crossTabTotalCell3 = reportInitializer.GetControl<DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell>("crossTabTotalCell3");
            this.crossTabHeaderCell6 = reportInitializer.GetControl<DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell>("crossTabHeaderCell6");
            this.crossTabHeaderCell7 = reportInitializer.GetControl<DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell>("crossTabHeaderCell7");
            this.crossTabHeaderCell8 = reportInitializer.GetControl<DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell>("crossTabHeaderCell8");
            this.crossTabTotalCell4 = reportInitializer.GetControl<DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell>("crossTabTotalCell4");
            this.crossTabTotalCell5 = reportInitializer.GetControl<DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell>("crossTabTotalCell5");

            // Parameters
            this.variantaBugetId = reportInitializer.GetParameter("variantaBugetId");
            this.anBVC = reportInitializer.GetParameter("anBVC");

            // Data Sources
            this.objectDataSource1 = reportInitializer.GetDataSource<DevExpress.DataAccess.ObjectBinding.ObjectDataSource>("objectDataSource1");
            this.objectDataSource1.DataSource = typeof(Niva.Erp.Web.Host.Reports.Conta.TestReportDataSource);

            // Styles
            this.crossTabGeneralStyle1 = reportInitializer.GetStyle("crossTabGeneralStyle1");
            this.crossTabHeaderStyle1 = reportInitializer.GetStyle("crossTabHeaderStyle1");
            this.crossTabDataStyle1 = reportInitializer.GetStyle("crossTabDataStyle1");
            this.crossTabTotalStyle1 = reportInitializer.GetStyle("crossTabTotalStyle1");
        }
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.DetailReportBand DetailReport;
        private DevExpress.XtraReports.UI.XRLabel label3;
        private DevExpress.XtraReports.UI.XRLabel label1;
        private DevExpress.XtraReports.UI.XRLabel label2;
        private DevExpress.XtraReports.UI.XRPageInfo pageInfo2;
        private DevExpress.XtraReports.UI.XRPageInfo pageInfo1;
        private DevExpress.XtraReports.UI.DetailBand Detail1;
        private DevExpress.XtraReports.UI.XRCrossTab crossTab1;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabHeaderCell1;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabDataCell1;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabHeaderCell2;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabHeaderCell3;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabHeaderCell4;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabTotalCell1;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabHeaderCell5;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabTotalCell2;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabTotalCell3;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabHeaderCell6;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabHeaderCell7;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabHeaderCell8;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabTotalCell4;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabTotalCell5;
        private DevExpress.DataAccess.ObjectBinding.ObjectDataSource objectDataSource1;
        private DevExpress.XtraReports.UI.XRControlStyle crossTabGeneralStyle1;
        private DevExpress.XtraReports.UI.XRControlStyle crossTabHeaderStyle1;
        private DevExpress.XtraReports.UI.XRControlStyle crossTabDataStyle1;
        private DevExpress.XtraReports.UI.XRControlStyle crossTabTotalStyle1;
        private DevExpress.XtraReports.Parameters.Parameter variantaBugetId;
        private DevExpress.XtraReports.Parameters.Parameter anBVC;
    }
}
