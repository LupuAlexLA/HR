import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { delay, finalize } from 'rxjs/operators';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../shared/app-component-base';
import { AutoOperationConfigDto, AutoOperationConfigServiceProxy, DocumentTypeListDto, DocumentTypeServiceProxy, EnumServiceProxy, EnumTypeDto } from '../../../shared/service-proxies/service-proxies';

@Component({
    templateUrl: './autoOperationConfig.component.html',
    animations: [appModuleAnimation()]
})
export class AutoOperationConfigComponent extends AppComponentBase implements OnInit {

    operation: AutoOperationConfigDto = new AutoOperationConfigDto();
    autoOperType: EnumTypeDto[] = [];
    operType = [];
    element: EnumTypeDto[] = [];
    valuesSign: EnumTypeDto[] = [];
    documentTypeList: DocumentTypeListDto[] = [];
    isLoading: boolean = false;

    constructor(inject: Injector,
        private _autoOperationConfigService: AutoOperationConfigServiceProxy,
        private _documentTypeService: DocumentTypeServiceProxy,
        private _enumService: EnumServiceProxy,
        private router: Router ) {
        super(inject);
    }

    ngOnInit() {
        if (this.isGranted('Admin.Conta.Monografii.Acces')) {
            this._autoOperationConfigService.initForm().subscribe(result => {
                this.operation = result;
                this.autoOperTypeSearch();
                this.autoOperTypeChange(this.operation.autoOperType);
                this.valueSignList();
                this.getDocumentTypeList();
            });
        }
        else {
            this.router.navigate(['/app/home']);
        }
    }

    autoOperTypeSearch() {
        this._enumService.autoOperationTypeListConfig().subscribe(result => {
            this.autoOperType = result;
        });
    }

    autoOperTypeChange(autoOperType: number) {
        this.operation.operationType = 0;
        if (autoOperType == 0) { // Mijloace fixe
            this.imoAssetOperTypeList();
            this.operation.showUnreceiveInvoice = false;
            this.operation.showExternalIssuer = false;
        } else if (autoOperType == 1) { // Obiecte de inventar
            this.invOperationTypeList();
            this.operation.showUnreceiveInvoice = false;
            this.operation.showExternalIssuer = false;
        } else if (autoOperType == 4) { // impozit pe profit
            this.taxProfitOperationTypeList();
            this.operation.showUnreceiveInvoice = false;
            this.operation.showExternalIssuer = false;
        } else if (autoOperType == 5) { // TitluriDePlasamentAFS
            this.titluriDePlasamentOperTypeList();
            this.operation.showUnreceiveInvoice = false;
            this.operation.showExternalIssuer = false;
        } else if (autoOperType == 6) { // ObligatiuniAFS
            this.titluriDePlasamentOperTypeList();
            this.operation.showUnreceiveInvoice = false;
            this.operation.showExternalIssuer = false;
        } else if (autoOperType == 7) { // TitluriDePlasamentHTML
            this.titluriDePlasamentOperTypeList();
            this.operation.showUnreceiveInvoice = false;
            this.operation.showExternalIssuer = false;
        } else if (autoOperType == 8) { // ObligatiuniHTML
            this.titluriDePlasamentOperTypeList();
            this.operation.showUnreceiveInvoice = false;
            this.operation.showExternalIssuer = false;
        } else if (autoOperType == 9) { // DepoziteBancare
            this.depoziteBancareOperTypeList();
            this.operation.showUnreceiveInvoice = false;
            this.operation.showExternalIssuer = false;
        } else if (autoOperType == 10) { // RepoDepoDateInPensiuneLivrata
            this.repoDepoDateInPensiuneLivrataOperTypeList();
            this.operation.showUnreceiveInvoice = false;
            this.operation.showExternalIssuer = false;
        } else if (autoOperType == 11) { // RepoDepoPrimiteInPensiuneLivrata
            this.repoDepoPrimiteInPensiuneLivrataOperTypeList();
            this.operation.showUnreceiveInvoice = false;
            this.operation.showExternalIssuer = false;
        } else if (autoOperType == 12) { // CertificateDepozit
            this.certificateDepozitOperTypeList();
            this.operation.showUnreceiveInvoice = false;
            this.operation.showExternalIssuer = false;
        } else if (autoOperType == 13) { // Contributii
            this.contributiiOperTypeList();
            this.operation.showUnreceiveInvoice = false;
            this.operation.showExternalIssuer = false;
        } else if (autoOperType == 14) { // AjustariDeprecierePlasamente
            this.ajustariDeprecierePlasamenteOperTypeList();
            this.operation.showUnreceiveInvoice = false;
            this.operation.showExternalIssuer = false;
        } else if (autoOperType == 15) { // reclasificari
            this.reclasificariOperTypeList();
            this.operation.showUnreceiveInvoice = false;
            this.operation.showExternalIssuer = false;
        } else if (autoOperType == 16) { // imprumuturi
            this.imprumuturiOperTypeList();
            this.operation.showUnreceiveInvoice = false;
            this.operation.showExternalIssuer = false;
        } else {
            this.prepaymentOperationTypeList(); // cheltuieli / venituri in avans
            this.operation.showUnreceiveInvoice = true;
            this.operation.showExternalIssuer = true;
        }
        this.searchConfig();
        this.elementList(autoOperType);
    }

