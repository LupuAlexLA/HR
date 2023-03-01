import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { delay, finalize } from 'rxjs/operators';
import { appModuleAnimation } from '../../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../../shared/app-component-base';
import { OperGenerateAddDto, OperGenerateServiceServiceProxy } from '../../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './operGenerateAdd.component.html',
    animations: [appModuleAnimation()]
})
export class OperGenerateAddComponent extends AppComponentBase implements OnInit {

    operGenerate: OperGenerateAddDto = new OperGenerateAddDto();
    isLoading: boolean = false;

    constructor(inject: Injector,
        private _operGenerateService: OperGenerateServiceServiceProxy,
        private route: Router) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Conta.Balanta.SfarsitLuna.Modificare')) {
            this._operGenerateService.operGenerateAddInit().subscribe(result => {
                this.operGenerate = result;
            });
        } else {
            this.route.navigate(['app/home']);
        }
    }

    searchOper() {
        this._operGenerateService.operGenerateAddSearchOper(this.operGenerate).subscribe(result => {
            this.operGenerate = result;
        });
    }

    countOperList() {
        if (this.operGenerate.operatiiPropuse === undefined) {
            return 0;
        } else {
            return this.operGenerate.operatiiPropuse.length;
        }
    }

    selectAll() {
        for (var i = 0; i < this.operGenerate.operatiiPropuse.length; i++) {
            this.operGenerate.operatiiPropuse[i].selected = true;
        }
    }

    selectNone() {
        for (var i = 0; i < this.operGenerate.operatiiPropuse.length; i++) {
            this.operGenerate.operatiiPropuse[i].selected = false;
        }
    }

    saveOper() {
        this.isLoading = true;
        this._operGenerateService.operGenerateAdd(this.operGenerate).pipe(
            delay(1000),
            finalize(() => {
                this.isLoading = false;
            }))
            .subscribe(() => {
                abp.notify.info(this.l('Operatiile au fost contate'));
                this.route.navigate(['/app/conta/balance/operGenerate/operGenerateList']);
            });
    }
}