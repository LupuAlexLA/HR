import { Component, Injector } from '@angular/core';
import { appModuleAnimation } from '../../../../shared/animations/routerTransition';
import { AppComponentBase } from '../../../../shared/app-component-base';

@Component({
    templateUrl: './inventoryBalanceList.component.html',
    animations: [appModuleAnimation()]
})
export class InventoryBalanceListComponent extends AppComponentBase {
    constructor(inject: Injector) {
        super(inject);
    }

    search() { }
}