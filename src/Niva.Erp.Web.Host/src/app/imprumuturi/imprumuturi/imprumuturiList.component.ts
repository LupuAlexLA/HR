import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { DocumentTypeListDDDto, DocumentTypeListDto, DocumentTypeServiceProxy, GetDocumentTypeOutput, ImprumutDto, ImprumutEditDto, ImprumutServiceProxy, ImprumuturiTipuriDto, ImprumuturiTipuriServiceProxy } from '../../../shared/service-proxies/service-proxies';
import { FormControl } from '@angular/forms';
import { Observable, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, filter, map, startWith, switchMap } from 'rxjs/operators';


@Component({
    
    templateUrl: './imprumuturiList.component.html',
    animations: [appModuleAnimation()]
})
/** imprumuturiList component*/
export class ImprumuturiListComponent extends AppComponentBase implements OnInit {
    /** imprumuturiList ctor */
    imprumut: ImprumutDto[] = [];
    documentTypes: GetDocumentTypeOutput;
    imprumutTipuri: ImprumuturiTipuriDto[] = [];
    imprumutId: number;
    myControl = new FormControl('');
    myControl2 = new FormControl('');
    myControlStartDate = new FormControl();
    myControlEndDate = new FormControl(Infinity);
    filteredOptionsTipuri: Observable<ImprumuturiTipuriDto[]>;
    filteredOptionsDocumentNr: Observable<ImprumutDto[]>;
    displayedOptions: ImprumutDto[] = [];


    constructor(injector: Injector,
        private _imprumutService: ImprumutServiceProxy,
        private _documentService: DocumentTypeServiceProxy,
        private _tipuriService: ImprumuturiTipuriServiceProxy,
    ) {
        super(injector);
        
    }

    ngOnInit() {


        this.getImprumutList();
        

        this.filteredOptionsTipuri = this.myControl.valueChanges.pipe(
            startWith(''),
            debounceTime(400),
            distinctUntilChanged(),
            switchMap(value => this._filterTipuri(value)),

        );

        this.filteredOptionsDocumentNr = this.myControl2.valueChanges.pipe(
            startWith(''),
            debounceTime(400),
            distinctUntilChanged(),
            switchMap(value => this._filterDocumentNr(value)),

        );


        this.myControl.valueChanges.subscribe(() => this.updateList());
        this.myControl2.valueChanges.subscribe(() => this.updateList());
        this.myControlStartDate.valueChanges.subscribe(() => this.updateList());
        this.myControlEndDate.valueChanges.subscribe(() => this.updateList());

    }

    private _filterTipuri(value: string) {
        console.log("filter call")
        const filterValue = value.toLowerCase();


        // varianta prin care fac call la api pentru fiecare modificare a inputului
        return this._tipuriService.imprumuturiTipuriList().pipe(
            filter(data => !!data),
            map((data) => {
                return data.filter(option => option.description.toLowerCase().includes(filterValue))
            })
        )
        //  return of(this.imprumutTipuri.filter(option => option.description.toLowerCase().includes(filterValue) || option.account.toLowerCase().includes(filterValue)))
    }

    private _filterDocumentNr(value: string) {
        console.log("filter call")
        const filterValue = value.toString().toLowerCase();


        // varianta prin care fac call la api pentru fiecare modificare a inputului
        return this._imprumutService.imprumutList().pipe(
            filter(data => !!data),
            map((data) => {
                return data.filter(option => option.documentNr.toString().toLowerCase().includes(filterValue))
            })
        )
        //  return of(this.imprumutTipuri.filter(option => option.description.toLowerCase().includes(filterValue) || option.account.toLowerCase().includes(filterValue)))
    }



    updateList() {
        console.log("updatelist call")
        this.displayedOptions = this.imprumut.filter(option => option.imprumuturiTipuri.toLowerCase().includes(this.myControl.value.toLowerCase())
            && ((option.documentNr.toString().toLowerCase() == this.myControl2.value.toString().toLowerCase()) || this.myControl2.value.toString().toLowerCase() == '')
                                                            && (option.startDate >= this.myControlStartDate.value && option.endDate <= this.myControlEndDate.value));
    }

    updateRobor() {
        this._imprumutService.updateRateVariabile().subscribe();
        abp.notify.success(this.l('Imprumuturi Actualizate'));
    }


    //updateList() {
    //    this.displayedOptions = this.imprumutTipuri;
    //    this._tipuriService.imprumuturiTipuriList().subscribe(result => {
    //        this.imprumutTipuri = result.filter(option => option.description.toLowerCase().includes(this.myControl.value) && option.account.toLowerCase().includes(this.myControl2.value));
    //    });
    //}




    delete(TipId: number) {

        abp.message.confirm('Esti sigur?',
            undefined,
            (result: boolean) => {
                if (result) {
                    
                    this._imprumutService.deleteImprumut(TipId).subscribe(() => {
                        this.myControl.setValue('');
                        this._imprumutService.imprumutList().subscribe(result => {
                            this.imprumut = result;
                            this.displayedOptions = result;
                            //     this.updateList(); De rezolvat problema filtrare linie de credit care nu are data de sfarsit
                        });

                        abp.notify.success(this.l('DeleteMessage'));
                    });
                }
            });

        
    }


    

    getImprumutList() {
        this._imprumutService.imprumutList().subscribe(result => {
            this.imprumut = result;
            this.displayedOptions = result;
        });
    }

    

    getImprumuturiCount() {
        if (this.imprumut.length > 0) {
            return this.imprumut.length;
        } else {
            return 0;
        }
    }

}