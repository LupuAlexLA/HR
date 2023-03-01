import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import {
    Router,
    RouterEvent,
    NavigationEnd,
    PRIMARY_OUTLET
} from '@angular/router';
import { BehaviorSubject, Observable, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, filter, map, startWith, switchMap } from 'rxjs/operators';
import { MenuItem } from '@shared/layout/menu-item';
import { FormControl } from '@angular/forms';

@Component({
    selector: 'sidebar-menu',
    templateUrl: './sidebar-menu.component.html'
})
export class SidebarMenuComponent extends AppComponentBase implements OnInit {
    control: FormControl;
    filteredMenu: Observable<any[]>;
    subscription: Subscription;
    filteredMenuItems: Observable<MenuItem[]>;
    menuItems: MenuItem[];
    menuItemsMap: { [key: number]: MenuItem } = {};
    activatedMenuItems: MenuItem[] = [];
    routerEvents: BehaviorSubject<RouterEvent> = new BehaviorSubject(undefined);
    homeRoute = '/app/home';

    constructor(injector: Injector, private router: Router) {
        super(injector);
        this.router.events.subscribe(this.routerEvents);

        this.control = new FormControl('');
        this.filteredMenu = this.control.valueChanges
            .pipe(
                startWith(''),
                debounceTime(400),
                distinctUntilChanged(),
                map(menu => menu ? this.filterMenu(menu) : this.recur(this.getMenuItems()).slice())
            );
    }

    ngOnInit(): void {
        

        this.menuItems = this.getMenuItems();
        this.patchMenuItems(this.menuItems);
        this.routerEvents
            .pipe(filter((event) => event instanceof NavigationEnd))
            .subscribe((event) => {
                const currentUrl = event.url !== '/' ? event.url : this.homeRoute;
                const primaryUrlSegmentGroup = this.router.parseUrl(currentUrl).root
                    .children[PRIMARY_OUTLET];
                if (primaryUrlSegmentGroup) {
                    this.activateMenuItems('/' + primaryUrlSegmentGroup.toString());
                }
            });
    }

    filterMenu(name: string) {
        
        return this.recur(this.getMenuItems()).filter(i => i.label.toLowerCase().includes(name.toLowerCase()) || i.parentLabel.toLowerCase().includes(name.toLowerCase()) );
    }

