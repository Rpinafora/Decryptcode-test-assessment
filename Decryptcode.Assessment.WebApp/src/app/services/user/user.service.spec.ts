import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { UserService } from './user.service';
import { User } from '../../models';
import { describe, it, expect, beforeEach, afterEach } from 'vitest';

describe('UserService', () => {
  let service: UserService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [UserService, provideHttpClient(), provideHttpClientTesting()]
    });
    service = TestBed.inject(UserService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch all users', () => {
    const mockUsers: User[] = [
      {
        id: '1',
        name: 'User 1',
        email: 'user1@example.com'
      }
    ];

    const mockResponse = { content: mockUsers, error: null, statusCode: 200 };

    service.getUsers().subscribe(result => {
      expect(result).toEqual(mockUsers);
      expect(result.length).toBe(1);
    });

    const req = httpMock.expectOne('https://localhost:7193/api/users');
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });
});
