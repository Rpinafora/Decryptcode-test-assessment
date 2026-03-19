import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { BaseService } from './base.service';
import { describe, it, expect, beforeEach, afterEach } from 'vitest';

// Mock implementation of BaseService for testing
class MockBaseService extends BaseService {
  override readonly BASE = 'https://localhost:7193/api';
}

describe('BaseService', () => {
  let service: MockBaseService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [MockBaseService, provideHttpClient(), provideHttpClientTesting()]
    });
    service = TestBed.inject(MockBaseService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

