import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BaseService } from '../base/base.service';
import { Invoice } from '../../models';

@Injectable({
  providedIn: 'root'
})
export class InvoiceService extends BaseService {
  constructor(http: HttpClient) {
    super(http);
  }

  getInvoices(params?: Record<string, any>): Observable<Invoice[]> {
    return this.get<Invoice[]>('/invoices', params);
  }
}
