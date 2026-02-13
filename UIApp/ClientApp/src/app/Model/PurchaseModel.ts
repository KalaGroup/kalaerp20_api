export class PurchaseRequisitionPartModel{
    purchase_req_uom: string;
    part_description:string;
    part_code:string;
    supplier: SupplierDetails[];
}

export class SupplierDetails{
    supplier_name: string;
    supplier_uom: string;
    rate:number;
    currency:string;
    validity:string;
    quantity:number;
    converted_quantity: number;
    converted_uom:string;
}
export class PurchaseRequisitionPCName{

    PCName:string;
    PCCode:string;
}
export class PurchaseRequisitionPartDesc{

    PartDesc:string;
    PartCode:string;
}
