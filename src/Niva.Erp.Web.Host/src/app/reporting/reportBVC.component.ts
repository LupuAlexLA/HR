import { Component, Injector, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { appModuleAnimation } from '../../shared/animations/routerTransition';
import { AppComponentBase } from '../../shared/app-component-base';
import { AppConsts } from '../../shared/AppConsts';
import {
    ActivityTypeDto, ActivityTypeServiceProxy, BugetBalRealizatSavedBalanceDateDto, BugetConfigDto, BugetConfigServiceProxy,
    BugetFormRandDto, BugetPrevListDto, BugetPrevServiceProxy, BugetRealizatSavedBalanceDateDto, BugetRealizatServiceProxy, EnumServiceProxy, EnumTypeDto
} from '../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './reportBVC.component.html',
    animations: [appModuleAnimation()]
})
export class ReportBVCComponent extends AppComponentBase implements OnInit {
    anBugetList: BugetConfigDto[] = [];
    bugetPrevList: BugetPrevListDto[] = [];
    savedBugetPrevList: BugetPrevListDto[] = [];
    bugetRealizatList: BugetRealizatSavedBalanceDateDto[] = [];
    bugetBalRealizatList: BugetBalRealizatSavedBalanceDateDto[] = [];
    anBugetId: any;
    selectedReport: any = "0";
    url: string = '';
    reportName: string = "";
    reportParams: any = {};
    rowTypeList: EnumTypeDto[] = [];
    bvcTypeList: EnumTypeDto[] = [];
    bvcTypeListResurse: EnumTypeDto[] = [];
    formRanduriList: BugetFormRandDto[] = [];
    nivelMax: any;

    selectedAnBuget: any;
    selectedTip: any;
    selectedFrecventa: any = "2";
    selectedTipActivitate: any = "0";
    selectedVersiuneBuget: any;
    selectedVersiuneBugetPreliminat: any;
    selectedBugetRealizatTip: any = "0";
    checkArray: FormArray;
    // Tip Rand
    form: FormGroup;

    isLoading: boolean = false;
    activityTypeList: ActivityTypeDto[];

    constructor(inject: Injector,
        private _bugetConfigService: BugetConfigServiceProxy,
        private _bugetPrevService: BugetPrevServiceProxy,
        private _activityTypeService: ActivityTypeServiceProxy,
        private _enumService: EnumServiceProxy,
        private _bugetRealizatService: BugetRealizatServiceProxy,
        private fb: FormBuilder,
        private router: Router) {
        super(inject);
        this.form = this.fb.group({
            checkArray: this.fb.array([], [Validators.required]),
        });
    }

    ngOnInit() {
        if (this.isGranted('Buget.BVC.Rapoarte')) {
            this.getAnBugetList();
            this.getBugetPrevList();
            this.getActivityTypeList();
            this.getRowTypeList();
            this.getBVCTypeList();
        } else {
            this.router.navigate(['app/home']);
        }
    }

    onCheckboxChange(e) {
        this.checkArray = this.form.get('checkArray') as FormArray

        if (e.target.checked) {
            this.checkArray.push(new FormControl(e.target.value));
        } else {
            let i: number = 0;
            this.checkArray.controls.forEach((item: FormControl) => {
                if (item.value == e.target.value) {
                    this.checkArray.removeAt(i);
                    return;
                }
                i++;
            });
        }
        this.reportParams.tipRand = this.form.get('checkArray').value.join('-');
    }

    changebvcTip() {
        if (this.checkArray !== undefined) {
            this.checkArray.clear();
        }
        this.reportParams = {};
        if (this.selectedReport == "4" || this.selectedReport == "1") {
            this.selectedTip = this.bvcTypeList.find(x => x.name == "CashFlow").id;
            this.getAnBugetList();
            this.getBugetPrevList();
            this.getActivityTypeList();
            this.getRowTypeList();
            this.getBugetRealizatList(this.selectedAnBuget, this.selectedTip);
            this.getBugetBalRealizatList(this.selectedAnBuget, this.selectedTip);
        }
        else {
            this.getBVCTypeList();
            this.getAnBugetList();
            this.getBugetPrevList();
            this.getActivityTypeList();
            this.getRowTypeList();
            this.getBugetRealizatList(this.selectedAnBuget, this.selectedTip);
            this.getBugetBalRealizatList(this.selectedAnBuget, this.selectedTip);
        }
    }

