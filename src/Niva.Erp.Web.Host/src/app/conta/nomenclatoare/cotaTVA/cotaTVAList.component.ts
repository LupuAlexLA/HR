import { Component, Injector, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { appModuleAnimation } from "../../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../../shared/app-component-base";
import { CotaTVAListDto, CotaTVAServiceProxy } from "../../../../shared/service-proxies/service-proxies";


@Component({
    templateUrl: './cotaTVAList.component.html',
    animations: [appModuleAnimation()]
})
export class CotaTVAListComponent extends AppComponentBase implements OnInit {

    coteTVAList: CotaTVAListDto[] = [];

    constructor(inject: Injector,
        private _cotaTVAService: CotaTVAServiceProxy,
        private router: Router    ) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Admin.Conta.CotaTVA.Acces')) {
            this.getTVAList();
        }
        else {
            this.router.navigate(['/app/home']);
        }
    }

    getTVAList() {
        this._cotaTVAService.getTVAList().subscribe(result => {
            this.coteTVAList = result;
        });
    }

    getCoteCount() {
        if (this.coteTVAList.length == 0) {
            return 0;
        } else {
            return this.coteTVAList.length;
        }
    }

    delete(cotaId: number) {
        abp.message.confirm("Cota TVA va fi stearsa. Sigur?",
            null, (response: boolean) => {
                if (response) {
                    this._cotaTVAService.deleteCotaTVA(cotaId).subscribe(result => {
                        abp.notify.info('cota TVA a fost stearsa');
                        this.getTVAList();
                    });
                }
            });
    }

}