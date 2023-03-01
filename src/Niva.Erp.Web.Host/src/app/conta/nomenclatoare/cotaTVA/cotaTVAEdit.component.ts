import { Component, Injector, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { appModuleAnimation } from "../../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../../shared/app-component-base";
import { CotaTVAEditDto, CotaTVAServiceProxy } from "../../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './cotaTVAEdit.component.html',
    animations: [appModuleAnimation()]
})
export class CotaTVAEditComponent extends AppComponentBase implements OnInit {

    cotaTVA: CotaTVAEditDto = new CotaTVAEditDto();
    cotaId: number;

    constructor(inject: Injector,
        private _cotaTVAService: CotaTVAServiceProxy,
        private route: ActivatedRoute,
        private router: Router) {
        super(inject);
    }

    ngOnInit(): void {
        this.cotaId = + this.route.snapshot.queryParamMap.get('cotaId');

        this._cotaTVAService.getTVAById(this.cotaId).subscribe(result => {
            this.cotaTVA = result;
        });
    }

    save() {
        this._cotaTVAService.saveCotaTVA(this.cotaTVA).subscribe(() => {
            abp.notify.info("Cota TVA a fost adaugata");
            this.router.navigate(['/app/conta/nomenclatoare/cotaTVA/cotaTVAList']);
        });
    }
}