    imoAssetOperTypeList() {
        this._enumService.imoAssetOperTypeList().subscribe(result => {
            this.operType = result.getImoAssetOperType;
        });
    }

    invOperationTypeList() {
        this._enumService.invOperationTypeList().subscribe(result => {
            this.operType = result.getInvOperationType;
        });
    }

    taxProfitOperationTypeList() {
        this._enumService.taxProfitOperationTypeList().subscribe(result => {
            this.operType = result;
        });
    }

    prepaymentOperationTypeList() {
        this._enumService.prepaymentsOperationTypeList().subscribe(result => {
            this.operType = result;
        });
    }

    getDocumentTypeList() {
        this._documentTypeService.documentTypeList().subscribe(result => {
            this.documentTypeList = result.getDocumentType;
        });
    }

    searchConfig() {
        this._autoOperationConfigService.searchConfig(this.operation).subscribe(result => {
            this.operation = result;
            console.log(this.operation);
        });
    }

    addDetailRow() {
        this._autoOperationConfigService.addDetailRow(this.operation).subscribe(result => {
            this.operation = result;
            this.changeIndivOperation();
        });
    }

    deleteRow(index: number) {
        this.operation.details[index].deleted = true;
    }

    changeIndivOperation() {
        for (var i = 0; i < this.operation.details.length; i++) {
            this.operation.details[i].individualOperation = this.operation.details[0].individualOperation;
        }
    }

    imoAssetStockElementList() {
        this._enumService.imoAssetStockElementList().subscribe(result => {
            this.element = result;
        });
    }

    valueSignList() {
        this._enumService.valueSignList().subscribe(result => {
            this.valuesSign = result;
        });
    }

    saveConfig() { // trebuie adaugat spinner
        this.isLoading = true;
        this._autoOperationConfigService.saveConfig(this.operation)
            .pipe(
                delay(1000),
                finalize(() => {
                    this.isLoading = false;
                })
            )
            .subscribe(result => {
                this.operation = result;
                abp.notify.info('AddUpdateMessage');
            });
    }

    save() {
        this.isLoading = true;
        this._autoOperationConfigService.saveAutoOperSearch(this.operation).pipe(
            delay(1000),
            finalize(() => {
                this.isLoading = false;
            }))
            .subscribe(result => {
            this.operation = result;
        });
    }

