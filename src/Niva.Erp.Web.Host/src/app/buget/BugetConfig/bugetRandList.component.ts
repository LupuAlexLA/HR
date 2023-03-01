import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { delay, finalize } from 'rxjs/operators';
import { AppComponentBase } from '../../../shared/app-component-base';
import { BugetConfigServiceProxy, BugetForm, BugetFormRandDetailDto, BugetFormRandDto, EnumServiceProxy, EnumTypeDto, InvoiceElementsDetailsCategoryListDTO, InvoiceElementsDetailsDTO, InvoiceServiceProxy } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './bugetRandList.component.html'
})
/** bugetRandList component*/
export class BugetRandListComponent extends AppComponentBase implements OnInit {
    /** bugetRandList ctor */

    categoryId: number;
    formularId: number;
    form: BugetForm = new BugetForm();
    elementDetailsCategories: InvoiceElementsDetailsCategoryListDTO[] = [];
    invoiceElementsDetails: InvoiceElementsDetailsDTO[][][] = [];
    rowDeleteList: BugetFormRandDto[] = [];
    rowTypes: EnumTypeDto[] = [];
    rowTypeIncome: EnumTypeDto[] = [];
    rowTypeSalarizare: EnumTypeDto[] = [];
    isLoading: boolean;
    balantaParametersShow: boolean = false;
    showDetail: boolean = false;

    constructor(inject: Injector,
        private route: ActivatedRoute,
        private _invoiceService: InvoiceServiceProxy,
        private _BugetConfigService: BugetConfigServiceProxy,
        private _EnumAppService: EnumServiceProxy,
        private router: Router    ) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Buget.BVC.Configurare')) {
            this.formularId = +this.route.snapshot.queryParamMap.get('formularId');
            this.getInvoiceElementDetailCategories();
            this.isLoading = true;
            this._BugetConfigService.bugetRandInit(this.formularId).pipe(
                delay(1000),
                finalize(() => {
                    this.isLoading = false
                })
            ).subscribe(result => {
                this.form = result;
                this.detailList();
                this.populateCategories(this.form);
            })

            this._EnumAppService.bVC_RowTypeList2().subscribe(result => {
                this.rowTypes = result;
            });
            this._EnumAppService.bVC_RowTypeIncome().subscribe(result => {
                this.rowTypeIncome = result;
            });
            this._EnumAppService.bVC_RowTypeSalarizare().subscribe(result => {
                this.rowTypeSalarizare = result;
            });
        } else {
            this.router.navigate(['app/home']);
        }
    }

    populateCategories(form: BugetForm) {
        //   form.randList.forEach(e => e.detaliiRand.forEach(f => f.categoryId = this.getCategoryIdByInvoiceElementDetailId(f.tipRandCheltuialaId)));
        for (let i = 0; i < form.randList.length; i++) {
            for (let j = 0; j < form.randList[i].detaliiRand.length; j++) {
                if (this.form.randList[i].detaliiRand[j].tipRandCheltuialaId) {
                    this._invoiceService.getCategoryIdByInvoiceElementDetailId(this.form.randList[i].detaliiRand[j].tipRandCheltuialaId).subscribe(result => {
                        this.form.randList[i].detaliiRand[j].categoryId = result;
                        this.getInvoiceElementDetailsByCategoryId(result, i, j);
                    });
                }
            }
        }
    }

    detailList() {
        this.invoiceElementsDetails = [];
        for (let i = 0; i < this.form.randList.length; i++) {
            this.invoiceElementsDetails.push([]);
            for (let j = 0; j < this.form.randList[i].detaliiRand.length; j++) {
                this.invoiceElementsDetails[i].push([]);
            }
        }
    }

    getCategoryIdByInvoiceElementDetailId(id: number): number {      
        this.isLoading = true;
        if (id != null) {
            this._invoiceService.getCategoryIdByInvoiceElementDetailId(id).pipe(
                delay(1000),
                finalize(() => {
                    this.isLoading = false
                })
            ).subscribe(result => {                
                return result;
            });
        }
        return null;
    }

    showParamsBalanta() {
        console.log(this.balantaParametersShow);
    }

    getInvoiceElementDetailsByCategoryId(categoryId: number, index: number, inside: number) {
        this.isLoading = true;
        if (categoryId != null) {
            this._invoiceService.getInvoiceElementsDetailsByCategoryId(categoryId).pipe(
                delay(1000),
                finalize(() => {
                    this.isLoading = false
                })
            ).subscribe(result => {
                this.invoiceElementsDetails[index][inside] = result;
               
            });
        }
    }

    getInvoiceElementDetailsByCategoryIdOnChange(categoryId: number, index: number, inside: number) {
         this.form.randList[index].detaliiRand[inside].tipRandCheltuialaId = null;
        if (categoryId != null) {
            this._invoiceService.getInvoiceElementsDetailsByCategoryId(categoryId).subscribe(result => {
                this.invoiceElementsDetails[index][inside] = result;
                console.log(result);
            });
        }
    }

    getDetailsByCategoryId(categoryId: number) {
        if (categoryId != null) {
            this._invoiceService.getInvoiceElementsDetailsByCategoryId(categoryId).subscribe(result => {
                return result;
            });
        }
    }

    getInvoiceElementDetailCategories() {
        this._invoiceService.getInvoiceElementsDetailsCategories().subscribe(result => {
            this.elementDetailsCategories = result;
        });
    }

    budgetOrder(i) {
       // this.form.randList.forEach(e => e.insert = false); // In situatia in care au mai fost inserate elemente
        this.form.randList[i].insert = true;
        
        this._BugetConfigService.bugetOrderRow(this.form).subscribe(result => {        
            this.form = result;
            this.getInvoiceElementDetailCategories();     
            this.detailList();
            this.populateCategories(result);
        });
    }

    bugetDelRow(index: number) {
        this.form.randList[index].delete = true;
        //this.rowDeleteList.push(this.form.randList[index]);
        //this.form.randList.splice(index, 1);      
    }

    rowDeleteCheltuiala(i, j) {
        this.form.randList[i].detaliiRand[j].delete = true;
    }

    bugetAddRow() {
        this._BugetConfigService.bugetAddRow(this.form).subscribe(result => {
            this.form = result;
            this.invoiceElementsDetails.push([]);
            this.invoiceElementsDetails[this.invoiceElementsDetails.length - 1].push([]);
            this.populateCategories(this.form);
        });
    }

    rowAddCheltuiala(index) {
        const det = new BugetFormRandDetailDto();
        det.delete = false;
        det.tipRandCheltuialaId = null;
        det.categoryId = null;
        this.form.randList[index].detaliiRand.push(det)
        //this.detailList();
        //this.populateCategories(this.form);
    }

    bugetSave() {
        //let form = new BugetForm(this.form);
        //this.rowDeleteList.forEach(f => form.randList.push(f))
        this.isLoading = true;
        this._BugetConfigService.bugetRandSave2(this.form).pipe(
            delay(1000),
            finalize(() => {
                this.isLoading = false
            })
        ).subscribe(result => {
            this.form = result;
            this.detailList();
            this.populateCategories(this.form);
            abp.notify.info('AddUpdateMessage');
        });
    }

    showDetails() {
        this.showDetail = true;
    }

    hideDetails() {
        this.showDetail = false;
    }
}