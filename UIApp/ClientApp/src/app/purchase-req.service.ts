import { Injectable } from '@angular/core';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { throwError, Observable } from 'rxjs';
import { PurchaseRequisitionPartModel, PurchaseRequisitionPCName, PurchaseRequisitionPartDesc } from './Model/PurchaseModel';
import { tap, catchError } from 'rxjs/operators';

const httpOptions : any    = {
  headers: new HttpHeaders({
    //'Content-Type':  'application/json',
    'Access-Control-Allow-Headers': 'Content-Type',
    'Access-Control-Allow-Methods': 'GET',
    'Access-Control-Allow-Origin': '*'
  })
};
@Injectable({
  providedIn: 'root'
})
export class PurchaseReqService {

  headers = new HttpHeaders().set('Content-Type', 'application/json').set('Accept', 'application/json');
  httpOptions = {
    headers: this.headers
  };
  constructor(private http: HttpClient) {}
 baseUrl: string = 'https://localhost:44386/PurchaseRequisition/';
  private handleError(error: any) {
    console.log(error);
    return throwError(error);
  }

  GetPurchaseReqPCName(companycode: string): Observable<PurchaseRequisitionPCName[]> {
    //baseUrl: string ='http://192.168.100.235/ERP_API/PurchaseRequisition/GetPurchaseReqPCName/companyCode?compCode=';
    return this.http.get<PurchaseRequisitionPCName[]>(this.baseUrl+'GetPurchaseReqPCName/companyCode?compCode='+companycode).pipe(
      tap(data => console.log(data)),
      catchError(this.handleError)
    );
  }

  GetPartDescReqByClassCode(classCode: string): Observable<any> {   
    //baseUrl: string ='http://192.168.100.235/ERP_API/PurchaseRequisition/GetPurchaseReqPCName/companyCode?compCode=';
    return this.http.get<any>(this.baseUrl+'GetPartDescReqByClassCode/ClassCode?ClassCode='+classCode).pipe(
      tap(data => console.log(data)),
      catchError(this.handleError)
    );
  }

  GetPurchaseReqSuppName(PartCode: string): Observable<PurchaseRequisitionPartModel> {
    //baseUrl: string ='http://192.168.100.235/ERP_API/PurchaseRequisition/GetPurchaseReqPCName/companyCode?compCode=';
    return this.http.get<PurchaseRequisitionPartModel>(this.baseUrl+'GetPurchaseReqSuppName/PartCode?PartCode='+PartCode).pipe(
      tap(data => console.log(data)),
      catchError(this.handleError)
    );
  }
}
