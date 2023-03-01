import { Component, Injector, OnInit} from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { ComisioaneServiceProxy, ComisionDto, ImprumutDto, ImprumutServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './comisioaneList.component.html',
    animations: [appModuleAnimation()]
})
/** comisioaneList component*/
export class ComisioaneListComponent extends AppComponentBase implements OnInit{
    /** comisioaneList ctor */

    imprumut: ImprumutDto = new ImprumutDto();
    comisioane: ComisionDto[] = [];
    imprumutId: number;

    constructor(injector: Injector,
        private route: ActivatedRoute,
        private router: Router,
        private _imprumutService: ImprumutServiceProxy,
        private _comisioaneService: ComisioaneServiceProxy, ) {
        super(injector);
    }
    ngOnInit(): void {
        this.imprumutId = + this.route.snapshot.queryParamMap.get('imprumutId');
        this.getImprumutId(this.imprumutId);
        this.getComisioaneListId(this.imprumutId);
    }

    getComisioaneListId(imprumutId: number) {
        this._comisioaneService.comisionListId(imprumutId).subscribe(result => this.comisioane = result);
    }
    

    getImprumutId(imprumutId: number) {

        this._imprumutService.imprumutId(imprumutId).subscribe(result => this.imprumut = result);


    }
    getComisioaneCount() {
        if (this.comisioane.length > 0) {
            return this.comisioane.length;
        } else {
            return 0;
        }
    }
    delete(comisionId: number) {
        this._comisioaneService.deleteComision(comisionId).subscribe(() => {

            this.getComisioaneListId(this.imprumutId);

            abp.notify.success(this.l('DeleteMessage'));
        })
    }

}