/// <reference path="../../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { FiledocSearchComponent } from './filedoc-search.component';

let component: FiledocSearchComponent;
let fixture: ComponentFixture<FiledocSearchComponent>;

describe('filedoc-search component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ FiledocSearchComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(FiledocSearchComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});