import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, FormControl } from '@angular/forms';
import { Observable } from 'rxjs';
import { startWith, map } from 'rxjs/operators';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { PurchaseRequisitionPartModel, SupplierDetails } from '../Model/PurchaseModel';
import { PurchaseReqService } from '../purchase-req.service';

export const _filter = (opt: string[], value: string): string[] => {
  const filterValue = value.toLowerCase();

  return opt.filter(item => item.toLowerCase().indexOf(filterValue) === 0);
};
@Component({
  selector: 'dialog-overview-example',
  templateUrl: './dialog-overview-example.html',
  styleUrls: ['./dialog-overview-example.css']
})

  
export class DialogOverviewExample {

  supplierList: SupplierDetails[];
  displayedColumns: string[] = ['Sr.No', 'SuppName','UOM','Rate','Currency','Validity','Qty','ConvQty','ConvUOM'];
  constructor(
    public dialogRef: MatDialogRef<DialogOverviewExample>,
    @Inject(MAT_DIALOG_DATA) public purchaseReqList: PurchaseRequisitionPartModel,dataservice:PurchaseReqService) {
      
      
      this.supplierList = purchaseReqList.supplier;
    }

  onNoClick(): void {
    this.dialogRef.close();
  }
  onOKClick(): void {
    purchaseReqaddedList:PurchaseRequisitionPartModel;
    this.dialogRef.close();
        
  }
  onCancelClick(): void {
    this.dialogRef.close();
    
  }
}

