import { Component, Injector, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import * as moment from "moment";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { ImoInventariereEditDto, ImoInventariereServiceProxy } from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './imoInventariereEdit.component.html',
    animations: [appModuleAnimation()]
})
export class ImoInventariereEditComponent extends AppComponentBase implements OnInit {

    imoInventariere: ImoInventariereEditDto = new ImoInventariereEditDto();
    imoInvId: any;

    constructor(inject: Injector,
        private route: ActivatedRoute,
        private router: Router,
        private _imoInvService: ImoInventariereServiceProxy) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Conta.MF.Inventar.Modificare')) {
            this.imoInvId = this.route.snapshot.queryParamMap.get('imoInvId');

            this._imoInvService.getImoInventariere(this.imoInvId).subscribe(result => {
                this.imoInventariere = result;
            });
        }
        else {
            this.router.navigate(['/app/home']);
        }

    }

    search() {
        this._imoInvService.searchComputeImoInv(moment(this.imoInventariere.dateStart)).subscribe(result => {
            this.imoInventariere.imoInventariereDetails = result;
        });
    }

    save() {
        this._imoInvService.saveImoInv(this.imoInventariere).subscribe(() => {
            abp.notify.info('AddMessage');
            this.router.navigate(["/app/conta/imoAsset/imoInventariere"]);
        });
    }

}