import { Component, Inject, ViewEncapsulation } from '@angular/core';
import 'devexpress-reporting/dx-richedit';

@Component({
    selector: 'appDxWebEdit',
    templateUrl: './appDxWebEdit.component.html',
    styleUrls: [
        
        "../../../assets/devextreme/dist/css/dx.common.css",
        "../../../assets/devextreme/dist/css/dx.light.css",
        "../../../assets/devexpress-richedit/dist/dx.richedit.css",
        "../../../assets/@devexpress/analytics-core/dist/css/dx-analytics.common.css",
        "../../../assets/@devexpress/analytics-core/dist/css/dx-analytics.light.css",
        "../../../assets/@devexpress/analytics-core/dist/css/dx-querybuilder.css",
        "../../../assets/devexpress-reporting/dist/css/dx-webdocumentviewer.css",
        "../../../assets/devexpress-reporting/dist/css/dx-reportdesigner.css"]
})
/** appDxWebEdit component*/
export class AppDxWebEditComponent {
    /** appDxWebEdit ctor */
    constructor() {

    }
}