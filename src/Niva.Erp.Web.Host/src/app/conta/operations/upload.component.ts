import { Component, Injector, OnInit } from "@angular/core";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { AppComponentBase } from "../../../shared/app-component-base";
import { FileUploadDto, OperationSearchDto, OperationServiceProxy } from "../../../shared/service-proxies/service-proxies";

@Component({
    templateUrl: './upload.component.html',
    animations: [appModuleAnimation()]
})
export class UploadComponent extends AppComponentBase implements OnInit {

    searchOperation: OperationSearchDto = new OperationSearchDto();
    operations: FileUploadDto;
    selectedFile: File;

    ngOnInit() {

    }

    constructor(injector: Injector,
        private _operationService: OperationServiceProxy) {
        super(injector);
    } 

    handleFileInput(event) {
        //const fileReader = new FileReader();

        //fileReader.readAsText(this.selectedFile, "UTF-8");
        //fileReader.readAsDataURL(event.target.files[0]);
        //console.log('fileReader' + JSON.stringify(fileReader));

        //fileReader.onload = (loadEvent) => {
        //    this.operations.content = loadEvent.target.result.toString();
        //}
        //console.log(this.operations);
    }

    uploadFile() {
        this._operationService.uploadOperationFile(this.operations).subscribe(result => {
            //abp.notify.info()
            //abp.notify.info(this.l('UploadMessage'));
        });
    }
}