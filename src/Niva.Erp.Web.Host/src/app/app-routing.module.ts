import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { AppRouteGuard } from '@shared/auth/auth-route-guard';
import { HomeComponent } from './home/home.component';
import { AboutComponent } from './about/about.component';
import { UsersComponent } from './users/users.component';
import { TenantsComponent } from './tenants/tenants.component';
import { RolesComponent } from 'app/roles/roles.component';
import { ChangePasswordComponent } from './users/change-password/change-password.component';


import { PersonComponent } from './setup/person/person.component';
import { ThirdPartyAccComponent } from './setup/banks/thirdPartyAcc.component';
import { ThirdPartyAccEditComponent } from './setup/banks/thirdPartyAccEdit.component';
import { PersonEditComponent } from './setup/person/personEdit.component';


import { appDxWebWiewComponent } from './reporting/appDxWebWiew.component';
import { ConfigReportingComponent } from './reporting/configReporting.component';


@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: AppComponent,
                children: [
                    { path: 'home', component: HomeComponent, canActivate: [AppRouteGuard] },
                    { path: 'users', component: UsersComponent, data: { permission: 'Pages.Users' }, canActivate: [AppRouteGuard] },
                    { path: 'roles', component: RolesComponent, data: { permission: 'Pages.Roles' }, canActivate: [AppRouteGuard] },
                    { path: 'tenants', component: TenantsComponent, data: { permission: 'Pages.Tenants' }, canActivate: [AppRouteGuard] },
                    { path: 'about', component: AboutComponent },
                    { path: 'update-password', component: ChangePasswordComponent },

                 
                    //Reporting
                    { path: 'dXWebView/:reportName', component: appDxWebWiewComponent, canActivate: [AppRouteGuard]},
                    { path: 'reporting/configReporting', component: ConfigReportingComponent, canActivate: [AppRouteGuard] },

                   

                    // Setup
                    { path: 'setup/person/person', component: PersonComponent, canActivate: [AppRouteGuard] },
                    { path: 'setup/person/personEdit', component: PersonEditComponent, canActivate: [AppRouteGuard] },
                    { path: 'setup/banks/thirdPartyAcc', component: ThirdPartyAccComponent, canActivate: [AppRouteGuard] },
                    { path: 'setup/banks/thirdPartyAccEdit', component: ThirdPartyAccEditComponent, canActivate: [AppRouteGuard] }
                ]
            }
        ])
    ],
    exports: [RouterModule]
})
export class AppRoutingModule { }
