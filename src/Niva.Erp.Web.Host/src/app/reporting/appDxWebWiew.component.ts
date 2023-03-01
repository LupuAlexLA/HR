import { AfterViewInit, Component, Inject, Injector, OnInit, ViewChild, ViewEncapsulation} from '@angular/core';
import { AppComponentBase } from '../../shared/app-component-base';
import { DxReportViewerComponent } from 'devexpress-reporting-angular';
import { ajaxSetup } from "@devexpress/analytics-core/analytics-utils";
import { AsyncExportApproach } from "devexpress-reporting/scopes/reporting-viewer-settings";
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { AppConsts } from '../../shared/AppConsts';
import { ActionId } from 'devexpress-reporting/viewer/constants';
import { PreviewElements } from 'devexpress-reporting/viewer/constants';
import * as ko from 'knockout';
 
@Component({
    selector: 'appDxWebWiew',
    encapsulation: ViewEncapsulation.None,
    templateUrl: './appDxWebWiew.component.html',
    styleUrls: [ 
        //"../../../src/assets/devexpress/devextreme/dist/css/dx.common.css",
        //"../../../src/assets/devexpress/devextreme/dist/css/dx.light.css",
        "../../../src/assets/devexpress/@devexpress/analytics-core/dist/css/dx-analytics.common.css",
        "../../../src/assets/devexpress/@devexpress/analytics-core/dist/css/dx-analytics.light.css",
        "../../../src/assets/devexpress/devexpress-reporting/dist/css/dx-webdocumentviewer.css"]
})
/** dxWebWiew component*/
export class appDxWebWiewComponent extends AppComponentBase implements OnInit, AfterViewInit {
    @ViewChild(DxReportViewerComponent) viewer: DxReportViewerComponent; 
    title = 'DXReportViewerSample';
    // The report's path. The Document Viewer opens it when the application starts.
    reportUrl: string = '';
    // Backend's project URI.
    
    hostUrl: string = '';
    // Use this line if you use an ASP.NET MVC backend
    //invokeAction: string = "/WebDocumentViewer/Invoke";
    // Uncomment this line if you use an ASP.NET Core backend
    invokeAction: string = 'DXXRDV';
    reportName: string = '';
    token = abp.auth.getToken();

    queryParams: any;

    /** dxWebWiew ctor */
    constructor(inject: Injector,
        private route: ActivatedRoute) {
        super(inject);
        ajaxSetup.ajaxSettings = { headers: { 'Authorization': 'Bearer '+ this.token } };
        AsyncExportApproach(true);
      
        this.hostUrl = AppConsts.remoteServiceBaseUrl + "/";
    }


    ngOnInit() {
        this.route.queryParams.subscribe(params => {
            // convertesc obiectul params in query string parameters
            this.queryParams = Object.keys(params).map(key => key + '=' + params[key]).join('&');
        });

        this.reportName = this.route.snapshot.paramMap.get('reportName');
        this.reportUrl = this.reportName + '?' + this.queryParams;
    }

    ngAfterViewInit() {
        this.viewer.bindingSender.OpenReport(this.reportUrl);
    }
    

    print() {
        this.viewer.bindingSender.Print();
    }  

    open() {
        this.viewer.bindingSender.OpenReport(this.reportUrl);
    }

    onCustomizeParameterEditors(event) {
        let e = event.args;
        if (this.reportName == 'FisaCont') {
            if (e.parameter.name === 'accountId') {
                e.parameter.visible = false;
            }
        } 
    }

    CustomizeMenuActions(event) {
        // Hide the "Print" and "PrintPage" actions. 
        //var printAction = event.args.GetById(ActionId.Print);
        //if (printAction)
        //    printAction.visible = false;
        //var printPageAction = event.args.GetById(ActionId.PrintPage);
        //if (printPageAction)
        //    printPageAction.visible = false;
        //var exportAction = event.args.GetById(ActionId.ExportTo);
        //if (exportAction)
            //exportAction.visible = false;
        var interval;
        var selected = ko.observable(false);
        var customData = this.reportUrl;
        event.args.Actions.push({
            text: "Salvare FileDoc",
            imageTemplateName: "slideshow",
            visible: true,
            disabled: false,
            selected: selected,
            hasSeparator: false,
            hotKey: { ctrlKey: true, keyCode: "Z".charCodeAt(0) },
            clickAction: function () {
                //if (selected()) {
                //    clearInterval(interval);
                //    selected(false);
                //    return;
                //}
                var model = event.sender.GetPreviewModel();
                //if (model) {
                //    selected(true);
                //    interval = setInterval(function () {
                //        var pageIndex = model.GetCurrentPageIndex();
                //        model.GoToPage(pageIndex + 1);
                //    }, 2000);
                //}
                model.PerformCustomDocumentOperation(customData); //todo add parameters
            }
        });

    }

    onCustomizeElements(event) {
        var toolbarPart = event.args.GetById(PreviewElements.Toolbar);
        var index = event.args.Elements.indexOf(toolbarPart);
        var toolbar = toolbarPart[index];
        
        //event.args.Elements.splice(index, 1);
    }

}