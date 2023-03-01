import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { ComisioaneServiceProxy, ComisionEditDto, ImprumutServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    
    templateUrl: './comisioaneEdit.component.html',
    animations: [appModuleAnimation()],
})
/** comisioaneEdit component*/
export class ComisioaneEditComponent extends AppComponentBase implements OnInit{
    /** comisioaneEdit ctor */
    comision: ComisionEditDto = new ComisionEditDto();
    imprumutId: number;
    comisionId: number;
    imprumutCreditare: string;


    constructor(injector: Injector,
        private route: ActivatedRoute,
        private router: Router,
        private _comisionService: ComisioaneServiceProxy,
        private _imprumutService: ImprumutServiceProxy,
         ) {
        super(injector);
    }
    ngOnInit(): void {
        this.imprumutId = + this.route.snapshot.queryParamMap.get('imprumutId');
        this.comisionId = + this.route.snapshot.queryParamMap.get('comisionId');
        
        if (this.imprumutId !== 0) {
            this.comision.imprumutId = this.imprumutId;
            this._imprumutService.imprumutId(this.imprumutId).subscribe(result => { this.imprumutCreditare = result.tipCreditare });
        }
        if (this.comisionId !== 0) {
            this._comisionService.getComisionId(this.comisionId).subscribe(result => {
                this.comision = result;
                this.imprumutId = this.comision.imprumutId;
                this._imprumutService.imprumutId(this.imprumutId).subscribe(result => { this.imprumutCreditare = result.tipCreditare });
            });
        }
    }
        saveComision() {
            

        this._comisionService.saveComision(this.comision).subscribe(() => {
                abp.notify.info(this.l('UpdateMessage'));

                this.router.navigate(['/app/imprumuturi/imprumuturiComisioaneList'], { queryParams: { imprumutId: this.imprumutId } });
            });
    }

        saveComisionLinieDeCredit() {
            this._comisionService.saveComisionLinieDeCredit(this.comision).subscribe(() => {
                abp.notify.info(this.l('UpdateMessage'));

                this.router.navigate(['/app/imprumuturi/imprumuturiComisioaneList'], { queryParams: { imprumutId: this.imprumutId } });
            });
        }
    
}