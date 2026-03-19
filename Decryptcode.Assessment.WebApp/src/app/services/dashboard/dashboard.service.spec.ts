import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { DashboardService } from './dashboard.service';
import { DashboardData } from '../../models';
import { describe, it, expect, beforeEach, afterEach } from 'vitest';

describe('DashboardService', () => {
  let service: DashboardService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [DashboardService, provideHttpClient(), provideHttpClientTesting()]
    });
    service = TestBed.inject(DashboardService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch dashboard data', () => {
    const mockData: DashboardData = {
      totalOrganizations: 5,
      totalUsers: 20,
      totalProjects: 10,
      activeProjects: 3,
      totalTimeEntries: 100,
      totalInvoiced: 50000
    };

    const mockResponse = { content: mockData, error: null, statusCode: 200 };

    service.getDashboard().subscribe(result => {
      expect(result).toEqual(mockData);
    });

    const req = httpMock.expectOne('https://localhost:7193/api/dashboard');
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should handle error when fetching dashboard', () => {
    const mockResponse = { content: null, error: 'Server error', statusCode: 500 };

    service.getDashboard().subscribe({
      next: () => expect.fail('should have failed'),
      error: (err) => {
        expect(err.message).toContain('Server error');
      }
    });

    const req = httpMock.expectOne('https://localhost:7193/api/dashboard');
    req.flush(mockResponse);
  });
});
