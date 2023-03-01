import { ChangeDetectionStrategy, Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { delay, finalize } from 'rxjs/operators';
import { appModuleAnimation } from '../../shared/animations/routerTransition';
import { IssuerListDto, IssuerServiceProxy } from '../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './issuer.component.html',
    animations: [appModuleAnimation()],
    changeDetection: ChangeDetectionStrategy.Default
})
export class IssuerComponent extends AppComponentBase implements OnInit {

    issuerList: IssuerListDto[];
    isLoading: boolean = false;

    constructor(injector: Injector,
        private _issuerService: IssuerServiceProxy,
        private router: Router) {
        super(injector);
    }

    ngOnInit() {
        if (this.isGranted('General.Emitenti.Acces')) {
            this.loadIssuerList();
        } else {
            this.router.navigate(['/app/home']);
        }

    }

    loadIssuerList() {
        this._issuerService.getIssuerList().subscribe(result => {
            this.issuerList = result;
        });
    }

    deleteIssuer(idIssuer: number) {
        abp.message.confirm(
            this.l('IssuerDeleteWarningMessage', idIssuer),
            undefined,
            (result: boolean) => {
                if (result) {
                    this._issuerService.deleteIssuer(idIssuer).pipe(finalize(() => {
                        abp.notify.success(this.l('SuccessfullyDeleted'));

                    })).subscribe(() => {
                        this.loadIssuerList();
                    });
                }
            }
        );
    }

    actualizeazaEmitenti() {
        this.isLoading = true;
        this._issuerService.actualizeazaIssuer().pipe(
            delay(1000),
            finalize(() => {
                this.isLoading = false;
            })
        ).subscribe(() => {
            abp.notify.success("Actualizare cu success");
            this.loadIssuerList();
        });

    }
}