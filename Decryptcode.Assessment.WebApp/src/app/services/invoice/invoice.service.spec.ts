import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { InvoiceService } from './invoice.service';
import { Invoice } from '../../models';
import { describe, it, expect, beforeEach, afterEach } from 'vitest';

describe('InvoiceService', () => {
  let service: InvoiceService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [InvoiceService, provideHttpClient(), provideHttpClientTesting()]
    });
    service = TestBed.inject(InvoiceService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch all invoices', () => {
    const mockInvoices: Invoice[] = [
      {
        id: '1',
        amount: 5000,
        date: '2024-01-01'
      }
    ];

    const mockResponse = { content: mockInvoices, error: null, statusCode: 200 };

    service.getInvoices().subscribe(result => {
      expect(result).toEqual(mockInvoices);
      expect(result[0].amount).toBe(5000);
    });

    const req = httpMock.expectOne('https://localhost:7193/api/invoices');
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });
});
