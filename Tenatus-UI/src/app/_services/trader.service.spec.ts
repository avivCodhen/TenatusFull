/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { TraderService } from './trader.service';

describe('Service: Trader', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [TraderService]
    });
  });

  it('should ...', inject([TraderService], (service: TraderService) => {
    expect(service).toBeTruthy();
  }));
});