    showReport() {
        this.url = AppConsts.appBaseUrl + '/app/dXWebView/';
        switch (this.selectedReport) {
            case "0":
                this.reportParams.variantaBugetId = this.selectedVersiuneBuget;
                this.reportParams.nivelRand = this.nivelMax;
                this.reportParams.anBuget = this.selectedAnBuget;
                this.reportParams.tip = this.selectedTip;
                this.reportParams.frecventa = this.selectedFrecventa;
                this.reportParams.tipActivitate = this.selectedTipActivitate;
                this.reportParams.tipRaport = +this.selectedReport;
                this.url += 'PrevazutBVC';
                this.url += '?' + this.ConvertToQueryStringParameters(this.reportParams);
                break;

            case "1":
                this.reportParams.tipRealizat = +this.selectedBugetRealizatTip;
                this.reportParams.anBuget = this.selectedAnBuget;
                this.reportParams.tip = this.bvcTypeList.find(x => x.name === "CashFlow").id;
                this.reportParams.variantaBugetId = this.selectedVersiuneBuget;
                this.reportParams.nivelRand = this.nivelMax;
                this.reportParams.tipRaport = +this.selectedReport;
                this.url += 'BVC_Realizat';
                this.url += '?' + this.ConvertToQueryStringParameters(this.reportParams);
                break;
            case "2":
                this.reportParams.tipRealizat = +this.selectedBugetRealizatTip;
                this.reportParams.anBuget = this.selectedAnBuget;
                this.reportParams.tip = this.bvcTypeList.find(x => x.name === "BVC").id;
                this.reportParams.variantaBugetId = this.selectedVersiuneBuget;
                this.reportParams.nivelRand = this.nivelMax;
                this.reportParams.tipActivitate = this.selectedTipActivitate;
                this.reportParams.tipRaport = this.selectedReport;
                if (this.reportParams.includPrevazutAnual === true) {
                    this.url += 'BVC_BalRealizatIncludPrevAnual';
                } else {
                    this.url += 'BVC_BalRealizat';
                }
                this.url += '?' + this.ConvertToQueryStringParameters(this.reportParams);
                break;
            case "3":
                this.reportParams = {};
                this.reportParams.AnBVC = this.selectedAnBuget;
                this.url += 'BVC_Report';
                this.url += '?' + this.ConvertToQueryStringParameters(this.reportParams);
                break;
            case "4":
                this.reportParams.variantaBugetId = this.selectedVersiuneBuget;
                this.reportParams.anBVC = this.selectedAnBuget;
                this.url += 'BVC_PrevResurse';
                this.url += '?' + this.ConvertToQueryStringParameters(this.reportParams);
                break;
            case "5":
                this.reportParams.anBuget = this.selectedAnBuget;
                this.reportParams.tipRealizat = +this.selectedBugetRealizatTip;
                this.reportParams.tip = this.bvcTypeList.find(x => x.name === "BVC").id;
                this.reportParams.variantaBugetId = this.selectedVersiuneBuget;
                this.reportParams.nivelRand = this.nivelMax;
                this.reportParams.tipRaport = this.selectedReport;
                this.reportParams.bugetPreliminatId = this.selectedVersiuneBugetPreliminat;
                this.url += 'BugetPreliminatReport';
                this.url += '?' + this.ConvertToQueryStringParameters(this.reportParams);
                break;
            case "6":
                this.reportParams.anBuget = this.selectedAnBuget;
                this.reportParams.tipRealizat = +this.selectedBugetRealizatTip;
                this.reportParams.tip = this.bvcTypeList.find(x => x.name === "BVC").id;
                this.reportParams.variantaBugetId = this.selectedVersiuneBuget;
                this.reportParams.nivelRand = this.nivelMax;
                this.reportParams.tipRaport = this.selectedReport;
                this.reportParams.bugetPreliminatId = this.selectedVersiuneBugetPreliminat;
                this.url += 'BugetPreliminatDetaliiReport';
                this.url += '?' + this.ConvertToQueryStringParameters(this.reportParams);
                break;
        }
        window.open(this.url);
    }

    onChangeAnSauTip() {
        this.bugetPrevList = this.savedBugetPrevList;

        if (this.selectedAnBuget !== null) {
            this.bugetPrevList = this.bugetPrevList.filter(f => f.formularId == this.selectedAnBuget);
        }

        if (this.selectedTip != null) {
            this.bugetPrevList = this.bugetPrevList.filter(f => f.bvC_TipId == this.selectedTip);
        }

        this.getBugetRealizatList(this.selectedAnBuget, this.selectedTip);
        this.getBugetBalRealizatList(this.selectedAnBuget, this.selectedTip);
    }

    onChangeTip(tip) {
        this.bugetPrevList = this.savedBugetPrevList.filter(f => f.bvC_Tip == tip);
    }

    getBugetPrevList() {
        this._bugetPrevService.bugetPrevList().subscribe(result => {
            this.bugetPrevList = result;
            this.savedBugetPrevList = result;
            this.bugetPrevList = this.savedBugetPrevList.filter(f => f.formularId == this.selectedAnBuget && f.bvC_TipId == this.selectedTip);

            if (this.selectedReport === '4') {
                var cashFlowId = this.bvcTypeList.find(x => x.name === "CashFlow").id;
                this.bugetPrevList = this.savedBugetPrevList.filter(f => f.bvC_TipId == cashFlowId && f.formularId == this.selectedAnBuget);
            }
            this.selectedVersiuneBuget = result[0].id;
        });
    }

    getAnBugetList() {
        this._bugetConfigService.bugetConfigList().subscribe(result => {
            this.anBugetList = result;
            this.selectedAnBuget = result[0].id;
            this._bugetConfigService.bugetRandInit(this.selectedAnBuget).subscribe(result => {
                this.nivelMax = Math.max.apply(Math, result.randList.map(f => { return f.nivelRand; }));
            });
        });
    }

    getActivityTypeList() {
        this._activityTypeService.activityTypeList().subscribe(result => {
            this.activityTypeList = result;
        });
    }

    getRowTypeList() {
        this._enumService.bVC_RowTypeList2().subscribe(result => {
            this.rowTypeList = result;
        });
    }

    getBVCTypeList() {
        this._enumService.bvcTipList().subscribe(result => {
            this.bvcTypeList = result;
            this.selectedTip = result[0].id;
        });
    }

    getBugetRealizatList(anBugetId: number, bvcTip: number) {
        this._bugetRealizatService.bugetRealizatSavedBalanceDateList(anBugetId, bvcTip).subscribe(result => {
            this.bugetRealizatList = result;
        });
    }

    getBugetBalRealizatList(anBugetId: number, bvcTip: number) {
        this._bugetRealizatService.bugetBalRealizatSavedBalanceDateList(anBugetId, bvcTip).subscribe(result => {
            this.bugetBalRealizatList = result;
        });
    }

    private ConvertToQueryStringParameters(query: any) {
        return Object.keys(query).map(key => key + '=' + query[key]).join('&');
    }
}