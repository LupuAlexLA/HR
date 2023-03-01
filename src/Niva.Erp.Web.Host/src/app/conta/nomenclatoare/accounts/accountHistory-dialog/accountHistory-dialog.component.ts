import { Component, Injector, OnInit } from "@angular/core";
import { BsModalRef } from "ngx-bootstrap/modal";
import { AppComponentBase } from "../../../../../shared/app-component-base";
import { AccountHistoryListDto, AccountServiceProxy } from "../../../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './accountHistory-dialog.component.html'
})
export class AccountHistoryDialogComponent extends AppComponentBase implements OnInit {

    accountHistoryList: AccountHistoryListDto[] = [];
    accountId: number;

    constructor(inject: Injector,
        private _accountService: AccountServiceProxy,
        public bsModalRef: BsModalRef) {
        super(inject);
    }

    ngOnInit() {
        this.getAccountHistoryList();
    }

    getAccountHistoryList() {
        this._accountService.accountHistoryList(this.accountId).subscribe(result => {
            this.accountHistoryList = result;
        });
    }
}