    getMenuItems(): MenuItem[] {
        var ret = Array<MenuItem>();
        ret.push( 
            new MenuItem(this.l('HomePage'), '/app/home', 'fas fa-home'),
            new MenuItem(
                this.l('Tenants'),
                '/app/tenants',
                'fas fa-building',
                'Pages.Tenants'
            ),
            new MenuItem(
                this.l('Users'),
                '/app/users',
                'fas fa-users',
                'Pages.Users'
            ),
            new MenuItem(
                this.l('Roles'),
                '/app/roles',
                'fas fa-theater-masks',
                'Pages.Roles'
            ));

        ret.push(
            // General
            new MenuItem(this.l('General'), '', 'fas fa-circle', '', [
                new MenuItem(this.l('Date Societate'), '/app/setup/person/person', 'far fa-circle','General.DateSocietate.Acces'),
                new MenuItem(this.l('Emitenti'), '/app/emitenti', 'far fa-circle','General.Emitenti.Acces'),
                new MenuItem(this.l('Persoane'), '/app/nomenclatoare/person/personList', 'far fa-circle', 'General.Persoane.Acces'),
                new MenuItem(this.l('Lista conturi'), '/app/setup/banks/thirdPartyAcc', 'far fa-circle', 'General.ListaConturi.Acces'),
                new MenuItem(this.l('Contracte'), '/app/economic/contracts/contractsList', 'far fa-circle', 'General.Contracte.Acces'),
                new MenuItem(this.l('Curs valutar'), '/app/conta/nomenclatoare/exchangeRates/exchangeRatesList', 'far fa-circle','General.CursValutar.Acces')
            ]),
            
            //Buget
            new MenuItem(this.l('Buget'), '', 'fas fa-circle', '', [
                new MenuItem(this.l('BVC'), '', 'fas fa-dot-circle', '', [
                    new MenuItem(this.l('Configurare'), '/app/buget/bugetConfig/bugetConfigList', 'far fa-circle', 'Buget.BVC.Configurare'),
                    new MenuItem(this.l('Parametrii'), '/app/buget/cheltuieli/bugetParametrii', 'far fa-circle', 'Buget.BVC.Parametrii.Acces'),
                    new MenuItem(this.l('Repartizare Cheltuieli'), '/app/buget/repartizat/bugetRepartizat', 'far fa-circle','Buget.BVC.Preliminat'),
                    new MenuItem(this.l('Prevazut'), '/app/buget/prevazut/bugetPrevList', 'far fa-circle','Buget.BVC.Prevazut.Acces'),
                    new MenuItem(this.l('Venituri / Incasari'), '/app/buget/venituri/bugetVenitList', 'far fa-circle', 'Buget.BVC.Venituri.Acces'),
                    new MenuItem(this.l('Realizat'), '/app/buget/realizat/bugetRealizatList', 'far fa-circle','Buget.BVC.Realizat.Acces'),
                    new MenuItem(this.l('Rapoarte'), '/app/buget/reportBVC', 'far fa-circle','Buget.BVC.Rapoarte'),
                ]),
                new MenuItem(this.l('Planificare achizitii'), '/app/buget/paap/paapList', 'far fa-circle','Buget.PAAP.Acces'),
                new MenuItem(this.l('Curs valutar estimat'), '/app/buget/paap/exchangeRateForecast', 'far fa-circle','Buget.CursValutarEstimat.Acces'),
            ]),

            //Casierie
            new MenuItem(this.l('Casierie'), '', 'fas fa-circle', '', [
                new MenuItem(this.l('Ordine de plata'), '/app/economic/paymentOrders/paymentOrders', 'far fa-circle','Casierie.OP.Acces'),
                new MenuItem(this.l('Operatiuni cu numerar'), '', 'fas fa-dot-circle', '', [
                    new MenuItem(this.l('Dispozitii catre casierie'), '/app/economic/dispositions/dispositionList', 'far fa-circle','Casierie.Numerar.Dispozitii.Acces'),
                    new MenuItem(this.l('Depunere/Retragere'), '/app/economic/dispositions/depositList', 'far fa-circle','Casierie.Numerar.DepunRetrag.Acces'),
                    new MenuItem(this.l('Cupiuri'), '/app/economic/cupiuri/cupiuriList', 'far fa-circle','Casierie.Numerar.Cupiuri.Acces'),
                    new MenuItem(this.l('Sold initial'), '/app/economic/dispositions/soldList', 'far fa-circle','Casierie.Numerar.SoldInitial.Acces'),
                ]),
                new MenuItem(this.l('Schimb valutar'), '/app/economic/exchange/exchangeList', 'far fa-circle','Casierie.SchimbValutar.Acces'),
                new MenuItem(this.l('Rapoarte'), '', 'fas fa-dot-circle', 'Casierie.Rapoarte.Acces', [
                    new MenuItem(this.l('Registru de casa'), '/app/reporting/registruCasa', 'far fa-circle',''),
                    new MenuItem(this.l('Declaratie casier'), '/app/reporting/declaratieCasier', 'far fa-circle'),
                ]),
            ]),

            //Contabilitate
            new MenuItem(this.l('Contabilitate'), '', 'fas fa-circle', '', [
                new MenuItem('Documente primite si emise', '/app/economic/invoices/invoicesList', 'fas fa-circle','Conta.Documente.Acces'),
                new MenuItem(this.l('Deconturi'), '/app/decont', 'fas fa-circle','Conta.Deconturi.Acces'),
                new MenuItem(this.l('Operatii contabile'), '', 'fas fa-dot-circle', '', [
                    new MenuItem(this.l('Operatii contabile'), '/app/conta/operations/operationsList', 'far fa-circle','Conta.OperContab.OperContab.Acces'),
                    new MenuItem(this.l('Import extrase bancare'), '/app/conta/operations/foreignOperation', 'far fa-circle','Conta.OperContab.Extras.Acces')
                ]),
                new MenuItem(this.l('Mijloace fixe'), '', 'fas fa-dot-circle', '', [
                    new MenuItem(this.l('Intrari mijloace fixe'), '/app/conta/imoAsset/imoAssetPV', 'far fa-circle','Conta.MF.Intrari.Acces'),
                    new MenuItem(this.l('Operatii mijloace fixe'), '/app/conta/imoAsset/imoAssetOperation', 'far fa-circle','Conta.MF.Operatii.Acces'),
                    new MenuItem(this.l('Inventariere mijloace fixe'), '/app/conta/imoAsset/imoInventariere', 'far fa-circle','Conta.MF.Inventar.Acces'),
                    new MenuItem(this.l('Rapoarte'), '/app/conta/imoAsset/imoAssetReporting', 'far fa-circle','Conta.MF.Rapoarte.Acces'),
                ]),
                new MenuItem(this.l('Obiecte de inventar'), '', 'fas fa-dot-circle', '', [
                    new MenuItem(this.l('Intrari obiecte de inventar'), '/app/conta/invObjects/invObject', 'far fa-circle','Conta.ObInventar.Intrari.Acces'),
                    new MenuItem(this.l('Operatii obiecte de inventar'), '/app/conta/invObjects/invObjectOperation', 'far fa-circle','Conta.ObInventar.Operatii.Acces'),
                    new MenuItem(this.l('Inventariere obiecte'), '/app/conta/invObjects/invObjectInventariere', 'far fa-circle','Conta.ObInventar.Inventar.Acces'),
                    new MenuItem(this.l('Rapoarte'), '/app/conta/invObjects/invObjectReport', 'far fa-circle','Conta.ObInventar.Rapoarte.Acces'),
                ]),
                new MenuItem(this.l('Cheltuieli in avans'), '', 'fas fa-dot-circle', '', [
                    new MenuItem(this.l('Cheltuieli'), '/app/conta/prepayments/prepaymentsInit', 'far fa-circle','Conta.CheltAvans.Chelt.Acces'),
                    new MenuItem(this.l('Rapoarte'), '/app/conta/prepayments/prepaymentsReporting', 'far fa-circle','Conta.CheltAvans.Rapoarte.Acces'),
                ]),
                new MenuItem(this.l('Venituri in avans'), '', 'fas fa-dot-circle', '', [
                    new MenuItem(this.l('Venituri'), '/app/conta/prepayments/preincomesInit', 'far fa-circle','Conta.VenitAvans.Venituri.Acces'),
                    new MenuItem(this.l('Rapoarte'), '/app/conta/prepayments/preincomesReporting', 'far fa-circle','Conta.VenitAvans.Venituri.Rapoarte'),
                ]),
                new MenuItem(this.l('Balanta'), '', 'fas fa-dot-circle', '', [
                    new MenuItem(this.l('Balante'), '/app/conta/balance/balanceList', 'far fa-circle','Conta.Balanta.Balante.Acces'),
                    new MenuItem(this.l('Sfarsit de luna'), '/app/conta/balance/operGenerate/operGenerateList', 'far fa-circle','Conta.Balanta.SfarsitLuna.Acces'),
                    new MenuItem(this.l('Balante salvate'), '/app/conta/balance/savedBalanceList', 'far fa-circle','Conta.Balanta.BalanteSalvate.Acces')
                ]),
                new MenuItem(this.l('Situatii financiare'), '', 'fas fa-dot-circle', '', [
                    new MenuItem(this.l('Calcul'), '/app/conta/sitFinan/sitFinanCalc', 'far fa-circle','Conta.SitFinan.Calcul.Acces'),
                    new MenuItem(this.l('Configurare'), '/app/conta/sitFinan/sitFinanConfig', 'far fa-circle','Conta.SitFinan.Config.Acces'),
                    new MenuItem(this.l('Rapoarte'), '/app/conta/sitFinan/sitFinanRap', 'far fa-circle','Conta.SitFinan.Rapoarte.Acces'),
                ]),
                new MenuItem(this.l("Raportare BNR"), '', 'fas fa-dot-circle', '', [
                    new MenuItem(this.l('BNR Conturi'), '/app/sectoareBnr/calculBnr', 'far fa-circle','Conta.BNR.Conturi.Acces'),
                    new MenuItem(this.l('Configurare'), '/app/sectoareBnr/anexaBnr', 'far fa-circle','Conta.BNR.Configurare.Acces'),
                    new MenuItem(this.l('Raportare'), '/app/sectoareBnr/raportareBNR', 'far fa-circle','Conta.BNR.Raportare.Acces'),
                ]),
                new MenuItem(this.l("Lichiditate"), '', 'fas fa-dot-circle', '', [
                    new MenuItem(this.l('Configurare'), '/app/conta/lichiditate/lichidConfig', 'far fa-circle', 'Conta.Lich.Configurare.Acces'),
                    new MenuItem(this.l('Calcul'), '/app/conta/lichiditate/lichidCalcul', 'far fa-circle', 'Conta.Lich.Calcul.Acces'),
                ]),
                new MenuItem(this.l('Rapoarte'), '', 'fas fa-dot-circle', '', [
                    new MenuItem(this.l('Rapoarte'), '/app/reporting', 'fas fa-circle','Conta.Rapoarte.Rapoarte.Acces'),
                    new MenuItem(this.l('Rapoarte configurabile'), '/app/reporting/configReporting', 'fas fa-circle','Conta.Rapoarte.RapoarteConfigurabile.Acces')

                ])
            ]),

            // Imprumuturi

            new MenuItem(this.l('Imprumuturi'), '', 'fas fa-circle', 'Imprumuturi.Acces', [
                new MenuItem(this.l('Lista Imprumuturi'), '/app/imprumuturi/imprumuturiList', 'far fa-circle'),
                new MenuItem(this.l('Calcul Dobanda Lunara'), '/app/imprumuturi/calculatorDobanda', 'far fa-circle'),
                new MenuItem(this.l('Dobanzi de Referinta'), '/app/imprumuturi/dobandaDeReferinta', 'far fa-circle'),
                new MenuItem(this.l('Nomenclator'), '/app/imprumuturi/imprumuturiNomenclatoare', 'far fa-circle'),
                
            ]),

            //Administrare
            new MenuItem(this.l('Administrare'), '', 'fas fa-circle', '', [
                new MenuItem(this.l('Contracte'), '', 'fas fa-dot-circle', '', [
                    new MenuItem(this.l('Beneficiari de contracte'), '/app/economic/contractCategory/contractCategoryList', 'far fa-circle','Administrare.Contracte.Beneficiari.Acces'),
                ]),
                new MenuItem(this.l('Mijloace fixe'), '', 'fas fa-dot-circle', '', [
                    new MenuItem(this.l('Nomenclatoare'), '/app/conta/imoAsset/imoNomenclatoare', 'far fa-circle','Administrare.MF.Nomenclatoare.Acces'),
                    new MenuItem(this.l('Setup'), '/app/conta/imoAsset/imoSetup', 'far fa-circle','Administrare.MF.Setup.Acces'),
                    new MenuItem(this.l('Calcul gestiune'), '/app/conta/imoAsset/imoGestList', 'far fa-circle','Administrare.MF.CalculGestiune.Acces'),
                ]),
                new MenuItem(this.l('Obiecte de inventar'), '', 'fas fa-dot-circle', '', [
                    new MenuItem(this.l('Nomenclatoare'), '/app/conta/invObjects/invObjectNomenclatoare', 'far fa-circle','Administrare.ObInventar.Nomenclatoare.Acces'),
                    new MenuItem(this.l('Calcul gestiune'), '/app/conta/invObjects/invObjectGestList', 'far fa-circle','Administrare.ObInventar.CalculGestiune.Acces'),
                ]),
                new MenuItem(this.l('Cheltuieli in avans'), '', 'fas fa-dot-circle', '', [
                    new MenuItem(this.l('Tipuri documente'), '/app/conta/prepayments/prepaymentsOperDocType', 'far fa-circle','Administrare.CheltAvans.TipuriDoc.Acces'),
                    new MenuItem(this.l('Calcul gestiune'), '/app/conta/prepayments/prepaymentsGestInit', 'far fa-circle','Administrare.CheltAvans.CalculGestiune.Acces'),
                    new MenuItem(this.l('Setup'), '/app/conta/prepayments/prepaymentsSetupInit', 'far fa-circle','Administrare.CheltAvans.Setup.Acces'),
                ]),
                new MenuItem(this.l('Venituri in avans'), '', 'fas fa-dot-circle', '', [
                    new MenuItem(this.l('Tipuri documente'), '/app/conta/prepayments/prepaymentsOperDocType', 'far fa-circle','Administrare.VenitAvans.TipuriDoc.Acces'),
                    new MenuItem(this.l('Calcul gestiune'), '/app/conta/prepayments/preincomesGestInit', 'far fa-circle','Administrare.VenitAvans.CalculGestiune.Acces'),
                    new MenuItem(this.l('Setup'), '/app/conta/prepayments/preincomesSetupInit', 'far fa-circle','Administrare.VenitAvans.Setup.Acces'),
                ]),
                new MenuItem(this.l('Contabilitate'), '', 'fas fa-dot-circle', '', [
                    new MenuItem(this.l('Plan de conturi'), '/app/conta/nomenclatoare/accounts/accountList', 'far fa-circle','Admin.Conta.PlanConturi.Acces'),
                    new MenuItem(this.l('Tipuri documente'), '/app/conta/nomenclatoare/document/docType', 'far fa-circle','Admin.Conta.TipDoc.Acces'),
                    new MenuItem(this.l('Tipuri operatii contabile'), '/app/conta/nomenclatoare/operationsType/operationType', 'far fa-circle','Admin.Conta.TipOperContab.Acces'),
                    new MenuItem(this.l('Configurare monografii'), '/app/conta//autoOperation/autoOperationConfig', 'far fa-circle','Admin.Conta.Monografii.Acces'),
                    new MenuItem(this.l('Config conturi'), '/app/conta/nomenclatoare/accounts/accountConfig', 'far fa-circle','Admin.Conta.ConfigConturi.Acces'),
                    new MenuItem(this.l('Categorii elemente factura'), '/app/conta/nomenclatoare/categoryElements/categoryDetailElementsList', 'far fa-circle','Admin.Conta.CategElemFactura.Acces'),
                    new MenuItem(this.l('Elemente factura'), '/app/conta/nomenclatoare/categoryElements/detailElementsList', 'far fa-circle','Admin.Conta.ElemFactura.Acces'),
                    new MenuItem(this.l('Tip Activitate'), '/app/nomenclatoare/activity/activityType', 'far fa-circle','Admin.Conta.TipActivitate.Acces'),
                    new MenuItem(this.l('Contari automate'), '/app/conta/autoOperation/autoOperations', 'far fa-circle','Admin.Conta.ContariAutomate.Acces'),
                    new MenuItem(this.l('Facturi'), '/app/conta/autoOperation/autoInvoices', 'far fa-circle','Admin.Conta.Facturi.Acces'),
                    new MenuItem(this.l('Valori diurna pe zi'), '/app/conta/nomenclatoare/diurna/diurnaPerZiList', 'far fa-circle','Admin.Conta.DiurnaZi.Acces'),
                    new MenuItem(this.l('Valori diurna legala'), '/app/conta/nomenclatoare/diurna/diurnaList', 'far fa-circle','Admin.Conta.DiurnaLegala.Acces'),
                    new MenuItem(this.l('Configurare rapoarte'), '/app/conta/nomenclatoare/configRapoarte/rapConfigList', 'far fa-circle','Admin.Conta.ConfigRapoarte.Acces'),
                    new MenuItem(this.l('Cota TVA'), '/app/conta/nomenclatoare/cotaTVA/cotaTVAList', 'far fa-circle','Admin.Conta.CotaTVA.Acces'),
                    new MenuItem(this.l('Sectoare BNR'), '/app/conta/bnrSector/bnrSectorList', 'far fa-circle','Admin.Conta.SectoareBNR.Acces'),
                ]),
                new MenuItem(this.l('FileDoc'), '', 'fas fa-dot-circle', '', [
                    new MenuItem(this.l('Erori Import'), '/app/setup/fileDoc/fileDocErrors', 'far fa-circle', 'Admin.Conta.FileDocErrors'),
                ])
            ])
        );
        return ret;
    }

