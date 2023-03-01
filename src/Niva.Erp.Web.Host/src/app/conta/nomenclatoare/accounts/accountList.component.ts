import { Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '../../../../shared/animations/routerTransition';
import { AppComponentBase } from '@shared/app-component-base';
import { AccountListFormDto, AccountServiceProxy } from '../../../../shared/service-proxies/service-proxies';
import { ActivatedRoute, Router } from '@angular/router';
import { delay, finalize } from 'rxjs/operators';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { AccountHistoryDialogComponent } from './accountHistory-dialog/accountHistory-dialog.component';

@Component({
    templateUrl: './accountList.component.html',
    animations: [appModuleAnimation()]
})
export class AccountListComponent extends AppComponentBase implements OnInit {

    accountList: AccountListFormDto = new AccountListFormDto();
    searchAccount: string;
    isLoading: boolean = false;

    constructor(injector: Injector,
        private _accountListService: AccountServiceProxy,
        private route: ActivatedRoute,
        private _modalService: BsModalService,
        private router: Router) {
        super(injector);
        this.editAccount = this.editAccount.bind(this);
        this.deleteAccount = this.deleteAccount.bind(this);
        this.showHistory = this.showHistory.bind(this);
        this.isVisible = this.isVisible.bind(this);
    }

    ngOnInit(): void {
        if (this.isGranted('Admin.Conta.PlanConturi.Acces')) {
            // this.searchAccount = this.route.snapshot.queryParamMap.get('accountSearch');
            if (sessionStorage.getItem('searchAccount')) {
                this.accountList.searchAccount = sessionStorage.getItem('searchAccount');
                this.searchAccount = this.accountList.searchAccount;
            }
            else {
                this.accountList.searchAccount = "";
                this.searchAccount = "";
            }

            //this.accountList.searchAccount = this.searchAccount;

            //this.getAccountList();
            this.search();
        }
        else {
            this.router.navigate(['/app/home']);
        }
    }

    onCellPrepared(e) {
        if (e.rowType == "header") {
            e.cellElement.style.fontWeight = 'bold';
            e.cellElement.style.color = 'white';
            e.cellElement.style.backgroundColor = "#17a2b8";
        }
    }

    search() {
        this.isLoading = true;
        sessionStorage.setItem('searchAccount', this.accountList.searchAccount);
        this._accountListService.accountList(this.accountList).pipe(
            delay(1000),
            finalize(() => {
                this.isLoading = false;
            })
        ).subscribe(result => {
            this.accountList = result;

        });

    }

    accountListCount() {
        if (this.accountList.accounts == null) {
            return 0;
        } else {
            return this.accountList.accounts.length;
        }
    }

    getAccountList() {
        this._accountListService.accountListInit().subscribe((result: AccountListFormDto) => {
            this.accountList = result;

            if (this.searchAccount != null && this.searchAccount != "") {
                this.accountList.searchAccount = this.searchAccount;
                this.search();
            }
        });
    }

    editAccount(e) {
        this.router.navigate(['/app/conta/nomenclatoare/accounts/accountEdit'], { queryParams: { accountSearch: this.accountList.searchAccount, account: e.row.key.id } });
    }

    deleteAccount(e) {
        abp.message.confirm(this.l("DeleteAccountMessage", e.row.key.id),
            undefined,
            (result: boolean) => {
                if (result) {
                    this._accountListService.deleteAccount(e.row.key.id).subscribe(() => {
                        abp.notify.success(this.l('SuccessfullyDeleted'));
                        this.getAccountList();
                    });
                }
            }
        );
    }

    showHistory(e) {
        let AccountHistoryDialog: BsModalRef;

        AccountHistoryDialog = this._modalService.show(
            AccountHistoryDialogComponent,
            {
                class: 'modal-lg',
                initialState: {
                    accountId: e.row.key.id
                }
            }
        );
    }

    isVisible(e) {
        if (this.isGranted('Admin.Conta.PlanConturi.Modificare')) {
            return true;
        }
        else {
            return false;
        }
    }

}