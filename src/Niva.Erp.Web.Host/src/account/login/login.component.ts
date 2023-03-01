import { Component, Injector, OnInit } from '@angular/core';
import { AbpSessionService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/app-component-base';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppAuthService } from '@shared/auth/app-auth.service';
import { ActivatedRoute } from '@angular/router';
import { AccountServiceProxy, IsTenantAvailableInput, IsTenantAvailableOutput } from '../../shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
import { AppTenantAvailabilityState } from '../../shared/AppEnums';

@Component({
  templateUrl: './login.component.html',
  animations: [accountModuleAnimation()]
})
export class LoginComponent extends AppComponentBase implements OnInit  {
    submitting = false;
    loginToken: string;
  constructor(
    injector: Injector,
    public authService: AppAuthService,
      private _sessionService: AbpSessionService,
      private _accountService: AccountServiceProxy,
    private _route: ActivatedRoute
  ) {
    super(injector);
  }
    ngOnInit(): void {
        this.loginToken = this._route.snapshot.queryParamMap.get('token');
        if (this.loginToken !== null) {
            this.submitting = true;
            abp.multiTenancy.setTenantIdCookie(1); //hardcoded default tenent
            //
            //const input = new IsTenantAvailableInput();
            //input.tenancyName = 'Default';

             
            //this._accountService
            //    .isTenantAvailable(input)
            //    .pipe(
            //        finalize(() => {
                         
            //        })
            //    )
            //    .subscribe((result: IsTenantAvailableOutput) => {
            //        switch (result.state) {
            //            case AppTenantAvailabilityState.Available:
            //                abp.multiTenancy.setTenantIdCookie(result.tenantId);
            //                location.reload();
            //                return;
            //            case AppTenantAvailabilityState.InActive:
            //                this.message.warn(this.l('TenantIsNotActive', 'Default'));
            //                break;
            //            case AppTenantAvailabilityState.NotFound:
            //                this.message.warn(
            //                    this.l('ThereIsNoTenantDefinedWithName{0}', 'Default')
            //                );
            //                break;
            //        }
            //    });
            //
            this.authService.tokenAuthenticate(this.loginToken, () => ( this.submitting = false));
        }
    }

  get multiTenancySideIsTeanant(): boolean {
    return this._sessionService.tenantId > 0;
  }

  get isSelfRegistrationAllowed(): boolean {
    if (!this._sessionService.tenantId) {
      return false;
    }

    return true;
  }

  login(): void {
    this.submitting = true;
      this.authService.authenticate(() => (this.submitting = false));
  }
}
