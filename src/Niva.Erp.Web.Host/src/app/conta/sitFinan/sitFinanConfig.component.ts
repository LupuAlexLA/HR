import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { SitFinanConfigServiceProxy, SitFinanDDDto, SitFinanForm } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './sitFinanConfig.component.html',
    animations: [appModuleAnimation()]
})
export class SitFinanConfigComponent extends AppComponentBase implements OnInit {

    form: SitFinanForm = new SitFinanForm();
    sitFinanPrec: SitFinanDDDto[] = [];

    constructor(inject: Injector,
        private _sitFinanConfigService: SitFinanConfigServiceProxy,
        private router: Router) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Conta.SitFinan.Config.Acces')) {
            this._sitFinanConfigService.sitFinanInitForm().subscribe(result => {
                this.form = result;
            });
        } else {
            this.router.navigate(['app/home']);
        }
    }

    sitFinanEdit(sitFinanId: number) {
        this._sitFinanConfigService.sitFinanEdit(sitFinanId, this.form).subscribe(result => {
            this.form = result;
            this.sitFinanListDD();
        });
    }

    sitFinanListDD() {
        this._sitFinanConfigService.sitFinanListDD().subscribe(result => {
            this.sitFinanPrec = result;
        });
    }

    sitFinanEditBack() {
        this.form.showEdit = false;
        this.form.showList = true;
    }

    sitFinanSave() {
        this._sitFinanConfigService.sitFinanSave(this.form).subscribe(result => {
            this.form = result;
            abp.notify.info('AddUpdateMessage');
        });
    }

    sitFinanDelete(sitFinanId: number) {
        abp.message.confirm(
            'Raportarea va fi stearsa. Sigur?',
            null,
            (isConfirmed: boolean) => {
                if (isConfirmed) {
                    this._sitFinanConfigService.sitFinanDelete(sitFinanId, this.form).subscribe(result => {
                        this.form = result;
                        abp.notify.info('DeleteMessage');
                    });
                }
            }
        );
    }
}