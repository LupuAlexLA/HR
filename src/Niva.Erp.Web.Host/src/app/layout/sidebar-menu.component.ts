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
