import { Component, Input, OnInit, ChangeDetectorRef, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormArray, FormBuilder } from '@angular/forms';
import { NgbDateParserFormatter, NgbCalendar, NgbDateAdapter } from '@ng-bootstrap/ng-bootstrap';
import { DateParserService } from 'src/app/service/dateParser.service';
import { SupplierService } from 'src/app/service/supplier.service';

@Component({
    selector: 'app-referral',
    templateUrl: './referral.component.html',
    styleUrls: ['./referral.component.scss'],
    providers: [
        {provide: NgbDateParserFormatter, useClass: DateParserService}
    ]
})
export class ReferralComponent implements OnInit {

    @Input() formGroupName: number;
    @Input() referralForm: FormGroup
    @Input() index: number;
    @Input() component: string;
    supportList: any ;
    @Output() referralToRemove = new EventEmitter<number>();
    @Input() rowArr: any;
    @Input() recArr: any;


    constructor(private builder: FormBuilder, private cd: ChangeDetectorRef, private ngbCalendar: NgbCalendar, private dateAdapter: NgbDateAdapter<string>, private supplierService: SupplierService) { }

    get referralRows() {
        return this.referralForm.get('referralRows') as FormArray;
    }

    get referralAttachments() {
        return this.referralForm.get('referralAttachments') as FormArray;
    }

    get receiptAttachments() {
        return this.referralForm.get('receiptAttachments') as FormArray;
    }

    get referralControl(){
        return this.referralForm.controls;
    }

    ngOnInit() {
        this.supportList = this.supplierService.getSupportItems();
        //this.referralForm.get('receiptNumber').setValue(this.index);
        console.log(this.rowArr)
            if(this.rowArr !== [] && this.rowArr !== undefined) {
                for(let i=0; i< this.rowArr.length; i++) {
                    let val = this.rowArr[i];
                    for(let j =0; j< val.length; j++) {
                        this.referralRows.push(this.createRowFormWithValues(val[j]));
                    }
                   
                };
                this.cd.detectChanges();
            } else if(this.recArr !== [] && this.recArr !== undefined) {
                for(let i=0; i< this.recArr.length; i++) {
                    let val = this.recArr[i];
                    for(let j =0; j< val.length; j++) {
                        this.referralRows.push(this.createRowFormWithValues(val[j]));
                    }
                   
                };
                this.cd.detectChanges();
            }
            else {

                this.referralRows.push(this.createRowForm());
            }
            this.referralRows.push(this.createRowForm());
            this.onChanges();
    }

    createRowForm() {
        return this.builder.group({
            supportProvided: [''],
            description: [''],
            gst: [''],
            amount: ['']
        })
    }

    onChanges() {
        this.referralForm.get('referralRows').valueChanges.subscribe(formrow =>{
            let gstSum = formrow.reduce((prev, next) => prev + +next.gst, 0);
            let amtSum = formrow.reduce((prev, next) => prev + +next.amount, 0);
            this.referralForm.get('totalGst').setValue(gstSum);
            this.referralForm.get('totalAmount').setValue(amtSum);
        });
    }

    deleteRow(rowIndex: number) {
        this.referralRows.removeAt(rowIndex);
    }

    addRow() {
        this.referralRows.push(this.createRowForm());
        this.cd.detectChanges();
    }

    setReferralFormControl(event: any) {
        const reader = new FileReader();
        reader.readAsDataURL(event);
        reader.onload = () => {
            this.referralAttachments.push(this.createAttachmentObject({
                fileName: event.name,
                file: reader.result
            }))
        }
        //this.cd.markForCheck();
    }

    deleteReferralFormControl(event: any) {
        this.referralAttachments.removeAt(event);
    }

    setReceiptFormControl(event: any) {
        const reader = new FileReader();
        reader.readAsDataURL(event);
        reader.onload = () => {
            this.receiptAttachments.push(this.createAttachmentObject({
                fileName: event.name,
                file: reader.result
            }))
        }
        //this.cd.markForCheck();
    }

    deleteReceiptFormControl(event: any) {
        this.receiptAttachments.removeAt(event);
    }

    createAttachmentObject(data: any) {
        return this.builder.group(data);
    }

    createRowFormWithValues(row: any) {
        return this.builder.group({
            supportProvided: [row.supportProvided],
            description: [row.description],
            gst: [row.gst],
            amount: [row.amount]
        })
    }

}