    recur(menuItems: MenuItem[]): MenuItem[]  {
        var ret = [];
        
        for (var i = 0; i < menuItems.length; i++) {
            
            if (menuItems[i].children) {

                let arr = this.recur(menuItems[i].children);


                for (let j = 0; j < arr.length; j++) {
                    if (!arr[j].parentLabel) {
                        arr[j].parentLabel = menuItems[i].label;
                    }
                }


                ret = ret.concat(arr);

            }
            else {
                menuItems[i].parentLabel = '';
            }
            if (menuItems[i].route != '') {
                ret.push(menuItems[i]);
            }
                
            
            
        }
        
    return ret;
}

    searchItem(value) {
        
     
        this.router.navigate([value]);
       
    }

    patchMenuItems(items: MenuItem[], parentId?: number): void {
        items.forEach((item: MenuItem, index: number) => {
            item.id = parentId ? Number(parentId + '' + (index + 1)) : index + 1;
            if (parentId) {
                item.parentId = parentId;
            }
            if (parentId || item.children) {
                this.menuItemsMap[item.id] = item;
            }
            if (item.children) {
                this.patchMenuItems(item.children, item.id);
            }
        });
    }

    activateMenuItems(url: string): void {
        this.deactivateMenuItems(this.menuItems);
        this.activatedMenuItems = [];
        const foundedItems = this.findMenuItemsByUrl(url, this.menuItems);
        foundedItems.forEach((item) => {
            this.activateMenuItem(item);
        });
    }

