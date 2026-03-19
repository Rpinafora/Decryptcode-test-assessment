import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { TimeEntryService } from './time-entry.service';
import { TimeEntry } from '../../models';
import { describe, it, expect, beforeEach, afterEach } from 'vitest';

describe('TimeEntryService', () => {
  let service: TimeEntryService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [TimeEntryService, provideHttpClient(), provideHttpClientTesting()]
    });
    service = TestBed.inject(TimeEntryService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch all time entries', () => {
    const mockEntries: TimeEntry[] = [
      {
        id: '1',
        hours: 8,
        date: '2024-01-01'
      }
    ];

    const mockResponse = { content: mockEntries, error: null, statusCode: 200 };

    service.getTimeEntries().subscribe(result => {
      expect(result).toEqual(mockEntries);
      expect(result[0].hours).toBe(8);
    });

    const req = httpMock.expectOne('https://localhost:7193/api/time-entries');
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });
});
