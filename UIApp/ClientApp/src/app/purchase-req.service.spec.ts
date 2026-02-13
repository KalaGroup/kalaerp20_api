import { TestBed } from '@angular/core/testing';

import { PurchaseReqService } from './purchase-req.service';

describe('PurchaseReqService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: PurchaseReqService = TestBed.get(PurchaseReqService);
    expect(service).toBeTruthy();
  });
});