    deactivateMenuItems(items: MenuItem[]): void {
        items.forEach((item: MenuItem) => {
            item.isActive = false;
            item.isCollapsed = true;
            if (item.children) {
                this.deactivateMenuItems(item.children);
            }
        });
    }

    findMenuItemsByUrl(
        url: string,
        items: MenuItem[],
        foundedItems: MenuItem[] = []
    ): MenuItem[] {
        items.forEach((item: MenuItem) => {
            if (item.route === url) {
                foundedItems.push(item);
            } else if (item.children) {
                this.findMenuItemsByUrl(url, item.children, foundedItems);
            }
        });
        return foundedItems;
    }

    activateMenuItem(item: MenuItem): void {
        item.isActive = true;
        if (item.children) {
            item.isCollapsed = false;
        }
        this.activatedMenuItems.push(item);
        if (item.parentId) {
            this.activateMenuItem(this.menuItemsMap[item.parentId]);
        }
    }

    isMenuItemVisible(item: MenuItem): boolean {
        var isAnyChildVisible = false;
        if (item.children) {
            
            for (var i = 0; i < item.children.length; i++) {
                if (this.isMenuItemVisible(item.children[i]) == true)
                    isAnyChildVisible= true;
            }
            if (item.permissionName)
                return isAnyChildVisible && this.permission.isGranted(item.permissionName);
            else
                return isAnyChildVisible;
        }        
         
        if (!item.permissionName) {
            return true;
        }
        return this.permission.isGranted(item.permissionName) ;
    }
}
