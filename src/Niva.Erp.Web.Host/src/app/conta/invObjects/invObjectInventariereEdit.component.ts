import { Component, Injector, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { delay, finalize } from "rxjs/operators";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { InvObjectInventariereEditDto, InvObjectInventariereServiceProxy } from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './invObjectInventariereEdit.component.html',
    animations: [appModuleAnimation()]
})
export class InvObjectInventariereEditComponent extends AppComponentBase implements OnInit {

    invObject: InvObjectInventariereEditDto = new InvObjectInventariereEditDto();
    invObjectId: any;
    isLoading: boolean = false;

    constructor(inject: Injector,
        private _invObjectInventariereService: InvObjectInventariereServiceProxy,
        private route: ActivatedRoute,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Conta.ObInventar.Inventar.Acces')) {
            this.invObjectId = this.route.snapshot.queryParamMap.get('invObjectId');
            this._invObjectInventariereService.getInvObjectInvetariere(this.invObjectId).subscribe(result => {
                this.invObject = result;
            });
        } else {
            this.router.navigate(['app/home']);
        }
    }

    search() {
        this._invObjectInventariereService.searchComputeInvObject(this.invObject.dataInventariere).subscribe(result => {
            this.invObject.invObjectInventariereDetails = result;
        });
    }

    save() {
        this.isLoading = true;
        this._invObjectInventariereService.saveInvObjectInventariere(this.invObject).pipe(
            delay(1000),
            finalize(() => {
                this.isLoading = false
            }))
                .subscribe(() => {
            abp.notify.info('AddMessage');
            this.router.navigate(["/app/conta/invObjects/invObjectInventariere"]);
        });
    }
}