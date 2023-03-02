import { Component, Injector, ChangeDetectionStrategy } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';

@Component({
  templateUrl: './home.component.html',
  animations: [appModuleAnimation()],
  //changeDetection: ChangeDetectionStrategy.OnPush
})

export class HomeComponent extends AppComponentBase {

    
    constructor(injector: Injector,
        private _modalService: BsModalService) {
        super(injector);
    }

    ngOnInit() {
        
    }

    
}
