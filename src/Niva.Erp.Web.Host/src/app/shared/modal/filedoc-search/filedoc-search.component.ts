import { Component, EventEmitter, Injector, OnInit, Output } from '@angular/core';
import * as moment from 'moment';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { AtasamentDTO, FileDocExternServiceProxy } from '@shared/service-proxies/service-proxies';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';

@Component({
    //selector: 'app-filedoc-search',
    templateUrl: './filedoc-search.component.html',
    styleUrls: ['./filedoc-search.component.css']
})
/** filedoc-search component*/
export class FiledocSearchComponent extends PagedListingComponentBase<AtasamentDTO> implements OnInit {
    protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
        
    }
    protected delete(entity: AtasamentDTO): void {
         
    }

    id: number;

    @Output() onSave = new EventEmitter<any>();

    public documentArhive: AtasamentDTO[];

    constructor(
        injector: Injector,
        public _filedocService: FileDocExternServiceProxy,
        public bsModalRef: BsModalRef
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.search();
    }

    search(): void {
        let myMoment: moment.Moment = moment(moment.now());
        //this._filedocService.getFacturiData(myMoment).subscribe((result) => {
        //    this.documentArhive = result;
        //});
    }
    select(id): void {
        this.notify.info(this.l('Ati selectat id:' + id));
        this.onSave.emit(id);
        this.bsModalRef.hide();        
    }
}