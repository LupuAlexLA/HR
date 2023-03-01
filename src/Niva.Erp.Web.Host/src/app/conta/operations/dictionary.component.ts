import { Component, Injector, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { FODictionaryFormDto, ForeignOperationDictionaryServiceProxy } from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './dictionary.component.html',
    animations: [appModuleAnimation()]
})
export class DictionaryComponent extends AppComponentBase implements OnInit {

    dictionaryList: FODictionaryFormDto = new FODictionaryFormDto();

    constructor(inject: Injector,
        private router: Router,
        private _foDictionaryService: ForeignOperationDictionaryServiceProxy) {
        super(inject);
    }

    ngOnInit() {
        this.getDictionaryList();
    }

    getDictionaryList() {
        this._foDictionaryService.fODictionaryListInit().subscribe(result => {
            this.dictionaryList = result;
        });
    }

    search() {
        this._foDictionaryService.fODictionarySearch(this.dictionaryList.searchAccount).subscribe(result => {
            this.dictionaryList = result;
        });
    }

    dictionaryListCount() {
        if (this.dictionaryList.fOdictionaryList == null) {
            return 0;
        } else {
            return this.dictionaryList.fOdictionaryList.length;
        }
    }

    delete(dictionaryId: number) {
        abp.message.confirm("Expresia va fi stearsa. Sigur?", null,
            (isConfirmed) => {
                if (isConfirmed) {
                    this._foDictionaryService.deleteFODictionary(dictionaryId).subscribe(() => {
                        this.getDictionaryList();
                        abp.notify.success("OkDelete");
                    });
                }
            })
    }

    backToList() {
        this.router.navigate(['/app/conta/operations/foreignOperation']);
    }
}