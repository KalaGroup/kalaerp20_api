// import {Component, Inject} from '@angular/core';
// import {MatDialog, MatDialogRef, MAT_DIALOG_DATA} from '@angular/material/dialog';

// export interface DialogData {
//   animal: string;
//   name: string;
// }

// /**
//  * @title Dialog Overview
//  */
// @Component({
//   selector: 'dialog-componant.component',
//   templateUrl: 'dialog-componant.component.html',
//   styleUrls: ['dialog-componant.component.css'],
// })
// export class DialogOverviewExample {

//   animal: string;
//   name: string;

  
//       constructor(
//         public dialogRef: MatDialogRef<DialogOverviewExample>,
//         @Inject(MAT_DIALOG_DATA) public data: DialogData) {}
    
//       onNoClick(): void {
//         this.dialogRef.close();
//       }

//       onOKClick(): void {
        
//       }
//       onCancelClick(): void {
//         this.dialogRef.close();
        
//       }
  
    
// }

// // @Component({
// //   selector: 'dialog-overview-example-dialog',
// //   templateUrl: 'dialog-overview-example-dialog.html',
// // })
// // export class DialogOverviewExampleDialog {

// //   constructor(
// //     public dialogRef: MatDialogRef<DialogOverviewExampleDialog>,
// //     @Inject(MAT_DIALOG_DATA) public data: DialogData) {}

// //   onNoClick(): void {
// //     this.dialogRef.close();
// //   }

// // }


// /**  Copyright 2019 Google Inc. All Rights Reserved.
//     Use of this source code is governed by an MIT-style license that
//     can be found in the LICENSE file at http://angular.io/license */