    elementList(autoOperType: number) {
        if (autoOperType == 0) {
            this.imoAssetStockElementList();
        }
        else if (autoOperType == 1) {
            this.invObjectStockElementList();
        }
        else if (autoOperType == 4) {
            this.taxProfitElementList();
        }
        else if (autoOperType == 5) { // TitluriDePlasamentAFS
            this.titluriDePlasamentElementList();
        }
        else if (autoOperType == 6) { // ObligatiuniAFS
            this.titluriDePlasamentElementList();
        }
        else if (autoOperType == 7) { // TitluriDePlasamentHTML
            this.titluriDePlasamentElementList();
        }
        else if (autoOperType == 8) { // ObligatiuniHTML
            this.titluriDePlasamentElementList();
        }
        else if (autoOperType == 9) { // DepoziteBancare
            this.depoziteBancareElementList();
        }
        else if (autoOperType == 10) { // RepoDepoDateInPensiuneLivrata
            this.repoDepoDateInPensiuneLivrataElementList();
        }
        else if (autoOperType == 11) { // RepoDepoPrimiteInPensiuneLivrata
            this.repoDepoPrimiteInPensiuneLivrataElementList();
        }
        else if (autoOperType == 12) { // CertificateDepozit
            this.certificateDepozitElementList();
        }
        else if (autoOperType == 13) { // Contributii
            this.contributiiElementList();
        }
        else if (autoOperType == 14) { // AjustariDeprecierePlasamente
            this.ajustariDeprecierePlasamenteElementList();
        }
        else if (autoOperType == 15) { // Reclasificari
            this.reclasificariElementList();
        }
        else if (autoOperType == 16) { // Imprumuturi
            this.imprumuturiElementList();
        }
        else {
            this.prepaymentStockElementList();
        }
    }

    invObjectStockElementList() {
        this._enumService.invObjectStockElementList().subscribe(result => {
            this.element = result;
        });
    }

    taxProfitElementList() {
        this._enumService.taxProfitElementList().subscribe(result => {
            this.element = result;
        });
    }

    prepaymentStockElementList() {
        this._enumService.prepaymentStockElementList().subscribe(result => {
            this.element = result;
        });
    }

    titluriDePlasamentOperTypeList() {
        this._enumService.titluriDePlasamentOperTypeList().subscribe(result => {
            this.operType = result;
        });
    }

    depoziteBancareOperTypeList() {
        this._enumService.depoziteBancareOperTypeList().subscribe(result => {
            this.operType = result;
        });
    }

    repoDepoDateInPensiuneLivrataOperTypeList() {
        this._enumService.repoDepoDateInPensiuneLivrataOperTypeList().subscribe(result => {
            this.operType = result;
        });
    }

    repoDepoPrimiteInPensiuneLivrataOperTypeList() {
        this._enumService.repoDepoPrimiteInPensiuneLivrataOperTypeList().subscribe(result => {
            this.operType = result;
        });
    }

    certificateDepozitOperTypeList() {
        this._enumService.certificateDepozitOperTypeList().subscribe(result => {
            this.operType = result;
        });
    }

    titluriDePlasamentElementList() {
        this._enumService.titluriDePlasamentElementList().subscribe(result => {
            this.element = result;
        });
    }

    depoziteBancareElementList() {
        this._enumService.depoziteBancareElementList().subscribe(result => {
            this.element = result;
        });
    }

    repoDepoDateInPensiuneLivrataElementList() {
        this._enumService.repoDepoDateInPensiuneLivrataElementList().subscribe(result => {
            this.element = result;
        });
    }

    repoDepoPrimiteInPensiuneLivrataElementList() {
        this._enumService.repoDepoPrimiteInPensiuneLivrataElementList().subscribe(result => {
            this.element = result;
        });
    }

    certificateDepozitElementList() {
        this._enumService.certificateDepozitElementList().subscribe(result => {
            this.element = result;
        });
    }

    contributiiElementList() {
        this._enumService.contributiiElementList().subscribe(result => {
            this.element = result;
        });
    }

    contributiiOperTypeList() {
        this._enumService.contributiiOperTypeList().subscribe(result => {
            this.operType = result;
        });
    }

    ajustariDeprecierePlasamenteElementList() {
        this._enumService.ajustariDeprecierePlasamenteElementList().subscribe(result => {
            this.element = result;
        });
    }

    ajustariDeprecierePlasamenteOperTypeList() {
        this._enumService.ajustariDeprecierePlasamenteOperTypeList().subscribe(result => {
            this.operType = result;
        });
    }

    reclasificariElementList() {
        this._enumService.reclasificariElementList().subscribe(result => {
            this.element = result;
        });
    }

    reclasificariOperTypeList() {
        this._enumService.reclasificariOperTypeList().subscribe(result => {
            this.operType = result;
        });
    }

    imprumuturiElementList() {
        this._enumService.imprumuturiElementList().subscribe(result => {
            this.element = result;
        });
    }

    imprumuturiOperTypeList() {
        this._enumService.imprumuturiOperTypeList().subscribe(result => {
            this.operType = result;
        });
    }
}