import { Component, Injector, Input, OnInit } from '@angular/core';
import { AppComponentBase } from '../../../shared/app-component-base';
import { AtasamentDTO, FileDocExternServiceProxy } from '../../../shared/service-proxies/service-proxies';
import { saveAs } from 'file-saver';
@Component({
    selector: 'app-file-doc-attachment',
    templateUrl: './file-doc-attachment.component.html',
    styleUrls: ['./file-doc-attachment.component.css']
})
/** fileDocAttachment component*/
export class FileDocAttachmentComponent extends AppComponentBase implements OnInit {
    @Input() fileId: number;
    @Input() regNumber;
    public attachments: AtasamentDTO[];
    public viewUrl: string;
    /** fileDocAttachment ctor */
    constructor(injector: Injector,
        private fileDocService: FileDocExternServiceProxy)
    {
        super(injector);
    }
    ngOnInit(): void {
        if (this.fileId) {
            this.fileDocService.getAtasamente(this.fileId)
            .subscribe(result => {
                this.attachments = result;
            });
            this.fileDocService.getFileDocViewUrl().subscribe(result => {
                this.viewUrl = result + this.fileId;
            });
        }

    }

    download(attId): void {
        this.fileDocService.getAtasamentFile(attId).subscribe(result => {
            const byteCharacters = atob(result.content);
            const byteNumbers = new Array(byteCharacters.length);
            for (let i = 0; i < byteCharacters.length; i++) {
                byteNumbers[i] = byteCharacters.charCodeAt(i);
            }
            const byteArray = new Uint8Array(byteNumbers);
            const blob = new Blob([byteArray], { type: 'application/binary' });
            //var blob = new Blob(result.u, { type: "text/plain;charset=utf-8" });
            saveAs(blob, result.fileName);
            abp.notify.success("Fisierul a fost downloadat"); 
        });
    }
}