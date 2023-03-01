import { Component, Injector, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { appModuleAnimation } from "../../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../../shared/app-component-base";
import { PaapPierdeDto, PaapPrimesteDto, PaapRedistribuireDetaliiDto, PaapRedistribuireDto, PaapRedistribuireServiceProxy } from "../../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './redistibuirePaapAdd.component.html',
    animations: [appModuleAnimation()]
})
export class RedistribuirePaapAddComponent extends AppComponentBase implements OnInit {
    paapPrimesteListDto: PaapPrimesteDto[] = [];
    paapRedistribuireDto: PaapRedistribuireDto = new PaapRedistribuireDto();
    paapPierdeListDto: PaapPierdeDto[] = [];
    paapRedistribDetaliiList: PaapRedistribuireDetaliiDto[] = [];
    an: any;
    paapPrimesteSuma: any;
    categorieId; any;
    paapPrimesteDto: PaapPrimesteDto = new PaapPrimesteDto();
    paapPierdeDto: PaapPierdeDto = new PaapPierdeDto();
    valoareDisponibila: number = 0;
    constructor(inject: Injector,
        private route: ActivatedRoute,
        private router: Router,
        private _paapRedistribuireService: PaapRedistribuireServiceProxy) {
        super(inject);
    }

    ngOnInit() {
        this.an = this.route.snapshot.queryParamMap.get('an');
        this.paapRedistribuireDto.paapRedistribuireDetaliiList = [];
        this.getPaapPrimesteList();
    }

    getPaapPrimesteList() {
        this._paapRedistribuireService.getPaapPrimList(this.an).subscribe(result => {
            this.paapPrimesteListDto = result;
        });
    }

    selectedPaap(paapPrimesteId: number) {
        this.paapPrimesteListDto.filter(f => f.paapPrimesteId == paapPrimesteId).map(x => {
            this.paapRedistribuireDto.categorieId = x.categorieId;
            this.paapRedistribuireDto.denumire = x.denumire;
            this.paapRedistribuireDto.sumaPrimita = x.sumaPrimita;
            this.paapRedistribuireDto.valoareInitiala = x.valoareInitiala;
            this.paapRedistribuireDto.paapPrimesteId = x.paapPrimesteId;
            this.getPaapPierdeList();
            this.getPaapRedistribDetalii(this.paapRedistribuireDto.paapPrimesteId);
        });
    }

    getPaapPierdeList() {
        this._paapRedistribuireService.getPaapPierdeList(this.an).subscribe(result => {
            this.paapPierdeListDto = result;
        });
    }

    selectedPaapPierde(paapPierdeId: number) {
        this.paapPierdeListDto.filter(f => f.paapPierdeId == paapPierdeId).map(x => {
            this.paapPierdeDto.paapPierdeId = x.paapPierdeId;
            this.paapPierdeDto.denumire = x.denumire;
            this.paapPierdeDto.categorieId = x.categorieId;
            this.paapPierdeDto.valoareDisponibila = x.valoareDisponibila;
            this.paapPierdeDto.valoareRedistribuita = x.valoareRedistribuita;
            this.paapPierdeDto.paapPierdeId = x.paapPierdeId;
            this.valoareDisponibila = x.valoareDisponibila;
        });
    }

    updateValoareDisponibila(event) {
        var sumaPierduta = event.target.value;
        if (sumaPierduta <= this.paapPierdeDto.valoareDisponibila) {
            this.paapPierdeDto.valoareDisponibila = this.valoareDisponibila - sumaPierduta;
        }
    }

    adaugaAchizitie() {
        if (this.paapPierdeDto.valoareRedistribuita > this.valoareDisponibila) {
            abp.notify.error("Suma redistribuita nu poate fi mai mare decat valoarea disponibila");
            this.paapPierdeDto.valoareRedistribuita = 0;

        } else {
            var redistribDetaliu = new PaapRedistribuireDetaliiDto();
            this.valoareDisponibila -= this.paapPierdeDto.valoareRedistribuita;
          //  this.paapPierdeDto.valoareRedistribuita = 0;
            redistribDetaliu.denumire = this.paapPierdeDto.denumire;
            redistribDetaliu.paapCarePierdeId = this.paapPierdeDto.paapPierdeId;
            redistribDetaliu.sumaPierduta = this.paapPierdeDto.valoareRedistribuita;

            this.paapRedistribuireDto.paapRedistribuireDetaliiList.push(redistribDetaliu);

            this.paapRedistribuireDto.sumaPrimita = this.paapRedistribuireDto.paapRedistribuireDetaliiList.reduce((sum, current) => sum + Number(current.sumaPierduta), 0);
        }
    }

    getPaapRedistribDetalii(paapPrimesteId: number) {
        this._paapRedistribuireService.getPaapRedistribDetalii(paapPrimesteId).subscribe(result => {
                this.paapRedistribuireDto.paapRedistribuireDetaliiList = result;
                this.paapRedistribuireDto.sumaPrimita = this.paapRedistribuireDto.paapRedistribuireDetaliiList/*.filter(item => item.paapRedistribuireId == paapRedistribId)*/
                    .reduce((sum, current) => sum + Number(current.sumaPierduta), 0);
            });
    }

    save() {
        this._paapRedistribuireService.save(this.paapRedistribuireDto).subscribe(() => {
            this.router.navigate(['/app/buget/paap/redistribuire/redistribuirePaapList'], { queryParams: { an: this.an } });
            abp.notify.info("Achizitia a fost inregistrata");
        });
    }
}