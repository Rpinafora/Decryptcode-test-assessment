import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { OrganizationService } from './organization.service';
import { Organization, OrganizationSummary } from '../../models';
import { describe, it, expect, beforeEach, afterEach } from 'vitest';

describe('OrganizationService', () => {
  let service: OrganizationService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [OrganizationService, provideHttpClient(), provideHttpClientTesting()]
    });
    service = TestBed.inject(OrganizationService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch all organizations', () => {
    const mockOrgs: Organization[] = [
      {
        id: '1',
        name: 'Org 1',
        industry: 'Tech',
        tier: 'enterprise',
        contactEmail: 'contact@org1.com'
      }
    ];

    const mockResponse = { content: mockOrgs, error: null, statusCode: 200 };

    service.getOrganizations().subscribe(result => {
      expect(result).toEqual(mockOrgs);
      expect(result.length).toBe(1);
    });

    const req = httpMock.expectOne('https://localhost:7193/api/organizations');
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should fetch single organization', () => {
    const mockOrg: Organization = {
      id: '1',
      name: 'Org 1',
      industry: 'Tech',
      tier: 'enterprise',
      contactEmail: 'contact@org1.com'
    };

    const mockResponse = { content: mockOrg, error: null, statusCode: 200 };

    service.getOrganization('1').subscribe(result => {
      expect(result).toEqual(mockOrg);
    });

    const req = httpMock.expectOne('https://localhost:7193/api/organizations/1');
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should fetch organization summary', () => {
    const mockSummary: OrganizationSummary = {
      organization: {
        id: '1',
        name: 'Org 1',
        industry: 'Tech',
        tier: 'enterprise',
        contactEmail: 'contact@org1.com'
      },
      projectCount: 5,
      userCount: 10,
      totalInvoiced: 100000,
      currency: 'USD'
    };

    const mockResponse = { content: mockSummary, error: null, statusCode: 200 };

    service.getOrganizationSummary('1').subscribe(result => {
      expect(result).toEqual(mockSummary);
      expect(result.projectCount).toBe(5);
    });

    const req = httpMock.expectOne('https://localhost:7193/api/organizations/1/summary');
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });
});
