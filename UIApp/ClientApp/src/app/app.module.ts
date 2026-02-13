import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { PurchaseRequisitionComponent} from './purchase-requisition/purchase-requisition.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatInputModule, MatFormFieldModule, MatSelectModule, MatStepperModule, MatTableModule, MatAutocompleteModule, MatDialogModule } from '@angular/material';
import { DialogOverviewExample} from './dialog-overview-example/dialog-overview-example';
//import { AgGridModule } from 'ag-grid-angular';
import { AggridComponent } from './aggrid/aggrid.component';
import { SupplierdetailspopupComponent } from './supplierdetailspopup/supplierdetailspopup.component';

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        HomeComponent,
        CounterComponent,
        FetchDataComponent,
        PurchaseRequisitionComponent,
        DialogOverviewExample,
        AggridComponent,
        SupplierdetailspopupComponent
    ],
    entryComponents: [DialogOverviewExample,SupplierdetailspopupComponent],    
    imports: [
        BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
        HttpClientModule,
        FormsModule,
        MatSelectModule,
        MatInputModule,
        MatFormFieldModule,        
        MatStepperModule,
        MatTableModule,
        MatAutocompleteModule,
        ReactiveFormsModule,MatDialogModule,
        RouterModule.forRoot([
            { path: '', component: HomeComponent, pathMatch: 'full' },
            { path: 'counter', component: CounterComponent },
            { path: 'fetch-data', component: FetchDataComponent },
            { path: 'purchase-requisition', component: PurchaseRequisitionComponent },
            { path: 'aggrid', component: AggridComponent },
        ]),
        BrowserAnimationsModule
    ],
    providers: [],
    bootstrap: [AppComponent]
})
export class AppModule { }
