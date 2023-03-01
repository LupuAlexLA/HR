import { Component, Injector, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import * as moment from "moment";
import { appModuleAnimation } from "../../shared/animations/routerTransition";
import { AppComponentBase } from "../../shared/app-component-base";
import { AppConsts } from "../../shared/AppConsts";

@Component({
    templateUrl: './declaratieCasier.component.html',
    animations: [appModuleAnimation()]
})
export class DeclaratieCasierComponent extends AppComponentBase implements OnInit {

    url: any;
    declaratieCasierParams: any = {};
    dataStart: any;
    dataDecizie: any;
    numeCasier: string;

    constructor(inject: Injector,
        private router: Router     ) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Casierie.Rapoarte.Acces')) {
            this.dataStart = new Date();
        }
        else {
            this.router.navigate(['/app/home']);
        }
    }

    showReport() {
        this.url = AppConsts.appBaseUrl + '/app/dXWebView/';

        this.declaratieCasierParams.data = moment(this.dataStart).format('M.DD.YYYY');
        this.declaratieCasierParams.dataDecizie = moment(this.dataDecizie).format('M.DD.YYYY');
        this.declaratieCasierParams.numeCasier = this.numeCasier.replace(/\s+/g, '-');
        this.url += "DeclaratieCasier?";
        this.url += this.ConvertToQueryStringParameters(this.declaratieCasierParams);

        window.open(this.url);
    }


    private ConvertToQueryStringParameters(query: any) {
        return Object.keys(query).map(key => key + '=' + query[key]).join('&');
    }

}