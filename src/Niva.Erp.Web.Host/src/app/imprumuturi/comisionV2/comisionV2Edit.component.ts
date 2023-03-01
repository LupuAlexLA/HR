import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { ComisioaneServiceProxy, ComisionEditDto, ComisionV2EditDto, ImprumutServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    
    templateUrl: './comisionV2Edit.component.html',
    animations: [appModuleAnimation()],
})
/** comisioaneEdit component*/
export class ComisionV2EditComponent extends AppComponentBase implements OnInit{
    /** comisioaneEdit ctor */
    comision: ComisionV2EditDto = new ComisionV2EditDto();
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
            this._comisionService.getComisionV2Id(this.comisionId).subscribe(result => {
                this.comision = result;
                this.imprumutId = this.comision.imprumutId;
                this._imprumutService.imprumutId(this.imprumutId).subscribe(result => { this.imprumutCreditare = result.tipCreditare });
            });
        }
    }
        saveComision() {
            

        this._comisionService.saveComisionV2(this.comision).subscribe(() => {
                abp.notify.info(this.l('UpdateMessage'));

                this.router.navigate(['/app/imprumuturi/imprumuturiComisioaneV2List'], { queryParams: { imprumutId: this.imprumutId } });
            });
    }

        
    
}