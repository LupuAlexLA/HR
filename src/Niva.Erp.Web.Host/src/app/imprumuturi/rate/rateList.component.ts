import { Component, Injector, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { result } from 'lodash';
import * as moment from 'moment-timezone';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { filter, map } from 'rxjs/operators';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { ImprumutDto, ImprumutServiceProxy, RataServiceProxy, RataEditDto, RataDto, TragereServiceProxy, Tragere, TragereDto, DateDobanziReferintaServiceProxy, DateDobanziReferintaDto, EnumServiceProxy, EnumTypeDto, ComisioaneServiceProxy, ComisionDto, DataComisionEditDto, TipCreditare, TipTragere } from '../../../shared/service-proxies/service-proxies';
import { RateListDialogComponent } from './rateList-dialog.component';

@Component({
    
    templateUrl: './rateList.component.html',
    animations: [appModuleAnimation()]

})
/** RateList component*/
export class RateListComponent extends AppComponentBase implements OnInit {
    /** RateList ctor */
    imprumut: ImprumutDto = new ImprumutDto();
    form: FormControl = new FormControl('');
    rate: RataDto[] = [];
    trageri: TragereDto[] = [];
    tragere: TragereDto = new TragereDto() ;
    rata: RataEditDto = new RataEditDto();
    rateOrientative: RataDto[] = [];
    //_rata: RataDto = new RataDto();
    ultimaRataValida: RataDto = new RataDto();
    imprumutId: number;
    numarRate: number;
    test: boolean = true;
    dobanzi: DateDobanziReferintaDto[] = [];
    dobandaValabila: boolean = true;
    tipTragereList: EnumTypeDto[] = [];
   
    constructor(injector: Injector,
        private route: ActivatedRoute,
        private router: Router,
        private _imprumutService: ImprumutServiceProxy,
        private _rataService: RataServiceProxy,
        private _tragereService: TragereServiceProxy,
        private _dateDobandaService: DateDobanziReferintaServiceProxy,
        private _comisionService: ComisioaneServiceProxy,
        private _enumService: EnumServiceProxy,
        private _modalService: BsModalService,    ) {
        super(injector);
        
    }

    ngOnInit(): void {
        
        this.imprumutId = + this.route.snapshot.queryParamMap.get('imprumutId');
    //    this.getImprumutId(this.imprumutId);
        this.getImprumutId(this.imprumutId);
        this.getEnums();
        
        
    }

   

    getEnums() {
        this._enumService.tipTragere().subscribe(result =>
            this.tipTragereList = result
            
            );
    }

    getRateAndImprumutListId(imprumutId: number) {
        this._rataService.rataListId(imprumutId).subscribe(result => {
            this.rate = result;
            this.numarRate = this.getRateCount();
            this.ultimaRataValida = this.rate[this.rate.length - 1];
            this._imprumutService.imprumutId(imprumutId).subscribe(result => {
                this.imprumut = result
                
            });
            
        });
        
    }

    getTragereListId(imprumutId: number) {
        this._tragereService.tragereListId(imprumutId).subscribe(result => {
            this.trageri = result;
            (this.trageri as any).forEach(e => e.isValid = true);
            console.log(this.trageri);
            let filter = this.trageri.filter(e => e.tipTragere == 3 || e.tipTragere == 1 || e.tipTragere == 0);
            (this.trageri as any)[this.trageri.length - 1].isValid = false;
            this.tragere.sumaDisponibila = filter[filter.length - 1].sumaDisponibila;
            this.tragere.sumaImprumutata = filter[filter.length - 1].sumaImprumutata;
        });
    }

    calculateDobanda() {
        if (this.imprumut.dobanziReferintaId) {
            this._dateDobandaService.dateDobanziReferintaListId(this.imprumut.dobanziReferintaId).subscribe(result => { this.dobanzi = result 
            let days = moment.duration(this.tragere.dataTragere.diff(this.trageri[this.trageri.length - 1].dataTragere)).asDays();
            let valabil = this.dobanzi.filter(e => e.data <= this.tragere.dataTragere);
                console.log(valabil);

            if (valabil.length) {
                this.tragere.dobanda = this.trageri[this.trageri.length - 1].sumaTrasa * (this.imprumut.marjaFixa + valabil[valabil.length - 1].valoare) / 100 * days / 365;
                this.tragere.dobanda = Number(this.tragere.dobanda.toFixed(2));
            }
            else {
                abp.notify.info(this.l('DOBANZI DE REFERINTA NEACTUALIZATE!'));
                }
            });
        }
        else {
            
            let days = moment.duration(this.tragere.dataTragere.diff(this.trageri[this.trageri.length - 1].dataTragere)).asDays();
            this.tragere.dobanda = this.trageri[this.trageri.length - 1].sumaTrasa * this.imprumut.procentDobanda / 100 * days / 365;
        }
    }

    onChangeType() {
        this.tragere.sumaTrasa = 0;
        let filter = this.trageri.filter(e => e.tipTragere == 3 || e.tipTragere == 1 || e.tipTragere == 0);
        this.tragere.sumaDisponibila = filter[filter.length - 1].sumaDisponibila;
        this.tragere.sumaImprumutata = filter[filter.length - 1].sumaImprumutata;
        this.tragere.comision = 0;
        this.tragere.dobanda = 0;
    }

    calculateDobanda2() {
        this.calculateComision();
        if (this.tragere.tipTragere == 2) {
            if (this.imprumut.dobanziReferintaId) {
                this._dateDobandaService.dateDobanziReferintaListId(this.imprumut.dobanziReferintaId).subscribe(result => {
                    this.dobanzi = result
                    let days = this.tragere.dataTragere.daysInMonth();
                    //   let days = moment.duration(this.tragere.dataTragere.diff(this.trageri[this.trageri.length - 1].dataTragere)).asDays();
                    let valabil = this.dobanzi.filter(e => e.data <= this.tragere.dataTragere);
                    let filter = this.trageri.filter(e => e.tipTragere == 1 || e.tipTragere == 0);

                    if (valabil.length) {
                        this.tragere.dobanda = filter[filter.length - 1].sumaImprumutata * (this.imprumut.marjaFixa + valabil[valabil.length - 1].valoare) / 100 * days / 365;
                        this.tragere.dobanda = Number(this.tragere.dobanda.toFixed(2));
                    }
                    else {
                        abp.notify.info(this.l('DOBANZI DE REFERINTA NEACTUALIZATE!'));
                    }
                });
            }
            else {
                let days = this.tragere.dataTragere.daysInMonth();
                //  let days = moment.duration(this.tragere.dataTragere.diff(this.trageri[this.trageri.length - 1].dataTragere)).asDays();
                let filter = this.trageri.filter(e => e.tipTragere == 1 || e.tipTragere == 0);
                this.tragere.dobanda = filter[filter.length - 1].sumaImprumutata * this.imprumut.procentDobanda / 100 * days / 365;
            }
        }
        
    }

    calculateComision() {
        if (this.tragere.tipTragere == 2) {
            this._comisionService.comisionEditNeutilizatId(this.imprumut.id).subscribe(result => {
                
                let comNeut = result;
                
                if (comNeut.id) {
                    this.tragere.comision = this.tragere.sumaDisponibila * comNeut.valoareComision / 100 * this.tragere.dataTragere.daysInMonth() / comNeut.bazaDeCalcul;
                    this.tragere.comision = Number(this.tragere.comision.toFixed(2));

                    let dateComision = new DataComisionEditDto();
                    dateComision.imprumutId = this.tragere.imprumutId;
                    dateComision.comisionId = comNeut.id;
                    dateComision.dataPlataComision = this.tragere.dataTragere;
                    dateComision.isValid = false;
                    dateComision.tipValoareComision = comNeut.tipValoareComision;
                    dateComision.valoareComision = comNeut.valoareComision;
                    dateComision.sumaComision = this.tragere.comision;
                    dateComision.tipSumaComision = comNeut.tipSumaComision;
                        
                    this._comisionService.saveDataComisionNeutilizare(dateComision).subscribe(() => { });
                }
                else {
                    abp.notify.info(this.l('VALOAREA COMISIONELOR NU ESTE DEFINITA!'));
                }
            });
            }

            

        }
    

    getRateListId(imprumutId: number) {
        this._rataService.rataListId(imprumutId).subscribe(result => {
            this.rate = result;
            this.numarRate = this.getRateCount();
            this.ultimaRataValida = this.rate[this.rate.length - 1];
        });

    }

    getImprumutId(imprumutId: number) {

        this._imprumutService.getImprumutId(imprumutId).subscribe(result => {
            
        //    this.rateOrientative = this.generateRateOrientative();
            if (result.tipCreditare == TipCreditare._0) {
                this.getRateListId(this.imprumutId);
            }
            else {
                this.getTragereListId(this.imprumutId);
            }
            

        });

        this._imprumutService.imprumutId(imprumutId).subscribe(result => {

            this.imprumut = result;

        });
    }

    getRateCount() {
        if (this.rate.length > 0) {
            return this.rate.length;
        } else {
            return 0;
        }
    }

    sumaTrasaOnChange() {
        let filter = this.trageri.filter(e => e.tipTragere == 1 || e.tipTragere == 0 || e.tipTragere == 3);
        if (this.tragere.tipTragere == 1) {
            this.tragere.sumaImprumutata = +filter[filter.length - 1].sumaImprumutata + +this.tragere.sumaTrasa;
        }
        else if (this.tragere.tipTragere == 3) {
            this.tragere.sumaImprumutata = +filter[filter.length - 1].sumaImprumutata - +this.tragere.sumaTrasa;
        }
        
        this.tragere.sumaDisponibila = this.imprumut.suma - this.tragere.sumaImprumutata;
    }

    sumaTrasaOnChangeElement() {
        
        
    }

    editareTragere() {
        this._tragereService.saveTragere(this.trageri[this.trageri.length - 1]).subscribe(() => { window.location.reload(); });
    }

    getTrageriCount() {
        if (this.trageri.length > 0) {
            return this.trageri.length;
        } else {
            return 0;
        }
    }

    regenerare(_rata: RataEditDto) {
        _rata.imprumutId = this.imprumutId;

        abp.message.confirm('Ratele vor fi regenerate. Esti sigur?',
            undefined,
            (result: boolean) => {
                if (result) {
                    console.log("validat");
                    this._rataService.regenerareScadentarDeLaRandCurent(_rata).subscribe(() => {
                        abp.notify.info(this.l('UpdateMessage'));
                        this.getRateListId(this.imprumutId);
                    })
                }
            });
    }

    validateRata(_rata: RataEditDto) {





        abp.message.confirm('Ratele validate nu sunt editabile. Esti sigur?',
            undefined,
            (result: boolean) => {
                if (result) {
                    console.log("validat");
                    _rata.isValid = true
                    _rata.imprumutId = this.imprumutId;
                    this._rataService.saveRata(_rata).subscribe(() => {
                        abp.notify.info(this.l('UpdateMessage'));
                        this.getRateListId(this.imprumutId);
                        this.router.navigate(['/app/rate/imprumuturiRateList'], { queryParams: { imprumutId: this.imprumutId } });
                    });
                }
            });
    }

    saveRata(_rata: RataEditDto) {
        //this.rata = Object.assign(this.rata,_rata);
        
        _rata.imprumutId = this.imprumutId;
        

        //this.rata.isValid = true;


        if ((Number(_rata.sumaPrincipal) + Number(_rata.sumaDobanda)).toFixed(2) == Number(_rata.sumaPlatita).toFixed(2)) {
            

            abp.message.confirm('Esti sigur?',
                undefined,
                (result: boolean) => {
                    if (result) {
                        console.log("validat");
                        console.log(_rata);
                        this._rataService.saveRata(_rata).subscribe(() => {
                            abp.notify.info(this.l('UpdateMessage'));
                            this.getRateListId(this.imprumutId);
                         //   this.router.navigate(['/app/rate/imprumuturiRateList'], { queryParams: { imprumutId: this.imprumutId } });
                        });
                    }
                });
        }
        else {
            abp.message.error('Suma Principal + Suma Dobanda nu este egal cu Suma platita (Rata)');
        }
    }

    saveTragere(tragere : TragereDto) {
        this.tragere.imprumutId = this.imprumutId;
        this.tragere.currencyId = this.trageri[this.trageri.length - 1].currencyId;
        console.log(this.tragere);
        this._tragereService.saveTragere(this.tragere).subscribe(() => { window.location.reload();});
            
    }

    deleteTragere() {
        this._tragereService.deleteTragere(this.trageri[this.trageri.length - 1].id).subscribe(() => { window.location.reload(); });
    }


    daysInMonth(month, year) {
    return new Date(year, month, 0).getDate();
    }

    delete(rataId: number) {
        this._rataService.deleteRata(rataId).subscribe(() => {

            this.getRateListId(this.imprumutId);

            abp.notify.success(this.l('DeleteMessage'));
        })
    }

    simulateDobandaAndComision(Date) {
        if (this.tragere.tipTragere == TipTragere._2) {
            this._tragereService.simulateCalculatorDobandaSiComision(Date, this.imprumutId).subscribe(result => {
                this.tragere.dobanda = result.dobanda;
                this.tragere.comision = result.comisionSum;
                this.tragere.comisionSum = result.comisionSum;
                this.tragere.comisions = result.comisions;
            });
        }
        
    }

    comisionDialog(tragere) {
        let comisionDialog: BsModalRef;

        comisionDialog = this._modalService.show(
            RateListDialogComponent,
            {
                class: 'modal-lg',
                initialState: {
                    comisionList: tragere.comisions
                },
            }
        );

        comisionDialog.content.onSave.subscribe(() => {
            
        });
    }



}