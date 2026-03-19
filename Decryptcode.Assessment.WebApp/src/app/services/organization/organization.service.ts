import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BaseService } from '../base/base.service';
import { Organization, OrganizationSummary } from '../../models';

@Injectable({
  providedIn: 'root'
})
export class OrganizationService extends BaseService {
  constructor(http: HttpClient) {
    super(http);
  }

  getOrganizations(params?: Record<string, any>): Observable<Organization[]> {
    return this.get<Organization[]>('/organizations', params);
  }

  getOrganization(id: string): Observable<Organization> {
    return this.get<Organization>(`/organizations/${id}`);
  }

  getOrganizationSummary(id: string): Observable<OrganizationSummary> {
    return this.get<OrganizationSummary>(`/organizations/${id}/summary`);
  }
}
