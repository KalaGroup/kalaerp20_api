import { Component, OnInit, Inject, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, FormControl } from '@angular/forms';
import { Observable } from 'rxjs';
import { startWith, map } from 'rxjs/operators';
import { MatDialog } from '@angular/material/dialog';//@angular/material/dialog
import { DialogOverviewExample } from '../dialog-overview-example/dialog-overview-example';
import { PurchaseRequisitionPartModel, SupplierDetails, PurchaseRequisitionPCName, PurchaseRequisitionPartDesc } from '../Model/PurchaseModel';
import { PurchaseReqService } from '../purchase-req.service';
import { SupplierdetailspopupComponent } from '../supplierdetailspopup/supplierdetailspopup.component';
//import { MatPaginator, MatSort, MatTable, MatTableDataSource, Sort } from '@angular/material';


export interface PeriodicElement {
    name: string;
    position: number;
    weight: number;
    symbol: string;
}

export interface PCName {
    value: string;
    viewValue: string;
}
export interface StateGroup {
    letter: string;
    names: string[];
}

export const _filter = (opt: string[], value: string): string[] => {
    const filterValue = value.toLowerCase();

    return opt.filter(item => item.toLowerCase().indexOf(filterValue) === 0);
};

@Component({
    selector: 'app-purchase-requisition',
    templateUrl: './purchase-requisition.component.html',
    styleUrls: ['./purchase-requisition.component.css']
})

export class PurchaseRequisitionComponent implements OnInit {
    purchaseReqList: PurchaseRequisitionPartModel;
    purchaseReqgridList: PurchaseRequisitionPartModel;
    purchaseReqPCName: PurchaseRequisitionPCName[] = [];

    purchaserequisitionPartDesc: PurchaseRequisitionPartDesc[] = [];

    animal: string;
    name: string;
    listarray: Array<any>;
    temparray: Array<any>;
    myControl = new FormControl();

   
    filteredOptions: PurchaseRequisitionPartDesc[] = [];
    displayedColumns: string[] = ['Sr.No', 'PartDescription','SuppName', 'UOM', 'RequiredQty', 'Action'];
    supplierList: SupplierDetails[];
    stateGroupOptions: Observable<StateGroup[]>;
    constructor(private _formBuilder: FormBuilder, private dialog: MatDialog, private dataservice: PurchaseReqService) { }

    ngOnInit() {

        this.dataservice.GetPurchaseReqPCName('03').subscribe(data => {
            this.purchaseReqPCName = data;
        })

        this.dataservice.GetPartDescReqByClassCode('03').subscribe(data => {
           
            this.purchaserequisitionPartDesc = data;
            this.filteredOptions = this.purchaserequisitionPartDesc;
        })

        // this.dataservice.GetPurchaseReqSuppName('0000301170000003000').subscribe(data => {
        //     debugger;
        // this.purchaseReqgridList=data;
       // })
       
        // this.filteredOptions = this.myControl.valueChanges.pipe(
        //     startWith(''),
        //     map(value => this._filter(value))
        // );

        this.purchaseReqgridList =
            {
                purchase_req_uom: "Nos", part_description: "(DW 801) 4 Grinder  Armature-->0000220010511000", part_code: "0000301170000003000",
                supplier:
                    [{
                        supplier_name: "Radhika Sales Corporation",
                        supplier_uom: "Nos",
                        rate: 135.36,
                        currency: "OMR",
                        validity: "31/08/2016",
                        quantity: 0,
                        converted_quantity: 1,
                        converted_uom: "nos"
                    }]
            }

        this.purchaseReqList =
            {
                purchase_req_uom: "pur_uom1", part_description: "part_desc_1", part_code: "123",
                supplier:
                    [
                    ]

            }

    }
    openDialog(): void {
        debugger;
        const dialogRef = this.dialog.open(DialogOverviewExample, {
            width: '70%',
            height: '60%',
            data: this.purchaseReqgridList
        });

        dialogRef.afterClosed().subscribe(result => {

            console.log('The dialog was closed');
            this.purchaseReqList = this.purchaseReqList = {
                purchase_req_uom: "Nos", part_description: "(DW 801) 4 Grinder  Armature-->0000220010511000", part_code: "0000301170000003000",
                supplier:
                    [{
                        supplier_name: "Radhika Sales Corporation",
                        supplier_uom: "Nos",
                        rate: 135.36,
                        currency: "OMR",
                        validity: "31/08/2016",
                        quantity: 0,
                        converted_quantity: 1,
                        converted_uom: "nos"
                    }]
            }
        });
    }
    openSupplierDetailsDialog(): void {
        debugger;
        const dialogRef = this.dialog.open(SupplierdetailspopupComponent, {
            width: '70%',
            height: '60%',
            data: this.purchaseReqgridList
        });

        dialogRef.afterClosed().subscribe(result => {

            console.log('The dialog was closed');
           
        });
    }
    

    private _filter(value: string): any[] {
        debugger;
        const filterValue = value.toLowerCase();

        return this.purchaserequisitionPartDesc.filter(option => option.PartDesc == filterValue);
    }

    // Responsive grid new code

    editField: string;


    changeValue(id: number, property: string, event: any) {
        this.editField = event.target.textContent;
    }
    //Actual API Classical

    

}



